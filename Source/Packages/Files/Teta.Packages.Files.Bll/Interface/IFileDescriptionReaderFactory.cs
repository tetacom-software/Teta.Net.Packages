// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IFileDescriptionReader.cs" company="TETA">
// Copyright (c) TETA. Ufa, 2023.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
// <summary>
// Интерпретатор файла для конкретного расширения
// </summary>

using Teta.Packages.Files.Contracts;

namespace Teta.Packages.Files.Bll.Interface
{
    /// <summary>
    /// Get description file reader
    /// </summary>
    public interface IFileDescriptionReaderFactory
    {
        /// <summary>
        /// Создать ридер для расширения файла
        /// </summary>
        /// <param name="extension">Расшрение файла</param>
        /// <returns>Ридер</returns>
        IFileDescriptionReader CreateReader(string extension);

        /// <summary>
        /// Create reader from descriptor
        /// </summary>
        /// <param name="descriptor">Descriptor</param>
        /// <returns>Reader</returns>
        IFileDescriptionReader CreateReader(FileDescriptor descriptor);


    }
}
