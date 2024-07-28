// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.WebApi.ReleaseReference
// Assembly: Microsoft.TeamFoundation.TestManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 10F0A812-3ECA-42B4-865D-429941F99EBE
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.TestManagement.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.WebApi
{
  [DataContract]
  public sealed class ReleaseReference : TestManagementBaseSecuredObject
  {
    public ReleaseReference()
    {
    }

    public ReleaseReference(ISecuredObject secureObject)
      : base(secureObject)
    {
    }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public int Id { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public string Name { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public int EnvironmentId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public string EnvironmentName { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public int DefinitionId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public int EnvironmentDefinitionId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public string EnvironmentDefinitionName { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public DateTime CreationDate { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public DateTime EnvironmentCreationDate { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int Attempt { get; set; }
  }
}
