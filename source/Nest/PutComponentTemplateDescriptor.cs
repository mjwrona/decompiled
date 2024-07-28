// Decompiled with JetBrains decompiler
// Type: Nest.PutComponentTemplateDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.ClusterApi;
using Elasticsearch.Net.Utf8Json;
using System;
using System.Collections.Generic;

namespace Nest
{
  public class PutComponentTemplateDescriptor : 
    RequestDescriptorBase<PutComponentTemplateDescriptor, PutComponentTemplateRequestParameters, IPutComponentTemplateRequest>,
    IPutComponentTemplateRequest,
    IRequest<PutComponentTemplateRequestParameters>,
    IRequest
  {
    ITemplate IPutComponentTemplateRequest.Template { get; set; }

    long? IPutComponentTemplateRequest.Version { get; set; }

    IDictionary<string, object> IPutComponentTemplateRequest.Meta { get; set; }

    public PutComponentTemplateDescriptor Template(Func<TemplateDescriptor, ITemplate> selector) => this.Assign<ITemplate>(selector != null ? selector(new TemplateDescriptor()) : (ITemplate) null, (Action<IPutComponentTemplateRequest, ITemplate>) ((a, v) => a.Template = v));

    public PutComponentTemplateDescriptor Version(long? version) => this.Assign<long?>(version, (Action<IPutComponentTemplateRequest, long?>) ((a, v) => a.Version = v));

    public PutComponentTemplateDescriptor Meta(
      Func<FluentDictionary<string, object>, FluentDictionary<string, object>> selector)
    {
      return this.Assign<Func<FluentDictionary<string, object>, FluentDictionary<string, object>>>(selector, (Action<IPutComponentTemplateRequest, Func<FluentDictionary<string, object>, FluentDictionary<string, object>>>) ((a, v) => a.Meta = v != null ? (IDictionary<string, object>) v(new FluentDictionary<string, object>()) : (IDictionary<string, object>) null));
    }

    internal override ApiUrls ApiUrls => ApiUrlsLookups.ClusterPutComponentTemplate;

    public PutComponentTemplateDescriptor(Name name)
      : base((Func<RouteValues, RouteValues>) (r => r.Required(nameof (name), (IUrlParameter) name)))
    {
    }

    [SerializationConstructor]
    protected PutComponentTemplateDescriptor()
    {
    }

    Name IPutComponentTemplateRequest.Name => this.Self.RouteValues.Get<Name>("name");

    public PutComponentTemplateDescriptor Create(bool? create = true) => this.Qs(nameof (create), (object) create);

    public PutComponentTemplateDescriptor MasterTimeout(Time mastertimeout) => this.Qs("master_timeout", (object) mastertimeout);

    public PutComponentTemplateDescriptor Timeout(Time timeout) => this.Qs(nameof (timeout), (object) timeout);
  }
}
