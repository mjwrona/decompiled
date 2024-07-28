// Decompiled with JetBrains decompiler
// Type: Nest.ClearCachedRolesDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.SecurityApi;
using Elasticsearch.Net.Utf8Json;
using System;

namespace Nest
{
  public class ClearCachedRolesDescriptor : 
    RequestDescriptorBase<ClearCachedRolesDescriptor, ClearCachedRolesRequestParameters, IClearCachedRolesRequest>,
    IClearCachedRolesRequest,
    IRequest<ClearCachedRolesRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.SecurityClearCachedRoles;

    public ClearCachedRolesDescriptor(Names name)
      : base((Func<RouteValues, RouteValues>) (r => r.Required(nameof (name), (IUrlParameter) name)))
    {
    }

    [SerializationConstructor]
    protected ClearCachedRolesDescriptor()
    {
    }

    Names IClearCachedRolesRequest.Name => this.Self.RouteValues.Get<Names>("name");
  }
}
