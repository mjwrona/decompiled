// Decompiled with JetBrains decompiler
// Type: Nest.SnapshotStatusDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.SnapshotApi;
using System;

namespace Nest
{
  public class SnapshotStatusDescriptor : 
    RequestDescriptorBase<SnapshotStatusDescriptor, SnapshotStatusRequestParameters, ISnapshotStatusRequest>,
    ISnapshotStatusRequest,
    IRequest<SnapshotStatusRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.SnapshotStatus;

    public SnapshotStatusDescriptor()
    {
    }

    public SnapshotStatusDescriptor(Name repository)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional(nameof (repository), (IUrlParameter) repository)))
    {
    }

    public SnapshotStatusDescriptor(Name repository, Names snapshot)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional(nameof (repository), (IUrlParameter) repository).Optional(nameof (snapshot), (IUrlParameter) snapshot)))
    {
    }

    Name ISnapshotStatusRequest.RepositoryName => this.Self.RouteValues.Get<Name>("repository");

    Names ISnapshotStatusRequest.Snapshot => this.Self.RouteValues.Get<Names>("snapshot");

    public SnapshotStatusDescriptor RepositoryName(Name repository) => this.Assign<Name>(repository, (Action<ISnapshotStatusRequest, Name>) ((a, v) => a.RouteValues.Optional(nameof (repository), (IUrlParameter) v)));

    public SnapshotStatusDescriptor Snapshot(Names snapshot) => this.Assign<Names>(snapshot, (Action<ISnapshotStatusRequest, Names>) ((a, v) => a.RouteValues.Optional(nameof (snapshot), (IUrlParameter) v)));

    public SnapshotStatusDescriptor IgnoreUnavailable(bool? ignoreunavailable = true) => this.Qs("ignore_unavailable", (object) ignoreunavailable);

    public SnapshotStatusDescriptor MasterTimeout(Time mastertimeout) => this.Qs("master_timeout", (object) mastertimeout);
  }
}
