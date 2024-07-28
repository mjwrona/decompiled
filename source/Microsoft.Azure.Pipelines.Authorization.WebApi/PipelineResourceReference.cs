// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Authorization.WebApi.PipelineResourceReference
// Assembly: Microsoft.Azure.Pipelines.Authorization.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4807FD31-F2A4-4329-AA76-35B262BDA671
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Authorization.WebApi.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.Azure.Pipelines.Authorization.WebApi
{
  [DataContract]
  public class PipelineResourceReference : IEquatable<PipelineResourceReference>
  {
    [DataMember(EmitDefaultValue = false)]
    public string Type { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool Authorized { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public Guid? AuthorizedBy { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public DateTime? AuthorizedOn { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int? DefinitionId { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Id { get; set; }

    public bool Equals(PipelineResourceReference other)
    {
      if (!(this.Type == other.Type) || string.IsNullOrWhiteSpace(this.Id) || string.IsNullOrWhiteSpace(other.Id) || !string.Equals(this.Id, other.Id, StringComparison.InvariantCultureIgnoreCase) || this.Authorized != other.Authorized)
        return false;
      int? definitionId1 = this.DefinitionId;
      int? definitionId2 = other.DefinitionId;
      return definitionId1.GetValueOrDefault() == definitionId2.GetValueOrDefault() & definitionId1.HasValue == definitionId2.HasValue;
    }

    public override int GetHashCode() => this == null || this.Type == null ? 0 : this.Type.GetHashCode();

    public string GetId() => this.Id.ToString();
  }
}
