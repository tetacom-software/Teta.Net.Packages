// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CsvFileHeaderReader.cs" company="TETA">
// Copyright (c) TETA. Ufa, 2022.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
// <summary>
//  Интерпритатор файлов csv, txt
// </summary>

using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using OfficeOpenXml;
using Teta.Files.Contracts;
using Teta.Files.Contracts.Internal;
using Teta.Packages.Files.Bll.Interface;
using Teta.Packages.Files.Contracts;

namespace Teta.Packages.Files.Bll.Implementation.HeaderInterpreters
{
    /// <summary>
    /// Интерпритатор файлов csv, txt
    /// </summary>
    [FileExtension(".csv")]
    [FileExtension(".txt")]
    public class CsvFileHeaderReader : IFileDescriptionReader
    {
        /// <summary>
        /// Обработать файл
        /// </summary>
        /// <param name="file">Файл</param>
        /// <param name="descriptor">Метаданные файла</param>
        /// <param name="description">File description</param>
        public void FillDescriptionData(Stream file, FileDescriptor descriptor, FileDescription description)
        {
            file.Position = 0;
            if (file == null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            var encoding = FilesExtensions.GetEncoding(file);
            var reader = new StreamReader(file, encoding);
            var csvOption = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = description.Delimiter,
                IgnoreBlankLines = false,
                HasHeaderRecord = true,
                BadDataFound = null,
            };

            var csvReader = new CsvReader(reader, csvOption);
            for (int i = 0; i < description.HeaderRowIdx; i++)
            {
                csvReader.Read();
            }

            csvReader.ReadHeader();
            var columns = csvReader.HeaderRecord;

            description.DataGroups = new List<DataGroup>();
            var dataGroup = new DataGroup
            {
                Name = descriptor.FileName,
                RowsPreview = new List<FileRow>(),
                ColumnDescriptions = new List<FileColumnDescription>(),
            };

            description.DataGroups.Add(dataGroup);
            var columnscCount = columns.Length > FilesExtensions.ColumnsLimit ? FilesExtensions.ColumnsLimit : columns.Length;
            for (var colIdx = 0; colIdx < columnscCount; colIdx++)
            {
                dataGroup.ColumnDescriptions.Add(new FileColumnDescription
                {
                    Name = !string.IsNullOrEmpty(columns[colIdx]) ? columns[colIdx] : ExcelCellAddress.GetColumnLetter(colIdx + 1),
                    Position = colIdx,
                    NeedImport = false,
                });
            }

            var rowCount = 0;
            while (true)
            {
                if (rowCount != 0)
                {
                    if (!csvReader.Read())
                    {
                        break;
                    }
                }

                if (rowCount == description.RowsInPreview)
                {
                    break;
                }

                var row = new FileRow
                {
                    Values = new object[dataGroup.ColumnDescriptions.Count],
                };

                for (var colIdx = 0; colIdx < dataGroup.ColumnDescriptions.Count; colIdx++)
                {
                    row.Values[colIdx] = csvReader.GetField<string>(colIdx);
                }

                dataGroup.RowsPreview.Add(row);
                rowCount++;
            }

            description.FileType = FileType.Csv;
        }
    }
}
