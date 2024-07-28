// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Client.AccountManagement.RegistryAccountStore
// Assembly: Microsoft.VisualStudio.Services.Client.Interactive, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 00B1FD41-439C-4B93-A417-9D1E4874E657
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Client.Interactive.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;

namespace Microsoft.VisualStudio.Services.Client.AccountManagement
{
  internal class RegistryAccountStore : IAccountStore, IDisposable
  {
    internal const string KeychainKeyName = "Keychain";
    internal const string AccountsKeyName = "Accounts";
    internal const string DefaultGlobalMutexName = "RegistryAccountStore";
    private const string RegistryChangedSentinel = "AccountsChanged";
    private const string PropertiesKeyName = "Properties";
    private const string ProviderIdKeyName = "ProviderId";
    private const string UniqueIdKeyName = "UniqueId";
    private const string ListOfSupportedProvidersName = "ListOfSupportedProviders";
    private const string AuthenticatorName = "Authenticator";
    private const string DisplayInfoDisplayName = "AccountDisplayName";
    private const string DisplayInfoAccountLogoName = "AccountLogo";
    private const string DisplayInfoProviderDisplayName = "ProviderDisplayName";
    private const string DisplayInfoUserName = "UserName";
    private const string DisplayInfoProviderLogoName = "ProviderLogo";
    private const string NeedsReauthenticationName = "NeedsReAuthentication";
    private const int DefaultAccountVersion = 1;
    private IReadOnlyCollection<Account> originalMemoryStore;
    private volatile IReadOnlyCollection<Account> memoryStore;
    private AccountRegistryWatcher watcher;
    private int currentAccountVersion = 1;
    private object syncObj = new object();
    private object watcherLock = new object();
    private bool disposed;

    public event EventHandler<AccountStoreChangedEventArgs> KeychainAccountStoreChanged;

    public event EventHandler<AccountStoreChangedEventArgs> KeychainAccountStoreChanging;

    internal RegistryAccountStore(string instanceName)
      : this((string) null, (string) null, instanceName)
    {
    }

    internal RegistryAccountStore(string registryRoot = null, string globalMutexName = null, string instanceName = null)
    {
      if (string.IsNullOrWhiteSpace(registryRoot))
      {
        this.RegistryRoot = Path.Combine("Software\\Microsoft\\VSCommon", "Keychain");
        if (!string.IsNullOrEmpty(instanceName))
          this.RegistryRoot = Path.Combine("Software\\Microsoft\\VSCommon", instanceName, "Keychain");
      }
      else
      {
        this.RegistryRoot = registryRoot;
        if (!string.IsNullOrWhiteSpace(instanceName))
          this.RegistryRoot = Path.Combine(this.RegistryRoot, instanceName);
      }
      this.RegistryRoot = Path.Combine(this.RegistryRoot, "Accounts");
      this.GlobalMutexName = !string.IsNullOrWhiteSpace(globalMutexName) ? globalMutexName : nameof (RegistryAccountStore);
      this.GlobalMutexName = string.IsNullOrEmpty(instanceName) ? this.GlobalMutexName : this.GlobalMutexName + instanceName;
      AccountManagementUtilities.ExecuteActionInGlobalMutex(this.GlobalMutexName, (Action) (() =>
      {
        using (RegistryKey rootKey = RegistryAccountStore.GetOrCreateRootKey(this.RegistryRoot))
        {
          this.RefreshCurrentMemoryStore(rootKey);
          this.originalMemoryStore = this.memoryStore;
        }
      }));
      this.watcher = new AccountRegistryWatcher(RegistryHive.CurrentUser, this.RegistryRoot, false, RegistryChangeNotificationFilter.ValueChange);
      this.watcher.KeyChanged = new Action(this.OnKeyChanged);
      this.watcher.Error = new Action<Exception>(this.OnWatcherError);
      this.watcher.Start();
    }

    internal string RegistryRoot { get; private set; }

    internal string GlobalMutexName { get; private set; }

    private void OnKeyChanged()
    {
      if (!AccountManagementUtilities.ExecuteInGlobalMutex<bool>(this.GlobalMutexName, new Func<bool>(this.OnKeyChangedCore)))
        return;
      this.RaiseChangedEvent();
    }

    private bool OnKeyChangedCore()
    {
      using (RegistryKey rootKey = RegistryAccountStore.GetOrCreateRootKey(this.RegistryRoot))
      {
        if (Convert.ToInt32(rootKey.GetValue("AccountsChanged", (object) null), (IFormatProvider) CultureInfo.InvariantCulture) == this.currentAccountVersion)
          return false;
        this.RefreshCurrentMemoryStore(rootKey);
        return true;
      }
    }

