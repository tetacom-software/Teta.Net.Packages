// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AllUserGrantAccessAttribute.cs" company="TETA">
// Copyright (c) TETA. Ufa, 2023.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
// <summary>
//  Filter implements user claim check logic
// </summary>

namespace Teta.Packages.Auth
{
    /// <summary>
    /// Доступ всех пользователей
    /// </summary>
    public class AllUserGrantAccessAttribute : TetaAuthorizeAttribute
    {
        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="AllUserGrantAccessAttribute"/>.
        /// </summary>
        public AllUserGrantAccessAttribute()
            : base()
        {
        }
    }
}