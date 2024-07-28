// Decompiled with JetBrains decompiler
// Type: Nest.RolloverIndexDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.IndicesApi;
using Elasticsearch.Net.Utf8Json;
using System;

namespace Nest
{
  public class RolloverIndexDescriptor : 
    RequestDescriptorBase<RolloverIndexDescriptor, RolloverIndexRequestParameters, IRolloverIndexRequest>,
    IRolloverIndexRequest,
    IIndexState,
    IRequest<RolloverIndexRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.IndicesRollover;

    public RolloverIndexDescriptor(Name alias)
      : base((Func<RouteValues, RouteValues>) (r => r.Required(nameof (alias), (IUrlParameter) alias)))
    {
    }

    public RolloverIndexDescriptor(Name alias, IndexName newIndex)
      : base((Func<RouteValues, RouteValues>) (r => r.Required(nameof (alias), (IUrlParameter) alias).Optional("new_index", (IUrlParameter) newIndex)))
    {
    }

    [SerializationConstructor]
    protected RolloverIndexDescriptor()
    {
    }

    Name IRolloverIndexRequest.Alias => this.Self.RouteValues.Get<Name>("alias");

    IndexName IRolloverIndexRequest.NewIndex => this.Self.RouteValues.Get<IndexName>("new_index");

    public RolloverIndexDescriptor NewIndex(IndexName newIndex) => this.Assign<IndexName>(newIndex, (Action<IRolloverIndexRequest, IndexName>) ((a, v) => a.RouteValues.Optional("new_index", (IUrlParameter) v)));

    public RolloverIndexDescriptor DryRun(bool? dryrun = true) => this.Qs("dry_run", (object) dryrun);

    public RolloverIndexDescriptor IncludeTypeName(bool? includetypename = true) => this.Qs("include_type_name", (object) includetypename);

    public RolloverIndexDescriptor MasterTimeout(Time mastertimeout) => this.Qs("master_timeout", (object) mastertimeout);

    public RolloverIndexDescriptor Timeout(Time timeout) => this.Qs(nameof (timeout), (object) timeout);

    public RolloverIndexDescriptor WaitForActiveShards(string waitforactiveshards) => this.Qs("wait_for_active_shards", (object) waitforactiveshards);

    IAliases IIndexState.Aliases { get; set; }

    IRolloverConditions IRolloverIndexRequest.Conditions { get; set; }

    ITypeMapping IIndexState.Mappings { get; set; }

    IIndexSettings IIndexState.Settings { get; set; }

    public RolloverIndexDescriptor Conditions(
      Func<RolloverConditionsDescriptor, IRolloverConditions> selector)
    {
      return this.Assign<Func<RolloverConditionsDescriptor, IRolloverConditions>>(selector, (Action<IRolloverIndexRequest, Func<RolloverConditionsDescriptor, IRolloverConditions>>) ((a, v) => a.Conditions = v != null ? v(new RolloverConditionsDescriptor()) : (IRolloverConditions) null));
    }

    public RolloverIndexDescriptor Settings(
      Func<IndexSettingsDescriptor, IPromise<IIndexSettings>> selector)
    {
      return this.Assign<Func<IndexSettingsDescriptor, IPromise<IIndexSettings>>>(selector, (Action<IRolloverIndexRequest, Func<IndexSettingsDescriptor, IPromise<IIndexSettings>>>) ((a, v) => a.Settings = v != null ? v(new IndexSettingsDescriptor())?.Value : (IIndexSettings) null));
    }

    public RolloverIndexDescriptor Map<T>(
      Func<TypeMappingDescriptor<T>, ITypeMapping> selector)
      where T : class
    {
      return this.Assign<Func<TypeMappingDescriptor<T>, ITypeMapping>>(selector, (Action<IRolloverIndexRequest, Func<TypeMappingDescriptor<T>, ITypeMapping>>) ((a, v) => a.Mappings = v != null ? v(new TypeMappingDescriptor<T>()) : (ITypeMapping) null));
    }

    public RolloverIndexDescriptor Map(
      Func<TypeMappingDescriptor<object>, ITypeMapping> selector)
    {
      return this.Assign<Func<TypeMappingDescriptor<object>, ITypeMapping>>(selector, (Action<IRolloverIndexRequest, Func<TypeMappingDescriptor<object>, ITypeMapping>>) ((a, v) => a.Mappings = v != null ? v(new TypeMappingDescriptor<object>()) : (ITypeMapping) null));
    }

    [Obsolete("Mappings is no longer a dictionary in 7.x, please use the simplified Map() method on this descriptor instead")]
    public RolloverIndexDescriptor Mappings(Func<MappingsDescriptor, ITypeMapping> selector) => this.Assign<Func<MappingsDescriptor, ITypeMapping>>(selector, (Action<IRolloverIndexRequest, Func<MappingsDescriptor, ITypeMapping>>) ((a, v) => a.Mappings = v != null ? v(new MappingsDescriptor()) : (ITypeMapping) null));

    public RolloverIndexDescriptor Aliases(
      Func<AliasesDescriptor, IPromise<IAliases>> selector)
    {
      return this.Assign<Func<AliasesDescriptor, IPromise<IAliases>>>(selector, (Action<IRolloverIndexRequest, Func<AliasesDescriptor, IPromise<IAliases>>>) ((a, v) => a.Aliases = v != null ? v(new AliasesDescriptor())?.Value : (IAliases) null));
    }
  }
}