    private void OnWatcherError(Exception ex)
    {
    }

    public IReadOnlyCollection<Account> GetAllAccounts() => this.memoryStore;

    public Account AddOrUpdateAccount(Account account)
    {
      ArgumentUtility.CheckForNull<Account>(account, "acount");
      this.RaiseChangingEvent(AccountStoreChangedEventArgs.CreateForAddingAccount(account));
      AccountManagementUtilities.ExecuteActionInGlobalMutex(this.GlobalMutexName, (Action) (() => this.AddOrUpdateAccountCore(account)));
      this.RaiseChangedEvent();
      return this.GetAccount((AccountKey) account);
    }

    private void AddOrUpdateAccountCore(Account account)
    {
      Account accountFromKey = this.GetAccountFromKey((AccountKey) account);
      using (RegistryKey rootKey = RegistryAccountStore.GetOrCreateRootKey(this.RegistryRoot))
      {
        using (RegistryKey accountKey = RegistryAccountStore.GetAccountKey(rootKey, (AccountKey) account, true))
          RegistryAccountStore.WriteAccountInformation(accountKey, account, accountFromKey);
        this.currentAccountVersion = AccountManagementUtilities.IncrementRegistryValue(rootKey, "AccountsChanged");
        this.RefreshCurrentMemoryStore(rootKey);
      }
    }

    private void RefreshCurrentMemoryStore(RegistryKey rootKey)
    {
      ArgumentUtility.CheckForNull<RegistryKey>(rootKey, nameof (rootKey));
      int num = (int) rootKey.GetValue("AccountsChanged", (object) 0);
      IReadOnlyCollection<Account> keychainAcccounts = RegistryAccountStore.GetKeychainAcccounts(rootKey);
      this.currentAccountVersion = num;
      this.memoryStore = keychainAcccounts;
    }

    public Account SetDisplayInfo(AccountKey account, AccountDisplayInfo info)
    {
      ArgumentUtility.CheckForNull<AccountKey>(account, nameof (account));
      ArgumentUtility.CheckForNull<AccountDisplayInfo>(info, nameof (info));
      this.RaiseChangingEvent(AccountStoreChangedEventArgs.CreateForModifyingAccount(this.GetAccount(account)));
      Account account1 = AccountManagementUtilities.ExecuteInGlobalMutex<Account>(this.GlobalMutexName, (Func<Account>) (() => this.SetDisplayInfoCore(account, info)));
      this.RaiseChangedEvent();
      return account1;
    }

    private Account SetDisplayInfoCore(AccountKey account, AccountDisplayInfo info)
    {
      using (RegistryKey rootKey = RegistryAccountStore.GetOrCreateRootKey(this.RegistryRoot))
      {
        using (RegistryKey accountKey = RegistryAccountStore.GetAccountKey(rootKey, account, false))
        {
          if (accountKey == null)
            return (Account) null;
          RegistryAccountStore.WriteDisplayInfo(accountKey, info);
        }
        this.currentAccountVersion = AccountManagementUtilities.IncrementRegistryValue(rootKey, "AccountsChanged");
        this.RefreshCurrentMemoryStore(rootKey);
      }
      return this.GetAccount(account);
    }

    public Account SetProperty(AccountKey account, string key, string value)
    {
      ArgumentUtility.CheckForNull<AccountKey>(account, nameof (account));
      ArgumentUtility.CheckStringForNullOrEmpty(key, nameof (key));
      return this.SetProperties(account, (IDictionary<string, string>) new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
      {
        {
          key,
          value
        }
      });
    }

    public Account SetProperties(AccountKey account, IDictionary<string, string> properties)
    {
      ArgumentUtility.CheckForNull<AccountKey>(account, nameof (account));
      ArgumentUtility.CheckForNull<IDictionary<string, string>>(properties, nameof (properties));
      this.RaiseChangingEvent(AccountStoreChangedEventArgs.CreateForModifyingAccount(this.GetAccount(account)));
      if (!AccountManagementUtilities.ExecuteInGlobalMutex<bool>(this.GlobalMutexName, (Func<bool>) (() => this.SetPropertiesCore(account, properties))))
        return (Account) null;
      this.RaiseChangedEvent();
      return this.GetAccount(account);
    }

