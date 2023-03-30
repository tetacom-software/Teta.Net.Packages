// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AwsS3Options.cs" company="TETA">
// Copyright (c) TETA. Ufa, 2022.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
// <summary>
//  Mapping profile for common DTO items
// </summary>

using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Transfer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System;
using Teta.Packages.Files.S3.Implementation;
using Teta.Packages.Files.S3.Interface;
using System.Security.Cryptography;

namespace Teta.Packages.Files.S3
{
    /// <summary>
    /// Расширения для работы с файловым хранилищем
    /// </summary>
    public static class S3FilesExtensions
    {
        /// <summary>
        /// Files s3 acccess extensions
        /// </summary>
        /// <param name="sc"></param>
        /// <param name="configuration">Application configuration</param>
        /// <returns></returns>
        public static IServiceCollection RegisterS3FilesStorage(this IServiceCollection sc, IConfiguration configuration)
        {
            var configSection = configuration.GetSection(nameof(S3Options));
            sc.AddOptions<S3Options>()
                .Bind(configSection);

            var s3Options = configSection.Get<S3Options>();

            var credentials = new BasicAWSCredentials(s3Options.AccessKey, s3Options.SecretKey);
            var config = new AmazonS3Config
            {
                RegionEndpoint = string.IsNullOrWhiteSpace(s3Options.Region) ? RegionEndpoint.EUCentral1
                : RegionEndpoint.GetBySystemName(s3Options.Region), // MUST set this before setting ServiceURL and it should match the `MINIO_REGION` environment variable.
                ServiceURL = s3Options.ServiceUrl, // replace http://localhost:9000 with URL of your MinIO server
                ForcePathStyle = s3Options.ForcePathStyle, // MUST be true to work correctly with MinIO server
            };

            sc.AddScoped<IAmazonS3>((sp) =>
            {
                return new AmazonS3Client(credentials, config);
            });

            sc.AddScoped<ITransferUtility>(sp =>
            {
                return new TransferUtility(sp.GetRequiredService<IAmazonS3>());
            });

            sc.AddScoped<IS3Service, AmazonBasedS3Service>();
            sc.Configure<S3Options>(configSection);

            return sc;
        }

        /// <summary>
        /// Computes based64 sha256 file checksum
        /// </summary>
        /// <param name="rawData"></param>
        /// <returns></returns>
        public static string ComputeSha256HashToBase64String(this Stream rawData)
        {
            // Create a SHA256   
            using var sha256Hash = SHA256.Create();
            using var memStream = new MemoryStream();

            rawData.CopyTo(memStream);

            // ComputeHash - returns byte array  
            byte[] bytes = sha256Hash.ComputeHash(memStream);

            rawData.Seek(0, SeekOrigin.Begin);
            return Convert.ToBase64String(bytes);
        }

        /// <summary>
        /// Computes based64 sha256 file checksum
        /// </summary>
        /// <param name="rawData"></param>
        /// <returns></returns>
        public static string ComputeMD5Hash(this Stream rawData)
        {
            // Create a SHA256   
            using var sha256Hash = MD5.Create();
            using var memStream = new MemoryStream();

            rawData.CopyTo(memStream);

            // ComputeHash - returns byte array  
            byte[] bytes = sha256Hash.ComputeHash(memStream);

            rawData.Seek(0, SeekOrigin.Begin);
            return Convert.ToHexString(bytes);
        }
    }
}
