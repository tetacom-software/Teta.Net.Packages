// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileRow.cs" company="TETA">
// Copyright (c) TETA. Ufa, 2023.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
// <summary>
//  Строка файла
// </summary>

namespace Teta.Packages.Files.Contracts.Internal
{
    /// <summary>
    /// Строка файла
    /// </summary>
    public class FileRow
    {
        /// <summary>
        /// Данные по столбцам
        /// </summary>
        public object[] Values { get; set; }
    }
}
