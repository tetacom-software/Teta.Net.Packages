// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LasFileHeaderReader.cs" company="TETA">
// Copyright (c) TETA. Ufa, 2022.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
// <summary>
//  Интерпритатор заголовка файлов LAS
// </summary>

using System.Text.RegularExpressions;
using Teta.Files.Contracts;
using Teta.Files.Contracts.Internal;
using Teta.Packages.Files.Bll.Implementation.DataParser;
using Teta.Packages.Files.Bll.Interface;
using Teta.Packages.Files.Contracts;

namespace Teta.Packages.Files.Bll.Implementation.HeaderInterpreters
{
    /// <summary>
    /// Интерпритатор заголовка файлов LAS
    /// </summary>
    [FileExtension(".las")]
    public class LasFileHeaderReader : IFileDescriptionReader
    {
        private const string NameRe = @"\.?(?<name>[^.]*)\.";
        private const string ValueRe = @"(?<value>.*):";
        private const string DescRe = @"(?<descr>.*)";
        private const string UnitRe = @"(?<unit>([0-9]+\s)?[^\s]*)";
        private const string NameMissingPeriodRe = "(?<name>[^:]*):";
        private const string ValueMissingPeriodRe = "(?<value>.*)";
        private const string ValueWithoutColonDelimiterRe = "(?<value>[^:]*)";
        private const string NameWithDotsRe = @"\.?(?<name>[^.].*[.])\.";

        /// <summary>
        /// Обработать файл
        /// </summary>
        /// <param name="file">Файл</param>
        /// <param name="descriptor">Метаданные файла</param>
        /// <param name="description">File description</param>
        public void FillDescriptionData(Stream file, FileDescriptor descriptor, FileDescription description)
        {
            file.Position = 0;

            description.FileType = FileType.Las;
            description.DataGroups = new List<DataGroup>();
            var dataGroup = new DataGroup
            {
                Name = descriptor.FileName,
                RowsPreview = new List<FileRow>(),
                ColumnDescriptions = new List<FileColumnDescription>(),
            };

            description.DataGroups.Add(dataGroup);
            var reader = new StreamReader(file, description.Encoding);

            var findSection = false;
            var colNumber = 0;
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                if (string.IsNullOrEmpty(line))
                {
                    continue;
                }

                if (line.StartsWith("~A"))
                {
                    break;
                }

                if (line.StartsWith("~C"))
                {
                    findSection = true;
                }

                if (!findSection || line.StartsWith("#"))
                {
                    continue;
                }

                var pattern = ConfigureMetadataPatterns(line);
                var match = Regex.Match(line, pattern);
                if (match.Success)
                {
                    dataGroup.ColumnDescriptions.Add(new FileColumnDescription
                    {
                        Name = match.Groups["name"].Value.Trim(),
                        Position = colNumber++,
                        NeedImport = false,
                    });
                }
            }

            file.Position = 0;
            var dataReader = new LasByRowReader(file);
            var rowCount = 0;

            while (dataReader.Read())
            {
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
                    row.Values[colIdx] = dataReader.ReadString(colIdx, null);
                }

                dataGroup.RowsPreview.Add(row);
                rowCount++;
            }
        }

        private static string ConfigureMetadataPatterns(string line)
        {
            var nameRe = NameRe;
            var unitRe = UnitRe;
            var valueRe = ValueRe;
            var descRe = DescRe;
            if (line.Contains(":"))
            {
                var split = line.Split(':');
                if (!split[0].Contains("."))
                {
                    nameRe = NameMissingPeriodRe;
                    valueRe = ValueMissingPeriodRe;
                    descRe = string.Empty;
                    unitRe = string.Empty;
                }

                if (Regex.IsMatch(line, @"[^ ]\.\."))
                {
                    var doubleDot = line.IndexOf("..", StringComparison.Ordinal);
                    var descColon = line.IndexOf(":", StringComparison.Ordinal);

                    if (doubleDot < descColon)
                    {
                        nameRe = NameWithDotsRe;
                    }
                }
            }
            else
            {
                valueRe = ValueWithoutColonDelimiterRe;
                descRe = string.Empty;
                if (line.Contains(".."))
                {
                    nameRe = NameWithDotsRe;
                }
            }

            return nameRe + unitRe + valueRe + descRe;
        }
    }
}
