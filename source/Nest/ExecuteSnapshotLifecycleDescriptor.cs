// Decompiled with JetBrains decompiler
// Type: Nest.ExecuteSnapshotLifecycleDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.SnapshotLifecycleManagementApi;
using Elasticsearch.Net.Utf8Json;
using System;

namespace Nest
{
  public class ExecuteSnapshotLifecycleDescriptor : 
    RequestDescriptorBase<ExecuteSnapshotLifecycleDescriptor, ExecuteSnapshotLifecycleRequestParameters, IExecuteSnapshotLifecycleRequest>,
    IExecuteSnapshotLifecycleRequest,
    IRequest<ExecuteSnapshotLifecycleRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.SnapshotLifecycleManagementExecuteSnapshotLifecycle;

    public ExecuteSnapshotLifecycleDescriptor(Id policyId)
      : base((Func<RouteValues, RouteValues>) (r => r.Required("policy_id", (IUrlParameter) policyId)))
    {
    }

    [SerializationConstructor]
    protected ExecuteSnapshotLifecycleDescriptor()
    {
    }

    Id IExecuteSnapshotLifecycleRequest.PolicyId => this.Self.RouteValues.Get<Id>("policy_id");
  }
}
