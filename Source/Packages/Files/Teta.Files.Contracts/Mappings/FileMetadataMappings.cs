// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileMetadataMappings.cs" company="TETA">
// Copyright (c) TETA. Ufa, 2023.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
// <summary>
// Маппинги для метаданных
// </summary>

using Mapster;
using Teta.Packages.Files.Contracts.Dto;

namespace Teta.Packages.Files.Contracts.Mappings
{
    /// <summary>
    /// Маппинги для метаданных
    /// </summary>
    public class FileMetadataMappings : IRegister
    {
        /// <summary>
        /// Регистрация маппинга
        /// </summary>
        /// <param name="config">Конфигурация</param>
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<FileDescriptor, FileMetadataDto>()
                .Map(s => s.FileName, d => d.FileName)
                .Map(s => s.OwnerUserMail, d => d.OwnerUserMail)
                .Map(s => s.ETag, d => d.ETag)
                .Map(s => s.ExpiryDateUtc, d => d.ExpiryDateUtc)
                .Map(s => s.FileKey, d => d.FileKey)
                .Map(s => s.UploadDate, d => d.UploadDate)
                .TwoWays();
        }
    }
}