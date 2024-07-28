// Decompiled with JetBrains decompiler
// Type: Nest.UpdateModelSnapshotDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.MachineLearningApi;
using Elasticsearch.Net.Utf8Json;
using System;

namespace Nest
{
  public class UpdateModelSnapshotDescriptor : 
    RequestDescriptorBase<UpdateModelSnapshotDescriptor, UpdateModelSnapshotRequestParameters, IUpdateModelSnapshotRequest>,
    IUpdateModelSnapshotRequest,
    IRequest<UpdateModelSnapshotRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.MachineLearningUpdateModelSnapshot;

    public UpdateModelSnapshotDescriptor(Id jobId, Id snapshotId)
      : base((Func<RouteValues, RouteValues>) (r => r.Required("job_id", (IUrlParameter) jobId).Required("snapshot_id", (IUrlParameter) snapshotId)))
    {
    }

    [SerializationConstructor]
    protected UpdateModelSnapshotDescriptor()
    {
    }

    Id IUpdateModelSnapshotRequest.JobId => this.Self.RouteValues.Get<Id>("job_id");

    Id IUpdateModelSnapshotRequest.SnapshotId => this.Self.RouteValues.Get<Id>("snapshot_id");

    string IUpdateModelSnapshotRequest.Description { get; set; }

    bool? IUpdateModelSnapshotRequest.Retain { get; set; }

    public UpdateModelSnapshotDescriptor Description(string description) => this.Assign<string>(description, (Action<IUpdateModelSnapshotRequest, string>) ((a, v) => a.Description = v));

    public UpdateModelSnapshotDescriptor Retain(bool? retain = true) => this.Assign<bool?>(retain, (Action<IUpdateModelSnapshotRequest, bool?>) ((a, v) => a.Retain = v));
  }
}
