// Decompiled with JetBrains decompiler
// Type: Nest.ClearApiKeyCacheDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.SecurityApi;
using System;

namespace Nest
{
  public class ClearApiKeyCacheDescriptor : 
    RequestDescriptorBase<ClearApiKeyCacheDescriptor, ClearApiKeyCacheRequestParameters, IClearApiKeyCacheRequest>,
    IClearApiKeyCacheRequest,
    IRequest<ClearApiKeyCacheRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.SecurityClearApiKeyCache;

    public ClearApiKeyCacheDescriptor(Nest.Ids ids)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional(nameof (ids), (IUrlParameter) ids)))
    {
    }

    public ClearApiKeyCacheDescriptor()
    {
    }

    Nest.Ids IClearApiKeyCacheRequest.Ids => this.Self.RouteValues.Get<Nest.Ids>("ids");

    public ClearApiKeyCacheDescriptor Ids(Nest.Ids ids) => this.Assign<Nest.Ids>(ids, (Action<IClearApiKeyCacheRequest, Nest.Ids>) ((a, v) => a.RouteValues.Optional(nameof (ids), (IUrlParameter) v)));
  }
}
