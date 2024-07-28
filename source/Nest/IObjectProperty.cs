// Decompiled with JetBrains decompiler
// Type: Nest.IObjectProperty
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System.Runtime.Serialization;

namespace Nest
{
  [InterfaceDataContract]
  public interface IObjectProperty : ICoreProperty, IProperty, IFieldMapping
  {
    [DataMember(Name = "dynamic")]
    [JsonFormatter(typeof (DynamicMappingFormatter))]
    Union<bool, DynamicMapping> Dynamic { get; set; }

    [DataMember(Name = "enabled")]
    bool? Enabled { get; set; }

    [DataMember(Name = "properties")]
    IProperties Properties { get; set; }
  }
}
