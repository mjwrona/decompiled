// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Address
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

using Newtonsoft.Json;

namespace Microsoft.Azure.Documents
{
  internal sealed class Address : Resource
  {
    [JsonProperty(PropertyName = "isPrimary")]
    public bool IsPrimary
    {
      get => this.GetValue<bool>("isPrimary");
      internal set => this.SetValue("isPrimary", (object) value);
    }

    [JsonProperty(PropertyName = "protocol")]
    public string Protocol
    {
      get => this.GetValue<string>("protocol");
      internal set => this.SetValue("protocol", (object) value);
    }

    [JsonProperty(PropertyName = "logicalUri")]
    public string LogicalUri
    {
      get => this.GetValue<string>("logicalUri");
      internal set => this.SetValue("logicalUri", (object) value);
    }

    [JsonProperty(PropertyName = "physcialUri")]
    public string PhysicalUri
    {
      get => this.GetValue<string>("physcialUri");
      internal set => this.SetValue("physcialUri", (object) value);
    }

    [JsonProperty(PropertyName = "partitionIndex")]
    public string PartitionIndex
    {
      get => this.GetValue<string>("partitionIndex");
      internal set => this.SetValue("partitionIndex", (object) value);
    }

    [JsonProperty(PropertyName = "partitionKeyRangeId")]
    public string PartitionKeyRangeId
    {
      get => this.GetValue<string>("partitionKeyRangeId");
      internal set => this.SetValue("partitionKeyRangeId", (object) value);
    }
  }
}
