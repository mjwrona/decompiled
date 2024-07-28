// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Admin.AccountInfo
// Assembly: Microsoft.VisualStudio.Services.Configuration, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AB461A1-8255-4EAB-B12B-E1D379571DC1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Configuration.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Principal;

namespace Microsoft.TeamFoundation.Admin
{
  public abstract class AccountInfo : IAccount
  {
    private static readonly char[] s_illegalIdentityNameChars = new char[47]
    {
      char.MinValue,
      '\u0001',
      '\u0002',
      '\u0003',
      '\u0004',
      '\u0005',
      '\u0006',
      '\a',
      '\b',
      '\t',
      '\n',
      '\v',
      '\f',
      '\r',
      '\u000E',
      '\u000F',
      '\u0010',
      '\u0011',
      '\u0012',
      '\u0013',
      '\u0014',
      '\u0015',
      '\u0016',
      '\u0017',
      '\u0018',
      '\u0019',
      '\u001A',
      '\u001B',
      '\u001C',
      '\u001D',
      '\u001E',
      '\u001F',
      '"',
      '/',
      '\\',
      '[',
      ']',
      ':',
      '|',
      '<',
      '>',
      '+',
      '=',
      ';',
      '?',
      '*',
      ','
    };
    private static readonly object s_lock = new object();
    private SecurityIdentifier m_sid;
    private AccountType? m_typeHint;
    private string m_providedName;
    private string m_fullName;
    private SecureString m_password;
    private static readonly SecurityIdentifier s_localSystemSid = new SecurityIdentifier(WellKnownSidType.LocalSystemSid, (SecurityIdentifier) null);
    private static readonly SecurityIdentifier s_networkServiceSid = new SecurityIdentifier(WellKnownSidType.NetworkServiceSid, (SecurityIdentifier) null);
    private static readonly SecurityIdentifier s_localServiceSid = new SecurityIdentifier(WellKnownSidType.LocalServiceSid, (SecurityIdentifier) null);
    private static SecurityIdentifier s_iisUsrsSid;
    private static string s_networkServiceName;
    private static string s_localServiceName;
    private static string s_localSystemName;
    private static string s_injectedMachineAccount;
    private static string s_currentUser = UserNameUtil.CurrentUserName;
    private const string c_localHost = "localhost";

    protected AccountInfo()
      : this(AccountType.NetworkService, (string) null, (SecureString) null)
    {
    }

    protected AccountInfo(string account)
      : this(AccountType.User, account, (SecureString) null, false)
    {
    }

    protected AccountInfo(AccountType type, string account, SecureString password)
      : this(type, account, password, true)
    {
    }

    protected AccountInfo(
      AccountType type,
      string account,
      SecureString password,
      bool requirePassword)
    {
      this.updateType(type);
      if (type == AccountType.User || type == AccountType.AcsServiceIdentity)
      {
        this.updateAccount(account);
        if (requirePassword && (password == null || password.Length == 0))
          throw new ConfigurationException(ConfigurationResources.MissingPasswordForAccount((object) account));
      }
      this.m_password = password;
    }

    public static AccountInfo Current => (AccountInfo) CurrentAccountInfo.Instance;

    public static string NonLocalizedLocalSystemName => "NT Authority\\System";

    public static string NonLocalizedNetworkServiceName => "NT Authority\\NetworkService";

    public static string NonLocalizedLocalServiceName => "NT Authority\\LocalService";

    public static string NonLocalizedBuiltinAdministrators => "Builtin\\Administrators";

    public static AccountInfo LocalSystem => (AccountInfo) new GenericAccountInfo(AccountInfo.NonLocalizedLocalSystemName);

    public static AccountInfo NetworkService => (AccountInfo) new GenericAccountInfo(AccountInfo.NonLocalizedNetworkServiceName);

    public static AccountInfo LocalService => (AccountInfo) new GenericAccountInfo(AccountInfo.NonLocalizedLocalServiceName);

    public static SecurityIdentifier IisUsrsSid
    {
      get
      {
        if (AccountInfo.s_iisUsrsSid == (SecurityIdentifier) null)
          AccountInfo.s_iisUsrsSid = new SecurityIdentifier("S-1-5-32-568");
        return AccountInfo.s_iisUsrsSid;
      }
    }

    public static bool IsRemoteNetworkServiceAccount(string accountName) => accountName != null && accountName.EndsWith("$", StringComparison.Ordinal);

