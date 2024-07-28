// Decompiled with JetBrains decompiler
// Type: Nest.PutRoleDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.SecurityApi;
using Elasticsearch.Net.Utf8Json;
using System;
using System.Collections.Generic;

namespace Nest
{
  public class PutRoleDescriptor : 
    RequestDescriptorBase<PutRoleDescriptor, PutRoleRequestParameters, IPutRoleRequest>,
    IPutRoleRequest,
    IRequest<PutRoleRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.SecurityPutRole;

    public PutRoleDescriptor(Name name)
      : base((Func<RouteValues, RouteValues>) (r => r.Required(nameof (name), (IUrlParameter) name)))
    {
    }

    [SerializationConstructor]
    protected PutRoleDescriptor()
    {
    }

    Name IPutRoleRequest.Name => this.Self.RouteValues.Get<Name>("name");

    public PutRoleDescriptor Refresh(Elasticsearch.Net.Refresh? refresh) => this.Qs(nameof (refresh), (object) refresh);

    IEnumerable<IApplicationPrivileges> IPutRoleRequest.Applications { get; set; }

    IEnumerable<string> IPutRoleRequest.Cluster { get; set; }

    IDictionary<string, object> IPutRoleRequest.Global { get; set; }

    IEnumerable<IIndicesPrivileges> IPutRoleRequest.Indices { get; set; }

    IDictionary<string, object> IPutRoleRequest.Metadata { get; set; }

    IEnumerable<string> IPutRoleRequest.RunAs { get; set; }

    public PutRoleDescriptor Applications(IEnumerable<IApplicationPrivileges> privileges) => this.Assign<List<IApplicationPrivileges>>(privileges.ToListOrNullIfEmpty<IApplicationPrivileges>(), (Action<IPutRoleRequest, List<IApplicationPrivileges>>) ((a, v) => a.Applications = (IEnumerable<IApplicationPrivileges>) v));

    public PutRoleDescriptor Applications(
      Func<ApplicationPrivilegesListDescriptor, IPromise<IList<IApplicationPrivileges>>> selector)
    {
      return this.Assign<Func<ApplicationPrivilegesListDescriptor, IPromise<IList<IApplicationPrivileges>>>>(selector, (Action<IPutRoleRequest, Func<ApplicationPrivilegesListDescriptor, IPromise<IList<IApplicationPrivileges>>>>) ((a, v) => a.Applications = v != null ? (IEnumerable<IApplicationPrivileges>) v(new ApplicationPrivilegesListDescriptor())?.Value : (IEnumerable<IApplicationPrivileges>) null));
    }

    public PutRoleDescriptor Cluster(IEnumerable<string> clusters) => this.Assign<IEnumerable<string>>(clusters, (Action<IPutRoleRequest, IEnumerable<string>>) ((a, v) => a.Cluster = v));

    public PutRoleDescriptor Cluster(params string[] clusters) => this.Assign<string[]>(clusters, (Action<IPutRoleRequest, string[]>) ((a, v) => a.Cluster = (IEnumerable<string>) v));

    public PutRoleDescriptor Global(IDictionary<string, object> global) => this.Assign<IDictionary<string, object>>(global, (Action<IPutRoleRequest, IDictionary<string, object>>) ((a, v) => a.Global = v));

    public PutRoleDescriptor Global(
      Func<FluentDictionary<string, object>, FluentDictionary<string, object>> selector)
    {
      return this.Assign<Func<FluentDictionary<string, object>, FluentDictionary<string, object>>>(selector, (Action<IPutRoleRequest, Func<FluentDictionary<string, object>, FluentDictionary<string, object>>>) ((a, v) => a.Global = v != null ? (IDictionary<string, object>) v(new FluentDictionary<string, object>()) : (IDictionary<string, object>) null));
    }

    public PutRoleDescriptor RunAs(IEnumerable<string> users) => this.Assign<IEnumerable<string>>(users, (Action<IPutRoleRequest, IEnumerable<string>>) ((a, v) => a.RunAs = v));

    public PutRoleDescriptor RunAs(params string[] users) => this.Assign<string[]>(users, (Action<IPutRoleRequest, string[]>) ((a, v) => a.RunAs = (IEnumerable<string>) v));

    public PutRoleDescriptor Indices(IEnumerable<IIndicesPrivileges> privileges) => this.Assign<List<IIndicesPrivileges>>(privileges.ToListOrNullIfEmpty<IIndicesPrivileges>(), (Action<IPutRoleRequest, List<IIndicesPrivileges>>) ((a, v) => a.Indices = (IEnumerable<IIndicesPrivileges>) v));

    public PutRoleDescriptor Indices(
      Func<IndicesPrivilegesDescriptor, IPromise<IList<IIndicesPrivileges>>> selector)
    {
      return this.Assign<Func<IndicesPrivilegesDescriptor, IPromise<IList<IIndicesPrivileges>>>>(selector, (Action<IPutRoleRequest, Func<IndicesPrivilegesDescriptor, IPromise<IList<IIndicesPrivileges>>>>) ((a, v) => a.Indices = v != null ? (IEnumerable<IIndicesPrivileges>) v(new IndicesPrivilegesDescriptor())?.Value : (IEnumerable<IIndicesPrivileges>) null));
    }

    public PutRoleDescriptor Metadata(IDictionary<string, object> metadata) => this.Assign<IDictionary<string, object>>(metadata, (Action<IPutRoleRequest, IDictionary<string, object>>) ((a, v) => a.Metadata = v));

    public PutRoleDescriptor Metadata(
      Func<FluentDictionary<string, object>, FluentDictionary<string, object>> selector)
    {
      return this.Assign<Func<FluentDictionary<string, object>, FluentDictionary<string, object>>>(selector, (Action<IPutRoleRequest, Func<FluentDictionary<string, object>, FluentDictionary<string, object>>>) ((a, v) => a.Metadata = v != null ? (IDictionary<string, object>) v(new FluentDictionary<string, object>()) : (IDictionary<string, object>) null));
    }
  }
}
