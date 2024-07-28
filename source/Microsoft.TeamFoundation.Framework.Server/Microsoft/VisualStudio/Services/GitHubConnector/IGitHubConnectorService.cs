// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.GitHubConnector.IGitHubConnectorService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.VisualStudio.Services.GitHubConnector
{
  [DefaultServiceImplementation(typeof (FrameworkGitHubConnectorService))]
  public interface IGitHubConnectorService : IVssFrameworkService
  {
    ConnectionInfo CreateConnection(
      IVssRequestContext collectionContext,
      ConnectionCreationContext connectionCreationContext);

    bool RemoveConnection(IVssRequestContext collectionContext, Guid connectionId);

    ConnectionInfo GetConnectionInfo(IVssRequestContext collectionContext, Guid connectionId);

    bool IsConnected(IVssRequestContext collectionContext, out Guid defaultConnectionId);

    OAuthUrl CreateUserOAuthValidationUrl(
      IVssRequestContext requestContext,
      OAuthUrlCreationContext oAuthUrlCreationContext);

    InstallationToken GetOrCreateInstallationToken(
      IVssRequestContext collectionContext,
      Guid connectionId);
  }
}
