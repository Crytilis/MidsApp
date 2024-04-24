using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MidsApp.Database;
using MidsApp.Services;
using MidsApp.Settings;
using MongoDB.Driver;

namespace MidsApp
{
    /// <summary>
    /// MidsReborn API (MidsApp)
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// Main Entry
        /// </summary>
        /// <param name="args"></param>
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                });
            
            // Add Swagger to aid development
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v2", new OpenApiInfo
                {
                    Version = "v2",
                    Title = "MidsReborn API",
                    Description = "This API is designed for MidsReborn.",
                    TermsOfService = new Uri("https://midsreborn.com/terms.html")
                });
                options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "MidsApp.xml"));
            });

            // Configure services for dependency injection
            var discordSettings = builder.Configuration.GetSection(nameof(DiscordSettings)).Get<DiscordSettings>();
            var jwtSettings = builder.Configuration.GetSection(nameof(JwtSettings)).Get<JwtSettings>();
            builder.Services.Configure<DatabaseSettings>(builder.Configuration.GetSection(nameof(DatabaseSettings)));
            builder.Services.Configure<DiscordSettings>(builder.Configuration.GetSection(nameof(DiscordSettings)));

            // Add dependency injection services
            builder.Services.AddSingleton(sp =>
            {
                var databaseSettings = sp.GetRequiredService<IOptions<DatabaseSettings>>().Value;
                var databaseClient = new MongoClient(databaseSettings.ConnectionString);
                var database = databaseClient.GetDatabase(databaseSettings.DatabaseName);
                return database;
            });

            builder.Services.AddSingleton<TtlInitializer>();
            builder.Services.AddSingleton<IUrlBuilder, UrlBuilder>();
            builder.Services.AddSingleton<IBuildRepository, BuildRepository>();

            // Add authentication methods used to secure endpoints when needed (to be used later)
            builder.Services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                })
                .AddCookie(options =>
                {
                    options.SlidingExpiration = true;
                    options.ExpireTimeSpan = TimeSpan.FromDays(7);
                })
                .AddJwtBearer(bearer =>
                {
                    bearer.RequireHttpsMetadata = false;
                    bearer.SaveToken = true;
                    if (jwtSettings?.AccessKey != null)
                        bearer.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuerSigningKey = true,
                            ValidateIssuer = true,
                            ValidateAudience = true,
                            ValidateLifetime = true,
                            ClockSkew = TimeSpan.Zero,

                            ValidIssuer = jwtSettings.Issuer,
                            ValidAudience = jwtSettings.Audience,
                            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.AccessKey))
                        };
                    bearer.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = context =>
                        {
                            if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                            {
                                context.Response.Headers.Append("Token-Expired", "true");
                            }

                            return Task.CompletedTask;
                        }
                    };
                })
                .AddOAuth("Discord", options =>
                {
                    options.AuthorizationEndpoint = "https://discord.com/oauth2/authorize";
                    options.Scope.Add("identify");
                    options.Scope.Add("email");
                    options.Scope.Add("guilds");

                    options.CallbackPath = new PathString("/auth/callback");
                    options.ClientId = discordSettings!.ClientId!;
                    options.ClientSecret = discordSettings.ClientSecret!;

                    options.TokenEndpoint = "https://discord.com/api/oauth2/token";
                    options.UserInformationEndpoint = "https://discord.com/api/users/@me";
                    options.SaveTokens = true;

                    options.ClaimActions.MapJsonKey("Username", "username");
                    options.ClaimActions.MapJsonKey("Avatar", "avatar");
                    options.ClaimActions.MapJsonKey("Bot", "bot", ClaimValueTypes.Boolean);
                    options.ClaimActions.MapJsonKey("Mfa", "mfa_enabled", ClaimValueTypes.Boolean);
                    options.ClaimActions.MapJsonKey("Verified", "verified", ClaimValueTypes.Boolean);
                    options.ClaimActions.MapJsonKey("Email", "email");

                    options.AccessDeniedPath = new PathString("/auth/failed");

                    options.Events = new OAuthEvents
                    {
                        OnCreatingTicket = async context =>
                        {
                            var request =
                                new HttpRequestMessage(HttpMethod.Get, context.Options.UserInformationEndpoint);
                            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                            request.Headers.Authorization =
                                new AuthenticationHeaderValue("Bearer", context.AccessToken);

                            var response = await context.Backchannel.SendAsync(request,
                                HttpCompletionOption.ResponseHeadersRead, context.HttpContext.RequestAborted);
                            response.EnsureSuccessStatusCode();

                            var user = JsonDocument.Parse(await response.Content.ReadAsStringAsync()).RootElement;

                            context.RunClaimActions(user);
                        }
                    };
                });
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v2/swagger.json", "API V2");
                });
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            await app.RunAsync();
        }
    }
}