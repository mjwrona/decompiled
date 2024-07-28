// Decompiled with JetBrains decompiler
// Type: Nest.CatSnapshotsRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.CatApi;
using System;
using System.Runtime.Serialization;

namespace Nest
{
  public class CatSnapshotsRequest : 
    PlainRequestBase<CatSnapshotsRequestParameters>,
    ICatSnapshotsRequest,
    IRequest<CatSnapshotsRequestParameters>,
    IRequest
  {
    protected ICatSnapshotsRequest Self => (ICatSnapshotsRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.CatSnapshots;

    public CatSnapshotsRequest()
    {
    }

    public CatSnapshotsRequest(Names repository)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional(nameof (repository), (IUrlParameter) repository)))
    {
    }

    [IgnoreDataMember]
    Names ICatSnapshotsRequest.RepositoryName => this.Self.RouteValues.Get<Names>("repository");

    public string Format
    {
      get => this.Q<string>("format");
      set
      {
        this.Q("format", (object) value);
        this.SetAcceptHeader(value);
      }
    }

    public string[] Headers
    {
      get => this.Q<string[]>("h");
      set => this.Q("h", (object) value);
    }

    public bool? Help
    {
      get => this.Q<bool?>("help");
      set => this.Q("help", (object) value);
    }

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

    public string[] SortByColumns
    {
      get => this.Q<string[]>("s");
      set => this.Q("s", (object) value);
    }

    public bool? Verbose
    {
      get => this.Q<bool?>("v");
      set => this.Q("v", (object) value);
    }
  }
}
