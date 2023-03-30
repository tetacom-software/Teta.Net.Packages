// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LasByRowReader.cs" company="TETA">
// Copyright (c) TETA. Ufa, 2022.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
// <summary>
//  Las by row reader
// </summary>

using System.Globalization;
using System.Text.RegularExpressions;
using Teta.Files.Contracts;
using Teta.Packages.Files.Bll.Interface;

namespace Teta.Packages.Files.Bll.Implementation.DataParser
{
    /// <summary>
    /// Las by row reader
    /// </summary>
    public class LasByRowReader : IByRowFileReader
    {
        private readonly StreamReader _streamReader;
        private readonly List<LasReplaceGroup> _replaceGroups = new()
        {
            new LasReplaceGroup(@"(\d),(\d)", "$1.$2"),
            new LasReplaceGroup(@"(\d)-(\d)", "$1 -$2"),
            new LasReplaceGroup(@"-?\d*\.\d*\.\d*", " NaN NaN "),
            new LasReplaceGroup(@"NaN[\.-]\d+", " NaN NaN "),
            /*new(@" \(null\)", " NaN"),
            new(@"\(null\) ", "NaN "),
            new(@" \(NULL\)", " NaN"),
            new(@"\(NULL\) ", "NaN "),
            new(@" null", " NaN"),
            new(@"null ", "NaN "),
            new(@" NULL", " NaN"),
            new(@"NULL ", "NaN "),
            new(@" -+ ", " NaN "),
            new(@"(#N/A)[ ]", "NaN "),
            new(@"[ ](#N/A)", " NaN"),
            new(@"(-?1\.#INF)[ ]", "NaN "),
            new(@"[ ](-?1\.#INF[0-9]*)", " NaN"),
            new(@"(-?1\.#IO)[ ]", "NaN "),
            new(@"[ ](-?1\.#IO)", " NaN"),
            new(@"(-?1\.#IND)[ ]", "NaN "),
            new(@"[ ](-?1\.#IND[0-9]*)", " NaN"),
            new(@"(-0\.0)[ ]", "NaN "),
            new(@"[ ](-0\.0)", " NaN"),
            new(@"([^ 0-9.\-+]+)[ ]", "NaN "),
            new(@"[ ]([^ 0-9.\-+]+)", " NaN"),*/
        };

        private readonly double[] _nullSubs =
        {
            -9999.99,
            -9999.25,
            9999.25,
            -999.25,
            999.25,
            -999,
            /*999,*/
            -999.99,
            999.99,
            -9999,
            9999,
            -32767,
            32767,
        };

        private string[] _currentRow;
        private int _dataRowCount;

        private class LasReplaceGroup
        {
            public LasReplaceGroup(string pattern, string replacement)
            {
                Pattern = new Regex(pattern, RegexOptions.Compiled);
                Replacement = replacement;
            }

            public Regex Pattern { get; }

            public string Replacement { get; }
        }

        /// <summary>
        /// Читает по строчно файлы Xlsx
        /// </summary>
        /// <param name="fileStream">Поток чтения файла</param>
        public LasByRowReader(Stream fileStream)
        {
            if (fileStream == null)
            {
                throw new ArgumentNullException(nameof(fileStream));
            }

            var encoding = FilesExtensions.GetEncoding(fileStream);
            _streamReader = new StreamReader(fileStream, encoding);
            ChangePointToLasData();
        }

        /// <summary>Advances the <see cref="T:System.Data.IDataReader" /> to the next record.</summary>
        /// <returns>
        /// <see langword="true" /> if there are more rows; otherwise, <see langword="false" />.</returns>
        public bool Read()
        {
            if (_streamReader.EndOfStream)
            {
                return false;
            }

            var line = ReadLine();
            if (string.IsNullOrEmpty(line))
            {
                return true;
            }

            foreach (var gr in _replaceGroups)
            {
                line = gr.Pattern.Replace(line, gr.Replacement);
            }

            _currentRow = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            return true;
        }

        /// <summary>
        /// Прочитать данные из столбца при ошибке записать её в сообщение
        /// </summary>
        /// <param name="index">Индекс столбца</param>
        /// <param name="baseEx">Exception class</param>
        /// <returns>Строковое значение</returns>
        public string ReadString(int index, FileParseException baseEx)
        {
            try
            {
                var result = _currentRow[index];
                if (result.Contains('.'))
                {
                    result = result.TrimEnd('0').TrimEnd('.');
                }

                return _nullSubs.Any(x => x.ToString(CultureInfo.InvariantCulture) == result) ? string.Empty : result;
            }
            catch (Exception)
            {
                baseEx?.AddParseException(_dataRowCount, index, "Can not get double value");
                return default;
            }
        }

