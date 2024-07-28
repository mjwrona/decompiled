// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.DeploymentMachineChangedData
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.WebApi
{
  [DataContract]
  public class DeploymentMachineChangedData : DeploymentMachine, ICloneable
  {
    [DataMember(IsRequired = false, EmitDefaultValue = false, Name = "AddedTags")]
    private IList<string> m_addedTags;
    [DataMember(IsRequired = false, EmitDefaultValue = false, Name = "DeletedTags")]
    private IList<string> m_deletedTags;

    public DeploymentMachineChangedData()
    {
    }

    private DeploymentMachineChangedData(DeploymentMachineChangedData machineToBeCloned)
    {
      this.Id = machineToBeCloned.Id;
      this.Tags = machineToBeCloned.Tags == null ? (IList<string>) null : (IList<string>) new List<string>((IEnumerable<string>) machineToBeCloned.Tags);
      this.Agent = machineToBeCloned.Agent?.Clone();
      this.TagsAdded = machineToBeCloned.TagsAdded == null ? (IList<string>) null : (IList<string>) new List<string>((IEnumerable<string>) machineToBeCloned.TagsAdded);
      this.TagsDeleted = machineToBeCloned.TagsDeleted == null ? (IList<string>) null : (IList<string>) new List<string>((IEnumerable<string>) machineToBeCloned.TagsDeleted);
    }

    public DeploymentMachineChangedData(DeploymentMachine deploymentMachine)
    {
      this.Id = deploymentMachine.Id;
      this.Tags = deploymentMachine.Tags;
      this.Agent = deploymentMachine.Agent;
    }

    public IList<string> TagsAdded
    {
      get => this.m_addedTags;
      set => this.m_addedTags = value;
    }

    public IList<string> TagsDeleted
    {
      get => this.m_deletedTags;
      set => this.m_deletedTags = value;
    }

    object ICloneable.Clone() => (object) this.Clone();

    public DeploymentMachineChangedData Clone() => new DeploymentMachineChangedData(this);
  }
}
