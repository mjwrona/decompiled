// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.ChangeFeed.LeaseManagement.DocumentServiceLeaseCore
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.Azure.Cosmos.ChangeFeed.LeaseManagement
{
  [Serializable]
  internal sealed class DocumentServiceLeaseCore : DocumentServiceLease
  {
    private static readonly DateTime UnixStartTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
    private bool isMigratingFromV2;

    [JsonProperty("id")]
    public string LeaseId { get; set; }

    [JsonProperty("partitionKey", NullValueHandling = NullValueHandling.Ignore)]
    public string LeasePartitionKey { get; set; }

    [JsonIgnore]
    public override string PartitionKey => this.LeasePartitionKey;

    [JsonProperty("version")]
    public DocumentServiceLeaseVersion Version => DocumentServiceLeaseVersion.PartitionKeyRangeBasedLease;

    [JsonIgnore]
    public override string Id => this.LeaseId;

    [JsonProperty("_etag")]
    public string ETag { get; set; }

    [JsonProperty("LeaseToken")]
    public string LeaseToken { get; set; }

    [JsonProperty("PartitionId", NullValueHandling = NullValueHandling.Ignore)]
    private string PartitionId
    {
      get => this.isMigratingFromV2 ? this.LeaseToken : (string) null;
      set
      {
        this.LeaseToken = value;
        this.isMigratingFromV2 = true;
      }
    }

    [JsonIgnore]
    public override string CurrentLeaseToken => this.LeaseToken;

    [JsonProperty("FeedRange", NullValueHandling = NullValueHandling.Ignore)]
    public override FeedRangeInternal FeedRange { get; set; }

    [JsonProperty("Owner")]
    public override string Owner { get; set; }

    [JsonProperty("ContinuationToken")]
    public override string ContinuationToken { get; set; }

    [JsonIgnore]
    public override DateTime Timestamp
    {
      get => this.ExplicitTimestamp ?? DocumentServiceLeaseCore.UnixStartTime.AddSeconds((double) this.TS);
      set => this.ExplicitTimestamp = new DateTime?(value);
    }

    [JsonIgnore]
    public override string ConcurrencyToken => this.ETag;

    [JsonProperty("properties")]
    public override Dictionary<string, string> Properties { get; set; } = new Dictionary<string, string>();

    [JsonProperty("timestamp")]
    private DateTime? ExplicitTimestamp { get; set; }

    [JsonProperty("_ts")]
    private long TS { get; set; }

    public override string ToString()
    {
      CultureInfo invariantCulture = CultureInfo.InvariantCulture;
      object[] objArray = new object[5]
      {
        (object) this.Id,
        (object) this.Owner,
        (object) this.ContinuationToken,
        (object) this.Timestamp.ToUniversalTime(),
        null
      };
      DateTime dateTime = DocumentServiceLeaseCore.UnixStartTime;
      dateTime = dateTime.AddSeconds((double) this.TS);
      objArray[4] = (object) dateTime.ToUniversalTime();
      return string.Format((IFormatProvider) invariantCulture, "{0} Owner='{1}' Continuation={2} Timestamp(local)={3} Timestamp(server)={4}", objArray);
    }
  }
}
