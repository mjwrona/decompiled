// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Client.AccountManagement.AccountMemberComparer
// Assembly: Microsoft.VisualStudio.Services.Client.Interactive, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 00B1FD41-439C-4B93-A417-9D1E4874E657
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Client.Interactive.dll

using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Client.AccountManagement
{
  public class AccountMemberComparer : IEqualityComparer<Account>
  {
    public bool Equals(Account accountA, Account accountB)
    {
      if (accountA == accountB)
        return true;
      if (accountA == null || accountB == null || !accountA.Authenticator.Equals(accountB.Authenticator, StringComparison.OrdinalIgnoreCase))
        return false;
      int count = accountA.SupportedAccountProviders.Count;
      if (count != accountB.SupportedAccountProviders.Count)
        return false;
      for (int index = 0; index < count; ++index)
      {
        if (accountA.SupportedAccountProviders[index] != accountB.SupportedAccountProviders[index])
          return false;
      }
      if (!AccountDisplayInfo.DisplayInfoNoLogoComparer.Equals(accountA.DisplayInfo, accountB.DisplayInfo) || accountA.NeedsReauthentication != accountB.NeedsReauthentication || accountA.Properties.Count != accountB.Properties.Count)
        return false;
      foreach (KeyValuePair<string, string> property in (IEnumerable<KeyValuePair<string, string>>) accountA.Properties)
      {
        string b = (string) null;
        if (!accountB.Properties.TryGetValue(property.Key, out b) || !string.Equals(property.Value, b, StringComparison.OrdinalIgnoreCase))
          return false;
      }
      return true;
    }

    public int GetHashCode(Account obj) => new AccountKeyComparer().GetHashCode((AccountKey) obj);
  }
}
