// Decompiled with JetBrains decompiler
// Type: Nest.CatCountRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.CatApi;
using System;
using System.Runtime.Serialization;

namespace Nest
{
  public class CatCountRequest : 
    PlainRequestBase<CatCountRequestParameters>,
    ICatCountRequest,
    IRequest<CatCountRequestParameters>,
    IRequest
  {
    protected ICatCountRequest Self => (ICatCountRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.CatCount;

    public CatCountRequest()
    {
    }

    public CatCountRequest(Indices index)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional(nameof (index), (IUrlParameter) index)))
    {
    }

    [IgnoreDataMember]
    Indices ICatCountRequest.Index => this.Self.RouteValues.Get<Indices>("index");

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

    [Obsolete("Scheduled to be removed in 8.0, Deprecated as of: 7.5.0, reason: Removed from the server as it was never a valid option")]
    public bool? Local
    {
      get => this.Q<bool?>("local");
      set => this.Q("local", (object) value);
    }

    [Obsolete("Scheduled to be removed in 8.0, Deprecated as of: 7.5.0, reason: Removed from the server as it was never a valid option")]
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