    private bool SetPropertiesCore(AccountKey account, IDictionary<string, string> properties)
    {
      using (RegistryKey rootKey = RegistryAccountStore.GetOrCreateRootKey(this.RegistryRoot))
      {
        using (RegistryKey accountKey = RegistryAccountStore.GetAccountKey(rootKey, account, false))
        {
          if (accountKey == null)
            return false;
          using (RegistryKey propertyKey = RegistryAccountStore.GetPropertyKey(accountKey, false))
          {
            if (propertyKey == null)
              return false;
            foreach (KeyValuePair<string, string> property in (IEnumerable<KeyValuePair<string, string>>) properties)
              RegistryAccountStore.WriteAccountPropertyValue(propertyKey, property.Key, property.Value);
          }
        }
        this.currentAccountVersion = AccountManagementUtilities.IncrementRegistryValue(rootKey, "AccountsChanged");
        this.RefreshCurrentMemoryStore(rootKey);
      }
      return true;
    }

    private void RaiseChangingEvent(AccountStoreChangedEventArgs args)
    {
      EventHandler<AccountStoreChangedEventArgs> accountStoreChanging = this.KeychainAccountStoreChanging;
      if (accountStoreChanging == null)
        return;
      accountStoreChanging((object) this, args);
    }

    private void RaiseChangedEvent()
    {
      IReadOnlyCollection<Account> source1 = this.originalMemoryStore;
      IReadOnlyCollection<Account> source2 = (IReadOnlyCollection<Account>) null;
      lock (this.syncObj)
      {
        source2 = this.memoryStore;
        this.originalMemoryStore = this.memoryStore;
        if (source1 == null)
          source1 = (IReadOnlyCollection<Account>) new List<Account>().AsReadOnly();
        if (source2 == null)
          source1 = (IReadOnlyCollection<Account>) new List<Account>().AsReadOnly();
      }
      Dictionary<AccountKey, Account> dictionary1 = source2.ToDictionary<Account, AccountKey, Account>((Func<Account, AccountKey>) (x => (AccountKey) x), (Func<Account, Account>) (x => x), (IEqualityComparer<AccountKey>) AccountKey.KeyComparer);
      Dictionary<AccountKey, Account> dictionary2 = source1.ToDictionary<Account, AccountKey, Account>((Func<Account, AccountKey>) (x => (AccountKey) x), (Func<Account, Account>) (x => x), (IEqualityComparer<AccountKey>) AccountKey.KeyComparer);
      List<Account> accountList1 = new List<Account>();
      List<Account> accountList2 = new List<Account>();
      List<Tuple<Account, Account>> tupleList = new List<Tuple<Account, Account>>();
      foreach (Account account in (IEnumerable<Account>) source1)
      {
        Account accountB = (Account) null;
        if (dictionary1.TryGetValue((AccountKey) account, out accountB))
        {
          if (!Account.AccountMemberComparer.Equals(account, accountB))
            tupleList.Add(Tuple.Create<Account, Account>(account, accountB));
        }
        else
          accountList2.Add(account);
      }
      foreach (Account key in (IEnumerable<Account>) source2)
      {
        if (!dictionary2.ContainsKey((AccountKey) key))
          accountList1.Add(key);
      }
      if (accountList1.Count + accountList2.Count + tupleList.Count <= 0)
        return;
      EventHandler<AccountStoreChangedEventArgs> eventHandler = Interlocked.CompareExchange<EventHandler<AccountStoreChangedEventArgs>>(ref this.KeychainAccountStoreChanged, (EventHandler<AccountStoreChangedEventArgs>) null, (EventHandler<AccountStoreChangedEventArgs>) null);
      if (eventHandler == null)
        return;
      AccountStoreChangedEventArgs e = new AccountStoreChangedEventArgs((IReadOnlyList<Account>) accountList1.AsReadOnly(), (IReadOnlyList<Account>) accountList2.AsReadOnly(), (IReadOnlyList<Tuple<Account, Account>>) tupleList.AsReadOnly());
      eventHandler((object) this, e);
    }

