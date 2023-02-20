// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AuthorizationExtensions.cs" company="TETA">
// Copyright (c) TETA. Ufa, 2023.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
// <summary>
//  Tetacom authentication extensions
// </summary>

using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Teta.Packages.Auth.Configuration;
using Teta.Packages.Auth.Implementation;
using Teta.Packages.Auth.Interfaces;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Authentication;

namespace Teta.Packages.Auth
{
    /// <summary>
    /// Authentication extensions
    /// </summary>
    public static class AuthorizationExtensions
    {
        /// <summary>
        /// Add custom JWT token validation using ASP .NET Core default libraries
        /// </summary>
        /// <param name="services">Service collection</param>
        /// <param name="configuration">Configuration</param>
        /// <param name="jwtOptionConfig">Configuration action for JWT bearer authentication, if null - default configuration will be used</param>
        /// <param name="authOptionConfig">Configure authentication, if null - default configuration will be used</param>
        /// <returns>Chain service collection</returns>
        public static IServiceCollection AddCustomAuthValidation(
            [NotNull] this IServiceCollection services, 
            [NotNull] IConfiguration configuration,
            [AllowNull] Action<JwtBearerOptions> jwtOptionConfig = null,
            [AllowNull] Action<AuthenticationOptions> authOptionConfig = null)
        {
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (configuration is null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            var clientOptions = services.BindOptions<AuthClientOptions>(configuration);
            services.AddAuthentication(
                options =>
                {
                    if (authOptionConfig is not null)
                    {
                        authOptionConfig.Invoke(options);
                        return;
                    }

                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                }).AddJwtBearer(
                options =>
                {
                    if (jwtOptionConfig != null)
                    {
                        jwtOptionConfig.Invoke(options);
                        return;
                    }

                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = GetokenValidationParameters(clientOptions.Secret);
                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            var accessToken = context.Request.Query["access_token"];
                            var path = context.HttpContext.Request.Path;
                            if (!string.IsNullOrEmpty(accessToken)
                                && path.StartsWithSegments("/signalr"))
                            {
                                context.Token = accessToken;
                            }

                            return Task.CompletedTask;
                        },
                    };
                });

            return services;
        }

        /// <summary>
        /// Registrates entities for keycloak 
        /// Resource owner authentication flow
        /// </summary>
        /// <param name="sc">DI builder</param>
        /// <param name="config">Application configuration</param>
        /// <param name="jwtOptionConfig">Configuration action for JWT bearer authentication, if null - default configuration will be used</param>
        /// <param name="authOptionConfig"></param>
        /// <returns></returns>
        public static IServiceCollection 
            UseKeycloakAuthentication(
            [NotNull] this IServiceCollection sc, 
            [NotNull] IConfiguration config,
            [AllowNull] Action<JwtBearerOptions> jwtOptionConfig = null,
            [AllowNull] Action<AuthenticationOptions> authOptionConfig = null)
        {
            if (sc is null)
            {
                throw new ArgumentNullException(nameof(sc));
            }

            if (config is null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            // Add JWK from keycloak to authentication extension
            var kcOptions = sc.BindOptions<KcOptions>(config);
            sc.AddHttpClient<IKcAccessClient, KcAccessClient>((sp, c) =>
            {
                var options = sp.GetService<IOptions<KcOptions>>();
                if (options?.Value == null)
                {
                    throw new ArgumentNullException(nameof(KcOptions));
                }

                var uriBuilder = new UriBuilder("http", options.Value.Host, options.Value.Port);
                c.BaseAddress = uriBuilder.Uri;
                c.DefaultRequestHeaders.Accept.Clear();
                c.Timeout = TimeSpan.FromMilliseconds(options.Value.RequestTimeoutMsec);
            });

            sc.AddAuthentication(
                options =>
                {
                    if (authOptionConfig is not null)
                    {
                        authOptionConfig.Invoke(options);
                        return;
                    }
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                }).AddJwtBearer(
                options =>
                {
                    if (jwtOptionConfig != null)
                    {
                        jwtOptionConfig.Invoke(options);
                        return;
                    }

                    options.RequireHttpsMetadata = false;
                    var cert = new X509Certificate2(Encoding.UTF8.GetBytes(kcOptions.PemCertificate));

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
#if DEBUG
                        ValidateLifetime = false,
#else
                        ValidateLifetime = true,
#endif
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new X509SecurityKey(cert),
                        ClockSkew = TimeSpan.Zero,
                    };

                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            var accessToken = context.Request.Query["access_token"];
                            var path = context.HttpContext.Request.Path;
                            if (!string.IsNullOrEmpty(accessToken)
                                && path.StartsWithSegments("/signalr"))
                            {
                                context.Token = accessToken;
                            }

                            return Task.CompletedTask;
                        },
                    };
                });

            return sc;
        }

        /// <summary>
        /// Add default user cotext and it implementation to DI
        /// </summary>
        /// <param name="services">Service collection</param>
        /// <returns>Chain service collection</returns>
        /// <typeparam name="TUserContextInface">Interface for UserContextImplementation</typeparam>
        /// <typeparam name="TUserContextImplementation">User context class</typeparam>
        public static IServiceCollection AddUserContext<TUserContextInface, TUserContextImplementation>(this IServiceCollection services)
            where TUserContextInface : class, IUserContext
            where TUserContextImplementation : class, TUserContextInface
        {
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            JwtSecurityTokenHandler.DefaultOutboundClaimTypeMap.Clear();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<TUserContextInface, TUserContextImplementation>();
            return services;
        }

        /// <summary>
        /// Parameters method
        /// </summary>
        /// <param name="securityKey">Security key</param>
        /// <returns> Returns token validation parameters</returns>
        private static TokenValidationParameters GetokenValidationParameters(JsonWebKey securityKey)
        {
            return new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = securityKey,
                ClockSkew = TimeSpan.Zero,
            };
        }

        private static T BindOptions<T>(this IServiceCollection services, IConfiguration configuration, string name = null, [AllowNull] Action<JwtBearerOptions> jwtOptionConfig = null)
            where T : class
        {
            if (string.IsNullOrEmpty(name))
            {
                name = typeof(T).Name;
            }

            services.AddOptions<T>()
                    .Bind(configuration.GetSection(name))
                    .ValidateDataAnnotations();

            return configuration.GetSection(name).Get<T>();
        }

        private static class Base64UrlEncoder
        {
            public static string Encode(byte[] arg)
            {
                string s = Convert.ToBase64String(arg); // Regular base64 encoder
                s = s.Split('=')[0]; // Remove any trailing '='s
                s = s.Replace('+', '-'); // 62nd char of encoding
                s = s.Replace('/', '_'); // 63rd char of encoding
                return s;
            }

            public static byte[] Decode(string arg)
            {
                string s = arg;
                s = s.Replace('-', '+'); // 62nd char of encoding
                s = s.Replace('_', '/'); // 63rd char of encoding
                switch (s.Length % 4) // Pad with trailing '='s
                {
                    case 0: break; // No pad chars in this case
                    case 2: s += "=="; break; // Two pad chars
                    case 3: s += "="; break; // One pad char
                    default:
                        throw new System.Exception("Illegal base64url string!");
                }
                return Convert.FromBase64String(s);
            }
        }
    }
}