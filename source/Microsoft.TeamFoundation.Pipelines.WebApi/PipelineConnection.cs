// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Pipelines.WebApi.PipelineConnection
// Assembly: Microsoft.TeamFoundation.Pipelines.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 29F2A1B3-A3F7-4291-91FA-6C4508EECB65
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Pipelines.WebApi.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Pipelines.WebApi
{
  [DataContract]
  public class PipelineConnection
  {
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public Guid? AccountId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public Guid? TeamProjectId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public Guid? ServiceEndpointId { get; set; }

    [Obsolete("This property is no longer filled in and will be removed soon.")]
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int? DefinitionId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string RedirectUrl { get; set; }
  }
}
