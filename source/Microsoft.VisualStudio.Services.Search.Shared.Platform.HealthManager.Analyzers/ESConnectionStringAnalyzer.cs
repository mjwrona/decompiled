// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.Analyzers.ESConnectionStringAnalyzer
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.Analyzers, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B1A24B0F-DA40-425D-8B54-1865D1FC90B8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.Analyzers.dll

using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Entities.EntityProperties;
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
  public class ESConnectionStringAnalyzer : IAnalyzer
  {
    public List<ActionData> Analyze(
      List<HealthData> dataList,
      Dictionary<DataType, ProviderContext> contextDataSet,
      out string result)
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append(FormattableString.Invariant(FormattableStringFactory.Create("Analyzer: {0}.", (object) nameof (ESConnectionStringAnalyzer))));
      bool flag = false;
      try
      {
        if (contextDataSet.ContainsKey(DataType.CollectionIndexingUnitData))
        {
          IndexingUnitContext contextData = (IndexingUnitContext) contextDataSet[DataType.CollectionIndexingUnitData];
          IndexingUnitData indexingUnitData1 = (IndexingUnitData) dataList.SingleOrDefault<HealthData>((Func<HealthData, bool>) (x => x.DataType == DataType.CollectionIndexingUnitData));
          ReindexingStatusData reindexingStatusData = (ReindexingStatusData) dataList.SingleOrDefault<HealthData>((Func<HealthData, bool>) (x => x.DataType == DataType.ReindexingStatusData));
          string connectionString = this.GetConnectionString(contextData);
          if (reindexingStatusData == null)
          {
            Tracer.TraceError(1083046, "Health Manager", "HealthManagerAnalyzer", FormattableString.Invariant(FormattableStringFactory.Create("Expected Data:{0} not found for Analyzer:{1}.", (object) "ReindexingStatusData", (object) nameof (ESConnectionStringAnalyzer))));
          }
          else
          {
            IDictionary<IEntityType, ReindexingStatusEntry> reindexingStatus = reindexingStatusData.GetReindexingStatus();
            if (reindexingStatus != null && reindexingStatus.Any<KeyValuePair<IEntityType, ReindexingStatusEntry>>())
            {
              foreach (ReindexingStatusEntry reindexingStatusEntry in (IEnumerable<ReindexingStatusEntry>) reindexingStatus.Values)
              {
                if (reindexingStatusEntry != null)
                  flag = flag || reindexingStatusEntry.IsReindexingFailedOrInProgress();
              }
            }
            if (flag)
              stringBuilder.Append("Re-indexing in progress.");
          }
          if (indexingUnitData1 != null)
          {
            IEnumerable<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> indexingUnitData2 = indexingUnitData1.GetIndexingUnitData();
            List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> list = indexingUnitData2 != null ? indexingUnitData2.ToList<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>() : (List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>) null;
            if (list != null && list.Any<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>())
            {
              foreach (Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit in list)
              {
                CollectionIndexingProperties properties = (CollectionIndexingProperties) indexingUnit.Properties;
                if (!string.IsNullOrWhiteSpace(properties.QueryESConnectionString) && !properties.QueryESConnectionString.Equals(connectionString) && !flag)
                  stringBuilder.Append(FormattableString.Invariant(FormattableStringFactory.Create("Mismatch in {0}: {1} ", (object) "JobAgentSearchPlatformConnectionString", (object) connectionString)) + FormattableString.Invariant(FormattableStringFactory.Create("and QueryESConnectionString:{0} of ", (object) properties.QueryESConnectionString)) + FormattableString.Invariant(FormattableStringFactory.Create("CollectionIndexingUnit:{0},{1} and no re-indexing is in progress.", (object) indexingUnit.IndexingUnitId, (object) indexingUnit.EntityType)));
                else
                  stringBuilder.Append(FormattableString.Invariant(FormattableStringFactory.Create("{0}: {1} ", (object) "JobAgentSearchPlatformConnectionString", (object) connectionString)) + FormattableString.Invariant(FormattableStringFactory.Create("QueryESConnectionString:{0} in CollectionIndexingUnit:{1},{2} are same as expected.", (object) properties.QueryESConnectionString, (object) indexingUnit.IndexingUnitId, (object) indexingUnit.EntityType)));
                if (!string.IsNullOrWhiteSpace(properties.IndexESConnectionString) && !properties.IndexESConnectionString.Equals(connectionString))
                  stringBuilder.Append(FormattableString.Invariant(FormattableStringFactory.Create("Mismatch in {0} ConfigKey: {1} ", (object) "JobAgentSearchPlatformConnectionString", (object) connectionString)) + FormattableString.Invariant(FormattableStringFactory.Create("and IndexESConnectionString:{0} of ", (object) properties.IndexESConnectionString)) + FormattableString.Invariant(FormattableStringFactory.Create("CollectionIndexingUnit:{0},{1}.", (object) indexingUnit.IndexingUnitId, (object) indexingUnit.EntityType)));
                else
                  stringBuilder.Append(FormattableString.Invariant(FormattableStringFactory.Create("{0}: {1} ", (object) "JobAgentSearchPlatformConnectionString", (object) connectionString)) + FormattableString.Invariant(FormattableStringFactory.Create("IndexESConnectionString:{0} in CollectionIndexingUnit:{1},{2} are same as expected.", (object) properties.IndexESConnectionString, (object) indexingUnit.IndexingUnitId, (object) indexingUnit.EntityType)));
              }
            }
            else
              stringBuilder.Append("No collection indexing unit found.");
          }
          else
            Tracer.TraceError(1083046, "Health Manager", "HealthManagerAnalyzer", FormattableString.Invariant(FormattableStringFactory.Create("Expected Data:{0} not found for Analyzer:{1}.", (object) "CollectionIndexingUnitData", (object) nameof (ESConnectionStringAnalyzer))));
          return new List<ActionData>();
        }
        Tracer.TraceError(1083046, "Health Manager", "HealthManagerAnalyzer", FormattableString.Invariant(FormattableStringFactory.Create("Expected {0} not found, required to get deployment connection string.", (object) "IndexingUnitContext")));
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
      DataType.CollectionIndexingUnitData,
      DataType.ReindexingStatusData
    };

    public virtual string GetConnectionString(IndexingUnitContext context) => context != null && context.RequestContext != null ? context.RequestContext.GetConfigValue("/Service/ALMSearch/Settings/JobAgentSearchPlatformConnectionString") ?? string.Empty : string.Empty;
  }
}
