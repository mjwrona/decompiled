// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.LocalVersion
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  public class LocalVersion : ICacheable
  {
    private string m_item;
    private int m_version;

    [XmlAttribute("i")]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private, PropertyName = "Item", Direction = ClientPropertySerialization.Bidirectional)]
    public string Item
    {
      get => this.m_item;
      set => this.m_item = value;
    }

    [XmlAttribute("v")]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private, PropertyName = "Version", Direction = ClientPropertySerialization.Bidirectional)]
    public int Version
    {
      get => this.m_version;
      set => this.m_version = value;
    }

    public int GetCachedSize() => 400;
  }
}
