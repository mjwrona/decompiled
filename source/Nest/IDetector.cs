// Decompiled with JetBrains decompiler
// Type: Nest.IDetector
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  [JsonFormatter(typeof (DetectorFormatter))]
  public interface IDetector
  {
    [DataMember(Name = "custom_rules")]
    IEnumerable<IDetectionRule> CustomRules { get; set; }

    [DataMember(Name = "detector_description")]
    string DetectorDescription { get; set; }

    [DataMember(Name = "detector_index")]
    int? DetectorIndex { get; set; }

    [DataMember(Name = "exclude_frequent")]
    Nest.ExcludeFrequent? ExcludeFrequent { get; set; }

    [DataMember(Name = "function")]
    string Function { get; }

    [DataMember(Name = "use_null")]
    bool? UseNull { get; set; }
  }
}
