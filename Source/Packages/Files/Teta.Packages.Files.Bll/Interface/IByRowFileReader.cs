// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IByRowFileReader.cs" company="TETA">
// Copyright (c) TETA. Ufa, 2023.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
// <summary>
// Интерфейс, выполняющий чтение класса по строчно, исходя из его типа
// </summary>

using System;
using System.IO;
using Teta.Files.Contracts;

namespace Teta.Packages.Files.Bll.Interface
{
    /// <summary>
    /// Интерфейс, выполняющий чтение класса по строчно, исходя из его типа
    /// </summary>
    public interface IByRowFileReader : IDisposable
    {
        /// <summary>Получить следующую строку</summary>
        /// <returns>
        /// <see langword="true" /> if there are more rows; otherwise, <see langword="false" />.</returns>
        bool Read();

        /// <summary>
        /// Прочитать данные из столбца при ошибке записать её в сообщение
        /// </summary>
        /// <param name="index">Индекс столбца</param>
        /// <param name="baseEx">Exception class</param>
        /// <returns>Строковое значение</returns>
        string ReadString(int index, FileParseException baseEx);

        /// <summary>
        /// Прочитать данные из столбца при ошибке записать её в сообщение
        /// </summary>
        /// <param name="index">Индекс столбца</param>
        /// <param name="baseEx">Exception class</param>
        /// <returns>Дробное значение</returns>
        double ReadDouble(int index, FileParseException baseEx);

        /// <summary>
        /// Прочитать nullable double
        /// </summary>
        /// <param name="index">Индекс</param>
        /// <param name="baseEx">Ошибка</param>
        /// <returns>Nullable double</returns>
        double? ReadDoubleNullOrDefault(int index, FileParseException baseEx);

        /// <summary>
        /// Прочитать данные из столбца при ошибке записать её в сообщение
        /// </summary>
        /// <param name="index">Индекс столбца</param>
        /// <param name="baseEx">Exception class</param>
        /// <returns>Дата</returns>
        DateTime ReadDateTime(int index, FileParseException baseEx);

        /// <summary>
        /// Прочитать данные из столбца при ошибке записать её в сообщение
        /// </summary>
        /// <param name="index">Индекс столбца</param>
        /// <param name="baseEx">Exception class</param>
        /// <returns>Текст ячейки</returns>
        string ReadText(int index, FileParseException baseEx);

        /// <summary>
        /// Прочитать данные из столбца при ошибке записать её в сообщение
        /// </summary>
        /// <param name="index">Индекс столбца</param>
        /// <param name="baseEx">Exception class</param>
        /// <returns>Время полученное из дня</returns>
        TimeSpan ReadTimeFromDays(int index, FileParseException baseEx);

        /// <summary>
        /// Прочитать данные из столбца при ошибке записать её в сообщение
        /// </summary>
        /// <param name="index">Индекс столбца</param>
        /// <param name="baseEx">Exception class</param>
        /// <returns>Время полученное из минут</returns>
        TimeSpan ReadTimeFromMinutes(int index, FileParseException baseEx);

        /// <summary>
        /// Get generic value
        /// </summary>
        /// <typeparam name="T">value type</typeparam>
        /// <param name="indexc">index</param>
        /// <param name="baseEx">Ex parser</param>
        /// <returns>value</returns>
        public T ReadValue<T>(int indexc, FileParseException baseEx);

        /// <summary>
        /// Get generic value
        /// </summary>
        /// <param name="t">type</param>
        /// <param name="indexc">index</param>
        /// <param name="baseEx">Ex parser</param>
        /// <returns>value</returns>
        public object ReadValue(Type t, int indexc, FileParseException baseEx);

        /// <summary>
        /// Метод обёртка для записи ошибки в объект и заливки ячейки цветом
        /// </summary>
        /// <param name="column">Номер колонки</param>
        /// <param name="msg">Текст сообщения об ошибке</param>
        /// <param name="baseEx">Экземпляр объекта с ошибками</param>
        public void AddParseException(int column, string msg, FileParseException baseEx);

        /// <summary>
        /// Save changes of excel book
        /// </summary>
        /// <returns>Book stream</returns>
        public Stream SaveChanges();

        /// <summary>
        /// Индекс текущей строки
        /// </summary>
        public int CurrentRowIdx { get; }

        /// <summary>
        /// Индекс текущей строки
        /// </summary>
        public string SheetName { get; }
    }
}
