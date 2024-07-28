// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.ExtendedMerge
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  public class ExtendedMerge : ICacheable
  {
    private ChangesetSummary m_sourceFrom;
    private ChangesetSummary m_target;
    private Change m_sourceItem;
    private ItemIdentifier m_targetItem;
    private int m_itemCount;

    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public ChangesetSummary SourceChangeset
    {
      get => this.m_sourceFrom;
      set => this.m_sourceFrom = value;
    }

    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public ChangesetSummary TargetChangeset
    {
      get => this.m_target;
      set => this.m_target = value;
    }

    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public int VersionedItemCount
    {
      get => this.m_itemCount;
      set => this.m_itemCount = value;
    }

    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public Change SourceItem
    {
      get => this.m_sourceItem;
      set => this.m_sourceItem = value;
    }

    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public ItemIdentifier TargetItem
    {
      get => this.m_targetItem;
      set => this.m_targetItem = value;
    }

    public int GetCachedSize()
    {
      int cachedSize = 956;
      if (this.SourceChangeset != null && this.SourceChangeset.Comment != null)
        cachedSize += this.SourceChangeset.Comment.Length;
      if (this.TargetChangeset.Comment != null)
        cachedSize += this.TargetChangeset.Comment.Length;
      return cachedSize;
    }
  }
}
