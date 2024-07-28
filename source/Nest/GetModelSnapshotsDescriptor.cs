// Decompiled with JetBrains decompiler
// Type: Nest.GetModelSnapshotsDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.MachineLearningApi;
using Elasticsearch.Net.Utf8Json;
using System;

namespace Nest
{
  public class GetModelSnapshotsDescriptor : 
    RequestDescriptorBase<GetModelSnapshotsDescriptor, GetModelSnapshotsRequestParameters, IGetModelSnapshotsRequest>,
    IGetModelSnapshotsRequest,
    IRequest<GetModelSnapshotsRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.MachineLearningGetModelSnapshots;

    public GetModelSnapshotsDescriptor(Id jobId, Id snapshotId)
      : base((Func<RouteValues, RouteValues>) (r => r.Required("job_id", (IUrlParameter) jobId).Optional("snapshot_id", (IUrlParameter) snapshotId)))
    {
    }

    public GetModelSnapshotsDescriptor(Id jobId)
      : base((Func<RouteValues, RouteValues>) (r => r.Required("job_id", (IUrlParameter) jobId)))
    {
    }

    [SerializationConstructor]
    protected GetModelSnapshotsDescriptor()
    {
    }

    Id IGetModelSnapshotsRequest.JobId => this.Self.RouteValues.Get<Id>("job_id");

    Id IGetModelSnapshotsRequest.SnapshotId => this.Self.RouteValues.Get<Id>("snapshot_id");

    public GetModelSnapshotsDescriptor SnapshotId(Id snapshotId) => this.Assign<Id>(snapshotId, (Action<IGetModelSnapshotsRequest, Id>) ((a, v) => a.RouteValues.Optional("snapshot_id", (IUrlParameter) v)));

    bool? IGetModelSnapshotsRequest.Descending { get; set; }

    DateTimeOffset? IGetModelSnapshotsRequest.End { get; set; }

    IPage IGetModelSnapshotsRequest.Page { get; set; }

    Field IGetModelSnapshotsRequest.Sort { get; set; }

    DateTimeOffset? IGetModelSnapshotsRequest.Start { get; set; }

    public GetModelSnapshotsDescriptor Descending(bool? desc = true) => this.Assign<bool?>(desc, (Action<IGetModelSnapshotsRequest, bool?>) ((a, v) => a.Descending = v));

    public GetModelSnapshotsDescriptor End(DateTimeOffset? end) => this.Assign<DateTimeOffset?>(end, (Action<IGetModelSnapshotsRequest, DateTimeOffset?>) ((a, v) => a.End = v));

    public GetModelSnapshotsDescriptor Page(Func<PageDescriptor, IPage> selector) => this.Assign<Func<PageDescriptor, IPage>>(selector, (Action<IGetModelSnapshotsRequest, Func<PageDescriptor, IPage>>) ((a, v) => a.Page = v != null ? v(new PageDescriptor()) : (IPage) null));

    public GetModelSnapshotsDescriptor Sort(Field field) => this.Assign<Field>(field, (Action<IGetModelSnapshotsRequest, Field>) ((a, v) => a.Sort = v));

    public GetModelSnapshotsDescriptor Start(DateTimeOffset? start) => this.Assign<DateTimeOffset?>(start, (Action<IGetModelSnapshotsRequest, DateTimeOffset?>) ((a, v) => a.Start = v));
  }
}
