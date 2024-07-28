// Decompiled with JetBrains decompiler
// Type: Nest.ExecuteSnapshotLifecycleRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.SnapshotLifecycleManagementApi;
using Elasticsearch.Net.Utf8Json;
using System;
using System.Runtime.Serialization;

namespace Nest
{
  public class ExecuteSnapshotLifecycleRequest : 
    PlainRequestBase<ExecuteSnapshotLifecycleRequestParameters>,
    IExecuteSnapshotLifecycleRequest,
    IRequest<ExecuteSnapshotLifecycleRequestParameters>,
    IRequest
  {
    protected IExecuteSnapshotLifecycleRequest Self => (IExecuteSnapshotLifecycleRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.SnapshotLifecycleManagementExecuteSnapshotLifecycle;

    public ExecuteSnapshotLifecycleRequest(Id policyId)
      : base((Func<RouteValues, RouteValues>) (r => r.Required("policy_id", (IUrlParameter) policyId)))
    {
    }

    [SerializationConstructor]
    protected ExecuteSnapshotLifecycleRequest()
    {
    }

    [IgnoreDataMember]
    Id IExecuteSnapshotLifecycleRequest.PolicyId => this.Self.RouteValues.Get<Id>("policy_id");
  }
}