    public static string ResolveFullName(string accountName) => !string.IsNullOrEmpty(accountName) ? AccountInfo.AccountDetailsCache.Find(accountName).FullName : accountName;

    public static string ServiceAccountName(IAccount serviceAccount, string machine) => AccountInfo.ServiceAccountName((serviceAccount as AccountInfo).FullName, machine);

    public static string ServiceAccountName(string fullName, string machine)
    {
      if (string.IsNullOrEmpty(fullName) || string.IsNullOrEmpty(machine) || AccountInfo.GetAccountType(fullName) != AccountType.NetworkService || ComputerInfo.IsLocalMachine(machine))
        return fullName;
      return string.IsNullOrEmpty(AccountInfo.s_injectedMachineAccount) ? UserNameUtil.CurrentMachineAccountName : AccountInfo.s_injectedMachineAccount;
    }

    public static void InjectFakeCacheItem(string fullName)
    {
      AdminTraceLogger.Default.Heading2(nameof (InjectFakeCacheItem));
      AccountInfo.AccountDetailsCache.AddCacheItem(fullName, (SecurityIdentifier) null);
      AdminTraceLogger.Default.Verbose("injected");
    }

    public static AccountType GetValidAccountTypes()
    {
      AccountType accountType = AccountType.User;
      return !EnvironmentHandler.IsMachineInWorkgroup() ? accountType | AccountType.NetworkService : accountType | AccountType.LocalService;
    }

    private void updateAccount(string newAccount)
    {
      AccountInfo.AccountDetailsCacheEntry details = new AccountInfo.AccountDetailsCacheEntry();
      if (!string.IsNullOrEmpty(newAccount) && AccountInfo.IsLegalIdentity(newAccount))
        details = AccountInfo.AccountDetailsCache.Find(newAccount);
      this.setAccount(details);
    }

    private void setAccount(AccountInfo.AccountDetailsCacheEntry details)
    {
      AccountType? typeHint1 = this.m_typeHint;
      AccountType accountType1 = AccountType.User;
      if (!(typeHint1.GetValueOrDefault() == accountType1 & typeHint1.HasValue))
      {
        AccountType? typeHint2 = this.m_typeHint;
        AccountType accountType2 = AccountType.AcsServiceIdentity;
        if (!(typeHint2.GetValueOrDefault() == accountType2 & typeHint2.HasValue))
          this.m_typeHint = new AccountType?();
      }
      this.m_sid = details.Sid;
      this.m_fullName = details.FullName;
    }

    private void updateType(AccountType type)
    {
      if (this.AccountType == type)
        return;
      this.clearDetails();
      switch (type)
      {
        case AccountType.None:
        case AccountType.User:
        case AccountType.Computer:
        case AccountType.AcsServiceIdentity:
          this.m_typeHint = new AccountType?(type);
          break;
        case AccountType.LocalSystem:
          this.setAccount(AccountInfo.AccountDetailsCache.Find(AccountInfo.LocalSystemName));
          break;
        case AccountType.LocalService:
          this.setAccount(AccountInfo.AccountDetailsCache.Find(AccountInfo.LocalServiceName));
          break;
        case AccountType.NetworkService:
          this.setAccount(AccountInfo.AccountDetailsCache.Find(AccountInfo.NetworkServiceName));
          break;
      }
    }

    private void clearDetails()
    {
      if (this.m_fullName != null)
        this.setAccount(new AccountInfo.AccountDetailsCacheEntry());
      if (this.Password != null)
        this.Password.Clear();
      this.m_typeHint = new AccountType?();
    }

    public string FullName
    {
      get => this.m_fullName;
      set => this.updateAccount(value);
    }

    public string ProvidedName
    {
      get => this.m_providedName;
      set
      {
        this.m_providedName = value;
        this.FullName = value;
      }
    }

    public bool IsLocalAccount => VssStringComparer.Hostname.Equals(this.Domain, ComputerInfo.MachineName);

    public bool IsCurrentUser => VssStringComparer.UserName.Equals(this.FullName, AccountInfo.s_currentUser);

    public bool IsInternetIdentity => this.IsLocalAccount && this.CheckInternetIdentity(this.UserName, out string _, out string _);

    public string InternetPrincipalName
    {
      get
      {
        string principalName;
        return this.IsLocalAccount && this.CheckInternetIdentity(this.UserName, out principalName, out string _) ? principalName : (string) null;
      }
    }

