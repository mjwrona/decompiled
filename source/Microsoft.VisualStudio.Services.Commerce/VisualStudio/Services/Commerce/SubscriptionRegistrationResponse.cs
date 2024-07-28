// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.SubscriptionRegistrationResponse
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Newtonsoft.Json;

namespace Microsoft.VisualStudio.Services.Commerce
{
  public class SubscriptionRegistrationResponse
  {
    public string id { get; set; }

    [JsonProperty(PropertyName = "namespace")]
    public string providerNamespace { get; set; }

    public string registrationState { get; set; }

    public ResourceType[] resourceTypes { get; set; }

    public string applicationId { get; set; }
  }
}
