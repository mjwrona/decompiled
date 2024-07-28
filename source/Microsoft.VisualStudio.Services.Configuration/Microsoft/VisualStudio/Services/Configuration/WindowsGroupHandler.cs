// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Configuration.WindowsGroupHandler
// Assembly: Microsoft.VisualStudio.Services.Configuration, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AB461A1-8255-4EAB-B12B-E1D379571DC1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Configuration.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Security.Principal;

namespace Microsoft.VisualStudio.Services.Configuration
{
  public static class WindowsGroupHandler
  {
    public const string Iis7Group = "IIS_IUSRS";
    private static SecurityIdentifier s_iisIusrsGroupSid;

    public static SecurityIdentifier IisIusrsGroupSid
    {
      get
      {
        SecurityIdentifier iisIusrsGroupSid = WindowsGroupHandler.s_iisIusrsGroupSid;
        return (object) iisIusrsGroupSid != null ? iisIusrsGroupSid : (WindowsGroupHandler.s_iisIusrsGroupSid = new SecurityIdentifier("S-1-5-32-568"));
      }
    }

    public static void CreateGroup(string groupName)
    {
      if (EnvironmentHandler.IsDomainController())
        WindowsGroupHandler.CreateGlobalGroup(groupName, true);
      else
        WindowsGroupHandler.CreateLocalGroup(groupName, true);
    }

    public static void DeleteGroup(string groupName)
    {
      if (EnvironmentHandler.IsDomainController())
        WindowsGroupHandler.DeleteGlobalGroup(groupName, true);
      else
        WindowsGroupHandler.DeleteLocalGroup(groupName, true);
    }

    public static void AddMemberToGroup(IAccount member, string groupName)
    {
      if (EnvironmentHandler.IsDomainController())
        WindowsGroupHandler.AddMemberToGlobalGroup(WindowsGroupHandler.GetNameForGlobalGroup(member), groupName, true);
      else
        WindowsGroupHandler.AddMemberToLocalGroup(member.FullName, groupName, true);
    }

    public static void RemoveMemberFromGroup(IAccount member, string groupName)
    {
      if (EnvironmentHandler.IsDomainController())
        WindowsGroupHandler.RemoveMemberFromGlobalGroup(WindowsGroupHandler.GetNameForGlobalGroup(member), groupName, true);
      else
        WindowsGroupHandler.RemoveMemberFromLocalGroup(member.FullName, groupName, true);
    }

    public static void RemoveAllMembersFromGroup(string groupName)
    {
      if (EnvironmentHandler.IsDomainController())
        WindowsGroupHandler.RemoveAllMembersFromGlobalGroup(groupName, true);
      else
        WindowsGroupHandler.RemoveAllMembersFromLocalGroup(groupName, true);
    }

    public static string GetIisGroup() => WindowsGroupHandler.GetIisGroup((ITFLogger) null);

    public static string GetIisGroup(ITFLogger logger)
    {
      AdminTraceLogger.Default.Verbose("Getting IIS Group ...");
      string iisGroup;
      try
      {
        iisGroup = WindowsGroupHandler.IisIusrsGroupSid.Translate(typeof (NTAccount)).Value;
        int num = iisGroup.IndexOf("\\", StringComparison.OrdinalIgnoreCase);
        if (num >= 0)
          iisGroup = iisGroup.Substring(num + 1);
      }
      catch (IdentityNotMappedException ex)
      {
        logger?.Error((Exception) ex);
        throw new ConfigurationException(ConfigurationResources.InvalidIISUserGroup((object) "IIS_IUSRS"));
      }
      logger?.Info("Using IIS Group '{0}'", (object) iisGroup);
      return iisGroup;
    }

