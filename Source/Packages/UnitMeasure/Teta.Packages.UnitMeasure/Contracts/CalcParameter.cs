// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CalcParameter.cs" company="TETA">
// Copyright (c) TETA. Ufa, 2022.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
// <summary>
//  Calc parameter
// </summary>

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Teta.Packages.UoW.EfCore.Interfaces.BusinessEntity;

namespace Teta.Packages.UnitMeasure.Contracts
{
    /// <summary>
    /// Calc parameter
    /// </summary>
    [Table("calc_parameter")]
    public class CalcParameter : IBusinessEntity<int>
    {
        /// <summary>
        /// Entity id
        /// </summary>
        [Key]
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// Parameter name
        /// </summary>
        [Column("name")]
        public string Name { get; set; }

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
        /// Unit group link
        /// </summary>
        public UnitGroup UnitGroup { get; set; }
    }
}
