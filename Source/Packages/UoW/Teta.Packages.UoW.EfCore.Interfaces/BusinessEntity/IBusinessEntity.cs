// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IBusinessEntity.cs" company="TETA">
// Copyright (c) TETA. Ufa, 2022.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
// <summary>
// Базовая бизнес сущность
// </summary>

namespace Teta.Packages.UoW.EfCore.Interfaces.BusinessEntity
{
    /// <summary>
    /// Базовая бизнес сущность
    /// </summary>
    /// <typeparam name="T">Key filed type</typeparam>
    public interface IBusinessEntity<T>
        where T : struct
    {
        /// <summary>
        /// Идентификатор записи
        /// </summary>
        T Id { get; set; }
    }
}