// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileProcessResponse.cs" company="TETA">
// Copyright (c) TETA. Ufa, 2023.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
// <summary>
// Результат предобработки файла
// </summary>

using System.Collections.Generic;
using Teta.Packages.Files.Contracts.Internal;

namespace Teta.Packages.Files.Contracts.Dto
{
    /// <summary>
    /// Результат предобработки файла
    /// </summary>
    public class FileProcessResponse
    {
        /// <summary>
        /// Тип файла
        /// </summary>
        public FileType FileType { get; set; }

        /// <summary>
        /// Исходные данные для файла
        /// </summary>
        public string RawData { get; set; }

        /// <summary>
        /// Группы данных
        /// </summary>
        public IList<DataGroup> DataGroup { get; set; }

        /// <summary>
        /// Кодировка
        /// </summary>
        public string Encoding { get; set; }

        /// <summary>
        /// Ошибка обработки файла
        /// </summary>
        public string ProcessException { get; set; }
        
        /// <summary>
        /// Разделитель
        /// </summary>
        public string Delimiter { get; set; }
    }
}
