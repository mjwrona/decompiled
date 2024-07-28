// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.SubStatusCodes
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

namespace Microsoft.Azure.Documents
{
  internal enum SubStatusCodes
  {
    Unknown = 0,
    WriteForbidden = 3,
    NameCacheIsStale = 1000, // 0x000003E8
    PartitionKeyMismatch = 1001, // 0x000003E9
    PartitionKeyRangeGone = 1002, // 0x000003EA
    ReadSessionNotAvailable = 1002, // 0x000003EA
    OwnerResourceNotFound = 1003, // 0x000003EB
    ConfigurationNameNotFound = 1004, // 0x000003EC
    CrossPartitionQueryNotServable = 1004, // 0x000003EC
    ConfigurationPropertyNotFound = 1005, // 0x000003ED
    ProvisionLimitReached = 1005, // 0x000003ED
    ConflictWithControlPlane = 1006, // 0x000003EE
    CompletingSplit = 1007, // 0x000003EF
    InsufficientBindablePartitions = 1007, // 0x000003EF
    CompletingPartitionMigration = 1008, // 0x000003F0
    DatabaseAccountNotFound = 1008, // 0x000003F0
    RedundantCollectionPut = 1009, // 0x000003F1
    SharedThroughputDatabaseQuotaExceeded = 1010, // 0x000003F2
    SharedThroughputOfferGrowNotNeeded = 1011, // 0x000003F3
    ComputeFederationNotFound = 1012, // 0x000003F4
    CollectionCreateInProgress = 1013, // 0x000003F5
    SharedThroughputDatabaseCollectionCountExceeded = 1019, // 0x000003FB
    SharedThroughputDatabaseCountExceeded = 1020, // 0x000003FC
    SplitIsDisabled = 2001, // 0x000007D1
    CollectionsInPartitionGotUpdated = 2002, // 0x000007D2
    CanNotAcquirePKRangesLock = 2003, // 0x000007D3
    ResourceNotFound = 2004, // 0x000007D4
    CanNotAcquireOfferOwnerLock = 2005, // 0x000007D5
    CanNotAcquireSnapshotOwnerLock = 2005, // 0x000007D5
    MigrationIsDisabled = 2006, // 0x000007D6
    CanNotAcquirePKRangeLock = 2007, // 0x000007D7
    CanNotAcquirePartitionLock = 2008, // 0x000007D8
    CanNotAcquireGlobalPartitionMigrationLock = 2009, // 0x000007D9
    CanNotAcquireFederationPartitionMigrationLock = 2010, // 0x000007DA
    StorageSplitConflictingWithNWayThroughputSplit = 2011, // 0x000007DB
    ConfigurationNameNotEmpty = 3001, // 0x00000BB9
    AnotherOfferReplaceOperationIsInProgress = 3205, // 0x00000C85
    DatabaseNameAlreadyExists = 3206, // 0x00000C86
    ConfigurationNameAlreadyExists = 3207, // 0x00000C87
    ClientTcpChannelFull = 3208, // 0x00000C88
    PartitionkeyHashCollisionForId = 3302, // 0x00000CE6
    AadClientCredentialsGrantFailure = 4000, // 0x00000FA0
    AadServiceUnavailable = 4001, // 0x00000FA1
    KeyVaultAuthenticationFailure = 4002, // 0x00000FA2
    KeyVaultKeyNotFound = 4003, // 0x00000FA3
    KeyVaultServiceUnavailable = 4004, // 0x00000FA4
    KeyVaultWrapUnwrapFailure = 4005, // 0x00000FA5
    InvalidKeyVaultKeyURI = 4006, // 0x00000FA6
    InvalidInputBytes = 4007, // 0x00000FA7
    KeyVaultInternalServerError = 4008, // 0x00000FA8
    OperationPaused = 9001, // 0x00002329
    ScriptCompileError = 65535, // 0x0000FFFF
  }
}
