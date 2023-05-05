// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AmazonBasedS3Service.cs" company="TETA">
// Copyright (c) TETA. Ufa, 2022.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
// <summary>
// S3 service based on Amazon S3 packages
// </summary>

using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Teta.Packages.Files.Contracts.Internal.S3;
using Teta.Packages.Files.S3.Interface;

namespace Teta.Packages.Files.S3.Implementation
{
    /// <summary>
    /// S3 service based on Amazon S3 packages
    /// </summary>
    internal class AmazonBasedS3Service : IS3Service
    {
        private readonly IAmazonS3 _s3Client;
        private readonly ITransferUtility _transferUtility;
        private readonly S3Options _options;

        private const string MetadataHeaderPrefix = "x-amz-meta-";
        private const string FolderFormat = "{0}/";

        public AmazonBasedS3Service(IAmazonS3 s3Client, ITransferUtility transferUtility, IOptions<S3Options> options)
        {
            _s3Client = s3Client ?? throw new ArgumentNullException(nameof(s3Client));
            _transferUtility = transferUtility ?? throw new ArgumentNullException(nameof(transferUtility));
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        }

        /// <inheritdoc />
        public async Task CreateFolderAsync([NotNull] string folderName, [AllowNull] string? bucketName = null)
        {
            if (string.IsNullOrEmpty(folderName))
            {
                throw new ArgumentException($"'{nameof(folderName)}' cannot be null or empty.", nameof(folderName));
            }

            var bucket = bucketName ?? _options.DefaultBucketName;
            var folderToCreate = string.Format(FolderFormat, folderName.Trim('/').Trim('\\').Replace("\\", "/"));

            var request = new PutObjectRequest
            {
                BucketName = bucket,
                Key = folderToCreate,
                ContentBody = string.Empty,
            };

            var resp = await _s3Client.PutObjectAsync(request);
            CheckOrThrow(resp);
        }

        /// <inheritdoc />
        public async Task<StoreObjectResponse> StoreObjectAsync([NotNull] Stream objectStream, [NotNull] string objectKey, [AllowNull] string folderPath, [AllowNull] string bucketName = null)
        {
            return await StoreObjectAsync(objectStream, objectKey, null, folderPath, bucketName);
        }


        /// <inheritdoc />
        public async Task<StoreObjectResponse> StoreObjectAsync([NotNull] Stream objectStream, [NotNull] string objectName, Dictionary<string, string> metadata, [AllowNull] string folderPath = null, [AllowNull] string bucketName = null)
        {
            if (objectStream is null)
            {
                throw new ArgumentNullException(nameof(objectStream));
            }

            if (string.IsNullOrEmpty(objectName))
            {
                throw new ArgumentException($"'{nameof(objectName)}' cannot be null or empty.", nameof(objectName));
            }

            string key;
            if (!string.IsNullOrEmpty(folderPath))
            {
                var folder = folderPath.Trim('/').Trim('\\');
                key = $"{folder}/{objectName}";
            }
            else
            {
                key = objectName;
            }

            var bucket = bucketName ?? _options.DefaultBucketName;
            var request = new PutObjectRequest
            {
                BucketName = bucket,
                Key = key,
                InputStream = objectStream
            };

            if (metadata != null && metadata.Any())
            {
                foreach (var pair in metadata)
                {
                    request.Metadata.Add(pair.Key, pair.Value);
                }
            }

            var sha256 = objectStream.ComputeSha256HashToBase64String();

            request.Metadata[S3ObjectMetadataResponse.TetacomSha256Header] = sha256;
            var resp = await _s3Client.PutObjectAsync(request);
 
            CheckOrThrow(resp);

            var sfr = new StoreObjectResponse()
            {
                Etag = resp.ETag,
                Key = key,
                VersionId = resp.VersionId,
                ExpirationUtc = resp.Expiration?.ExpiryDateUtc,
                ContentLengthBytes = resp.ContentLength,
                FileName = ExtractFileName(key),
            };

            return sfr;
        }

        /// <inheritdoc/>
        public async Task<GetS3ObjectResponse> GetObjectAsync([NotNull] string objectKey, [AllowNull] string bucketName = null)
        {
            return await GetObjectAsync(objectKey, bucketName: bucketName, versionId: null);
        }

        /// <inheritdoc/>
        public async Task<S3ObjectMetadataResponse> GetObjectMetadata([NotNull] string objectKey, [AllowNull] string versionId = null, [AllowNull] string bucketName = null)
        {
            if (objectKey is null)
            {
                throw new ArgumentNullException(nameof(objectKey));
            }

            var bucket = bucketName ?? _options.DefaultBucketName;
            var request = new GetObjectMetadataRequest
            {
                BucketName = bucket,
                Key = objectKey,
                VersionId = versionId,
            };

            var response = await _s3Client.GetObjectMetadataAsync(request);
            CheckOrThrow(response);

            var dict = MetadataCollectionToDictionary(response.Metadata);

            return new S3ObjectMetadataResponse(dict)
            {
                VersionId = response.VersionId,
                Etag = response.ETag,
                ExpirationUtc = response.Expiration?.ExpiryDateUtc,
                Key = objectKey,
                ContentLengthBytes = response.ContentLength,
                FileName = ExtractFileName(objectKey)
            };

        }

