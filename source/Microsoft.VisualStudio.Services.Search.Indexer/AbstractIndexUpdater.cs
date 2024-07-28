// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Indexer.AbstractIndexUpdater
// Assembly: Microsoft.VisualStudio.Services.Search.Indexer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 167B1EA6-4D18-408E-89C6-597B8290976F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Indexer.dll

using Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Indexer.IndexProvisioner.Definitions;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.Indexer
{
  public abstract class AbstractIndexUpdater
  {
    protected ISearchIndex SearchIndex { get; }

    protected string RoutingInfo { get; }

    protected IndexingExecutionContext IndexingExecutionContext { get; }

    protected DocumentContractType DocumentContractType { get; }

    protected AbstractIndexUpdater()
    {
    }

    protected AbstractIndexUpdater(
      IndexingExecutionContext indexingExecutionContext,
      IndexSubScope subScopes,
      ISearchIndex searchIndex,
      RouteLevel routeLevel,
      DocumentContractType docContractType)
    {
      this.IndexingExecutionContext = indexingExecutionContext;
      this.SearchIndex = searchIndex;
      this.RoutingInfo = subScopes.GetRouteInfoToUpdate(routeLevel);
      this.DocumentContractType = docContractType;
    }

    public abstract IndexOperationsResponse InsertBatch(
      IEnumerable<AbstractSearchDocumentContract> batch);

    public abstract IndexOperationsResponse DeleteDocuments(
      IEnumerable<AbstractSearchDocumentContract> batch);

    public abstract IndexOperationsResponse DeleteDocumentsByQuery(
      IExpression query,
      bool forceComplete,
      bool leniant = false);

    public abstract void PublishTimeToIndex(
      IEnumerable<AbstractSearchDocumentContract> documents);
  }
}
