// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Rntbd.TransportSerialization
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

using Microsoft.Azure.Cosmos.Core.Trace;
using Microsoft.Azure.Cosmos.Rntbd;
using Microsoft.Azure.Documents.Collections;
using System;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;

namespace Microsoft.Azure.Documents.Rntbd
{
  internal static class TransportSerialization
  {
    internal static readonly char[] UrlTrim = new char[1]
    {
      '/'
    };

    internal static TransportSerialization.SerializedRequest BuildRequest(
      DocumentServiceRequest request,
      string replicaPath,
      ResourceOperation resourceOperation,
      Guid activityId,
      BufferProvider bufferProvider,
      out int headerSize,
      out int? bodySize)
    {
      RntbdConstants.RntbdOperationType rntbdOperationType = TransportSerialization.GetRntbdOperationType(resourceOperation.operationType);
      RntbdConstants.RntbdResourceType rntbdResourceType = TransportSerialization.GetRntbdResourceType(resourceOperation.resourceType);
      using (RntbdConstants.RntbdEntityPool<RntbdConstants.Request, RntbdConstants.RequestIdentifiers>.EntityOwner entityOwner = RntbdConstants.RntbdEntityPool<RntbdConstants.Request, RntbdConstants.RequestIdentifiers>.Instance.Get())
      {
        RntbdConstants.Request entity = entityOwner.Entity;
        entity.replicaPath.value.valueBytes = BytesSerializer.GetBytesForString(replicaPath, entity);
        entity.replicaPath.isPresent = true;
        if (!(request.Headers is RequestNameValueCollection requestHeaders))
          requestHeaders = new RequestNameValueCollection(request.Headers);
        TransportSerialization.AddResourceIdOrPathHeaders(request, entity);
        TransportSerialization.AddDateHeader(requestHeaders, entity);
        TransportSerialization.AddContinuation(requestHeaders, entity);
        TransportSerialization.AddMatchHeader(requestHeaders, rntbdOperationType, entity);
        TransportSerialization.AddIfModifiedSinceHeader(requestHeaders, entity);
        TransportSerialization.AddA_IMHeader(requestHeaders, entity);
        TransportSerialization.AddIndexingDirectiveHeader(requestHeaders, entity);
        TransportSerialization.AddMigrateCollectionDirectiveHeader(requestHeaders, entity);
        TransportSerialization.AddConsistencyLevelHeader(requestHeaders, entity);
        TransportSerialization.AddIsFanout(requestHeaders, entity);
        TransportSerialization.AddEntityId(request, entity);
        TransportSerialization.AddAllowScanOnQuery(requestHeaders, entity);
        TransportSerialization.AddEmitVerboseTracesInQuery(requestHeaders, entity);
        TransportSerialization.AddCanCharge(requestHeaders, entity);
        TransportSerialization.AddCanThrottle(requestHeaders, entity);
        TransportSerialization.AddProfileRequest(requestHeaders, entity);
        TransportSerialization.AddEnableLowPrecisionOrderBy(requestHeaders, entity);
        TransportSerialization.AddPageSize(requestHeaders, entity);
        TransportSerialization.AddSupportSpatialLegacyCoordinates(requestHeaders, entity);
        TransportSerialization.AddUsePolygonsSmallerThanAHemisphere(requestHeaders, entity);
        TransportSerialization.AddEnableLogging(requestHeaders, entity);
        TransportSerialization.AddPopulateQuotaInfo(requestHeaders, entity);
        TransportSerialization.AddPopulateResourceCount(requestHeaders, entity);
        TransportSerialization.AddDisableRUPerMinuteUsage(requestHeaders, entity);
        TransportSerialization.AddPopulateQueryMetrics(requestHeaders, entity);
        TransportSerialization.AddPopulateQueryMetricsIndexUtilization(requestHeaders, entity);
        TransportSerialization.AddQueryForceScan(requestHeaders, entity);
        TransportSerialization.AddResponseContinuationTokenLimitInKb(requestHeaders, entity);
        TransportSerialization.AddPopulatePartitionStatistics(requestHeaders, entity);
        TransportSerialization.AddRemoteStorageType(requestHeaders, entity);
        TransportSerialization.AddCollectionRemoteStorageSecurityIdentifier(requestHeaders, entity);
        TransportSerialization.AddCollectionChildResourceNameLimitInBytes(requestHeaders, entity);
        TransportSerialization.AddCollectionChildResourceContentLengthLimitInKB(requestHeaders, entity);
        TransportSerialization.AddUniqueIndexNameEncodingMode(requestHeaders, entity);
        TransportSerialization.AddUniqueIndexReIndexingState(requestHeaders, entity);
        TransportSerialization.AddCorrelatedActivityId(requestHeaders, entity);
        TransportSerialization.AddPopulateCollectionThroughputInfo(requestHeaders, entity);
        TransportSerialization.AddShareThroughput(requestHeaders, entity);
        TransportSerialization.AddIsReadOnlyScript(requestHeaders, entity);
        TransportSerialization.AddCanOfferReplaceComplete(requestHeaders, entity);
        TransportSerialization.AddIgnoreSystemLoweringMaxThroughput(requestHeaders, entity);
        TransportSerialization.AddExcludeSystemProperties(requestHeaders, entity);
        TransportSerialization.AddEnumerationDirection(request, requestHeaders, entity);
        TransportSerialization.AddFanoutOperationStateHeader(requestHeaders, entity);
        TransportSerialization.AddStartAndEndKeys(request, requestHeaders, entity);
        TransportSerialization.AddContentSerializationFormat(requestHeaders, entity);
        TransportSerialization.AddIsUserRequest(requestHeaders, entity);
        TransportSerialization.AddPreserveFullContent(requestHeaders, entity);
        TransportSerialization.AddIsRUPerGBEnforcementRequest(requestHeaders, entity);
        TransportSerialization.AddIsOfferStorageRefreshRequest(requestHeaders, entity);
        TransportSerialization.AddGetAllPartitionKeyStatistics(requestHeaders, entity);
        TransportSerialization.AddForceSideBySideIndexMigration(requestHeaders, entity);
        TransportSerialization.AddIsMigrateOfferToManualThroughputRequest(requestHeaders, entity);
        TransportSerialization.AddIsMigrateOfferToAutopilotRequest(requestHeaders, entity);
        TransportSerialization.AddSystemDocumentTypeHeader(requestHeaders, entity);
        TransportSerialization.AddTransactionMetaData(request, entity);
        TransportSerialization.AddTransactionCompletionFlag(request, entity);
        TransportSerialization.AddResourceTypes(requestHeaders, entity);
        TransportSerialization.AddUpdateMaxthroughputEverProvisioned(requestHeaders, entity);
        TransportSerialization.AddUseSystemBudget(requestHeaders, entity);
        TransportSerialization.AddTruncateMergeLogRequest(requestHeaders, entity);
        TransportSerialization.AddRetriableWriteRequestMetadata(request, entity);
        TransportSerialization.AddRequestedCollectionType(requestHeaders, entity);
        TransportSerialization.AddIsThroughputCapRequest(requestHeaders, entity);
        TransportSerialization.AddUpdateOfferStateToPending(requestHeaders, entity);
        TransportSerialization.AddIsInternalServerlessRequest(requestHeaders, entity);
        TransportSerialization.AddOfferReplaceRURedistribution(requestHeaders, entity);
        TransportSerialization.FillTokenFromHeader(request, "authorization", requestHeaders.Authorization, entity.authorizationToken, entity);
        TransportSerialization.FillTokenFromHeader(request, "x-ms-session-token", requestHeaders.SessionToken, entity.sessionToken, entity);
        TransportSerialization.FillTokenFromHeader(request, "x-ms-documentdb-pre-trigger-include", requestHeaders.PreTriggerInclude, entity.preTriggerInclude, entity);
        TransportSerialization.FillTokenFromHeader(request, "x-ms-documentdb-pre-trigger-exclude", requestHeaders.PreTriggerExclude, entity.preTriggerExclude, entity);
        TransportSerialization.FillTokenFromHeader(request, "x-ms-documentdb-post-trigger-include", requestHeaders.PostTriggerInclude, entity.postTriggerInclude, entity);
        TransportSerialization.FillTokenFromHeader(request, "x-ms-documentdb-post-trigger-exclude", requestHeaders.PostTriggerExclude, entity.postTriggerExclude, entity);
        TransportSerialization.FillTokenFromHeader(request, "x-ms-documentdb-partitionkey", requestHeaders.PartitionKey, entity.partitionKey, entity);
        TransportSerialization.FillTokenFromHeader(request, "x-ms-documentdb-partitionkeyrangeid", requestHeaders.PartitionKeyRangeId, entity.partitionKeyRangeId, entity);
        TransportSerialization.FillTokenFromHeader(request, "x-ms-documentdb-expiry-seconds", requestHeaders.ResourceTokenExpiry, entity.resourceTokenExpiry, entity);
        TransportSerialization.FillTokenFromHeader(request, "x-ms-documentdb-filterby-schema-rid", requestHeaders.FilterBySchemaResourceId, entity.filterBySchemaRid, entity);
        TransportSerialization.FillTokenFromHeader(request, "x-ms-cosmos-batch-continue-on-error", requestHeaders.ShouldBatchContinueOnError, entity.shouldBatchContinueOnError, entity);
        TransportSerialization.FillTokenFromHeader(request, "x-ms-cosmos-batch-ordered", requestHeaders.IsBatchOrdered, entity.isBatchOrdered, entity);
        TransportSerialization.FillTokenFromHeader(request, "x-ms-cosmos-batch-atomic", requestHeaders.IsBatchAtomic, entity.isBatchAtomic, entity);
        TransportSerialization.FillTokenFromHeader(request, "collection-partition-index", requestHeaders.CollectionPartitionIndex, entity.collectionPartitionIndex, entity);
        TransportSerialization.FillTokenFromHeader(request, "collection-service-index", requestHeaders.CollectionServiceIndex, entity.collectionServiceIndex, entity);
        TransportSerialization.FillTokenFromHeader(request, "x-ms-resource-schema-name", requestHeaders.ResourceSchemaName, entity.resourceSchemaName, entity);
        TransportSerialization.FillTokenFromHeader(request, "x-ms-bind-replica", requestHeaders.BindReplicaDirective, entity.bindReplicaDirective, entity);
        TransportSerialization.FillTokenFromHeader(request, "x-ms-primary-master-key", requestHeaders.PrimaryMasterKey, entity.primaryMasterKey, entity);
        TransportSerialization.FillTokenFromHeader(request, "x-ms-secondary-master-key", requestHeaders.SecondaryMasterKey, entity.secondaryMasterKey, entity);
        TransportSerialization.FillTokenFromHeader(request, "x-ms-primary-readonly-key", requestHeaders.PrimaryReadonlyKey, entity.primaryReadonlyKey, entity);
        TransportSerialization.FillTokenFromHeader(request, "x-ms-secondary-readonly-key", requestHeaders.SecondaryReadonlyKey, entity.secondaryReadonlyKey, entity);
        TransportSerialization.FillTokenFromHeader(request, "x-ms-documentdb-partitioncount", requestHeaders.PartitionCount, entity.partitionCount, entity);
        TransportSerialization.FillTokenFromHeader(request, "x-ms-documentdb-collection-rid", requestHeaders.CollectionRid, entity.collectionRid, entity);
        TransportSerialization.FillTokenFromHeader(request, "x-ms-gateway-signature", requestHeaders.GatewaySignature, entity.gatewaySignature, entity);
        TransportSerialization.FillTokenFromHeader(request, "x-ms-remaining-time-in-ms-on-client", requestHeaders.RemainingTimeInMsOnClientRequest, entity.remainingTimeInMsOnClientRequest, entity);
        TransportSerialization.FillTokenFromHeader(request, "x-ms-client-retry-attempt-count", requestHeaders.ClientRetryAttemptCount, entity.clientRetryAttemptCount, entity);
        TransportSerialization.FillTokenFromHeader(request, "x-ms-target-lsn", requestHeaders.TargetLsn, entity.targetLsn, entity);
        TransportSerialization.FillTokenFromHeader(request, "x-ms-target-global-committed-lsn", requestHeaders.TargetGlobalCommittedLsn, entity.targetGlobalCommittedLsn, entity);
        TransportSerialization.FillTokenFromHeader(request, "x-ms-transport-request-id", requestHeaders.TransportRequestID, entity.transportRequestID, entity);
        TransportSerialization.FillTokenFromHeader(request, "x-ms-restore-metadata-filter", requestHeaders.RestoreMetadataFilter, entity.restoreMetadataFilter, entity);
        TransportSerialization.FillTokenFromHeader(request, "x-ms-restore-params", requestHeaders.RestoreParams, entity.restoreParams, entity);
        TransportSerialization.FillTokenFromHeader(request, "x-ms-partition-resource-filter", requestHeaders.PartitionResourceFilter, entity.partitionResourceFilter, entity);
        TransportSerialization.FillTokenFromHeader(request, "x-ms-enable-dynamic-rid-range-allocation", requestHeaders.EnableDynamicRidRangeAllocation, entity.enableDynamicRidRangeAllocation, entity);
        TransportSerialization.FillTokenFromHeader(request, "x-ms-schema-owner-rid", requestHeaders.SchemaOwnerRid, entity.schemaOwnerRid, entity);
        TransportSerialization.FillTokenFromHeader(request, "x-ms-schema-hash", requestHeaders.SchemaHash, entity.schemaHash, entity);
        TransportSerialization.FillTokenFromHeader(request, "x-ms-schema-id", requestHeaders.SchemaId, entity.schemaId, entity);
        TransportSerialization.FillTokenFromHeader(request, "x-ms-cosmos-is-client-encrypted", requestHeaders.IsClientEncrypted, entity.isClientEncrypted, entity);
        TransportSerialization.AddReturnPreferenceIfPresent(requestHeaders, entity);
        TransportSerialization.AddBinaryIdIfPresent(request, entity);
        TransportSerialization.FillTokenFromHeader(request, "x-ms-time-to-live-in-seconds", requestHeaders.TimeToLiveInSeconds, entity.timeToLiveInSeconds, entity);
        TransportSerialization.AddEffectivePartitionKeyIfPresent(request, entity);
        TransportSerialization.FillTokenFromHeader(request, "x-ms-binary-passthrough-request", requestHeaders.BinaryPassthroughRequest, entity.binaryPassthroughRequest, entity);
        TransportSerialization.FillTokenFromHeader(request, "x-ms-cosmos-allow-tentative-writes", requestHeaders.AllowTentativeWrites, entity.allowTentativeWrites, entity);
        TransportSerialization.FillTokenFromHeader(request, "x-ms-cosmos-include-tentative-writes", requestHeaders.IncludeTentativeWrites, entity.includeTentativeWrites, entity);
        TransportSerialization.AddMergeStaticIdIfPresent(request, entity);
        TransportSerialization.AddMergeStaticIdIfPresent(request, entity);
        TransportSerialization.FillTokenFromHeader(request, "x-ms-cosmos-max-polling-interval", requestHeaders.MaxPollingIntervalMilliseconds, entity.maxPollingIntervalMilliseconds, entity);
        TransportSerialization.FillTokenFromHeader(request, "x-ms-cosmos-populate-logstoreinfo", requestHeaders.PopulateLogStoreInfo, entity.populateLogStoreInfo, entity);
        TransportSerialization.FillTokenFromHeader(request, "x-ms-cosmos-internal-merge-checkpoint-glsn", requestHeaders.MergeCheckPointGLSN, entity.mergeCheckpointGlsnKeyName, entity);
        TransportSerialization.FillTokenFromHeader(request, "x-ms-cosmos-internal-populate-unflushed-merge-entry-count", requestHeaders.PopulateUnflushedMergeEntryCount, entity.populateUnflushedMergeEntryCount, entity);
        TransportSerialization.FillTokenFromHeader(request, "x-ms-cosmos-add-resource-properties-to-response", requestHeaders.AddResourcePropertiesToResponse, entity.addResourcePropertiesToResponse, entity);
        TransportSerialization.FillTokenFromHeader(request, "x-ms-cosmos-internal-system-restore-operation", requestHeaders.SystemRestoreOperation, entity.systemRestoreOperation, entity);
        TransportSerialization.FillTokenFromHeader(request, "x-ms-cosmos-start-full-fidelity-if-none-match", requestHeaders.ChangeFeedStartFullFidelityIfNoneMatch, entity.changeFeedStartFullFidelityIfNoneMatch, entity);
        TransportSerialization.FillTokenFromHeader(request, "x-ms-cosmos-skip-refresh-databaseaccountconfig", requestHeaders.SkipRefreshDatabaseAccountConfigs, entity.skipRefreshDatabaseAccountConfigs, entity);
        TransportSerialization.FillTokenFromHeader(request, "x-ms-cosmos-intended-collection-rid", requestHeaders.IntendedCollectionRid, entity.intendedCollectionRid, entity);
        TransportSerialization.FillTokenFromHeader(request, "x-ms-cosmos-use-archival-partition", requestHeaders.UseArchivalPartition, entity.useArchivalPartition, entity);
        TransportSerialization.FillTokenFromHeader(request, "x-ms-cosmos-collection-truncate", requestHeaders.CollectionTruncate, entity.collectionTruncate, entity);
        TransportSerialization.FillTokenFromHeader(request, "x-ms-cosmos-sdk-supportedcapabilities", requestHeaders.SDKSupportedCapabilities, entity.sdkSupportedCapabilities, entity);
        TransportSerialization.FillTokenFromHeader(request, "x-ms-cosmosdb-populateuniqueindexreindexprogress", requestHeaders.PopulateUniqueIndexReIndexProgress, entity.populateUniqueIndexReIndexProgress, entity);
        TransportSerialization.FillTokenFromHeader(request, "x-ms-cosmos-internal-is-materialized-view-build", requestHeaders.IsMaterializedViewBuild, entity.isMaterializedViewBuild, entity);
        TransportSerialization.FillTokenFromHeader(request, "x-ms-cosmos-builder-client-identifier", requestHeaders.BuilderClientIdentifier, entity.builderClientIdentifier, entity);
        TransportSerialization.FillTokenFromHeader(request, "x-ms-cosmos-source-collection-if-match", requestHeaders.SourceCollectionIfMatch, entity.sourceCollectionIfMatch, entity);
        TransportSerialization.FillTokenFromHeader(request, "x-ms-cosmos-populate-analytical-migration-progress", requestHeaders.PopulateAnalyticalMigrationProgress, entity.populateAnalyticalMigrationProgress, entity);
        TransportSerialization.FillTokenFromHeader(request, "x-ms-should-return-current-server-datetime", requestHeaders.ShouldReturnCurrentServerDateTime, entity.shouldReturnCurrentServerDateTime, entity);
        TransportSerialization.FillTokenFromHeader(request, "x-ms-rbac-user-id", requestHeaders.RbacUserId, entity.rbacUserId, entity);
        TransportSerialization.FillTokenFromHeader(request, "x-ms-rbac-action", requestHeaders.RbacAction, entity.rbacAction, entity);
        TransportSerialization.FillTokenFromHeader(request, "x-ms-rbac-resource", requestHeaders.RbacResource, entity.rbacResource, entity);
        TransportSerialization.FillTokenFromHeader(request, "x-ms-cosmos-changefeed-wire-format-version", requestHeaders.ChangeFeedWireFormatVersion, entity.changeFeedWireFormatVersion, entity);
        TransportSerialization.FillTokenFromHeader(request, "x-ms-cosmos-populate-byok-encryption-progress", requestHeaders.PopulateByokEncryptionProgress, entity.populateBYOKEncryptionProgress, entity);
        TransportSerialization.FillTokenFromHeader(request, "x-ms-cosmos-use-background-task-budget", requestHeaders.UseUserBackgroundBudget, entity.useUserBackgroundBudget, entity);
        TransportSerialization.FillTokenFromHeader(request, "x-ms-cosmos-include-physical-partition-throughput-info", requestHeaders.IncludePhysicalPartitionThroughputInfo, entity.includePhysicalPartitionThroughputInfo, entity);
        TransportSerialization.FillTokenFromHeader(request, "x-ms-cosmos-populate-oldest-active-schema", requestHeaders.PopulateOldestActiveSchema, entity.populateOldestActiveSchema, entity);
        TransportSerialization.FillTokenFromHeader(request, "x-ms-version", requestHeaders.Version, entity.clientVersion, entity);
        int num1;
        int num2 = num1 = 8 + BytesSerializer.GetSizeOfGuid();
        int num3 = 0;
        bodySize = new int?();
        int num4 = 0;
        CloneableStream requestBody = (CloneableStream) null;
        if (request.CloneableBody != null)
        {
          requestBody = request.CloneableBody.Clone();
          num4 = (int) requestBody.Length;
        }
        if (num4 > 0)
        {
          num3 += 4;
          entity.payloadPresent.value.valueByte = (byte) 1;
          entity.payloadPresent.isPresent = true;
        }
        else
        {
          entity.payloadPresent.value.valueByte = (byte) 0;
          entity.payloadPresent.isPresent = true;
        }
        int num5 = num2 + entity.CalculateLength();
        int num6 = num3 + num5;
        BufferProvider.DisposableBuffer buffer = bufferProvider.GetBuffer(num6);
        BytesSerializer writer = new BytesSerializer(buffer.Buffer.Array, num6);
        writer.Write((uint) num5);
        writer.Write((ushort) rntbdResourceType);
        writer.Write((ushort) rntbdOperationType);
        writer.Write(activityId);
        int num7 = num1;
        int tokensLength;
        entity.SerializeToBinaryWriter(ref writer, out tokensLength);
        int num8 = num7 + tokensLength;
        if (num8 != num5)
        {
          requestBody?.Dispose();
          DefaultTrace.TraceCritical("Bug in RNTBD token serialization. Calculated header size: {0}. Actual header size: {1}", (object) num5, (object) num8);
          throw new InternalServerErrorException();
        }
        if (num4 > 0)
        {
          writer.Write((uint) num4);
          bodySize = new int?(4 + num4);
        }
        headerSize = num5;
        if (headerSize > 131072)
          DefaultTrace.TraceWarning("The request header is large. Header size: {0}. Warning threshold: {1}. RID: {2}. Resource type: {3}. Operation: {4}. Address: {5}", (object) headerSize, (object) 131072, (object) request.ResourceAddress, (object) request.ResourceType, (object) resourceOperation, (object) replicaPath);
        int? nullable = bodySize;
        int num9 = 16777216;
        if (nullable.GetValueOrDefault() > num9 & nullable.HasValue)
          DefaultTrace.TraceWarning("The request body is large. Body size: {0}. Warning threshold: {1}. RID: {2}. Resource type: {3}. Operation: {4}. Address: {5}", (object) bodySize, (object) 16777216, (object) request.ResourceAddress, (object) request.ResourceType, (object) resourceOperation, (object) replicaPath);
        return new TransportSerialization.SerializedRequest(buffer, requestBody);
      }
    }

