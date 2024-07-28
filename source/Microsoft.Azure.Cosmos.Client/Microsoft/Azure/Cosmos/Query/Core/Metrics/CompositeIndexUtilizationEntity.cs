// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Query.Core.Metrics.CompositeIndexUtilizationEntity
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Newtonsoft.Json;
using System.Collections.Generic;

namespace Microsoft.Azure.Cosmos.Query.Core.Metrics
{
  internal sealed class CompositeIndexUtilizationEntity
  {
    [JsonConstructor]
    public CompositeIndexUtilizationEntity(
      IReadOnlyList<string> indexDocumentExpressions,
      bool indexPlanFullFidelity,
      string indexImpactScore)
    {
      this.IndexDocumentExpressions = indexDocumentExpressions;
      this.IndexPlanFullFidelity = indexPlanFullFidelity;
      this.IndexImpactScore = indexImpactScore;
    }

    [JsonProperty(PropertyName = "IndexSpecs")]
    public IReadOnlyList<string> IndexDocumentExpressions { get; }

    [JsonProperty(PropertyName = "IndexPreciseSet")]
    public bool IndexPlanFullFidelity { get; }

    [JsonProperty(PropertyName = "IndexImpactScore")]
    public string IndexImpactScore { get; }
  }
}
