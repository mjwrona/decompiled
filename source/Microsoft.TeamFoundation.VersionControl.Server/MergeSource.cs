// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.MergeSource
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  public class MergeSource : ICacheable
  {
    [XmlAttribute("s")]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public string ServerItem
    {
      get => this.ItemPathPair.ProjectNamePath;
      set => this.ItemPathPair = ItemPathPair.FromServerItem(value);
    }

    [XmlIgnore]
    internal ItemPathPair ItemPathPair { get; set; }

    [XmlAttribute("vf")]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public int VersionFrom { get; set; }

    [XmlAttribute("vt")]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public int VersionTo { get; set; }

    [DefaultValue(false)]
    [XmlAttribute("r")]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public bool IsRename { get; set; }

    internal int SequenceId { get; set; }

    public int GetCachedSize() => 300;
  }
}
