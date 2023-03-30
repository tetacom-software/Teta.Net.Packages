using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using Teta.Files.Contracts.Internal;
using Teta.Packages.Extensions;
using Teta.Packages.Files.Bll.Implementation;
using Teta.Packages.Files.Bll.Interface;
using Teta.Packages.Files.S3;
using UtfUnknown;

namespace Teta.Packages.Files.Bll
{
    /// <summary>
    /// Adds business logic for 
    /// </summary>
    public static class FilesExtensions
    {
        /// <summary>
        /// Limiting preview columns count
        /// </summary>
        public const int ColumnsLimit = 50;

        private static readonly Dictionary<string, FileType> FileTypesExtensions;

        static FilesExtensions()
        {
            var fileTypeType = typeof(FileType);
            FileTypesExtensions = Enum.GetValues(typeof(FileType)).Cast<FileType>()
                .Select(val => new
                {
                    val,
                    resourceLocale = val.GetEnumDescription() ?? Enum.GetName(fileTypeType, val),
                })
                .ToDictionary(k => k.resourceLocale.ToLower(), k => k.val);
        }

        /// <summary>
        /// Register business 
        /// </summary>
        /// <param name="sc">Service collection</param>
        /// <param name="configuration">Application configuration</param>
        /// <returns></returns>
        public static IServiceCollection RegisterFilesBll(this IServiceCollection sc, IConfiguration configuration)
        {
            if (configuration is null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            sc.RegisterS3FilesStorage(configuration);
            sc.AddScoped<IFileService, FileService>()
                .AddScoped<IFileDescriptionReaderFactory, FileDescriptionReaderFactory>();

            return sc;
        }

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
        /// Check encoding using encoding special chars and line endings chars codes
        /// </summary>
        /// <param name="fileStream">File stream</param>
        /// <returns>Encoding</returns>
        public static Encoding GetEncoding2(Stream fileStream)
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

        /// <summary>
        /// Преобразовать строку в дробное число
        /// </summary>
        /// <param name="val">Строка</param>
        /// <returns>Дробное значение</returns>
        public static double GetDouble(string val)
        {
            if (string.IsNullOrEmpty(val))
            {
                return default;
            }

            if (!double.TryParse(val.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out var dVal))
            {
                return default;
            }

            return dVal;
        }
    }
}
