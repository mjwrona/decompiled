// Decompiled with JetBrains decompiler
// Type: Nest.ITypeMapping
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  [InterfaceDataContract]
  [ReadAs(typeof (TypeMapping))]
  public interface ITypeMapping
  {
    [Obsolete("The _all field is no longer supported in Elasticsearch 7.x and will be removed in the next major release. The value will not be sent in a request. An _all like field can be achieved using copy_to")]
    [IgnoreDataMember]
    IAllField AllField { get; set; }

    [DataMember(Name = "date_detection")]
    bool? DateDetection { get; set; }

    [DataMember(Name = "dynamic")]
    [JsonFormatter(typeof (DynamicMappingFormatter))]
    Union<bool, DynamicMapping> Dynamic { get; set; }

    [DataMember(Name = "dynamic_date_formats")]
    IEnumerable<string> DynamicDateFormats { get; set; }

    [DataMember(Name = "dynamic_templates")]
    IDynamicTemplateContainer DynamicTemplates { get; set; }

    [DataMember(Name = "_field_names")]
    IFieldNamesField FieldNamesField { get; set; }

    [Obsolete("Configuration for the _index field is no longer supported in Elasticsearch 7.x and will be removed in the next major release.")]
    [IgnoreDataMember]
    IIndexField IndexField { get; set; }

    [DataMember(Name = "_meta")]
    [JsonFormatter(typeof (VerbatimDictionaryInterfaceKeysFormatter<string, object>))]
    IDictionary<string, object> Meta { get; set; }

    [DataMember(Name = "numeric_detection")]
    bool? NumericDetection { get; set; }

    [DataMember(Name = "properties")]
    IProperties Properties { get; set; }

    [DataMember(Name = "_routing")]
    IRoutingField RoutingField { get; set; }

    [DataMember(Name = "runtime")]
    IRuntimeFields RuntimeFields { get; set; }

    [DataMember(Name = "_size")]
    ISizeField SizeField { get; set; }

    [DataMember(Name = "_source")]
    ISourceField SourceField { get; set; }
  }
}
