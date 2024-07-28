// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.WebApi.Repository
// Assembly: Microsoft.Azure.Pipelines.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9955A178-37CB-46CB-B455-32EA2A66C5BA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.WebApi.dll

using Microsoft.Azure.Pipelines.WebApi.Contracts;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Microsoft.Azure.Pipelines.WebApi
{
  [DataContract]
  [KnownType(typeof (AzureReposGitRepository))]
  [KnownType(typeof (GitHubRepository))]
  [JsonConverter(typeof (RepositoryJsonConverter))]
  public class Repository : BaseSecuredObject
  {
    [JsonConstructor]
    protected Repository(RepositoryType type)
      : this((ISecuredObject) null, type)
    {
    }

    internal Repository(ISecuredObject securedObject, RepositoryType type)
      : base(securedObject)
    {
      this.Type = type;
    }

    [DataMember]
    public RepositoryType Type { get; private set; }
  }
}
