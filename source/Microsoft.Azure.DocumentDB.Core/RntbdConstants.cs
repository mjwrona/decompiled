// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.RntbdConstants
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

namespace Microsoft.Azure.Documents
{
  internal static class RntbdConstants
  {
    public const uint CurrentProtocolVersion = 1;

    public enum RntbdResourceType : ushort
    {
      Connection = 0,
      Database = 1,
      Collection = 2,
      Document = 3,
      Attachment = 4,
      User = 5,
      Permission = 6,
      StoredProcedure = 7,
      Conflict = 8,
      Trigger = 9,
      UserDefinedFunction = 10, // 0x000A
      Module = 11, // 0x000B
      Replica = 12, // 0x000C
      ModuleCommand = 13, // 0x000D
      Record = 14, // 0x000E
      Offer = 15, // 0x000F
      PartitionSetInformation = 16, // 0x0010
      XPReplicatorAddress = 17, // 0x0011
      MasterPartition = 18, // 0x0012
      ServerPartition = 19, // 0x0013
      DatabaseAccount = 20, // 0x0014
      Topology = 21, // 0x0015
      PartitionKeyRange = 22, // 0x0016
      Schema = 24, // 0x0018
      BatchApply = 25, // 0x0019
      RestoreMetadata = 26, // 0x001A
      ComputeGatewayCharges = 27, // 0x001B
      RidRange = 28, // 0x001C
      UserDefinedType = 29, // 0x001D
      VectorClock = 31, // 0x001F
      PartitionKey = 32, // 0x0020
      Snapshot = 33, // 0x0021
      ClientEncryptionKey = 35, // 0x0023
    }

    public enum RntbdOperationType : ushort
    {
      Connection = 0,
      Create = 1,
      Patch = 2,
      Read = 3,
      ReadFeed = 4,
      Delete = 5,
      Replace = 6,
      ExecuteJavaScript = 8,
      SQLQuery = 9,
      Pause = 10, // 0x000A
      Resume = 11, // 0x000B
      Stop = 12, // 0x000C
      Recycle = 13, // 0x000D
      Crash = 14, // 0x000E
      Query = 15, // 0x000F
      ForceConfigRefresh = 16, // 0x0010
      Head = 17, // 0x0011
      HeadFeed = 18, // 0x0012
      Upsert = 19, // 0x0013
      Recreate = 20, // 0x0014
      Throttle = 21, // 0x0015
      GetSplitPoint = 22, // 0x0016
      PreCreateValidation = 23, // 0x0017
      BatchApply = 24, // 0x0018
      AbortSplit = 25, // 0x0019
      CompleteSplit = 26, // 0x001A
      OfferUpdateOperation = 27, // 0x001B
      OfferPreGrowValidation = 28, // 0x001C
      BatchReportThroughputUtilization = 29, // 0x001D
      CompletePartitionMigration = 30, // 0x001E
      AbortPartitionMigration = 31, // 0x001F
      PreReplaceValidation = 32, // 0x0020
      AddComputeGatewayRequestCharges = 33, // 0x0021
      MigratePartition = 34, // 0x0022
      MasterReplaceOfferOperation = 35, // 0x0023
      ProvisionedCollectionOfferUpdateOperation = 36, // 0x0024
      Batch = 37, // 0x0025
      InitiateDatabaseOfferPartitionShrink = 38, // 0x0026
      CompleteDatabaseOfferPartitionShrink = 39, // 0x0027
      EnsureSnapshotOperation = 40, // 0x0028
      GetSplitPoints = 41, // 0x0029
    }

    public enum ConnectionContextRequestTokenIdentifiers : ushort
    {
      ProtocolVersion,
      ClientVersion,
      UserAgent,
      CallerId,
    }

    public sealed class ConnectionContextRequest : RntbdTokenStream
    {
      public RntbdToken protocolVersion;
      public RntbdToken clientVersion;
      public RntbdToken userAgent;
      public RntbdToken callerId;

      public ConnectionContextRequest()
      {
        this.protocolVersion = new RntbdToken(true, RntbdTokenTypes.ULong, (ushort) 0);
        this.clientVersion = new RntbdToken(true, RntbdTokenTypes.SmallString, (ushort) 1);
        this.userAgent = new RntbdToken(true, RntbdTokenTypes.SmallString, (ushort) 2);
        this.callerId = new RntbdToken(false, RntbdTokenTypes.Byte, (ushort) 3);
        this.SetTokens(new RntbdToken[4]
        {
          this.protocolVersion,
          this.clientVersion,
          this.userAgent,
          this.callerId
        });
      }
    }

    public enum ConnectionContextResponseTokenIdentifiers : ushort
    {
      ProtocolVersion,
      ClientVersion,
      ServerAgent,
      ServerVersion,
      IdleTimeoutInSeconds,
      UnauthenticatedTimeoutInSeconds,
    }

    public sealed class ConnectionContextResponse : RntbdTokenStream
    {
      public RntbdToken protocolVersion;
      public RntbdToken clientVersion;
      public RntbdToken serverAgent;
      public RntbdToken serverVersion;
      public RntbdToken idleTimeoutInSeconds;
      public RntbdToken unauthenticatedTimeoutInSeconds;

      public ConnectionContextResponse()
      {
        this.protocolVersion = new RntbdToken(false, RntbdTokenTypes.ULong, (ushort) 0);
        this.clientVersion = new RntbdToken(false, RntbdTokenTypes.SmallString, (ushort) 1);
        this.serverAgent = new RntbdToken(true, RntbdTokenTypes.SmallString, (ushort) 2);
        this.serverVersion = new RntbdToken(true, RntbdTokenTypes.SmallString, (ushort) 3);
        this.idleTimeoutInSeconds = new RntbdToken(false, RntbdTokenTypes.ULong, (ushort) 4);
        this.unauthenticatedTimeoutInSeconds = new RntbdToken(false, RntbdTokenTypes.ULong, (ushort) 5);
        this.SetTokens(new RntbdToken[6]
        {
          this.protocolVersion,
          this.clientVersion,
          this.serverAgent,
          this.serverVersion,
          this.idleTimeoutInSeconds,
          this.unauthenticatedTimeoutInSeconds
        });
      }
    }

