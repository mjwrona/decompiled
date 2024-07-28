// Decompiled with JetBrains decompiler
// Type: Nest.ExplainLifecycleDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.IndexLifecycleManagementApi;
using Elasticsearch.Net.Utf8Json;
using System;

namespace Nest
{
  public class ExplainLifecycleDescriptor : 
    RequestDescriptorBase<ExplainLifecycleDescriptor, ExplainLifecycleRequestParameters, IExplainLifecycleRequest>,
    IExplainLifecycleRequest,
    IRequest<ExplainLifecycleRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.IndexLifecycleManagementExplainLifecycle;

    public ExplainLifecycleDescriptor(IndexName index)
      : base((Func<RouteValues, RouteValues>) (r => r.Required(nameof (index), (IUrlParameter) index)))
    {
    }

    [SerializationConstructor]
    protected ExplainLifecycleDescriptor()
    {
    }

    IndexName IExplainLifecycleRequest.Index => this.Self.RouteValues.Get<IndexName>("index");

    public ExplainLifecycleDescriptor Index(IndexName index) => this.Assign<IndexName>(index, (Action<IExplainLifecycleRequest, IndexName>) ((a, v) => a.RouteValues.Required(nameof (index), (IUrlParameter) v)));

    public ExplainLifecycleDescriptor Index<TOther>() where TOther : class => this.Assign<Type>(typeof (TOther), (Action<IExplainLifecycleRequest, Type>) ((a, v) => a.RouteValues.Required("index", (IUrlParameter) (IndexName) v)));

    public ExplainLifecycleDescriptor OnlyErrors(bool? onlyerrors = true) => this.Qs("only_errors", (object) onlyerrors);

    public ExplainLifecycleDescriptor OnlyManaged(bool? onlymanaged = true) => this.Qs("only_managed", (object) onlymanaged);
  }
}
