// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileDescription.cs" company="TETA">
// Copyright (c) TETA. Ufa, 2023.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
// <summary>
//  Описание файла дданных системы
// </summary>

using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Teta.Packages.Files.Contracts.Internal
{
    /// <summary>
    /// Описание файла дданных системы
    /// </summary>
    public class FileDescription
    {
        /// <summary>
        /// Группы данных, например листы EXCEL
        /// </summary>
        public List<DataGroup> DataGroups { get; set; }

        /// <summary>
        /// Кодировка
        /// </summary>
        [JsonIgnore]
        public Encoding Encoding { get; set; }

        /// <summary>
        /// Кодировка
        /// </summary>
        public string WebEncoding => Encoding?.WebName;

        /// <summary>
        /// Исходное содержимое файла (первые несколько строк)
        /// </summary>
        public string RawData { get; set; }

        /// <summary>
        /// Разделитель
        /// </summary>
        public string Delimiter { get; set; }

        /// <summary>
        /// Идентификатор строки с которой начинается шапка
        /// </summary>
        public int HeaderRowIdx { get; set; }

        /// <summary>
        /// Тип загружаемого файла
        /// </summary>
        public FileType FileType { get; set; }

        /// <summary>
        /// Число строк в предпросмотре
        /// </summary>
        public int RowsInPreview { get; set; }

        /// <summary>
        /// Replce group
        /// </summary>
        /// <param name="baseGroup">base group</param>
        /// <param name="idx">Index</param>
        public void ReplacaGroup(DataGroupBase baseGroup, int idx)
        {
            if (DataGroups != null)
            {
                var grp = DataGroups[idx];
                DataGroups[idx] = new DataGroup()
                {
                    RowsPreview = grp.RowsPreview,
                    ColumnDescriptions = baseGroup.ColumnDescriptions,
                    Name = baseGroup.Name,
                    FirstRowIndex = baseGroup.FirstRowIndex,
                };
            }
        }
    }
}