        /// <summary>
        /// Прочитать данные из столбца при ошибке записать её в сообщение
        /// </summary>
        /// <param name="index">Индекс столбца</param>
        /// <param name="baseEx">Exception class</param>
        /// <returns>Дробное значение</returns>
        public double ReadDouble(int index, FileParseException baseEx)
        {
            try
            {
                return GetDouble(index);
            }
            catch (Exception)
            {
                baseEx.AddParseException(_dataRowCount, index, "Can not get double value");
                return default;
            }
        }

        /// <summary>
        /// Прочитать nullable double
        /// </summary>
        /// <param name="index">Индекс</param>
        /// <param name="baseEx">Ошибка</param>
        /// <returns>Nullable double</returns>
        public double? ReadDoubleNullOrDefault(int index, FileParseException baseEx)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Прочитать данные из столбца при ошибке записать её в сообщение
        /// </summary>
        /// <param name="index">Индекс столбца</param>
        /// <param name="baseEx">Exception class</param>
        /// <returns>Дробное значение</returns>
        public DateTime ReadDateTime(int index, FileParseException baseEx)
        {
            throw new NotSupportedException("Can't get DateTime value for LAS file");
        }

        /// <summary>
        /// Прочитать данные из столбца при ошибке записать её в сообщение
        /// </summary>
        /// <param name="index">Индекс столбца</param>
        /// <param name="baseEx">Exception class</param>
        /// <returns>Текст ячейки</returns>
        public string ReadText(int index, FileParseException baseEx)
        {
            return ReadString(index, baseEx);
        }

        /// <summary>
        /// Прочитать данные из столбца при ошибке записать её в сообщение
        /// </summary>
        /// <param name="index">Индекс столбца</param>
        /// <param name="baseEx">Exception class</param>
        /// <returns>Время полученное из дня</returns>
        public TimeSpan ReadTimeFromDays(int index, FileParseException baseEx)
        {
            try
            {
                var serialTime = GetDouble(index);
                return TimeSpan.FromDays(serialTime);
            }
            catch
            {
                baseEx.AddParseException(_dataRowCount, index, "Can not get TimeSpan value");
                return default;
            }
        }

        /// <summary>
        /// Прочитать данные из столбца при ошибке записать её в сообщение
        /// </summary>
        /// <param name="index">Индекс столбца</param>
        /// <param name="baseEx">Exception class</param>
        /// <returns>Время полученное из минут</returns>
        public TimeSpan ReadTimeFromMinutes(int index, FileParseException baseEx)
        {
            try
            {
                var serialTime = GetDouble(index);
                return TimeSpan.FromMinutes(serialTime);
            }
            catch
            {
                baseEx.AddParseException(_dataRowCount, index, "Can not get TimeSpan value");
                return default;
            }
        }

        /// <summary>
        /// Индекс текущей строки
        /// </summary>
        public int CurrentRowIdx => _dataRowCount;

        /// <inheritdoc/>
        public string SheetName => throw new NotImplementedException();

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            _streamReader.Dispose();
        }

        /// <inheritdoc/>
        public T ReadValue<T>(int indexc, FileParseException baseEx)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public object ReadValue(Type t, int indexc, FileParseException baseEx)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Stream SaveChanges()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void AddParseException(int column, string msg, FileParseException baseEx)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Смещаем курсор на первую строку с данными
        /// </summary>
        private void ChangePointToLasData()
        {
            var line = ReadLine();
            while (!_streamReader.EndOfStream)
            {
                if (!string.IsNullOrEmpty(line) && line.StartsWith("~A"))
                {
                    break;
                }

                line = ReadLine();
            }
        }

        private T GetValue<T>(int index)
        {
            if (_currentRow.Length <= index)
            {
                return default;
            }

            var item = _currentRow[index];
            return (T)Convert.ChangeType(item, typeof(T), CultureInfo.InvariantCulture);
        }

        private double GetDouble(int index)
        {
            if (_currentRow.Length <= index)
            {
                return default;
            }

            var item = _currentRow[index];
            var value = FilesExtensions.GetDouble(item);

            return _nullSubs.Any(x => Math.Abs(x - value) < double.Epsilon) ? default : value;
        }

        private string ReadLine()
        {
            _dataRowCount++;
            return _streamReader.ReadLine();
        }
    }
}