    public enum RntbdIndexingDirective : byte
    {
      Default = 0,
      Include = 1,
      Exclude = 2,
      Invalid = 255, // 0xFF
    }

    public enum RntbdMigrateCollectionDirective : byte
    {
      Thaw = 0,
      Freeze = 1,
      Invalid = 255, // 0xFF
    }

    public enum RntbdRemoteStorageType : byte
    {
      Invalid,
      NotSpecified,
      Standard,
      Premium,
    }

    public enum RntbdConsistencyLevel : byte
    {
      Strong = 0,
      BoundedStaleness = 1,
      Session = 2,
      Eventual = 3,
      ConsistentPrefix = 4,
      Invalid = 255, // 0xFF
    }

    public enum RntdbEnumerationDirection : byte
    {
      Invalid,
      Forward,
      Reverse,
    }

    public enum RntbdFanoutOperationState : byte
    {
      Started = 1,
      Completed = 2,
    }

    public enum RntdbReadFeedKeyType : byte
    {
      Invalid,
      ResourceId,
      EffectivePartitionKey,
    }

    public enum RntbdContentSerializationFormat : byte
    {
      JsonText = 0,
      CosmosBinary = 1,
      HybridRow = 2,
      Invalid = 255, // 0xFF
    }

    public enum RequestIdentifiers : ushort
    {
      ResourceId = 0,
      AuthorizationToken = 1,
      PayloadPresent = 2,
      Date = 3,
      PageSize = 4,
      SessionToken = 5,
      ContinuationToken = 6,
      IndexingDirective = 7,
      Match = 8,
      PreTriggerInclude = 9,
      PostTriggerInclude = 10, // 0x000A
      IsFanout = 11, // 0x000B
      CollectionPartitionIndex = 12, // 0x000C
      CollectionServiceIndex = 13, // 0x000D
      PreTriggerExclude = 14, // 0x000E
      PostTriggerExclude = 15, // 0x000F
      ConsistencyLevel = 16, // 0x0010
      EntityId = 17, // 0x0011
      ResourceSchemaName = 18, // 0x0012
      ReplicaPath = 19, // 0x0013
      ResourceTokenExpiry = 20, // 0x0014
      DatabaseName = 21, // 0x0015
      CollectionName = 22, // 0x0016
      DocumentName = 23, // 0x0017
      AttachmentName = 24, // 0x0018
      UserName = 25, // 0x0019
      PermissionName = 26, // 0x001A
      StoredProcedureName = 27, // 0x001B
      UserDefinedFunctionName = 28, // 0x001C
      TriggerName = 29, // 0x001D
      EnableScanInQuery = 30, // 0x001E
      EmitVerboseTracesInQuery = 31, // 0x001F
      ConflictName = 32, // 0x0020
      BindReplicaDirective = 33, // 0x0021
      PrimaryMasterKey = 34, // 0x0022
      SecondaryMasterKey = 35, // 0x0023
      PrimaryReadonlyKey = 36, // 0x0024
      SecondaryReadonlyKey = 37, // 0x0025
      ProfileRequest = 38, // 0x0026
      EnableLowPrecisionOrderBy = 39, // 0x0027
      ClientVersion = 40, // 0x0028
      CanCharge = 41, // 0x0029
      CanThrottle = 42, // 0x002A
      PartitionKey = 43, // 0x002B
      PartitionKeyRangeId = 44, // 0x002C
      NotUsed2D = 45, // 0x002D
      NotUsed2E = 46, // 0x002E
      NotUsed2F = 47, // 0x002F
      MigrateCollectionDirective = 49, // 0x0031
      NotUsed32 = 50, // 0x0032
      SupportSpatialLegacyCoordinates = 51, // 0x0033
      PartitionCount = 52, // 0x0034
      CollectionRid = 53, // 0x0035
      PartitionKeyRangeName = 54, // 0x0036
      SchemaName = 58, // 0x003A
      FilterBySchemaRid = 59, // 0x003B
      UsePolygonsSmallerThanAHemisphere = 60, // 0x003C
      GatewaySignature = 61, // 0x003D
      EnableLogging = 62, // 0x003E
      A_IM = 63, // 0x003F
      PopulateQuotaInfo = 64, // 0x0040
      DisableRUPerMinuteUsage = 65, // 0x0041
      PopulateQueryMetrics = 66, // 0x0042
      ResponseContinuationTokenLimitInKb = 67, // 0x0043
      PopulatePartitionStatistics = 68, // 0x0044
      RemoteStorageType = 69, // 0x0045
      CollectionRemoteStorageSecurityIdentifier = 70, // 0x0046
      IfModifiedSince = 71, // 0x0047
      PopulateCollectionThroughputInfo = 72, // 0x0048
      RemainingTimeInMsOnClientRequest = 73, // 0x0049
      ClientRetryAttemptCount = 74, // 0x004A
      TargetLsn = 75, // 0x004B
      TargetGlobalCommittedLsn = 76, // 0x004C
      TransportRequestID = 77, // 0x004D
      RestoreMetadaFilter = 78, // 0x004E
      RestoreParams = 79, // 0x004F
      ShareThroughput = 80, // 0x0050
      PartitionResourceFilter = 81, // 0x0051
      IsReadOnlyScript = 82, // 0x0052
      IsAutoScaleRequest = 83, // 0x0053
      ForceQueryScan = 84, // 0x0054
      CanOfferReplaceComplete = 86, // 0x0056
      ExcludeSystemProperties = 87, // 0x0057
      BinaryId = 88, // 0x0058
      TimeToLiveInSeconds = 89, // 0x0059
      EffectivePartitionKey = 90, // 0x005A
      BinaryPassthroughRequest = 91, // 0x005B
      UserDefinedTypeName = 92, // 0x005C
      EnableDynamicRidRangeAllocation = 93, // 0x005D
      EnumerationDirection = 94, // 0x005E
      StartId = 95, // 0x005F
      EndId = 96, // 0x0060
      FanoutOperationState = 97, // 0x0061
      StartEpk = 98, // 0x0062
      EndEpk = 99, // 0x0063
      ReadFeedKeyType = 100, // 0x0064
      ContentSerializationFormat = 101, // 0x0065
      AllowTentativeWrites = 102, // 0x0066
      IsUserRequest = 103, // 0x0067
      SharedOfferthroughput = 104, // 0x0068
      PreserveFullContent = 105, // 0x0069
      IncludeTentativeWrites = 112, // 0x0070
      PopulateResourceCount = 113, // 0x0071
      MergeStaticId = 114, // 0x0072
      IsBatchAtomic = 115, // 0x0073
      ShouldBatchContinueOnError = 116, // 0x0074
      IsBatchOrdered = 117, // 0x0075
      SchemaOwnerRid = 118, // 0x0076
      SchemaHash = 119, // 0x0077
      IsRUPerGBEnforcementRequest = 120, // 0x0078
      MaxPollingIntervalMilliseconds = 121, // 0x0079
      SnapshotName = 122, // 0x007A
      PopulateLogStoreInfo = 123, // 0x007B
      GetAllPartitionKeyStatistics = 124, // 0x007C
      ForceSideBySideIndexMigration = 125, // 0x007D
      CollectionChildResourceNameLimitInBytes = 126, // 0x007E
      CollectionChildResourceContentLengthLimitInKB = 127, // 0x007F
      ClientEncryptionKeyName = 128, // 0x0080
      MergeCheckpointGLSNKeyName = 129, // 0x0081
    }

