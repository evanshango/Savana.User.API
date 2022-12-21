using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Savana.User.API.Entities;

namespace Savana.User.API.Data;

public static class SeedDb {
    public static async Task SeedData(StoreContext ctx, UserManager<UserEntity> uMgr, string adminPass) {
        if (!ctx.Users.Any(x => x.Email.Equals("admin@admin.com"))) {
            var gAd = await ctx.Groups.FirstOrDefaultAsync(g =>
                g.Name.ToLower().Equals("administrator")
            );

            var gUs = await ctx.Groups.FirstOrDefaultAsync(g => g.Name.ToLower().Equals("user"));

            var roleAd = await ctx.Roles.FirstOrDefaultAsync(r => r.Name.ToLower().Equals("admin"));

            var roleUser = await ctx.Roles.FirstOrDefaultAsync(r => r.Name.ToLower().Equals("user"));

            if (gAd != null && roleAd != null) {
                var gr = new GroupRole { Group = gAd, GroupId = gAd.Id, Role = roleAd, RoleId = roleAd.Id };
                gAd.RoleGroups = new List<GroupRole> { gr };
                ctx.Groups.Update(gAd);
            }

            if (gUs != null && roleUser != null) {
                var gr = new GroupRole { Group = gUs, GroupId = gUs.Id, Role = roleUser, RoleId = roleUser.Id };
                gUs.RoleGroups = new List<GroupRole> { gr };
                ctx.Groups.Update(gUs);
            }

            await ctx.SaveChangesAsync();

            var user = new UserEntity {
                UserName = "admin@admin.com", Email = "admin@admin.com", EmailConfirmed = true, FirstName = "Admin",
                LastName = "Admin", Gender = "Male", PhoneNumber = "07123456789", PhoneNumberConfirmed = true,
                CreatedAt = DateTime.UtcNow, Enabled = true
            };
            var result = await uMgr.CreateAsync(user, adminPass);
            if (result.Succeeded) {
                var userGroup = new UserGroup { Group = gAd, GroupId = gAd?.Id, User = user, UserId = user.Id };

                user.UserGroups = new List<UserGroup> { userGroup };
                await uMgr.UpdateAsync(user);
                Console.WriteLine("Data successfully seeded...");
            }
        }
    }
}