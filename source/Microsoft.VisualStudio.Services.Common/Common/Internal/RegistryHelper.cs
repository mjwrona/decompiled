// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Common.Internal.RegistryHelper
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.AccessControl;
using System.Text;

namespace Microsoft.VisualStudio.Services.Common.Internal
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  [CLSCompliant(false)]
  public static class RegistryHelper
  {
    private static uint MAX_KEY_LENGTH = (uint) byte.MaxValue;
    private static uint MAX_VALUE_NAME = 16383;
    private static int ERROR_FILE_NOT_FOUND = 2;
    private static IntPtr HKEY_CLASSES_ROOT = new IntPtr(int.MinValue);
    private static IntPtr HKEY_CURRENT_USER = new IntPtr(-2147483647);
    private static IntPtr HKEY_LOCAL_MACHINE = new IntPtr(-2147483646);
    private static IntPtr HKEY_USERS = new IntPtr(-2147483645);
    private static IntPtr HKEY_CURRENT_CONFIG = new IntPtr(-2147483643);

    public static SafeHandle CreateSubKey(
      RegistryHive hive,
      string subKey,
      RegistryAccessMask accessMask)
    {
      using (RegistryHelper.SafeRegistryHandle key = new RegistryHelper.SafeRegistryHandle(RegistryHelper.Convert(hive)))
        return RegistryHelper.CreateSubKey((SafeHandle) key, subKey, accessMask);
    }

    public static SafeHandle CreateSubKey(
      SafeHandle key,
      string subKey,
      RegistryAccessMask accessMask)
    {
      return RegistryHelper.CreateSubKey(key, subKey, accessMask, out bool _);
    }

    public static SafeHandle CreateSubKey(
      SafeHandle key,
      string subKey,
      RegistryAccessMask accessMask,
      out bool createdNew)
    {
      ArgumentUtility.CheckForNull<SafeHandle>(key, nameof (key));
      ArgumentUtility.CheckStringForNullOrEmpty(subKey, nameof (subKey));
      RegistryHelper.SafeRegistryHandle phkResult;
      RegistryHelper.RegistryDispositionValue lpdwDisposition;
      int keyEx = RegistryHelper.RegCreateKeyEx(key, subKey, 0U, (string) null, RegistryHelper.RegistryOptions.NonVolatile, accessMask, IntPtr.Zero, out phkResult, out lpdwDisposition);
      if (keyEx != 0)
        throw new Win32Exception(keyEx);
      createdNew = lpdwDisposition == RegistryHelper.RegistryDispositionValue.CreatedNewKey;
      return (SafeHandle) phkResult;
    }

    public static SafeHandle CreateSubKey(
      RegistryHive hive,
      string subKey,
      RegistryAccessMask accessMask,
      RegistrySecurity security)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(subKey, nameof (subKey));
      ArgumentUtility.CheckForNull<RegistrySecurity>(security, nameof (security));
      using (RegistryHelper.SafeRegistryHandle key = new RegistryHelper.SafeRegistryHandle(RegistryHelper.Convert(hive)))
        return RegistryHelper.CreateSubKey((SafeHandle) key, subKey, accessMask, security);
    }

    public static SafeHandle CreateSubKey(
      SafeHandle key,
      string subKey,
      RegistryAccessMask accessMask,
      RegistrySecurity security)
    {
      ArgumentUtility.CheckForNull<SafeHandle>(key, nameof (key));
      ArgumentUtility.CheckStringForNullOrEmpty(subKey, nameof (subKey));
      ArgumentUtility.CheckForNull<RegistrySecurity>(security, nameof (security));
      IntPtr num1 = IntPtr.Zero;
      IntPtr num2 = IntPtr.Zero;
      int error = 6;
      RegistryHelper.SafeRegistryHandle phkResult;
      try
      {
        NativeMethods.SECURITY_ATTRIBUTES structure = new NativeMethods.SECURITY_ATTRIBUTES();
        structure.nLength = Marshal.SizeOf<NativeMethods.SECURITY_ATTRIBUTES>(structure);
        num2 = Marshal.AllocHGlobal(structure.nLength);
        byte[] descriptorBinaryForm = security.GetSecurityDescriptorBinaryForm();
        num1 = Marshal.AllocHGlobal(descriptorBinaryForm.Length);
        Marshal.Copy(descriptorBinaryForm, 0, num1, descriptorBinaryForm.Length);
        structure.lpSecurityDescriptor = num1;
        Marshal.StructureToPtr<NativeMethods.SECURITY_ATTRIBUTES>(structure, num2, true);
        error = RegistryHelper.RegCreateKeyEx(key, subKey, 0U, (string) null, RegistryHelper.RegistryOptions.NonVolatile, accessMask, num2, out phkResult, out RegistryHelper.RegistryDispositionValue _);
      }
      finally
      {
        Marshal.FreeHGlobal(num1);
        Marshal.FreeHGlobal(num2);
      }
      if (error != 0)
        throw new Win32Exception(error);
      return (SafeHandle) phkResult;
    }

    public static void DeleteSubKeyTree(
      RegistryHive hive,
      string subKey,
      RegistryAccessMask accessMask)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(subKey, nameof (subKey));
      int error = !RegistryHelper.IsWin2k3OrGreater() ? RegistryHelper.RegDeleteKey(RegistryHelper.Convert(hive), subKey) : RegistryHelper.RegDeleteKeyEx(RegistryHelper.Convert(hive), subKey, accessMask & (RegistryAccessMask.Wow6432Key | RegistryAccessMask.Wow6464Key), 0U);
      if (error != 0 && error != 2)
        throw new Win32Exception(error);
    }

    public static void DeleteKeyTree(SafeHandle registryKey)
    {
      int error1 = RegistryHelper.RegDeleteTree(registryKey, string.Empty);
      switch (error1)
      {
        case 0:
        case 2:
          int error2 = RegistryHelper.RegDeleteKey(registryKey, string.Empty);
          switch (error2)
          {
            case 0:
              return;
            case 2:
              return;
            default:
              throw new Win32Exception(error2);
          }
        default:
          throw new Win32Exception(error1);
      }
    }

    public static void DeleteKeyValue(SafeHandle registryKey, string valueName)
    {
      int error = RegistryHelper.RegDeleteValue(registryKey, valueName);
      switch (error)
      {
        case 0:
          break;
        case 2:
          break;
        default:
          throw new Win32Exception(error);
      }
    }

    public static void NotifyChangeKeyValue(
      SafeHandle hKey,
      bool watchSubtree,
      RegistryChangeNotificationFilter dwNotifyFilter,
      SafeHandle hEvent,
      bool fAsynchronous)
    {
      int error = RegistryHelper.RegNotifyChangeKeyValue(hKey, watchSubtree, dwNotifyFilter, hEvent, fAsynchronous);
      if (error != 0)
        throw new Win32Exception(error);
    }

    public static SafeHandle OpenSubKey(
      RegistryHive hive,
      string subKey,
      RegistryAccessMask accessMask)
    {
      RegistryHelper.SafeRegistryHandle hkResult;
      int error = RegistryHelper.RegOpenKeyEx(RegistryHelper.Convert(hive), subKey, 0, accessMask, out hkResult);
      if (error == 0 && !hkResult.IsInvalid)
        return (SafeHandle) hkResult;
      if (error == RegistryHelper.ERROR_FILE_NOT_FOUND)
        return (SafeHandle) null;
      throw new Win32Exception(error);
    }

    public static object GetValue(SafeHandle registryKey, string name, object defaultValue)
    {
      ArgumentUtility.CheckForNull<SafeHandle>(registryKey, nameof (registryKey));
      ArgumentUtility.CheckStringForNullOrEmpty(name, nameof (name));
      uint valueSize = 0;
      IntPtr zero = IntPtr.Zero;
      RegistryValueKind registryValueKind;
      int error = !RegistryHelper.IsWin2k3OrGreater() ? RegistryHelper.RegQueryValueEx(registryKey, name, IntPtr.Zero, out registryValueKind, zero, ref valueSize) : RegistryHelper.RegGetValue(registryKey, (string) null, name, RegistryHelper.RegistryValueRestrictions.Any, out registryValueKind, zero, ref valueSize);
      if (error != 0)
      {
        if (error == RegistryHelper.ERROR_FILE_NOT_FOUND)
          return defaultValue;
        throw new Win32Exception(error);
      }
      object obj;
      return !RegistryHelper.TryExtractValue(registryKey, name, valueSize, out obj) ? defaultValue : obj;
    }

    public static void SetValue(SafeHandle registryKey, string name, string value)
    {
      if (value == null)
        value = string.Empty;
      int error = RegistryHelper.RegSetValueEx(registryKey, name, 0U, RegistryValueKind.String, value, (uint) ((value.Length + 1) * Marshal.SystemDefaultCharSize));
      if (error != 0)
        throw new Win32Exception(error);
    }

    public static void SetValue(SafeHandle registryKey, string name, int value)
    {
      int error = RegistryHelper.RegSetValueEx(registryKey, name, 0U, RegistryValueKind.DWord, ref value, (uint) Marshal.SizeOf(typeof (int)));
      if (error != 0)
        throw new Win32Exception(error);
    }

    public static void SetValue(SafeHandle registryKey, string name, SecureString value)
    {
      if (value == null)
        value = new SecureString();
      IntPtr coTaskMemUnicode = Marshal.SecureStringToCoTaskMemUnicode(value);
      int error = RegistryHelper.RegSetValueEx(registryKey, name, 0U, RegistryValueKind.String, coTaskMemUnicode, (uint) ((value.Length + 1) * Marshal.SystemDefaultCharSize));
      Marshal.ZeroFreeCoTaskMemUnicode(coTaskMemUnicode);
      if (error != 0)
        throw new Win32Exception(error);
    }

    public static string[] GetSubKeyNames(SafeHandle registryKey)
    {
      uint lpcbClass = 260;
      uint lpcSubKeys;
      int error1 = RegistryHelper.RegQueryInfoKey(registryKey, out StringBuilder _, ref lpcbClass, IntPtr.Zero, out lpcSubKeys, out uint _, out uint _, out uint _, out uint _, out uint _, out uint _, IntPtr.Zero);
      if (error1 != 0)
        throw new Win32Exception(error1);
      string[] subKeyNames = new string[(int) lpcSubKeys];
      for (uint index = 0; index < lpcSubKeys; ++index)
      {
        uint maxKeyLength = RegistryHelper.MAX_KEY_LENGTH;
        StringBuilder lpName = new StringBuilder((int) RegistryHelper.MAX_KEY_LENGTH);
        int error2 = RegistryHelper.RegEnumKeyEx(registryKey, index, lpName, ref maxKeyLength, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, out long _);
        if (error2 != 0)
          throw new Win32Exception(error2);
        subKeyNames[(int) index] = lpName.ToString();
      }
      return subKeyNames;
    }

    public static string[] GetValueNames(SafeHandle registryKey)
    {
      uint lpcbClass = 260;
      uint lpcValues;
      int error = RegistryHelper.RegQueryInfoKey(registryKey, out StringBuilder _, ref lpcbClass, IntPtr.Zero, out uint _, out uint _, out uint _, out lpcValues, out uint _, out uint _, out uint _, IntPtr.Zero);
      if (error != 0)
        throw new Win32Exception(error);
      string[] valueNames = new string[(int) lpcValues];
      for (uint dwIndex = 0; dwIndex < lpcValues; ++dwIndex)
      {
        uint maxValueName = RegistryHelper.MAX_VALUE_NAME;
        StringBuilder lpValueName = new StringBuilder((int) RegistryHelper.MAX_VALUE_NAME);
        if (RegistryHelper.RegEnumValue(registryKey, dwIndex, lpValueName, ref maxValueName, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero) != 0U)
          throw new Win32Exception(error);
        valueNames[(int) dwIndex] = lpValueName.ToString();
      }
      return valueNames;
    }

    public static void QueryInfoKey(
      SafeHandle hKey,
      out StringBuilder lpClass,
      ref uint lpcbClass,
      out uint lpcSubKeys,
      out uint lpcbMaxSubKeyLen,
      out uint lpcbMaxClassLen,
      out uint lpcValues,
      out uint lpcbMaxValueNameLen,
      out uint lpcbMaxValueLen,
      out uint lpcbSecurityDescriptor,
      IntPtr lpftLastWriteTime)
    {
      ArgumentUtility.CheckForNull<SafeHandle>(hKey, nameof (hKey));
      int error = RegistryHelper.RegQueryInfoKey(hKey, out lpClass, ref lpcbClass, IntPtr.Zero, out lpcSubKeys, out lpcbMaxSubKeyLen, out lpcbMaxClassLen, out lpcValues, out lpcbMaxValueNameLen, out lpcbMaxValueLen, out lpcbSecurityDescriptor, lpftLastWriteTime);
      if (error != 0)
        throw new Win32Exception(error);
    }

    private static IntPtr Convert(RegistryHive hive)
    {
      IntPtr num = IntPtr.Zero;
      switch (hive)
      {
        case RegistryHive.ClassesRoot:
          num = RegistryHelper.HKEY_CLASSES_ROOT;
          break;
        case RegistryHive.CurrentUser:
          num = RegistryHelper.HKEY_CURRENT_USER;
          break;
        case RegistryHive.LocalMachine:
          num = RegistryHelper.HKEY_LOCAL_MACHINE;
          break;
        case RegistryHive.Users:
          num = RegistryHelper.HKEY_USERS;
          break;
        case RegistryHive.CurrentConfig:
          num = RegistryHelper.HKEY_CURRENT_CONFIG;
          break;
      }
      return num;
    }

    public static bool SetKeySecurity(SafeHandle registryKey, RegistrySecurity security)
    {
      ArgumentUtility.CheckForNull<SafeHandle>(registryKey, nameof (registryKey));
      ArgumentUtility.CheckForNull<RegistrySecurity>(security, nameof (security));
      byte[] descriptorBinaryForm = security.GetSecurityDescriptorBinaryForm();
      IntPtr num = Marshal.AllocHGlobal(descriptorBinaryForm.Length);
      int error = 6;
      try
      {
        Marshal.Copy(descriptorBinaryForm, 0, num, descriptorBinaryForm.Length);
        error = RegistryHelper.RegSetKeySecurity(registryKey, RegistrySecurityInformation.Dacl, num);
      }
      finally
      {
        Marshal.FreeHGlobal(num);
      }
      if (error != 0)
        throw new Win32Exception(error);
      return true;
    }

    private static bool TryExtractValue(
      SafeHandle registryKey,
      string name,
      uint valueSize,
      out object value)
    {
      IntPtr num1 = IntPtr.Zero;
      try
      {
        num1 = Marshal.AllocHGlobal((int) valueSize);
        RegistryValueKind registryValueKind;
        if ((!RegistryHelper.IsWin2k3OrGreater() ? RegistryHelper.RegQueryValueEx(registryKey, name, IntPtr.Zero, out registryValueKind, num1, ref valueSize) : RegistryHelper.RegGetValue(registryKey, (string) null, name, RegistryHelper.RegistryValueRestrictions.Any, out registryValueKind, num1, ref valueSize)) != 0)
        {
          value = (object) null;
          return false;
        }
        switch (registryValueKind)
        {
          case RegistryValueKind.String:
            value = (object) Marshal.PtrToStringUni(num1);
            break;
          case RegistryValueKind.DWord:
            value = (object) Marshal.ReadInt32(num1);
            break;
          case RegistryValueKind.MultiString:
            char[] chArray = new char[(int) valueSize];
            uint ofs = 0;
            uint num2 = 0;
            for (; ofs < valueSize; ofs += 2U)
              chArray[(int) num2++] = System.Convert.ToChar(Marshal.ReadInt16(num1, (int) ofs));
            value = (object) chArray;
            break;
          default:
            value = (object) null;
            return false;
        }
      }
      finally
      {
        Marshal.FreeHGlobal(num1);
      }
      return true;
    }

    private static bool IsWin2k3OrGreater()
    {
      if (Environment.OSVersion.Version.Major >= 6)
        return true;
      return Environment.OSVersion.Version.Major == 5 && Environment.OSVersion.Version.Minor >= 2;
    }

    public static string Get32BitRegistryKeyPath(string keyPath)
    {
      ArgumentUtility.CheckForNull<string>(keyPath, nameof (keyPath));
      if (OSDetails.Is64BitNotWow64)
      {
        string pattern = "Software";
        if (VssStringComparer.RegistryPath.StartsWith(keyPath, pattern) && (keyPath.Length == pattern.Length || keyPath[pattern.Length] == '\\'))
        {
          string str = keyPath.Substring(pattern.Length);
          return pattern + "\\Wow6432Node" + str;
        }
      }
      return keyPath;
    }

    public static bool CheckRegistryFlag(
      RegistryAccessMask wowKind,
      RegistryHive hive,
      string keyPath,
      string valueName,
      int value)
    {
      return RegistryHelper.TestRegistryFlag(wowKind, hive, keyPath, valueName, (object) value);
    }

    public static bool CheckRegistryFlag(
      RegistryAccessMask wowKind,
      RegistryHive hive,
      string keyPath,
      string valueName,
      string value)
    {
      return RegistryHelper.TestRegistryFlag(wowKind, hive, keyPath, valueName, (object) value);
    }

    public static bool CheckRegistryFlag(
      RegistryAccessMask wowKind,
      RegistryHive hive,
      string keyPath,
      string valueName)
    {
      return RegistryHelper.TestRegistryFlag(wowKind, hive, keyPath, valueName);
    }

    public static bool CheckRegistryFlag(
      RegistryAccessMask wowKind,
      RegistryHive hive,
      string keyPath)
    {
      return RegistryHelper.TestRegistryFlag(wowKind, hive, keyPath);
    }

    private static bool TestRegistryFlag(
      RegistryAccessMask wowKind,
      RegistryHive hive,
      string keyPath,
      string valueName = null,
      object value = null)
    {
      bool flag = false;
      using (SafeHandle safeHandle = RegistryHelper.OpenSubKey(hive, keyPath, wowKind | RegistryAccessMask.Execute | RegistryAccessMask.QueryValue))
      {
        if (safeHandle != null)
        {
          if (valueName == null)
            flag = true;
          else if (value == null)
          {
            flag = ((IEnumerable<string>) RegistryHelper.GetValueNames(safeHandle)).Any<string>((Func<string, bool>) (name => name.Equals(valueName, StringComparison.OrdinalIgnoreCase)));
          }
          else
          {
            uint pcbData = 0;
            RegistryValueKind pdwType;
            if (RegistryHelper.RegGetValue(safeHandle, (string) null, valueName, RegistryHelper.RegistryValueRestrictions.Any, out pdwType, IntPtr.Zero, ref pcbData) == 0)
            {
              object obj;
              if (RegistryHelper.TryExtractValue(safeHandle, valueName, pcbData, out obj))
              {
                switch (pdwType)
                {
                  case RegistryValueKind.String:
                    flag = value is string && ((string) value).Equals((string) obj, StringComparison.OrdinalIgnoreCase);
                    break;
                  case RegistryValueKind.DWord:
                    flag = value is int num && num == (int) obj;
                    break;
                }
              }
            }
          }
        }
      }
      return flag;
    }

    [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern int RegCreateKeyEx(
      SafeHandle registryKey,
      [MarshalAs(UnmanagedType.LPWStr)] string lpSubKey,
      uint dwReserved,
      [MarshalAs(UnmanagedType.LPWStr)] string lpClass,
      RegistryHelper.RegistryOptions dwOptions,
      RegistryAccessMask samDesired,
      IntPtr lpSecurityAttributes,
      out RegistryHelper.SafeRegistryHandle phkResult,
      out RegistryHelper.RegistryDispositionValue lpdwDisposition);

    [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern int RegDeleteKey(IntPtr hKey, [MarshalAs(UnmanagedType.LPWStr)] string lpSubKey);

    [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern int RegDeleteKey(SafeHandle hKey, [MarshalAs(UnmanagedType.LPWStr)] string lpSubKey);

    [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern int RegDeleteTree(SafeHandle hKey, [MarshalAs(UnmanagedType.LPWStr)] string lpSubKey);

    [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern int RegDeleteKeyEx(
      IntPtr hKey,
      [MarshalAs(UnmanagedType.LPWStr)] string lpSubKey,
      RegistryAccessMask samDesired,
      uint Reserved);

    [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern int RegDeleteValue(SafeHandle hKey, [MarshalAs(UnmanagedType.LPWStr)] string lpValueName);

    [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern int RegGetValue(
      SafeHandle hKey,
      [MarshalAs(UnmanagedType.LPWStr)] string lpSubKey,
      [MarshalAs(UnmanagedType.LPWStr)] string lpValue,
      RegistryHelper.RegistryValueRestrictions dwFlags,
      out RegistryValueKind pdwType,
      IntPtr pvData,
      ref uint pcbData);

    [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern int RegQueryValueEx(
      SafeHandle registryKey,
      [MarshalAs(UnmanagedType.LPWStr)] string lpValueName,
      IntPtr lpReserved,
      out RegistryValueKind lpType,
      IntPtr lpData,
      ref uint lpcbData);

    [DllImport("advapi32.dll", SetLastError = true)]
    private static extern int RegNotifyChangeKeyValue(
      SafeHandle hKey,
      bool watchSubtree,
      [MarshalAs(UnmanagedType.U4)] RegistryChangeNotificationFilter dwNotifyFilter,
      SafeHandle hEvent,
      bool fAsynchronous);

    [DllImport("advapi32.dll", CharSet = CharSet.Auto)]
    private static extern int RegOpenKeyEx(
      IntPtr hKey,
      [MarshalAs(UnmanagedType.LPWStr)] string lpSubKey,
      int ulOptions,
      RegistryAccessMask samDesired,
      out RegistryHelper.SafeRegistryHandle hkResult);

    [DllImport("advapi32.dll", CharSet = CharSet.Auto)]
    private static extern int RegSetValueEx(
      SafeHandle hKey,
      [MarshalAs(UnmanagedType.LPWStr)] string lpValueName,
      uint Reserved,
      [MarshalAs(UnmanagedType.U4)] RegistryValueKind dwType,
      ref int lpData,
      uint cbData);

    [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern int RegSetValueEx(
      SafeHandle hKey,
      [MarshalAs(UnmanagedType.LPWStr)] string lpValueName,
      uint Reserved,
      [MarshalAs(UnmanagedType.U4)] RegistryValueKind dwType,
      [MarshalAs(UnmanagedType.LPWStr)] string lpData,
      uint cbData);

    [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern int RegSetValueEx(
      SafeHandle hKey,
      [MarshalAs(UnmanagedType.LPWStr)] string lpValueName,
      uint Reserved,
      [MarshalAs(UnmanagedType.U4)] RegistryValueKind dwType,
      IntPtr lpData,
      uint cbData);

    [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern int RegQueryInfoKey(
      SafeHandle hKey,
      out StringBuilder lpClass,
      ref uint lpcbClass,
      IntPtr lpReserved,
      out uint lpcSubKeys,
      out uint lpcbMaxSubKeyLen,
      out uint lpcbMaxClassLen,
      out uint lpcValues,
      out uint lpcbMaxValueNameLen,
      out uint lpcbMaxValueLen,
      out uint lpcbSecurityDescriptor,
      IntPtr lpftLastWriteTime);

    [DllImport("advapi32.dll", CharSet = CharSet.Auto)]
    private static extern int RegEnumKeyEx(
      SafeHandle hkey,
      uint index,
      [MarshalAs(UnmanagedType.LPWStr)] StringBuilder lpName,
      ref uint lpcbName,
      IntPtr reserved,
      IntPtr lpClass,
      IntPtr lpcbClass,
      out long lpftLastWriteTime);

    [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern uint RegEnumValue(
      SafeHandle hKey,
      uint dwIndex,
      [MarshalAs(UnmanagedType.LPWStr)] StringBuilder lpValueName,
      ref uint lpcValueName,
      IntPtr lpReserved,
      IntPtr lpType,
      IntPtr lpData,
      IntPtr lpcbData);

    [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern int RegSetKeySecurity(
      SafeHandle registryKey,
      RegistrySecurityInformation securityInformation,
      IntPtr lpSecurityDescriptor);

    private enum RegistryDispositionValue : uint
    {
      CreatedNewKey = 1,
      OpenedExistingKey = 2,
    }

    private enum RegistryOptions : uint
    {
      NonVolatile = 0,
      Volatile = 1,
      CreateLink = 2,
      BackupRestore = 4,
    }

    private enum RegistryValueRestrictions : uint
    {
      DWord = 16, // 0x00000010
      QWord = 64, // 0x00000040
      Any = 65535, // 0x0000FFFF
    }

    internal struct SECURITY_ATTRIBUTES
    {
      public int nLength;
      public IntPtr lpSecurityDescriptor;
      public int bInheritHandle;
    }

    private sealed class SafeRegistryHandle : SafeHandle
    {
      internal SafeRegistryHandle()
        : base(IntPtr.Zero, true)
      {
      }

      public SafeRegistryHandle(IntPtr preexistingHandle)
        : base(preexistingHandle, true)
      {
        this.SetHandle(preexistingHandle);
      }

      [SuppressUnmanagedCodeSecurity]
      [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
      [DllImport("advapi32.dll")]
      private static extern int RegCloseKey(IntPtr hKey);

      [SecurityCritical]
      protected override bool ReleaseHandle() => RegistryHelper.SafeRegistryHandle.RegCloseKey(this.handle) == 0;

      public override bool IsInvalid => !(this.handle != IntPtr.Zero) || this.handle == new IntPtr(-1);
    }
  }
}