    public sealed class Request : RntbdTokenStream
    {
      public RntbdToken resourceId;
      public RntbdToken authorizationToken;
      public RntbdToken payloadPresent;
      public RntbdToken date;
      public RntbdToken pageSize;
      public RntbdToken sessionToken;
      public RntbdToken continuationToken;
      public RntbdToken indexingDirective;
      public RntbdToken match;
      public RntbdToken preTriggerInclude;
      public RntbdToken postTriggerInclude;
      public RntbdToken isFanout;
      public RntbdToken collectionPartitionIndex;
      public RntbdToken collectionServiceIndex;
      public RntbdToken preTriggerExclude;
      public RntbdToken postTriggerExclude;
      public RntbdToken consistencyLevel;
      public RntbdToken entityId;
      public RntbdToken resourceSchemaName;
      public RntbdToken replicaPath;
      public RntbdToken resourceTokenExpiry;
      public RntbdToken databaseName;
      public RntbdToken collectionName;
      public RntbdToken documentName;
      public RntbdToken attachmentName;
      public RntbdToken userName;
      public RntbdToken permissionName;
      public RntbdToken storedProcedureName;
      public RntbdToken userDefinedFunctionName;
      public RntbdToken triggerName;
      public RntbdToken enableScanInQuery;
      public RntbdToken emitVerboseTracesInQuery;
      public RntbdToken conflictName;
      public RntbdToken bindReplicaDirective;
      public RntbdToken primaryMasterKey;
      public RntbdToken secondaryMasterKey;
      public RntbdToken primaryReadonlyKey;
      public RntbdToken secondaryReadonlyKey;
      public RntbdToken profileRequest;
      public RntbdToken enableLowPrecisionOrderBy;
      public RntbdToken clientVersion;
      public RntbdToken canCharge;
      public RntbdToken canThrottle;
      public RntbdToken partitionKey;
      public RntbdToken partitionKeyRangeId;
      public RntbdToken migrateCollectionDirective;
      public RntbdToken supportSpatialLegacyCoordinates;
      public RntbdToken partitionCount;
      public RntbdToken collectionRid;
      public RntbdToken partitionKeyRangeName;
      public RntbdToken schemaName;
      public RntbdToken filterBySchemaRid;
      public RntbdToken usePolygonsSmallerThanAHemisphere;
      public RntbdToken gatewaySignature;
      public RntbdToken enableLogging;
      public RntbdToken a_IM;
      public RntbdToken ifModifiedSince;
      public RntbdToken populateQuotaInfo;
      public RntbdToken disableRUPerMinuteUsage;
      public RntbdToken populateQueryMetrics;
      public RntbdToken responseContinuationTokenLimitInKb;
      public RntbdToken populatePartitionStatistics;
      public RntbdToken remoteStorageType;
      public RntbdToken remainingTimeInMsOnClientRequest;
      public RntbdToken clientRetryAttemptCount;
      public RntbdToken targetLsn;
      public RntbdToken targetGlobalCommittedLsn;
      public RntbdToken transportRequestID;
      public RntbdToken collectionRemoteStorageSecurityIdentifier;
      public RntbdToken populateCollectionThroughputInfo;
      public RntbdToken restoreMetadataFilter;
      public RntbdToken restoreParams;
      public RntbdToken shareThroughput;
      public RntbdToken partitionResourceFilter;
      public RntbdToken isReadOnlyScript;
      public RntbdToken isAutoScaleRequest;
      public RntbdToken forceQueryScan;
      public RntbdToken canOfferReplaceComplete;
      public RntbdToken excludeSystemProperties;
      public RntbdToken binaryId;
      public RntbdToken timeToLiveInSeconds;
      public RntbdToken effectivePartitionKey;
      public RntbdToken binaryPassthroughRequest;
      public RntbdToken userDefinedTypeName;
      public RntbdToken enableDynamicRidRangeAllocation;
      public RntbdToken enumerationDirection;
      public RntbdToken StartId;
      public RntbdToken EndId;
      public RntbdToken FanoutOperationState;
      public RntbdToken StartEpk;
      public RntbdToken EndEpk;
      public RntbdToken readFeedKeyType;
      public RntbdToken contentSerializationFormat;
      public RntbdToken allowTentativeWrites;
      public RntbdToken isUserRequest;
      public RntbdToken preserveFullContent;
      public RntbdToken includeTentativeWrites;
      public RntbdToken populateResourceCount;
      public RntbdToken mergeStaticId;
      public RntbdToken isBatchAtomic;
      public RntbdToken shouldBatchContinueOnError;
      public RntbdToken isBatchOrdered;
      public RntbdToken schemaOwnerRid;
      public RntbdToken schemaHash;
      public RntbdToken isRUPerGBEnforcementRequest;
      public RntbdToken maxPollingIntervalMilliseconds;
      public RntbdToken snapshotName;
      public RntbdToken populateLogStoreInfo;
      public RntbdToken getAllPartitionKeyStatistics;
      public RntbdToken forceSideBySideIndexMigration;
      public RntbdToken collectionChildResourceNameLimitInBytes;
      public RntbdToken collectionChildResourceContentLengthLimitInKB;
      public RntbdToken clientEncryptionKeyName;
      public RntbdToken mergeCheckpointGlsnKeyName;

