// Decompiled with JetBrains decompiler
// Type: Nest.DeleteDataStreamRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.IndicesApi;
using Elasticsearch.Net.Utf8Json;
using System;
using System.Runtime.Serialization;

namespace Nest
{
  public class DeleteDataStreamRequest : 
    PlainRequestBase<DeleteDataStreamRequestParameters>,
    IDeleteDataStreamRequest,
    IRequest<DeleteDataStreamRequestParameters>,
    IRequest
  {
    protected IDeleteDataStreamRequest Self => (IDeleteDataStreamRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.IndicesDeleteDataStream;

    public DeleteDataStreamRequest(Names name)
      : base((Func<RouteValues, RouteValues>) (r => r.Required(nameof (name), (IUrlParameter) name)))
    {
    }

    [SerializationConstructor]
    protected DeleteDataStreamRequest()
    {
    }

    [IgnoreDataMember]
    Names IDeleteDataStreamRequest.Name => this.Self.RouteValues.Get<Names>("name");

    public Elasticsearch.Net.ExpandWildcards? ExpandWildcards
    {
      get => this.Q<Elasticsearch.Net.ExpandWildcards?>("expand_wildcards");
      set => this.Q("expand_wildcards", (object) value);
    }
  }
}
