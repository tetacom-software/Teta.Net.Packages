// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TetaAuthorizeAttribute.cs" company="TETA">
// Copyright (c) TETA. Ufa, 2023.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
// <summary>
//  Authorization attribute, claims check
// </summary>

using Microsoft.AspNetCore.Mvc;

namespace Teta.Packages.Auth
{
    /// <summary>
    /// Authorization attribute, claims check
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class TetaAuthorizeAttribute : TypeFilterAttribute
    {
        /// <summary>
        /// Role claims
        /// </summary>
        public string[] Claims { get; }

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="TetaAuthorizeAttribute"/>.
        /// </summary>
        /// <param name="claims">Acceptable user claims</param>
        public TetaAuthorizeAttribute(params string[] claims)
            : base(typeof(AuthorizationFilter))
        {
            Claims = claims;
            Arguments = new object[] { claims };
        }
    }
}
