// Decompiled with JetBrains decompiler
// Type: Microsoft.Cloud.Metrics.Client.MetricsStreaming.StreamingBloomfilterRequest
// Assembly: Microsoft.Cloud.Metrics.Client, Version=2.2023.705.2051, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 06B39E1C-7DF0-4BC1-AFBA-9AD635E73CB0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Cloud.Metrics.Client.dll

using Newtonsoft.Json;

namespace Microsoft.Cloud.Metrics.Client.MetricsStreaming
{
  public sealed class StreamingBloomfilterRequest
  {
    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("namespace")]
    public string Namespace { get; set; }

    [JsonProperty("serviceTreeId")]
    public string ServiceTreeId { get; set; }

    [JsonProperty(PropertyName = "contactEmail")]
    public string ContactEmail { get; set; }

    [JsonProperty("bloomfilterData")]
    public StreamingBloomfilterData BloomfilterData { get; set; }

    [JsonProperty("etag")]
    public string Etag { get; set; }
  }
}
