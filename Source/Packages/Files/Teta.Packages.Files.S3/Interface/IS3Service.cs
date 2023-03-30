// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IS3Service.cs" company="TETA">
// Copyright (c) TETA. Ufa, 2023.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
// <summary>
// Implements S3 access
// </summary>

using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using System;
using System.IO;
using Teta.Packages.Files.Contracts.Internal.S3;
using System.Collections.Generic;

namespace Teta.Packages.Files.S3.Interface
{
    /// <summary>
    /// Implements S3 access
    /// </summary>
    public interface IS3Service
    {
        /// <summary>
        /// Creates folder in bucket by name
        /// </summary>
        /// <param name="folderName">Folder name</param>
        /// <param name="bucketName">Bucket name</param>
        /// <returns></returns>
        Task CreateFolderAsync([NotNull] string objectName, [AllowNull] string? bucketName = null);


        /// <summary>
        /// Checks that folder already exists in bucket
        /// </summary>
        /// <param name="objectName">Folder name</param>
        /// <param name="bucketName">Bucket</param>
        /// <returns>true if folder exists</returns>
        Task<bool> CheckFolderExistsAsync([NotNull] string objectName, [AllowNull] string? bucketName = null);

        /// <summary>
        /// Check bucket exists
        /// </summary>
        /// <param name="bucketName">Bucket</param>
        /// <returns></returns>
        Task<bool> CheckBucketExistsAsync([NotNull]string bucketName = null);

        /// <summary>
        /// Checks that object exits in S3
        /// </summary>
        /// <param name="objectKey">Object key</param>
        /// <param name="versionId">Version identifier</param>
        /// <param name="bucketName">Bucket name</param>
        /// <returns></returns>
        Task<(bool isExist, S3ObjectMetadataResponse metadata)> CheckObjectExitstsAsync([NotNull]string objectKey, [AllowNull] string versionId = null, [AllowNull]string bucketName = null);

        /// <summary>
        /// Find object
        /// </summary>
        /// <param name="fileName">File name</param>
        /// <param name="folderPath">Folder path</param>
        /// <param name="bucketName">Bucket name</param>
        /// <returns></returns>
        Task<(bool isExist, S3ObjectMetadataResponse metadata)> FindObject([NotNull] string fileName, [AllowNull] string folderPath, [AllowNull] string bucketName = null);

        /// <summary>
        /// Creates an Amazon S3 bucket with versioning enabled, if the bucket does not already exist.
        /// </summary>
        /// <param name="bucketName">The name of the bucket to create.</param>
        Task CreateBucketWithVersioningAsync(string bucketName = null, bool useVersioning = true);

        /// <summary>
        /// Creates folder in bucket by name
        /// </summary>
        /// <param name="folderName">Folder name</param>
        /// <param name="bucketName">Bucket name</param>
        /// <returns></returns>
        Task DropBucketRecursive(string? bucketName);

        /// <summary>
        /// Store file with response result
        /// </summary>
        /// <param name="fileStream">File stream</param>
        /// <param name="folderPath">Path to file inside bucket</param>
        /// <param name="bucketName"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">File strean and file name if requeired</exception>
        Task<StoreObjectResponse> StoreObjectAsync([NotNull] Stream fileStream, [NotNull] string fileName, [AllowNull] string folderPath = null, [AllowNull] string bucketName = null);

        /// <summary>
        /// Store file with metadata if required
        /// </summary>
        /// <param name="objectStream">File stream</param>
        /// <param name="objectName">File name</param>
        /// <param name="folderPath">Folder path</param>
        /// <param name="metadata">File metadata</param>
        /// <param name="bucketName">Bucket name</param>
        /// <returns></returns>
        Task<StoreObjectResponse> StoreObjectAsync([NotNull] Stream objectStream, [NotNull] string objectName, Dictionary<string, string> metadata, [AllowNull] string folderPath = null,  [AllowNull] string bucketName = null);

        /// <summary>
        /// Gets exiting file by its key
        /// </summary>
        /// <param name="objectKey">file key</param>
        /// <param name="bucketName">Bucket name</param>
        /// <returns></returns>
        Task<GetS3ObjectResponse> GetObjectAsync([NotNull] string objectKey, [AllowNull] string bucketName = null);

        /// <summary>
        /// Gets exiting file by its key and version
        /// </summary>
        /// <param name="objectKey">File key</param>
        /// <param name="versionId">File version</param>
        /// <param name="bucketName">Bucket name</param>
        /// <returns></returns>
        Task<GetS3ObjectResponse> GetObjectAsync([NotNull] string objectKey, [AllowNull] string versionId = null, [AllowNull] string bucketName = null);

        /// <summary>
        /// Get object metadata
        /// </summary>
        /// <param name="objectKey">OBject key</param>
        /// <param name="versionId">VersionId</param>
        /// <param name="bucketName">bucket name</param>
        /// <returns></returns>
        Task<S3ObjectMetadataResponse> GetObjectMetadata([NotNull] string objectKey, [AllowNull] string versionId = null, [AllowNull] string bucketName = null);

        /// <summary>
        /// Update metadata
        /// </summary>
        /// <param name="objectKey">file key</param>
        /// <param name="metadata">metadata</param>
        /// <param name="versionId">Version</param>
        /// <param name="bucketName">Bucket name</param>
        /// <returns></returns>
        Task<StoreObjectResponse> ReplaceObjectMetadataAsync([NotNull] string objectKey, Dictionary<string, string> metadata, [AllowNull] string versionId = null, [AllowNull] string bucketName = null);
    }
}
