// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.AzureBillingQueue
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Newtonsoft.Json;
using System;

namespace Microsoft.VisualStudio.Services.Commerce
{
  public class AzureBillingQueue
  {
    [JsonProperty(Required = Required.Default, PropertyName = "partitionId")]
    public string PartitionKey { set; get; }

    [JsonProperty(Required = Required.Default, PropertyName = "batchId")]
    public Guid BatchId { set; get; }

    [JsonIgnore]
    public DateTime? LastSuccessfullyProcessedEventTime { set; get; }

    public AzureBillingQueue UpdatePartitionKeyAndBatchId(string partitionKey, Guid batchId)
    {
      this.PartitionKey = partitionKey;
      this.BatchId = batchId;
      return this;
    }

    public override string ToString() => JsonConvert.SerializeObject((object) this);
  }
}
