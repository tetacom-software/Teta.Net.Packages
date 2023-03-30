// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CsvByRowReader.cs" company="TETA">
// Copyright (c) TETA. Ufa, 2022.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
// <summary>
//  Csv by row reader
// </summary>

using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using Teta.Files.Contracts;
using Teta.Packages.Files.Bll.Interface;

namespace Teta.Packages.Files.Bll.Implementation.DataParser
{
    /// <summary>
    /// Csv by row reader
    /// </summary>
    public class CsvByRowReader : IByRowFileReader
    {
        private readonly CsvReader _csvReader;
        private readonly CsvDoubleTypeConverter _doubleTypeConverter = new();

        /// <summary>
        /// Читает по строчно файлы Csv
        /// </summary>
        /// <param name="fileStream">Поток чтения файла</param>
        /// <param name="delimiter">Разделитель</param>
        /// <param name="firstDataRow">Строка данных</param>
        public CsvByRowReader(Stream fileStream, string delimiter, int? firstDataRow)
        {
            if (fileStream == null)
            {
                throw new ArgumentNullException(nameof(fileStream));
            }

            var encoding = FilesExtensions.GetEncoding(fileStream);
            var reader = new StreamReader(fileStream);
            var csvOption = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = delimiter,
                IgnoreBlankLines = true,
                Encoding = encoding,
            };

            _csvReader = new CsvReader(reader, csvOption);
            if (!firstDataRow.HasValue)
            {
                throw new Exception("First data row required");
            }

            /*
            // Считывание строк должно происходить по индексу
            // Пользователь вводит НОМЕР строки, по этому от номера отнимаем 1 чтобы получить индекс.
            firstDataRow = firstDataRow.Value == 0 ? 0 : firstDataRow.Value - 1;
            */

            // Промотать ридер
            for (var i = 0; i < firstDataRow; i++)
            {
                if (!_csvReader.Read())
                {
                    break;
                }
            }

            var formats = new[]
            {
                "dd.MM.yyyy h:mm:ss",
                "dd.MM.yyyy h:mm",
                "dd.MM.yyyy HH:mm",
                "dd.MM.yyyy hh:mm:ss",
                "dd.MM.yyyy",
                "dd.MM.yy h:mm:ss",
                "dd.MM.yy h:mm",
                "dd.MM.yy HH:mm",
                "dd.MM.yy hh:mm:ss",
                "dd.MM.yy",
                "dd/MM/yyyy h:mm:ss",
                "dd/MM/yyyy h:mm",
                "dd/MM/yyyy HH:mm",
                "dd/MM/yyyy hh:mm:ss",
                "dd/MM/yyyy",
                "dd/MM/yy h:mm:ss",
                "dd/MM/yy h:mm",
                "dd/MM/yy HH:mm",
                "dd/MM/yy hh:mm:ss",
                "dd/MM/yy",
            };

            var options = new TypeConverterOptions { Formats = formats };
            _csvReader.Context.TypeConverterOptionsCache.AddOptions<DateTime>(options);
        }

        /// <summary>Получить следующую строку</summary>
        /// <returns>
        /// <see langword="true" /> if there are more rows; otherwise, <see langword="false" />.</returns>
        public bool Read()
        {
            return _csvReader.Read();
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
                return _csvReader.GetField(index);
            }
            catch (Exception)
            {
                baseEx?.AddParseException(_csvReader.CurrentIndex, index, "Can not get string value");
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
                if (_csvReader.TryGetField<double>(index, _doubleTypeConverter, out var fld))
                {
                    return fld;
                }

                throw new Exception();
            }
            catch (Exception)
            {
                baseEx.AddParseException(_csvReader.CurrentIndex, index, "Can not get double value");
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
            if (!_csvReader.TryGetField<double>(index, _doubleTypeConverter, out var doubleVal))
            {
                return null;
            }

            return doubleVal;
        }

        /// <summary>
        /// Прочитать данные из столбца при ошибке записать её в сообщение
        /// </summary>
        /// <param name="index">Индекс столбца</param>
        /// <param name="baseEx">Exception class</param>
        /// <returns>Дробное значение</returns>
        public DateTime ReadDateTime(int index, FileParseException baseEx)
        {
            try
            {
                return _csvReader.GetField<DateTime>(index);
            }
            catch
            {
                baseEx.AddParseException(_csvReader.CurrentIndex, index, "Can not get DateTime value");
                return default;
            }
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
                var serialTime = ReadDouble(index, baseEx);
                return TimeSpan.FromDays(serialTime);
            }
            catch
            {
                baseEx.AddParseException(_csvReader.CurrentIndex, index, "Can not get TimeSpan value");
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
                var serialTime = ReadDouble(index, baseEx);
                return TimeSpan.FromMinutes(serialTime);
            }
            catch
            {
                baseEx.AddParseException(_csvReader.CurrentIndex, index, "Can not get TimeSpan value");
                return default;
            }
        }

        /// <summary>
        /// Индекс текущей строки
        /// </summary>
        public int CurrentRowIdx => _csvReader.Parser.Row;

        /// <inheritdoc/>
        public string SheetName => throw new NotImplementedException();

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            _csvReader.Dispose();
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
    }
}