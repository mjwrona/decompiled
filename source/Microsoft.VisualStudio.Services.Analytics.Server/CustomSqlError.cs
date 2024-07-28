// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.CustomSqlError
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

namespace Microsoft.VisualStudio.Services.Analytics
{
  internal static class CustomSqlError
  {
    public const int LowerBound = 1670000;
    public const int UpperBound = 1679999;
    public const int GenericWrapperCode = 50000;
    public const int GenericDatabaseFailure = 1670001;
    public const int TransactionRequired = 1670002;
    public const int GenericStagingError = 1670003;
    public const int CounterPopulationError = 1670004;
    public const int CounterRetrievalError = 1670005;
    public const int ProviderShardInvalidationError = 1670006;
    public const int TransformQueryError = 1670007;
    public const int TransformProcessFieldsError = 1670008;
    public const int FailedUpgradeDowngradeCCIIndexesError = 1670009;
    public const int CCIRequestedButIsNotAvailable = 1670010;
    public const int IndexDoesNotExists = 1670011;
    public const int FailedToSplitPartitions = 1670012;
    public const int PartitionMaintenanceNotSupported = 1670013;
    public const int UnknownPartitionOperation = 1670014;
    public const int SplitNotSupportedForPartition = 1670015;
    public const int MergeNotSupportedForPartition = 1670016;
    public const int FailedToCopyTable = 1670017;
    public const int NumbersOfRecordsDontMatchForTable = 1670018;
    public const int MaintenanceInProcess = 1670019;
    public const int TransformBatchUnknownSproc = 1670020;
    public const int TableNameParameterSupportedOnlyForSort = 1670021;
    public const int UnknownTableToSort = 1670022;
    public const int TableNameParameterRequiredForSort = 1670023;
    public const int FailedToMergeAnalyticsViewsUpdates = 1670024;
    public const int KeysOnlyStagingNotSupported = 1670025;
    public const int UnknownPartitionScheme = 1670026;
    public const int CleanUpSupportedOnlyOnPrem = 1670027;
    public const int MaintenceModeRequiresReason = 1670028;
    public const int FailedToApplyCreateProcessBatchLock = 1670029;
    public const int MaintenanceCleanupNotSupportedWhenDataInOldTable = 1670030;
    public const int NumbersOfRecordsDontMatchForMainTableAndTempTable = 1670031;
    public const int MAX_SQL_ERROR = 1670032;
  }
}
