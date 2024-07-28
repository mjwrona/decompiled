// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Client.AccountManagement.IAccountCacheItem
// Assembly: Microsoft.VisualStudio.Services.Client.Interactive, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 00B1FD41-439C-4B93-A417-9D1E4874E657
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Client.Interactive.dll

using Microsoft.Identity.Client;
using System;

namespace Microsoft.VisualStudio.Services.Client.AccountManagement
{
  public interface IAccountCacheItem
  {
    string UniqueId { get; }

    string TenantId { get; }

    string Username { get; }

    string Environment { get; }

    string IdToken { get; }

    DateTimeOffset ExpiresOn { get; }

    string AccessToken { get; }

    AuthenticationResult InnerResult { get; }

    string GivenName { get; }

    string FamilyName { get; }
  }
}
