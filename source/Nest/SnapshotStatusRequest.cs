// Decompiled with JetBrains decompiler
// Type: Nest.SnapshotStatusRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.SnapshotApi;
using System;
using System.Runtime.Serialization;

namespace Nest
{
  public class SnapshotStatusRequest : 
    PlainRequestBase<SnapshotStatusRequestParameters>,
    ISnapshotStatusRequest,
    IRequest<SnapshotStatusRequestParameters>,
    IRequest
  {
    protected ISnapshotStatusRequest Self => (ISnapshotStatusRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.SnapshotStatus;

    public SnapshotStatusRequest()
    {
    }

    public SnapshotStatusRequest(Name repository)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional(nameof (repository), (IUrlParameter) repository)))
    {
    }

    public SnapshotStatusRequest(Name repository, Names snapshot)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional(nameof (repository), (IUrlParameter) repository).Optional(nameof (snapshot), (IUrlParameter) snapshot)))
    {
    }

    [IgnoreDataMember]
    Name ISnapshotStatusRequest.RepositoryName => this.Self.RouteValues.Get<Name>("repository");

    [IgnoreDataMember]
    Names ISnapshotStatusRequest.Snapshot => this.Self.RouteValues.Get<Names>("snapshot");

    public bool? IgnoreUnavailable
    {
      get => this.Q<bool?>("ignore_unavailable");
      set => this.Q("ignore_unavailable", (object) value);
    }

    public Time MasterTimeout
    {
      get => this.Q<Time>("master_timeout");
      set => this.Q("master_timeout", (object) value);
    }
  }
}
