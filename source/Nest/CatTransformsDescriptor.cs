// Decompiled with JetBrains decompiler
// Type: Nest.CatTransformsDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.CatApi;
using System;

namespace Nest
{
  public class CatTransformsDescriptor : 
    RequestDescriptorBase<CatTransformsDescriptor, CatTransformsRequestParameters, ICatTransformsRequest>,
    ICatTransformsRequest,
    IRequest<CatTransformsRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.CatTransforms;

    public CatTransformsDescriptor()
    {
    }

    public CatTransformsDescriptor(Id transformId)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional("transform_id", (IUrlParameter) transformId)))
    {
    }

    Id ICatTransformsRequest.TransformId => this.Self.RouteValues.Get<Id>("transform_id");

    public CatTransformsDescriptor TransformId(Id transformId) => this.Assign<Id>(transformId, (Action<ICatTransformsRequest, Id>) ((a, v) => a.RouteValues.Optional("transform_id", (IUrlParameter) v)));

    public CatTransformsDescriptor AllowNoMatch(bool? allownomatch = true) => this.Qs("allow_no_match", (object) allownomatch);

    public CatTransformsDescriptor Format(string format) => this.Qs(nameof (format), (object) format);

    public CatTransformsDescriptor From(int? from) => this.Qs(nameof (from), (object) from);

    public CatTransformsDescriptor Headers(params string[] headers) => this.Qs("h", (object) headers);

    public CatTransformsDescriptor Help(bool? help = true) => this.Qs(nameof (help), (object) help);

    public CatTransformsDescriptor Size(int? size) => this.Qs(nameof (size), (object) size);

    public CatTransformsDescriptor SortByColumns(params string[] sortbycolumns) => this.Qs("s", (object) sortbycolumns);

    public CatTransformsDescriptor Verbose(bool? verbose = true) => this.Qs("v", (object) verbose);
  }
}
