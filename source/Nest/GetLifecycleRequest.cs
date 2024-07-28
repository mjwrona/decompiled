// Decompiled with JetBrains decompiler
// Type: Nest.GetLifecycleRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.IndexLifecycleManagementApi;
using System;
using System.Runtime.Serialization;

namespace Nest
{
  public class GetLifecycleRequest : 
    PlainRequestBase<GetLifecycleRequestParameters>,
    IGetLifecycleRequest,
    IRequest<GetLifecycleRequestParameters>,
    IRequest
  {
    protected IGetLifecycleRequest Self => (IGetLifecycleRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.IndexLifecycleManagementGetLifecycle;

    public GetLifecycleRequest(Id policyId)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional("policy_id", (IUrlParameter) policyId)))
    {
    }

    public GetLifecycleRequest()
    {
    }

    [IgnoreDataMember]
    Id IGetLifecycleRequest.PolicyId => this.Self.RouteValues.Get<Id>("policy_id");
  }
}
