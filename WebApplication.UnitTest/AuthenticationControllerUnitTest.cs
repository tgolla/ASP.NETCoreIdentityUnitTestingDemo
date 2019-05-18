using WebApplication;
using WebApplication.Controllers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using System.IO;
using System.Threading.Tasks;
using static WebApplication.Controllers.AuthenticationController;
using WebApplication.Identity;

namespace WebApplication.UnitTest
{
    [TestFixture]
    class AuthenticationControllerUnitTest
    {
        private SqliteConnection sqliteConnection;
        private ApplicationIdentityDbContext identityDbContext;
        private IOptions<TokenValidation> tokenValidation;
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

            tokenValidation = serviceCollection.BuildServiceProvider().GetService<IOptions<TokenValidation>>();
        }
        
        [TearDown]
        public void TearDown()
        {
            identityDbContext.Database.EnsureDeleted();
            identityDbContext.Dispose();
            sqliteConnection.Close();
        }

        [Test]
        public async Task AuthenticateAsync_WithValidUserAndPassword_ReturnsToken()
        {
            // Seed database with user.
            string userName = "Test";
            string password = "Abc!23";
            ApplicationUser user = new ApplicationUser() { UserName = userName, Email = "mail@domain.com" };
            await userManager.CreateAsync(user, password);

            // Run test.
            var authenticationController = new AuthenticationController(tokenValidation, userManager);
            LoginInfo loginInfo = new LoginInfo() { UserName = userName, Password = password };
            IActionResult result = await authenticationController.AuthenticateAsync(loginInfo);

            // Confirm user was returned with a token.
            var authenticationInfo = (ReturnAuthenticationInfo)((ObjectResult)result).Value;
            Assert.That(authenticationInfo.User.UserName, Is.EqualTo(userName));
            Assert.That(authenticationInfo.Token, Is.Not.Null);
        }


        [Test]
        public async Task AuthenticateAsync_WithInvalidUserAndPassword_DoesNotReturnToken()
        {
            // Seed database with user.
            string userName = "Test";
            string password = "Abc!23";
            ApplicationUser user = new ApplicationUser() { UserName = userName, Email = "mail@domain.com" };
            await userManager.CreateAsync(user, password);

            // Run test.
            var authenticationController = new AuthenticationController(tokenValidation, userManager);
            LoginInfo loginInfo = new LoginInfo() { UserName = userName, Password = "" };
            IActionResult result = await authenticationController.AuthenticateAsync(loginInfo);

            // Confirm HTTP status 401 Unauthorized.
            var statusCode = ((StatusCodeResult)result).StatusCode;
            Assert.That(statusCode, Is.EqualTo(401));
        }

        [Test]
        public void IsAuthenticated_ReturnOk()
        {
            // Run test.
            var authenticationController = new AuthenticationController(tokenValidation, userManager);
            IActionResult result = authenticationController.IsAuthenticated();

            // Confirm service retruns OK.
            Assert.That(result, Is.InstanceOf(typeof(OkResult)));
        }

        [Test]
        public void IsAdministrator_ReturnOk()
        {
            // Run test.
            var authenticationController = new AuthenticationController(tokenValidation, userManager);
            IActionResult result = authenticationController.IsAdministrator();

            // Confirm service retruns OK.
            Assert.That(result, Is.InstanceOf(typeof(OkResult)));
        }
    }
}
