// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Utilities.Internal.FileBasedRegistryTools
// Assembly: Microsoft.VisualStudio.Utilities.Internal, Version=14.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E02F1555-E063-4795-BC05-853CA7424F51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Utilities.Internal.dll

using Microsoft.Win32;
using System;

namespace Microsoft.VisualStudio.Utilities.Internal
{
  public sealed class FileBasedRegistryTools : 
    IRegistryTools4,
    IRegistryTools3,
    IRegistryTools2,
    IRegistryTools
  {
    public bool DeleteRegistryKeyFromCurrentUserRoot(string regKeyPath)
    {
      try
      {
        using (FileBasedRegistry fileBasedRegistry = new FileBasedRegistry(regKeyPath))
          fileBasedRegistry.Drop();
        return true;
      }
      catch
      {
      }
      return false;
    }

    public bool DoesRegistryKeyExistInCurrentUserRoot(string regKeyPath)
    {
      try
      {
        using (FileBasedRegistry fileBasedRegistry = new FileBasedRegistry(regKeyPath))
          return fileBasedRegistry.Exists;
      }
      catch
      {
      }
      return false;
    }

    public bool DeleteRegistryValueFromCurrentUserRoot(string regKeyPath, string regKeyName)
    {
      try
      {
        using (FileBasedRegistry fileBasedRegistry = new FileBasedRegistry(regKeyPath))
          fileBasedRegistry.RemoveValue(regKeyName);
      }
      catch
      {
      }
      return false;
    }

    public object GetRegistryValueFromCurrentUserRoot(
      string regKeyPath,
      string regKeyName,
      object defaultOnError = null)
    {
      try
      {
        using (FileBasedRegistry fileBasedRegistry = new FileBasedRegistry(regKeyPath))
        {
          object fromCurrentUserRoot = fileBasedRegistry.GetValue(regKeyName);
          if (fromCurrentUserRoot != null)
            return fromCurrentUserRoot;
        }
      }
      catch
      {
      }
      return defaultOnError;
    }

    public string[] GetRegistryValueNamesFromCurrentUserRoot(string regKeyPath)
    {
      try
      {
        using (FileBasedRegistry fileBasedRegistry = new FileBasedRegistry(regKeyPath))
          return fileBasedRegistry.GetValueNames();
      }
      catch
      {
      }
      return new string[0];
    }

    public bool SetRegistryFromCurrentUserRoot(string regKeyPath, string regKeyName, object value)
    {
      try
      {
        using (FileBasedRegistry fileBasedRegistry = new FileBasedRegistry(regKeyPath))
        {
          if (value.GetType() == typeof (bool))
            fileBasedRegistry.SetValue(regKeyName, (object) Convert.ToInt32(value));
          else
            fileBasedRegistry.SetValue(regKeyName, value);
        }
        return true;
      }
      catch
      {
      }
      return false;
    }

    public bool SetRegistryFromCurrentUserRoot(
      string regKeyPath,
      string regKeyName,
      object value,
      RegistryValueKind valueKind)
    {
      return this.SetRegistryFromCurrentUserRoot(regKeyPath, regKeyName, value);
    }

    public object GetRegistryValueFromLocalMachineRoot(
      string regKeyPath,
      string regKeyName,
      object defaultOnError = null)
    {
      throw new NotImplementedException();
    }

    public object GetRegistryValueFromLocalMachineRoot(
      string regKeyPath,
      string regKeyName,
      bool use64Bit,
      object defaultOnError = null)
    {
      throw new NotImplementedException();
    }

    public bool SetRegistryFromLocalMachineRoot(
      string regKeyPath,
      string regKeyName,
      object value,
      bool use64Bit = false)
    {
      throw new NotImplementedException();
    }

    public bool TryGetRegistryValueKindFromCurrentUserRoot(
      string regKeyPath,
      string regKeyName,
      out RegistryValueKind kind)
    {
      throw new NotImplementedException();
    }

    public bool TryGetRegistryValueKindFromLocalMachineRoot(
      string regKeyPath,
      string regKeyName,
      out RegistryValueKind kind,
      bool use64Bit = false)
    {
      throw new NotImplementedException();
    }

    public bool DeleteRegistryKeyFromLocalMachineRoot(string regKeyPath, bool use64Bit = false) => throw new NotImplementedException();

    public bool DeleteRegistryValueFromLocalMachineRoot(
      string regKeyPath,
      string regKeyName,
      bool use64Bit = false)
    {
      throw new NotImplementedException();
    }

    public bool DoesRegistryKeyExistInLocalMachineRoot(string regKeyPath, bool use64Bit = false) => throw new NotImplementedException();

    public int? GetRegistryIntValueFromLocalMachineRoot(
      string regKeyPath,
      string regKeyName,
      int? defaultOnError = null)
    {
      throw new NotImplementedException();
    }

    public int? GetRegistryIntValueFromLocalMachineRoot(
      string regKeyPath,
      string regKeyName,
      bool use64Bit,
      int? defaultOnError = null)
    {
      throw new NotImplementedException();
    }

    public string[] GetRegistrySubKeyNamesFromLocalMachineRoot(string regKeyPath, bool use64Bit = false) => throw new NotImplementedException();

    public string[] GetRegistrySubKeyNamesFromCurrentUserRoot(string regKeyPath)
    {
      try
      {
        using (FileBasedRegistry fileBasedRegistry = new FileBasedRegistry(regKeyPath))
          return fileBasedRegistry.GetSubKeyNames(regKeyPath);
      }
      catch
      {
      }
      return new string[0];
    }

    public string[] GetRegistryValueNamesFromLocalMachineRoot(string regKeyPath, bool use64Bit = false) => throw new NotImplementedException();
  }
}
