// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.PartitionKeyStatistics
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Microsoft.Azure.Documents.Routing;
using Newtonsoft.Json;

namespace Microsoft.Azure.Documents
{
  [JsonObject(MemberSerialization.OptIn)]
  public sealed class PartitionKeyStatistics
  {
    public PartitionKey PartitionKey => PartitionKey.FromInternalKey(this.PartitionKeyInternal);

    [JsonProperty(PropertyName = "sizeInKB")]
    public long SizeInKB { get; private set; }

    public override string ToString() => JsonConvert.SerializeObject((object) this);

    [JsonProperty(PropertyName = "partitionKey")]
    internal PartitionKeyInternal PartitionKeyInternal { get; private set; }
  }
}
