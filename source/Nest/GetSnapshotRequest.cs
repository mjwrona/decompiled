// Decompiled with JetBrains decompiler
// Type: Nest.GetSnapshotRequest
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
  public class GetSnapshotRequest : 
    PlainRequestBase<GetSnapshotRequestParameters>,
    IGetSnapshotRequest,
    IRequest<GetSnapshotRequestParameters>,
    IRequest
  {
    protected IGetSnapshotRequest Self => (IGetSnapshotRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.SnapshotGet;

    public GetSnapshotRequest(Name repository, Names snapshot)
      : base((Func<RouteValues, RouteValues>) (r => r.Required(nameof (repository), (IUrlParameter) repository).Required(nameof (snapshot), (IUrlParameter) snapshot)))
    {
    }

    [SerializationConstructor]
    protected GetSnapshotRequest()
    {
    }

    [IgnoreDataMember]
    Name IGetSnapshotRequest.RepositoryName => this.Self.RouteValues.Get<Name>("repository");

    [IgnoreDataMember]
    Names IGetSnapshotRequest.Snapshot => this.Self.RouteValues.Get<Names>("snapshot");

    public bool? IgnoreUnavailable
    {
      get => this.Q<bool?>("ignore_unavailable");
      set => this.Q("ignore_unavailable", (object) value);
    }

    public bool? IncludeRepository
    {
      get => this.Q<bool?>("include_repository");
      set => this.Q("include_repository", (object) value);
    }

    public bool? IndexDetails
    {
      get => this.Q<bool?>("index_details");
      set => this.Q("index_details", (object) value);
    }

    public Time MasterTimeout
    {
      get => this.Q<Time>("master_timeout");
      set => this.Q("master_timeout", (object) value);
    }

    public bool? Verbose
    {
      get => this.Q<bool?>("verbose");
      set => this.Q("verbose", (object) value);
    }
  }
}
