// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.WebServer.IndexingStatusProvider
// Assembly: Microsoft.VisualStudio.Services.Search.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1112A012-BB03-4D21-B53E-3AFB00CCC7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.WebServer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Indexer;
using Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Search.WebServer
{
  public abstract class IndexingStatusProvider
  {
    private readonly IDataAccessFactory m_dataAccessFactory;

    protected abstract IEntityType EntityType { get; }

    internal IndexingStatusProvider(IDataAccessFactory dataAccessFactory) => this.m_dataAccessFactory = dataAccessFactory;

    public virtual CollectionIndexingStatus GetCollectionIndexingStatus(
      IVssRequestContext collectionRequestContext,
      List<Tuple<IEntityType, string, int>> inProgressOperationsTuple,
      out IndexingStatusDetails indexingStatusDetails)
    {
      indexingStatusDetails = (IndexingStatusDetails) null;
      if (this.IsReindexingFailedOrInProgress(collectionRequestContext))
      {
        CollectionIndexingStatus? indexingStatus = this.GetIndexingStatus(collectionRequestContext, 33, out indexingStatusDetails);
        if (indexingStatus.HasValue)
          return indexingStatus.Value;
      }
      CollectionIndexingStatus? nullable1 = new CollectionIndexingStatus?(CollectionIndexingStatus.NotIndexing);
      if (inProgressOperationsTuple != null && inProgressOperationsTuple.Count > 0)
      {
        int? nullable2 = inProgressOperationsTuple.Find((Predicate<Tuple<IEntityType, string, int>>) (tuple => tuple.Item1.Name == this.EntityType.Name && this.GetSupportedOperations().Contains(tuple.Item2)))?.Item3;
        if (nullable2.HasValue)
          nullable1 = this.GetIndexingStatus(collectionRequestContext, nullable2.Value, out indexingStatusDetails);
      }
      if (nullable1.HasValue)
      {
        CollectionIndexingStatus? nullable3 = nullable1;
        CollectionIndexingStatus collectionIndexingStatus = CollectionIndexingStatus.NotIndexing;
        if (!(nullable3.GetValueOrDefault() == collectionIndexingStatus & nullable3.HasValue))
          goto label_10;
      }
      IEnumerable<IndexInfo> queryIndexInfo = collectionRequestContext.GetService<IDocumentContractTypeService>().GetQueryIndexInfo(this.EntityType);
      if (queryIndexInfo == null || !queryIndexInfo.Any<IndexInfo>())
        nullable1 = new CollectionIndexingStatus?(CollectionIndexingStatus.Onboarding);
label_10:
      return nullable1.GetValueOrDefault();
    }

    public abstract ISet<string> GetSupportedOperations();

    public abstract ISet<int> GetSupportedJobTriggers();

    internal virtual bool IsReindexingFailedOrInProgress(IVssRequestContext collectionRequestContext)
    {
      if (collectionRequestContext.ExecutionEnvironment.IsOnPremisesDeployment)
        return false;
      ReindexingStatusEntry reindexingStatusEntry = this.m_dataAccessFactory.GetReindexingStatusDataAccess().GetReindexingStatusEntry(collectionRequestContext.To(TeamFoundationHostType.Deployment), collectionRequestContext.GetCollectionID(), this.EntityType);
      return reindexingStatusEntry != null && reindexingStatusEntry.IsReindexingFailedOrInProgress();
    }

    protected abstract CollectionIndexingStatus? GetIndexingStatus(
      IVssRequestContext requestContext,
      int trigger,
      out IndexingStatusDetails indexingStatusDetails);
  }
}
