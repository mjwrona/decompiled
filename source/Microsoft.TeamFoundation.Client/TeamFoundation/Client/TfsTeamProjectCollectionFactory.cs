// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.TfsTeamProjectCollectionFactory
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.TeamFoundation.Framework.Client;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net;

namespace Microsoft.TeamFoundation.Client
{
  public static class TfsTeamProjectCollectionFactory
  {
    private static Dictionary<Uri, TfsTeamProjectCollection> s_serverCache = new Dictionary<Uri, TfsTeamProjectCollection>(UriUtility.AbsoluteUriStringComparer);

    public static ReadOnlyCollection<TfsTeamProjectCollection> Collections
    {
      get
      {
        lock (TfsTeamProjectCollectionFactory.s_serverCache)
          return TfsTeamProjectCollectionFactory.s_serverCache.Values.ToList<TfsTeamProjectCollection>().AsReadOnly();
      }
    }

    public static TfsTeamProjectCollection GetTeamProjectCollection(Uri uri) => TfsTeamProjectCollectionFactory.GetTeamProjectCollection((string) null, uri);

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static TfsTeamProjectCollection GetTeamProjectCollection(
      Uri uri,
      VssCredentials credentials)
    {
      return TfsTeamProjectCollectionFactory.GetTeamProjectCollection((string) null, uri, credentials);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static TfsTeamProjectCollection GetTeamProjectCollection(
      string featureRegistryKeyword,
      Uri uri)
    {
      return TfsTeamProjectCollectionFactory.GetTeamProjectCollection(featureRegistryKeyword, uri, (VssCredentials) null);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static TfsTeamProjectCollection GetTeamProjectCollection(
      string featureRegistryKeyword,
      Uri uri,
      VssCredentials credentials)
    {
      ArgumentUtility.CheckForNull<Uri>(uri, nameof (uri));
      string str = UriUtility.GetInvariantAbsoluteUri(uri);
      if (VssStringComparer.ServerUrl.EndsWith(str, LocationServiceConstants.CollectionLocationServiceRelativePath))
        str = str.Remove(str.Length - LocationServiceConstants.CollectionLocationServiceRelativePath.Length);
      uri = new Uri(str);
      TfsTeamProjectCollection projectCollection = (TfsTeamProjectCollection) null;
      bool flag1 = credentials != null;
      bool flag2 = flag1 && (credentials.Federated != null || !credentials.Windows.UseDefaultCredentials);
      lock (TfsTeamProjectCollectionFactory.s_serverCache)
      {
        foreach (Uri possibleCollectionUri in (IEnumerable<Uri>) TfsTeamProjectCollectionFactory.EnumeratePossibleCollectionUris(uri))
        {
          if (TfsTeamProjectCollectionFactory.s_serverCache.ContainsKey(possibleCollectionUri))
          {
            uri = possibleCollectionUri;
            projectCollection = TfsTeamProjectCollectionFactory.s_serverCache[possibleCollectionUri];
            break;
          }
        }
        credentials = credentials ?? TfsClientCredentialsCache.GetCredentials(featureRegistryKeyword, uri);
        if (flag2)
          TfsClientCredentialsCache.SetCredentials(uri, credentials);
        if (projectCollection == null || projectCollection.Disposed || !flag1 && credentials != null && projectCollection.ClientCredentials != credentials)
        {
          TeamFoundationTrace.Verbose(TraceKeywordSets.Authentication, "Creating a new TfsTeamProjectCollection for '{0}' {1} credentials {2}", (object) uri.ToString(), credentials != null ? (object) "with" : (object) "without", flag1 ? (object) "provided" : (object) "from the cache.");
          projectCollection = new TfsTeamProjectCollection(uri, credentials, (IdentityDescriptor) null, true);
          TfsTeamProjectCollectionFactory.s_serverCache[uri] = projectCollection;
        }
        else if (((projectCollection.HasAuthenticated ? 0 : (projectCollection.ClientCredentials != credentials ? 1 : 0)) & (flag2 ? 1 : 0)) != 0)
        {
          TeamFoundationTrace.Verbose(TraceKeywordSets.Authentication, "Setting provided credentials on a TfsTeamProjectCollection already found in the server instance cache.");
          try
          {
            projectCollection.ClientCredentials = credentials;
          }
          catch (InvalidOperationException ex)
          {
            TeamFoundationTrace.TraceException(TraceKeywordSets.Authentication, "Attempted to set credentials on an unauthenticated TfsTeamProjectCollection but failed.", (Exception) ex);
          }
        }
      }
      return projectCollection;
    }

    public static TfsTeamProjectCollection GetTeamProjectCollection(
      RegisteredProjectCollection projectCollection)
    {
      return TfsTeamProjectCollectionFactory.GetTeamProjectCollection(projectCollection.Uri);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static TfsTeamProjectCollection GetTeamProjectCollection(
      string serverName,
      bool ensureAuthenticated,
      bool ensureServerIsLocallyRegistered)
    {
      return TfsTeamProjectCollectionFactory.GetTeamProjectCollection(serverName, ensureAuthenticated, ensureServerIsLocallyRegistered, (VssCredentials) null);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static TfsTeamProjectCollection GetTeamProjectCollection(
      string serverName,
      bool ensureAuthenticated,
      bool ensureServerIsLocallyRegistered,
      VssCredentials credentials)
    {
      ArgumentUtility.CheckForNull<string>(serverName, nameof (serverName));
      TfsTeamProjectCollection projectCollection = TfsTeamProjectCollectionFactory.GetTeamProjectCollection(TfsTeamProjectCollection.GetFullyQualifiedUriForName(serverName), credentials);
      try
      {
        if (ensureAuthenticated)
          projectCollection.EnsureAuthenticated();
      }
      catch (WebException ex)
      {
        throw new TeamFoundationServiceUnavailableException(TFCommonResources.ServicesUnavailable((object) projectCollection.Name, (object) ex.Message), (Exception) ex);
      }
      if (ensureServerIsLocallyRegistered && projectCollection != null)
        RegisteredTfsConnections.RegisterProjectCollection(projectCollection);
      return projectCollection;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void ReplaceTeamProjectCollection(TfsTeamProjectCollection projectCollection)
    {
      ArgumentUtility.CheckForNull<TfsTeamProjectCollection>(projectCollection, nameof (projectCollection));
      lock (TfsTeamProjectCollectionFactory.s_serverCache)
      {
        projectCollection.UseFactory = true;
        TfsTeamProjectCollectionFactory.s_serverCache[projectCollection.Uri] = projectCollection;
        TfsClientCredentialsCache.SetCredentials(projectCollection.Uri, projectCollection.ClientCredentials);
      }
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void RemoveTeamProjectCollection(TfsTeamProjectCollection projectCollection)
    {
      ArgumentUtility.CheckForNull<TfsTeamProjectCollection>(projectCollection, nameof (projectCollection));
      lock (TfsTeamProjectCollectionFactory.s_serverCache)
      {
        TfsTeamProjectCollection projectCollection1;
        if (!TfsTeamProjectCollectionFactory.s_serverCache.TryGetValue(projectCollection.Uri, out projectCollection1) || projectCollection1 != projectCollection)
          return;
        TfsTeamProjectCollectionFactory.s_serverCache.Remove(projectCollection.Uri);
        TfsClientCredentialsCache.RemoveCredentials(projectCollection.Uri, projectCollection.ClientCredentials);
      }
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    private static IList<Uri> EnumeratePossibleCollectionUris(Uri tpcUri)
    {
      if (TFUtil.IsHostedServer(tpcUri))
      {
        string str = tpcUri.LocalPath.Trim('/');
        if (str.Equals(string.Empty, StringComparison.OrdinalIgnoreCase))
        {
          Uri uri = new UriBuilder(tpcUri)
          {
            Path = "defaultcollection"
          }.Uri;
          return (IList<Uri>) new List<Uri>()
          {
            tpcUri,
            uri
          };
        }
        if (str.Equals("defaultcollection", StringComparison.OrdinalIgnoreCase))
        {
          Uri uri = new UriBuilder(tpcUri)
          {
            Path = string.Empty
          }.Uri;
          return (IList<Uri>) new List<Uri>()
          {
            tpcUri,
            uri
          };
        }
      }
      return (IList<Uri>) new List<Uri>() { tpcUri };
    }
  }
}
