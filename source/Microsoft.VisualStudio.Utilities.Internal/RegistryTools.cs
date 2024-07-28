// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Utilities.Internal.RegistryTools
// Assembly: Microsoft.VisualStudio.Utilities.Internal, Version=14.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E02F1555-E063-4795-BC05-853CA7424F51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Utilities.Internal.dll

using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.AccessControl;

namespace Microsoft.VisualStudio.Utilities.Internal
{
  [ExcludeFromCodeCoverage]
  public class RegistryTools : IRegistryTools4, IRegistryTools3, IRegistryTools2, IRegistryTools
  {
    public int? GetRegistryIntValueFromLocalMachineRoot(
      string regKeyPath,
      string regKeyName,
      int? defaultOnError = null)
    {
      return this.GetRegistryIntValueFromLocalMachineRoot(regKeyPath, regKeyName, false, defaultOnError);
    }

    public int? GetRegistryIntValueFromLocalMachineRoot(
      string regKeyPath,
      string regKeyName,
      bool use64Bit,
      int? defaultOnError = null)
    {
      return (int?) this.GetRegistryValueFromLocalMachineRoot(regKeyPath, regKeyName, use64Bit, (object) defaultOnError);
    }

    public object GetRegistryValueFromLocalMachineRoot(
      string regKeyPath,
      string regKeyName,
      object defaultOnError = null)
    {
      return this.GetRegistryValueFromLocalMachineRoot(regKeyPath, regKeyName, false, defaultOnError);
    }

    public object GetRegistryValueFromLocalMachineRoot(
      string regKeyPath,
      string regKeyName,
      bool use64Bit,
      object defaultOnError = null)
    {
      using (RegistryKey rootKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, use64Bit ? RegistryView.Registry64 : RegistryView.Registry32))
        return this.GetRegistryValue(rootKey, regKeyPath, regKeyName, defaultOnError);
    }

    public object GetRegistryValueFromCurrentUserRoot(
      string regKeyPath,
      string regKeyName,
      object defaultOnError = null)
    {
      return this.GetRegistryValue(Registry.CurrentUser, regKeyPath, regKeyName, defaultOnError);
    }

    public bool TryGetRegistryValueKindFromCurrentUserRoot(
      string regKeyPath,
      string regKeyName,
      out RegistryValueKind kind)
    {
      return this.TryGetRegistryValueKind(Registry.CurrentUser, regKeyPath, regKeyName, out kind);
    }

