using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Swagger;
using WebApplication.Identity;

namespace WebApplication
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Configure ASP.NET Core Identity DB Context.
            services.AddDbContext<ApplicationIdentityDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("IdentityDbDemo")));

            // Configure ASP.NET Core Identity
            services.AddApplicationIdentity();

            // Configure token validation.
            var tokenValidationSection = Configuration.GetSection("TokenValidation");
            services.Configure<TokenValidation>(tokenValidationSection);
            var tokenValidation = tokenValidationSection.Get<TokenValidation>();

            services.AddAuthentication(options => {
                options.DefaultAuthenticateScheme = "JwtBearer";
                options.DefaultChallengeScheme = "JwtBearer";
            })
            .AddJwtBearer("JwtBearer", jwtBearerOptions =>
            {
                jwtBearerOptions.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidAudience = tokenValidation.Audience,
                    ClockSkew = TimeSpan.FromMinutes(tokenValidation.ClockSkew),
                    ValidIssuer = tokenValidation.Issuer,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenValidation.IssuerSigningKey)),
                    ValidateAudience = tokenValidation.ValidateAudience,
                    ValidateIssuer = tokenValidation.ValidateIssuer,
                    ValidateIssuerSigningKey = tokenValidation.ValidateIssuerSigningKey,
                    ValidateLifetime = tokenValidation.ValidateLifetime
                };
            });

            services.AddSwaggerDocumentation("v1.0", new Info { Title = "ASP.NET Identity Unit Test Demo API", Version = "v1.0" },
                "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"  A token can be acquired using /api/authentication/authenticate.");

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseAuthentication();

            app.UseSwaggerDocumentation("/swagger/v1.0/swagger.json", "ASP.NET Identity Unit Test Demo API", "ASP.NET Identity Unit Test Demo API");

            app.UseMvc();
        }
    }
}
