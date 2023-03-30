// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AwsS3Options.cs" company="TETA">
// Copyright (c) TETA. Ufa, 2022.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
// <summary>
//  Mapping profile for common DTO items
// </summary>

using System.ComponentModel.DataAnnotations;

namespace Teta.Packages.Files.S3
{
    /// <summary>
    /// Amazon S3 file server access options
    /// </summary>
    public class S3Options
    {
        /// <summary>
        /// Acces key
        /// </summary>
        [Required]
        public string AccessKey { get; set; }

        /// <summary>
        /// Secret key
        /// </summary>
        [Required]
        public string SecretKey { get; set; }

        /// <summary>
        /// AmazonS3 Api service URL
        /// </summary>
        [Required]
        public string ServiceUrl { get; set; }

        /// <summary>
        /// Bucket name
        /// </summary>
        [Required]
        public string DefaultBucketName { get; set; }

        /// <summary>
        /// Bucket region
        /// </summary>
        public string Region { get; set; }

        /// <summary>
        /// Use path style addressing (compatibility with minio and etc)
        /// </summary>
        public bool ForcePathStyle { get; set; } = true;
    }
}
