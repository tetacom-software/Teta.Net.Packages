// --------------------------------------------------------------------------------------------------------------------
// <copyright file="XlsxByRowReader.cs" company="TETA">
// Copyright (c) TETA. Ufa, 2022.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
// <summary>
//  Xlsx by row reader
// </summary>

using System.Drawing;
using OfficeOpenXml;
using Teta.Files.Contracts;
using Teta.Packages.Files.Bll.Interface;

namespace Teta.Packages.Files.Bll.Implementation.DataParser
{
    /// <summary>
    /// Xlsx by row reader
    /// </summary>
    public class XlsxByRowReader : IByRowFileReader
    {
        private static readonly DateTime XlsxMinDate;

        private readonly ExcelPackage _epPackage;

        private readonly ExcelWorksheet _worksheet;

        private int _dataRowCount;

        private string _sheetName;

        static XlsxByRowReader()
        {
            XlsxMinDate = new DateTime(1899, 12, 31);
        }

        /// <summary>
        /// Читает построчно файлы Xlsx со всего документа
        /// </summary>
        /// <param name="workSheet">Лист документа</param>
        /// <param name="firstDataRow">Начальная строка данных</param>
        public XlsxByRowReader(ExcelWorksheet workSheet, int firstDataRow)
        {
            if (firstDataRow < 0)
            {
                throw new ArgumentNullException($"{nameof(firstDataRow)} can not be less then 0");
            }

            _worksheet = workSheet;
            _dataRowCount = firstDataRow;
            _sheetName = _worksheet.Name;
        }

        /// <summary>
        /// Читает по строчно файлы Xlsx
        /// </summary>
        /// <param name="fileStream">Поток чтения файла</param>
        /// <param name="firstDataRow">Строка данных</param>
        /// <param name="worksheetName">Имя листа с данными</param>
        public XlsxByRowReader(Stream fileStream, int firstDataRow, string worksheetName)
        {
            if (firstDataRow < 0)
            {
                throw new ArgumentNullException($"{nameof(firstDataRow)} can not be less then 0");
            }

            if (string.IsNullOrEmpty(worksheetName))
            {
                throw new ArgumentNullException(nameof(worksheetName));
            }

            if (fileStream == null)
            {
                throw new ArgumentNullException(nameof(fileStream));
            }

            _dataRowCount = firstDataRow;
            _epPackage = new ExcelPackage(fileStream);
            _worksheet = _epPackage.Workbook.Worksheets[worksheetName];
            _sheetName = worksheetName;
            if (_worksheet == null)
            {
                throw new KeyNotFoundException($"Не найден лист с именем: {worksheetName}");
            }
        }

        /// <summary>
        /// Читает построчно файлы Xlsx
        /// </summary>
        /// <param name="fileStream">Поток файла</param>
        /// <param name="firstDataRow">Строка, с которой начинаются данные</param>
        /// <param name="worksheetNum">Порядковый номер листа с данными (Отрицательный - с конца)</param>
        public XlsxByRowReader(Stream fileStream, int firstDataRow, int worksheetNum)
        {
            if (firstDataRow < 0)
            {
                throw new ArgumentNullException($"{nameof(firstDataRow)} can not be less then 0");
            }

            if (fileStream == null)
            {
                throw new ArgumentNullException(nameof(fileStream));
            }

            if (worksheetNum < 0)
            {
                worksheetNum = _epPackage.Workbook.Worksheets.Count() + worksheetNum;
            }

            _dataRowCount = firstDataRow;
            _epPackage = new ExcelPackage(fileStream);
            _worksheet = _epPackage.Workbook.Worksheets[worksheetNum];
            _sheetName = _worksheet.Name;

            if (_worksheet == null)
            {
                throw new KeyNotFoundException($"Не найден лист под номером {worksheetNum}");
            }
        }

        /// <summary>Advances the <see cref="T:System.Data.IDataReader" /> to the next record.</summary>
        /// <returns>
        /// <see langword="true" /> if there are more rows; otherwise, <see langword="false" />.</returns>
        public bool Read()
        {
            _dataRowCount++;
            return _worksheet.Dimension.Rows >= _dataRowCount;
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
                return _worksheet.GetValue<string>(_dataRowCount, index + 1);
            }
            catch (Exception)
            {
                AddParseException(index, "Can not get string value", baseEx);
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
                /*return _worksheet.GetValue<double>(_dataRowCount, index + 1);*/
                var depthString = _worksheet.GetValue(_dataRowCount, index + 1)?.ToString();
                if (string.IsNullOrEmpty(depthString) || !double.TryParse(depthString.Replace('.', ','), out var dVal))
                {
                    return default;
                }

                return dVal;
            }
            catch (Exception)
            {
                AddParseException(index, "Can not get double value", baseEx);
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
            try
            {
                var depthString = _worksheet.GetValue(_dataRowCount, index + 1)?.ToString();
                if (string.IsNullOrEmpty(depthString) || !double.TryParse(depthString.Replace('.', ','), out var dVal))
                {
                    return default;
                }

                return dVal;
            }
            catch (Exception)
            {
                AddParseException(index, "Can not get double value", baseEx);
                return default;
            }
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
                var serialDate = _worksheet.GetValue<double>(_dataRowCount, index + 1);
                if (serialDate > 59)
                {
                    serialDate -= 1; // Excel/Lotus 2/29/1900 bug
                }

                var resp = XlsxMinDate.AddDays(serialDate);
                return resp == XlsxMinDate ? DateTime.MinValue : resp;
            }
            catch (InvalidCastException)
            {
                return _worksheet.GetValue<DateTime>(_dataRowCount, index + 1);
            }
            catch
            {
                AddParseException(index, "Can not get DateTime value", baseEx);
                return default;
            }
        }

