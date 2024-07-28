// Decompiled with JetBrains decompiler
// Type: Nest.PauseAutoFollowPatternRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.CrossClusterReplicationApi;
using Elasticsearch.Net.Utf8Json;
using System;
using System.Runtime.Serialization;

namespace Nest
{
  public class PauseAutoFollowPatternRequest : 
    PlainRequestBase<PauseAutoFollowPatternRequestParameters>,
    IPauseAutoFollowPatternRequest,
    IRequest<PauseAutoFollowPatternRequestParameters>,
    IRequest
  {
    protected IPauseAutoFollowPatternRequest Self => (IPauseAutoFollowPatternRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.CrossClusterReplicationPauseAutoFollowPattern;

    public PauseAutoFollowPatternRequest(Name name)
      : base((Func<RouteValues, RouteValues>) (r => r.Required(nameof (name), (IUrlParameter) name)))
    {
    }

    [SerializationConstructor]
    protected PauseAutoFollowPatternRequest()
    {
    }

    [IgnoreDataMember]
    Name IPauseAutoFollowPatternRequest.Name => this.Self.RouteValues.Get<Name>("name");
  }
}
