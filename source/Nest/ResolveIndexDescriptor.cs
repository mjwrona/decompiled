// Decompiled with JetBrains decompiler
// Type: Nest.ResolveIndexDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.IndicesApi;
using Elasticsearch.Net.Utf8Json;
using System;

namespace Nest
{
  public class ResolveIndexDescriptor : 
    RequestDescriptorBase<ResolveIndexDescriptor, ResolveIndexRequestParameters, IResolveIndexRequest>,
    IResolveIndexRequest,
    IRequest<ResolveIndexRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.IndicesResolve;

    public ResolveIndexDescriptor(Names name)
      : base((Func<RouteValues, RouteValues>) (r => r.Required(nameof (name), (IUrlParameter) name)))
    {
    }

    [SerializationConstructor]
    protected ResolveIndexDescriptor()
    {
    }

    Names IResolveIndexRequest.Name => this.Self.RouteValues.Get<Names>("name");

    public ResolveIndexDescriptor ExpandWildcards(Elasticsearch.Net.ExpandWildcards? expandwildcards) => this.Qs("expand_wildcards", (object) expandwildcards);
  }
}
