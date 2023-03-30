// --------------------------------------------------------------------------------------------------------------------
// <copyright file="XlsxByRowAllSheetsReader.cs" company="TETA">
// Copyright (c) TETA. Ufa, 2022.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
// <summary>
// Класс для чтения всех строк со всех страниц Ексель
// </summary>

using OfficeOpenXml;
using Teta.Files.Contracts;
using Teta.Packages.Files.Bll.Interface;

namespace Teta.Packages.Files.Bll.Implementation.DataParser
{
    /// <summary>
    /// Класс для чтения всех строк со всех страниц Ексель
    /// </summary>
    public class XlsxByRowAllSheetsReader : IByRowFileReader
    {
        private readonly int _firstRow;
        private IByRowFileReader _reader;
        private ExcelPackage _package;
        private int _sheetsCount;
        private int _wsCounter = 1;

        /// <inheritdoc/>
        public int CurrentRowIdx => _reader.CurrentRowIdx;

        /// <summary>
        /// Название текущей страницы
        /// </summary>
        public string SheetName => _reader.SheetName;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="stream">Файлстрим</param>
        /// <param name="firstRow">Начальная строка для считывания</param>
        public XlsxByRowAllSheetsReader(Stream stream, int firstRow)
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            _firstRow = firstRow;
            _package = new ExcelPackage(stream);
            _sheetsCount = _package.Workbook.Worksheets.Count;

            if (_sheetsCount > 0)
            {
                var ws = _package.Workbook.Worksheets[0];
                _reader = new XlsxByRowReader(ws, _firstRow);
            }
            else
            {
                throw new ArgumentNullException(nameof(_sheetsCount));
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            _package.Dispose();
            _reader.Dispose();
        }

        /// <summary>
        /// Переопределяем метод чтения
        /// </summary>
        /// <returns>Если ещё не все строки прочтены, то возвращаем true</returns>
        public bool Read()
        {
            var readRes = _reader.Read();
            if (!readRes && _wsCounter < _sheetsCount)
            {
                var ws = _package.Workbook.Worksheets[_wsCounter];
                _reader = new XlsxByRowReader(ws, _firstRow);
                _wsCounter++;
                return Read();
            }
            else if (_wsCounter == _sheetsCount && !readRes)
            {
                return false;
            }

            return readRes;
        }

        /// <inheritdoc/>
        public DateTime ReadDateTime(int index, FileParseException baseEx)
        {
            return _reader.ReadDateTime(index, baseEx);
        }

        /// <inheritdoc/>
        public double ReadDouble(int index, FileParseException baseEx)
        {
            return _reader.ReadDouble(index, baseEx);
        }

        /// <inheritdoc/>
        public double? ReadDoubleNullOrDefault(int index, FileParseException baseEx)
        {
            return _reader.ReadDoubleNullOrDefault(index, baseEx);
        }

        /// <inheritdoc/>
        public string ReadString(int index, FileParseException baseEx)
        {
            return _reader.ReadString(index, baseEx);
        }

        /// <inheritdoc/>
        public string ReadText(int index, FileParseException baseEx)
        {
            return _reader.ReadText(index, baseEx);
        }

        /// <inheritdoc/>
        public TimeSpan ReadTimeFromDays(int index, FileParseException baseEx)
        {
            return _reader.ReadTimeFromDays(index, baseEx);
        }

        /// <inheritdoc/>
        public TimeSpan ReadTimeFromMinutes(int index, FileParseException baseEx)
        {
            return _reader.ReadTimeFromMinutes(index, baseEx);
        }

        /// <inheritdoc/>
        public T ReadValue<T>(int indexc, FileParseException baseEx)
        {
            return _reader.ReadValue<T>(indexc, baseEx);
        }

        /// <inheritdoc/>
        public object ReadValue(Type t, int indexc, FileParseException baseEx)
        {
            return _reader.ReadValue(t, indexc, baseEx);
        }

        /// <inheritdoc/>
        public Stream SaveChanges()
        {
            _package.Save();
            _package.Stream.Position = 0;
            return _package.Stream;
        }

        /// <inheritdoc/>
        public void AddParseException(int column, string msg, FileParseException baseEx)
        {
            _reader.AddParseException(column, msg, baseEx);
        }
    }
}
