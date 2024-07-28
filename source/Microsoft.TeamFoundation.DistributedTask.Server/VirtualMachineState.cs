// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.VirtualMachineState
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  [DataContract]
  public class VirtualMachineState
  {
    [DataMember]
    public int Id { get; set; }

    [DataMember]
    public string Name { get; set; }

    [DataMember]
    public bool DemandsMatched { get; set; }

    [DataMember]
    public TaskAgentStatus AgentStatus { get; set; }

    [DataMember]
    public bool DeploymentAttempted { get; set; }

    [DataMember]
    public bool DeploymentInQueue { get; set; }

    [DataMember]
    public Guid? JobId { get; set; }

    [DataMember]
    public TaskResult? DeploymentResult { get; set; }
  }
}
