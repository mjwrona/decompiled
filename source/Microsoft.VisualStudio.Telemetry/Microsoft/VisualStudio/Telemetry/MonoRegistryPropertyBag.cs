// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.MonoRegistryPropertyBag
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.Utilities.Internal;
using Microsoft.Win32;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Telemetry
{
  internal class MonoRegistryPropertyBag : RegistryPropertyBag
  {
    public MonoRegistryPropertyBag(string processName)
      : base(processName)
    {
    }

    protected override IEnumerable<KeyValuePair<string, object>> ReadRegistryValues(
      string registryKeyName)
    {
      List<KeyValuePair<string, object>> keyValuePairList = new List<KeyValuePair<string, object>>();
      FileBasedRegistry fileBasedRegistry = new FileBasedRegistry(registryKeyName);
      foreach (string valueName in fileBasedRegistry.GetValueNames())
      {
        object obj = fileBasedRegistry.GetValue(valueName);
        if (obj != null)
          keyValuePairList.Add(new KeyValuePair<string, object>(valueName, obj));
      }
      return (IEnumerable<KeyValuePair<string, object>>) keyValuePairList;
    }

    protected override object GetProperty(
      string fullRegistryKeyName,
      string registryKeyName,
      string propertyName,
      object defaultValue)
    {
      using (FileBasedRegistry fileBasedRegistry = new FileBasedRegistry(registryKeyName))
        return fileBasedRegistry.GetValue(propertyName) ?? defaultValue;
    }

    protected override void SetProperty(string registryKeyName, string propertyName, object value)
    {
      using (FileBasedRegistry fileBasedRegistry = new FileBasedRegistry(registryKeyName))
        fileBasedRegistry.SetValue(propertyName, value);
    }

    protected override void RemoveProperty(string registryKeyName, string propertyName)
    {
      using (FileBasedRegistry fileBasedRegistry = new FileBasedRegistry(registryKeyName))
        fileBasedRegistry.RemoveValue(propertyName);
    }

    protected override void Clear(string registryKeyName)
    {
      using (FileBasedRegistry fileBasedRegistry = new FileBasedRegistry(registryKeyName))
        fileBasedRegistry.Clear();
    }

    protected override void SetAccessControl(RegistryKey key)
    {
    }
  }
}