      public Request()
      {
        this.resourceId = new RntbdToken(false, RntbdTokenTypes.Bytes, (ushort) 0);
        this.authorizationToken = new RntbdToken(false, RntbdTokenTypes.String, (ushort) 1);
        this.payloadPresent = new RntbdToken(true, RntbdTokenTypes.Byte, (ushort) 2);
        this.date = new RntbdToken(false, RntbdTokenTypes.SmallString, (ushort) 3);
        this.pageSize = new RntbdToken(false, RntbdTokenTypes.ULong, (ushort) 4);
        this.sessionToken = new RntbdToken(false, RntbdTokenTypes.String, (ushort) 5);
        this.continuationToken = new RntbdToken(false, RntbdTokenTypes.String, (ushort) 6);
        this.indexingDirective = new RntbdToken(false, RntbdTokenTypes.Byte, (ushort) 7);
        this.match = new RntbdToken(false, RntbdTokenTypes.String, (ushort) 8);
        this.preTriggerInclude = new RntbdToken(false, RntbdTokenTypes.String, (ushort) 9);
        this.postTriggerInclude = new RntbdToken(false, RntbdTokenTypes.String, (ushort) 10);
        this.isFanout = new RntbdToken(false, RntbdTokenTypes.Byte, (ushort) 11);
        this.collectionPartitionIndex = new RntbdToken(false, RntbdTokenTypes.ULong, (ushort) 12);
        this.collectionServiceIndex = new RntbdToken(false, RntbdTokenTypes.ULong, (ushort) 13);
        this.preTriggerExclude = new RntbdToken(false, RntbdTokenTypes.String, (ushort) 14);
        this.postTriggerExclude = new RntbdToken(false, RntbdTokenTypes.String, (ushort) 15);
        this.consistencyLevel = new RntbdToken(false, RntbdTokenTypes.Byte, (ushort) 16);
        this.entityId = new RntbdToken(false, RntbdTokenTypes.String, (ushort) 17);
        this.resourceSchemaName = new RntbdToken(false, RntbdTokenTypes.SmallString, (ushort) 18);
        this.replicaPath = new RntbdToken(true, RntbdTokenTypes.String, (ushort) 19);
        this.resourceTokenExpiry = new RntbdToken(false, RntbdTokenTypes.ULong, (ushort) 20);
        this.databaseName = new RntbdToken(false, RntbdTokenTypes.String, (ushort) 21);
        this.collectionName = new RntbdToken(false, RntbdTokenTypes.String, (ushort) 22);
        this.documentName = new RntbdToken(false, RntbdTokenTypes.String, (ushort) 23);
        this.attachmentName = new RntbdToken(false, RntbdTokenTypes.String, (ushort) 24);
        this.userName = new RntbdToken(false, RntbdTokenTypes.String, (ushort) 25);
        this.permissionName = new RntbdToken(false, RntbdTokenTypes.String, (ushort) 26);
        this.storedProcedureName = new RntbdToken(false, RntbdTokenTypes.String, (ushort) 27);
        this.userDefinedFunctionName = new RntbdToken(false, RntbdTokenTypes.String, (ushort) 28);
        this.triggerName = new RntbdToken(false, RntbdTokenTypes.String, (ushort) 29);
        this.enableScanInQuery = new RntbdToken(false, RntbdTokenTypes.Byte, (ushort) 30);
        this.emitVerboseTracesInQuery = new RntbdToken(false, RntbdTokenTypes.Byte, (ushort) 31);
        this.conflictName = new RntbdToken(false, RntbdTokenTypes.String, (ushort) 32);
        this.bindReplicaDirective = new RntbdToken(false, RntbdTokenTypes.String, (ushort) 33);
        this.primaryMasterKey = new RntbdToken(false, RntbdTokenTypes.String, (ushort) 34);
        this.secondaryMasterKey = new RntbdToken(false, RntbdTokenTypes.String, (ushort) 35);
        this.primaryReadonlyKey = new RntbdToken(false, RntbdTokenTypes.String, (ushort) 36);
        this.secondaryReadonlyKey = new RntbdToken(false, RntbdTokenTypes.String, (ushort) 37);
        this.profileRequest = new RntbdToken(false, RntbdTokenTypes.Byte, (ushort) 38);
        this.enableLowPrecisionOrderBy = new RntbdToken(false, RntbdTokenTypes.Byte, (ushort) 39);
        this.clientVersion = new RntbdToken(false, RntbdTokenTypes.SmallString, (ushort) 40);
        this.canCharge = new RntbdToken(false, RntbdTokenTypes.Byte, (ushort) 41);
        this.canThrottle = new RntbdToken(false, RntbdTokenTypes.Byte, (ushort) 42);
        this.partitionKey = new RntbdToken(false, RntbdTokenTypes.String, (ushort) 43);
        this.partitionKeyRangeId = new RntbdToken(false, RntbdTokenTypes.String, (ushort) 44);
        this.migrateCollectionDirective = new RntbdToken(false, RntbdTokenTypes.Byte, (ushort) 49);
        this.supportSpatialLegacyCoordinates = new RntbdToken(false, RntbdTokenTypes.Byte, (ushort) 51);
        this.partitionCount = new RntbdToken(false, RntbdTokenTypes.ULong, (ushort) 52);
        this.collectionRid = new RntbdToken(false, RntbdTokenTypes.String, (ushort) 53);
        this.partitionKeyRangeName = new RntbdToken(false, RntbdTokenTypes.String, (ushort) 54);
        this.schemaName = new RntbdToken(false, RntbdTokenTypes.String, (ushort) 58);
        this.filterBySchemaRid = new RntbdToken(false, RntbdTokenTypes.String, (ushort) 59);
        this.usePolygonsSmallerThanAHemisphere = new RntbdToken(false, RntbdTokenTypes.Byte, (ushort) 60);
        this.gatewaySignature = new RntbdToken(false, RntbdTokenTypes.String, (ushort) 61);
        this.enableLogging = new RntbdToken(false, RntbdTokenTypes.Byte, (ushort) 62);
        this.a_IM = new RntbdToken(false, RntbdTokenTypes.String, (ushort) 63);
        this.ifModifiedSince = new RntbdToken(false, RntbdTokenTypes.String, (ushort) 71);
        this.populateQuotaInfo = new RntbdToken(false, RntbdTokenTypes.Byte, (ushort) 64);
        this.disableRUPerMinuteUsage = new RntbdToken(false, RntbdTokenTypes.Byte, (ushort) 65);
        this.populateQueryMetrics = new RntbdToken(false, RntbdTokenTypes.Byte, (ushort) 66);
        this.responseContinuationTokenLimitInKb = new RntbdToken(false, RntbdTokenTypes.ULong, (ushort) 67);
        this.populatePartitionStatistics = new RntbdToken(false, RntbdTokenTypes.Byte, (ushort) 68);
        this.remoteStorageType = new RntbdToken(false, RntbdTokenTypes.Byte, (ushort) 69);
        this.collectionRemoteStorageSecurityIdentifier = new RntbdToken(false, RntbdTokenTypes.String, (ushort) 70);
        this.populateCollectionThroughputInfo = new RntbdToken(false, RntbdTokenTypes.Byte, (ushort) 72);
        this.remainingTimeInMsOnClientRequest = new RntbdToken(false, RntbdTokenTypes.ULong, (ushort) 73);
        this.clientRetryAttemptCount = new RntbdToken(false, RntbdTokenTypes.ULong, (ushort) 74);
        this.targetLsn = new RntbdToken(false, RntbdTokenTypes.LongLong, (ushort) 75);
        this.targetGlobalCommittedLsn = new RntbdToken(false, RntbdTokenTypes.LongLong, (ushort) 76);
        this.transportRequestID = new RntbdToken(false, RntbdTokenTypes.ULong, (ushort) 77);
        this.restoreMetadataFilter = new RntbdToken(false, RntbdTokenTypes.String, (ushort) 78);
        this.restoreParams = new RntbdToken(false, RntbdTokenTypes.String, (ushort) 79);
        this.shareThroughput = new RntbdToken(false, RntbdTokenTypes.Byte, (ushort) 80);
        this.partitionResourceFilter = new RntbdToken(false, RntbdTokenTypes.String, (ushort) 81);
        this.isReadOnlyScript = new RntbdToken(false, RntbdTokenTypes.Byte, (ushort) 82);
        this.isAutoScaleRequest = new RntbdToken(false, RntbdTokenTypes.Byte, (ushort) 83);
        this.forceQueryScan = new RntbdToken(false, RntbdTokenTypes.Byte, (ushort) 84);
        this.canOfferReplaceComplete = new RntbdToken(false, RntbdTokenTypes.Byte, (ushort) 86);
        this.excludeSystemProperties = new RntbdToken(false, RntbdTokenTypes.Byte, (ushort) 87);
        this.binaryId = new RntbdToken(false, RntbdTokenTypes.Bytes, (ushort) 88);
        this.timeToLiveInSeconds = new RntbdToken(false, RntbdTokenTypes.ULong, (ushort) 89);
        this.effectivePartitionKey = new RntbdToken(false, RntbdTokenTypes.Bytes, (ushort) 90);
        this.binaryPassthroughRequest = new RntbdToken(false, RntbdTokenTypes.Byte, (ushort) 91);
        this.userDefinedTypeName = new RntbdToken(false, RntbdTokenTypes.String, (ushort) 92);
        this.enableDynamicRidRangeAllocation = new RntbdToken(false, RntbdTokenTypes.Byte, (ushort) 93);
        this.enumerationDirection = new RntbdToken(false, RntbdTokenTypes.Byte, (ushort) 94);
        this.StartId = new RntbdToken(false, RntbdTokenTypes.Bytes, (ushort) 95);
        this.EndId = new RntbdToken(false, RntbdTokenTypes.Bytes, (ushort) 96);
        this.FanoutOperationState = new RntbdToken(false, RntbdTokenTypes.Byte, (ushort) 97);
        this.StartEpk = new RntbdToken(false, RntbdTokenTypes.Bytes, (ushort) 98);
        this.EndEpk = new RntbdToken(false, RntbdTokenTypes.Bytes, (ushort) 99);
        this.readFeedKeyType = new RntbdToken(false, RntbdTokenTypes.Byte, (ushort) 100);
        this.contentSerializationFormat = new RntbdToken(false, RntbdTokenTypes.Byte, (ushort) 101);
        this.allowTentativeWrites = new RntbdToken(false, RntbdTokenTypes.Byte, (ushort) 102);
        this.isUserRequest = new RntbdToken(false, RntbdTokenTypes.Byte, (ushort) 103);
        this.preserveFullContent = new RntbdToken(false, RntbdTokenTypes.Byte, (ushort) 105);
        this.includeTentativeWrites = new RntbdToken(false, RntbdTokenTypes.Byte, (ushort) 112);
        this.populateResourceCount = new RntbdToken(false, RntbdTokenTypes.Byte, (ushort) 113);
        this.mergeStaticId = new RntbdToken(false, RntbdTokenTypes.Bytes, (ushort) 114);
        this.isBatchAtomic = new RntbdToken(false, RntbdTokenTypes.Byte, (ushort) 115);
        this.shouldBatchContinueOnError = new RntbdToken(false, RntbdTokenTypes.Byte, (ushort) 116);
        this.isBatchOrdered = new RntbdToken(false, RntbdTokenTypes.Byte, (ushort) 117);
        this.schemaOwnerRid = new RntbdToken(false, RntbdTokenTypes.String, (ushort) 118);
        this.schemaHash = new RntbdToken(false, RntbdTokenTypes.Bytes, (ushort) 119);
        this.isRUPerGBEnforcementRequest = new RntbdToken(false, RntbdTokenTypes.Byte, (ushort) 120);
        this.maxPollingIntervalMilliseconds = new RntbdToken(false, RntbdTokenTypes.ULong, (ushort) 121);
        this.snapshotName = new RntbdToken(false, RntbdTokenTypes.String, (ushort) 122);
        this.populateLogStoreInfo = new RntbdToken(false, RntbdTokenTypes.Byte, (ushort) 123);
        this.getAllPartitionKeyStatistics = new RntbdToken(false, RntbdTokenTypes.Byte, (ushort) 124);
        this.forceSideBySideIndexMigration = new RntbdToken(false, RntbdTokenTypes.Byte, (ushort) 125);
        this.collectionChildResourceNameLimitInBytes = new RntbdToken(false, RntbdTokenTypes.Bytes, (ushort) 126);
        this.collectionChildResourceContentLengthLimitInKB = new RntbdToken(false, RntbdTokenTypes.Bytes, (ushort) sbyte.MaxValue);
        this.clientEncryptionKeyName = new RntbdToken(false, RntbdTokenTypes.String, (ushort) 128);
        this.mergeCheckpointGlsnKeyName = new RntbdToken(false, RntbdTokenTypes.LongLong, (ushort) 129);
        this.SetTokens(new RntbdToken[114]
        {
          this.resourceId,
          this.authorizationToken,
          this.payloadPresent,
          this.date,
          this.pageSize,
          this.sessionToken,
          this.continuationToken,
          this.indexingDirective,
          this.match,
          this.preTriggerInclude,
          this.postTriggerInclude,
          this.isFanout,
          this.collectionPartitionIndex,
          this.collectionServiceIndex,
          this.preTriggerExclude,
          this.postTriggerExclude,
          this.consistencyLevel,
          this.entityId,
          this.resourceSchemaName,
          this.replicaPath,
          this.resourceTokenExpiry,
          this.databaseName,
          this.collectionName,
          this.documentName,
          this.attachmentName,
          this.userName,
          this.permissionName,
          this.storedProcedureName,
          this.userDefinedFunctionName,
          this.triggerName,
          this.enableScanInQuery,
          this.emitVerboseTracesInQuery,
          this.conflictName,
          this.bindReplicaDirective,
          this.primaryMasterKey,
          this.secondaryMasterKey,
          this.primaryReadonlyKey,
          this.secondaryReadonlyKey,
          this.profileRequest,
          this.enableLowPrecisionOrderBy,
          this.clientVersion,
          this.canCharge,
          this.canThrottle,
          this.partitionKey,
          this.partitionKeyRangeId,
          this.migrateCollectionDirective,
          this.supportSpatialLegacyCoordinates,
          this.partitionCount,
          this.collectionRid,
          this.partitionKeyRangeName,
          this.schemaName,
          this.filterBySchemaRid,
          this.usePolygonsSmallerThanAHemisphere,
          this.gatewaySignature,
          this.enableLogging,
          this.a_IM,
          this.ifModifiedSince,
          this.populateQuotaInfo,
          this.disableRUPerMinuteUsage,
          this.populateQueryMetrics,
          this.responseContinuationTokenLimitInKb,
          this.populatePartitionStatistics,
          this.remoteStorageType,
          this.collectionRemoteStorageSecurityIdentifier,
          this.populateCollectionThroughputInfo,
          this.remainingTimeInMsOnClientRequest,
          this.clientRetryAttemptCount,
          this.targetLsn,
          this.targetGlobalCommittedLsn,
          this.transportRequestID,
          this.restoreMetadataFilter,
          this.restoreParams,
          this.shareThroughput,
          this.partitionResourceFilter,
          this.isReadOnlyScript,
          this.isAutoScaleRequest,
          this.forceQueryScan,
          this.canOfferReplaceComplete,
          this.excludeSystemProperties,
          this.binaryId,
          this.timeToLiveInSeconds,
          this.effectivePartitionKey,
          this.binaryPassthroughRequest,
          this.userDefinedTypeName,
          this.enableDynamicRidRangeAllocation,
          this.enumerationDirection,
          this.StartId,
          this.EndId,
          this.FanoutOperationState,
          this.StartEpk,
          this.EndEpk,
          this.readFeedKeyType,
          this.contentSerializationFormat,
          this.allowTentativeWrites,
          this.isUserRequest,
          this.preserveFullContent,
          this.includeTentativeWrites,
          this.populateResourceCount,
          this.mergeStaticId,
          this.isBatchAtomic,
          this.shouldBatchContinueOnError,
          this.isBatchOrdered,
          this.schemaOwnerRid,
          this.schemaHash,
          this.isRUPerGBEnforcementRequest,
          this.maxPollingIntervalMilliseconds,
          this.snapshotName,
          this.populateLogStoreInfo,
          this.getAllPartitionKeyStatistics,
          this.forceSideBySideIndexMigration,
          this.collectionChildResourceNameLimitInBytes,
          this.collectionChildResourceContentLengthLimitInKB,
          this.clientEncryptionKeyName,
          this.mergeCheckpointGlsnKeyName
        });
      }
    }

