// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.PartitionKeyRangeStatistics
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Newtonsoft.Json;
using System.Collections.Generic;

namespace Microsoft.Azure.Documents
{
  public sealed class PartitionKeyRangeStatistics
  {
    [JsonProperty(PropertyName = "id")]
    public string PartitionKeyRangeId { get; private set; }

    [JsonProperty(PropertyName = "sizeInKB")]
    public long SizeInKB { get; private set; }

    [JsonProperty(PropertyName = "documentCount")]
    public long DocumentCount { get; private set; }

    [JsonProperty(PropertyName = "partitionKeys")]
    public IReadOnlyList<Microsoft.Azure.Documents.PartitionKeyStatistics> PartitionKeyStatistics { get; private set; }

    public override string ToString() => JsonConvert.SerializeObject((object) this);
  }
}
