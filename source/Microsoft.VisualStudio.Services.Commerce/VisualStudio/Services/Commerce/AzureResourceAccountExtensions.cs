// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.AzureResourceAccountExtensions
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Commerce
{
  public static class AzureResourceAccountExtensions
  {
    internal static AzureResourceAccount UseCollectionHostAsAccountId(
      this AzureResourceAccount resourceAccount,
      bool useCollectionId = true)
    {
      if (useCollectionId && resourceAccount != null && resourceAccount.CollectionId != Guid.Empty)
        resourceAccount.AccountId = resourceAccount.CollectionId;
      return resourceAccount;
    }

    internal static IEnumerable<AzureResourceAccount> UseCollectionHostAsAccountId(
      this IEnumerable<AzureResourceAccount> resourceAccounts,
      bool useCollectionId)
    {
      if (useCollectionId)
      {
        foreach (AzureResourceAccount resourceAccount in resourceAccounts)
          resourceAccount.UseCollectionHostAsAccountId(useCollectionId);
      }
      return resourceAccounts;
    }
  }
}
