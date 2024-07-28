// Decompiled with JetBrains decompiler
// Type: Nest.GetRepositoryRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.SnapshotApi;
using System;
using System.Runtime.Serialization;

namespace Nest
{
  public class GetRepositoryRequest : 
    PlainRequestBase<GetRepositoryRequestParameters>,
    IGetRepositoryRequest,
    IRequest<GetRepositoryRequestParameters>,
    IRequest
  {
    protected IGetRepositoryRequest Self => (IGetRepositoryRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.SnapshotGetRepository;

    public GetRepositoryRequest()
    {
    }

    public GetRepositoryRequest(Names repository)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional(nameof (repository), (IUrlParameter) repository)))
    {
    }

    [IgnoreDataMember]
    Names IGetRepositoryRequest.RepositoryName => this.Self.RouteValues.Get<Names>("repository");

    public bool? Local
    {
      get => this.Q<bool?>("local");
      set => this.Q("local", (object) value);
    }

    public Time MasterTimeout
    {
      get => this.Q<Time>("master_timeout");
      set => this.Q("master_timeout", (object) value);
    }
  }
}