    public static List<SecurityIdentifier> GetSidsOfLocalGroupMembers(
      string serverName,
      string localGroupName,
      ITFLogger logger)
    {
      AdminTraceLogger.Default.Enter(nameof (GetSidsOfLocalGroupMembers));
      ArgumentUtility.CheckStringForNullOrEmpty(localGroupName, nameof (localGroupName));
      ArgumentUtility.CheckForNull<ITFLogger>(logger, nameof (logger));
      IntPtr resumeHandle = IntPtr.Zero;
      IntPtr bufptr = IntPtr.Zero;
      try
      {
        uint entriesRead;
        int members = WindowsGroupHandler.NativeMethods.NetLocalGroupGetMembers(serverName, localGroupName, 0U, out bufptr, uint.MaxValue, out entriesRead, out uint _, out resumeHandle);
        switch (members)
        {
          case 0:
            List<SecurityIdentifier> localGroupMembers = new List<SecurityIdentifier>();
            AdminTraceLogger.Default.Verbose("{0} sids were returned.", (object) entriesRead);
            for (int index = 0; (long) index < (long) entriesRead; ++index)
            {
              SecurityIdentifier securityIdentifier = new SecurityIdentifier(((WindowsGroupHandler.NativeMethods.LocalGroupMemberInfo0) Marshal.PtrToStructure(bufptr + Marshal.SizeOf(typeof (WindowsGroupHandler.NativeMethods.LocalGroupMemberInfo0)) * index, typeof (WindowsGroupHandler.NativeMethods.LocalGroupMemberInfo0))).Sid);
              logger.Info("sid = {0}", (object) securityIdentifier.ToString());
              localGroupMembers.Add(securityIdentifier);
            }
            return localGroupMembers;
          case 5:
            throw new UnauthorizedAccessException(ConfigurationResources.AccessDenied());
          case 1376:
          case 2220:
            throw new ArgumentException(ConfigurationResources.GroupNotExist(), nameof (localGroupName));
          default:
            throw new ConfigurationException(ConfigurationResources.ErrorOperationWithReturnCode((object) "NetLocalGroupGetMembers", (object) members.ToString((IFormatProvider) CultureInfo.CurrentCulture)));
        }
      }
      finally
      {
        if (bufptr != IntPtr.Zero)
        {
          int error = WindowsGroupHandler.NativeMethods.NetApiBufferFree(bufptr);
          if (error != 0)
            logger.Error("Failed to free buffer returned by NetLocalGroupGetMembers(). Error: {0}, Message: {1}", (object) error, (object) new Win32Exception(error).Message);
        }
        AdminTraceLogger.Default.Exit(nameof (GetSidsOfLocalGroupMembers));
      }
    }

    public static void CreateLocalGroup(string name, bool passive)
    {
      int num = WindowsGroupHandler.NativeMethods.NetLocalGroupAdd((string) null, 1, ref new WindowsGroupHandler.NativeMethods.LocalGroupInfo()
      {
        Name = name,
        Comment = ConfigurationResources.TfsBuiltInGroupsComment()
      }, 0);
      switch (num)
      {
        case 0:
          break;
        case 5:
          throw new UnauthorizedAccessException(ConfigurationResources.AccessDenied());
        case 87:
          throw new ArgumentException(ConfigurationResources.InvalidGroupName(), nameof (name));
        case 1379:
        case 2223:
          if (passive)
            break;
          throw new AlreadyExistsException(ConfigurationResources.GroupAlreadyExists((object) name));
        default:
          throw new ConfigurationException(ConfigurationResources.ErrorOperationWithReturnCode((object) "NetLocalGroupAdd", (object) num.ToString((IFormatProvider) CultureInfo.CurrentCulture)));
      }
    }

    public static void CreateGlobalGroup(string name, bool passive)
    {
      int num = WindowsGroupHandler.NativeMethods.NetGroupAdd((string) null, 1, ref new WindowsGroupHandler.NativeMethods.GroupInfo()
      {
        Name = name,
        Comment = ConfigurationResources.TfsBuiltInGroupsComment()
      }, 0);
      switch (num)
      {
        case 0:
          break;
        case 5:
          throw new UnauthorizedAccessException(ConfigurationResources.AccessDenied());
        case 87:
          throw new ArgumentException(ConfigurationResources.InvalidGroupName(), nameof (name));
        case 1379:
        case 2223:
          if (passive)
            break;
          throw new AlreadyExistsException(ConfigurationResources.GroupAlreadyExists((object) name));
        default:
          throw new ConfigurationException(ConfigurationResources.ErrorOperationWithReturnCode((object) "NetGroupAdd", (object) num.ToString((IFormatProvider) CultureInfo.CurrentCulture)));
      }
    }

    public static void DeleteLocalGroup(string name, bool passive)
    {
      int num = WindowsGroupHandler.NativeMethods.NetLocalGroupDel((string) null, name);
      switch (num)
      {
        case 0:
          break;
        case 5:
          throw new UnauthorizedAccessException(ConfigurationResources.AccessDenied());
        case 1376:
        case 2220:
          if (passive)
            break;
          throw new ArgumentException(ConfigurationResources.GroupNotExist());
        default:
          throw new ConfigurationException(ConfigurationResources.ErrorOperationWithReturnCode((object) "NetLocalGroupDel", (object) num.ToString((IFormatProvider) CultureInfo.CurrentCulture)));
      }
    }

