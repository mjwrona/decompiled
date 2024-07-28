// Decompiled with JetBrains decompiler
// Type: Nest.ISetProcessor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System.Runtime.Serialization;

namespace Nest
{
  [InterfaceDataContract]
  public interface ISetProcessor : IProcessor
  {
    [DataMember(Name = "field")]
    Field Field { get; set; }

    [DataMember(Name = "value")]
    [JsonFormatter(typeof (SourceWriteFormatter<>))]
    object Value { get; set; }

    [DataMember(Name = "override")]
    bool? Override { get; set; }

    [DataMember(Name = "ignore_empty_value")]
    bool? IgnoreEmptyValue { get; set; }

    [DataMember(Name = "copy_from")]
    Field CopyFrom { get; set; }
  }
}
