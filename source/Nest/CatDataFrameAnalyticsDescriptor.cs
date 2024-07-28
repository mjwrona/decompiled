// Decompiled with JetBrains decompiler
// Type: Nest.CatDataFrameAnalyticsDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.CatApi;
using System;

namespace Nest
{
  public class CatDataFrameAnalyticsDescriptor : 
    RequestDescriptorBase<CatDataFrameAnalyticsDescriptor, CatDataFrameAnalyticsRequestParameters, ICatDataFrameAnalyticsRequest>,
    ICatDataFrameAnalyticsRequest,
    IRequest<CatDataFrameAnalyticsRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.CatDataFrameAnalytics;

    public CatDataFrameAnalyticsDescriptor()
    {
    }

    public CatDataFrameAnalyticsDescriptor(Nest.Id id)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional(nameof (id), (IUrlParameter) id)))
    {
    }

    Nest.Id ICatDataFrameAnalyticsRequest.Id => this.Self.RouteValues.Get<Nest.Id>("id");

    public CatDataFrameAnalyticsDescriptor Id(Nest.Id id) => this.Assign<Nest.Id>(id, (Action<ICatDataFrameAnalyticsRequest, Nest.Id>) ((a, v) => a.RouteValues.Optional(nameof (id), (IUrlParameter) v)));

    public CatDataFrameAnalyticsDescriptor AllowNoMatch(bool? allownomatch = true) => this.Qs("allow_no_match", (object) allownomatch);

    public CatDataFrameAnalyticsDescriptor Bytes(Elasticsearch.Net.Bytes? bytes) => this.Qs(nameof (bytes), (object) bytes);

    public CatDataFrameAnalyticsDescriptor Format(string format) => this.Qs(nameof (format), (object) format);

    public CatDataFrameAnalyticsDescriptor Headers(params string[] headers) => this.Qs("h", (object) headers);

    public CatDataFrameAnalyticsDescriptor Help(bool? help = true) => this.Qs(nameof (help), (object) help);

    public CatDataFrameAnalyticsDescriptor SortByColumns(params string[] sortbycolumns) => this.Qs("s", (object) sortbycolumns);

    public CatDataFrameAnalyticsDescriptor Verbose(bool? verbose = true) => this.Qs("v", (object) verbose);
  }
}
