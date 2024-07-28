// Decompiled with JetBrains decompiler
// Type: Nest.IProcessor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  [InterfaceDataContract]
  [JsonFormatter(typeof (ProcessorFormatter))]
  public interface IProcessor
  {
    [IgnoreDataMember]
    string Name { get; }

    [DataMember(Name = "description")]
    string Description { get; set; }

    [DataMember(Name = "on_failure")]
    IEnumerable<IProcessor> OnFailure { get; set; }

    [DataMember(Name = "if")]
    string If { get; set; }

    [DataMember(Name = "tag")]
    string Tag { get; set; }

    [DataMember(Name = "ignore_failure")]
    bool? IgnoreFailure { get; set; }
  }
}