    public enum ResponseIdentifiers : ushort
    {
      PayloadPresent = 0,
      LastStateChangeDateTime = 2,
      ContinuationToken = 3,
      ETag = 4,
      ReadsPerformed = 7,
      WritesPerformed = 8,
      QueriesPerformed = 9,
      IndexTermsGenerated = 10, // 0x000A
      ScriptsExecuted = 11, // 0x000B
      RetryAfterMilliseconds = 12, // 0x000C
      IndexingDirective = 13, // 0x000D
      StorageMaxResoureQuota = 14, // 0x000E
      StorageResourceQuotaUsage = 15, // 0x000F
      SchemaVersion = 16, // 0x0010
      CollectionPartitionIndex = 17, // 0x0011
      CollectionServiceIndex = 18, // 0x0012
      LSN = 19, // 0x0013
      ItemCount = 20, // 0x0014
      RequestCharge = 21, // 0x0015
      OwnerFullName = 23, // 0x0017
      OwnerId = 24, // 0x0018
      DatabaseAccountId = 25, // 0x0019
      QuorumAckedLSN = 26, // 0x001A
      RequestValidationFailure = 27, // 0x001B
      SubStatus = 28, // 0x001C
      CollectionUpdateProgress = 29, // 0x001D
      CurrentWriteQuorum = 30, // 0x001E
      CurrentReplicaSetSize = 31, // 0x001F
      CollectionLazyIndexProgress = 32, // 0x0020
      PartitionKeyRangeId = 33, // 0x0021
      LogResults = 37, // 0x0025
      XPRole = 38, // 0x0026
      IsRUPerMinuteUsed = 39, // 0x0027
      QueryMetrics = 40, // 0x0028
      GlobalCommittedLSN = 41, // 0x0029
      NumberOfReadRegions = 48, // 0x0030
      OfferReplacePending = 49, // 0x0031
      ItemLSN = 50, // 0x0032
      RestoreState = 51, // 0x0033
      CollectionSecurityIdentifier = 52, // 0x0034
      TransportRequestID = 53, // 0x0035
      ShareThroughput = 54, // 0x0036
      DisableRntbdChannel = 56, // 0x0038
      ServerDateTimeUtc = 57, // 0x0039
      LocalLSN = 58, // 0x003A
      QuorumAckedLocalLSN = 59, // 0x003B
      ItemLocalLSN = 60, // 0x003C
      HasTentativeWrites = 61, // 0x003D
      SessionToken = 62, // 0x003E
      ReplicatorLSNToGLSNDelta = 63, // 0x003F
      ReplicatorLSNToLLSNDelta = 64, // 0x0040
      VectorClockLocalProgress = 65, // 0x0041
      MinimumRUsForOffer = 66, // 0x0042
      XPConfigurationSessionsCount = 67, // 0x0043
      IndexUtilization = 68, // 0x0044
    }

