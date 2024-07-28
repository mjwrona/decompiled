// Decompiled with JetBrains decompiler
// Type: Nest.TypeMappingDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;

namespace Nest
{
  public class TypeMappingDescriptor<T> : 
    DescriptorBase<TypeMappingDescriptor<T>, ITypeMapping>,
    ITypeMapping
    where T : class
  {
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

    public TypeMappingDescriptor<T> AutoMap(IPropertyVisitor visitor = null, int maxRecursion = 0) => this.Assign<IProperties>(this.Self.Properties.AutoMap<T>(visitor, maxRecursion), (Action<ITypeMapping, IProperties>) ((a, v) => a.Properties = v));

    public TypeMappingDescriptor<T> AutoMap(
      Type documentType,
      IPropertyVisitor visitor = null,
      int maxRecursion = 0)
    {
      if (!documentType.IsClass)
        throw new ArgumentException("must be a reference type", nameof (documentType));
      return this.Assign<IProperties>(this.Self.Properties.AutoMap(documentType, visitor, maxRecursion), (Action<ITypeMapping, IProperties>) ((a, v) => a.Properties = v));
    }

    public TypeMappingDescriptor<T> AutoMap<TDocument>(IPropertyVisitor visitor = null, int maxRecursion = 0) where TDocument : class => this.Assign<IProperties>(this.Self.Properties.AutoMap<TDocument>(visitor, maxRecursion), (Action<ITypeMapping, IProperties>) ((a, v) => a.Properties = v));

    public TypeMappingDescriptor<T> AutoMap(int maxRecursion) => this.AutoMap((IPropertyVisitor) null, maxRecursion);

    public TypeMappingDescriptor<T> Dynamic(Union<bool, DynamicMapping> dynamic) => this.Assign<Union<bool, DynamicMapping>>(dynamic, (Action<ITypeMapping, Union<bool, DynamicMapping>>) ((a, v) => a.Dynamic = v));

    public TypeMappingDescriptor<T> Dynamic(bool dynamic = true) => this.Assign<bool>(dynamic, (Action<ITypeMapping, bool>) ((a, v) => a.Dynamic = (Union<bool, DynamicMapping>) v));

    [Obsolete("The _all field is no longer supported in Elasticsearch 7.x and will be removed in the next major release. The value will not be sent in a request. An _all like field can be achieved using copy_to")]
    public TypeMappingDescriptor<T> AllField(
      Func<AllFieldDescriptor, IAllField> allFieldSelector)
    {
      return this.Assign<Func<AllFieldDescriptor, IAllField>>(allFieldSelector, (Action<ITypeMapping, Func<AllFieldDescriptor, IAllField>>) ((a, v) => a.AllField = v != null ? v(new AllFieldDescriptor()) : (IAllField) null));
    }

    [Obsolete("Configuration for the _index field is no longer supported in Elasticsearch 7.x and will be removed in the next major release.")]
    public TypeMappingDescriptor<T> IndexField(
      Func<IndexFieldDescriptor, IIndexField> indexFieldSelector)
    {
      return this.Assign<Func<IndexFieldDescriptor, IIndexField>>(indexFieldSelector, (Action<ITypeMapping, Func<IndexFieldDescriptor, IIndexField>>) ((a, v) => a.IndexField = v != null ? v(new IndexFieldDescriptor()) : (IIndexField) null));
    }

    public TypeMappingDescriptor<T> SizeField(
      Func<SizeFieldDescriptor, ISizeField> sizeFieldSelector)
    {
      return this.Assign<Func<SizeFieldDescriptor, ISizeField>>(sizeFieldSelector, (Action<ITypeMapping, Func<SizeFieldDescriptor, ISizeField>>) ((a, v) => a.SizeField = v != null ? v(new SizeFieldDescriptor()) : (ISizeField) null));
    }

    public TypeMappingDescriptor<T> SourceField(
      Func<SourceFieldDescriptor, ISourceField> sourceFieldSelector)
    {
      return this.Assign<Func<SourceFieldDescriptor, ISourceField>>(sourceFieldSelector, (Action<ITypeMapping, Func<SourceFieldDescriptor, ISourceField>>) ((a, v) => a.SourceField = v != null ? v(new SourceFieldDescriptor()) : (ISourceField) null));
    }

    public TypeMappingDescriptor<T> DisableSizeField(bool? disabled = true)
    {
      Nest.SizeField sizeField = new Nest.SizeField();
      bool? nullable = disabled;
      sizeField.Enabled = nullable.HasValue ? new bool?(!nullable.GetValueOrDefault()) : new bool?();
      return this.Assign<Nest.SizeField>(sizeField, (Action<ITypeMapping, Nest.SizeField>) ((a, v) => a.SizeField = (ISizeField) v));
    }

    [Obsolete("Configuration for the _index field is no longer supported in Elasticsearch 7.x and will be removed in the next major release.")]
    public TypeMappingDescriptor<T> DisableIndexField(bool? disabled = true)
    {
      Nest.IndexField indexField = new Nest.IndexField();
      bool? nullable = disabled;
      indexField.Enabled = nullable.HasValue ? new bool?(!nullable.GetValueOrDefault()) : new bool?();
      return this.Assign<Nest.IndexField>(indexField, (Action<ITypeMapping, Nest.IndexField>) ((a, v) => a.IndexField = (IIndexField) v));
    }

    public TypeMappingDescriptor<T> DynamicDateFormats(IEnumerable<string> dateFormats) => this.Assign<IEnumerable<string>>(dateFormats, (Action<ITypeMapping, IEnumerable<string>>) ((a, v) => a.DynamicDateFormats = v));

    public TypeMappingDescriptor<T> DateDetection(bool? detect = true) => this.Assign<bool?>(detect, (Action<ITypeMapping, bool?>) ((a, v) => a.DateDetection = v));

    public TypeMappingDescriptor<T> NumericDetection(bool? detect = true) => this.Assign<bool?>(detect, (Action<ITypeMapping, bool?>) ((a, v) => a.NumericDetection = v));

    public TypeMappingDescriptor<T> RoutingField(
      Func<RoutingFieldDescriptor<T>, IRoutingField> routingFieldSelector)
    {
      return this.Assign<Func<RoutingFieldDescriptor<T>, IRoutingField>>(routingFieldSelector, (Action<ITypeMapping, Func<RoutingFieldDescriptor<T>, IRoutingField>>) ((a, v) => a.RoutingField = v != null ? v(new RoutingFieldDescriptor<T>()) : (IRoutingField) null));
    }

    public TypeMappingDescriptor<T> RuntimeFields(
      Func<RuntimeFieldsDescriptor<T>, IPromise<IRuntimeFields>> runtimeFieldsSelector)
    {
      return this.Assign<Func<RuntimeFieldsDescriptor<T>, IPromise<IRuntimeFields>>>(runtimeFieldsSelector, (Action<ITypeMapping, Func<RuntimeFieldsDescriptor<T>, IPromise<IRuntimeFields>>>) ((a, v) => a.RuntimeFields = v != null ? v(new RuntimeFieldsDescriptor<T>())?.Value : (IRuntimeFields) null));
    }

    public TypeMappingDescriptor<T> RuntimeFields<TDocument>(
      Func<RuntimeFieldsDescriptor<TDocument>, IPromise<IRuntimeFields>> runtimeFieldsSelector)
      where TDocument : class
    {
      return this.Assign<Func<RuntimeFieldsDescriptor<TDocument>, IPromise<IRuntimeFields>>>(runtimeFieldsSelector, (Action<ITypeMapping, Func<RuntimeFieldsDescriptor<TDocument>, IPromise<IRuntimeFields>>>) ((a, v) => a.RuntimeFields = v != null ? v(new RuntimeFieldsDescriptor<TDocument>())?.Value : (IRuntimeFields) null));
    }

    public TypeMappingDescriptor<T> FieldNamesField(
      Func<FieldNamesFieldDescriptor<T>, IFieldNamesField> fieldNamesFieldSelector)
    {
      return this.Assign<IFieldNamesField>(fieldNamesFieldSelector(new FieldNamesFieldDescriptor<T>()), (Action<ITypeMapping, IFieldNamesField>) ((a, v) => a.FieldNamesField = v));
    }

    public TypeMappingDescriptor<T> Meta(
      Func<FluentDictionary<string, object>, FluentDictionary<string, object>> metaSelector)
    {
      return this.Assign<FluentDictionary<string, object>>(metaSelector(new FluentDictionary<string, object>()), (Action<ITypeMapping, FluentDictionary<string, object>>) ((a, v) => a.Meta = (IDictionary<string, object>) v));
    }

    public TypeMappingDescriptor<T> Meta(Dictionary<string, object> metaDictionary) => this.Assign<Dictionary<string, object>>(metaDictionary, (Action<ITypeMapping, Dictionary<string, object>>) ((a, v) => a.Meta = (IDictionary<string, object>) v));

    public TypeMappingDescriptor<T> Properties(
      Func<PropertiesDescriptor<T>, IPromise<IProperties>> propertiesSelector)
    {
      return this.Assign<Func<PropertiesDescriptor<T>, IPromise<IProperties>>>(propertiesSelector, (Action<ITypeMapping, Func<PropertiesDescriptor<T>, IPromise<IProperties>>>) ((a, v) => a.Properties = v != null ? v(new PropertiesDescriptor<T>(this.Self.Properties))?.Value : (IProperties) null));
    }

    public TypeMappingDescriptor<T> Properties<TDocument>(
      Func<PropertiesDescriptor<TDocument>, IPromise<IProperties>> propertiesSelector)
      where TDocument : class
    {
      return this.Assign<Func<PropertiesDescriptor<TDocument>, IPromise<IProperties>>>(propertiesSelector, (Action<ITypeMapping, Func<PropertiesDescriptor<TDocument>, IPromise<IProperties>>>) ((a, v) => a.Properties = v != null ? v(new PropertiesDescriptor<TDocument>(this.Self.Properties))?.Value : (IProperties) null));
    }

    public TypeMappingDescriptor<T> DynamicTemplates(
      Func<DynamicTemplateContainerDescriptor<T>, IPromise<IDynamicTemplateContainer>> dynamicTemplatesSelector)
    {
      return this.Assign<Func<DynamicTemplateContainerDescriptor<T>, IPromise<IDynamicTemplateContainer>>>(dynamicTemplatesSelector, (Action<ITypeMapping, Func<DynamicTemplateContainerDescriptor<T>, IPromise<IDynamicTemplateContainer>>>) ((a, v) => a.DynamicTemplates = v != null ? v(new DynamicTemplateContainerDescriptor<T>())?.Value : (IDynamicTemplateContainer) null));
    }
  }
}
