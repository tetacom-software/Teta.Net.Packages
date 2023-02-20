// --------------------------------------------------------------------------------------------------------------------
// <copyright file="KcOptions.cs" company="TETA">
// Copyright (c) TETA. Ufa, 2023.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
// <summary>
// Keycloak options
// </summary>

using System.ComponentModel.DataAnnotations;

namespace Teta.Packages.Auth.Contracts
{
    /// <summary>
    /// Keycloak options
    /// </summary>
    public class KcOptions
    {
        /// <summary>
        /// Auth realm name
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        public string Realm { get; set; }

        /// <summary>
        /// Client identifier
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        public string ClientId { get; set; }

        /// <summary>
        /// Client secret key
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        public string ClientSecret { get; set; }

        /// <summary>
        /// Keycloak URL
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        public string Host { get; set; }

        /// <summary>
        /// Keycloak port
        /// </summary>
        public int Port { get; set; } = 80;

        /// <summary>
        /// KC service request timeout in milliseconds (default 1000)
        /// </summary>
        public int RequestTimeoutMsec { get; set; } = 1000;

        /// <summary>
        /// Certificate path to KC jwt validation
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        public string PemCertificate { get; set; }
    }

}
