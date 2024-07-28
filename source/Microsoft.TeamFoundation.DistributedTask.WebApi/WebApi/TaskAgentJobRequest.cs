// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.TaskAgentJobRequest
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.WebApi
{
  [DataContract]
  public class TaskAgentJobRequest : ICloneable
  {
    private List<TaskAgentReference> m_matchedAgents;
    private IDictionary<string, string> m_requestAgentData;
    [DataMember(Name = "MatchedAgents", Order = 18, EmitDefaultValue = false)]
    private List<TaskAgentReference> m_serializedMatchedAgents;

    public TaskAgentJobRequest()
    {
    }

    private TaskAgentJobRequest(TaskAgentJobRequest requestToBeCloned)
    {
      this.RequestId = requestToBeCloned.RequestId;
      this.QueueTime = requestToBeCloned.QueueTime;
      this.AssignTime = requestToBeCloned.AssignTime;
      this.ReceiveTime = requestToBeCloned.ReceiveTime;
      this.FinishTime = requestToBeCloned.FinishTime;
      this.Result = requestToBeCloned.Result;
      this.LockedUntil = requestToBeCloned.LockedUntil;
      this.ServiceOwner = requestToBeCloned.ServiceOwner;
      this.HostId = requestToBeCloned.HostId;
      this.ScopeId = requestToBeCloned.ScopeId;
      this.PlanType = requestToBeCloned.PlanType;
      this.PlanGroup = requestToBeCloned.PlanGroup;
      this.PlanId = requestToBeCloned.PlanId;
      this.QueueId = requestToBeCloned.QueueId;
      this.PoolId = requestToBeCloned.PoolId;
      this.JobId = requestToBeCloned.JobId;
      this.JobName = requestToBeCloned.JobName;
      this.Demands = (IList<Demand>) new List<Demand>((IEnumerable<Demand>) ((object) requestToBeCloned.Demands ?? (object) Array.Empty<Demand>()));
      this.LockToken = requestToBeCloned.LockToken;
      this.OrchestrationId = requestToBeCloned.OrchestrationId;
      this.MatchesAllAgentsInPool = requestToBeCloned.MatchesAllAgentsInPool;
      this.Priority = requestToBeCloned.Priority;
      if (requestToBeCloned.m_matchedAgents != null && requestToBeCloned.m_matchedAgents.Count > 0)
        this.m_matchedAgents = requestToBeCloned.m_matchedAgents.Select<TaskAgentReference, TaskAgentReference>((Func<TaskAgentReference, TaskAgentReference>) (x => x.Clone())).ToList<TaskAgentReference>();
      if (requestToBeCloned.ReservedAgent != null)
        this.ReservedAgent = requestToBeCloned.ReservedAgent.Clone();
      IDictionary<string, string> requestAgentData = requestToBeCloned.m_requestAgentData;
      if ((requestAgentData != null ? (requestAgentData.Count > 0 ? 1 : 0) : 0) != 0)
      {
        foreach (KeyValuePair<string, string> keyValuePair in (IEnumerable<KeyValuePair<string, string>>) requestToBeCloned.m_requestAgentData)
          this.Data[keyValuePair.Key] = keyValuePair.Value;
      }
      if (requestToBeCloned.AgentSpecification == null)
        return;
      this.AgentSpecification = new JObject(requestToBeCloned.AgentSpecification);
    }

    [DataMember(Order = 2)]
    public long RequestId { get; internal set; }

    [DataMember(Order = 3, EmitDefaultValue = false)]
    public DateTime QueueTime { get; internal set; }

    [DataMember(Order = 4, EmitDefaultValue = false)]
    public DateTime? AssignTime { get; internal set; }

    [DataMember(Order = 5, EmitDefaultValue = false)]
    public DateTime? ReceiveTime { get; internal set; }

    [DataMember(Order = 6, EmitDefaultValue = false)]
    public DateTime? FinishTime { get; internal set; }

    [DataMember(Order = 8, EmitDefaultValue = false)]
    public TaskResult? Result { get; set; }

    [DataMember(Order = 9, EmitDefaultValue = false)]
    public DateTime? LockedUntil { get; internal set; }

    [DataMember(Order = 10, EmitDefaultValue = false)]
    public Guid ServiceOwner { get; set; }

    [DataMember(Order = 11, EmitDefaultValue = false)]
    public Guid HostId { get; set; }

    [DataMember(Order = 12, EmitDefaultValue = false)]
    public Guid ScopeId { get; set; }

    [DataMember(Order = 13, EmitDefaultValue = false)]
    public string PlanType { get; set; }

    [DataMember(Order = 14, EmitDefaultValue = false)]
    public Guid PlanId { get; set; }

    [DataMember(Order = 15, EmitDefaultValue = false)]
    public Guid JobId { get; set; }

    [DataMember(Order = 21, EmitDefaultValue = false)]
    public string JobName { get; set; }

    [DataMember(Order = 16, EmitDefaultValue = false)]
    public IList<Demand> Demands { get; set; }

    [DataMember(Order = 17, EmitDefaultValue = false)]
    public TaskAgentReference ReservedAgent { get; internal set; }

    public List<TaskAgentReference> MatchedAgents
    {
      get
      {
        if (this.m_matchedAgents == null)
          this.m_matchedAgents = new List<TaskAgentReference>();
        return this.m_matchedAgents;
      }
    }

    [DataMember(Order = 19, EmitDefaultValue = false)]
    public TaskOrchestrationOwner Definition { get; set; }

    [DataMember(Order = 20, EmitDefaultValue = false)]
    public TaskOrchestrationOwner Owner { get; set; }

    [DataMember(Order = 22, EmitDefaultValue = false)]
    public IDictionary<string, string> Data
    {
      get
      {
        if (this.m_requestAgentData == null)
          this.m_requestAgentData = (IDictionary<string, string>) new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        return this.m_requestAgentData;
      }
    }

    [DataMember(Order = 23, EmitDefaultValue = false)]
    public string PlanGroup { get; set; }

    [DataMember(Order = 24, EmitDefaultValue = false)]
    internal int PoolId { get; set; }

    [DataMember(Order = 25, EmitDefaultValue = false)]
    internal int? QueueId { get; set; }

    [DataMember(Order = 28, EmitDefaultValue = false)]
    public JObject AgentSpecification { get; set; }

    [DataMember(Order = 29, EmitDefaultValue = false)]
    public string OrchestrationId { get; set; }

    [DataMember(Order = 30, EmitDefaultValue = false)]
    public bool MatchesAllAgentsInPool { get; set; }

    [DataMember(Order = 31, EmitDefaultValue = false)]
    public string StatusMessage { get; set; }

    [DataMember(Order = 32, EmitDefaultValue = false)]
    public bool UserDelayed { get; set; }

    [IgnoreDataMember]
    internal Guid? LockToken { get; set; }

    [DataMember(Order = 33, EmitDefaultValue = false)]
    public int? Priority { get; set; }

    object ICloneable.Clone() => (object) this.Clone();

    public TaskAgentJobRequest Clone() => new TaskAgentJobRequest(this);

    [System.Runtime.Serialization.OnDeserialized]
    private void OnDeserialized(StreamingContext context) => SerializationHelper.Copy<TaskAgentReference>(ref this.m_serializedMatchedAgents, ref this.m_matchedAgents, true);

    [System.Runtime.Serialization.OnSerialized]
    private void OnSerialized(StreamingContext context) => this.m_serializedMatchedAgents = (List<TaskAgentReference>) null;

    [System.Runtime.Serialization.OnSerializing]
    private void OnSerializing(StreamingContext context) => SerializationHelper.Copy<TaskAgentReference>(ref this.m_matchedAgents, ref this.m_serializedMatchedAgents);
  }
}
