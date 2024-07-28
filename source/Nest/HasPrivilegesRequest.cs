// Decompiled with JetBrains decompiler
// Type: Nest.HasPrivilegesRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.SecurityApi;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  public class HasPrivilegesRequest : 
    PlainRequestBase<HasPrivilegesRequestParameters>,
    IHasPrivilegesRequest,
    IRequest<HasPrivilegesRequestParameters>,
    IRequest
  {
    protected IHasPrivilegesRequest Self => (IHasPrivilegesRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.SecurityHasPrivileges;

    public HasPrivilegesRequest()
    {
    }

    public HasPrivilegesRequest(Name user)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional(nameof (user), (IUrlParameter) user)))
    {
    }

    [IgnoreDataMember]
    Name IHasPrivilegesRequest.User => this.Self.RouteValues.Get<Name>("user");

    public IEnumerable<IApplicationPrivilegesCheck> Application { get; set; }

    public IEnumerable<string> Cluster { get; set; }

    public IEnumerable<IndexPrivilegesCheck> Index { get; set; }
  }
}
