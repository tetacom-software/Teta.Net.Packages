using System;

namespace Teta.Packages.Files.Contracts.Internal.S3
{
    /// <summary>
    /// Main S3 object properties
    /// </summary>
    public abstract class S3ObjectMainProperties
    {
        /// <summary>
        /// File Etag e.g. unique hash based on checksum
        /// </summary>
        public string Etag { get; set; }

        /// <summary>
        /// File expire date, if null file will be stored forever
        /// </summary>
        public DateTime? ExpirationUtc { get; set; }

        /// <summary>
        /// File key
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// File version
        /// </summary>
        public string VersionId { get; set; }

        /// <summary>
        /// Размер файла в байтах
        /// </summary>
        public long ContentLengthBytes { get; set; }

        /// <summary>
        /// File name
        /// </summary>
        public string FileName { get; set; }
    }
}
