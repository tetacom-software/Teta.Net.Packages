// --------------------------------------------------------------------------------------------------------------------
// <copyright file="XlsxFileHeaderReader.cs" company="TETA">
// Copyright (c) TETA. Ufa, 2022.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
// <summary>
//  Интерпритатор файлов Excel
// </summary>

using OfficeOpenXml;
using Teta.Files.Contracts;
using Teta.Files.Contracts.Internal;
using Teta.Packages.Files.Bll.Interface;
using Teta.Packages.Files.Contracts;

namespace Teta.Packages.Files.Bll.Implementation.HeaderInterpreters
{
    /// <summary>
    /// Интерпритатор файлов Excel
    /// </summary>
    [FileExtension(".xlsx")]
    public class XlsxFileHeaderReader : IFileDescriptionReader
    {
        /// <summary>
        /// Обработать файл
        /// </summary>
        /// <param name="file">Файл</param>
        /// <param name="descriptor">Метаданные файла</param>
        /// <param name="description">File description</param>
        public void FillDescriptionData(Stream file, FileDescriptor descriptor, FileDescription description)
        {
            using var eppack = new ExcelPackage();
            try
            {
                eppack.Load(file);
            }
            catch (Exception ex)
            {
                throw new Exception("File should be an Excel 2007/2010 XLSX file", ex);
            }

            description.DataGroups = new List<DataGroup>();
            foreach (var ws in eppack.Workbook.Worksheets)
            {
                if (ws is ExcelChartsheet)
                {
                    continue;
                }

                if (ws.Dimension == null)
                {
                    continue;
                }

                var dg = new DataGroup { Name = ws.Name, RowsPreview = new List<FileRow>(), ColumnDescriptions = new List<FileColumnDescription>(), FirstRowIndex = 2 };
                description.DataGroups.Add(dg);
                var columnCount = ws.Dimension.Columns > FilesExtensions.ColumnsLimit ? FilesExtensions.ColumnsLimit : ws.Dimension.Columns;
                for (var colIdx = 1; colIdx <= columnCount; colIdx++)
                {
                    var columnLetter = ExcelCellAddress.GetColumnLetter(colIdx);
                    var columnName = description.HeaderRowIdx == 0 ? columnLetter : ws.GetValue(description.HeaderRowIdx, colIdx)?.ToString();

                    dg.ColumnDescriptions.Add(new FileColumnDescription
                    {
                        Name = string.IsNullOrWhiteSpace(columnName) ? columnLetter : columnName,
                        Position = colIdx - 1,
                        NeedImport = false,
                    });
                }

                var rowsCount = ws.Dimension.Rows > description.RowsInPreview ? description.RowsInPreview : ws.Dimension.Rows;
                for (var rowIdx = 1; rowIdx <= rowsCount; rowIdx++)
                {
                    var row = new FileRow { Values = new object[dg.ColumnDescriptions.Count] };
                    for (var colIdx = 0; colIdx < dg.ColumnDescriptions.Count; colIdx++)
                    {
                        row.Values[colIdx] = ws.Cells[rowIdx, colIdx + 1].Text;
                    }

                    dg.RowsPreview.Add(row);
                }
            }

            description.FileType = FileType.Xlsx;
        }
    }
}
