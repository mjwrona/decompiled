// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Common.WorkItemTrackingQueryExecutionLoggingConfiguration
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Common
{
  public class WorkItemTrackingQueryExecutionLoggingConfiguration
  {
    public readonly int DetailTableCutoffDaysOffset;
    public readonly int DetailTableMaxRowCount;
    public readonly int InfoTableCutoffDaysOffset;
    public readonly int InfoTableMaxAdhocQueryRowCount;
    public readonly int InfoTableRowCountLimit;
    public readonly int MaxTextLength;
    public readonly int GetRowCountIntervalInHours;
    public readonly int HistoryRecordCountLimit;
    public readonly int PercentileForThreshold;
    public readonly double SampleBucketSizeInMinutes;
    public readonly int MinRecordCountForOpt;
    public readonly int LookBackInDaysForResetNonOptimizable;
    public readonly int CleanupBatchSize;
    public const int MinQueryExecutionDetailRowCount = 10000;
    private const int defaultHistoryRecordCountLimit = 80;
    private const int defaultPercentileForThreshold = 50;
    private const int defaultGetRowCountIntervalInHours = 2;
    private const int defaultCleanupCutOffDaysOffset = -14;
    private const int defaultMaxTextLength = 4000;
    private const double defaultSampleBucketSizeInMinutes = 2.0;
    private const int defaultMinRecordCountForOpt = 5;
    private const int defaultLookBackInDaysForResetNonOptimizable = 1;
    private const int defaultCleanupBatchSize = 2000;
    private const int defaultQueryExecutionDetailMaxRowCount = 1000000;
    private const int defaultAdhocQueryExecutionInformationlMaxRowCount = 4000000;
    private const int defaultInfoTableRowCountLimit = 5000000;

    public WorkItemTrackingQueryExecutionLoggingConfiguration(
      RegistryEntryCollection queryExecutionLoggingConfig)
    {
      if (queryExecutionLoggingConfig != null)
      {
        this.DetailTableCutoffDaysOffset = queryExecutionLoggingConfig.GetValueFromPath<int>("/Service/WorkItemTracking/Settings/QueryExecutionLogging/DetailTableCutOffDaysOffset", -14);
        this.DetailTableMaxRowCount = queryExecutionLoggingConfig.GetValueFromPath<int>("/Service/WorkItemTracking/Settings/QueryExecutionLogging/DetailTableMaxRowCount", 1000000);
        this.InfoTableCutoffDaysOffset = queryExecutionLoggingConfig.GetValueFromPath<int>("/Service/WorkItemTracking/Settings/QueryExecutionLogging/InfoTableCutOffDaysOffset", -14);
        this.InfoTableMaxAdhocQueryRowCount = queryExecutionLoggingConfig.GetValueFromPath<int>("/Service/WorkItemTracking/Settings/QueryExecutionLogging/InfoTableMaxAdhocQueryRowCount", 4000000);
        this.InfoTableRowCountLimit = queryExecutionLoggingConfig.GetValueFromPath<int>("/Service/WorkItemTracking/Settings/QueryExecutionLogging/InfoTableRowCountLimit", 5000000);
        this.MaxTextLength = queryExecutionLoggingConfig.GetValueFromPath<int>("/Service/WorkItemTracking/Settings/QueryExecutionLogging/MaxTextLength", 4000);
        this.GetRowCountIntervalInHours = queryExecutionLoggingConfig.GetValueFromPath<int>("/Service/WorkItemTracking/Settings/QueryExecutionLogging/GetRowCountInterval", 2);
        this.HistoryRecordCountLimit = queryExecutionLoggingConfig.GetValueFromPath<int>("/Service/WorkItemTracking/Settings/QueryExecutionLogging/HistoryRecordCountLimit", 80);
        this.PercentileForThreshold = queryExecutionLoggingConfig.GetValueFromPath<int>("/Service/WorkItemTracking/Settings/QueryExecutionLogging/PercentileForThreshold", 50);
        this.SampleBucketSizeInMinutes = queryExecutionLoggingConfig.GetValueFromPath<double>("/Service/WorkItemTracking/Settings/QueryExecutionLogging/SampleBucketSizeInMinutes", 2.0);
        this.MinRecordCountForOpt = queryExecutionLoggingConfig.GetValueFromPath<int>("/Service/WorkItemTracking/Settings/QueryExecutionLogging/MinRecordCountForOpt", 5);
        this.LookBackInDaysForResetNonOptimizable = queryExecutionLoggingConfig.GetValueFromPath<int>("/Service/WorkItemTracking/Settings/QueryExecutionLogging/LookBackInDaysForResetNonOptimizable", 1);
        this.CleanupBatchSize = queryExecutionLoggingConfig.GetValueFromPath<int>("/Service/WorkItemTracking/Settings/QueryExecutionLogging/CleanupBatchSize", 2000);
      }
      else
      {
        this.DetailTableCutoffDaysOffset = -14;
        this.DetailTableMaxRowCount = 1000000;
        this.InfoTableCutoffDaysOffset = -14;
        this.InfoTableMaxAdhocQueryRowCount = 4000000;
        this.MaxTextLength = 4000;
      }
    }
  }
}
