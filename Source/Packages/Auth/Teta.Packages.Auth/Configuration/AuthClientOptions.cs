// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AuthClientOptions.cs" company="TETA">
// Copyright (c) TETA. Ufa, 2023.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
// <summary>
//  Authorization client settings
// </summary>

using System.ComponentModel.DataAnnotations;
using Microsoft.IdentityModel.Tokens;

namespace Teta.Packages.Auth.Configuration
{
    /// <summary>
    /// Authorization client settings
    /// </summary>
    public class AuthClientOptions
    {
        /// <summary>
        /// Использовать сервис keycloak
        /// </summary>
        public bool UseKeycloakIdentityService { get; set; } = false;

        /// <summary>
        /// Secret for JWT token auth
        /// </summary>
        [Required]
        public JsonWebKey Secret { get; set; }
    }
}
