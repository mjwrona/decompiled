// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.TaskAgentPoolReference
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.WebApi
{
  [DataContract]
  public class TaskAgentPoolReference
  {
    private TaskAgentPoolType m_poolType = TaskAgentPoolType.Automation;

    public TaskAgentPoolReference()
    {
    }

    public TaskAgentPoolReference(Guid scope, int id, TaskAgentPoolType poolType = TaskAgentPoolType.Automation)
    {
      this.Id = id;
      this.Scope = scope;
      this.PoolType = poolType;
    }

    protected TaskAgentPoolReference(TaskAgentPoolReference referenceToBeCloned)
    {
      this.Id = referenceToBeCloned.Id;
      this.Name = referenceToBeCloned.Name;
      this.Scope = referenceToBeCloned.Scope;
      this.IsHosted = referenceToBeCloned.IsHosted;
      this.PoolType = referenceToBeCloned.PoolType;
      this.Size = referenceToBeCloned.Size;
      this.IsLegacy = referenceToBeCloned.IsLegacy;
      this.Options = referenceToBeCloned.Options;
    }

    public TaskAgentPoolReference Clone() => new TaskAgentPoolReference(this);

    [DataMember(EmitDefaultValue = false)]
    public int Id { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public Guid Scope { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Name { get; set; }

    [DataMember]
    public bool IsHosted { get; set; }

    [DataMember]
    public TaskAgentPoolType PoolType
    {
      get => this.m_poolType;
      set => this.m_poolType = value;
    }

    [DataMember]
    public int Size { get; set; }

    [DataMember]
    public bool? IsLegacy { get; set; }

    [DataMember]
    public TaskAgentPoolOptions? Options { get; set; }
  }
}