    public static void DeleteGlobalGroup(string name, bool passive)
    {
      int num = WindowsGroupHandler.NativeMethods.NetGroupDel((string) null, name);
      switch (num)
      {
        case 0:
          break;
        case 5:
          throw new UnauthorizedAccessException(ConfigurationResources.AccessDenied());
        case 1376:
        case 2220:
          if (passive)
            break;
          throw new ArgumentException(ConfigurationResources.GroupNotExist());
        default:
          throw new ConfigurationException(ConfigurationResources.ErrorOperationWithReturnCode((object) "NetGroupDel", (object) num.ToString((IFormatProvider) CultureInfo.CurrentCulture)));
      }
    }

    public static void AddMemberToLocalGroup(string fullName, string groupName, bool passive)
    {
      int num = WindowsGroupHandler.NativeMethods.NetLocalGroupAddMembers((string) null, groupName, 3, ref new WindowsGroupHandler.NativeMethods.LocalGroupMemberInfo()
      {
        FullName = fullName
      }, 1);
      switch (num)
      {
        case 0:
          break;
        case 5:
          throw new UnauthorizedAccessException(ConfigurationResources.AccessDenied());
        case 1376:
        case 2220:
          throw new ArgumentException(ConfigurationResources.GroupNotExist());
        case 1378:
          if (passive)
            break;
          throw new AlreadyExistsException(ConfigurationResources.MemberAlreadyExistsInGroup());
        case 1387:
          throw new ArgumentException(ConfigurationResources.MemberNotExist());
        default:
          throw new ConfigurationException(ConfigurationResources.ErrorOperationWithReturnCode((object) "NetLocalGroupAddMember", (object) num.ToString((IFormatProvider) CultureInfo.CurrentCulture)));
      }
    }

    public static void AddMemberToGlobalGroup(string userName, string groupName, bool passive)
    {
      int num = WindowsGroupHandler.NativeMethods.NetGroupAddUser((string) null, groupName, userName);
      switch (num)
      {
        case 0:
          break;
        case 5:
          throw new UnauthorizedAccessException(ConfigurationResources.AccessDenied());
        case 1376:
        case 2220:
          throw new ArgumentException(ConfigurationResources.GroupNotExist());
        case 1378:
        case 2236:
          if (passive)
            break;
          throw new AlreadyExistsException(ConfigurationResources.MemberAlreadyExistsInGroup());
        case 1387:
          throw new ArgumentException(ConfigurationResources.MemberNotExist());
        default:
          throw new ConfigurationException(ConfigurationResources.ErrorOperationWithReturnCode((object) "NetGroupAddUser", (object) num.ToString((IFormatProvider) CultureInfo.CurrentCulture)));
      }
    }

    public static void RemoveMemberFromLocalGroup(string fullName, string groupName, bool passive)
    {
      int num = WindowsGroupHandler.NativeMethods.NetLocalGroupDelMembers((string) null, groupName, 3, ref new WindowsGroupHandler.NativeMethods.LocalGroupMemberInfo()
      {
        FullName = fullName
      }, 1);
      switch (num)
      {
        case 0:
          break;
        case 5:
          throw new UnauthorizedAccessException(ConfigurationResources.AccessDenied());
        case 1376:
        case 2220:
          throw new ArgumentException(ConfigurationResources.GroupNotExist());
        case 1377:
          if (passive)
            break;
          throw new NotExistsException(ConfigurationResources.MemberNotInGroup((object) fullName, (object) groupName));
        case 1387:
          throw new ArgumentException(ConfigurationResources.MemberNotExist());
        default:
          throw new ConfigurationException(ConfigurationResources.ErrorOperationWithReturnCode((object) "NetLocalGroupDelMember", (object) num.ToString((IFormatProvider) CultureInfo.CurrentCulture)));
      }
    }

    public static void RemoveAllMembersFromLocalGroup(string groupName, bool passive)
    {
      WindowsGroupHandler.NativeMethods.LocalGroupMemberInfo[] buf = Array.Empty<WindowsGroupHandler.NativeMethods.LocalGroupMemberInfo>();
      int num = WindowsGroupHandler.NativeMethods.NetLocalGroupSetMembers((string) null, groupName, 3U, buf, 0U);
      switch (num)
      {
        case 0:
          break;
        case 5:
          throw new UnauthorizedAccessException(ConfigurationResources.AccessDenied());
        case 1376:
        case 2220:
          if (passive)
            break;
          throw new ArgumentException(ConfigurationResources.GroupNotExist());
        case 1387:
          throw new ArgumentException(ConfigurationResources.MemberNotExist());
        default:
          throw new ConfigurationException(ConfigurationResources.ErrorOperationWithReturnCode((object) "NetLocalGroupSetMembers", (object) num.ToString((IFormatProvider) CultureInfo.CurrentCulture)));
      }
    }

