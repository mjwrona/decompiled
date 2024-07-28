// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.WFConstants
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

namespace Microsoft.Azure.Documents
{
  internal sealed class WFConstants
  {
    public const int DefaultFabricNameResolutionTimeoutInSeconds = 10;

    public static class WireNames
    {
      public const string NamedEndpoint = "App=";
    }

    public static class BackendHeaders
    {
      public const string ResourceId = "x-docdb-resource-id";
      public const string OwnerId = "x-docdb-owner-id";
      public const string EntityId = "x-docdb-entity-id";
      public const string DatabaseEntityMaxCount = "x-ms-database-entity-max-count";
      public const string DatabaseEntityCurrentCount = "x-ms-database-entity-current-count";
      public const string CollectionEntityMaxCount = "x-ms-collection-entity-max-count";
      public const string CollectionEntityCurrentCount = "x-ms-collection-entity-current-count";
      public const string UserEntityMaxCount = "x-ms-user-entity-max-count";
      public const string UserEntityCurrentCount = "x-ms-user-entity-current-count";
      public const string PermissionEntityMaxCount = "x-ms-permission-entity-max-count";
      public const string PermissionEntityCurrentCount = "x-ms-permission-entity-current-count";
      public const string RootEntityMaxCount = "x-ms-root-entity-max-count";
      public const string RootEntityCurrentCount = "x-ms-root-entity-current-count";
      public const string ResourceSchemaName = "x-ms-resource-schema-name";
      public const string LSN = "lsn";
      public const string QuorumAckedLSN = "x-ms-quorum-acked-lsn";
      public const string QuorumAckedLLSN = "x-ms-cosmos-quorum-acked-llsn";
      public const string CurrentWriteQuorum = "x-ms-current-write-quorum";
      public const string CurrentReplicaSetSize = "x-ms-current-replica-set-size";
      public const string CollectionPartitionIndex = "collection-partition-index";
      public const string CollectionServiceIndex = "collection-service-index";
      public const string Status = "Status";
      public const string ActivityId = "ActivityId";
      public const string IsFanoutRequest = "x-ms-is-fanout-request";
      public const string PrimaryMasterKey = "x-ms-primary-master-key";
      public const string SecondaryMasterKey = "x-ms-secondary-master-key";
      public const string PrimaryReadonlyKey = "x-ms-primary-readonly-key";
      public const string SecondaryReadonlyKey = "x-ms-secondary-readonly-key";
      public const string BindReplicaDirective = "x-ms-bind-replica";
      public const string DatabaseAccountId = "x-ms-database-account-id";
      public const string RequestValidationFailure = "x-ms-request-validation-failure";
      public const string SubStatus = "x-ms-substatus";
      public const string PartitionKeyRangeId = "x-ms-documentdb-partitionkeyrangeid";
      public const string PartitionCount = "x-ms-documentdb-partitioncount";
      public const string CollectionRid = "x-ms-documentdb-collection-rid";
      public const string XPRole = "x-ms-xp-role";
      public const string HasTentativeWrites = "x-ms-cosmosdb-has-tentative-writes";
      public const string IsRUPerMinuteUsed = "x-ms-documentdb-is-ru-per-minute-used";
      public const string QueryMetrics = "x-ms-documentdb-query-metrics";
      public const string IndexUtilization = "x-ms-cosmos-index-utilization";
      public const string GlobalCommittedLSN = "x-ms-global-Committed-lsn";
      public const string NumberOfReadRegions = "x-ms-number-of-read-regions";
      public const string OfferReplacePending = "x-ms-offer-replace-pending";
      public const string ItemLSN = "x-ms-item-lsn";
      public const string RemoteStorageType = "x-ms-remote-storage-type";
      public const string RestoreState = "x-ms-restore-state";
      public const string CollectionSecurityIdentifier = "x-ms-collection-security-identifier";
      public const string RestoreParams = "x-ms-restore-params";
      public const string ShareThroughput = "x-ms-share-throughput";
      public const string PartitionResourceFilter = "x-ms-partition-resource-filter";
      public const string FederationIdForAuth = "x-ms-federation-for-auth";
      public const string ForceQueryScan = "x-ms-documentdb-force-query-scan";
      public const string EnableDynamicRidRangeAllocation = "x-ms-enable-dynamic-rid-range-allocation";
      public const string ExcludeSystemProperties = "x-ms-exclude-system-properties";
      public const string LocalLSN = "x-ms-cosmos-llsn";
      public const string QuorumAckedLocalLSN = "x-ms-cosmos-quorum-acked-llsn";
      public const string ItemLocalLSN = "x-ms-cosmos-item-llsn";
      public const string MergeStaticId = "x-ms-cosmos-merge-static-id";
      public const string ReplicatorLSNToGLSNDelta = "x-ms-cosmos-replicator-glsn-delta";
      public const string ReplicatorLSNToLLSNDelta = "x-ms-cosmos-replicator-llsn-delta";
      public const string VectorClockLocalProgress = "x-ms-cosmos-vectorclock-local-progress";
      public const string MinimumRUsForOffer = "x-ms-cosmos-min-throughput";
      public const string XPConfigurationSessionsCount = "x-ms-cosmos-xpconfiguration-sessions-count";
      public const string BinaryId = "x-ms-binary-id";
      public const string TimeToLiveInSeconds = "x-ms-time-to-live-in-seconds";
      public const string EffectivePartitionKey = "x-ms-effective-partition-key";
      public const string BinaryPassthroughRequest = "x-ms-binary-passthrough-request";
      public const string FanoutOperationState = "x-ms-fanout-operation-state";
      public const string ContentSerializationFormat = "x-ms-documentdb-content-serialization-format";
      public const string AllowTentativeWrites = "x-ms-cosmos-allow-tentative-writes";
      public const string IsUserRequest = "x-ms-cosmos-internal-is-user-request";
      public const string PreserveFullContent = "x-ms-cosmos-preserve-full-content";
      public const string EffectivePartitionKeyString = "x-ms-effective-partition-key-string";
      public const string SchemaOwnerRid = "x-ms-schema-owner-rid";
      public const string SchemaHash = "x-ms-schema-hash";
      public const string PopulateLogStoreInfo = "x-ms-cosmos-populate-logstoreinfo";
      public const string ForceSideBySideIndexMigration = "x-ms-cosmos-force-sidebyside-indexmigration";
      public const string CollectionChildResourceNameLimitInBytes = "x-ms-cosmos-collection-child-resourcename-limit";
      public const string CollectionChildResourceContentLimitInKB = "x-ms-cosmos-collection-child-contentlength-resourcelimit";
      public const string MergeCheckPointGLSN = "x-ms-cosmos-internal-merge-checkpoint-glsn";
    }
  }
}
