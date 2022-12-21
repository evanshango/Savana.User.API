using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Savana.User.API.Data;
using Savana.User.API.Entities;
using Savana.User.API.Interfaces;
using Savana.User.API.Services;
using Treasures.Common.Extensions;
using Treasures.Common.Interfaces;
using Treasures.Common.Middlewares;
using Treasures.Common.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddJsonOptions(opt => {
    opt.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
}).ConfigureApiBehaviorOptions(x => { x.SuppressMapClientErrors = true; });
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddDbContext<StoreContext>(opt => {
    opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")!);
});
builder.Services
    .AddIdentity<UserEntity, RoleEntity>(opt => { opt.User.RequireUniqueEmail = true; })
    .AddEntityFrameworkStores<StoreContext>().AddDefaultTokenProviders();
builder.Services.AddAuthorization();

//Inject services from Treasures.Common library
builder.Services.AddScoped(typeof(ISqlRepository<>), typeof(SqlRepository<>));
builder.Services.AddScoped<IUnitOfWork>(s => new UnitOfWork(s.GetService<StoreContext>()!));
builder.Services.AddJwtAuthentication(builder.Configuration["Token:Key"], builder.Configuration["Token:Issuer"]);
builder.Services.AddSwaggerAuthenticated("Savana User API Service", "v1");
builder.Services.AddErrorResponse<ApiBehaviorOptions>();

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IGroupService, GroupService>();
builder.Services.AddScoped<ITokenService>(_ => {
    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Token:Key"]));
    return new TokenService(key, builder.Configuration["Token:Issuer"], builder.Configuration["Token:ExpiresIn"]);
});

builder.Services.AddRouting(opt => {
    opt.LowercaseUrls = true;
    opt.LowercaseQueryStrings = true;
});

var app = builder.Build();

await using (var scope = app.Services.CreateAsyncScope()) {
    var services = scope.ServiceProvider;
    var loggerFactory = services.GetRequiredService<ILoggerFactory>();
    try {
        var context = services.GetRequiredService<StoreContext>();
        var userManager = services.GetRequiredService<UserManager<UserEntity>>();
        await context.Database.MigrateAsync();
        await SeedDb.SeedData(context, userManager, builder.Configuration["SeedInfo:AdminPass"]);
    }
    catch (Exception e) {
        var logger = loggerFactory.CreateLogger<Program>();
        logger.LogError(e, "An error occurred during migration");
    }
}

app.UseForwardedHeaders(new ForwardedHeadersOptions
    { ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto }
);
app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment()) app.UseSavanaSwaggerDoc("Savana User API Service v1");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();