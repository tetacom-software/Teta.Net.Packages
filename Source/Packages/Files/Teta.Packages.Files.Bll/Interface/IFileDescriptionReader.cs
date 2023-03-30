// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IFileDescriptionReader.cs" company="TETA">
// Copyright (c) TETA. Ufa, 2023.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
// <summary>
// Интерпретатор файла для конкретного расширения
// </summary>

using System.IO;
using Teta.Files.Contracts;
using Teta.Files.Contracts.Internal;
using Teta.Packages.Files.Contracts;

namespace Teta.Packages.Files.Bll.Interface
{
    /// <summary>
    /// Интерпретатор файла для конкретного расширения
    /// </summary>
    public interface IFileDescriptionReader
    {
        /// <summary>
        /// Обработать файл
        /// </summary>
        /// <param name="file">Файл</param>
        /// <param name="descriptor">Метаданные файла</param>
        /// <param name="description">File description</param>
        void FillDescriptionData(Stream file, FileDescriptor descriptor, FileDescription description);
    }
}
