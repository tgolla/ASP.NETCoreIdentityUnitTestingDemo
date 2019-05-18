using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WebApplication.Identity;

namespace WebApplication.Identity.Initialize
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            // Build service colection to create identity UserManager and RoleManager.           
            IServiceCollection serviceCollection = new ServiceCollection();

            // Determine Environment (not a console application thing).
            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            // Environment can also be passes as the first argument.
            if (args != null && args.Count() == 1)
                environmentName = args[0];
                
            // And if not found it defaults to Development.
            if (string.IsNullOrWhiteSpace(environmentName))
                environmentName = "Development";

            // Get appsettings.
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile($"appsettings.json", true, true)
                .AddJsonFile($"appsettings.{environmentName}.json", true, true)
                .AddEnvironmentVariables();

            IConfigurationRoot configuration = builder.Build();

            var appSettingsSection = configuration.GetSection("AppSettings");
            serviceCollection.Configure<AppSettings>(appSettingsSection);

            IOptions<AppSettings> appSettings = serviceCollection.BuildServiceProvider().GetService<IOptions<AppSettings>>();

            // Add ASP.NET Core Identity database.
            serviceCollection.AddDbContext<ApplicationIdentityDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("IdentityDbDemo")));

            ApplicationIdentityDbContext identityDbContext = serviceCollection.BuildServiceProvider().GetService<ApplicationIdentityDbContext>();
            identityDbContext.Database.EnsureCreated();

            // Add Identity using in memory database to create UserManager and RoleManager.
            serviceCollection.AddApplicationIdentity();

            // Get UserManager and RoleManager.
            UserManager<ApplicationUser> userManager = serviceCollection.BuildServiceProvider().GetService<UserManager<ApplicationUser>>();
            RoleManager<ApplicationRole> roleManager = serviceCollection.BuildServiceProvider().GetService<RoleManager<ApplicationRole>>();

            Console.WriteLine("Adding Roles...");
            Console.WriteLine();

            // Create maintenance scheduler roles.
            foreach (string role in appSettings.Value.Roles)
            {
                ApplicationRole roleExist = await roleManager.FindByNameAsync(role);

                if (roleExist != null)
                    Console.WriteLine("Role '{0}' already exist.", role);
                else
                {
                    IdentityResult result = await roleManager.CreateAsync(new ApplicationRole(role));

                    if (result.Succeeded)
                        Console.WriteLine("Role '{0}' has been created.", role);
                    else
                        Console.WriteLine("Error creating role '{0}'.  {1}", role, result.ErrorsToString());
                }
            }

            Console.WriteLine();
            Console.WriteLine("Adding User...");

            // Create default user.
            ApplicationUser user = await userManager.FindByNameAsync(appSettings.Value.User.UserName);

            if (user == null)
            {
                IdentityResult result;

                user = new ApplicationUser(appSettings.Value.User.UserName, appSettings.Value.User.Email);

                result = await userManager.CreateAsync(user, appSettings.Value.UserPassword);

                if (result.Succeeded)
                {
                    Console.WriteLine("User '{0}' has been created.", appSettings.Value.User.UserName);

                    // Add user to roles.
                    var createdUser = await userManager.FindByNameAsync(user.UserName);
                    result = await userManager.AddToRolesAsync(createdUser, appSettings.Value.UserRoles);

                    if (result.Succeeded)
                        Console.WriteLine("User '{0}' has been added to role(s) '{1}'.", 
                            appSettings.Value.User.UserName, appSettings.Value.UserRoles.Aggregate((x, y) => x + ", " + y));
                    else
                        Console.WriteLine("Error adding user '{0}' to role(s) '{1}'.  {2}",
                            appSettings.Value.User.UserName, appSettings.Value.UserRoles, result.ErrorsToString());
                }
                else
                {
                    Console.WriteLine("Error creating user {0}.  {1}", appSettings.Value.User.UserName, result.ErrorsToString());
                }
            }
            else
                Console.WriteLine("User '{0}' already exist.", appSettings.Value.User.UserName);
        }
    }
}
