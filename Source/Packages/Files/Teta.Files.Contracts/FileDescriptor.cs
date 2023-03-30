// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ObjectMetadata.cs" company="TETA">
// Copyright (c) TETA. Ufa, 2023.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
// <summary>
// Represents response for stored file
// </summary>

using System;

namespace Teta.Packages.Files.Contracts
{
    /// <summary>
    /// Represents response for stored file
    /// </summary>
    public class FileDescriptor
    {
        /// <summary>
        /// File hash
        /// </summary>
        public string FileKey { get; set; }

        /// <summary>
        /// File name
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Minio file tag
        /// </summary>
        public string ETag { get; set; }

        /// <summary>
        /// Размер файла в байтах
        /// </summary>
        public long FileSizeBytes { get; set; }

        /// <summary>
        /// Expires
        /// </summary>
        public DateTime? ExpiryDateUtc { get; set; }

        /// <summary>
        /// Upload date
        /// </summary>
        public DateTime UploadDate { get; set; }

        /// <summary>
        /// Owner user mail
        /// </summary>
        public string OwnerUserMail { get; set; }
    }
}
