// Decompiled with JetBrains decompiler
// Type: Nest.AnomalyDetectors
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Runtime.Serialization;

namespace Nest
{
  public class AnomalyDetectors
  {
    [DataMember(Name = "model_memory_limit")]
    public string ModelMemoryLimit { get; internal set; }

    [DataMember(Name = "categorization_examples_limit")]
    public int CategorizationExamplesLimit { get; internal set; }

    [DataMember(Name = "model_snapshot_retention_days")]
    public int ModelSnapshotRetentionDays { get; internal set; }

    [DataMember(Name = "daily_model_snapshot_retention_after_days")]
    public long DailyModelSnapshotRetentionAfterDays { get; internal set; }

    [DataMember(Name = "categorization_analyzer")]
    public CategorizationAnalyzer CategorizationAnalyzer { get; internal set; }
  }
}
