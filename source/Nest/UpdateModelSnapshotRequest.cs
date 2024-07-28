// Decompiled with JetBrains decompiler
// Type: Nest.UpdateModelSnapshotRequest
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
  public class UpdateModelSnapshotRequest : 
    PlainRequestBase<UpdateModelSnapshotRequestParameters>,
    IUpdateModelSnapshotRequest,
    IRequest<UpdateModelSnapshotRequestParameters>,
    IRequest
  {
    protected IUpdateModelSnapshotRequest Self => (IUpdateModelSnapshotRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.MachineLearningUpdateModelSnapshot;

    public UpdateModelSnapshotRequest(Id jobId, Id snapshotId)
      : base((Func<RouteValues, RouteValues>) (r => r.Required("job_id", (IUrlParameter) jobId).Required("snapshot_id", (IUrlParameter) snapshotId)))
    {
    }

    [SerializationConstructor]
    protected UpdateModelSnapshotRequest()
    {
    }

    [IgnoreDataMember]
    Id IUpdateModelSnapshotRequest.JobId => this.Self.RouteValues.Get<Id>("job_id");

    [IgnoreDataMember]
    Id IUpdateModelSnapshotRequest.SnapshotId => this.Self.RouteValues.Get<Id>("snapshot_id");

    public string Description { get; set; }

    public bool? Retain { get; set; }
  }
}
