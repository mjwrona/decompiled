// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Indexer.CreateSearchIndexHelper
// Assembly: Microsoft.VisualStudio.Services.Search.Indexer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 167B1EA6-4D18-408E-89C6-597B8290976F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Indexer.dll

using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Common.FaultManagement;
using Microsoft.VisualStudio.Services.Search.Indexer.IndexProvisioner.Entities;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions;
using Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using Nest;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;

namespace Microsoft.VisualStudio.Services.Search.Indexer
{
  public class CreateSearchIndexHelper
  {
    internal virtual bool CreateIndex(
      ExecutionContext executionContext,
      IndexIdentity indexIdentity,
      ISearchPlatform searchPlatform,
      ISearchClusterManagementService searchClusterManagementService,
      IDataAccessFactory dataAccessFactory,
      IndexProvisionType provisionType,
      DocumentContractType contractType,
      ProvisionerConfigAndConstantsProvider entityProvisionProvider)
    {
      try
      {
        if (!entityProvisionProvider.IsEnabled(executionContext.RequestContext))
          throw new InvalidOperationException(FormattableString.Invariant(FormattableStringFactory.Create("Provisioner provider: {0} is disabled", (object) entityProvisionProvider.GetType().Name)));
        searchPlatform.CreateIndex(executionContext, indexIdentity, entityProvisionProvider.GetIndexSettings(executionContext, provisionType, contractType), entityProvisionProvider.GetIndexMappings(executionContext.RequestContext, contractType));
        GenericInvoker.Instance.Invoke<bool>((Func<bool>) (() => this.ValidateIndex(executionContext, indexIdentity, searchPlatform, provisionType, entityProvisionProvider.EntityType, entityProvisionProvider)), 2, executionContext.ServiceSettings.ProvisionerSettings.IndexCreationWaitTimeInSec, new TraceMetaData(1082256, "Indexing Pipeline", "IndexingOperation"));
        this.PopulateShardDetails(executionContext, searchClusterManagementService, dataAccessFactory, entityProvisionProvider, provisionType, indexIdentity, contractType);
        return true;
      }
      catch (SearchPlatformException ex)
      {
        if (ex.ErrorCode == SearchServiceErrorCode.IndexExists)
          this.ValidateIndex(executionContext, indexIdentity, searchPlatform, provisionType, entityProvisionProvider.EntityType, entityProvisionProvider);
        else
          ExceptionDispatchInfo.Capture((Exception) ex).Throw();
        return true;
      }
    }

    internal virtual void PopulateShardDetails(
      ExecutionContext executionContext,
      ISearchClusterManagementService searchClusterManagementService,
      IDataAccessFactory dataAccessFactory,
      ProvisionerConfigAndConstantsProvider entityProvisionProvider,
      IndexProvisionType provisionType,
      IndexIdentity indexIdentity,
      DocumentContractType contractType)
    {
      List<EsShardDetails> shardsDetails = searchClusterManagementService.GetShardsDetails(executionContext, indexIdentity.Name);
      IShardDetailsDataAccess detailsDataAccess = dataAccessFactory.GetShardDetailsDataAccess();
      List<ShardDetails> shardDetails = new List<ShardDetails>();
      Action<EsShardDetails> action = (Action<EsShardDetails>) (esShardDetails =>
      {
        long reservedSpace;
        int reservedDocCount;
        entityProvisionProvider.GetReservedSpaceInShards(executionContext, provisionType, contractType, esShardDetails.ActualSize, out reservedSpace, out reservedDocCount);
        shardDetails.Add(new ShardDetails()
        {
          ActualDocCount = esShardDetails.ActualDocCount,
          EstimatedDocCount = esShardDetails.ActualDocCount,
          EsClusterName = esShardDetails.EsClusterName,
          ActualSize = esShardDetails.ActualSize,
          EstimatedSize = esShardDetails.ActualSize,
          IndexName = esShardDetails.IndexName,
          ShardId = esShardDetails.ShardId,
          DeletedDocCount = esShardDetails.DeletedDocCount,
          EntityType = entityProvisionProvider.EntityType,
          ReservedDocCount = reservedDocCount,
          ReservedSpace = reservedSpace
        });
      });
      shardsDetails.ForEach(action);
      detailsDataAccess.InsertOrUpdateShardDetails(executionContext.RequestContext, shardDetails);
    }

    public virtual bool ValidateIndex(
      ExecutionContext executionContext,
      IndexIdentity indexIdentity,
      ISearchPlatform searchPlatform,
      IndexProvisionType provisionType,
      IEntityType entityType,
      ProvisionerConfigAndConstantsProvider provisionerConfigAndConstantsProvider)
    {
      ISearchIndex index = searchPlatform.GetIndex(indexIdentity);
      HealthStatus health = index.GetHealth();
      switch (health)
      {
        case HealthStatus.Yellow:
        case HealthStatus.Green:
          IIndexSettings settings = index.GetSettings();
          int? nullable = settings.NumberOfShards;
          int numberOfPrimaries = provisionerConfigAndConstantsProvider.GetNumberOfPrimaries(executionContext.RequestContext, provisionType);
          if (nullable.GetValueOrDefault() == numberOfPrimaries & nullable.HasValue)
          {
            nullable = settings.NumberOfReplicas;
            int replicas = executionContext.ServiceSettings.ProvisionerSettings.Replicas;
            if (nullable.GetValueOrDefault() == replicas & nullable.HasValue)
              return true;
          }
          throw new IndexProvisionException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}IndexProvisioner: Index: {1} is created outside of Service Expected P:{2} R:{3} Found P:{4} R:{5}", (object) provisionType, (object) indexIdentity.Name, (object) provisionerConfigAndConstantsProvider.GetNumberOfPrimaries(executionContext.RequestContext, provisionType), (object) executionContext.ServiceSettings.ProvisionerSettings.Replicas, (object) settings.NumberOfShards, (object) settings.NumberOfReplicas));
        default:
          throw new IndexProvisionException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}IndexProvisioner: Index: {1} is still {2}", (object) provisionType, (object) indexIdentity.Name, (object) health));
      }
    }
  }
}
