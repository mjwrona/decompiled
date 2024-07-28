// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.ChangesetMergeDetails
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  public class ChangesetMergeDetails
  {
    private StreamingCollection<ItemMerge> m_mergedItems;
    private StreamingCollection<ItemMerge> m_unmergedItems;
    private StreamingCollection<Changeset> m_changesets;

    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public StreamingCollection<ItemMerge> MergedItems
    {
      get => this.m_mergedItems;
      set => this.m_mergedItems = value;
    }

    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public StreamingCollection<ItemMerge> UnmergedItems
    {
      get => this.m_unmergedItems;
      set => this.m_unmergedItems = value;
    }

    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public StreamingCollection<Changeset> Changesets
    {
      get => this.m_changesets;
      set => this.m_changesets = value;
    }
  }
}
