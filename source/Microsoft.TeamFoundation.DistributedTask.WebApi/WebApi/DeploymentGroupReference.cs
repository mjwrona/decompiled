// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.DeploymentGroupReference
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.WebApi
{
  [DataContract]
  public class DeploymentGroupReference
  {
    [JsonConstructor]
    public DeploymentGroupReference()
    {
    }

    private DeploymentGroupReference(DeploymentGroupReference referenceToClone)
    {
      this.Id = referenceToClone.Id;
      this.Name = referenceToClone.Name;
      if (referenceToClone.Project != null)
        this.Project = new ProjectReference()
        {
          Id = referenceToClone.Project.Id,
          Name = referenceToClone.Project.Name
        };
      if (referenceToClone.Pool == null)
        return;
      this.Pool = new TaskAgentPoolReference()
      {
        Id = referenceToClone.Pool.Id,
        IsHosted = referenceToClone.Pool.IsHosted,
        Name = referenceToClone.Pool.Name,
        PoolType = referenceToClone.Pool.PoolType,
        Scope = referenceToClone.Pool.Scope
      };
    }

    [DataMember(EmitDefaultValue = false)]
    public int Id { get; internal set; }

    [DataMember(EmitDefaultValue = false)]
    public ProjectReference Project { get; internal set; }

    [DataMember(EmitDefaultValue = false)]
    public string Name { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public TaskAgentPoolReference Pool { get; set; }

    public virtual DeploymentGroupReference Clone() => new DeploymentGroupReference(this);
  }
}
