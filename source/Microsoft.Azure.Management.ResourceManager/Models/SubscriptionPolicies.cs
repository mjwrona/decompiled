// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Management.ResourceManager.Models.SubscriptionPolicies
// Assembly: Microsoft.Azure.Management.ResourceManager, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: ABBAD935-2366-4053-A43B-1C3AE5FDB3D3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Management.ResourceManager.dll

using Newtonsoft.Json;

namespace Microsoft.Azure.Management.ResourceManager.Models
{
  public class SubscriptionPolicies
  {
    public SubscriptionPolicies()
    {
    }

    public SubscriptionPolicies(
      string locationPlacementId = null,
      string quotaId = null,
      Microsoft.Azure.Management.ResourceManager.Models.SpendingLimit? spendingLimit = null)
    {
      this.LocationPlacementId = locationPlacementId;
      this.QuotaId = quotaId;
      this.SpendingLimit = spendingLimit;
    }

    [JsonProperty(PropertyName = "locationPlacementId")]
    public string LocationPlacementId { get; private set; }

    [JsonProperty(PropertyName = "quotaId")]
    public string QuotaId { get; private set; }

    [JsonProperty(PropertyName = "spendingLimit")]
    public Microsoft.Azure.Management.ResourceManager.Models.SpendingLimit? SpendingLimit { get; private set; }
  }
}
