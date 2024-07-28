// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.TfsConfigurationServerManager
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Client.Internal;
using Microsoft.TeamFoundation.Framework.Client;
using Microsoft.VisualStudio.Services.Client;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Client
{
  public static class TfsConfigurationServerManager
  {
    public static void Update(TfsConfigurationServer configurationServer)
    {
      ArgumentUtility.CheckForNull<TfsConfigurationServer>(configurationServer, nameof (configurationServer));
      configurationServer.EnsureAuthenticated();
      Microsoft.VisualStudio.Services.Common.IssuedTokenProvider provider;
      if (configurationServer.ClientCredentials.TryGetTokenProvider(configurationServer.Uri, out provider) && provider != null && provider.CurrentToken != null && provider.CurrentToken.IsAuthenticated && provider.CurrentToken.CredentialType == VssCredentialsType.Federated)
      {
        VssClientCredentialStorage credentialStorage = new VssClientCredentialStorage();
        credentialStorage.StoreToken(configurationServer.Uri, provider.CurrentToken);
        if (provider.Credential != null && provider.Credential.Storage == null)
          provider.Credential.Storage = (IVssCredentialStorage) credentialStorage;
      }
      else
      {
        VssClientCredentialStorage credentialStorage = new VssClientCredentialStorage();
        Guid result;
        Guid.TryParse(credentialStorage.GetTokenProperty(configurationServer.Uri, "UserId") ?? string.Empty, out result);
        if (result != Guid.Empty)
        {
          TeamFoundationIdentity identity;
          configurationServer.GetAuthenticatedIdentity(out identity);
          if (identity.TeamFoundationId != result)
            throw new TeamFoundationInvalidAuthenticationException(Microsoft.TeamFoundation.Client.Internal.ClientResources.CannotAuthenticateAsAnotherUser((object) credentialStorage.GetTokenProperty(configurationServer.Uri, "UserName"), (object) identity.GetAttribute("Account", string.Empty)), TeamFoundationAuthenticationError.UserMismatched);
        }
      }
      TfsConfigurationServerFactory.ReplaceConfigurationServer(configurationServer);
      RegisteredTfsConnections.RegisterConfigurationServer(configurationServer);
    }

    public static void Remove(TfsConfigurationServer configurationServer)
    {
      ArgumentUtility.CheckForNull<TfsConfigurationServer>(configurationServer, nameof (configurationServer));
      List<RegisteredProjectCollection> projectCollections = RegisteredTfsConnections.GetProjectCollections(configurationServer.Name);
      if (projectCollections != null)
      {
        foreach (RegisteredProjectCollection projectCollection in projectCollections)
        {
          if (projectCollection != null)
          {
            TfsClientCacheUtility.DeleteVolatileCacheDirectory(projectCollection.Uri, projectCollection.InstanceId);
            RegisteredTfsConnections.UnregisterProjectCollection(projectCollection.Name);
          }
        }
      }
      RegisteredConfigurationServer configurationServer1 = RegisteredTfsConnections.GetConfigurationServer(configurationServer.Name);
      if (configurationServer1 != null)
      {
        TfsClientCacheUtility.DeleteVolatileCacheDirectory(configurationServer1.Uri, configurationServer1.InstanceId);
        RegisteredTfsConnections.UnregisterConfigurationServer(configurationServer1.Name);
      }
      new VssClientCredentialStorage().RemoveToken(configurationServer.Uri);
      TfsConfigurationServerFactory.RemoveConfigurationServer(configurationServer);
      foreach (TfsTeamProjectCollection projectCollection in configurationServer.GetTeamProjectCollections())
        TfsTeamProjectCollectionFactory.RemoveTeamProjectCollection(projectCollection);
    }

    public static TfsConfigurationServer SwitchUser(Uri uri)
    {
      ArgumentUtility.CheckForNull<Uri>(uri, nameof (uri));
      TfsConfigurationServer serverForSwitchUser = TfsConfigurationServerManager.GetServerForSwitchUser(uri);
      serverForSwitchUser.Authenticate();
      TfsConfigurationServerManager.SwitchUser(serverForSwitchUser);
      return serverForSwitchUser;
    }

    public static void SwitchUser(TfsConfigurationServer configurationServer)
    {
      ArgumentUtility.CheckForNull<TfsConfigurationServer>(configurationServer, nameof (configurationServer));
      configurationServer.EnsureAuthenticated();
      if (!configurationServer.IsHostedServer)
        throw new InvalidOperationException(Microsoft.TeamFoundation.Client.Internal.ClientResources.SwitchUserNotSupportedOnPremise());
      TfsClientCacheUtility.DeleteVolatileCacheDirectory(configurationServer.Uri, configurationServer.InstanceId);
      foreach (RegisteredProjectCollection projectCollection in RegisteredTfsConnections.GetProjectCollections(configurationServer.Name))
        TfsClientCacheUtility.DeleteVolatileCacheDirectory(projectCollection.Uri, projectCollection.InstanceId);
      Microsoft.VisualStudio.Services.Common.IssuedTokenProvider provider;
      if (!configurationServer.ClientCredentials.TryGetTokenProvider(configurationServer.Uri, out provider) || provider == null || provider.CurrentToken == null || !provider.CurrentToken.IsAuthenticated || provider.CurrentToken.CredentialType != VssCredentialsType.Federated)
        throw new InvalidOperationException(Microsoft.TeamFoundation.Client.Internal.ClientResources.SwitchUserRequiresFederatedAuthentication());
      VssClientCredentialStorage credentialStorage = new VssClientCredentialStorage();
      credentialStorage.StoreToken(configurationServer.Uri, provider.CurrentToken);
      if (provider.Credential != null && provider.Credential.Storage == null)
        provider.Credential.Storage = (IVssCredentialStorage) credentialStorage;
      TfsConfigurationServerFactory.ReplaceConfigurationServer(configurationServer);
      RegisteredTfsConnections.RegisterConfigurationServer(configurationServer);
      int? instanceIdHashCode = configurationServer.InstanceIdHashCode;
      if (instanceIdHashCode.HasValue)
      {
        IntPtr wParam = new IntPtr(5);
        instanceIdHashCode = configurationServer.InstanceIdHashCode;
        IntPtr lParam = new IntPtr(instanceIdHashCode.Value);
        NotificationManager.EnqueueNotification(Notification.TfsConnectionUserChanged, wParam, lParam);
      }
      TfsConfigurationServerManager.OnConnectionUserChanged(configurationServer.InstanceId);
    }

    internal static void OnConnectionUserChanged(Guid instanceId)
    {
      EventHandler<ConnectionUserChangedEventArgs> connectionUserChanged = TfsConfigurationServerManager.ConnectionUserChanged;
      if (connectionUserChanged == null)
        return;
      ConnectionUserChangedEventArgs e = new ConnectionUserChangedEventArgs(instanceId);
      connectionUserChanged((object) null, e);
    }

    internal static TfsConfigurationServer GetServerForSwitchUser(
      Uri uri,
      IdentityDescriptor identityToImpersonate = null)
    {
      VssClientCredentials credentials = new VssClientCredentials();
      credentials.Federated.Storage = (IVssCredentialStorage) null;
      return new TfsConfigurationServer(uri, (VssCredentials) credentials, identityToImpersonate, false);
    }

    public static event EventHandler<ConnectionUserChangedEventArgs> ConnectionUserChanged;
  }
}
