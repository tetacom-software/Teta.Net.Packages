// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IUnitDbContext.cs" company="TETA">
// Copyright (c) TETA. Ufa, 2022.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
// <summary>
//  Interface unit measure db context
// </summary>

using Microsoft.EntityFrameworkCore;
using Teta.Packages.UnitMeasure.Contracts;

namespace Teta.Packages.UnitMeasure
{
    /// <summary>
    /// Interface unit measure db context
    /// </summary>
    public interface IUnitDbContext
    {
        /// <summary>
        /// Calc parameter set
        /// </summary>
        DbSet<CalcParameter> CalcParameters { get; set; }

        /// <summary>
        /// Calc unit set
        /// </summary>
        DbSet<CalcUnit> Units { get; set; }

        /// <summary>
        /// Unit group set
        /// </summary>
        DbSet<UnitGroup> Groups { get; set; }

        /// <summary>
        /// Unit convertation set
        /// </summary>
        DbSet<CalcUnitConvert> UnitsConvert { get; set; }

        /// <summary>
        /// Default measure system set
        /// </summary>
        DbSet<MeasureSystem> MeasureSystems { get; set; }

        /// <summary>
        /// Measure system parameter
        /// </summary>
        DbSet<MeasureSystemCalcParameter> MeasureSystemCalcParameters { get; set; }

        /// <summary>
        /// User settings
        /// </summary>
        DbSet<UserCalcParameterWithUnit> UserCalcParameterWithUnit { get; set; }
    }
}