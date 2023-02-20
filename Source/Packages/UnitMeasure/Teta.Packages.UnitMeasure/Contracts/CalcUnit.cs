// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CalcUnit.cs" company="TETA">
// Copyright (c) TETA. Ufa, 2022.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
// <summary>
//  Calc unit
// </summary>

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Teta.Packages.UoW.EfCore.Interfaces.BusinessEntity;

namespace Teta.Packages.UnitMeasure.Contracts
{
    /// <summary>
    /// Calc unit
    /// </summary>
    [Table("calc_unit")]
    [Comment("Calc unit")]
    public class CalcUnit : IBusinessEntity<int>
    {
        /// <summary>
        /// PK
        /// </summary>
        [Column("id")]
        [Key]
        [Comment("Primary key")]
        public int Id { get; set; }

        /// <summary>
        /// Unit name
        /// </summary>
        [Column("name")]
        [Comment("Unit name")]
        [Required(AllowEmptyStrings = false)]
        public string Name { get; set; }

        /// <summary>
        /// Unit short name
        /// </summary>
        [Column("short_name")]
        [Comment("Unit short name")]
        public string ShortName { get; set; }

        /// <summary>
        /// Localization tag
        /// </summary>
        [Column("tag")]
        [Comment("Localization tag")]
        [Required(AllowEmptyStrings = false)]
        public string Tag { get; set; }

        /// <summary>
        /// Unit group id
        /// </summary>
        [Column("unit_group_id")]
        [Comment("Unit group id")]
        public int UnitGroupId { get; set; }

        /// <summary>
        /// Unit convert table link
        /// </summary>
        public CalcUnitConvert Convertation { get; set; }
    }
}