    public string InternetProviderName
    {
      get
      {
        string providerName;
        return this.IsLocalAccount && this.CheckInternetIdentity(this.UserName, out string _, out providerName) ? providerName : (string) null;
      }
    }

    public string SidAsString => this.m_sid != (SecurityIdentifier) null ? this.m_sid.Value : (string) null;

    public SecurityIdentifier SecurityIdentifier => this.m_sid;

    public string GetDatabaseAccount(bool isDualServer) => !(this.IsServiceAccount & isDualServer) ? this.FullName : (string.IsNullOrEmpty(AccountInfo.s_injectedMachineAccount) ? UserNameUtil.CurrentMachineAccountName : AccountInfo.s_injectedMachineAccount);

    public string GetDatabaseAccount(string dataSource)
    {
      AdminTraceLogger.Default.Verbose("GetDatabaseAccount: {0}", (object) dataSource);
      string databaseAccount = this.GetDatabaseAccount(!ComputerInfo.IsLocalMachine(AccountInfo.GetSqlInstanceComputerName(dataSource)));
      AdminTraceLogger.Default.Verbose("databaseAccount: {0}", (object) databaseAccount);
      return databaseAccount;
    }

    public AccountType AccountType
    {
      get
      {
        if (this.IsNetworkService)
          return AccountType.NetworkService;
        if (this.IsLocalService)
          return AccountType.LocalService;
        if (this.IsLocalSystem)
          return AccountType.LocalSystem;
        if (AccountInfo.IsRemoteNetworkServiceAccount(this.FullName))
          return AccountType.Computer;
        if (this.m_typeHint.HasValue)
          return this.m_typeHint.Value;
        return string.IsNullOrEmpty(this.FullName) ? AccountType.None : AccountType.User;
      }
      set => this.updateType(value);
    }

    public SecureString Password
    {
      get => this.m_password;
      set
      {
        AdminTraceLogger.Default.Verbose("setting password");
        this.m_password = value;
      }
    }

    public string PasswordAsPlainText
    {
      get => StringUtil.GetSecureStringContent(this.Password);
      set
      {
        AdminTraceLogger.Default.Verbose("setting password");
        this.Password = StringUtil.CreateSecureString(value);
      }
    }

    public bool Supported => this.m_sid != (SecurityIdentifier) null;

    public static AccountType GetAccountType(string fullName)
    {
      if (string.IsNullOrEmpty(fullName))
        throw new ArgumentNullException(nameof (fullName));
      AccountType accountType = AccountType.User;
      AdminTraceLogger.Default.Verbose("getting account {0} from details cache", (object) fullName);
      AccountInfo.AccountDetailsCacheEntry detailsCacheEntry = AccountInfo.AccountDetailsCache.Find(fullName);
      if (detailsCacheEntry != null && detailsCacheEntry.Sid != (SecurityIdentifier) null)
      {
        if (detailsCacheEntry.Sid.Equals(AccountInfo.s_networkServiceSid))
          accountType = AccountType.NetworkService;
        else if (detailsCacheEntry.Sid.Equals(AccountInfo.s_localServiceSid))
          accountType = AccountType.LocalService;
        else if (detailsCacheEntry.Sid.Equals(AccountInfo.s_localSystemSid))
          accountType = AccountType.LocalSystem;
        else if (AccountInfo.IsRemoteNetworkServiceAccount(fullName))
          accountType = AccountType.Computer;
      }
      AdminTraceLogger.Default.Verbose("AccountType = {0}", (object) accountType.ToString());
      return accountType;
    }

    public bool IsServiceAccount => this.IsNetworkService || this.IsLocalService || this.IsLocalSystem;

    public bool IsLocalSystem => !(this.m_sid == (SecurityIdentifier) null) && this.m_sid.Equals(AccountInfo.s_localSystemSid);

    public bool IsNetworkService => !(this.m_sid == (SecurityIdentifier) null) && this.m_sid.Equals(AccountInfo.s_networkServiceSid);

    public bool IsLocalService => !(this.m_sid == (SecurityIdentifier) null) && this.m_sid.Equals(AccountInfo.s_localServiceSid);

    public bool IsComputerAccount => this.AccountType == AccountType.Computer;

