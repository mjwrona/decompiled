// Decompiled with JetBrains decompiler
// Type: Nest.CatSegmentsRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.CatApi;
using System;
using System.Runtime.Serialization;

namespace Nest
{
  public class CatSegmentsRequest : 
    PlainRequestBase<CatSegmentsRequestParameters>,
    ICatSegmentsRequest,
    IRequest<CatSegmentsRequestParameters>,
    IRequest
  {
    protected ICatSegmentsRequest Self => (ICatSegmentsRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.CatSegments;

    public CatSegmentsRequest()
    {
    }

    public CatSegmentsRequest(Indices index)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional(nameof (index), (IUrlParameter) index)))
    {
    }

    [IgnoreDataMember]
    Indices ICatSegmentsRequest.Index => this.Self.RouteValues.Get<Indices>("index");

    public Elasticsearch.Net.Bytes? Bytes
    {
      get => this.Q<Elasticsearch.Net.Bytes?>("bytes");
      set => this.Q("bytes", (object) value);
    }

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
