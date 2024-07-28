// Decompiled with JetBrains decompiler
// Type: Nest.DeleteSnapshotDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.SnapshotApi;
using Elasticsearch.Net.Utf8Json;
using System;

namespace Nest
{
  public class DeleteSnapshotDescriptor : 
    RequestDescriptorBase<DeleteSnapshotDescriptor, DeleteSnapshotRequestParameters, IDeleteSnapshotRequest>,
    IDeleteSnapshotRequest,
    IRequest<DeleteSnapshotRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.SnapshotDelete;

    public DeleteSnapshotDescriptor(Name repository, Name snapshot)
      : base((Func<RouteValues, RouteValues>) (r => r.Required(nameof (repository), (IUrlParameter) repository).Required(nameof (snapshot), (IUrlParameter) snapshot)))
    {
    }

    [SerializationConstructor]
    protected DeleteSnapshotDescriptor()
    {
    }

    Name IDeleteSnapshotRequest.RepositoryName => this.Self.RouteValues.Get<Name>("repository");

    Name IDeleteSnapshotRequest.Snapshot => this.Self.RouteValues.Get<Name>("snapshot");

    public DeleteSnapshotDescriptor MasterTimeout(Time mastertimeout) => this.Qs("master_timeout", (object) mastertimeout);
  }
}
