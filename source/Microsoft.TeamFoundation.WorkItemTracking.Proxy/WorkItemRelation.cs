// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Proxy.WorkItemRelation
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Proxy, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FF15D8B4-8AC0-4915-8153-9054E8546EA2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Proxy.dll

using Microsoft.TeamFoundation.WorkItemTracking.Internals;

namespace Microsoft.TeamFoundation.WorkItemTracking.Proxy
{
  public struct WorkItemRelation : IWorkItemRelation
  {
    private int m_sourceID;
    private int m_targetID;
    private int m_linkType;
    private bool m_locked;

    public WorkItemRelation(int sourceId, int targetId, int linkTypeId, bool isLocked)
    {
      this.m_sourceID = sourceId;
      this.m_targetID = targetId;
      this.m_linkType = linkTypeId;
      this.m_locked = isLocked;
    }

    public int SourceID
    {
      get => this.m_sourceID;
      set => this.m_sourceID = value;
    }

    public int TargetID
    {
      get => this.m_targetID;
      set => this.m_targetID = value;
    }

    public int LinkTypeID
    {
      get => this.m_linkType;
      set => this.m_linkType = value;
    }

    public bool IsLocked
    {
      get => this.m_locked;
      set => this.m_locked = value;
    }

    public static int MissingID => 0;
  }
}
