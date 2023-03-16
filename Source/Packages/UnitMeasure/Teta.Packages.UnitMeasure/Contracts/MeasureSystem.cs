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
using Microsoft.EntityFrameworkCore;
using Teta.Packages.UoW.EfCore.Interfaces.BusinessEntity;

namespace Teta.Packages.UnitMeasure.Contracts
{
    /// <summary>
    /// Default measure system
    /// </summary>
    [Table("measure_system")]
    [Comment("Default measure system")]
    public class MeasureSystem : IBusinessEntity<int>
    {
        /// <inheritdoc/>
        [Key]
        [Column("id")]
        [Comment("Primary key")]
        public int Id { get; set; }

        /// <summary>
        /// System name
        /// </summary>
        [Column("name")]
        [Comment("System name")]
        [Required(AllowEmptyStrings = false)]
        public string Name { get; set; }

        /// <summary>
        /// Calc parameter link
        /// </summary>
        public List<MeasureSystemCalcParameter> MeasureSystemParameters { get; set; }
    }
}