// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Client.AccountManagement.PartialCacheItem
// Assembly: Microsoft.VisualStudio.Services.Client.Interactive, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 00B1FD41-439C-4B93-A417-9D1E4874E657
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Client.Interactive.dll

using Microsoft.Identity.Client;
using System;

namespace Microsoft.VisualStudio.Services.Client.AccountManagement
{
  internal class PartialCacheItem : IAccountCacheItem
  {
    private IAccount account;

    public PartialCacheItem(IAccount account) => this.account = account;

    public string UniqueId => this.account.HomeAccountId.ObjectId;

    public string TenantId => this.account.HomeAccountId.TenantId;

    public string Username => this.account.Username;

    public string Environment => this.account.Environment;

    public string IdToken => throw new NotImplementedException();

    public DateTimeOffset ExpiresOn => throw new NotImplementedException();

    public string AccessToken => throw new NotImplementedException();

    public AuthenticationResult InnerResult => throw new NotImplementedException();

    public string GivenName => throw new NotImplementedException();

    public string FamilyName => throw new NotImplementedException();
  }
}
