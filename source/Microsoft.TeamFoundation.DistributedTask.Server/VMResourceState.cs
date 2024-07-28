// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.VMResourceState
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  [DataContract]
  public class VMResourceState
  {
    [DataMember]
    public int ResourceId { get; set; }

    [DataMember]
    public string ResourceName { get; set; }

    [DataMember]
    public int AgentId { get; set; }

    [DataMember]
    public string AgentName { get; set; }

    [DataMember]
    public TaskAgentStatus AgentStatus { get; set; }

    [DataMember]
    public bool DeploymentAttempted { get; set; }

    [DataMember]
    public string LifeCycleInstanceName { get; set; }

    [DataMember]
    public TaskResult? DeploymentResult { get; set; }
  }
}
