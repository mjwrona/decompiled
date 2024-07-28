// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch.RoutingLookupService
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions;
using Nest;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch
{
  public class RoutingLookupService : IRoutingLookupService, IVssFrameworkService
  {
    private IDictionary<string, ISearchPlatform> m_connectionStringSearchPlatformMap;
    private IDictionary<string, IDictionary<string, IndexSetting>> m_searchClusterIndexVersionMap;
    private IDictionary<string, IDictionary<int, IndexRoutingLookupTable>> m_esVersionedRoutingKeyLookupMap;
    private readonly ISearchPlatformFactory m_searchPlatformFactory;
    private readonly object m_lock = new object();

    public RoutingLookupService()
      : this(SearchPlatformFactory.GetInstance())
    {
    }

    [Info("InternalForTestPurpose")]
    internal RoutingLookupService(ISearchPlatformFactory searchPlatformFactory) => this.m_searchPlatformFactory = searchPlatformFactory;

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      if (!systemRequestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        throw new UnsupportedHostTypeException(systemRequestContext.ServiceHost.HostType);
      this.InitializeService();
    }

    public string GetRoutingKey(
      IVssRequestContext requestContext,
      string connectionString,
      string indexName,
      int shardId)
    {
      this.UpdateRoutingLookupCacheIfNeeded(requestContext, connectionString, indexName);
      IndexSetting indexSetting = this.m_searchClusterIndexVersionMap[connectionString][indexName];
      if (shardId < 0 || shardId >= indexSetting.NumPrimaries)
        throw new ArgumentOutOfRangeException(nameof (shardId));
      return this.m_esVersionedRoutingKeyLookupMap[indexSetting.IndexCreatedVersion][indexSetting.NumPrimaries].GetRoutingKey(shardId);
    }

    public int GetShardId(
      IVssRequestContext requestContext,
      string connectionString,
      string indexName,
      string routing)
    {
      this.UpdateRoutingLookupCacheIfNeeded(requestContext, connectionString, indexName);
      IndexSetting indexSetting = this.m_searchClusterIndexVersionMap[connectionString][indexName];
      return !this.m_esVersionedRoutingKeyLookupMap[indexSetting.IndexCreatedVersion][indexSetting.NumPrimaries].ContainsRoutingMapping(routing) ? this.m_connectionStringSearchPlatformMap[connectionString].GetShardId(this.ToExecutionContext(requestContext), indexName, routing) : this.m_esVersionedRoutingKeyLookupMap[indexSetting.IndexCreatedVersion][indexSetting.NumPrimaries].GetShardId(routing);
    }

    private void InitializeService()
    {
      this.m_searchClusterIndexVersionMap = (IDictionary<string, IDictionary<string, IndexSetting>>) new FriendlyDictionary<string, IDictionary<string, IndexSetting>>();
      this.m_esVersionedRoutingKeyLookupMap = (IDictionary<string, IDictionary<int, IndexRoutingLookupTable>>) new FriendlyDictionary<string, IDictionary<int, IndexRoutingLookupTable>>();
      this.m_connectionStringSearchPlatformMap = (IDictionary<string, ISearchPlatform>) new FriendlyDictionary<string, ISearchPlatform>();
    }

    private void PopulateIndexLevelRoutingInformation(
      ExecutionContext executionContext,
      string esConnectionString,
      string index)
    {
      ISearchPlatform stringSearchPlatform = this.m_connectionStringSearchPlatformMap[esConnectionString];
      IIndexSettings settings = stringSearchPlatform.GetIndex(IndexIdentity.CreateIndexIdentity(index)).GetSettings();
      string str = (string) settings["index.version.created"];
      int num = settings.NumberOfShards.Value;
      if (!this.m_searchClusterIndexVersionMap[esConnectionString].ContainsKey(index))
        this.m_searchClusterIndexVersionMap[esConnectionString].Add(index, new IndexSetting(str, num));
      if (!this.m_esVersionedRoutingKeyLookupMap.ContainsKey(str))
      {
        IDictionary<int, IndexRoutingLookupTable> dictionary = (IDictionary<int, IndexRoutingLookupTable>) new Dictionary<int, IndexRoutingLookupTable>();
        this.m_esVersionedRoutingKeyLookupMap.Add(str, dictionary);
      }
      if (this.m_esVersionedRoutingKeyLookupMap[str].ContainsKey(num))
        return;
      IndexRoutingLookupTable routingLookupTable = this.GetShardRoutingLookupTable(stringSearchPlatform, executionContext, index, num);
      this.m_esVersionedRoutingKeyLookupMap[str].Add(num, routingLookupTable);
    }

    private void InitializeSearchPlatform(
      IVssRequestContext requestContext,
      string searchConnectionString)
    {
      ISearchPlatform searchPlatform = this.m_searchPlatformFactory.Create(searchConnectionString, requestContext.GetElasticsearchPlatformSettings("/Service/ALMSearch/Settings/JobAgentSearchPlatformSettings"), requestContext.ExecutionEnvironment.IsOnPremisesDeployment);
      IDictionary<string, IndexSetting> dictionary = (IDictionary<string, IndexSetting>) new Dictionary<string, IndexSetting>();
      this.m_searchClusterIndexVersionMap[searchConnectionString] = dictionary;
      this.m_connectionStringSearchPlatformMap[searchConnectionString] = searchPlatform;
      ExecutionContext executionContext = this.ToExecutionContext(requestContext);
      foreach (CatIndicesRecord record in (IEnumerable<CatIndicesRecord>) searchPlatform.GetIndices(executionContext).Records)
      {
        if (record.Index != null && !record.Index.StartsWith(".", StringComparison.OrdinalIgnoreCase))
          this.PopulateIndexLevelRoutingInformation(executionContext, searchConnectionString, record.Index);
      }
    }

    private void UpdateRoutingLookupCacheIfNeeded(
      IVssRequestContext requestContext,
      string connectionString,
      string indexName)
    {
      if (!this.m_searchClusterIndexVersionMap.ContainsKey(connectionString))
      {
        lock (this.m_lock)
        {
          if (!this.m_searchClusterIndexVersionMap.ContainsKey(connectionString))
            this.InitializeSearchPlatform(requestContext, connectionString);
        }
      }
      IDictionary<string, IndexSetting> clusterIndexVersion = this.m_searchClusterIndexVersionMap[connectionString];
      if (clusterIndexVersion.ContainsKey(indexName))
        return;
      lock (this.m_lock)
      {
        if (clusterIndexVersion.ContainsKey(indexName))
          return;
        this.PopulateIndexLevelRoutingInformation(this.ToExecutionContext(requestContext), connectionString, indexName);
      }
    }

    private IndexRoutingLookupTable GetShardRoutingLookupTable(
      ISearchPlatform searchPlatform,
      ExecutionContext executionContext,
      string indexName,
      int numPrimaries)
    {
      IndexRoutingLookupTable routingLookupTable = new IndexRoutingLookupTable(numPrimaries);
      int num1 = 0;
      int num2 = 0;
      while (num1 < numPrimaries)
      {
        int shardId = searchPlatform.GetShardId(executionContext, indexName, num2.ToString((IFormatProvider) CultureInfo.InvariantCulture));
        if (routingLookupTable.GetRoutingKey(shardId) == null)
        {
          routingLookupTable.AddorUpdate(num2.ToString((IFormatProvider) CultureInfo.InvariantCulture), shardId);
          ++num1;
        }
        ++num2;
      }
      return routingLookupTable;
    }

    private ExecutionContext ToExecutionContext(IVssRequestContext requestContext) => requestContext.GetExecutionContext(nameof (RoutingLookupService), 0);

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
      this.m_searchClusterIndexVersionMap = (IDictionary<string, IDictionary<string, IndexSetting>>) null;
      this.m_esVersionedRoutingKeyLookupMap = (IDictionary<string, IDictionary<int, IndexRoutingLookupTable>>) null;
      this.m_connectionStringSearchPlatformMap = (IDictionary<string, ISearchPlatform>) null;
    }
  }
}
