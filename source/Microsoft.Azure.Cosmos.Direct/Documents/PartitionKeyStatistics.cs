// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.PartitionKeyStatistics
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

using Microsoft.Azure.Documents.Routing;
using Newtonsoft.Json;

namespace Microsoft.Azure.Documents
{
  [JsonObject(MemberSerialization.OptIn)]
  internal sealed class PartitionKeyStatistics
  {
    public PartitionKey PartitionKey => PartitionKey.FromInternalKey(this.PartitionKeyInternal);

    [JsonProperty(PropertyName = "sizeInKB")]
    public long SizeInKB { get; private set; }

    public override string ToString() => JsonConvert.SerializeObject((object) this);

    [JsonProperty(PropertyName = "partitionKey")]
    internal PartitionKeyInternal PartitionKeyInternal { get; private set; }
  }
}
