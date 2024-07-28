// Decompiled with JetBrains decompiler
// Type: Nest.AsyncSearchDeleteRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.AsyncSearchApi;
using Elasticsearch.Net.Utf8Json;
using System;
using System.Runtime.Serialization;

namespace Nest
{
  public class AsyncSearchDeleteRequest : 
    PlainRequestBase<AsyncSearchDeleteRequestParameters>,
    IAsyncSearchDeleteRequest,
    IRequest<AsyncSearchDeleteRequestParameters>,
    IRequest
  {
    protected IAsyncSearchDeleteRequest Self => (IAsyncSearchDeleteRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.AsyncSearchDelete;

    public AsyncSearchDeleteRequest(Id id)
      : base((Func<RouteValues, RouteValues>) (r => r.Required(nameof (id), (IUrlParameter) id)))
    {
    }

    [SerializationConstructor]
    protected AsyncSearchDeleteRequest()
    {
    }

    [IgnoreDataMember]
    Id IAsyncSearchDeleteRequest.Id => this.Self.RouteValues.Get<Id>("id");
  }
}
