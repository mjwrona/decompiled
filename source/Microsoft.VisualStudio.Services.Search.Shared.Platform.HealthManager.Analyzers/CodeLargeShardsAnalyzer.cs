// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.Analyzers.CodeLargeShardsAnalyzer
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.Analyzers, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B1A24B0F-DA40-425D-8B54-1865D1FC90B8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.Analyzers.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.VersionControl.Common.Internal;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Indexer;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch;
using Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.Data;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManagerAPI;
using Microsoft.VisualStudio.Services.WebPlatform;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.Analyzers
{
  public class CodeLargeShardsAnalyzer : LargeShardsAnalyzer
  {
    private readonly string m_methodName = "Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.DataProviders.EsIndexDocumentCountDataProvider.Initialize";
    internal ISearchPlatformFactory m_searchPlatformFactory;
    private ISearchClusterManagementService m_searchClusterManagementService;
    private readonly IDictionary<string, string> m_vcTypeIndexingUnitTyeMap;

    public CodeLargeShardsAnalyzer()
      : this(SearchPlatformFactory.GetInstance(), Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer.DataAccessFactory.GetInstance())
    {
    }

    [Info("InternalForTestPurpose")]
    internal CodeLargeShardsAnalyzer(
      ISearchPlatformFactory searchPlatformFactory,
      IDataAccessFactory dataAccessFactory)
    {
      this.m_vcTypeIndexingUnitTyeMap = (IDictionary<string, string>) new Dictionary<string, string>()
      {
        {
          "git",
          "Git_Repository"
        },
        {
          "tfvc",
          "TFVC_Repository"
        },
        {
          "custom",
          "CustomRepository"
        }
      };
      this.m_searchPlatformFactory = searchPlatformFactory;
      this.DataAccessFactory = dataAccessFactory;
    }

    internal IDataAccessFactory DataAccessFactory { get; }

    protected ExecutionContext ExecutionContext { get; set; }

    internal string _esConnectionString { get; set; }

    [Info("InternalForTestPurpose")]
    internal void InstantiateSearchClusterManagementService(ESDeploymentContext esDeploymentContext)
    {
      this.m_searchClusterManagementService = this.m_searchPlatformFactory.CreateSearchClusterManagementService(esDeploymentContext.SearchPlatformConnectionString, esDeploymentContext.SearchPlatformSettings, false);
      this.ExecutionContext = esDeploymentContext.RequestContext.GetExecutionContext(TracerCICorrelationDetailsFactory.GetCICorrelationDetails(esDeploymentContext.RequestContext.ActivityId.ToString(), this.m_methodName, 0));
      this._esConnectionString = esDeploymentContext.RequestContext.GetConfigValue("/Service/ALMSearch/Settings/JobAgentSearchPlatformConnectionString");
    }

    public override List<ActionData> Analyze(
      List<HealthData> dataList,
      Dictionary<DataType, ProviderContext> contextDataSet,
      out string result)
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append(FormattableString.Invariant(FormattableStringFactory.Create("Analyzer: {0}.", (object) nameof (CodeLargeShardsAnalyzer))));
      string result1;
      base.Analyze(dataList, contextDataSet, out result1);
      stringBuilder.Append(result1);
      try
      {
        IDictionary<string, string[]> largeShardMap = this.LargeShardMap;
        if ((largeShardMap != null ? (!largeShardMap.Any<KeyValuePair<string, string[]>>() ? 1 : 0) : 1) != 0)
        {
          stringBuilder.Append(FormattableString.Invariant(FormattableStringFactory.Create("{0} is Empty.Nothing to analyze. Exiting", (object) "LargeShardMap")));
          return new List<ActionData>();
        }
        ESDeploymentContext contextData = (ESDeploymentContext) contextDataSet[DataType.ShardSizeData];
        IVssRequestContext requestContext = contextData.RequestContext;
        this.InstantiateSearchClusterManagementService(contextData);
        this.GetCollectionsToBeMigrated(requestContext);
        return new List<ActionData>();
      }
      finally
      {
        result = stringBuilder.ToString();
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishClientTraceMessage("Health Manager", "HealthManagerAnalyzer", Level.Info, result);
        stringBuilder.Clear();
      }
    }

    private List<Guid> GetCollectionsToBeMigrated(IVssRequestContext requestContext)
    {
      DocumentContractType documentContractType = requestContext.GetConfigValue<DocumentContractType>("/Service/ALMSearch/Settings/DefaultCodeDocumentContractType", TeamFoundationHostType.Deployment);
      if (DocumentContractTypeExtension.IsValidCodeDocumentContractType(documentContractType))
        return this.LargeShardMap.SelectMany<KeyValuePair<string, string[]>, Guid>((Func<KeyValuePair<string, string[]>, IEnumerable<Guid>>) (it => (IEnumerable<Guid>) this.GetCollectionsTobeMigratedForAnIndex(requestContext, documentContractType, it))).Distinct<Guid>().ToList<Guid>();
      throw new InvalidOperationException(FormattableString.Invariant(FormattableStringFactory.Create("Unsupported contract type {0}", (object) documentContractType)));
    }

    [Info("InternalForTestPurpose")]
    internal Set<Guid> GetCollectionsTobeMigratedForAnIndex(
      IVssRequestContext requestContext,
      DocumentContractType contractType,
      KeyValuePair<string, string[]> largeShards)
    {
      Set<Guid> migratedForAnIndex = new Set<Guid>();
      IRoutingLookupService service = requestContext.GetService<IRoutingLookupService>();
      string key = largeShards.Key;
      foreach (string s in largeShards.Value)
      {
        string routingKey = service.GetRoutingKey(requestContext, this._esConnectionString, key, int.Parse(s, (IFormatProvider) CultureInfo.CurrentCulture));
        Guid migratedFromTheShard = this.GetCollectionToBeMigratedFromTheShard(requestContext, key, contractType, routingKey);
        if (migratedFromTheShard != Guid.Empty)
          migratedForAnIndex.Add(migratedFromTheShard);
      }
      return migratedForAnIndex;
    }

    [Info("InternalForTestPurpose")]
    internal Guid GetCollectionToBeMigratedFromTheShard(
      IVssRequestContext requestContext,
      string indexName,
      DocumentContractType contractType,
      string routingId)
    {
      string key = this.GetRepoWiseDocCountForGivenShard(indexName, contractType, routingId).First<KeyValuePair<string, long>>().Key;
      IDictionary<string, string> dataForGivenRepoId = this.GetMetaDataForGivenRepoId(indexName, contractType, key);
      Guid guid = new Guid(dataForGivenRepoId["collectionId"]);
      return !this.VerifyCollectionRepo(requestContext, dataForGivenRepoId, key) ? Guid.Empty : guid;
    }

    [Info("InternalForTestPurpose")]
    internal bool VerifyCollectionRepo(
      IVssRequestContext requestContext,
      IDictionary<string, string> repositoryMetaData,
      string repoId)
    {
      string g = repositoryMetaData["collectionId"];
      string typeIndexingUnitTye = this.m_vcTypeIndexingUnitTyeMap[repositoryMetaData["vcType"]];
      ITeamFoundationHostManagementService service = requestContext.GetService<ITeamFoundationHostManagementService>();
      bool flag = false;
      IVssRequestContext requestContext1 = requestContext;
      Guid instanceId = new Guid(g);
      using (IVssRequestContext requestContext2 = service.BeginRequest(requestContext1, instanceId, RequestContextType.SystemContext))
        flag = this.DataAccessFactory.GetIndexingUnitDataAccess().GetIndexingUnit(requestContext2, new Guid(repoId), typeIndexingUnitTye, (IEntityType) CodeEntityType.GetInstance()) != null;
      if (!flag)
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceError(1083041, "Indexing Pipeline", "IndexingOperation", "Collection Id ,repo mismatch, inconsistent indexing");
      return flag;
    }

    private IDictionary<string, string> GetMetaDataForGivenRepoId(
      string indexName,
      DocumentContractType contractType,
      string repoId)
    {
      return this.m_searchClusterManagementService.GetFieldValues("repositoryId", repoId, new string[2]
      {
        "collectionId",
        "vcType"
      }, indexName, contractType);
    }

    private List<KeyValuePair<string, long>> GetRepoWiseDocCountForGivenShard(
      string indexName,
      DocumentContractType contractType,
      string routingId)
    {
      return this.m_searchClusterManagementService.GetFieldWiseDocumentCount(this.ExecutionContext, indexName, contractType, "repositoryId", routingId);
    }
  }
}
