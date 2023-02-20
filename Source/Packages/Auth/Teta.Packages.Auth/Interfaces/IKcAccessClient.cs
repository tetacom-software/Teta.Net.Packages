// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IKcHttpClient.cs" company="TETA">
// Copyright (c) TETA. Ufa, 2023.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
// <summary>
//  Keycloak interface
// </summary>

using Teta.Packages.Auth.Contracts;

namespace Teta.Packages.Auth.Interfaces
{
    /// <summary>
    /// Keycloak interface
    /// </summary>
    public interface IKcAccessClient
    {
        /// <summary>
        /// Get access token 
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        Task<KcAuthResponse> GetAccessTokenAsync(string username, string password);

        /// <summary>
        /// Renew access token using refresh token
        /// </summary>
        /// <param name="refreshToken"></param>
        /// <returns></returns>
        Task<KcAuthResponse> RefreshAccessToken(string refreshToken);
    }

}
