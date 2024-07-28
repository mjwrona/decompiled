// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Management.ResourceManager.Models.Subscription
// Assembly: Microsoft.Azure.Management.ResourceManager, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: ABBAD935-2366-4053-A43B-1C3AE5FDB3D3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Management.ResourceManager.dll

using Newtonsoft.Json;

namespace Microsoft.Azure.Management.ResourceManager.Models
{
  public class Subscription
  {
    public Subscription()
    {
    }

    public Subscription(
      string id = null,
      string subscriptionId = null,
      string displayName = null,
      SubscriptionState? state = null,
      SubscriptionPolicies subscriptionPolicies = null,
      string authorizationSource = null)
    {
      this.Id = id;
      this.SubscriptionId = subscriptionId;
      this.DisplayName = displayName;
      this.State = state;
      this.SubscriptionPolicies = subscriptionPolicies;
      this.AuthorizationSource = authorizationSource;
    }

    [JsonProperty(PropertyName = "id")]
    public string Id { get; private set; }

    [JsonProperty(PropertyName = "subscriptionId")]
    public string SubscriptionId { get; private set; }

    [JsonProperty(PropertyName = "displayName")]
    public string DisplayName { get; private set; }

    [JsonProperty(PropertyName = "state")]
    public SubscriptionState? State { get; private set; }

    [JsonProperty(PropertyName = "subscriptionPolicies")]
    public SubscriptionPolicies SubscriptionPolicies { get; set; }

    [JsonProperty(PropertyName = "authorizationSource")]
    public string AuthorizationSource { get; set; }
  }
}
