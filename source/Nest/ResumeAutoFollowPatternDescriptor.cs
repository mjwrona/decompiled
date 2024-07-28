// Decompiled with JetBrains decompiler
// Type: Nest.ResumeAutoFollowPatternDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.CrossClusterReplicationApi;
using Elasticsearch.Net.Utf8Json;
using System;

namespace Nest
{
  public class ResumeAutoFollowPatternDescriptor : 
    RequestDescriptorBase<ResumeAutoFollowPatternDescriptor, ResumeAutoFollowPatternRequestParameters, IResumeAutoFollowPatternRequest>,
    IResumeAutoFollowPatternRequest,
    IRequest<ResumeAutoFollowPatternRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.CrossClusterReplicationResumeAutoFollowPattern;

    public ResumeAutoFollowPatternDescriptor(Name name)
      : base((Func<RouteValues, RouteValues>) (r => r.Required(nameof (name), (IUrlParameter) name)))
    {
    }

    [SerializationConstructor]
    protected ResumeAutoFollowPatternDescriptor()
    {
    }

    Name IResumeAutoFollowPatternRequest.Name => this.Self.RouteValues.Get<Name>("name");
  }
}
