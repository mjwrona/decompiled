// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.Analyzers.EsDeletedDocumentCountAnalyzer
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.Analyzers, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B1A24B0F-DA40-425D-8B54-1865D1FC90B8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.Analyzers.dll

using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Indexer;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.Data;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManagerAPI;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using Microsoft.VisualStudio.Services.WebPlatform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.Analyzers
{
  public class EsDeletedDocumentCountAnalyzer : IAnalyzer
  {
    private readonly ElasticsearchFeedbackProcessor m_elasticsearchFeedbackProcessor;
    private IEnumerable<string> m_indicesWithHighDeletedDocs;

    public EsDeletedDocumentCountAnalyzer() => this.m_elasticsearchFeedbackProcessor = new ElasticsearchFeedbackProcessor();

    public List<ActionData> Analyze(
      List<HealthData> dataList,
      Dictionary<DataType, ProviderContext> contextDataSet,
      out string result)
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append(FormattableString.Invariant(FormattableStringFactory.Create("Analyzer: {0}.", (object) nameof (EsDeletedDocumentCountAnalyzer))));
      if (contextDataSet != null)
      {
        if (contextDataSet.ContainsKey(DataType.SearchIndexDocumentCountData))
        {
          try
          {
            ESDeploymentContext context = (ESDeploymentContext) contextDataSet[DataType.SearchIndexDocumentCountData];
            if (context.Indices == null)
            {
              Tracer.PublishClientTraceMessage("Health Manager", "HealthManagerAnalyzer", Level.Info, FormattableString.Invariant(FormattableStringFactory.Create("No Indices are present as a part of ask as per {0}. Exiting", (object) "ESDeploymentContext")));
              return new List<ActionData>();
            }
            IEnumerable<ElasticsearchIndexDetail> indexDocCountDetails = ((SearchIndexDocumentCountData) dataList.Single<HealthData>((Func<HealthData, bool>) (x => x.DataType == DataType.SearchIndexDocumentCountData))).GetElasticsearchIndexDocCountDetails();
            if (indexDocCountDetails == null || !indexDocCountDetails.Any<ElasticsearchIndexDetail>())
            {
              stringBuilder.Append(FormattableString.Invariant(FormattableStringFactory.Create("There is no Index available in the Elasticsearch cluster matching the asked criteria - ")) + FormattableString.Invariant(FormattableStringFactory.Create("Entity Type {0} and Index Name {1}", (object) context.EntityType.Name, (object) context.Indices.Aggregate<string>((Func<string, string, string>) ((i, j) => i + "," + j)))));
              return new List<ActionData>();
            }
            this.m_indicesWithHighDeletedDocs = indexDocCountDetails.Where<ElasticsearchIndexDetail>((Func<ElasticsearchIndexDetail, bool>) (it => this.m_elasticsearchFeedbackProcessor.IsEsIndexWithHighDeletedDocsPercentage(context.RequestContext, it))).Select<ElasticsearchIndexDetail, string>((Func<ElasticsearchIndexDetail, string>) (it => it.IndexName));
            if (this.m_indicesWithHighDeletedDocs.Any<string>())
              stringBuilder.Append(FormattableString.Invariant(FormattableStringFactory.Create("Indices :  {0} ", (object) this.m_indicesWithHighDeletedDocs.Aggregate<string>((Func<string, string, string>) ((i, j) => i + "," + j)))) + FormattableString.Invariant(FormattableStringFactory.Create("recognized for indices having high deleted document count for the asked criteria : - Entity Type ")) + FormattableString.Invariant(FormattableStringFactory.Create("{0} and Index Name ", (object) context.EntityType.Name)) + FormattableString.Invariant(FormattableStringFactory.Create("{0}", (object) context.Indices.Aggregate<string>((Func<string, string, string>) ((i, j) => i + "," + j)))));
            else
              stringBuilder.Append("No Index having high deleted document Count");
            return new List<ActionData>();
          }
          finally
          {
            result = stringBuilder.ToString();
            Tracer.PublishClientTraceMessage("Health Manager", "HealthManagerAnalyzer", Level.Info, result);
            stringBuilder.Clear();
          }
        }
      }
      Tracer.TraceError(1083046, "Health Manager", "HealthManagerAnalyzer", FormattableString.Invariant(FormattableStringFactory.Create("Expected {0} not found, required to get document count in indexes.", (object) "SearchIndexDocumentCountData")));
      result = stringBuilder.ToString();
      return new List<ActionData>();
    }

    public HashSet<DataType> GetDataTypes() => new HashSet<DataType>()
    {
      DataType.SearchIndexDocumentCountData
    };
  }
}
