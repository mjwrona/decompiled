// Decompiled with JetBrains decompiler
// Type: Nest.ClearApiKeyCacheRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.SecurityApi;
using System;
using System.Runtime.Serialization;

namespace Nest
{
  public class ClearApiKeyCacheRequest : 
    PlainRequestBase<ClearApiKeyCacheRequestParameters>,
    IClearApiKeyCacheRequest,
    IRequest<ClearApiKeyCacheRequestParameters>,
    IRequest
  {
    protected IClearApiKeyCacheRequest Self => (IClearApiKeyCacheRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.SecurityClearApiKeyCache;

    public ClearApiKeyCacheRequest(Ids ids)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional(nameof (ids), (IUrlParameter) ids)))
    {
    }

    public ClearApiKeyCacheRequest()
    {
    }

    [IgnoreDataMember]
    Ids IClearApiKeyCacheRequest.Ids => this.Self.RouteValues.Get<Ids>("ids");
  }
}
