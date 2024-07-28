// Decompiled with JetBrains decompiler
// Type: Nest.PutSnapshotLifecycleRequest
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
  public class PutSnapshotLifecycleRequest : 
    PlainRequestBase<PutSnapshotLifecycleRequestParameters>,
    IPutSnapshotLifecycleRequest,
    IRequest<PutSnapshotLifecycleRequestParameters>,
    IRequest
  {
    protected IPutSnapshotLifecycleRequest Self => (IPutSnapshotLifecycleRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.SnapshotLifecycleManagementPutSnapshotLifecycle;

    public PutSnapshotLifecycleRequest(Id policyId)
      : base((Func<RouteValues, RouteValues>) (r => r.Required("policy_id", (IUrlParameter) policyId)))
    {
    }

    [SerializationConstructor]
    protected PutSnapshotLifecycleRequest()
    {
    }

    [IgnoreDataMember]
    Id IPutSnapshotLifecycleRequest.PolicyId => this.Self.RouteValues.Get<Id>("policy_id");

    public ISnapshotLifecycleConfig Config { get; set; }

    public string Name { get; set; }

    public string Repository { get; set; }

    public CronExpression Schedule { get; set; }

    public ISnapshotRetentionConfiguration Retention { get; set; }
  }
}
