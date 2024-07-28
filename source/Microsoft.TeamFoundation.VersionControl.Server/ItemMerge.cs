// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.ItemMerge
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.VersionControl.Common;
using System;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  public class ItemMerge : IComparable<ItemMerge>, ICacheable
  {
    private int m_sourceItemId;
    private int m_sourceVersionFrom;
    private ItemPathPair m_sourceItemPathPair;
    private int m_targetItemId;
    private int m_targetVersionFrom;
    private ItemPathPair m_targetItemPathPair;

    [XmlAttribute("sid")]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public int SourceItemId
    {
      get => this.m_sourceItemId;
      set => this.m_sourceItemId = value;
    }

    [XmlAttribute("svf")]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public int SourceVersionFrom
    {
      get => this.m_sourceVersionFrom;
      set => this.m_sourceVersionFrom = value;
    }

    [XmlAttribute("ssi")]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public string SourceServerItem
    {
      get => this.SourceItemPathPair.ProjectNamePath;
      set => this.SourceItemPathPair = ItemPathPair.FromServerItem(value);
    }

    [XmlIgnore]
    internal ItemPathPair SourceItemPathPair
    {
      get => this.m_sourceItemPathPair;
      set => this.m_sourceItemPathPair = value;
    }

    [XmlAttribute("tid")]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public int TargetItemId
    {
      get => this.m_targetItemId;
      set => this.m_targetItemId = value;
    }

    [XmlAttribute("tvf")]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public int TargetVersionFrom
    {
      get => this.m_targetVersionFrom;
      set => this.m_targetVersionFrom = value;
    }

    [XmlAttribute("tsi")]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public string TargetServerItem
    {
      get => this.TargetItemPathPair.ProjectNamePath;
      set => this.TargetItemPathPair = ItemPathPair.FromServerItem(value);
    }

    [XmlIgnore]
    internal ItemPathPair TargetItemPathPair
    {
      get => this.m_targetItemPathPair;
      set => this.m_targetItemPathPair = value;
    }

    internal bool HasPermission(
      VersionControlRequestContext versionControlRequestContext)
    {
      SecurityManager securityWrapper = versionControlRequestContext.VersionControlService.SecurityWrapper;
      if (!securityWrapper.HasItemPermission(versionControlRequestContext, VersionedItemPermissions.Read, this.SourceItemPathPair))
        return false;
      return this.TargetServerItem == null || securityWrapper.HasItemPermission(versionControlRequestContext, VersionedItemPermissions.Read, this.TargetItemPathPair);
    }

    public int CompareTo(ItemMerge other)
    {
      int num = this.SourceVersionFrom - other.SourceVersionFrom;
      if (num == 0)
        num = this.TargetVersionFrom - other.TargetVersionFrom;
      if (num == 0)
        num = this.SourceItemId - other.SourceItemId;
      if (num == 0)
        num = this.TargetItemId - other.TargetItemId;
      return num;
    }

    public int GetCachedSize() => 800;
  }
}
