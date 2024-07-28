// Decompiled with JetBrains decompiler
// Type: Nest.PutRoleMappingDescriptor
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
  public class PutRoleMappingDescriptor : 
    RequestDescriptorBase<PutRoleMappingDescriptor, PutRoleMappingRequestParameters, IPutRoleMappingRequest>,
    IPutRoleMappingRequest,
    IRequest<PutRoleMappingRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.SecurityPutRoleMapping;

    public PutRoleMappingDescriptor(Name name)
      : base((Func<RouteValues, RouteValues>) (r => r.Required(nameof (name), (IUrlParameter) name)))
    {
    }

    [SerializationConstructor]
    protected PutRoleMappingDescriptor()
    {
    }

    Name IPutRoleMappingRequest.Name => this.Self.RouteValues.Get<Name>("name");

    public PutRoleMappingDescriptor Refresh(Elasticsearch.Net.Refresh? refresh) => this.Qs(nameof (refresh), (object) refresh);

    bool? IPutRoleMappingRequest.Enabled { get; set; }

    IDictionary<string, object> IPutRoleMappingRequest.Metadata { get; set; }

    IEnumerable<string> IPutRoleMappingRequest.Roles { get; set; }

    RoleMappingRuleBase IPutRoleMappingRequest.Rules { get; set; }

    IEnumerable<string> IPutRoleMappingRequest.RunAs { get; set; }

    public PutRoleMappingDescriptor Roles(RoleMappingRuleBase rules) => this.Assign<RoleMappingRuleBase>(rules, (Action<IPutRoleMappingRequest, RoleMappingRuleBase>) ((a, v) => a.Rules = v));

    public PutRoleMappingDescriptor Roles(IEnumerable<string> roles) => this.Assign<IEnumerable<string>>(roles, (Action<IPutRoleMappingRequest, IEnumerable<string>>) ((a, v) => a.Roles = v));

    public PutRoleMappingDescriptor Roles(params string[] roles) => this.Assign<string[]>(roles, (Action<IPutRoleMappingRequest, string[]>) ((a, v) => a.Roles = (IEnumerable<string>) v));

    public PutRoleMappingDescriptor Rules(
      Func<RoleMappingRuleDescriptor, RoleMappingRuleBase> selector)
    {
      return this.Assign<Func<RoleMappingRuleDescriptor, RoleMappingRuleBase>>(selector, (Action<IPutRoleMappingRequest, Func<RoleMappingRuleDescriptor, RoleMappingRuleBase>>) ((a, v) => a.Rules = v != null ? v(new RoleMappingRuleDescriptor()) : (RoleMappingRuleBase) null));
    }

    public PutRoleMappingDescriptor Rules(RoleMappingRuleBase rules) => this.Assign<RoleMappingRuleBase>(rules, (Action<IPutRoleMappingRequest, RoleMappingRuleBase>) ((a, v) => a.Rules = v));

    public PutRoleMappingDescriptor RunAs(IEnumerable<string> users) => this.Assign<IEnumerable<string>>(users, (Action<IPutRoleMappingRequest, IEnumerable<string>>) ((a, v) => a.RunAs = v));

    public PutRoleMappingDescriptor RunAs(params string[] users) => this.Assign<string[]>(users, (Action<IPutRoleMappingRequest, string[]>) ((a, v) => a.RunAs = (IEnumerable<string>) v));

    public PutRoleMappingDescriptor Enabled(bool? enabled = true) => this.Assign<bool?>(enabled, (Action<IPutRoleMappingRequest, bool?>) ((a, v) => a.Enabled = v));

    public PutRoleMappingDescriptor Metadata(IDictionary<string, object> metadata) => this.Assign<IDictionary<string, object>>(metadata, (Action<IPutRoleMappingRequest, IDictionary<string, object>>) ((a, v) => a.Metadata = v));

    public PutRoleMappingDescriptor Metadata(
      Func<FluentDictionary<string, object>, FluentDictionary<string, object>> selector)
    {
      return this.Assign<Func<FluentDictionary<string, object>, FluentDictionary<string, object>>>(selector, (Action<IPutRoleMappingRequest, Func<FluentDictionary<string, object>, FluentDictionary<string, object>>>) ((a, v) => a.Metadata = v != null ? (IDictionary<string, object>) v(new FluentDictionary<string, object>()) : (IDictionary<string, object>) null));
    }
  }
}
