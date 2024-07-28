// Decompiled with JetBrains decompiler
// Type: Nest.ClearCachedPrivilegesDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.SecurityApi;
using Elasticsearch.Net.Utf8Json;
using System;

namespace Nest
{
  public class ClearCachedPrivilegesDescriptor : 
    RequestDescriptorBase<ClearCachedPrivilegesDescriptor, ClearCachedPrivilegesRequestParameters, IClearCachedPrivilegesRequest>,
    IClearCachedPrivilegesRequest,
    IRequest<ClearCachedPrivilegesRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.SecurityClearCachedPrivileges;

    public ClearCachedPrivilegesDescriptor(Names application)
      : base((Func<RouteValues, RouteValues>) (r => r.Required(nameof (application), (IUrlParameter) application)))
    {
    }

    [SerializationConstructor]
    protected ClearCachedPrivilegesDescriptor()
    {
    }

    Names IClearCachedPrivilegesRequest.Application => this.Self.RouteValues.Get<Names>("application");
  }
}