    internal static byte[] BuildContextRequest(
      Guid activityId,
      UserAgentContainer userAgent,
      RntbdConstants.CallerId callerId,
      bool enableChannelMultiplexing)
    {
      byte[] byteArray = activityId.ToByteArray();
      RntbdConstants.ConnectionContextRequest connectionContextRequest = new RntbdConstants.ConnectionContextRequest();
      connectionContextRequest.protocolVersion.value.valueULong = 1U;
      connectionContextRequest.protocolVersion.isPresent = true;
      connectionContextRequest.clientVersion.value.valueBytes = (ReadOnlyMemory<byte>) HttpConstants.Versions.CurrentVersionUTF8;
      connectionContextRequest.clientVersion.isPresent = true;
      connectionContextRequest.userAgent.value.valueBytes = (ReadOnlyMemory<byte>) userAgent.UserAgentUTF8;
      connectionContextRequest.userAgent.isPresent = true;
      connectionContextRequest.callerId.isPresent = false;
      if (callerId != RntbdConstants.CallerId.Invalid)
      {
        connectionContextRequest.callerId.value.valueByte = (byte) callerId;
        connectionContextRequest.callerId.isPresent = true;
      }
      connectionContextRequest.enableChannelMultiplexing.isPresent = true;
      connectionContextRequest.enableChannelMultiplexing.value.valueByte = enableChannelMultiplexing ? (byte) 1 : (byte) 0;
      int length = 8 + byteArray.Length + connectionContextRequest.CalculateLength();
      byte[] targetByteArray = new byte[length];
      BytesSerializer writer = new BytesSerializer(targetByteArray, length);
      writer.Write(length);
      writer.Write((ushort) 0);
      writer.Write((ushort) 0);
      writer.Write(byteArray);
      connectionContextRequest.SerializeToBinaryWriter(ref writer, out int _);
      return targetByteArray;
    }

