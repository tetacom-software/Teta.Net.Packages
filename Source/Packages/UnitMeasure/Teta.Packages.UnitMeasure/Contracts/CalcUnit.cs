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
using Teta.Packages.UoW.EfCore.Interfaces.BusinessEntity;

namespace Teta.Packages.UnitMeasure.Contracts
{
    /// <summary>
    /// Calc unit
    /// </summary>
    [Table("calc_unit")]
    public class CalcUnit : IBusinessEntity<int>
    {
        /// <summary>
        /// PK
        /// </summary>
        [Column("id")]
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Unit name
        /// </summary>
        [Column("name")]
        public string Name { get; set; }

        /// <summary>
        /// Unit short name
        /// </summary>
        [Column("short_name")]
        public string ShortName { get; set; }

        /// <summary>
        /// Localization tag
        /// </summary>
        [Column("tag")]
        public string Tag { get; set; }

        /// <summary>
        /// Unit group id
        /// </summary>
        [Column("unit_group_id")]
        public int UnitGroupId { get; set; }

        /// <summary>
        /// Unit convert table link
        /// </summary>
        public CalcUnitConvert Convertation { get; set; }
    }
}
