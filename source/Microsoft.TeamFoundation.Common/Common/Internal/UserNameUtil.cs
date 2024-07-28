// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Common.Internal.UserNameUtil
// Assembly: Microsoft.TeamFoundation.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E643B42-FE11-4FC2-A9D6-79417E26CF92
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Common.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Security.Principal;

namespace Microsoft.TeamFoundation.Common.Internal
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class UserNameUtil
  {
    public static readonly string AuthenticatedUser = ".";
    private static string s_defaultDomain;

    public static bool IsComplete(string userName)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(userName, nameof (userName));
      return userName.IndexOf('\\') >= 0 || userName == UserNameUtil.AuthenticatedUser;
    }

    public static string Complete(string userName) => UserNameUtil.Complete(userName, (string) null);

    public static string Complete(string userName, string relative)
    {
      ArgumentUtility.CheckForNull<string>(userName, nameof (userName));
      if (userName.IndexOf('\\') >= 0 || userName.Length == 0 || userName == UserNameUtil.AuthenticatedUser)
        return userName;
      if (relative == null)
        relative = UserNameUtil.DefaultDomain;
      else if (relative.Contains("\\"))
        relative = UserNameUtil.GetDomainName(relative);
      return relative + "\\" + userName;
    }

    public static string[] Complete(string[] userNames, string relative)
    {
      if (userNames == null)
        return (string[]) null;
      if (relative == null)
        relative = UserNameUtil.DefaultDomain;
      else if (relative.Contains("\\"))
        relative = UserNameUtil.GetDomainName(relative);
      string[] strArray = new string[userNames.Length];
      for (int index = 0; index < userNames.Length; ++index)
        strArray[index] = UserNameUtil.Complete(userNames[index], relative);
      return strArray;
    }

    public static string MakePartial(string userName) => UserNameUtil.MakePartial(userName, (string) null);

    public static string MakePartial(string userName, string relative)
    {
      if (string.IsNullOrEmpty(userName))
        return string.Empty;
      if (userName.IndexOf('\\') < 0)
        return userName;
      string str = relative != null ? (!relative.Contains("\\") ? relative : UserNameUtil.GetDomainName(relative)) : UserNameUtil.DefaultDomain;
      return userName.StartsWith(str, StringComparison.OrdinalIgnoreCase) && userName.Length > str.Length + 1 && userName[str.Length] == '\\' ? userName.Substring(str.Length + 1) : userName;
    }

    public static void Parse(string userName, out string name, out string domain)
    {
      int length;
      if ((length = userName.IndexOf('\\')) < 0)
      {
        domain = string.Empty;
        name = userName;
      }
      else
      {
        if (length == userName.Length)
          throw new ArgumentException(TFCommonResources.InvalidUserName((object) userName));
        domain = userName.Substring(0, length);
        name = userName.Substring(length + 1);
      }
    }

    public static string GetDomainName(string userName)
    {
      int length;
      if ((length = userName.IndexOf('\\')) < 0)
        return string.Empty;
      if (length == userName.Length)
        throw new ArgumentException(TFCommonResources.InvalidUserName((object) userName));
      return userName.Substring(0, length);
    }

    public static void ParseUniqueUserName(
      string identityName,
      out string userName,
      out int uniqueUserId)
    {
      int length = identityName.LastIndexOf(':');
      if (length < 0)
      {
        userName = identityName;
        uniqueUserId = 0;
      }
      else
      {
        userName = identityName.Substring(0, length);
        uniqueUserId = int.Parse(identityName.Substring(length + 1), (IFormatProvider) CultureInfo.InvariantCulture);
      }
    }

    public static bool TryParseUniqueUserName(
      string identityName,
      out string userName,
      out int uniqueUserId)
    {
      int length = identityName.LastIndexOf(':');
      if (length < 0)
      {
        userName = identityName;
        uniqueUserId = 0;
        return true;
      }
      if (int.TryParse(identityName.Substring(length + 1), NumberStyles.Integer, (IFormatProvider) CultureInfo.InvariantCulture, out uniqueUserId))
      {
        userName = identityName.Substring(0, length);
        return true;
      }
      userName = identityName;
      uniqueUserId = 0;
      return false;
    }

    public static string BuildUniqueUserName(string userName, int uniqueUserId) => uniqueUserId == 0 ? userName : userName + ":" + (object) uniqueUserId;

    public static void GetIdentityName(
      string identityType,
      string displayName,
      string domainName,
      string accountName,
      int uniqueUserId,
      out string resolvableName,
      out string displayableName)
    {
      if (identityType.Equals("Microsoft.TeamFoundation.Identity", StringComparison.OrdinalIgnoreCase))
      {
        if (string.IsNullOrEmpty(domainName))
        {
          displayableName = "[" + "SERVER" + "]\\" + accountName;
          resolvableName = accountName;
        }
        else
        {
          displayableName = displayName;
          resolvableName = domainName + "\\" + accountName;
        }
      }
      else
      {
        displayableName = string.IsNullOrEmpty(domainName) ? accountName : domainName + "\\" + accountName;
        resolvableName = displayableName;
      }
      if (uniqueUserId == 0)
        return;
      displayableName = CultureInfo.InvariantCulture.ToString() + displayableName + ":" + (object) uniqueUserId;
      resolvableName = CultureInfo.InvariantCulture.ToString() + resolvableName + ":" + (object) uniqueUserId;
    }

    public static int Compare(string user1, string user2) => VssStringComparer.UserName.Compare(user1, user2);

    public static int CompareUI(string user1, string user2)
    {
      string name1;
      string domain1;
      UserNameUtil.Parse(user1, out name1, out domain1);
      string name2;
      string domain2;
      UserNameUtil.Parse(user2, out name2, out domain2);
      int num = VssStringComparer.UserNameUI.Compare(name1, name2);
      if (num != 0)
        return num;
      if (string.IsNullOrEmpty(domain1) && string.IsNullOrEmpty(domain2))
        return 0;
      if (string.IsNullOrEmpty(domain1))
        return -1;
      return string.IsNullOrEmpty(domain2) ? 1 : VssStringComparer.DomainNameUI.Compare(domain1, domain2);
    }

    public static SecurityIdentifier GetSecurityIdentifier(string userName) => (SecurityIdentifier) new NTAccount(userName).Translate(typeof (SecurityIdentifier));

    public static bool IsWellKnownAccount(string userName, WellKnownSidType wellKnownType)
    {
      bool flag = false;
      try
      {
        SecurityIdentifier securityIdentifier1 = UserNameUtil.GetSecurityIdentifier(userName);
        SecurityIdentifier securityIdentifier2 = new SecurityIdentifier(wellKnownType, (SecurityIdentifier) null);
        NTAccount ntAccount = (NTAccount) securityIdentifier2.Translate(typeof (NTAccount));
        SecurityIdentifier sid = securityIdentifier2;
        flag = securityIdentifier1.Equals(sid);
      }
      catch
      {
      }
      return flag;
    }

    public static string NormalizeDomainUserFormat(string account)
    {
      string str = (string) null;
      if (!string.IsNullOrEmpty(account))
      {
        IntPtr phDS;
        uint num = NativeMethods.DsBind((string) null, (string) null, out phDS);
        try
        {
          if (num == 0U)
          {
            string[] names = new string[1]{ account };
            NativeMethods.DS_NAME_RESULT_ITEM[] dsNameResultItemArray = NativeMethods.DsCrackNames(phDS, NativeMethods.DS_NAME_FLAGS.DS_NAME_NO_FLAGS, NativeMethods.DS_NAME_FORMAT.DS_UNKNOWN_NAME, NativeMethods.DS_NAME_FORMAT.DS_NT4_ACCOUNT_NAME, names);
            if (dsNameResultItemArray.Length != 0)
            {
              if (dsNameResultItemArray[0].status == NativeMethods.DS_NAME_ERROR.DS_NAME_NO_ERROR)
                str = dsNameResultItemArray[0].pName;
            }
          }
        }
        finally
        {
          NativeMethods.DsUnBind(ref phDS);
        }
      }
      return str;
    }

    public static string NetBiosName => Environment.MachineName;

    public static string CurrentMachineAccountName => UserNameUtil.GetMachineAccountName(UserNameUtil.NetBiosName);

    public static string GetMachineAccountName(string hostName)
    {
      NTAccount ntAccount = (NTAccount) new NTAccount(hostName + "$").Translate(typeof (SecurityIdentifier)).Translate(typeof (NTAccount));
      return UserNameUtil.NormalizeDomainUserFormat(ntAccount.Value) ?? ntAccount.Value;
    }

    public static string TranslateAccountNameIfNetworkService(string userName)
    {
      try
      {
        if (UserNameUtil.IsWellKnownAccount(userName, WellKnownSidType.NetworkServiceSid))
          return UserNameUtil.CurrentMachineAccountName;
      }
      catch (Exception ex)
      {
        TeamFoundationTrace.TraceAndDebugFailException(ex);
      }
      return userName;
    }

    public static string CurrentUserName
    {
      get
      {
        using (WindowsIdentity current = WindowsIdentity.GetCurrent())
          return current.Name;
      }
    }

    private static string DefaultDomain
    {
      get
      {
        if (UserNameUtil.s_defaultDomain == null)
        {
          string domain;
          UserNameUtil.Parse(UserNameUtil.CurrentUserName, out string _, out domain);
          UserNameUtil.s_defaultDomain = domain;
        }
        return UserNameUtil.s_defaultDomain;
      }
    }
  }
}
