// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.Analyzers.RoutingInfoAnalyzer
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.Analyzers, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B1A24B0F-DA40-425D-8B54-1865D1FC90B8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.Analyzers.dll

using Microsoft.VisualStudio.Services.Search.Common;
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
  public class RoutingInfoAnalyzer : IAnalyzer
  {
    private Guid m_collectionId;

    public virtual List<ActionData> Analyze(
      List<HealthData> dataList,
      Dictionary<DataType, ProviderContext> contextDataSet,
      out string result)
    {
      List<ActionData> actionDataList = new List<ActionData>();
      StringBuilder infoMessage = new StringBuilder();
      infoMessage.Append(FormattableString.Invariant(FormattableStringFactory.Create("Analyzer: {0}.", (object) nameof (RoutingInfoAnalyzer))));
      IndexingUnitContext indexingUnitContext = (IndexingUnitContext) null;
      try
      {
        if (contextDataSet.ContainsKey(DataType.CollectionIndexingUnitData))
          indexingUnitContext = (IndexingUnitContext) contextDataSet[DataType.CollectionIndexingUnitData];
        IndexingUnitData indexingUnitData1 = (IndexingUnitData) dataList.SingleOrDefault<HealthData>((Func<HealthData, bool>) (x => x.DataType == DataType.CollectionIndexingUnitData));
        ReindexingStatusData reindexingStatusData = (ReindexingStatusData) dataList.SingleOrDefault<HealthData>((Func<HealthData, bool>) (x => x.DataType == DataType.ReindexingStatusData));
        IndexingUnitData repositoryIndexingUnitData = (IndexingUnitData) dataList.SingleOrDefault<HealthData>((Func<HealthData, bool>) (x => x.DataType == DataType.RepoIndexingUnitData));
        IndexingUnitData scopedIndexingUnitData = (IndexingUnitData) dataList.SingleOrDefault<HealthData>((Func<HealthData, bool>) (x => x.DataType == DataType.ScopedIndexingUnitData));
        if (indexingUnitData1 != null)
        {
          infoMessage.Append(FormattableString.Invariant(FormattableStringFactory.Create("Analyzing dataType: {0}.", (object) "CollectionIndexingUnitData")));
          IEnumerable<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> indexingUnitData2 = indexingUnitData1.GetIndexingUnitData();
          List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> list1 = indexingUnitData2 != null ? indexingUnitData2.ToList<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>() : (List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>) null;
          if (list1 != null && list1.Any<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>())
          {
            if (indexingUnitContext != null)
            {
              ExecutionTracerContext executionTracerContext = new ExecutionTracerContext(TracerCICorrelationDetailsFactory.GetCICorrelationDetails(indexingUnitContext.RequestContext, nameof (RoutingInfoAnalyzer), 50));
              switch (indexingUnitContext.EntityType.Name)
              {
                case "WorkItem":
                case "Wiki":
                  infoMessage.Append(FormattableString.Invariant(FormattableStringFactory.Create("Analyzing routing info of the collection for entity type: {0} is not implemented.", (object) indexingUnitContext.EntityType.Name)));
                  break;
                default:
                  List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> list2 = list1.Where<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>((Func<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, bool>) (x => x.EntityType.Name == "Code")).ToList<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>();
                  if (list2.Count == 1)
                  {
                    this.m_collectionId = list2[0].TFSEntityId;
                    infoMessage.Append(FormattableString.Invariant(FormattableStringFactory.Create("Collection: IndexingUnitId:{0}, entity type: {1}.", (object) list2[0].IndexingUnitId, (object) "Code")));
                    ReindexingStatusEntry reindexingStatu = reindexingStatusData?.GetReindexingStatus()[(IEntityType) CodeEntityType.GetInstance()];
                    if (reindexingStatu != null)
                    {
                      infoMessage.Append(FormattableString.Invariant(FormattableStringFactory.Create("Re-indexing status for EntityType: {0} is {1}.", (object) "Code", (object) reindexingStatu.Status.ToString())));
                      actionDataList.AddRange((IEnumerable<ActionData>) this.AnalyzeCodeRoutingInfo(list2[0], repositoryIndexingUnitData, scopedIndexingUnitData, reindexingStatu.IsReindexingFailedOrInProgress(), infoMessage, executionTracerContext));
                      break;
                    }
                    infoMessage.Append(FormattableString.Invariant(FormattableStringFactory.Create("No entry found in re-indexing status table of the collection of entity type: {0}. Continuing as re-indexing status not required.", (object) "Code")));
                    actionDataList.AddRange((IEnumerable<ActionData>) this.AnalyzeCodeRoutingInfo(list2[0], repositoryIndexingUnitData, scopedIndexingUnitData, false, infoMessage, executionTracerContext));
                    break;
                  }
                  infoMessage.Append(FormattableString.Invariant(FormattableStringFactory.Create("Unexpected no. of collection indexing unit ({0}) found  for entity {1}.", (object) list2.Count, (object) "Code")));
                  break;
              }
            }
            else
              Tracer.TraceError(1083046, "Health Manager", "HealthManagerAnalyzer", FormattableString.Invariant(FormattableStringFactory.Create("Expected context: {0} for dataType:{1} not found.", (object) "IndexingUnitContext", (object) "CollectionIndexingUnitData")));
          }
          else
            infoMessage.Append("No Collection Indexing Unit found.");
        }
        else
          Tracer.TraceError(1083046, "Health Manager", "HealthManagerAnalyzer", FormattableString.Invariant(FormattableStringFactory.Create("Expected Data:{0} not found for Analyzer:{1} to proceed.", (object) "CollectionIndexingUnitData", (object) nameof (RoutingInfoAnalyzer))));
        return actionDataList;
      }
      finally
      {
        result = infoMessage.ToString();
        Tracer.PublishClientTraceMessage("Health Manager", "HealthManagerAnalyzer", Level.Info, result);
        infoMessage.Clear();
      }
    }

    public virtual HashSet<DataType> GetDataTypes() => new HashSet<DataType>()
    {
      DataType.CollectionIndexingUnitData,
      DataType.RepoIndexingUnitData,
      DataType.ReindexingStatusData,
      DataType.ScopedIndexingUnitData
    };

    private List<ActionData> AnalyzeCodeRoutingInfo(
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit collectionIndexingUnit,
      IndexingUnitData repositoryIndexingUnitData,
      IndexingUnitData scopedIndexingUnitData,
      bool isReindexingInProgress,
      StringBuilder infoMessage,
      ExecutionTracerContext executionTracerContext)
    {
      List<ActionData> actionDataList = new List<ActionData>();
      bool isCollectionFinalizeRequired = false;
      IEnumerable<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> indexingUnits = (IEnumerable<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>) new List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>();
      IDictionary<int, List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>> dictionary1 = (IDictionary<int, List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>>) new Dictionary<int, List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>>();
      if (repositoryIndexingUnitData != null)
        indexingUnits = repositoryIndexingUnitData.GetIndexingUnitData().Where<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>((Func<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, bool>) (x => x.EntityType.Name == "Code"));
      if (scopedIndexingUnitData != null)
        dictionary1 = (IDictionary<int, List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>>) scopedIndexingUnitData.GetIndexingUnitData().Where<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>((Func<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, bool>) (x => x.EntityType.Name == "Code")).GroupBy<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, int>((Func<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, int>) (x => x.ParentUnitId)).ToDictionary<IGrouping<int, Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>, int, List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>>((Func<IGrouping<int, Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>, int>) (x => x.Key), (Func<IGrouping<int, Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>, List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>>) (x => x.ToList<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>()));
      List<IndexInfo> source1;
      List<IndexInfo> source2;
      if (isReindexingInProgress)
      {
        infoMessage.Append("Re-indexing is in progress, unable to determine the correct state before Collection-Finalize completes. Analyzing on pre-reindexing state.");
        source1 = collectionIndexingUnit.Properties.IndexIndicesPreReindexing;
        source2 = collectionIndexingUnit.Properties.QueryIndicesPreReindexing;
      }
      else
      {
        source1 = collectionIndexingUnit.Properties.IndexIndices;
        source2 = collectionIndexingUnit.Properties.QueryIndices;
      }
      if (source2 == null || source2.Count == 0)
        infoMessage.Append("No query index present in collection indexing unit.");
      else if (source2.Count == 1)
      {
        infoMessage.Append(FormattableString.Invariant(FormattableStringFactory.Create("One or More QueryIndexInfo present in collection indexing unit. No of QueryIndexInfo {0}.", (object) source2.Count)));
        if (!isReindexingInProgress && source2.Count<IndexInfo>() == 1 && (source1 == null || source1.Count != source2.Count<IndexInfo>() || source2.FirstOrDefault<IndexInfo>().IndexName == null || !source2.FirstOrDefault<IndexInfo>().IndexName.Equals(source1.FirstOrDefault<IndexInfo>().IndexName)))
        {
          executionTracerContext.PublishCi("Health Manager", "HealthManagerAnalyzer", "MismatchedCollectionQueryIndexName", source2.FirstOrDefault<IndexInfo>().IndexName);
          infoMessage.Append(FormattableString.Invariant(FormattableStringFactory.Create("Mismatch in Query and Indexing index info. ")) + FormattableString.Invariant(FormattableStringFactory.Create("Collection-Indexing IndexNames: {0}. ", source1 != null ? (object) source1.FirstOrDefault<IndexInfo>()?.IndexName : (object) (string) null)) + FormattableString.Invariant(FormattableStringFactory.Create("Collection-QueryIndexing IndexNames: {0}. ", (object) source2.FirstOrDefault<IndexInfo>().IndexName)) + FormattableString.Invariant(FormattableStringFactory.Create("Need to be fixed manually as automatic resolution not exists.")));
        }
        IDictionary<string, IndexInfo> dictionary2 = (IDictionary<string, IndexInfo>) new Dictionary<string, IndexInfo>();
        IDictionary<string, List<string>> dictionary3 = (IDictionary<string, List<string>>) new Dictionary<string, List<string>>();
        IDictionary<string, IndexInfo> dictionary4 = (IDictionary<string, IndexInfo>) source2.Where<IndexInfo>((Func<IndexInfo, bool>) (x => x.IndexName != null)).GroupBy<IndexInfo, string>((Func<IndexInfo, string>) (x => x.IndexName)).ToDictionary<IGrouping<string, IndexInfo>, string, IndexInfo>((Func<IGrouping<string, IndexInfo>, string>) (x => x.Key), (Func<IGrouping<string, IndexInfo>, IndexInfo>) (x => x.FirstOrDefault<IndexInfo>()));
        IDictionary<string, List<string>> dictionary5 = (IDictionary<string, List<string>>) source2.Where<IndexInfo>((Func<IndexInfo, bool>) (x => x.IndexName != null)).GroupBy<IndexInfo, string>((Func<IndexInfo, string>) (x => x.IndexName)).ToDictionary<IGrouping<string, IndexInfo>, string, List<string>>((Func<IGrouping<string, IndexInfo>, string>) (x => x.Key), (Func<IGrouping<string, IndexInfo>, List<string>>) (x =>
        {
          string routing = x.FirstOrDefault<IndexInfo>().Routing;
          if (routing == null)
            return (List<string>) null;
          return ((IEnumerable<string>) routing.Split(new char[1]
          {
            ','
          }, StringSplitOptions.RemoveEmptyEntries)).ToList<string>();
        }));
        List<string> stringList1 = new List<string>();
        foreach (string key in (IEnumerable<string>) dictionary5.Keys)
        {
          List<string> collection = dictionary5[key];
          if (collection != null)
            stringList1.AddRange((IEnumerable<string>) collection);
        }
        if (stringList1.Count == 0)
        {
          infoMessage.Append(FormattableString.Invariant(FormattableStringFactory.Create("Mismatch in RoutingIds: No routing ids's found in collection ")) + FormattableString.Invariant(FormattableStringFactory.Create("Query Indexing Info for {0}. ", (object) string.Join(", ", (IEnumerable<string>) dictionary5.Keys))) + FormattableString.Invariant(FormattableStringFactory.Create("Recommending:{0}.", (object) ActionType.CollectionFinalize)));
          executionTracerContext.PublishCi("Health Manager", "HealthManagerAnalyzer", "IndexUnitIdWithNoRoutingIds", (double) collectionIndexingUnit.IndexingUnitId);
          actionDataList.Add(new ActionData(ActionType.CollectionFinalize, new ActionContext()
          {
            EntityType = (IEntityType) CodeEntityType.GetInstance(),
            CollectionIds = new List<Guid>()
            {
              this.m_collectionId
            }
          }));
          return actionDataList;
        }
        List<string> second1 = new List<string>();
        foreach (Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexUnit1 in indexingUnits)
        {
          if (indexUnit1.GetIndexInfo()?.IndexName != null && dictionary4.ContainsKey(indexUnit1.GetIndexInfo().IndexName))
          {
            IndexInfo parentIndexInfo = dictionary4[indexUnit1.GetIndexInfo().IndexName];
            List<string> parentRoutingIds = dictionary5[parentIndexInfo.IndexName];
            infoMessage.Append(FormattableString.Invariant(FormattableStringFactory.Create("Collection-QueryIndex: IndexName: {0},", (object) parentIndexInfo.IndexName)) + FormattableString.Invariant(FormattableStringFactory.Create("Routing: {0}.", (object) parentIndexInfo.Routing)));
            List<string> stringList2 = this.CompareRoutingInfo(isReindexingInProgress, infoMessage, ref isCollectionFinalizeRequired, indexUnit1, parentIndexInfo, parentRoutingIds, collectionIndexingUnit.IndexingUnitType, executionTracerContext);
            if (stringList2 != null)
              second1.AddRange((IEnumerable<string>) stringList2);
            if (!indexUnit1.IndexingUnitType.Equals("CustomRepository"))
            {
              List<string> second2 = new List<string>();
              if (dictionary1.ContainsKey(indexUnit1.IndexingUnitId))
              {
                foreach (Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexUnit2 in dictionary1[indexUnit1.IndexingUnitId])
                {
                  List<string> collection = this.CompareRoutingInfo(isReindexingInProgress, infoMessage, ref isCollectionFinalizeRequired, indexUnit2, indexUnit1.GetIndexInfo(), stringList2, indexUnit1.IndexingUnitType, executionTracerContext);
                  if (collection != null)
                    second2.AddRange((IEnumerable<string>) collection);
                  if (isCollectionFinalizeRequired)
                    break;
                }
              }
              if (second2.Count > 0 && stringList2 != null && stringList2.Except<string>((IEnumerable<string>) second2).Any<string>())
              {
                infoMessage.Append(FormattableString.Invariant(FormattableStringFactory.Create("Mismatch in RoutingIds: Routing Id {0} ", (object) string.Join(", ", (IEnumerable<string>) stringList2))) + FormattableString.Invariant(FormattableStringFactory.Create("of Repo id: {0} and iuid:{1}  is ", (object) indexUnit1.TFSEntityId, (object) indexUnit1.IndexingUnitId)) + FormattableString.Invariant(FormattableStringFactory.Create("not present in ScopedIndexing unit. Recommending:{0}.", (object) ActionType.CollectionFinalize)));
                FriendlyDictionary<string, object> properties = new FriendlyDictionary<string, object>()
                {
                  ["RoutingIdsNotPresentInChildren"] = (object) string.Join(", ", stringList2.Except<string>((IEnumerable<string>) second2)),
                  ["IndexingUnitId"] = (object) indexUnit1.IndexingUnitId
                };
                executionTracerContext.PublishCi("Health Manager", "HealthManagerAnalyzer", (IDictionary<string, object>) properties);
                isCollectionFinalizeRequired = true;
                break;
              }
            }
          }
          else
          {
            infoMessage.Append(FormattableString.Invariant(FormattableStringFactory.Create("Mismatch in Collection Query and Repo Indexing info. ")) + FormattableString.Invariant(FormattableStringFactory.Create("Repo-Indexing IndexName: {0}. of {1} ", (object) indexUnit1.GetIndexInfo()?.IndexName, (object) indexUnit1.IndexingUnitType)) + FormattableString.Invariant(FormattableStringFactory.Create("id: {0} and iuid:{1}. ", (object) indexUnit1.TFSEntityId, (object) indexUnit1.IndexingUnitId)) + FormattableString.Invariant(FormattableStringFactory.Create("Need to be fixed manually as automatic resolution not exists.")));
            FriendlyDictionary<string, object> properties = new FriendlyDictionary<string, object>()
            {
              ["MismatchedIndexNameWithParent"] = (object) indexUnit1.GetIndexInfo()?.IndexName,
              ["IndexingUnitId"] = (object) indexUnit1.IndexingUnitId
            };
            executionTracerContext.PublishCi("Health Manager", "HealthManagerAnalyzer", (IDictionary<string, object>) properties);
          }
        }
        if (stringList1.Except<string>((IEnumerable<string>) second1).Any<string>())
        {
          infoMessage.Append(FormattableString.Invariant(FormattableStringFactory.Create("Mismatch in RoutingIds: Routing Id {0} ", (object) string.Join(", ", (IEnumerable<string>) stringList1))) + FormattableString.Invariant(FormattableStringFactory.Create("of collection id: {0} and iuid:{1}", (object) collectionIndexingUnit.TFSEntityId, (object) collectionIndexingUnit.IndexingUnitId)) + FormattableString.Invariant(FormattableStringFactory.Create("  is not present in Repo indexinging units. Recommending:{0}.", (object) ActionType.CollectionFinalize)));
          FriendlyDictionary<string, object> properties = new FriendlyDictionary<string, object>()
          {
            ["RoutingIdsNotPresentInChildren"] = (object) string.Join(", ", stringList1.Except<string>((IEnumerable<string>) second1)),
            ["IndexingUnitId"] = (object) collectionIndexingUnit.IndexingUnitId
          };
          executionTracerContext.PublishCi("Health Manager", "HealthManagerAnalyzer", (IDictionary<string, object>) properties);
          isCollectionFinalizeRequired = true;
        }
      }
      else
        infoMessage.Append("More than one QueryIndexInfo present in collection indexing unit. Expected in case of collections with large repos.");
      if (isCollectionFinalizeRequired)
        actionDataList.Add(new ActionData(ActionType.CollectionFinalize, new ActionContext()
        {
          EntityType = (IEntityType) CodeEntityType.GetInstance(),
          CollectionIds = new List<Guid>()
          {
            this.m_collectionId
          }
        }));
      return actionDataList;
    }

    private List<string> CompareRoutingInfo(
      bool isReindexingInProgress,
      StringBuilder infoMessage,
      ref bool isCollectionFinalizeRequired,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexUnit,
      IndexInfo parentIndexInfo,
      List<string> parentRoutingIds,
      string parentIndexUnitType,
      ExecutionTracerContext executionTracerContext)
    {
      IndexInfo indexInfo;
      if (isReindexingInProgress)
      {
        List<IndexInfo> indicesPreReindexing = indexUnit.Properties.IndexIndicesPreReindexing;
        // ISSUE: explicit non-virtual call
        indexInfo = (indicesPreReindexing != null ? (__nonvirtual (indicesPreReindexing.Count) > 0 ? 1 : 0) : 0) != 0 ? indexUnit.Properties.IndexIndicesPreReindexing?[0] : (IndexInfo) null;
      }
      else
        indexInfo = indexUnit.GetIndexInfo();
      List<string> stringList;
      if (indexInfo == null)
      {
        stringList = (List<string>) null;
      }
      else
      {
        string routing = indexInfo.Routing;
        if (routing == null)
          stringList = (List<string>) null;
        else
          stringList = ((IEnumerable<string>) routing.Split(new char[1]
          {
            ','
          }, StringSplitOptions.RemoveEmptyEntries)).ToList<string>();
      }
      List<string> first = stringList;
      if (parentIndexInfo.IndexName != indexInfo?.IndexName)
      {
        infoMessage.Append(FormattableString.Invariant(FormattableStringFactory.Create("Mismatch in Indexing info. Index name {0} of {1} id: {2} ", (object) indexInfo?.IndexName, (object) indexUnit.IndexingUnitType, (object) indexUnit.TFSEntityId)) + FormattableString.Invariant(FormattableStringFactory.Create("and iuid:{0}  is not present in {1}. ", (object) indexUnit.IndexingUnitId, (object) parentIndexUnitType)) + FormattableString.Invariant(FormattableStringFactory.Create("Need to be fixed manually as automatic resolution not exists.")));
        FriendlyDictionary<string, object> properties = new FriendlyDictionary<string, object>()
        {
          ["MismatchedIndexNameWithParent"] = (object) indexInfo?.IndexName,
          ["IndexingUnitId"] = (object) indexUnit.IndexingUnitId
        };
        executionTracerContext.PublishCi("Health Manager", "HealthManagerAnalyzer", (IDictionary<string, object>) properties);
      }
      if (parentRoutingIds == null)
        parentRoutingIds = new List<string>();
      if (first != null && first.Except<string>((IEnumerable<string>) parentRoutingIds).Any<string>())
      {
        infoMessage.Append(FormattableString.Invariant(FormattableStringFactory.Create("Mismatch in RoutingIds: Routing Id {0} of {1} id: {2} ", (object) indexInfo.Routing, (object) indexUnit.IndexingUnitType, (object) indexUnit.TFSEntityId)) + FormattableString.Invariant(FormattableStringFactory.Create("and iuid:{0} is not present in {1}. Recommending:{2}.", (object) indexUnit.IndexingUnitId, (object) parentIndexUnitType, (object) ActionType.CollectionFinalize)));
        FriendlyDictionary<string, object> properties = new FriendlyDictionary<string, object>()
        {
          ["RoutingIdsNotPresentInParent"] = (object) string.Join(", ", first.Except<string>((IEnumerable<string>) parentRoutingIds)),
          ["IndexingUnitId"] = (object) indexUnit.IndexingUnitId
        };
        executionTracerContext.PublishCi("Health Manager", "HealthManagerAnalyzer", (IDictionary<string, object>) properties);
        isCollectionFinalizeRequired = true;
      }
      return first;
    }
  }
}
