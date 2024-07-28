// Decompiled with JetBrains decompiler
// Type: Microsoft.Cloud.Metrics.Client.MetricEnrichmentRuleManagement.MetricEnrichmentRuleTransformationDefinition
// Assembly: Microsoft.Cloud.Metrics.Client, Version=2.2023.705.2051, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 06B39E1C-7DF0-4BC1-AFBA-9AD635E73CB0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Cloud.Metrics.Client.dll

using System.Collections.Generic;

namespace Microsoft.Cloud.Metrics.Client.MetricEnrichmentRuleManagement
{
  public sealed class MetricEnrichmentRuleTransformationDefinition
  {
    public MetricEnrichmentRuleTransformationDefinition(
      string executorId,
      List<string> sourceEventDimensionNamesForKey,
      string executorConnectionStringOverride,
      MetricEnrichmentTransformationType transformationType,
      Dictionary<string, string> destinationColumnNamesForDimensions)
    {
      this.ExecutorId = executorId;
      this.SourceEventDimensionNamesForKey = sourceEventDimensionNamesForKey;
      this.ExecutorConnectionStringOverride = executorConnectionStringOverride;
      this.TransformationType = transformationType;
      this.DestinationColumnNamesForDimensions = destinationColumnNamesForDimensions;
    }

    public string ExecutorId { get; }

    public List<string> SourceEventDimensionNamesForKey { get; }

    public string ExecutorConnectionStringOverride { get; }

    public MetricEnrichmentTransformationType TransformationType { get; }

    public Dictionary<string, string> DestinationColumnNamesForDimensions { get; }

    internal string Validate() => string.IsNullOrEmpty(this.ExecutorId) ? "Executor id cannot be null" : string.Empty;
  }
}
