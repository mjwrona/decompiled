// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.IndexUtilizationData
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Newtonsoft.Json;
using System;

namespace Microsoft.Azure.Documents
{
  internal sealed class IndexUtilizationData
  {
    [JsonProperty(PropertyName = "FilterExpression")]
    public string FilterExpression { get; }

    [JsonProperty(PropertyName = "IndexSpec")]
    public string IndexDocumentExpression { get; }

    [JsonProperty(PropertyName = "FilterPreciseSet")]
    public bool FilterExpressionPrecision { get; }

    [JsonProperty(PropertyName = "IndexPreciseSet")]
    public bool IndexPlanFullFidelity { get; }

    [JsonConstructor]
    public IndexUtilizationData(
      string filterExpression,
      string indexDocumentExpression,
      bool filterExpressionPrecision,
      bool indexPlanFullFidelity)
    {
      if (filterExpression == null)
        throw new ArgumentNullException(nameof (filterExpression));
      if (indexDocumentExpression == null)
        throw new ArgumentNullException(nameof (indexDocumentExpression));
      this.FilterExpression = filterExpression;
      this.IndexDocumentExpression = indexDocumentExpression;
      this.FilterExpressionPrecision = filterExpressionPrecision;
      this.IndexPlanFullFidelity = indexPlanFullFidelity;
    }
  }
}
