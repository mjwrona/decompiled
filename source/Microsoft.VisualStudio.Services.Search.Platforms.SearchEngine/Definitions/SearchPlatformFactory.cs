// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.SearchPlatformFactory
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions
{
  public sealed class SearchPlatformFactory : ISearchPlatformFactory
  {
    private static readonly object s_platformLock = new object();
    private static readonly object s_clusterStateServiceLock = new object();
    private static readonly object s_backupPlatformLock = new object();
    [StaticSafe]
    private static readonly IDictionary<SearchPlatformFactory.SearchPlatformId, ISearchPlatform> s_platformMap = (IDictionary<SearchPlatformFactory.SearchPlatformId, ISearchPlatform>) new FriendlyDictionary<SearchPlatformFactory.SearchPlatformId, ISearchPlatform>();
    [StaticSafe]
    private static readonly IDictionary<SearchPlatformFactory.SearchPlatformId, ISearchClusterManagementService> s_searchClusterManagementServicesMap = (IDictionary<SearchPlatformFactory.SearchPlatformId, ISearchClusterManagementService>) new FriendlyDictionary<SearchPlatformFactory.SearchPlatformId, ISearchClusterManagementService>();
    [StaticSafe]
    private static readonly IDictionary<SearchPlatformFactory.SearchPlatformId, ISearchBackupPlatform> s_backupPlatformMap = (IDictionary<SearchPlatformFactory.SearchPlatformId, ISearchBackupPlatform>) new FriendlyDictionary<SearchPlatformFactory.SearchPlatformId, ISearchBackupPlatform>();
    [StaticSafe]
    private static readonly SearchPlatformFactory s_instance = new SearchPlatformFactory();

    private SearchPlatformFactory()
    {
    }

    public static ISearchPlatformFactory GetInstance() => (ISearchPlatformFactory) SearchPlatformFactory.s_instance;

    public ISearchPlatform Create(string connectionString, string platformSettings, bool isOnPrem)
    {
      ISearchPlatform searchPlatform;
      lock (SearchPlatformFactory.s_platformLock)
      {
        SearchPlatformFactory.SearchPlatformId key = new SearchPlatformFactory.SearchPlatformId(connectionString, platformSettings);
        if (!SearchPlatformFactory.s_platformMap.TryGetValue(key, out searchPlatform))
        {
          searchPlatform = (ISearchPlatform) new ElasticSearchPlatform(connectionString, platformSettings, isOnPrem);
          SearchPlatformFactory.s_platformMap.Add(key, searchPlatform);
        }
      }
      return searchPlatform;
    }

    public ISearchClusterManagementService CreateSearchClusterManagementService(
      string connectionString,
      string platformSettings,
      bool isOnPrem)
    {
      ISearchClusterManagementService managementService;
      lock (SearchPlatformFactory.s_clusterStateServiceLock)
      {
        SearchPlatformFactory.SearchPlatformId key = new SearchPlatformFactory.SearchPlatformId(connectionString, platformSettings);
        if (!SearchPlatformFactory.s_searchClusterManagementServicesMap.TryGetValue(key, out managementService))
        {
          managementService = (ISearchClusterManagementService) new ElasticsearchClusterManagementService(connectionString, platformSettings, isOnPrem);
          SearchPlatformFactory.s_searchClusterManagementServicesMap.Add(key, managementService);
        }
      }
      return managementService;
    }

    public ISearchBackupPlatform CreateBackupPlatform(
      string connectionString,
      string platformSettings,
      bool isOnPrem)
    {
      ISearchBackupPlatform backupPlatform;
      lock (SearchPlatformFactory.s_backupPlatformLock)
      {
        SearchPlatformFactory.SearchPlatformId key = new SearchPlatformFactory.SearchPlatformId(connectionString, platformSettings);
        if (!SearchPlatformFactory.s_backupPlatformMap.TryGetValue(key, out backupPlatform))
        {
          backupPlatform = (ISearchBackupPlatform) new ElasticSearchPlatformBackupOperations(connectionString, platformSettings, isOnPrem);
          SearchPlatformFactory.s_backupPlatformMap.Add(key, backupPlatform);
        }
      }
      return backupPlatform;
    }

    private class SearchPlatformId : Tuple<string, string>
    {
      public SearchPlatformId(string platformConnectionString, string platformSettings)
        : base(platformConnectionString, platformSettings)
      {
      }
    }
  }
}
