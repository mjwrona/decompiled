// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.ExternalEvents.BoardsGitHubAppHostIdMappingProviderData
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.ExternalIntegration.HostIdMapping;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.ExternalEvents
{
  public class BoardsGitHubAppHostIdMappingProviderData : IHostIdMappingProviderData
  {
    public static readonly BoardsGitHubAppHostIdMappingProviderData Instance = new BoardsGitHubAppHostIdMappingProviderData();

    public string ProviderId => "github-boards";

    public IReadOnlyList<IHostIdMappingRouter> Routers => (IReadOnlyList<IHostIdMappingRouter>) new List<IHostIdMappingRouter>()
    {
      (IHostIdMappingRouter) new BoardsGitHubAppInstallationIdRouter()
    };

    public string DeliveryIdHeaderName => "X-GitHub-Delivery";

    public IReadOnlyList<string> SensitiveHeaderNames => (IReadOnlyList<string>) new List<string>()
    {
      "X-Hub-Signature"
    };

    public IReadOnlyList<string> AllowedHeaders => (IReadOnlyList<string>) null;

    private BoardsGitHubAppHostIdMappingProviderData()
    {
    }
  }
}
