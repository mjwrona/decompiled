// Decompiled with JetBrains decompiler
// Type: Nest.DeprecationInfoRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.MigrationApi;
using System;
using System.Runtime.Serialization;

namespace Nest
{
  public class DeprecationInfoRequest : 
    PlainRequestBase<DeprecationInfoRequestParameters>,
    IDeprecationInfoRequest,
    IRequest<DeprecationInfoRequestParameters>,
    IRequest
  {
    protected IDeprecationInfoRequest Self => (IDeprecationInfoRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.MigrationDeprecationInfo;

    public DeprecationInfoRequest()
    {
    }

    public DeprecationInfoRequest(IndexName index)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional(nameof (index), (IUrlParameter) index)))
    {
    }

    [IgnoreDataMember]
    IndexName IDeprecationInfoRequest.Index => this.Self.RouteValues.Get<IndexName>("index");
  }
}
