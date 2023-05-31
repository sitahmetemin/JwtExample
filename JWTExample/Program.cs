using JWTExample.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace JWTExample
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            AddOptions(builder);
            ConfigureAuth(builder);

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }

        private static void AddOptions(WebApplicationBuilder builder)
        {
            builder.Services.AddOptions<JwtOption>().BindConfiguration("Auth");
        }

        private static void ConfigureAuth(WebApplicationBuilder builder)
        {
            var settings = builder.Configuration.GetSection("Auth").Get<JwtOption>();
            ArgumentNullException.ThrowIfNull(settings);

            builder.Services.AddAuthentication(k =>
            {
                k.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                k.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(p =>
            {
                p.IncludeErrorDetails = true;
                p.SaveToken = true;
                p.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = settings.Issuer,
                    ValidateAudience = true,
                    ValidAudience = settings.Audience,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.Key)),
                    ValidateLifetime = true
                };
            });
        }
    }
}