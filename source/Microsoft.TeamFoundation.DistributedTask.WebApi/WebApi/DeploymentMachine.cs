// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.DeploymentMachine
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.WebApi
{
  [DataContract]
  public class DeploymentMachine : ICloneable
  {
    [DataMember(IsRequired = false, EmitDefaultValue = false, Name = "Tags")]
    private IList<string> m_tags;
    [DataMember(EmitDefaultValue = false, Name = "Properties")]
    private PropertiesCollection m_properties;

    public DeploymentMachine()
    {
    }

    private DeploymentMachine(DeploymentMachine machineToBeCloned)
    {
      this.Id = machineToBeCloned.Id;
      this.Tags = this.Tags == null ? (IList<string>) null : (IList<string>) new List<string>((IEnumerable<string>) machineToBeCloned.Tags);
      this.Agent = machineToBeCloned.Agent?.Clone();
    }

    [DataMember]
    public int Id { get; set; }

    public IList<string> Tags
    {
      get => this.m_tags;
      set => this.m_tags = value;
    }

    [DataMember(EmitDefaultValue = false)]
    public TaskAgent Agent { get; set; }

    public PropertiesCollection Properties
    {
      get
      {
        if (this.m_properties == null)
          this.m_properties = new PropertiesCollection();
        return this.m_properties;
      }
      internal set => this.m_properties = value;
    }

    object ICloneable.Clone() => (object) this.Clone();

    public DeploymentMachine Clone() => new DeploymentMachine(this);
  }
}
