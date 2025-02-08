using Amazon.CognitoIdentityProvider;
using Core.Domain.Notificacoes;
using Core.WebApi.Configurations;
using Core.WebApi.GlobalErrorMiddleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Core.WebApi.DependencyInjection
{
    [ExcludeFromCodeCoverage]
    public static class ServiceCollectionExtensions
    {
        public static void AddApiDefautConfig(this IServiceCollection services, JwtBearerConfigureOptions jwtBearerConfigureOptions)
        {
            services.AddScoped<INotificador, Notificador>();

            services.AddControllers().AddJsonOptions(delegate (JsonOptions options)
            {
                options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;

                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;

            });

            services.AddSwaggerConfig();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(opt =>
                {
                    opt.Authority = jwtBearerConfigureOptions.Authority;
                    opt.MetadataAddress = jwtBearerConfigureOptions.MetadataAddress;
                    opt.IncludeErrorDetails = true;
                    opt.RequireHttpsMetadata = false;
                    opt.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = false,
                        ValidateIssuerSigningKey = true,
                        RoleClaimType = "cognito:groups"
                    };
                });

            services.AddAuthorizationBuilder()
                // Política para usuários no grupo "admin"
                .AddPolicy("AdminRole", policy => policy.RequireRole("admin"))
                // Política para usuários no grupo "cliente"
                .AddPolicy("ClienteRole", policy => policy.RequireRole("cliente"))
                // Política para usuários que pertencem a "admin" ou "cliente"
                .AddPolicy("AdminOrClienteRole", policy => policy.RequireRole("admin", "cliente"));

            services.AddAWSService<IAmazonCognitoIdentityProvider>();
        }

        public static void AddWorkerDefautConfig(this IServiceCollection services)
        {
            services.AddScoped<INotificador, Notificador>();

            services.AddControllers().AddJsonOptions(delegate (JsonOptions options)
            {
                options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;

                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;

            });
        }

        public static void UseApiDefautConfig(this IApplicationBuilder app)
        {
            app.UseApplicationErrorMiddleware();

            app.UseSwaggerConfig();

            app.UseHsts();
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/health");
            });
        }

        public static void UseWorkerDefautConfig(this IApplicationBuilder app)
        {
            app.UseApplicationErrorMiddleware();

            app.UseHsts();
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/health");
            });
        }
    }
}