        /// <summary>
        /// Прочитать данные из столбца при ошибке записать её в сообщение
        /// </summary>
        /// <param name="index">Индекс столбца</param>
        /// <param name="baseEx">Exception class</param>
        /// <returns>Время</returns>
        public TimeSpan ReadTimeSpan(int index, FileParseException baseEx)
        {
            try
            {
                var data = _worksheet.Cells[_dataRowCount, index + 1];
                if (string.IsNullOrEmpty(data.Text))
                {
                    return default;
                }

                var serialTime = _worksheet.GetValue<double>(_dataRowCount, index + 1);
                var timeSeparator = new char[] { ':', '/', '-' };
                if (data.Text.Any(x => timeSeparator.Contains(x)))
                {
                    return TimeSpan.FromDays(serialTime);
                }

                return TimeSpan.FromMinutes(serialTime);
            }
            catch
            {
                AddParseException(index, "Can not get TimeSpan value", baseEx);
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
            try
            {
                var data = _worksheet.Cells[_dataRowCount, index + 1];
                return data.Text;
            }
            catch
            {
                AddParseException(index, "Can not get text from cell", baseEx);
                return default;
            }
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
                var serialTime = _worksheet.GetValue<double>(_dataRowCount, index + 1);
                return TimeSpan.FromDays(serialTime);
            }
            catch
            {
                AddParseException(index, "Can not get TimeSpan value", baseEx);
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
                var serialTime = _worksheet.GetValue<double>(_dataRowCount, index + 1);
                return TimeSpan.FromMinutes(serialTime);
            }
            catch
            {
                var msg = "Can not get TimeSpan value";
                AddParseException(index, msg, baseEx);
                return default;
            }
        }

        /// <summary>
        /// Индекс текущей строки
        /// </summary>
        public int CurrentRowIdx => _dataRowCount;

        /// <inheritdoc/>
        public string SheetName => _sheetName;

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            _epPackage?.Dispose();
        }

        /// <inheritdoc/>
        public T ReadValue<T>(int indexc, FileParseException baseEx)
        {
            return (T)ReadValue(typeof(T), indexc, baseEx);
        }

        /// <inheritdoc/>
        public object ReadValue(Type propType, int indexc, FileParseException baseEx)
        {
            var nullableType = Nullable.GetUnderlyingType(propType);
            if (nullableType != null)
            {
                propType = nullableType;
            }

            var typeCode = Type.GetTypeCode(propType);
            var stringValue = ReadString(indexc, baseEx);
            if (string.IsNullOrEmpty(stringValue) && nullableType != null)
            {
                return null;
            }

            switch (typeCode)
            {
                case TypeCode.Int32:
                    return Convert.ToInt32(ReadDouble(indexc, baseEx));

                case TypeCode.String:
                    return stringValue;

                case TypeCode.Double:
                    return ReadDouble(indexc, baseEx);

                case TypeCode.DateTime:
                    {
                        DateTime cellVal;
                        if (!DateTime.TryParse(stringValue, out cellVal))
                        {
                            cellVal = ReadDateTime(indexc, baseEx);
                        }

                        return cellVal;
                    }

                default: throw new NotSupportedException($"type {typeCode} not supported");
            }
        }

        /// <inheritdoc/>
        public Stream SaveChanges()
        {
            _epPackage.Save();
            _epPackage.Stream.Position = 0;
            return _epPackage.Stream;
        }

        /// <inheritdoc/>
        public void AddParseException(int column, string msg, FileParseException baseEx)
        {
            baseEx.AddParseException(_dataRowCount, column, msg);
            SetCellMark(column, msg);
        }

        private void SetCellMark(int column, string msg, KnownColor color = KnownColor.Yellow)
        {
            column++;
            _worksheet.Cells[CurrentRowIdx, column].Style.Fill.SetBackground(Color.FromKnownColor(color));
            _worksheet.Cells[CurrentRowIdx, column].AddComment(msg);
        }
    }
}
