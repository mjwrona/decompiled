// Decompiled with JetBrains decompiler
// Type: Nest.AsyncSearchStatusRequest
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
  public class AsyncSearchStatusRequest : 
    PlainRequestBase<AsyncSearchStatusRequestParameters>,
    IAsyncSearchStatusRequest,
    IRequest<AsyncSearchStatusRequestParameters>,
    IRequest
  {
    protected IAsyncSearchStatusRequest Self => (IAsyncSearchStatusRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.AsyncSearchStatus;

    public AsyncSearchStatusRequest(Id id)
      : base((Func<RouteValues, RouteValues>) (r => r.Required(nameof (id), (IUrlParameter) id)))
    {
    }

    [SerializationConstructor]
    protected AsyncSearchStatusRequest()
    {
    }

    [IgnoreDataMember]
    Id IAsyncSearchStatusRequest.Id => this.Self.RouteValues.Get<Id>("id");
  }
}
