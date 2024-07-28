// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Configuration.RegistryHandler
// Assembly: Microsoft.VisualStudio.Services.Configuration, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AB461A1-8255-4EAB-B12B-E1D379571DC1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Configuration.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.Win32;
using System;

namespace Microsoft.VisualStudio.Services.Configuration
{
  public static class RegistryHandler
  {
    public static readonly RegistryKey LocalMachineView = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);

    public static T ReadRegistryValue<T>(RegistryKey hiveRoot, string key, string valueName) => TFCommonUtil.ReadRegistryValue<T>(hiveRoot, key, valueName);

    public static string[] ReadRegistryValueNames(RegistryKey hiveRoot, string key)
    {
      string[] strArray = Array.Empty<string>();
      using (RegistryKey registryKey = hiveRoot.OpenSubKey(key))
      {
        if (registryKey != null)
          strArray = registryKey.GetValueNames();
      }
      return strArray;
    }

    public static string[] ReadRegistrySubKeyNames(RegistryKey hiveRoot, string key)
    {
      string[] strArray = Array.Empty<string>();
      using (RegistryKey registryKey = hiveRoot.OpenSubKey(key))
      {
        if (registryKey != null)
          strArray = registryKey.GetSubKeyNames();
      }
      return strArray;
    }

    public static void WriteRegistryValue(
      RegistryKey hiveRoot,
      string key,
      string valueName,
      object data)
    {
      using (RegistryKey registryKey = hiveRoot.OpenSubKey(key, true))
        registryKey?.SetValue(valueName, data);
    }

    public static void RemoveRegistryValue(RegistryKey hiveRoot, string key, string valueName)
    {
      using (RegistryKey registryKey = hiveRoot.OpenSubKey(key, true))
        registryKey?.DeleteValue(valueName, false);
    }
  }
}
