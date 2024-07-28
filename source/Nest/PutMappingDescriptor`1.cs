// Decompiled with JetBrains decompiler
// Type: Nest.PutMappingDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.IndicesApi;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  [DataContract]
  public class PutMappingDescriptor<TDocument> : 
    RequestDescriptorBase<PutMappingDescriptor<TDocument>, PutMappingRequestParameters, IPutMappingRequest<TDocument>>,
    IPutMappingRequest<TDocument>,
    IPutMappingRequest,
    ITypeMapping,
    IRequest<PutMappingRequestParameters>,
    IRequest
    where TDocument : class
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.IndicesPutMapping;

    public PutMappingDescriptor(Indices index)
      : base((Func<RouteValues, RouteValues>) (r => r.Required(nameof (index), (IUrlParameter) index)))
    {
    }

    public PutMappingDescriptor()
      : this((Indices) typeof (TDocument))
    {
    }

    Indices IPutMappingRequest.Index => this.Self.RouteValues.Get<Indices>("index");

    public PutMappingDescriptor<TDocument> Index(Indices index) => this.Assign<Indices>(index, (Action<IPutMappingRequest<TDocument>, Indices>) ((a, v) => a.RouteValues.Required(nameof (index), (IUrlParameter) v)));

    public PutMappingDescriptor<TDocument> Index<TOther>() where TOther : class => this.Assign<Type>(typeof (TOther), (Action<IPutMappingRequest<TDocument>, Type>) ((a, v) => a.RouteValues.Required("index", (IUrlParameter) (Indices) v)));

    public PutMappingDescriptor<TDocument> AllIndices() => this.Index(Indices.All);

    public PutMappingDescriptor<TDocument> AllowNoIndices(bool? allownoindices = true) => this.Qs("allow_no_indices", (object) allownoindices);

    public PutMappingDescriptor<TDocument> ExpandWildcards(Elasticsearch.Net.ExpandWildcards? expandwildcards) => this.Qs("expand_wildcards", (object) expandwildcards);

    public PutMappingDescriptor<TDocument> IgnoreUnavailable(bool? ignoreunavailable = true) => this.Qs("ignore_unavailable", (object) ignoreunavailable);

    public PutMappingDescriptor<TDocument> IncludeTypeName(bool? includetypename = true) => this.Qs("include_type_name", (object) includetypename);

    public PutMappingDescriptor<TDocument> MasterTimeout(Time mastertimeout) => this.Qs("master_timeout", (object) mastertimeout);

    public PutMappingDescriptor<TDocument> Timeout(Time timeout) => this.Qs(nameof (timeout), (object) timeout);

    public PutMappingDescriptor<TDocument> WriteIndexOnly(bool? writeindexonly = true) => this.Qs("write_index_only", (object) writeindexonly);

    [Obsolete("The _all field is no longer supported in Elasticsearch 7.x and will be removed in the next major release. The value will not be sent in a request. An _all like field can be achieved using copy_to")]
    IAllField ITypeMapping.AllField { get; set; }

    bool? ITypeMapping.DateDetection { get; set; }

    Union<bool, DynamicMapping> ITypeMapping.Dynamic { get; set; }

    IEnumerable<string> ITypeMapping.DynamicDateFormats { get; set; }

    IDynamicTemplateContainer ITypeMapping.DynamicTemplates { get; set; }

    IFieldNamesField ITypeMapping.FieldNamesField { get; set; }

    [Obsolete("Configuration for the _index field is no longer supported in Elasticsearch 7.x and will be removed in the next major release.")]
    IIndexField ITypeMapping.IndexField { get; set; }

    IDictionary<string, object> ITypeMapping.Meta { get; set; }

    bool? ITypeMapping.NumericDetection { get; set; }

    IProperties ITypeMapping.Properties { get; set; }

    IRoutingField ITypeMapping.RoutingField { get; set; }

    IRuntimeFields ITypeMapping.RuntimeFields { get; set; }

    ISizeField ITypeMapping.SizeField { get; set; }

    ISourceField ITypeMapping.SourceField { get; set; }

    protected PutMappingDescriptor<TDocument> Assign<TValue>(
      TValue value,
      Action<ITypeMapping, TValue> assigner)
    {
      return Fluent.Assign<PutMappingDescriptor<TDocument>, ITypeMapping, TValue>(this, value, assigner);
    }

    public PutMappingDescriptor<TDocument> AutoMap(IPropertyVisitor visitor = null, int maxRecursion = 0)
    {
      this.Self.Properties = this.Self.Properties.AutoMap<TDocument>(visitor, maxRecursion);
      return this;
    }

    public PutMappingDescriptor<TDocument> AutoMap(int maxRecursion) => this.AutoMap((IPropertyVisitor) null, maxRecursion);

    public PutMappingDescriptor<TDocument> Dynamic(Union<bool, DynamicMapping> dynamic) => this.Assign<Union<bool, DynamicMapping>>(dynamic, (Action<ITypeMapping, Union<bool, DynamicMapping>>) ((a, v) => a.Dynamic = v));

    public PutMappingDescriptor<TDocument> Dynamic(bool? dynamic = true) => this.Assign<bool?>(dynamic, (Action<ITypeMapping, bool?>) ((a, v) =>
    {
      ITypeMapping typeMapping = a;
      bool? nullable = v;
      Union<bool, DynamicMapping> valueOrDefault = nullable.HasValue ? (Union<bool, DynamicMapping>) nullable.GetValueOrDefault() : (Union<bool, DynamicMapping>) null;
      typeMapping.Dynamic = valueOrDefault;
    }));

    [Obsolete("The _all field is no longer supported in Elasticsearch 7.x and will be removed in the next major release. The value will not be sent in a request. An _all like field can be achieved using copy_to")]
    public PutMappingDescriptor<TDocument> AllField(
      Func<AllFieldDescriptor, IAllField> allFieldSelector)
    {
      return this.Assign<Func<AllFieldDescriptor, IAllField>>(allFieldSelector, (Action<ITypeMapping, Func<AllFieldDescriptor, IAllField>>) ((a, v) => a.AllField = v != null ? v(new AllFieldDescriptor()) : (IAllField) null));
    }

    [Obsolete("Configuration for the _index field is no longer supported in Elasticsearch 7.x and will be removed in the next major release.")]
    public PutMappingDescriptor<TDocument> IndexField(
      Func<IndexFieldDescriptor, IIndexField> indexFieldSelector)
    {
      return this.Assign<Func<IndexFieldDescriptor, IIndexField>>(indexFieldSelector, (Action<ITypeMapping, Func<IndexFieldDescriptor, IIndexField>>) ((a, v) => a.IndexField = v != null ? v(new IndexFieldDescriptor()) : (IIndexField) null));
    }

    public PutMappingDescriptor<TDocument> SizeField(
      Func<SizeFieldDescriptor, ISizeField> sizeFieldSelector)
    {
      return this.Assign<Func<SizeFieldDescriptor, ISizeField>>(sizeFieldSelector, (Action<ITypeMapping, Func<SizeFieldDescriptor, ISizeField>>) ((a, v) => a.SizeField = v != null ? v(new SizeFieldDescriptor()) : (ISizeField) null));
    }

    public PutMappingDescriptor<TDocument> DisableSizeField(bool? disabled = true) => this.Assign<bool?>(disabled, (Action<ITypeMapping, bool?>) ((a, v) =>
    {
      ITypeMapping typeMapping = a;
      Nest.SizeField sizeField = new Nest.SizeField();
      bool? nullable = v;
      sizeField.Enabled = nullable.HasValue ? new bool?(!nullable.GetValueOrDefault()) : new bool?();
      typeMapping.SizeField = (ISizeField) sizeField;
    }));

    [Obsolete("Configuration for the _index field is no longer supported in Elasticsearch 7.x and will be removed in the next major release.")]
    public PutMappingDescriptor<TDocument> DisableIndexField(bool? disabled = true) => this.Assign<bool?>(disabled, (Action<ITypeMapping, bool?>) ((a, v) =>
    {
      ITypeMapping typeMapping = a;
      Nest.IndexField indexField = new Nest.IndexField();
      bool? nullable = v;
      indexField.Enabled = nullable.HasValue ? new bool?(!nullable.GetValueOrDefault()) : new bool?();
      typeMapping.IndexField = (IIndexField) indexField;
    }));

    public PutMappingDescriptor<TDocument> DynamicDateFormats(IEnumerable<string> dateFormats) => this.Assign<IEnumerable<string>>(dateFormats, (Action<ITypeMapping, IEnumerable<string>>) ((a, v) => a.DynamicDateFormats = v));

    public PutMappingDescriptor<TDocument> DateDetection(bool? detect = true) => this.Assign<bool?>(detect, (Action<ITypeMapping, bool?>) ((a, v) => a.DateDetection = v));

    public PutMappingDescriptor<TDocument> NumericDetection(bool? detect = true) => this.Assign<bool?>(detect, (Action<ITypeMapping, bool?>) ((a, v) => a.NumericDetection = v));

    public PutMappingDescriptor<TDocument> SourceField(
      Func<SourceFieldDescriptor, ISourceField> sourceFieldSelector)
    {
      return this.Assign<Func<SourceFieldDescriptor, ISourceField>>(sourceFieldSelector, (Action<ITypeMapping, Func<SourceFieldDescriptor, ISourceField>>) ((a, v) => a.SourceField = v != null ? v(new SourceFieldDescriptor()) : (ISourceField) null));
    }

    public PutMappingDescriptor<TDocument> RoutingField(
      Func<RoutingFieldDescriptor<TDocument>, IRoutingField> routingFieldSelector)
    {
      return this.Assign<Func<RoutingFieldDescriptor<TDocument>, IRoutingField>>(routingFieldSelector, (Action<ITypeMapping, Func<RoutingFieldDescriptor<TDocument>, IRoutingField>>) ((a, v) => a.RoutingField = v != null ? v(new RoutingFieldDescriptor<TDocument>()) : (IRoutingField) null));
    }

    public PutMappingDescriptor<TDocument> RuntimeFields(
      Func<RuntimeFieldsDescriptor<TDocument>, IPromise<IRuntimeFields>> runtimeFieldsSelector)
    {
      return this.Assign<Func<RuntimeFieldsDescriptor<TDocument>, IPromise<IRuntimeFields>>>(runtimeFieldsSelector, (Action<ITypeMapping, Func<RuntimeFieldsDescriptor<TDocument>, IPromise<IRuntimeFields>>>) ((a, v) => a.RuntimeFields = v != null ? v(new RuntimeFieldsDescriptor<TDocument>())?.Value : (IRuntimeFields) null));
    }

    public PutMappingDescriptor<TDocument> RuntimeFields<TSource>(
      Func<RuntimeFieldsDescriptor<TSource>, IPromise<IRuntimeFields>> runtimeFieldsSelector)
      where TSource : class
    {
      return this.Assign<Func<RuntimeFieldsDescriptor<TSource>, IPromise<IRuntimeFields>>>(runtimeFieldsSelector, (Action<ITypeMapping, Func<RuntimeFieldsDescriptor<TSource>, IPromise<IRuntimeFields>>>) ((a, v) => a.RuntimeFields = v != null ? v(new RuntimeFieldsDescriptor<TSource>())?.Value : (IRuntimeFields) null));
    }

    public PutMappingDescriptor<TDocument> FieldNamesField(
      Func<FieldNamesFieldDescriptor<TDocument>, IFieldNamesField> fieldNamesFieldSelector)
    {
      return this.Assign<Func<FieldNamesFieldDescriptor<TDocument>, IFieldNamesField>>(fieldNamesFieldSelector, (Action<ITypeMapping, Func<FieldNamesFieldDescriptor<TDocument>, IFieldNamesField>>) ((a, v) => a.FieldNamesField = v(new FieldNamesFieldDescriptor<TDocument>())));
    }

    public PutMappingDescriptor<TDocument> Meta(
      Func<FluentDictionary<string, object>, FluentDictionary<string, object>> metaSelector)
    {
      return this.Assign<Func<FluentDictionary<string, object>, FluentDictionary<string, object>>>(metaSelector, (Action<ITypeMapping, Func<FluentDictionary<string, object>, FluentDictionary<string, object>>>) ((a, v) => a.Meta = (IDictionary<string, object>) v(new FluentDictionary<string, object>())));
    }

    public PutMappingDescriptor<TDocument> Meta(Dictionary<string, object> metaDictionary) => this.Assign<Dictionary<string, object>>(metaDictionary, (Action<ITypeMapping, Dictionary<string, object>>) ((a, v) => a.Meta = (IDictionary<string, object>) v));

    public PutMappingDescriptor<TDocument> Properties(
      Func<PropertiesDescriptor<TDocument>, IPromise<IProperties>> propertiesSelector)
    {
      return this.Assign<Func<PropertiesDescriptor<TDocument>, IPromise<IProperties>>>(propertiesSelector, (Action<ITypeMapping, Func<PropertiesDescriptor<TDocument>, IPromise<IProperties>>>) ((a, v) => a.Properties = v != null ? v(new PropertiesDescriptor<TDocument>(a.Properties))?.Value : (IProperties) null));
    }

    public PutMappingDescriptor<TDocument> DynamicTemplates(
      Func<DynamicTemplateContainerDescriptor<TDocument>, IPromise<IDynamicTemplateContainer>> dynamicTemplatesSelector)
    {
      return this.Assign<Func<DynamicTemplateContainerDescriptor<TDocument>, IPromise<IDynamicTemplateContainer>>>(dynamicTemplatesSelector, (Action<ITypeMapping, Func<DynamicTemplateContainerDescriptor<TDocument>, IPromise<IDynamicTemplateContainer>>>) ((a, v) => a.DynamicTemplates = v != null ? v(new DynamicTemplateContainerDescriptor<TDocument>())?.Value : (IDynamicTemplateContainer) null));
    }
  }
}
