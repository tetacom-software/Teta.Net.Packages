﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICommonDbContext.cs" company="TETA">
// Copyright (c) TETA. Ufa, 2022.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
// <summary>
// Interface for common DbContext
// </summary>

using Microsoft.EntityFrameworkCore;

namespace Teta.Packages.UoW.EfCore.Interfaces.BusinessEntity;

/// <summary>
/// Interface for common DbContext
/// </summary>
public interface ICommonDbContext
{
    /// <summary>
    /// Returns DbContext
    /// </summary>
    DbContext DbContext { get; }
}