// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Indexer.IndexingUnitRoutingExtensions
// Assembly: Microsoft.VisualStudio.Services.Search.Indexer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 167B1EA6-4D18-408E-89C6-597B8290976F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Indexer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities.EntityProperties;
using Microsoft.VisualStudio.Services.Search.Common.Entities.EntityProperties.Code;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions;
using Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Microsoft.VisualStudio.Services.Search.Indexer
{
  public static class IndexingUnitRoutingExtensions
  {
    public static void SetupIndexRouting(
      this IndexingUnit indexingUnit,
      IndexingExecutionContext executionContext,
      IList<IndexingUnit> newIndexingUnits = null)
    {
      if (indexingUnit.IsRepository())
      {
        if (indexingUnit.IsLargeRepository(executionContext.RequestContext))
          indexingUnit.SetupIndex((ExecutionContext) executionContext);
        else
          indexingUnit.SetupRouting(executionContext, newIndexingUnits);
      }
      else
      {
        if (!(indexingUnit.IndexingUnitType == "ScopedIndexingUnit"))
          return;
        if (executionContext.RepositoryIndexingUnit.IsLargeRepository(executionContext.RequestContext))
        {
          string scopePath = ((ScopedGitRepositoryAttributes) indexingUnit.TFSEntityAttributes).ScopePath;
          string routing = scopePath != null ? scopePath.NormalizePath() : (string) null;
          if (string.IsNullOrWhiteSpace(routing))
            routing = "rootfolder";
          IndexingProperties properties = indexingUnit.Properties;
          IndexingUnit repositoryIndexingUnit = executionContext.RepositoryIndexingUnit;
          List<IndexInfo> indexInfoList;
          if (repositoryIndexingUnit == null)
          {
            indexInfoList = (List<IndexInfo>) null;
          }
          else
          {
            List<IndexInfo> indexIndices = repositoryIndexingUnit.Properties.IndexIndices;
            indexInfoList = indexIndices != null ? indexIndices.Select<IndexInfo, IndexInfo>((Func<IndexInfo, IndexInfo>) (i => new IndexInfo()
            {
              IndexName = i.IndexName,
              Routing = routing,
              Version = i.Version
            })).ToList<IndexInfo>() : (List<IndexInfo>) null;
          }
          properties.IndexIndices = indexInfoList;
        }
        else
          indexingUnit.Properties.IndexIndices = executionContext.RepositoryIndexingUnit?.Properties.IndexIndices;
      }
    }

    private static void SetupIndex(
      this IndexingUnit indexingUnit,
      ExecutionContext executionContext)
    {
      IndexingExecutionContext indexingExecutionContext = new IndexingExecutionContext(executionContext.RequestContext, indexingUnit, TracerCICorrelationDetailsFactory.GetCICorrelationDetails(executionContext.RequestContext, MethodBase.GetCurrentMethod().Name, 29));
      indexingExecutionContext.FaultService = executionContext.FaultService;
      indexingExecutionContext.InitializeNameAndIds(DataAccessFactory.GetInstance());
      if (indexingUnit.GetIndexInfo()?.IndexName != null && indexingExecutionContext.ProvisioningContext.SearchPlatform.IndexExists(executionContext, IndexIdentity.CreateIndexIdentity(indexingUnit.GetIndexInfo().IndexName)))
        return;
      IndexIdentity indexIdentity = executionContext.RequestContext.GetIndexProvisionerFactory(indexingUnit.EntityType).GetIndexProvisioner(indexingExecutionContext, indexingUnit).ProvisionIndex(indexingExecutionContext, indexingExecutionContext.ProvisioningContext.SearchPlatform);
      indexingUnit.Properties.IndexIndices = new List<IndexInfo>()
      {
        new IndexInfo()
        {
          IndexName = indexIdentity.Name,
          Version = new int?(indexingExecutionContext.GetIndexVersion(indexIdentity.Name))
        }
      };
    }

    private static void SetupRouting(
      this IndexingUnit indexingUnit,
      IndexingExecutionContext executionContext,
      IList<IndexingUnit> newIndexingUnits)
    {
      string b = (string) null;
      if (executionContext.CollectionIndexingUnit.IsIndexingIndexNameAvailable())
        b = executionContext.CollectionIndexingUnit.GetIndexInfo().IndexName;
      IndexInfo indexInfo = indexingUnit.GetIndexInfo();
      if (indexInfo == null || indexInfo.Routing == null)
      {
        IRoutingSelectorService service = executionContext.RequestContext.GetService<IRoutingSelectorService>();
        string indexName = b;
        string str = newIndexingUnits != null ? service.SelectRoutingForNextRepository(executionContext, newIndexingUnits, indexName) : service.SelectRoutingForNextRepository(executionContext, indexName);
        indexInfo = new IndexInfo()
        {
          IndexName = indexName,
          Version = new int?(executionContext.GetIndexVersion(indexName)),
          Routing = str
        };
        indexingUnit.Properties.IndexIndices = new List<IndexInfo>()
        {
          indexInfo
        };
      }
      if (string.Equals(indexInfo.IndexName, b, StringComparison.Ordinal))
        return;
      indexInfo.IndexName = b;
      indexingUnit.Properties.IndexIndices = new List<IndexInfo>()
      {
        indexInfo
      };
    }
  }
}
