// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CsvDoubleTypeConverter.cs" company="TETA">
// Copyright (c) TETA. Ufa, 2023.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
// <summary>
//  Конвертер типов
// </summary>

using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;

namespace Teta.Packages.Files.Bll
{
    /// <summary>
    /// Конвертер типов
    /// </summary>
    public class CsvDoubleTypeConverter : DefaultTypeConverter
    {
        /// <inheritdoc />
        public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
        {
            var item = text.Replace(",", ".");
            if (string.IsNullOrWhiteSpace(item))
            {
                return 0;
            }

            if (double.TryParse(item, NumberStyles.Number, CultureInfo.InvariantCulture, out var value))
            {
                return value;
            }

            return base.ConvertFromString(text, row, memberMapData);
        }
    }
}
