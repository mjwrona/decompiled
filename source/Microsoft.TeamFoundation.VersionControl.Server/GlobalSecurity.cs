// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.GlobalSecurity
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  public class GlobalSecurity
  {
    private List<AccessEntry> m_entries;
    private bool m_writable = true;

    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public List<AccessEntry> Entries
    {
      get => this.m_entries;
      set => this.m_entries = value;
    }

    [DefaultValue(true)]
    [XmlAttribute("mod")]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public bool Writable
    {
      get => this.m_writable;
      set => this.m_writable = value;
    }
  }
}
