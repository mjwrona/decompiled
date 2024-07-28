// Decompiled with JetBrains decompiler
// Type: Nest.DeletePrivilegesRequest
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
  public class DeletePrivilegesRequest : 
    PlainRequestBase<DeletePrivilegesRequestParameters>,
    IDeletePrivilegesRequest,
    IRequest<DeletePrivilegesRequestParameters>,
    IRequest
  {
    protected IDeletePrivilegesRequest Self => (IDeletePrivilegesRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.SecurityDeletePrivileges;

    public DeletePrivilegesRequest(Name application, Name name)
      : base((Func<RouteValues, RouteValues>) (r => r.Required(nameof (application), (IUrlParameter) application).Required(nameof (name), (IUrlParameter) name)))
    {
    }

    [SerializationConstructor]
    protected DeletePrivilegesRequest()
    {
    }

    [IgnoreDataMember]
    Name IDeletePrivilegesRequest.Application => this.Self.RouteValues.Get<Name>("application");

    [IgnoreDataMember]
    Name IDeletePrivilegesRequest.Name => this.Self.RouteValues.Get<Name>("name");

    public Elasticsearch.Net.Refresh? Refresh
    {
      get => this.Q<Elasticsearch.Net.Refresh?>("refresh");
      set => this.Q("refresh", (object) value);
    }
  }
}
