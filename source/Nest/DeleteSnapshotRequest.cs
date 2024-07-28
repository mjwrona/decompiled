// Decompiled with JetBrains decompiler
// Type: Nest.DeleteSnapshotRequest
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
  public class DeleteSnapshotRequest : 
    PlainRequestBase<DeleteSnapshotRequestParameters>,
    IDeleteSnapshotRequest,
    IRequest<DeleteSnapshotRequestParameters>,
    IRequest
  {
    protected IDeleteSnapshotRequest Self => (IDeleteSnapshotRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.SnapshotDelete;

    public DeleteSnapshotRequest(Name repository, Name snapshot)
      : base((Func<RouteValues, RouteValues>) (r => r.Required(nameof (repository), (IUrlParameter) repository).Required(nameof (snapshot), (IUrlParameter) snapshot)))
    {
    }

    [SerializationConstructor]
    protected DeleteSnapshotRequest()
    {
    }

    [IgnoreDataMember]
    Name IDeleteSnapshotRequest.RepositoryName => this.Self.RouteValues.Get<Name>("repository");

    [IgnoreDataMember]
    Name IDeleteSnapshotRequest.Snapshot => this.Self.RouteValues.Get<Name>("snapshot");

    public Time MasterTimeout
    {
      get => this.Q<Time>("master_timeout");
      set => this.Q("master_timeout", (object) value);
    }
  }
}