        /// <inheritdoc/>
        public async Task<StoreObjectResponse> ReplaceObjectMetadataAsync([NotNull] string objectKey, Dictionary<string, string> metadata, [AllowNull] string versionId = null, [AllowNull] string bucketName = null)
        {
            if (objectKey is null)
            {
                throw new ArgumentNullException(nameof(objectKey));
            }

            if (metadata is null)
            {
                throw new ArgumentNullException(nameof(metadata));
            }
            
            var bucket = bucketName ?? _options.DefaultBucketName;
            var currentMetadata = await GetObjectMetadata(objectKey, versionId: versionId, bucketName: bucket);
            
            var request = new CopyObjectRequest
            {
                SourceBucket = bucket,
                SourceKey = objectKey,
                DestinationBucket = bucket,
                DestinationKey = objectKey,
                MetadataDirective = S3MetadataDirective.REPLACE,
                SourceVersionId = versionId,
            };

            DictionaryToMetadataCollection(request.Metadata, metadata);

            request.Metadata[S3ObjectMetadataResponse.TetacomSha256Header] = currentMetadata[S3ObjectMetadataResponse.TetacomSha256Header];

            var resp = await _s3Client.CopyObjectAsync(request);
            CheckOrThrow(resp);

            return new StoreObjectResponse()
            {
                Etag = resp.ETag,
                ExpirationUtc = resp?.Expiration?.ExpiryDateUtc,
                Key = objectKey,
                VersionId = resp.VersionId,
                ContentLengthBytes = resp.ContentLength,
                FileName = ExtractFileName(objectKey)
            };
        }

        /// <inheritdoc/>
        public async Task<GetS3ObjectResponse> GetObjectAsync([NotNull] string objectKEy, [NotNull] string versionId = null, [AllowNull] string bucketName = null)
        {
            if (objectKEy is null)
            {
                throw new ArgumentNullException(nameof(objectKEy));
            }

            var bucket = bucketName ?? _options.DefaultBucketName;
            var request = new GetObjectRequest
            {
                BucketName = bucket,
                Key = objectKEy,
                VersionId = versionId
            };

            var response = await _s3Client.GetObjectAsync(request);
            CheckOrThrow(response);

            var metadata = MetadataCollectionToDictionary(response.Metadata);
            var objectResponse = new GetS3ObjectResponse(metadata)
            {
                ObjectDataStream = response.ResponseStream,
                Key = objectKEy,
                VersionId = versionId,
                ContentLengthBytes = response.ContentLength,
                FileName = ExtractFileName(objectKEy)
            };

            return objectResponse;

        }

        /// <inheritdoc />
        public async Task<bool> CheckFolderExistsAsync([NotNull] string folderName, [AllowNull] string? bucketName = null)
        {
            if (string.IsNullOrEmpty(folderName))
            {
                throw new ArgumentException($"'{nameof(folderName)}' cannot be null or empty.", nameof(folderName));
            }

            var folderFormat = "{0}/";
            var bucket = bucketName ?? _options.DefaultBucketName;

            var folder = string.Format(folderFormat, folderName.Trim('/').Trim('\\'));

            using (var s3Client = new AmazonS3Client())
            {
                var request = new ListObjectsV2Request
                {
                    BucketName = bucket,
                    Prefix = folder,
                    Delimiter = "/",
                };


                ListObjectsV2Response resp;
                do
                {
                    resp = await _s3Client.ListObjectsV2Async(request);

                    CheckOrThrow(resp);
                    if (resp.CommonPrefixes.Count > 0)
                    {
                        return true;
                    }

                    request.ContinuationToken = resp.NextContinuationToken;

                } while (resp.IsTruncated);

                return false;
            }
        }

        /// <inheritdoc />
        public async Task<bool> CheckBucketExistsAsync([AllowNull] string bucketName = null)
        {
            var bucketForSearch = bucketName ?? _options.DefaultBucketName;

            var bucketsResponse = await _s3Client.ListBucketsAsync(new ListBucketsRequest());
            CheckOrThrow(bucketsResponse);
            var currentBucket = bucketsResponse.Buckets
                .FirstOrDefault(b => b.BucketName == bucketForSearch);
            return currentBucket != null;
        }

