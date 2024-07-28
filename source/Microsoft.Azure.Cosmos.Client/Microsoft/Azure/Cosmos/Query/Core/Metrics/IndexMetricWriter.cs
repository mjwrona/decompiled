// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Query.Core.Metrics.IndexMetricWriter
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Microsoft.Azure.Cosmos.Query.Core.Metrics
{
  internal class IndexMetricWriter
  {
    private const string IndexUtilizationInfo = "Index Utilization Information";
    private const string UtilizedSingleIndexes = "Utilized Single Indexes";
    private const string PotentialSingleIndexes = "Potential Single Indexes";
    private const string UtilizedCompositeIndexes = "Utilized Composite Indexes";
    private const string PotentialCompositeIndexes = "Potential Composite Indexes";
    private const string IndexExpression = "Index Spec";
    private const string IndexImpactScore = "Index Impact Score";
    private const string IndexUtilizationSeparator = "---";
    private readonly StringBuilder stringBuilder;

    public IndexMetricWriter(StringBuilder stringBuilder) => this.stringBuilder = stringBuilder ?? throw new ArgumentNullException("stringBuilder must not be null.");

    public void WriteIndexMetrics(Microsoft.Azure.Cosmos.Query.Core.Metrics.IndexUtilizationInfo indexUtilizationInfo)
    {
      this.WriteBeforeIndexUtilizationInfo();
      this.WriteIndexUtilizationInfo(indexUtilizationInfo);
      this.WriteAfterIndexUtilizationInfo();
    }

    protected void WriteBeforeIndexUtilizationInfo()
    {
      IndexMetricWriter.AppendNewlineToStringBuilder(this.stringBuilder);
      IndexMetricWriter.AppendHeaderToStringBuilder(this.stringBuilder, "Index Utilization Information", 0);
    }

    protected void WriteIndexUtilizationInfo(Microsoft.Azure.Cosmos.Query.Core.Metrics.IndexUtilizationInfo indexUtilizationInfo)
    {
      IndexMetricWriter.AppendHeaderToStringBuilder(this.stringBuilder, "Utilized Single Indexes", 1);
      foreach (SingleIndexUtilizationEntity utilizedSingleIndex in (IEnumerable<SingleIndexUtilizationEntity>) indexUtilizationInfo.UtilizedSingleIndexes)
        WriteSingleIndexUtilizationEntity(utilizedSingleIndex);
      IndexMetricWriter.AppendHeaderToStringBuilder(this.stringBuilder, "Potential Single Indexes", 1);
      foreach (SingleIndexUtilizationEntity potentialSingleIndex in (IEnumerable<SingleIndexUtilizationEntity>) indexUtilizationInfo.PotentialSingleIndexes)
        WriteSingleIndexUtilizationEntity(potentialSingleIndex);
      IndexMetricWriter.AppendHeaderToStringBuilder(this.stringBuilder, "Utilized Composite Indexes", 1);
      foreach (CompositeIndexUtilizationEntity utilizedCompositeIndex in (IEnumerable<CompositeIndexUtilizationEntity>) indexUtilizationInfo.UtilizedCompositeIndexes)
        WriteCompositeIndexUtilizationEntity(utilizedCompositeIndex);
      IndexMetricWriter.AppendHeaderToStringBuilder(this.stringBuilder, "Potential Composite Indexes", 1);
      foreach (CompositeIndexUtilizationEntity potentialCompositeIndex in (IEnumerable<CompositeIndexUtilizationEntity>) indexUtilizationInfo.PotentialCompositeIndexes)
        WriteCompositeIndexUtilizationEntity(potentialCompositeIndex);

      void WriteSingleIndexUtilizationEntity(
        SingleIndexUtilizationEntity indexUtilizationEntity)
      {
        IndexMetricWriter.AppendHeaderToStringBuilder(this.stringBuilder, "Index Spec: " + indexUtilizationEntity.IndexDocumentExpression, 2);
        IndexMetricWriter.AppendHeaderToStringBuilder(this.stringBuilder, "Index Impact Score: " + indexUtilizationEntity.IndexImpactScore, 2);
        IndexMetricWriter.AppendHeaderToStringBuilder(this.stringBuilder, "---", 2);
      }

      void WriteCompositeIndexUtilizationEntity(
        CompositeIndexUtilizationEntity indexUtilizationEntity)
      {
        IndexMetricWriter.AppendHeaderToStringBuilder(this.stringBuilder, "Index Spec: " + string.Join(", ", (IEnumerable<string>) indexUtilizationEntity.IndexDocumentExpressions), 2);
        IndexMetricWriter.AppendHeaderToStringBuilder(this.stringBuilder, "Index Impact Score: " + indexUtilizationEntity.IndexImpactScore, 2);
        IndexMetricWriter.AppendHeaderToStringBuilder(this.stringBuilder, "---", 2);
      }
    }

    protected void WriteAfterIndexUtilizationInfo()
    {
    }

    private static void AppendHeaderToStringBuilder(
      StringBuilder stringBuilder,
      string headerTitle,
      int indentLevel)
    {
      stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "{0}{1}", (object) (string.Concat(Enumerable.Repeat<string>("  ", indentLevel)) + headerTitle), (object) Environment.NewLine);
    }

    private static void AppendNewlineToStringBuilder(StringBuilder stringBuilder) => IndexMetricWriter.AppendHeaderToStringBuilder(stringBuilder, string.Empty, 0);
  }
}
