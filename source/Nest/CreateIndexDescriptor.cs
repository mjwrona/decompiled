// Decompiled with JetBrains decompiler
// Type: Nest.CreateIndexDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.IndicesApi;
using Elasticsearch.Net.Utf8Json;
using System;

namespace Nest
{
  public class CreateIndexDescriptor : 
    RequestDescriptorBase<CreateIndexDescriptor, CreateIndexRequestParameters, ICreateIndexRequest>,
    ICreateIndexRequest,
    IIndexState,
    IRequest<CreateIndexRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.IndicesCreate;

    public CreateIndexDescriptor(IndexName index)
      : base((Func<RouteValues, RouteValues>) (r => r.Required(nameof (index), (IUrlParameter) index)))
    {
    }

    [SerializationConstructor]
    protected CreateIndexDescriptor()
    {
    }

    IndexName ICreateIndexRequest.Index => this.Self.RouteValues.Get<IndexName>("index");

    public CreateIndexDescriptor Index(IndexName index) => this.Assign<IndexName>(index, (Action<ICreateIndexRequest, IndexName>) ((a, v) => a.RouteValues.Required(nameof (index), (IUrlParameter) v)));

    public CreateIndexDescriptor Index<TOther>() where TOther : class => this.Assign<Type>(typeof (TOther), (Action<ICreateIndexRequest, Type>) ((a, v) => a.RouteValues.Required("index", (IUrlParameter) (IndexName) v)));

    public CreateIndexDescriptor IncludeTypeName(bool? includetypename = true) => this.Qs("include_type_name", (object) includetypename);

    public CreateIndexDescriptor MasterTimeout(Time mastertimeout) => this.Qs("master_timeout", (object) mastertimeout);

    public CreateIndexDescriptor Timeout(Time timeout) => this.Qs(nameof (timeout), (object) timeout);

    public CreateIndexDescriptor WaitForActiveShards(string waitforactiveshards) => this.Qs("wait_for_active_shards", (object) waitforactiveshards);

    IAliases IIndexState.Aliases { get; set; }

    ITypeMapping IIndexState.Mappings { get; set; }

    IIndexSettings IIndexState.Settings { get; set; }

    public CreateIndexDescriptor InitializeUsing(IIndexState indexSettings) => this.Assign<IIndexState>(indexSettings, (Action<ICreateIndexRequest, IIndexState>) ((a, v) =>
    {
      a.Settings = v.Settings;
      a.Mappings = v.Mappings;
      a.Aliases = v.Aliases;
      CreateIndexRequest.RemoveReadOnlySettings(a.Settings);
    }));

    public CreateIndexDescriptor Settings(
      Func<IndexSettingsDescriptor, IPromise<IIndexSettings>> selector)
    {
      return this.Assign<Func<IndexSettingsDescriptor, IPromise<IIndexSettings>>>(selector, (Action<ICreateIndexRequest, Func<IndexSettingsDescriptor, IPromise<IIndexSettings>>>) ((a, v) => a.Settings = v != null ? v(new IndexSettingsDescriptor())?.Value : (IIndexSettings) null));
    }

    public CreateIndexDescriptor Map<T>(
      Func<TypeMappingDescriptor<T>, ITypeMapping> selector)
      where T : class
    {
      return this.Assign<Func<TypeMappingDescriptor<T>, ITypeMapping>>(selector, (Action<ICreateIndexRequest, Func<TypeMappingDescriptor<T>, ITypeMapping>>) ((a, v) => a.Mappings = v != null ? v(new TypeMappingDescriptor<T>()) : (ITypeMapping) null));
    }

    public CreateIndexDescriptor Map(
      Func<TypeMappingDescriptor<object>, ITypeMapping> selector)
    {
      return this.Assign<Func<TypeMappingDescriptor<object>, ITypeMapping>>(selector, (Action<ICreateIndexRequest, Func<TypeMappingDescriptor<object>, ITypeMapping>>) ((a, v) => a.Mappings = v != null ? v(new TypeMappingDescriptor<object>()) : (ITypeMapping) null));
    }

    [Obsolete("Mappings is no longer a dictionary in 7.x, please use the simplified Map() method on this descriptor instead")]
    public CreateIndexDescriptor Mappings(Func<MappingsDescriptor, ITypeMapping> selector) => this.Assign<Func<MappingsDescriptor, ITypeMapping>>(selector, (Action<ICreateIndexRequest, Func<MappingsDescriptor, ITypeMapping>>) ((a, v) => a.Mappings = v != null ? v(new MappingsDescriptor()) : (ITypeMapping) null));

    public CreateIndexDescriptor Aliases(
      Func<AliasesDescriptor, IPromise<IAliases>> selector)
    {
      return this.Assign<Func<AliasesDescriptor, IPromise<IAliases>>>(selector, (Action<ICreateIndexRequest, Func<AliasesDescriptor, IPromise<IAliases>>>) ((a, v) => a.Aliases = v != null ? v(new AliasesDescriptor())?.Value : (IAliases) null));
    }
  }
}
