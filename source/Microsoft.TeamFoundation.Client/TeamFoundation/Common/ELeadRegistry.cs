// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Common.ELeadRegistry
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.Win32;

namespace Microsoft.TeamFoundation.Common
{
  internal sealed class ELeadRegistry
  {
    private ELeadRegistry()
    {
    }

    public static RegistryKey OpenSubKey(string keyName, bool writable)
    {
      RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(keyName, writable);
      if (registryKey == null)
      {
        if (writable)
        {
          ELeadRegistry.CopySubKeyFromHKLM(keyName);
          registryKey = Registry.CurrentUser.OpenSubKey(keyName, writable);
        }
        else
          registryKey = Registry.LocalMachine.OpenSubKey(keyName, writable);
      }
      return registryKey;
    }

    public static RegistryKey OpenSubKeyOrCreate(string keyName)
    {
      RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(keyName, true);
      if (registryKey != null)
        return registryKey;
      ELeadRegistry.CopySubKeyFromHKLM(keyName);
      return Registry.CurrentUser.CreateSubKey(keyName);
    }

    public static object GetValueFromRegistry(string keyName, string valueName)
    {
      RegistryKey registryKey = (RegistryKey) null;
      try
      {
        registryKey = Registry.CurrentUser.OpenSubKey(keyName, false);
        if (registryKey == null || registryKey.GetValue(valueName) == null)
        {
          registryKey?.Close();
          registryKey = Registry.LocalMachine.OpenSubKey(keyName, false);
        }
        return registryKey == null ? (object) null : registryKey.GetValue(valueName);
      }
      finally
      {
        registryKey?.Close();
      }
    }

    public static RegistryKey OpenSubKey(string keyName) => ELeadRegistry.OpenSubKey(keyName, false);

    private static void CopySubKeyFromHKLM(string keyName)
    {
      RegistryKey registryKey1 = (RegistryKey) null;
      RegistryKey registryKey2 = (RegistryKey) null;
      try
      {
        registryKey1 = Registry.LocalMachine.OpenSubKey(keyName);
        if (registryKey1 == null)
          return;
        registryKey2 = Registry.CurrentUser.CreateSubKey(keyName);
        foreach (string valueName in registryKey1.GetValueNames())
        {
          object obj = registryKey1.GetValue(valueName);
          registryKey2.SetValue(valueName, obj);
        }
        foreach (string subKeyName in registryKey1.GetSubKeyNames())
          ELeadRegistry.CopySubKeyFromHKLM(keyName + "\\" + subKeyName);
      }
      finally
      {
        registryKey1?.Close();
        registryKey2?.Close();
      }
    }
  }
}
