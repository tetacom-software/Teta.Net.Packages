// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Support.cs" company="TETA">
// Copyright (c) TETA. Ufa, 2022.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
// <summary>
//  Методы для помощи в обработке файлов
// </summary>

using System.Globalization;
using System.Text;
using UtfUnknown;

namespace Teta.Packages.Files.Bll
{
    /// <summary>
    /// Методы для помощи в обработке файлов
    /// </summary>
    public static class Support
    {
        /// <summary>
        /// Количество столбцов для предпросмотра
        /// </summary>
        public const int ColumnsLimit = 50;

        /// <summary>
        /// Количество строк для предпросмотра
        /// </summary>
        public const int RowsLimit = 500;

        /// <summary>
        /// Determines a text file's encoding by analyzing its byte order mark (BOM).
        /// Defaults to ASCII when detection of the text file's endianness fails.
        /// </summary>
        /// <param name="file">The text file to analyze.</param>
        /// <returns>The detected encoding.</returns>
        public static Encoding GetEncoding(Stream file)
        {
            var result = CharsetDetector.DetectFromStream(file);
            file.Position = 0;

            Encoding resultEncoding;
            if (result.Detected == null)
            {
                resultEncoding = GetEncoding2(file);
                file.Position = 0;
            }
            else
            {
                resultEncoding = result.Detected.Encoding;
            }

            return resultEncoding;
        }

        /// <summary>
        /// Преобразовать строку в дробное число
        /// </summary>
        /// <param name="val">Строка</param>
        /// <returns>Дробное значение</returns>
        public static double GetDouble(string val)
        {
            var item = val.Replace(",", ".");
            return !double.TryParse(item, NumberStyles.Number, CultureInfo.InvariantCulture, out var value) ? default : value;
        }

        private static Encoding GetEncoding2(Stream fileStream)
        {
            // Read the BOM
            var bom = new byte[4];
            fileStream.Read(bom, 0, 4);

            // Analyze the BOM
            if (bom[0] == 0x2b && bom[1] == 0x2f && bom[2] == 0x76)
            {
                return Encoding.UTF7;
            }

            if (bom[0] == 0xef && bom[1] == 0xbb && bom[2] == 0xbf)
            {
                return Encoding.UTF8;
            }

            if (bom[0] == 0xff && bom[1] == 0xfe && bom[2] == 0 && bom[3] == 0)
            {
                return Encoding.UTF32;
            }

            if (bom[0] == 0xff && bom[1] == 0xfe)
            {
                return Encoding.Unicode;
            }

            if (bom[0] == 0xfe && bom[1] == 0xff)
            {
                return Encoding.BigEndianUnicode;
            }

            if (bom[0] == 0 && bom[1] == 0 && bom[2] == 0xfe && bom[3] == 0xff)
            {
                return new UTF32Encoding(true, true);
            }

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            var enc1251 = Encoding.GetEncoding("windows-1251");
            return enc1251;
        }
    }
}
