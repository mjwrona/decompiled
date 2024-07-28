// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.RegistryEntry
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using System;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [ClassVisibility(ClientVisibility.Public, ClientVisibility.Private)]
  public sealed class RegistryEntry
  {
    private string m_path;
    private string m_value;
    public static readonly RegistryEntry[] EmptyArray = Array.Empty<RegistryEntry>();

    public RegistryEntry()
    {
    }

    public RegistryEntry(string registryPath, string registryValue)
    {
      this.m_path = registryPath;
      this.m_value = registryValue;
    }

    [XmlIgnore]
    public string Name => RegistryUtility.GetKeyName(this.m_path);

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public string Path
    {
      get => this.m_path;
      set => this.m_path = value;
    }

    [ClientProperty(ClientVisibility.Private, ClientVisibility.Private)]
    public string Value
    {
      get => this.m_value;
      set => this.m_value = value;
    }

    public string GetValue(string defaultValue) => this.m_value == null ? defaultValue : this.m_value;

    public T GetValue<T>() => RegistryUtility.FromString<T>(this.m_value);

    public T GetValue<T>(T defaultValue) => RegistryUtility.FromString<T>(this.m_value, defaultValue);

    public void SetValue<T>(T value) => this.m_value = RegistryUtility.ToString<T>(value);

    public static RegistryEntry Create<T>(string registryPath, T registryValue)
    {
      RegistryEntry registryEntry = new RegistryEntry();
      registryEntry.Path = registryPath;
      registryEntry.SetValue<T>(registryValue);
      return registryEntry;
    }
  }
}