    internal static StoreResponse MakeStoreResponse(
      StatusCodes status,
      Guid activityId,
      RntbdConstants.Response response,
      Stream body,
      string serverVersion)
    {
      StoreResponseNameValueCollection nameValueCollection = new StoreResponseNameValueCollection();
      StoreResponse storeResponse = new StoreResponse()
      {
        Headers = (INameValueCollection) nameValueCollection
      };
      nameValueCollection.LastStateChangeUtc = TransportSerialization.GetStringFromRntbdTokenIfPresent(response.lastStateChangeDateTime);
      nameValueCollection.Continuation = TransportSerialization.GetStringFromRntbdTokenIfPresent(response.continuationToken);
      nameValueCollection.ETag = TransportSerialization.GetStringFromRntbdTokenIfPresent(response.eTag);
      nameValueCollection.RetryAfterInMilliseconds = TransportSerialization.GetStringFromRntbdTokenIfPresent(response.retryAfterMilliseconds);
      nameValueCollection.MaxResourceQuota = TransportSerialization.GetStringFromRntbdTokenIfPresent(response.storageMaxResoureQuota);
      nameValueCollection.CurrentResourceQuotaUsage = TransportSerialization.GetStringFromRntbdTokenIfPresent(response.storageResourceQuotaUsage);
      nameValueCollection.CollectionPartitionIndex = TransportSerialization.GetStringFromRntbdTokenIfPresent(response.collectionPartitionIndex);
      nameValueCollection.CollectionServiceIndex = TransportSerialization.GetStringFromRntbdTokenIfPresent(response.collectionServiceIndex);
      nameValueCollection.LSN = TransportSerialization.GetStringFromRntbdTokenIfPresent(response.LSN);
      nameValueCollection.ItemCount = TransportSerialization.GetStringFromRntbdTokenIfPresent(response.itemCount);
      nameValueCollection.SchemaVersion = TransportSerialization.GetStringFromRntbdTokenIfPresent(response.schemaVersion);
      nameValueCollection.OwnerFullName = TransportSerialization.GetStringFromRntbdTokenIfPresent(response.ownerFullName);
      nameValueCollection.OwnerId = TransportSerialization.GetStringFromRntbdTokenIfPresent(response.ownerId);
      nameValueCollection.DatabaseAccountId = TransportSerialization.GetStringFromRntbdTokenIfPresent(response.databaseAccountId);
      nameValueCollection.QuorumAckedLSN = TransportSerialization.GetStringFromRntbdTokenIfPresent(response.quorumAckedLSN);
      nameValueCollection.RequestValidationFailure = TransportSerialization.GetStringFromRntbdTokenIfPresent(response.requestValidationFailure);
      nameValueCollection.SubStatus = TransportSerialization.GetStringFromRntbdTokenIfPresent(response.subStatus);
      nameValueCollection.CollectionIndexTransformationProgress = TransportSerialization.GetStringFromRntbdTokenIfPresent(response.collectionUpdateProgress);
      nameValueCollection.CurrentWriteQuorum = TransportSerialization.GetStringFromRntbdTokenIfPresent(response.currentWriteQuorum);
      nameValueCollection.CurrentReplicaSetSize = TransportSerialization.GetStringFromRntbdTokenIfPresent(response.currentReplicaSetSize);
      nameValueCollection.CollectionLazyIndexingProgress = TransportSerialization.GetStringFromRntbdTokenIfPresent(response.collectionLazyIndexProgress);
      nameValueCollection.PartitionKeyRangeId = TransportSerialization.GetStringFromRntbdTokenIfPresent(response.partitionKeyRangeId);
      nameValueCollection.LogResults = TransportSerialization.GetStringFromRntbdTokenIfPresent(response.logResults);
      nameValueCollection.XPRole = TransportSerialization.GetStringFromRntbdTokenIfPresent(response.xpRole);
      nameValueCollection.IsRUPerMinuteUsed = TransportSerialization.GetStringFromRntbdTokenIfPresent(response.isRUPerMinuteUsed);
      nameValueCollection.QueryMetrics = TransportSerialization.GetStringFromRntbdTokenIfPresent(response.queryMetrics);
      nameValueCollection.QueryExecutionInfo = TransportSerialization.GetStringFromRntbdTokenIfPresent(response.queryExecutionInfo);
      nameValueCollection.IndexUtilization = TransportSerialization.GetStringFromRntbdTokenIfPresent(response.indexUtilization);
      nameValueCollection.GlobalCommittedLSN = TransportSerialization.GetStringFromRntbdTokenIfPresent(response.globalCommittedLSN);
      nameValueCollection.NumberOfReadRegions = TransportSerialization.GetStringFromRntbdTokenIfPresent(response.numberOfReadRegions);
      nameValueCollection.OfferReplacePending = TransportSerialization.GetResponseBoolHeaderIfPresent(response.offerReplacePending);
      nameValueCollection.ItemLSN = TransportSerialization.GetStringFromRntbdTokenIfPresent(response.itemLSN);
      nameValueCollection.RestoreState = TransportSerialization.GetStringFromRntbdTokenIfPresent(response.restoreState);
      nameValueCollection.CollectionSecurityIdentifier = TransportSerialization.GetStringFromRntbdTokenIfPresent(response.collectionSecurityIdentifier);
      nameValueCollection.TransportRequestID = TransportSerialization.GetStringFromRntbdTokenIfPresent(response.transportRequestID);
      nameValueCollection.ShareThroughput = TransportSerialization.GetResponseBoolHeaderIfPresent(response.shareThroughput);
      nameValueCollection.DisableRntbdChannel = TransportSerialization.GetResponseBoolHeaderIfPresent(response.disableRntbdChannel);
      nameValueCollection.XDate = TransportSerialization.GetStringFromRntbdTokenIfPresent(response.serverDateTimeUtc);
      nameValueCollection.LocalLSN = TransportSerialization.GetStringFromRntbdTokenIfPresent(response.localLSN);
      nameValueCollection.QuorumAckedLocalLSN = TransportSerialization.GetStringFromRntbdTokenIfPresent(response.quorumAckedLocalLSN);
      nameValueCollection.ItemLocalLSN = TransportSerialization.GetStringFromRntbdTokenIfPresent(response.itemLocalLSN);
      nameValueCollection.HasTentativeWrites = TransportSerialization.GetResponseBoolHeaderIfPresent(response.hasTentativeWrites);
      nameValueCollection.SessionToken = TransportSerialization.GetStringFromRntbdTokenIfPresent(response.sessionToken);
      nameValueCollection.ReplicatorLSNToGLSNDelta = TransportSerialization.GetStringFromRntbdTokenIfPresent(response.replicatorLSNToGLSNDelta);
      nameValueCollection.ReplicatorLSNToLLSNDelta = TransportSerialization.GetStringFromRntbdTokenIfPresent(response.replicatorLSNToLLSNDelta);
      nameValueCollection.VectorClockLocalProgress = TransportSerialization.GetStringFromRntbdTokenIfPresent(response.vectorClockLocalProgress);
      nameValueCollection.MinimumRUsForOffer = TransportSerialization.GetStringFromRntbdTokenIfPresent(response.minimumRUsForOffer);
      nameValueCollection.XPConfigurationSessionsCount = TransportSerialization.GetStringFromRntbdTokenIfPresent(response.xpConfigurationSesssionsCount);
      nameValueCollection.UnflushedMergLogEntryCount = TransportSerialization.GetStringFromRntbdTokenIfPresent(response.unflushedMergeLogEntryCount);
      nameValueCollection.ResourceId = TransportSerialization.GetStringFromRntbdTokenIfPresent(response.resourceName);
      nameValueCollection.TimeToLiveInSeconds = TransportSerialization.GetStringFromRntbdTokenIfPresent(response.timeToLiveInSeconds);
      nameValueCollection.ReplicaStatusRevoked = TransportSerialization.GetResponseBoolHeaderIfPresent(response.replicaStatusRevoked);
      nameValueCollection.SoftMaxAllowedThroughput = TransportSerialization.GetStringFromRntbdTokenIfPresent(response.softMaxAllowedThroughput);
      nameValueCollection.BackendRequestDurationMilliseconds = TransportSerialization.GetStringFromRntbdTokenIfPresent(response.backendRequestDurationMilliseconds);
      nameValueCollection.CorrelatedActivityId = TransportSerialization.GetStringFromRntbdTokenIfPresent(response.correlatedActivityId);
      nameValueCollection.ConfirmedStoreChecksum = TransportSerialization.GetStringFromRntbdTokenIfPresent(response.confirmedStoreChecksum);
      nameValueCollection.TentativeStoreChecksum = TransportSerialization.GetStringFromRntbdTokenIfPresent(response.tentativeStoreChecksum);
      nameValueCollection.PendingPKDelete = TransportSerialization.GetResponseBoolHeaderIfPresent(response.pendingPKDelete);
      nameValueCollection.AadAppliedRoleAssignmentId = TransportSerialization.GetStringFromRntbdTokenIfPresent(response.aadAppliedRoleAssignmentId);
      nameValueCollection.CollectionUniqueIndexReIndexProgress = TransportSerialization.GetStringFromRntbdTokenIfPresent(response.collectionUniqueIndexReIndexProgress);
      nameValueCollection.CollectionUniqueKeysUnderReIndex = TransportSerialization.GetStringFromRntbdTokenIfPresent(response.collectionUniqueKeysUnderReIndex);
      nameValueCollection.AnalyticalMigrationProgress = TransportSerialization.GetStringFromRntbdTokenIfPresent(response.analyticalMigrationProgress);
      nameValueCollection.TotalAccountThroughput = TransportSerialization.GetStringFromRntbdTokenIfPresent(response.totalAccountThroughput);
      nameValueCollection.ByokEncryptionProgress = TransportSerialization.GetStringFromRntbdTokenIfPresent(response.byokEncryptionProgress);
      nameValueCollection.AppliedPolicyElementId = TransportSerialization.GetStringFromRntbdTokenIfPresent(response.appliedPolicyElementId);
      nameValueCollection.MergeProgressBlocked = TransportSerialization.GetResponseBoolHeaderIfPresent(response.mergeProgressBlocked);
      nameValueCollection.ChangeFeedInfo = TransportSerialization.GetStringFromRntbdTokenIfPresent(response.changeFeedInfo);
      if (response.requestCharge.isPresent)
        nameValueCollection.RequestCharge = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0:0.##}", (object) response.requestCharge.value.valueDouble);
      if (response.indexingDirective.isPresent)
      {
        string str;
        switch (response.indexingDirective.value.valueByte)
        {
          case 0:
            str = IndexingDirectiveStrings.Default;
            break;
          case 1:
            str = IndexingDirectiveStrings.Include;
            break;
          case 2:
            str = IndexingDirectiveStrings.Exclude;
            break;
          default:
            throw new Exception();
        }
        nameValueCollection.IndexingDirective = str;
      }
      nameValueCollection.ServerVersion = serverVersion;
      nameValueCollection.ActivityId = activityId.ToString();
      storeResponse.ResponseBody = body;
      storeResponse.Status = (int) status;
      return storeResponse;
    }

    internal static TransportSerialization.RntbdHeader DecodeRntbdHeader(byte[] header) => new TransportSerialization.RntbdHeader((StatusCodes) BitConverter.ToUInt32(header, 4), BytesSerializer.ReadGuidFromBytes(new ArraySegment<byte>(header, 8, 16)));

    private static string GetStringFromRntbdTokenIfPresent(RntbdToken token)
    {
      if (!token.isPresent)
        return (string) null;
      switch (token.GetTokenType())
      {
        case RntbdTokenTypes.Byte:
          return token.value.valueByte.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        case RntbdTokenTypes.ULong:
          return token.value.valueULong.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        case RntbdTokenTypes.Long:
          return token.value.valueLong.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        case RntbdTokenTypes.ULongLong:
          return token.value.valueULongLong.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        case RntbdTokenTypes.LongLong:
          return token.value.valueLongLong.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        case RntbdTokenTypes.Guid:
          return token.value.valueGuid.ToString();
        case RntbdTokenTypes.SmallString:
        case RntbdTokenTypes.String:
          return BytesSerializer.GetStringFromBytes(token.value.valueBytes);
        case RntbdTokenTypes.Double:
          return token.value.valueDouble.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        default:
          throw new Exception(string.Format("Unsupported token type {0}", (object) token.GetTokenType()));
      }
    }

    private static string GetResponseBoolHeaderIfPresent(RntbdToken token) => token.isPresent ? (token.value.valueByte > (byte) 0).ToString().ToLowerInvariant() : (string) null;

    private static RntbdConstants.RntbdOperationType GetRntbdOperationType(
      OperationType operationType)
    {
      switch (operationType)
      {
        case OperationType.ExecuteJavaScript:
          return RntbdConstants.RntbdOperationType.ExecuteJavaScript;
        case OperationType.Create:
          return RntbdConstants.RntbdOperationType.Create;
        case OperationType.Patch:
          return RntbdConstants.RntbdOperationType.Patch;
        case OperationType.Read:
          return RntbdConstants.RntbdOperationType.Read;
        case OperationType.ReadFeed:
          return RntbdConstants.RntbdOperationType.ReadFeed;
        case OperationType.Delete:
          return RntbdConstants.RntbdOperationType.Delete;
        case OperationType.Replace:
          return RntbdConstants.RntbdOperationType.Replace;
        case OperationType.BatchApply:
          return RntbdConstants.RntbdOperationType.BatchApply;
        case OperationType.SqlQuery:
          return RntbdConstants.RntbdOperationType.SQLQuery;
        case OperationType.Query:
          return RntbdConstants.RntbdOperationType.Query;
        case OperationType.Head:
          return RntbdConstants.RntbdOperationType.Head;
        case OperationType.HeadFeed:
          return RntbdConstants.RntbdOperationType.HeadFeed;
        case OperationType.Upsert:
          return RntbdConstants.RntbdOperationType.Upsert;
        case OperationType.AddComputeGatewayRequestCharges:
          return RntbdConstants.RntbdOperationType.AddComputeGatewayRequestCharges;
        case OperationType.Batch:
          return RntbdConstants.RntbdOperationType.Batch;
        case OperationType.CompleteUserTransaction:
          return RntbdConstants.RntbdOperationType.CompleteUserTransaction;
        default:
          throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Invalid operation type: {0}", (object) operationType), nameof (operationType));
      }
    }

    private static RntbdConstants.RntbdResourceType GetRntbdResourceType(ResourceType resourceType)
    {
      switch (resourceType)
      {
        case ResourceType.Database:
          return RntbdConstants.RntbdResourceType.Database;
        case ResourceType.Collection:
          return RntbdConstants.RntbdResourceType.Collection;
        case ResourceType.Document:
          return RntbdConstants.RntbdResourceType.Document;
        case ResourceType.Attachment:
          return RntbdConstants.RntbdResourceType.Attachment;
        case ResourceType.User:
          return RntbdConstants.RntbdResourceType.User;
        case ResourceType.Permission:
          return RntbdConstants.RntbdResourceType.Permission;
        case ResourceType.Conflict:
          return RntbdConstants.RntbdResourceType.Conflict;
        case ResourceType.Record:
          return RntbdConstants.RntbdResourceType.Record;
        case ResourceType.StoredProcedure:
          return RntbdConstants.RntbdResourceType.StoredProcedure;
        case ResourceType.Trigger:
          return RntbdConstants.RntbdResourceType.Trigger;
        case ResourceType.UserDefinedFunction:
          return RntbdConstants.RntbdResourceType.UserDefinedFunction;
        case ResourceType.BatchApply:
          return RntbdConstants.RntbdResourceType.BatchApply;
        case ResourceType.Offer:
          return RntbdConstants.RntbdResourceType.Offer;
        case ResourceType.DatabaseAccount:
          return RntbdConstants.RntbdResourceType.DatabaseAccount;
        case ResourceType.Schema:
          return RntbdConstants.RntbdResourceType.Schema;
        case ResourceType.PartitionKeyRange:
          return RntbdConstants.RntbdResourceType.PartitionKeyRange;
        case ResourceType.ComputeGatewayCharges:
          return RntbdConstants.RntbdResourceType.ComputeGatewayCharges;
        case ResourceType.UserDefinedType:
          return RntbdConstants.RntbdResourceType.UserDefinedType;
        case ResourceType.PartitionKey:
          return RntbdConstants.RntbdResourceType.PartitionKey;
        case ResourceType.PartitionedSystemDocument:
          return RntbdConstants.RntbdResourceType.PartitionedSystemDocument;
        case ResourceType.ClientEncryptionKey:
          return RntbdConstants.RntbdResourceType.ClientEncryptionKey;
        case ResourceType.Transaction:
          return RntbdConstants.RntbdResourceType.Transaction;
        case ResourceType.RoleDefinition:
          return RntbdConstants.RntbdResourceType.RoleDefinition;
        case ResourceType.RoleAssignment:
          return RntbdConstants.RntbdResourceType.RoleAssignment;
        case ResourceType.SystemDocument:
          return RntbdConstants.RntbdResourceType.SystemDocument;
        case ResourceType.InteropUser:
          return RntbdConstants.RntbdResourceType.InteropUser;
        case ResourceType.AuthPolicyElement:
          return RntbdConstants.RntbdResourceType.AuthPolicyElement;
        case ResourceType.RetriableWriteCachedResponse:
          return RntbdConstants.RntbdResourceType.RetriableWriteCachedResponse;
        default:
          throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Invalid resource type: {0}", (object) resourceType), nameof (resourceType));
      }
    }

    private static void AddMatchHeader(
      RequestNameValueCollection requestHeaders,
      RntbdConstants.RntbdOperationType operationType,
      RntbdConstants.Request rntbdRequest)
    {
      string toConvert;
      switch (operationType)
      {
        case RntbdConstants.RntbdOperationType.Read:
        case RntbdConstants.RntbdOperationType.ReadFeed:
          toConvert = requestHeaders.IfNoneMatch;
          break;
        default:
          toConvert = requestHeaders.IfMatch;
          break;
      }
      if (string.IsNullOrEmpty(toConvert))
        return;
      rntbdRequest.match.value.valueBytes = BytesSerializer.GetBytesForString(toConvert, rntbdRequest);
      rntbdRequest.match.isPresent = true;
    }

