// --------------------------------------------------------------------------------------------------------------------
// <copyright file="KcClaimTypesConstants.cs" company="TETA">
// Copyright (c) TETA. Ufa, 2023.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
// <summary>
// Claim type constants
// </summary>

namespace Teta.Packages.Auth.Contracts
{

    /// <summary>
    /// Claim types constants
    /// </summary>
    public static class KcClaimTypesConstants
    {
        /// <summary>
        /// Sid claim type cinstants
        /// </summary>
        public static string Sid { get; } = "sid";

        /// <summary>
        /// Email
        /// </summary>
        public static string Email { get; } = "email";

        /// <summary>
        /// Name
        /// </summary>
        public static string Name { get; } = "name";

        /// <summary>
        /// Given name
        /// </summary>
        public static string GivenName { get; } = "given_name";

        /// <summary>
        /// Family name
        /// </summary>
        public static string FamilyName { get; } = "family_name";

        /// <summary>
        /// User name (depending of KC realm settings can contain mail or username)
        /// </summary>
        public static string PreferredUserName { get; } = "preferred_username";

        /// <summary>
        /// Role code
        /// </summary>
        public static string RoleType { get; } = "teta_role";

        /// <summary>
        /// Company identifier
        /// </summary>
        public static string Company { get; } = "company_id";
    }
}