    public bool IsSameAccountName(AccountInfo newAccount)
    {
      if (newAccount == null)
        return false;
      if (string.IsNullOrEmpty(this.FullName))
        return string.IsNullOrEmpty(newAccount.FullName);
      return !string.IsNullOrEmpty(newAccount.FullName) && VssStringComparer.AccountInfoAccount.Equals(this.FullName, newAccount.FullName);
    }

    public static bool IsServiceAccountName(string account)
    {
      bool flag = false;
      if (!string.IsNullOrEmpty(account))
      {
        AccountInfo.AccountDetailsCacheEntry detailsCacheEntry = AccountInfo.AccountDetailsCache.Find(account);
        if (detailsCacheEntry.Sid != (SecurityIdentifier) null)
          flag = detailsCacheEntry.Sid.Equals(AccountInfo.s_networkServiceSid) || detailsCacheEntry.Sid.Equals(AccountInfo.s_localServiceSid) || detailsCacheEntry.Sid.Equals(AccountInfo.s_localSystemSid);
      }
      return flag;
    }

    public bool IsValid => !string.IsNullOrEmpty(this.FullName) && AccountInfo.IsLegalIdentity(this.FullName);

    public string UserName
    {
      get
      {
        string userName = (string) null;
        if (!string.IsNullOrEmpty(this.FullName))
        {
          string fullName = this.FullName;
          int num = fullName.IndexOf('\\');
          if (num > 0 && num != fullName.Length - 1)
            userName = fullName.Substring(num + 1);
        }
        return userName;
      }
    }

    public string Domain
    {
      get
      {
        string domain = (string) null;
        if (!string.IsNullOrEmpty(this.FullName))
        {
          string fullName = this.FullName;
          int length = fullName.IndexOf('\\');
          if (length > 0 && length != fullName.Length - 1)
            domain = fullName.Substring(0, length);
        }
        return domain;
      }
    }

    private bool CheckInternetIdentity(
      string localUserAccountName,
      out string principalName,
      out string providerName)
    {
      principalName = (string) null;
      providerName = (string) null;
      Microsoft.TeamFoundation.Common.Internal.NativeMethods.USER_INFO_24 userInfo24 = new Microsoft.TeamFoundation.Common.Internal.NativeMethods.USER_INFO_24();
      IntPtr BufPtr;
      if (Microsoft.TeamFoundation.Common.Internal.NativeMethods.NetUserGetInfo((string) null, localUserAccountName, 24, out BufPtr) != Microsoft.TeamFoundation.Common.Internal.NativeMethods.NET_API_STATUS.NERR_Success)
        return false;
      Microsoft.TeamFoundation.Common.Internal.NativeMethods.USER_INFO_24 structure = (Microsoft.TeamFoundation.Common.Internal.NativeMethods.USER_INFO_24) Marshal.PtrToStructure(BufPtr, typeof (Microsoft.TeamFoundation.Common.Internal.NativeMethods.USER_INFO_24));
      int error = Microsoft.TeamFoundation.Common.Internal.NativeMethods.NetApiBufferFree(BufPtr);
      if (error != 0)
        throw new Win32Exception(error);
      principalName = structure.usri24_internet_principal_name;
      providerName = structure.usri24_internet_provider_name;
      return !string.IsNullOrEmpty(principalName) && !string.IsNullOrEmpty(providerName);
    }

    public static bool AreAccountNamesEqual(string existingAccount, string expectedAccount)
    {
      if (string.IsNullOrEmpty(existingAccount) || string.IsNullOrEmpty(expectedAccount))
        return false;
      if (VssStringComparer.AccountInfoAccount.Equals(existingAccount, expectedAccount))
        return true;
      try
      {
        string[] strArray1 = existingAccount.Split(new char[1]
        {
          '\\'
        }, 2);
        string[] strArray2 = expectedAccount.Split(new char[1]
        {
          '\\'
        }, 2);
        if (strArray1.Length != strArray2.Length)
          return false;
        if (strArray2.Length == 1 && VssStringComparer.AccountInfoAccount.Equals(strArray1[0], strArray2[0]))
          return true;
        if (VssStringComparer.AccountInfoAccount.Equals(strArray1[1], strArray2[1]))
        {
          if (!VssStringComparer.AccountInfoAccount.Equals(strArray1[0], ComputerInfo.MachineName))
          {
            if (!VssStringComparer.AccountInfoAccount.Equals(strArray2[0], ComputerInfo.MachineName))
              goto label_15;
          }
          if (!(strArray1[0] == "."))
          {
            if (!(strArray2[0] == "."))
              goto label_15;
          }
          return true;
        }
      }
      catch (IndexOutOfRangeException ex)
      {
      }
      catch (NullReferenceException ex)
      {
      }
label_15:
      return false;
    }