    public sealed class Response : RntbdTokenStream
    {
      public RntbdToken payloadPresent;
      public RntbdToken lastStateChangeDateTime;
      public RntbdToken continuationToken;
      public RntbdToken eTag;
      public RntbdToken readsPerformed;
      public RntbdToken writesPerformed;
      public RntbdToken queriesPerformed;
      public RntbdToken indexTermsGenerated;
      public RntbdToken scriptsExecuted;
      public RntbdToken retryAfterMilliseconds;
      public RntbdToken indexingDirective;
      public RntbdToken storageMaxResoureQuota;
      public RntbdToken storageResourceQuotaUsage;
      public RntbdToken schemaVersion;
      public RntbdToken collectionPartitionIndex;
      public RntbdToken collectionServiceIndex;
      public RntbdToken LSN;
      public RntbdToken itemCount;
      public RntbdToken requestCharge;
      public RntbdToken ownerFullName;
      public RntbdToken ownerId;
      public RntbdToken databaseAccountId;
      public RntbdToken quorumAckedLSN;
      public RntbdToken requestValidationFailure;
      public RntbdToken subStatus;
      public RntbdToken collectionUpdateProgress;
      public RntbdToken currentWriteQuorum;
      public RntbdToken currentReplicaSetSize;
      public RntbdToken collectionLazyIndexProgress;
      public RntbdToken partitionKeyRangeId;
      public RntbdToken logResults;
      public RntbdToken xpRole;
      public RntbdToken isRUPerMinuteUsed;
      public RntbdToken queryMetrics;
      public RntbdToken indexUtilization;
      public RntbdToken globalCommittedLSN;
      public RntbdToken numberOfReadRegions;
      public RntbdToken offerReplacePending;
      public RntbdToken itemLSN;
      public RntbdToken restoreState;
      public RntbdToken collectionSecurityIdentifier;
      public RntbdToken transportRequestID;
      public RntbdToken shareThroughput;
      public RntbdToken disableRntbdChannel;
      public RntbdToken serverDateTimeUtc;
      public RntbdToken localLSN;
      public RntbdToken quorumAckedLocalLSN;
      public RntbdToken itemLocalLSN;
      public RntbdToken hasTentativeWrites;
      public RntbdToken sessionToken;
      public RntbdToken replicatorLSNToGLSNDelta;
      public RntbdToken replicatorLSNToLLSNDelta;
      public RntbdToken vectorClockLocalProgress;
      public RntbdToken minimumRUsForOffer;
      public RntbdToken xpConfigurationSesssionsCount;

