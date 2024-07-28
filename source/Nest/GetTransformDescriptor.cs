// Decompiled with JetBrains decompiler
// Type: Nest.GetTransformDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.TransformApi;
using System;

namespace Nest
{
  public class GetTransformDescriptor : 
    RequestDescriptorBase<GetTransformDescriptor, GetTransformRequestParameters, IGetTransformRequest>,
    IGetTransformRequest,
    IRequest<GetTransformRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.TransformGet;

    public GetTransformDescriptor(Id transformId)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional("transform_id", (IUrlParameter) transformId)))
    {
    }

    public GetTransformDescriptor()
    {
    }

    Id IGetTransformRequest.TransformId => this.Self.RouteValues.Get<Id>("transform_id");

    public GetTransformDescriptor TransformId(Id transformId) => this.Assign<Id>(transformId, (Action<IGetTransformRequest, Id>) ((a, v) => a.RouteValues.Optional("transform_id", (IUrlParameter) v)));

    public GetTransformDescriptor AllowNoMatch(bool? allownomatch = true) => this.Qs("allow_no_match", (object) allownomatch);

    public GetTransformDescriptor ExcludeGenerated(bool? excludegenerated = true) => this.Qs("exclude_generated", (object) excludegenerated);

    public GetTransformDescriptor From(int? from) => this.Qs(nameof (from), (object) from);

    public GetTransformDescriptor Size(int? size) => this.Qs(nameof (size), (object) size);
  }
}
