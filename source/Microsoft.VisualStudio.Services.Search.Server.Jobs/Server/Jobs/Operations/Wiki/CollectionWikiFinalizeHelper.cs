// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.Wiki.CollectionWikiFinalizeHelper
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Indexer;
using Microsoft.VisualStudio.Services.Search.Indexer.IndexProvisioner.Definitions;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.Wiki
{
  internal class CollectionWikiFinalizeHelper : EntityFinalizerBase
  {
    internal override List<IndexInfo> GetQueryIndexInfo(IndexingExecutionContext executionContext)
    {
      IndexingExecutionContext differentIndexingUnit = executionContext.ToIndexingExecutionContextWithDifferentIndexingUnit(executionContext.IndexingUnit);
      IndexIdentity index = differentIndexingUnit.GetIndex();
      List<IndexSubScope> validSubScopes = new List<IndexSubScope>()
      {
        new IndexSubScope()
        {
          AccountId = executionContext.RequestContext.GetOrganizationID().ToString(),
          CollectionId = executionContext.RequestContext.GetCollectionID().ToString()
        }
      };
      return new List<IndexInfo>()
      {
        new IndexInfo()
        {
          EntityName = executionContext.IndexingUnit.Properties.Name,
          IndexName = index.Name,
          Version = new int?(executionContext.GetIndexVersion(index.Name)),
          Routing = IndexSubScope.GetRoutingValuesToPublishIndex(differentIndexingUnit.GetRouteLevel(), (IList<IndexSubScope>) validSubScopes),
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
      return this.GetQueryIndexInfo(indexingExecutionContext);
    }
  }
}