    public static bool IsLegalIdentity(string identity)
    {
      if (!string.IsNullOrEmpty(identity))
      {
        int num = identity.IndexOf('\\');
        if (num > 0 && num != identity.Length - 1)
          identity = identity.Substring(num + 1);
      }
      bool flag = !string.IsNullOrEmpty(identity) && identity.Length <= 256 && identity.IndexOfAny(AccountInfo.s_illegalIdentityNameChars) == -1;
      if (!flag)
        AdminTraceLogger.Default.Verbose("IsLegal: {0}", (object) flag);
      return flag;
    }

    private static string LoadOrReturnStringByName(ref string accountName, SecurityIdentifier sid)
    {
      if (accountName == null)
      {
        lock (AccountInfo.s_lock)
        {
          if (accountName == null)
            accountName = AccountInfo.GetNameFromSid(sid);
        }
      }
      return accountName;
    }

    public static bool IsUserValid(string fullName, string password)
    {
      GenericAccountInfo genericAccountInfo = new GenericAccountInfo(fullName);
      IntPtr tokenHandle = IntPtr.Zero;
      int num = Microsoft.TeamFoundation.Common.Internal.NativeMethods.LogonUser(genericAccountInfo.UserName, genericAccountInfo.Domain, password, Microsoft.TeamFoundation.Common.Internal.NativeMethods.LOGON32_LOGON_NETWORK, Microsoft.TeamFoundation.Common.Internal.NativeMethods.LOGON32_PROVIDER_DEFAULT, out tokenHandle);
      if (tokenHandle.ToInt32() != 0)
        Microsoft.TeamFoundation.Common.Internal.NativeMethods.CloseHandle(tokenHandle);
      return (uint) num > 0U;
    }

    public static string GetNonLocalizedAccountNameFromAccountType(AccountType type, string name)
    {
      switch (type)
      {
        case AccountType.None:
          return string.Empty;
        case AccountType.LocalSystem:
          return AccountInfo.NonLocalizedLocalSystemName;
        case AccountType.LocalService:
          return AccountInfo.NonLocalizedLocalServiceName;
        case AccountType.NetworkService:
          return AccountInfo.NonLocalizedNetworkServiceName;
        case AccountType.User:
        case AccountType.Computer:
        case AccountType.AcsServiceIdentity:
          return name;
        default:
          return string.Empty;
      }
    }

    private static string GetNameFromSid(SecurityIdentifier sid) => sid.Translate(typeof (NTAccount)).Value;

    private static string NetworkServiceName => AccountInfo.LoadOrReturnStringByName(ref AccountInfo.s_networkServiceName, AccountInfo.s_networkServiceSid);

    private static string LocalServiceName => AccountInfo.LoadOrReturnStringByName(ref AccountInfo.s_localServiceName, AccountInfo.s_localServiceSid);

    private static string LocalSystemName => AccountInfo.LoadOrReturnStringByName(ref AccountInfo.s_localSystemName, AccountInfo.s_localSystemSid);

    private static string GetSqlInstanceComputerName(string sqlInstanceAddress)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(sqlInstanceAddress, nameof (sqlInstanceAddress));
      string instanceComputerName = sqlInstanceAddress.Split('\\', ',')[0];
      if (instanceComputerName.StartsWith(FrameworkServerConstants.TcpProtocolPrefix, StringComparison.OrdinalIgnoreCase))
        instanceComputerName = instanceComputerName.Substring(FrameworkServerConstants.TcpProtocolPrefix.Length);
      else if (instanceComputerName.StartsWith(FrameworkServerConstants.NamedPipesProtocolPrefix, StringComparison.OrdinalIgnoreCase))
        instanceComputerName = instanceComputerName.Substring(FrameworkServerConstants.NamedPipesProtocolPrefix.Length);
      return instanceComputerName;
    }

    private class AccountDetailsCacheEntry
    {
      public AccountDetailsCacheEntry()
        : this((string) null, (SecurityIdentifier) null)
      {
      }

