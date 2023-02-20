// --------------------------------------------------------------------------------------------------------------------
// <copyright file="KcAuthResponse.cs" company="TETA">
// Copyright (c) TETA. Ufa, 2023.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
// <summary>
// Keycloak auth response
// </summary>

namespace Teta.Packages.Auth.Contracts
{
    /// <summary>
    /// Keycloak auth response
    /// </summary>
    public class KcAuthResponse
    {
        /// <summary>
        /// Access token
        /// </summary>
        public string AccessToken { get; set; }

        /// <summary>
        /// Refresh token
        /// </summary>
        public string RefreshToken { get; set; }
    }
}
