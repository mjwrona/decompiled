// Decompiled with JetBrains decompiler
// Type: Nest.PutIndexTemplateV2Descriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.IndicesApi;
using Elasticsearch.Net.Utf8Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Nest
{
  public class PutIndexTemplateV2Descriptor : 
    RequestDescriptorBase<PutIndexTemplateV2Descriptor, PutIndexTemplateV2RequestParameters, IPutIndexTemplateV2Request>,
    IPutIndexTemplateV2Request,
    IRequest<PutIndexTemplateV2RequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.IndicesPutTemplateV2;

    public PutIndexTemplateV2Descriptor(Name name)
      : base((Func<RouteValues, RouteValues>) (r => r.Required(nameof (name), (IUrlParameter) name)))
    {
    }

    [SerializationConstructor]
    protected PutIndexTemplateV2Descriptor()
    {
    }

    Name IPutIndexTemplateV2Request.Name => this.Self.RouteValues.Get<Name>("name");

    public PutIndexTemplateV2Descriptor Cause(string cause) => this.Qs(nameof (cause), (object) cause);

    public PutIndexTemplateV2Descriptor Create(bool? create = true) => this.Qs(nameof (create), (object) create);

    public PutIndexTemplateV2Descriptor MasterTimeout(Time mastertimeout) => this.Qs("master_timeout", (object) mastertimeout);

    IEnumerable<string> IPutIndexTemplateV2Request.IndexPatterns { get; set; }

    IEnumerable<string> IPutIndexTemplateV2Request.ComposedOf { get; set; }

    ITemplate IPutIndexTemplateV2Request.Template { get; set; }

    Nest.DataStream IPutIndexTemplateV2Request.DataStream { get; set; }

    int? IPutIndexTemplateV2Request.Priority { get; set; }

    long? IPutIndexTemplateV2Request.Version { get; set; }

    IDictionary<string, object> IPutIndexTemplateV2Request.Meta { get; set; }

    public PutIndexTemplateV2Descriptor IndexPatterns(params string[] patterns) => this.Assign<string[]>(patterns, (Action<IPutIndexTemplateV2Request, string[]>) ((a, v) => a.IndexPatterns = (IEnumerable<string>) v));

    public PutIndexTemplateV2Descriptor IndexPatterns(IEnumerable<string> patterns) => this.Assign<string[]>(patterns != null ? patterns.ToArray<string>() : (string[]) null, (Action<IPutIndexTemplateV2Request, string[]>) ((a, v) => a.IndexPatterns = (IEnumerable<string>) v));

    public PutIndexTemplateV2Descriptor ComposedOf(params string[] composedOf) => this.Assign<string[]>(composedOf, (Action<IPutIndexTemplateV2Request, string[]>) ((a, v) => a.ComposedOf = (IEnumerable<string>) v));

    public PutIndexTemplateV2Descriptor ComposedOf(IEnumerable<string> composedOf) => this.Assign<string[]>(composedOf != null ? composedOf.ToArray<string>() : (string[]) null, (Action<IPutIndexTemplateV2Request, string[]>) ((a, v) => a.ComposedOf = (IEnumerable<string>) v));

    public PutIndexTemplateV2Descriptor Template(Func<TemplateDescriptor, ITemplate> selector) => this.Assign<ITemplate>(selector != null ? selector(new TemplateDescriptor()) : (ITemplate) null, (Action<IPutIndexTemplateV2Request, ITemplate>) ((a, v) => a.Template = v));

    public PutIndexTemplateV2Descriptor DataStream(Nest.DataStream dataStream) => this.Assign<Nest.DataStream>(dataStream, (Action<IPutIndexTemplateV2Request, Nest.DataStream>) ((a, v) => a.DataStream = v));

    public PutIndexTemplateV2Descriptor Priority(int? priority) => this.Assign<int?>(priority, (Action<IPutIndexTemplateV2Request, int?>) ((a, v) => a.Priority = v));

    public PutIndexTemplateV2Descriptor Version(long? version) => this.Assign<long?>(version, (Action<IPutIndexTemplateV2Request, long?>) ((a, v) => a.Version = v));

    public PutIndexTemplateV2Descriptor Meta(
      Func<FluentDictionary<string, object>, FluentDictionary<string, object>> selector)
    {
      return this.Assign<Func<FluentDictionary<string, object>, FluentDictionary<string, object>>>(selector, (Action<IPutIndexTemplateV2Request, Func<FluentDictionary<string, object>, FluentDictionary<string, object>>>) ((a, v) => a.Meta = v != null ? (IDictionary<string, object>) v(new FluentDictionary<string, object>()) : (IDictionary<string, object>) null));
    }
  }
}
