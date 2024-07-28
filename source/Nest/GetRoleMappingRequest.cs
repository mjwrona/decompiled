// Decompiled with JetBrains decompiler
// Type: Nest.GetRoleMappingRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.SecurityApi;
using System;
using System.Runtime.Serialization;

namespace Nest
{
  public class GetRoleMappingRequest : 
    PlainRequestBase<GetRoleMappingRequestParameters>,
    IGetRoleMappingRequest,
    IRequest<GetRoleMappingRequestParameters>,
    IRequest
  {
    protected IGetRoleMappingRequest Self => (IGetRoleMappingRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.SecurityGetRoleMapping;

    public GetRoleMappingRequest(Names name)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional(nameof (name), (IUrlParameter) name)))
    {
    }

    public GetRoleMappingRequest()
    {
    }

    [IgnoreDataMember]
    Names IGetRoleMappingRequest.Name => this.Self.RouteValues.Get<Names>("name");
  }
}
