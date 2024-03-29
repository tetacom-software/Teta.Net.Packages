﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnitGroup.cs" company="TETA">
// Copyright (c) TETA. Ufa, 2022.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
// <summary>
//  Calc unit group
// </summary>

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Teta.Packages.UoW.EfCore.Interfaces.BusinessEntity;

namespace Teta.Packages.UnitMeasure.Contracts
{
    /// <summary>
    /// Calc unit group
    /// </summary>
    [Table("calc_unit_group")]
    [Comment("Calc unit group")]
    public class UnitGroup : IBusinessEntity<int>
    {
        /// <summary>
        /// PK
        /// </summary>
        [Key]
        [Column("id")]
        [Comment("Primary key")]
        public int Id { get; set; }
        
        /// <summary>
        /// Group name
        /// </summary>
        [Column("name")]
        [Comment("Group name")]
        [Required(AllowEmptyStrings = false)]
        public string Name { get; set; }

        /// <summary>
        /// Localization tag
        /// </summary>
        [Column("tag")]
        [Comment("Localization tag")]
        [Required(AllowEmptyStrings = false)]
        public string Tag { get; set; }

        /// <summary>
        /// Calc unit link
        /// </summary>
        public IList<CalcUnit> Units { get; set; }

        /// <summary>
        /// Get unit or default
        /// </summary>
        /// <param name="unitId">Unit id</param>
        /// <returns>Default calc unit</returns>
        public CalcUnit GetUnitOrDefault(int unitId)
        {
            var unit = Units.FirstOrDefault(u => u.Id == unitId);
            if (unit == null)
            {
                unit = Units.FirstOrDefault(u => u.Convertation.IsDefault);
            }

            return unit;
        }
    }
}