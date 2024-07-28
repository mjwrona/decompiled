// Decompiled with JetBrains decompiler
// Type: Nest.CatDataFrameAnalyticsRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.CatApi;
using System;
using System.Runtime.Serialization;

namespace Nest
{
  public class CatDataFrameAnalyticsRequest : 
    PlainRequestBase<CatDataFrameAnalyticsRequestParameters>,
    ICatDataFrameAnalyticsRequest,
    IRequest<CatDataFrameAnalyticsRequestParameters>,
    IRequest
  {
    protected ICatDataFrameAnalyticsRequest Self => (ICatDataFrameAnalyticsRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.CatDataFrameAnalytics;

    public CatDataFrameAnalyticsRequest()
    {
    }

    public CatDataFrameAnalyticsRequest(Id id)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional(nameof (id), (IUrlParameter) id)))
    {
    }

    [IgnoreDataMember]
    Id ICatDataFrameAnalyticsRequest.Id => this.Self.RouteValues.Get<Id>("id");

    public bool? AllowNoMatch
    {
      get => this.Q<bool?>("allow_no_match");
      set => this.Q("allow_no_match", (object) value);
    }

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
