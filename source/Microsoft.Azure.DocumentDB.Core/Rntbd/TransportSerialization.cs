// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Rntbd.TransportSerialization
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Microsoft.Azure.Cosmos.Core.Trace;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace Microsoft.Azure.Documents.Rntbd
{
  internal static class TransportSerialization
  {
    internal static readonly char[] UrlTrim = new char[1]
    {
      '/'
    };

    internal static byte[] BuildRequest(
      DocumentServiceRequest request,
      string replicaPath,
      ResourceOperation resourceOperation,
      Guid activityId,
      out int headerSize,
      out int bodySize)
    {
      RntbdConstants.RntbdOperationType rntbdOperationType = TransportSerialization.GetRntbdOperationType(resourceOperation.operationType);
      RntbdConstants.RntbdResourceType rntbdResourceType = TransportSerialization.GetRntbdResourceType(resourceOperation.resourceType);
      RntbdConstants.Request rntbdRequest = new RntbdConstants.Request();
      rntbdRequest.replicaPath.value.valueBytes = Encoding.UTF8.GetBytes(replicaPath);
      rntbdRequest.replicaPath.isPresent = true;
      TransportSerialization.AddResourceIdOrPathHeaders(request, rntbdRequest);
      TransportSerialization.AddDateHeader(request, rntbdRequest);
      TransportSerialization.AddContinuation(request, rntbdRequest);
      TransportSerialization.AddMatchHeader(request, rntbdOperationType, rntbdRequest);
      TransportSerialization.AddIfModifiedSinceHeader(request, rntbdRequest);
      TransportSerialization.AddA_IMHeader(request, rntbdRequest);
      TransportSerialization.AddIndexingDirectiveHeader(request, rntbdRequest);
      TransportSerialization.AddMigrateCollectionDirectiveHeader(request, rntbdRequest);
      TransportSerialization.AddConsistencyLevelHeader(request, rntbdRequest);
      TransportSerialization.AddIsFanout(request, rntbdRequest);
      TransportSerialization.AddEntityId(request, rntbdRequest);
      TransportSerialization.AddAllowScanOnQuery(request, rntbdRequest);
      TransportSerialization.AddEmitVerboseTracesInQuery(request, rntbdRequest);
      TransportSerialization.AddCanCharge(request, rntbdRequest);
      TransportSerialization.AddCanThrottle(request, rntbdRequest);
      TransportSerialization.AddProfileRequest(request, rntbdRequest);
      TransportSerialization.AddEnableLowPrecisionOrderBy(request, rntbdRequest);
      TransportSerialization.AddPageSize(request, rntbdRequest);
      TransportSerialization.AddSupportSpatialLegacyCoordinates(request, rntbdRequest);
      TransportSerialization.AddUsePolygonsSmallerThanAHemisphere(request, rntbdRequest);
      TransportSerialization.AddEnableLogging(request, rntbdRequest);
      TransportSerialization.AddPopulateQuotaInfo(request, rntbdRequest);
      TransportSerialization.AddPopulateResourceCount(request, rntbdRequest);
      TransportSerialization.AddDisableRUPerMinuteUsage(request, rntbdRequest);
      TransportSerialization.AddPopulateQueryMetrics(request, rntbdRequest);
      TransportSerialization.AddQueryForceScan(request, rntbdRequest);
      TransportSerialization.AddResponseContinuationTokenLimitInKb(request, rntbdRequest);
      TransportSerialization.AddPopulatePartitionStatistics(request, rntbdRequest);
      TransportSerialization.AddRemoteStorageType(request, rntbdRequest);
      TransportSerialization.AddCollectionRemoteStorageSecurityIdentifier(request, rntbdRequest);
      TransportSerialization.AddCollectionChildResourceNameLimitInBytes(request, rntbdRequest);
      TransportSerialization.AddCollectionChildResourceContentLengthLimitInKB(request, rntbdRequest);
      TransportSerialization.AddPopulateCollectionThroughputInfo(request, rntbdRequest);
      TransportSerialization.AddShareThroughput(request, rntbdRequest);
      TransportSerialization.AddIsReadOnlyScript(request, rntbdRequest);
      TransportSerialization.AddIsAutoScaleRequest(request, rntbdRequest);
      TransportSerialization.AddCanOfferReplaceComplete(request, rntbdRequest);
      TransportSerialization.AddExcludeSystemProperties(request, rntbdRequest);
      TransportSerialization.AddEnumerationDirection(request, rntbdRequest);
      TransportSerialization.AddFanoutOperationStateHeader(request, rntbdRequest);
      TransportSerialization.AddStartAndEndKeys(request, rntbdRequest);
      TransportSerialization.AddContentSerializationFormat(request, rntbdRequest);
      TransportSerialization.AddIsUserRequest(request, rntbdRequest);
      TransportSerialization.AddPreserveFullContent(request, rntbdRequest);
      TransportSerialization.AddIsRUPerGBEnforcementRequest(request, rntbdRequest);
      TransportSerialization.AddGetAllPartitionKeyStatistics(request, rntbdRequest);
      TransportSerialization.AddForceSideBySideIndexMigration(request, rntbdRequest);
      TransportSerialization.FillTokenFromHeader(request, "authorization", rntbdRequest.authorizationToken);
      TransportSerialization.FillTokenFromHeader(request, "x-ms-session-token", rntbdRequest.sessionToken);
      TransportSerialization.FillTokenFromHeader(request, "x-ms-documentdb-pre-trigger-include", rntbdRequest.preTriggerInclude);
      TransportSerialization.FillTokenFromHeader(request, "x-ms-documentdb-pre-trigger-exclude", rntbdRequest.preTriggerExclude);
      TransportSerialization.FillTokenFromHeader(request, "x-ms-documentdb-post-trigger-include", rntbdRequest.postTriggerInclude);
      TransportSerialization.FillTokenFromHeader(request, "x-ms-documentdb-post-trigger-exclude", rntbdRequest.postTriggerExclude);
      TransportSerialization.FillTokenFromHeader(request, "x-ms-documentdb-partitionkey", rntbdRequest.partitionKey);
      TransportSerialization.FillTokenFromHeader(request, "x-ms-documentdb-partitionkeyrangeid", rntbdRequest.partitionKeyRangeId);
      TransportSerialization.FillTokenFromHeader(request, "x-ms-documentdb-expiry-seconds", rntbdRequest.resourceTokenExpiry);
      TransportSerialization.FillTokenFromHeader(request, "x-ms-documentdb-filterby-schema-rid", rntbdRequest.filterBySchemaRid);
      TransportSerialization.FillTokenFromHeader(request, "x-ms-cosmos-batch-continue-on-error", rntbdRequest.shouldBatchContinueOnError);
      TransportSerialization.FillTokenFromHeader(request, "x-ms-cosmos-batch-ordered", rntbdRequest.isBatchOrdered);
      TransportSerialization.FillTokenFromHeader(request, "x-ms-cosmos-batch-atomic", rntbdRequest.isBatchAtomic);
      TransportSerialization.FillTokenFromHeader(request, "collection-partition-index", rntbdRequest.collectionPartitionIndex);
      TransportSerialization.FillTokenFromHeader(request, "collection-service-index", rntbdRequest.collectionServiceIndex);
      TransportSerialization.FillTokenFromHeader(request, "x-ms-resource-schema-name", rntbdRequest.resourceSchemaName);
      TransportSerialization.FillTokenFromHeader(request, "x-ms-bind-replica", rntbdRequest.bindReplicaDirective);
      TransportSerialization.FillTokenFromHeader(request, "x-ms-primary-master-key", rntbdRequest.primaryMasterKey);
      TransportSerialization.FillTokenFromHeader(request, "x-ms-secondary-master-key", rntbdRequest.secondaryMasterKey);
      TransportSerialization.FillTokenFromHeader(request, "x-ms-primary-readonly-key", rntbdRequest.primaryReadonlyKey);
      TransportSerialization.FillTokenFromHeader(request, "x-ms-secondary-readonly-key", rntbdRequest.secondaryReadonlyKey);
      TransportSerialization.FillTokenFromHeader(request, "x-ms-documentdb-partitioncount", rntbdRequest.partitionCount);
      TransportSerialization.FillTokenFromHeader(request, "x-ms-documentdb-collection-rid", rntbdRequest.collectionRid);
      TransportSerialization.FillTokenFromHeader(request, "x-ms-gateway-signature", rntbdRequest.gatewaySignature);
      TransportSerialization.FillTokenFromHeader(request, "x-ms-remaining-time-in-ms-on-client", rntbdRequest.remainingTimeInMsOnClientRequest);
      TransportSerialization.FillTokenFromHeader(request, "x-ms-client-retry-attempt-count", rntbdRequest.clientRetryAttemptCount);
      TransportSerialization.FillTokenFromHeader(request, "x-ms-target-lsn", rntbdRequest.targetLsn);
      TransportSerialization.FillTokenFromHeader(request, "x-ms-target-global-committed-lsn", rntbdRequest.targetGlobalCommittedLsn);
      TransportSerialization.FillTokenFromHeader(request, "x-ms-transport-request-id", rntbdRequest.transportRequestID);
      TransportSerialization.FillTokenFromHeader(request, "x-ms-restore-metadata-filter", rntbdRequest.restoreMetadataFilter);
      TransportSerialization.FillTokenFromHeader(request, "x-ms-restore-params", rntbdRequest.restoreParams);
      TransportSerialization.FillTokenFromHeader(request, "x-ms-partition-resource-filter", rntbdRequest.partitionResourceFilter);
      TransportSerialization.FillTokenFromHeader(request, "x-ms-enable-dynamic-rid-range-allocation", rntbdRequest.enableDynamicRidRangeAllocation);
      TransportSerialization.FillTokenFromHeader(request, "x-ms-schema-owner-rid", rntbdRequest.schemaOwnerRid);
      TransportSerialization.FillTokenFromHeader(request, "x-ms-schema-hash", rntbdRequest.schemaHash);
      TransportSerialization.AddBinaryIdIfPresent(request, rntbdRequest);
      TransportSerialization.FillTokenFromHeader(request, "x-ms-time-to-live-in-seconds", rntbdRequest.timeToLiveInSeconds);
      TransportSerialization.AddEffectivePartitionKeyIfPresent(request, rntbdRequest);
      TransportSerialization.FillTokenFromHeader(request, "x-ms-binary-passthrough-request", rntbdRequest.binaryPassthroughRequest);
      TransportSerialization.FillTokenFromHeader(request, "x-ms-cosmos-allow-tentative-writes", rntbdRequest.allowTentativeWrites);
      TransportSerialization.FillTokenFromHeader(request, "x-ms-cosmos-include-tentative-writes", rntbdRequest.includeTentativeWrites);
      TransportSerialization.AddMergeStaticIdIfPresent(request, rntbdRequest);
      TransportSerialization.FillTokenFromHeader(request, "x-ms-cosmos-max-polling-interval", rntbdRequest.maxPollingIntervalMilliseconds);
      TransportSerialization.FillTokenFromHeader(request, "x-ms-cosmos-populate-logstoreinfo", rntbdRequest.populateLogStoreInfo);
      TransportSerialization.FillTokenFromHeader(request, "x-ms-cosmos-internal-merge-checkpoint-glsn", rntbdRequest.mergeCheckpointGlsnKeyName);
      TransportSerialization.FillTokenFromHeader(request, "x-ms-version", rntbdRequest.clientVersion);
      byte[] byteArray = activityId.ToByteArray();
      int num1 = 8 + byteArray.Length;
      int num2 = 0;
      int num3 = 0;
      CloneableStream cloneableStream = (CloneableStream) null;
      if (request.CloneableBody != null)
      {
        cloneableStream = request.CloneableBody.Clone();
        num3 = (int) cloneableStream.Length;
      }
      byte[] buffer = (byte[]) null;
      using (cloneableStream)
      {
        if (num3 > 0)
        {
          num2 = num2 + 4 + num3;
          rntbdRequest.payloadPresent.value.valueByte = (byte) 1;
          rntbdRequest.payloadPresent.isPresent = true;
        }
        else
        {
          rntbdRequest.payloadPresent.value.valueByte = (byte) 0;
          rntbdRequest.payloadPresent.isPresent = true;
        }
        num1 += rntbdRequest.CalculateLength();
        buffer = new byte[num2 + num1];
        using (MemoryStream memoryStream = new MemoryStream(buffer, true))
        {
          using (BinaryWriter writer = new BinaryWriter((Stream) memoryStream))
          {
            writer.Write((uint) num1);
            writer.Write((ushort) rntbdResourceType);
            writer.Write((ushort) rntbdOperationType);
            writer.Write(byteArray);
            int num4 = 8 + byteArray.Length;
            int tokensLength = 0;
            rntbdRequest.SerializeToBinaryWriter(writer, out tokensLength);
            int num5 = num4 + tokensLength;
            if (num5 != num1)
            {
              DefaultTrace.TraceCritical("Bug in RNTBD token serialization. Calculated header size: {0}. Actual header size: {1}", (object) num1, (object) num5);
              throw new InternalServerErrorException();
            }
            if (num3 > 0)
            {
              writer.Write((uint) num3);
              writer.Flush();
              cloneableStream.WriteTo((Stream) memoryStream);
            }
            writer.Flush();
          }
        }
      }
      headerSize = num1;
      bodySize = 4 + num3;
      if (headerSize > 131072)
        DefaultTrace.TraceWarning("The request header is large. Header size: {0}. Warning threshold: {1}. RID: {2}. Resource type: {3}. Operation: {4}. Address: {5}", (object) headerSize, (object) 131072, (object) request.ResourceAddress, (object) request.ResourceType, (object) resourceOperation, (object) replicaPath);
      if (bodySize > 2097152)
        DefaultTrace.TraceWarning("The request body is large. Body size: {0}. Warning threshold: {1}. RID: {2}. Resource type: {3}. Operation: {4}. Address: {5}", (object) bodySize, (object) 2097152, (object) request.ResourceAddress, (object) request.ResourceType, (object) resourceOperation, (object) replicaPath);
      return buffer;
    }

    internal static byte[] BuildContextRequest(
      Guid activityId,
      UserAgentContainer userAgent,
      RntbdConstants.CallerId callerId)
    {
      byte[] byteArray = activityId.ToByteArray();
      RntbdConstants.ConnectionContextRequest connectionContextRequest = new RntbdConstants.ConnectionContextRequest();
      connectionContextRequest.protocolVersion.value.valueULong = 1U;
      connectionContextRequest.protocolVersion.isPresent = true;
      connectionContextRequest.clientVersion.value.valueBytes = HttpConstants.Versions.CurrentVersionUTF8;
      connectionContextRequest.clientVersion.isPresent = true;
      connectionContextRequest.userAgent.value.valueBytes = userAgent.UserAgentUTF8;
      connectionContextRequest.userAgent.isPresent = true;
      connectionContextRequest.callerId.isPresent = false;
      if (callerId == RntbdConstants.CallerId.Gateway)
      {
        connectionContextRequest.callerId.value.valueByte = (byte) callerId;
        connectionContextRequest.callerId.isPresent = true;
      }
      int length = 8 + byteArray.Length + connectionContextRequest.CalculateLength();
      byte[] buffer = new byte[length];
      using (MemoryStream output = new MemoryStream(buffer, true))
      {
        using (BinaryWriter writer = new BinaryWriter((Stream) output))
        {
          writer.Write(length);
          writer.Write((ushort) 0);
          writer.Write((ushort) 0);
          writer.Write(byteArray);
          int tokensLength = 0;
          connectionContextRequest.SerializeToBinaryWriter(writer, out tokensLength);
          writer.Flush();
        }
      }
      return buffer;
    }

    internal static StoreResponse MakeStoreResponse(
      StatusCodes status,
      Guid activityId,
      RntbdConstants.Response response,
      Stream body,
      string serverVersion)
    {
      List<string> headerNames = new List<string>(response.tokens.Length);
      List<string> headerValues = new List<string>(response.tokens.Length);
      TransportSerialization.AddResponseStringHeaderIfPresent(response.lastStateChangeDateTime, "x-ms-last-state-change-utc", headerNames, headerValues);
      TransportSerialization.AddResponseStringHeaderIfPresent(response.continuationToken, "x-ms-continuation", headerNames, headerValues);
      TransportSerialization.AddResponseStringHeaderIfPresent(response.eTag, "etag", headerNames, headerValues);
      TransportSerialization.AddResponseULongHeaderIfPresent(response.retryAfterMilliseconds, "x-ms-retry-after-ms", headerNames, headerValues);
      TransportSerialization.AddResponseStringHeaderIfPresent(response.storageMaxResoureQuota, "x-ms-resource-quota", headerNames, headerValues);
      TransportSerialization.AddResponseStringHeaderIfPresent(response.storageResourceQuotaUsage, "x-ms-resource-usage", headerNames, headerValues);
      TransportSerialization.AddResponseULongHeaderIfPresent(response.collectionPartitionIndex, "collection-partition-index", headerNames, headerValues);
      TransportSerialization.AddResponseULongHeaderIfPresent(response.collectionServiceIndex, "collection-service-index", headerNames, headerValues);
      TransportSerialization.AddResponseLongLongHeaderIfPresent(response.LSN, "lsn", headerNames, headerValues);
      TransportSerialization.AddResponseULongHeaderIfPresent(response.itemCount, "x-ms-item-count", headerNames, headerValues);
      TransportSerialization.AddResponseStringHeaderIfPresent(response.schemaVersion, "x-ms-schemaversion", headerNames, headerValues);
      TransportSerialization.AddResponseStringHeaderIfPresent(response.ownerFullName, "x-ms-alt-content-path", headerNames, headerValues);
      TransportSerialization.AddResponseStringHeaderIfPresent(response.ownerId, "x-ms-content-path", headerNames, headerValues);
      TransportSerialization.AddResponseStringHeaderIfPresent(response.databaseAccountId, "x-ms-database-account-id", headerNames, headerValues);
      TransportSerialization.AddResponseLongLongHeaderIfPresent(response.quorumAckedLSN, "x-ms-quorum-acked-lsn", headerNames, headerValues);
      TransportSerialization.AddResponseByteHeaderIfPresent(response.requestValidationFailure, "x-ms-request-validation-failure", headerNames, headerValues);
      TransportSerialization.AddResponseULongHeaderIfPresent(response.subStatus, "x-ms-substatus", headerNames, headerValues);
      TransportSerialization.AddResponseULongHeaderIfPresent(response.collectionUpdateProgress, "x-ms-documentdb-collection-index-transformation-progress", headerNames, headerValues);
      TransportSerialization.AddResponseULongHeaderIfPresent(response.currentWriteQuorum, "x-ms-current-write-quorum", headerNames, headerValues);
      TransportSerialization.AddResponseULongHeaderIfPresent(response.currentReplicaSetSize, "x-ms-current-replica-set-size", headerNames, headerValues);
      TransportSerialization.AddResponseULongHeaderIfPresent(response.collectionLazyIndexProgress, "x-ms-documentdb-collection-lazy-indexing-progress", headerNames, headerValues);
      TransportSerialization.AddResponseStringHeaderIfPresent(response.partitionKeyRangeId, "x-ms-documentdb-partitionkeyrangeid", headerNames, headerValues);
      TransportSerialization.AddResponseStringHeaderIfPresent(response.logResults, "x-ms-documentdb-script-log-results", headerNames, headerValues);
      TransportSerialization.AddResponseULongHeaderIfPresent(response.xpRole, "x-ms-xp-role", headerNames, headerValues);
      TransportSerialization.AddResponseByteHeaderIfPresent(response.isRUPerMinuteUsed, "x-ms-documentdb-is-ru-per-minute-used", headerNames, headerValues);
      TransportSerialization.AddResponseStringHeaderIfPresent(response.queryMetrics, "x-ms-documentdb-query-metrics", headerNames, headerValues);
      TransportSerialization.AddResponseStringHeaderIfPresent(response.indexUtilization, "x-ms-cosmos-index-utilization", headerNames, headerValues);
      TransportSerialization.AddResponseLongLongHeaderIfPresent(response.globalCommittedLSN, "x-ms-global-Committed-lsn", headerNames, headerValues);
      TransportSerialization.AddResponseULongHeaderIfPresent(response.numberOfReadRegions, "x-ms-number-of-read-regions", headerNames, headerValues);
      TransportSerialization.AddResponseBoolHeaderIfPresent(response.offerReplacePending, "x-ms-offer-replace-pending", headerNames, headerValues);
      TransportSerialization.AddResponseLongLongHeaderIfPresent(response.itemLSN, "x-ms-item-lsn", headerNames, headerValues);
      TransportSerialization.AddResponseStringHeaderIfPresent(response.restoreState, "x-ms-restore-state", headerNames, headerValues);
      TransportSerialization.AddResponseStringHeaderIfPresent(response.collectionSecurityIdentifier, "x-ms-collection-security-identifier", headerNames, headerValues);
      TransportSerialization.AddResponseULongHeaderIfPresent(response.transportRequestID, "x-ms-transport-request-id", headerNames, headerValues);
      TransportSerialization.AddResponseBoolHeaderIfPresent(response.shareThroughput, "x-ms-share-throughput", headerNames, headerValues);
      TransportSerialization.AddResponseBoolHeaderIfPresent(response.disableRntbdChannel, "x-ms-disable-rntbd-channel", headerNames, headerValues);
      TransportSerialization.AddResponseStringHeaderIfPresent(response.serverDateTimeUtc, "x-ms-date", headerNames, headerValues);
      TransportSerialization.AddResponseLongLongHeaderIfPresent(response.localLSN, "x-ms-cosmos-llsn", headerNames, headerValues);
      TransportSerialization.AddResponseLongLongHeaderIfPresent(response.quorumAckedLocalLSN, "x-ms-cosmos-quorum-acked-llsn", headerNames, headerValues);
      TransportSerialization.AddResponseLongLongHeaderIfPresent(response.itemLocalLSN, "x-ms-cosmos-item-llsn", headerNames, headerValues);
      TransportSerialization.AddResponseBoolHeaderIfPresent(response.hasTentativeWrites, "x-ms-cosmosdb-has-tentative-writes", headerNames, headerValues);
      TransportSerialization.AddResponseStringHeaderIfPresent(response.sessionToken, "x-ms-session-token", headerNames, headerValues);
      TransportSerialization.AddResponseLongLongHeaderIfPresent(response.replicatorLSNToGLSNDelta, "x-ms-cosmos-replicator-glsn-delta", headerNames, headerValues);
      TransportSerialization.AddResponseLongLongHeaderIfPresent(response.replicatorLSNToLLSNDelta, "x-ms-cosmos-replicator-llsn-delta", headerNames, headerValues);
      TransportSerialization.AddResponseLongLongHeaderIfPresent(response.vectorClockLocalProgress, "x-ms-cosmos-vectorclock-local-progress", headerNames, headerValues);
      TransportSerialization.AddResponseULongHeaderIfPresent(response.minimumRUsForOffer, "x-ms-cosmos-min-throughput", headerNames, headerValues);
      TransportSerialization.AddResponseULongHeaderIfPresent(response.xpConfigurationSesssionsCount, "x-ms-cosmos-xpconfiguration-sessions-count", headerNames, headerValues);
      if (response.requestCharge.isPresent)
      {
        headerNames.Add("x-ms-request-charge");
        headerValues.Add(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0:0.##}", (object) response.requestCharge.value.valueDouble));
      }
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
        headerNames.Add("x-ms-indexing-directive");
        headerValues.Add(str);
      }
      headerNames.Add("x-ms-serviceversion");
      headerValues.Add(serverVersion);
      headerNames.Add("x-ms-activity-id");
      headerValues.Add(activityId.ToString());
      return new StoreResponse()
      {
        ResponseBody = body,
        Status = (int) status,
        ResponseHeaderValues = headerValues.ToArray(),
        ResponseHeaderNames = headerNames.ToArray()
      };
    }

    internal static TransportSerialization.RntbdHeader DecodeRntbdHeader(byte[] header)
    {
      int uint32 = (int) BitConverter.ToUInt32(header, 4);
      byte[] numArray = new byte[16];
      Buffer.BlockCopy((Array) header, 8, (Array) numArray, 0, 16);
      Guid activityId = new Guid(numArray);
      return new TransportSerialization.RntbdHeader((StatusCodes) uint32, activityId);
    }

    private static void AddResponseByteHeaderIfPresent(
      RntbdToken token,
      string header,
      List<string> headerNames,
      List<string> headerValues)
    {
      if (!token.isPresent)
        return;
      headerNames.Add(header);
      headerValues.Add(token.value.valueByte.ToString((IFormatProvider) CultureInfo.InvariantCulture));
    }

    private static void AddResponseBoolHeaderIfPresent(
      RntbdToken token,
      string header,
      List<string> headerNames,
      List<string> headerValues)
    {
      if (!token.isPresent)
        return;
      headerNames.Add(header);
      headerValues.Add((token.value.valueByte > (byte) 0).ToString().ToLowerInvariant());
    }

    private static void AddResponseStringHeaderIfPresent(
      RntbdToken token,
      string header,
      List<string> headerNames,
      List<string> headerValues)
    {
      if (!token.isPresent)
        return;
      headerNames.Add(header);
      headerValues.Add(Encoding.UTF8.GetString(token.value.valueBytes));
    }

    private static void AddResponseULongHeaderIfPresent(
      RntbdToken token,
      string header,
      List<string> headerNames,
      List<string> headerValues)
    {
      if (!token.isPresent)
        return;
      headerNames.Add(header);
      headerValues.Add(token.value.valueULong.ToString((IFormatProvider) CultureInfo.InvariantCulture));
    }

    private static void AddResponseDoubleHeaderIfPresent(
      RntbdToken token,
      string header,
      List<string> headerNames,
      List<string> headerValues)
    {
      if (!token.isPresent)
        return;
      headerNames.Add(header);
      headerValues.Add(token.value.valueDouble.ToString((IFormatProvider) CultureInfo.InvariantCulture));
    }

    private static void AddResponseFloatHeaderIfPresent(
      RntbdToken token,
      string header,
      List<string> headerNames,
      List<string> headerValues)
    {
      if (!token.isPresent)
        return;
      headerNames.Add(header);
      headerValues.Add(token.value.valueFloat.ToString((IFormatProvider) CultureInfo.InvariantCulture));
    }

    private static void AddResponseLongLongHeaderIfPresent(
      RntbdToken token,
      string header,
      List<string> headerNames,
      List<string> headerValues)
    {
      if (!token.isPresent)
        return;
      headerNames.Add(header);
      headerValues.Add(token.value.valueLongLong.ToString((IFormatProvider) CultureInfo.InvariantCulture));
    }

    private static RntbdConstants.RntbdOperationType GetRntbdOperationType(
      OperationType operationType)
    {
      switch (operationType)
      {
        case OperationType.ForceConfigRefresh:
          return RntbdConstants.RntbdOperationType.ForceConfigRefresh;
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
        case OperationType.Pause:
          return RntbdConstants.RntbdOperationType.Pause;
        case OperationType.Resume:
          return RntbdConstants.RntbdOperationType.Resume;
        case OperationType.Stop:
          return RntbdConstants.RntbdOperationType.Stop;
        case OperationType.Recycle:
          return RntbdConstants.RntbdOperationType.Recycle;
        case OperationType.Crash:
          return RntbdConstants.RntbdOperationType.Crash;
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
        case OperationType.Recreate:
          return RntbdConstants.RntbdOperationType.Recreate;
        case OperationType.Throttle:
          return RntbdConstants.RntbdOperationType.Throttle;
        case OperationType.GetSplitPoint:
          return RntbdConstants.RntbdOperationType.GetSplitPoint;
        case OperationType.PreCreateValidation:
          return RntbdConstants.RntbdOperationType.PreCreateValidation;
        case OperationType.AbortSplit:
          return RntbdConstants.RntbdOperationType.AbortSplit;
        case OperationType.CompleteSplit:
          return RntbdConstants.RntbdOperationType.CompleteSplit;
        case OperationType.CompletePartitionMigration:
          return RntbdConstants.RntbdOperationType.CompletePartitionMigration;
        case OperationType.AbortPartitionMigration:
          return RntbdConstants.RntbdOperationType.AbortPartitionMigration;
        case OperationType.OfferUpdateOperation:
          return RntbdConstants.RntbdOperationType.OfferUpdateOperation;
        case OperationType.OfferPreGrowValidation:
          return RntbdConstants.RntbdOperationType.OfferPreGrowValidation;
        case OperationType.BatchReportThroughputUtilization:
          return RntbdConstants.RntbdOperationType.BatchReportThroughputUtilization;
        case OperationType.PreReplaceValidation:
          return RntbdConstants.RntbdOperationType.PreReplaceValidation;
        case OperationType.MigratePartition:
          return RntbdConstants.RntbdOperationType.MigratePartition;
        case OperationType.AddComputeGatewayRequestCharges:
          return RntbdConstants.RntbdOperationType.AddComputeGatewayRequestCharges;
        case OperationType.MasterReplaceOfferOperation:
          return RntbdConstants.RntbdOperationType.MasterReplaceOfferOperation;
        case OperationType.ProvisionedCollectionOfferUpdateOperation:
          return RntbdConstants.RntbdOperationType.ProvisionedCollectionOfferUpdateOperation;
        case OperationType.Batch:
          return RntbdConstants.RntbdOperationType.Batch;
        case OperationType.InitiateDatabaseOfferPartitionShrink:
          return RntbdConstants.RntbdOperationType.InitiateDatabaseOfferPartitionShrink;
        case OperationType.CompleteDatabaseOfferPartitionShrink:
          return RntbdConstants.RntbdOperationType.CompleteDatabaseOfferPartitionShrink;
        case OperationType.EnsureSnapshotOperation:
          return RntbdConstants.RntbdOperationType.EnsureSnapshotOperation;
        case OperationType.GetSplitPoints:
          return RntbdConstants.RntbdOperationType.GetSplitPoints;
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
        case ResourceType.Replica:
          return RntbdConstants.RntbdResourceType.Replica;
        case ResourceType.Module:
          return RntbdConstants.RntbdResourceType.Module;
        case ResourceType.ModuleCommand:
          return RntbdConstants.RntbdResourceType.ModuleCommand;
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
        case ResourceType.PartitionSetInformation:
          return RntbdConstants.RntbdResourceType.PartitionSetInformation;
        case ResourceType.XPReplicatorAddress:
          return RntbdConstants.RntbdResourceType.XPReplicatorAddress;
        case ResourceType.DatabaseAccount:
          return RntbdConstants.RntbdResourceType.DatabaseAccount;
        case ResourceType.MasterPartition:
          return RntbdConstants.RntbdResourceType.MasterPartition;
        case ResourceType.ServerPartition:
          return RntbdConstants.RntbdResourceType.ServerPartition;
        case ResourceType.Topology:
          return RntbdConstants.RntbdResourceType.Topology;
        case ResourceType.Schema:
          return RntbdConstants.RntbdResourceType.Schema;
        case ResourceType.PartitionKeyRange:
          return RntbdConstants.RntbdResourceType.PartitionKeyRange;
        case ResourceType.RestoreMetadata:
          return RntbdConstants.RntbdResourceType.RestoreMetadata;
        case ResourceType.VectorClock:
          return RntbdConstants.RntbdResourceType.VectorClock;
        case ResourceType.RidRange:
          return RntbdConstants.RntbdResourceType.RidRange;
        case ResourceType.ComputeGatewayCharges:
          return RntbdConstants.RntbdResourceType.ComputeGatewayCharges;
        case ResourceType.UserDefinedType:
          return RntbdConstants.RntbdResourceType.UserDefinedType;
        case ResourceType.PartitionKey:
          return RntbdConstants.RntbdResourceType.PartitionKey;
        case ResourceType.Snapshot:
          return RntbdConstants.RntbdResourceType.Snapshot;
        case ResourceType.ClientEncryptionKey:
          return RntbdConstants.RntbdResourceType.ClientEncryptionKey;
        default:
          throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Invalid resource type: {0}", (object) resourceType), nameof (resourceType));
      }
    }

    private static void AddMatchHeader(
      DocumentServiceRequest request,
      RntbdConstants.RntbdOperationType operationType,
      RntbdConstants.Request rntbdRequest)
    {
      string header;
      switch (operationType)
      {
        case RntbdConstants.RntbdOperationType.Read:
        case RntbdConstants.RntbdOperationType.ReadFeed:
          header = request.Headers["If-None-Match"];
          break;
        default:
          header = request.Headers["If-Match"];
          break;
      }
      if (string.IsNullOrEmpty(header))
        return;
      rntbdRequest.match.value.valueBytes = Encoding.UTF8.GetBytes(header);
      rntbdRequest.match.isPresent = true;
    }

    private static void AddIfModifiedSinceHeader(
      DocumentServiceRequest request,
      RntbdConstants.Request rntbdRequest)
    {
      string header = request.Headers["If-Modified-Since"];
      if (string.IsNullOrEmpty(header))
        return;
      rntbdRequest.ifModifiedSince.value.valueBytes = Encoding.UTF8.GetBytes(header);
      rntbdRequest.ifModifiedSince.isPresent = true;
    }

    private static void AddA_IMHeader(
      DocumentServiceRequest request,
      RntbdConstants.Request rntbdRequest)
    {
      string header = request.Headers["A-IM"];
      if (string.IsNullOrEmpty(header))
        return;
      rntbdRequest.a_IM.value.valueBytes = Encoding.UTF8.GetBytes(header);
      rntbdRequest.a_IM.isPresent = true;
    }

    private static void AddDateHeader(
      DocumentServiceRequest request,
      RntbdConstants.Request rntbdRequest)
    {
      string dateHeader = Helpers.GetDateHeader(request.Headers);
      if (string.IsNullOrEmpty(dateHeader))
        return;
      rntbdRequest.date.value.valueBytes = Encoding.UTF8.GetBytes(dateHeader);
      rntbdRequest.date.isPresent = true;
    }

    private static void AddContinuation(
      DocumentServiceRequest request,
      RntbdConstants.Request rntbdRequest)
    {
      if (string.IsNullOrEmpty(request.Continuation))
        return;
      rntbdRequest.continuationToken.value.valueBytes = Encoding.UTF8.GetBytes(request.Continuation);
      rntbdRequest.continuationToken.isPresent = true;
    }

    private static void AddResourceIdOrPathHeaders(
      DocumentServiceRequest request,
      RntbdConstants.Request rntbdRequest)
    {
      if (!string.IsNullOrEmpty(request.ResourceId))
      {
        rntbdRequest.resourceId.value.valueBytes = ResourceId.Parse(request.ResourceType, request.ResourceId);
        rntbdRequest.resourceId.isPresent = true;
      }
      if (!request.IsNameBased)
        return;
      string[] strArray = request.ResourceAddress.Split(TransportSerialization.UrlTrim, StringSplitOptions.RemoveEmptyEntries);
      if (strArray.Length >= 2)
      {
        switch (strArray[0])
        {
          case "dbs":
            rntbdRequest.databaseName.value.valueBytes = Encoding.UTF8.GetBytes(strArray[1]);
            rntbdRequest.databaseName.isPresent = true;
            break;
          case "snapshots":
            rntbdRequest.snapshotName.value.valueBytes = Encoding.UTF8.GetBytes(strArray[1]);
            rntbdRequest.snapshotName.isPresent = true;
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
            rntbdRequest.collectionName.value.valueBytes = Encoding.UTF8.GetBytes(strArray[3]);
            rntbdRequest.collectionName.isPresent = true;
            break;
          case "clientencryptionkeys":
            rntbdRequest.clientEncryptionKeyName.value.valueBytes = Encoding.UTF8.GetBytes(strArray[3]);
            rntbdRequest.clientEncryptionKeyName.isPresent = true;
            break;
          case "users":
            rntbdRequest.userName.value.valueBytes = Encoding.UTF8.GetBytes(strArray[3]);
            rntbdRequest.userName.isPresent = true;
            break;
          case "udts":
            rntbdRequest.userDefinedTypeName.value.valueBytes = Encoding.UTF8.GetBytes(strArray[3]);
            rntbdRequest.userDefinedTypeName.isPresent = true;
            break;
        }
      }
      if (strArray.Length >= 6)
      {
        switch (strArray[4])
        {
          case "conflicts":
            rntbdRequest.conflictName.value.valueBytes = Encoding.UTF8.GetBytes(strArray[5]);
            rntbdRequest.conflictName.isPresent = true;
            break;
          case "docs":
            rntbdRequest.documentName.value.valueBytes = Encoding.UTF8.GetBytes(strArray[5]);
            rntbdRequest.documentName.isPresent = true;
            break;
          case "permissions":
            rntbdRequest.permissionName.value.valueBytes = Encoding.UTF8.GetBytes(strArray[5]);
            rntbdRequest.permissionName.isPresent = true;
            break;
          case "pkranges":
            rntbdRequest.partitionKeyRangeName.value.valueBytes = Encoding.UTF8.GetBytes(strArray[5]);
            rntbdRequest.partitionKeyRangeName.isPresent = true;
            break;
          case "schemas":
            rntbdRequest.schemaName.value.valueBytes = Encoding.UTF8.GetBytes(strArray[5]);
            rntbdRequest.schemaName.isPresent = true;
            break;
          case "sprocs":
            rntbdRequest.storedProcedureName.value.valueBytes = Encoding.UTF8.GetBytes(strArray[5]);
            rntbdRequest.storedProcedureName.isPresent = true;
            break;
          case "triggers":
            rntbdRequest.triggerName.value.valueBytes = Encoding.UTF8.GetBytes(strArray[5]);
            rntbdRequest.triggerName.isPresent = true;
            break;
          case "udfs":
            rntbdRequest.userDefinedFunctionName.value.valueBytes = Encoding.UTF8.GetBytes(strArray[5]);
            rntbdRequest.userDefinedFunctionName.isPresent = true;
            break;
        }
      }
      if (strArray.Length < 8 || !(strArray[6] == "attachments"))
        return;
      rntbdRequest.attachmentName.value.valueBytes = Encoding.UTF8.GetBytes(strArray[7]);
      rntbdRequest.attachmentName.isPresent = true;
    }

    private static void AddBinaryIdIfPresent(
      DocumentServiceRequest request,
      RntbdConstants.Request rntbdRequest)
    {
      object obj;
      if (request.Properties == null || !request.Properties.TryGetValue("x-ms-binary-id", out obj))
        return;
      rntbdRequest.binaryId.value.valueBytes = obj is byte[] numArray ? numArray : throw new ArgumentOutOfRangeException("x-ms-binary-id");
      rntbdRequest.binaryId.isPresent = true;
    }

    private static void AddEffectivePartitionKeyIfPresent(
      DocumentServiceRequest request,
      RntbdConstants.Request rntbdRequest)
    {
      object obj;
      if (request.Properties == null || !request.Properties.TryGetValue("x-ms-effective-partition-key", out obj))
        return;
      rntbdRequest.effectivePartitionKey.value.valueBytes = obj is byte[] numArray ? numArray : throw new ArgumentOutOfRangeException("x-ms-effective-partition-key");
      rntbdRequest.effectivePartitionKey.isPresent = true;
    }

    private static void AddMergeStaticIdIfPresent(
      DocumentServiceRequest request,
      RntbdConstants.Request rntbdRequest)
    {
      object obj;
      if (request.Properties == null || !request.Properties.TryGetValue("x-ms-cosmos-merge-static-id", out obj))
        return;
      rntbdRequest.mergeStaticId.value.valueBytes = obj is byte[] numArray ? numArray : throw new ArgumentOutOfRangeException("x-ms-cosmos-merge-static-id");
      rntbdRequest.mergeStaticId.isPresent = true;
    }

    private static void AddEntityId(
      DocumentServiceRequest request,
      RntbdConstants.Request rntbdRequest)
    {
      if (string.IsNullOrEmpty(request.EntityId))
        return;
      rntbdRequest.entityId.value.valueBytes = Encoding.UTF8.GetBytes(request.EntityId);
      rntbdRequest.entityId.isPresent = true;
    }

    private static void AddIndexingDirectiveHeader(
      DocumentServiceRequest request,
      RntbdConstants.Request rntbdRequest)
    {
      if (string.IsNullOrEmpty(request.Headers["x-ms-indexing-directive"]))
        return;
      IndexingDirective result;
      if (!Enum.TryParse<IndexingDirective>(request.Headers["x-ms-indexing-directive"], true, out result))
        throw new BadRequestException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.InvalidEnumValue, (object) request.Headers["x-ms-indexing-directive"], (object) typeof (IndexingDirective).Name));
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
          throw new BadRequestException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.InvalidEnumValue, (object) request.Headers["x-ms-indexing-directive"], (object) typeof (IndexingDirective).Name));
      }
      rntbdRequest.indexingDirective.value.valueByte = (byte) indexingDirective;
      rntbdRequest.indexingDirective.isPresent = true;
    }

    private static void AddMigrateCollectionDirectiveHeader(
      DocumentServiceRequest request,
      RntbdConstants.Request rntbdRequest)
    {
      if (string.IsNullOrEmpty(request.Headers["x-ms-migratecollection-directive"]))
        return;
      MigrateCollectionDirective result;
      if (!Enum.TryParse<MigrateCollectionDirective>(request.Headers["x-ms-migratecollection-directive"], true, out result))
        throw new BadRequestException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.InvalidEnumValue, (object) request.Headers["x-ms-migratecollection-directive"], (object) typeof (MigrateCollectionDirective).Name));
      RntbdConstants.RntbdMigrateCollectionDirective collectionDirective;
      if (result != MigrateCollectionDirective.Thaw)
      {
        if (result != MigrateCollectionDirective.Freeze)
          throw new BadRequestException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.InvalidEnumValue, (object) request.Headers["x-ms-migratecollection-directive"], (object) typeof (MigrateCollectionDirective).Name));
        collectionDirective = RntbdConstants.RntbdMigrateCollectionDirective.Freeze;
      }
      else
        collectionDirective = RntbdConstants.RntbdMigrateCollectionDirective.Thaw;
      rntbdRequest.migrateCollectionDirective.value.valueByte = (byte) collectionDirective;
      rntbdRequest.migrateCollectionDirective.isPresent = true;
    }

    private static void AddConsistencyLevelHeader(
      DocumentServiceRequest request,
      RntbdConstants.Request rntbdRequest)
    {
      if (string.IsNullOrEmpty(request.Headers["x-ms-consistency-level"]))
        return;
      ConsistencyLevel result;
      if (!Enum.TryParse<ConsistencyLevel>(request.Headers["x-ms-consistency-level"], true, out result))
        throw new BadRequestException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.InvalidEnumValue, (object) request.Headers["x-ms-consistency-level"], (object) typeof (ConsistencyLevel).Name));
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
          throw new BadRequestException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.InvalidEnumValue, (object) request.Headers["x-ms-consistency-level"], (object) typeof (ConsistencyLevel).Name));
      }
      rntbdRequest.consistencyLevel.value.valueByte = (byte) consistencyLevel;
      rntbdRequest.consistencyLevel.isPresent = true;
    }

    private static void AddIsFanout(
      DocumentServiceRequest request,
      RntbdConstants.Request rntbdRequest)
    {
      if (string.IsNullOrEmpty(request.Headers["x-ms-is-fanout-request"]))
        return;
      rntbdRequest.isFanout.value.valueByte = request.Headers["x-ms-is-fanout-request"].Equals(bool.TrueString) ? (byte) 1 : (byte) 0;
      rntbdRequest.isFanout.isPresent = true;
    }

    private static void AddAllowScanOnQuery(
      DocumentServiceRequest request,
      RntbdConstants.Request rntbdRequest)
    {
      if (string.IsNullOrEmpty(request.Headers["x-ms-documentdb-query-enable-scan"]))
        return;
      rntbdRequest.enableScanInQuery.value.valueByte = request.Headers["x-ms-documentdb-query-enable-scan"].Equals(bool.TrueString, StringComparison.OrdinalIgnoreCase) ? (byte) 1 : (byte) 0;
      rntbdRequest.enableScanInQuery.isPresent = true;
    }

    private static void AddEnableLowPrecisionOrderBy(
      DocumentServiceRequest request,
      RntbdConstants.Request rntbdRequest)
    {
      if (string.IsNullOrEmpty(request.Headers["x-ms-documentdb-query-enable-low-precision-order-by"]))
        return;
      rntbdRequest.enableLowPrecisionOrderBy.value.valueByte = request.Headers["x-ms-documentdb-query-enable-low-precision-order-by"].Equals(bool.TrueString, StringComparison.OrdinalIgnoreCase) ? (byte) 1 : (byte) 0;
      rntbdRequest.enableLowPrecisionOrderBy.isPresent = true;
    }

    private static void AddEmitVerboseTracesInQuery(
      DocumentServiceRequest request,
      RntbdConstants.Request rntbdRequest)
    {
      if (string.IsNullOrEmpty(request.Headers["x-ms-documentdb-query-emit-traces"]))
        return;
      rntbdRequest.emitVerboseTracesInQuery.value.valueByte = request.Headers["x-ms-documentdb-query-emit-traces"].Equals(bool.TrueString, StringComparison.OrdinalIgnoreCase) ? (byte) 1 : (byte) 0;
      rntbdRequest.emitVerboseTracesInQuery.isPresent = true;
    }

    private static void AddCanCharge(
      DocumentServiceRequest request,
      RntbdConstants.Request rntbdRequest)
    {
      if (string.IsNullOrEmpty(request.Headers["x-ms-cancharge"]))
        return;
      rntbdRequest.canCharge.value.valueByte = request.Headers["x-ms-cancharge"].Equals(bool.TrueString, StringComparison.OrdinalIgnoreCase) ? (byte) 1 : (byte) 0;
      rntbdRequest.canCharge.isPresent = true;
    }

    private static void AddCanThrottle(
      DocumentServiceRequest request,
      RntbdConstants.Request rntbdRequest)
    {
      if (string.IsNullOrEmpty(request.Headers["x-ms-canthrottle"]))
        return;
      rntbdRequest.canThrottle.value.valueByte = request.Headers["x-ms-canthrottle"].Equals(bool.TrueString, StringComparison.OrdinalIgnoreCase) ? (byte) 1 : (byte) 0;
      rntbdRequest.canThrottle.isPresent = true;
    }

    private static void AddProfileRequest(
      DocumentServiceRequest request,
      RntbdConstants.Request rntbdRequest)
    {
      if (string.IsNullOrEmpty(request.Headers["x-ms-profile-request"]))
        return;
      rntbdRequest.profileRequest.value.valueByte = request.Headers["x-ms-profile-request"].Equals(bool.TrueString, StringComparison.OrdinalIgnoreCase) ? (byte) 1 : (byte) 0;
      rntbdRequest.profileRequest.isPresent = true;
    }

    private static void AddPageSize(
      DocumentServiceRequest request,
      RntbdConstants.Request rntbdRequest)
    {
      string header = request.Headers["x-ms-max-item-count"];
      if (string.IsNullOrEmpty(header))
        return;
      int result;
      if (!int.TryParse(header, NumberStyles.Integer, (IFormatProvider) CultureInfo.InvariantCulture, out result))
        throw new BadRequestException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.InvalidPageSize, (object) header));
      if (result == -1)
        rntbdRequest.pageSize.value.valueULong = uint.MaxValue;
      else
        rntbdRequest.pageSize.value.valueULong = result >= 0 ? (uint) result : throw new BadRequestException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.InvalidPageSize, (object) header));
      rntbdRequest.pageSize.isPresent = true;
    }

    private static void AddEnableLogging(
      DocumentServiceRequest request,
      RntbdConstants.Request rntbdRequest)
    {
      if (string.IsNullOrEmpty(request.Headers["x-ms-documentdb-script-enable-logging"]))
        return;
      rntbdRequest.enableLogging.value.valueByte = request.Headers["x-ms-documentdb-script-enable-logging"].Equals(bool.TrueString, StringComparison.OrdinalIgnoreCase) ? (byte) 1 : (byte) 0;
      rntbdRequest.enableLogging.isPresent = true;
    }

    private static void AddSupportSpatialLegacyCoordinates(
      DocumentServiceRequest request,
      RntbdConstants.Request rntbdRequest)
    {
      if (string.IsNullOrEmpty(request.Headers["x-ms-documentdb-supportspatiallegacycoordinates"]))
        return;
      rntbdRequest.supportSpatialLegacyCoordinates.value.valueByte = request.Headers["x-ms-documentdb-supportspatiallegacycoordinates"].Equals(bool.TrueString, StringComparison.OrdinalIgnoreCase) ? (byte) 1 : (byte) 0;
      rntbdRequest.supportSpatialLegacyCoordinates.isPresent = true;
    }

    private static void AddUsePolygonsSmallerThanAHemisphere(
      DocumentServiceRequest request,
      RntbdConstants.Request rntbdRequest)
    {
      if (string.IsNullOrEmpty(request.Headers["x-ms-documentdb-usepolygonssmallerthanahemisphere"]))
        return;
      rntbdRequest.usePolygonsSmallerThanAHemisphere.value.valueByte = request.Headers["x-ms-documentdb-usepolygonssmallerthanahemisphere"].Equals(bool.TrueString, StringComparison.OrdinalIgnoreCase) ? (byte) 1 : (byte) 0;
      rntbdRequest.usePolygonsSmallerThanAHemisphere.isPresent = true;
    }

    private static void AddPopulateQuotaInfo(
      DocumentServiceRequest request,
      RntbdConstants.Request rntbdRequest)
    {
      if (string.IsNullOrEmpty(request.Headers["x-ms-documentdb-populatequotainfo"]))
        return;
      rntbdRequest.populateQuotaInfo.value.valueByte = request.Headers["x-ms-documentdb-populatequotainfo"].Equals(bool.TrueString, StringComparison.OrdinalIgnoreCase) ? (byte) 1 : (byte) 0;
      rntbdRequest.populateQuotaInfo.isPresent = true;
    }

    private static void AddPopulateResourceCount(
      DocumentServiceRequest request,
      RntbdConstants.Request rntbdRequest)
    {
      if (string.IsNullOrEmpty(request.Headers["x-ms-documentdb-populateresourcecount"]))
        return;
      rntbdRequest.populateResourceCount.value.valueByte = request.Headers["x-ms-documentdb-populateresourcecount"].Equals(bool.TrueString, StringComparison.OrdinalIgnoreCase) ? (byte) 1 : (byte) 0;
      rntbdRequest.populateResourceCount.isPresent = true;
    }

    private static void AddPopulatePartitionStatistics(
      DocumentServiceRequest request,
      RntbdConstants.Request rntbdRequest)
    {
      if (string.IsNullOrEmpty(request.Headers["x-ms-documentdb-populatepartitionstatistics"]))
        return;
      rntbdRequest.populatePartitionStatistics.value.valueByte = request.Headers["x-ms-documentdb-populatepartitionstatistics"].Equals(bool.TrueString, StringComparison.OrdinalIgnoreCase) ? (byte) 1 : (byte) 0;
      rntbdRequest.populatePartitionStatistics.isPresent = true;
    }

    private static void AddDisableRUPerMinuteUsage(
      DocumentServiceRequest request,
      RntbdConstants.Request rntbdRequest)
    {
      if (string.IsNullOrEmpty(request.Headers["x-ms-documentdb-disable-ru-per-minute-usage"]))
        return;
      rntbdRequest.disableRUPerMinuteUsage.value.valueByte = request.Headers["x-ms-documentdb-disable-ru-per-minute-usage"].Equals(bool.TrueString, StringComparison.OrdinalIgnoreCase) ? (byte) 1 : (byte) 0;
      rntbdRequest.disableRUPerMinuteUsage.isPresent = true;
    }

    private static void AddPopulateQueryMetrics(
      DocumentServiceRequest request,
      RntbdConstants.Request rntbdRequest)
    {
      if (string.IsNullOrEmpty(request.Headers["x-ms-documentdb-populatequerymetrics"]))
        return;
      rntbdRequest.populateQueryMetrics.value.valueByte = request.Headers["x-ms-documentdb-populatequerymetrics"].Equals(bool.TrueString, StringComparison.OrdinalIgnoreCase) ? (byte) 1 : (byte) 0;
      rntbdRequest.populateQueryMetrics.isPresent = true;
    }

    private static void AddQueryForceScan(
      DocumentServiceRequest request,
      RntbdConstants.Request rntbdRequest)
    {
      if (string.IsNullOrEmpty(request.Headers["x-ms-documentdb-force-query-scan"]))
        return;
      rntbdRequest.forceQueryScan.value.valueByte = request.Headers["x-ms-documentdb-force-query-scan"].Equals(bool.TrueString, StringComparison.OrdinalIgnoreCase) ? (byte) 1 : (byte) 0;
      rntbdRequest.forceQueryScan.isPresent = true;
    }

    private static void AddPopulateCollectionThroughputInfo(
      DocumentServiceRequest request,
      RntbdConstants.Request rntbdRequest)
    {
      if (string.IsNullOrEmpty(request.Headers["x-ms-documentdb-populatecollectionthroughputinfo"]))
        return;
      rntbdRequest.populateCollectionThroughputInfo.value.valueByte = request.Headers["x-ms-documentdb-populatecollectionthroughputinfo"].Equals(bool.TrueString, StringComparison.OrdinalIgnoreCase) ? (byte) 1 : (byte) 0;
      rntbdRequest.populateCollectionThroughputInfo.isPresent = true;
    }

    private static void AddShareThroughput(
      DocumentServiceRequest request,
      RntbdConstants.Request rntbdRequest)
    {
      if (string.IsNullOrEmpty(request.Headers["x-ms-share-throughput"]))
        return;
      rntbdRequest.shareThroughput.value.valueByte = request.Headers["x-ms-share-throughput"].Equals(bool.TrueString, StringComparison.OrdinalIgnoreCase) ? (byte) 1 : (byte) 0;
      rntbdRequest.shareThroughput.isPresent = true;
    }

    private static void AddIsReadOnlyScript(
      DocumentServiceRequest request,
      RntbdConstants.Request rntbdRequest)
    {
      if (string.IsNullOrEmpty(request.Headers["x-ms-is-readonly-script"]))
        return;
      rntbdRequest.isReadOnlyScript.value.valueByte = request.Headers["x-ms-is-readonly-script"].Equals(bool.TrueString, StringComparison.OrdinalIgnoreCase) ? (byte) 1 : (byte) 0;
      rntbdRequest.isReadOnlyScript.isPresent = true;
    }

    private static void AddIsAutoScaleRequest(
      DocumentServiceRequest request,
      RntbdConstants.Request rntbdRequest)
    {
      if (string.IsNullOrEmpty(request.Headers["x-ms-is-auto-scale"]))
        return;
      rntbdRequest.isAutoScaleRequest.value.valueByte = request.Headers["x-ms-is-auto-scale"].Equals(bool.TrueString, StringComparison.OrdinalIgnoreCase) ? (byte) 1 : (byte) 0;
      rntbdRequest.isAutoScaleRequest.isPresent = true;
    }

    private static void AddCanOfferReplaceComplete(
      DocumentServiceRequest request,
      RntbdConstants.Request rntbdRequest)
    {
      if (string.IsNullOrEmpty(request.Headers["x-ms-can-offer-replace-complete"]))
        return;
      rntbdRequest.canOfferReplaceComplete.value.valueByte = request.Headers["x-ms-can-offer-replace-complete"].Equals(bool.TrueString, StringComparison.OrdinalIgnoreCase) ? (byte) 1 : (byte) 0;
      rntbdRequest.canOfferReplaceComplete.isPresent = true;
    }

    private static void AddGetAllPartitionKeyStatistics(
      DocumentServiceRequest request,
      RntbdConstants.Request rntbdRequest)
    {
      if (string.IsNullOrEmpty(request.Headers["x-ms-cosmos-internal-get-all-partition-key-stats"]))
        return;
      rntbdRequest.getAllPartitionKeyStatistics.value.valueByte = request.Headers["x-ms-cosmos-internal-get-all-partition-key-stats"].Equals(bool.TrueString, StringComparison.OrdinalIgnoreCase) ? (byte) 1 : (byte) 0;
      rntbdRequest.getAllPartitionKeyStatistics.isPresent = true;
    }

    private static void AddResponseContinuationTokenLimitInKb(
      DocumentServiceRequest request,
      RntbdConstants.Request rntbdRequest)
    {
      if (string.IsNullOrEmpty(request.Headers["x-ms-documentdb-responsecontinuationtokenlimitinkb"]))
        return;
      string header = request.Headers["x-ms-documentdb-responsecontinuationtokenlimitinkb"];
      int result;
      if (!int.TryParse(header, NumberStyles.Integer, (IFormatProvider) CultureInfo.InvariantCulture, out result))
        throw new BadRequestException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.InvalidPageSize, (object) header));
      rntbdRequest.responseContinuationTokenLimitInKb.value.valueULong = result >= 0 ? (uint) result : throw new BadRequestException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.InvalidResponseContinuationTokenLimit, (object) header));
      rntbdRequest.responseContinuationTokenLimitInKb.isPresent = true;
    }

    private static void AddRemoteStorageType(
      DocumentServiceRequest request,
      RntbdConstants.Request rntbdRequest)
    {
      if (string.IsNullOrEmpty(request.Headers["x-ms-remote-storage-type"]))
        return;
      RemoteStorageType result;
      if (!Enum.TryParse<RemoteStorageType>(request.Headers["x-ms-remote-storage-type"], true, out result))
        throw new BadRequestException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.InvalidEnumValue, (object) request.Headers["x-ms-remote-storage-type"], (object) typeof (RemoteStorageType).Name));
      RntbdConstants.RntbdRemoteStorageType remoteStorageType;
      if (result != RemoteStorageType.Standard)
      {
        if (result != RemoteStorageType.Premium)
          throw new BadRequestException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.InvalidEnumValue, (object) request.Headers["x-ms-remote-storage-type"], (object) typeof (RemoteStorageType).Name));
        remoteStorageType = RntbdConstants.RntbdRemoteStorageType.Premium;
      }
      else
        remoteStorageType = RntbdConstants.RntbdRemoteStorageType.Standard;
      rntbdRequest.remoteStorageType.value.valueByte = (byte) remoteStorageType;
      rntbdRequest.remoteStorageType.isPresent = true;
    }

    private static void AddCollectionChildResourceNameLimitInBytes(
      DocumentServiceRequest request,
      RntbdConstants.Request rntbdRequest)
    {
      string header = request.Headers["x-ms-cosmos-collection-child-resourcename-limit"];
      if (string.IsNullOrEmpty(header))
        return;
      if (!int.TryParse(header, NumberStyles.Integer, (IFormatProvider) CultureInfo.InvariantCulture, out rntbdRequest.collectionChildResourceNameLimitInBytes.value.valueLong))
        throw new BadRequestException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.InvalidHeaderValue, (object) header, (object) "x-ms-cosmos-collection-child-resourcename-limit"));
      rntbdRequest.collectionChildResourceNameLimitInBytes.isPresent = true;
    }

    private static void AddCollectionChildResourceContentLengthLimitInKB(
      DocumentServiceRequest request,
      RntbdConstants.Request rntbdRequest)
    {
      string header = request.Headers["x-ms-cosmos-collection-child-contentlength-resourcelimit"];
      if (string.IsNullOrEmpty(header))
        return;
      if (!int.TryParse(header, NumberStyles.Integer, (IFormatProvider) CultureInfo.InvariantCulture, out rntbdRequest.collectionChildResourceContentLengthLimitInKB.value.valueLong))
        throw new BadRequestException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.InvalidHeaderValue, (object) header, (object) "x-ms-cosmos-collection-child-contentlength-resourcelimit"));
      rntbdRequest.collectionChildResourceContentLengthLimitInKB.isPresent = true;
    }

    private static void AddCollectionRemoteStorageSecurityIdentifier(
      DocumentServiceRequest request,
      RntbdConstants.Request rntbdRequest)
    {
      string header = request.Headers["x-ms-collection-security-identifier"];
      if (string.IsNullOrEmpty(header))
        return;
      rntbdRequest.collectionRemoteStorageSecurityIdentifier.value.valueBytes = Encoding.UTF8.GetBytes(header);
      rntbdRequest.collectionRemoteStorageSecurityIdentifier.isPresent = true;
    }

    private static void AddIsUserRequest(
      DocumentServiceRequest request,
      RntbdConstants.Request rntbdRequest)
    {
      if (string.IsNullOrEmpty(request.Headers["x-ms-cosmos-internal-is-user-request"]))
        return;
      rntbdRequest.isUserRequest.value.valueByte = request.Headers["x-ms-cosmos-internal-is-user-request"].Equals(bool.TrueString, StringComparison.OrdinalIgnoreCase) ? (byte) 1 : (byte) 0;
      rntbdRequest.isUserRequest.isPresent = true;
    }

    private static void AddPreserveFullContent(
      DocumentServiceRequest request,
      RntbdConstants.Request rntbdRequest)
    {
      if (string.IsNullOrEmpty(request.Headers["x-ms-cosmos-preserve-full-content"]))
        return;
      rntbdRequest.preserveFullContent.value.valueByte = request.Headers["x-ms-cosmos-preserve-full-content"].Equals(bool.TrueString, StringComparison.OrdinalIgnoreCase) ? (byte) 1 : (byte) 0;
      rntbdRequest.preserveFullContent.isPresent = true;
    }

    private static void AddForceSideBySideIndexMigration(
      DocumentServiceRequest request,
      RntbdConstants.Request rntbdRequest)
    {
      if (string.IsNullOrEmpty(request.Headers["x-ms-cosmos-force-sidebyside-indexmigration"]))
        return;
      rntbdRequest.forceSideBySideIndexMigration.value.valueByte = request.Headers["x-ms-cosmos-force-sidebyside-indexmigration"].Equals(bool.TrueString, StringComparison.OrdinalIgnoreCase) ? (byte) 1 : (byte) 0;
      rntbdRequest.forceSideBySideIndexMigration.isPresent = true;
    }

    private static void AddIsRUPerGBEnforcementRequest(
      DocumentServiceRequest request,
      RntbdConstants.Request rntbdRequest)
    {
      if (string.IsNullOrEmpty(request.Headers["x-ms-cosmos-internal-is-ru-per-gb-enforcement-request"]))
        return;
      rntbdRequest.isRUPerGBEnforcementRequest.value.valueByte = request.Headers["x-ms-cosmos-internal-is-ru-per-gb-enforcement-request"].Equals(bool.TrueString, StringComparison.OrdinalIgnoreCase) ? (byte) 1 : (byte) 0;
      rntbdRequest.isRUPerGBEnforcementRequest.isPresent = true;
    }

    private static void AddEnumerationDirection(
      DocumentServiceRequest request,
      RntbdConstants.Request rntbdRequest)
    {
      object obj = (object) null;
      if (request.Properties != null && request.Properties.TryGetValue("x-ms-enumeration-direction", out obj))
      {
        rntbdRequest.enumerationDirection.value.valueByte = (obj as byte? ?? throw new BadRequestException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.InvalidEnumValue, (object) "x-ms-enumeration-direction", (object) typeof (EnumerationDirection).Name))).Value;
        rntbdRequest.enumerationDirection.isPresent = true;
      }
      else
      {
        if (string.IsNullOrEmpty(request.Headers["x-ms-enumeration-direction"]))
          return;
        EnumerationDirection result;
        if (!Enum.TryParse<EnumerationDirection>(request.Headers["x-ms-enumeration-direction"], true, out result))
          throw new BadRequestException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.InvalidEnumValue, (object) request.Headers["x-ms-enumeration-direction"], (object) typeof (EnumerationDirection).Name));
        RntbdConstants.RntdbEnumerationDirection enumerationDirection;
        if (result != EnumerationDirection.Forward)
        {
          if (result != EnumerationDirection.Reverse)
            throw new BadRequestException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.InvalidEnumValue, (object) request.Headers["x-ms-enumeration-direction"], (object) typeof (EnumerationDirection).Name));
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
      RntbdConstants.Request rntbdRequest)
    {
      if (request.Properties == null)
      {
        TransportSerialization.AddStartAndEndKeysFromHeaders(request, rntbdRequest);
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
          nullable2 = nullable1;
          RntbdConstants.RntdbReadFeedKeyType rntdbReadFeedKeyType2 = RntbdConstants.RntdbReadFeedKeyType.EffectivePartitionKey;
          if (!(nullable2.GetValueOrDefault() == rntdbReadFeedKeyType2 & nullable2.HasValue))
            return;
          TransportSerialization.SetBytesValue(request, "x-ms-start-epk", rntbdRequest.StartEpk);
          TransportSerialization.SetBytesValue(request, "x-ms-end-epk", rntbdRequest.EndEpk);
        }
      }
    }

    private static void AddStartAndEndKeysFromHeaders(
      DocumentServiceRequest request,
      RntbdConstants.Request rntbdRequest)
    {
      if (!string.IsNullOrEmpty(request.Headers["x-ms-read-key-type"]))
      {
        ReadFeedKeyType result;
        if (!Enum.TryParse<ReadFeedKeyType>(request.Headers["x-ms-read-key-type"], true, out result))
          throw new BadRequestException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.InvalidEnumValue, (object) request.Headers["x-ms-read-key-type"], (object) typeof (ReadFeedKeyType).Name));
        RntbdConstants.RntdbReadFeedKeyType rntdbReadFeedKeyType;
        if (result != ReadFeedKeyType.ResourceId)
        {
          if (result != ReadFeedKeyType.EffectivePartitionKey)
            throw new BadRequestException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.InvalidEnumValue, (object) request.Headers["x-ms-read-key-type"], (object) typeof (ReadFeedKeyType).Name));
          rntdbReadFeedKeyType = RntbdConstants.RntdbReadFeedKeyType.EffectivePartitionKey;
        }
        else
          rntdbReadFeedKeyType = RntbdConstants.RntdbReadFeedKeyType.ResourceId;
        rntbdRequest.readFeedKeyType.value.valueByte = (byte) rntdbReadFeedKeyType;
        rntbdRequest.readFeedKeyType.isPresent = true;
      }
      string header1 = request.Headers["x-ms-start-id"];
      if (!string.IsNullOrEmpty(header1))
      {
        rntbdRequest.StartId.value.valueBytes = Convert.FromBase64String(header1);
        rntbdRequest.StartId.isPresent = true;
      }
      string header2 = request.Headers["x-ms-end-id"];
      if (!string.IsNullOrEmpty(header2))
      {
        rntbdRequest.EndId.value.valueBytes = Convert.FromBase64String(header2);
        rntbdRequest.EndId.isPresent = true;
      }
      string header3 = request.Headers["x-ms-start-epk"];
      if (!string.IsNullOrEmpty(header3))
      {
        rntbdRequest.StartEpk.value.valueBytes = Convert.FromBase64String(header3);
        rntbdRequest.StartEpk.isPresent = true;
      }
      string header4 = request.Headers["x-ms-end-epk"];
      if (string.IsNullOrEmpty(header4))
        return;
      rntbdRequest.EndEpk.value.valueBytes = Convert.FromBase64String(header4);
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
      token.value.valueBytes = obj is byte[] numArray ? numArray : throw new ArgumentOutOfRangeException(headerName);
      token.isPresent = true;
    }

    private static void AddContentSerializationFormat(
      DocumentServiceRequest request,
      RntbdConstants.Request rntbdRequest)
    {
      if (string.IsNullOrEmpty(request.Headers["x-ms-documentdb-content-serialization-format"]))
        return;
      ContentSerializationFormat result;
      if (!Enum.TryParse<ContentSerializationFormat>(request.Headers["x-ms-documentdb-content-serialization-format"], true, out result))
        throw new BadRequestException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.InvalidEnumValue, (object) request.Headers["x-ms-documentdb-content-serialization-format"], (object) typeof (ContentSerializationFormat).Name));
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
          throw new BadRequestException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.InvalidEnumValue, (object) request.Headers["x-ms-documentdb-content-serialization-format"], (object) typeof (ContentSerializationFormat).Name));
      }
      rntbdRequest.contentSerializationFormat.value.valueByte = (byte) serializationFormat;
      rntbdRequest.contentSerializationFormat.isPresent = true;
    }

    private static void FillTokenFromHeader(
      DocumentServiceRequest request,
      string headerName,
      RntbdToken token)
    {
      string s = request.Headers[headerName];
      object obj;
      if (string.IsNullOrEmpty(s) && request.Properties != null && request.Properties.TryGetValue(headerName, out obj))
        s = (string) obj;
      if (string.IsNullOrEmpty(s))
        return;
      switch (token.GetTokenType())
      {
        case RntbdTokenTypes.Byte:
          token.value.valueByte = s.Equals(bool.TrueString, StringComparison.OrdinalIgnoreCase) ? (byte) 1 : (byte) 0;
          break;
        case RntbdTokenTypes.ULong:
          uint result1;
          if (!uint.TryParse(s, NumberStyles.Integer, (IFormatProvider) CultureInfo.InvariantCulture, out result1))
            throw new BadRequestException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.InvalidHeaderValue, (object) s, (object) headerName));
          token.value.valueULong = result1;
          break;
        case RntbdTokenTypes.Long:
          int result2;
          if (!int.TryParse(s, NumberStyles.Integer, (IFormatProvider) CultureInfo.InvariantCulture, out result2))
            throw new BadRequestException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.InvalidHeaderValue, (object) s, (object) headerName));
          token.value.valueLong = result2;
          break;
        case RntbdTokenTypes.LongLong:
          long result3;
          if (!long.TryParse(s, NumberStyles.Integer, (IFormatProvider) CultureInfo.InvariantCulture, out result3))
            throw new BadRequestException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.InvalidHeaderValue, (object) s, (object) headerName));
          token.value.valueLongLong = result3;
          break;
        case RntbdTokenTypes.SmallString:
        case RntbdTokenTypes.String:
        case RntbdTokenTypes.ULongString:
          token.value.valueBytes = Encoding.UTF8.GetBytes(s);
          break;
        case RntbdTokenTypes.Double:
          token.value.valueDouble = double.Parse(s, (IFormatProvider) CultureInfo.InvariantCulture);
          break;
        default:
          throw new BadRequestException();
      }
      token.isPresent = true;
    }

    private static void AddExcludeSystemProperties(
      DocumentServiceRequest request,
      RntbdConstants.Request rntbdRequest)
    {
      if (string.IsNullOrEmpty(request.Headers["x-ms-exclude-system-properties"]))
        return;
      rntbdRequest.excludeSystemProperties.value.valueByte = request.Headers["x-ms-exclude-system-properties"].Equals(bool.TrueString, StringComparison.OrdinalIgnoreCase) ? (byte) 1 : (byte) 0;
      rntbdRequest.excludeSystemProperties.isPresent = true;
    }

    private static void AddFanoutOperationStateHeader(
      DocumentServiceRequest request,
      RntbdConstants.Request rntbdRequest)
    {
      string header = request.Headers["x-ms-fanout-operation-state"];
      if (string.IsNullOrEmpty(header))
        return;
      FanoutOperationState result;
      if (!Enum.TryParse<FanoutOperationState>(header, true, out result))
        throw new BadRequestException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.InvalidEnumValue, (object) header, (object) "FanoutOperationState"));
      RntbdConstants.RntbdFanoutOperationState fanoutOperationState;
      if (result != FanoutOperationState.Started)
      {
        if (result != FanoutOperationState.Completed)
          throw new BadRequestException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.InvalidEnumValue, (object) header, (object) "FanoutOperationState"));
        fanoutOperationState = RntbdConstants.RntbdFanoutOperationState.Completed;
      }
      else
        fanoutOperationState = RntbdConstants.RntbdFanoutOperationState.Started;
      rntbdRequest.FanoutOperationState.value.valueByte = (byte) fanoutOperationState;
      rntbdRequest.FanoutOperationState.isPresent = true;
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
  }
}
