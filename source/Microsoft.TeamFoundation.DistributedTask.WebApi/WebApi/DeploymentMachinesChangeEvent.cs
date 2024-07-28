// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.DeploymentMachinesChangeEvent
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.WebApi
{
  [DataContract]
  [ServiceEventObject]
  public class DeploymentMachinesChangeEvent
  {
    [DataMember(Name = "Machines")]
    private IList<DeploymentMachineChangedData> m_machines;

    public DeploymentMachinesChangeEvent(
      DeploymentGroupReference machineGroupReference,
      IList<DeploymentMachineChangedData> machines)
    {
      ArgumentUtility.CheckForNull<DeploymentGroupReference>(machineGroupReference, nameof (machineGroupReference));
      this.MachineGroupReference = machineGroupReference;
      this.m_machines = machines;
    }

    public IList<DeploymentMachineChangedData> Machines
    {
      get
      {
        if (this.m_machines == null)
          this.m_machines = (IList<DeploymentMachineChangedData>) new List<DeploymentMachineChangedData>();
        return this.m_machines;
      }
    }

    [DataMember]
    public DeploymentGroupReference MachineGroupReference { get; private set; }
  }
}
