// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.WebApi.PipelineResource
// Assembly: Microsoft.Azure.Pipelines.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9955A178-37CB-46CB-B455-32EA2A66C5BA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Microsoft.Azure.Pipelines.WebApi
{
  [DataContract]
  public class PipelineResource : BaseSecuredObject
  {
    [JsonConstructor]
    public PipelineResource()
    {
    }

    internal PipelineResource(ISecuredObject securedObject)
      : base(securedObject)
    {
    }

    [DataMember]
    public PipelineReference Pipeline { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Version { get; set; }
  }
}
