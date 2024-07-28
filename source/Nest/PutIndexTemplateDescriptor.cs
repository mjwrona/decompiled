// Decompiled with JetBrains decompiler
// Type: Nest.PutIndexTemplateDescriptor
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
  public class PutIndexTemplateDescriptor : 
    RequestDescriptorBase<PutIndexTemplateDescriptor, PutIndexTemplateRequestParameters, IPutIndexTemplateRequest>,
    IPutIndexTemplateRequest,
    ITemplateMapping,
    IRequest<PutIndexTemplateRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.IndicesPutTemplate;

    public PutIndexTemplateDescriptor(Name name)
      : base((Func<RouteValues, RouteValues>) (r => r.Required(nameof (name), (IUrlParameter) name)))
    {
    }

    [SerializationConstructor]
    protected PutIndexTemplateDescriptor()
    {
    }

    Name IPutIndexTemplateRequest.Name => this.Self.RouteValues.Get<Name>("name");

    public PutIndexTemplateDescriptor Create(bool? create = true) => this.Qs(nameof (create), (object) create);

    [Obsolete("Scheduled to be removed in 8.0, Deprecated as of: 7.7.0, reason: Removed from the server as it was never a valid option")]
    public PutIndexTemplateDescriptor FlatSettings(bool? flatsettings = true) => this.Qs("flat_settings", (object) flatsettings);

    public PutIndexTemplateDescriptor IncludeTypeName(bool? includetypename = true) => this.Qs("include_type_name", (object) includetypename);

    public PutIndexTemplateDescriptor MasterTimeout(Time mastertimeout) => this.Qs("master_timeout", (object) mastertimeout);

    [Obsolete("Scheduled to be removed in 8.0, Deprecated as of: 7.7.0, reason: Removed from the server as it was never a valid option")]
    public PutIndexTemplateDescriptor Timeout(Time timeout) => this.Qs(nameof (timeout), (object) timeout);

    IAliases ITemplateMapping.Aliases { get; set; }

    IReadOnlyCollection<string> ITemplateMapping.IndexPatterns { get; set; }

    ITypeMapping ITemplateMapping.Mappings { get; set; }

    int? ITemplateMapping.Order { get; set; }

    IIndexSettings ITemplateMapping.Settings { get; set; }

    int? ITemplateMapping.Version { get; set; }

    public PutIndexTemplateDescriptor Order(int? order) => this.Assign<int?>(order, (Action<IPutIndexTemplateRequest, int?>) ((a, v) => a.Order = v));

    public PutIndexTemplateDescriptor Version(int? version) => this.Assign<int?>(version, (Action<IPutIndexTemplateRequest, int?>) ((a, v) => a.Version = v));

    public PutIndexTemplateDescriptor IndexPatterns(params string[] patterns) => this.Assign<string[]>(patterns, (Action<IPutIndexTemplateRequest, string[]>) ((a, v) => a.IndexPatterns = (IReadOnlyCollection<string>) v));

    public PutIndexTemplateDescriptor IndexPatterns(IEnumerable<string> patterns) => this.Assign<string[]>(patterns != null ? patterns.ToArray<string>() : (string[]) null, (Action<IPutIndexTemplateRequest, string[]>) ((a, v) => a.IndexPatterns = (IReadOnlyCollection<string>) v));

    public PutIndexTemplateDescriptor Settings(
      Func<IndexSettingsDescriptor, IPromise<IIndexSettings>> settingsSelector)
    {
      return this.Assign<Func<IndexSettingsDescriptor, IPromise<IIndexSettings>>>(settingsSelector, (Action<IPutIndexTemplateRequest, Func<IndexSettingsDescriptor, IPromise<IIndexSettings>>>) ((a, v) => a.Settings = v != null ? v(new IndexSettingsDescriptor())?.Value : (IIndexSettings) null));
    }

    public PutIndexTemplateDescriptor Map<T>(
      Func<TypeMappingDescriptor<T>, ITypeMapping> selector)
      where T : class
    {
      return this.Assign<Func<TypeMappingDescriptor<T>, ITypeMapping>>(selector, (Action<IPutIndexTemplateRequest, Func<TypeMappingDescriptor<T>, ITypeMapping>>) ((a, v) => a.Mappings = v != null ? v(new TypeMappingDescriptor<T>()) : (ITypeMapping) null));
    }

    public PutIndexTemplateDescriptor Map(
      Func<TypeMappingDescriptor<object>, ITypeMapping> selector)
    {
      return this.Assign<Func<TypeMappingDescriptor<object>, ITypeMapping>>(selector, (Action<IPutIndexTemplateRequest, Func<TypeMappingDescriptor<object>, ITypeMapping>>) ((a, v) => a.Mappings = v != null ? v(new TypeMappingDescriptor<object>()) : (ITypeMapping) null));
    }

    [Obsolete("Mappings is no longer a dictionary in 7.x, please use the simplified Map() method on this descriptor instead")]
    public PutIndexTemplateDescriptor Mappings(
      Func<MappingsDescriptor, ITypeMapping> mappingSelector)
    {
      return this.Assign<Func<MappingsDescriptor, ITypeMapping>>(mappingSelector, (Action<IPutIndexTemplateRequest, Func<MappingsDescriptor, ITypeMapping>>) ((a, v) => a.Mappings = v != null ? v(new MappingsDescriptor()) : (ITypeMapping) null));
    }

    public PutIndexTemplateDescriptor Aliases(
      Func<AliasesDescriptor, IPromise<IAliases>> aliasDescriptor)
    {
      return this.Assign<Func<AliasesDescriptor, IPromise<IAliases>>>(aliasDescriptor, (Action<IPutIndexTemplateRequest, Func<AliasesDescriptor, IPromise<IAliases>>>) ((a, v) => a.Aliases = v != null ? v(new AliasesDescriptor())?.Value : (IAliases) null));
    }
  }
}
