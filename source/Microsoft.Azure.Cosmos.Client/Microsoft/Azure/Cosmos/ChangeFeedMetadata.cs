// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.ChangeFeedMetadata
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace Microsoft.Azure.Cosmos
{
  internal class ChangeFeedMetadata
  {
    public ChangeFeedMetadata(
      DateTime conflictResolutionTimestamp,
      long lsn,
      ChangeFeedOperationType operationType,
      long previousLsn)
    {
      this.ConflictResolutionTimestamp = conflictResolutionTimestamp;
      this.Lsn = lsn;
      this.OperationType = operationType;
      this.PreviousLsn = previousLsn;
    }

    [JsonProperty(PropertyName = "crts", NullValueHandling = NullValueHandling.Ignore)]
    [JsonConverter(typeof (Microsoft.Azure.Documents.UnixDateTimeConverter))]
    public DateTime ConflictResolutionTimestamp { get; }

    [JsonProperty(PropertyName = "lsn", NullValueHandling = NullValueHandling.Ignore)]
    public long Lsn { get; }

    [JsonProperty(PropertyName = "operationType")]
    [JsonConverter(typeof (StringEnumConverter))]
    public ChangeFeedOperationType OperationType { get; }

    [JsonProperty(PropertyName = "previousImageLSN", NullValueHandling = NullValueHandling.Ignore)]
    public long PreviousLsn { get; }

    [JsonProperty(PropertyName = "timeToLiveExpired", NullValueHandling = NullValueHandling.Ignore)]
    public bool IsTimeToLiveExpired { get; }
  }
}
