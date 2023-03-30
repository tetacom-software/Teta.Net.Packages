// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ColumnDescription.cs" company="TETA">
// Copyright (c) TETA. Ufa, 2023.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
// <summary>
//  Описание колонок файла данных
// </summary>

namespace Teta.Packages.Files.Contracts.Internal
{
    /// <summary>
    ///  Описание колонок файла данных
    /// </summary>
    public class FileColumnDescription
    {
        /// <summary>
        /// Имя колонки
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Позиция столбца в файле
        /// </summary>
        public int Position { get; set; }

        /// <summary>
        /// Единица измерения
        /// </summary>
        public int? UnitId { get; set; }

        /// <summary>
        /// Связанный показатель системы
        /// </summary>
        public int? LinkedParameterId { get; set; }

        /// <summary>
        /// Кастомное имя параметра
        /// </summary>
        public string ParamCustomName { get; set; }

        /// <summary>
        /// Необходимо импортировать столбец
        /// </summary>
        public bool NeedImport { get; set; }
    }
}
