// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.GitHubConnector.FrameworkGitHubConnectorService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.GitHubConnector.Client;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.ComponentModel;

namespace Microsoft.VisualStudio.Services.GitHubConnector
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  internal class FrameworkGitHubConnectorService : IGitHubConnectorService, IVssFrameworkService
  {
    private Guid m_serviceHostId;
    private const string c_area = "GitHubConnector";
    private const string c_layer = "FrameworkGitHubConnectorService";

    public void ServiceStart(IVssRequestContext context)
    {
      this.m_serviceHostId = context.ServiceHost.InstanceId;
      this.ValidateRequestContext(context);
    }

    public void ServiceEnd(IVssRequestContext context)
    {
    }

    private void ValidateRequestContext(IVssRequestContext context)
    {
      context.CheckHostedDeployment();
      context.CheckServiceHostId(this.m_serviceHostId, (IVssFrameworkService) this);
    }

    public ConnectionInfo CreateConnection(
      IVssRequestContext collectionContext,
      ConnectionCreationContext connectionCreationContext)
    {
      this.ValidateRequestContext(collectionContext);
      collectionContext.CheckProjectCollectionRequestContext();
      return collectionContext.GetClient<GitHubConnectorHttpClient>().CreateConnectionAsync(connectionCreationContext).SyncResult<ConnectionInfo>();
    }

    public bool RemoveConnection(IVssRequestContext collectionContext, Guid connectionId)
    {
      this.ValidateRequestContext(collectionContext);
      collectionContext.CheckProjectCollectionRequestContext();
      return collectionContext.GetClient<GitHubConnectorHttpClient>().RemoveConnectionAsync(connectionId.ToString()).SyncResult<bool>();
    }

    public ConnectionInfo GetConnectionInfo(IVssRequestContext collectionContext, Guid connectionId)
    {
      this.ValidateRequestContext(collectionContext);
      collectionContext.CheckProjectCollectionRequestContext();
      return collectionContext.GetClient<GitHubConnectorHttpClient>().GetConnectionInfoAsync(connectionId.ToString()).SyncResult<ConnectionInfo>();
    }

    public bool IsConnected(IVssRequestContext collectionContext, out Guid defaultConnectionId)
    {
      this.ValidateRequestContext(collectionContext);
      collectionContext.CheckProjectCollectionRequestContext();
      ConnectionInfo connectionInfo = collectionContext.GetClient<GitHubConnectorHttpClient>().GetConnectionInfoAsync("default").SyncResult<ConnectionInfo>();
      if (connectionInfo == null)
      {
        defaultConnectionId = new Guid();
        return false;
      }
      defaultConnectionId = connectionInfo.ConnectionId;
      return true;
    }

    public OAuthUrl CreateUserOAuthValidationUrl(
      IVssRequestContext requestContext,
      OAuthUrlCreationContext oAuthUrlCreationContext)
    {
      this.ValidateRequestContext(requestContext);
      requestContext.CheckDeploymentRequestContext();
      return requestContext.GetClient<GitHubConnectorHttpClient>().CreateUserOAuthValidationUrlAsync(oAuthUrlCreationContext).SyncResult<OAuthUrl>();
    }

    public InstallationToken GetOrCreateInstallationToken(
      IVssRequestContext collectionContext,
      Guid connectionId)
    {
      this.ValidateRequestContext(collectionContext);
      collectionContext.CheckProjectCollectionRequestContext();
      return collectionContext.GetClient<GitHubConnectorHttpClient>().GetOrCreateInstallationTokenAsync(connectionId.ToString()).SyncResult<InstallationToken>();
    }
  }
}