    public void RemoveAccount(AccountKey key)
    {
      ArgumentUtility.CheckForNull<AccountKey>(key, nameof (key));
      this.RaiseChangingEvent(AccountStoreChangedEventArgs.CreateForRemovingAccount(this.GetAccount(key)));
      AccountManagementUtilities.ExecuteActionInGlobalMutex(this.GlobalMutexName, (Action) (() =>
      {
        using (RegistryKey rootKey = RegistryAccountStore.GetOrCreateRootKey(this.RegistryRoot))
        {
          string accountSubKeyName = RegistryAccountStore.GetAccountSubKeyName(rootKey, key);
          if (accountSubKeyName != null)
            rootKey.DeleteSubKeyTree(accountSubKeyName, false);
          this.currentAccountVersion = AccountManagementUtilities.IncrementRegistryValue(rootKey, "AccountsChanged");
          this.RefreshCurrentMemoryStore(rootKey);
        }
      }));
      this.RaiseChangedEvent();
    }

    private static void ThrowExceptionCouldNotCreateSubKey(string parentKey, string subkey) => throw new AccountStorageException(ClientResources.RegistryAccountStoreCannotCreateSubKey((object) parentKey, (object) subkey));

    private Account GetAccount(AccountKey accountToFind)
    {
      foreach (Account allAccount in (IEnumerable<Account>) this.GetAllAccounts())
      {
        if (AccountKey.KeyComparer.Equals((AccountKey) allAccount, accountToFind))
          return allAccount;
      }
      return (Account) null;
    }

    private static void WriteProperties(
      RegistryKey accountKey,
      IEnumerable<KeyValuePair<string, string>> properties)
    {
      using (RegistryKey propertyKey = RegistryAccountStore.GetPropertyKey(accountKey, true))
      {
        if (propertyKey == null)
          return;
        foreach (KeyValuePair<string, string> property in properties)
        {
          if (!string.IsNullOrWhiteSpace(property.Key))
            RegistryAccountStore.WriteAccountPropertyValue(propertyKey, property.Key, property.Value);
        }
      }
    }

    private static IReadOnlyDictionary<string, string> ReadProperties(RegistryKey accountKey)
    {
      Dictionary<string, string> dictionary = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      using (RegistryKey propertyKey = RegistryAccountStore.GetPropertyKey(accountKey, false))
      {
        if (propertyKey != null)
        {
          foreach (string valueName in propertyKey.GetValueNames())
          {
            string str = RegistryAccountStore.ReadAccountPropertyValue(propertyKey, valueName);
            dictionary[valueName] = str;
          }
        }
      }
      return (IReadOnlyDictionary<string, string>) new ReadOnlyDictionary<string, string>((IDictionary<string, string>) dictionary);
    }

    private static Account ReadAccountInformation(RegistryKey accountKey)
    {
      string input = accountKey.GetValue("ProviderId", (object) null) as string;
      string str1 = accountKey.GetValue("UniqueId", (object) null) as string;
      string[] source = accountKey.GetValue("ListOfSupportedProviders", (object) null) as string[];
      string str2 = accountKey.GetValue("Authenticator", (object) null) as string;
      bool boolean = Convert.ToBoolean(accountKey.GetValue("NeedsReAuthentication", (object) 0));
      AccountDisplayInfo accountDisplayInfo = RegistryAccountStore.ReadDisplayInfo(accountKey);
      IReadOnlyDictionary<string, string> readOnlyDictionary = RegistryAccountStore.ReadProperties(accountKey);
      Guid result1 = Guid.Empty;
      Guid.TryParse(input, out result1);
      try
      {
        RegistryAccountStore.ThrowMemberNotValid(input != null, "ProviderId", accountKey.Name);
        RegistryAccountStore.ThrowMemberNotValid(str1 != null, "uniqueId", accountKey.Name);
        RegistryAccountStore.ThrowMemberNotValid(source != null, "listOfProviders", accountKey.Name);
        RegistryAccountStore.ThrowMemberNotValid(str2 != null, "authenticator", accountKey.Name);
        RegistryAccountStore.ThrowMemberNotValid(result1 != Guid.Empty, "properties", accountKey.Name);
        Guid result2;
        return new Account(new AccountInitializationData()
        {
          Authenticator = str2,
          ParentProviderId = result1,
          UniqueId = str1,
          DisplayInfo = accountDisplayInfo,
          SupportedAccountProviders = (IReadOnlyList<Guid>) ((IEnumerable<string>) source).Select<string, Guid>((Func<string, Guid>) (x => Guid.TryParse(x, out result2) ? result2 : Guid.Empty)).Where<Guid>((Func<Guid, bool>) (x => x != Guid.Empty)).ToList<Guid>().AsReadOnly(),
          Properties = readOnlyDictionary,
          NeedsReauthentication = boolean
        });
      }
      catch (AccountStorageException ex)
      {
        return (Account) null;
      }
    }

