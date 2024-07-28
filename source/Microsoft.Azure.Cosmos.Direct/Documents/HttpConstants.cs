// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.HttpConstants
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Microsoft.Azure.Documents
{
  internal static class HttpConstants
  {
    private static readonly Dictionary<int, string> StatusCodeDescriptionMap = new Dictionary<int, string>();

    static HttpConstants()
    {
      HttpConstants.StatusCodeDescriptionMap.Add(202, "Accepted");
      HttpConstants.StatusCodeDescriptionMap.Add(409, "Conflict");
      HttpConstants.StatusCodeDescriptionMap.Add(200, "Ok");
      HttpConstants.StatusCodeDescriptionMap.Add(412, "Precondition Failed");
      HttpConstants.StatusCodeDescriptionMap.Add(304, "Not Modified");
      HttpConstants.StatusCodeDescriptionMap.Add(404, "Not Found");
      HttpConstants.StatusCodeDescriptionMap.Add(502, "Bad Gateway");
      HttpConstants.StatusCodeDescriptionMap.Add(400, "Bad Request");
      HttpConstants.StatusCodeDescriptionMap.Add(500, "Internal Server Error");
      HttpConstants.StatusCodeDescriptionMap.Add(405, "MethodNotAllowed");
      HttpConstants.StatusCodeDescriptionMap.Add(406, "Not Acceptable");
      HttpConstants.StatusCodeDescriptionMap.Add(204, "No Content");
      HttpConstants.StatusCodeDescriptionMap.Add(201, "Created");
      HttpConstants.StatusCodeDescriptionMap.Add(207, "Multi-Status");
      HttpConstants.StatusCodeDescriptionMap.Add(415, "Unsupported Media Type");
      HttpConstants.StatusCodeDescriptionMap.Add(411, "Length Required");
      HttpConstants.StatusCodeDescriptionMap.Add(503, "Service Unavailable");
      HttpConstants.StatusCodeDescriptionMap.Add(413, "Request Entity Too Large");
      HttpConstants.StatusCodeDescriptionMap.Add(401, "Unauthorized");
      HttpConstants.StatusCodeDescriptionMap.Add(403, "Forbidden");
      HttpConstants.StatusCodeDescriptionMap.Add(410, "Gone");
      HttpConstants.StatusCodeDescriptionMap.Add(408, "Request timed out");
      HttpConstants.StatusCodeDescriptionMap.Add(504, "Gateway timed out");
      HttpConstants.StatusCodeDescriptionMap.Add(429, "Too Many Requests");
      HttpConstants.StatusCodeDescriptionMap.Add(449, "Retry the request");
      HttpConstants.StatusCodeDescriptionMap.Add(423, "Locked");
      HttpConstants.StatusCodeDescriptionMap.Add(424, "Failed Dependency");
    }

    public static string GetStatusCodeDescription(int statusCode)
    {
      string empty = string.Empty;
      if (!HttpConstants.StatusCodeDescriptionMap.TryGetValue(statusCode, out empty))
        empty = string.Empty;
      return empty;
    }

    public static class HttpMethods
    {
      public const string Get = "GET";
      public const string Post = "POST";
      public const string Put = "PUT";
      public const string Delete = "DELETE";
      public const string Head = "HEAD";
      public const string Options = "OPTIONS";
      public const string Patch = "PATCH";
      public const string Merge = "MERGE";
    }

    public static class HttpHeaders
    {
      public const string Authorization = "authorization";
      public const string ETag = "etag";
      public const string MethodOverride = "X-HTTP-Method";
      public const string Slug = "Slug";
      public const string ContentType = "Content-Type";
      public const string LastModified = "Last-Modified";
      public const string LastEventId = "Last-Event-ID";
      public const string ContentEncoding = "Content-Encoding";
      public const string ContentTransferEncoding = "Content-Transfer-Encoding";
      public const string CharacterSet = "CharacterSet";
      public const string UserAgent = "User-Agent";
      public const string IfModifiedSince = "If-Modified-Since";
      public const string IfMatch = "If-Match";
      public const string IfNoneMatch = "If-None-Match";
      public const string A_IM = "A-IM";
      public const string ContentLength = "Content-Length";
      public const string AcceptEncoding = "Accept-Encoding";
      public const string KeepAlive = "Keep-Alive";
      public const string CacheControl = "Cache-Control";
      public const string TransferEncoding = "Transfer-Encoding";
      public const string Connection = "Connection";
      public const string ContentLanguage = "Content-Language";
      public const string ContentLocation = "Content-Location";
      public const string ContentMd5 = "Content-Md5";
      public const string ContentRange = "Content-Range";
      public const string Accept = "Accept";
      public const string AcceptCharset = "Accept-Charset";
      public const string AcceptLanguage = "Accept-Language";
      public const string IfRange = "If-Range";
      public const string IfUnmodifiedSince = "If-Unmodified-Since";
      public const string MaxForwards = "Max-Forwards";
      public const string ProxyAuthorization = "Proxy-Authorization";
      public const string AcceptRanges = "Accept-Ranges";
      public const string ProxyAuthenticate = "Proxy-Authenticate";
      public const string RetryAfter = "Retry-After";
      public const string SetCookie = "Set-Cookie";
      public const string WwwAuthenticate = "Www-Authenticate";
      public const string WwwAuthenticateDsts = "Www-Authenticate-dSTS";
      public const string Origin = "Origin";
      public const string Host = "Host";
      public const string AccessControlAllowCredentials = "Access-Control-Allow-Credentials";
      public const string AccessControlAllowOrigin = "Access-Control-Allow-Origin";
      public const string AccessControlAllowHeaders = "Access-Control-Allow-Headers";
      public const string AccessControlAllowMethods = "Access-Control-Allow-Methods";
      public const string AccessControlExposeHeaders = "Access-Control-Expose-Headers";
      public const string AccessControlMaxAge = "Access-Control-Max-Age";
      public const string AccessControlRequestHeaders = "Access-Control-Request-Headers";
      public const string AccessControlRequestMethod = "Access-Control-Request-Method";
      public const string KeyValueEncodingFormat = "application/x-www-form-urlencoded";
      public const string WrapAssertionFormat = "wrap_assertion_format";
      public const string WrapAssertion = "wrap_assertion";
      public const string WrapScope = "wrap_scope";
      public const string SimpleToken = "SWT";
      public const string HttpDate = "date";
      public const string Prefer = "Prefer";
      public const string PreferenceApplied = "Preference-Applied";
      public const string Location = "Location";
      public const string GlobalDatabaseAccountName = "GlobalDatabaseAccountName";
      public const string AzureAsyncOperation = "Azure-AsyncOperation";
      public const string Referer = "referer";
      public const string StrictTransportSecurity = "Strict-Transport-Security";
      public const string Pragma = "Pragma";
      public const string IsParallel = "isParallel";
      public const string Expires = "Expires";
      public const string Server = "Server";
      public const string XForwardedFor = "x-forwarded-for";
      public const string IsForwardedRequest = "isForwardedRequest";
      public const string TablePartitonKey = "Partiton-Key";
      public const string TableRowKey = "Row-Key";
      public const string ColumnName = "Column-Name";
      public const string ColumnValue = "Column-Value";
      public const string PartitionKeyName = "PartitionKeyName";
      public const string RowKeyName = "RowKeyName";
      public const string IsListRequest = "IsListRequest";
      public const string IsCreateRequest = "IsCreateRequest";
      public const string Query = "x-ms-documentdb-query";
      public const string IsQuery = "x-ms-documentdb-isquery";
      public const string QueryMetrics = "x-ms-documentdb-query-metrics";
      public const string QueryExecutionInfo = "x-ms-cosmos-query-execution-info";
      public const string IndexUtilization = "x-ms-cosmos-index-utilization";
      public const string SqlCacheHit = "x-ms-cosmos-cachehit";
      public const string SqlCacheStale = "x-ms-cosmos-cache-stale";
      public const string PopulateQueryMetrics = "x-ms-documentdb-populatequerymetrics";
      public const string PopulateIndexMetrics = "x-ms-cosmos-populateindexmetrics";
      public const string CorrelatedActivityId = "x-ms-cosmos-correlated-activityid";
      public const string ResponseContinuationTokenLimitInKB = "x-ms-documentdb-responsecontinuationtokenlimitinkb";
      public const string ForceQueryScan = "x-ms-documentdb-force-query-scan";
      public const string CanCharge = "x-ms-cancharge";
      public const string CanThrottle = "x-ms-canthrottle";
      public const string AllowCachedReads = "x-ms-cosmos-allow-cachedreads";
      public const string Continuation = "x-ms-continuation";
      public const string PageSize = "x-ms-max-item-count";
      public const string EnableLogging = "x-ms-documentdb-script-enable-logging";
      public const string LogResults = "x-ms-documentdb-script-log-results";
      public const string IsBatchRequest = "x-ms-cosmos-is-batch-request";
      public const string ShouldBatchContinueOnError = "x-ms-cosmos-batch-continue-on-error";
      public const string IsBatchOrdered = "x-ms-cosmos-batch-ordered";
      public const string IsBatchAtomic = "x-ms-cosmos-batch-atomic";
      public const string PartitionProgresses = "x-ms-cosmos-batch-partition-progresses";
      public const string ActivityId = "x-ms-activity-id";
      public const string PreTriggerInclude = "x-ms-documentdb-pre-trigger-include";
      public const string PreTriggerExclude = "x-ms-documentdb-pre-trigger-exclude";
      public const string PostTriggerInclude = "x-ms-documentdb-post-trigger-include";
      public const string PostTriggerExclude = "x-ms-documentdb-post-trigger-exclude";
      public const string IndexingDirective = "x-ms-indexing-directive";
      public const string MigrateCollectionDirective = "x-ms-migratecollection-directive";
      public const string SessionToken = "x-ms-session-token";
      public const string ConsistencyLevel = "x-ms-consistency-level";
      public const string XDate = "x-ms-date";
      public const string CollectionPartitionInfo = "x-ms-collection-partition-info";
      public const string CollectionServiceInfo = "x-ms-collection-service-info";
      public const string RetryAfterInMilliseconds = "x-ms-retry-after-ms";
      public const string IsFeedUnfiltered = "x-ms-is-feed-unfiltered";
      public const string ResourceTokenExpiry = "x-ms-documentdb-expiry-seconds";
      public const string EnableScanInQuery = "x-ms-documentdb-query-enable-scan";
      public const string EnableLowPrecisionOrderBy = "x-ms-documentdb-query-enable-low-precision-order-by";
      public const string EmitVerboseTracesInQuery = "x-ms-documentdb-query-emit-traces";
      public const string EnableCrossPartitionQuery = "x-ms-documentdb-query-enablecrosspartition";
      public const string ParallelizeCrossPartitionQuery = "x-ms-documentdb-query-parallelizecrosspartitionquery";
      public const string IsContinuationExpected = "x-ms-documentdb-query-iscontinuationexpected";
      public const string SqlQueryForPartitionKeyExtraction = "x-ms-documentdb-query-sqlqueryforpartitionkeyextraction";
      public const string ContentSerializationFormat = "x-ms-documentdb-content-serialization-format";
      public const string ProfileRequest = "x-ms-profile-request";
      public const string MaxResourceQuota = "x-ms-resource-quota";
      public const string CurrentResourceQuotaUsage = "x-ms-resource-usage";
      public const string MaxMediaStorageUsageInMB = "x-ms-max-media-storage-usage-mb";
      public const string RequestCharge = "x-ms-request-charge";
      public const string CurrentMediaStorageUsageInMB = "x-ms-media-storage-usage-mb";
      public const string DatabaseAccountConsumedDocumentStorageInMB = "x-ms-databaseaccount-consumed-mb";
      public const string DatabaseAccountReservedDocumentStorageInMB = "x-ms-databaseaccount-reserved-mb";
      public const string DatabaseAccountProvisionedDocumentStorageInMB = "x-ms-databaseaccount-provisioned-mb";
      public const string OwnerFullName = "x-ms-alt-content-path";
      public const string OwnerId = "x-ms-content-path";
      public const string ForceRefresh = "x-ms-force-refresh";
      public const string ForceNameCacheRefresh = "x-ms-namecache-refresh";
      public const string ForceCollectionRoutingMapRefresh = "x-ms-collectionroutingmap-refresh";
      public const string ItemCount = "x-ms-item-count";
      public const string NewResourceId = "x-ms-new-resource-id";
      public const string UseMasterCollectionResolver = "x-ms-use-master-collection-resolver";
      public const string LocalRegionRequest = "x-ms-local-region-request";
      public const string FullUpgrade = "x-ms-force-full-upgrade";
      public const string OnlyUpgradeSystemApplications = "x-ms-only-upgrade-system-applications";
      public const string OnlyUpgradeNonSystemApplications = "x-ms-only-upgrade-non-system-applications";
      public const string IgnoreInProgressUpgrade = "x-ms-ignore-inprogress-upgrade";
      public const string IgnoreVersionCheck = "x-ms-ignore-version-check";
      public const string IgnoreUpgradeControlRule = "x-ms-ignore-upgrade-control-rule";
      public const string IsDowngrade = "x-ms-is-downgrade";
      public const string CommonAssemblyVersion = "x-ms-common-assembly-version";
      public const string AuthorizeOperationUsingHeader = "x-ms-authorize-using-header";
      public const string CapabilityToMigrate = "x-ms-capability";
      public const string IncludeAccountSecrets = "x-ms-include-account-secrets";
      public const string MongoServerVersion = "x-ms-mongo-server-version";
      public const string UpgradeOrder = "x-ms-upgrade-order";
      public const string UpgradeAppConfigOnly = "x-ms-upgrade-app-config-only";
      public const string UpgradeFabricRingCodeAndConfig = "x-ms-upgrade-fabric-code-config";
      public const string UpgradeFabricRingConfigOnly = "x-ms-upgrade-fabric-config-only";
      public const string IsAzGrowShrink = "x-ms-is-az-grow-shrink";
      public const string UpgradePackageConfigOnly = "x-ms-upgrade-package-config-only";
      public const string Flight = "x-ms-flight";
      public const string BatchUpgrade = "x-ms-is-batch-upgrade";
      public const string SkipClusterVersionCheck = "x-ms-skip-cluster-version-check";
      public const string UpgradeVerificationKind = "x-ms-upgrade-verification-kind";
      public const string IsCanary = "x-ms-iscanary";
      public const string SubscriptionId = "x-ms-subscription-id";
      public const string ForceDelete = "x-ms-force-delete";
      public const string ForceUpdate = "x-ms-force-update";
      public const string SystemStoreType = "x-ms-storetype";
      public const string PitrEnabled = "x-ms-pitrenabled";
      public const string PitrSku = "x-ms-pitrsku";
      public const string BackupRedundancy = "x-ms-backupredundancy";
      public const string EnableFullFidelityChangeFeed = "x-ms-enable-full-fidelity-changefeed";
      public const string GremlinClusterPlacementHint = "x-ms-gremlin-cluster-placement-hint";
      public const string GremlinClusterSize = "x-ms-gremlin-cluster-size";
      public const string GremlinClusterInstanceCount = "x-ms-gremlin-cluster-instance-count";
      public const string UseMonitoredUpgrade = "x-ms-use-monitored-upgrade";
      public const string UseUnmonitoredAutoUpgrade = "x-ms-use-unmonitored-auto-upgrade";
      public const string HealthCheckRetryTimeout = "x-ms-health-check-retry-timeout";
      public const string UpgradeDomainTimeout = "x-ms-upgrade-domain-timeout";
      public const string UpgradeTimeout = "x-ms-upgrade-timeout";
      public const string ConsiderWarningAsError = "x-ms-consider-warning-as-error";
      public const string QueueId = "x-ms-queueid";
      public const string Priority = "x-ms-priority";
      public const string CancelAll = "x-ms-cancelall";
      public const string Version = "x-ms-version";
      public const string SchemaVersion = "x-ms-schemaversion";
      public const string ServerVersion = "x-ms-serviceversion";
      public const string GatewayVersion = "x-ms-gatewayversion";
      public const string RequestValidationFailure = "x-ms-request-validation-failure";
      public const string WriteRequestTriggerAddressRefresh = "x-ms-write-request-trigger-refresh";
      public const string OcpResourceProviderRegisteredUri = "ocp-resourceprovider-registered-uri";
      public const string RequestId = "x-ms-request-id";
      public const string CorrelationId = "x-ms-correlation-request-id";
      public const string LastStateChangeUtc = "x-ms-last-state-change-utc";
      private static readonly Dictionary<string, string> HeaderValueDictionary = new Dictionary<string, string>();
      public const string ClientRequestId = "x-ms-client-request-id";
      public const string ClientAppId = "x-ms-client-app-id";
      public const string OfferType = "x-ms-offer-type";
      public const string OfferThroughput = "x-ms-offer-throughput";
      public const string BackgroundTaskMaxAllowedThroughputPercent = "x-ms-offer-bg-task-max-allowed-throughput-percent";
      public const string OfferIsRUPerMinuteThroughputEnabled = "x-ms-offer-is-ru-per-minute-throughput-enabled";
      public const string OfferIsAutoScaleEnabled = "x-ms-offer-is-autoscale-enabled";
      public const string OfferAutopilotTier = "x-ms-cosmos-offer-autopilot-tier";
      public const string OfferAutopilotAutoUpgrade = "x-ms-cosmos-offer-autopilot-autoupgrade";
      public const string OfferAutopilotSettings = "x-ms-cosmos-offer-autopilot-settings";
      public const string PopulateCollectionThroughputInfo = "x-ms-documentdb-populatecollectionthroughputinfo";
      public const string IsRUPerGBEnforcementRequest = "x-ms-cosmos-internal-is-ru-per-gb-enforcement-request";
      public const string IsOfferStorageRefreshRequest = "x-ms-cosmos-internal-is-offer-storage-refresh-request";
      public const string IsAutopilotTierEnforcementRequest = "x-ms-cosmos-internal-autopilot-tier-enforcement-request";
      public const string IsServerlessStorageRefreshRequest = "x-ms-cosmos-internal-serverless-offer-storage-refresh-request";
      public const string IsInternalServerlessRequest = "x-ms-cosmos-internal-serverless-request";
      public const string MigrateOfferToManualThroughput = "x-ms-cosmos-migrate-offer-to-manual-throughput";
      public const string MigrateOfferToAutopilot = "x-ms-cosmos-migrate-offer-to-autopilot";
      public const string TruncateMergeLogRequest = "x-ms-cosmos-internal-truncate-merge-log";
      public const string TotalAccountThroughput = "x-ms-cosmos-total-account-throughput";
      public const string IsThroughputCapRequest = "x-ms-cosmos-internal-is-throughputcap-request";
      public const string ThroughputPolicy = "x-ms-cosmos-throughput-policy";
      public const string SourcePartitionThroughputInfo = "x-ms-cosmos-source-partition-throughput-info";
      public const string TargetPartitionThroughputInfo = "x-ms-cosmos-target-partition-throughput-info";
      public const string IncludePhysicalPartitionThroughputInfo = "x-ms-cosmos-include-physical-partition-throughput-info";
      public const string UpdateOfferStateToPending = "x-ms-cosmos-internal-update-offer-state-to-pending";
      public const string OfferReplaceRURedistribution = "x-ms-cosmos-internal-offer-replace-ru-redistribution";
      public const string CollectionIndexTransformationProgress = "x-ms-documentdb-collection-index-transformation-progress";
      public const string CollectionLazyIndexingProgress = "x-ms-documentdb-collection-lazy-indexing-progress";
      public const string IsUpsert = "x-ms-documentdb-is-upsert";
      public const string PartitionKey = "x-ms-documentdb-partitionkey";
      public const string PartitionKeyRangeId = "x-ms-documentdb-partitionkeyrangeid";
      public const string InsertSystemPartitionKey = "x-ms-cosmos-insert-systempartitionkey";
      public const string SupportSpatialLegacyCoordinates = "x-ms-documentdb-supportspatiallegacycoordinates";
      public const string PartitionCount = "x-ms-documentdb-partitioncount";
      public const string FilterBySchemaResourceId = "x-ms-documentdb-filterby-schema-rid";
      public const string UsePolygonsSmallerThanAHemisphere = "x-ms-documentdb-usepolygonssmallerthanahemisphere";
      public const string GatewaySignature = "x-ms-gateway-signature";
      public const string MtlsSignature = "x-ms-gateway-use-mtls";
      public const string UseGatewaySignature = "x-ms-use-gateway-signature";
      public const string ContinuationToken = "x-ms-continuationtoken";
      public const string PopulateRestoreStatus = "x-ms-cosmosdb-populaterestorestatus";
      public const string PopulateQuotaInfo = "x-ms-documentdb-populatequotainfo";
      public const string PopulateResourceCount = "x-ms-documentdb-populateresourcecount";
      public const string PopulatePartitionStatistics = "x-ms-documentdb-populatepartitionstatistics";
      public const string PopulateUniqueIndexReIndexProgress = "x-ms-cosmosdb-populateuniqueindexreindexprogress";
      public const string XPRole = "x-ms-xp-role";
      public const string DisableRUPerMinuteUsage = "x-ms-documentdb-disable-ru-per-minute-usage";
      public const string IsRUPerMinuteUsed = "x-ms-documentdb-is-ru-per-minute-used";
      public const string CollectionRemoteStorageSecurityIdentifier = "x-ms-collection-security-identifier";
      public const string RemainingTimeInMsOnClientRequest = "x-ms-remaining-time-in-ms-on-client";
      public const string ClientRetryAttemptCount = "x-ms-client-retry-attempt-count";
      public const string SourceDatabaseId = "x-ms-source-database-Id";
      public const string SourceCollectionId = "x-ms-source-collection-Id";
      public const string RestorePointInTime = "x-ms-restore-point-in-time";
      public const string SystemRestoreOperation = "x-ms-cosmos-internal-system-restore-operation";
      public const string TargetLsn = "x-ms-target-lsn";
      public const string TargetGlobalCommittedLsn = "x-ms-target-global-committed-lsn";
      public const string TransportRequestID = "x-ms-transport-request-id";
      public const string DisableRntbdChannel = "x-ms-disable-rntbd-channel";
      public const string RestoreMetadataFilter = "x-ms-restore-metadata-filter";
      public const string IsReadOnlyScript = "x-ms-is-readonly-script";
      public const string IsAutoScaleRequest = "x-ms-is-auto-scale";
      public const string AllowTentativeWrites = "x-ms-cosmos-allow-tentative-writes";
      public const string IncludeTentativeWrites = "x-ms-cosmos-include-tentative-writes";
      public const string CanOfferReplaceComplete = "x-ms-can-offer-replace-complete";
      public const string IsOfferReplacePending = "x-ms-offer-replace-pending";
      public const string IgnoreSystemLoweringMaxThroughput = "x-ms-cosmos-internal-ignore-system-lowering-max-throughput";
      public const string UpdateMaxThroughputEverProvisioned = "x-ms-cosmos-internal-update-max-throughput-ever-provisioned";
      public const string GetAllPartitionKeyStatistics = "x-ms-cosmos-internal-get-all-partition-key-stats";
      public const string EnumerationDirection = "x-ms-enumeration-direction";
      public const string ReadFeedKeyType = "x-ms-read-key-type";
      public const string StartId = "x-ms-start-id";
      public const string EndId = "x-ms-end-id";
      public const string StartEpk = "x-ms-start-epk";
      public const string EndEpk = "x-ms-end-epk";
      public const string ApiType = "x-ms-cosmos-apitype";
      public const string MergeStaticId = "x-ms-cosmos-merge-static-id";
      public const string IsClientEncrypted = "x-ms-cosmos-is-client-encrypted";
      public const string SystemDocumentType = "x-ms-cosmos-systemdocument-type";
      public const string SDKSupportedCapabilities = "x-ms-cosmos-sdk-supportedcapabilities";
      public const string IsQueryPlanRequest = "x-ms-cosmos-is-query-plan-request";
      public const string SupportedQueryFeatures = "x-ms-cosmos-supported-query-features";
      public const string QueryVersion = "x-ms-cosmos-query-version";
      public const string PreserveFullContent = "x-ms-cosmos-preserve-full-content";
      public const string ForceSideBySideIndexMigration = "x-ms-cosmos-force-sidebyside-indexmigration";
      public const string MaxPollingIntervalMilliseconds = "x-ms-cosmos-max-polling-interval";
      public const string ChangeFeedStartFullFidelityIfNoneMatch = "x-ms-cosmos-start-full-fidelity-if-none-match";
      public const string IsMaterializedViewBuild = "x-ms-cosmos-internal-is-materialized-view-build";
      public const string UseArchivalPartition = "x-ms-cosmos-use-archival-partition";
      public const string ChangeFeedWireFormatVersion = "x-ms-cosmos-changefeed-wire-format-version";
      public const string ChangeFeedInfo = "x-ms-cosmos-changefeed-info";
      public const string IncludeSnapshotDirectories = "x-ms-cosmos-include-snapshot-directories";
      public const string ResourceIdentityUrl = "x-ms-identity-url";
      public const string ResourceIdentityPrincipalId = "x-ms-identity-principal-id";
      public const string ResourceClientTenantId = "x-ms-client-tenant-id";
      public const string ResourceHomeTenantId = "x-ms-home-tenant-id";
      internal const string AllowRequestWithoutInstanceId = "x-ms-cosmos-allow-without-instance-id";
      public const string RequestHopCount = "x-ms-gateway-hop-count";
      public const string BackendRequestDurationMilliseconds = "x-ms-request-duration-ms";
      public const string ConfirmedStoreChecksum = "x-ms-cosmos-replica-confirmed-checksum";
      public const string TentativeStoreChecksum = "x-ms-cosmos-replica-tentative-checksum";
      public const string PendingPKDelete = "x-ms-cosmos-is-partition-key-delete-pending";
      public const string AadAppliedRoleAssignmentId = "x-ms-aad-applied-role-assignment";
      public const string AppliedPolicyElementId = "x-ms-applied-policy-element";
      public const string CollectionUniqueIndexReIndexProgress = "x-ms-cosmos-collection-unique-index-reindex-progress";
      public const string CollectionTruncate = "x-ms-cosmos-collection-truncate";
      public const string BuilderClientIdentifier = "x-ms-cosmos-builder-client-identifier";
      public const string ListFeedThrottledErrorCode = "x-ms-errorcode";
      public const string PopulateAnalyticalMigrationProgress = "x-ms-cosmos-populate-analytical-migration-progress";
      public const string PopulateOldestActiveSchema = "x-ms-cosmos-populate-oldest-active-schema";
      public const string AnalyticalMigrationProgress = "x-ms-cosmos-analytical-migration-progress";
      public const string PopulateByokEncryptionProgress = "x-ms-cosmos-populate-byok-encryption-progress";
      public const string ByokEncryptionProgress = "x-ms-cosmos-byok-encryption-progress";
      public const string DedicatedGatewayPerRequestCacheStaleness = "x-ms-dedicatedgateway-max-age";
      public const string FromDedicatedGateway = "x-ms-from-dedicated-gateway";
      public const string DatabaseRid = "x-ms-cosmos-database-rid";
      public const string ShouldReturnCurrentServerDateTime = "x-ms-should-return-current-server-datetime";
      public const string NoRetryOn449StatusCode = "x-ms-noretry-449";
      public const string IsRequestOriginRP = "x-ms-is-request-origin-rp";
      public const string RbacUserId = "x-ms-rbac-user-id";
      public const string RbacAction = "x-ms-rbac-action";
      public const string RbacResource = "x-ms-rbac-resource";
      public const string DatabaseAccountName = "x-ms-databaseaccount-name";
      public const string EnvironmentName = "x-ms-environment-name";
      public const string ClientPrincipalName = "x-ms-client-principal-name";
      public const string IsRecreate = "x-ms-cosmos-internal-is-recreate";

      static HttpHeaders()
      {
        HttpConstants.HttpHeaders.HeaderValueDictionary.Add("X-HTTP-Method", "X-HTTP-Method");
        HttpConstants.HttpHeaders.HeaderValueDictionary.Add(nameof (Slug), nameof (Slug));
        HttpConstants.HttpHeaders.HeaderValueDictionary.Add(HttpRequestHeader.ContentType.ToString(), "Content-Type");
        HttpConstants.HttpHeaders.HeaderValueDictionary.Add(HttpRequestHeader.LastModified.ToString(), "Last-Modified");
        HttpConstants.HttpHeaders.HeaderValueDictionary.Add(HttpRequestHeader.ContentEncoding.ToString(), "Content-Encoding");
        HttpConstants.HttpHeaders.HeaderValueDictionary.Add(HttpRequestHeader.UserAgent.ToString(), "User-Agent");
        HttpConstants.HttpHeaders.HeaderValueDictionary.Add(HttpRequestHeader.IfModifiedSince.ToString(), "If-Modified-Since");
        HttpConstants.HttpHeaders.HeaderValueDictionary.Add(HttpRequestHeader.IfMatch.ToString(), "If-Match");
        HttpConstants.HttpHeaders.HeaderValueDictionary.Add(HttpRequestHeader.IfNoneMatch.ToString(), "If-None-Match");
        HttpConstants.HttpHeaders.HeaderValueDictionary.Add(HttpRequestHeader.ContentLength.ToString(), "Content-Length");
        HttpConstants.HttpHeaders.HeaderValueDictionary.Add(HttpRequestHeader.AcceptEncoding.ToString(), "Accept-Encoding");
        HttpConstants.HttpHeaders.HeaderValueDictionary.Add(HttpRequestHeader.KeepAlive.ToString(), "Keep-Alive");
        HttpConstants.HttpHeaders.HeaderValueDictionary.Add(HttpRequestHeader.CacheControl.ToString(), "Cache-Control");
        HttpConstants.HttpHeaders.HeaderValueDictionary.Add(HttpRequestHeader.TransferEncoding.ToString(), "Transfer-Encoding");
        HttpConstants.HttpHeaders.HeaderValueDictionary.Add(HttpRequestHeader.ContentLanguage.ToString(), "Content-Language");
        HttpConstants.HttpHeaders.HeaderValueDictionary.Add(HttpRequestHeader.ContentLocation.ToString(), "Content-Location");
        HttpConstants.HttpHeaders.HeaderValueDictionary.Add(HttpRequestHeader.ContentMd5.ToString(), "Content-Md5");
        HttpConstants.HttpHeaders.HeaderValueDictionary.Add(HttpRequestHeader.ContentRange.ToString(), "Content-Range");
        HttpConstants.HttpHeaders.HeaderValueDictionary.Add(HttpRequestHeader.AcceptCharset.ToString(), "Accept-Charset");
        HttpConstants.HttpHeaders.HeaderValueDictionary.Add(HttpRequestHeader.AcceptLanguage.ToString(), "Accept-Language");
        HttpConstants.HttpHeaders.HeaderValueDictionary.Add(HttpRequestHeader.IfRange.ToString(), "If-Range");
        HttpConstants.HttpHeaders.HeaderValueDictionary.Add(HttpRequestHeader.IfUnmodifiedSince.ToString(), "If-Unmodified-Since");
        HttpConstants.HttpHeaders.HeaderValueDictionary.Add(HttpRequestHeader.MaxForwards.ToString(), "Max-Forwards");
        HttpConstants.HttpHeaders.HeaderValueDictionary.Add(HttpRequestHeader.ProxyAuthorization.ToString(), "Proxy-Authorization");
        HttpConstants.HttpHeaders.HeaderValueDictionary.Add(HttpResponseHeader.AcceptRanges.ToString(), "Accept-Ranges");
        HttpConstants.HttpHeaders.HeaderValueDictionary.Add(HttpResponseHeader.ProxyAuthenticate.ToString(), "Proxy-Authenticate");
        HttpConstants.HttpHeaders.HeaderValueDictionary.Add(HttpResponseHeader.RetryAfter.ToString(), "Retry-After");
        HttpConstants.HttpHeaders.HeaderValueDictionary.Add(HttpResponseHeader.SetCookie.ToString(), "Set-Cookie");
        HttpConstants.HttpHeaders.HeaderValueDictionary.Add(HttpResponseHeader.WwwAuthenticate.ToString(), "Www-Authenticate");
      }

      public static string GetValue(object requestHeader)
      {
        string str = (string) null;
        if (!HttpConstants.HttpHeaders.HeaderValueDictionary.TryGetValue(requestHeader.ToString(), out str))
          str = requestHeader.ToString();
        return str;
      }
    }

    public static class HttpHeaderPreferenceTokens
    {
      public const string PreferUnfilteredQueryResponse = "PreferUnfilteredQueryResponse";
    }

    public static class HttpStatusDescriptions
    {
      public const string Accepted = "Accepted";
      public const string Conflict = "Conflict";
      public const string OK = "Ok";
      public const string PreconditionFailed = "Precondition Failed";
      public const string NotModified = "Not Modified";
      public const string NotFound = "Not Found";
      public const string BadGateway = "Bad Gateway";
      public const string BadRequest = "Bad Request";
      public const string InternalServerError = "Internal Server Error";
      public const string MethodNotAllowed = "MethodNotAllowed";
      public const string NotAcceptable = "Not Acceptable";
      public const string NoContent = "No Content";
      public const string Created = "Created";
      public const string MultiStatus = "Multi-Status";
      public const string UnsupportedMediaType = "Unsupported Media Type";
      public const string LengthRequired = "Length Required";
      public const string ServiceUnavailable = "Service Unavailable";
      public const string RequestEntityTooLarge = "Request Entity Too Large";
      public const string Unauthorized = "Unauthorized";
      public const string Forbidden = "Forbidden";
      public const string Gone = "Gone";
      public const string RequestTimeout = "Request timed out";
      public const string GatewayTimeout = "Gateway timed out";
      public const string TooManyRequests = "Too Many Requests";
      public const string RetryWith = "Retry the request";
      public const string InvalidPartition = "InvalidPartition";
      public const string PartitionMigrating = "Partition is migrating";
      public const string Schema = "Schema";
      public const string Locked = "Locked";
      public const string FailedDependency = "Failed Dependency";
      public const string ConnectionIsBusy = "Connection Is Busy";
      public const string DatabaseAccountNotFound = "Database Account Not Found";
      public const string ThrottledResponse = "List feed results exceeded limit";
    }

    public static class QueryStrings
    {
      public const string Filter = "$filter";
      public const string PartitionKeyRangeIds = "$partitionKeyRangeIds";
      public const string GenerateId = "$generateFor";
      public const string GenerateIdBatchSize = "$batchSize";
      public const string GetChildResourcePartitions = "$getChildResourcePartitions";
      public const string Url = "$resolveFor";
      public const string RootIndex = "$rootIndex";
      public const string Query = "query";
      public const string SQLQueryType = "sql";
      public const string GoalState = "goalstate";
      public const string ContentView = "contentview";
      public const string Generic = "generic";
      public const string DsmsResourceUri = "resourceUri";
      public const string DsmsResourceVersion = "version";
      public const string APIVersion = "api-version";
    }

    public static class CookieHeaders
    {
      public const string SessionToken = "x-ms-session-token";
    }

    public static class Versions
    {
      public static string v2014_08_21 = "2014-08-21";
      public static string v2015_04_08 = "2015-04-08";
      public static string v2015_06_03 = "2015-06-03";
      public static string v2015_08_06 = "2015-08-06";
      public static string v2015_12_16 = "2015-12-16";
      public static string v2016_05_30 = "2016-05-30";
      public static string v2016_07_11 = "2016-07-11";
      public static string v2016_11_14 = "2016-11-14";
      public static string v2017_01_19 = "2017-01-19";
      public static string v2017_02_22 = "2017-02-22";
      public static string v2017_05_03 = "2017-05-03";
      public static string v2017_11_15 = "2017-11-15";
      public static string v2018_06_18 = "2018-06-18";
      public static string v2018_08_31 = "2018-08-31";
      public static string v2018_09_17 = "2018-09-17";
      public static string v2018_12_31 = "2018-12-31";
      public static string v2019_10_14 = "2019-10-14";
      public static string v2020_07_15 = "2020-07-15";
      public static string v2020_11_05 = "2020-11-05";
      public static string CurrentVersion = HttpConstants.Versions.v2018_09_17;
      public static string[] SupportedRuntimeAPIVersions = new string[19]
      {
        HttpConstants.Versions.v2020_11_05,
        HttpConstants.Versions.v2020_07_15,
        HttpConstants.Versions.v2019_10_14,
        HttpConstants.Versions.v2018_12_31,
        HttpConstants.Versions.v2018_09_17,
        HttpConstants.Versions.v2018_08_31,
        HttpConstants.Versions.v2018_06_18,
        HttpConstants.Versions.v2017_11_15,
        HttpConstants.Versions.v2017_05_03,
        HttpConstants.Versions.v2017_02_22,
        HttpConstants.Versions.v2017_01_19,
        HttpConstants.Versions.v2016_11_14,
        HttpConstants.Versions.v2016_07_11,
        HttpConstants.Versions.v2016_05_30,
        HttpConstants.Versions.v2015_12_16,
        HttpConstants.Versions.v2015_08_06,
        HttpConstants.Versions.v2015_06_03,
        HttpConstants.Versions.v2015_04_08,
        HttpConstants.Versions.v2014_08_21
      };
      public static byte[] CurrentVersionUTF8 = Encoding.UTF8.GetBytes(HttpConstants.Versions.CurrentVersion);
    }

    public static class VersionDates
    {
      public static readonly DateTime v2020_11_05 = VersionUtility.ParseNonPreviewDateTimeExact(HttpConstants.Versions.v2020_11_05);
      public static readonly DateTime v2020_07_15 = VersionUtility.ParseNonPreviewDateTimeExact(HttpConstants.Versions.v2020_07_15);
      public static readonly DateTime v2019_10_14 = VersionUtility.ParseNonPreviewDateTimeExact(HttpConstants.Versions.v2019_10_14);
      public static readonly DateTime v2018_12_31 = VersionUtility.ParseNonPreviewDateTimeExact(HttpConstants.Versions.v2018_12_31);
      public static readonly DateTime v2018_09_17 = VersionUtility.ParseNonPreviewDateTimeExact(HttpConstants.Versions.v2018_09_17);
      public static readonly DateTime v2018_08_31 = VersionUtility.ParseNonPreviewDateTimeExact(HttpConstants.Versions.v2018_08_31);
      public static readonly DateTime v2018_06_18 = VersionUtility.ParseNonPreviewDateTimeExact(HttpConstants.Versions.v2018_06_18);
      public static readonly DateTime v2017_11_15 = VersionUtility.ParseNonPreviewDateTimeExact(HttpConstants.Versions.v2017_11_15);
      public static readonly DateTime v2017_05_03 = VersionUtility.ParseNonPreviewDateTimeExact(HttpConstants.Versions.v2017_05_03);
      public static readonly DateTime v2017_02_22 = VersionUtility.ParseNonPreviewDateTimeExact(HttpConstants.Versions.v2017_02_22);
      public static readonly DateTime v2017_01_19 = VersionUtility.ParseNonPreviewDateTimeExact(HttpConstants.Versions.v2017_01_19);
      public static readonly DateTime v2016_11_14 = VersionUtility.ParseNonPreviewDateTimeExact(HttpConstants.Versions.v2016_11_14);
      public static readonly DateTime v2016_07_11 = VersionUtility.ParseNonPreviewDateTimeExact(HttpConstants.Versions.v2016_07_11);
      public static readonly DateTime v2016_05_30 = VersionUtility.ParseNonPreviewDateTimeExact(HttpConstants.Versions.v2016_05_30);
      public static readonly DateTime v2015_12_16 = VersionUtility.ParseNonPreviewDateTimeExact(HttpConstants.Versions.v2015_12_16);
      public static readonly DateTime v2015_08_06 = VersionUtility.ParseNonPreviewDateTimeExact(HttpConstants.Versions.v2015_08_06);
      public static readonly DateTime v2015_06_03 = VersionUtility.ParseNonPreviewDateTimeExact(HttpConstants.Versions.v2015_06_03);
      public static readonly DateTime v2015_04_08 = VersionUtility.ParseNonPreviewDateTimeExact(HttpConstants.Versions.v2015_04_08);
      public static readonly DateTime v2014_08_21 = VersionUtility.ParseNonPreviewDateTimeExact(HttpConstants.Versions.v2014_08_21);
    }

    public static class Delimiters
    {
      public const string ClientContinuationDelimiter = "!!";
      public const string ClientContinuationFormat = "{0}!!{1}";
      public static string[] ClientContinuationDelimiterArray = new string[1]
      {
        "!!"
      };
    }

    public static class HttpListenerErrorCodes
    {
      public const int ERROR_OPERATION_ABORTED = 995;
      public const int ERROR_CONNECTION_INVALID = 1229;
      public const int ERROR_CONNECTION_RESET = 10054;
    }

    public static class UserAgents
    {
      public static string PortalUserAgent = "Azure Portal";
    }

    public static class HttpContextProperties
    {
      public const string SubscriptionId = "SubscriptionId";
      public const string OperationId = "OperationId";
      public const string OperationName = "OperationName";
      public const string ResourceName = "ResourceName";
      public const string LocationName = "LocationName";
      public const string FederationName = "FederationName";
      public const string DatabaseAccountName = "DatabaseAccountName";
      public const string GlobalDatabaseAccountName = "GlobalDatabaseAccountName";
      public const string RegionalDatabaseAccountName = "RegionalDatabaseAccountName";
      public const string CollectionResourceId = "CollectionResourceId";
      public const string DatabaseResourceId = "DatabaseResourceId";
      public const string OperationKind = "OperationKind";
      public const string ResourceTokenPermissionId = "ResourceTokenPermissionId";
      public const string ResourceTokenPermissionMode = "ResourceTokenPermissionMode";
      public const string DatabaseName = "DatabaseName";
      public const string CollectionName = "CollectionName";
      public const string EnabledDiagLogsForCustomer = "EnabledDiagLogsForCustomer";
      public const string EnabledDiagLogsForAtp = "EnabledDiagLogsForAtp";
      public const string IsAllowedWithoutInstanceId = "IsAllowedWithoutInstanceId";
    }

    public static class HttpHeaderValues
    {
      public const string PreferReturnContent = "return-content";
      public const string PreferReturnNoContent = "return-no-content";
      public const string PreferReturnRepresentation = "return=representation";
      public const string PreferReturnMinimal = "return=minimal";
    }

    public static class A_IMHeaderValues
    {
      public const string IncrementalFeed = "Incremental Feed";
      public const string FullFidelityFeed = "Full-Fidelity Feed";
      public const string IncrementalFullFidelityFeed = "Incremental Full-Fidelity Feed";
    }

    public static class IfNoneMatchHeaderValues
    {
      public const string GetChangeFeedInfo = "get-changefeed-info";
    }

    public static class Paths
    {
      public const string Sprocs = "sprocs";
    }
  }
}