    public static void RemoveMemberFromGlobalGroup(string userName, string groupName, bool passive)
    {
      int num = WindowsGroupHandler.NativeMethods.NetGroupDelUser((string) null, groupName, userName);
      switch (num)
      {
        case 0:
          break;
        case 5:
          throw new UnauthorizedAccessException(ConfigurationResources.AccessDenied());
        case 1376:
        case 2220:
          throw new ArgumentException(ConfigurationResources.GroupNotExist());
        case 1377:
          if (passive)
            break;
          throw new NotExistsException(ConfigurationResources.MemberNotInGroup((object) userName, (object) groupName));
        case 1387:
          throw new ArgumentException(ConfigurationResources.MemberNotExist());
        default:
          throw new ConfigurationException(ConfigurationResources.ErrorOperationWithReturnCode((object) "NetGroupDelUser", (object) num.ToString((IFormatProvider) CultureInfo.CurrentCulture)));
      }
    }

    public static void RemoveAllMembersFromGlobalGroup(string groupName, bool passive)
    {
      WindowsGroupHandler.NativeMethods.GroupUsersInfo[] buf = Array.Empty<WindowsGroupHandler.NativeMethods.GroupUsersInfo>();
      int num = WindowsGroupHandler.NativeMethods.NetGroupSetUsers((string) null, groupName, 0U, buf, 0U);
      switch (num)
      {
        case 0:
          break;
        case 5:
          throw new UnauthorizedAccessException(ConfigurationResources.AccessDenied());
        case 1376:
        case 2220:
          if (passive)
            break;
          throw new ArgumentException(ConfigurationResources.GroupNotExist());
        case 1387:
          throw new ArgumentException(ConfigurationResources.MemberNotExist());
        default:
          throw new ConfigurationException(ConfigurationResources.ErrorOperationWithReturnCode((object) "NetGroupSetUsers", (object) num.ToString((IFormatProvider) CultureInfo.CurrentCulture)));
      }
    }

    public static bool LocalGroupExists(string groupName)
    {
      IntPtr bufptr;
      int info = WindowsGroupHandler.NativeMethods.NetLocalGroupGetInfo((string) null, groupName, 1, out bufptr);
      WindowsGroupHandler.NativeMethods.NetApiBufferFree(bufptr);
      if (info <= 5)
      {
        if (info == 0)
          return true;
        if (info == 5)
          throw new UnauthorizedAccessException(ConfigurationResources.AccessDenied());
      }
      else if (info == 1376 || info == 2220)
        return false;
      throw new ConfigurationException(ConfigurationResources.ErrorOperationWithReturnCode((object) "NetLocalGroupGetInfo", (object) info.ToString((IFormatProvider) CultureInfo.CurrentCulture)));
    }

    private static string GetNameForGlobalGroup(IAccount member) => member.AccountType == Microsoft.TeamFoundation.Admin.AccountType.NetworkService ? UserNameUtil.NetBiosName + "$" : member.UserName;

    private static class NativeMethods
    {
      [DllImport("Netapi32.dll")]
      internal static extern int NetGroupAdd(
        [MarshalAs(UnmanagedType.LPWStr)] string servername,
        int level,
        ref WindowsGroupHandler.NativeMethods.GroupInfo buf,
        int parm_err);

      [DllImport("Netapi32.dll")]
      internal static extern int NetGroupAddUser(
        [MarshalAs(UnmanagedType.LPWStr)] string servername,
        [MarshalAs(UnmanagedType.LPWStr)] string groupname,
        [MarshalAs(UnmanagedType.LPWStr)] string username);

      [DllImport("Netapi32.dll")]
      internal static extern int NetGroupDelUser(
        [MarshalAs(UnmanagedType.LPWStr)] string servername,
        [MarshalAs(UnmanagedType.LPWStr)] string groupname,
        [MarshalAs(UnmanagedType.LPWStr)] string username);

      [DllImport("Netapi32.dll")]
      internal static extern int NetGroupDel([MarshalAs(UnmanagedType.LPWStr)] string servername, [MarshalAs(UnmanagedType.LPWStr)] string groupname);

