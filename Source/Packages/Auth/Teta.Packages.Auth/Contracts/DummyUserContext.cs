// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DummyUserContext.cs" company="TETA">
// Copyright (c) TETA. Ufa, 2023.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
// <summary>
// User Context
// </summary>

using Teta.Packages.Auth.Configuration;
using Teta.Packages.Auth.Interfaces;

namespace Teta.Packages.Auth.Contracts;

/// <summary>
/// User Context
/// </summary>
public class DummyUserContext:IUserContext
{
    public string Token { get; }
    public string RequestCulture { get; } = "ru-ru";
    public bool IsAnonymous { get; } = false;
    public string Sid { get; }
    public string PreferredUserName { get; }
    public string GivenName { get; }
    public bool IsSysAdministrator { get; }
    public bool IsCompanyOwner { get; }
    public int Id { get; } = 1;
    public string UserName { get; } = "TestUser";
    public string DisplayName { get; } = "TestUser";
    public string Email { get; } = "TestUser@email.ru";
    public bool IsSuperUser { get; }
    public int ContextCompanyId { get; }
    public IEnumerable<string> Roles => new[] { "ADMIN" };
    public UserData UserData { get; }
    public bool HasCompany(int companyId)
    {
        return true;
    }

    public bool HasContextCompany()
    {
        return true;
    }

    public bool HasCompanyRole(int companyId, params string[] roles)
    {
        return true;
    }

    public bool HasContextCompanyRole(params string[] roles)
    {
        return true;
    }

    public int[] GetUserCompanies()
    {
        return new[] { 1 };
    }
}