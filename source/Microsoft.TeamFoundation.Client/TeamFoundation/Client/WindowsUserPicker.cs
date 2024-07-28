// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.WindowsUserPicker
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Framework.Client;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Windows.Forms;

namespace Microsoft.TeamFoundation.Client
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class WindowsUserPicker
  {
    private static readonly string s_localAccountsToken = "WinNT:";

    public static IEnumerable<TeamFoundationIdentity> SelectWindowsUsers(
      IIdentityManagementService identityService,
      IntPtr hWnd)
    {
      return WindowsUserPicker.SelectWindowsUsers(identityService, hWnd, true, true, true);
    }

    public static IEnumerable<TeamFoundationIdentity> SelectWindowsUsers(
      IIdentityManagementService identityService,
      IntPtr hWnd,
      bool isMultiSelected,
      bool allowGroups,
      bool includeComputers)
    {
      return WindowsUserPicker.SelectWindowsUsers(identityService, hWnd, isMultiSelected, allowGroups, includeComputers, out List<string> _);
    }

    public static IEnumerable<TeamFoundationIdentity> SelectWindowsUsers(
      IIdentityManagementService identityService,
      IntPtr hWnd,
      bool isMultiSelected,
      bool allowGroups,
      bool includeComputers,
      out List<string> unresolvedIdentities)
    {
      List<TeamFoundationIdentity> foundationIdentityList = new List<TeamFoundationIdentity>();
      unresolvedIdentities = new List<string>();
      string[] attributeNames = new string[1]{ "objectSid" };
      int count = 0;
      WindowsUserPicker.DS_SELECTION[] dataArray = (WindowsUserPicker.DS_SELECTION[]) null;
      int dsAttributesCount = 0;
      WindowsUserPicker.DS_SELECTION_ATTRIBUTES[] dsAttributes = (WindowsUserPicker.DS_SELECTION_ATTRIBUTES[]) null;
      WindowsUserPicker.ObjectPickerWrapper.GetSelectedAccounts(hWnd, (string) null, isMultiSelected, allowGroups, includeComputers, attributeNames, ref count, ref dataArray, ref dsAttributesCount, ref dsAttributes);
      if (count != 0)
      {
        Dictionary<IdentityDescriptor, string> dictionary = new Dictionary<IdentityDescriptor, string>((IEqualityComparer<IdentityDescriptor>) IdentityDescriptorComparer.Instance);
        for (int index = 0; index < dataArray.Length; ++index)
        {
          string selectionName;
          if (WindowsUserPicker.GetAccountName(dataArray[index], out selectionName))
          {
            TeamFoundationIdentity[] readIdentity = identityService.ReadIdentities(IdentitySearchFactor.AccountName, new string[1]
            {
              selectionName
            }, MembershipQuery.None, ReadIdentityOptions.IncludeReadFromSource)[0];
            TeamFoundationIdentity foundationIdentity1 = (TeamFoundationIdentity) null;
            TeamFoundationIdentity foundationIdentity2 = (TeamFoundationIdentity) null;
            string str = Environment.MachineName.Trim();
            foreach (TeamFoundationIdentity foundationIdentity3 in readIdentity)
            {
              if (foundationIdentity3.IsActive && foundationIdentity3.Descriptor.IdentityType.Equals("System.Security.Principal.WindowsIdentity", StringComparison.OrdinalIgnoreCase))
              {
                string attribute = foundationIdentity3.GetAttribute("DN", (string) null);
                if (string.IsNullOrEmpty(attribute) || attribute.StartsWith(WindowsUserPicker.s_localAccountsToken, StringComparison.Ordinal))
                {
                  if (foundationIdentity3.GetAttribute("Domain", string.Empty).Equals(str, StringComparison.OrdinalIgnoreCase))
                  {
                    foundationIdentity2 = foundationIdentity3;
                    break;
                  }
                  foundationIdentity1 = foundationIdentity3;
                }
              }
            }
            if (foundationIdentity2 != null)
              foundationIdentityList.Add(foundationIdentity2);
            else if (foundationIdentity1 != null)
              foundationIdentityList.Add(foundationIdentity1);
            else
              unresolvedIdentities.Add(selectionName);
          }
          else if (dsAttributesCount > 0 && dsAttributes[index].attributeValues[0] is byte[] attributeValue)
          {
            IdentityDescriptor windowsDescriptor = IdentityHelper.CreateWindowsDescriptor(new SecurityIdentifier(attributeValue, 0));
            dictionary[windowsDescriptor] = selectionName;
          }
        }
        if (dictionary.Count > 0)
        {
          foreach (TeamFoundationIdentity readIdentity in identityService.ReadIdentities(dictionary.Keys.ToArray<IdentityDescriptor>(), MembershipQuery.None, ReadIdentityOptions.IncludeReadFromSource))
          {
            if (readIdentity != null)
            {
              foundationIdentityList.Add(readIdentity);
              dictionary.Remove(readIdentity.Descriptor);
            }
          }
          foreach (string str in dictionary.Values)
            unresolvedIdentities.Add(str);
        }
      }
      return (IEnumerable<TeamFoundationIdentity>) foundationIdentityList;
    }

    public static List<SecurityIdentifier> SelectAccounts(
      IntPtr hWnd,
      bool isMultiSelected,
      bool allowGroups,
      bool includeComputers)
    {
      List<SecurityIdentifier> securityIdentifierList = new List<SecurityIdentifier>();
      string[] attributeNames = new string[1]{ "objectSid" };
      int count = 0;
      WindowsUserPicker.DS_SELECTION[] dataArray = (WindowsUserPicker.DS_SELECTION[]) null;
      int dsAttributesCount = 0;
      WindowsUserPicker.DS_SELECTION_ATTRIBUTES[] dsAttributes = (WindowsUserPicker.DS_SELECTION_ATTRIBUTES[]) null;
      WindowsUserPicker.ObjectPickerWrapper.GetSelectedAccounts(hWnd, (string) null, isMultiSelected, allowGroups, includeComputers, attributeNames, ref count, ref dataArray, ref dsAttributesCount, ref dsAttributes);
      if (count != 0 && dsAttributesCount > 0)
      {
        for (int index = 0; index < count; ++index)
        {
          byte[] attributeValue = dsAttributes[index].attributeValues[0] as byte[];
          securityIdentifierList.Add(new SecurityIdentifier(attributeValue, 0));
        }
      }
      return securityIdentifierList;
    }

    private static bool GetAccountName(
      WindowsUserPicker.DS_SELECTION selection,
      out string selectionName)
    {
      string str = selection.pwzADsPath;
      bool accountName;
      if (string.IsNullOrEmpty(str) || str.StartsWith(WindowsUserPicker.s_localAccountsToken, StringComparison.Ordinal))
      {
        selectionName = selection.pwzName;
        accountName = true;
      }
      else
      {
        int startIndex = str.IndexOf("CN=", StringComparison.Ordinal);
        if (startIndex > 0)
          str = str.Substring(startIndex);
        selectionName = str;
        accountName = false;
      }
      return accountName;
    }

    private enum DsScope : uint
    {
      DSOP_SCOPE_TYPE_TARGET_COMPUTER = 1,
      DSOP_SCOPE_TYPE_UPLEVEL_JOINED_DOMAIN = 2,
      DSOP_SCOPE_TYPE_DOWNLEVEL_JOINED_DOMAIN = 4,
      DSOP_SCOPE_TYPE_ENTERPRISE_DOMAIN = 8,
      DSOP_SCOPE_TYPE_GLOBAL_CATALOG = 16, // 0x00000010
      DSOP_SCOPE_TYPE_EXTERNAL_UPLEVEL_DOMAIN = 32, // 0x00000020
      DSOP_SCOPE_TYPE_EXTERNAL_DOWNLEVEL_DOMAIN = 64, // 0x00000040
      DSOP_SCOPE_TYPE_WORKGROUP = 128, // 0x00000080
      DSOP_SCOPE_TYPE_USER_ENTERED_UPLEVEL_SCOPE = 256, // 0x00000100
      DSOP_SCOPE_TYPE_USER_ENTERED_DOWNLEVEL_SCOPE = 512, // 0x00000200
    }

    private enum DsScopeInitInfo : uint
    {
      DSOP_SCOPE_FLAG_STARTING_SCOPE = 1,
      DSOP_SCOPE_FLAG_WANT_PROVIDER_WINNT = 2,
      DSOP_SCOPE_FLAG_WANT_PROVIDER_LDAP = 4,
      DSOP_SCOPE_FLAG_WANT_PROVIDER_GC = 8,
      DSOP_SCOPE_FLAG_WANT_SID_PATH = 16, // 0x00000010
      DSOP_SCOPE_FLAG_WANT_DOWNLEVEL_BUILTIN_PATH = 32, // 0x00000020
      DSOP_SCOPE_FLAG_DEFAULT_FILTER_USERS = 64, // 0x00000040
      DSOP_SCOPE_FLAG_DEFAULT_FILTER_GROUPS = 128, // 0x00000080
      DSOP_SCOPE_FLAG_DEFAULT_FILTER_COMPUTERS = 256, // 0x00000100
      DSOP_SCOPE_FLAG_DEFAULT_FILTER_CONTACTS = 512, // 0x00000200
    }

    private enum DsFilter : uint
    {
      DSOP_FILTER_INCLUDE_ADVANCED_VIEW = 1,
      DSOP_FILTER_USERS = 2,
      DSOP_FILTER_BUILTIN_GROUPS = 4,
      DSOP_FILTER_WELL_KNOWN_PRINCIPALS = 8,
      DSOP_FILTER_UNIVERSAL_GROUPS_DL = 16, // 0x00000010
      DSOP_FILTER_UNIVERSAL_GROUPS_SE = 32, // 0x00000020
      DSOP_FILTER_GLOBAL_GROUPS_DL = 64, // 0x00000040
      DSOP_FILTER_GLOBAL_GROUPS_SE = 128, // 0x00000080
      DSOP_FILTER_DOMAIN_LOCAL_GROUPS_DL = 256, // 0x00000100
      DSOP_FILTER_DOMAIN_LOCAL_GROUPS_SE = 512, // 0x00000200
      DSOP_FILTER_CONTACTS = 1024, // 0x00000400
      DSOP_FILTER_COMPUTERS = 2048, // 0x00000800
    }

    private enum DsDownlevelFilter : uint
    {
      DSOP_DOWNLEVEL_FILTER_USERS = 2147483649, // 0x80000001
      DSOP_DOWNLEVEL_FILTER_LOCAL_GROUPS = 2147483650, // 0x80000002
      DSOP_DOWNLEVEL_FILTER_GLOBAL_GROUPS = 2147483652, // 0x80000004
      DSOP_DOWNLEVEL_FILTER_COMPUTERS = 2147483656, // 0x80000008
      DSOP_DOWNLEVEL_FILTER_WORLD = 2147483664, // 0x80000010
      DSOP_DOWNLEVEL_FILTER_AUTHENTICATED_USER = 2147483680, // 0x80000020
      DSOP_DOWNLEVEL_FILTER_ANONYMOUS = 2147483712, // 0x80000040
      DSOP_DOWNLEVEL_FILTER_BATCH = 2147483776, // 0x80000080
      DSOP_DOWNLEVEL_FILTER_CREATOR_OWNER = 2147483904, // 0x80000100
      DSOP_DOWNLEVEL_FILTER_CREATOR_GROUP = 2147484160, // 0x80000200
      DSOP_DOWNLEVEL_FILTER_DIALUP = 2147484672, // 0x80000400
      DSOP_DOWNLEVEL_FILTER_INTERACTIVE = 2147485696, // 0x80000800
      DSOP_DOWNLEVEL_FILTER_NETWORK = 2147487744, // 0x80001000
      DSOP_DOWNLEVEL_FILTER_SERVICE = 2147491840, // 0x80002000
      DSOP_DOWNLEVEL_FILTER_SYSTEM = 2147500032, // 0x80004000
      DSOP_DOWNLEVEL_FILTER_EXCLUDE_BUILTIN_GROUPS = 2147516416, // 0x80008000
      DSOP_DOWNLEVEL_FILTER_TERMINAL_SERVER = 2147549184, // 0x80010000
      DSOP_DOWNLEVEL_FILTER_ALL_WELLKNOWN_SIDS = 2147614720, // 0x80020000
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct DSOP_UPLEVEL_FILTER_FLAGS
    {
      public uint flBothModes;
      public uint flMixedModeOnly;
      public uint flNativeModeOnly;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct DSOP_FILTER_FLAGS
    {
      public WindowsUserPicker.DSOP_UPLEVEL_FILTER_FLAGS Uplevel;
      public uint flDownlevel;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct DSOP_SCOPE_INIT_INFO
    {
      public uint cbSize;
      public uint flType;
      public uint flScope;
      public WindowsUserPicker.DSOP_FILTER_FLAGS FilterFlags;
      [MarshalAs(UnmanagedType.LPWStr)]
      public string pwzDcName;
      [MarshalAs(UnmanagedType.LPWStr)]
      public string pwzADsPath;
      public uint hr;
    }

    private enum DsInitInfo
    {
      DSOP_FLAG_MULTISELECT = 1,
      DSOP_FLAG_SKIP_TARGET_COMPUTER_DC_CHECK = 2,
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct DSOP_INIT_INFO
    {
      public uint cbSize;
      [MarshalAs(UnmanagedType.LPWStr)]
      public string pwzTargetComputer;
      public uint cDsScopeInfos;
      public IntPtr aDsScopeInfos;
      public uint flOptions;
      public uint cAttributesToFetch;
      public IntPtr apwzAttributeNames;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct DS_SELECTION
    {
      [MarshalAs(UnmanagedType.LPWStr)]
      public string pwzName;
      [MarshalAs(UnmanagedType.LPWStr)]
      public string pwzADsPath;
      [MarshalAs(UnmanagedType.LPWStr)]
      public string pwzClass;
      [MarshalAs(UnmanagedType.LPWStr)]
      public string pwzUPN;
      internal IntPtr pvarFetchedAttributes;
      public uint flScopeType;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct DS_SELECTION_LIST
    {
      public uint cItems;
      public uint cFetchedAttributes;
    }

    private class DS_SELECTION_ATTRIBUTES
    {
      public object[] attributeValues;
    }

    [Guid("0C87E64E-3B7A-11D2-B9E0-00C04FD8DBF7")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    private interface IDsObjectPicker
    {
      int Initialize(ref WindowsUserPicker.DSOP_INIT_INFO pInitInfo);

      int InvokeDialog(IntPtr hWnd, ref IntPtr ppDoSelections);
    }

    [Guid("17D6CCD8-3B7B-11D2-B9E0-00C04FD8DBF7")]
    [ComImport]
    private class DsObjectPicker
    {
      [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
      public extern DsObjectPicker();
    }

    private enum CLIPFORMAT : uint
    {
      CF_TEXT = 1,
      CF_BITMAP = 2,
      CF_METAFILEPICT = 3,
      CF_SYLK = 4,
      CF_DIF = 5,
      CF_TIFF = 6,
      CF_OEMTEXT = 7,
      CF_DIB = 8,
      CF_PALETTE = 9,
      CF_PENDATA = 10, // 0x0000000A
      CF_RIFF = 11, // 0x0000000B
      CF_WAVE = 12, // 0x0000000C
      CF_UNICODETEXT = 13, // 0x0000000D
      CF_ENHMETAFILE = 14, // 0x0000000E
      CF_HDROP = 15, // 0x0000000F
      CF_LOCALE = 16, // 0x00000010
      CF_MAX = 17, // 0x00000011
      CF_OWNERDISPLAY = 128, // 0x00000080
      CF_DSPTEXT = 129, // 0x00000081
      CF_DSPBITMAP = 130, // 0x00000082
      CF_DSPMETAFILEPICT = 131, // 0x00000083
      CF_DSPENHMETAFILE = 142, // 0x0000008E
      CF_PRIVATEFIRST = 512, // 0x00000200
      CF_PRIVATELAST = 767, // 0x000002FF
      CF_GDIOBJFIRST = 768, // 0x00000300
      CF_GDIOBJLAST = 1023, // 0x000003FF
    }

    private enum DVASPECT : uint
    {
      DVASPECT_CONTENT = 1,
      DVASPECT_THUMBNAIL = 2,
      DVASPECT_ICON = 4,
      DVASPECT_DOCPRINT = 8,
    }

    private enum TYMED : uint
    {
      TYMED_NULL = 0,
      TYMED_HGLOBAL = 1,
      TYMED_FILE = 2,
      TYMED_ISTREAM = 4,
      TYMED_ISTORAGE = 8,
      TYMED_GDI = 16, // 0x00000010
      TYMED_MFPICT = 32, // 0x00000020
      TYMED_ENHMF = 64, // 0x00000040
    }

    private struct FORMATETC
    {
      public WindowsUserPicker.CLIPFORMAT cfFormat;
      internal IntPtr ptd;
      public WindowsUserPicker.DVASPECT dwAspect;
      public int lindex;
      public WindowsUserPicker.TYMED tymed;
    }

    private struct STGMEDIUM
    {
      internal WindowsUserPicker.TYMED tymed;
      internal IntPtr hGlobal;
      internal IntPtr pUnkForRelease;
    }

    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("0000010e-0000-0000-C000-000000000046")]
    [ComImport]
    private interface IDataObject
    {
      [MethodImpl(MethodImplOptions.PreserveSig)]
      int GetData(ref WindowsUserPicker.FORMATETC a, ref WindowsUserPicker.STGMEDIUM b);

      [MethodImpl(MethodImplOptions.PreserveSig)]
      void GetDataHere(int a, ref WindowsUserPicker.STGMEDIUM b);

      [MethodImpl(MethodImplOptions.PreserveSig)]
      int QueryGetData(int a);

      [MethodImpl(MethodImplOptions.PreserveSig)]
      int GetCanonicalFormatEtc(int a, ref int b);

      [MethodImpl(MethodImplOptions.PreserveSig)]
      int SetData(int a, int b, int c);

      [MethodImpl(MethodImplOptions.PreserveSig)]
      int EnumFormatEtc(uint a, ref object b);

      [MethodImpl(MethodImplOptions.PreserveSig)]
      int DAdvise(int a, uint b, object c, ref uint d);

      [MethodImpl(MethodImplOptions.PreserveSig)]
      int DUnadvise(uint a);

      [MethodImpl(MethodImplOptions.PreserveSig)]
      int EnumDAdvise(ref object a);
    }

    [Guid("7CABCF1E-78F5-11D2-960C-00C04FA31A86")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    private interface IDsBrowseDomainTree
    {
      uint BrowseTo(IntPtr hWnd, ref IntPtr targetPath, uint dwFlags);

      int GetDomains(ref IntPtr domainTree, int dwFlags);

      int FreeDomains(ref IntPtr domainTree);

      int FlushCachedDomains();

      int SetComputer([MarshalAs(UnmanagedType.LPWStr)] string computerName, [MarshalAs(UnmanagedType.LPWStr)] string userName, [MarshalAs(UnmanagedType.LPWStr)] string password);
    }

    private enum BrowseType
    {
      BrowseForComputer,
      BrowseForShare,
      BrowseForFolder,
    }

    private class ObjectPickerWrapper
    {
      public const int S_OK = 0;
      public const int S_FALSE = 1;

      [DllImport("kernel32.dll")]
      private static extern IntPtr GlobalLock(IntPtr hMem);

      [DllImport("kernel32.dll")]
      private static extern bool GlobalUnlock(IntPtr hMem);

      [DllImport("ole32")]
      public static extern void ReleaseStgMedium(ref WindowsUserPicker.STGMEDIUM pmedium);

      private ObjectPickerWrapper()
      {
      }

      public static void DisplayObjectPicker(
        IntPtr hWnd,
        string machineName,
        bool IsMultiSelected,
        uint uplevelFilterFlags,
        uint downlevelFilterFlags,
        uint flType,
        string[] attributeNames,
        ref int count,
        ref WindowsUserPicker.DS_SELECTION[] dataArray,
        ref int dsAttributesCount,
        ref WindowsUserPicker.DS_SELECTION_ATTRIBUTES[] dsAttributes)
      {
        WindowsUserPicker.IDsObjectPicker dsObjectPicker = (WindowsUserPicker.IDsObjectPicker) new WindowsUserPicker.DsObjectPicker();
        IntPtr zero1 = IntPtr.Zero;
        WindowsUserPicker.DSOP_INIT_INFO pInitInfo = new WindowsUserPicker.DSOP_INIT_INFO();
        WindowsUserPicker.DSOP_SCOPE_INIT_INFO[] dsopScopeInitInfoArray = new WindowsUserPicker.DSOP_SCOPE_INIT_INFO[3];
        dsopScopeInitInfoArray[0].cbSize = (uint) Marshal.SizeOf<WindowsUserPicker.DSOP_SCOPE_INIT_INFO>(dsopScopeInitInfoArray[0]);
        dsopScopeInitInfoArray[0].flType = flType;
        dsopScopeInitInfoArray[0].FilterFlags.Uplevel.flBothModes = uplevelFilterFlags;
        dsopScopeInitInfoArray[0].FilterFlags.flDownlevel = downlevelFilterFlags;
        dsopScopeInitInfoArray[0].flScope = 200U;
        dsopScopeInitInfoArray[0].pwzDcName = "";
        dsopScopeInitInfoArray[0].pwzADsPath = "";
        dsopScopeInitInfoArray[0].hr = 0U;
        dsopScopeInitInfoArray[1].cbSize = (uint) Marshal.SizeOf<WindowsUserPicker.DSOP_SCOPE_INIT_INFO>(dsopScopeInitInfoArray[1]);
        dsopScopeInitInfoArray[1].flType = 24U;
        dsopScopeInitInfoArray[1].FilterFlags.Uplevel.flBothModes = uplevelFilterFlags;
        dsopScopeInitInfoArray[1].FilterFlags.flDownlevel = downlevelFilterFlags;
        dsopScopeInitInfoArray[1].flScope = 8U;
        dsopScopeInitInfoArray[1].pwzDcName = "";
        dsopScopeInitInfoArray[1].pwzADsPath = "";
        dsopScopeInitInfoArray[1].hr = 0U;
        dsopScopeInitInfoArray[2].cbSize = (uint) Marshal.SizeOf<WindowsUserPicker.DSOP_SCOPE_INIT_INFO>(dsopScopeInitInfoArray[2]);
        dsopScopeInitInfoArray[2].flType = 2U;
        dsopScopeInitInfoArray[2].FilterFlags.Uplevel.flBothModes = uplevelFilterFlags;
        dsopScopeInitInfoArray[2].FilterFlags.flDownlevel = downlevelFilterFlags;
        dsopScopeInitInfoArray[2].flScope = 8U;
        dsopScopeInitInfoArray[2].pwzDcName = "";
        dsopScopeInitInfoArray[2].pwzADsPath = "";
        dsopScopeInitInfoArray[2].hr = 0U;
        dsopScopeInitInfoArray[2].flScope |= 1U;
        IntPtr hglobal = Marshal.AllocHGlobal(Marshal.SizeOf<WindowsUserPicker.DSOP_SCOPE_INIT_INFO>(dsopScopeInitInfoArray[0]) * 3);
        IntPtr ptr1 = hglobal;
        pInitInfo.cbSize = (uint) Marshal.SizeOf<WindowsUserPicker.DSOP_INIT_INFO>(pInitInfo);
        pInitInfo.pwzTargetComputer = machineName;
        pInitInfo.cDsScopeInfos = 3U;
        pInitInfo.aDsScopeInfos = hglobal;
        pInitInfo.flOptions = !IsMultiSelected ? (2048 != ((int) uplevelFilterFlags & 2048) ? 0U : 2U) : 1U;
        try
        {
          Marshal.StructureToPtr<WindowsUserPicker.DSOP_SCOPE_INIT_INFO>(dsopScopeInitInfoArray[0], ptr1, false);
          IntPtr ptr2 = ptr1 + Marshal.SizeOf<WindowsUserPicker.DSOP_SCOPE_INIT_INFO>(dsopScopeInitInfoArray[0]);
          Marshal.StructureToPtr<WindowsUserPicker.DSOP_SCOPE_INIT_INFO>(dsopScopeInitInfoArray[1], ptr2, false);
          IntPtr ptr3 = ptr2 + Marshal.SizeOf<WindowsUserPicker.DSOP_SCOPE_INIT_INFO>(dsopScopeInitInfoArray[0]);
          Marshal.StructureToPtr<WindowsUserPicker.DSOP_SCOPE_INIT_INFO>(dsopScopeInitInfoArray[2], ptr3, false);
          uint length = (uint) attributeNames.Length;
          pInitInfo.cAttributesToFetch = length;
          if (length > 0U)
          {
            WindowsUserPicker.ObjectPickerWrapper.MarshalStrings marshalStrings = new WindowsUserPicker.ObjectPickerWrapper.MarshalStrings(attributeNames);
            pInitInfo.apwzAttributeNames = marshalStrings.ArrayPtr;
          }
          else
            pInitInfo.apwzAttributeNames = IntPtr.Zero;
          if (dsObjectPicker.Initialize(ref pInitInfo) != 0)
            return;
          IntPtr zero2 = IntPtr.Zero;
          if (dsObjectPicker.InvokeDialog(hWnd, ref zero2) != 0)
            return;
          if (!(IntPtr.Zero != zero2))
            return;
          try
          {
            WindowsUserPicker.IDataObject objectForIunknown = (WindowsUserPicker.IDataObject) Marshal.GetTypedObjectForIUnknown(zero2, typeof (WindowsUserPicker.IDataObject));
            DataFormats.Format format = DataFormats.GetFormat("CFSTR_DSOP_DS_SELECTION_LIST");
            WindowsUserPicker.FORMATETC a = new WindowsUserPicker.FORMATETC();
            WindowsUserPicker.STGMEDIUM stgmedium = new WindowsUserPicker.STGMEDIUM();
            a.cfFormat = (WindowsUserPicker.CLIPFORMAT) format.Id;
            a.ptd = IntPtr.Zero;
            a.dwAspect = WindowsUserPicker.DVASPECT.DVASPECT_CONTENT;
            a.lindex = -1;
            a.tymed = WindowsUserPicker.TYMED.TYMED_HGLOBAL;
            if (objectForIunknown.GetData(ref a, ref stgmedium) != 0)
              return;
            IntPtr zero3 = IntPtr.Zero;
            IntPtr ptr4 = WindowsUserPicker.ObjectPickerWrapper.GlobalLock(stgmedium.hGlobal);
            WindowsUserPicker.DS_SELECTION_LIST structure = (WindowsUserPicker.DS_SELECTION_LIST) Marshal.PtrToStructure(ptr4, typeof (WindowsUserPicker.DS_SELECTION_LIST));
            count = (int) structure.cItems;
            dsAttributesCount = (int) structure.cFetchedAttributes;
            if (count != 0)
            {
              IntPtr ptr5 = (IntPtr) ((long) ptr4 + (long) Marshal.SizeOf<WindowsUserPicker.DS_SELECTION_LIST>(structure));
              dataArray = new WindowsUserPicker.DS_SELECTION[count];
              dsAttributes = new WindowsUserPicker.DS_SELECTION_ATTRIBUTES[count];
              for (int index = 0; index < count; ++index)
              {
                dataArray[index] = new WindowsUserPicker.DS_SELECTION();
                dataArray[index] = (WindowsUserPicker.DS_SELECTION) Marshal.PtrToStructure(ptr5, typeof (WindowsUserPicker.DS_SELECTION));
                if (length > 0U && dsAttributesCount > 0)
                {
                  dsAttributes[index] = new WindowsUserPicker.DS_SELECTION_ATTRIBUTES();
                  dsAttributes[index].attributeValues = Marshal.GetObjectsForNativeVariants(dataArray[index].pvarFetchedAttributes, dsAttributesCount);
                }
                ptr5 += Marshal.SizeOf<WindowsUserPicker.DS_SELECTION>(dataArray[index]);
              }
            }
            WindowsUserPicker.ObjectPickerWrapper.GlobalUnlock(stgmedium.hGlobal);
            WindowsUserPicker.ObjectPickerWrapper.ReleaseStgMedium(ref stgmedium);
          }
          finally
          {
            Marshal.Release(zero2);
          }
        }
        catch
        {
          throw;
        }
        finally
        {
          Marshal.FreeHGlobal(hglobal);
        }
      }

      internal static void GetSelectedAccounts(
        IntPtr hWnd,
        string machineName,
        bool IsMultiSelected,
        bool AllowGroups,
        bool IncludeComputers,
        string[] attributeNames,
        ref int count,
        ref WindowsUserPicker.DS_SELECTION[] dataArray,
        ref int dsAttributesCount,
        ref WindowsUserPicker.DS_SELECTION_ATTRIBUTES[] dsAttributes)
      {
        uint uplevelFilterFlags = 2;
        uint downlevelFilterFlags = 2147483649;
        if (AllowGroups)
        {
          uplevelFilterFlags |= 672U;
          downlevelFilterFlags |= 2147614726U;
        }
        if (IncludeComputers)
        {
          uplevelFilterFlags |= 2048U;
          downlevelFilterFlags |= 2147483656U;
        }
        uint flType = !VssStringComparer.Hostname.Equals(Environment.UserDomainName, Environment.MachineName) ? 935U : 1U;
        WindowsUserPicker.ObjectPickerWrapper.DisplayObjectPicker(hWnd, machineName, IsMultiSelected, uplevelFilterFlags, downlevelFilterFlags, flType, attributeNames, ref count, ref dataArray, ref dsAttributesCount, ref dsAttributes);
      }

      internal sealed class MarshalStrings
      {
        private IntPtr m_taskAlloc;
        private readonly int m_length;
        private IntPtr[] m_strings;

        public MarshalStrings(string[] theArray)
        {
          int size = IntPtr.Size;
          if (theArray == null)
            return;
          this.m_length = theArray.Length;
          this.m_strings = new IntPtr[this.m_length];
          this.m_taskAlloc = Marshal.AllocCoTaskMem(this.m_length * size);
          for (int index = this.m_length - 1; index >= 0; --index)
          {
            this.m_strings[index] = Marshal.StringToCoTaskMemUni(theArray[index]);
            Marshal.WriteIntPtr(this.m_taskAlloc, index * size, this.m_strings[index]);
          }
        }

        public IntPtr ArrayPtr => this.m_taskAlloc;

        ~MarshalStrings()
        {
          if (!(this.m_taskAlloc != IntPtr.Zero))
            return;
          Marshal.FreeCoTaskMem(this.m_taskAlloc);
          int length = this.m_length;
          while (length-- != 0)
            Marshal.FreeCoTaskMem(this.m_strings[length]);
        }
      }
    }
  }
}
