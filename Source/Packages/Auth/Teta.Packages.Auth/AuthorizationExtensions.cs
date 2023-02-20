// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AuthorizationExtensions.cs" company="TETA">
// Copyright (c) TETA. Ufa, 2023.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
// <summary>
//  Расширение для регистрации блока авторизации
// </summary>

using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Net;
using System.Xml.Linq;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Teta.Packages.Auth.Configuration;
using Teta.Packages.Auth.Contracts;
using Teta.Packages.Auth.Implementation;
using Teta.Packages.Auth.Interfaces;
using System.Security.Cryptography;
using Newtonsoft.Json;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Teta.Packages.Auth
{
    /// <summary>
    /// Расширение для регистрации блока авторизации
    /// </summary>
    public static class AuthorizationExtensions
    {
        /// <summary>
        /// Расширение для валидации токена
        /// </summary>
        /// <param name="services">Service collection</param>
        /// <param name="configuration">Объект конфигурации</param>
        /// <returns>Chain service collection</returns>
        public static IServiceCollection AddCustomAuthValidation(this IServiceCollection services, IConfiguration configuration)
        {
            var clientOptions = services.AddWebOptions<AuthClientOptions>(configuration);
            services.AddAuthentication(
                options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                }).AddJwtBearer(
                options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = GeTokenValidationParameters(clientOptions.Secret);
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
        /// Добавляем секцию userdata в jwt
        /// </summary>
        /// <param name="builder">Application builder</param>
        /// <returns>Chain application builder</returns>
        public static IApplicationBuilder UseCustomAuthentication(this IApplicationBuilder builder)
        {
            builder.UseAuthentication();
            return builder;
        }

        /// <summary>
        /// Registrates entities for keycloak 
        /// Resource owner authentication flow
        /// </summary>
        /// <returns></returns>
        public static IServiceCollection UseKeycloakAuthentication(this IServiceCollection sc, IConfiguration config)
        {
            //specify to use TLS 1.2 as default connection
            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;

            // Add JWK from keycloak to authentication extension

            var kcOptions = sc.AddWebOptions<KcOptions>(config);
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

                // Тут же сконфигурировать токены и все что может понадобиться для доступа
                c.Timeout = TimeSpan.FromMilliseconds(options.Value.RequestTimeoutMsec);
            });

   
            sc.AddAuthentication(
                options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                }).AddJwtBearer(
                options =>
                {
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
        /// Регистрация контекста пользователя
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
        /// Расширение для генерации токена
        /// </summary>
        /// <typeparam name="T">Тип опций</typeparam>
        /// <param name="services">Service collection</param>
        /// <param name="configuration">Объект конфигурации</param>
        /// <param name="name">Название конфигурации</param>
        /// <returns>Chain service collection</returns>
        public static T AddWebOptions<T>(this IServiceCollection services, IConfiguration configuration, string name = null)
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

        /// <summary>
        /// Регистрация ключа безопасности
        /// </summary>
        /// <param name="services">Service collection</param>
        /// <param name="secretKey">Секретная строка</param>
        /// <returns>Ключ безопасности</returns>
        public static SigningCredentials AddSecurityKey(this IServiceCollection services, JsonWebKey secretKey)
        {
            var signingCredentials = new SigningCredentials(secretKey, secretKey.Alg);
            services.AddSingleton(_ => signingCredentials);

            return signingCredentials;
        }

        /// <summary>
        /// Parameters method
        /// </summary>
        /// <param name="securityKey">Security key</param>
        /// <returns> Returns token validation parameters</returns>
        public static TokenValidationParameters GeTokenValidationParameters(JsonWebKey securityKey)
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


        public static string ToJWK(this RSA rsa, string kid)
        {
            var parameters = rsa.ExportParameters(false);

            var jwk = new
            {
                kty = "RSA",
                kid = kid,
                n = Base64UrlEncoder.Encode(parameters.Modulus),
                e = Base64UrlEncoder.Encode(parameters.Exponent)
            };

            return JsonConvert.SerializeObject(jwk);
        }


        public static class Base64UrlEncoder
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