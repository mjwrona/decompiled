// Decompiled with JetBrains decompiler
// Type: Nest.CloneSnapshotDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.SnapshotApi;
using Elasticsearch.Net.Utf8Json;
using System;

namespace Nest
{
  public class CloneSnapshotDescriptor : 
    RequestDescriptorBase<CloneSnapshotDescriptor, CloneSnapshotRequestParameters, ICloneSnapshotRequest>,
    ICloneSnapshotRequest,
    IRequest<CloneSnapshotRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.SnapshotClone;

    public CloneSnapshotDescriptor(Name repository, Name snapshot, Name targetSnapshot)
      : base((Func<RouteValues, RouteValues>) (r => r.Required(nameof (repository), (IUrlParameter) repository).Required(nameof (snapshot), (IUrlParameter) snapshot).Required("target_snapshot", (IUrlParameter) targetSnapshot)))
    {
    }

    [SerializationConstructor]
    protected CloneSnapshotDescriptor()
    {
    }

    Name ICloneSnapshotRequest.RepositoryName => this.Self.RouteValues.Get<Name>("repository");

    Name ICloneSnapshotRequest.Snapshot => this.Self.RouteValues.Get<Name>("snapshot");

    Name ICloneSnapshotRequest.TargetSnapshot => this.Self.RouteValues.Get<Name>("target_snapshot");

    public CloneSnapshotDescriptor MasterTimeout(Time mastertimeout) => this.Qs("master_timeout", (object) mastertimeout);

    Nest.Indices ICloneSnapshotRequest.Indices { get; set; }

    public CloneSnapshotDescriptor Index(IndexName index) => this.Indices((Nest.Indices) index);

    public CloneSnapshotDescriptor Index<T>() where T : class => this.Indices((Nest.Indices) typeof (T));

    public CloneSnapshotDescriptor Indices(Nest.Indices indices) => this.Assign<Nest.Indices>(indices, (Action<ICloneSnapshotRequest, Nest.Indices>) ((a, v) => a.Indices = v));
  }
}
