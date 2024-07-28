// Decompiled with JetBrains decompiler
// Type: Nest.IInferenceProcessor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  [InterfaceDataContract]
  public interface IInferenceProcessor : IProcessor
  {
    [DataMember(Name = "model_id")]
    string ModelId { get; set; }

    [DataMember(Name = "target_field")]
    Field TargetField { get; set; }

    [DataMember(Name = "field_mappings")]
    IDictionary<Field, Field> FieldMappings { get; set; }

    [DataMember(Name = "inference_config")]
    IInferenceConfig InferenceConfig { get; set; }
  }
}
