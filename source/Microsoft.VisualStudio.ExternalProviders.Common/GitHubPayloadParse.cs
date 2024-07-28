// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.ExternalProviders.Common.GitHubPayloadParse
// Assembly: Microsoft.VisualStudio.ExternalProviders.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7E34B318-B0E9-49BD-88C0-4A425E8D0753
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.ExternalProviders.Common.dll

using Newtonsoft.Json.Linq;

namespace Microsoft.VisualStudio.ExternalProviders.Common
{
  public static class GitHubPayloadParse
  {
    public static Microsoft.VisualStudio.ExternalProviders.GitHub.WebApi.Models.Event.GitHubAppInstallationEvent GitHubAppInstallationEvent(
      JObject payload)
    {
      return payload.ToObject<Microsoft.VisualStudio.ExternalProviders.GitHub.WebApi.Models.Event.GitHubAppInstallationEvent>();
    }

    public static Microsoft.VisualStudio.ExternalProviders.GitHub.WebApi.Models.Event.GitHubAppInstallationRepositoriesEvent GitHubAppInstallationRepositoriesEvent(
      JObject payload)
    {
      return payload.ToObject<Microsoft.VisualStudio.ExternalProviders.GitHub.WebApi.Models.Event.GitHubAppInstallationRepositoriesEvent>();
    }

    public static Microsoft.VisualStudio.ExternalProviders.GitHub.WebApi.Models.Event.GitHubAppMarketPurchaseEvent GitHubAppMarketPurchaseEvent(
      JObject payload)
    {
      return payload.ToObject<Microsoft.VisualStudio.ExternalProviders.GitHub.WebApi.Models.Event.GitHubAppMarketPurchaseEvent>();
    }

    public static Microsoft.VisualStudio.ExternalProviders.GitHub.WebApi.Models.Event.GitHubRepositoryEvent GitHubRepositoryEvent(
      JObject payload)
    {
      return payload.ToObject<Microsoft.VisualStudio.ExternalProviders.GitHub.WebApi.Models.Event.GitHubRepositoryEvent>();
    }
  }
}
