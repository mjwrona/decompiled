// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.WebApi.RepositoryWebhook
// Assembly: Microsoft.Azure.Pipelines.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9955A178-37CB-46CB-B455-32EA2A66C5BA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Microsoft.Azure.Pipelines.WebApi
{
  [DataContract]
  public class RepositoryWebhook : BaseSecuredObject
  {
    [JsonConstructor]
    public RepositoryWebhook()
    {
    }

    internal RepositoryWebhook(ISecuredObject securedObject)
      : base(securedObject)
    {
    }

    [DataMember]
    public string Alias { get; set; }

    [DataMember]
    public string Id { get; set; }

    [DataMember]
    public string Type { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public RepositoryChange Change { get; set; }

    [DataMember]
    public string Url { get; set; }
  }
}
