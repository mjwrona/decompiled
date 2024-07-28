// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Client.AccountManagement.AccountDisplayInfoNoLogoComparer
// Assembly: Microsoft.VisualStudio.Services.Client.Interactive, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 00B1FD41-439C-4B93-A417-9D1E4874E657
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Client.Interactive.dll

using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Client.AccountManagement
{
  public class AccountDisplayInfoNoLogoComparer : IEqualityComparer<AccountDisplayInfo>
  {
    public bool Equals(AccountDisplayInfo x, AccountDisplayInfo y) => x == y || x != null && y != null && x.AccountDisplayName.Equals(y.AccountDisplayName, StringComparison.OrdinalIgnoreCase) && x.ProviderDisplayName.Equals(y.ProviderDisplayName, StringComparison.OrdinalIgnoreCase) && x.UserName.Equals(y.UserName, StringComparison.OrdinalIgnoreCase) && (x.ProviderLogo != null || y.ProviderLogo == null) && (x.ProviderLogo == null || y.ProviderLogo != null) && (x.AccountLogo != null || y.AccountLogo == null) && (x.AccountLogo == null || y.AccountLogo != null);

    public int GetHashCode(AccountDisplayInfo obj) => StringComparer.OrdinalIgnoreCase.GetHashCode(obj.AccountDisplayName) ^ StringComparer.OrdinalIgnoreCase.GetHashCode(obj.ProviderDisplayName) ^ StringComparer.OrdinalIgnoreCase.GetHashCode(obj.UserName);
  }
}
