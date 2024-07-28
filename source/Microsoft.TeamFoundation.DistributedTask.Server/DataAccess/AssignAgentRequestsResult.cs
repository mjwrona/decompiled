// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.AssignAgentRequestsResult
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Tasks;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.DistributedTask.Server.DataAccess
{
  internal struct AssignAgentRequestsResult
  {
    private List<ResourceUsageData> m_resourceUsageDataCollection;
    private List<AssignedAgentRequestResult> m_requests;
    private List<RunAgentEvent> m_events;

    public List<AssignedAgentRequestResult> AssignedRequestResults
    {
      get
      {
        if (this.m_requests == null)
          this.m_requests = new List<AssignedAgentRequestResult>();
        return this.m_requests;
      }
    }

    public List<ResourceUsageData> ResourceUsageDataCollection
    {
      get
      {
        if (this.m_resourceUsageDataCollection == null)
          this.m_resourceUsageDataCollection = new List<ResourceUsageData>();
        return this.m_resourceUsageDataCollection;
      }
    }

    public List<RunAgentEvent> Events
    {
      get
      {
        if (this.m_events == null)
          this.m_events = new List<RunAgentEvent>();
        return this.m_events;
      }
    }
  }
}
