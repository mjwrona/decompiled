// Decompiled with JetBrains decompiler
// Type: Nest.ClearCachedRolesRequest
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
  public class ClearCachedRolesRequest : 
    PlainRequestBase<ClearCachedRolesRequestParameters>,
    IClearCachedRolesRequest,
    IRequest<ClearCachedRolesRequestParameters>,
    IRequest
  {
    protected IClearCachedRolesRequest Self => (IClearCachedRolesRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.SecurityClearCachedRoles;

    public ClearCachedRolesRequest(Names name)
      : base((Func<RouteValues, RouteValues>) (r => r.Required(nameof (name), (IUrlParameter) name)))
    {
    }

    [SerializationConstructor]
    protected ClearCachedRolesRequest()
    {
    }

    [IgnoreDataMember]
    Names IClearCachedRolesRequest.Name => this.Self.RouteValues.Get<Names>("name");
  }
}
