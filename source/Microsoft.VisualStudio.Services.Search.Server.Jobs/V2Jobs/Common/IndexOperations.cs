// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.V2Jobs.Common.IndexOperations
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions;
using Microsoft.VisualStudio.Services.Search.Indexer;
using Microsoft.VisualStudio.Services.Search.Indexer.IndexProvisioner.Definitions;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions;

namespace Microsoft.VisualStudio.Services.Search.V2Jobs.Common
{
  public class IndexOperations
  {
    public virtual IndexOperationsResponse PerformBulkDelete(
      IndexingExecutionContext executionContext,
      IndexIdentity index,
      IExpression query,
      bool forceComplete = false,
      bool leniant = false)
    {
      IndexUpdaterParams indexUpdaterParams = new IndexUpdaterParams()
      {
        ContractType = executionContext.ProvisioningContext.ContractType,
        IndexSubScope = new IndexSubScope()
        {
          AccountId = executionContext.RequestContext.GetAccountIdAsNormalizedString(),
          CollectionId = executionContext.RequestContext.GetCollectionIdAsNormalizedString()
        },
        IndexIdentity = index
      };
      return IndexerFactory.CreateIndexUpdater(executionContext, indexUpdaterParams, executionContext.ProvisioningContext.SearchPlatform, executionContext.GetRouteLevel()).DeleteDocumentsByQuery(query, forceComplete, leniant);
    }
  }
}
