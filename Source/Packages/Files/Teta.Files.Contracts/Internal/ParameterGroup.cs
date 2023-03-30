// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ParameterGroup.cs" company="TETA">
// Copyright (c) TETA. Ufa, 2023.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
// <summary>
//  Группа показателей ключ-наименование для маппинга на столбцы
// </summary>

using System.Collections.Generic;
using Teta.Packages.Files.Contracts.Parameters;

namespace Teta.Packages.Files.Contracts.Internal
{
    /// <summary>
    /// Группа показателей ключ-наименование для маппинга на столбцы
    /// </summary>
    public class ParameterGroup
    {
        /// <summary>
        /// Конструктор
        /// </summary>
        public ParameterGroup()
        {
            Parameters = new List<MappingParameterDto>();
        }

        /// <summary>
        /// Имя группы показателей
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Список показателей
        /// </summary>
        public IList<MappingParameterDto> Parameters { get; set; }
    }
}
