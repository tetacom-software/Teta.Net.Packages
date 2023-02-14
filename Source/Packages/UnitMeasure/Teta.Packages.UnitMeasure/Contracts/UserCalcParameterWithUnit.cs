// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UserCalcParameterWithUnit.cs" company="TETA">
// Copyright (c) TETA. Ufa, 2022.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
// <summary>
//  Calc unit user parameter link
// </summary>

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Teta.Packages.UoW.EfCore.Interfaces.BusinessEntity;

namespace Teta.Packages.UnitMeasure.Contracts
{
    /// <summary>
    /// Calc unit user parameter link
    /// </summary>
    [Table("calc_unit_user_parameter_link")]
    [Comment("Calc unit user parameter link")]
    public class UserCalcParameterWithUnit : IBusinessEntity<int>
    {
        /// <summary>
        /// PK
        /// </summary>
        [Key]
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Comment("Primary key")]
        public int Id { get; set; }

        /// <summary>
        /// Calc parameter id
        /// </summary>
        [Column("calc_parameter_id")]
        [Comment("Calc parameter id")]
        public int ParameterId { get; set; }

        /// <summary>
        /// Calc unit id
        /// </summary>
        [Column("unit_id")]
        [Comment("Calc unit id")]
        public int UnitId { get; set; }

        /// <summary>
        /// User id
        /// </summary>
        [Column("user_id")]
        [Comment("User id")]
        public int UserId { get; set; }

        /// <summary>
        /// Calc parameter link
        /// </summary>
        public CalcParameter CalcParameter { get; set; }

        /// <summary>
        /// Calc unit link
        /// </summary>
        public CalcUnit Unit { get; set; }
    }
}