using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerUI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace WebApplication
{
    /// <summary>
    /// Swagger service extensions.
    /// </summary>
    /// <remarks>
    /// Reference:
    /// https://docs.microsoft.com/en-us/aspnet/core/tutorials/getting-started-with-swashbuckle?view=aspnetcore-2.2&tabs=visual-studio
    /// https://ppolyzos.com/2017/10/30/add-jwt-bearer-authorization-to-swagger-and-asp-net-core/
    /// </remarks>
    public static class SwaggerServiceExtensions
    {
        /// <summary>
        /// Added Swagger support to ConfigureServices(IServiceCollection services) including Authorization.
        /// </summary>
        /// <param name="services">The IServiceCollection to add services to.</param>
        /// <param name="swaggerDocName">The Swagger document name.</param>
        /// <param name="swaggerDocInfo">The Swagger document information.</param>
        /// <param name="securitySchemeDescription">The security scheme description.</param>
        /// <returns>The original services parameter.</returns>
        public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services, string swaggerDocName, Info swaggerDocInfo, string securitySchemeDescription)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc(swaggerDocName, swaggerDocInfo);

                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);

                // Swagger 2.+ support
                var security = new Dictionary<string, IEnumerable<string>>
                {
                        {"Bearer", new string[] { }},
                };

                c.AddSecurityDefinition("Bearer", new ApiKeyScheme
                {
                    Description = securitySchemeDescription,
                    Name = "Authorization",
                    In = "header",
                    Type = "apiKey"
                });
                c.AddSecurityRequirement(security);
            });

            return services;
        }

        /// <summary>
        /// Added Swagger support to Configure(IApplicationBuilder app, IHostingEnvironment env) including Authorization.
        /// </summary>
        /// <param name="app">The IApplicationBuilder instance this method extends.</param>
        /// <param name="swaggerEndpointUrl">The Swagger end point URL.</param>
        /// <param name="swaggerEndpointName">The Swagger end point name.</param>
        /// <param name="swaggerUiOptionsDocumentTitle">The Swagger UI options document title.</param>
        /// <returns>The original app parameter.</returns>
        public static IApplicationBuilder UseSwaggerDocumentation(this IApplicationBuilder app, string swaggerEndpointUrl, string swaggerEndpointName, string swaggerUiOptionsDocumentTitle)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint(swaggerEndpointUrl, swaggerEndpointName);

                c.DocumentTitle = swaggerUiOptionsDocumentTitle;
                c.DocExpansion(DocExpansion.None);
            });

            return app;
        }
    }
}