    private static void ThrowMemberNotValid(
      bool memberValid,
      string memberName,
      string registryLocation)
    {
      if (!memberValid)
        throw new AccountStorageException(ClientResources.RegistryAccountStoreMemberNotValid((object) memberName, (object) registryLocation));
    }

    private static void WriteAccountInformation(
      RegistryKey accountKey,
      Account account,
      Account existingAccount)
    {
      ArgumentUtility.CheckForNull<RegistryKey>(accountKey, nameof (accountKey));
      ArgumentUtility.CheckForNull<Account>(account, nameof (account));
      Dictionary<string, string> properties = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      if (existingAccount != null && existingAccount.Properties != null)
      {
        foreach (KeyValuePair<string, string> property in (IEnumerable<KeyValuePair<string, string>>) existingAccount.Properties)
          properties[property.Key] = property.Value;
      }
      foreach (KeyValuePair<string, string> property in (IEnumerable<KeyValuePair<string, string>>) account.Properties)
        properties[property.Key] = property.Value;
      accountKey.SetValue("ProviderId", (object) account.ProviderId, RegistryValueKind.String);
      accountKey.SetValue("UniqueId", (object) account.UniqueId, RegistryValueKind.String);
      accountKey.SetValue("ListOfSupportedProviders", (object) account.SupportedAccountProviders.Select<Guid, string>((Func<Guid, string>) (x => x.ToString("N"))).ToArray<string>(), RegistryValueKind.MultiString);
      accountKey.SetValue("Authenticator", (object) account.Authenticator, RegistryValueKind.String);
      RegistryAccountStore.WriteNeedsReauthentication(accountKey, account.NeedsReauthentication);
      RegistryAccountStore.WriteProperties(accountKey, (IEnumerable<KeyValuePair<string, string>>) properties);
      RegistryAccountStore.WriteDisplayInfo(accountKey, account.DisplayInfo);
    }

    private static void WriteDisplayInfo(RegistryKey accountKey, AccountDisplayInfo displayInfo)
    {
      accountKey.SetValue("AccountDisplayName", (object) displayInfo.AccountDisplayName, RegistryValueKind.String);
      accountKey.SetValue("ProviderDisplayName", (object) displayInfo.ProviderDisplayName, RegistryValueKind.String);
      accountKey.SetValue("UserName", (object) displayInfo.UserName, RegistryValueKind.String);
      if (displayInfo.AccountLogo != null)
        accountKey.SetValue("AccountLogo", (object) displayInfo.AccountLogo, RegistryValueKind.Binary);
      if (displayInfo.ProviderLogo == null)
        return;
      accountKey.SetValue("ProviderLogo", (object) displayInfo.ProviderLogo, RegistryValueKind.Binary);
    }

    private static AccountDisplayInfo ReadDisplayInfo(RegistryKey accountKey)
    {
      string accountDisplayName = accountKey.GetValue("AccountDisplayName", (object) null) as string;
      string providerDisplayName = accountKey.GetValue("ProviderDisplayName", (object) null) as string;
      string userName = accountKey.GetValue("UserName", (object) null) as string;
      byte[] accountLogo = accountKey.GetValue("AccountLogo", (object) null) as byte[];
      byte[] providerLogo = accountKey.GetValue("ProviderLogo", (object) null) as byte[];
      RegistryAccountStore.ThrowMemberNotValid(accountDisplayName != null, "displayName", accountKey.Name);
      RegistryAccountStore.ThrowMemberNotValid(providerDisplayName != null, "providerDisplayName", accountKey.Name);
      RegistryAccountStore.ThrowMemberNotValid(userName != null, "userName", accountKey.Name);
      return new AccountDisplayInfo(accountDisplayName, providerDisplayName, userName, accountLogo, providerLogo);
    }

    private static void WriteAccountPropertyValue(
      RegistryKey propertyKey,
      string name,
      string value)
    {
      if (value == null)
        propertyKey.DeleteValue(name, false);
      else
        propertyKey.SetValue(name.ToUpperInvariant(), (object) value, RegistryValueKind.String);
    }

    private static string ReadAccountPropertyValue(RegistryKey propertyKey, string propertyName) => Convert.ToString(propertyKey.GetValue(propertyName, (object) null), (IFormatProvider) CultureInfo.InvariantCulture);

