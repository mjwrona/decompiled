// Decompiled with JetBrains decompiler
// Type: Nest.GetLifecycleDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.IndexLifecycleManagementApi;
using System;

namespace Nest
{
  public class GetLifecycleDescriptor : 
    RequestDescriptorBase<GetLifecycleDescriptor, GetLifecycleRequestParameters, IGetLifecycleRequest>,
    IGetLifecycleRequest,
    IRequest<GetLifecycleRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.IndexLifecycleManagementGetLifecycle;

    public GetLifecycleDescriptor(Id policyId)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional("policy_id", (IUrlParameter) policyId)))
    {
    }

    public GetLifecycleDescriptor()
    {
    }

    Id IGetLifecycleRequest.PolicyId => this.Self.RouteValues.Get<Id>("policy_id");

    public GetLifecycleDescriptor PolicyId(Id policyId) => this.Assign<Id>(policyId, (Action<IGetLifecycleRequest, Id>) ((a, v) => a.RouteValues.Optional("policy_id", (IUrlParameter) v)));
  }
}
