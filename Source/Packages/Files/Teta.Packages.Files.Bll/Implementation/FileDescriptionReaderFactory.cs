// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileDescriptionReaderFactory.cs" company="TETA">
// Copyright (c) TETA. Ufa, 2022.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
// <summary>
// Интерпретатор файла для конкретного расширения
// </summary>

using Teta.Packages.Files.Bll.Implementation.HeaderInterpreters;
using Teta.Packages.Files.Bll.Interface;
using Teta.Packages.Files.Contracts;

namespace Teta.Packages.Files.Bll.Implementation
{
    /// <summary>
    /// Фабрика ридеров для заполнения описания файла
    /// </summary>
    public class FileDescriptionReaderFactory : IFileDescriptionReaderFactory
    {
        /// <inheritdoc/>
        public IFileDescriptionReader CreateReader(string extension)
        {
            switch (extension)
            {
                case ".txt":
                case ".csv":
                    return new CsvFileHeaderReader();
                case ".xlsx":
                    return new XlsxFileHeaderReader();
                case ".las":
                    return new LasFileHeaderReader();
                default: return null;
            }
        }
        
        /// <inheritdoc/>
        public IFileDescriptionReader CreateReader(FileDescriptor descriptor)
        {
            var extension = Path.GetExtension(descriptor.FileName);
            switch (extension)
            {
                case ".txt":
                case ".csv":
                    return new CsvFileHeaderReader();
                case ".xlsx":
                    return new XlsxFileHeaderReader();
                case ".las":
                    return new LasFileHeaderReader();
                default: return null;
            }
        }
    }
}
