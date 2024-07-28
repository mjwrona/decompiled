// Decompiled with JetBrains decompiler
// Type: Nest.CloneSnapshotRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.SnapshotApi;
using Elasticsearch.Net.Utf8Json;
using System;
using System.Runtime.Serialization;

namespace Nest
{
  public class CloneSnapshotRequest : 
    PlainRequestBase<CloneSnapshotRequestParameters>,
    ICloneSnapshotRequest,
    IRequest<CloneSnapshotRequestParameters>,
    IRequest
  {
    public Indices Indices { get; set; }

    protected ICloneSnapshotRequest Self => (ICloneSnapshotRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.SnapshotClone;

    public CloneSnapshotRequest(Name repository, Name snapshot, Name targetSnapshot)
      : base((Func<RouteValues, RouteValues>) (r => r.Required(nameof (repository), (IUrlParameter) repository).Required(nameof (snapshot), (IUrlParameter) snapshot).Required("target_snapshot", (IUrlParameter) targetSnapshot)))
    {
    }

    [SerializationConstructor]
    protected CloneSnapshotRequest()
    {
    }

    [IgnoreDataMember]
    Name ICloneSnapshotRequest.RepositoryName => this.Self.RouteValues.Get<Name>("repository");

    [IgnoreDataMember]
    Name ICloneSnapshotRequest.Snapshot => this.Self.RouteValues.Get<Name>("snapshot");

    [IgnoreDataMember]
    Name ICloneSnapshotRequest.TargetSnapshot => this.Self.RouteValues.Get<Name>("target_snapshot");

    public Time MasterTimeout
    {
      get => this.Q<Time>("master_timeout");
      set => this.Q("master_timeout", (object) value);
    }
  }
}
