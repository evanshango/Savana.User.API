using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Savana.User.API.Entities;

namespace Savana.User.API.Data;

public class StoreContext : IdentityDbContext<UserEntity> {
    public DbSet<GroupEntity> Groups { get; set; } = null!;
    public new DbSet<RoleEntity> Roles { get; set; } = null!;

    public StoreContext(DbContextOptions<StoreContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder) {
        base.OnModelCreating(builder);

        builder.Ignore<IdentityUserToken<string>>();
        builder.Ignore<IdentityUserClaim<string>>();
        builder.Ignore<IdentityUserLogin<string>>();
        builder.Ignore<IdentityRoleClaim<string>>();
        builder.Ignore<IdentityUserRole<string>>();
        builder.Ignore<IdentityRole>();

        builder.Entity<UserEntity>().ToTable("users");
        builder.Entity<RoleEntity>().ToTable("roles").HasData(
            new RoleEntity(Guid.NewGuid().ToString(), "Admin", "Administrator Role"),
            new RoleEntity(Guid.NewGuid().ToString(), "User", "Default User Role")
        );
        builder.Entity<GroupEntity>().ToTable("groups").HasData(
            new GroupEntity(Guid.NewGuid().ToString(), "Administrator", "Administrator Group", ""),
            new GroupEntity(Guid.NewGuid().ToString(), "User", "Default User Group", "")
        );
        builder.Entity<UserGroup>().ToTable("user_groups").HasKey(ug => new { ug.UserId, ug.GroupId });
        builder.Entity<GroupRole>().ToTable("group_roles").HasKey(gr => new { gr.GroupId, gr.RoleId });
    }
}