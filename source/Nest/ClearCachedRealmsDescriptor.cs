// Decompiled with JetBrains decompiler
// Type: Nest.ClearCachedRealmsDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.SecurityApi;
using Elasticsearch.Net.Utf8Json;
using System;

namespace Nest
{
  public class ClearCachedRealmsDescriptor : 
    RequestDescriptorBase<ClearCachedRealmsDescriptor, ClearCachedRealmsRequestParameters, IClearCachedRealmsRequest>,
    IClearCachedRealmsRequest,
    IRequest<ClearCachedRealmsRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.SecurityClearCachedRealms;

    public ClearCachedRealmsDescriptor(Names realms)
      : base((Func<RouteValues, RouteValues>) (r => r.Required(nameof (realms), (IUrlParameter) realms)))
    {
    }

    [SerializationConstructor]
    protected ClearCachedRealmsDescriptor()
    {
    }

    Names IClearCachedRealmsRequest.Realms => this.Self.RouteValues.Get<Names>("realms");

    public ClearCachedRealmsDescriptor Usernames(params string[] usernames) => this.Qs(nameof (usernames), (object) usernames);
  }
}
