// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileService.cs" company="TETA">
// Copyright (c) TETA. Ufa, 2022.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
// <summary>
//  Implements file service
// </summary>

using System.Text;
using Teta.Packages.Auth.Interfaces;
using Teta.Packages.Files.Bll.Interface;
using Teta.Packages.Files.Contracts;
using Teta.Packages.Files.Contracts.Internal.S3;
using Teta.Packages.Files.S3;
using Teta.Packages.Files.S3.Interface;

namespace Teta.Packages.Files.Bll.Implementation
{
    /// <summary>
    /// File service implementation
    /// </summary>
    public class FileService : IFileService
    {
        private readonly string _bucketName;
        private readonly IS3Service _s3Service;
        private readonly IUserContext _userContext;

        private const string FileKeyFormat = "<{0}>{1}";
        
        /// <summary>
        /// Crates new instance of file service
        /// </summary>
        /// <param name="s3Service">S3 service</param>
        /// <param name="logger">Logger</param>
        public FileService(
            IS3Service s3Service,
            IUserContext userContext)
        {
            _s3Service = s3Service ?? throw new ArgumentNullException(nameof(s3Service));
            _userContext = userContext ?? throw new ArgumentNullException(nameof(userContext));
        }

        /// <summary>
        /// Store file data
        /// </summary>
        /// <param name="fileStream">Fiel data stream</param>
        /// <param name="fileName">File name</param>
        /// <param name="expireTimeSpan">Время жизни файла</param>
        /// <returns>representing the asynchronous operation.</returns>
        public async Task<FileDescriptor> StoreFileAsync(Stream fileStream, string fileName, TimeSpan? expireTimeSpan = null)
        {
            await EnsureBucketAsync();

            var existsResponse = await _s3Service.FindObject(fileName, _userContext.Email);

            // Check file checksum if exits
            if (existsResponse.isExist)
            {
                var sha256 = fileStream.ComputeSha256HashToBase64String();
                if (sha256 == existsResponse.metadata.ChecksumSHA256Base64)
                {
                    return ConvertToFileDescriptor(existsResponse.metadata);
                }
            }

            var resp = await _s3Service.StoreObjectAsync(fileStream, fileName, folderPath: _userContext.Email);
            return ConvertToFileDescriptor(resp);
        }

        /// <inheritdoc/>
        public async Task<Stream> GetFileStreamAsync(FileDescriptor descriptor)
        {
            var key = Extract(descriptor.FileKey);

            await EnsureBucketAsync();

            // Check access
            var fs = await _s3Service.GetObjectAsync(key.key, versionId: key.version);
            return fs.ObjectDataStream;
        }

        /// <inheritdoc/>
        public async Task<(Stream stream, FileDescriptor descriptor)> GetFileStreamAsync(string fileKey)
        {
            var key = Extract(fileKey);

            await EnsureBucketAsync();
            var objectReponse = await _s3Service.GetObjectAsync(key.key, key.version);

            var metadata = ConvertToFileDescriptor(objectReponse);
            return (objectReponse.ObjectDataStream, metadata);
        }

        /// <inheritdoc/>
        public async Task<(FileDescriptor, IDictionary<string, string>)> GetFileDescriptorWithMetadataAsync(string fileKey)
        {
            var key = Extract(fileKey);

            await EnsureBucketAsync();
            var response = await _s3Service.GetObjectMetadata(key.key, key.version);

            var fileDescriptor = ConvertToFileDescriptor(response);
            return (fileDescriptor, response);
        }

        /// <inheritdoc/>
        public async Task<FileDescriptor> UpdateFileMetadata(FileDescriptor descriptor, Dictionary<string, string> metadataCollection)
        {
            var key = Extract(descriptor.FileKey);
            await EnsureBucketAsync();

            var response = await _s3Service.ReplaceObjectMetadataAsync(key.key, metadataCollection, versionId: key.version);
            return ConvertToFileDescriptor(response);
        }

        /// <summary>
        /// returs colection of files for current user
        /// </summary>
        /// <returns>User files</returns>
        public async Task<IEnumerable<FileDescriptor>> GetCurrentUserFilesAsync()
        {
            await EnsureBucketAsync();
            throw new NotImplementedException();
            //return await _metadataRepository.FindBy(md => md.UserOwnerId == _userContext.Id).ToArrayAsync();
        }

        private (string version, string key) Extract(string fileKey)
        {
            var bytes = Convert.FromBase64String(fileKey);
            var strKey = System.Text.Encoding.UTF8.GetString(bytes);
            
            var split = strKey.Split(">");
            if (split.Length != 2)
            {
                throw new Exception("Key format exception");
            }

            return (split[0].Substring(1, split[0].Length - 1), split[1]);
        }

        private string CreateFileKey(S3ObjectMainProperties props)
        {
            var key = string.Format(FileKeyFormat, props.VersionId, props.Key);
            var bytes = Encoding.UTF8.GetBytes(key);
            return Convert.ToBase64String(bytes);
        }

        private string GetFolderFromS3OBjectKey(string objectKey)
        {
            // Извлечь папку пользователя к которой будет происходить доступ
            var pathPairs = objectKey.Split("/");
            
            return pathPairs[0];
        }

        private async Task EnsureBucketAsync()
        {
            // Checks that default bucket exists
            var exists = await _s3Service.CheckBucketExistsAsync();
            if (!exists)
            {
                await _s3Service.CreateBucketWithVersioningAsync();
            }
        }

        private FileDescriptor ConvertToFileDescriptor(S3ObjectMainProperties props)
        {
            return new FileDescriptor
            {
                ETag = props.Etag,
                ExpiryDateUtc = props.ExpirationUtc,
                FileKey = CreateFileKey(props),
                FileName =  props.FileName,
                FileSizeBytes = props.ContentLengthBytes,
                OwnerUserMail = GetFolderFromS3OBjectKey(props.Key),
            };
        }
    }
}
