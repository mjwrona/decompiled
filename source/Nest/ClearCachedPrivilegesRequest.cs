// Decompiled with JetBrains decompiler
// Type: Nest.ClearCachedPrivilegesRequest
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
  public class ClearCachedPrivilegesRequest : 
    PlainRequestBase<ClearCachedPrivilegesRequestParameters>,
    IClearCachedPrivilegesRequest,
    IRequest<ClearCachedPrivilegesRequestParameters>,
    IRequest
  {
    protected IClearCachedPrivilegesRequest Self => (IClearCachedPrivilegesRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.SecurityClearCachedPrivileges;

    public ClearCachedPrivilegesRequest(Names application)
      : base((Func<RouteValues, RouteValues>) (r => r.Required(nameof (application), (IUrlParameter) application)))
    {
    }

    [SerializationConstructor]
    protected ClearCachedPrivilegesRequest()
    {
    }

    [IgnoreDataMember]
    Names IClearCachedPrivilegesRequest.Application => this.Self.RouteValues.Get<Names>("application");
  }
}
