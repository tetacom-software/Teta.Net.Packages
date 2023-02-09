// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnitGroup.cs" company="TETA">
// Copyright (c) TETA. Ufa, 2022.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
// <summary>
//  Unit measure settings
// </summary>

using System.ComponentModel.DataAnnotations;

namespace Teta.Packages.UnitMeasure.Contracts
{
    /// <summary>
    /// Unit measure settings
    /// </summary>
    public class UnitMeasureOptions
    {
        /// <summary>
        /// Schema name
        /// </summary>
        [Required]
        public string SchemaName { get; set; }
    }
}