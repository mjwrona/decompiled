// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.TaskAgentQueryResult
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  public sealed class TaskAgentQueryResult
  {
    private List<Demand> m_demands;
    private List<TaskAgent> m_matchedAgents;
    private List<Tuple<TaskAgent, IList<Demand>>> m_unmatchedAgents;

    public TaskAgentQueryResult(JObject agentSpecification)
      : this((IEnumerable<Demand>) null, agentSpecification, (IEnumerable<TaskAgent>) null, new bool?())
    {
    }

    public TaskAgentQueryResult(IEnumerable<Demand> demands, bool? returnedAllAgents)
      : this(demands, (JObject) null, (IEnumerable<TaskAgent>) null, returnedAllAgents)
    {
    }

    public TaskAgentQueryResult(
      IEnumerable<Demand> demands,
      JObject agentSpecification,
      IEnumerable<TaskAgent> matchedAgents,
      bool? returnedAllAgents)
    {
      this.m_demands = new List<Demand>(demands ?? Enumerable.Empty<Demand>());
      this.AgentSpecification = agentSpecification;
      this.m_matchedAgents = new List<TaskAgent>(matchedAgents ?? Enumerable.Empty<TaskAgent>());
      this.ReturnedAllAgents = returnedAllAgents;
    }

    public JObject AgentSpecification { get; set; }

    public IList<Demand> Demands
    {
      get
      {
        if (this.m_demands == null)
          this.m_demands = new List<Demand>();
        return (IList<Demand>) this.m_demands;
      }
    }

    public IList<TaskAgent> MatchedAgents
    {
      get
      {
        if (this.m_matchedAgents == null)
          this.m_matchedAgents = new List<TaskAgent>();
        return (IList<TaskAgent>) this.m_matchedAgents;
      }
    }

    public IList<Tuple<TaskAgent, IList<Demand>>> UnmatchedAgents
    {
      get
      {
        if (this.m_unmatchedAgents == null)
          this.m_unmatchedAgents = new List<Tuple<TaskAgent, IList<Demand>>>();
        return (IList<Tuple<TaskAgent, IList<Demand>>>) this.m_unmatchedAgents;
      }
    }

    public bool? ReturnedAllAgents { get; set; }

    public bool MatchesAllAgents(IVssRequestContext requestContext)
    {
      if (requestContext.IsFeatureEnabled("DistributedTask.MarkRequestsAllMatch"))
      {
        bool? returnedAllAgents = this.ReturnedAllAgents;
        bool flag = true;
        if (returnedAllAgents.GetValueOrDefault() == flag & returnedAllAgents.HasValue && this.MatchedAgents.Count != 0)
          return this.UnmatchedAgents.Count == 0;
      }
      return false;
    }
  }
}
