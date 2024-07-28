// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.OperationType
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

namespace Microsoft.Azure.Documents
{
  internal enum OperationType
  {
    ReadReplicaFromMasterPartition = -13, // 0xFFFFFFF3
    GetUnwrappedDek = -12, // 0xFFFFFFF4
    GetDatabaseAccountConfigurations = -11, // 0xFFFFFFF5
    GetFederationConfigurations = -10, // 0xFFFFFFF6
    GetStorageAccountKey = -9, // 0xFFFFFFF7
    GetConfiguration = -8, // 0xFFFFFFF8
    ControllerBatchGetOutput = -7, // 0xFFFFFFF9
    ControllerBatchReportCharges = -6, // 0xFFFFFFFA
    ServiceReservation = -5, // 0xFFFFFFFB
    ReportThroughputUtilization = -4, // 0xFFFFFFFC
    ForceConfigRefresh = -3, // 0xFFFFFFFD
    ExecuteJavaScript = -2, // 0xFFFFFFFE
    Invalid = -1, // 0xFFFFFFFF
    Create = 0,
    Patch = 1,
    Read = 2,
    ReadFeed = 3,
    Delete = 4,
    Replace = 5,
    Pause = 6,
    Resume = 7,
    Stop = 8,
    Execute = 9,
    Recycle = 10, // 0x0000000A
    Crash = 11, // 0x0000000B
    FanoutDelete = 12, // 0x0000000C
    BatchApply = 13, // 0x0000000D
    SqlQuery = 14, // 0x0000000E
    Query = 15, // 0x0000000F
    BindReplica = 16, // 0x00000010
    JSQuery = 17, // 0x00000011
    Head = 18, // 0x00000012
    HeadFeed = 19, // 0x00000013
    Upsert = 20, // 0x00000014
    Recreate = 21, // 0x00000015
    Throttle = 22, // 0x00000016
    GetSplitPoint = 23, // 0x00000017
    PreCreateValidation = 24, // 0x00000018
    ApplyTransactionLogs = 25, // 0x00000019
    Relocate = 26, // 0x0000001A
    AbortSplit = 27, // 0x0000001B
    CompleteSplit = 28, // 0x0000001C
    WriteValue = 29, // 0x0000001D
    CompletePartitionMigration = 30, // 0x0000001E
    AbortPartitionMigration = 31, // 0x0000001F
    OfferUpdateOperation = 32, // 0x00000020
    OfferPreGrowValidation = 33, // 0x00000021
    BatchReportThroughputUtilization = 34, // 0x00000022
    PreReplaceValidation = 35, // 0x00000023
    MigratePartition = 36, // 0x00000024
    AddComputeGatewayRequestCharges = 37, // 0x00000025
    MasterReplaceOfferOperation = 38, // 0x00000026
    ProvisionedCollectionOfferUpdateOperation = 39, // 0x00000027
    Batch = 40, // 0x00000028
    QueryPlan = 41, // 0x00000029
    InitiateDatabaseOfferPartitionShrink = 42, // 0x0000002A
    CompleteDatabaseOfferPartitionShrink = 43, // 0x0000002B
    EnsureSnapshotOperation = 44, // 0x0000002C
    GetSplitPoints = 45, // 0x0000002D
  }
}
