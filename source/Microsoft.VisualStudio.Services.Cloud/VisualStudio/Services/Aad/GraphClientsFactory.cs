// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Aad.GraphClientsFactory
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.VisualStudio.Services.Aad.Graph;
using Microsoft.VisualStudio.Services.Cloud.AzureActiveDirectory.Graph;

namespace Microsoft.VisualStudio.Services.Aad
{
  public class GraphClientsFactory
  {
    private readonly GraphServiceClientFactory GraphServiceClientFactory;

    public GraphClientsFactory() => this.GraphServiceClientFactory = new GraphServiceClientFactory();

    public virtual IAadGraphClient CreateGraphClient(
      string graphApiVersion = null,
      string graphApiDomainName = null)
    {
      return (IAadGraphClient) new AadGraphClient(new GraphConnectionFactory(graphApiVersion, graphApiDomainName));
    }

    public virtual IMicrosoftGraphClient CreateMicrosoftGraphClient(
      string microsoftGraphRootUrlOverride = null,
      string microsoftGraphVersionOverride = null)
    {
      if (string.IsNullOrEmpty(microsoftGraphRootUrlOverride) && string.IsNullOrEmpty(microsoftGraphVersionOverride))
        return (IMicrosoftGraphClient) new MicrosoftGraphClient((IGraphServiceClientFactory) this.GraphServiceClientFactory);
      return (IMicrosoftGraphClient) new MicrosoftGraphClient((IGraphServiceClientFactory) new GraphServiceClientFactory()
      {
        BaseUrlOverride = microsoftGraphRootUrlOverride,
        VersionOverride = microsoftGraphVersionOverride
      });
    }
  }
}