      [DllImport("Netapi32.dll", CharSet = CharSet.Unicode)]
      internal static extern int NetGroupSetUsers(
        [MarshalAs(UnmanagedType.LPWStr)] string servername,
        [MarshalAs(UnmanagedType.LPWStr)] string groupname,
        uint level,
        WindowsGroupHandler.NativeMethods.GroupUsersInfo[] buf,
        uint totalEntries);

      [DllImport("Netapi32.dll")]
      internal static extern int NetLocalGroupAdd(
        [MarshalAs(UnmanagedType.LPWStr)] string servername,
        int level,
        ref WindowsGroupHandler.NativeMethods.LocalGroupInfo buf,
        int parm_err);

      [DllImport("Netapi32.dll")]
      internal static extern int NetLocalGroupAddMembers(
        [MarshalAs(UnmanagedType.LPWStr)] string serverName,
        [MarshalAs(UnmanagedType.LPWStr)] string groupName,
        int level,
        ref WindowsGroupHandler.NativeMethods.LocalGroupMemberInfo buf,
        int totalEntries);

      [DllImport("Netapi32.dll")]
      internal static extern int NetLocalGroupDelMembers(
        [MarshalAs(UnmanagedType.LPWStr)] string serverName,
        [MarshalAs(UnmanagedType.LPWStr)] string groupName,
        int level,
        ref WindowsGroupHandler.NativeMethods.LocalGroupMemberInfo buf,
        int totalEntries);

      [DllImport("Netapi32.dll")]
      internal static extern int NetLocalGroupDel([MarshalAs(UnmanagedType.LPWStr)] string servername, [MarshalAs(UnmanagedType.LPWStr)] string groupname);

      [DllImport("Netapi32.dll")]
      internal static extern int NetLocalGroupGetInfo(
        [MarshalAs(UnmanagedType.LPWStr)] string servername,
        [MarshalAs(UnmanagedType.LPWStr)] string groupname,
        int level,
        out IntPtr bufptr);

      [DllImport("NetAPI32.dll", CharSet = CharSet.Unicode)]
      internal static extern int NetLocalGroupGetMembers(
        [MarshalAs(UnmanagedType.LPWStr)] string serverName,
        [MarshalAs(UnmanagedType.LPWStr)] string localGroupName,
        uint level,
        out IntPtr bufptr,
        uint preferredMaximumLength,
        out uint entriesRead,
        out uint totalEntries,
        out IntPtr resumeHandle);

      [DllImport("NetAPI32.dll", CharSet = CharSet.Unicode)]
      internal static extern int NetLocalGroupSetMembers(
        [MarshalAs(UnmanagedType.LPWStr)] string servername,
        [MarshalAs(UnmanagedType.LPWStr)] string groupname,
        uint level,
        WindowsGroupHandler.NativeMethods.LocalGroupMemberInfo[] buf,
        uint totalEntries);

      [DllImport("Netapi32.dll")]
      internal static extern int NetApiBufferFree(IntPtr Buffer);

      [StructLayout(LayoutKind.Sequential, Size = 1)]
      internal struct ReturnCode
      {
        internal const int S_OK = 0;
        internal const int ERROR_ACCESS_DENIED = 5;
        internal const int ERROR_INVALID_PARAMETER = 87;
        internal const int ERROR_MEMBER_NOT_IN_ALIAS = 1377;
        internal const int ERROR_MEMBER_IN_ALIAS = 1378;
        internal const int ERROR_ALIAS_EXISTS = 1379;
        internal const int ERROR_NO_SUCH_ALIAS = 1376;
        internal const int ERROR_NO_SUCH_MEMBER = 1387;
        internal const int NERR_GroupNotFound = 2220;
        internal const int NERR_GroupExists = 2223;
        internal const int NERR_UserInGroup = 2236;
      }

      [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
      internal struct GroupInfo
      {
        [MarshalAs(UnmanagedType.LPWStr)]
        internal string Name;
        [MarshalAs(UnmanagedType.LPWStr)]
        internal string Comment;
      }

      [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
      internal struct LocalGroupInfo
      {
        [MarshalAs(UnmanagedType.LPWStr)]
        internal string Name;
        [MarshalAs(UnmanagedType.LPWStr)]
        internal string Comment;
      }

      [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
      internal struct GroupUsersInfo
      {
        [MarshalAs(UnmanagedType.LPWStr)]
        internal string FullName;
      }

      [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
      internal struct LocalGroupMemberInfo0
      {
        public IntPtr Sid;
      }

      [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
      internal struct LocalGroupMemberInfo
      {
        [MarshalAs(UnmanagedType.LPWStr)]
        internal string FullName;
      }
    }
  }
}
