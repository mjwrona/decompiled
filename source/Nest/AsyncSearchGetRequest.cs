// Decompiled with JetBrains decompiler
// Type: Nest.AsyncSearchGetRequest
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
  public class AsyncSearchGetRequest : 
    PlainRequestBase<AsyncSearchGetRequestParameters>,
    IAsyncSearchGetRequest,
    IRequest<AsyncSearchGetRequestParameters>,
    IRequest
  {
    protected IAsyncSearchGetRequest Self => (IAsyncSearchGetRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.AsyncSearchGet;

    public AsyncSearchGetRequest(Id id)
      : base((Func<RouteValues, RouteValues>) (r => r.Required(nameof (id), (IUrlParameter) id)))
    {
    }

    [SerializationConstructor]
    protected AsyncSearchGetRequest()
    {
    }

    [IgnoreDataMember]
    Id IAsyncSearchGetRequest.Id => this.Self.RouteValues.Get<Id>("id");

    public Time KeepAlive
    {
      get => this.Q<Time>("keep_alive");
      set => this.Q("keep_alive", (object) value);
    }

    public bool? TypedKeys
    {
      get => this.Q<bool?>("typed_keys");
      set => this.Q("typed_keys", (object) value);
    }

    public Time WaitForCompletionTimeout
    {
      get => this.Q<Time>("wait_for_completion_timeout");
      set => this.Q("wait_for_completion_timeout", (object) value);
    }
  }
}
