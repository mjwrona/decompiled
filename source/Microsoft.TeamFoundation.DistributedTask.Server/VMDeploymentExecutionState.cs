// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.VMDeploymentExecutionState
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  [DataContract]
  public class VMDeploymentExecutionState
  {
    [DataMember(Name = "VirtualMachines")]
    private readonly List<VMResourceState> vmResources;

    public VMDeploymentExecutionState() => this.vmResources = new List<VMResourceState>();

    [DataMember]
    public TaskAgentPoolReference DeploymentPool { get; set; }

    public List<VMResourceState> VMResources => this.vmResources;
  }
}
