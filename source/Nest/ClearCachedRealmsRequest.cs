// Decompiled with JetBrains decompiler
// Type: Nest.ClearCachedRealmsRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.SecurityApi;
using Elasticsearch.Net.Utf8Json;
using System;
using System.Runtime.Serialization;

namespace Nest
{
  public class ClearCachedRealmsRequest : 
    PlainRequestBase<ClearCachedRealmsRequestParameters>,
    IClearCachedRealmsRequest,
    IRequest<ClearCachedRealmsRequestParameters>,
    IRequest
  {
    protected IClearCachedRealmsRequest Self => (IClearCachedRealmsRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.SecurityClearCachedRealms;

    public ClearCachedRealmsRequest(Names realms)
      : base((Func<RouteValues, RouteValues>) (r => r.Required(nameof (realms), (IUrlParameter) realms)))
    {
    }

    [SerializationConstructor]
    protected ClearCachedRealmsRequest()
    {
    }

    [IgnoreDataMember]
    Names IClearCachedRealmsRequest.Realms => this.Self.RouteValues.Get<Names>("realms");

    public string[] Usernames
    {
      get => this.Q<string[]>("usernames");
      set => this.Q("usernames", (object) value);
    }
  }
}