    public bool TryGetRegistryValueKindFromLocalMachineRoot(
      string regKeyPath,
      string regKeyName,
      out RegistryValueKind kind,
      bool use64Bit = false)
    {
      using (RegistryKey rootKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, use64Bit ? RegistryView.Registry64 : RegistryView.Registry32))
        return this.TryGetRegistryValueKind(rootKey, regKeyPath, regKeyName, out kind);
    }

    public string[] GetRegistryValueNamesFromCurrentUserRoot(string regKeyPath) => this.GetRegistryValueNames(Registry.CurrentUser, regKeyPath);

    public string[] GetRegistryValueNamesFromLocalMachineRoot(string regKeyPath, bool use64Bit = false)
    {
      using (RegistryKey rootKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, use64Bit ? RegistryView.Registry64 : RegistryView.Registry32))
        return this.GetRegistryValueNames(rootKey, regKeyPath);
    }

    public string[] GetRegistrySubKeyNamesFromCurrentUserRoot(string regKeyPath) => this.GetRegistrySubKeyNames(Registry.CurrentUser, regKeyPath);

    public string[] GetRegistrySubKeyNamesFromLocalMachineRoot(string regKeyPath, bool use64Bit = false)
    {
      using (RegistryKey rootKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, use64Bit ? RegistryView.Registry64 : RegistryView.Registry32))
        return this.GetRegistrySubKeyNames(rootKey, regKeyPath);
    }

    public bool DoesRegistryKeyExistInCurrentUserRoot(string regKeyPath) => this.DoesRegistryKeyExist(Registry.CurrentUser, regKeyPath);

    public bool DoesRegistryKeyExistInLocalMachineRoot(string regKeyPath, bool use64Bit = false)
    {
      using (RegistryKey rootKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, use64Bit ? RegistryView.Registry64 : RegistryView.Registry32))
        return this.DoesRegistryKeyExist(rootKey, regKeyPath);
    }

    public bool SetRegistryFromCurrentUserRoot(string regKeyPath, string regKeyName, object value) => this.SetRegistryValue(Registry.CurrentUser, regKeyPath, regKeyName, value);

    public bool SetRegistryFromCurrentUserRoot(
      string regKeyPath,
      string regKeyName,
      object value,
      RegistryValueKind valueKind)
    {
      return this.SetRegistryValue(Registry.CurrentUser, regKeyPath, regKeyName, value, valueKind);
    }

    public bool SetRegistryFromLocalMachineRoot(
      string regKeyPath,
      string regKeyName,
      object value,
      bool use64Bit = false)
    {
      using (RegistryKey rootKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, use64Bit ? RegistryView.Registry64 : RegistryView.Registry32))
        return this.SetRegistryValue(rootKey, regKeyPath, regKeyName, value);
    }

    public bool DeleteRegistryKeyFromCurrentUserRoot(string regKeyPath) => this.DeleteRegistrySubKey(Registry.CurrentUser, regKeyPath);

    public bool DeleteRegistryKeyFromLocalMachineRoot(string regKeyPath, bool use64Bit = false)
    {
      using (RegistryKey rootKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, use64Bit ? RegistryView.Registry64 : RegistryView.Registry32))
        return this.DeleteRegistrySubKey(rootKey, regKeyPath);
    }

    public bool DeleteRegistryValueFromCurrentUserRoot(string regKeyPath, string regKeyName) => this.DeleteRegistryValue(Registry.CurrentUser, regKeyPath, regKeyName);

    public bool DeleteRegistryValueFromLocalMachineRoot(
      string regKeyPath,
      string regKeyName,
      bool use64Bit = false)
    {
      using (RegistryKey rootKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, use64Bit ? RegistryView.Registry64 : RegistryView.Registry32))
        return this.DeleteRegistryValue(rootKey, regKeyPath, regKeyName);
    }

    private object GetRegistryValue(
      RegistryKey rootKey,
      string regKeyPath,
      string regKeyName,
      object defaultOnError = null)
    {
      try
      {
        using (RegistryKey registryKey = rootKey.OpenSubKey(regKeyPath, RegistryKeyPermissionCheck.ReadSubTree, RegistryRights.QueryValues))
        {
          if (registryKey != null)
          {
            if (((IEnumerable<string>) registryKey.GetValueNames()).Contains<string>(regKeyName, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase))
            {
              object registryValue = registryKey.GetValue(regKeyName);
              if (registryValue != null)
                return registryValue;
            }
          }
        }
      }
      catch
      {
      }
      return defaultOnError;
    }

    private bool TryGetRegistryValueKind(
      RegistryKey rootKey,
      string regKeyPath,
      string regKeyName,
      out RegistryValueKind kind)
    {
      kind = RegistryValueKind.Unknown;
      try
      {
        using (RegistryKey registryKey = rootKey.OpenSubKey(regKeyPath, RegistryKeyPermissionCheck.ReadSubTree, RegistryRights.QueryValues))
        {
          if (registryKey != null)
          {
            if (((IEnumerable<string>) registryKey.GetValueNames()).Contains<string>(regKeyName, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase))
            {
              kind = registryKey.GetValueKind(regKeyName);
              return true;
            }
          }
        }
      }
      catch
      {
      }
      return false;
    }

    private string[] GetRegistryValueNames(RegistryKey rootKey, string regKeyPath)
    {
      try
      {
        using (RegistryKey registryKey = rootKey.OpenSubKey(regKeyPath, RegistryKeyPermissionCheck.ReadSubTree, RegistryRights.QueryValues))
        {
          if (registryKey != null)
            return registryKey.GetValueNames();
        }
      }
      catch
      {
      }
      return new string[0];
    }

    private string[] GetRegistrySubKeyNames(RegistryKey rootKey, string regKeyPath)
    {
      try
      {
        using (RegistryKey registryKey = rootKey.OpenSubKey(regKeyPath, RegistryKeyPermissionCheck.ReadSubTree, RegistryRights.ExecuteKey))
        {
          if (registryKey != null)
            return registryKey.GetSubKeyNames();
        }
      }
      catch
      {
      }
      return new string[0];
    }

    private bool DoesRegistryKeyExist(RegistryKey rootKey, string regKeyPath)
    {
      try
      {
        using (RegistryKey registryKey = rootKey.OpenSubKey(regKeyPath, RegistryKeyPermissionCheck.ReadSubTree, RegistryRights.QueryValues))
        {
          if (registryKey != null)
            return true;
        }
      }
      catch
      {
      }
      return false;
    }

    private bool SetRegistryValue(
      RegistryKey rootKey,
      string regKeyPath,
      string regKeyName,
      object value,
      RegistryValueKind valueKind = RegistryValueKind.Unknown)
    {
      try
      {
        using (RegistryKey subKey = rootKey.CreateSubKey(regKeyPath, RegistryKeyPermissionCheck.ReadWriteSubTree))
        {
          if (subKey != null)
          {
            if (valueKind == RegistryValueKind.Unknown)
            {
              RegistryValueKind registryValueKind;
              if (RegistryHelpers.TryGetRegistryValueKindForSet(value, out registryValueKind))
                subKey.SetValue(regKeyName, value, registryValueKind);
              else
                subKey.SetValue(regKeyName, value);
            }
            else
              subKey.SetValue(regKeyName, value, valueKind);
            return true;
          }
        }
      }
      catch
      {
      }
      return false;
    }

    private bool DeleteRegistrySubKey(RegistryKey rootKey, string regKeyPath)
    {
      try
      {
        rootKey.DeleteSubKeyTree(regKeyPath);
        return true;
      }
      catch
      {
      }
      return false;
    }

    private bool DeleteRegistryValue(RegistryKey rootKey, string regKeyPath, string regKeyName)
    {
      try
      {
        using (RegistryKey registryKey = rootKey.OpenSubKey(regKeyPath, RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.SetValue))
        {
          if (registryKey != null)
          {
            registryKey.DeleteValue(regKeyName);
            return true;
          }
        }
      }
      catch
      {
      }
      return false;
    }
  }
}