    private static void AddIfModifiedSinceHeader(
      RequestNameValueCollection requestHeaders,
      RntbdConstants.Request rntbdRequest)
    {
      string ifModifiedSince = requestHeaders.IfModifiedSince;
      if (string.IsNullOrEmpty(ifModifiedSince))
        return;
      rntbdRequest.ifModifiedSince.value.valueBytes = BytesSerializer.GetBytesForString(ifModifiedSince, rntbdRequest);
      rntbdRequest.ifModifiedSince.isPresent = true;
    }

    private static void AddA_IMHeader(
      RequestNameValueCollection requestHeaders,
      RntbdConstants.Request rntbdRequest)
    {
      string aIm = requestHeaders.A_IM;
      if (string.IsNullOrEmpty(aIm))
        return;
      rntbdRequest.a_IM.value.valueBytes = BytesSerializer.GetBytesForString(aIm, rntbdRequest);
      rntbdRequest.a_IM.isPresent = true;
    }

    private static void AddDateHeader(
      RequestNameValueCollection requestHeaders,
      RntbdConstants.Request rntbdRequest)
    {
      string dateHeader = Helpers.GetDateHeader(requestHeaders);
      if (string.IsNullOrEmpty(dateHeader))
        return;
      rntbdRequest.date.value.valueBytes = BytesSerializer.GetBytesForString(dateHeader, rntbdRequest);
      rntbdRequest.date.isPresent = true;
    }

    private static void AddContinuation(
      RequestNameValueCollection requestHeaders,
      RntbdConstants.Request rntbdRequest)
    {
      if (string.IsNullOrEmpty(requestHeaders.Continuation))
        return;
      rntbdRequest.continuationToken.value.valueBytes = BytesSerializer.GetBytesForString(requestHeaders.Continuation, rntbdRequest);
      rntbdRequest.continuationToken.isPresent = true;
    }

    private static void AddResourceIdOrPathHeaders(
      DocumentServiceRequest request,
      RntbdConstants.Request rntbdRequest)
    {
      if (!string.IsNullOrEmpty(request.ResourceId))
      {
        rntbdRequest.resourceId.value.valueBytes = (ReadOnlyMemory<byte>) ResourceId.Parse(request.ResourceType, request.ResourceId);
        rntbdRequest.resourceId.isPresent = true;
      }
      if (!request.IsNameBased)
        return;
      if (request.ResourceType == ResourceType.Document && request.IsResourceNameParsedFromUri)
        TransportSerialization.SetResourceIdHeadersFromDocumentServiceRequest(request, rntbdRequest);
      else
        TransportSerialization.SetResourceIdHeadersFromUri(request, rntbdRequest);
    }

    private static void SetResourceIdHeadersFromUri(
      DocumentServiceRequest request,
      RntbdConstants.Request rntbdRequest)
    {
      string[] strArray = request.ResourceAddress.Split(TransportSerialization.UrlTrim, StringSplitOptions.RemoveEmptyEntries);
      if (strArray.Length >= 2)
      {
        switch (strArray[0])
        {
          case "dbs":
            rntbdRequest.databaseName.value.valueBytes = BytesSerializer.GetBytesForString(strArray[1], rntbdRequest);
            rntbdRequest.databaseName.isPresent = true;
            break;
          case "snapshots":
            rntbdRequest.snapshotName.value.valueBytes = BytesSerializer.GetBytesForString(strArray[1], rntbdRequest);
            rntbdRequest.snapshotName.isPresent = true;
            break;
          case "roledefinitions":
            rntbdRequest.roleDefinitionName.value.valueBytes = BytesSerializer.GetBytesForString(strArray[1], rntbdRequest);
            rntbdRequest.roleDefinitionName.isPresent = true;
            break;
          case "roleassignments":
            rntbdRequest.roleAssignmentName.value.valueBytes = BytesSerializer.GetBytesForString(strArray[1], rntbdRequest);
            rntbdRequest.roleAssignmentName.isPresent = true;
            break;
          case "interopusers":
            rntbdRequest.interopUserName.value.valueBytes = BytesSerializer.GetBytesForString(strArray[1], rntbdRequest);
            rntbdRequest.interopUserName.isPresent = true;
            break;
          case "authpolicyelements":
            rntbdRequest.authPolicyElementName.value.valueBytes = BytesSerializer.GetBytesForString(strArray[1], rntbdRequest);
            rntbdRequest.authPolicyElementName.isPresent = true;
            break;
          default:
            throw new BadRequestException();
        }
      }
      if (strArray.Length >= 4)
      {
        switch (strArray[2])
        {
          case "colls":
            rntbdRequest.collectionName.value.valueBytes = BytesSerializer.GetBytesForString(strArray[3], rntbdRequest);
            rntbdRequest.collectionName.isPresent = true;
            break;
          case "clientencryptionkeys":
            rntbdRequest.clientEncryptionKeyName.value.valueBytes = BytesSerializer.GetBytesForString(strArray[3], rntbdRequest);
            rntbdRequest.clientEncryptionKeyName.isPresent = true;
            break;
          case "users":
            rntbdRequest.userName.value.valueBytes = BytesSerializer.GetBytesForString(strArray[3], rntbdRequest);
            rntbdRequest.userName.isPresent = true;
            break;
          case "udts":
            rntbdRequest.userDefinedTypeName.value.valueBytes = BytesSerializer.GetBytesForString(strArray[3], rntbdRequest);
            rntbdRequest.userDefinedTypeName.isPresent = true;
            break;
        }
      }
      if (strArray.Length >= 6)
      {
        switch (strArray[4])
        {
          case "conflicts":
            rntbdRequest.conflictName.value.valueBytes = BytesSerializer.GetBytesForString(strArray[5], rntbdRequest);
            rntbdRequest.conflictName.isPresent = true;
            break;
          case "docs":
            rntbdRequest.documentName.value.valueBytes = BytesSerializer.GetBytesForString(strArray[5], rntbdRequest);
            rntbdRequest.documentName.isPresent = true;
            break;
          case "partitionedsystemdocuments":
            rntbdRequest.systemDocumentName.value.valueBytes = BytesSerializer.GetBytesForString(strArray[5], rntbdRequest);
            rntbdRequest.systemDocumentName.isPresent = true;
            break;
          case "permissions":
            rntbdRequest.permissionName.value.valueBytes = BytesSerializer.GetBytesForString(strArray[5], rntbdRequest);
            rntbdRequest.permissionName.isPresent = true;
            break;
          case "pkranges":
            rntbdRequest.partitionKeyRangeName.value.valueBytes = BytesSerializer.GetBytesForString(strArray[5], rntbdRequest);
            rntbdRequest.partitionKeyRangeName.isPresent = true;
            break;
          case "schemas":
            rntbdRequest.schemaName.value.valueBytes = BytesSerializer.GetBytesForString(strArray[5], rntbdRequest);
            rntbdRequest.schemaName.isPresent = true;
            break;
          case "sprocs":
            rntbdRequest.storedProcedureName.value.valueBytes = BytesSerializer.GetBytesForString(strArray[5], rntbdRequest);
            rntbdRequest.storedProcedureName.isPresent = true;
            break;
          case "systemdocuments":
            rntbdRequest.systemDocumentName.value.valueBytes = BytesSerializer.GetBytesForString(strArray[5], rntbdRequest);
            rntbdRequest.systemDocumentName.isPresent = true;
            break;
          case "triggers":
            rntbdRequest.triggerName.value.valueBytes = BytesSerializer.GetBytesForString(strArray[5], rntbdRequest);
            rntbdRequest.triggerName.isPresent = true;
            break;
          case "udfs":
            rntbdRequest.userDefinedFunctionName.value.valueBytes = BytesSerializer.GetBytesForString(strArray[5], rntbdRequest);
            rntbdRequest.userDefinedFunctionName.isPresent = true;
            break;
        }
      }
      if (strArray.Length < 8 || !(strArray[6] == "attachments"))
        return;
      rntbdRequest.attachmentName.value.valueBytes = BytesSerializer.GetBytesForString(strArray[7], rntbdRequest);
      rntbdRequest.attachmentName.isPresent = true;
    }

    private static void SetResourceIdHeadersFromDocumentServiceRequest(
      DocumentServiceRequest request,
      RntbdConstants.Request rntbdRequest)
    {
      if (string.IsNullOrEmpty(request.DatabaseName))
        throw new ArgumentException("DatabaseName");
      rntbdRequest.databaseName.value.valueBytes = BytesSerializer.GetBytesForString(request.DatabaseName, rntbdRequest);
      rntbdRequest.databaseName.isPresent = true;
      if (string.IsNullOrEmpty(request.CollectionName))
        throw new ArgumentException("CollectionName");
      rntbdRequest.collectionName.value.valueBytes = BytesSerializer.GetBytesForString(request.CollectionName, rntbdRequest);
      rntbdRequest.collectionName.isPresent = true;
      if (string.IsNullOrEmpty(request.DocumentName))
        return;
      rntbdRequest.documentName.value.valueBytes = BytesSerializer.GetBytesForString(request.DocumentName, rntbdRequest);
      rntbdRequest.documentName.isPresent = true;
    }

    private static void AddBinaryIdIfPresent(
      DocumentServiceRequest request,
      RntbdConstants.Request rntbdRequest)
    {
      object obj;
      if (request.Properties != null && request.Properties.TryGetValue("x-ms-binary-id", out obj))
      {
        switch (obj)
        {
          case byte[] numArray:
            rntbdRequest.binaryId.value.valueBytes = (ReadOnlyMemory<byte>) numArray;
            break;
          case ReadOnlyMemory<byte> readOnlyMemory:
            rntbdRequest.binaryId.value.valueBytes = readOnlyMemory;
            break;
          default:
            throw new ArgumentOutOfRangeException("x-ms-binary-id");
        }
        rntbdRequest.binaryId.isPresent = true;
      }
      else
      {
        string headerValue;
        if (!TransportSerialization.TryGetHeaderValueString(request, "x-ms-binary-id", out headerValue))
          return;
        rntbdRequest.binaryId.value.valueBytes = (ReadOnlyMemory<byte>) Convert.FromBase64String(headerValue);
        rntbdRequest.binaryId.isPresent = true;
      }
    }

    private static void AddReturnPreferenceIfPresent(
      RequestNameValueCollection requestHeaders,
      RntbdConstants.Request rntbdRequest)
    {
      string prefer = requestHeaders.Prefer;
      if (string.IsNullOrEmpty(prefer))
        return;
      if (string.Equals(prefer, "return=minimal", StringComparison.OrdinalIgnoreCase))
      {
        rntbdRequest.returnPreference.value.valueByte = (byte) 1;
        rntbdRequest.returnPreference.isPresent = true;
      }
      else
      {
        if (!string.Equals(prefer, "return=representation", StringComparison.OrdinalIgnoreCase))
          return;
        rntbdRequest.returnPreference.value.valueByte = (byte) 0;
        rntbdRequest.returnPreference.isPresent = true;
      }
    }

    private static void AddEffectivePartitionKeyIfPresent(
      DocumentServiceRequest request,
      RntbdConstants.Request rntbdRequest)
    {
      object obj;
      if (request.Properties == null || !request.Properties.TryGetValue("x-ms-effective-partition-key", out obj))
        return;
      rntbdRequest.effectivePartitionKey.value.valueBytes = obj is byte[] numArray ? (ReadOnlyMemory<byte>) numArray : throw new ArgumentOutOfRangeException("x-ms-effective-partition-key");
      rntbdRequest.effectivePartitionKey.isPresent = true;
    }

    private static bool TryGetHeaderValueString(
      DocumentServiceRequest request,
      string headerName,
      out string headerValue)
    {
      headerValue = (string) null;
      if (request.Headers != null)
        headerValue = request.Headers.Get(headerName);
      return !string.IsNullOrWhiteSpace(headerValue);
    }

    private static void AddMergeStaticIdIfPresent(
      DocumentServiceRequest request,
      RntbdConstants.Request rntbdRequest)
    {
      object obj;
      if (request.Properties == null || !request.Properties.TryGetValue("x-ms-cosmos-merge-static-id", out obj))
        return;
      switch (obj)
      {
        case byte[] numArray:
          rntbdRequest.mergeStaticId.value.valueBytes = (ReadOnlyMemory<byte>) numArray;
          break;
        case ReadOnlyMemory<byte> readOnlyMemory:
          rntbdRequest.mergeStaticId.value.valueBytes = readOnlyMemory;
          break;
        default:
          throw new ArgumentOutOfRangeException("x-ms-cosmos-merge-static-id");
      }
      rntbdRequest.mergeStaticId.isPresent = true;
    }

    private static void AddEntityId(
      DocumentServiceRequest request,
      RntbdConstants.Request rntbdRequest)
    {
      if (string.IsNullOrEmpty(request.EntityId))
        return;
      rntbdRequest.entityId.value.valueBytes = BytesSerializer.GetBytesForString(request.EntityId, rntbdRequest);
      rntbdRequest.entityId.isPresent = true;
    }

