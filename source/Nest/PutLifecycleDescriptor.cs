// Decompiled with JetBrains decompiler
// Type: Nest.PutLifecycleDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.IndexLifecycleManagementApi;
using Elasticsearch.Net.Utf8Json;
using System;

namespace Nest
{
  public class PutLifecycleDescriptor : 
    RequestDescriptorBase<PutLifecycleDescriptor, PutLifecycleRequestParameters, IPutLifecycleRequest>,
    IPutLifecycleRequest,
    IRequest<PutLifecycleRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.IndexLifecycleManagementPutLifecycle;

    public PutLifecycleDescriptor(Id policyId)
      : base((Func<RouteValues, RouteValues>) (r => r.Required("policy_id", (IUrlParameter) policyId)))
    {
    }

    [SerializationConstructor]
    protected PutLifecycleDescriptor()
    {
    }

    Id IPutLifecycleRequest.PolicyId => this.Self.RouteValues.Get<Id>("policy_id");

    IPolicy IPutLifecycleRequest.Policy { get; set; }

    public PutLifecycleDescriptor Policy(Func<PolicyDescriptor, IPolicy> selector) => this.Assign<Func<PolicyDescriptor, IPolicy>>(selector, (Action<IPutLifecycleRequest, Func<PolicyDescriptor, IPolicy>>) ((a, v) => a.Policy = v != null ? v.InvokeOrDefault<PolicyDescriptor, IPolicy>(new PolicyDescriptor()) : (IPolicy) null));
  }
}
