// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileExtensionAttribute.cs" company="TETA">
// Copyright (c) TETA. Ufa, 2022.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
// <summary>
//  Расширение файла
// </summary>

using System;

namespace Teta.Packages.Files.Bll.Implementation.HeaderInterpreters
{
    /// <summary>
    /// File extension
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public class FileExtensionAttribute : Attribute
    {
        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="FileExtensionAttribute"/>.
        /// </summary>
        /// <param name="extension">Расширение файла</param>
        public FileExtensionAttribute(string extension)
        {
            Extension = extension;
        }

        /// <summary>
        /// Расширение
        /// </summary>
        public string Extension { get; set; }
    }
}
