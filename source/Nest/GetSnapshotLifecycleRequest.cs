// Decompiled with JetBrains decompiler
// Type: Nest.GetSnapshotLifecycleRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.SnapshotLifecycleManagementApi;
using System;
using System.Runtime.Serialization;

namespace Nest
{
  public class GetSnapshotLifecycleRequest : 
    PlainRequestBase<GetSnapshotLifecycleRequestParameters>,
    IGetSnapshotLifecycleRequest,
    IRequest<GetSnapshotLifecycleRequestParameters>,
    IRequest
  {
    protected IGetSnapshotLifecycleRequest Self => (IGetSnapshotLifecycleRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.SnapshotLifecycleManagementGetSnapshotLifecycle;

    public GetSnapshotLifecycleRequest(Ids policyId)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional("policy_id", (IUrlParameter) policyId)))
    {
    }

    public GetSnapshotLifecycleRequest()
    {
    }

    [IgnoreDataMember]
    Ids IGetSnapshotLifecycleRequest.PolicyId => this.Self.RouteValues.Get<Ids>("policy_id");
  }
}
