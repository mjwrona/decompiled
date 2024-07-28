// Decompiled with JetBrains decompiler
// Type: Nest.RetryIlmDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.IndexLifecycleManagementApi;
using Elasticsearch.Net.Utf8Json;
using System;

namespace Nest
{
  public class RetryIlmDescriptor : 
    RequestDescriptorBase<RetryIlmDescriptor, RetryIlmRequestParameters, IRetryIlmRequest>,
    IRetryIlmRequest,
    IRequest<RetryIlmRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.IndexLifecycleManagementRetry;

    public RetryIlmDescriptor(IndexName index)
      : base((Func<RouteValues, RouteValues>) (r => r.Required(nameof (index), (IUrlParameter) index)))
    {
    }

    [SerializationConstructor]
    protected RetryIlmDescriptor()
    {
    }

    IndexName IRetryIlmRequest.Index => this.Self.RouteValues.Get<IndexName>("index");

    public RetryIlmDescriptor Index(IndexName index) => this.Assign<IndexName>(index, (Action<IRetryIlmRequest, IndexName>) ((a, v) => a.RouteValues.Required(nameof (index), (IUrlParameter) v)));

    public RetryIlmDescriptor Index<TOther>() where TOther : class => this.Assign<Type>(typeof (TOther), (Action<IRetryIlmRequest, Type>) ((a, v) => a.RouteValues.Required("index", (IUrlParameter) (IndexName) v)));
  }
}
