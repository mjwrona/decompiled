// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.WebApi.RepositoryChange
// Assembly: Microsoft.Azure.Pipelines.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9955A178-37CB-46CB-B455-32EA2A66C5BA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Microsoft.Azure.Pipelines.WebApi
{
  [DataContract]
  public class RepositoryChange : BaseSecuredObject
  {
    [JsonConstructor]
    public RepositoryChange()
    {
    }

    internal RepositoryChange(ISecuredObject securedObject)
      : base(securedObject)
    {
    }

    [DataMember]
    public RepositoryChangeIndentity Author { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public RepositoryChangeIndentity Committer { get; set; }

    [DataMember]
    public string Message { get; set; }

    [DataMember]
    public string Version { get; set; }
  }
}
