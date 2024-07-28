// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.TaskAgentCloud
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.WebApi
{
  [DataContract]
  public class TaskAgentCloud
  {
    private TaskAgentCloud(TaskAgentCloud cloudToBeCloned)
    {
      this.Id = cloudToBeCloned.Id;
      this.AgentCloudId = cloudToBeCloned.AgentCloudId;
      this.Name = cloudToBeCloned.Name;
      this.Type = cloudToBeCloned.Type;
      this.AcquireAgentEndpoint = cloudToBeCloned.AcquireAgentEndpoint;
      this.ReleaseAgentEndpoint = cloudToBeCloned.ReleaseAgentEndpoint;
      this.SharedSecret = cloudToBeCloned.SharedSecret;
      this.Internal = cloudToBeCloned.Internal;
      if (cloudToBeCloned.GetAgentDefinitionEndpoint != null)
        this.GetAgentDefinitionEndpoint = cloudToBeCloned.GetAgentDefinitionEndpoint;
      if (cloudToBeCloned.GetAgentRequestStatusEndpoint != null)
        this.GetAgentRequestStatusEndpoint = cloudToBeCloned.GetAgentRequestStatusEndpoint;
      if (cloudToBeCloned.AcquisitionTimeout.HasValue)
        this.AcquisitionTimeout = cloudToBeCloned.AcquisitionTimeout;
      if (cloudToBeCloned.GetAccountParallelismEndpoint != null)
        this.GetAccountParallelismEndpoint = cloudToBeCloned.GetAccountParallelismEndpoint;
      if (!cloudToBeCloned.MaxParallelism.HasValue)
        return;
      this.MaxParallelism = cloudToBeCloned.MaxParallelism;
    }

    public TaskAgentCloud()
    {
    }

    [DataMember]
    public Guid Id { get; set; }

    [DataMember]
    public int AgentCloudId { get; set; }

    [DataMember]
    public string Name { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Type { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool? Internal { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string SharedSecret { get; set; }

    [DataMember]
    public string AcquireAgentEndpoint { get; set; }

    [DataMember]
    public string ReleaseAgentEndpoint { get; set; }

    [DataMember(EmitDefaultValue = false, IsRequired = false)]
    public string GetAgentDefinitionEndpoint { get; set; }

    [DataMember(EmitDefaultValue = false, IsRequired = false)]
    public string GetAgentRequestStatusEndpoint { get; set; }

    [DataMember(EmitDefaultValue = false, IsRequired = false)]
    public int? AcquisitionTimeout { get; set; }

    [DataMember(EmitDefaultValue = false, IsRequired = false)]
    public string GetAccountParallelismEndpoint { get; set; }

    [DataMember(EmitDefaultValue = false, IsRequired = false)]
    public int? MaxParallelism { get; set; }

    public TaskAgentCloud Clone() => new TaskAgentCloud(this);
  }
}
