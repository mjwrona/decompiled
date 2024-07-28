// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.ChangeFeed.LeaseManagement.DocumentServiceLease
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Microsoft.Azure.Cosmos.ChangeFeed.LeaseManagement
{
  [JsonConverter(typeof (DocumentServiceLeaseConverter))]
  [Serializable]
  internal abstract class DocumentServiceLease
  {
    public const string IdPropertyName = "id";
    public const string LeasePartitionKeyPropertyName = "partitionKey";

    public abstract string CurrentLeaseToken { get; }

    public abstract FeedRangeInternal FeedRange { get; set; }

    public abstract string Owner { get; set; }

    public abstract DateTime Timestamp { get; set; }

    public abstract string ContinuationToken { get; set; }

    public abstract string Id { get; }

    public abstract string PartitionKey { get; }

    public abstract string ConcurrencyToken { get; }

    public abstract Dictionary<string, string> Properties { get; set; }
  }
}
