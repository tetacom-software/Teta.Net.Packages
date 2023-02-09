// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultMeasureSystem.cs" company="TETA">
// Copyright (c) TETA. Ufa, 2022.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
// <summary>
//  Default measure system
// </summary>

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Teta.Packages.UoW.EfCore.Interfaces.BusinessEntity;

namespace Teta.Packages.UnitMeasure.Contracts
{
    /// <summary>
    /// Default measure system
    /// </summary>
    [Table("measure_system")]
    public class MeasureSystem : IBusinessEntity<int>
    {
        /// <inheritdoc/>
        [Key]
        [Column("id")]
        public int Id { get; set; }

        /// <summary>
        /// System name
        /// </summary>
        [Column("name")]
        public string Name { get; set; }

        /// <summary>
        /// Calc parameter link
        /// </summary>
        public MeasureSystemCalcParameter[] MeasureSystemParameters { get; set; }
    }
}
