// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileType.cs" company="TETA">
// Copyright (c) TETA. Ufa, 2023.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
// <summary>
//  Тип файла данных
// </summary>

using System.ComponentModel;

namespace Teta.Packages.Files.Contracts.Internal
{
    /// <summary>
    /// Тип файла данных
    /// </summary>
    public enum FileType
    {
        /// <summary>
        /// Неизвестный тип файла
        /// </summary>
        [Description("unknown")]
        Unknown = 0,

        /// <summary>
        /// Файл xlsx
        /// </summary>
        [Description(".xlsx")]
        Xlsx = 1,

        /// <summary>
        /// Файл csv
        /// </summary>
        [Description(".csv")]
        Csv = 2,

        /// <summary>
        /// Файл las
        /// </summary>
        [Description(".las")]
        Las = 3,

        /// <summary>
        /// Файл txt
        /// </summary>
        [Description(".txt")]
        Txt = 4,
    }
}
