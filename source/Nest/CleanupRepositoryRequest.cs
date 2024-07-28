// Decompiled with JetBrains decompiler
// Type: Nest.CleanupRepositoryRequest
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
  public class CleanupRepositoryRequest : 
    PlainRequestBase<CleanupRepositoryRequestParameters>,
    ICleanupRepositoryRequest,
    IRequest<CleanupRepositoryRequestParameters>,
    IRequest
  {
    protected ICleanupRepositoryRequest Self => (ICleanupRepositoryRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.SnapshotCleanupRepository;

    public CleanupRepositoryRequest(Name repository)
      : base((Func<RouteValues, RouteValues>) (r => r.Required(nameof (repository), (IUrlParameter) repository)))
    {
    }

    [SerializationConstructor]
    protected CleanupRepositoryRequest()
    {
    }

    [IgnoreDataMember]
    Name ICleanupRepositoryRequest.RepositoryName => this.Self.RouteValues.Get<Name>("repository");

    public Time MasterTimeout
    {
      get => this.Q<Time>("master_timeout");
      set => this.Q("master_timeout", (object) value);
    }

    public Time Timeout
    {
      get => this.Q<Time>("timeout");
      set => this.Q("timeout", (object) value);
    }
  }
}
