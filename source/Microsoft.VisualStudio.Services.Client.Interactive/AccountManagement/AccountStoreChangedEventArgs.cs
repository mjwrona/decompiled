// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Client.AccountManagement.AccountStoreChangedEventArgs
// Assembly: Microsoft.VisualStudio.Services.Client.Interactive, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 00B1FD41-439C-4B93-A417-9D1E4874E657
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Client.Interactive.dll

using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Client.AccountManagement
{
  public class AccountStoreChangedEventArgs : EventArgs
  {
    public IReadOnlyList<Account> Added { get; private set; }

    public IReadOnlyList<Account> Removed { get; private set; }

    public IReadOnlyList<Tuple<Account, Account>> Modified { get; private set; }

    public AccountStoreChangedEventArgs(
      IReadOnlyList<Account> added,
      IReadOnlyList<Account> removed,
      IReadOnlyList<Tuple<Account, Account>> modified)
    {
      this.Added = added ?? (IReadOnlyList<Account>) new List<Account>().AsReadOnly();
      this.Removed = removed ?? (IReadOnlyList<Account>) new List<Account>().AsReadOnly();
      this.Modified = modified ?? (IReadOnlyList<Tuple<Account, Account>>) new List<Tuple<Account, Account>>().AsReadOnly();
    }

    public AccountStoreChangedEventArgs()
      : this((IReadOnlyList<Account>) null, (IReadOnlyList<Account>) null, (IReadOnlyList<Tuple<Account, Account>>) null)
    {
    }

    private static IReadOnlyList<Account> GetReadOnlyCollection(Account account)
    {
      if (account == null)
        return (IReadOnlyList<Account>) new List<Account>().AsReadOnly();
      return (IReadOnlyList<Account>) new List<Account>()
      {
        account
      }.AsReadOnly();
    }

    internal static AccountStoreChangedEventArgs CreateForAddingAccount(Account account) => new AccountStoreChangedEventArgs()
    {
      Added = AccountStoreChangedEventArgs.GetReadOnlyCollection(account)
    };

    internal static AccountStoreChangedEventArgs CreateForRemovingAccount(Account account) => new AccountStoreChangedEventArgs()
    {
      Removed = AccountStoreChangedEventArgs.GetReadOnlyCollection(account)
    };

    internal static AccountStoreChangedEventArgs CreateForModifyingAccount(Account afterAccount) => new AccountStoreChangedEventArgs()
    {
      Modified = (IReadOnlyList<Tuple<Account, Account>>) new List<Tuple<Account, Account>>()
      {
        Tuple.Create<Account, Account>((Account) null, afterAccount)
      }.AsReadOnly()
    };
  }
}
