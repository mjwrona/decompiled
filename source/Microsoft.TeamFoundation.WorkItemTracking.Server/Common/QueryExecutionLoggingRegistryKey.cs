// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Common.QueryExecutionLoggingRegistryKey
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using System.ComponentModel;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Common
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class QueryExecutionLoggingRegistryKey
  {
    public const string Root = "/Service/WorkItemTracking/Settings/QueryExecutionLogging/";
    public const string All = "/Service/WorkItemTracking/Settings/QueryExecutionLogging/...";
    public const string DetailTableCutOffDaysOffset = "/Service/WorkItemTracking/Settings/QueryExecutionLogging/DetailTableCutOffDaysOffset";
    public const string DetailTableMaxRowCount = "/Service/WorkItemTracking/Settings/QueryExecutionLogging/DetailTableMaxRowCount";
    public const string InfoTableCutOffDaysOffset = "/Service/WorkItemTracking/Settings/QueryExecutionLogging/InfoTableCutOffDaysOffset";
    public const string InfoTableMaxAdhocQueryRowCount = "/Service/WorkItemTracking/Settings/QueryExecutionLogging/InfoTableMaxAdhocQueryRowCount";
    public const string InfoTableRowCountLimit = "/Service/WorkItemTracking/Settings/QueryExecutionLogging/InfoTableRowCountLimit";
    public const string MaxTextLength = "/Service/WorkItemTracking/Settings/QueryExecutionLogging/MaxTextLength";
    public const string SampleBucketSizeInMinutes = "/Service/WorkItemTracking/Settings/QueryExecutionLogging/SampleBucketSizeInMinutes";
    public const string MinRecordCountForOpt = "/Service/WorkItemTracking/Settings/QueryExecutionLogging/MinRecordCountForOpt";
    public const string LookBackInDaysForResetNonOptimizable = "/Service/WorkItemTracking/Settings/QueryExecutionLogging/LookBackInDaysForResetNonOptimizable";
    public const string GetRowCountInterval = "/Service/WorkItemTracking/Settings/QueryExecutionLogging/GetRowCountInterval";
    public const string HistoryRecordCountLimit = "/Service/WorkItemTracking/Settings/QueryExecutionLogging/HistoryRecordCountLimit";
    public const string PercentileForThreshold = "/Service/WorkItemTracking/Settings/QueryExecutionLogging/PercentileForThreshold";
    public const string CleanupBatchSize = "/Service/WorkItemTracking/Settings/QueryExecutionLogging/CleanupBatchSize";
  }
}
