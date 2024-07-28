// Decompiled with JetBrains decompiler
// Type: Nest.GetRoleDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.SecurityApi;
using System;

namespace Nest
{
  public class GetRoleDescriptor : 
    RequestDescriptorBase<GetRoleDescriptor, GetRoleRequestParameters, IGetRoleRequest>,
    IGetRoleRequest,
    IRequest<GetRoleRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.SecurityGetRole;

    public GetRoleDescriptor(Names name)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional(nameof (name), (IUrlParameter) name)))
    {
    }

    public GetRoleDescriptor()
    {
    }

    Names IGetRoleRequest.Name => this.Self.RouteValues.Get<Names>("name");

    public GetRoleDescriptor Name(Names name) => this.Assign<Names>(name, (Action<IGetRoleRequest, Names>) ((a, v) => a.RouteValues.Optional(nameof (name), (IUrlParameter) v)));
  }
}
