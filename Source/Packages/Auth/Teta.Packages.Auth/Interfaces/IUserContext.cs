// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IUserContext.cs" company="TETA">
// Copyright (c) TETA. Ufa, 2023.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
// <summary>
//  Context for autorized user
// </summary>

namespace Teta.Packages.Auth.Interfaces
{
    /// <summary>
    /// Context for autorized user
    /// </summary>
    public interface IUserContext
    {
        /// <summary>
        /// Получает токен из хедера Authtorization
        /// </summary>
        string Token { get; }

        /// <summary>
        /// Anonymous user
        /// </summary>
        bool IsAnonymous { get; }

        /// <summary>
        /// User id
        /// </summary>
        string Sid { get; }

        /// <summary>
        /// User name
        /// </summary>
        string PreferredUserName { get; }

        /// <summary>
        /// Display name
        /// </summary>
        string GivenName { get; }

        /// <summary>
        /// Email
        /// </summary>
        string Email { get; }

        /// <summary>
        /// Entity has super user claim
        /// </summary>
        bool IsSuperUser { get; }

        /// <summary>
        /// Идентификаторы компаний пользователя
        /// </summary>
        public IEnumerable<int> CompanyIds { get; }
    }
}
