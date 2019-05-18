using WebApplication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using System;
using System.IO;
using System.Threading.Tasks;

namespace WebApplication.Identity.UnitTest
{
    [TestFixture]
    public class UserManagerExtensionsUnitTest
    {
        private SqliteConnection sqliteConnection;
        private ApplicationIdentityDbContext identityDbContext;
        private TokenValidation tokenValidationSettings;
        private UserManager<ApplicationUser> userManager;
        private RoleManager<ApplicationRole> roleManager;

        [SetUp]
        public void Setup()
        {
            // Build service colection to create identity UserManager and RoleManager.           
            IServiceCollection serviceCollection = new ServiceCollection();

            // Add ASP.NET Core Identity database in memory.
            sqliteConnection = new SqliteConnection("DataSource=:memory:");
            serviceCollection.AddDbContext<ApplicationIdentityDbContext>(options => options.UseSqlite(sqliteConnection));

            identityDbContext = serviceCollection.BuildServiceProvider().GetService<ApplicationIdentityDbContext>();
            identityDbContext.Database.OpenConnection();
            identityDbContext.Database.EnsureCreated();

            // Add Identity using in memory database to create UserManager and RoleManager.
            serviceCollection.AddApplicationIdentity();

            // Get UserManager and RoleManager.
            userManager = serviceCollection.BuildServiceProvider().GetService<UserManager<ApplicationUser>>();
            roleManager = serviceCollection.BuildServiceProvider().GetService<RoleManager<ApplicationRole>>();

            // Get token validation settings.
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            IConfigurationRoot configuration = builder.Build();

            var tokenValidationSection = configuration.GetSection("TokenValidation");
            serviceCollection.Configure<TokenValidation>(tokenValidationSection);

            tokenValidationSettings = serviceCollection.BuildServiceProvider().GetService<IOptions<TokenValidation>>().Value;
        }

        [TearDown]
        public void TearDown()
        {
            identityDbContext.Database.EnsureDeleted();
            identityDbContext.Dispose();
            sqliteConnection.Close();
        }

        /// <summary>
        /// Seed database with roles. 
        /// </summary>
        private async Task SeedDatabaseWithRoles()
        {
            string[] roles = { "User", "Administrator", "Manager" };

            foreach (string role in roles)
            {
                await roleManager.CreateAsync(new ApplicationRole(role));
            }
        }

        [Test]
        public async Task ChangePasswordAsync_ConfirmChange()
        {
            // Seed database with user.
            string userName = "Test";
            string password = "Abc!23";
            ApplicationUser user = new ApplicationUser() { UserName = userName, Email = "mail@domain.com" };
            await userManager.CreateAsync(user, password);
            string passwordHash = user.PasswordHash;
            
            // Run test.
            IdentityResult result = await userManager.ChangePasswordAsync(user, "!23Abc");

            Assert.That(user.PasswordHash, Is.Not.EqualTo(passwordHash));
        }

        [Test]
        public async Task GenerateJwtSecurityTokenAsync_GeneratesJwtSecurityToken()
        {
            await SeedDatabaseWithRoles();

            // Seed database with user.
            string userName = "Test";
            string password = "Abc!23";
            ApplicationUser user = new ApplicationUser() { UserName = userName, Email = "mail@domain.com" };
            await userManager.CreateAsync(user, password);
            user = await userManager.FindByNameAsync(userName);
            IdentityResult result = await userManager.AddToRolesAsync(user, new string[] { "User", "Manager" });

            // Run test.
            string token = await userManager.GenerateJwtSecurityTokenAsync(user, tokenValidationSettings.Issuer, 
                tokenValidationSettings.IssuerSigningKey, tokenValidationSettings.Audience, DateTime.Now.AddMinutes(tokenValidationSettings.Expires));

            Assert.That(token, Is.Not.Empty);
        }
    }
}
