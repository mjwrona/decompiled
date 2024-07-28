// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.WebApi.PullRequest
// Assembly: Microsoft.TeamFoundation.Build2.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0683407D-7C61-4505-8CA6-86AD7E3B6BCA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Build2.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Build.WebApi
{
  [DataContract]
  public class PullRequest : BaseSecuredObject
  {
    public PullRequest() => this.Links = new ReferenceLinks();

    internal PullRequest(ISecuredObject securedObject)
      : base(securedObject)
    {
      this.Links = new ReferenceLinks();
    }

    [DataMember]
    public string ProviderName { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Id { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Title { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Description { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string CurrentState { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IdentityRef Author { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool Draft { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string SourceRepositoryOwner { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string SourceBranchRef { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string TargetRepositoryOwner { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string TargetBranchRef { get; set; }

    [DataMember(Name = "_links", EmitDefaultValue = false)]
    public ReferenceLinks Links { get; set; }
  }
}