      public Response()
      {
        this.payloadPresent = new RntbdToken(true, RntbdTokenTypes.Byte, (ushort) 0);
        this.lastStateChangeDateTime = new RntbdToken(false, RntbdTokenTypes.SmallString, (ushort) 2);
        this.continuationToken = new RntbdToken(false, RntbdTokenTypes.String, (ushort) 3);
        this.eTag = new RntbdToken(false, RntbdTokenTypes.String, (ushort) 4);
        this.readsPerformed = new RntbdToken(false, RntbdTokenTypes.ULong, (ushort) 7);
        this.writesPerformed = new RntbdToken(false, RntbdTokenTypes.ULong, (ushort) 8);
        this.queriesPerformed = new RntbdToken(false, RntbdTokenTypes.ULong, (ushort) 9);
        this.indexTermsGenerated = new RntbdToken(false, RntbdTokenTypes.ULong, (ushort) 10);
        this.scriptsExecuted = new RntbdToken(false, RntbdTokenTypes.ULong, (ushort) 11);
        this.retryAfterMilliseconds = new RntbdToken(false, RntbdTokenTypes.ULong, (ushort) 12);
        this.indexingDirective = new RntbdToken(false, RntbdTokenTypes.Byte, (ushort) 13);
        this.storageMaxResoureQuota = new RntbdToken(false, RntbdTokenTypes.String, (ushort) 14);
        this.storageResourceQuotaUsage = new RntbdToken(false, RntbdTokenTypes.String, (ushort) 15);
        this.schemaVersion = new RntbdToken(false, RntbdTokenTypes.SmallString, (ushort) 16);
        this.collectionPartitionIndex = new RntbdToken(false, RntbdTokenTypes.ULong, (ushort) 17);
        this.collectionServiceIndex = new RntbdToken(false, RntbdTokenTypes.ULong, (ushort) 18);
        this.LSN = new RntbdToken(false, RntbdTokenTypes.LongLong, (ushort) 19);
        this.itemCount = new RntbdToken(false, RntbdTokenTypes.ULong, (ushort) 20);
        this.requestCharge = new RntbdToken(false, RntbdTokenTypes.Double, (ushort) 21);
        this.ownerFullName = new RntbdToken(false, RntbdTokenTypes.String, (ushort) 23);
        this.ownerId = new RntbdToken(false, RntbdTokenTypes.String, (ushort) 24);
        this.databaseAccountId = new RntbdToken(false, RntbdTokenTypes.String, (ushort) 25);
        this.quorumAckedLSN = new RntbdToken(false, RntbdTokenTypes.LongLong, (ushort) 26);
        this.requestValidationFailure = new RntbdToken(false, RntbdTokenTypes.Byte, (ushort) 27);
        this.subStatus = new RntbdToken(false, RntbdTokenTypes.ULong, (ushort) 28);
        this.collectionUpdateProgress = new RntbdToken(false, RntbdTokenTypes.ULong, (ushort) 29);
        this.currentWriteQuorum = new RntbdToken(false, RntbdTokenTypes.ULong, (ushort) 30);
        this.currentReplicaSetSize = new RntbdToken(false, RntbdTokenTypes.ULong, (ushort) 31);
        this.collectionLazyIndexProgress = new RntbdToken(false, RntbdTokenTypes.ULong, (ushort) 32);
        this.partitionKeyRangeId = new RntbdToken(false, RntbdTokenTypes.String, (ushort) 33);
        this.logResults = new RntbdToken(false, RntbdTokenTypes.String, (ushort) 37);
        this.xpRole = new RntbdToken(false, RntbdTokenTypes.ULong, (ushort) 38);
        this.isRUPerMinuteUsed = new RntbdToken(false, RntbdTokenTypes.Byte, (ushort) 39);
        this.queryMetrics = new RntbdToken(false, RntbdTokenTypes.String, (ushort) 40);
        this.globalCommittedLSN = new RntbdToken(false, RntbdTokenTypes.LongLong, (ushort) 41);
        this.numberOfReadRegions = new RntbdToken(false, RntbdTokenTypes.ULong, (ushort) 48);
        this.offerReplacePending = new RntbdToken(false, RntbdTokenTypes.Byte, (ushort) 49);
        this.itemLSN = new RntbdToken(false, RntbdTokenTypes.LongLong, (ushort) 50);
        this.restoreState = new RntbdToken(false, RntbdTokenTypes.String, (ushort) 51);
        this.collectionSecurityIdentifier = new RntbdToken(false, RntbdTokenTypes.String, (ushort) 52);
        this.transportRequestID = new RntbdToken(false, RntbdTokenTypes.ULong, (ushort) 53);
        this.shareThroughput = new RntbdToken(false, RntbdTokenTypes.Byte, (ushort) 54);
        this.disableRntbdChannel = new RntbdToken(false, RntbdTokenTypes.Byte, (ushort) 56);
        this.serverDateTimeUtc = new RntbdToken(false, RntbdTokenTypes.SmallString, (ushort) 57);
        this.localLSN = new RntbdToken(false, RntbdTokenTypes.LongLong, (ushort) 58);
        this.quorumAckedLocalLSN = new RntbdToken(false, RntbdTokenTypes.LongLong, (ushort) 59);
        this.itemLocalLSN = new RntbdToken(false, RntbdTokenTypes.LongLong, (ushort) 60);
        this.hasTentativeWrites = new RntbdToken(false, RntbdTokenTypes.Byte, (ushort) 61);
        this.sessionToken = new RntbdToken(false, RntbdTokenTypes.String, (ushort) 62);
        this.replicatorLSNToGLSNDelta = new RntbdToken(false, RntbdTokenTypes.LongLong, (ushort) 63);
        this.replicatorLSNToLLSNDelta = new RntbdToken(false, RntbdTokenTypes.LongLong, (ushort) 64);
        this.vectorClockLocalProgress = new RntbdToken(false, RntbdTokenTypes.LongLong, (ushort) 65);
        this.minimumRUsForOffer = new RntbdToken(false, RntbdTokenTypes.ULong, (ushort) 66);
        this.xpConfigurationSesssionsCount = new RntbdToken(false, RntbdTokenTypes.ULong, (ushort) 67);
        this.indexUtilization = new RntbdToken(false, RntbdTokenTypes.String, (ushort) 68);
        this.SetTokens(new RntbdToken[55]
        {
          this.payloadPresent,
          this.lastStateChangeDateTime,
          this.continuationToken,
          this.eTag,
          this.readsPerformed,
          this.writesPerformed,
          this.queriesPerformed,
          this.indexTermsGenerated,
          this.scriptsExecuted,
          this.retryAfterMilliseconds,
          this.indexingDirective,
          this.storageMaxResoureQuota,
          this.storageResourceQuotaUsage,
          this.schemaVersion,
          this.collectionPartitionIndex,
          this.collectionServiceIndex,
          this.LSN,
          this.itemCount,
          this.requestCharge,
          this.ownerFullName,
          this.ownerId,
          this.databaseAccountId,
          this.quorumAckedLSN,
          this.requestValidationFailure,
          this.subStatus,
          this.collectionUpdateProgress,
          this.currentWriteQuorum,
          this.currentReplicaSetSize,
          this.collectionLazyIndexProgress,
          this.partitionKeyRangeId,
          this.logResults,
          this.xpRole,
          this.isRUPerMinuteUsed,
          this.queryMetrics,
          this.globalCommittedLSN,
          this.numberOfReadRegions,
          this.offerReplacePending,
          this.itemLSN,
          this.restoreState,
          this.collectionSecurityIdentifier,
          this.transportRequestID,
          this.shareThroughput,
          this.disableRntbdChannel,
          this.serverDateTimeUtc,
          this.localLSN,
          this.quorumAckedLocalLSN,
          this.itemLocalLSN,
          this.hasTentativeWrites,
          this.sessionToken,
          this.replicatorLSNToGLSNDelta,
          this.replicatorLSNToLLSNDelta,
          this.vectorClockLocalProgress,
          this.minimumRUsForOffer,
          this.xpConfigurationSesssionsCount,
          this.indexUtilization
        });
      }
    }

    public enum CallerId : byte
    {
      Anonymous,
      Gateway,
      Invalid,
    }
  }
}
