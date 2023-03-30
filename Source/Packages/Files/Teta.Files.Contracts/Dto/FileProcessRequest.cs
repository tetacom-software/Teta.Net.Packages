// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileProcessRequest.cs" company="TETA">
// Copyright (c) TETA. Ufa, 2023.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
// <summary>
// Запрос на сохранение данных
// </summary>

using System.ComponentModel.DataAnnotations;

namespace Teta.Packages.Files.Contracts.Dto
{
    /// <summary>
    /// Запрос на сохранение данных
    /// </summary>
    public class FileProcessRequest
    {
        /// <summary>
        /// Илентификатор файла
        /// </summary>
        [Required]
        public string FileKey { get; set; }

        /// <summary>
        /// Разделитель
        /// </summary>
        public string Delimiter { get; set; }

        /// <summary>
        /// Кодировка
        /// </summary>
        public string Encoding { get; set; }

        /// <summary>
        /// строка по которой читается шапка (для CSV)
        /// </summary>
        public int? HeaderDetectRow { get; set; }

        /// <summary>
        /// Число строк в предпросмотре
        /// </summary>
        public int? RowsInPreview { get; set; }

        /// <summary>
        /// Первый запрос
        /// </summary>
        public bool IsFirstRequest { get; set; }
    }
}
