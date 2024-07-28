// Decompiled with JetBrains decompiler
// Type: Nest.DeleteLifecycleRequest
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
  public class DeleteLifecycleRequest : 
    PlainRequestBase<DeleteLifecycleRequestParameters>,
    IDeleteLifecycleRequest,
    IRequest<DeleteLifecycleRequestParameters>,
    IRequest
  {
    protected IDeleteLifecycleRequest Self => (IDeleteLifecycleRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.IndexLifecycleManagementDeleteLifecycle;

    public DeleteLifecycleRequest(Id policyId)
      : base((Func<RouteValues, RouteValues>) (r => r.Required("policy_id", (IUrlParameter) policyId)))
    {
    }

    [SerializationConstructor]
    protected DeleteLifecycleRequest()
    {
    }

    [IgnoreDataMember]
    Id IDeleteLifecycleRequest.PolicyId => this.Self.RouteValues.Get<Id>("policy_id");
  }
}
