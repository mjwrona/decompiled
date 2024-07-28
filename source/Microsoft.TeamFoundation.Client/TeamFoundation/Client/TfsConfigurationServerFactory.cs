// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.TfsConfigurationServerFactory
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Client.Internal;
using Microsoft.TeamFoundation.Framework.Client;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace Microsoft.TeamFoundation.Client
{
  public static class TfsConfigurationServerFactory
  {
    private static Dictionary<Uri, TfsConfigurationServer> s_serverCache = new Dictionary<Uri, TfsConfigurationServer>(UriUtility.AbsoluteUriStringComparer);

    static TfsConfigurationServerFactory()
    {
      if (!Environment.UserInteractive)
        return;
      NotificationManager.AddNotificationHandler(new NotificationManager.NotificationHandler(TfsConfigurationServerFactory.CrossProcessNotificationCallback), Notification.TfsConnectionNotificationBegin, Notification.TfsConnectionNotificationEnd);
    }

    public static ReadOnlyCollection<TfsConfigurationServer> Servers
    {
      get
      {
        lock (TfsConfigurationServerFactory.s_serverCache)
          return TfsConfigurationServerFactory.s_serverCache.Values.ToList<TfsConfigurationServer>().AsReadOnly();
      }
    }

    public static TfsConfigurationServer GetConfigurationServer(Uri uri) => TfsConfigurationServerFactory.GetConfigurationServer(uri, (VssCredentials) null);

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static TfsConfigurationServer GetConfigurationServer(Uri uri, VssCredentials credentials)
    {
      ArgumentUtility.CheckForNull<Uri>(uri, nameof (uri));
      string str = UriUtility.GetInvariantAbsoluteUri(uri);
      if (VssStringComparer.ServerUrl.EndsWith(str, LocationServiceConstants.ApplicationLocationServiceRelativePath))
        str = str.Remove(str.Length - LocationServiceConstants.ApplicationLocationServiceRelativePath.Length);
      uri = new Uri(str);
      TfsConfigurationServer configurationServer = (TfsConfigurationServer) null;
      bool flag = credentials != null;
      lock (TfsConfigurationServerFactory.s_serverCache)
      {
        credentials = credentials ?? TfsClientCredentialsCache.GetCredentials(uri);
        if (flag)
          TfsClientCredentialsCache.SetCredentials(uri, credentials);
        if (TfsConfigurationServerFactory.s_serverCache.ContainsKey(uri))
          configurationServer = TfsConfigurationServerFactory.s_serverCache[uri];
        if (configurationServer == null || configurationServer.Disposed || configurationServer.ClientCredentials != credentials && !flag && (credentials != null || configurationServer.ClientCredentials == null))
        {
          TeamFoundationTrace.Verbose(TraceKeywordSets.Authentication, "Creating a new TfsConfigurationServer for '{0}' {1} credentials {2}", (object) uri.ToString(), credentials != null ? (object) "with" : (object) "without", flag ? (object) "provided" : (object) "from the cache.");
          configurationServer = new TfsConfigurationServer(uri, credentials, (IdentityDescriptor) null, true);
          TfsConfigurationServerFactory.s_serverCache[uri] = configurationServer;
        }
        else if (flag)
        {
          if (configurationServer.ClientCredentials != credentials)
          {
            if (!configurationServer.HasAuthenticated)
            {
              TeamFoundationTrace.Verbose(TraceKeywordSets.Authentication, "Setting provided credentials on a TfsConfigurationServer already found in the server instance cache.");
              try
              {
                configurationServer.ClientCredentials = credentials;
              }
              catch (InvalidOperationException ex)
              {
                TeamFoundationTrace.TraceException(TraceKeywordSets.Authentication, "Attempted to set credentials on an unauthenticated TfsConfigurationServer but failed.", (Exception) ex);
              }
            }
          }
        }
      }
      return configurationServer;
    }

    public static TfsConfigurationServer GetConfigurationServer(
      RegisteredConfigurationServer application)
    {
      ArgumentUtility.CheckForNull<RegisteredConfigurationServer>(application, nameof (application));
      return TfsConfigurationServerFactory.GetConfigurationServer(application.Uri);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void ReplaceConfigurationServer(TfsConfigurationServer configurationServer)
    {
      ArgumentUtility.CheckForNull<TfsConfigurationServer>(configurationServer, "newConfigurationServer");
      lock (TfsConfigurationServerFactory.s_serverCache)
      {
        configurationServer.UseFactory = true;
        TfsConfigurationServerFactory.s_serverCache[configurationServer.Uri] = configurationServer;
        TfsClientCredentialsCache.SetCredentials(configurationServer.Uri, configurationServer.ClientCredentials);
      }
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void RemoveConfigurationServer(TfsConfigurationServer configurationServer)
    {
      ArgumentUtility.CheckForNull<TfsConfigurationServer>(configurationServer, nameof (configurationServer));
      lock (TfsConfigurationServerFactory.s_serverCache)
      {
        TfsConfigurationServer configurationServer1;
        if (!TfsConfigurationServerFactory.s_serverCache.TryGetValue(configurationServer.Uri, out configurationServer1) || configurationServer1 != configurationServer)
          return;
        TfsConfigurationServerFactory.s_serverCache.Remove(configurationServer.Uri);
        TfsClientCredentialsCache.RemoveCredentials(configurationServer.Uri, configurationServer.ClientCredentials);
      }
    }

    private static void CrossProcessNotificationCallback(
      Notification notification,
      IntPtr param1,
      IntPtr param2)
    {
      if (notification != Notification.TfsConnectionUserChanged || ((int) param1.ToInt64() & 5) != 5)
        return;
      TfsConfigurationServer configurationServer1 = (TfsConfigurationServer) null;
      int int64 = (int) param2.ToInt64();
      lock (TfsConfigurationServerFactory.s_serverCache)
      {
        foreach (TfsConfigurationServer configurationServer2 in TfsConfigurationServerFactory.s_serverCache.Values)
        {
          int? instanceIdHashCode = configurationServer2.InstanceIdHashCode;
          if (instanceIdHashCode.HasValue)
          {
            instanceIdHashCode = configurationServer2.InstanceIdHashCode;
            if (instanceIdHashCode.Value == int64)
            {
              configurationServer1 = configurationServer2;
              break;
            }
          }
        }
      }
      if (configurationServer1 != null)
      {
        int num = (int) UIHost.ShowWarning(ClientResources.ConnectionUserChangedWarning((object) configurationServer1.Uri));
      }
      TfsConfigurationServerManager.OnConnectionUserChanged(configurationServer1 != null ? configurationServer1.InstanceId : Guid.Empty);
    }
  }
}
