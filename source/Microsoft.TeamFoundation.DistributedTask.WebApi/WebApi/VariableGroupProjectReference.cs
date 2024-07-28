// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.VariableGroupProjectReference
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.WebApi
{
  [DataContract]
  public class VariableGroupProjectReference
  {
    public VariableGroupProjectReference()
    {
    }

    private VariableGroupProjectReference(
      VariableGroupProjectReference variableGroupReference)
    {
      this.ProjectReference = variableGroupReference.ProjectReference;
      this.Name = variableGroupReference.Name;
      this.Description = variableGroupReference.Description;
    }

    [DataMember(EmitDefaultValue = false)]
    public ProjectReference ProjectReference { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Name { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Description { get; set; }

    public VariableGroupProjectReference Clone() => new VariableGroupProjectReference()
    {
      Name = this.Name,
      Description = this.Description,
      ProjectReference = new ProjectReference()
      {
        Id = this.ProjectReference.Id,
        Name = this.ProjectReference.Name
      }
    };
  }
}
