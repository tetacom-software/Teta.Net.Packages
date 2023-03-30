
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileParseException.cs" company="TETA">
// Copyright (c) TETA. Ufa, 2023.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
// <summary>
// Пользовательский файл с данными
// </summary>

using System;
using System.Collections.Generic;
using System.Text;

namespace Teta.Packages.Files.Contracts
{
    /// <summary>
    /// Ошибка парсинга файла
    /// </summary>
    public class FileParseException : Exception
    {
        private readonly IList<(int row, int col, string message)> _errors;

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="FileParseException"/>.
        /// </summary>
        /// <param name="baseMsg">Основное сообщение об ошибке</param>
        public FileParseException(string baseMsg)
        : base(baseMsg)
        {
            _errors = new List<(int row, int col, string message)>();
        }

        /// <summary>
        /// Имеются ошибки
        /// </summary>
        public bool HasErrors => _errors.Count != 0;

        /// <summary>
        /// Ошибки разбора файла
        /// </summary>
        /// <param name="row">Строка</param>
        /// <param name="col">Столбец</param>
        /// <param name="msg">Сообщение</param>
        public void AddParseException(int row, int col, string msg)
        {
            _errors.Add((row, col, msg));
        }

        /// <summary>Creates and returns a string representation of the current exception.</summary>
        /// <returns>A string representation of the current exception.</returns>
        public override string ToString()
        {
            var sb = new StringBuilder(Message);
            foreach (var valueTuple in _errors)
            {
                sb.AppendLine($"[{valueTuple.row}:{valueTuple.col}]: {valueTuple.message}");
            }

            sb.AppendLine(StackTrace);

            return sb.ToString();
        }
    }
}
