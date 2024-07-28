// Decompiled with JetBrains decompiler
// Type: Nest.GetTransformStatsDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.TransformApi;
using Elasticsearch.Net.Utf8Json;
using System;

namespace Nest
{
  public class GetTransformStatsDescriptor : 
    RequestDescriptorBase<GetTransformStatsDescriptor, GetTransformStatsRequestParameters, IGetTransformStatsRequest>,
    IGetTransformStatsRequest,
    IRequest<GetTransformStatsRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.TransformGetStats;

    public GetTransformStatsDescriptor(Id transformId)
      : base((Func<RouteValues, RouteValues>) (r => r.Required("transform_id", (IUrlParameter) transformId)))
    {
    }

    [SerializationConstructor]
    protected GetTransformStatsDescriptor()
    {
    }

    Id IGetTransformStatsRequest.TransformId => this.Self.RouteValues.Get<Id>("transform_id");

    public GetTransformStatsDescriptor AllowNoMatch(bool? allownomatch = true) => this.Qs("allow_no_match", (object) allownomatch);

    public GetTransformStatsDescriptor From(long? from) => this.Qs(nameof (from), (object) from);

    public GetTransformStatsDescriptor Size(long? size) => this.Qs(nameof (size), (object) size);
  }
}
