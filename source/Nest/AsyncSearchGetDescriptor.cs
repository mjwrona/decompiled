// Decompiled with JetBrains decompiler
// Type: Nest.AsyncSearchGetDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.AsyncSearchApi;
using Elasticsearch.Net.Utf8Json;
using System;

namespace Nest
{
  public class AsyncSearchGetDescriptor : 
    RequestDescriptorBase<AsyncSearchGetDescriptor, AsyncSearchGetRequestParameters, IAsyncSearchGetRequest>,
    IAsyncSearchGetRequest,
    IRequest<AsyncSearchGetRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.AsyncSearchGet;

    public AsyncSearchGetDescriptor(Id id)
      : base((Func<RouteValues, RouteValues>) (r => r.Required(nameof (id), (IUrlParameter) id)))
    {
    }

    [SerializationConstructor]
    protected AsyncSearchGetDescriptor()
    {
    }

    Id IAsyncSearchGetRequest.Id => this.Self.RouteValues.Get<Id>("id");

    public AsyncSearchGetDescriptor KeepAlive(Time keepalive) => this.Qs("keep_alive", (object) keepalive);

    public AsyncSearchGetDescriptor TypedKeys(bool? typedkeys = true) => this.Qs("typed_keys", (object) typedkeys);

    public AsyncSearchGetDescriptor WaitForCompletionTimeout(Time waitforcompletiontimeout) => this.Qs("wait_for_completion_timeout", (object) waitforcompletiontimeout);
  }
}
