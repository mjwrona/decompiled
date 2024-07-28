// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.TaskDefinitionReference
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.WebApi
{
  [DataContract]
  public class TaskDefinitionReference
  {
    private string m_definitionType;

    public TaskDefinitionReference() => this.DefinitionType = "task";

    private TaskDefinitionReference(TaskDefinitionReference definitionReference)
    {
      this.Id = definitionReference.Id;
      this.VersionSpec = definitionReference.VersionSpec;
      this.DefinitionType = definitionReference.DefinitionType ?? "task";
    }

    [DataMember(IsRequired = true)]
    public Guid Id { get; set; }

    [DataMember(IsRequired = true)]
    public string VersionSpec { get; set; }

    [DataMember(IsRequired = true)]
    public string DefinitionType
    {
      get => this.m_definitionType ?? (this.m_definitionType = "task");
      set => this.m_definitionType = value;
    }

    public override bool Equals(object obj)
    {
      TaskDefinitionReference definitionReference = (TaskDefinitionReference) obj;
      if (definitionReference == null || !this.Id.Equals(definitionReference.Id))
        return false;
      string versionSpec = this.VersionSpec;
      if ((versionSpec != null ? (versionSpec.Equals(definitionReference.VersionSpec) ? 1 : 0) : (this.VersionSpec == definitionReference.VersionSpec ? 1 : 0)) == 0)
        return false;
      string definitionType = this.DefinitionType;
      return definitionType == null ? this.DefinitionType == definitionReference.DefinitionType : definitionType.Equals(definitionReference.DefinitionType);
    }

    public override int GetHashCode() => this.ToString().GetHashCode();

    internal TaskDefinitionReference Clone() => new TaskDefinitionReference(this);
  }
}
