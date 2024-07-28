// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.ExternalEvents.BoardsGitHubAppEventUrlResolver
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.ExternalIntegration.HostIdMapping;
using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.ExternalEvents
{
  [ExtensionPriority(50)]
  [ExtensionStrategy("Hosted")]
  internal class BoardsGitHubAppEventUrlResolver : ExternalServiceEventUrlResolver
  {
    protected override string Layer => nameof (BoardsGitHubAppEventUrlResolver);

    public override string Name => "Boards";

    protected override string AccessMappingName => "X-Boards";

    protected override bool UseExactMatch => true;

    protected override IHostIdMappingProviderData GetProviderData(IVssRequestContext requestContext) => (IHostIdMappingProviderData) BoardsGitHubAppHostIdMappingProviderData.Instance;
  }
}