      public AccountDetailsCacheEntry(string fullName, SecurityIdentifier sid)
      {
        this.FullName = fullName;
        this.Sid = sid;
      }

      public string FullName { get; private set; }

      public SecurityIdentifier Sid { get; private set; }
    }

    private static class AccountDetailsCache
    {
      private static Dictionary<string, AccountInfo.AccountDetailsCacheEntry> m_cache = new Dictionary<string, AccountInfo.AccountDetailsCacheEntry>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      private static object s_locker = new object();

      static AccountDetailsCache()
      {
        AccountInfo.AccountDetailsCache.AddCacheItem(AccountInfo.NetworkServiceName, AccountInfo.s_networkServiceSid);
        AccountInfo.AccountDetailsCache.AddCacheItem(AccountInfo.LocalServiceName, AccountInfo.s_localServiceSid);
        AccountInfo.AccountDetailsCache.AddCacheItem(AccountInfo.LocalSystemName, AccountInfo.s_localSystemSid);
      }

      public static AccountInfo.AccountDetailsCacheEntry Find(string fullName)
      {
        ArgumentUtility.CheckStringForNullOrEmpty(fullName, nameof (fullName));
        lock (AccountInfo.AccountDetailsCache.s_locker)
          return AccountInfo.AccountDetailsCache.LoadOrCreate(fullName);
      }

      public static void AddCacheItem(string fullName, SecurityIdentifier sid)
      {
        lock (AccountInfo.AccountDetailsCache.s_locker)
          AccountInfo.AccountDetailsCache.m_cache[fullName] = new AccountInfo.AccountDetailsCacheEntry(fullName, sid);
      }

      private static AccountInfo.AccountDetailsCacheEntry LoadOrCreate(string fullName)
      {
        AccountInfo.AccountDetailsCacheEntry detailsCacheEntry;
        if (!AccountInfo.AccountDetailsCache.m_cache.TryGetValue(fullName, out detailsCacheEntry))
          detailsCacheEntry = AccountInfo.AccountDetailsCache.AddCacheItem(fullName);
        return detailsCacheEntry;
      }

      private static AccountInfo.AccountDetailsCacheEntry AddCacheItem(string fullName)
      {
        AccountInfo.AccountDetailsCacheEntry accountDetails = AccountInfo.AccountDetailsCache.GetAccountDetails(fullName);
        if (accountDetails.Sid != (SecurityIdentifier) null)
          AccountInfo.AccountDetailsCache.m_cache[fullName] = accountDetails;
        return accountDetails;
      }

      private static AccountInfo.AccountDetailsCacheEntry GetAccountDetails(string accountName)
      {
        AdminTraceLogger.Default.Info("Loading account details for {0}", new object[1]
        {
          (object) accountName
        });
        string str = UserNameUtil.NormalizeDomainUserFormat(accountName);
        if (!string.IsNullOrEmpty(str))
          return new AccountInfo.AccountDetailsCacheEntry(str, AccountInfo.AccountDetailsCache.TryGetSid(str));
        string name;
        string domain;
        UserNameUtil.Parse(accountName, out name, out domain);
        if (domain.Equals(".", StringComparison.OrdinalIgnoreCase) || domain.Equals("localhost", StringComparison.OrdinalIgnoreCase))
          accountName = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}\\{1}", (object) UserNameUtil.NetBiosName, (object) name);
        SecurityIdentifier sid = AccountInfo.AccountDetailsCache.TryGetSid(accountName);
        if (sid != (SecurityIdentifier) null)
          accountName = AccountInfo.AccountDetailsCache.TryGetAccountName(sid) ?? accountName;
        return new AccountInfo.AccountDetailsCacheEntry(accountName, sid);
      }

      private static SecurityIdentifier TryGetSid(string accountName)
      {
        try
        {
          return (SecurityIdentifier) new NTAccount(accountName).Translate(typeof (SecurityIdentifier));
        }
        catch (Exception ex)
        {
          AdminTraceLogger.Default.Info("No SID found for account name: " + accountName);
          return (SecurityIdentifier) null;
        }
      }

      private static string TryGetAccountName(SecurityIdentifier sid)
      {
        try
        {
          return sid.Translate(typeof (NTAccount)).Value;
        }
        catch (Exception ex)
        {
          AdminTraceLogger.Default.Info("No account found for SID: " + sid.ToString());
          return (string) null;
        }
      }
    }
  }
}
