// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.PendingChangeLight
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class PendingChangeLight
  {
    private ChangeType m_changeType;
    private ItemPathPair m_itemPathPair;
    private ItemPathPair m_sourceItemPathPair;
    private ItemType m_itemType;
    private bool m_hasContentChange;

    public ChangeType ChangeType
    {
      get => this.m_changeType;
      set => this.m_changeType = value;
    }

    public string ServerItem => this.ItemPathPair.ProjectNamePath;

    internal ItemPathPair ItemPathPair
    {
      get => this.m_itemPathPair;
      set => this.m_itemPathPair = value;
    }

    public string SourceServerItem => this.SourceItemPathPair.ProjectNamePath;

    internal ItemPathPair SourceItemPathPair
    {
      get => this.m_sourceItemPathPair;
      set => this.m_sourceItemPathPair = value;
    }

    public ItemType ItemType
    {
      get => this.m_itemType;
      set => this.m_itemType = value;
    }

    public bool HasContentChange
    {
      get => this.m_hasContentChange;
      set => this.m_hasContentChange = value;
    }
  }
}
