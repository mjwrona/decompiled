// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Client.AccountManagement.AccountKeyComparer
// Assembly: Microsoft.VisualStudio.Services.Client.Interactive, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 00B1FD41-439C-4B93-A417-9D1E4874E657
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Client.Interactive.dll

using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Client.AccountManagement
{
  public class AccountKeyComparer : IEqualityComparer<AccountKey>
  {
    public bool Equals(AccountKey x, AccountKey y) => x == y || x != null && y != null && string.Equals(x.UniqueId, y.UniqueId, StringComparison.Ordinal) && x.ProviderId.Equals(y.ProviderId);

    public int GetHashCode(AccountKey obj) => StringComparer.Ordinal.GetHashCode(obj.UniqueId) ^ obj.ProviderId.GetHashCode();
  }
}
