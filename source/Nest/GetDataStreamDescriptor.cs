// Decompiled with JetBrains decompiler
// Type: Nest.GetDataStreamDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.IndicesApi;
using System;

namespace Nest
{
  public class GetDataStreamDescriptor : 
    RequestDescriptorBase<GetDataStreamDescriptor, GetDataStreamRequestParameters, IGetDataStreamRequest>,
    IGetDataStreamRequest,
    IRequest<GetDataStreamRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.IndicesGetDataStream;

    public GetDataStreamDescriptor()
    {
    }

    public GetDataStreamDescriptor(Names name)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional(nameof (name), (IUrlParameter) name)))
    {
    }

    Names IGetDataStreamRequest.Name => this.Self.RouteValues.Get<Names>("name");

    public GetDataStreamDescriptor Name(Names name) => this.Assign<Names>(name, (Action<IGetDataStreamRequest, Names>) ((a, v) => a.RouteValues.Optional(nameof (name), (IUrlParameter) v)));

    public GetDataStreamDescriptor ExpandWildcards(Elasticsearch.Net.ExpandWildcards? expandwildcards) => this.Qs("expand_wildcards", (object) expandwildcards);
  }
}
