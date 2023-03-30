// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataGroupBase.cs" company="TETA">
// Copyright (c) TETA. Ufa, 2023.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
// <summary>
//  Группа данных, напримео набор строк и столбцов
// </summary>

using System.Collections.Generic;

namespace Teta.Packages.Files.Contracts.Internal
{
    /// <summary>
    /// Группа данных, напримео набор строк и столбцов
    /// </summary>
    public class DataGroupBase
    {
        /// <summary>
        /// Имя для группы данных
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Первая строка с данными
        /// </summary>
        public int? FirstRowIndex { get; set; }

        /// <summary>
        /// Описание столбцов файлов
        /// </summary>
        public List<FileColumnDescription> ColumnDescriptions { get; set; }
    }
}
