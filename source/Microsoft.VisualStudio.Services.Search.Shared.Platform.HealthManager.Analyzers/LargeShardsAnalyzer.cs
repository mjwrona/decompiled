// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.Analyzers.LargeShardsAnalyzer
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
  public class LargeShardsAnalyzer : IAnalyzer
  {
    internal IDictionary<string, string[]> LargeShardMap;
    private readonly ElasticsearchFeedbackProcessor m_elasticsearchFeedbackProcessor;

    public LargeShardsAnalyzer() => this.m_elasticsearchFeedbackProcessor = new ElasticsearchFeedbackProcessor();

    public virtual List<ActionData> Analyze(
      List<HealthData> dataList,
      Dictionary<DataType, ProviderContext> contextDataSet,
      out string result)
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append(FormattableString.Invariant(FormattableStringFactory.Create("Analyzer: {0}.", (object) nameof (LargeShardsAnalyzer))));
      // ISSUE: explicit non-virtual call
      if ((contextDataSet != null ? (!__nonvirtual (contextDataSet.ContainsKey(DataType.ShardSizeData)) ? 1 : 0) : 1) != 0)
      {
        Tracer.TraceError(1083046, "Health Manager", "HealthManagerAnalyzer", FormattableString.Invariant(FormattableStringFactory.Create("Expected {0} not found, required to get shard sizes.", (object) "ShardSizeData")));
        result = stringBuilder.ToString();
        return new List<ActionData>();
      }
      try
      {
        ESDeploymentContext contextData = (ESDeploymentContext) contextDataSet[DataType.ShardSizeData];
        if (contextData.Indices == null)
        {
          Tracer.PublishClientTraceMessage("Health Manager", "HealthManagerAnalyzer", Level.Info, FormattableString.Invariant(FormattableStringFactory.Create("No Indices are present as a part of ask as per {0}. Exiting", (object) "ESDeploymentContext")));
          return new List<ActionData>();
        }
        ShardsSizeData shardsSizeData = (ShardsSizeData) dataList.SingleOrDefault<HealthData>((Func<HealthData, bool>) (x => x.DataType == DataType.ShardSizeData));
        IEntityType entityType = contextData.EntityType;
        Dictionary<string, IEnumerable<KeyValuePair<string, double>>> indexShardsSizes = shardsSizeData?.GetIndexShardsSizes();
        if (indexShardsSizes != null && indexShardsSizes.Any<KeyValuePair<string, IEnumerable<KeyValuePair<string, double>>>>())
        {
          this.LargeShardMap = (IDictionary<string, string[]>) new Dictionary<string, string[]>();
          stringBuilder.Append(FormattableString.Invariant(FormattableStringFactory.Create("Fetched details of {0} indices present in the elasticsearch cluster matching the criteria - Entity Type {1} and Index Name {2}", (object) indexShardsSizes.Count, (object) contextData.EntityType.Name, (object) contextData.Indices.Aggregate<string>((Func<string, string, string>) ((i, j) => i + "," + j)))));
          stringBuilder.Append(FormattableString.Invariant(FormattableStringFactory.Create("Information fetched for Indices :{0} ", (object) indexShardsSizes.Keys.Aggregate<string>((Func<string, string, string>) ((i, j) => i + "," + j)))));
          foreach (string key in indexShardsSizes.Keys)
          {
            IEnumerable<KeyValuePair<string, double>> shardSizes;
            if (indexShardsSizes.TryGetValue(key, out shardSizes))
            {
              string[] array = this.m_elasticsearchFeedbackProcessor.GetExtremelyLargeShards(contextData.RequestContext, entityType, shardSizes).ToArray<string>();
              if (((IEnumerable<string>) array).Any<string>())
                this.LargeShardMap.Add(key, array);
            }
            if (this.LargeShardMap.Any<KeyValuePair<string, string[]>>())
            {
              stringBuilder.Append(FormattableString.Invariant(FormattableStringFactory.Create("{0} indices present in the elasticsearch cluster have large shards", (object) this.LargeShardMap.Count)));
              stringBuilder.Append(FormattableString.Invariant(FormattableStringFactory.Create("Information fetched for Indices :{0}", (object) this.LargeShardMap.Keys.Aggregate<string>((Func<string, string, string>) ((i, j) => i + "," + j)))));
            }
            else
              stringBuilder.Append("No Large Shards present in the indices");
          }
        }
        else
          stringBuilder.Append(FormattableString.Invariant(FormattableStringFactory.Create("There is no Index available in the Elasticsearch cluster matching the asked criteria - Entity Type {0} and Index Name {1}", (object) contextData.EntityType.Name, (object) contextData.Indices.Aggregate<string>((Func<string, string, string>) ((i, j) => i + "," + j)))));
        return new List<ActionData>();
      }
      finally
      {
        result = stringBuilder.ToString();
        Tracer.PublishClientTraceMessage("Health Manager", "HealthManagerAnalyzer", Level.Info, result);
        stringBuilder.Clear();
      }
    }

    public HashSet<DataType> GetDataTypes() => new HashSet<DataType>()
    {
      DataType.ShardSizeData
    };
  }
}
