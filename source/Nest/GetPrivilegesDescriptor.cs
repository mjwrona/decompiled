// Decompiled with JetBrains decompiler
// Type: Nest.GetPrivilegesDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.SecurityApi;
using System;

namespace Nest
{
  public class GetPrivilegesDescriptor : 
    RequestDescriptorBase<GetPrivilegesDescriptor, GetPrivilegesRequestParameters, IGetPrivilegesRequest>,
    IGetPrivilegesRequest,
    IRequest<GetPrivilegesRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.SecurityGetPrivileges;

    public GetPrivilegesDescriptor()
    {
    }

    public GetPrivilegesDescriptor(Nest.Name application)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional(nameof (application), (IUrlParameter) application)))
    {
    }

    public GetPrivilegesDescriptor(Nest.Name application, Nest.Name name)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional(nameof (application), (IUrlParameter) application).Optional(nameof (name), (IUrlParameter) name)))
    {
    }

    Nest.Name IGetPrivilegesRequest.Application => this.Self.RouteValues.Get<Nest.Name>("application");

    Nest.Name IGetPrivilegesRequest.Name => this.Self.RouteValues.Get<Nest.Name>("name");

    public GetPrivilegesDescriptor Application(Nest.Name application) => this.Assign<Nest.Name>(application, (Action<IGetPrivilegesRequest, Nest.Name>) ((a, v) => a.RouteValues.Optional(nameof (application), (IUrlParameter) v)));

    public GetPrivilegesDescriptor Name(Nest.Name name) => this.Assign<Nest.Name>(name, (Action<IGetPrivilegesRequest, Nest.Name>) ((a, v) => a.RouteValues.Optional(nameof (name), (IUrlParameter) v)));
  }
}