    private static void AddIndexingDirectiveHeader(
      RequestNameValueCollection requestHeaders,
      RntbdConstants.Request rntbdRequest)
    {
      if (string.IsNullOrEmpty(requestHeaders.IndexingDirective))
        return;
      IndexingDirective result;
      if (!Enum.TryParse<IndexingDirective>(requestHeaders.IndexingDirective, true, out result))
        throw new BadRequestException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.InvalidEnumValue, (object) requestHeaders.IndexingDirective, (object) typeof (IndexingDirective).Name));
      RntbdConstants.RntbdIndexingDirective indexingDirective;
      switch (result)
      {
        case IndexingDirective.Default:
          indexingDirective = RntbdConstants.RntbdIndexingDirective.Default;
          break;
        case IndexingDirective.Include:
          indexingDirective = RntbdConstants.RntbdIndexingDirective.Include;
          break;
        case IndexingDirective.Exclude:
          indexingDirective = RntbdConstants.RntbdIndexingDirective.Exclude;
          break;
        default:
          throw new BadRequestException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.InvalidEnumValue, (object) requestHeaders.IndexingDirective, (object) typeof (IndexingDirective).Name));
      }
      rntbdRequest.indexingDirective.value.valueByte = (byte) indexingDirective;
      rntbdRequest.indexingDirective.isPresent = true;
    }

    private static void AddMigrateCollectionDirectiveHeader(
      RequestNameValueCollection requestHeaders,
      RntbdConstants.Request rntbdRequest)
    {
      if (string.IsNullOrEmpty(requestHeaders.MigrateCollectionDirective))
        return;
      MigrateCollectionDirective result;
      if (!Enum.TryParse<MigrateCollectionDirective>(requestHeaders.MigrateCollectionDirective, true, out result))
        throw new BadRequestException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.InvalidEnumValue, (object) requestHeaders.MigrateCollectionDirective, (object) typeof (MigrateCollectionDirective).Name));
      RntbdConstants.RntbdMigrateCollectionDirective collectionDirective;
      if (result != MigrateCollectionDirective.Thaw)
      {
        if (result != MigrateCollectionDirective.Freeze)
          throw new BadRequestException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.InvalidEnumValue, (object) requestHeaders.MigrateCollectionDirective, (object) typeof (MigrateCollectionDirective).Name));
        collectionDirective = RntbdConstants.RntbdMigrateCollectionDirective.Freeze;
      }
      else
        collectionDirective = RntbdConstants.RntbdMigrateCollectionDirective.Thaw;
      rntbdRequest.migrateCollectionDirective.value.valueByte = (byte) collectionDirective;
      rntbdRequest.migrateCollectionDirective.isPresent = true;
    }

    private static void AddConsistencyLevelHeader(
      RequestNameValueCollection requestHeaders,
      RntbdConstants.Request rntbdRequest)
    {
      if (string.IsNullOrEmpty(requestHeaders.ConsistencyLevel))
        return;
      ConsistencyLevel result;
      if (!Enum.TryParse<ConsistencyLevel>(requestHeaders.ConsistencyLevel, true, out result))
        throw new BadRequestException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.InvalidEnumValue, (object) requestHeaders.ConsistencyLevel, (object) typeof (ConsistencyLevel).Name));
      RntbdConstants.RntbdConsistencyLevel consistencyLevel;
      switch (result)
      {
        case ConsistencyLevel.Strong:
          consistencyLevel = RntbdConstants.RntbdConsistencyLevel.Strong;
          break;
        case ConsistencyLevel.BoundedStaleness:
          consistencyLevel = RntbdConstants.RntbdConsistencyLevel.BoundedStaleness;
          break;
        case ConsistencyLevel.Session:
          consistencyLevel = RntbdConstants.RntbdConsistencyLevel.Session;
          break;
        case ConsistencyLevel.Eventual:
          consistencyLevel = RntbdConstants.RntbdConsistencyLevel.Eventual;
          break;
        case ConsistencyLevel.ConsistentPrefix:
          consistencyLevel = RntbdConstants.RntbdConsistencyLevel.ConsistentPrefix;
          break;
        default:
          throw new BadRequestException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.InvalidEnumValue, (object) requestHeaders.ConsistencyLevel, (object) typeof (ConsistencyLevel).Name));
      }
      rntbdRequest.consistencyLevel.value.valueByte = (byte) consistencyLevel;
      rntbdRequest.consistencyLevel.isPresent = true;
    }

    private static void AddIsThroughputCapRequest(
      RequestNameValueCollection requestHeaders,
      RntbdConstants.Request rntbdRequest)
    {
      if (string.IsNullOrEmpty(requestHeaders.IsThroughputCapRequest))
        return;
      rntbdRequest.isThroughputCapRequest.value.valueByte = requestHeaders.IsThroughputCapRequest.Equals(bool.TrueString, StringComparison.OrdinalIgnoreCase) ? (byte) 1 : (byte) 0;
      rntbdRequest.isThroughputCapRequest.isPresent = true;
    }

    private static void AddIsFanout(
      RequestNameValueCollection requestHeaders,
      RntbdConstants.Request rntbdRequest)
    {
      if (string.IsNullOrEmpty(requestHeaders.IsFanoutRequest))
        return;
      rntbdRequest.isFanout.value.valueByte = requestHeaders.IsFanoutRequest.Equals(bool.TrueString) ? (byte) 1 : (byte) 0;
      rntbdRequest.isFanout.isPresent = true;
    }

    private static void AddAllowScanOnQuery(
      RequestNameValueCollection requestHeaders,
      RntbdConstants.Request rntbdRequest)
    {
      if (string.IsNullOrEmpty(requestHeaders.EnableScanInQuery))
        return;
      rntbdRequest.enableScanInQuery.value.valueByte = requestHeaders.EnableScanInQuery.Equals(bool.TrueString, StringComparison.OrdinalIgnoreCase) ? (byte) 1 : (byte) 0;
      rntbdRequest.enableScanInQuery.isPresent = true;
    }

    private static void AddEnableLowPrecisionOrderBy(
      RequestNameValueCollection requestHeaders,
      RntbdConstants.Request rntbdRequest)
    {
      if (string.IsNullOrEmpty(requestHeaders.EnableLowPrecisionOrderBy))
        return;
      rntbdRequest.enableLowPrecisionOrderBy.value.valueByte = requestHeaders.EnableLowPrecisionOrderBy.Equals(bool.TrueString, StringComparison.OrdinalIgnoreCase) ? (byte) 1 : (byte) 0;
      rntbdRequest.enableLowPrecisionOrderBy.isPresent = true;
    }

    private static void AddEmitVerboseTracesInQuery(
      RequestNameValueCollection requestHeaders,
      RntbdConstants.Request rntbdRequest)
    {
      if (string.IsNullOrEmpty(requestHeaders.EmitVerboseTracesInQuery))
        return;
      rntbdRequest.emitVerboseTracesInQuery.value.valueByte = requestHeaders.EmitVerboseTracesInQuery.Equals(bool.TrueString, StringComparison.OrdinalIgnoreCase) ? (byte) 1 : (byte) 0;
      rntbdRequest.emitVerboseTracesInQuery.isPresent = true;
    }

    private static void AddCanCharge(
      RequestNameValueCollection requestHeaders,
      RntbdConstants.Request rntbdRequest)
    {
      if (string.IsNullOrEmpty(requestHeaders.CanCharge))
        return;
      rntbdRequest.canCharge.value.valueByte = requestHeaders.CanCharge.Equals(bool.TrueString, StringComparison.OrdinalIgnoreCase) ? (byte) 1 : (byte) 0;
      rntbdRequest.canCharge.isPresent = true;
    }

    private static void AddCanThrottle(
      RequestNameValueCollection requestHeaders,
      RntbdConstants.Request rntbdRequest)
    {
      if (string.IsNullOrEmpty(requestHeaders.CanThrottle))
        return;
      rntbdRequest.canThrottle.value.valueByte = requestHeaders.CanThrottle.Equals(bool.TrueString, StringComparison.OrdinalIgnoreCase) ? (byte) 1 : (byte) 0;
      rntbdRequest.canThrottle.isPresent = true;
    }

    private static void AddProfileRequest(
      RequestNameValueCollection requestHeaders,
      RntbdConstants.Request rntbdRequest)
    {
      if (string.IsNullOrEmpty(requestHeaders.ProfileRequest))
        return;
      rntbdRequest.profileRequest.value.valueByte = requestHeaders.ProfileRequest.Equals(bool.TrueString, StringComparison.OrdinalIgnoreCase) ? (byte) 1 : (byte) 0;
      rntbdRequest.profileRequest.isPresent = true;
    }

    private static void AddPageSize(
      RequestNameValueCollection requestHeaders,
      RntbdConstants.Request rntbdRequest)
    {
      string pageSize = requestHeaders.PageSize;
      if (string.IsNullOrEmpty(pageSize))
        return;
      int result;
      if (!int.TryParse(pageSize, NumberStyles.Integer, (IFormatProvider) CultureInfo.InvariantCulture, out result))
        throw new BadRequestException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.InvalidPageSize, (object) pageSize));
      if (result == -1)
        rntbdRequest.pageSize.value.valueULong = uint.MaxValue;
      else
        rntbdRequest.pageSize.value.valueULong = result >= 0 ? (uint) result : throw new BadRequestException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.InvalidPageSize, (object) pageSize));
      rntbdRequest.pageSize.isPresent = true;
    }

    private static void AddEnableLogging(
      RequestNameValueCollection requestHeaders,
      RntbdConstants.Request rntbdRequest)
    {
      if (string.IsNullOrEmpty(requestHeaders.EnableLogging))
        return;
      rntbdRequest.enableLogging.value.valueByte = requestHeaders.EnableLogging.Equals(bool.TrueString, StringComparison.OrdinalIgnoreCase) ? (byte) 1 : (byte) 0;
      rntbdRequest.enableLogging.isPresent = true;
    }

    private static void AddSupportSpatialLegacyCoordinates(
      RequestNameValueCollection requestHeaders,
      RntbdConstants.Request rntbdRequest)
    {
      if (string.IsNullOrEmpty(requestHeaders.SupportSpatialLegacyCoordinates))
        return;
      rntbdRequest.supportSpatialLegacyCoordinates.value.valueByte = requestHeaders.SupportSpatialLegacyCoordinates.Equals(bool.TrueString, StringComparison.OrdinalIgnoreCase) ? (byte) 1 : (byte) 0;
      rntbdRequest.supportSpatialLegacyCoordinates.isPresent = true;
    }

    private static void AddUsePolygonsSmallerThanAHemisphere(
      RequestNameValueCollection requestHeaders,
      RntbdConstants.Request rntbdRequest)
    {
      if (string.IsNullOrEmpty(requestHeaders.UsePolygonsSmallerThanAHemisphere))
        return;
      rntbdRequest.usePolygonsSmallerThanAHemisphere.value.valueByte = requestHeaders.UsePolygonsSmallerThanAHemisphere.Equals(bool.TrueString, StringComparison.OrdinalIgnoreCase) ? (byte) 1 : (byte) 0;
      rntbdRequest.usePolygonsSmallerThanAHemisphere.isPresent = true;
    }

    private static void AddPopulateQuotaInfo(
      RequestNameValueCollection requestHeaders,
      RntbdConstants.Request rntbdRequest)
    {
      if (string.IsNullOrEmpty(requestHeaders.PopulateQuotaInfo))
        return;
      rntbdRequest.populateQuotaInfo.value.valueByte = requestHeaders.PopulateQuotaInfo.Equals(bool.TrueString, StringComparison.OrdinalIgnoreCase) ? (byte) 1 : (byte) 0;
      rntbdRequest.populateQuotaInfo.isPresent = true;
    }

    private static void AddPopulateResourceCount(
      RequestNameValueCollection requestHeaders,
      RntbdConstants.Request rntbdRequest)
    {
      if (string.IsNullOrEmpty(requestHeaders.PopulateResourceCount))
        return;
      rntbdRequest.populateResourceCount.value.valueByte = requestHeaders.PopulateResourceCount.Equals(bool.TrueString, StringComparison.OrdinalIgnoreCase) ? (byte) 1 : (byte) 0;
      rntbdRequest.populateResourceCount.isPresent = true;
    }

    private static void AddPopulatePartitionStatistics(
      RequestNameValueCollection requestHeaders,
      RntbdConstants.Request rntbdRequest)
    {
      if (string.IsNullOrEmpty(requestHeaders.PopulatePartitionStatistics))
        return;
      rntbdRequest.populatePartitionStatistics.value.valueByte = requestHeaders.PopulatePartitionStatistics.Equals(bool.TrueString, StringComparison.OrdinalIgnoreCase) ? (byte) 1 : (byte) 0;
      rntbdRequest.populatePartitionStatistics.isPresent = true;
    }

    private static void AddDisableRUPerMinuteUsage(
      RequestNameValueCollection requestHeaders,
      RntbdConstants.Request rntbdRequest)
    {
      if (string.IsNullOrEmpty(requestHeaders.DisableRUPerMinuteUsage))
        return;
      rntbdRequest.disableRUPerMinuteUsage.value.valueByte = requestHeaders.DisableRUPerMinuteUsage.Equals(bool.TrueString, StringComparison.OrdinalIgnoreCase) ? (byte) 1 : (byte) 0;
      rntbdRequest.disableRUPerMinuteUsage.isPresent = true;
    }

    private static void AddPopulateQueryMetrics(
      RequestNameValueCollection requestHeaders,
      RntbdConstants.Request rntbdRequest)
    {
      if (string.IsNullOrEmpty(requestHeaders.PopulateQueryMetrics))
        return;
      rntbdRequest.populateQueryMetrics.value.valueByte = requestHeaders.PopulateQueryMetrics.Equals(bool.TrueString, StringComparison.OrdinalIgnoreCase) ? (byte) 1 : (byte) 0;
      rntbdRequest.populateQueryMetrics.isPresent = true;
    }

    private static void AddPopulateQueryMetricsIndexUtilization(
      RequestNameValueCollection requestHeaders,
      RntbdConstants.Request rntbdRequest)
    {
      if (string.IsNullOrEmpty(requestHeaders.PopulateIndexMetrics))
        return;
      rntbdRequest.populateIndexMetrics.value.valueByte = requestHeaders.PopulateIndexMetrics.Equals(bool.TrueString, StringComparison.OrdinalIgnoreCase) ? (byte) 1 : (byte) 0;
      rntbdRequest.populateIndexMetrics.isPresent = true;
    }

    private static void AddQueryForceScan(
      RequestNameValueCollection requestHeaders,
      RntbdConstants.Request rntbdRequest)
    {
      if (string.IsNullOrEmpty(requestHeaders.ForceQueryScan))
        return;
      rntbdRequest.forceQueryScan.value.valueByte = requestHeaders.ForceQueryScan.Equals(bool.TrueString, StringComparison.OrdinalIgnoreCase) ? (byte) 1 : (byte) 0;
      rntbdRequest.forceQueryScan.isPresent = true;
    }

    private static void AddPopulateCollectionThroughputInfo(
      RequestNameValueCollection requestHeaders,
      RntbdConstants.Request rntbdRequest)
    {
      if (string.IsNullOrEmpty(requestHeaders.PopulateCollectionThroughputInfo))
        return;
      rntbdRequest.populateCollectionThroughputInfo.value.valueByte = requestHeaders.PopulateCollectionThroughputInfo.Equals(bool.TrueString, StringComparison.OrdinalIgnoreCase) ? (byte) 1 : (byte) 0;
      rntbdRequest.populateCollectionThroughputInfo.isPresent = true;
    }

    private static void AddShareThroughput(
      RequestNameValueCollection requestHeaders,
      RntbdConstants.Request rntbdRequest)
    {
      if (string.IsNullOrEmpty(requestHeaders.ShareThroughput))
        return;
      rntbdRequest.shareThroughput.value.valueByte = requestHeaders.ShareThroughput.Equals(bool.TrueString, StringComparison.OrdinalIgnoreCase) ? (byte) 1 : (byte) 0;
      rntbdRequest.shareThroughput.isPresent = true;
    }

    private static void AddIsReadOnlyScript(
      RequestNameValueCollection requestHeaders,
      RntbdConstants.Request rntbdRequest)
    {
      if (string.IsNullOrEmpty(requestHeaders.IsReadOnlyScript))
        return;
      rntbdRequest.isReadOnlyScript.value.valueByte = requestHeaders.IsReadOnlyScript.Equals(bool.TrueString, StringComparison.OrdinalIgnoreCase) ? (byte) 1 : (byte) 0;
      rntbdRequest.isReadOnlyScript.isPresent = true;
    }

    private static void AddCanOfferReplaceComplete(
      RequestNameValueCollection requestHeaders,
      RntbdConstants.Request rntbdRequest)
    {
      if (string.IsNullOrEmpty(requestHeaders.CanOfferReplaceComplete))
        return;
      rntbdRequest.canOfferReplaceComplete.value.valueByte = requestHeaders.CanOfferReplaceComplete.Equals(bool.TrueString, StringComparison.OrdinalIgnoreCase) ? (byte) 1 : (byte) 0;
      rntbdRequest.canOfferReplaceComplete.isPresent = true;
    }

    private static void AddIgnoreSystemLoweringMaxThroughput(
      RequestNameValueCollection requestHeaders,
      RntbdConstants.Request rntbdRequest)
    {
      if (string.IsNullOrEmpty(requestHeaders.IgnoreSystemLoweringMaxThroughput))
        return;
      rntbdRequest.ignoreSystemLoweringMaxThroughput.value.valueByte = requestHeaders.IgnoreSystemLoweringMaxThroughput.Equals(bool.TrueString, StringComparison.OrdinalIgnoreCase) ? (byte) 1 : (byte) 0;
      rntbdRequest.ignoreSystemLoweringMaxThroughput.isPresent = true;
    }

    private static void AddUpdateMaxthroughputEverProvisioned(
      RequestNameValueCollection requestHeaders,
      RntbdConstants.Request rntbdRequest)
    {
      if (string.IsNullOrEmpty(requestHeaders.UpdateMaxThroughputEverProvisioned))
        return;
      string throughputEverProvisioned = requestHeaders.UpdateMaxThroughputEverProvisioned;
      int result;
      if (!int.TryParse(throughputEverProvisioned, NumberStyles.Integer, (IFormatProvider) CultureInfo.InvariantCulture, out result))
        throw new BadRequestException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.InvalidUpdateMaxthroughputEverProvisioned, (object) throughputEverProvisioned));
      rntbdRequest.updateMaxThroughputEverProvisioned.value.valueULong = result >= 0 ? (uint) result : throw new BadRequestException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.InvalidUpdateMaxthroughputEverProvisioned, (object) throughputEverProvisioned));
      rntbdRequest.updateMaxThroughputEverProvisioned.isPresent = true;
    }

    private static void AddGetAllPartitionKeyStatistics(
      RequestNameValueCollection requestHeaders,
      RntbdConstants.Request rntbdRequest)
    {
      if (string.IsNullOrEmpty(requestHeaders.GetAllPartitionKeyStatistics))
        return;
      rntbdRequest.getAllPartitionKeyStatistics.value.valueByte = requestHeaders.GetAllPartitionKeyStatistics.Equals(bool.TrueString, StringComparison.OrdinalIgnoreCase) ? (byte) 1 : (byte) 0;
      rntbdRequest.getAllPartitionKeyStatistics.isPresent = true;
    }

    private static void AddResponseContinuationTokenLimitInKb(
      RequestNameValueCollection requestHeaders,
      RntbdConstants.Request rntbdRequest)
    {
      if (string.IsNullOrEmpty(requestHeaders.ResponseContinuationTokenLimitInKB))
        return;
      string continuationTokenLimitInKb = requestHeaders.ResponseContinuationTokenLimitInKB;
      int result;
      if (!int.TryParse(continuationTokenLimitInKb, NumberStyles.Integer, (IFormatProvider) CultureInfo.InvariantCulture, out result))
        throw new BadRequestException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.InvalidPageSize, (object) continuationTokenLimitInKb));
      rntbdRequest.responseContinuationTokenLimitInKb.value.valueULong = result >= 0 ? (uint) result : throw new BadRequestException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.InvalidResponseContinuationTokenLimit, (object) continuationTokenLimitInKb));
      rntbdRequest.responseContinuationTokenLimitInKb.isPresent = true;
    }

    private static void AddRemoteStorageType(
      RequestNameValueCollection requestHeaders,
      RntbdConstants.Request rntbdRequest)
    {
      if (string.IsNullOrEmpty(requestHeaders.RemoteStorageType))
        return;
      RemoteStorageType result;
      if (!Enum.TryParse<RemoteStorageType>(requestHeaders.RemoteStorageType, true, out result))
        throw new BadRequestException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.InvalidEnumValue, (object) requestHeaders.RemoteStorageType, (object) typeof (RemoteStorageType).Name));
      RntbdConstants.RntbdRemoteStorageType remoteStorageType;
      if (result != RemoteStorageType.Standard)
      {
        if (result != RemoteStorageType.Premium)
          throw new BadRequestException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.InvalidEnumValue, (object) requestHeaders.RemoteStorageType, (object) typeof (RemoteStorageType).Name));
        remoteStorageType = RntbdConstants.RntbdRemoteStorageType.Premium;
      }
      else
        remoteStorageType = RntbdConstants.RntbdRemoteStorageType.Standard;
      rntbdRequest.remoteStorageType.value.valueByte = (byte) remoteStorageType;
      rntbdRequest.remoteStorageType.isPresent = true;
    }

    private static void AddCollectionChildResourceNameLimitInBytes(
      RequestNameValueCollection requestHeaders,
      RntbdConstants.Request rntbdRequest)
    {
      string nameLimitInBytes = requestHeaders.CollectionChildResourceNameLimitInBytes;
      if (string.IsNullOrEmpty(nameLimitInBytes))
        return;
      if (!int.TryParse(nameLimitInBytes, NumberStyles.Integer, (IFormatProvider) CultureInfo.InvariantCulture, out rntbdRequest.collectionChildResourceNameLimitInBytes.value.valueLong))
        throw new BadRequestException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.InvalidHeaderValue, (object) nameLimitInBytes, (object) "x-ms-cosmos-collection-child-resourcename-limit"));
      rntbdRequest.collectionChildResourceNameLimitInBytes.isPresent = true;
    }

    private static void AddCollectionChildResourceContentLengthLimitInKB(
      RequestNameValueCollection requestHeaders,
      RntbdConstants.Request rntbdRequest)
    {
      string contentLimitInKb = requestHeaders.CollectionChildResourceContentLimitInKB;
      if (string.IsNullOrEmpty(contentLimitInKb))
        return;
      if (!int.TryParse(contentLimitInKb, NumberStyles.Integer, (IFormatProvider) CultureInfo.InvariantCulture, out rntbdRequest.collectionChildResourceContentLengthLimitInKB.value.valueLong))
        throw new BadRequestException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.InvalidHeaderValue, (object) contentLimitInKb, (object) "x-ms-cosmos-collection-child-contentlength-resourcelimit"));
      rntbdRequest.collectionChildResourceContentLengthLimitInKB.isPresent = true;
    }

    private static void AddUniqueIndexNameEncodingMode(
      RequestNameValueCollection requestHeaders,
      RntbdConstants.Request rntbdRequest)
    {
      string nameEncodingMode = requestHeaders.UniqueIndexNameEncodingMode;
      if (string.IsNullOrEmpty(nameEncodingMode))
        return;
      if (!byte.TryParse(nameEncodingMode, NumberStyles.Integer, (IFormatProvider) CultureInfo.InvariantCulture, out rntbdRequest.uniqueIndexNameEncodingMode.value.valueByte))
        throw new BadRequestException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.InvalidHeaderValue, (object) nameEncodingMode, (object) "x-ms-cosmos-unique-index-name-encoding-mode"));
      rntbdRequest.uniqueIndexNameEncodingMode.isPresent = true;
    }

    private static void AddUniqueIndexReIndexingState(
      RequestNameValueCollection requestHeaders,
      RntbdConstants.Request rntbdRequest)
    {
      string indexReIndexingState = requestHeaders.UniqueIndexReIndexingState;
      if (string.IsNullOrEmpty(indexReIndexingState))
        return;
      if (!byte.TryParse(indexReIndexingState, NumberStyles.Integer, (IFormatProvider) CultureInfo.InvariantCulture, out rntbdRequest.uniqueIndexReIndexingState.value.valueByte))
        throw new BadRequestException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.InvalidHeaderValue, (object) indexReIndexingState, (object) "x-ms-cosmos-uniqueindex-reindexing-state"));
      rntbdRequest.uniqueIndexReIndexingState.isPresent = true;
    }

    private static void AddIsInternalServerlessRequest(
      RequestNameValueCollection requestHeaders,
      RntbdConstants.Request rntbdRequest)
    {
      if (string.IsNullOrEmpty(requestHeaders.IsInternalServerlessRequest))
        return;
      rntbdRequest.isInternalServerlessRequest.value.valueByte = requestHeaders.IsInternalServerlessRequest.Equals(bool.TrueString, StringComparison.OrdinalIgnoreCase) ? (byte) 1 : (byte) 0;
      rntbdRequest.isInternalServerlessRequest.isPresent = true;
    }

    private static void AddCorrelatedActivityId(
      RequestNameValueCollection requestHeaders,
      RntbdConstants.Request rntbdRequest)
    {
      string correlatedActivityId = requestHeaders.CorrelatedActivityId;
      if (string.IsNullOrEmpty(correlatedActivityId))
        return;
      if (!Guid.TryParse(correlatedActivityId, out rntbdRequest.correlatedActivityId.value.valueGuid))
        throw new BadRequestException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.InvalidHeaderValue, (object) correlatedActivityId, (object) "x-ms-cosmos-correlated-activityid"));
      rntbdRequest.correlatedActivityId.isPresent = true;
    }

    private static void AddCollectionRemoteStorageSecurityIdentifier(
      RequestNameValueCollection requestHeaders,
      RntbdConstants.Request rntbdRequest)
    {
      string securityIdentifier = requestHeaders.CollectionRemoteStorageSecurityIdentifier;
      if (string.IsNullOrEmpty(securityIdentifier))
        return;
      rntbdRequest.collectionRemoteStorageSecurityIdentifier.value.valueBytes = BytesSerializer.GetBytesForString(securityIdentifier, rntbdRequest);
      rntbdRequest.collectionRemoteStorageSecurityIdentifier.isPresent = true;
    }

    private static void AddIsUserRequest(
      RequestNameValueCollection requestHeaders,
      RntbdConstants.Request rntbdRequest)
    {
      if (string.IsNullOrEmpty(requestHeaders.IsUserRequest))
        return;
      rntbdRequest.isUserRequest.value.valueByte = requestHeaders.IsUserRequest.Equals(bool.TrueString, StringComparison.OrdinalIgnoreCase) ? (byte) 1 : (byte) 0;
      rntbdRequest.isUserRequest.isPresent = true;
    }

    private static void AddPreserveFullContent(
      RequestNameValueCollection requestHeaders,
      RntbdConstants.Request rntbdRequest)
    {
      if (string.IsNullOrEmpty(requestHeaders.PreserveFullContent))
        return;
      rntbdRequest.preserveFullContent.value.valueByte = requestHeaders.PreserveFullContent.Equals(bool.TrueString, StringComparison.OrdinalIgnoreCase) ? (byte) 1 : (byte) 0;
      rntbdRequest.preserveFullContent.isPresent = true;
    }

    private static void AddForceSideBySideIndexMigration(
      RequestNameValueCollection requestHeaders,
      RntbdConstants.Request rntbdRequest)
    {
      if (string.IsNullOrEmpty(requestHeaders.ForceSideBySideIndexMigration))
        return;
      rntbdRequest.forceSideBySideIndexMigration.value.valueByte = requestHeaders.ForceSideBySideIndexMigration.Equals(bool.TrueString, StringComparison.OrdinalIgnoreCase) ? (byte) 1 : (byte) 0;
      rntbdRequest.forceSideBySideIndexMigration.isPresent = true;
    }

    private static void AddPopulateUniqueIndexReIndexProgress(
      object headerObjectValue,
      RntbdConstants.Request rntbdRequest)
    {
      if (!(headerObjectValue is string b) || string.IsNullOrEmpty(b))
        return;
      rntbdRequest.populateUniqueIndexReIndexProgress.value.valueByte = !string.Equals(bool.TrueString, b, StringComparison.OrdinalIgnoreCase) ? (byte) 0 : (byte) 1;
      rntbdRequest.populateUniqueIndexReIndexProgress.isPresent = true;
    }

    private static void AddIsRUPerGBEnforcementRequest(
      RequestNameValueCollection requestHeaders,
      RntbdConstants.Request rntbdRequest)
    {
      if (string.IsNullOrEmpty(requestHeaders.IsRUPerGBEnforcementRequest))
        return;
      rntbdRequest.isRUPerGBEnforcementRequest.value.valueByte = requestHeaders.IsRUPerGBEnforcementRequest.Equals(bool.TrueString, StringComparison.OrdinalIgnoreCase) ? (byte) 1 : (byte) 0;
      rntbdRequest.isRUPerGBEnforcementRequest.isPresent = true;
    }

    private static void AddIsOfferStorageRefreshRequest(
      RequestNameValueCollection requestHeaders,
      RntbdConstants.Request rntbdRequest)
    {
      if (string.IsNullOrEmpty(requestHeaders.IsOfferStorageRefreshRequest))
        return;
      rntbdRequest.isofferStorageRefreshRequest.value.valueByte = requestHeaders.IsOfferStorageRefreshRequest.Equals(bool.TrueString, StringComparison.OrdinalIgnoreCase) ? (byte) 1 : (byte) 0;
      rntbdRequest.isofferStorageRefreshRequest.isPresent = true;
    }

    private static void AddIsMigrateOfferToManualThroughputRequest(
      RequestNameValueCollection requestHeaders,
      RntbdConstants.Request rntbdRequest)
    {
      if (string.IsNullOrEmpty(requestHeaders.MigrateOfferToManualThroughput))
        return;
      rntbdRequest.migrateOfferToManualThroughput.value.valueByte = requestHeaders.MigrateOfferToManualThroughput.Equals(bool.TrueString, StringComparison.OrdinalIgnoreCase) ? (byte) 1 : (byte) 0;
      rntbdRequest.migrateOfferToManualThroughput.isPresent = true;
    }

    private static void AddIsMigrateOfferToAutopilotRequest(
      RequestNameValueCollection requestHeaders,
      RntbdConstants.Request rntbdRequest)
    {
      if (string.IsNullOrEmpty(requestHeaders.MigrateOfferToAutopilot))
        return;
      rntbdRequest.migrateOfferToAutopilot.value.valueByte = requestHeaders.MigrateOfferToAutopilot.Equals(bool.TrueString, StringComparison.OrdinalIgnoreCase) ? (byte) 1 : (byte) 0;
      rntbdRequest.migrateOfferToAutopilot.isPresent = true;
    }

    private static void AddTruncateMergeLogRequest(
      RequestNameValueCollection requestHeaders,
      RntbdConstants.Request rntbdRequest)
    {
      if (string.IsNullOrEmpty(requestHeaders.TruncateMergeLogRequest))
        return;
      rntbdRequest.truncateMergeLogRequest.value.valueByte = requestHeaders.TruncateMergeLogRequest.Equals(bool.TrueString, StringComparison.OrdinalIgnoreCase) ? (byte) 1 : (byte) 0;
      rntbdRequest.truncateMergeLogRequest.isPresent = true;
    }

    private static void AddEnumerationDirection(
      DocumentServiceRequest request,
      RequestNameValueCollection requestHeaders,
      RntbdConstants.Request rntbdRequest)
    {
      object obj;
      if (request.Properties != null && request.Properties.TryGetValue("x-ms-enumeration-direction", out obj))
      {
        rntbdRequest.enumerationDirection.value.valueByte = (obj as byte? ?? throw new BadRequestException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.InvalidEnumValue, (object) "x-ms-enumeration-direction", (object) "EnumerationDirection"))).Value;
        rntbdRequest.enumerationDirection.isPresent = true;
      }
      else
      {
        if (string.IsNullOrEmpty(requestHeaders.EnumerationDirection))
          return;
        EnumerationDirection result;
        if (!Enum.TryParse<EnumerationDirection>(requestHeaders.EnumerationDirection, true, out result))
          throw new BadRequestException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.InvalidEnumValue, (object) requestHeaders.EnumerationDirection, (object) "EnumerationDirection"));
        RntbdConstants.RntdbEnumerationDirection enumerationDirection;
        if (result != EnumerationDirection.Forward)
        {
          if (result != EnumerationDirection.Reverse)
            throw new BadRequestException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.InvalidEnumValue, (object) requestHeaders.EnumerationDirection, (object) typeof (EnumerationDirection).Name));
          enumerationDirection = RntbdConstants.RntdbEnumerationDirection.Reverse;
        }
        else
          enumerationDirection = RntbdConstants.RntdbEnumerationDirection.Forward;
        rntbdRequest.enumerationDirection.value.valueByte = (byte) enumerationDirection;
        rntbdRequest.enumerationDirection.isPresent = true;
      }
    }

    private static void AddStartAndEndKeys(
      DocumentServiceRequest request,
      RequestNameValueCollection requestHeaders,
      RntbdConstants.Request rntbdRequest)
    {
      if (request.Properties == null || !string.IsNullOrEmpty(requestHeaders.ReadFeedKeyType))
      {
        TransportSerialization.AddStartAndEndKeysFromHeaders(requestHeaders, rntbdRequest);
      }
      else
      {
        RntbdConstants.RntdbReadFeedKeyType? nullable1 = new RntbdConstants.RntdbReadFeedKeyType?();
        object obj;
        if (request.Properties.TryGetValue("x-ms-read-key-type", out obj))
        {
          rntbdRequest.readFeedKeyType.value.valueByte = obj is byte num ? num : throw new ArgumentOutOfRangeException("x-ms-read-key-type");
          rntbdRequest.readFeedKeyType.isPresent = true;
          nullable1 = new RntbdConstants.RntdbReadFeedKeyType?((RntbdConstants.RntdbReadFeedKeyType) obj);
        }
        RntbdConstants.RntdbReadFeedKeyType? nullable2 = nullable1;
        RntbdConstants.RntdbReadFeedKeyType rntdbReadFeedKeyType1 = RntbdConstants.RntdbReadFeedKeyType.ResourceId;
        if (nullable2.GetValueOrDefault() == rntdbReadFeedKeyType1 & nullable2.HasValue)
        {
          TransportSerialization.SetBytesValue(request, "x-ms-start-id", rntbdRequest.StartId);
          TransportSerialization.SetBytesValue(request, "x-ms-end-id", rntbdRequest.EndId);
        }
        else
        {
          RntbdConstants.RntdbReadFeedKeyType? nullable3 = nullable1;
          RntbdConstants.RntdbReadFeedKeyType rntdbReadFeedKeyType2 = RntbdConstants.RntdbReadFeedKeyType.EffectivePartitionKey;
          if (!(nullable3.GetValueOrDefault() == rntdbReadFeedKeyType2 & nullable3.HasValue))
          {
            RntbdConstants.RntdbReadFeedKeyType? nullable4 = nullable1;
            RntbdConstants.RntdbReadFeedKeyType rntdbReadFeedKeyType3 = RntbdConstants.RntdbReadFeedKeyType.EffectivePartitionKeyRange;
            if (!(nullable4.GetValueOrDefault() == rntdbReadFeedKeyType3 & nullable4.HasValue))
              return;
          }
          TransportSerialization.SetBytesValue(request, "x-ms-start-epk", rntbdRequest.StartEpk);
          TransportSerialization.SetBytesValue(request, "x-ms-end-epk", rntbdRequest.EndEpk);
        }
      }
    }

    private static void AddStartAndEndKeysFromHeaders(
      RequestNameValueCollection requestHeaders,
      RntbdConstants.Request rntbdRequest)
    {
      bool flag = false;
      if (!string.IsNullOrEmpty(requestHeaders.ReadFeedKeyType))
      {
        ReadFeedKeyType result;
        if (!Enum.TryParse<ReadFeedKeyType>(requestHeaders.ReadFeedKeyType, true, out result))
          throw new BadRequestException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.InvalidEnumValue, (object) requestHeaders.ReadFeedKeyType, (object) "ReadFeedKeyType"));
        RntbdConstants.RntdbReadFeedKeyType rntdbReadFeedKeyType;
        switch (result)
        {
          case ReadFeedKeyType.ResourceId:
            rntdbReadFeedKeyType = RntbdConstants.RntdbReadFeedKeyType.ResourceId;
            break;
          case ReadFeedKeyType.EffectivePartitionKey:
            rntdbReadFeedKeyType = RntbdConstants.RntdbReadFeedKeyType.EffectivePartitionKey;
            break;
          case ReadFeedKeyType.EffectivePartitionKeyRange:
            rntdbReadFeedKeyType = RntbdConstants.RntdbReadFeedKeyType.EffectivePartitionKeyRange;
            flag = true;
            break;
          default:
            throw new BadRequestException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.InvalidEnumValue, (object) requestHeaders.ReadFeedKeyType, (object) typeof (ReadFeedKeyType).Name));
        }
        rntbdRequest.readFeedKeyType.value.valueByte = (byte) rntdbReadFeedKeyType;
        rntbdRequest.readFeedKeyType.isPresent = true;
      }
      string startId = requestHeaders.StartId;
      if (!string.IsNullOrEmpty(startId))
      {
        rntbdRequest.StartId.value.valueBytes = (ReadOnlyMemory<byte>) Convert.FromBase64String(startId);
        rntbdRequest.StartId.isPresent = true;
      }
      string endId = requestHeaders.EndId;
      if (!string.IsNullOrEmpty(endId))
      {
        rntbdRequest.EndId.value.valueBytes = (ReadOnlyMemory<byte>) Convert.FromBase64String(endId);
        rntbdRequest.EndId.isPresent = true;
      }
      string startEpk = requestHeaders.StartEpk;
      if (!string.IsNullOrEmpty(startEpk))
      {
        rntbdRequest.StartEpk.value.valueBytes = flag ? BytesSerializer.GetBytesForString(startEpk, rntbdRequest) : (ReadOnlyMemory<byte>) Convert.FromBase64String(startEpk);
        rntbdRequest.StartEpk.isPresent = true;
      }
      string endEpk = requestHeaders.EndEpk;
      if (string.IsNullOrEmpty(endEpk))
        return;
      rntbdRequest.EndEpk.value.valueBytes = flag ? BytesSerializer.GetBytesForString(endEpk, rntbdRequest) : (ReadOnlyMemory<byte>) Convert.FromBase64String(endEpk);
      rntbdRequest.EndEpk.isPresent = true;
    }

    private static void SetBytesValue(
      DocumentServiceRequest request,
      string headerName,
      RntbdToken token)
    {
      object obj;
      if (!request.Properties.TryGetValue(headerName, out obj))
        return;
      switch (obj)
      {
        case byte[] numArray:
          token.value.valueBytes = (ReadOnlyMemory<byte>) numArray;
          break;
        case ReadOnlyMemory<byte> readOnlyMemory:
          token.value.valueBytes = readOnlyMemory;
          break;
        default:
          throw new ArgumentOutOfRangeException(headerName);
      }
      token.isPresent = true;
    }

    private static void AddContentSerializationFormat(
      RequestNameValueCollection requestHeaders,
      RntbdConstants.Request rntbdRequest)
    {
      if (string.IsNullOrEmpty(requestHeaders.ContentSerializationFormat))
        return;
      ContentSerializationFormat result;
      if (!Enum.TryParse<ContentSerializationFormat>(requestHeaders.ContentSerializationFormat, true, out result))
        throw new BadRequestException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.InvalidEnumValue, (object) requestHeaders.ContentSerializationFormat, (object) "ContentSerializationFormat"));
      RntbdConstants.RntbdContentSerializationFormat serializationFormat;
      switch (result)
      {
        case ContentSerializationFormat.JsonText:
          serializationFormat = RntbdConstants.RntbdContentSerializationFormat.JsonText;
          break;
        case ContentSerializationFormat.CosmosBinary:
          serializationFormat = RntbdConstants.RntbdContentSerializationFormat.CosmosBinary;
          break;
        case ContentSerializationFormat.HybridRow:
          serializationFormat = RntbdConstants.RntbdContentSerializationFormat.HybridRow;
          break;
        default:
          throw new BadRequestException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.InvalidEnumValue, (object) requestHeaders.ContentSerializationFormat, (object) "ContentSerializationFormat"));
      }
      rntbdRequest.contentSerializationFormat.value.valueByte = (byte) serializationFormat;
      rntbdRequest.contentSerializationFormat.isPresent = true;
    }

    private static void FillTokenFromHeader(
      DocumentServiceRequest request,
      string headerName,
      string headerStringValue,
      RntbdToken token,
      RntbdConstants.Request rntbdRequest)
    {
      object obj = (object) null;
      if (string.IsNullOrEmpty(headerStringValue))
      {
        if (request.Properties == null || !request.Properties.TryGetValue(headerName, out obj) || obj == null)
          return;
        if (obj is string str)
        {
          headerStringValue = str;
          if (string.IsNullOrEmpty(headerStringValue))
            return;
        }
      }
      switch (token.GetTokenType())
      {
        case RntbdTokenTypes.Byte:
          bool flag1;
          if (headerStringValue != null)
            flag1 = string.Equals(headerStringValue, bool.TrueString, StringComparison.OrdinalIgnoreCase);
          else
            flag1 = obj is bool flag2 ? flag2 : throw new BadRequestException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.InvalidHeaderValue, obj, (object) headerName));
          token.value.valueByte = flag1 ? (byte) 1 : (byte) 0;
          break;
        case RntbdTokenTypes.ULong:
          uint result1;
          if (headerStringValue != null)
          {
            if (!uint.TryParse(headerStringValue, NumberStyles.Integer, (IFormatProvider) CultureInfo.InvariantCulture, out result1))
              throw new BadRequestException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.InvalidHeaderValue, (object) headerStringValue, (object) headerName));
          }
          else
            result1 = obj is uint num1 ? num1 : throw new BadRequestException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.InvalidHeaderValue, obj, (object) headerName));
          token.value.valueULong = result1;
          break;
        case RntbdTokenTypes.Long:
          int result2;
          if (headerStringValue != null)
          {
            if (!int.TryParse(headerStringValue, NumberStyles.Integer, (IFormatProvider) CultureInfo.InvariantCulture, out result2))
              throw new BadRequestException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.InvalidHeaderValue, (object) headerStringValue, (object) headerName));
          }
          else
            result2 = obj is int num2 ? num2 : throw new BadRequestException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.InvalidHeaderValue, obj, (object) headerName));
          token.value.valueLong = result2;
          break;
        case RntbdTokenTypes.LongLong:
          long result3;
          if (headerStringValue != null)
          {
            if (!long.TryParse(headerStringValue, NumberStyles.Integer, (IFormatProvider) CultureInfo.InvariantCulture, out result3))
              throw new BadRequestException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.InvalidHeaderValue, (object) headerStringValue, (object) headerName));
          }
          else
            result3 = obj is long num3 ? num3 : throw new BadRequestException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.InvalidHeaderValue, obj, (object) headerName));
          token.value.valueLongLong = result3;
          break;
        case RntbdTokenTypes.Guid:
          Guid result4;
          if (headerStringValue != null)
          {
            if (!Guid.TryParse(headerStringValue, out result4))
              throw new BadRequestException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.InvalidHeaderValue, (object) headerStringValue, (object) headerName));
          }
          else
            result4 = obj is Guid guid ? guid : throw new BadRequestException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.InvalidHeaderValue, obj, (object) headerName));
          token.value.valueGuid = result4;
          break;
        case RntbdTokenTypes.SmallString:
        case RntbdTokenTypes.String:
        case RntbdTokenTypes.ULongString:
          token.value.valueBytes = headerStringValue != null ? BytesSerializer.GetBytesForString(headerStringValue, rntbdRequest) : throw new BadRequestException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.InvalidHeaderValue, (object) headerStringValue, (object) headerName));
          break;
        case RntbdTokenTypes.Bytes:
          if (headerStringValue != null)
            throw new BadRequestException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.InvalidHeaderValue, (object) headerStringValue, (object) headerName));
          token.value.valueBytes = obj is byte[] numArray ? (ReadOnlyMemory<byte>) numArray : throw new BadRequestException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.InvalidHeaderValue, obj, (object) headerName));
          break;
        case RntbdTokenTypes.Double:
          double result5;
          if (headerStringValue != null)
          {
            if (!double.TryParse(headerStringValue, NumberStyles.Integer, (IFormatProvider) CultureInfo.InvariantCulture, out result5))
              throw new BadRequestException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.InvalidHeaderValue, (object) headerStringValue, (object) headerName));
          }
          else
            result5 = obj is double num4 ? num4 : throw new BadRequestException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.InvalidHeaderValue, obj, (object) headerName));
          token.value.valueDouble = result5;
          break;
        default:
          throw new BadRequestException();
      }
      token.isPresent = true;
    }

    private static void AddExcludeSystemProperties(
      RequestNameValueCollection requestHeaders,
      RntbdConstants.Request rntbdRequest)
    {
      if (string.IsNullOrEmpty(requestHeaders.ExcludeSystemProperties))
        return;
      rntbdRequest.excludeSystemProperties.value.valueByte = requestHeaders.ExcludeSystemProperties.Equals(bool.TrueString, StringComparison.OrdinalIgnoreCase) ? (byte) 1 : (byte) 0;
      rntbdRequest.excludeSystemProperties.isPresent = true;
    }

    private static void AddFanoutOperationStateHeader(
      RequestNameValueCollection requestHeaders,
      RntbdConstants.Request rntbdRequest)
    {
      string fanoutOperationState1 = requestHeaders.FanoutOperationState;
      if (string.IsNullOrEmpty(fanoutOperationState1))
        return;
      FanoutOperationState result;
      if (!Enum.TryParse<FanoutOperationState>(fanoutOperationState1, true, out result))
        throw new BadRequestException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.InvalidEnumValue, (object) fanoutOperationState1, (object) "FanoutOperationState"));
      RntbdConstants.RntbdFanoutOperationState fanoutOperationState2;
      if (result != FanoutOperationState.Started)
      {
        if (result != FanoutOperationState.Completed)
          throw new BadRequestException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.InvalidEnumValue, (object) fanoutOperationState1, (object) "FanoutOperationState"));
        fanoutOperationState2 = RntbdConstants.RntbdFanoutOperationState.Completed;
      }
      else
        fanoutOperationState2 = RntbdConstants.RntbdFanoutOperationState.Started;
      rntbdRequest.FanoutOperationState.value.valueByte = (byte) fanoutOperationState2;
      rntbdRequest.FanoutOperationState.isPresent = true;
    }

    private static void AddResourceTypes(
      RequestNameValueCollection requestHeaders,
      RntbdConstants.Request rntbdRequest)
    {
      string resourceTypes = requestHeaders.ResourceTypes;
      if (string.IsNullOrEmpty(resourceTypes))
        return;
      rntbdRequest.resourceTypes.value.valueBytes = BytesSerializer.GetBytesForString(resourceTypes, rntbdRequest);
      rntbdRequest.resourceTypes.isPresent = true;
    }

    private static void AddSystemDocumentTypeHeader(
      RequestNameValueCollection requestHeaders,
      RntbdConstants.Request rntbdRequest)
    {
      if (string.IsNullOrEmpty(requestHeaders.SystemDocumentType))
        return;
      SystemDocumentType result;
      if (!Enum.TryParse<SystemDocumentType>(requestHeaders.SystemDocumentType, true, out result))
        throw new BadRequestException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.InvalidEnumValue, (object) requestHeaders.SystemDocumentType, (object) "SystemDocumentType"));
      RntbdConstants.RntbdSystemDocumentType systemDocumentType;
      switch (result)
      {
        case SystemDocumentType.MaterializedViewLeaseDocument:
          systemDocumentType = RntbdConstants.RntbdSystemDocumentType.MaterializedViewLeaseDocument;
          break;
        case SystemDocumentType.MaterializedViewBuilderOwnershipDocument:
          systemDocumentType = RntbdConstants.RntbdSystemDocumentType.MaterializedViewBuilderOwnershipDocument;
          break;
        case SystemDocumentType.MaterializedViewLeaseStoreInitDocument:
          systemDocumentType = RntbdConstants.RntbdSystemDocumentType.MaterializedViewLeaseStoreInitDocument;
          break;
        default:
          throw new BadRequestException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.InvalidEnumValue, (object) requestHeaders.SystemDocumentType, (object) typeof (SystemDocumentType).Name));
      }
      rntbdRequest.systemDocumentType.value.valueByte = (byte) systemDocumentType;
      rntbdRequest.systemDocumentType.isPresent = true;
    }

    private static void AddTransactionMetaData(
      DocumentServiceRequest request,
      RntbdConstants.Request rntbdRequest)
    {
      object obj1;
      object obj2;
      if (request.Properties == null || !request.Properties.TryGetValue("x-ms-cosmos-tx-id", out obj1) || !request.Properties.TryGetValue("x-ms-cosmos-tx-init", out obj2))
        return;
      if (!(obj1 is byte[] numArray))
        throw new ArgumentOutOfRangeException("x-ms-cosmos-tx-id");
      bool? nullable = obj2 as bool?;
      if (!nullable.HasValue)
        throw new ArgumentOutOfRangeException("x-ms-cosmos-tx-init");
      rntbdRequest.transactionId.value.valueBytes = (ReadOnlyMemory<byte>) numArray;
      rntbdRequest.transactionId.isPresent = true;
      rntbdRequest.transactionFirstRequest.value.valueByte = nullable.Value ? (byte) 1 : (byte) 0;
      rntbdRequest.transactionFirstRequest.isPresent = true;
    }

    private static void AddTransactionCompletionFlag(
      DocumentServiceRequest request,
      RntbdConstants.Request rntbdRequest)
    {
      object obj;
      if (request.Properties == null || !request.Properties.TryGetValue("x-ms-cosmos-tx-commit", out obj))
        return;
      bool? nullable = obj as bool?;
      if (!nullable.HasValue)
        throw new ArgumentOutOfRangeException("x-ms-cosmos-tx-commit");
      rntbdRequest.transactionCommit.value.valueByte = nullable.Value ? (byte) 1 : (byte) 0;
      rntbdRequest.transactionCommit.isPresent = true;
    }

    private static void AddRetriableWriteRequestMetadata(
      DocumentServiceRequest request,
      RntbdConstants.Request rntbdRequest)
    {
      object obj1;
      if (request.Properties == null || !request.Properties.TryGetValue("x-ms-cosmos-retriable-write-request-id", out obj1))
        return;
      rntbdRequest.retriableWriteRequestId.value.valueBytes = obj1 is byte[] numArray ? (ReadOnlyMemory<byte>) numArray : throw new ArgumentOutOfRangeException("x-ms-cosmos-retriable-write-request-id");
      rntbdRequest.retriableWriteRequestId.isPresent = true;
      object obj2;
      if (request.Properties.TryGetValue("x-ms-cosmos-is-retried-write-request", out obj2))
      {
        bool? nullable = obj2 as bool?;
        if (!nullable.HasValue)
          throw new ArgumentOutOfRangeException("x-ms-cosmos-is-retried-write-request");
        rntbdRequest.isRetriedWriteRequest.value.valueByte = nullable.Value ? (byte) 1 : (byte) 0;
        rntbdRequest.isRetriedWriteRequest.isPresent = true;
      }
      object obj3;
      if (!request.Properties.TryGetValue("x-ms-cosmos-retriable-write-request-start-timestamp", out obj3))
        return;
      ulong result;
      rntbdRequest.retriableWriteRequestStartTimestamp.value.valueULongLong = ulong.TryParse(obj3.ToString(), out result) && result > 0UL ? result : throw new ArgumentOutOfRangeException("x-ms-cosmos-retriable-write-request-start-timestamp");
      rntbdRequest.retriableWriteRequestStartTimestamp.isPresent = true;
    }

    private static void AddUseSystemBudget(
      RequestNameValueCollection requestHeaders,
      RntbdConstants.Request rntbdRequest)
    {
      if (string.IsNullOrEmpty(requestHeaders.UseSystemBudget))
        return;
      rntbdRequest.useSystemBudget.value.valueByte = requestHeaders.UseSystemBudget.Equals(bool.TrueString, StringComparison.OrdinalIgnoreCase) ? (byte) 1 : (byte) 0;
      rntbdRequest.useSystemBudget.isPresent = true;
    }

    private static void AddRequestedCollectionType(
      RequestNameValueCollection requestHeaders,
      RntbdConstants.Request rntbdRequest)
    {
      string requestedCollectionType1 = requestHeaders.RequestedCollectionType;
      if (string.IsNullOrEmpty(requestedCollectionType1))
        return;
      RequestedCollectionType result;
      if (!Enum.TryParse<RequestedCollectionType>(requestedCollectionType1, true, out result))
        throw new BadRequestException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.InvalidEnumValue, (object) requestedCollectionType1, (object) "RequestedCollectionType"));
      RntbdConstants.RntbdRequestedCollectionType requestedCollectionType2;
      switch (result)
      {
        case RequestedCollectionType.All:
          requestedCollectionType2 = RntbdConstants.RntbdRequestedCollectionType.All;
          break;
        case RequestedCollectionType.Standard:
          requestedCollectionType2 = RntbdConstants.RntbdRequestedCollectionType.Standard;
          break;
        case RequestedCollectionType.MaterializedView:
          requestedCollectionType2 = RntbdConstants.RntbdRequestedCollectionType.MaterializedView;
          break;
        default:
          throw new BadRequestException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.InvalidEnumValue, (object) requestedCollectionType1, (object) "RequestedCollectionType"));
      }
      rntbdRequest.requestedCollectionType.value.valueByte = (byte) requestedCollectionType2;
      rntbdRequest.requestedCollectionType.isPresent = true;
    }

    private static void AddUpdateOfferStateToPending(
      RequestNameValueCollection requestHeaders,
      RntbdConstants.Request rntbdRequest)
    {
      if (string.IsNullOrEmpty(requestHeaders.UpdateOfferStateToPending))
        return;
      rntbdRequest.updateOfferStateToPending.value.valueByte = requestHeaders.UpdateOfferStateToPending.Equals(bool.TrueString, StringComparison.OrdinalIgnoreCase) ? (byte) 1 : (byte) 0;
      rntbdRequest.updateOfferStateToPending.isPresent = true;
    }

    private static void AddOfferReplaceRURedistribution(
      RequestNameValueCollection requestHeaders,
      RntbdConstants.Request rntbdRequest)
    {
      if (string.IsNullOrEmpty(requestHeaders.OfferReplaceRURedistribution))
        return;
      rntbdRequest.offerReplaceRURedistribution.value.valueByte = requestHeaders.OfferReplaceRURedistribution.Equals(bool.TrueString, StringComparison.OrdinalIgnoreCase) ? (byte) 1 : (byte) 0;
      rntbdRequest.offerReplaceRURedistribution.isPresent = true;
    }

    internal class RntbdHeader
    {
      public RntbdHeader(StatusCodes status, Guid activityId)
      {
        this.Status = status;
        this.ActivityId = activityId;
      }

      public StatusCodes Status { get; private set; }

      public Guid ActivityId { get; private set; }
    }

    internal sealed class SerializedRequest : IDisposable
    {
      private readonly BufferProvider.DisposableBuffer requestHeader;
      private readonly CloneableStream requestBody;

      public SerializedRequest(
        BufferProvider.DisposableBuffer requestHeader,
        CloneableStream requestBody)
      {
        this.requestHeader = requestHeader;
        this.requestBody = requestBody;
      }

      public int RequestSize
      {
        get
        {
          int count = this.requestHeader.Buffer.Count;
          CloneableStream requestBody = this.requestBody;
          int length = requestBody != null ? (int) requestBody.Length : 0;
          return count + length;
        }
      }

      public void Dispose()
      {
        this.requestHeader.Dispose();
        this.requestBody?.Dispose();
      }

      internal void CopyTo(ArraySegment<byte> buffer)
      {
        if (buffer.Count < this.RequestSize)
          throw new ArgumentException("Buffer should at least be as big as the request size");
        ArraySegment<byte> buffer1 = this.requestHeader.Buffer;
        byte[] array1 = buffer1.Array;
        buffer1 = this.requestHeader.Buffer;
        int offset1 = buffer1.Offset;
        byte[] array2 = buffer.Array;
        int offset2 = buffer.Offset;
        buffer1 = this.requestHeader.Buffer;
        int count1 = buffer1.Count;
        Array.Copy((Array) array1, offset1, (Array) array2, offset2, count1);
        if (this.requestBody == null)
          return;
        ArraySegment<byte> buffer2 = this.requestBody.GetBuffer();
        byte[] array3 = buffer2.Array;
        int offset3 = buffer2.Offset;
        byte[] array4 = buffer.Array;
        int offset4 = buffer.Offset;
        buffer1 = this.requestHeader.Buffer;
        int count2 = buffer1.Count;
        int destinationIndex = offset4 + count2;
        int count3 = buffer2.Count;
        Array.Copy((Array) array3, offset3, (Array) array4, destinationIndex, count3);
      }

      internal async Task CopyToStreamAsync(Stream stream)
      {
        Stream stream1 = stream;
        byte[] array = this.requestHeader.Buffer.Array;
        ArraySegment<byte> buffer1 = this.requestHeader.Buffer;
        int offset = buffer1.Offset;
        buffer1 = this.requestHeader.Buffer;
        int count = buffer1.Count;
        await stream1.WriteAsync(array, offset, count);
        if (this.requestBody == null)
          return;
        ArraySegment<byte> buffer2 = this.requestBody.GetBuffer();
        await stream.WriteAsync(buffer2.Array, buffer2.Offset, buffer2.Count);
      }
    }
  }
}
