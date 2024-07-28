// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Client.AccountManagement.AccountCacheItem
// Assembly: Microsoft.VisualStudio.Services.Client.Interactive, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 00B1FD41-439C-4B93-A417-9D1E4874E657
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Client.Interactive.dll

using Microsoft.Identity.Client;
using System;
using System.Linq;
using System.Security.Claims;

namespace Microsoft.VisualStudio.Services.Client.AccountManagement
{
  internal class AccountCacheItem : IAccountCacheItem
  {
    private AuthenticationResult authenticationResult;

    public AccountCacheItem(AuthenticationResult authenticationResult) => this.authenticationResult = authenticationResult;

    public string UniqueId => this.authenticationResult.Account.HomeAccountId.ObjectId;

    public string TenantId => this.authenticationResult.Account.HomeAccountId.TenantId;

    public string Username => this.authenticationResult.Account.Username;

    public string Environment => this.authenticationResult.Account.Environment;

    public string IdToken => this.authenticationResult.IdToken;

    public DateTimeOffset ExpiresOn => this.authenticationResult.ExpiresOn;

    public string AccessToken => this.authenticationResult.AccessToken;

    public AuthenticationResult InnerResult => this.authenticationResult;

    public string GivenName => this.authenticationResult.ClaimsPrincipal.Claims.FirstOrDefault<Claim>((Func<Claim, bool>) (x => string.Equals(x.Type, "given_name", StringComparison.OrdinalIgnoreCase)))?.Value;

    public string FamilyName => this.authenticationResult.ClaimsPrincipal.Claims.FirstOrDefault<Claim>((Func<Claim, bool>) (x => string.Equals(x.Type, "family_name", StringComparison.OrdinalIgnoreCase)))?.Value;
  }
}
