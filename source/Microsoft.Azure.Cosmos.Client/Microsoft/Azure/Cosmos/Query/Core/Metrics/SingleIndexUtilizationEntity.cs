// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Query.Core.Metrics.SingleIndexUtilizationEntity
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Newtonsoft.Json;

namespace Microsoft.Azure.Cosmos.Query.Core.Metrics
{
  internal sealed class SingleIndexUtilizationEntity
  {
    [JsonConstructor]
    public SingleIndexUtilizationEntity(
      string filterExpression,
      string indexDocumentExpression,
      bool filterExpressionPrecision,
      bool indexPlanFullFidelity,
      string indexImpactScore)
    {
      this.FilterExpression = filterExpression;
      this.IndexDocumentExpression = indexDocumentExpression;
      this.FilterExpressionPrecision = filterExpressionPrecision;
      this.IndexPlanFullFidelity = indexPlanFullFidelity;
      this.IndexImpactScore = indexImpactScore;
    }

    [JsonProperty(PropertyName = "FilterExpression")]
    public string FilterExpression { get; }

    [JsonProperty(PropertyName = "IndexSpec")]
    public string IndexDocumentExpression { get; }

    [JsonProperty(PropertyName = "FilterPreciseSet")]
    public bool FilterExpressionPrecision { get; }

    [JsonProperty(PropertyName = "IndexPreciseSet")]
    public bool IndexPlanFullFidelity { get; }

    [JsonProperty(PropertyName = "IndexImpactScore")]
    public string IndexImpactScore { get; }
  }
}
