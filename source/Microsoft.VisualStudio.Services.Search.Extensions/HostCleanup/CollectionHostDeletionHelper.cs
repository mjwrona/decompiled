// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Extensions.HostCleanup.CollectionHostDeletionHelper
// Assembly: Microsoft.VisualStudio.Services.Search.Extensions, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1D8FF195-304B-4BBA-9D1C-F4A6093CE2E1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Extensions.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Arriba;
using Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Indexer;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions;
using Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer.Sql;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Extensions.HostCleanup
{
  public class CollectionHostDeletionHelper
  {
    private readonly IVssRequestContext m_deploymentContext;
    private readonly Guid m_collectionId;

    public CollectionHostDeletionHelper(IVssRequestContext deploymentContext, Guid collectionId)
    {
      this.m_deploymentContext = deploymentContext;
      this.m_collectionId = collectionId;
    }

    public virtual void CleanUpElasticsearchAndReindexingTable()
    {
      ExecutionContext executionContext = this.m_deploymentContext.GetExecutionContext(TracerCICorrelationDetailsFactory.GetCICorrelationDetails(this.m_deploymentContext, "HostCleanup", 21));
      ISearchPlatform searchPlatform = SearchPlatformFactory.GetInstance().Create(executionContext.ServiceSettings.JobAgentSearchPlatformConnectionString, executionContext.ServiceSettings.JobAgentSearchPlatformSettings, this.m_deploymentContext.ExecutionEnvironment.IsOnPremisesDeployment);
      this.DeleteElasticSearchData(executionContext, this.m_collectionId, searchPlatform);
      ReindexingStatusDataAccess statusDataAccess = new ReindexingStatusDataAccess();
      statusDataAccess.DeleteReindexingStatusEntry(this.m_deploymentContext, this.m_collectionId, (IEntityType) CodeEntityType.GetInstance());
      statusDataAccess.DeleteReindexingStatusEntry(this.m_deploymentContext, this.m_collectionId, (IEntityType) WorkItemEntityType.GetInstance());
      statusDataAccess.DeleteReindexingStatusEntry(this.m_deploymentContext, this.m_collectionId, (IEntityType) WikiEntityType.GetInstance());
      this.m_deploymentContext.TraceAlways(1082601, TraceLevel.Info, "Host Cleanup", "HostCleanup", FormattableString.Invariant(FormattableStringFactory.Create("Deleted data from ES and SQL successfully for host id {0}.", (object) this.m_collectionId)));
    }

    private void DeleteElasticSearchData(
      ExecutionContext executionContext,
      Guid collectionId,
      ISearchPlatform searchPlatform)
    {
      TermExpression filter = new TermExpression(nameof (collectionId), Operator.Equals, collectionId.ToString().NormalizeString());
      searchPlatform.DeleteAllDocuments(executionContext, (IExpression) filter);
    }
  }
}
