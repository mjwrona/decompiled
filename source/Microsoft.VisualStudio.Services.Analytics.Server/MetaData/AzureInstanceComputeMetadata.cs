// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.MetaData.AzureInstanceComputeMetadata
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Newtonsoft.Json;

namespace Microsoft.VisualStudio.Services.Analytics.MetaData
{
  [JsonObject(MemberSerialization.OptIn)]
  public class AzureInstanceComputeMetadata
  {
    [JsonProperty(PropertyName = "azEnvironment")]
    public string AzureEnvironment { get; set; }

    [JsonProperty(PropertyName = "location")]
    public string Location { get; set; }

    [JsonProperty(PropertyName = "name")]
    public string Name { get; set; }

    [JsonProperty(PropertyName = "offer")]
    public string Offer { get; set; }

    [JsonProperty(PropertyName = "osType")]
    public string OSType { get; set; }

    [JsonProperty(PropertyName = "placementGroupId")]
    public string PlacementGroupId { get; set; }

    [JsonProperty(PropertyName = "provider")]
    public string Provider { get; set; }

    [JsonProperty(PropertyName = "platformFaultDomain")]
    public string PlatformFaultDomain { get; set; }

    [JsonProperty(PropertyName = "platformUpdateDomain")]
    public string PlatformUpdateDomain { get; set; }

    [JsonProperty(PropertyName = "publisher")]
    public string Publisher { get; set; }

    [JsonProperty(PropertyName = "resourceGroupName")]
    public string ResourceGroupName { get; set; }

    [JsonProperty(PropertyName = "sku")]
    public string SKU { get; set; }

    [JsonProperty(PropertyName = "tags")]
    public string Tags { get; set; }

    [JsonProperty(PropertyName = "version")]
    public string Version { get; set; }

    [JsonProperty(PropertyName = "vmScaleSetName")]
    public string VMScaleSetName { get; set; }

    [JsonProperty(PropertyName = "vmSize")]
    public string VMSize { get; set; }

    [JsonProperty(PropertyName = "zone")]
    public string Zone { get; set; }
  }
}