    internal static RegistryKey GetOrCreateRootKey(string subKey)
    {
      RegistryKey subKey1 = Registry.CurrentUser.CreateSubKey(subKey, RegistryKeyPermissionCheck.Default, RegistryOptions.None);
      if (subKey1 != null)
        return subKey1;
      RegistryAccountStore.ThrowExceptionCouldNotCreateSubKey(Registry.CurrentUser.Name, subKey);
      return subKey1;
    }

    private static RegistryKey GetPropertyKey(RegistryKey accountKey, bool createIfNotFound)
    {
      RegistryKey propertyKey = accountKey.OpenSubKey("Properties", true);
      if (propertyKey == null & createIfNotFound)
        propertyKey = accountKey.CreateSubKey("Properties", RegistryKeyPermissionCheck.ReadWriteSubTree);
      return propertyKey;
    }

    private static string GetAccountSubKeyName(RegistryKey root, AccountKey account)
    {
      foreach (string subKeyName in root.GetSubKeyNames())
      {
        using (RegistryKey registryKey = root.OpenSubKey(subKeyName, true))
        {
          if (registryKey != null)
          {
            Guid empty = Guid.Empty;
            string input = registryKey.GetValue("ProviderId", (object) null) as string;
            string uniqueId = registryKey.GetValue("UniqueId", (object) null) as string;
            ref Guid local = ref empty;
            if (Guid.TryParse(input, out local))
            {
              if (!string.IsNullOrWhiteSpace(uniqueId))
              {
                AccountKey y = new AccountKey(uniqueId, empty);
                if (AccountKey.KeyComparer.Equals(account, y))
                  return subKeyName;
              }
            }
          }
        }
      }
      return (string) null;
    }

    private static RegistryKey GetAccountKey(
      RegistryKey root,
      AccountKey account,
      bool createIfNotFound)
    {
      ArgumentUtility.CheckForNull<RegistryKey>(root, nameof (root));
      string accountSubKeyName = RegistryAccountStore.GetAccountSubKeyName(root, account);
      if (accountSubKeyName != null)
      {
        RegistryKey accountKey = root.OpenSubKey(accountSubKeyName, true);
        if (accountKey != null)
          return accountKey;
      }
      if (!createIfNotFound)
        return (RegistryKey) null;
      string subkey = Guid.NewGuid().ToString("N");
      return root.CreateSubKey(subkey, RegistryKeyPermissionCheck.ReadWriteSubTree);
    }

    private static IReadOnlyCollection<Account> GetKeychainAcccounts(RegistryKey root)
    {
      string[] subKeyNames = root.GetSubKeyNames();
      List<Account> accountList = new List<Account>();
      foreach (string name in subKeyNames)
      {
        using (RegistryKey accountKey = root.OpenSubKey(name, true))
        {
          Account account = RegistryAccountStore.ReadAccountInformation(accountKey);
          if (account != null)
            accountList.Add(account);
        }
      }
      return (IReadOnlyCollection<Account>) accountList.AsReadOnly();
    }

    public Account SetNeedsReauthentication(AccountKey account, bool value)
    {
      ArgumentUtility.CheckForNull<AccountKey>(account, nameof (account));
      Account account1 = AccountManagementUtilities.ExecuteInGlobalMutex<Account>(this.GlobalMutexName, (Func<Account>) (() => this.SetNeedsReAuthenticationCore(account, value)));
      this.RaiseChangedEvent();
      return account1;
    }

    private Account SetNeedsReAuthenticationCore(AccountKey account, bool value)
    {
      using (RegistryKey rootKey = RegistryAccountStore.GetOrCreateRootKey(this.RegistryRoot))
      {
        using (RegistryKey accountKey = RegistryAccountStore.GetAccountKey(rootKey, account, false))
        {
          if (accountKey != null)
            RegistryAccountStore.WriteNeedsReauthentication(accountKey, value);
        }
        this.currentAccountVersion = AccountManagementUtilities.IncrementRegistryValue(rootKey, "AccountsChanged");
        this.RefreshCurrentMemoryStore(rootKey);
      }
      return this.GetAccount(account);
    }

    private static void WriteNeedsReauthentication(RegistryKey accountKey, bool value) => accountKey.SetValue("NeedsReAuthentication", (object) value, RegistryValueKind.DWord);

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    protected virtual void Dispose(bool disposing)
    {
      if (this.disposed)
        return;
      if (disposing && this.watcher != null)
      {
        lock (this.watcherLock)
        {
          if (this.watcher != null)
          {
            this.watcher.Dispose();
            this.watcher = (AccountRegistryWatcher) null;
          }
        }
      }
      this.disposed = true;
    }
  }
}
