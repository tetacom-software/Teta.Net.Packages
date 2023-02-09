// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CalcUnitConvert.cs" company="TETA">
// Copyright (c) TETA. Ufa, 2022.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
// <summary>
//  Calc unit convertation formula
// </summary>

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Teta.Packages.UoW.EfCore.Interfaces.BusinessEntity;

namespace Teta.Packages.UnitMeasure.Contracts
{
    /// <summary>
    /// Calc unit convertation formula
    /// </summary>
    [Table("calc_unit_convert")]
    public class CalcUnitConvert : IBusinessEntity<int>
    {
        /// <summary>
        /// PK
        /// </summary>
        [Key]
        [Column("id")]
        public int Id { get; set; }

        /// <summary>
        /// Unit id
        /// </summary>
        [Column("unit_id")]
        public int UnitId { get; set; }

        /// <summary>
        /// The formula for converting from this unit to the standard
        /// </summary>
        [Column("convert_from")]
        public string ConvertFrom { get; set; }

        /// <summary>
        /// The formula for converting to this unit from the standard
        /// </summary>
        [Column("convert_to")]
        public string ConvertTo { get; set; }

        /// <summary>
        /// Is default unit
        /// </summary>
        [Column("is_default")]
        public bool IsDefault { get; set; }
    }
}
