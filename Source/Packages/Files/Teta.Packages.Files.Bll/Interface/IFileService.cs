// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IFileService.cs" company="TETA">
// Copyright (c) TETA. Ufa, 2023.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
// <summary>
//  File service interface
// </summary>

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Teta.Files.Contracts;
using Teta.Packages.Files.Contracts;

namespace Teta.Packages.Files.Bll.Interface
{
    /// <summary>
    /// File service interface
    /// </summary>
    public interface IFileService
    {
        /// <summary>
        /// Store file data
        /// </summary>
        /// <param name="fileStream">Fiel data stream</param>
        /// <param name="fileName">File name</param>
        /// <param name="expireTimeSpan">Время жизни файла</param>
        /// <returns>A <see cref="FileDescriptor"/> representing the asynchronous operation.</returns>
        Task<FileDescriptor> StoreFileAsync(Stream fileStream, string fileName, TimeSpan? expireTimeSpan = null);

        /// <summary>
        /// Gets file by descriptor
        /// </summary>
        /// <param name="descriptor">file descriptor</param>
        /// <returns>Returns file</returns>
        Task<Stream> GetFileStreamAsync(FileDescriptor descriptor);

        /// <summary>
        /// Gets file by key
        /// </summary>
        /// <param name="fileKey">file descriptor</param>
        /// <returns>Returns file</returns>
        Task<(Stream stream, FileDescriptor descriptor)> GetFileStreamAsync(string fileKey);

        /// <summary>
        /// Get file descriptor by id
        /// </summary>
        /// <param name="fileKey">Metadata id</param>
        /// <returns>File descriptor</returns>
        Task<(FileDescriptor, IDictionary<string, string>)> GetFileDescriptorWithMetadataAsync(string fileKey);
        
        /// <summary>
        /// Update file metadata
        /// </summary>
        /// <param name="descriptor">Descriptor</param>
        /// <param name="metadataCollection">Metadata collection</param>
        /// <returns></returns>
        Task<FileDescriptor> UpdateFileMetadata(FileDescriptor descriptor, Dictionary<string, string> metadataCollection);

        /// <summary>
        /// returs colection of files for current user
        /// </summary>
        /// <returns>User files</returns>
        Task<IEnumerable<FileDescriptor>> GetCurrentUserFilesAsync();
    }
}
