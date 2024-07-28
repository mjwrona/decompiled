// Decompiled with JetBrains decompiler
// Type: Nest.GetDataStreamRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.IndicesApi;
using System;
using System.Runtime.Serialization;

namespace Nest
{
  public class GetDataStreamRequest : 
    PlainRequestBase<GetDataStreamRequestParameters>,
    IGetDataStreamRequest,
    IRequest<GetDataStreamRequestParameters>,
    IRequest
  {
    protected IGetDataStreamRequest Self => (IGetDataStreamRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.IndicesGetDataStream;

    public GetDataStreamRequest()
    {
    }

    public GetDataStreamRequest(Names name)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional(nameof (name), (IUrlParameter) name)))
    {
    }

    [IgnoreDataMember]
    Names IGetDataStreamRequest.Name => this.Self.RouteValues.Get<Names>("name");

    public Elasticsearch.Net.ExpandWildcards? ExpandWildcards
    {
      get => this.Q<Elasticsearch.Net.ExpandWildcards?>("expand_wildcards");
      set => this.Q("expand_wildcards", (object) value);
    }
  }
}
