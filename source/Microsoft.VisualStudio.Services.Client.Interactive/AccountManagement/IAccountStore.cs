// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Client.AccountManagement.IAccountStore
// Assembly: Microsoft.VisualStudio.Services.Client.Interactive, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 00B1FD41-439C-4B93-A417-9D1E4874E657
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Client.Interactive.dll

using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Client.AccountManagement
{
  public interface IAccountStore
  {
    event EventHandler<AccountStoreChangedEventArgs> KeychainAccountStoreChanged;

    event EventHandler<AccountStoreChangedEventArgs> KeychainAccountStoreChanging;

    Account AddOrUpdateAccount(Account account);

    Account SetDisplayInfo(AccountKey account, AccountDisplayInfo info);

    Account SetProperty(AccountKey account, string key, string value);

    Account SetNeedsReauthentication(AccountKey account, bool value);

    Account SetProperties(AccountKey account, IDictionary<string, string> properties);

    void RemoveAccount(AccountKey key);

    IReadOnlyCollection<Account> GetAllAccounts();
  }
}
