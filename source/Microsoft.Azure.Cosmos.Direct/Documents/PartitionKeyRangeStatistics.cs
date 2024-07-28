// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.PartitionKeyRangeStatistics
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

using Newtonsoft.Json;
using System.Collections.Generic;

namespace Microsoft.Azure.Documents
{
  internal sealed class PartitionKeyRangeStatistics
  {
    [JsonProperty(PropertyName = "id")]
    public string PartitionKeyRangeId { get; private set; }

    [JsonProperty(PropertyName = "sizeInKB")]
    public long SizeInKB { get; private set; }

    [JsonProperty(PropertyName = "documentCount")]
    public long DocumentCount { get; private set; }

    [JsonProperty(PropertyName = "sampledDistinctPartitionKeyCount")]
    internal long? SampledDistinctPartitionKeyCount { get; private set; }

    [JsonProperty(PropertyName = "partitionKeys")]
    public IReadOnlyList<Microsoft.Azure.Documents.PartitionKeyStatistics> PartitionKeyStatistics { get; private set; }

    public override string ToString() => JsonConvert.SerializeObject((object) this);
  }
}
