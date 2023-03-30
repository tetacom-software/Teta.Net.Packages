// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MappingParameterDto.cs" company="TETA">
// Copyright (c) TETA. Ufa, 2023.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
// <summary>
// Показатель для маппинга столбцов при загрузке данных
// </summary>

namespace Teta.Packages.Files.Contracts.Parameters
{
    /// <summary>
    /// Показатель для маппинга столбцов при загрузке данных
    /// </summary>
    public class MappingParameterDto
    {
        /// <summary>
        /// Идентификатор записи
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Название показателя
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Идентификатор показателя используемый в системе единиц измерений
        /// </summary>
        public int? CalcParameterId { get; set; }

        /// <summary>
        /// Идентифитор единицы измерения
        /// </summary>
        public int? UnitId { get; set; }
    }
}