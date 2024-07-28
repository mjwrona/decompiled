// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Collections.RequestNameValueCollection
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace Microsoft.Azure.Documents.Collections
{
  internal class RequestNameValueCollection : INameValueCollection, IEnumerable
  {
    private static readonly StringComparer DefaultStringComparer = StringComparer.OrdinalIgnoreCase;
    private readonly Lazy<Dictionary<string, string>> lazyNotCommonHeaders;
    private NameValueCollection nameValueCollection;

    public string A_IM { get; set; }

    public string AddResourcePropertiesToResponse { get; set; }

    public string AllowTentativeWrites { get; set; }

    public string Authorization { get; set; }

    public string BinaryId { get; set; }

    public string BinaryPassthroughRequest { get; set; }

    public string BindReplicaDirective { get; set; }

    public string BuilderClientIdentifier { get; set; }

    public string CanCharge { get; set; }

    public string CanOfferReplaceComplete { get; set; }

    public string CanThrottle { get; set; }

    public string ChangeFeedStartFullFidelityIfNoneMatch { get; set; }

    public string ChangeFeedWireFormatVersion { get; set; }

    public string ClientRetryAttemptCount { get; set; }

    public string CollectionChildResourceContentLimitInKB { get; set; }

    public string CollectionChildResourceNameLimitInBytes { get; set; }

    public string CollectionPartitionIndex { get; set; }

    public string CollectionRemoteStorageSecurityIdentifier { get; set; }

    public string CollectionRid { get; set; }

    public string CollectionServiceIndex { get; set; }

    public string CollectionTruncate { get; set; }

    public string ConsistencyLevel { get; set; }

    public string ContentSerializationFormat { get; set; }

    public string Continuation { get; set; }

    public string CorrelatedActivityId { get; set; }

    public string DisableRUPerMinuteUsage { get; set; }

    public string EffectivePartitionKey { get; set; }

    public string EmitVerboseTracesInQuery { get; set; }

    public string EnableDynamicRidRangeAllocation { get; set; }

    public string EnableLogging { get; set; }

    public string EnableLowPrecisionOrderBy { get; set; }

    public string EnableScanInQuery { get; set; }

    public string EndEpk { get; set; }

    public string EndId { get; set; }

    public string EntityId { get; set; }

    public string EnumerationDirection { get; set; }

    public string ExcludeSystemProperties { get; set; }

    public string FanoutOperationState { get; set; }

    public string FilterBySchemaResourceId { get; set; }

    public string ForceQueryScan { get; set; }

    public string ForceSideBySideIndexMigration { get; set; }

    public string GatewaySignature { get; set; }

    public string GetAllPartitionKeyStatistics { get; set; }

    public string HttpDate { get; set; }

    public string IfMatch { get; set; }

    public string IfModifiedSince { get; set; }

    public string IfNoneMatch { get; set; }

    public string IgnoreSystemLoweringMaxThroughput { get; set; }

    public string IncludePhysicalPartitionThroughputInfo { get; set; }

    public string IncludeTentativeWrites { get; set; }

    public string IndexingDirective { get; set; }

    public string IntendedCollectionRid { get; set; }

    public string IsAutoScaleRequest { get; set; }

    public string IsBatchAtomic { get; set; }

    public string IsBatchOrdered { get; set; }

    public string IsClientEncrypted { get; set; }

    public string IsFanoutRequest { get; set; }

    public string IsInternalServerlessRequest { get; set; }

    public string IsMaterializedViewBuild { get; set; }

    public string IsOfferStorageRefreshRequest { get; set; }

    public string IsReadOnlyScript { get; set; }

    public string IsRetriedWriteRequest { get; set; }

    public string IsRUPerGBEnforcementRequest { get; set; }

    public string IsServerlessStorageRefreshRequest { get; set; }

    public string IsThroughputCapRequest { get; set; }

    public string IsUserRequest { get; set; }

    public string MaxPollingIntervalMilliseconds { get; set; }

    public string MergeCheckPointGLSN { get; set; }

    public string MergeStaticId { get; set; }

    public string MigrateCollectionDirective { get; set; }

    public string MigrateOfferToAutopilot { get; set; }

    public string MigrateOfferToManualThroughput { get; set; }

    public string OfferReplaceRURedistribution { get; set; }

    public string PageSize { get; set; }

    public string PartitionCount { get; set; }

    public string PartitionKey { get; set; }

    public string PartitionKeyRangeId { get; set; }

    public string PartitionResourceFilter { get; set; }

    public string PopulateAnalyticalMigrationProgress { get; set; }

    public string PopulateByokEncryptionProgress { get; set; }

    public string PopulateCollectionThroughputInfo { get; set; }

    public string PopulateIndexMetrics { get; set; }

    public string PopulateLogStoreInfo { get; set; }

    public string PopulateOldestActiveSchema { get; set; }

    public string PopulatePartitionStatistics { get; set; }

    public string PopulateQueryMetrics { get; set; }

    public string PopulateQuotaInfo { get; set; }

    public string PopulateResourceCount { get; set; }

    public string PopulateUnflushedMergeEntryCount { get; set; }

    public string PopulateUniqueIndexReIndexProgress { get; set; }

    public string PostTriggerExclude { get; set; }

    public string PostTriggerInclude { get; set; }

    public string Prefer { get; set; }

    public string PreserveFullContent { get; set; }

    public string PreTriggerExclude { get; set; }

    public string PreTriggerInclude { get; set; }

    public string PrimaryMasterKey { get; set; }

    public string PrimaryReadonlyKey { get; set; }

    public string ProfileRequest { get; set; }

    public string RbacAction { get; set; }

    public string RbacResource { get; set; }

    public string RbacUserId { get; set; }

    public string ReadFeedKeyType { get; set; }

    public string RemainingTimeInMsOnClientRequest { get; set; }

    public string RemoteStorageType { get; set; }

    public string RequestedCollectionType { get; set; }

    public string ResourceId { get; set; }

    public string ResourceSchemaName { get; set; }

    public string ResourceTokenExpiry { get; set; }

    public string ResourceTypes { get; set; }

    public string ResponseContinuationTokenLimitInKB { get; set; }

    public string RestoreMetadataFilter { get; set; }

    public string RestoreParams { get; set; }

    public string RetriableWriteRequestId { get; set; }

    public string RetriableWriteRequestStartTimestamp { get; set; }

    public string SchemaHash { get; set; }

    public string SchemaId { get; set; }

    public string SchemaOwnerRid { get; set; }

    public string SDKSupportedCapabilities { get; set; }

    public string SecondaryMasterKey { get; set; }

    public string SecondaryReadonlyKey { get; set; }

    public string SessionToken { get; set; }

    public string ShareThroughput { get; set; }

    public string ShouldBatchContinueOnError { get; set; }

    public string ShouldReturnCurrentServerDateTime { get; set; }

    public string SkipRefreshDatabaseAccountConfigs { get; set; }

    public string SourceCollectionIfMatch { get; set; }

    public string StartEpk { get; set; }

    public string StartId { get; set; }

    public string SupportSpatialLegacyCoordinates { get; set; }

    public string SystemDocumentType { get; set; }

    public string SystemRestoreOperation { get; set; }

    public string TargetGlobalCommittedLsn { get; set; }

    public string TargetLsn { get; set; }

    public string TimeToLiveInSeconds { get; set; }

    public string TransactionCommit { get; set; }

    public string TransactionFirstRequest { get; set; }

    public string TransactionId { get; set; }

    public string TransportRequestID { get; set; }

    public string TruncateMergeLogRequest { get; set; }

    public string UniqueIndexNameEncodingMode { get; set; }

    public string UniqueIndexReIndexingState { get; set; }

    public string UpdateMaxThroughputEverProvisioned { get; set; }

    public string UpdateOfferStateToPending { get; set; }

    public string UseArchivalPartition { get; set; }

    public string UsePolygonsSmallerThanAHemisphere { get; set; }

    public string UseSystemBudget { get; set; }

    public string UseUserBackgroundBudget { get; set; }

    public string Version { get; set; }

    public string XDate { get; set; }

    public RequestNameValueCollection()
      : this(new Lazy<Dictionary<string, string>>((Func<Dictionary<string, string>>) (() => new Dictionary<string, string>((IEqualityComparer<string>) RequestNameValueCollection.DefaultStringComparer))))
    {
    }

    public RequestNameValueCollection(INameValueCollection nameValueCollection)
    {
      this.ResourceId = nameValueCollection["x-docdb-resource-id"];
      this.Authorization = nameValueCollection["authorization"];
      this.HttpDate = nameValueCollection["date"];
      this.XDate = nameValueCollection["x-ms-date"];
      this.PageSize = nameValueCollection["x-ms-max-item-count"];
      this.SessionToken = nameValueCollection["x-ms-session-token"];
      this.Continuation = nameValueCollection["x-ms-continuation"];
      this.IndexingDirective = nameValueCollection["x-ms-indexing-directive"];
      this.IfNoneMatch = nameValueCollection["If-None-Match"];
      this.PreTriggerInclude = nameValueCollection["x-ms-documentdb-pre-trigger-include"];
      this.PostTriggerInclude = nameValueCollection["x-ms-documentdb-post-trigger-include"];
      this.IsFanoutRequest = nameValueCollection["x-ms-is-fanout-request"];
      this.CollectionPartitionIndex = nameValueCollection["collection-partition-index"];
      this.CollectionServiceIndex = nameValueCollection["collection-service-index"];
      this.PreTriggerExclude = nameValueCollection["x-ms-documentdb-pre-trigger-exclude"];
      this.PostTriggerExclude = nameValueCollection["x-ms-documentdb-post-trigger-exclude"];
      this.ConsistencyLevel = nameValueCollection["x-ms-consistency-level"];
      this.EntityId = nameValueCollection["x-docdb-entity-id"];
      this.ResourceSchemaName = nameValueCollection["x-ms-resource-schema-name"];
      this.ResourceTokenExpiry = nameValueCollection["x-ms-documentdb-expiry-seconds"];
      this.EnableScanInQuery = nameValueCollection["x-ms-documentdb-query-enable-scan"];
      this.EmitVerboseTracesInQuery = nameValueCollection["x-ms-documentdb-query-emit-traces"];
      this.BindReplicaDirective = nameValueCollection["x-ms-bind-replica"];
      this.PrimaryMasterKey = nameValueCollection["x-ms-primary-master-key"];
      this.SecondaryMasterKey = nameValueCollection["x-ms-secondary-master-key"];
      this.PrimaryReadonlyKey = nameValueCollection["x-ms-primary-readonly-key"];
      this.SecondaryReadonlyKey = nameValueCollection["x-ms-secondary-readonly-key"];
      this.ProfileRequest = nameValueCollection["x-ms-profile-request"];
      this.EnableLowPrecisionOrderBy = nameValueCollection["x-ms-documentdb-query-enable-low-precision-order-by"];
      this.Version = nameValueCollection["x-ms-version"];
      this.CanCharge = nameValueCollection["x-ms-cancharge"];
      this.CanThrottle = nameValueCollection["x-ms-canthrottle"];
      this.PartitionKey = nameValueCollection["x-ms-documentdb-partitionkey"];
      this.PartitionKeyRangeId = nameValueCollection["x-ms-documentdb-partitionkeyrangeid"];
      this.MigrateCollectionDirective = nameValueCollection["x-ms-migratecollection-directive"];
      this.SupportSpatialLegacyCoordinates = nameValueCollection["x-ms-documentdb-supportspatiallegacycoordinates"];
      this.PartitionCount = nameValueCollection["x-ms-documentdb-partitioncount"];
      this.CollectionRid = nameValueCollection["x-ms-documentdb-collection-rid"];
      this.FilterBySchemaResourceId = nameValueCollection["x-ms-documentdb-filterby-schema-rid"];
      this.UsePolygonsSmallerThanAHemisphere = nameValueCollection["x-ms-documentdb-usepolygonssmallerthanahemisphere"];
      this.GatewaySignature = nameValueCollection["x-ms-gateway-signature"];
      this.EnableLogging = nameValueCollection["x-ms-documentdb-script-enable-logging"];
      this.A_IM = nameValueCollection["A-IM"];
      this.IfModifiedSince = nameValueCollection["If-Modified-Since"];
      this.PopulateQuotaInfo = nameValueCollection["x-ms-documentdb-populatequotainfo"];
      this.DisableRUPerMinuteUsage = nameValueCollection["x-ms-documentdb-disable-ru-per-minute-usage"];
      this.PopulateQueryMetrics = nameValueCollection["x-ms-documentdb-populatequerymetrics"];
      this.ResponseContinuationTokenLimitInKB = nameValueCollection["x-ms-documentdb-responsecontinuationtokenlimitinkb"];
      this.PopulatePartitionStatistics = nameValueCollection["x-ms-documentdb-populatepartitionstatistics"];
      this.RemoteStorageType = nameValueCollection["x-ms-remote-storage-type"];
      this.RemainingTimeInMsOnClientRequest = nameValueCollection["x-ms-remaining-time-in-ms-on-client"];
      this.ClientRetryAttemptCount = nameValueCollection["x-ms-client-retry-attempt-count"];
      this.TargetLsn = nameValueCollection["x-ms-target-lsn"];
      this.TargetGlobalCommittedLsn = nameValueCollection["x-ms-target-global-committed-lsn"];
      this.TransportRequestID = nameValueCollection["x-ms-transport-request-id"];
      this.CollectionRemoteStorageSecurityIdentifier = nameValueCollection["x-ms-collection-security-identifier"];
      this.PopulateCollectionThroughputInfo = nameValueCollection["x-ms-documentdb-populatecollectionthroughputinfo"];
      this.RestoreMetadataFilter = nameValueCollection["x-ms-restore-metadata-filter"];
      this.RestoreParams = nameValueCollection["x-ms-restore-params"];
      this.ShareThroughput = nameValueCollection["x-ms-share-throughput"];
      this.PartitionResourceFilter = nameValueCollection["x-ms-partition-resource-filter"];
      this.IsReadOnlyScript = nameValueCollection["x-ms-is-readonly-script"];
      this.IsAutoScaleRequest = nameValueCollection["x-ms-is-auto-scale"];
      this.ForceQueryScan = nameValueCollection["x-ms-documentdb-force-query-scan"];
      this.CanOfferReplaceComplete = nameValueCollection["x-ms-can-offer-replace-complete"];
      this.ExcludeSystemProperties = nameValueCollection["x-ms-exclude-system-properties"];
      this.BinaryId = nameValueCollection["x-ms-binary-id"];
      this.TimeToLiveInSeconds = nameValueCollection["x-ms-time-to-live-in-seconds"];
      this.EffectivePartitionKey = nameValueCollection["x-ms-effective-partition-key"];
      this.BinaryPassthroughRequest = nameValueCollection["x-ms-binary-passthrough-request"];
      this.EnableDynamicRidRangeAllocation = nameValueCollection["x-ms-enable-dynamic-rid-range-allocation"];
      this.EnumerationDirection = nameValueCollection["x-ms-enumeration-direction"];
      this.StartId = nameValueCollection["x-ms-start-id"];
      this.EndId = nameValueCollection["x-ms-end-id"];
      this.FanoutOperationState = nameValueCollection["x-ms-fanout-operation-state"];
      this.StartEpk = nameValueCollection["x-ms-start-epk"];
      this.EndEpk = nameValueCollection["x-ms-end-epk"];
      this.ReadFeedKeyType = nameValueCollection["x-ms-read-key-type"];
      this.ContentSerializationFormat = nameValueCollection["x-ms-documentdb-content-serialization-format"];
      this.AllowTentativeWrites = nameValueCollection["x-ms-cosmos-allow-tentative-writes"];
      this.IsUserRequest = nameValueCollection["x-ms-cosmos-internal-is-user-request"];
      this.PreserveFullContent = nameValueCollection["x-ms-cosmos-preserve-full-content"];
      this.IncludeTentativeWrites = nameValueCollection["x-ms-cosmos-include-tentative-writes"];
      this.PopulateResourceCount = nameValueCollection["x-ms-documentdb-populateresourcecount"];
      this.MergeStaticId = nameValueCollection["x-ms-cosmos-merge-static-id"];
      this.IsBatchAtomic = nameValueCollection["x-ms-cosmos-batch-atomic"];
      this.ShouldBatchContinueOnError = nameValueCollection["x-ms-cosmos-batch-continue-on-error"];
      this.IsBatchOrdered = nameValueCollection["x-ms-cosmos-batch-ordered"];
      this.SchemaOwnerRid = nameValueCollection["x-ms-schema-owner-rid"];
      this.SchemaHash = nameValueCollection["x-ms-schema-hash"];
      this.IsRUPerGBEnforcementRequest = nameValueCollection["x-ms-cosmos-internal-is-ru-per-gb-enforcement-request"];
      this.MaxPollingIntervalMilliseconds = nameValueCollection["x-ms-cosmos-max-polling-interval"];
      this.PopulateLogStoreInfo = nameValueCollection["x-ms-cosmos-populate-logstoreinfo"];
      this.GetAllPartitionKeyStatistics = nameValueCollection["x-ms-cosmos-internal-get-all-partition-key-stats"];
      this.ForceSideBySideIndexMigration = nameValueCollection["x-ms-cosmos-force-sidebyside-indexmigration"];
      this.CollectionChildResourceNameLimitInBytes = nameValueCollection["x-ms-cosmos-collection-child-resourcename-limit"];
      this.CollectionChildResourceContentLimitInKB = nameValueCollection["x-ms-cosmos-collection-child-contentlength-resourcelimit"];
      this.MergeCheckPointGLSN = nameValueCollection["x-ms-cosmos-internal-merge-checkpoint-glsn"];
      this.Prefer = nameValueCollection[nameof (Prefer)];
      this.UniqueIndexNameEncodingMode = nameValueCollection["x-ms-cosmos-unique-index-name-encoding-mode"];
      this.PopulateUnflushedMergeEntryCount = nameValueCollection["x-ms-cosmos-internal-populate-unflushed-merge-entry-count"];
      this.MigrateOfferToManualThroughput = nameValueCollection["x-ms-cosmos-migrate-offer-to-manual-throughput"];
      this.MigrateOfferToAutopilot = nameValueCollection["x-ms-cosmos-migrate-offer-to-autopilot"];
      this.IsClientEncrypted = nameValueCollection["x-ms-cosmos-is-client-encrypted"];
      this.SystemDocumentType = nameValueCollection["x-ms-cosmos-systemdocument-type"];
      this.IsOfferStorageRefreshRequest = nameValueCollection["x-ms-cosmos-internal-is-offer-storage-refresh-request"];
      this.ResourceTypes = nameValueCollection["x-ms-cosmos-resourcetypes"];
      this.TransactionId = nameValueCollection["x-ms-cosmos-tx-id"];
      this.TransactionFirstRequest = nameValueCollection["x-ms-cosmos-tx-init"];
      this.TransactionCommit = nameValueCollection["x-ms-cosmos-tx-commit"];
      this.UpdateMaxThroughputEverProvisioned = nameValueCollection["x-ms-cosmos-internal-update-max-throughput-ever-provisioned"];
      this.UniqueIndexReIndexingState = nameValueCollection["x-ms-cosmos-uniqueindex-reindexing-state"];
      this.UseSystemBudget = nameValueCollection["x-ms-cosmos-use-systembudget"];
      this.IgnoreSystemLoweringMaxThroughput = nameValueCollection["x-ms-cosmos-internal-ignore-system-lowering-max-throughput"];
      this.TruncateMergeLogRequest = nameValueCollection["x-ms-cosmos-internal-truncate-merge-log"];
      this.RetriableWriteRequestId = nameValueCollection["x-ms-cosmos-retriable-write-request-id"];
      this.IsRetriedWriteRequest = nameValueCollection["x-ms-cosmos-is-retried-write-request"];
      this.RetriableWriteRequestStartTimestamp = nameValueCollection["x-ms-cosmos-retriable-write-request-start-timestamp"];
      this.AddResourcePropertiesToResponse = nameValueCollection["x-ms-cosmos-add-resource-properties-to-response"];
      this.ChangeFeedStartFullFidelityIfNoneMatch = nameValueCollection["x-ms-cosmos-start-full-fidelity-if-none-match"];
      this.SystemRestoreOperation = nameValueCollection["x-ms-cosmos-internal-system-restore-operation"];
      this.SkipRefreshDatabaseAccountConfigs = nameValueCollection["x-ms-cosmos-skip-refresh-databaseaccountconfig"];
      this.IntendedCollectionRid = nameValueCollection["x-ms-cosmos-intended-collection-rid"];
      this.UseArchivalPartition = nameValueCollection["x-ms-cosmos-use-archival-partition"];
      this.PopulateUniqueIndexReIndexProgress = nameValueCollection["x-ms-cosmosdb-populateuniqueindexreindexprogress"];
      this.SchemaId = nameValueCollection["x-ms-schema-id"];
      this.CollectionTruncate = nameValueCollection["x-ms-cosmos-collection-truncate"];
      this.SDKSupportedCapabilities = nameValueCollection["x-ms-cosmos-sdk-supportedcapabilities"];
      this.IsMaterializedViewBuild = nameValueCollection["x-ms-cosmos-internal-is-materialized-view-build"];
      this.BuilderClientIdentifier = nameValueCollection["x-ms-cosmos-builder-client-identifier"];
      this.SourceCollectionIfMatch = nameValueCollection["x-ms-cosmos-source-collection-if-match"];
      this.RequestedCollectionType = nameValueCollection["x-ms-cosmos-collectiontype"];
      this.PopulateIndexMetrics = nameValueCollection["x-ms-cosmos-populateindexmetrics"];
      this.PopulateAnalyticalMigrationProgress = nameValueCollection["x-ms-cosmos-populate-analytical-migration-progress"];
      this.ShouldReturnCurrentServerDateTime = nameValueCollection["x-ms-should-return-current-server-datetime"];
      this.RbacUserId = nameValueCollection["x-ms-rbac-user-id"];
      this.RbacAction = nameValueCollection["x-ms-rbac-action"];
      this.RbacResource = nameValueCollection["x-ms-rbac-resource"];
      this.CorrelatedActivityId = nameValueCollection["x-ms-cosmos-correlated-activityid"];
      this.IsThroughputCapRequest = nameValueCollection["x-ms-cosmos-internal-is-throughputcap-request"];
      this.ChangeFeedWireFormatVersion = nameValueCollection["x-ms-cosmos-changefeed-wire-format-version"];
      this.PopulateByokEncryptionProgress = nameValueCollection["x-ms-cosmos-populate-byok-encryption-progress"];
      this.UseUserBackgroundBudget = nameValueCollection["x-ms-cosmos-use-background-task-budget"];
      this.IncludePhysicalPartitionThroughputInfo = nameValueCollection["x-ms-cosmos-include-physical-partition-throughput-info"];
      this.IsServerlessStorageRefreshRequest = nameValueCollection["x-ms-cosmos-internal-serverless-offer-storage-refresh-request"];
      this.UpdateOfferStateToPending = nameValueCollection["x-ms-cosmos-internal-update-offer-state-to-pending"];
      this.PopulateOldestActiveSchema = nameValueCollection["x-ms-cosmos-populate-oldest-active-schema"];
      this.IsInternalServerlessRequest = nameValueCollection["x-ms-cosmos-internal-serverless-request"];
      this.OfferReplaceRURedistribution = nameValueCollection["x-ms-cosmos-internal-offer-replace-ru-redistribution"];
      this.IfMatch = nameValueCollection["If-Match"];
    }

    private RequestNameValueCollection(Lazy<Dictionary<string, string>> notCommonHeaders) => this.lazyNotCommonHeaders = notCommonHeaders ?? throw new ArgumentNullException(nameof (notCommonHeaders));

    public string this[string key]
    {
      get => this.Get(key);
      set => this.Set(key, value);
    }

    public void Add(INameValueCollection collection)
    {
      if (collection == null)
        throw new ArgumentNullException(nameof (collection));
      foreach (string key in collection.Keys())
        this.Set(key, collection[key]);
    }

    public string[] AllKeys() => this.Keys().ToArray<string>();

    public void Clear()
    {
      if (this.lazyNotCommonHeaders.IsValueCreated)
        this.lazyNotCommonHeaders.Value.Clear();
      this.A_IM = (string) null;
      this.AddResourcePropertiesToResponse = (string) null;
      this.AllowTentativeWrites = (string) null;
      this.Authorization = (string) null;
      this.BinaryId = (string) null;
      this.BinaryPassthroughRequest = (string) null;
      this.BindReplicaDirective = (string) null;
      this.BuilderClientIdentifier = (string) null;
      this.CanCharge = (string) null;
      this.CanOfferReplaceComplete = (string) null;
      this.CanThrottle = (string) null;
      this.ChangeFeedStartFullFidelityIfNoneMatch = (string) null;
      this.ChangeFeedWireFormatVersion = (string) null;
      this.ClientRetryAttemptCount = (string) null;
      this.CollectionChildResourceContentLimitInKB = (string) null;
      this.CollectionChildResourceNameLimitInBytes = (string) null;
      this.CollectionPartitionIndex = (string) null;
      this.CollectionRemoteStorageSecurityIdentifier = (string) null;
      this.CollectionRid = (string) null;
      this.CollectionServiceIndex = (string) null;
      this.CollectionTruncate = (string) null;
      this.ConsistencyLevel = (string) null;
      this.ContentSerializationFormat = (string) null;
      this.Continuation = (string) null;
      this.CorrelatedActivityId = (string) null;
      this.DisableRUPerMinuteUsage = (string) null;
      this.EffectivePartitionKey = (string) null;
      this.EmitVerboseTracesInQuery = (string) null;
      this.EnableDynamicRidRangeAllocation = (string) null;
      this.EnableLogging = (string) null;
      this.EnableLowPrecisionOrderBy = (string) null;
      this.EnableScanInQuery = (string) null;
      this.EndEpk = (string) null;
      this.EndId = (string) null;
      this.EntityId = (string) null;
      this.EnumerationDirection = (string) null;
      this.ExcludeSystemProperties = (string) null;
      this.FanoutOperationState = (string) null;
      this.FilterBySchemaResourceId = (string) null;
      this.ForceQueryScan = (string) null;
      this.ForceSideBySideIndexMigration = (string) null;
      this.GatewaySignature = (string) null;
      this.GetAllPartitionKeyStatistics = (string) null;
      this.HttpDate = (string) null;
      this.IfMatch = (string) null;
      this.IfModifiedSince = (string) null;
      this.IfNoneMatch = (string) null;
      this.IgnoreSystemLoweringMaxThroughput = (string) null;
      this.IncludePhysicalPartitionThroughputInfo = (string) null;
      this.IncludeTentativeWrites = (string) null;
      this.IndexingDirective = (string) null;
      this.IntendedCollectionRid = (string) null;
      this.IsAutoScaleRequest = (string) null;
      this.IsBatchAtomic = (string) null;
      this.IsBatchOrdered = (string) null;
      this.IsClientEncrypted = (string) null;
      this.IsFanoutRequest = (string) null;
      this.IsInternalServerlessRequest = (string) null;
      this.IsMaterializedViewBuild = (string) null;
      this.IsOfferStorageRefreshRequest = (string) null;
      this.IsReadOnlyScript = (string) null;
      this.IsRetriedWriteRequest = (string) null;
      this.IsRUPerGBEnforcementRequest = (string) null;
      this.IsServerlessStorageRefreshRequest = (string) null;
      this.IsThroughputCapRequest = (string) null;
      this.IsUserRequest = (string) null;
      this.MaxPollingIntervalMilliseconds = (string) null;
      this.MergeCheckPointGLSN = (string) null;
      this.MergeStaticId = (string) null;
      this.MigrateCollectionDirective = (string) null;
      this.MigrateOfferToAutopilot = (string) null;
      this.MigrateOfferToManualThroughput = (string) null;
      this.OfferReplaceRURedistribution = (string) null;
      this.PageSize = (string) null;
      this.PartitionCount = (string) null;
      this.PartitionKey = (string) null;
      this.PartitionKeyRangeId = (string) null;
      this.PartitionResourceFilter = (string) null;
      this.PopulateAnalyticalMigrationProgress = (string) null;
      this.PopulateByokEncryptionProgress = (string) null;
      this.PopulateCollectionThroughputInfo = (string) null;
      this.PopulateIndexMetrics = (string) null;
      this.PopulateLogStoreInfo = (string) null;
      this.PopulateOldestActiveSchema = (string) null;
      this.PopulatePartitionStatistics = (string) null;
      this.PopulateQueryMetrics = (string) null;
      this.PopulateQuotaInfo = (string) null;
      this.PopulateResourceCount = (string) null;
      this.PopulateUnflushedMergeEntryCount = (string) null;
      this.PopulateUniqueIndexReIndexProgress = (string) null;
      this.PostTriggerExclude = (string) null;
      this.PostTriggerInclude = (string) null;
      this.Prefer = (string) null;
      this.PreserveFullContent = (string) null;
      this.PreTriggerExclude = (string) null;
      this.PreTriggerInclude = (string) null;
      this.PrimaryMasterKey = (string) null;
      this.PrimaryReadonlyKey = (string) null;
      this.ProfileRequest = (string) null;
      this.RbacAction = (string) null;
      this.RbacResource = (string) null;
      this.RbacUserId = (string) null;
      this.ReadFeedKeyType = (string) null;
      this.RemainingTimeInMsOnClientRequest = (string) null;
      this.RemoteStorageType = (string) null;
      this.RequestedCollectionType = (string) null;
      this.ResourceId = (string) null;
      this.ResourceSchemaName = (string) null;
      this.ResourceTokenExpiry = (string) null;
      this.ResourceTypes = (string) null;
      this.ResponseContinuationTokenLimitInKB = (string) null;
      this.RestoreMetadataFilter = (string) null;
      this.RestoreParams = (string) null;
      this.RetriableWriteRequestId = (string) null;
      this.RetriableWriteRequestStartTimestamp = (string) null;
      this.SchemaHash = (string) null;
      this.SchemaId = (string) null;
      this.SchemaOwnerRid = (string) null;
      this.SDKSupportedCapabilities = (string) null;
      this.SecondaryMasterKey = (string) null;
      this.SecondaryReadonlyKey = (string) null;
      this.SessionToken = (string) null;
      this.ShareThroughput = (string) null;
      this.ShouldBatchContinueOnError = (string) null;
      this.ShouldReturnCurrentServerDateTime = (string) null;
      this.SkipRefreshDatabaseAccountConfigs = (string) null;
      this.SourceCollectionIfMatch = (string) null;
      this.StartEpk = (string) null;
      this.StartId = (string) null;
      this.SupportSpatialLegacyCoordinates = (string) null;
      this.SystemDocumentType = (string) null;
      this.SystemRestoreOperation = (string) null;
      this.TargetGlobalCommittedLsn = (string) null;
      this.TargetLsn = (string) null;
      this.TimeToLiveInSeconds = (string) null;
      this.TransactionCommit = (string) null;
      this.TransactionFirstRequest = (string) null;
      this.TransactionId = (string) null;
      this.TransportRequestID = (string) null;
      this.TruncateMergeLogRequest = (string) null;
      this.UniqueIndexNameEncodingMode = (string) null;
      this.UniqueIndexReIndexingState = (string) null;
      this.UpdateMaxThroughputEverProvisioned = (string) null;
      this.UpdateOfferStateToPending = (string) null;
      this.UseArchivalPartition = (string) null;
      this.UsePolygonsSmallerThanAHemisphere = (string) null;
      this.UseSystemBudget = (string) null;
      this.UseUserBackgroundBudget = (string) null;
      this.Version = (string) null;
      this.XDate = (string) null;
    }

    public INameValueCollection Clone()
    {
      Lazy<Dictionary<string, string>> notCommonHeaders = new Lazy<Dictionary<string, string>>((Func<Dictionary<string, string>>) (() => new Dictionary<string, string>((IEqualityComparer<string>) RequestNameValueCollection.DefaultStringComparer)));
      if (this.lazyNotCommonHeaders.IsValueCreated)
      {
        foreach (KeyValuePair<string, string> keyValuePair in this.lazyNotCommonHeaders.Value)
          notCommonHeaders.Value[keyValuePair.Key] = keyValuePair.Value;
      }
      return (INameValueCollection) new RequestNameValueCollection(notCommonHeaders)
      {
        A_IM = this.A_IM,
        AddResourcePropertiesToResponse = this.AddResourcePropertiesToResponse,
        AllowTentativeWrites = this.AllowTentativeWrites,
        Authorization = this.Authorization,
        BinaryId = this.BinaryId,
        BinaryPassthroughRequest = this.BinaryPassthroughRequest,
        BindReplicaDirective = this.BindReplicaDirective,
        BuilderClientIdentifier = this.BuilderClientIdentifier,
        CanCharge = this.CanCharge,
        CanOfferReplaceComplete = this.CanOfferReplaceComplete,
        CanThrottle = this.CanThrottle,
        ChangeFeedStartFullFidelityIfNoneMatch = this.ChangeFeedStartFullFidelityIfNoneMatch,
        ChangeFeedWireFormatVersion = this.ChangeFeedWireFormatVersion,
        ClientRetryAttemptCount = this.ClientRetryAttemptCount,
        CollectionChildResourceContentLimitInKB = this.CollectionChildResourceContentLimitInKB,
        CollectionChildResourceNameLimitInBytes = this.CollectionChildResourceNameLimitInBytes,
        CollectionPartitionIndex = this.CollectionPartitionIndex,
        CollectionRemoteStorageSecurityIdentifier = this.CollectionRemoteStorageSecurityIdentifier,
        CollectionRid = this.CollectionRid,
        CollectionServiceIndex = this.CollectionServiceIndex,
        CollectionTruncate = this.CollectionTruncate,
        ConsistencyLevel = this.ConsistencyLevel,
        ContentSerializationFormat = this.ContentSerializationFormat,
        Continuation = this.Continuation,
        CorrelatedActivityId = this.CorrelatedActivityId,
        DisableRUPerMinuteUsage = this.DisableRUPerMinuteUsage,
        EffectivePartitionKey = this.EffectivePartitionKey,
        EmitVerboseTracesInQuery = this.EmitVerboseTracesInQuery,
        EnableDynamicRidRangeAllocation = this.EnableDynamicRidRangeAllocation,
        EnableLogging = this.EnableLogging,
        EnableLowPrecisionOrderBy = this.EnableLowPrecisionOrderBy,
        EnableScanInQuery = this.EnableScanInQuery,
        EndEpk = this.EndEpk,
        EndId = this.EndId,
        EntityId = this.EntityId,
        EnumerationDirection = this.EnumerationDirection,
        ExcludeSystemProperties = this.ExcludeSystemProperties,
        FanoutOperationState = this.FanoutOperationState,
        FilterBySchemaResourceId = this.FilterBySchemaResourceId,
        ForceQueryScan = this.ForceQueryScan,
        ForceSideBySideIndexMigration = this.ForceSideBySideIndexMigration,
        GatewaySignature = this.GatewaySignature,
        GetAllPartitionKeyStatistics = this.GetAllPartitionKeyStatistics,
        HttpDate = this.HttpDate,
        IfMatch = this.IfMatch,
        IfModifiedSince = this.IfModifiedSince,
        IfNoneMatch = this.IfNoneMatch,
        IgnoreSystemLoweringMaxThroughput = this.IgnoreSystemLoweringMaxThroughput,
        IncludePhysicalPartitionThroughputInfo = this.IncludePhysicalPartitionThroughputInfo,
        IncludeTentativeWrites = this.IncludeTentativeWrites,
        IndexingDirective = this.IndexingDirective,
        IntendedCollectionRid = this.IntendedCollectionRid,
        IsAutoScaleRequest = this.IsAutoScaleRequest,
        IsBatchAtomic = this.IsBatchAtomic,
        IsBatchOrdered = this.IsBatchOrdered,
        IsClientEncrypted = this.IsClientEncrypted,
        IsFanoutRequest = this.IsFanoutRequest,
        IsInternalServerlessRequest = this.IsInternalServerlessRequest,
        IsMaterializedViewBuild = this.IsMaterializedViewBuild,
        IsOfferStorageRefreshRequest = this.IsOfferStorageRefreshRequest,
        IsReadOnlyScript = this.IsReadOnlyScript,
        IsRetriedWriteRequest = this.IsRetriedWriteRequest,
        IsRUPerGBEnforcementRequest = this.IsRUPerGBEnforcementRequest,
        IsServerlessStorageRefreshRequest = this.IsServerlessStorageRefreshRequest,
        IsThroughputCapRequest = this.IsThroughputCapRequest,
        IsUserRequest = this.IsUserRequest,
        MaxPollingIntervalMilliseconds = this.MaxPollingIntervalMilliseconds,
        MergeCheckPointGLSN = this.MergeCheckPointGLSN,
        MergeStaticId = this.MergeStaticId,
        MigrateCollectionDirective = this.MigrateCollectionDirective,
        MigrateOfferToAutopilot = this.MigrateOfferToAutopilot,
        MigrateOfferToManualThroughput = this.MigrateOfferToManualThroughput,
        OfferReplaceRURedistribution = this.OfferReplaceRURedistribution,
        PageSize = this.PageSize,
        PartitionCount = this.PartitionCount,
        PartitionKey = this.PartitionKey,
        PartitionKeyRangeId = this.PartitionKeyRangeId,
        PartitionResourceFilter = this.PartitionResourceFilter,
        PopulateAnalyticalMigrationProgress = this.PopulateAnalyticalMigrationProgress,
        PopulateByokEncryptionProgress = this.PopulateByokEncryptionProgress,
        PopulateCollectionThroughputInfo = this.PopulateCollectionThroughputInfo,
        PopulateIndexMetrics = this.PopulateIndexMetrics,
        PopulateLogStoreInfo = this.PopulateLogStoreInfo,
        PopulateOldestActiveSchema = this.PopulateOldestActiveSchema,
        PopulatePartitionStatistics = this.PopulatePartitionStatistics,
        PopulateQueryMetrics = this.PopulateQueryMetrics,
        PopulateQuotaInfo = this.PopulateQuotaInfo,
        PopulateResourceCount = this.PopulateResourceCount,
        PopulateUnflushedMergeEntryCount = this.PopulateUnflushedMergeEntryCount,
        PopulateUniqueIndexReIndexProgress = this.PopulateUniqueIndexReIndexProgress,
        PostTriggerExclude = this.PostTriggerExclude,
        PostTriggerInclude = this.PostTriggerInclude,
        Prefer = this.Prefer,
        PreserveFullContent = this.PreserveFullContent,
        PreTriggerExclude = this.PreTriggerExclude,
        PreTriggerInclude = this.PreTriggerInclude,
        PrimaryMasterKey = this.PrimaryMasterKey,
        PrimaryReadonlyKey = this.PrimaryReadonlyKey,
        ProfileRequest = this.ProfileRequest,
        RbacAction = this.RbacAction,
        RbacResource = this.RbacResource,
        RbacUserId = this.RbacUserId,
        ReadFeedKeyType = this.ReadFeedKeyType,
        RemainingTimeInMsOnClientRequest = this.RemainingTimeInMsOnClientRequest,
        RemoteStorageType = this.RemoteStorageType,
        RequestedCollectionType = this.RequestedCollectionType,
        ResourceId = this.ResourceId,
        ResourceSchemaName = this.ResourceSchemaName,
        ResourceTokenExpiry = this.ResourceTokenExpiry,
        ResourceTypes = this.ResourceTypes,
        ResponseContinuationTokenLimitInKB = this.ResponseContinuationTokenLimitInKB,
        RestoreMetadataFilter = this.RestoreMetadataFilter,
        RestoreParams = this.RestoreParams,
        RetriableWriteRequestId = this.RetriableWriteRequestId,
        RetriableWriteRequestStartTimestamp = this.RetriableWriteRequestStartTimestamp,
        SchemaHash = this.SchemaHash,
        SchemaId = this.SchemaId,
        SchemaOwnerRid = this.SchemaOwnerRid,
        SDKSupportedCapabilities = this.SDKSupportedCapabilities,
        SecondaryMasterKey = this.SecondaryMasterKey,
        SecondaryReadonlyKey = this.SecondaryReadonlyKey,
        SessionToken = this.SessionToken,
        ShareThroughput = this.ShareThroughput,
        ShouldBatchContinueOnError = this.ShouldBatchContinueOnError,
        ShouldReturnCurrentServerDateTime = this.ShouldReturnCurrentServerDateTime,
        SkipRefreshDatabaseAccountConfigs = this.SkipRefreshDatabaseAccountConfigs,
        SourceCollectionIfMatch = this.SourceCollectionIfMatch,
        StartEpk = this.StartEpk,
        StartId = this.StartId,
        SupportSpatialLegacyCoordinates = this.SupportSpatialLegacyCoordinates,
        SystemDocumentType = this.SystemDocumentType,
        SystemRestoreOperation = this.SystemRestoreOperation,
        TargetGlobalCommittedLsn = this.TargetGlobalCommittedLsn,
        TargetLsn = this.TargetLsn,
        TimeToLiveInSeconds = this.TimeToLiveInSeconds,
        TransactionCommit = this.TransactionCommit,
        TransactionFirstRequest = this.TransactionFirstRequest,
        TransactionId = this.TransactionId,
        TransportRequestID = this.TransportRequestID,
        TruncateMergeLogRequest = this.TruncateMergeLogRequest,
        UniqueIndexNameEncodingMode = this.UniqueIndexNameEncodingMode,
        UniqueIndexReIndexingState = this.UniqueIndexReIndexingState,
        UpdateMaxThroughputEverProvisioned = this.UpdateMaxThroughputEverProvisioned,
        UpdateOfferStateToPending = this.UpdateOfferStateToPending,
        UseArchivalPartition = this.UseArchivalPartition,
        UsePolygonsSmallerThanAHemisphere = this.UsePolygonsSmallerThanAHemisphere,
        UseSystemBudget = this.UseSystemBudget,
        UseUserBackgroundBudget = this.UseUserBackgroundBudget,
        Version = this.Version,
        XDate = this.XDate
      };
    }

    public int Count() => this.Keys().Count<string>();

    public IEnumerator GetEnumerator() => (IEnumerator) this.Keys().GetEnumerator();

    public string[] GetValues(string key)
    {
      string str = this.Get(key);
      if (str == null)
        return (string[]) null;
      return new string[1]{ str };
    }

    public IEnumerable<string> Keys()
    {
      if (this.ResourceId != null)
        yield return "x-docdb-resource-id";
      if (this.Authorization != null)
        yield return "authorization";
      if (this.HttpDate != null)
        yield return "date";
      if (this.XDate != null)
        yield return "x-ms-date";
      if (this.PageSize != null)
        yield return "x-ms-max-item-count";
      if (this.SessionToken != null)
        yield return "x-ms-session-token";
      if (this.Continuation != null)
        yield return "x-ms-continuation";
      if (this.IndexingDirective != null)
        yield return "x-ms-indexing-directive";
      if (this.IfNoneMatch != null)
        yield return "If-None-Match";
      if (this.PreTriggerInclude != null)
        yield return "x-ms-documentdb-pre-trigger-include";
      if (this.PostTriggerInclude != null)
        yield return "x-ms-documentdb-post-trigger-include";
      if (this.IsFanoutRequest != null)
        yield return "x-ms-is-fanout-request";
      if (this.CollectionPartitionIndex != null)
        yield return "collection-partition-index";
      if (this.CollectionServiceIndex != null)
        yield return "collection-service-index";
      if (this.PreTriggerExclude != null)
        yield return "x-ms-documentdb-pre-trigger-exclude";
      if (this.PostTriggerExclude != null)
        yield return "x-ms-documentdb-post-trigger-exclude";
      if (this.ConsistencyLevel != null)
        yield return "x-ms-consistency-level";
      if (this.EntityId != null)
        yield return "x-docdb-entity-id";
      if (this.ResourceSchemaName != null)
        yield return "x-ms-resource-schema-name";
      if (this.ResourceTokenExpiry != null)
        yield return "x-ms-documentdb-expiry-seconds";
      if (this.EnableScanInQuery != null)
        yield return "x-ms-documentdb-query-enable-scan";
      if (this.EmitVerboseTracesInQuery != null)
        yield return "x-ms-documentdb-query-emit-traces";
      if (this.BindReplicaDirective != null)
        yield return "x-ms-bind-replica";
      if (this.PrimaryMasterKey != null)
        yield return "x-ms-primary-master-key";
      if (this.SecondaryMasterKey != null)
        yield return "x-ms-secondary-master-key";
      if (this.PrimaryReadonlyKey != null)
        yield return "x-ms-primary-readonly-key";
      if (this.SecondaryReadonlyKey != null)
        yield return "x-ms-secondary-readonly-key";
      if (this.ProfileRequest != null)
        yield return "x-ms-profile-request";
      if (this.EnableLowPrecisionOrderBy != null)
        yield return "x-ms-documentdb-query-enable-low-precision-order-by";
      if (this.Version != null)
        yield return "x-ms-version";
      if (this.CanCharge != null)
        yield return "x-ms-cancharge";
      if (this.CanThrottle != null)
        yield return "x-ms-canthrottle";
      if (this.PartitionKey != null)
        yield return "x-ms-documentdb-partitionkey";
      if (this.PartitionKeyRangeId != null)
        yield return "x-ms-documentdb-partitionkeyrangeid";
      if (this.MigrateCollectionDirective != null)
        yield return "x-ms-migratecollection-directive";
      if (this.SupportSpatialLegacyCoordinates != null)
        yield return "x-ms-documentdb-supportspatiallegacycoordinates";
      if (this.PartitionCount != null)
        yield return "x-ms-documentdb-partitioncount";
      if (this.CollectionRid != null)
        yield return "x-ms-documentdb-collection-rid";
      if (this.FilterBySchemaResourceId != null)
        yield return "x-ms-documentdb-filterby-schema-rid";
      if (this.UsePolygonsSmallerThanAHemisphere != null)
        yield return "x-ms-documentdb-usepolygonssmallerthanahemisphere";
      if (this.GatewaySignature != null)
        yield return "x-ms-gateway-signature";
      if (this.EnableLogging != null)
        yield return "x-ms-documentdb-script-enable-logging";
      if (this.A_IM != null)
        yield return "A-IM";
      if (this.IfModifiedSince != null)
        yield return "If-Modified-Since";
      if (this.PopulateQuotaInfo != null)
        yield return "x-ms-documentdb-populatequotainfo";
      if (this.DisableRUPerMinuteUsage != null)
        yield return "x-ms-documentdb-disable-ru-per-minute-usage";
      if (this.PopulateQueryMetrics != null)
        yield return "x-ms-documentdb-populatequerymetrics";
      if (this.ResponseContinuationTokenLimitInKB != null)
        yield return "x-ms-documentdb-responsecontinuationtokenlimitinkb";
      if (this.PopulatePartitionStatistics != null)
        yield return "x-ms-documentdb-populatepartitionstatistics";
      if (this.RemoteStorageType != null)
        yield return "x-ms-remote-storage-type";
      if (this.RemainingTimeInMsOnClientRequest != null)
        yield return "x-ms-remaining-time-in-ms-on-client";
      if (this.ClientRetryAttemptCount != null)
        yield return "x-ms-client-retry-attempt-count";
      if (this.TargetLsn != null)
        yield return "x-ms-target-lsn";
      if (this.TargetGlobalCommittedLsn != null)
        yield return "x-ms-target-global-committed-lsn";
      if (this.TransportRequestID != null)
        yield return "x-ms-transport-request-id";
      if (this.CollectionRemoteStorageSecurityIdentifier != null)
        yield return "x-ms-collection-security-identifier";
      if (this.PopulateCollectionThroughputInfo != null)
        yield return "x-ms-documentdb-populatecollectionthroughputinfo";
      if (this.RestoreMetadataFilter != null)
        yield return "x-ms-restore-metadata-filter";
      if (this.RestoreParams != null)
        yield return "x-ms-restore-params";
      if (this.ShareThroughput != null)
        yield return "x-ms-share-throughput";
      if (this.PartitionResourceFilter != null)
        yield return "x-ms-partition-resource-filter";
      if (this.IsReadOnlyScript != null)
        yield return "x-ms-is-readonly-script";
      if (this.IsAutoScaleRequest != null)
        yield return "x-ms-is-auto-scale";
      if (this.ForceQueryScan != null)
        yield return "x-ms-documentdb-force-query-scan";
      if (this.CanOfferReplaceComplete != null)
        yield return "x-ms-can-offer-replace-complete";
      if (this.ExcludeSystemProperties != null)
        yield return "x-ms-exclude-system-properties";
      if (this.BinaryId != null)
        yield return "x-ms-binary-id";
      if (this.TimeToLiveInSeconds != null)
        yield return "x-ms-time-to-live-in-seconds";
      if (this.EffectivePartitionKey != null)
        yield return "x-ms-effective-partition-key";
      if (this.BinaryPassthroughRequest != null)
        yield return "x-ms-binary-passthrough-request";
      if (this.EnableDynamicRidRangeAllocation != null)
        yield return "x-ms-enable-dynamic-rid-range-allocation";
      if (this.EnumerationDirection != null)
        yield return "x-ms-enumeration-direction";
      if (this.StartId != null)
        yield return "x-ms-start-id";
      if (this.EndId != null)
        yield return "x-ms-end-id";
      if (this.FanoutOperationState != null)
        yield return "x-ms-fanout-operation-state";
      if (this.StartEpk != null)
        yield return "x-ms-start-epk";
      if (this.EndEpk != null)
        yield return "x-ms-end-epk";
      if (this.ReadFeedKeyType != null)
        yield return "x-ms-read-key-type";
      if (this.ContentSerializationFormat != null)
        yield return "x-ms-documentdb-content-serialization-format";
      if (this.AllowTentativeWrites != null)
        yield return "x-ms-cosmos-allow-tentative-writes";
      if (this.IsUserRequest != null)
        yield return "x-ms-cosmos-internal-is-user-request";
      if (this.PreserveFullContent != null)
        yield return "x-ms-cosmos-preserve-full-content";
      if (this.IncludeTentativeWrites != null)
        yield return "x-ms-cosmos-include-tentative-writes";
      if (this.PopulateResourceCount != null)
        yield return "x-ms-documentdb-populateresourcecount";
      if (this.MergeStaticId != null)
        yield return "x-ms-cosmos-merge-static-id";
      if (this.IsBatchAtomic != null)
        yield return "x-ms-cosmos-batch-atomic";
      if (this.ShouldBatchContinueOnError != null)
        yield return "x-ms-cosmos-batch-continue-on-error";
      if (this.IsBatchOrdered != null)
        yield return "x-ms-cosmos-batch-ordered";
      if (this.SchemaOwnerRid != null)
        yield return "x-ms-schema-owner-rid";
      if (this.SchemaHash != null)
        yield return "x-ms-schema-hash";
      if (this.IsRUPerGBEnforcementRequest != null)
        yield return "x-ms-cosmos-internal-is-ru-per-gb-enforcement-request";
      if (this.MaxPollingIntervalMilliseconds != null)
        yield return "x-ms-cosmos-max-polling-interval";
      if (this.PopulateLogStoreInfo != null)
        yield return "x-ms-cosmos-populate-logstoreinfo";
      if (this.GetAllPartitionKeyStatistics != null)
        yield return "x-ms-cosmos-internal-get-all-partition-key-stats";
      if (this.ForceSideBySideIndexMigration != null)
        yield return "x-ms-cosmos-force-sidebyside-indexmigration";
      if (this.CollectionChildResourceNameLimitInBytes != null)
        yield return "x-ms-cosmos-collection-child-resourcename-limit";
      if (this.CollectionChildResourceContentLimitInKB != null)
        yield return "x-ms-cosmos-collection-child-contentlength-resourcelimit";
      if (this.MergeCheckPointGLSN != null)
        yield return "x-ms-cosmos-internal-merge-checkpoint-glsn";
      if (this.Prefer != null)
        yield return "Prefer";
      if (this.UniqueIndexNameEncodingMode != null)
        yield return "x-ms-cosmos-unique-index-name-encoding-mode";
      if (this.PopulateUnflushedMergeEntryCount != null)
        yield return "x-ms-cosmos-internal-populate-unflushed-merge-entry-count";
      if (this.MigrateOfferToManualThroughput != null)
        yield return "x-ms-cosmos-migrate-offer-to-manual-throughput";
      if (this.MigrateOfferToAutopilot != null)
        yield return "x-ms-cosmos-migrate-offer-to-autopilot";
      if (this.IsClientEncrypted != null)
        yield return "x-ms-cosmos-is-client-encrypted";
      if (this.SystemDocumentType != null)
        yield return "x-ms-cosmos-systemdocument-type";
      if (this.IsOfferStorageRefreshRequest != null)
        yield return "x-ms-cosmos-internal-is-offer-storage-refresh-request";
      if (this.ResourceTypes != null)
        yield return "x-ms-cosmos-resourcetypes";
      if (this.TransactionId != null)
        yield return "x-ms-cosmos-tx-id";
      if (this.TransactionFirstRequest != null)
        yield return "x-ms-cosmos-tx-init";
      if (this.TransactionCommit != null)
        yield return "x-ms-cosmos-tx-commit";
      if (this.UpdateMaxThroughputEverProvisioned != null)
        yield return "x-ms-cosmos-internal-update-max-throughput-ever-provisioned";
      if (this.UniqueIndexReIndexingState != null)
        yield return "x-ms-cosmos-uniqueindex-reindexing-state";
      if (this.UseSystemBudget != null)
        yield return "x-ms-cosmos-use-systembudget";
      if (this.IgnoreSystemLoweringMaxThroughput != null)
        yield return "x-ms-cosmos-internal-ignore-system-lowering-max-throughput";
      if (this.TruncateMergeLogRequest != null)
        yield return "x-ms-cosmos-internal-truncate-merge-log";
      if (this.RetriableWriteRequestId != null)
        yield return "x-ms-cosmos-retriable-write-request-id";
      if (this.IsRetriedWriteRequest != null)
        yield return "x-ms-cosmos-is-retried-write-request";
      if (this.RetriableWriteRequestStartTimestamp != null)
        yield return "x-ms-cosmos-retriable-write-request-start-timestamp";
      if (this.AddResourcePropertiesToResponse != null)
        yield return "x-ms-cosmos-add-resource-properties-to-response";
      if (this.ChangeFeedStartFullFidelityIfNoneMatch != null)
        yield return "x-ms-cosmos-start-full-fidelity-if-none-match";
      if (this.SystemRestoreOperation != null)
        yield return "x-ms-cosmos-internal-system-restore-operation";
      if (this.SkipRefreshDatabaseAccountConfigs != null)
        yield return "x-ms-cosmos-skip-refresh-databaseaccountconfig";
      if (this.IntendedCollectionRid != null)
        yield return "x-ms-cosmos-intended-collection-rid";
      if (this.UseArchivalPartition != null)
        yield return "x-ms-cosmos-use-archival-partition";
      if (this.PopulateUniqueIndexReIndexProgress != null)
        yield return "x-ms-cosmosdb-populateuniqueindexreindexprogress";
      if (this.SchemaId != null)
        yield return "x-ms-schema-id";
      if (this.CollectionTruncate != null)
        yield return "x-ms-cosmos-collection-truncate";
      if (this.SDKSupportedCapabilities != null)
        yield return "x-ms-cosmos-sdk-supportedcapabilities";
      if (this.IsMaterializedViewBuild != null)
        yield return "x-ms-cosmos-internal-is-materialized-view-build";
      if (this.BuilderClientIdentifier != null)
        yield return "x-ms-cosmos-builder-client-identifier";
      if (this.SourceCollectionIfMatch != null)
        yield return "x-ms-cosmos-source-collection-if-match";
      if (this.RequestedCollectionType != null)
        yield return "x-ms-cosmos-collectiontype";
      if (this.PopulateIndexMetrics != null)
        yield return "x-ms-cosmos-populateindexmetrics";
      if (this.PopulateAnalyticalMigrationProgress != null)
        yield return "x-ms-cosmos-populate-analytical-migration-progress";
      if (this.ShouldReturnCurrentServerDateTime != null)
        yield return "x-ms-should-return-current-server-datetime";
      if (this.RbacUserId != null)
        yield return "x-ms-rbac-user-id";
      if (this.RbacAction != null)
        yield return "x-ms-rbac-action";
      if (this.RbacResource != null)
        yield return "x-ms-rbac-resource";
      if (this.CorrelatedActivityId != null)
        yield return "x-ms-cosmos-correlated-activityid";
      if (this.IsThroughputCapRequest != null)
        yield return "x-ms-cosmos-internal-is-throughputcap-request";
      if (this.ChangeFeedWireFormatVersion != null)
        yield return "x-ms-cosmos-changefeed-wire-format-version";
      if (this.PopulateByokEncryptionProgress != null)
        yield return "x-ms-cosmos-populate-byok-encryption-progress";
      if (this.UseUserBackgroundBudget != null)
        yield return "x-ms-cosmos-use-background-task-budget";
      if (this.IncludePhysicalPartitionThroughputInfo != null)
        yield return "x-ms-cosmos-include-physical-partition-throughput-info";
      if (this.IsServerlessStorageRefreshRequest != null)
        yield return "x-ms-cosmos-internal-serverless-offer-storage-refresh-request";
      if (this.UpdateOfferStateToPending != null)
        yield return "x-ms-cosmos-internal-update-offer-state-to-pending";
      if (this.PopulateOldestActiveSchema != null)
        yield return "x-ms-cosmos-populate-oldest-active-schema";
      if (this.IsInternalServerlessRequest != null)
        yield return "x-ms-cosmos-internal-serverless-request";
      if (this.OfferReplaceRURedistribution != null)
        yield return "x-ms-cosmos-internal-offer-replace-ru-redistribution";
      if (this.IfMatch != null)
        yield return "If-Match";
      if (this.lazyNotCommonHeaders.IsValueCreated)
      {
        foreach (string key in this.lazyNotCommonHeaders.Value.Keys)
          yield return key;
      }
    }

    public NameValueCollection ToNameValueCollection()
    {
      if (this.nameValueCollection == null)
      {
        lock (this)
        {
          if (this.nameValueCollection == null)
          {
            this.nameValueCollection = new NameValueCollection(this.Count(), (IEqualityComparer) RequestNameValueCollection.DefaultStringComparer);
            if (this.ResourceId != null)
              this.nameValueCollection.Add("x-docdb-resource-id", this.ResourceId);
            if (this.Authorization != null)
              this.nameValueCollection.Add("authorization", this.Authorization);
            if (this.HttpDate != null)
              this.nameValueCollection.Add("date", this.HttpDate);
            if (this.XDate != null)
              this.nameValueCollection.Add("x-ms-date", this.XDate);
            if (this.PageSize != null)
              this.nameValueCollection.Add("x-ms-max-item-count", this.PageSize);
            if (this.SessionToken != null)
              this.nameValueCollection.Add("x-ms-session-token", this.SessionToken);
            if (this.Continuation != null)
              this.nameValueCollection.Add("x-ms-continuation", this.Continuation);
            if (this.IndexingDirective != null)
              this.nameValueCollection.Add("x-ms-indexing-directive", this.IndexingDirective);
            if (this.IfNoneMatch != null)
              this.nameValueCollection.Add("If-None-Match", this.IfNoneMatch);
            if (this.PreTriggerInclude != null)
              this.nameValueCollection.Add("x-ms-documentdb-pre-trigger-include", this.PreTriggerInclude);
            if (this.PostTriggerInclude != null)
              this.nameValueCollection.Add("x-ms-documentdb-post-trigger-include", this.PostTriggerInclude);
            if (this.IsFanoutRequest != null)
              this.nameValueCollection.Add("x-ms-is-fanout-request", this.IsFanoutRequest);
            if (this.CollectionPartitionIndex != null)
              this.nameValueCollection.Add("collection-partition-index", this.CollectionPartitionIndex);
            if (this.CollectionServiceIndex != null)
              this.nameValueCollection.Add("collection-service-index", this.CollectionServiceIndex);
            if (this.PreTriggerExclude != null)
              this.nameValueCollection.Add("x-ms-documentdb-pre-trigger-exclude", this.PreTriggerExclude);
            if (this.PostTriggerExclude != null)
              this.nameValueCollection.Add("x-ms-documentdb-post-trigger-exclude", this.PostTriggerExclude);
            if (this.ConsistencyLevel != null)
              this.nameValueCollection.Add("x-ms-consistency-level", this.ConsistencyLevel);
            if (this.EntityId != null)
              this.nameValueCollection.Add("x-docdb-entity-id", this.EntityId);
            if (this.ResourceSchemaName != null)
              this.nameValueCollection.Add("x-ms-resource-schema-name", this.ResourceSchemaName);
            if (this.ResourceTokenExpiry != null)
              this.nameValueCollection.Add("x-ms-documentdb-expiry-seconds", this.ResourceTokenExpiry);
            if (this.EnableScanInQuery != null)
              this.nameValueCollection.Add("x-ms-documentdb-query-enable-scan", this.EnableScanInQuery);
            if (this.EmitVerboseTracesInQuery != null)
              this.nameValueCollection.Add("x-ms-documentdb-query-emit-traces", this.EmitVerboseTracesInQuery);
            if (this.BindReplicaDirective != null)
              this.nameValueCollection.Add("x-ms-bind-replica", this.BindReplicaDirective);
            if (this.PrimaryMasterKey != null)
              this.nameValueCollection.Add("x-ms-primary-master-key", this.PrimaryMasterKey);
            if (this.SecondaryMasterKey != null)
              this.nameValueCollection.Add("x-ms-secondary-master-key", this.SecondaryMasterKey);
            if (this.PrimaryReadonlyKey != null)
              this.nameValueCollection.Add("x-ms-primary-readonly-key", this.PrimaryReadonlyKey);
            if (this.SecondaryReadonlyKey != null)
              this.nameValueCollection.Add("x-ms-secondary-readonly-key", this.SecondaryReadonlyKey);
            if (this.ProfileRequest != null)
              this.nameValueCollection.Add("x-ms-profile-request", this.ProfileRequest);
            if (this.EnableLowPrecisionOrderBy != null)
              this.nameValueCollection.Add("x-ms-documentdb-query-enable-low-precision-order-by", this.EnableLowPrecisionOrderBy);
            if (this.Version != null)
              this.nameValueCollection.Add("x-ms-version", this.Version);
            if (this.CanCharge != null)
              this.nameValueCollection.Add("x-ms-cancharge", this.CanCharge);
            if (this.CanThrottle != null)
              this.nameValueCollection.Add("x-ms-canthrottle", this.CanThrottle);
            if (this.PartitionKey != null)
              this.nameValueCollection.Add("x-ms-documentdb-partitionkey", this.PartitionKey);
            if (this.PartitionKeyRangeId != null)
              this.nameValueCollection.Add("x-ms-documentdb-partitionkeyrangeid", this.PartitionKeyRangeId);
            if (this.MigrateCollectionDirective != null)
              this.nameValueCollection.Add("x-ms-migratecollection-directive", this.MigrateCollectionDirective);
            if (this.SupportSpatialLegacyCoordinates != null)
              this.nameValueCollection.Add("x-ms-documentdb-supportspatiallegacycoordinates", this.SupportSpatialLegacyCoordinates);
            if (this.PartitionCount != null)
              this.nameValueCollection.Add("x-ms-documentdb-partitioncount", this.PartitionCount);
            if (this.CollectionRid != null)
              this.nameValueCollection.Add("x-ms-documentdb-collection-rid", this.CollectionRid);
            if (this.FilterBySchemaResourceId != null)
              this.nameValueCollection.Add("x-ms-documentdb-filterby-schema-rid", this.FilterBySchemaResourceId);
            if (this.UsePolygonsSmallerThanAHemisphere != null)
              this.nameValueCollection.Add("x-ms-documentdb-usepolygonssmallerthanahemisphere", this.UsePolygonsSmallerThanAHemisphere);
            if (this.GatewaySignature != null)
              this.nameValueCollection.Add("x-ms-gateway-signature", this.GatewaySignature);
            if (this.EnableLogging != null)
              this.nameValueCollection.Add("x-ms-documentdb-script-enable-logging", this.EnableLogging);
            if (this.A_IM != null)
              this.nameValueCollection.Add("A-IM", this.A_IM);
            if (this.IfModifiedSince != null)
              this.nameValueCollection.Add("If-Modified-Since", this.IfModifiedSince);
            if (this.PopulateQuotaInfo != null)
              this.nameValueCollection.Add("x-ms-documentdb-populatequotainfo", this.PopulateQuotaInfo);
            if (this.DisableRUPerMinuteUsage != null)
              this.nameValueCollection.Add("x-ms-documentdb-disable-ru-per-minute-usage", this.DisableRUPerMinuteUsage);
            if (this.PopulateQueryMetrics != null)
              this.nameValueCollection.Add("x-ms-documentdb-populatequerymetrics", this.PopulateQueryMetrics);
            if (this.ResponseContinuationTokenLimitInKB != null)
              this.nameValueCollection.Add("x-ms-documentdb-responsecontinuationtokenlimitinkb", this.ResponseContinuationTokenLimitInKB);
            if (this.PopulatePartitionStatistics != null)
              this.nameValueCollection.Add("x-ms-documentdb-populatepartitionstatistics", this.PopulatePartitionStatistics);
            if (this.RemoteStorageType != null)
              this.nameValueCollection.Add("x-ms-remote-storage-type", this.RemoteStorageType);
            if (this.RemainingTimeInMsOnClientRequest != null)
              this.nameValueCollection.Add("x-ms-remaining-time-in-ms-on-client", this.RemainingTimeInMsOnClientRequest);
            if (this.ClientRetryAttemptCount != null)
              this.nameValueCollection.Add("x-ms-client-retry-attempt-count", this.ClientRetryAttemptCount);
            if (this.TargetLsn != null)
              this.nameValueCollection.Add("x-ms-target-lsn", this.TargetLsn);
            if (this.TargetGlobalCommittedLsn != null)
              this.nameValueCollection.Add("x-ms-target-global-committed-lsn", this.TargetGlobalCommittedLsn);
            if (this.TransportRequestID != null)
              this.nameValueCollection.Add("x-ms-transport-request-id", this.TransportRequestID);
            if (this.CollectionRemoteStorageSecurityIdentifier != null)
              this.nameValueCollection.Add("x-ms-collection-security-identifier", this.CollectionRemoteStorageSecurityIdentifier);
            if (this.PopulateCollectionThroughputInfo != null)
              this.nameValueCollection.Add("x-ms-documentdb-populatecollectionthroughputinfo", this.PopulateCollectionThroughputInfo);
            if (this.RestoreMetadataFilter != null)
              this.nameValueCollection.Add("x-ms-restore-metadata-filter", this.RestoreMetadataFilter);
            if (this.RestoreParams != null)
              this.nameValueCollection.Add("x-ms-restore-params", this.RestoreParams);
            if (this.ShareThroughput != null)
              this.nameValueCollection.Add("x-ms-share-throughput", this.ShareThroughput);
            if (this.PartitionResourceFilter != null)
              this.nameValueCollection.Add("x-ms-partition-resource-filter", this.PartitionResourceFilter);
            if (this.IsReadOnlyScript != null)
              this.nameValueCollection.Add("x-ms-is-readonly-script", this.IsReadOnlyScript);
            if (this.IsAutoScaleRequest != null)
              this.nameValueCollection.Add("x-ms-is-auto-scale", this.IsAutoScaleRequest);
            if (this.ForceQueryScan != null)
              this.nameValueCollection.Add("x-ms-documentdb-force-query-scan", this.ForceQueryScan);
            if (this.CanOfferReplaceComplete != null)
              this.nameValueCollection.Add("x-ms-can-offer-replace-complete", this.CanOfferReplaceComplete);
            if (this.ExcludeSystemProperties != null)
              this.nameValueCollection.Add("x-ms-exclude-system-properties", this.ExcludeSystemProperties);
            if (this.BinaryId != null)
              this.nameValueCollection.Add("x-ms-binary-id", this.BinaryId);
            if (this.TimeToLiveInSeconds != null)
              this.nameValueCollection.Add("x-ms-time-to-live-in-seconds", this.TimeToLiveInSeconds);
            if (this.EffectivePartitionKey != null)
              this.nameValueCollection.Add("x-ms-effective-partition-key", this.EffectivePartitionKey);
            if (this.BinaryPassthroughRequest != null)
              this.nameValueCollection.Add("x-ms-binary-passthrough-request", this.BinaryPassthroughRequest);
            if (this.EnableDynamicRidRangeAllocation != null)
              this.nameValueCollection.Add("x-ms-enable-dynamic-rid-range-allocation", this.EnableDynamicRidRangeAllocation);
            if (this.EnumerationDirection != null)
              this.nameValueCollection.Add("x-ms-enumeration-direction", this.EnumerationDirection);
            if (this.StartId != null)
              this.nameValueCollection.Add("x-ms-start-id", this.StartId);
            if (this.EndId != null)
              this.nameValueCollection.Add("x-ms-end-id", this.EndId);
            if (this.FanoutOperationState != null)
              this.nameValueCollection.Add("x-ms-fanout-operation-state", this.FanoutOperationState);
            if (this.StartEpk != null)
              this.nameValueCollection.Add("x-ms-start-epk", this.StartEpk);
            if (this.EndEpk != null)
              this.nameValueCollection.Add("x-ms-end-epk", this.EndEpk);
            if (this.ReadFeedKeyType != null)
              this.nameValueCollection.Add("x-ms-read-key-type", this.ReadFeedKeyType);
            if (this.ContentSerializationFormat != null)
              this.nameValueCollection.Add("x-ms-documentdb-content-serialization-format", this.ContentSerializationFormat);
            if (this.AllowTentativeWrites != null)
              this.nameValueCollection.Add("x-ms-cosmos-allow-tentative-writes", this.AllowTentativeWrites);
            if (this.IsUserRequest != null)
              this.nameValueCollection.Add("x-ms-cosmos-internal-is-user-request", this.IsUserRequest);
            if (this.PreserveFullContent != null)
              this.nameValueCollection.Add("x-ms-cosmos-preserve-full-content", this.PreserveFullContent);
            if (this.IncludeTentativeWrites != null)
              this.nameValueCollection.Add("x-ms-cosmos-include-tentative-writes", this.IncludeTentativeWrites);
            if (this.PopulateResourceCount != null)
              this.nameValueCollection.Add("x-ms-documentdb-populateresourcecount", this.PopulateResourceCount);
            if (this.MergeStaticId != null)
              this.nameValueCollection.Add("x-ms-cosmos-merge-static-id", this.MergeStaticId);
            if (this.IsBatchAtomic != null)
              this.nameValueCollection.Add("x-ms-cosmos-batch-atomic", this.IsBatchAtomic);
            if (this.ShouldBatchContinueOnError != null)
              this.nameValueCollection.Add("x-ms-cosmos-batch-continue-on-error", this.ShouldBatchContinueOnError);
            if (this.IsBatchOrdered != null)
              this.nameValueCollection.Add("x-ms-cosmos-batch-ordered", this.IsBatchOrdered);
            if (this.SchemaOwnerRid != null)
              this.nameValueCollection.Add("x-ms-schema-owner-rid", this.SchemaOwnerRid);
            if (this.SchemaHash != null)
              this.nameValueCollection.Add("x-ms-schema-hash", this.SchemaHash);
            if (this.IsRUPerGBEnforcementRequest != null)
              this.nameValueCollection.Add("x-ms-cosmos-internal-is-ru-per-gb-enforcement-request", this.IsRUPerGBEnforcementRequest);
            if (this.MaxPollingIntervalMilliseconds != null)
              this.nameValueCollection.Add("x-ms-cosmos-max-polling-interval", this.MaxPollingIntervalMilliseconds);
            if (this.PopulateLogStoreInfo != null)
              this.nameValueCollection.Add("x-ms-cosmos-populate-logstoreinfo", this.PopulateLogStoreInfo);
            if (this.GetAllPartitionKeyStatistics != null)
              this.nameValueCollection.Add("x-ms-cosmos-internal-get-all-partition-key-stats", this.GetAllPartitionKeyStatistics);
            if (this.ForceSideBySideIndexMigration != null)
              this.nameValueCollection.Add("x-ms-cosmos-force-sidebyside-indexmigration", this.ForceSideBySideIndexMigration);
            if (this.CollectionChildResourceNameLimitInBytes != null)
              this.nameValueCollection.Add("x-ms-cosmos-collection-child-resourcename-limit", this.CollectionChildResourceNameLimitInBytes);
            if (this.CollectionChildResourceContentLimitInKB != null)
              this.nameValueCollection.Add("x-ms-cosmos-collection-child-contentlength-resourcelimit", this.CollectionChildResourceContentLimitInKB);
            if (this.MergeCheckPointGLSN != null)
              this.nameValueCollection.Add("x-ms-cosmos-internal-merge-checkpoint-glsn", this.MergeCheckPointGLSN);
            if (this.Prefer != null)
              this.nameValueCollection.Add("Prefer", this.Prefer);
            if (this.UniqueIndexNameEncodingMode != null)
              this.nameValueCollection.Add("x-ms-cosmos-unique-index-name-encoding-mode", this.UniqueIndexNameEncodingMode);
            if (this.PopulateUnflushedMergeEntryCount != null)
              this.nameValueCollection.Add("x-ms-cosmos-internal-populate-unflushed-merge-entry-count", this.PopulateUnflushedMergeEntryCount);
            if (this.MigrateOfferToManualThroughput != null)
              this.nameValueCollection.Add("x-ms-cosmos-migrate-offer-to-manual-throughput", this.MigrateOfferToManualThroughput);
            if (this.MigrateOfferToAutopilot != null)
              this.nameValueCollection.Add("x-ms-cosmos-migrate-offer-to-autopilot", this.MigrateOfferToAutopilot);
            if (this.IsClientEncrypted != null)
              this.nameValueCollection.Add("x-ms-cosmos-is-client-encrypted", this.IsClientEncrypted);
            if (this.SystemDocumentType != null)
              this.nameValueCollection.Add("x-ms-cosmos-systemdocument-type", this.SystemDocumentType);
            if (this.IsOfferStorageRefreshRequest != null)
              this.nameValueCollection.Add("x-ms-cosmos-internal-is-offer-storage-refresh-request", this.IsOfferStorageRefreshRequest);
            if (this.ResourceTypes != null)
              this.nameValueCollection.Add("x-ms-cosmos-resourcetypes", this.ResourceTypes);
            if (this.TransactionId != null)
              this.nameValueCollection.Add("x-ms-cosmos-tx-id", this.TransactionId);
            if (this.TransactionFirstRequest != null)
              this.nameValueCollection.Add("x-ms-cosmos-tx-init", this.TransactionFirstRequest);
            if (this.TransactionCommit != null)
              this.nameValueCollection.Add("x-ms-cosmos-tx-commit", this.TransactionCommit);
            if (this.UpdateMaxThroughputEverProvisioned != null)
              this.nameValueCollection.Add("x-ms-cosmos-internal-update-max-throughput-ever-provisioned", this.UpdateMaxThroughputEverProvisioned);
            if (this.UniqueIndexReIndexingState != null)
              this.nameValueCollection.Add("x-ms-cosmos-uniqueindex-reindexing-state", this.UniqueIndexReIndexingState);
            if (this.UseSystemBudget != null)
              this.nameValueCollection.Add("x-ms-cosmos-use-systembudget", this.UseSystemBudget);
            if (this.IgnoreSystemLoweringMaxThroughput != null)
              this.nameValueCollection.Add("x-ms-cosmos-internal-ignore-system-lowering-max-throughput", this.IgnoreSystemLoweringMaxThroughput);
            if (this.TruncateMergeLogRequest != null)
              this.nameValueCollection.Add("x-ms-cosmos-internal-truncate-merge-log", this.TruncateMergeLogRequest);
            if (this.RetriableWriteRequestId != null)
              this.nameValueCollection.Add("x-ms-cosmos-retriable-write-request-id", this.RetriableWriteRequestId);
            if (this.IsRetriedWriteRequest != null)
              this.nameValueCollection.Add("x-ms-cosmos-is-retried-write-request", this.IsRetriedWriteRequest);
            if (this.RetriableWriteRequestStartTimestamp != null)
              this.nameValueCollection.Add("x-ms-cosmos-retriable-write-request-start-timestamp", this.RetriableWriteRequestStartTimestamp);
            if (this.AddResourcePropertiesToResponse != null)
              this.nameValueCollection.Add("x-ms-cosmos-add-resource-properties-to-response", this.AddResourcePropertiesToResponse);
            if (this.ChangeFeedStartFullFidelityIfNoneMatch != null)
              this.nameValueCollection.Add("x-ms-cosmos-start-full-fidelity-if-none-match", this.ChangeFeedStartFullFidelityIfNoneMatch);
            if (this.SystemRestoreOperation != null)
              this.nameValueCollection.Add("x-ms-cosmos-internal-system-restore-operation", this.SystemRestoreOperation);
            if (this.SkipRefreshDatabaseAccountConfigs != null)
              this.nameValueCollection.Add("x-ms-cosmos-skip-refresh-databaseaccountconfig", this.SkipRefreshDatabaseAccountConfigs);
            if (this.IntendedCollectionRid != null)
              this.nameValueCollection.Add("x-ms-cosmos-intended-collection-rid", this.IntendedCollectionRid);
            if (this.UseArchivalPartition != null)
              this.nameValueCollection.Add("x-ms-cosmos-use-archival-partition", this.UseArchivalPartition);
            if (this.PopulateUniqueIndexReIndexProgress != null)
              this.nameValueCollection.Add("x-ms-cosmosdb-populateuniqueindexreindexprogress", this.PopulateUniqueIndexReIndexProgress);
            if (this.SchemaId != null)
              this.nameValueCollection.Add("x-ms-schema-id", this.SchemaId);
            if (this.CollectionTruncate != null)
              this.nameValueCollection.Add("x-ms-cosmos-collection-truncate", this.CollectionTruncate);
            if (this.SDKSupportedCapabilities != null)
              this.nameValueCollection.Add("x-ms-cosmos-sdk-supportedcapabilities", this.SDKSupportedCapabilities);
            if (this.IsMaterializedViewBuild != null)
              this.nameValueCollection.Add("x-ms-cosmos-internal-is-materialized-view-build", this.IsMaterializedViewBuild);
            if (this.BuilderClientIdentifier != null)
              this.nameValueCollection.Add("x-ms-cosmos-builder-client-identifier", this.BuilderClientIdentifier);
            if (this.SourceCollectionIfMatch != null)
              this.nameValueCollection.Add("x-ms-cosmos-source-collection-if-match", this.SourceCollectionIfMatch);
            if (this.RequestedCollectionType != null)
              this.nameValueCollection.Add("x-ms-cosmos-collectiontype", this.RequestedCollectionType);
            if (this.PopulateIndexMetrics != null)
              this.nameValueCollection.Add("x-ms-cosmos-populateindexmetrics", this.PopulateIndexMetrics);
            if (this.PopulateAnalyticalMigrationProgress != null)
              this.nameValueCollection.Add("x-ms-cosmos-populate-analytical-migration-progress", this.PopulateAnalyticalMigrationProgress);
            if (this.ShouldReturnCurrentServerDateTime != null)
              this.nameValueCollection.Add("x-ms-should-return-current-server-datetime", this.ShouldReturnCurrentServerDateTime);
            if (this.RbacUserId != null)
              this.nameValueCollection.Add("x-ms-rbac-user-id", this.RbacUserId);
            if (this.RbacAction != null)
              this.nameValueCollection.Add("x-ms-rbac-action", this.RbacAction);
            if (this.RbacResource != null)
              this.nameValueCollection.Add("x-ms-rbac-resource", this.RbacResource);
            if (this.CorrelatedActivityId != null)
              this.nameValueCollection.Add("x-ms-cosmos-correlated-activityid", this.CorrelatedActivityId);
            if (this.IsThroughputCapRequest != null)
              this.nameValueCollection.Add("x-ms-cosmos-internal-is-throughputcap-request", this.IsThroughputCapRequest);
            if (this.ChangeFeedWireFormatVersion != null)
              this.nameValueCollection.Add("x-ms-cosmos-changefeed-wire-format-version", this.ChangeFeedWireFormatVersion);
            if (this.PopulateByokEncryptionProgress != null)
              this.nameValueCollection.Add("x-ms-cosmos-populate-byok-encryption-progress", this.PopulateByokEncryptionProgress);
            if (this.UseUserBackgroundBudget != null)
              this.nameValueCollection.Add("x-ms-cosmos-use-background-task-budget", this.UseUserBackgroundBudget);
            if (this.IncludePhysicalPartitionThroughputInfo != null)
              this.nameValueCollection.Add("x-ms-cosmos-include-physical-partition-throughput-info", this.IncludePhysicalPartitionThroughputInfo);
            if (this.IsServerlessStorageRefreshRequest != null)
              this.nameValueCollection.Add("x-ms-cosmos-internal-serverless-offer-storage-refresh-request", this.IsServerlessStorageRefreshRequest);
            if (this.UpdateOfferStateToPending != null)
              this.nameValueCollection.Add("x-ms-cosmos-internal-update-offer-state-to-pending", this.UpdateOfferStateToPending);
            if (this.PopulateOldestActiveSchema != null)
              this.nameValueCollection.Add("x-ms-cosmos-populate-oldest-active-schema", this.PopulateOldestActiveSchema);
            if (this.IsInternalServerlessRequest != null)
              this.nameValueCollection.Add("x-ms-cosmos-internal-serverless-request", this.IsInternalServerlessRequest);
            if (this.OfferReplaceRURedistribution != null)
              this.nameValueCollection.Add("x-ms-cosmos-internal-offer-replace-ru-redistribution", this.OfferReplaceRURedistribution);
            if (this.IfMatch != null)
              this.nameValueCollection.Add("If-Match", this.IfMatch);
            if (this.lazyNotCommonHeaders.IsValueCreated)
            {
              foreach (KeyValuePair<string, string> keyValuePair in this.lazyNotCommonHeaders.Value)
                this.nameValueCollection.Add(keyValuePair.Key, keyValuePair.Value);
            }
          }
        }
      }
      return this.nameValueCollection;
    }

    public void Remove(string key)
    {
      if (key == null)
        throw new ArgumentNullException(nameof (key));
      this.UpdateHelper(key, (string) null, false);
    }

    public string Get(string key)
    {
      if (key == null)
        throw new ArgumentNullException(nameof (key));
      switch (key.Length)
      {
        case 4:
          if ((object) "date" == (object) key)
            return this.HttpDate;
          if ((object) "A-IM" == (object) key)
            return this.A_IM;
          if (string.Equals("date", key, StringComparison.OrdinalIgnoreCase))
            return this.HttpDate;
          if (string.Equals("A-IM", key, StringComparison.OrdinalIgnoreCase))
            return this.A_IM;
          break;
        case 6:
          if (string.Equals("Prefer", key, StringComparison.OrdinalIgnoreCase))
            return this.Prefer;
          break;
        case 8:
          if (string.Equals("If-Match", key, StringComparison.OrdinalIgnoreCase))
            return this.IfMatch;
          break;
        case 9:
          if (string.Equals("x-ms-date", key, StringComparison.OrdinalIgnoreCase))
            return this.XDate;
          break;
        case 11:
          if (string.Equals("x-ms-end-id", key, StringComparison.OrdinalIgnoreCase))
            return this.EndId;
          break;
        case 12:
          if ((object) "x-ms-version" == (object) key)
            return this.Version;
          if ((object) "x-ms-end-epk" == (object) key)
            return this.EndEpk;
          if (string.Equals("x-ms-version", key, StringComparison.OrdinalIgnoreCase))
            return this.Version;
          if (string.Equals("x-ms-end-epk", key, StringComparison.OrdinalIgnoreCase))
            return this.EndEpk;
          break;
        case 13:
          if ((object) "authorization" == (object) key)
            return this.Authorization;
          if ((object) "If-None-Match" == (object) key)
            return this.IfNoneMatch;
          if ((object) "x-ms-start-id" == (object) key)
            return this.StartId;
          if (string.Equals("authorization", key, StringComparison.OrdinalIgnoreCase))
            return this.Authorization;
          if (string.Equals("If-None-Match", key, StringComparison.OrdinalIgnoreCase))
            return this.IfNoneMatch;
          if (string.Equals("x-ms-start-id", key, StringComparison.OrdinalIgnoreCase))
            return this.StartId;
          break;
        case 14:
          if ((object) "x-ms-cancharge" == (object) key)
            return this.CanCharge;
          if ((object) "x-ms-binary-id" == (object) key)
            return this.BinaryId;
          if ((object) "x-ms-start-epk" == (object) key)
            return this.StartEpk;
          if ((object) "x-ms-schema-id" == (object) key)
            return this.SchemaId;
          if (string.Equals("x-ms-cancharge", key, StringComparison.OrdinalIgnoreCase))
            return this.CanCharge;
          if (string.Equals("x-ms-binary-id", key, StringComparison.OrdinalIgnoreCase))
            return this.BinaryId;
          if (string.Equals("x-ms-start-epk", key, StringComparison.OrdinalIgnoreCase))
            return this.StartEpk;
          if (string.Equals("x-ms-schema-id", key, StringComparison.OrdinalIgnoreCase))
            return this.SchemaId;
          break;
        case 15:
          if (string.Equals("x-ms-target-lsn", key, StringComparison.OrdinalIgnoreCase))
            return this.TargetLsn;
          break;
        case 16:
          if ((object) "x-ms-canthrottle" == (object) key)
            return this.CanThrottle;
          if ((object) "x-ms-schema-hash" == (object) key)
            return this.SchemaHash;
          if ((object) "x-ms-rbac-action" == (object) key)
            return this.RbacAction;
          if (string.Equals("x-ms-canthrottle", key, StringComparison.OrdinalIgnoreCase))
            return this.CanThrottle;
          if (string.Equals("x-ms-schema-hash", key, StringComparison.OrdinalIgnoreCase))
            return this.SchemaHash;
          if (string.Equals("x-ms-rbac-action", key, StringComparison.OrdinalIgnoreCase))
            return this.RbacAction;
          break;
        case 17:
          if ((object) "x-ms-continuation" == (object) key)
            return this.Continuation;
          if ((object) "x-docdb-entity-id" == (object) key)
            return this.EntityId;
          if ((object) "x-ms-bind-replica" == (object) key)
            return this.BindReplicaDirective;
          if ((object) "If-Modified-Since" == (object) key)
            return this.IfModifiedSince;
          if ((object) "x-ms-cosmos-tx-id" == (object) key)
            return this.TransactionId;
          if ((object) "x-ms-rbac-user-id" == (object) key)
            return this.RbacUserId;
          if (string.Equals("x-ms-continuation", key, StringComparison.OrdinalIgnoreCase))
            return this.Continuation;
          if (string.Equals("x-docdb-entity-id", key, StringComparison.OrdinalIgnoreCase))
            return this.EntityId;
          if (string.Equals("x-ms-bind-replica", key, StringComparison.OrdinalIgnoreCase))
            return this.BindReplicaDirective;
          if (string.Equals("If-Modified-Since", key, StringComparison.OrdinalIgnoreCase))
            return this.IfModifiedSince;
          if (string.Equals("x-ms-cosmos-tx-id", key, StringComparison.OrdinalIgnoreCase))
            return this.TransactionId;
          if (string.Equals("x-ms-rbac-user-id", key, StringComparison.OrdinalIgnoreCase))
            return this.RbacUserId;
          break;
        case 18:
          if ((object) "x-ms-session-token" == (object) key)
            return this.SessionToken;
          if ((object) "x-ms-is-auto-scale" == (object) key)
            return this.IsAutoScaleRequest;
          if ((object) "x-ms-read-key-type" == (object) key)
            return this.ReadFeedKeyType;
          if ((object) "x-ms-rbac-resource" == (object) key)
            return this.RbacResource;
          if (string.Equals("x-ms-session-token", key, StringComparison.OrdinalIgnoreCase))
            return this.SessionToken;
          if (string.Equals("x-ms-is-auto-scale", key, StringComparison.OrdinalIgnoreCase))
            return this.IsAutoScaleRequest;
          if (string.Equals("x-ms-read-key-type", key, StringComparison.OrdinalIgnoreCase))
            return this.ReadFeedKeyType;
          if (string.Equals("x-ms-rbac-resource", key, StringComparison.OrdinalIgnoreCase))
            return this.RbacResource;
          break;
        case 19:
          if ((object) "x-docdb-resource-id" == (object) key)
            return this.ResourceId;
          if ((object) "x-ms-max-item-count" == (object) key)
            return this.PageSize;
          if ((object) "x-ms-restore-params" == (object) key)
            return this.RestoreParams;
          if ((object) "x-ms-cosmos-tx-init" == (object) key)
            return this.TransactionFirstRequest;
          if (string.Equals("x-docdb-resource-id", key, StringComparison.OrdinalIgnoreCase))
            return this.ResourceId;
          if (string.Equals("x-ms-max-item-count", key, StringComparison.OrdinalIgnoreCase))
            return this.PageSize;
          if (string.Equals("x-ms-restore-params", key, StringComparison.OrdinalIgnoreCase))
            return this.RestoreParams;
          if (string.Equals("x-ms-cosmos-tx-init", key, StringComparison.OrdinalIgnoreCase))
            return this.TransactionFirstRequest;
          break;
        case 20:
          if (string.Equals("x-ms-profile-request", key, StringComparison.OrdinalIgnoreCase))
            return this.ProfileRequest;
          break;
        case 21:
          if ((object) "x-ms-share-throughput" == (object) key)
            return this.ShareThroughput;
          if ((object) "x-ms-schema-owner-rid" == (object) key)
            return this.SchemaOwnerRid;
          if ((object) "x-ms-cosmos-tx-commit" == (object) key)
            return this.TransactionCommit;
          if (string.Equals("x-ms-share-throughput", key, StringComparison.OrdinalIgnoreCase))
            return this.ShareThroughput;
          if (string.Equals("x-ms-schema-owner-rid", key, StringComparison.OrdinalIgnoreCase))
            return this.SchemaOwnerRid;
          if (string.Equals("x-ms-cosmos-tx-commit", key, StringComparison.OrdinalIgnoreCase))
            return this.TransactionCommit;
          break;
        case 22:
          if ((object) "x-ms-is-fanout-request" == (object) key)
            return this.IsFanoutRequest;
          if ((object) "x-ms-consistency-level" == (object) key)
            return this.ConsistencyLevel;
          if ((object) "x-ms-gateway-signature" == (object) key)
            return this.GatewaySignature;
          if (string.Equals("x-ms-is-fanout-request", key, StringComparison.OrdinalIgnoreCase))
            return this.IsFanoutRequest;
          if (string.Equals("x-ms-consistency-level", key, StringComparison.OrdinalIgnoreCase))
            return this.ConsistencyLevel;
          if (string.Equals("x-ms-gateway-signature", key, StringComparison.OrdinalIgnoreCase))
            return this.GatewaySignature;
          break;
        case 23:
          if ((object) "x-ms-indexing-directive" == (object) key)
            return this.IndexingDirective;
          if ((object) "x-ms-primary-master-key" == (object) key)
            return this.PrimaryMasterKey;
          if ((object) "x-ms-is-readonly-script" == (object) key)
            return this.IsReadOnlyScript;
          if (string.Equals("x-ms-indexing-directive", key, StringComparison.OrdinalIgnoreCase))
            return this.IndexingDirective;
          if (string.Equals("x-ms-primary-master-key", key, StringComparison.OrdinalIgnoreCase))
            return this.PrimaryMasterKey;
          if (string.Equals("x-ms-is-readonly-script", key, StringComparison.OrdinalIgnoreCase))
            return this.IsReadOnlyScript;
          break;
        case 24:
          if ((object) "collection-service-index" == (object) key)
            return this.CollectionServiceIndex;
          if ((object) "x-ms-remote-storage-type" == (object) key)
            return this.RemoteStorageType;
          if ((object) "x-ms-cosmos-batch-atomic" == (object) key)
            return this.IsBatchAtomic;
          if (string.Equals("collection-service-index", key, StringComparison.OrdinalIgnoreCase))
            return this.CollectionServiceIndex;
          if (string.Equals("x-ms-remote-storage-type", key, StringComparison.OrdinalIgnoreCase))
            return this.RemoteStorageType;
          if (string.Equals("x-ms-cosmos-batch-atomic", key, StringComparison.OrdinalIgnoreCase))
            return this.IsBatchAtomic;
          break;
        case 25:
          if ((object) "x-ms-resource-schema-name" == (object) key)
            return this.ResourceSchemaName;
          if ((object) "x-ms-secondary-master-key" == (object) key)
            return this.SecondaryMasterKey;
          if ((object) "x-ms-primary-readonly-key" == (object) key)
            return this.PrimaryReadonlyKey;
          if ((object) "x-ms-transport-request-id" == (object) key)
            return this.TransportRequestID;
          if ((object) "x-ms-cosmos-batch-ordered" == (object) key)
            return this.IsBatchOrdered;
          if ((object) "x-ms-cosmos-resourcetypes" == (object) key)
            return this.ResourceTypes;
          if (string.Equals("x-ms-resource-schema-name", key, StringComparison.OrdinalIgnoreCase))
            return this.ResourceSchemaName;
          if (string.Equals("x-ms-secondary-master-key", key, StringComparison.OrdinalIgnoreCase))
            return this.SecondaryMasterKey;
          if (string.Equals("x-ms-primary-readonly-key", key, StringComparison.OrdinalIgnoreCase))
            return this.PrimaryReadonlyKey;
          if (string.Equals("x-ms-transport-request-id", key, StringComparison.OrdinalIgnoreCase))
            return this.TransportRequestID;
          if (string.Equals("x-ms-cosmos-batch-ordered", key, StringComparison.OrdinalIgnoreCase))
            return this.IsBatchOrdered;
          if (string.Equals("x-ms-cosmos-resourcetypes", key, StringComparison.OrdinalIgnoreCase))
            return this.ResourceTypes;
          break;
        case 26:
          if ((object) "collection-partition-index" == (object) key)
            return this.CollectionPartitionIndex;
          if ((object) "x-ms-enumeration-direction" == (object) key)
            return this.EnumerationDirection;
          if ((object) "x-ms-cosmos-collectiontype" == (object) key)
            return this.RequestedCollectionType;
          if (string.Equals("collection-partition-index", key, StringComparison.OrdinalIgnoreCase))
            return this.CollectionPartitionIndex;
          if (string.Equals("x-ms-enumeration-direction", key, StringComparison.OrdinalIgnoreCase))
            return this.EnumerationDirection;
          if (string.Equals("x-ms-cosmos-collectiontype", key, StringComparison.OrdinalIgnoreCase))
            return this.RequestedCollectionType;
          break;
        case 27:
          if ((object) "x-ms-secondary-readonly-key" == (object) key)
            return this.SecondaryReadonlyKey;
          if ((object) "x-ms-fanout-operation-state" == (object) key)
            return this.FanoutOperationState;
          if ((object) "x-ms-cosmos-merge-static-id" == (object) key)
            return this.MergeStaticId;
          if (string.Equals("x-ms-secondary-readonly-key", key, StringComparison.OrdinalIgnoreCase))
            return this.SecondaryReadonlyKey;
          if (string.Equals("x-ms-fanout-operation-state", key, StringComparison.OrdinalIgnoreCase))
            return this.FanoutOperationState;
          if (string.Equals("x-ms-cosmos-merge-static-id", key, StringComparison.OrdinalIgnoreCase))
            return this.MergeStaticId;
          break;
        case 28:
          if ((object) "x-ms-documentdb-partitionkey" == (object) key)
            return this.PartitionKey;
          if ((object) "x-ms-restore-metadata-filter" == (object) key)
            return this.RestoreMetadataFilter;
          if ((object) "x-ms-time-to-live-in-seconds" == (object) key)
            return this.TimeToLiveInSeconds;
          if ((object) "x-ms-effective-partition-key" == (object) key)
            return this.EffectivePartitionKey;
          if ((object) "x-ms-cosmos-use-systembudget" == (object) key)
            return this.UseSystemBudget;
          if (string.Equals("x-ms-documentdb-partitionkey", key, StringComparison.OrdinalIgnoreCase))
            return this.PartitionKey;
          if (string.Equals("x-ms-restore-metadata-filter", key, StringComparison.OrdinalIgnoreCase))
            return this.RestoreMetadataFilter;
          if (string.Equals("x-ms-time-to-live-in-seconds", key, StringComparison.OrdinalIgnoreCase))
            return this.TimeToLiveInSeconds;
          if (string.Equals("x-ms-effective-partition-key", key, StringComparison.OrdinalIgnoreCase))
            return this.EffectivePartitionKey;
          if (string.Equals("x-ms-cosmos-use-systembudget", key, StringComparison.OrdinalIgnoreCase))
            return this.UseSystemBudget;
          break;
        case 30:
          if ((object) "x-ms-documentdb-expiry-seconds" == (object) key)
            return this.ResourceTokenExpiry;
          if ((object) "x-ms-documentdb-partitioncount" == (object) key)
            return this.PartitionCount;
          if ((object) "x-ms-documentdb-collection-rid" == (object) key)
            return this.CollectionRid;
          if ((object) "x-ms-partition-resource-filter" == (object) key)
            return this.PartitionResourceFilter;
          if ((object) "x-ms-exclude-system-properties" == (object) key)
            return this.ExcludeSystemProperties;
          if (string.Equals("x-ms-documentdb-expiry-seconds", key, StringComparison.OrdinalIgnoreCase))
            return this.ResourceTokenExpiry;
          if (string.Equals("x-ms-documentdb-partitioncount", key, StringComparison.OrdinalIgnoreCase))
            return this.PartitionCount;
          if (string.Equals("x-ms-documentdb-collection-rid", key, StringComparison.OrdinalIgnoreCase))
            return this.CollectionRid;
          if (string.Equals("x-ms-partition-resource-filter", key, StringComparison.OrdinalIgnoreCase))
            return this.PartitionResourceFilter;
          if (string.Equals("x-ms-exclude-system-properties", key, StringComparison.OrdinalIgnoreCase))
            return this.ExcludeSystemProperties;
          break;
        case 31:
          if ((object) "x-ms-client-retry-attempt-count" == (object) key)
            return this.ClientRetryAttemptCount;
          if ((object) "x-ms-can-offer-replace-complete" == (object) key)
            return this.CanOfferReplaceComplete;
          if ((object) "x-ms-binary-passthrough-request" == (object) key)
            return this.BinaryPassthroughRequest;
          if ((object) "x-ms-cosmos-is-client-encrypted" == (object) key)
            return this.IsClientEncrypted;
          if ((object) "x-ms-cosmos-systemdocument-type" == (object) key)
            return this.SystemDocumentType;
          if ((object) "x-ms-cosmos-collection-truncate" == (object) key)
            return this.CollectionTruncate;
          if (string.Equals("x-ms-client-retry-attempt-count", key, StringComparison.OrdinalIgnoreCase))
            return this.ClientRetryAttemptCount;
          if (string.Equals("x-ms-can-offer-replace-complete", key, StringComparison.OrdinalIgnoreCase))
            return this.CanOfferReplaceComplete;
          if (string.Equals("x-ms-binary-passthrough-request", key, StringComparison.OrdinalIgnoreCase))
            return this.BinaryPassthroughRequest;
          if (string.Equals("x-ms-cosmos-is-client-encrypted", key, StringComparison.OrdinalIgnoreCase))
            return this.IsClientEncrypted;
          if (string.Equals("x-ms-cosmos-systemdocument-type", key, StringComparison.OrdinalIgnoreCase))
            return this.SystemDocumentType;
          if (string.Equals("x-ms-cosmos-collection-truncate", key, StringComparison.OrdinalIgnoreCase))
            return this.CollectionTruncate;
          break;
        case 32:
          if ((object) "x-ms-migratecollection-directive" == (object) key)
            return this.MigrateCollectionDirective;
          if ((object) "x-ms-target-global-committed-lsn" == (object) key)
            return this.TargetGlobalCommittedLsn;
          if ((object) "x-ms-documentdb-force-query-scan" == (object) key)
            return this.ForceQueryScan;
          if ((object) "x-ms-cosmos-max-polling-interval" == (object) key)
            return this.MaxPollingIntervalMilliseconds;
          if ((object) "x-ms-cosmos-populateindexmetrics" == (object) key)
            return this.PopulateIndexMetrics;
          if (string.Equals("x-ms-migratecollection-directive", key, StringComparison.OrdinalIgnoreCase))
            return this.MigrateCollectionDirective;
          if (string.Equals("x-ms-target-global-committed-lsn", key, StringComparison.OrdinalIgnoreCase))
            return this.TargetGlobalCommittedLsn;
          if (string.Equals("x-ms-documentdb-force-query-scan", key, StringComparison.OrdinalIgnoreCase))
            return this.ForceQueryScan;
          if (string.Equals("x-ms-cosmos-max-polling-interval", key, StringComparison.OrdinalIgnoreCase))
            return this.MaxPollingIntervalMilliseconds;
          if (string.Equals("x-ms-cosmos-populateindexmetrics", key, StringComparison.OrdinalIgnoreCase))
            return this.PopulateIndexMetrics;
          break;
        case 33:
          if ((object) "x-ms-documentdb-query-enable-scan" == (object) key)
            return this.EnableScanInQuery;
          if ((object) "x-ms-documentdb-query-emit-traces" == (object) key)
            return this.EmitVerboseTracesInQuery;
          if ((object) "x-ms-documentdb-populatequotainfo" == (object) key)
            return this.PopulateQuotaInfo;
          if ((object) "x-ms-cosmos-preserve-full-content" == (object) key)
            return this.PreserveFullContent;
          if ((object) "x-ms-cosmos-populate-logstoreinfo" == (object) key)
            return this.PopulateLogStoreInfo;
          if ((object) "x-ms-cosmos-correlated-activityid" == (object) key)
            return this.CorrelatedActivityId;
          if (string.Equals("x-ms-documentdb-query-enable-scan", key, StringComparison.OrdinalIgnoreCase))
            return this.EnableScanInQuery;
          if (string.Equals("x-ms-documentdb-query-emit-traces", key, StringComparison.OrdinalIgnoreCase))
            return this.EmitVerboseTracesInQuery;
          if (string.Equals("x-ms-documentdb-populatequotainfo", key, StringComparison.OrdinalIgnoreCase))
            return this.PopulateQuotaInfo;
          if (string.Equals("x-ms-cosmos-preserve-full-content", key, StringComparison.OrdinalIgnoreCase))
            return this.PreserveFullContent;
          if (string.Equals("x-ms-cosmos-populate-logstoreinfo", key, StringComparison.OrdinalIgnoreCase))
            return this.PopulateLogStoreInfo;
          if (string.Equals("x-ms-cosmos-correlated-activityid", key, StringComparison.OrdinalIgnoreCase))
            return this.CorrelatedActivityId;
          break;
        case 34:
          if ((object) "x-ms-cosmos-allow-tentative-writes" == (object) key)
            return this.AllowTentativeWrites;
          if ((object) "x-ms-cosmos-use-archival-partition" == (object) key)
            return this.UseArchivalPartition;
          if (string.Equals("x-ms-cosmos-allow-tentative-writes", key, StringComparison.OrdinalIgnoreCase))
            return this.AllowTentativeWrites;
          if (string.Equals("x-ms-cosmos-use-archival-partition", key, StringComparison.OrdinalIgnoreCase))
            return this.UseArchivalPartition;
          break;
        case 35:
          if ((object) "x-ms-documentdb-pre-trigger-include" == (object) key)
            return this.PreTriggerInclude;
          if ((object) "x-ms-documentdb-pre-trigger-exclude" == (object) key)
            return this.PreTriggerExclude;
          if ((object) "x-ms-documentdb-partitionkeyrangeid" == (object) key)
            return this.PartitionKeyRangeId;
          if ((object) "x-ms-documentdb-filterby-schema-rid" == (object) key)
            return this.FilterBySchemaResourceId;
          if ((object) "x-ms-remaining-time-in-ms-on-client" == (object) key)
            return this.RemainingTimeInMsOnClientRequest;
          if ((object) "x-ms-collection-security-identifier" == (object) key)
            return this.CollectionRemoteStorageSecurityIdentifier;
          if ((object) "x-ms-cosmos-batch-continue-on-error" == (object) key)
            return this.ShouldBatchContinueOnError;
          if ((object) "x-ms-cosmos-intended-collection-rid" == (object) key)
            return this.IntendedCollectionRid;
          if (string.Equals("x-ms-documentdb-pre-trigger-include", key, StringComparison.OrdinalIgnoreCase))
            return this.PreTriggerInclude;
          if (string.Equals("x-ms-documentdb-pre-trigger-exclude", key, StringComparison.OrdinalIgnoreCase))
            return this.PreTriggerExclude;
          if (string.Equals("x-ms-documentdb-partitionkeyrangeid", key, StringComparison.OrdinalIgnoreCase))
            return this.PartitionKeyRangeId;
          if (string.Equals("x-ms-documentdb-filterby-schema-rid", key, StringComparison.OrdinalIgnoreCase))
            return this.FilterBySchemaResourceId;
          if (string.Equals("x-ms-remaining-time-in-ms-on-client", key, StringComparison.OrdinalIgnoreCase))
            return this.RemainingTimeInMsOnClientRequest;
          if (string.Equals("x-ms-collection-security-identifier", key, StringComparison.OrdinalIgnoreCase))
            return this.CollectionRemoteStorageSecurityIdentifier;
          if (string.Equals("x-ms-cosmos-batch-continue-on-error", key, StringComparison.OrdinalIgnoreCase))
            return this.ShouldBatchContinueOnError;
          if (string.Equals("x-ms-cosmos-intended-collection-rid", key, StringComparison.OrdinalIgnoreCase))
            return this.IntendedCollectionRid;
          break;
        case 36:
          if ((object) "x-ms-documentdb-post-trigger-include" == (object) key)
            return this.PostTriggerInclude;
          if ((object) "x-ms-documentdb-post-trigger-exclude" == (object) key)
            return this.PostTriggerExclude;
          if ((object) "x-ms-documentdb-populatequerymetrics" == (object) key)
            return this.PopulateQueryMetrics;
          if ((object) "x-ms-cosmos-internal-is-user-request" == (object) key)
            return this.IsUserRequest;
          if ((object) "x-ms-cosmos-include-tentative-writes" == (object) key)
            return this.IncludeTentativeWrites;
          if ((object) "x-ms-cosmos-is-retried-write-request" == (object) key)
            return this.IsRetriedWriteRequest;
          if (string.Equals("x-ms-documentdb-post-trigger-include", key, StringComparison.OrdinalIgnoreCase))
            return this.PostTriggerInclude;
          if (string.Equals("x-ms-documentdb-post-trigger-exclude", key, StringComparison.OrdinalIgnoreCase))
            return this.PostTriggerExclude;
          if (string.Equals("x-ms-documentdb-populatequerymetrics", key, StringComparison.OrdinalIgnoreCase))
            return this.PopulateQueryMetrics;
          if (string.Equals("x-ms-cosmos-internal-is-user-request", key, StringComparison.OrdinalIgnoreCase))
            return this.IsUserRequest;
          if (string.Equals("x-ms-cosmos-include-tentative-writes", key, StringComparison.OrdinalIgnoreCase))
            return this.IncludeTentativeWrites;
          if (string.Equals("x-ms-cosmos-is-retried-write-request", key, StringComparison.OrdinalIgnoreCase))
            return this.IsRetriedWriteRequest;
          break;
        case 37:
          if ((object) "x-ms-documentdb-script-enable-logging" == (object) key)
            return this.EnableLogging;
          if ((object) "x-ms-documentdb-populateresourcecount" == (object) key)
            return this.PopulateResourceCount;
          if ((object) "x-ms-cosmos-sdk-supportedcapabilities" == (object) key)
            return this.SDKSupportedCapabilities;
          if ((object) "x-ms-cosmos-builder-client-identifier" == (object) key)
            return this.BuilderClientIdentifier;
          if (string.Equals("x-ms-documentdb-script-enable-logging", key, StringComparison.OrdinalIgnoreCase))
            return this.EnableLogging;
          if (string.Equals("x-ms-documentdb-populateresourcecount", key, StringComparison.OrdinalIgnoreCase))
            return this.PopulateResourceCount;
          if (string.Equals("x-ms-cosmos-sdk-supportedcapabilities", key, StringComparison.OrdinalIgnoreCase))
            return this.SDKSupportedCapabilities;
          if (string.Equals("x-ms-cosmos-builder-client-identifier", key, StringComparison.OrdinalIgnoreCase))
            return this.BuilderClientIdentifier;
          break;
        case 38:
          if ((object) "x-ms-cosmos-migrate-offer-to-autopilot" == (object) key)
            return this.MigrateOfferToAutopilot;
          if ((object) "x-ms-cosmos-retriable-write-request-id" == (object) key)
            return this.RetriableWriteRequestId;
          if ((object) "x-ms-cosmos-source-collection-if-match" == (object) key)
            return this.SourceCollectionIfMatch;
          if ((object) "x-ms-cosmos-use-background-task-budget" == (object) key)
            return this.UseUserBackgroundBudget;
          if (string.Equals("x-ms-cosmos-migrate-offer-to-autopilot", key, StringComparison.OrdinalIgnoreCase))
            return this.MigrateOfferToAutopilot;
          if (string.Equals("x-ms-cosmos-retriable-write-request-id", key, StringComparison.OrdinalIgnoreCase))
            return this.RetriableWriteRequestId;
          if (string.Equals("x-ms-cosmos-source-collection-if-match", key, StringComparison.OrdinalIgnoreCase))
            return this.SourceCollectionIfMatch;
          if (string.Equals("x-ms-cosmos-use-background-task-budget", key, StringComparison.OrdinalIgnoreCase))
            return this.UseUserBackgroundBudget;
          break;
        case 39:
          if ((object) "x-ms-cosmos-internal-truncate-merge-log" == (object) key)
            return this.TruncateMergeLogRequest;
          if ((object) "x-ms-cosmos-internal-serverless-request" == (object) key)
            return this.IsInternalServerlessRequest;
          if (string.Equals("x-ms-cosmos-internal-truncate-merge-log", key, StringComparison.OrdinalIgnoreCase))
            return this.TruncateMergeLogRequest;
          if (string.Equals("x-ms-cosmos-internal-serverless-request", key, StringComparison.OrdinalIgnoreCase))
            return this.IsInternalServerlessRequest;
          break;
        case 40:
          if ((object) "x-ms-enable-dynamic-rid-range-allocation" == (object) key)
            return this.EnableDynamicRidRangeAllocation;
          if ((object) "x-ms-cosmos-uniqueindex-reindexing-state" == (object) key)
            return this.UniqueIndexReIndexingState;
          if (string.Equals("x-ms-enable-dynamic-rid-range-allocation", key, StringComparison.OrdinalIgnoreCase))
            return this.EnableDynamicRidRangeAllocation;
          if (string.Equals("x-ms-cosmos-uniqueindex-reindexing-state", key, StringComparison.OrdinalIgnoreCase))
            return this.UniqueIndexReIndexingState;
          break;
        case 41:
          if (string.Equals("x-ms-cosmos-populate-oldest-active-schema", key, StringComparison.OrdinalIgnoreCase))
            return this.PopulateOldestActiveSchema;
          break;
        case 42:
          if ((object) "x-ms-cosmos-internal-merge-checkpoint-glsn" == (object) key)
            return this.MergeCheckPointGLSN;
          if ((object) "x-ms-should-return-current-server-datetime" == (object) key)
            return this.ShouldReturnCurrentServerDateTime;
          if ((object) "x-ms-cosmos-changefeed-wire-format-version" == (object) key)
            return this.ChangeFeedWireFormatVersion;
          if (string.Equals("x-ms-cosmos-internal-merge-checkpoint-glsn", key, StringComparison.OrdinalIgnoreCase))
            return this.MergeCheckPointGLSN;
          if (string.Equals("x-ms-should-return-current-server-datetime", key, StringComparison.OrdinalIgnoreCase))
            return this.ShouldReturnCurrentServerDateTime;
          if (string.Equals("x-ms-cosmos-changefeed-wire-format-version", key, StringComparison.OrdinalIgnoreCase))
            return this.ChangeFeedWireFormatVersion;
          break;
        case 43:
          if ((object) "x-ms-documentdb-disable-ru-per-minute-usage" == (object) key)
            return this.DisableRUPerMinuteUsage;
          if ((object) "x-ms-documentdb-populatepartitionstatistics" == (object) key)
            return this.PopulatePartitionStatistics;
          if ((object) "x-ms-cosmos-force-sidebyside-indexmigration" == (object) key)
            return this.ForceSideBySideIndexMigration;
          if ((object) "x-ms-cosmos-unique-index-name-encoding-mode" == (object) key)
            return this.UniqueIndexNameEncodingMode;
          if (string.Equals("x-ms-documentdb-disable-ru-per-minute-usage", key, StringComparison.OrdinalIgnoreCase))
            return this.DisableRUPerMinuteUsage;
          if (string.Equals("x-ms-documentdb-populatepartitionstatistics", key, StringComparison.OrdinalIgnoreCase))
            return this.PopulatePartitionStatistics;
          if (string.Equals("x-ms-cosmos-force-sidebyside-indexmigration", key, StringComparison.OrdinalIgnoreCase))
            return this.ForceSideBySideIndexMigration;
          if (string.Equals("x-ms-cosmos-unique-index-name-encoding-mode", key, StringComparison.OrdinalIgnoreCase))
            return this.UniqueIndexNameEncodingMode;
          break;
        case 44:
          if (string.Equals("x-ms-documentdb-content-serialization-format", key, StringComparison.OrdinalIgnoreCase))
            return this.ContentSerializationFormat;
          break;
        case 45:
          if ((object) "x-ms-cosmos-start-full-fidelity-if-none-match" == (object) key)
            return this.ChangeFeedStartFullFidelityIfNoneMatch;
          if ((object) "x-ms-cosmos-internal-system-restore-operation" == (object) key)
            return this.SystemRestoreOperation;
          if ((object) "x-ms-cosmos-internal-is-throughputcap-request" == (object) key)
            return this.IsThroughputCapRequest;
          if ((object) "x-ms-cosmos-populate-byok-encryption-progress" == (object) key)
            return this.PopulateByokEncryptionProgress;
          if (string.Equals("x-ms-cosmos-start-full-fidelity-if-none-match", key, StringComparison.OrdinalIgnoreCase))
            return this.ChangeFeedStartFullFidelityIfNoneMatch;
          if (string.Equals("x-ms-cosmos-internal-system-restore-operation", key, StringComparison.OrdinalIgnoreCase))
            return this.SystemRestoreOperation;
          if (string.Equals("x-ms-cosmos-internal-is-throughputcap-request", key, StringComparison.OrdinalIgnoreCase))
            return this.IsThroughputCapRequest;
          if (string.Equals("x-ms-cosmos-populate-byok-encryption-progress", key, StringComparison.OrdinalIgnoreCase))
            return this.PopulateByokEncryptionProgress;
          break;
        case 46:
          if ((object) "x-ms-cosmos-migrate-offer-to-manual-throughput" == (object) key)
            return this.MigrateOfferToManualThroughput;
          if ((object) "x-ms-cosmos-skip-refresh-databaseaccountconfig" == (object) key)
            return this.SkipRefreshDatabaseAccountConfigs;
          if (string.Equals("x-ms-cosmos-migrate-offer-to-manual-throughput", key, StringComparison.OrdinalIgnoreCase))
            return this.MigrateOfferToManualThroughput;
          if (string.Equals("x-ms-cosmos-skip-refresh-databaseaccountconfig", key, StringComparison.OrdinalIgnoreCase))
            return this.SkipRefreshDatabaseAccountConfigs;
          break;
        case 47:
          if ((object) "x-ms-documentdb-supportspatiallegacycoordinates" == (object) key)
            return this.SupportSpatialLegacyCoordinates;
          if ((object) "x-ms-cosmos-collection-child-resourcename-limit" == (object) key)
            return this.CollectionChildResourceNameLimitInBytes;
          if ((object) "x-ms-cosmos-add-resource-properties-to-response" == (object) key)
            return this.AddResourcePropertiesToResponse;
          if ((object) "x-ms-cosmos-internal-is-materialized-view-build" == (object) key)
            return this.IsMaterializedViewBuild;
          if (string.Equals("x-ms-documentdb-supportspatiallegacycoordinates", key, StringComparison.OrdinalIgnoreCase))
            return this.SupportSpatialLegacyCoordinates;
          if (string.Equals("x-ms-cosmos-collection-child-resourcename-limit", key, StringComparison.OrdinalIgnoreCase))
            return this.CollectionChildResourceNameLimitInBytes;
          if (string.Equals("x-ms-cosmos-add-resource-properties-to-response", key, StringComparison.OrdinalIgnoreCase))
            return this.AddResourcePropertiesToResponse;
          if (string.Equals("x-ms-cosmos-internal-is-materialized-view-build", key, StringComparison.OrdinalIgnoreCase))
            return this.IsMaterializedViewBuild;
          break;
        case 48:
          if ((object) "x-ms-documentdb-populatecollectionthroughputinfo" == (object) key)
            return this.PopulateCollectionThroughputInfo;
          if ((object) "x-ms-cosmos-internal-get-all-partition-key-stats" == (object) key)
            return this.GetAllPartitionKeyStatistics;
          if ((object) "x-ms-cosmosdb-populateuniqueindexreindexprogress" == (object) key)
            return this.PopulateUniqueIndexReIndexProgress;
          if (string.Equals("x-ms-documentdb-populatecollectionthroughputinfo", key, StringComparison.OrdinalIgnoreCase))
            return this.PopulateCollectionThroughputInfo;
          if (string.Equals("x-ms-cosmos-internal-get-all-partition-key-stats", key, StringComparison.OrdinalIgnoreCase))
            return this.GetAllPartitionKeyStatistics;
          if (string.Equals("x-ms-cosmosdb-populateuniqueindexreindexprogress", key, StringComparison.OrdinalIgnoreCase))
            return this.PopulateUniqueIndexReIndexProgress;
          break;
        case 49:
          if (string.Equals("x-ms-documentdb-usepolygonssmallerthanahemisphere", key, StringComparison.OrdinalIgnoreCase))
            return this.UsePolygonsSmallerThanAHemisphere;
          break;
        case 50:
          if ((object) "x-ms-documentdb-responsecontinuationtokenlimitinkb" == (object) key)
            return this.ResponseContinuationTokenLimitInKB;
          if ((object) "x-ms-cosmos-populate-analytical-migration-progress" == (object) key)
            return this.PopulateAnalyticalMigrationProgress;
          if ((object) "x-ms-cosmos-internal-update-offer-state-to-pending" == (object) key)
            return this.UpdateOfferStateToPending;
          if (string.Equals("x-ms-documentdb-responsecontinuationtokenlimitinkb", key, StringComparison.OrdinalIgnoreCase))
            return this.ResponseContinuationTokenLimitInKB;
          if (string.Equals("x-ms-cosmos-populate-analytical-migration-progress", key, StringComparison.OrdinalIgnoreCase))
            return this.PopulateAnalyticalMigrationProgress;
          if (string.Equals("x-ms-cosmos-internal-update-offer-state-to-pending", key, StringComparison.OrdinalIgnoreCase))
            return this.UpdateOfferStateToPending;
          break;
        case 51:
          if ((object) "x-ms-documentdb-query-enable-low-precision-order-by" == (object) key)
            return this.EnableLowPrecisionOrderBy;
          if ((object) "x-ms-cosmos-retriable-write-request-start-timestamp" == (object) key)
            return this.RetriableWriteRequestStartTimestamp;
          if (string.Equals("x-ms-documentdb-query-enable-low-precision-order-by", key, StringComparison.OrdinalIgnoreCase))
            return this.EnableLowPrecisionOrderBy;
          if (string.Equals("x-ms-cosmos-retriable-write-request-start-timestamp", key, StringComparison.OrdinalIgnoreCase))
            return this.RetriableWriteRequestStartTimestamp;
          break;
        case 52:
          if (string.Equals("x-ms-cosmos-internal-offer-replace-ru-redistribution", key, StringComparison.OrdinalIgnoreCase))
            return this.OfferReplaceRURedistribution;
          break;
        case 53:
          if ((object) "x-ms-cosmos-internal-is-ru-per-gb-enforcement-request" == (object) key)
            return this.IsRUPerGBEnforcementRequest;
          if ((object) "x-ms-cosmos-internal-is-offer-storage-refresh-request" == (object) key)
            return this.IsOfferStorageRefreshRequest;
          if (string.Equals("x-ms-cosmos-internal-is-ru-per-gb-enforcement-request", key, StringComparison.OrdinalIgnoreCase))
            return this.IsRUPerGBEnforcementRequest;
          if (string.Equals("x-ms-cosmos-internal-is-offer-storage-refresh-request", key, StringComparison.OrdinalIgnoreCase))
            return this.IsOfferStorageRefreshRequest;
          break;
        case 54:
          if (string.Equals("x-ms-cosmos-include-physical-partition-throughput-info", key, StringComparison.OrdinalIgnoreCase))
            return this.IncludePhysicalPartitionThroughputInfo;
          break;
        case 56:
          if (string.Equals("x-ms-cosmos-collection-child-contentlength-resourcelimit", key, StringComparison.OrdinalIgnoreCase))
            return this.CollectionChildResourceContentLimitInKB;
          break;
        case 57:
          if (string.Equals("x-ms-cosmos-internal-populate-unflushed-merge-entry-count", key, StringComparison.OrdinalIgnoreCase))
            return this.PopulateUnflushedMergeEntryCount;
          break;
        case 58:
          if (string.Equals("x-ms-cosmos-internal-ignore-system-lowering-max-throughput", key, StringComparison.OrdinalIgnoreCase))
            return this.IgnoreSystemLoweringMaxThroughput;
          break;
        case 59:
          if (string.Equals("x-ms-cosmos-internal-update-max-throughput-ever-provisioned", key, StringComparison.OrdinalIgnoreCase))
            return this.UpdateMaxThroughputEverProvisioned;
          break;
        case 61:
          if (string.Equals("x-ms-cosmos-internal-serverless-offer-storage-refresh-request", key, StringComparison.OrdinalIgnoreCase))
            return this.IsServerlessStorageRefreshRequest;
          break;
      }
      string str;
      return this.lazyNotCommonHeaders.IsValueCreated && this.lazyNotCommonHeaders.Value.TryGetValue(key, out str) ? str : (string) null;
    }

    public void Add(string key, string value)
    {
      if (key == null)
        throw new ArgumentNullException(nameof (key));
      if (value == null)
        throw new ArgumentNullException(nameof (value));
      this.UpdateHelper(key, value, true);
    }

    public void Set(string key, string value)
    {
      if (key == null)
        throw new ArgumentNullException(nameof (key));
      this.UpdateHelper(key, value, false);
    }

    public void UpdateHelper(string key, string value, bool throwIfAlreadyExists)
    {
      if (key == null)
        throw new ArgumentNullException(nameof (key));
      switch (key.Length)
      {
        case 4:
          if ((object) "date" == (object) key)
          {
            this.HttpDate = !throwIfAlreadyExists || this.HttpDate == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if ((object) "A-IM" == (object) key)
          {
            this.A_IM = !throwIfAlreadyExists || this.A_IM == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("date", key, StringComparison.OrdinalIgnoreCase))
          {
            this.HttpDate = !throwIfAlreadyExists || this.HttpDate == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("A-IM", key, StringComparison.OrdinalIgnoreCase))
          {
            this.A_IM = !throwIfAlreadyExists || this.A_IM == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          break;
        case 6:
          if (string.Equals("Prefer", key, StringComparison.OrdinalIgnoreCase))
          {
            this.Prefer = !throwIfAlreadyExists || this.Prefer == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          break;
        case 8:
          if (string.Equals("If-Match", key, StringComparison.OrdinalIgnoreCase))
          {
            this.IfMatch = !throwIfAlreadyExists || this.IfMatch == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          break;
        case 9:
          if (string.Equals("x-ms-date", key, StringComparison.OrdinalIgnoreCase))
          {
            this.XDate = !throwIfAlreadyExists || this.XDate == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          break;
        case 11:
          if (string.Equals("x-ms-end-id", key, StringComparison.OrdinalIgnoreCase))
          {
            this.EndId = !throwIfAlreadyExists || this.EndId == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          break;
        case 12:
          if ((object) "x-ms-version" == (object) key)
          {
            this.Version = !throwIfAlreadyExists || this.Version == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if ((object) "x-ms-end-epk" == (object) key)
          {
            this.EndEpk = !throwIfAlreadyExists || this.EndEpk == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-version", key, StringComparison.OrdinalIgnoreCase))
          {
            this.Version = !throwIfAlreadyExists || this.Version == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-end-epk", key, StringComparison.OrdinalIgnoreCase))
          {
            this.EndEpk = !throwIfAlreadyExists || this.EndEpk == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          break;
        case 13:
          if ((object) "authorization" == (object) key)
          {
            this.Authorization = !throwIfAlreadyExists || this.Authorization == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if ((object) "If-None-Match" == (object) key)
          {
            this.IfNoneMatch = !throwIfAlreadyExists || this.IfNoneMatch == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if ((object) "x-ms-start-id" == (object) key)
          {
            this.StartId = !throwIfAlreadyExists || this.StartId == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("authorization", key, StringComparison.OrdinalIgnoreCase))
          {
            this.Authorization = !throwIfAlreadyExists || this.Authorization == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("If-None-Match", key, StringComparison.OrdinalIgnoreCase))
          {
            this.IfNoneMatch = !throwIfAlreadyExists || this.IfNoneMatch == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-start-id", key, StringComparison.OrdinalIgnoreCase))
          {
            this.StartId = !throwIfAlreadyExists || this.StartId == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          break;
        case 14:
          if ((object) "x-ms-cancharge" == (object) key)
          {
            this.CanCharge = !throwIfAlreadyExists || this.CanCharge == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if ((object) "x-ms-binary-id" == (object) key)
          {
            this.BinaryId = !throwIfAlreadyExists || this.BinaryId == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if ((object) "x-ms-start-epk" == (object) key)
          {
            this.StartEpk = !throwIfAlreadyExists || this.StartEpk == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if ((object) "x-ms-schema-id" == (object) key)
          {
            this.SchemaId = !throwIfAlreadyExists || this.SchemaId == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-cancharge", key, StringComparison.OrdinalIgnoreCase))
          {
            this.CanCharge = !throwIfAlreadyExists || this.CanCharge == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-binary-id", key, StringComparison.OrdinalIgnoreCase))
          {
            this.BinaryId = !throwIfAlreadyExists || this.BinaryId == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-start-epk", key, StringComparison.OrdinalIgnoreCase))
          {
            this.StartEpk = !throwIfAlreadyExists || this.StartEpk == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-schema-id", key, StringComparison.OrdinalIgnoreCase))
          {
            this.SchemaId = !throwIfAlreadyExists || this.SchemaId == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          break;
        case 15:
          if (string.Equals("x-ms-target-lsn", key, StringComparison.OrdinalIgnoreCase))
          {
            this.TargetLsn = !throwIfAlreadyExists || this.TargetLsn == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          break;
        case 16:
          if ((object) "x-ms-canthrottle" == (object) key)
          {
            this.CanThrottle = !throwIfAlreadyExists || this.CanThrottle == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if ((object) "x-ms-schema-hash" == (object) key)
          {
            this.SchemaHash = !throwIfAlreadyExists || this.SchemaHash == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if ((object) "x-ms-rbac-action" == (object) key)
          {
            this.RbacAction = !throwIfAlreadyExists || this.RbacAction == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-canthrottle", key, StringComparison.OrdinalIgnoreCase))
          {
            this.CanThrottle = !throwIfAlreadyExists || this.CanThrottle == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-schema-hash", key, StringComparison.OrdinalIgnoreCase))
          {
            this.SchemaHash = !throwIfAlreadyExists || this.SchemaHash == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-rbac-action", key, StringComparison.OrdinalIgnoreCase))
          {
            this.RbacAction = !throwIfAlreadyExists || this.RbacAction == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          break;
        case 17:
          if ((object) "x-ms-continuation" == (object) key)
          {
            this.Continuation = !throwIfAlreadyExists || this.Continuation == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if ((object) "x-docdb-entity-id" == (object) key)
          {
            this.EntityId = !throwIfAlreadyExists || this.EntityId == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if ((object) "x-ms-bind-replica" == (object) key)
          {
            this.BindReplicaDirective = !throwIfAlreadyExists || this.BindReplicaDirective == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if ((object) "If-Modified-Since" == (object) key)
          {
            this.IfModifiedSince = !throwIfAlreadyExists || this.IfModifiedSince == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if ((object) "x-ms-cosmos-tx-id" == (object) key)
          {
            this.TransactionId = !throwIfAlreadyExists || this.TransactionId == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if ((object) "x-ms-rbac-user-id" == (object) key)
          {
            this.RbacUserId = !throwIfAlreadyExists || this.RbacUserId == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-continuation", key, StringComparison.OrdinalIgnoreCase))
          {
            this.Continuation = !throwIfAlreadyExists || this.Continuation == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-docdb-entity-id", key, StringComparison.OrdinalIgnoreCase))
          {
            this.EntityId = !throwIfAlreadyExists || this.EntityId == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-bind-replica", key, StringComparison.OrdinalIgnoreCase))
          {
            this.BindReplicaDirective = !throwIfAlreadyExists || this.BindReplicaDirective == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("If-Modified-Since", key, StringComparison.OrdinalIgnoreCase))
          {
            this.IfModifiedSince = !throwIfAlreadyExists || this.IfModifiedSince == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-cosmos-tx-id", key, StringComparison.OrdinalIgnoreCase))
          {
            this.TransactionId = !throwIfAlreadyExists || this.TransactionId == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-rbac-user-id", key, StringComparison.OrdinalIgnoreCase))
          {
            this.RbacUserId = !throwIfAlreadyExists || this.RbacUserId == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          break;
        case 18:
          if ((object) "x-ms-session-token" == (object) key)
          {
            this.SessionToken = !throwIfAlreadyExists || this.SessionToken == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if ((object) "x-ms-is-auto-scale" == (object) key)
          {
            this.IsAutoScaleRequest = !throwIfAlreadyExists || this.IsAutoScaleRequest == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if ((object) "x-ms-read-key-type" == (object) key)
          {
            this.ReadFeedKeyType = !throwIfAlreadyExists || this.ReadFeedKeyType == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if ((object) "x-ms-rbac-resource" == (object) key)
          {
            this.RbacResource = !throwIfAlreadyExists || this.RbacResource == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-session-token", key, StringComparison.OrdinalIgnoreCase))
          {
            this.SessionToken = !throwIfAlreadyExists || this.SessionToken == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-is-auto-scale", key, StringComparison.OrdinalIgnoreCase))
          {
            this.IsAutoScaleRequest = !throwIfAlreadyExists || this.IsAutoScaleRequest == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-read-key-type", key, StringComparison.OrdinalIgnoreCase))
          {
            this.ReadFeedKeyType = !throwIfAlreadyExists || this.ReadFeedKeyType == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-rbac-resource", key, StringComparison.OrdinalIgnoreCase))
          {
            this.RbacResource = !throwIfAlreadyExists || this.RbacResource == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          break;
        case 19:
          if ((object) "x-docdb-resource-id" == (object) key)
          {
            this.ResourceId = !throwIfAlreadyExists || this.ResourceId == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if ((object) "x-ms-max-item-count" == (object) key)
          {
            this.PageSize = !throwIfAlreadyExists || this.PageSize == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if ((object) "x-ms-restore-params" == (object) key)
          {
            this.RestoreParams = !throwIfAlreadyExists || this.RestoreParams == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if ((object) "x-ms-cosmos-tx-init" == (object) key)
          {
            this.TransactionFirstRequest = !throwIfAlreadyExists || this.TransactionFirstRequest == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-docdb-resource-id", key, StringComparison.OrdinalIgnoreCase))
          {
            this.ResourceId = !throwIfAlreadyExists || this.ResourceId == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-max-item-count", key, StringComparison.OrdinalIgnoreCase))
          {
            this.PageSize = !throwIfAlreadyExists || this.PageSize == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-restore-params", key, StringComparison.OrdinalIgnoreCase))
          {
            this.RestoreParams = !throwIfAlreadyExists || this.RestoreParams == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-cosmos-tx-init", key, StringComparison.OrdinalIgnoreCase))
          {
            this.TransactionFirstRequest = !throwIfAlreadyExists || this.TransactionFirstRequest == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          break;
        case 20:
          if (string.Equals("x-ms-profile-request", key, StringComparison.OrdinalIgnoreCase))
          {
            this.ProfileRequest = !throwIfAlreadyExists || this.ProfileRequest == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          break;
        case 21:
          if ((object) "x-ms-share-throughput" == (object) key)
          {
            this.ShareThroughput = !throwIfAlreadyExists || this.ShareThroughput == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if ((object) "x-ms-schema-owner-rid" == (object) key)
          {
            this.SchemaOwnerRid = !throwIfAlreadyExists || this.SchemaOwnerRid == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if ((object) "x-ms-cosmos-tx-commit" == (object) key)
          {
            this.TransactionCommit = !throwIfAlreadyExists || this.TransactionCommit == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-share-throughput", key, StringComparison.OrdinalIgnoreCase))
          {
            this.ShareThroughput = !throwIfAlreadyExists || this.ShareThroughput == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-schema-owner-rid", key, StringComparison.OrdinalIgnoreCase))
          {
            this.SchemaOwnerRid = !throwIfAlreadyExists || this.SchemaOwnerRid == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-cosmos-tx-commit", key, StringComparison.OrdinalIgnoreCase))
          {
            this.TransactionCommit = !throwIfAlreadyExists || this.TransactionCommit == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          break;
        case 22:
          if ((object) "x-ms-is-fanout-request" == (object) key)
          {
            this.IsFanoutRequest = !throwIfAlreadyExists || this.IsFanoutRequest == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if ((object) "x-ms-consistency-level" == (object) key)
          {
            this.ConsistencyLevel = !throwIfAlreadyExists || this.ConsistencyLevel == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if ((object) "x-ms-gateway-signature" == (object) key)
          {
            this.GatewaySignature = !throwIfAlreadyExists || this.GatewaySignature == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-is-fanout-request", key, StringComparison.OrdinalIgnoreCase))
          {
            this.IsFanoutRequest = !throwIfAlreadyExists || this.IsFanoutRequest == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-consistency-level", key, StringComparison.OrdinalIgnoreCase))
          {
            this.ConsistencyLevel = !throwIfAlreadyExists || this.ConsistencyLevel == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-gateway-signature", key, StringComparison.OrdinalIgnoreCase))
          {
            this.GatewaySignature = !throwIfAlreadyExists || this.GatewaySignature == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          break;
        case 23:
          if ((object) "x-ms-indexing-directive" == (object) key)
          {
            this.IndexingDirective = !throwIfAlreadyExists || this.IndexingDirective == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if ((object) "x-ms-primary-master-key" == (object) key)
          {
            this.PrimaryMasterKey = !throwIfAlreadyExists || this.PrimaryMasterKey == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if ((object) "x-ms-is-readonly-script" == (object) key)
          {
            this.IsReadOnlyScript = !throwIfAlreadyExists || this.IsReadOnlyScript == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-indexing-directive", key, StringComparison.OrdinalIgnoreCase))
          {
            this.IndexingDirective = !throwIfAlreadyExists || this.IndexingDirective == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-primary-master-key", key, StringComparison.OrdinalIgnoreCase))
          {
            this.PrimaryMasterKey = !throwIfAlreadyExists || this.PrimaryMasterKey == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-is-readonly-script", key, StringComparison.OrdinalIgnoreCase))
          {
            this.IsReadOnlyScript = !throwIfAlreadyExists || this.IsReadOnlyScript == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          break;
        case 24:
          if ((object) "collection-service-index" == (object) key)
          {
            this.CollectionServiceIndex = !throwIfAlreadyExists || this.CollectionServiceIndex == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if ((object) "x-ms-remote-storage-type" == (object) key)
          {
            this.RemoteStorageType = !throwIfAlreadyExists || this.RemoteStorageType == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if ((object) "x-ms-cosmos-batch-atomic" == (object) key)
          {
            this.IsBatchAtomic = !throwIfAlreadyExists || this.IsBatchAtomic == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("collection-service-index", key, StringComparison.OrdinalIgnoreCase))
          {
            this.CollectionServiceIndex = !throwIfAlreadyExists || this.CollectionServiceIndex == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-remote-storage-type", key, StringComparison.OrdinalIgnoreCase))
          {
            this.RemoteStorageType = !throwIfAlreadyExists || this.RemoteStorageType == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-cosmos-batch-atomic", key, StringComparison.OrdinalIgnoreCase))
          {
            this.IsBatchAtomic = !throwIfAlreadyExists || this.IsBatchAtomic == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          break;
        case 25:
          if ((object) "x-ms-resource-schema-name" == (object) key)
          {
            this.ResourceSchemaName = !throwIfAlreadyExists || this.ResourceSchemaName == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if ((object) "x-ms-secondary-master-key" == (object) key)
          {
            this.SecondaryMasterKey = !throwIfAlreadyExists || this.SecondaryMasterKey == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if ((object) "x-ms-primary-readonly-key" == (object) key)
          {
            this.PrimaryReadonlyKey = !throwIfAlreadyExists || this.PrimaryReadonlyKey == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if ((object) "x-ms-transport-request-id" == (object) key)
          {
            this.TransportRequestID = !throwIfAlreadyExists || this.TransportRequestID == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if ((object) "x-ms-cosmos-batch-ordered" == (object) key)
          {
            this.IsBatchOrdered = !throwIfAlreadyExists || this.IsBatchOrdered == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if ((object) "x-ms-cosmos-resourcetypes" == (object) key)
          {
            this.ResourceTypes = !throwIfAlreadyExists || this.ResourceTypes == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-resource-schema-name", key, StringComparison.OrdinalIgnoreCase))
          {
            this.ResourceSchemaName = !throwIfAlreadyExists || this.ResourceSchemaName == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-secondary-master-key", key, StringComparison.OrdinalIgnoreCase))
          {
            this.SecondaryMasterKey = !throwIfAlreadyExists || this.SecondaryMasterKey == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-primary-readonly-key", key, StringComparison.OrdinalIgnoreCase))
          {
            this.PrimaryReadonlyKey = !throwIfAlreadyExists || this.PrimaryReadonlyKey == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-transport-request-id", key, StringComparison.OrdinalIgnoreCase))
          {
            this.TransportRequestID = !throwIfAlreadyExists || this.TransportRequestID == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-cosmos-batch-ordered", key, StringComparison.OrdinalIgnoreCase))
          {
            this.IsBatchOrdered = !throwIfAlreadyExists || this.IsBatchOrdered == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-cosmos-resourcetypes", key, StringComparison.OrdinalIgnoreCase))
          {
            this.ResourceTypes = !throwIfAlreadyExists || this.ResourceTypes == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          break;
        case 26:
          if ((object) "collection-partition-index" == (object) key)
          {
            this.CollectionPartitionIndex = !throwIfAlreadyExists || this.CollectionPartitionIndex == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if ((object) "x-ms-enumeration-direction" == (object) key)
          {
            this.EnumerationDirection = !throwIfAlreadyExists || this.EnumerationDirection == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if ((object) "x-ms-cosmos-collectiontype" == (object) key)
          {
            this.RequestedCollectionType = !throwIfAlreadyExists || this.RequestedCollectionType == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("collection-partition-index", key, StringComparison.OrdinalIgnoreCase))
          {
            this.CollectionPartitionIndex = !throwIfAlreadyExists || this.CollectionPartitionIndex == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-enumeration-direction", key, StringComparison.OrdinalIgnoreCase))
          {
            this.EnumerationDirection = !throwIfAlreadyExists || this.EnumerationDirection == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-cosmos-collectiontype", key, StringComparison.OrdinalIgnoreCase))
          {
            this.RequestedCollectionType = !throwIfAlreadyExists || this.RequestedCollectionType == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          break;
        case 27:
          if ((object) "x-ms-secondary-readonly-key" == (object) key)
          {
            this.SecondaryReadonlyKey = !throwIfAlreadyExists || this.SecondaryReadonlyKey == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if ((object) "x-ms-fanout-operation-state" == (object) key)
          {
            this.FanoutOperationState = !throwIfAlreadyExists || this.FanoutOperationState == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if ((object) "x-ms-cosmos-merge-static-id" == (object) key)
          {
            this.MergeStaticId = !throwIfAlreadyExists || this.MergeStaticId == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-secondary-readonly-key", key, StringComparison.OrdinalIgnoreCase))
          {
            this.SecondaryReadonlyKey = !throwIfAlreadyExists || this.SecondaryReadonlyKey == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-fanout-operation-state", key, StringComparison.OrdinalIgnoreCase))
          {
            this.FanoutOperationState = !throwIfAlreadyExists || this.FanoutOperationState == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-cosmos-merge-static-id", key, StringComparison.OrdinalIgnoreCase))
          {
            this.MergeStaticId = !throwIfAlreadyExists || this.MergeStaticId == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          break;
        case 28:
          if ((object) "x-ms-documentdb-partitionkey" == (object) key)
          {
            this.PartitionKey = !throwIfAlreadyExists || this.PartitionKey == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if ((object) "x-ms-restore-metadata-filter" == (object) key)
          {
            this.RestoreMetadataFilter = !throwIfAlreadyExists || this.RestoreMetadataFilter == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if ((object) "x-ms-time-to-live-in-seconds" == (object) key)
          {
            this.TimeToLiveInSeconds = !throwIfAlreadyExists || this.TimeToLiveInSeconds == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if ((object) "x-ms-effective-partition-key" == (object) key)
          {
            this.EffectivePartitionKey = !throwIfAlreadyExists || this.EffectivePartitionKey == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if ((object) "x-ms-cosmos-use-systembudget" == (object) key)
          {
            this.UseSystemBudget = !throwIfAlreadyExists || this.UseSystemBudget == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-documentdb-partitionkey", key, StringComparison.OrdinalIgnoreCase))
          {
            this.PartitionKey = !throwIfAlreadyExists || this.PartitionKey == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-restore-metadata-filter", key, StringComparison.OrdinalIgnoreCase))
          {
            this.RestoreMetadataFilter = !throwIfAlreadyExists || this.RestoreMetadataFilter == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-time-to-live-in-seconds", key, StringComparison.OrdinalIgnoreCase))
          {
            this.TimeToLiveInSeconds = !throwIfAlreadyExists || this.TimeToLiveInSeconds == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-effective-partition-key", key, StringComparison.OrdinalIgnoreCase))
          {
            this.EffectivePartitionKey = !throwIfAlreadyExists || this.EffectivePartitionKey == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-cosmos-use-systembudget", key, StringComparison.OrdinalIgnoreCase))
          {
            this.UseSystemBudget = !throwIfAlreadyExists || this.UseSystemBudget == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          break;
        case 30:
          if ((object) "x-ms-documentdb-expiry-seconds" == (object) key)
          {
            this.ResourceTokenExpiry = !throwIfAlreadyExists || this.ResourceTokenExpiry == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if ((object) "x-ms-documentdb-partitioncount" == (object) key)
          {
            this.PartitionCount = !throwIfAlreadyExists || this.PartitionCount == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if ((object) "x-ms-documentdb-collection-rid" == (object) key)
          {
            this.CollectionRid = !throwIfAlreadyExists || this.CollectionRid == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if ((object) "x-ms-partition-resource-filter" == (object) key)
          {
            this.PartitionResourceFilter = !throwIfAlreadyExists || this.PartitionResourceFilter == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if ((object) "x-ms-exclude-system-properties" == (object) key)
          {
            this.ExcludeSystemProperties = !throwIfAlreadyExists || this.ExcludeSystemProperties == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-documentdb-expiry-seconds", key, StringComparison.OrdinalIgnoreCase))
          {
            this.ResourceTokenExpiry = !throwIfAlreadyExists || this.ResourceTokenExpiry == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-documentdb-partitioncount", key, StringComparison.OrdinalIgnoreCase))
          {
            this.PartitionCount = !throwIfAlreadyExists || this.PartitionCount == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-documentdb-collection-rid", key, StringComparison.OrdinalIgnoreCase))
          {
            this.CollectionRid = !throwIfAlreadyExists || this.CollectionRid == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-partition-resource-filter", key, StringComparison.OrdinalIgnoreCase))
          {
            this.PartitionResourceFilter = !throwIfAlreadyExists || this.PartitionResourceFilter == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-exclude-system-properties", key, StringComparison.OrdinalIgnoreCase))
          {
            this.ExcludeSystemProperties = !throwIfAlreadyExists || this.ExcludeSystemProperties == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          break;
        case 31:
          if ((object) "x-ms-client-retry-attempt-count" == (object) key)
          {
            this.ClientRetryAttemptCount = !throwIfAlreadyExists || this.ClientRetryAttemptCount == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if ((object) "x-ms-can-offer-replace-complete" == (object) key)
          {
            this.CanOfferReplaceComplete = !throwIfAlreadyExists || this.CanOfferReplaceComplete == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if ((object) "x-ms-binary-passthrough-request" == (object) key)
          {
            this.BinaryPassthroughRequest = !throwIfAlreadyExists || this.BinaryPassthroughRequest == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if ((object) "x-ms-cosmos-is-client-encrypted" == (object) key)
          {
            this.IsClientEncrypted = !throwIfAlreadyExists || this.IsClientEncrypted == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if ((object) "x-ms-cosmos-systemdocument-type" == (object) key)
          {
            this.SystemDocumentType = !throwIfAlreadyExists || this.SystemDocumentType == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if ((object) "x-ms-cosmos-collection-truncate" == (object) key)
          {
            this.CollectionTruncate = !throwIfAlreadyExists || this.CollectionTruncate == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-client-retry-attempt-count", key, StringComparison.OrdinalIgnoreCase))
          {
            this.ClientRetryAttemptCount = !throwIfAlreadyExists || this.ClientRetryAttemptCount == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-can-offer-replace-complete", key, StringComparison.OrdinalIgnoreCase))
          {
            this.CanOfferReplaceComplete = !throwIfAlreadyExists || this.CanOfferReplaceComplete == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-binary-passthrough-request", key, StringComparison.OrdinalIgnoreCase))
          {
            this.BinaryPassthroughRequest = !throwIfAlreadyExists || this.BinaryPassthroughRequest == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-cosmos-is-client-encrypted", key, StringComparison.OrdinalIgnoreCase))
          {
            this.IsClientEncrypted = !throwIfAlreadyExists || this.IsClientEncrypted == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-cosmos-systemdocument-type", key, StringComparison.OrdinalIgnoreCase))
          {
            this.SystemDocumentType = !throwIfAlreadyExists || this.SystemDocumentType == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-cosmos-collection-truncate", key, StringComparison.OrdinalIgnoreCase))
          {
            this.CollectionTruncate = !throwIfAlreadyExists || this.CollectionTruncate == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          break;
        case 32:
          if ((object) "x-ms-migratecollection-directive" == (object) key)
          {
            this.MigrateCollectionDirective = !throwIfAlreadyExists || this.MigrateCollectionDirective == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if ((object) "x-ms-target-global-committed-lsn" == (object) key)
          {
            this.TargetGlobalCommittedLsn = !throwIfAlreadyExists || this.TargetGlobalCommittedLsn == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if ((object) "x-ms-documentdb-force-query-scan" == (object) key)
          {
            this.ForceQueryScan = !throwIfAlreadyExists || this.ForceQueryScan == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if ((object) "x-ms-cosmos-max-polling-interval" == (object) key)
          {
            this.MaxPollingIntervalMilliseconds = !throwIfAlreadyExists || this.MaxPollingIntervalMilliseconds == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if ((object) "x-ms-cosmos-populateindexmetrics" == (object) key)
          {
            this.PopulateIndexMetrics = !throwIfAlreadyExists || this.PopulateIndexMetrics == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-migratecollection-directive", key, StringComparison.OrdinalIgnoreCase))
          {
            this.MigrateCollectionDirective = !throwIfAlreadyExists || this.MigrateCollectionDirective == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-target-global-committed-lsn", key, StringComparison.OrdinalIgnoreCase))
          {
            this.TargetGlobalCommittedLsn = !throwIfAlreadyExists || this.TargetGlobalCommittedLsn == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-documentdb-force-query-scan", key, StringComparison.OrdinalIgnoreCase))
          {
            this.ForceQueryScan = !throwIfAlreadyExists || this.ForceQueryScan == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-cosmos-max-polling-interval", key, StringComparison.OrdinalIgnoreCase))
          {
            this.MaxPollingIntervalMilliseconds = !throwIfAlreadyExists || this.MaxPollingIntervalMilliseconds == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-cosmos-populateindexmetrics", key, StringComparison.OrdinalIgnoreCase))
          {
            this.PopulateIndexMetrics = !throwIfAlreadyExists || this.PopulateIndexMetrics == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          break;
        case 33:
          if ((object) "x-ms-documentdb-query-enable-scan" == (object) key)
          {
            this.EnableScanInQuery = !throwIfAlreadyExists || this.EnableScanInQuery == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if ((object) "x-ms-documentdb-query-emit-traces" == (object) key)
          {
            this.EmitVerboseTracesInQuery = !throwIfAlreadyExists || this.EmitVerboseTracesInQuery == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if ((object) "x-ms-documentdb-populatequotainfo" == (object) key)
          {
            this.PopulateQuotaInfo = !throwIfAlreadyExists || this.PopulateQuotaInfo == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if ((object) "x-ms-cosmos-preserve-full-content" == (object) key)
          {
            this.PreserveFullContent = !throwIfAlreadyExists || this.PreserveFullContent == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if ((object) "x-ms-cosmos-populate-logstoreinfo" == (object) key)
          {
            this.PopulateLogStoreInfo = !throwIfAlreadyExists || this.PopulateLogStoreInfo == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if ((object) "x-ms-cosmos-correlated-activityid" == (object) key)
          {
            this.CorrelatedActivityId = !throwIfAlreadyExists || this.CorrelatedActivityId == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-documentdb-query-enable-scan", key, StringComparison.OrdinalIgnoreCase))
          {
            this.EnableScanInQuery = !throwIfAlreadyExists || this.EnableScanInQuery == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-documentdb-query-emit-traces", key, StringComparison.OrdinalIgnoreCase))
          {
            this.EmitVerboseTracesInQuery = !throwIfAlreadyExists || this.EmitVerboseTracesInQuery == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-documentdb-populatequotainfo", key, StringComparison.OrdinalIgnoreCase))
          {
            this.PopulateQuotaInfo = !throwIfAlreadyExists || this.PopulateQuotaInfo == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-cosmos-preserve-full-content", key, StringComparison.OrdinalIgnoreCase))
          {
            this.PreserveFullContent = !throwIfAlreadyExists || this.PreserveFullContent == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-cosmos-populate-logstoreinfo", key, StringComparison.OrdinalIgnoreCase))
          {
            this.PopulateLogStoreInfo = !throwIfAlreadyExists || this.PopulateLogStoreInfo == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-cosmos-correlated-activityid", key, StringComparison.OrdinalIgnoreCase))
          {
            this.CorrelatedActivityId = !throwIfAlreadyExists || this.CorrelatedActivityId == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          break;
        case 34:
          if ((object) "x-ms-cosmos-allow-tentative-writes" == (object) key)
          {
            this.AllowTentativeWrites = !throwIfAlreadyExists || this.AllowTentativeWrites == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if ((object) "x-ms-cosmos-use-archival-partition" == (object) key)
          {
            this.UseArchivalPartition = !throwIfAlreadyExists || this.UseArchivalPartition == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-cosmos-allow-tentative-writes", key, StringComparison.OrdinalIgnoreCase))
          {
            this.AllowTentativeWrites = !throwIfAlreadyExists || this.AllowTentativeWrites == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-cosmos-use-archival-partition", key, StringComparison.OrdinalIgnoreCase))
          {
            this.UseArchivalPartition = !throwIfAlreadyExists || this.UseArchivalPartition == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          break;
        case 35:
          if ((object) "x-ms-documentdb-pre-trigger-include" == (object) key)
          {
            this.PreTriggerInclude = !throwIfAlreadyExists || this.PreTriggerInclude == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if ((object) "x-ms-documentdb-pre-trigger-exclude" == (object) key)
          {
            this.PreTriggerExclude = !throwIfAlreadyExists || this.PreTriggerExclude == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if ((object) "x-ms-documentdb-partitionkeyrangeid" == (object) key)
          {
            this.PartitionKeyRangeId = !throwIfAlreadyExists || this.PartitionKeyRangeId == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if ((object) "x-ms-documentdb-filterby-schema-rid" == (object) key)
          {
            this.FilterBySchemaResourceId = !throwIfAlreadyExists || this.FilterBySchemaResourceId == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if ((object) "x-ms-remaining-time-in-ms-on-client" == (object) key)
          {
            this.RemainingTimeInMsOnClientRequest = !throwIfAlreadyExists || this.RemainingTimeInMsOnClientRequest == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if ((object) "x-ms-collection-security-identifier" == (object) key)
          {
            this.CollectionRemoteStorageSecurityIdentifier = !throwIfAlreadyExists || this.CollectionRemoteStorageSecurityIdentifier == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if ((object) "x-ms-cosmos-batch-continue-on-error" == (object) key)
          {
            this.ShouldBatchContinueOnError = !throwIfAlreadyExists || this.ShouldBatchContinueOnError == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if ((object) "x-ms-cosmos-intended-collection-rid" == (object) key)
          {
            this.IntendedCollectionRid = !throwIfAlreadyExists || this.IntendedCollectionRid == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-documentdb-pre-trigger-include", key, StringComparison.OrdinalIgnoreCase))
          {
            this.PreTriggerInclude = !throwIfAlreadyExists || this.PreTriggerInclude == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-documentdb-pre-trigger-exclude", key, StringComparison.OrdinalIgnoreCase))
          {
            this.PreTriggerExclude = !throwIfAlreadyExists || this.PreTriggerExclude == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-documentdb-partitionkeyrangeid", key, StringComparison.OrdinalIgnoreCase))
          {
            this.PartitionKeyRangeId = !throwIfAlreadyExists || this.PartitionKeyRangeId == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-documentdb-filterby-schema-rid", key, StringComparison.OrdinalIgnoreCase))
          {
            this.FilterBySchemaResourceId = !throwIfAlreadyExists || this.FilterBySchemaResourceId == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-remaining-time-in-ms-on-client", key, StringComparison.OrdinalIgnoreCase))
          {
            this.RemainingTimeInMsOnClientRequest = !throwIfAlreadyExists || this.RemainingTimeInMsOnClientRequest == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-collection-security-identifier", key, StringComparison.OrdinalIgnoreCase))
          {
            this.CollectionRemoteStorageSecurityIdentifier = !throwIfAlreadyExists || this.CollectionRemoteStorageSecurityIdentifier == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-cosmos-batch-continue-on-error", key, StringComparison.OrdinalIgnoreCase))
          {
            this.ShouldBatchContinueOnError = !throwIfAlreadyExists || this.ShouldBatchContinueOnError == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-cosmos-intended-collection-rid", key, StringComparison.OrdinalIgnoreCase))
          {
            this.IntendedCollectionRid = !throwIfAlreadyExists || this.IntendedCollectionRid == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          break;
        case 36:
          if ((object) "x-ms-documentdb-post-trigger-include" == (object) key)
          {
            this.PostTriggerInclude = !throwIfAlreadyExists || this.PostTriggerInclude == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if ((object) "x-ms-documentdb-post-trigger-exclude" == (object) key)
          {
            this.PostTriggerExclude = !throwIfAlreadyExists || this.PostTriggerExclude == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if ((object) "x-ms-documentdb-populatequerymetrics" == (object) key)
          {
            this.PopulateQueryMetrics = !throwIfAlreadyExists || this.PopulateQueryMetrics == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if ((object) "x-ms-cosmos-internal-is-user-request" == (object) key)
          {
            this.IsUserRequest = !throwIfAlreadyExists || this.IsUserRequest == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if ((object) "x-ms-cosmos-include-tentative-writes" == (object) key)
          {
            this.IncludeTentativeWrites = !throwIfAlreadyExists || this.IncludeTentativeWrites == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if ((object) "x-ms-cosmos-is-retried-write-request" == (object) key)
          {
            this.IsRetriedWriteRequest = !throwIfAlreadyExists || this.IsRetriedWriteRequest == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-documentdb-post-trigger-include", key, StringComparison.OrdinalIgnoreCase))
          {
            this.PostTriggerInclude = !throwIfAlreadyExists || this.PostTriggerInclude == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-documentdb-post-trigger-exclude", key, StringComparison.OrdinalIgnoreCase))
          {
            this.PostTriggerExclude = !throwIfAlreadyExists || this.PostTriggerExclude == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-documentdb-populatequerymetrics", key, StringComparison.OrdinalIgnoreCase))
          {
            this.PopulateQueryMetrics = !throwIfAlreadyExists || this.PopulateQueryMetrics == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-cosmos-internal-is-user-request", key, StringComparison.OrdinalIgnoreCase))
          {
            this.IsUserRequest = !throwIfAlreadyExists || this.IsUserRequest == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-cosmos-include-tentative-writes", key, StringComparison.OrdinalIgnoreCase))
          {
            this.IncludeTentativeWrites = !throwIfAlreadyExists || this.IncludeTentativeWrites == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-cosmos-is-retried-write-request", key, StringComparison.OrdinalIgnoreCase))
          {
            this.IsRetriedWriteRequest = !throwIfAlreadyExists || this.IsRetriedWriteRequest == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          break;
        case 37:
          if ((object) "x-ms-documentdb-script-enable-logging" == (object) key)
          {
            this.EnableLogging = !throwIfAlreadyExists || this.EnableLogging == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if ((object) "x-ms-documentdb-populateresourcecount" == (object) key)
          {
            this.PopulateResourceCount = !throwIfAlreadyExists || this.PopulateResourceCount == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if ((object) "x-ms-cosmos-sdk-supportedcapabilities" == (object) key)
          {
            this.SDKSupportedCapabilities = !throwIfAlreadyExists || this.SDKSupportedCapabilities == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if ((object) "x-ms-cosmos-builder-client-identifier" == (object) key)
          {
            this.BuilderClientIdentifier = !throwIfAlreadyExists || this.BuilderClientIdentifier == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-documentdb-script-enable-logging", key, StringComparison.OrdinalIgnoreCase))
          {
            this.EnableLogging = !throwIfAlreadyExists || this.EnableLogging == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-documentdb-populateresourcecount", key, StringComparison.OrdinalIgnoreCase))
          {
            this.PopulateResourceCount = !throwIfAlreadyExists || this.PopulateResourceCount == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-cosmos-sdk-supportedcapabilities", key, StringComparison.OrdinalIgnoreCase))
          {
            this.SDKSupportedCapabilities = !throwIfAlreadyExists || this.SDKSupportedCapabilities == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-cosmos-builder-client-identifier", key, StringComparison.OrdinalIgnoreCase))
          {
            this.BuilderClientIdentifier = !throwIfAlreadyExists || this.BuilderClientIdentifier == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          break;
        case 38:
          if ((object) "x-ms-cosmos-migrate-offer-to-autopilot" == (object) key)
          {
            this.MigrateOfferToAutopilot = !throwIfAlreadyExists || this.MigrateOfferToAutopilot == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if ((object) "x-ms-cosmos-retriable-write-request-id" == (object) key)
          {
            this.RetriableWriteRequestId = !throwIfAlreadyExists || this.RetriableWriteRequestId == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if ((object) "x-ms-cosmos-source-collection-if-match" == (object) key)
          {
            this.SourceCollectionIfMatch = !throwIfAlreadyExists || this.SourceCollectionIfMatch == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if ((object) "x-ms-cosmos-use-background-task-budget" == (object) key)
          {
            this.UseUserBackgroundBudget = !throwIfAlreadyExists || this.UseUserBackgroundBudget == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-cosmos-migrate-offer-to-autopilot", key, StringComparison.OrdinalIgnoreCase))
          {
            this.MigrateOfferToAutopilot = !throwIfAlreadyExists || this.MigrateOfferToAutopilot == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-cosmos-retriable-write-request-id", key, StringComparison.OrdinalIgnoreCase))
          {
            this.RetriableWriteRequestId = !throwIfAlreadyExists || this.RetriableWriteRequestId == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-cosmos-source-collection-if-match", key, StringComparison.OrdinalIgnoreCase))
          {
            this.SourceCollectionIfMatch = !throwIfAlreadyExists || this.SourceCollectionIfMatch == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-cosmos-use-background-task-budget", key, StringComparison.OrdinalIgnoreCase))
          {
            this.UseUserBackgroundBudget = !throwIfAlreadyExists || this.UseUserBackgroundBudget == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          break;
        case 39:
          if ((object) "x-ms-cosmos-internal-truncate-merge-log" == (object) key)
          {
            this.TruncateMergeLogRequest = !throwIfAlreadyExists || this.TruncateMergeLogRequest == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if ((object) "x-ms-cosmos-internal-serverless-request" == (object) key)
          {
            this.IsInternalServerlessRequest = !throwIfAlreadyExists || this.IsInternalServerlessRequest == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-cosmos-internal-truncate-merge-log", key, StringComparison.OrdinalIgnoreCase))
          {
            this.TruncateMergeLogRequest = !throwIfAlreadyExists || this.TruncateMergeLogRequest == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-cosmos-internal-serverless-request", key, StringComparison.OrdinalIgnoreCase))
          {
            this.IsInternalServerlessRequest = !throwIfAlreadyExists || this.IsInternalServerlessRequest == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          break;
        case 40:
          if ((object) "x-ms-enable-dynamic-rid-range-allocation" == (object) key)
          {
            this.EnableDynamicRidRangeAllocation = !throwIfAlreadyExists || this.EnableDynamicRidRangeAllocation == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if ((object) "x-ms-cosmos-uniqueindex-reindexing-state" == (object) key)
          {
            this.UniqueIndexReIndexingState = !throwIfAlreadyExists || this.UniqueIndexReIndexingState == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-enable-dynamic-rid-range-allocation", key, StringComparison.OrdinalIgnoreCase))
          {
            this.EnableDynamicRidRangeAllocation = !throwIfAlreadyExists || this.EnableDynamicRidRangeAllocation == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-cosmos-uniqueindex-reindexing-state", key, StringComparison.OrdinalIgnoreCase))
          {
            this.UniqueIndexReIndexingState = !throwIfAlreadyExists || this.UniqueIndexReIndexingState == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          break;
        case 41:
          if (string.Equals("x-ms-cosmos-populate-oldest-active-schema", key, StringComparison.OrdinalIgnoreCase))
          {
            this.PopulateOldestActiveSchema = !throwIfAlreadyExists || this.PopulateOldestActiveSchema == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          break;
        case 42:
          if ((object) "x-ms-cosmos-internal-merge-checkpoint-glsn" == (object) key)
          {
            this.MergeCheckPointGLSN = !throwIfAlreadyExists || this.MergeCheckPointGLSN == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if ((object) "x-ms-should-return-current-server-datetime" == (object) key)
          {
            this.ShouldReturnCurrentServerDateTime = !throwIfAlreadyExists || this.ShouldReturnCurrentServerDateTime == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if ((object) "x-ms-cosmos-changefeed-wire-format-version" == (object) key)
          {
            this.ChangeFeedWireFormatVersion = !throwIfAlreadyExists || this.ChangeFeedWireFormatVersion == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-cosmos-internal-merge-checkpoint-glsn", key, StringComparison.OrdinalIgnoreCase))
          {
            this.MergeCheckPointGLSN = !throwIfAlreadyExists || this.MergeCheckPointGLSN == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-should-return-current-server-datetime", key, StringComparison.OrdinalIgnoreCase))
          {
            this.ShouldReturnCurrentServerDateTime = !throwIfAlreadyExists || this.ShouldReturnCurrentServerDateTime == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-cosmos-changefeed-wire-format-version", key, StringComparison.OrdinalIgnoreCase))
          {
            this.ChangeFeedWireFormatVersion = !throwIfAlreadyExists || this.ChangeFeedWireFormatVersion == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          break;
        case 43:
          if ((object) "x-ms-documentdb-disable-ru-per-minute-usage" == (object) key)
          {
            this.DisableRUPerMinuteUsage = !throwIfAlreadyExists || this.DisableRUPerMinuteUsage == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if ((object) "x-ms-documentdb-populatepartitionstatistics" == (object) key)
          {
            this.PopulatePartitionStatistics = !throwIfAlreadyExists || this.PopulatePartitionStatistics == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if ((object) "x-ms-cosmos-force-sidebyside-indexmigration" == (object) key)
          {
            this.ForceSideBySideIndexMigration = !throwIfAlreadyExists || this.ForceSideBySideIndexMigration == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if ((object) "x-ms-cosmos-unique-index-name-encoding-mode" == (object) key)
          {
            this.UniqueIndexNameEncodingMode = !throwIfAlreadyExists || this.UniqueIndexNameEncodingMode == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-documentdb-disable-ru-per-minute-usage", key, StringComparison.OrdinalIgnoreCase))
          {
            this.DisableRUPerMinuteUsage = !throwIfAlreadyExists || this.DisableRUPerMinuteUsage == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-documentdb-populatepartitionstatistics", key, StringComparison.OrdinalIgnoreCase))
          {
            this.PopulatePartitionStatistics = !throwIfAlreadyExists || this.PopulatePartitionStatistics == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-cosmos-force-sidebyside-indexmigration", key, StringComparison.OrdinalIgnoreCase))
          {
            this.ForceSideBySideIndexMigration = !throwIfAlreadyExists || this.ForceSideBySideIndexMigration == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-cosmos-unique-index-name-encoding-mode", key, StringComparison.OrdinalIgnoreCase))
          {
            this.UniqueIndexNameEncodingMode = !throwIfAlreadyExists || this.UniqueIndexNameEncodingMode == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          break;
        case 44:
          if (string.Equals("x-ms-documentdb-content-serialization-format", key, StringComparison.OrdinalIgnoreCase))
          {
            this.ContentSerializationFormat = !throwIfAlreadyExists || this.ContentSerializationFormat == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          break;
        case 45:
          if ((object) "x-ms-cosmos-start-full-fidelity-if-none-match" == (object) key)
          {
            this.ChangeFeedStartFullFidelityIfNoneMatch = !throwIfAlreadyExists || this.ChangeFeedStartFullFidelityIfNoneMatch == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if ((object) "x-ms-cosmos-internal-system-restore-operation" == (object) key)
          {
            this.SystemRestoreOperation = !throwIfAlreadyExists || this.SystemRestoreOperation == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if ((object) "x-ms-cosmos-internal-is-throughputcap-request" == (object) key)
          {
            this.IsThroughputCapRequest = !throwIfAlreadyExists || this.IsThroughputCapRequest == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if ((object) "x-ms-cosmos-populate-byok-encryption-progress" == (object) key)
          {
            this.PopulateByokEncryptionProgress = !throwIfAlreadyExists || this.PopulateByokEncryptionProgress == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-cosmos-start-full-fidelity-if-none-match", key, StringComparison.OrdinalIgnoreCase))
          {
            this.ChangeFeedStartFullFidelityIfNoneMatch = !throwIfAlreadyExists || this.ChangeFeedStartFullFidelityIfNoneMatch == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-cosmos-internal-system-restore-operation", key, StringComparison.OrdinalIgnoreCase))
          {
            this.SystemRestoreOperation = !throwIfAlreadyExists || this.SystemRestoreOperation == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-cosmos-internal-is-throughputcap-request", key, StringComparison.OrdinalIgnoreCase))
          {
            this.IsThroughputCapRequest = !throwIfAlreadyExists || this.IsThroughputCapRequest == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-cosmos-populate-byok-encryption-progress", key, StringComparison.OrdinalIgnoreCase))
          {
            this.PopulateByokEncryptionProgress = !throwIfAlreadyExists || this.PopulateByokEncryptionProgress == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          break;
        case 46:
          if ((object) "x-ms-cosmos-migrate-offer-to-manual-throughput" == (object) key)
          {
            this.MigrateOfferToManualThroughput = !throwIfAlreadyExists || this.MigrateOfferToManualThroughput == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if ((object) "x-ms-cosmos-skip-refresh-databaseaccountconfig" == (object) key)
          {
            this.SkipRefreshDatabaseAccountConfigs = !throwIfAlreadyExists || this.SkipRefreshDatabaseAccountConfigs == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-cosmos-migrate-offer-to-manual-throughput", key, StringComparison.OrdinalIgnoreCase))
          {
            this.MigrateOfferToManualThroughput = !throwIfAlreadyExists || this.MigrateOfferToManualThroughput == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-cosmos-skip-refresh-databaseaccountconfig", key, StringComparison.OrdinalIgnoreCase))
          {
            this.SkipRefreshDatabaseAccountConfigs = !throwIfAlreadyExists || this.SkipRefreshDatabaseAccountConfigs == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          break;
        case 47:
          if ((object) "x-ms-documentdb-supportspatiallegacycoordinates" == (object) key)
          {
            this.SupportSpatialLegacyCoordinates = !throwIfAlreadyExists || this.SupportSpatialLegacyCoordinates == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if ((object) "x-ms-cosmos-collection-child-resourcename-limit" == (object) key)
          {
            this.CollectionChildResourceNameLimitInBytes = !throwIfAlreadyExists || this.CollectionChildResourceNameLimitInBytes == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if ((object) "x-ms-cosmos-add-resource-properties-to-response" == (object) key)
          {
            this.AddResourcePropertiesToResponse = !throwIfAlreadyExists || this.AddResourcePropertiesToResponse == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if ((object) "x-ms-cosmos-internal-is-materialized-view-build" == (object) key)
          {
            this.IsMaterializedViewBuild = !throwIfAlreadyExists || this.IsMaterializedViewBuild == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-documentdb-supportspatiallegacycoordinates", key, StringComparison.OrdinalIgnoreCase))
          {
            this.SupportSpatialLegacyCoordinates = !throwIfAlreadyExists || this.SupportSpatialLegacyCoordinates == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-cosmos-collection-child-resourcename-limit", key, StringComparison.OrdinalIgnoreCase))
          {
            this.CollectionChildResourceNameLimitInBytes = !throwIfAlreadyExists || this.CollectionChildResourceNameLimitInBytes == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-cosmos-add-resource-properties-to-response", key, StringComparison.OrdinalIgnoreCase))
          {
            this.AddResourcePropertiesToResponse = !throwIfAlreadyExists || this.AddResourcePropertiesToResponse == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-cosmos-internal-is-materialized-view-build", key, StringComparison.OrdinalIgnoreCase))
          {
            this.IsMaterializedViewBuild = !throwIfAlreadyExists || this.IsMaterializedViewBuild == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          break;
        case 48:
          if ((object) "x-ms-documentdb-populatecollectionthroughputinfo" == (object) key)
          {
            this.PopulateCollectionThroughputInfo = !throwIfAlreadyExists || this.PopulateCollectionThroughputInfo == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if ((object) "x-ms-cosmos-internal-get-all-partition-key-stats" == (object) key)
          {
            this.GetAllPartitionKeyStatistics = !throwIfAlreadyExists || this.GetAllPartitionKeyStatistics == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if ((object) "x-ms-cosmosdb-populateuniqueindexreindexprogress" == (object) key)
          {
            this.PopulateUniqueIndexReIndexProgress = !throwIfAlreadyExists || this.PopulateUniqueIndexReIndexProgress == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-documentdb-populatecollectionthroughputinfo", key, StringComparison.OrdinalIgnoreCase))
          {
            this.PopulateCollectionThroughputInfo = !throwIfAlreadyExists || this.PopulateCollectionThroughputInfo == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-cosmos-internal-get-all-partition-key-stats", key, StringComparison.OrdinalIgnoreCase))
          {
            this.GetAllPartitionKeyStatistics = !throwIfAlreadyExists || this.GetAllPartitionKeyStatistics == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-cosmosdb-populateuniqueindexreindexprogress", key, StringComparison.OrdinalIgnoreCase))
          {
            this.PopulateUniqueIndexReIndexProgress = !throwIfAlreadyExists || this.PopulateUniqueIndexReIndexProgress == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          break;
        case 49:
          if (string.Equals("x-ms-documentdb-usepolygonssmallerthanahemisphere", key, StringComparison.OrdinalIgnoreCase))
          {
            this.UsePolygonsSmallerThanAHemisphere = !throwIfAlreadyExists || this.UsePolygonsSmallerThanAHemisphere == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          break;
        case 50:
          if ((object) "x-ms-documentdb-responsecontinuationtokenlimitinkb" == (object) key)
          {
            this.ResponseContinuationTokenLimitInKB = !throwIfAlreadyExists || this.ResponseContinuationTokenLimitInKB == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if ((object) "x-ms-cosmos-populate-analytical-migration-progress" == (object) key)
          {
            this.PopulateAnalyticalMigrationProgress = !throwIfAlreadyExists || this.PopulateAnalyticalMigrationProgress == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if ((object) "x-ms-cosmos-internal-update-offer-state-to-pending" == (object) key)
          {
            this.UpdateOfferStateToPending = !throwIfAlreadyExists || this.UpdateOfferStateToPending == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-documentdb-responsecontinuationtokenlimitinkb", key, StringComparison.OrdinalIgnoreCase))
          {
            this.ResponseContinuationTokenLimitInKB = !throwIfAlreadyExists || this.ResponseContinuationTokenLimitInKB == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-cosmos-populate-analytical-migration-progress", key, StringComparison.OrdinalIgnoreCase))
          {
            this.PopulateAnalyticalMigrationProgress = !throwIfAlreadyExists || this.PopulateAnalyticalMigrationProgress == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-cosmos-internal-update-offer-state-to-pending", key, StringComparison.OrdinalIgnoreCase))
          {
            this.UpdateOfferStateToPending = !throwIfAlreadyExists || this.UpdateOfferStateToPending == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          break;
        case 51:
          if ((object) "x-ms-documentdb-query-enable-low-precision-order-by" == (object) key)
          {
            this.EnableLowPrecisionOrderBy = !throwIfAlreadyExists || this.EnableLowPrecisionOrderBy == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if ((object) "x-ms-cosmos-retriable-write-request-start-timestamp" == (object) key)
          {
            this.RetriableWriteRequestStartTimestamp = !throwIfAlreadyExists || this.RetriableWriteRequestStartTimestamp == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-documentdb-query-enable-low-precision-order-by", key, StringComparison.OrdinalIgnoreCase))
          {
            this.EnableLowPrecisionOrderBy = !throwIfAlreadyExists || this.EnableLowPrecisionOrderBy == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-cosmos-retriable-write-request-start-timestamp", key, StringComparison.OrdinalIgnoreCase))
          {
            this.RetriableWriteRequestStartTimestamp = !throwIfAlreadyExists || this.RetriableWriteRequestStartTimestamp == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          break;
        case 52:
          if (string.Equals("x-ms-cosmos-internal-offer-replace-ru-redistribution", key, StringComparison.OrdinalIgnoreCase))
          {
            this.OfferReplaceRURedistribution = !throwIfAlreadyExists || this.OfferReplaceRURedistribution == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          break;
        case 53:
          if ((object) "x-ms-cosmos-internal-is-ru-per-gb-enforcement-request" == (object) key)
          {
            this.IsRUPerGBEnforcementRequest = !throwIfAlreadyExists || this.IsRUPerGBEnforcementRequest == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if ((object) "x-ms-cosmos-internal-is-offer-storage-refresh-request" == (object) key)
          {
            this.IsOfferStorageRefreshRequest = !throwIfAlreadyExists || this.IsOfferStorageRefreshRequest == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-cosmos-internal-is-ru-per-gb-enforcement-request", key, StringComparison.OrdinalIgnoreCase))
          {
            this.IsRUPerGBEnforcementRequest = !throwIfAlreadyExists || this.IsRUPerGBEnforcementRequest == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-cosmos-internal-is-offer-storage-refresh-request", key, StringComparison.OrdinalIgnoreCase))
          {
            this.IsOfferStorageRefreshRequest = !throwIfAlreadyExists || this.IsOfferStorageRefreshRequest == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          break;
        case 54:
          if (string.Equals("x-ms-cosmos-include-physical-partition-throughput-info", key, StringComparison.OrdinalIgnoreCase))
          {
            this.IncludePhysicalPartitionThroughputInfo = !throwIfAlreadyExists || this.IncludePhysicalPartitionThroughputInfo == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          break;
        case 56:
          if (string.Equals("x-ms-cosmos-collection-child-contentlength-resourcelimit", key, StringComparison.OrdinalIgnoreCase))
          {
            this.CollectionChildResourceContentLimitInKB = !throwIfAlreadyExists || this.CollectionChildResourceContentLimitInKB == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          break;
        case 57:
          if (string.Equals("x-ms-cosmos-internal-populate-unflushed-merge-entry-count", key, StringComparison.OrdinalIgnoreCase))
          {
            this.PopulateUnflushedMergeEntryCount = !throwIfAlreadyExists || this.PopulateUnflushedMergeEntryCount == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          break;
        case 58:
          if (string.Equals("x-ms-cosmos-internal-ignore-system-lowering-max-throughput", key, StringComparison.OrdinalIgnoreCase))
          {
            this.IgnoreSystemLoweringMaxThroughput = !throwIfAlreadyExists || this.IgnoreSystemLoweringMaxThroughput == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          break;
        case 59:
          if (string.Equals("x-ms-cosmos-internal-update-max-throughput-ever-provisioned", key, StringComparison.OrdinalIgnoreCase))
          {
            this.UpdateMaxThroughputEverProvisioned = !throwIfAlreadyExists || this.UpdateMaxThroughputEverProvisioned == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          break;
        case 61:
          if (string.Equals("x-ms-cosmos-internal-serverless-offer-storage-refresh-request", key, StringComparison.OrdinalIgnoreCase))
          {
            this.IsServerlessStorageRefreshRequest = !throwIfAlreadyExists || this.IsServerlessStorageRefreshRequest == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          break;
      }
      if (throwIfAlreadyExists)
        this.lazyNotCommonHeaders.Value.Add(key, value);
      else if (value == null)
      {
        if (!this.lazyNotCommonHeaders.IsValueCreated)
          return;
        this.lazyNotCommonHeaders.Value.Remove(key);
      }
      else
        this.lazyNotCommonHeaders.Value[key] = value;
    }
  }
}
