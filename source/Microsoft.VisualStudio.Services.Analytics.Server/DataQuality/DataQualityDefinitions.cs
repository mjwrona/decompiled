// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.DataQuality.DataQualityDefinitions
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Microsoft.VisualStudio.Services.Analytics.DataQuality
{
  public static class DataQualityDefinitions
  {
    public static readonly DataQualityDefinition[] All;
    public static string MicroserviceModelReadyTestName = "ModelReady";
    public static readonly ReadOnlyDictionary<string, DataQualityDefinition> DataQualityNameToDefinitionDict = new ReadOnlyDictionary<string, DataQualityDefinition>((IDictionary<string, DataQualityDefinition>) new Dictionary<string, DataQualityDefinition>()
    {
      {
        "KanbanColumnStageModelDiffCount",
        new DataQualityDefinition(2, new int?(), "KanbanColumnStageModelDiffCount", "AnalyticsInternal.prc_DQ_KanbanColumn_StageModelCountDiff", 86400)
      },
      {
        "KanbanColumnStageModelMatchedDiffCount",
        new DataQualityDefinition(2, new int?(), "KanbanColumnStageModelMatchedDiffCount", "AnalyticsInternal.prc_DQ_KanbanColumn_StageModelJoinCountDiff", 86400)
      },
      {
        "WorkItemRevisionLatencySeconds",
        new DataQualityDefinition(3, new int?(), "WorkItemRevisionLatencySeconds", "AnalyticsInternal.prc_DQ_WorkItemRevision_ModelLatency", 300, "WorkItemRevisionLatencySeconds", "WorkItemRevision Latency", "Maximum Latency of new Work Item revisions over the previous minute.", shouldExposeWarning: true, warningMessage: (Func<DataQualityResult, string>) (result => AnalyticsResources.WORK_ITEM_REVISION_LATENCY_SECONDS((object) result.ActualValue)))
      },
      {
        "WorkItemRevisionStageModelDiffCount",
        new DataQualityDefinition(3, new int?(), "WorkItemRevisionStageModelDiffCount", "AnalyticsInternal.prc_DQ_WorkItemRevision_StageModelCountDiff", 86400, "WorkItemRevisionStageModelDiffCount", "WorkItemRevision Stage Model Diff Count", "Difference in the number of work item revisions in the stage table and the number in the model table.")
      },
      {
        "BatchUncorrectedFailedTransformCount",
        new DataQualityDefinition(4, new int?(), "BatchUncorrectedFailedTransformCount", "AnalyticsInternal.prc_DQ_Batch_UncorrectedFailedTransformCount", 86400, "BatchUncorrectedFailedTransformCount", "Batch Uncorrected Failed Transform Count", "The count of uncorrected failed transform.")
      },
      {
        "CustomWorkItemRevisionTooManyFields",
        new DataQualityDefinition(6, new int?(), "CustomWorkItemRevisionTooManyFields", "AnalyticsInternal.prc_DQ_CustomWorkItemRevision_TooManyFields", 86400, "CustomWorkItemRevisionTooManyFields", "Reached maximum custom field number", "The count of field types that reached maximum custom columns. 0 is good, anything greater than 0 means new custom table is needed")
      },
      {
        "TablesRowsCount",
        new DataQualityDefinition(7, new int?(), "TablesRowsCount", "AnalyticsInternal.prc_DQ_Tables_RowsCount", 86400, "AnalyticsTablesRowsCount", "Total count of critical tables", "Total count of critical tables used for capacity planning.")
      },
      {
        "CountOfIsLastRevisionOfDayWorkItems",
        new DataQualityDefinition(8, new int?(), "CountOfIsLastRevisionOfDayWorkItems", "AnalyticsInternal.prc_DQ_IsLastRevisionOfDay", 86400, "AnalyticsIsLastRevisionDQCheck", "CDQ IsLastRevision.", "Check for IsLastRevision in WorkItem.")
      },
      {
        "CheckOfGapAndOverlapOfReviedDateAndChangedDate",
        new DataQualityDefinition(8, new int?(), "CheckOfGapAndOverlapOfReviedDateAndChangedDate", "AnalyticsInternal.prc_DQ_RevisedDateAndChangedDateGapAndOverlapCheck", 86400, "AnalyticsDQRevisedDateAndChangedDate", "CDQ RevisedDateAndChangedDate.", "Check Validity of Revised Date and Changed Date.")
      },
      {
        "WorkItemLinkLatencySeconds",
        new DataQualityDefinition(9, new int?(), "WorkItemLinkLatencySeconds", "AnalyticsInternal.prc_DQ_WorkItemLink_ModelLatency", 300, "WorkItemLinkLatencySeconds", "WorkItemLink Latency", "Maximum Latency of new Work Item links over one minute.", shouldExposeWarning: true, warningMessage: (Func<DataQualityResult, string>) (result => AnalyticsResources.WORK_ITEM_LINK_LATENCY_SECONDS((object) result.ActualValue)))
      },
      {
        "TestRunLatencySeconds",
        new DataQualityDefinition(9, new int?(), "TestRunLatencySeconds", "AnalyticsInternal.prc_DQ_TestRun_ModelLatency", 3600, "TestRunLatencySeconds", "TestRun Latency", "Maximum Latency of newly completed test runs over one minute.", shouldExposeWarning: true, warningMessage: (Func<DataQualityResult, string>) (result => AnalyticsResources.TEST_RUN_LATENCY_SECONDS((object) result.ActualValue)))
      },
      {
        "ProjectStageModelDiffCount",
        new DataQualityDefinition(11, new int?(), "ProjectStageModelDiffCount", "AnalyticsInternal.prc_DQ_Project_StageModelCountDiff", 86400, "ProjectStageModelDiffCount", "Project Stage Model Count Diff", "Difference in the number of projects in the stage table and the number in the model table.")
      },
      {
        "WorkItemLinkStageModelCountDiff",
        new DataQualityDefinition(11, new int?(), "WorkItemLinkStageModelCountDiff", "AnalyticsInternal.prc_DQ_WorkItemLink_StageModelCountDiff", 86400, "WorkItemLinkStageModelCountDiff", "Work Item Link Stage Model Diff Count", "Difference in the number of work item links in the stage table and the number in the model table.")
      },
      {
        "TestRunStageModelCountDiff",
        new DataQualityDefinition(12, new int?(), "TestRunStageModelCountDiff", "AnalyticsInternal.prc_DQ_TestRun_StageModelJoinCountDiff", 86400, "TestRunStageModelCountDiff", "Test Run Stage Model Diff Count", "The number of test runs in the stage table not present in the model table, within the retention period.")
      },
      {
        "TableHoldDuration",
        new DataQualityDefinition(14, new int?(), "TableHoldDuration", "AnalyticsInternal.prc_DQ_TableHolds", 900, "TableHoldDurationSeconds", "Table Hold Time", "The duration of transform holds and maintenance mode holds.")
      },
      {
        "ModelReady",
        new DataQualityDefinition(15, new int?(), "ModelReady", "AnalyticsInternal.prc_DQ_ModelReady", 0, "ModelReady", "Model Ready", "Number of dependent transforms that are known to not be loaded.", DataQualityCriterias.ModelReady)
      },
      {
        "TestConfigurationStageModelCountDiff",
        new DataQualityDefinition(16, new int?(), "TestConfigurationStageModelCountDiff", "AnalyticsInternal.prc_DQ_TestConfiguration_StageModelCountDiff", 86400, "TestConfigurationStageModelCountDiff", "Test Configuration Stage Model Diff Count", "Difference in the number of test configurations in the stage table and the number in the model table.", featureFlagToRun: "Analytics.DataQuality.RunManualTestDataQualityTests")
      },
      {
        "TestSuiteStageModelCountDiff",
        new DataQualityDefinition(16, new int?(), "TestSuiteStageModelCountDiff", "AnalyticsInternal.prc_DQ_TestSuite_StageModelCountDiff", 86400, "TestSuiteStageModelCountDiff", "Test Suite Stage Model Diff Count", "Difference in the number of test suites in the stage table and the number in the model table.", featureFlagToRun: "Analytics.DataQuality.RunManualTestDataQualityTests")
      },
      {
        "TestPointStageModelCountDiff",
        new DataQualityDefinition(16, new int?(), "TestPointStageModelCountDiff", "AnalyticsInternal.prc_DQ_TestPoint_StageModelCountDiff", 86400, "TestPointStageModelCountDiff", "Test Point Stage Model Diff Count", "Difference in the number of test suites in the stage table and the number in the model table.", featureFlagToRun: "Analytics.DataQuality.RunManualTestDataQualityTests")
      },
      {
        "PipelineRunStageModelJoinCountDiff",
        new DataQualityDefinition(17, new int?(), "PipelineRunStageModelJoinCountDiff", "AnalyticsInternal.prc_DQ_PipelineRun_StageModelJoinCountDiff", 86400, "PipelineRunStageModelJoinCountDiff", "Pipeline Run Stage Model Join Diff Count", "Difference in number of PipelineRuns present in stage and model table within retention period", featureFlagToRun: "Analytics.DataQuality.PipelineRunDataQualityTests")
      },
      {
        "PipelineRunLatencySeconds",
        new DataQualityDefinition(17, new int?(), "PipelineRunLatencySeconds", "AnalyticsInternal.prc_DQ_PipelineRun_ModelLatency", 3600, "PipelineRunLatencySeconds", "Pipeline Run Stage Model Latency Seconds", "Pipeline Run Model latency in seconds", featureFlagToRun: "Analytics.DataQuality.PipelineRunDataQualityTests")
      },
      {
        "PipelineRunActivityStageModelJoinCountDiff",
        new DataQualityDefinition(17, new int?(), "PipelineRunActivityStageModelJoinCountDiff", "AnalyticsInternal.prc_DQ_PipelineRunActivity_StageModelJoinCountDiff", 86400, "PipelineRunActivityCountDiff", "Pipeline Run Activity Stage Model Join Diff Count", "Difference in number of timeline records present in stage and model table within retention period", featureFlagToRun: "Analytics.DataQuality.PipelineRunActivityDataQualityTests")
      },
      {
        "PipelineRunActivityLatencySeconds",
        new DataQualityDefinition(17, new int?(), "PipelineRunActivityLatencySeconds", "AnalyticsInternal.prc_DQ_PipelineRunActivity_ModelLatency", 3600, "PipelineRunActivityLatencySeconds", "Pipeline Run Activity Latency Seconds", "Pipeline Run Activity latency in seconds", featureFlagToRun: "Analytics.DataQuality.PipelineRunActivityDataQualityTests")
      }
    });

    static DataQualityDefinitions()
    {
      DataQualityDefinitions.All = new DataQualityDefinition[DataQualityDefinitions.DataQualityNameToDefinitionDict.Count];
      int num = 0;
      foreach (KeyValuePair<string, DataQualityDefinition> keyValuePair in DataQualityDefinitions.DataQualityNameToDefinitionDict)
        DataQualityDefinitions.All[num++] = keyValuePair.Value;
    }
  }
}
