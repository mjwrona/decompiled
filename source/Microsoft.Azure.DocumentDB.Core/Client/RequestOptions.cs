// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Client.RequestOptions
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Microsoft.Azure.Documents.Client
{
  public sealed class RequestOptions
  {
    public IList<string> PreTriggerInclude { get; set; }

    public IList<string> PostTriggerInclude { get; set; }

    public AccessCondition AccessCondition { get; set; }

    public Microsoft.Azure.Documents.IndexingDirective? IndexingDirective { get; set; }

    public Microsoft.Azure.Documents.ConsistencyLevel? ConsistencyLevel { get; set; }

    public string SessionToken { get; set; }

    public int? ResourceTokenExpirySeconds { get; set; }

    public string OfferType { get; set; }

    public int? OfferThroughput { get; set; }

    public bool OfferEnableRUPerMinuteThroughput { get; set; }

    public PartitionKey PartitionKey { get; set; }

    public bool EnableScriptLogging { get; set; }

    internal bool IsReadOnlyScript { get; set; }

    internal bool IncludeSnapshotDirectories { get; set; }

    public bool PopulateQuotaInfo { get; set; }

    public bool DisableRUPerMinuteUsage { get; set; }

    public JsonSerializerSettings JsonSerializerSettings { get; set; }

    public bool PopulatePartitionKeyRangeStatistics { get; set; }

    internal Microsoft.Azure.Documents.RemoteStorageType? RemoteStorageType { get; set; }

    internal string PartitionKeyRangeId { get; set; }

    internal string SourceDatabaseId { get; set; }

    internal string SourceCollectionId { get; set; }

    internal long? RestorePointInTime { get; set; }

    internal bool PopulateRestoreStatus { get; set; }

    internal bool? ExcludeSystemProperties { get; set; }

    internal bool InsertSystemPartitionKey { get; set; }

    internal string MergeStaticId { get; set; }

    internal bool PreserveFullContent { get; set; }

    internal bool ForceSideBySideIndexMigration { get; set; }

    [Obsolete("Deprecated")]
    public int? SharedOfferThroughput { get; set; }

    internal int? OfferAutopilotTier { get; set; }

    internal bool? OfferAutopilotAutoUpgrade { get; set; }

    internal AutopilotSettings OfferAutopilotSettings { get; set; }
  }
}
