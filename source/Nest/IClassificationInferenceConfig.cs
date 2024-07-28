// Decompiled with JetBrains decompiler
// Type: Nest.IClassificationInferenceConfig
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Runtime.Serialization;

namespace Nest
{
  [ReadAs(typeof (ClassificationInferenceConfig))]
  public interface IClassificationInferenceConfig
  {
    [DataMember(Name = "results_field")]
    Field ResultsField { get; set; }

    [DataMember(Name = "num_top_classes")]
    int? NumTopClasses { get; set; }

    [DataMember(Name = "top_classes_results_field")]
    Field TopClassesResultsField { get; set; }
  }
}
