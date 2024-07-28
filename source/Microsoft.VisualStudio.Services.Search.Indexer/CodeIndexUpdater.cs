// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Indexer.CodeIndexUpdater
// Assembly: Microsoft.VisualStudio.Services.Search.Indexer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 167B1EA6-4D18-408E-89C6-597B8290976F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Indexer.dll

using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Indexer.IndexProvisioner.Definitions;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.Code;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.Indexer
{
  internal sealed class CodeIndexUpdater : AbstractIndexUpdater
  {
    public CodeIndexUpdater(
      IndexingExecutionContext indexingExecutionContext,
      IndexSubScope subScope,
      ISearchIndex searchIndex,
      RouteLevel routingLevel,
      DocumentContractType docContractType)
      : base(indexingExecutionContext, subScope, searchIndex, routingLevel, docContractType)
    {
    }

    public override IndexOperationsResponse InsertBatch(
      IEnumerable<AbstractSearchDocumentContract> batch)
    {
      BulkIndexSyncRequest<AbstractSearchDocumentContract> indexRequest = new BulkIndexSyncRequest<AbstractSearchDocumentContract>()
      {
        Batch = batch,
        IndexIdentity = this.SearchIndex.IndexIdentity,
        ContractType = this.DocumentContractType,
        Routing = this.RoutingInfo
      };
      return CodeFileContract.GetContractInstance(this.DocumentContractType).InsertBatch<AbstractSearchDocumentContract>((ExecutionContext) this.IndexingExecutionContext, indexRequest, this.SearchIndex);
    }

    public override IndexOperationsResponse DeleteDocuments(
      IEnumerable<AbstractSearchDocumentContract> batch)
    {
      BulkDeleteRequest<AbstractSearchDocumentContract> deleteRequest = new BulkDeleteRequest<AbstractSearchDocumentContract>()
      {
        Batch = batch,
        IndexIdentity = this.SearchIndex.IndexIdentity,
        ContractType = this.DocumentContractType,
        Routing = this.RoutingInfo
      };
      return CodeFileContract.GetContractInstance(this.DocumentContractType).DeleteDocuments<AbstractSearchDocumentContract>((ExecutionContext) this.IndexingExecutionContext, deleteRequest, this.SearchIndex);
    }

    public override IndexOperationsResponse DeleteDocumentsByQuery(
      IExpression query,
      bool forceComplete,
      bool leniant = false)
    {
      TimeSpan requestTimeOut = TimeSpan.FromSeconds((double) this.IndexingExecutionContext.ServiceSettings.JobSettings.MaxAllowedOnPremiseJobExecutionTimeInSec);
      BulkDeleteByQueryRequest<AbstractSearchDocumentContract> bulkDeleteByQueryRequest = new BulkDeleteByQueryRequest<AbstractSearchDocumentContract>(query, this.DocumentContractType, requestTimeOut)
      {
        Lenient = leniant
      };
      return CodeFileContract.GetContractInstance(this.DocumentContractType).DeleteDocumentsByQuery<AbstractSearchDocumentContract>((ExecutionContext) this.IndexingExecutionContext, bulkDeleteByQueryRequest, this.SearchIndex, forceComplete);
    }

    public override void PublishTimeToIndex(
      IEnumerable<AbstractSearchDocumentContract> documents)
    {
    }
  }
}
