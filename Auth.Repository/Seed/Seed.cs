using Auth.Domain.AppGroups.Aggregate;
using Auth.Domain.AppGroups.Entity;
using Auth.Domain.AppRoles.Aggregate;
using Auth.Domain.AppUsers.Aggregate;
using Auth.Domain.AppUsers.Entity;
using Auth.Infrastracture.AuthHelper;
using Auth.Repository.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Auth.Repository.Seed
{
    public static class Seed
    {
        public static async Task AddDbContextSeedDataAsync(this IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbcontext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();

            await dbContext.Database.EnsureCreatedAsync();

            // ----------------------------------------------------
            // 1) Seed Groups: Root → Admin → MainAdmin
            // ----------------------------------------------------
            AppGroup rootGroup;
            if (!dbContext.AppGroups.Any(g => g.Title == "Root"))
            {
                rootGroup = new AppGroup
                {
                    Title = "Root",
                    Description = "System root group"
                };
                dbContext.AppGroups.Add(rootGroup);
                await dbContext.SaveChangesAsync();
            }
            else
            {
                rootGroup = dbContext.AppGroups.First(g => g.Title == "Root");
            }

            AppGroup adminGroup;
            if (!dbContext.AppGroups.Any(g => g.Title == "Admin"))
            {
                adminGroup = new AppGroup
                {
                    Title = "Admin",
                    Description = "Administrator group",
                    ParentId = rootGroup.Id
                };
                dbContext.AppGroups.Add(adminGroup);
                await dbContext.SaveChangesAsync();
            }
            else
            {
                adminGroup = dbContext.AppGroups.First(g => g.Title == "Admin");
            }

            AppGroup mainAdminGroup;
            if (!dbContext.AppGroups.Any(g => g.Title == "MainAdmin"))
            {
                mainAdminGroup = new AppGroup
                {
                    Title = "MainAdmin",
                    Description = "Main admin subgroup",
                    ParentId = adminGroup.Id
                };
                dbContext.AppGroups.Add(mainAdminGroup);
                await dbContext.SaveChangesAsync();
            }
            else
            {
                mainAdminGroup = dbContext.AppGroups.First(g => g.Title == "MainAdmin");
            }

            // ----------------------------------------------------
            // 2) Create Admin User
            // ----------------------------------------------------
            if (dbContext.Users.Any())
            {
                Console.WriteLine("Admin already exists.");
                return;
            }

            Console.WriteLine("=== First Time Setup: Create Admin ===");
            Console.Write("Email: ");
            var email = Console.ReadLine();

            Console.Write("FullName: ");
            var fullname = Console.ReadLine();

            Console.Write("Username: ");
            var username = Console.ReadLine();

            Console.Write("Password: ");
            var password = ReadPassword();

            var adminUser = new AppUser
            {
                UserName = username,
                FullName = fullname,
                Email = email,
                EmailConfirmed = true,
                IsConfirm = true,
                IsAdmin = true,
                Groups = new List<AppGroupUser>()
            };

            adminUser.Groups.Add(new AppGroupUser { UserId = adminUser.Id, GroupId = mainAdminGroup.Id });
            var result = await userManager.CreateAsync(adminUser, password);
            if (!result.Succeeded)
                throw new Exception("Cannot create admin: "
                    + string.Join(", ", result.Errors.Select(e => e.Description)));
            // ----------------------------------------------------
            // 3) Give MainAdmin group all URLs
            // ----------------------------------------------------
            var allUrls = dbContext.AppUrls.ToList();
            foreach (var url in allUrls)
            {
                if (!dbContext.AppGroupUrls.Any(x => x.GroupId == mainAdminGroup.Id &&
                                                      x.UrlId == url.Id))
                {
                    var perm = new AppGroupUrl
                    {
                        GroupId = mainAdminGroup.Id,
                        UrlId = url.Id
                    };
                    dbContext.AppGroupUrls.Add(perm);
                }
            }

            await dbContext.SaveChangesAsync();

            Console.WriteLine("Admin + Groups + Permissions setup complete!");
        }

        private static string ReadPassword()
        {
            string password = "";
            ConsoleKey key;
            do
            {
                var keyInfo = Console.ReadKey(true);
                key = keyInfo.Key;

                if (key == ConsoleKey.Backspace && password.Length > 0)
                {
                    password = password[..^1];
                    Console.Write("\b \b");
                }
                else if (!char.IsControl(keyInfo.KeyChar))
                {
                    password += keyInfo.KeyChar;
                    Console.Write('*');
                }

            } while (key != ConsoleKey.Enter);

            Console.WriteLine();
            return password;
        }
    }

}
