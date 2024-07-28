// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Indexer.ExecutionContextExtensions
// Assembly: Microsoft.VisualStudio.Services.Search.Indexer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 167B1EA6-4D18-408E-89C6-597B8290976F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Indexer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Entities.EntityProperties;
using Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.Search.Indexer
{
  public static class ExecutionContextExtensions
  {
    public static bool IsReindexingFailedOrInProgress(
      this ExecutionContext executionContext,
      IDataAccessFactory dataAccessFactory,
      IEntityType entityType)
    {
      if (executionContext.RequestContext.ExecutionEnvironment.IsOnPremisesDeployment)
        return false;
      ReindexingStatusEntry reindexingStatusEntry = dataAccessFactory.GetReindexingStatusDataAccess().GetReindexingStatusEntry(executionContext.RequestContext.To(TeamFoundationHostType.Deployment), executionContext.RequestContext.GetCollectionID(), entityType);
      return reindexingStatusEntry != null && reindexingStatusEntry.IsReindexingFailedOrInProgress();
    }

    public static bool IsReindexingInProgress(
      this ExecutionContext executionContext,
      IDataAccessFactory dataAccessFactory,
      IEntityType entityType)
    {
      if (executionContext.RequestContext.ExecutionEnvironment.IsOnPremisesDeployment)
        return false;
      ReindexingStatusEntry reindexingStatusEntry = dataAccessFactory.GetReindexingStatusDataAccess().GetReindexingStatusEntry(executionContext.RequestContext.To(TeamFoundationHostType.Deployment), executionContext.RequestContext.GetCollectionID(), entityType);
      return reindexingStatusEntry != null && reindexingStatusEntry.IsReindexingInProgress();
    }

    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "executionContext")]
    public static bool IsCrossClusterReindexingInProgress(
      this ExecutionContext executionContext,
      CollectionIndexingProperties collectionIndexingProperties)
    {
      return collectionIndexingProperties != null && !string.Equals(collectionIndexingProperties.IndexESConnectionString, collectionIndexingProperties.QueryESConnectionString);
    }
  }
}
