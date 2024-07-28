// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.TaskAgentCloudRequest
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Newtonsoft.Json.Linq;
using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.WebApi
{
  [DataContract]
  public class TaskAgentCloudRequest
  {
    private TaskAgentCloudRequest(TaskAgentCloudRequest requestToBeCloned)
    {
      this.AgentCloudId = requestToBeCloned.AgentCloudId;
      this.RequestId = requestToBeCloned.RequestId;
      this.AgentSpecification = requestToBeCloned.AgentSpecification;
      this.ProvisionRequestTime = requestToBeCloned.ProvisionRequestTime;
      this.ProvisionedTime = requestToBeCloned.ProvisionedTime;
      this.AgentConnectedTime = requestToBeCloned.AgentConnectedTime;
      this.ReleaseRequestTime = requestToBeCloned.ReleaseRequestTime;
      if (requestToBeCloned.AgentData != null)
        this.AgentData = new JObject(requestToBeCloned.AgentData);
      if (requestToBeCloned.Pool != null)
        this.Pool = requestToBeCloned.Pool.Clone();
      if (requestToBeCloned.Agent == null)
        return;
      this.Agent = requestToBeCloned.Agent.Clone();
    }

    public TaskAgentCloudRequest()
    {
    }

    [DataMember]
    public int AgentCloudId { get; set; }

    [DataMember]
    public Guid RequestId { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public TaskAgentPoolReference Pool { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public TaskAgentReference Agent { get; set; }

    [DataMember]
    public JObject AgentSpecification { get; set; }

    [DataMember]
    public JObject AgentData { get; set; }

    [DataMember]
    public DateTime? ProvisionRequestTime { get; set; }

    [DataMember]
    public DateTime? ProvisionedTime { get; set; }

    [DataMember]
    public DateTime? AgentConnectedTime { get; set; }

    [DataMember]
    public DateTime? ReleaseRequestTime { get; set; }

    public TaskAgentCloudRequest Clone() => new TaskAgentCloudRequest(this);
  }
}
