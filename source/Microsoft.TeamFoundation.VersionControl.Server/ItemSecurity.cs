// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.ItemSecurity
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using System.ComponentModel;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  public class ItemSecurity
  {
    private AccessEntry[] m_entries;
    private bool m_inherit = true;
    private string m_serverItem;
    private bool m_writable = true;

    public AccessEntry[] Entries
    {
      get => this.m_entries;
      set => this.m_entries = value;
    }

    [DefaultValue(true)]
    [XmlAttribute("inherit")]
    public bool Inherit
    {
      get => this.m_inherit;
      set => this.m_inherit = value;
    }

    [XmlAttribute("item")]
    public string ServerItem
    {
      get => this.m_serverItem;
      set => this.m_serverItem = value;
    }

    [DefaultValue(true)]
    [XmlAttribute("mod")]
    public bool Writable
    {
      get => this.m_writable;
      set => this.m_writable = value;
    }
  }
}
