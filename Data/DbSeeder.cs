using Microsoft.AspNetCore.Identity;
using System.Data;

namespace AgriStream_ST10157545_Practium.Data
{
    public class DbSeeder
    {
        public static async Task SeedAsync(IServiceProvider services, IConfiguration configuration)
        {
            using var scope = services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher<User>>();
            await db.Database.EnsureCreatedAsync();
            if (!await db.Roles.AnyAsync())
            {
                db.Roles.AddRange(
                    new Role { RoleName = AppRoles.Farmer },
                    new Role { RoleName = AppRoles.Agronomist },
                    new Role { RoleName = AppRoles.Admin });
                await db.SaveChangesAsync();
            }
            if (!await db.Devices.AnyAsync())
            {
                db.Devices.AddRange(
                    new Device { DeviceId = "sensor-001", FieldId = "field-A" },
                    new Device { DeviceId = "sensor-002", FieldId = "field-A" },
                    new Device { DeviceId = "valve-001", FieldId = "field-B" });
                await db.SaveChangesAsync();
            }
            var adminName = configuration["Seed:AdminUsername"];
            var adminPassword = configuration["Seed:AdminPassword"];
            if (string.IsNullOrWhiteSpace(adminName) || string.IsNullOrWhiteSpace(adminPassword))
            {
                return;
            }
            if (!await db.Users.AnyAsync(u => u.Username == adminName))
            {
                var adminRole = await db.Roles.SingleAsync(r => r.RoleName == AppRoles.Admin);
                var admin = new User { Username = adminName, RoleId = adminRole.RoleId };
                admin.PasswordHash = hasher.HashPassword(admin, adminPassword);
                db.Users.Add(admin);
                await db.SaveChangesAsync();
            }
        }
    }
}
