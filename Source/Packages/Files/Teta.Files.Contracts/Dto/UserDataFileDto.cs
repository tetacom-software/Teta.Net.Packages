// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UserDataFileDto.cs" company="TETA">
// Copyright (c) TETA. Ufa, 2023.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
// <summary>
//  DTO класс для пользовательского файла
// </summary>

namespace Teta.Packages.Files.Contracts.Dto
{
    /// <summary>
    /// DTO класс для пользовательского файла
    /// </summary>
    public class UserDataFileDto
    {
        /// <summary>
        /// Идентификатор записи о файле данных пользователя
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Идентификатор пользователя
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Идентификатор файла
        /// </summary>
        public int FileId { get; set; }

        /// <summary>
        /// Ошибка обработки файла
        /// </summary>
        public string ProcessException { get; set; }
    }
}
