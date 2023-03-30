// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataGroup.cs" company="TETA">
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
    public class DataGroup : DataGroupBase
    {
        /// <summary>
        /// Строки
        /// </summary>
        public List<FileRow> RowsPreview { get; set; }
    }
}
