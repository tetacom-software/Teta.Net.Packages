// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompanyUserDto.cs" company="TETA">
// Copyright (c) TETA. Ufa, 2023.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
// <summary>
//  Экземпляр объекта "пользователь компании" отображаемый в интерфейсе администрирования
// </summary>

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using Teta.Packages.Grid.Contracts.Attributes;
using Teta.Packages.Grid.Contracts.Column;

namespace Teta.Auth.Contracts.Model.Dto
{
    /// <summary>
    /// Пользователь компании
    /// </summary>
    public class CompanyUserDto
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        [IgnoreColumn]
        public int? Id { get; set; }

        /// <summary>
        /// Firstname
        /// </summary>
        [Required]
        [ColumnDescription(
            Caption = "Name",
            Sortable = true,
            Flex = 1,
            SortField = "Name",
            FilterField = "Name",
            FilterType = FilterType.String,
            SortOrder = 1)]
        public string Name { get; set; }

        /// <summary>
        /// Surname
        /// </summary>
        [Required]
        [ColumnDescription(
            Caption = "Surname",
            Sortable = true,
            Flex = 1,
            SortField = "Surname",
            FilterField = "Surname",
            FilterType = FilterType.String,
            SortOrder = 1)]
        public string Surname { get; set; }

        /// <summary>
        /// Middle name
        /// </summary>
        [ColumnDescription(
            Caption = "MiddleName",
            Sortable = true,
            Flex = 1,
            SortField = "MiddleName",
            FilterField = "MiddleName",
            FilterType = FilterType.String,
            SortOrder = 1)]
        public string MiddleName { get; set; }

        /// <summary>
        /// Unique user name
        /// </summary>
        [ColumnDescription(
            Caption = "UserName",
            Sortable = true,
            Flex = 1,
            SortField = "UserName",
            FilterField = "UserName",
            FilterType = FilterType.String,
            SortOrder = 1)]
        public string UserName { get; set; }

        /// <summary>
        /// Email
        /// </summary>
        [EmailAddress]
        [Required]
        [ColumnDescription(
            Caption = "Email",
            Sortable = true,
            Flex = 1,
            SortField = "Email",
            FilterField = "Email",
            FilterType = FilterType.String,
            SortOrder = 1)]
        public string Email { get; set; }

        /// <summary>
        /// Phone number
        /// </summary>
        [ColumnDescription(
            Caption = "Phone",
            Sortable = true,
            Flex = 1,
            SortField = "Phone",
            FilterField = "Phone",
            FilterType = FilterType.String,
            SortOrder = 1)]
        public string Phone { get; set; }

        /// <summary>
        /// Cell phone
        /// </summary>
        [IgnoreColumn]
        public string CellPhone { get; set; }

        /// <summary>
        /// Пользователь верифицирован
        /// </summary>
        [ColumnDescription(
            Caption = "Verified",
            Sortable = true,
            Flex = 1,
            SortField = "Verified",
            FilterField = "Verified",
            FilterType = FilterType.Boolean,
            SortOrder = 1)]
        public bool Verified { get; set; }

        /// <summary>
        /// Идентификатор файла аватара
        /// </summary>
        [IgnoreColumn]
        public string AvatarFileId { get; set; }

        /*
        /// <summary>
        /// Пол пользователя
        /// </summary>
        [ColumnDescription(
            Caption = "Gender",
            Sortable = true,
            Flex = 1,
            SortField = "Gender",
            FilterField = "Gender",
            FilterType = FilterType.List,
            SortOrder = 1)]
        public Gender Gender { get; set; }

        /// <summary>
        /// Права пользователей в рамках информационных систем/компании
        /// </summary>
        [JsonIgnore]
        [NotMapped]
        public List<TetaClaimDto> TetaClaims { get; set; }

        /// <summary>
        /// Клеймы в рамках информационной системы
        /// </summary>
        public Dictionary<SystemType, TetaClaimDto[]> SystemClaims => TetaClaims?.Select(c => new { type = ClaimTypeExtensions.GetSystemType(c.Type), c = c })
            .GroupBy(t => t.type).ToDictionary(c => c.Key, c => c.Select(v => v.c).ToArray());
        */
    }
}
