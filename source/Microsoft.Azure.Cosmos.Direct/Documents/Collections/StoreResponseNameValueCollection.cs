// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Collections.StoreResponseNameValueCollection
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
  internal class StoreResponseNameValueCollection : INameValueCollection, IEnumerable
  {
    private static readonly StringComparer DefaultStringComparer = StringComparer.OrdinalIgnoreCase;
    private readonly Lazy<Dictionary<string, string>> lazyNotCommonHeaders;
    private NameValueCollection nameValueCollection;

    public string AadAppliedRoleAssignmentId { get; set; }

    public string ActivityId { get; set; }

    public string AnalyticalMigrationProgress { get; set; }

    public string AppliedPolicyElementId { get; set; }

    public string BackendRequestDurationMilliseconds { get; set; }

    public string ByokEncryptionProgress { get; set; }

    public string ChangeFeedInfo { get; set; }

    public string CollectionIndexTransformationProgress { get; set; }

    public string CollectionLazyIndexingProgress { get; set; }

    public string CollectionPartitionIndex { get; set; }

    public string CollectionSecurityIdentifier { get; set; }

    public string CollectionServiceIndex { get; set; }

    public string CollectionUniqueIndexReIndexProgress { get; set; }

    public string CollectionUniqueKeysUnderReIndex { get; set; }

    public string ConfirmedStoreChecksum { get; set; }

    public string Continuation { get; set; }

    public string CorrelatedActivityId { get; set; }

    public string CurrentReplicaSetSize { get; set; }

    public string CurrentResourceQuotaUsage { get; set; }

    public string CurrentWriteQuorum { get; set; }

    public string DatabaseAccountId { get; set; }

    public string DisableRntbdChannel { get; set; }

    public string ETag { get; set; }

    public string GlobalCommittedLSN { get; set; }

    public string HasTentativeWrites { get; set; }

    public string IndexingDirective { get; set; }

    public string IndexUtilization { get; set; }

    public string IsRUPerMinuteUsed { get; set; }

    public string ItemCount { get; set; }

    public string ItemLocalLSN { get; set; }

    public string ItemLSN { get; set; }

    public string LastStateChangeUtc { get; set; }

    public string LocalLSN { get; set; }

    public string LogResults { get; set; }

    public string LSN { get; set; }

    public string MaxResourceQuota { get; set; }

    public string MergeProgressBlocked { get; set; }

    public string MinimumRUsForOffer { get; set; }

    public string NumberOfReadRegions { get; set; }

    public string OfferReplacePending { get; set; }

    public string OwnerFullName { get; set; }

    public string OwnerId { get; set; }

    public string PartitionKeyRangeId { get; set; }

    public string PendingPKDelete { get; set; }

    public string QueryExecutionInfo { get; set; }

    public string QueryMetrics { get; set; }

    public string QuorumAckedLocalLSN { get; set; }

    public string QuorumAckedLSN { get; set; }

    public string ReplicaStatusRevoked { get; set; }

    public string ReplicatorLSNToGLSNDelta { get; set; }

    public string ReplicatorLSNToLLSNDelta { get; set; }

    public string RequestCharge { get; set; }

    public string RequestValidationFailure { get; set; }

    public string ResourceId { get; set; }

    public string RestoreState { get; set; }

    public string RetryAfterInMilliseconds { get; set; }

    public string SchemaVersion { get; set; }

    public string ServerVersion { get; set; }

    public string SessionToken { get; set; }

    public string ShareThroughput { get; set; }

    public string SoftMaxAllowedThroughput { get; set; }

    public string SubStatus { get; set; }

    public string TentativeStoreChecksum { get; set; }

    public string TimeToLiveInSeconds { get; set; }

    public string TotalAccountThroughput { get; set; }

    public string TransportRequestID { get; set; }

    public string UnflushedMergLogEntryCount { get; set; }

    public string VectorClockLocalProgress { get; set; }

    public string XDate { get; set; }

    public string XPConfigurationSessionsCount { get; set; }

    public string XPRole { get; set; }

    public StoreResponseNameValueCollection()
      : this(new Lazy<Dictionary<string, string>>((Func<Dictionary<string, string>>) (() => new Dictionary<string, string>((IEqualityComparer<string>) StoreResponseNameValueCollection.DefaultStringComparer))))
    {
    }

    private StoreResponseNameValueCollection(Lazy<Dictionary<string, string>> notCommonHeaders) => this.lazyNotCommonHeaders = notCommonHeaders ?? throw new ArgumentNullException(nameof (notCommonHeaders));

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
      this.AadAppliedRoleAssignmentId = (string) null;
      this.ActivityId = (string) null;
      this.AnalyticalMigrationProgress = (string) null;
      this.AppliedPolicyElementId = (string) null;
      this.BackendRequestDurationMilliseconds = (string) null;
      this.ByokEncryptionProgress = (string) null;
      this.ChangeFeedInfo = (string) null;
      this.CollectionIndexTransformationProgress = (string) null;
      this.CollectionLazyIndexingProgress = (string) null;
      this.CollectionPartitionIndex = (string) null;
      this.CollectionSecurityIdentifier = (string) null;
      this.CollectionServiceIndex = (string) null;
      this.CollectionUniqueIndexReIndexProgress = (string) null;
      this.CollectionUniqueKeysUnderReIndex = (string) null;
      this.ConfirmedStoreChecksum = (string) null;
      this.Continuation = (string) null;
      this.CorrelatedActivityId = (string) null;
      this.CurrentReplicaSetSize = (string) null;
      this.CurrentResourceQuotaUsage = (string) null;
      this.CurrentWriteQuorum = (string) null;
      this.DatabaseAccountId = (string) null;
      this.DisableRntbdChannel = (string) null;
      this.ETag = (string) null;
      this.GlobalCommittedLSN = (string) null;
      this.HasTentativeWrites = (string) null;
      this.IndexingDirective = (string) null;
      this.IndexUtilization = (string) null;
      this.IsRUPerMinuteUsed = (string) null;
      this.ItemCount = (string) null;
      this.ItemLocalLSN = (string) null;
      this.ItemLSN = (string) null;
      this.LastStateChangeUtc = (string) null;
      this.LocalLSN = (string) null;
      this.LogResults = (string) null;
      this.LSN = (string) null;
      this.MaxResourceQuota = (string) null;
      this.MergeProgressBlocked = (string) null;
      this.MinimumRUsForOffer = (string) null;
      this.NumberOfReadRegions = (string) null;
      this.OfferReplacePending = (string) null;
      this.OwnerFullName = (string) null;
      this.OwnerId = (string) null;
      this.PartitionKeyRangeId = (string) null;
      this.PendingPKDelete = (string) null;
      this.QueryExecutionInfo = (string) null;
      this.QueryMetrics = (string) null;
      this.QuorumAckedLocalLSN = (string) null;
      this.QuorumAckedLSN = (string) null;
      this.ReplicaStatusRevoked = (string) null;
      this.ReplicatorLSNToGLSNDelta = (string) null;
      this.ReplicatorLSNToLLSNDelta = (string) null;
      this.RequestCharge = (string) null;
      this.RequestValidationFailure = (string) null;
      this.ResourceId = (string) null;
      this.RestoreState = (string) null;
      this.RetryAfterInMilliseconds = (string) null;
      this.SchemaVersion = (string) null;
      this.ServerVersion = (string) null;
      this.SessionToken = (string) null;
      this.ShareThroughput = (string) null;
      this.SoftMaxAllowedThroughput = (string) null;
      this.SubStatus = (string) null;
      this.TentativeStoreChecksum = (string) null;
      this.TimeToLiveInSeconds = (string) null;
      this.TotalAccountThroughput = (string) null;
      this.TransportRequestID = (string) null;
      this.UnflushedMergLogEntryCount = (string) null;
      this.VectorClockLocalProgress = (string) null;
      this.XDate = (string) null;
      this.XPConfigurationSessionsCount = (string) null;
      this.XPRole = (string) null;
    }

    public INameValueCollection Clone()
    {
      Lazy<Dictionary<string, string>> notCommonHeaders = new Lazy<Dictionary<string, string>>((Func<Dictionary<string, string>>) (() => new Dictionary<string, string>((IEqualityComparer<string>) StoreResponseNameValueCollection.DefaultStringComparer)));
      if (this.lazyNotCommonHeaders.IsValueCreated)
      {
        foreach (KeyValuePair<string, string> keyValuePair in this.lazyNotCommonHeaders.Value)
          notCommonHeaders.Value[keyValuePair.Key] = keyValuePair.Value;
      }
      return (INameValueCollection) new StoreResponseNameValueCollection(notCommonHeaders)
      {
        AadAppliedRoleAssignmentId = this.AadAppliedRoleAssignmentId,
        ActivityId = this.ActivityId,
        AnalyticalMigrationProgress = this.AnalyticalMigrationProgress,
        AppliedPolicyElementId = this.AppliedPolicyElementId,
        BackendRequestDurationMilliseconds = this.BackendRequestDurationMilliseconds,
        ByokEncryptionProgress = this.ByokEncryptionProgress,
        ChangeFeedInfo = this.ChangeFeedInfo,
        CollectionIndexTransformationProgress = this.CollectionIndexTransformationProgress,
        CollectionLazyIndexingProgress = this.CollectionLazyIndexingProgress,
        CollectionPartitionIndex = this.CollectionPartitionIndex,
        CollectionSecurityIdentifier = this.CollectionSecurityIdentifier,
        CollectionServiceIndex = this.CollectionServiceIndex,
        CollectionUniqueIndexReIndexProgress = this.CollectionUniqueIndexReIndexProgress,
        CollectionUniqueKeysUnderReIndex = this.CollectionUniqueKeysUnderReIndex,
        ConfirmedStoreChecksum = this.ConfirmedStoreChecksum,
        Continuation = this.Continuation,
        CorrelatedActivityId = this.CorrelatedActivityId,
        CurrentReplicaSetSize = this.CurrentReplicaSetSize,
        CurrentResourceQuotaUsage = this.CurrentResourceQuotaUsage,
        CurrentWriteQuorum = this.CurrentWriteQuorum,
        DatabaseAccountId = this.DatabaseAccountId,
        DisableRntbdChannel = this.DisableRntbdChannel,
        ETag = this.ETag,
        GlobalCommittedLSN = this.GlobalCommittedLSN,
        HasTentativeWrites = this.HasTentativeWrites,
        IndexingDirective = this.IndexingDirective,
        IndexUtilization = this.IndexUtilization,
        IsRUPerMinuteUsed = this.IsRUPerMinuteUsed,
        ItemCount = this.ItemCount,
        ItemLocalLSN = this.ItemLocalLSN,
        ItemLSN = this.ItemLSN,
        LastStateChangeUtc = this.LastStateChangeUtc,
        LocalLSN = this.LocalLSN,
        LogResults = this.LogResults,
        LSN = this.LSN,
        MaxResourceQuota = this.MaxResourceQuota,
        MergeProgressBlocked = this.MergeProgressBlocked,
        MinimumRUsForOffer = this.MinimumRUsForOffer,
        NumberOfReadRegions = this.NumberOfReadRegions,
        OfferReplacePending = this.OfferReplacePending,
        OwnerFullName = this.OwnerFullName,
        OwnerId = this.OwnerId,
        PartitionKeyRangeId = this.PartitionKeyRangeId,
        PendingPKDelete = this.PendingPKDelete,
        QueryExecutionInfo = this.QueryExecutionInfo,
        QueryMetrics = this.QueryMetrics,
        QuorumAckedLocalLSN = this.QuorumAckedLocalLSN,
        QuorumAckedLSN = this.QuorumAckedLSN,
        ReplicaStatusRevoked = this.ReplicaStatusRevoked,
        ReplicatorLSNToGLSNDelta = this.ReplicatorLSNToGLSNDelta,
        ReplicatorLSNToLLSNDelta = this.ReplicatorLSNToLLSNDelta,
        RequestCharge = this.RequestCharge,
        RequestValidationFailure = this.RequestValidationFailure,
        ResourceId = this.ResourceId,
        RestoreState = this.RestoreState,
        RetryAfterInMilliseconds = this.RetryAfterInMilliseconds,
        SchemaVersion = this.SchemaVersion,
        ServerVersion = this.ServerVersion,
        SessionToken = this.SessionToken,
        ShareThroughput = this.ShareThroughput,
        SoftMaxAllowedThroughput = this.SoftMaxAllowedThroughput,
        SubStatus = this.SubStatus,
        TentativeStoreChecksum = this.TentativeStoreChecksum,
        TimeToLiveInSeconds = this.TimeToLiveInSeconds,
        TotalAccountThroughput = this.TotalAccountThroughput,
        TransportRequestID = this.TransportRequestID,
        UnflushedMergLogEntryCount = this.UnflushedMergLogEntryCount,
        VectorClockLocalProgress = this.VectorClockLocalProgress,
        XDate = this.XDate,
        XPConfigurationSessionsCount = this.XPConfigurationSessionsCount,
        XPRole = this.XPRole
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
      if (this.ActivityId != null)
        yield return "x-ms-activity-id";
      if (this.LastStateChangeUtc != null)
        yield return "x-ms-last-state-change-utc";
      if (this.Continuation != null)
        yield return "x-ms-continuation";
      if (this.ETag != null)
        yield return "etag";
      if (this.RetryAfterInMilliseconds != null)
        yield return "x-ms-retry-after-ms";
      if (this.IndexingDirective != null)
        yield return "x-ms-indexing-directive";
      if (this.MaxResourceQuota != null)
        yield return "x-ms-resource-quota";
      if (this.CurrentResourceQuotaUsage != null)
        yield return "x-ms-resource-usage";
      if (this.SchemaVersion != null)
        yield return "x-ms-schemaversion";
      if (this.CollectionPartitionIndex != null)
        yield return "collection-partition-index";
      if (this.CollectionServiceIndex != null)
        yield return "collection-service-index";
      if (this.LSN != null)
        yield return "lsn";
      if (this.ItemCount != null)
        yield return "x-ms-item-count";
      if (this.RequestCharge != null)
        yield return "x-ms-request-charge";
      if (this.OwnerFullName != null)
        yield return "x-ms-alt-content-path";
      if (this.OwnerId != null)
        yield return "x-ms-content-path";
      if (this.DatabaseAccountId != null)
        yield return "x-ms-database-account-id";
      if (this.QuorumAckedLSN != null)
        yield return "x-ms-quorum-acked-lsn";
      if (this.RequestValidationFailure != null)
        yield return "x-ms-request-validation-failure";
      if (this.SubStatus != null)
        yield return "x-ms-substatus";
      if (this.CollectionIndexTransformationProgress != null)
        yield return "x-ms-documentdb-collection-index-transformation-progress";
      if (this.CurrentWriteQuorum != null)
        yield return "x-ms-current-write-quorum";
      if (this.CurrentReplicaSetSize != null)
        yield return "x-ms-current-replica-set-size";
      if (this.CollectionLazyIndexingProgress != null)
        yield return "x-ms-documentdb-collection-lazy-indexing-progress";
      if (this.PartitionKeyRangeId != null)
        yield return "x-ms-documentdb-partitionkeyrangeid";
      if (this.LogResults != null)
        yield return "x-ms-documentdb-script-log-results";
      if (this.XPRole != null)
        yield return "x-ms-xp-role";
      if (this.IsRUPerMinuteUsed != null)
        yield return "x-ms-documentdb-is-ru-per-minute-used";
      if (this.QueryMetrics != null)
        yield return "x-ms-documentdb-query-metrics";
      if (this.QueryExecutionInfo != null)
        yield return "x-ms-cosmos-query-execution-info";
      if (this.IndexUtilization != null)
        yield return "x-ms-cosmos-index-utilization";
      if (this.GlobalCommittedLSN != null)
        yield return "x-ms-global-Committed-lsn";
      if (this.NumberOfReadRegions != null)
        yield return "x-ms-number-of-read-regions";
      if (this.OfferReplacePending != null)
        yield return "x-ms-offer-replace-pending";
      if (this.ItemLSN != null)
        yield return "x-ms-item-lsn";
      if (this.RestoreState != null)
        yield return "x-ms-restore-state";
      if (this.CollectionSecurityIdentifier != null)
        yield return "x-ms-collection-security-identifier";
      if (this.TransportRequestID != null)
        yield return "x-ms-transport-request-id";
      if (this.ShareThroughput != null)
        yield return "x-ms-share-throughput";
      if (this.DisableRntbdChannel != null)
        yield return "x-ms-disable-rntbd-channel";
      if (this.XDate != null)
        yield return "x-ms-date";
      if (this.LocalLSN != null)
        yield return "x-ms-cosmos-llsn";
      if (this.QuorumAckedLocalLSN != null)
        yield return "x-ms-cosmos-quorum-acked-llsn";
      if (this.ItemLocalLSN != null)
        yield return "x-ms-cosmos-item-llsn";
      if (this.HasTentativeWrites != null)
        yield return "x-ms-cosmosdb-has-tentative-writes";
      if (this.SessionToken != null)
        yield return "x-ms-session-token";
      if (this.ReplicatorLSNToGLSNDelta != null)
        yield return "x-ms-cosmos-replicator-glsn-delta";
      if (this.ReplicatorLSNToLLSNDelta != null)
        yield return "x-ms-cosmos-replicator-llsn-delta";
      if (this.VectorClockLocalProgress != null)
        yield return "x-ms-cosmos-vectorclock-local-progress";
      if (this.MinimumRUsForOffer != null)
        yield return "x-ms-cosmos-min-throughput";
      if (this.XPConfigurationSessionsCount != null)
        yield return "x-ms-cosmos-xpconfiguration-sessions-count";
      if (this.UnflushedMergLogEntryCount != null)
        yield return "x-ms-cosmos-internal-unflushed-merge-log-entry-count";
      if (this.ResourceId != null)
        yield return "x-docdb-resource-id";
      if (this.TimeToLiveInSeconds != null)
        yield return "x-ms-time-to-live-in-seconds";
      if (this.ReplicaStatusRevoked != null)
        yield return "x-ms-cosmos-is-replica-status-revoked";
      if (this.SoftMaxAllowedThroughput != null)
        yield return "x-ms-cosmos-offer-max-allowed-throughput";
      if (this.BackendRequestDurationMilliseconds != null)
        yield return "x-ms-request-duration-ms";
      if (this.ServerVersion != null)
        yield return "x-ms-serviceversion";
      if (this.ConfirmedStoreChecksum != null)
        yield return "x-ms-cosmos-replica-confirmed-checksum";
      if (this.TentativeStoreChecksum != null)
        yield return "x-ms-cosmos-replica-tentative-checksum";
      if (this.CorrelatedActivityId != null)
        yield return "x-ms-cosmos-correlated-activityid";
      if (this.PendingPKDelete != null)
        yield return "x-ms-cosmos-is-partition-key-delete-pending";
      if (this.AadAppliedRoleAssignmentId != null)
        yield return "x-ms-aad-applied-role-assignment";
      if (this.CollectionUniqueIndexReIndexProgress != null)
        yield return "x-ms-cosmos-collection-unique-index-reindex-progress";
      if (this.CollectionUniqueKeysUnderReIndex != null)
        yield return "x-ms-cosmos-collection-unique-keys-under-reindex";
      if (this.AnalyticalMigrationProgress != null)
        yield return "x-ms-cosmos-analytical-migration-progress";
      if (this.TotalAccountThroughput != null)
        yield return "x-ms-cosmos-total-account-throughput";
      if (this.ByokEncryptionProgress != null)
        yield return "x-ms-cosmos-byok-encryption-progress";
      if (this.AppliedPolicyElementId != null)
        yield return "x-ms-applied-policy-element";
      if (this.MergeProgressBlocked != null)
        yield return "x-ms-cosmos-is-merge-progress-blocked";
      if (this.ChangeFeedInfo != null)
        yield return "x-ms-cosmos-changefeed-info";
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
            this.nameValueCollection = new NameValueCollection(this.Count(), (IEqualityComparer) StoreResponseNameValueCollection.DefaultStringComparer);
            if (this.ActivityId != null)
              this.nameValueCollection.Add("x-ms-activity-id", this.ActivityId);
            if (this.LastStateChangeUtc != null)
              this.nameValueCollection.Add("x-ms-last-state-change-utc", this.LastStateChangeUtc);
            if (this.Continuation != null)
              this.nameValueCollection.Add("x-ms-continuation", this.Continuation);
            if (this.ETag != null)
              this.nameValueCollection.Add("etag", this.ETag);
            if (this.RetryAfterInMilliseconds != null)
              this.nameValueCollection.Add("x-ms-retry-after-ms", this.RetryAfterInMilliseconds);
            if (this.IndexingDirective != null)
              this.nameValueCollection.Add("x-ms-indexing-directive", this.IndexingDirective);
            if (this.MaxResourceQuota != null)
              this.nameValueCollection.Add("x-ms-resource-quota", this.MaxResourceQuota);
            if (this.CurrentResourceQuotaUsage != null)
              this.nameValueCollection.Add("x-ms-resource-usage", this.CurrentResourceQuotaUsage);
            if (this.SchemaVersion != null)
              this.nameValueCollection.Add("x-ms-schemaversion", this.SchemaVersion);
            if (this.CollectionPartitionIndex != null)
              this.nameValueCollection.Add("collection-partition-index", this.CollectionPartitionIndex);
            if (this.CollectionServiceIndex != null)
              this.nameValueCollection.Add("collection-service-index", this.CollectionServiceIndex);
            if (this.LSN != null)
              this.nameValueCollection.Add("lsn", this.LSN);
            if (this.ItemCount != null)
              this.nameValueCollection.Add("x-ms-item-count", this.ItemCount);
            if (this.RequestCharge != null)
              this.nameValueCollection.Add("x-ms-request-charge", this.RequestCharge);
            if (this.OwnerFullName != null)
              this.nameValueCollection.Add("x-ms-alt-content-path", this.OwnerFullName);
            if (this.OwnerId != null)
              this.nameValueCollection.Add("x-ms-content-path", this.OwnerId);
            if (this.DatabaseAccountId != null)
              this.nameValueCollection.Add("x-ms-database-account-id", this.DatabaseAccountId);
            if (this.QuorumAckedLSN != null)
              this.nameValueCollection.Add("x-ms-quorum-acked-lsn", this.QuorumAckedLSN);
            if (this.RequestValidationFailure != null)
              this.nameValueCollection.Add("x-ms-request-validation-failure", this.RequestValidationFailure);
            if (this.SubStatus != null)
              this.nameValueCollection.Add("x-ms-substatus", this.SubStatus);
            if (this.CollectionIndexTransformationProgress != null)
              this.nameValueCollection.Add("x-ms-documentdb-collection-index-transformation-progress", this.CollectionIndexTransformationProgress);
            if (this.CurrentWriteQuorum != null)
              this.nameValueCollection.Add("x-ms-current-write-quorum", this.CurrentWriteQuorum);
            if (this.CurrentReplicaSetSize != null)
              this.nameValueCollection.Add("x-ms-current-replica-set-size", this.CurrentReplicaSetSize);
            if (this.CollectionLazyIndexingProgress != null)
              this.nameValueCollection.Add("x-ms-documentdb-collection-lazy-indexing-progress", this.CollectionLazyIndexingProgress);
            if (this.PartitionKeyRangeId != null)
              this.nameValueCollection.Add("x-ms-documentdb-partitionkeyrangeid", this.PartitionKeyRangeId);
            if (this.LogResults != null)
              this.nameValueCollection.Add("x-ms-documentdb-script-log-results", this.LogResults);
            if (this.XPRole != null)
              this.nameValueCollection.Add("x-ms-xp-role", this.XPRole);
            if (this.IsRUPerMinuteUsed != null)
              this.nameValueCollection.Add("x-ms-documentdb-is-ru-per-minute-used", this.IsRUPerMinuteUsed);
            if (this.QueryMetrics != null)
              this.nameValueCollection.Add("x-ms-documentdb-query-metrics", this.QueryMetrics);
            if (this.QueryExecutionInfo != null)
              this.nameValueCollection.Add("x-ms-cosmos-query-execution-info", this.QueryExecutionInfo);
            if (this.IndexUtilization != null)
              this.nameValueCollection.Add("x-ms-cosmos-index-utilization", this.IndexUtilization);
            if (this.GlobalCommittedLSN != null)
              this.nameValueCollection.Add("x-ms-global-Committed-lsn", this.GlobalCommittedLSN);
            if (this.NumberOfReadRegions != null)
              this.nameValueCollection.Add("x-ms-number-of-read-regions", this.NumberOfReadRegions);
            if (this.OfferReplacePending != null)
              this.nameValueCollection.Add("x-ms-offer-replace-pending", this.OfferReplacePending);
            if (this.ItemLSN != null)
              this.nameValueCollection.Add("x-ms-item-lsn", this.ItemLSN);
            if (this.RestoreState != null)
              this.nameValueCollection.Add("x-ms-restore-state", this.RestoreState);
            if (this.CollectionSecurityIdentifier != null)
              this.nameValueCollection.Add("x-ms-collection-security-identifier", this.CollectionSecurityIdentifier);
            if (this.TransportRequestID != null)
              this.nameValueCollection.Add("x-ms-transport-request-id", this.TransportRequestID);
            if (this.ShareThroughput != null)
              this.nameValueCollection.Add("x-ms-share-throughput", this.ShareThroughput);
            if (this.DisableRntbdChannel != null)
              this.nameValueCollection.Add("x-ms-disable-rntbd-channel", this.DisableRntbdChannel);
            if (this.XDate != null)
              this.nameValueCollection.Add("x-ms-date", this.XDate);
            if (this.LocalLSN != null)
              this.nameValueCollection.Add("x-ms-cosmos-llsn", this.LocalLSN);
            if (this.QuorumAckedLocalLSN != null)
              this.nameValueCollection.Add("x-ms-cosmos-quorum-acked-llsn", this.QuorumAckedLocalLSN);
            if (this.ItemLocalLSN != null)
              this.nameValueCollection.Add("x-ms-cosmos-item-llsn", this.ItemLocalLSN);
            if (this.HasTentativeWrites != null)
              this.nameValueCollection.Add("x-ms-cosmosdb-has-tentative-writes", this.HasTentativeWrites);
            if (this.SessionToken != null)
              this.nameValueCollection.Add("x-ms-session-token", this.SessionToken);
            if (this.ReplicatorLSNToGLSNDelta != null)
              this.nameValueCollection.Add("x-ms-cosmos-replicator-glsn-delta", this.ReplicatorLSNToGLSNDelta);
            if (this.ReplicatorLSNToLLSNDelta != null)
              this.nameValueCollection.Add("x-ms-cosmos-replicator-llsn-delta", this.ReplicatorLSNToLLSNDelta);
            if (this.VectorClockLocalProgress != null)
              this.nameValueCollection.Add("x-ms-cosmos-vectorclock-local-progress", this.VectorClockLocalProgress);
            if (this.MinimumRUsForOffer != null)
              this.nameValueCollection.Add("x-ms-cosmos-min-throughput", this.MinimumRUsForOffer);
            if (this.XPConfigurationSessionsCount != null)
              this.nameValueCollection.Add("x-ms-cosmos-xpconfiguration-sessions-count", this.XPConfigurationSessionsCount);
            if (this.UnflushedMergLogEntryCount != null)
              this.nameValueCollection.Add("x-ms-cosmos-internal-unflushed-merge-log-entry-count", this.UnflushedMergLogEntryCount);
            if (this.ResourceId != null)
              this.nameValueCollection.Add("x-docdb-resource-id", this.ResourceId);
            if (this.TimeToLiveInSeconds != null)
              this.nameValueCollection.Add("x-ms-time-to-live-in-seconds", this.TimeToLiveInSeconds);
            if (this.ReplicaStatusRevoked != null)
              this.nameValueCollection.Add("x-ms-cosmos-is-replica-status-revoked", this.ReplicaStatusRevoked);
            if (this.SoftMaxAllowedThroughput != null)
              this.nameValueCollection.Add("x-ms-cosmos-offer-max-allowed-throughput", this.SoftMaxAllowedThroughput);
            if (this.BackendRequestDurationMilliseconds != null)
              this.nameValueCollection.Add("x-ms-request-duration-ms", this.BackendRequestDurationMilliseconds);
            if (this.ServerVersion != null)
              this.nameValueCollection.Add("x-ms-serviceversion", this.ServerVersion);
            if (this.ConfirmedStoreChecksum != null)
              this.nameValueCollection.Add("x-ms-cosmos-replica-confirmed-checksum", this.ConfirmedStoreChecksum);
            if (this.TentativeStoreChecksum != null)
              this.nameValueCollection.Add("x-ms-cosmos-replica-tentative-checksum", this.TentativeStoreChecksum);
            if (this.CorrelatedActivityId != null)
              this.nameValueCollection.Add("x-ms-cosmos-correlated-activityid", this.CorrelatedActivityId);
            if (this.PendingPKDelete != null)
              this.nameValueCollection.Add("x-ms-cosmos-is-partition-key-delete-pending", this.PendingPKDelete);
            if (this.AadAppliedRoleAssignmentId != null)
              this.nameValueCollection.Add("x-ms-aad-applied-role-assignment", this.AadAppliedRoleAssignmentId);
            if (this.CollectionUniqueIndexReIndexProgress != null)
              this.nameValueCollection.Add("x-ms-cosmos-collection-unique-index-reindex-progress", this.CollectionUniqueIndexReIndexProgress);
            if (this.CollectionUniqueKeysUnderReIndex != null)
              this.nameValueCollection.Add("x-ms-cosmos-collection-unique-keys-under-reindex", this.CollectionUniqueKeysUnderReIndex);
            if (this.AnalyticalMigrationProgress != null)
              this.nameValueCollection.Add("x-ms-cosmos-analytical-migration-progress", this.AnalyticalMigrationProgress);
            if (this.TotalAccountThroughput != null)
              this.nameValueCollection.Add("x-ms-cosmos-total-account-throughput", this.TotalAccountThroughput);
            if (this.ByokEncryptionProgress != null)
              this.nameValueCollection.Add("x-ms-cosmos-byok-encryption-progress", this.ByokEncryptionProgress);
            if (this.AppliedPolicyElementId != null)
              this.nameValueCollection.Add("x-ms-applied-policy-element", this.AppliedPolicyElementId);
            if (this.MergeProgressBlocked != null)
              this.nameValueCollection.Add("x-ms-cosmos-is-merge-progress-blocked", this.MergeProgressBlocked);
            if (this.ChangeFeedInfo != null)
              this.nameValueCollection.Add("x-ms-cosmos-changefeed-info", this.ChangeFeedInfo);
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
        case 3:
          if (string.Equals("lsn", key, StringComparison.OrdinalIgnoreCase))
            return this.LSN;
          break;
        case 4:
          if (string.Equals("etag", key, StringComparison.OrdinalIgnoreCase))
            return this.ETag;
          break;
        case 9:
          if (string.Equals("x-ms-date", key, StringComparison.OrdinalIgnoreCase))
            return this.XDate;
          break;
        case 12:
          if (string.Equals("x-ms-xp-role", key, StringComparison.OrdinalIgnoreCase))
            return this.XPRole;
          break;
        case 13:
          if (string.Equals("x-ms-item-lsn", key, StringComparison.OrdinalIgnoreCase))
            return this.ItemLSN;
          break;
        case 14:
          if (string.Equals("x-ms-substatus", key, StringComparison.OrdinalIgnoreCase))
            return this.SubStatus;
          break;
        case 15:
          if (string.Equals("x-ms-item-count", key, StringComparison.OrdinalIgnoreCase))
            return this.ItemCount;
          break;
        case 16:
          if ((object) "x-ms-activity-id" == (object) key)
            return this.ActivityId;
          if ((object) "x-ms-cosmos-llsn" == (object) key)
            return this.LocalLSN;
          if (string.Equals("x-ms-activity-id", key, StringComparison.OrdinalIgnoreCase))
            return this.ActivityId;
          if (string.Equals("x-ms-cosmos-llsn", key, StringComparison.OrdinalIgnoreCase))
            return this.LocalLSN;
          break;
        case 17:
          if ((object) "x-ms-continuation" == (object) key)
            return this.Continuation;
          if ((object) "x-ms-content-path" == (object) key)
            return this.OwnerId;
          if (string.Equals("x-ms-continuation", key, StringComparison.OrdinalIgnoreCase))
            return this.Continuation;
          if (string.Equals("x-ms-content-path", key, StringComparison.OrdinalIgnoreCase))
            return this.OwnerId;
          break;
        case 18:
          if ((object) "x-ms-schemaversion" == (object) key)
            return this.SchemaVersion;
          if ((object) "x-ms-restore-state" == (object) key)
            return this.RestoreState;
          if ((object) "x-ms-session-token" == (object) key)
            return this.SessionToken;
          if (string.Equals("x-ms-schemaversion", key, StringComparison.OrdinalIgnoreCase))
            return this.SchemaVersion;
          if (string.Equals("x-ms-restore-state", key, StringComparison.OrdinalIgnoreCase))
            return this.RestoreState;
          if (string.Equals("x-ms-session-token", key, StringComparison.OrdinalIgnoreCase))
            return this.SessionToken;
          break;
        case 19:
          if ((object) "x-ms-retry-after-ms" == (object) key)
            return this.RetryAfterInMilliseconds;
          if ((object) "x-ms-resource-quota" == (object) key)
            return this.MaxResourceQuota;
          if ((object) "x-ms-resource-usage" == (object) key)
            return this.CurrentResourceQuotaUsage;
          if ((object) "x-ms-request-charge" == (object) key)
            return this.RequestCharge;
          if ((object) "x-docdb-resource-id" == (object) key)
            return this.ResourceId;
          if ((object) "x-ms-serviceversion" == (object) key)
            return this.ServerVersion;
          if (string.Equals("x-ms-retry-after-ms", key, StringComparison.OrdinalIgnoreCase))
            return this.RetryAfterInMilliseconds;
          if (string.Equals("x-ms-resource-quota", key, StringComparison.OrdinalIgnoreCase))
            return this.MaxResourceQuota;
          if (string.Equals("x-ms-resource-usage", key, StringComparison.OrdinalIgnoreCase))
            return this.CurrentResourceQuotaUsage;
          if (string.Equals("x-ms-request-charge", key, StringComparison.OrdinalIgnoreCase))
            return this.RequestCharge;
          if (string.Equals("x-docdb-resource-id", key, StringComparison.OrdinalIgnoreCase))
            return this.ResourceId;
          if (string.Equals("x-ms-serviceversion", key, StringComparison.OrdinalIgnoreCase))
            return this.ServerVersion;
          break;
        case 21:
          if ((object) "x-ms-alt-content-path" == (object) key)
            return this.OwnerFullName;
          if ((object) "x-ms-quorum-acked-lsn" == (object) key)
            return this.QuorumAckedLSN;
          if ((object) "x-ms-share-throughput" == (object) key)
            return this.ShareThroughput;
          if ((object) "x-ms-cosmos-item-llsn" == (object) key)
            return this.ItemLocalLSN;
          if (string.Equals("x-ms-alt-content-path", key, StringComparison.OrdinalIgnoreCase))
            return this.OwnerFullName;
          if (string.Equals("x-ms-quorum-acked-lsn", key, StringComparison.OrdinalIgnoreCase))
            return this.QuorumAckedLSN;
          if (string.Equals("x-ms-share-throughput", key, StringComparison.OrdinalIgnoreCase))
            return this.ShareThroughput;
          if (string.Equals("x-ms-cosmos-item-llsn", key, StringComparison.OrdinalIgnoreCase))
            return this.ItemLocalLSN;
          break;
        case 23:
          if (string.Equals("x-ms-indexing-directive", key, StringComparison.OrdinalIgnoreCase))
            return this.IndexingDirective;
          break;
        case 24:
          if ((object) "collection-service-index" == (object) key)
            return this.CollectionServiceIndex;
          if ((object) "x-ms-database-account-id" == (object) key)
            return this.DatabaseAccountId;
          if ((object) "x-ms-request-duration-ms" == (object) key)
            return this.BackendRequestDurationMilliseconds;
          if (string.Equals("collection-service-index", key, StringComparison.OrdinalIgnoreCase))
            return this.CollectionServiceIndex;
          if (string.Equals("x-ms-database-account-id", key, StringComparison.OrdinalIgnoreCase))
            return this.DatabaseAccountId;
          if (string.Equals("x-ms-request-duration-ms", key, StringComparison.OrdinalIgnoreCase))
            return this.BackendRequestDurationMilliseconds;
          break;
        case 25:
          if ((object) "x-ms-current-write-quorum" == (object) key)
            return this.CurrentWriteQuorum;
          if ((object) "x-ms-global-Committed-lsn" == (object) key)
            return this.GlobalCommittedLSN;
          if ((object) "x-ms-transport-request-id" == (object) key)
            return this.TransportRequestID;
          if (string.Equals("x-ms-current-write-quorum", key, StringComparison.OrdinalIgnoreCase))
            return this.CurrentWriteQuorum;
          if (string.Equals("x-ms-global-Committed-lsn", key, StringComparison.OrdinalIgnoreCase))
            return this.GlobalCommittedLSN;
          if (string.Equals("x-ms-transport-request-id", key, StringComparison.OrdinalIgnoreCase))
            return this.TransportRequestID;
          break;
        case 26:
          if ((object) "x-ms-last-state-change-utc" == (object) key)
            return this.LastStateChangeUtc;
          if ((object) "collection-partition-index" == (object) key)
            return this.CollectionPartitionIndex;
          if ((object) "x-ms-offer-replace-pending" == (object) key)
            return this.OfferReplacePending;
          if ((object) "x-ms-disable-rntbd-channel" == (object) key)
            return this.DisableRntbdChannel;
          if ((object) "x-ms-cosmos-min-throughput" == (object) key)
            return this.MinimumRUsForOffer;
          if (string.Equals("x-ms-last-state-change-utc", key, StringComparison.OrdinalIgnoreCase))
            return this.LastStateChangeUtc;
          if (string.Equals("collection-partition-index", key, StringComparison.OrdinalIgnoreCase))
            return this.CollectionPartitionIndex;
          if (string.Equals("x-ms-offer-replace-pending", key, StringComparison.OrdinalIgnoreCase))
            return this.OfferReplacePending;
          if (string.Equals("x-ms-disable-rntbd-channel", key, StringComparison.OrdinalIgnoreCase))
            return this.DisableRntbdChannel;
          if (string.Equals("x-ms-cosmos-min-throughput", key, StringComparison.OrdinalIgnoreCase))
            return this.MinimumRUsForOffer;
          break;
        case 27:
          if ((object) "x-ms-number-of-read-regions" == (object) key)
            return this.NumberOfReadRegions;
          if ((object) "x-ms-applied-policy-element" == (object) key)
            return this.AppliedPolicyElementId;
          if ((object) "x-ms-cosmos-changefeed-info" == (object) key)
            return this.ChangeFeedInfo;
          if (string.Equals("x-ms-number-of-read-regions", key, StringComparison.OrdinalIgnoreCase))
            return this.NumberOfReadRegions;
          if (string.Equals("x-ms-applied-policy-element", key, StringComparison.OrdinalIgnoreCase))
            return this.AppliedPolicyElementId;
          if (string.Equals("x-ms-cosmos-changefeed-info", key, StringComparison.OrdinalIgnoreCase))
            return this.ChangeFeedInfo;
          break;
        case 28:
          if (string.Equals("x-ms-time-to-live-in-seconds", key, StringComparison.OrdinalIgnoreCase))
            return this.TimeToLiveInSeconds;
          break;
        case 29:
          if ((object) "x-ms-current-replica-set-size" == (object) key)
            return this.CurrentReplicaSetSize;
          if ((object) "x-ms-documentdb-query-metrics" == (object) key)
            return this.QueryMetrics;
          if ((object) "x-ms-cosmos-index-utilization" == (object) key)
            return this.IndexUtilization;
          if ((object) "x-ms-cosmos-quorum-acked-llsn" == (object) key)
            return this.QuorumAckedLocalLSN;
          if (string.Equals("x-ms-current-replica-set-size", key, StringComparison.OrdinalIgnoreCase))
            return this.CurrentReplicaSetSize;
          if (string.Equals("x-ms-documentdb-query-metrics", key, StringComparison.OrdinalIgnoreCase))
            return this.QueryMetrics;
          if (string.Equals("x-ms-cosmos-index-utilization", key, StringComparison.OrdinalIgnoreCase))
            return this.IndexUtilization;
          if (string.Equals("x-ms-cosmos-quorum-acked-llsn", key, StringComparison.OrdinalIgnoreCase))
            return this.QuorumAckedLocalLSN;
          break;
        case 31:
          if (string.Equals("x-ms-request-validation-failure", key, StringComparison.OrdinalIgnoreCase))
            return this.RequestValidationFailure;
          break;
        case 32:
          if ((object) "x-ms-cosmos-query-execution-info" == (object) key)
            return this.QueryExecutionInfo;
          if ((object) "x-ms-aad-applied-role-assignment" == (object) key)
            return this.AadAppliedRoleAssignmentId;
          if (string.Equals("x-ms-cosmos-query-execution-info", key, StringComparison.OrdinalIgnoreCase))
            return this.QueryExecutionInfo;
          if (string.Equals("x-ms-aad-applied-role-assignment", key, StringComparison.OrdinalIgnoreCase))
            return this.AadAppliedRoleAssignmentId;
          break;
        case 33:
          if ((object) "x-ms-cosmos-replicator-glsn-delta" == (object) key)
            return this.ReplicatorLSNToGLSNDelta;
          if ((object) "x-ms-cosmos-replicator-llsn-delta" == (object) key)
            return this.ReplicatorLSNToLLSNDelta;
          if ((object) "x-ms-cosmos-correlated-activityid" == (object) key)
            return this.CorrelatedActivityId;
          if (string.Equals("x-ms-cosmos-replicator-glsn-delta", key, StringComparison.OrdinalIgnoreCase))
            return this.ReplicatorLSNToGLSNDelta;
          if (string.Equals("x-ms-cosmos-replicator-llsn-delta", key, StringComparison.OrdinalIgnoreCase))
            return this.ReplicatorLSNToLLSNDelta;
          if (string.Equals("x-ms-cosmos-correlated-activityid", key, StringComparison.OrdinalIgnoreCase))
            return this.CorrelatedActivityId;
          break;
        case 34:
          if ((object) "x-ms-documentdb-script-log-results" == (object) key)
            return this.LogResults;
          if ((object) "x-ms-cosmosdb-has-tentative-writes" == (object) key)
            return this.HasTentativeWrites;
          if (string.Equals("x-ms-documentdb-script-log-results", key, StringComparison.OrdinalIgnoreCase))
            return this.LogResults;
          if (string.Equals("x-ms-cosmosdb-has-tentative-writes", key, StringComparison.OrdinalIgnoreCase))
            return this.HasTentativeWrites;
          break;
        case 35:
          if ((object) "x-ms-documentdb-partitionkeyrangeid" == (object) key)
            return this.PartitionKeyRangeId;
          if ((object) "x-ms-collection-security-identifier" == (object) key)
            return this.CollectionSecurityIdentifier;
          if (string.Equals("x-ms-documentdb-partitionkeyrangeid", key, StringComparison.OrdinalIgnoreCase))
            return this.PartitionKeyRangeId;
          if (string.Equals("x-ms-collection-security-identifier", key, StringComparison.OrdinalIgnoreCase))
            return this.CollectionSecurityIdentifier;
          break;
        case 36:
          if ((object) "x-ms-cosmos-total-account-throughput" == (object) key)
            return this.TotalAccountThroughput;
          if ((object) "x-ms-cosmos-byok-encryption-progress" == (object) key)
            return this.ByokEncryptionProgress;
          if (string.Equals("x-ms-cosmos-total-account-throughput", key, StringComparison.OrdinalIgnoreCase))
            return this.TotalAccountThroughput;
          if (string.Equals("x-ms-cosmos-byok-encryption-progress", key, StringComparison.OrdinalIgnoreCase))
            return this.ByokEncryptionProgress;
          break;
        case 37:
          if ((object) "x-ms-documentdb-is-ru-per-minute-used" == (object) key)
            return this.IsRUPerMinuteUsed;
          if ((object) "x-ms-cosmos-is-replica-status-revoked" == (object) key)
            return this.ReplicaStatusRevoked;
          if ((object) "x-ms-cosmos-is-merge-progress-blocked" == (object) key)
            return this.MergeProgressBlocked;
          if (string.Equals("x-ms-documentdb-is-ru-per-minute-used", key, StringComparison.OrdinalIgnoreCase))
            return this.IsRUPerMinuteUsed;
          if (string.Equals("x-ms-cosmos-is-replica-status-revoked", key, StringComparison.OrdinalIgnoreCase))
            return this.ReplicaStatusRevoked;
          if (string.Equals("x-ms-cosmos-is-merge-progress-blocked", key, StringComparison.OrdinalIgnoreCase))
            return this.MergeProgressBlocked;
          break;
        case 38:
          if ((object) "x-ms-cosmos-vectorclock-local-progress" == (object) key)
            return this.VectorClockLocalProgress;
          if ((object) "x-ms-cosmos-replica-confirmed-checksum" == (object) key)
            return this.ConfirmedStoreChecksum;
          if ((object) "x-ms-cosmos-replica-tentative-checksum" == (object) key)
            return this.TentativeStoreChecksum;
          if (string.Equals("x-ms-cosmos-vectorclock-local-progress", key, StringComparison.OrdinalIgnoreCase))
            return this.VectorClockLocalProgress;
          if (string.Equals("x-ms-cosmos-replica-confirmed-checksum", key, StringComparison.OrdinalIgnoreCase))
            return this.ConfirmedStoreChecksum;
          if (string.Equals("x-ms-cosmos-replica-tentative-checksum", key, StringComparison.OrdinalIgnoreCase))
            return this.TentativeStoreChecksum;
          break;
        case 40:
          if (string.Equals("x-ms-cosmos-offer-max-allowed-throughput", key, StringComparison.OrdinalIgnoreCase))
            return this.SoftMaxAllowedThroughput;
          break;
        case 41:
          if (string.Equals("x-ms-cosmos-analytical-migration-progress", key, StringComparison.OrdinalIgnoreCase))
            return this.AnalyticalMigrationProgress;
          break;
        case 42:
          if (string.Equals("x-ms-cosmos-xpconfiguration-sessions-count", key, StringComparison.OrdinalIgnoreCase))
            return this.XPConfigurationSessionsCount;
          break;
        case 43:
          if (string.Equals("x-ms-cosmos-is-partition-key-delete-pending", key, StringComparison.OrdinalIgnoreCase))
            return this.PendingPKDelete;
          break;
        case 48:
          if (string.Equals("x-ms-cosmos-collection-unique-keys-under-reindex", key, StringComparison.OrdinalIgnoreCase))
            return this.CollectionUniqueKeysUnderReIndex;
          break;
        case 49:
          if (string.Equals("x-ms-documentdb-collection-lazy-indexing-progress", key, StringComparison.OrdinalIgnoreCase))
            return this.CollectionLazyIndexingProgress;
          break;
        case 52:
          if ((object) "x-ms-cosmos-internal-unflushed-merge-log-entry-count" == (object) key)
            return this.UnflushedMergLogEntryCount;
          if ((object) "x-ms-cosmos-collection-unique-index-reindex-progress" == (object) key)
            return this.CollectionUniqueIndexReIndexProgress;
          if (string.Equals("x-ms-cosmos-internal-unflushed-merge-log-entry-count", key, StringComparison.OrdinalIgnoreCase))
            return this.UnflushedMergLogEntryCount;
          if (string.Equals("x-ms-cosmos-collection-unique-index-reindex-progress", key, StringComparison.OrdinalIgnoreCase))
            return this.CollectionUniqueIndexReIndexProgress;
          break;
        case 56:
          if (string.Equals("x-ms-documentdb-collection-index-transformation-progress", key, StringComparison.OrdinalIgnoreCase))
            return this.CollectionIndexTransformationProgress;
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
      if (value == null)
        throw new ArgumentNullException(nameof (value));
      this.UpdateHelper(key, value, false);
    }

    public void UpdateHelper(string key, string value, bool throwIfAlreadyExists)
    {
      if (key == null)
        throw new ArgumentNullException(nameof (key));
      switch (key.Length)
      {
        case 3:
          if (string.Equals("lsn", key, StringComparison.OrdinalIgnoreCase))
          {
            this.LSN = !throwIfAlreadyExists || this.LSN == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          break;
        case 4:
          if (string.Equals("etag", key, StringComparison.OrdinalIgnoreCase))
          {
            this.ETag = !throwIfAlreadyExists || this.ETag == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
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
        case 12:
          if (string.Equals("x-ms-xp-role", key, StringComparison.OrdinalIgnoreCase))
          {
            this.XPRole = !throwIfAlreadyExists || this.XPRole == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          break;
        case 13:
          if (string.Equals("x-ms-item-lsn", key, StringComparison.OrdinalIgnoreCase))
          {
            this.ItemLSN = !throwIfAlreadyExists || this.ItemLSN == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          break;
        case 14:
          if (string.Equals("x-ms-substatus", key, StringComparison.OrdinalIgnoreCase))
          {
            this.SubStatus = !throwIfAlreadyExists || this.SubStatus == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          break;
        case 15:
          if (string.Equals("x-ms-item-count", key, StringComparison.OrdinalIgnoreCase))
          {
            this.ItemCount = !throwIfAlreadyExists || this.ItemCount == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          break;
        case 16:
          if ((object) "x-ms-activity-id" == (object) key)
          {
            this.ActivityId = !throwIfAlreadyExists || this.ActivityId == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if ((object) "x-ms-cosmos-llsn" == (object) key)
          {
            this.LocalLSN = !throwIfAlreadyExists || this.LocalLSN == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-activity-id", key, StringComparison.OrdinalIgnoreCase))
          {
            this.ActivityId = !throwIfAlreadyExists || this.ActivityId == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-cosmos-llsn", key, StringComparison.OrdinalIgnoreCase))
          {
            this.LocalLSN = !throwIfAlreadyExists || this.LocalLSN == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          break;
        case 17:
          if ((object) "x-ms-continuation" == (object) key)
          {
            this.Continuation = !throwIfAlreadyExists || this.Continuation == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if ((object) "x-ms-content-path" == (object) key)
          {
            this.OwnerId = !throwIfAlreadyExists || this.OwnerId == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-continuation", key, StringComparison.OrdinalIgnoreCase))
          {
            this.Continuation = !throwIfAlreadyExists || this.Continuation == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-content-path", key, StringComparison.OrdinalIgnoreCase))
          {
            this.OwnerId = !throwIfAlreadyExists || this.OwnerId == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          break;
        case 18:
          if ((object) "x-ms-schemaversion" == (object) key)
          {
            this.SchemaVersion = !throwIfAlreadyExists || this.SchemaVersion == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if ((object) "x-ms-restore-state" == (object) key)
          {
            this.RestoreState = !throwIfAlreadyExists || this.RestoreState == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if ((object) "x-ms-session-token" == (object) key)
          {
            this.SessionToken = !throwIfAlreadyExists || this.SessionToken == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-schemaversion", key, StringComparison.OrdinalIgnoreCase))
          {
            this.SchemaVersion = !throwIfAlreadyExists || this.SchemaVersion == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-restore-state", key, StringComparison.OrdinalIgnoreCase))
          {
            this.RestoreState = !throwIfAlreadyExists || this.RestoreState == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-session-token", key, StringComparison.OrdinalIgnoreCase))
          {
            this.SessionToken = !throwIfAlreadyExists || this.SessionToken == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          break;
        case 19:
          if ((object) "x-ms-retry-after-ms" == (object) key)
          {
            this.RetryAfterInMilliseconds = !throwIfAlreadyExists || this.RetryAfterInMilliseconds == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if ((object) "x-ms-resource-quota" == (object) key)
          {
            this.MaxResourceQuota = !throwIfAlreadyExists || this.MaxResourceQuota == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if ((object) "x-ms-resource-usage" == (object) key)
          {
            this.CurrentResourceQuotaUsage = !throwIfAlreadyExists || this.CurrentResourceQuotaUsage == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if ((object) "x-ms-request-charge" == (object) key)
          {
            this.RequestCharge = !throwIfAlreadyExists || this.RequestCharge == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if ((object) "x-docdb-resource-id" == (object) key)
          {
            this.ResourceId = !throwIfAlreadyExists || this.ResourceId == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if ((object) "x-ms-serviceversion" == (object) key)
          {
            this.ServerVersion = !throwIfAlreadyExists || this.ServerVersion == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-retry-after-ms", key, StringComparison.OrdinalIgnoreCase))
          {
            this.RetryAfterInMilliseconds = !throwIfAlreadyExists || this.RetryAfterInMilliseconds == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-resource-quota", key, StringComparison.OrdinalIgnoreCase))
          {
            this.MaxResourceQuota = !throwIfAlreadyExists || this.MaxResourceQuota == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-resource-usage", key, StringComparison.OrdinalIgnoreCase))
          {
            this.CurrentResourceQuotaUsage = !throwIfAlreadyExists || this.CurrentResourceQuotaUsage == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-request-charge", key, StringComparison.OrdinalIgnoreCase))
          {
            this.RequestCharge = !throwIfAlreadyExists || this.RequestCharge == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-docdb-resource-id", key, StringComparison.OrdinalIgnoreCase))
          {
            this.ResourceId = !throwIfAlreadyExists || this.ResourceId == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-serviceversion", key, StringComparison.OrdinalIgnoreCase))
          {
            this.ServerVersion = !throwIfAlreadyExists || this.ServerVersion == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          break;
        case 21:
          if ((object) "x-ms-alt-content-path" == (object) key)
          {
            this.OwnerFullName = !throwIfAlreadyExists || this.OwnerFullName == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if ((object) "x-ms-quorum-acked-lsn" == (object) key)
          {
            this.QuorumAckedLSN = !throwIfAlreadyExists || this.QuorumAckedLSN == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if ((object) "x-ms-share-throughput" == (object) key)
          {
            this.ShareThroughput = !throwIfAlreadyExists || this.ShareThroughput == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if ((object) "x-ms-cosmos-item-llsn" == (object) key)
          {
            this.ItemLocalLSN = !throwIfAlreadyExists || this.ItemLocalLSN == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-alt-content-path", key, StringComparison.OrdinalIgnoreCase))
          {
            this.OwnerFullName = !throwIfAlreadyExists || this.OwnerFullName == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-quorum-acked-lsn", key, StringComparison.OrdinalIgnoreCase))
          {
            this.QuorumAckedLSN = !throwIfAlreadyExists || this.QuorumAckedLSN == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-share-throughput", key, StringComparison.OrdinalIgnoreCase))
          {
            this.ShareThroughput = !throwIfAlreadyExists || this.ShareThroughput == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-cosmos-item-llsn", key, StringComparison.OrdinalIgnoreCase))
          {
            this.ItemLocalLSN = !throwIfAlreadyExists || this.ItemLocalLSN == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          break;
        case 23:
          if (string.Equals("x-ms-indexing-directive", key, StringComparison.OrdinalIgnoreCase))
          {
            this.IndexingDirective = !throwIfAlreadyExists || this.IndexingDirective == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          break;
        case 24:
          if ((object) "collection-service-index" == (object) key)
          {
            this.CollectionServiceIndex = !throwIfAlreadyExists || this.CollectionServiceIndex == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if ((object) "x-ms-database-account-id" == (object) key)
          {
            this.DatabaseAccountId = !throwIfAlreadyExists || this.DatabaseAccountId == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if ((object) "x-ms-request-duration-ms" == (object) key)
          {
            this.BackendRequestDurationMilliseconds = !throwIfAlreadyExists || this.BackendRequestDurationMilliseconds == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("collection-service-index", key, StringComparison.OrdinalIgnoreCase))
          {
            this.CollectionServiceIndex = !throwIfAlreadyExists || this.CollectionServiceIndex == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-database-account-id", key, StringComparison.OrdinalIgnoreCase))
          {
            this.DatabaseAccountId = !throwIfAlreadyExists || this.DatabaseAccountId == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-request-duration-ms", key, StringComparison.OrdinalIgnoreCase))
          {
            this.BackendRequestDurationMilliseconds = !throwIfAlreadyExists || this.BackendRequestDurationMilliseconds == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          break;
        case 25:
          if ((object) "x-ms-current-write-quorum" == (object) key)
          {
            this.CurrentWriteQuorum = !throwIfAlreadyExists || this.CurrentWriteQuorum == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if ((object) "x-ms-global-Committed-lsn" == (object) key)
          {
            this.GlobalCommittedLSN = !throwIfAlreadyExists || this.GlobalCommittedLSN == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if ((object) "x-ms-transport-request-id" == (object) key)
          {
            this.TransportRequestID = !throwIfAlreadyExists || this.TransportRequestID == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-current-write-quorum", key, StringComparison.OrdinalIgnoreCase))
          {
            this.CurrentWriteQuorum = !throwIfAlreadyExists || this.CurrentWriteQuorum == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-global-Committed-lsn", key, StringComparison.OrdinalIgnoreCase))
          {
            this.GlobalCommittedLSN = !throwIfAlreadyExists || this.GlobalCommittedLSN == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-transport-request-id", key, StringComparison.OrdinalIgnoreCase))
          {
            this.TransportRequestID = !throwIfAlreadyExists || this.TransportRequestID == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          break;
        case 26:
          if ((object) "x-ms-last-state-change-utc" == (object) key)
          {
            this.LastStateChangeUtc = !throwIfAlreadyExists || this.LastStateChangeUtc == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if ((object) "collection-partition-index" == (object) key)
          {
            this.CollectionPartitionIndex = !throwIfAlreadyExists || this.CollectionPartitionIndex == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if ((object) "x-ms-offer-replace-pending" == (object) key)
          {
            this.OfferReplacePending = !throwIfAlreadyExists || this.OfferReplacePending == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if ((object) "x-ms-disable-rntbd-channel" == (object) key)
          {
            this.DisableRntbdChannel = !throwIfAlreadyExists || this.DisableRntbdChannel == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if ((object) "x-ms-cosmos-min-throughput" == (object) key)
          {
            this.MinimumRUsForOffer = !throwIfAlreadyExists || this.MinimumRUsForOffer == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-last-state-change-utc", key, StringComparison.OrdinalIgnoreCase))
          {
            this.LastStateChangeUtc = !throwIfAlreadyExists || this.LastStateChangeUtc == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("collection-partition-index", key, StringComparison.OrdinalIgnoreCase))
          {
            this.CollectionPartitionIndex = !throwIfAlreadyExists || this.CollectionPartitionIndex == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-offer-replace-pending", key, StringComparison.OrdinalIgnoreCase))
          {
            this.OfferReplacePending = !throwIfAlreadyExists || this.OfferReplacePending == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-disable-rntbd-channel", key, StringComparison.OrdinalIgnoreCase))
          {
            this.DisableRntbdChannel = !throwIfAlreadyExists || this.DisableRntbdChannel == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-cosmos-min-throughput", key, StringComparison.OrdinalIgnoreCase))
          {
            this.MinimumRUsForOffer = !throwIfAlreadyExists || this.MinimumRUsForOffer == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          break;
        case 27:
          if ((object) "x-ms-number-of-read-regions" == (object) key)
          {
            this.NumberOfReadRegions = !throwIfAlreadyExists || this.NumberOfReadRegions == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if ((object) "x-ms-applied-policy-element" == (object) key)
          {
            this.AppliedPolicyElementId = !throwIfAlreadyExists || this.AppliedPolicyElementId == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if ((object) "x-ms-cosmos-changefeed-info" == (object) key)
          {
            this.ChangeFeedInfo = !throwIfAlreadyExists || this.ChangeFeedInfo == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-number-of-read-regions", key, StringComparison.OrdinalIgnoreCase))
          {
            this.NumberOfReadRegions = !throwIfAlreadyExists || this.NumberOfReadRegions == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-applied-policy-element", key, StringComparison.OrdinalIgnoreCase))
          {
            this.AppliedPolicyElementId = !throwIfAlreadyExists || this.AppliedPolicyElementId == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-cosmos-changefeed-info", key, StringComparison.OrdinalIgnoreCase))
          {
            this.ChangeFeedInfo = !throwIfAlreadyExists || this.ChangeFeedInfo == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          break;
        case 28:
          if (string.Equals("x-ms-time-to-live-in-seconds", key, StringComparison.OrdinalIgnoreCase))
          {
            this.TimeToLiveInSeconds = !throwIfAlreadyExists || this.TimeToLiveInSeconds == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          break;
        case 29:
          if ((object) "x-ms-current-replica-set-size" == (object) key)
          {
            this.CurrentReplicaSetSize = !throwIfAlreadyExists || this.CurrentReplicaSetSize == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if ((object) "x-ms-documentdb-query-metrics" == (object) key)
          {
            this.QueryMetrics = !throwIfAlreadyExists || this.QueryMetrics == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if ((object) "x-ms-cosmos-index-utilization" == (object) key)
          {
            this.IndexUtilization = !throwIfAlreadyExists || this.IndexUtilization == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if ((object) "x-ms-cosmos-quorum-acked-llsn" == (object) key)
          {
            this.QuorumAckedLocalLSN = !throwIfAlreadyExists || this.QuorumAckedLocalLSN == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-current-replica-set-size", key, StringComparison.OrdinalIgnoreCase))
          {
            this.CurrentReplicaSetSize = !throwIfAlreadyExists || this.CurrentReplicaSetSize == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-documentdb-query-metrics", key, StringComparison.OrdinalIgnoreCase))
          {
            this.QueryMetrics = !throwIfAlreadyExists || this.QueryMetrics == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-cosmos-index-utilization", key, StringComparison.OrdinalIgnoreCase))
          {
            this.IndexUtilization = !throwIfAlreadyExists || this.IndexUtilization == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-cosmos-quorum-acked-llsn", key, StringComparison.OrdinalIgnoreCase))
          {
            this.QuorumAckedLocalLSN = !throwIfAlreadyExists || this.QuorumAckedLocalLSN == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          break;
        case 31:
          if (string.Equals("x-ms-request-validation-failure", key, StringComparison.OrdinalIgnoreCase))
          {
            this.RequestValidationFailure = !throwIfAlreadyExists || this.RequestValidationFailure == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          break;
        case 32:
          if ((object) "x-ms-cosmos-query-execution-info" == (object) key)
          {
            this.QueryExecutionInfo = !throwIfAlreadyExists || this.QueryExecutionInfo == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if ((object) "x-ms-aad-applied-role-assignment" == (object) key)
          {
            this.AadAppliedRoleAssignmentId = !throwIfAlreadyExists || this.AadAppliedRoleAssignmentId == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-cosmos-query-execution-info", key, StringComparison.OrdinalIgnoreCase))
          {
            this.QueryExecutionInfo = !throwIfAlreadyExists || this.QueryExecutionInfo == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-aad-applied-role-assignment", key, StringComparison.OrdinalIgnoreCase))
          {
            this.AadAppliedRoleAssignmentId = !throwIfAlreadyExists || this.AadAppliedRoleAssignmentId == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          break;
        case 33:
          if ((object) "x-ms-cosmos-replicator-glsn-delta" == (object) key)
          {
            this.ReplicatorLSNToGLSNDelta = !throwIfAlreadyExists || this.ReplicatorLSNToGLSNDelta == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if ((object) "x-ms-cosmos-replicator-llsn-delta" == (object) key)
          {
            this.ReplicatorLSNToLLSNDelta = !throwIfAlreadyExists || this.ReplicatorLSNToLLSNDelta == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if ((object) "x-ms-cosmos-correlated-activityid" == (object) key)
          {
            this.CorrelatedActivityId = !throwIfAlreadyExists || this.CorrelatedActivityId == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-cosmos-replicator-glsn-delta", key, StringComparison.OrdinalIgnoreCase))
          {
            this.ReplicatorLSNToGLSNDelta = !throwIfAlreadyExists || this.ReplicatorLSNToGLSNDelta == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-cosmos-replicator-llsn-delta", key, StringComparison.OrdinalIgnoreCase))
          {
            this.ReplicatorLSNToLLSNDelta = !throwIfAlreadyExists || this.ReplicatorLSNToLLSNDelta == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-cosmos-correlated-activityid", key, StringComparison.OrdinalIgnoreCase))
          {
            this.CorrelatedActivityId = !throwIfAlreadyExists || this.CorrelatedActivityId == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          break;
        case 34:
          if ((object) "x-ms-documentdb-script-log-results" == (object) key)
          {
            this.LogResults = !throwIfAlreadyExists || this.LogResults == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if ((object) "x-ms-cosmosdb-has-tentative-writes" == (object) key)
          {
            this.HasTentativeWrites = !throwIfAlreadyExists || this.HasTentativeWrites == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-documentdb-script-log-results", key, StringComparison.OrdinalIgnoreCase))
          {
            this.LogResults = !throwIfAlreadyExists || this.LogResults == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-cosmosdb-has-tentative-writes", key, StringComparison.OrdinalIgnoreCase))
          {
            this.HasTentativeWrites = !throwIfAlreadyExists || this.HasTentativeWrites == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          break;
        case 35:
          if ((object) "x-ms-documentdb-partitionkeyrangeid" == (object) key)
          {
            this.PartitionKeyRangeId = !throwIfAlreadyExists || this.PartitionKeyRangeId == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if ((object) "x-ms-collection-security-identifier" == (object) key)
          {
            this.CollectionSecurityIdentifier = !throwIfAlreadyExists || this.CollectionSecurityIdentifier == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-documentdb-partitionkeyrangeid", key, StringComparison.OrdinalIgnoreCase))
          {
            this.PartitionKeyRangeId = !throwIfAlreadyExists || this.PartitionKeyRangeId == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-collection-security-identifier", key, StringComparison.OrdinalIgnoreCase))
          {
            this.CollectionSecurityIdentifier = !throwIfAlreadyExists || this.CollectionSecurityIdentifier == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          break;
        case 36:
          if ((object) "x-ms-cosmos-total-account-throughput" == (object) key)
          {
            this.TotalAccountThroughput = !throwIfAlreadyExists || this.TotalAccountThroughput == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if ((object) "x-ms-cosmos-byok-encryption-progress" == (object) key)
          {
            this.ByokEncryptionProgress = !throwIfAlreadyExists || this.ByokEncryptionProgress == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-cosmos-total-account-throughput", key, StringComparison.OrdinalIgnoreCase))
          {
            this.TotalAccountThroughput = !throwIfAlreadyExists || this.TotalAccountThroughput == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-cosmos-byok-encryption-progress", key, StringComparison.OrdinalIgnoreCase))
          {
            this.ByokEncryptionProgress = !throwIfAlreadyExists || this.ByokEncryptionProgress == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          break;
        case 37:
          if ((object) "x-ms-documentdb-is-ru-per-minute-used" == (object) key)
          {
            this.IsRUPerMinuteUsed = !throwIfAlreadyExists || this.IsRUPerMinuteUsed == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if ((object) "x-ms-cosmos-is-replica-status-revoked" == (object) key)
          {
            this.ReplicaStatusRevoked = !throwIfAlreadyExists || this.ReplicaStatusRevoked == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if ((object) "x-ms-cosmos-is-merge-progress-blocked" == (object) key)
          {
            this.MergeProgressBlocked = !throwIfAlreadyExists || this.MergeProgressBlocked == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-documentdb-is-ru-per-minute-used", key, StringComparison.OrdinalIgnoreCase))
          {
            this.IsRUPerMinuteUsed = !throwIfAlreadyExists || this.IsRUPerMinuteUsed == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-cosmos-is-replica-status-revoked", key, StringComparison.OrdinalIgnoreCase))
          {
            this.ReplicaStatusRevoked = !throwIfAlreadyExists || this.ReplicaStatusRevoked == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-cosmos-is-merge-progress-blocked", key, StringComparison.OrdinalIgnoreCase))
          {
            this.MergeProgressBlocked = !throwIfAlreadyExists || this.MergeProgressBlocked == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          break;
        case 38:
          if ((object) "x-ms-cosmos-vectorclock-local-progress" == (object) key)
          {
            this.VectorClockLocalProgress = !throwIfAlreadyExists || this.VectorClockLocalProgress == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if ((object) "x-ms-cosmos-replica-confirmed-checksum" == (object) key)
          {
            this.ConfirmedStoreChecksum = !throwIfAlreadyExists || this.ConfirmedStoreChecksum == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if ((object) "x-ms-cosmos-replica-tentative-checksum" == (object) key)
          {
            this.TentativeStoreChecksum = !throwIfAlreadyExists || this.TentativeStoreChecksum == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-cosmos-vectorclock-local-progress", key, StringComparison.OrdinalIgnoreCase))
          {
            this.VectorClockLocalProgress = !throwIfAlreadyExists || this.VectorClockLocalProgress == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-cosmos-replica-confirmed-checksum", key, StringComparison.OrdinalIgnoreCase))
          {
            this.ConfirmedStoreChecksum = !throwIfAlreadyExists || this.ConfirmedStoreChecksum == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-cosmos-replica-tentative-checksum", key, StringComparison.OrdinalIgnoreCase))
          {
            this.TentativeStoreChecksum = !throwIfAlreadyExists || this.TentativeStoreChecksum == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          break;
        case 40:
          if (string.Equals("x-ms-cosmos-offer-max-allowed-throughput", key, StringComparison.OrdinalIgnoreCase))
          {
            this.SoftMaxAllowedThroughput = !throwIfAlreadyExists || this.SoftMaxAllowedThroughput == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          break;
        case 41:
          if (string.Equals("x-ms-cosmos-analytical-migration-progress", key, StringComparison.OrdinalIgnoreCase))
          {
            this.AnalyticalMigrationProgress = !throwIfAlreadyExists || this.AnalyticalMigrationProgress == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          break;
        case 42:
          if (string.Equals("x-ms-cosmos-xpconfiguration-sessions-count", key, StringComparison.OrdinalIgnoreCase))
          {
            this.XPConfigurationSessionsCount = !throwIfAlreadyExists || this.XPConfigurationSessionsCount == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          break;
        case 43:
          if (string.Equals("x-ms-cosmos-is-partition-key-delete-pending", key, StringComparison.OrdinalIgnoreCase))
          {
            this.PendingPKDelete = !throwIfAlreadyExists || this.PendingPKDelete == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          break;
        case 48:
          if (string.Equals("x-ms-cosmos-collection-unique-keys-under-reindex", key, StringComparison.OrdinalIgnoreCase))
          {
            this.CollectionUniqueKeysUnderReIndex = !throwIfAlreadyExists || this.CollectionUniqueKeysUnderReIndex == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          break;
        case 49:
          if (string.Equals("x-ms-documentdb-collection-lazy-indexing-progress", key, StringComparison.OrdinalIgnoreCase))
          {
            this.CollectionLazyIndexingProgress = !throwIfAlreadyExists || this.CollectionLazyIndexingProgress == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          break;
        case 52:
          if ((object) "x-ms-cosmos-internal-unflushed-merge-log-entry-count" == (object) key)
          {
            this.UnflushedMergLogEntryCount = !throwIfAlreadyExists || this.UnflushedMergLogEntryCount == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if ((object) "x-ms-cosmos-collection-unique-index-reindex-progress" == (object) key)
          {
            this.CollectionUniqueIndexReIndexProgress = !throwIfAlreadyExists || this.CollectionUniqueIndexReIndexProgress == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-cosmos-internal-unflushed-merge-log-entry-count", key, StringComparison.OrdinalIgnoreCase))
          {
            this.UnflushedMergLogEntryCount = !throwIfAlreadyExists || this.UnflushedMergLogEntryCount == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          if (string.Equals("x-ms-cosmos-collection-unique-index-reindex-progress", key, StringComparison.OrdinalIgnoreCase))
          {
            this.CollectionUniqueIndexReIndexProgress = !throwIfAlreadyExists || this.CollectionUniqueIndexReIndexProgress == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
            return;
          }
          break;
        case 56:
          if (string.Equals("x-ms-documentdb-collection-index-transformation-progress", key, StringComparison.OrdinalIgnoreCase))
          {
            this.CollectionIndexTransformationProgress = !throwIfAlreadyExists || this.CollectionIndexTransformationProgress == null ? value : throw new ArgumentException("The " + key + " already exists in the collection");
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
