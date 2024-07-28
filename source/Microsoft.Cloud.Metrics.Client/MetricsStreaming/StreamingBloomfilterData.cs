// Decompiled with JetBrains decompiler
// Type: Microsoft.Cloud.Metrics.Client.MetricsStreaming.StreamingBloomfilterData
// Assembly: Microsoft.Cloud.Metrics.Client, Version=2.2023.705.2051, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 06B39E1C-7DF0-4BC1-AFBA-9AD635E73CB0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Cloud.Metrics.Client.dll

using Newtonsoft.Json;
using System;

namespace Microsoft.Cloud.Metrics.Client.MetricsStreaming
{
  public sealed class StreamingBloomfilterData
  {
    [JsonProperty("bloomfilterDataVersion")]
    public int BloomfilterDataVersion { get; set; }

    [JsonProperty("estimatedCount")]
    public int EstimatedCount { get; set; }

    [JsonProperty("hashFuncCount")]
    public int HashFuncCount { get; set; }

    [JsonProperty("bitCapacity")]
    public int BitCapacity { get; set; }

    [JsonProperty("dimensionName")]
    public string DimensionName { get; set; }

    [JsonProperty("keyCount")]
    public int KeyCount { get; set; }

    [JsonProperty("expectedFalsePositiveRate")]
    public double ExpectedFalsePositiveRate { get; set; }

    [JsonProperty("lastUpdateTimeUtc")]
    public DateTime LastUpdateTimeUtc { get; set; }

    [JsonProperty("data")]
    public byte[] Data { get; set; }
  }
}
