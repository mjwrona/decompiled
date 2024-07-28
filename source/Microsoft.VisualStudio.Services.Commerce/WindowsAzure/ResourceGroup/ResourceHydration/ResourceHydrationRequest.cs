// Decompiled with JetBrains decompiler
// Type: Microsoft.WindowsAzure.ResourceGroup.ResourceHydration.ResourceHydrationRequest
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.WindowsAzure.ResourceGroup.ResourceHydration
{
  [ExcludeFromCodeCoverage]
  public class ResourceHydrationRequest
  {
    [JsonProperty(Required = Required.Default)]
    public ProvisioningOperation ResourceOperation { get; set; }

    [JsonProperty(Required = Required.Default)]
    public string ResourceUri { get; set; }

    [JsonProperty(Required = Required.Default)]
    public string ResourceLocation { get; set; }

    [JsonProperty(Required = Required.Default)]
    public string ApiVersion { get; set; }

    [JsonProperty(Required = Required.Default)]
    public string CorrelationId { get; set; }
  }
}