        /// <inheritdoc />
        public async Task<(bool isExist, S3ObjectMetadataResponse metadata)> CheckObjectExitstsAsync([NotNull] string objectKey, [AllowNull] string versionId = null, [AllowNull] string bucketName = null)
        {
            if (objectKey is null)
            {
                throw new ArgumentNullException(nameof(objectKey));
            }

            var bucket = bucketName ?? _options.DefaultBucketName;

            try
            {
                var request = new GetObjectMetadataRequest()
                {
                    BucketName = bucket,
                    VersionId = versionId,
                    Key = objectKey,
                };

                // THROWING ERROR IS VALID! For checking object exists!!!
                // https://stackoverflow.com/questions/3526585/determine-if-an-object-exists-in-a-s3-bucket-based-on-wildcard
                var response = await _s3Client.GetObjectMetadataAsync(request);

                var metadata = new S3ObjectMetadataResponse(MetadataCollectionToDictionary(response.Metadata))
                {
                    VersionId = response.VersionId,
                    Etag = response.ETag,
                    ExpirationUtc = response.Expiration?.ExpiryDateUtc,
                    Key = objectKey,
                    ContentLengthBytes = response.ContentLength,
                    FileName = ExtractFileName(objectKey),
                };

                return (true, metadata);
            }

            catch (AmazonS3Exception ex)
            {
                if (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return (false, null);
                }

                //status wasn't not found, so throw the exception
                throw;
            }
        }

        /// <inheritdoc />
        public async Task<(bool isExist, S3ObjectMetadataResponse metadata)> FindObject([NotNull] string fileName, [AllowNull] string folderPath, [AllowNull] string bucketName = null)
        {
            if (fileName is null)
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            string objectKey;
            if (!String.IsNullOrEmpty(folderPath))
            {
                var folder = string.Format(FolderFormat, folderPath.Trim('/').Trim('\\').Replace("\\", "/"));
                objectKey = $"{folder}{fileName}";
            } 
            else
            {
                objectKey = fileName;
            }
 
            return await CheckObjectExitstsAsync(objectKey,  versionId: null, bucketName: bucketName);
        }

        /// <inheritdoc />
        public async Task CreateBucketWithVersioningAsync(string bucketName = null, bool useVersioning = true)
        {
            var bucketToCreate = bucketName ?? _options.DefaultBucketName;
            var resp = await _s3Client.PutBucketAsync(bucketToCreate);

            CheckOrThrow(resp);
            if (useVersioning)
            {
                // Enable versioning on the bucket
                var versioning = await _s3Client.PutBucketVersioningAsync(new PutBucketVersioningRequest
                {
                    BucketName = bucketToCreate,
                    VersioningConfig = new S3BucketVersioningConfig
                    {
                        Status = VersionStatus.Enabled
                    }
                });

                CheckOrThrow(versioning);
            }
        }

        /// <inheritdoc/>
        public async Task DropBucketRecursive(string bucketName)
        {
            var listObjectsResponse = await _s3Client.ListObjectsAsync(bucketName);
            while (listObjectsResponse.S3Objects.Count > 0)
            {
                var objects = listObjectsResponse.S3Objects.Select(o => new KeyVersion { Key = o.Key }).ToList();
                var deleteObjectsRequest = new DeleteObjectsRequest
                {
                    BucketName = bucketName,
                    Objects = objects
                };

                var deleteObjectsResponse = await _s3Client.DeleteObjectsAsync(deleteObjectsRequest);
                listObjectsResponse = await _s3Client.ListObjectsAsync(bucketName);
            }

            // Delete the bucket itself
            var resp = await _s3Client.DeleteBucketAsync(bucketName);
            CheckOrThrow(resp);
        }

        private bool SuccessResponse(AmazonWebServiceResponse resp)
        {
            return resp.HttpStatusCode > System.Net.HttpStatusCode.OK && resp.HttpStatusCode < System.Net.HttpStatusCode.MultipleChoices;
        }

        private Dictionary<string, string> MetadataCollectionToDictionary(MetadataCollection metadataCollection)
        {
            var dict = new Dictionary<string, string>();

            foreach (var key in metadataCollection.Keys)
            {
                dict.Add(key.Replace(MetadataHeaderPrefix, string.Empty), metadataCollection[key]);
            }

            return dict;
        }

        private void DictionaryToMetadataCollection(MetadataCollection metadataCollection, Dictionary<string, string> metadata)
        {
            if (metadata != null)
            {
                foreach (var keyPair in metadata)
                {
                    metadataCollection.Add(keyPair.Key, keyPair.Value);
                }
            }
        }

        private void CheckOrThrow(AmazonWebServiceResponse resp)
        {
            if (SuccessResponse(resp))
            {
                throw new Exception($"Error in creating folder: {resp.HttpStatusCode}, requestId: {resp.ResponseMetadata.RequestId}");
            }
        }

        private string ExtractFileName(string key)
        {
            return Path.GetFileName(key);
        }
    }
}