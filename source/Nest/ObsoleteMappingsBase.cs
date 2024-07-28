// Decompiled with JetBrains decompiler
// Type: Nest.ObsoleteMappingsBase
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  public abstract class ObsoleteMappingsBase : ITypeMapping
  {
    [IgnoreDataMember]
    [Obsolete("The _all field is no longer supported in Elasticsearch 7.x and will be removed in the next major release. The value will not be sent in a request. An _all like field can be achieved using copy_to")]
    public IAllField AllField
    {
      get => this.Wrapped.AllField;
      set => this.Wrapped.AllField = value;
    }

    [DataMember(Name = "date_detection")]
    bool? ITypeMapping.DateDetection
    {
      get => this.Wrapped.DateDetection;
      set => this.Wrapped.DateDetection = value;
    }

    [DataMember(Name = "dynamic")]
    Union<bool, DynamicMapping> ITypeMapping.Dynamic
    {
      get => this.Wrapped.Dynamic;
      set => this.Wrapped.Dynamic = value;
    }

    [DataMember(Name = "dynamic_date_formats")]
    IEnumerable<string> ITypeMapping.DynamicDateFormats
    {
      get => this.Wrapped.DynamicDateFormats;
      set => this.Wrapped.DynamicDateFormats = value;
    }

    [DataMember(Name = "dynamic_templates")]
    IDynamicTemplateContainer ITypeMapping.DynamicTemplates
    {
      get => this.Wrapped.DynamicTemplates;
      set => this.Wrapped.DynamicTemplates = value;
    }

    [DataMember(Name = "_field_names")]
    IFieldNamesField ITypeMapping.FieldNamesField
    {
      get => this.Wrapped.FieldNamesField;
      set => this.Wrapped.FieldNamesField = value;
    }

    [IgnoreDataMember]
    [Obsolete("Configuration for the _index field is no longer supported in Elasticsearch 7.x and will be removed in the next major release.")]
    IIndexField ITypeMapping.IndexField
    {
      get => this.Wrapped.IndexField;
      set => this.Wrapped.IndexField = value;
    }

    [DataMember(Name = "_meta")]
    IDictionary<string, object> ITypeMapping.Meta
    {
      get => this.Wrapped.Meta;
      set => this.Wrapped.Meta = value;
    }

    [DataMember(Name = "numeric_detection")]
    bool? ITypeMapping.NumericDetection
    {
      get => this.Wrapped.NumericDetection;
      set => this.Wrapped.NumericDetection = value;
    }

    [DataMember(Name = "properties")]
    IProperties ITypeMapping.Properties
    {
      get => this.Wrapped.Properties;
      set => this.Wrapped.Properties = value;
    }

    [DataMember(Name = "_routing")]
    IRoutingField ITypeMapping.RoutingField
    {
      get => this.Wrapped.RoutingField;
      set => this.Wrapped.RoutingField = value;
    }

    [DataMember(Name = "runtime")]
    IRuntimeFields ITypeMapping.RuntimeFields
    {
      get => this.Wrapped.RuntimeFields;
      set => this.Wrapped.RuntimeFields = value;
    }

    [DataMember(Name = "_size")]
    ISizeField ITypeMapping.SizeField
    {
      get => this.Wrapped.SizeField;
      set => this.Wrapped.SizeField = value;
    }

    [DataMember(Name = "_source")]
    ISourceField ITypeMapping.SourceField
    {
      get => this.Wrapped.SourceField;
      set => this.Wrapped.SourceField = value;
    }

    protected ITypeMapping Wrapped { get; set; } = (ITypeMapping) new TypeMapping();
  }
}
