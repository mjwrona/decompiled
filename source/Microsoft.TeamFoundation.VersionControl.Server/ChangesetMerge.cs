// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.ChangesetMerge
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  public class ChangesetMerge
  {
    private int m_sourceVersion;
    private int m_targetVersion;
    private bool m_partial;

    public ChangesetMerge()
    {
    }

    internal ChangesetMerge(ItemMerge itemMerge)
    {
      this.SourceVersion = itemMerge.SourceVersionFrom;
      this.TargetVersion = itemMerge.TargetVersionFrom;
    }

    [XmlAttribute("srcver")]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public int SourceVersion
    {
      get => this.m_sourceVersion;
      set => this.m_sourceVersion = value;
    }

    [XmlAttribute("tgtver")]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public int TargetVersion
    {
      get => this.m_targetVersion;
      set => this.m_targetVersion = value;
    }

    [XmlAttribute("part")]
    [DefaultValue(false)]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public bool Partial
    {
      get => this.m_partial;
      set => this.m_partial = value;
    }
  }
}
