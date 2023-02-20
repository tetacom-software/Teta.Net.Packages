// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UserData.cs" company="TETA">
// Copyright (c) TETA. Ufa, 2023.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
// <summary>
//   Additional user data
// </summary>

using System.Linq;

namespace Teta.Packages.Auth.Configuration
{
    /// <summary>
    /// Additional user data
    /// </summary>
    public class UserData
    {
        /// <summary>
        /// User company
        /// </summary>
        public int[] UserCompaniesClaims { get; set; }

        /// <summary>
        /// Проверить принадлежность пользователя к компании
        /// </summary>
        /// <param name="companyId">Идентификатор компании</param>
        /// <returns>Принадлежность пользователя к компании</returns>
        public bool ContainsCompany(int companyId)
        {
            return UserCompaniesClaims.Any(c => c == companyId);
        }
    }
}
