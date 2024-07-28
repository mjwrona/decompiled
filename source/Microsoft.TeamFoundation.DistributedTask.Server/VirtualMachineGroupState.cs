// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.VirtualMachineGroupState
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  [DataContract]
  public class VirtualMachineGroupState
  {
    [DataMember(Name = "VirtualMachines")]
    private readonly IDictionary<int, VirtualMachineState> virtualMachines;
    [DataMember(Name = "Demands")]
    private readonly IList<Demand> demands;

    public VirtualMachineGroupState()
    {
      this.virtualMachines = (IDictionary<int, VirtualMachineState>) new Dictionary<int, VirtualMachineState>();
      this.demands = (IList<Demand>) new List<Demand>();
    }

    [DataMember]
    public int EnvironmentId { get; set; }

    [DataMember]
    public VirtualMachineGroup VirtualMachineGroup { get; set; }

    [DataMember]
    public TaskAgentPoolReference DeploymentPool { get; set; }

    [DataMember]
    public string PhaseIdentifier { get; set; }

    public IDictionary<int, VirtualMachineState> VirtualMachines => this.virtualMachines;

    public IList<Demand> Demands => this.demands;
  }
}
