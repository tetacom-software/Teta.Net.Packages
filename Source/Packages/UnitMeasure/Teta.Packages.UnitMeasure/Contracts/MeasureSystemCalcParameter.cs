// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultMeasureSystemCalcParameter.cs" company="TETA">
// Copyright (c) TETA. Ufa, 2022.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
// <summary>
//  Measure system parameter
// </summary>

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Teta.Packages.UoW.EfCore.Interfaces.BusinessEntity;

namespace Teta.Packages.UnitMeasure.Contracts
{
    /// <summary>
    /// Measure system parameter
    /// </summary>
    [Table("measure_system_parameter_unit")]
    [Comment("Measure system parameter")]
    public class MeasureSystemCalcParameter : IBusinessEntity<int>
    {
        /// <summary>
        /// Id
        /// </summary>
        [Key]
        [Column("id")]
        [Comment("Primary key")]
        public int Id { get; set; }

        /// <summary>
        /// Calc parameter id
        /// </summary>
        [Column("calc_parameter_id")]
        [Comment("Calc parameter id")]
        public int ParameterId { get; set; }

        /// <summary>
        /// Measure system id
        /// </summary>
        [Column("measure_system_id")]
        [Comment("Measure system id")]
        public int MeasureSystemId { get; set; }

        /// <summary>
        /// Unit id
        /// </summary>
        [Column("unit_id")]
        [Comment("Unit id")]
        public int UnitId { get; set; }
    }
}