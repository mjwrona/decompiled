// Decompiled with JetBrains decompiler
// Type: Nest.HasPrivilegesDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.SecurityApi;
using System;
using System.Collections.Generic;

namespace Nest
{
  public class HasPrivilegesDescriptor : 
    RequestDescriptorBase<HasPrivilegesDescriptor, HasPrivilegesRequestParameters, IHasPrivilegesRequest>,
    IHasPrivilegesRequest,
    IRequest<HasPrivilegesRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.SecurityHasPrivileges;

    public HasPrivilegesDescriptor()
    {
    }

    public HasPrivilegesDescriptor(Name user)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional(nameof (user), (IUrlParameter) user)))
    {
    }

    Name IHasPrivilegesRequest.User => this.Self.RouteValues.Get<Name>("user");

    public HasPrivilegesDescriptor User(Name user) => this.Assign<Name>(user, (Action<IHasPrivilegesRequest, Name>) ((a, v) => a.RouteValues.Optional(nameof (user), (IUrlParameter) v)));

    IEnumerable<IApplicationPrivilegesCheck> IHasPrivilegesRequest.Application { get; set; }

    IEnumerable<string> IHasPrivilegesRequest.Cluster { get; set; }

    IEnumerable<IndexPrivilegesCheck> IHasPrivilegesRequest.Index { get; set; }

    public HasPrivilegesDescriptor Cluster(IEnumerable<string> cluster) => this.Assign<IEnumerable<string>>(cluster, (Action<IHasPrivilegesRequest, IEnumerable<string>>) ((a, v) => a.Cluster = v));

    public HasPrivilegesDescriptor Cluster(params string[] cluster) => this.Assign<string[]>(cluster, (Action<IHasPrivilegesRequest, string[]>) ((a, v) => a.Cluster = (IEnumerable<string>) v));

    public HasPrivilegesDescriptor Indices(
      Func<ApplicationPrivilegesChecksDescriptor, IPromise<List<IApplicationPrivilegesCheck>>> selector)
    {
      return this.Assign<Func<ApplicationPrivilegesChecksDescriptor, IPromise<List<IApplicationPrivilegesCheck>>>>(selector, (Action<IHasPrivilegesRequest, Func<ApplicationPrivilegesChecksDescriptor, IPromise<List<IApplicationPrivilegesCheck>>>>) ((a, v) => a.Application = v != null ? (IEnumerable<IApplicationPrivilegesCheck>) v(new ApplicationPrivilegesChecksDescriptor())?.Value : (IEnumerable<IApplicationPrivilegesCheck>) null));
    }

    public HasPrivilegesDescriptor Applications(
      Func<ApplicationPrivilegesChecksDescriptor, IPromise<List<IApplicationPrivilegesCheck>>> selector)
    {
      return this.Assign<Func<ApplicationPrivilegesChecksDescriptor, IPromise<List<IApplicationPrivilegesCheck>>>>(selector, (Action<IHasPrivilegesRequest, Func<ApplicationPrivilegesChecksDescriptor, IPromise<List<IApplicationPrivilegesCheck>>>>) ((a, v) => a.Application = v != null ? (IEnumerable<IApplicationPrivilegesCheck>) v(new ApplicationPrivilegesChecksDescriptor())?.Value : (IEnumerable<IApplicationPrivilegesCheck>) null));
    }
  }
}
