// Decompiled with JetBrains decompiler
// Type: Nest.GetModelSnapshotsRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.MachineLearningApi;
using Elasticsearch.Net.Utf8Json;
using System;
using System.Runtime.Serialization;

namespace Nest
{
  public class GetModelSnapshotsRequest : 
    PlainRequestBase<GetModelSnapshotsRequestParameters>,
    IGetModelSnapshotsRequest,
    IRequest<GetModelSnapshotsRequestParameters>,
    IRequest
  {
    protected IGetModelSnapshotsRequest Self => (IGetModelSnapshotsRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.MachineLearningGetModelSnapshots;

    public GetModelSnapshotsRequest(Id jobId, Id snapshotId)
      : base((Func<RouteValues, RouteValues>) (r => r.Required("job_id", (IUrlParameter) jobId).Optional("snapshot_id", (IUrlParameter) snapshotId)))
    {
    }

    public GetModelSnapshotsRequest(Id jobId)
      : base((Func<RouteValues, RouteValues>) (r => r.Required("job_id", (IUrlParameter) jobId)))
    {
    }

    [SerializationConstructor]
    protected GetModelSnapshotsRequest()
    {
    }

    [IgnoreDataMember]
    Id IGetModelSnapshotsRequest.JobId => this.Self.RouteValues.Get<Id>("job_id");

    [IgnoreDataMember]
    Id IGetModelSnapshotsRequest.SnapshotId => this.Self.RouteValues.Get<Id>("snapshot_id");

    public bool? Descending { get; set; }

    public DateTimeOffset? End { get; set; }

    public IPage Page { get; set; }

    public Field Sort { get; set; }

    public DateTimeOffset? Start { get; set; }
  }
}
