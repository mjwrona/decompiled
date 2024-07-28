// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.ArtifactWorkItemIds
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  public class ArtifactWorkItemIds : ICacheable
  {
    private const int CACHE_OBJECT_BASE_SIZE = 220;
    private const int CACHE_OBJECT_WORKITEMID_SIZE = 20;
    private string m_uri;
    private int m_uriListOffset;
    private List<int> m_workItemIds;

    public ArtifactWorkItemIds() => this.m_workItemIds = new List<int>();

    public List<int> WorkItemIds
    {
      get => this.m_workItemIds;
      set => this.m_workItemIds = value;
    }

    public string Uri
    {
      get => this.m_uri;
      set => this.m_uri = value;
    }

    public int UriListOffset
    {
      get => this.m_uriListOffset;
      set => this.m_uriListOffset = value;
    }

    public int GetCachedSize() => 220 + 20 * this.m_workItemIds.Count;
  }
}
