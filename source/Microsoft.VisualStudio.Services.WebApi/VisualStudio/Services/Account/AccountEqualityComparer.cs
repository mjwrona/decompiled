// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Account.AccountEqualityComparer
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Account
{
  public class AccountEqualityComparer : IEqualityComparer<Microsoft.VisualStudio.Services.Account.Account>
  {
    bool IEqualityComparer<Microsoft.VisualStudio.Services.Account.Account>.Equals(
      Microsoft.VisualStudio.Services.Account.Account x,
      Microsoft.VisualStudio.Services.Account.Account y)
    {
      return x == y || x != null && y != null && x.AccountId.Equals(y.AccountId) && string.Equals(x.AccountName, y.AccountName, StringComparison.OrdinalIgnoreCase);
    }

    int IEqualityComparer<Microsoft.VisualStudio.Services.Account.Account>.GetHashCode(Microsoft.VisualStudio.Services.Account.Account obj) => obj.AccountId.GetHashCode() ^ obj.AccountName.GetHashCode();

    public static AccountEqualityComparer Instance { get; } = new AccountEqualityComparer();
  }
}
