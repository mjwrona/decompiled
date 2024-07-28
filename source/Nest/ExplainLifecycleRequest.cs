// Decompiled with JetBrains decompiler
// Type: Nest.ExplainLifecycleRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.IndexLifecycleManagementApi;
using Elasticsearch.Net.Utf8Json;
using System;
using System.Runtime.Serialization;

namespace Nest
{
  public class ExplainLifecycleRequest : 
    PlainRequestBase<ExplainLifecycleRequestParameters>,
    IExplainLifecycleRequest,
    IRequest<ExplainLifecycleRequestParameters>,
    IRequest
  {
    protected IExplainLifecycleRequest Self => (IExplainLifecycleRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.IndexLifecycleManagementExplainLifecycle;

    public ExplainLifecycleRequest(IndexName index)
      : base((Func<RouteValues, RouteValues>) (r => r.Required(nameof (index), (IUrlParameter) index)))
    {
    }

    [SerializationConstructor]
    protected ExplainLifecycleRequest()
    {
    }

    [IgnoreDataMember]
    IndexName IExplainLifecycleRequest.Index => this.Self.RouteValues.Get<IndexName>("index");

    public bool? OnlyErrors
    {
      get => this.Q<bool?>("only_errors");
      set => this.Q("only_errors", (object) value);
    }

    public bool? OnlyManaged
    {
      get => this.Q<bool?>("only_managed");
      set => this.Q("only_managed", (object) value);
    }
  }
}
