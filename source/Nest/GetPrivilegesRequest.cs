// Decompiled with JetBrains decompiler
// Type: Nest.GetPrivilegesRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.SecurityApi;
using System;
using System.Runtime.Serialization;

namespace Nest
{
  public class GetPrivilegesRequest : 
    PlainRequestBase<GetPrivilegesRequestParameters>,
    IGetPrivilegesRequest,
    IRequest<GetPrivilegesRequestParameters>,
    IRequest
  {
    protected IGetPrivilegesRequest Self => (IGetPrivilegesRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.SecurityGetPrivileges;

    public GetPrivilegesRequest()
    {
    }

    public GetPrivilegesRequest(Name application)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional(nameof (application), (IUrlParameter) application)))
    {
    }

    public GetPrivilegesRequest(Name application, Name name)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional(nameof (application), (IUrlParameter) application).Optional(nameof (name), (IUrlParameter) name)))
    {
    }

    [IgnoreDataMember]
    Name IGetPrivilegesRequest.Application => this.Self.RouteValues.Get<Name>("application");

    [IgnoreDataMember]
    Name IGetPrivilegesRequest.Name => this.Self.RouteValues.Get<Name>("name");
  }
}
