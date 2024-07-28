// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.WorkItem.CollectionWorkItemFinalizeHelper
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Indexer;
using Microsoft.VisualStudio.Services.Search.Indexer.IndexProvisioner.Definitions;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.WorkItem
{
  internal class CollectionWorkItemFinalizeHelper : EntityFinalizerBase
  {
    public override void UpdateFeatureFlagsIfNeeded(IndexingExecutionContext executionContext) => executionContext.RequestContext.GetService<ITeamFoundationFeatureAvailabilityService>().SetFeatureState(executionContext.RequestContext, "Search.Server.WorkItem.QueryIdentityFields", FeatureAvailabilityState.On);

    internal override IEnumerable<IndexingUnit> GetChildrenIndexingUnitsToFinalize(
      IndexingExecutionContext executionContext,
      bool isShadow = false)
    {
      throw new NotSupportedException();
    }

    internal override List<IndexInfo> GetQueryIndexInfo(IndexingExecutionContext executionContext)
    {
      List<IndexSubScope> validSubScopes = new List<IndexSubScope>()
      {
        new IndexSubScope()
        {
          AccountId = executionContext.RequestContext.GetOrganizationID().ToString(),
          CollectionId = executionContext.RequestContext.GetCollectionID().ToString()
        }
      };
      string indexingIndexName = executionContext.CollectionIndexingUnit.GetIndexingIndexName();
      return new List<IndexInfo>()
      {
        new IndexInfo()
        {
          EntityName = executionContext.CollectionIndexingUnit.Properties.Name,
          IndexName = indexingIndexName,
          Version = new int?(executionContext.GetIndexVersion(indexingIndexName)),
          Routing = IndexSubScope.GetRoutingValuesToPublishIndex(executionContext.GetRouteLevel(), (IList<IndexSubScope>) validSubScopes),
          DocumentContractType = executionContext.ProvisioningContext.ContractType
        }
      };
    }

    internal override List<IndexInfo> GetQueryIndexInfoWhenReIndexCompleted(
      IndexingExecutionContext executionContext)
    {
      return this.GetQueryIndexInfo(executionContext);
    }

    internal override List<IndexInfo> GetQueryIndexInfoWhenReIndexInProgress(
      IndexingExecutionContext indexingExecutionContext)
    {
      List<IndexSubScope> validSubScopes = new List<IndexSubScope>()
      {
        new IndexSubScope()
        {
          AccountId = indexingExecutionContext.RequestContext.GetOrganizationID().ToString(),
          CollectionId = indexingExecutionContext.RequestContext.GetCollectionID().ToString()
        }
      };
      string queryIndexName = indexingExecutionContext.CollectionIndexingUnit.GetQueryIndexName();
      return new List<IndexInfo>()
      {
        new IndexInfo()
        {
          EntityName = indexingExecutionContext.CollectionIndexingUnit.Properties.Name,
          IndexName = queryIndexName,
          Version = new int?(indexingExecutionContext.GetIndexVersion(queryIndexName)),
          Routing = IndexSubScope.GetRoutingValuesToPublishIndex(indexingExecutionContext.GetRouteLevel(), (IList<IndexSubScope>) validSubScopes),
          DocumentContractType = indexingExecutionContext.ProvisioningContext.ContractType
        }
      };
    }

    internal override List<string> GetIndexingUnitsTypeSupportingShadowIndexing(bool rootOnly = false) => new List<string>()
    {
      "Project"
    };

    internal override void PromoteShadowIndexingUnitsToPrimary(
      IndexingExecutionContext indexingExecutionContext,
      bool isZLRIEnabledForCustom = false)
    {
      indexingExecutionContext.IndexingUnitDataAccess.PromoteShadowIndexingUnitsToPrimary(indexingExecutionContext.RequestContext, this.GetIndexingUnitsTypeSupportingShadowIndexing(false), (IEntityType) WorkItemEntityType.GetInstance());
    }
  }
}
