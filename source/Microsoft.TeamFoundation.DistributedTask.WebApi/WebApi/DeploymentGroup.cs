// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.DeploymentGroup
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.WebApi
{
  [DataContract]
  public class DeploymentGroup : DeploymentGroupReference
  {
    [DataMember(IsRequired = false, EmitDefaultValue = false, Name = "Machines")]
    private IList<DeploymentMachine> m_machines;
    [DataMember(IsRequired = false, EmitDefaultValue = false, Name = "MachineTags")]
    private IList<string> m_tags;

    [DataMember]
    public int MachineCount { get; internal set; }

    public IList<DeploymentMachine> Machines
    {
      get
      {
        if (this.m_machines == null)
          this.m_machines = (IList<DeploymentMachine>) new List<DeploymentMachine>();
        return this.m_machines;
      }
      internal set => this.m_machines = value;
    }

    [DataMember(EmitDefaultValue = false)]
    public string Description { get; set; }

    public IList<string> MachineTags
    {
      get
      {
        if (this.m_tags == null)
          this.m_tags = (IList<string>) new List<string>();
        return this.m_tags;
      }
      internal set => this.m_tags = value;
    }
  }
}
