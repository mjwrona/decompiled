// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.Code.GitRepoRenameFinalizeOperation
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Entities.EntityProperties;
using Microsoft.VisualStudio.Services.Search.Common.Utils;
using Microsoft.VisualStudio.Services.Search.Indexer;
using Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions;
using Microsoft.VisualStudio.Services.Search.Server.Pipeline;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.Code
{
  internal class GitRepoRenameFinalizeOperation : AbstractIndexingOperation
  {
    public GitRepoRenameFinalizeOperation(
      ExecutionContext executionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent)
      : base(executionContext, indexingUnit, indexingUnitChangeEvent)
    {
    }

    public override OperationResult RunOperation(
      CoreIndexingExecutionContext coreIndexingExecutionContext)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(1080641, "Indexing Pipeline", "IndexingOperation", nameof (RunOperation));
      OperationResult operationResult = new OperationResult();
      StringBuilder stringBuilder = new StringBuilder();
      QueryScopingCacheUpdateData scopingCacheUpdateData = new QueryScopingCacheUpdateData();
      IndexingExecutionContext executionContext = (IndexingExecutionContext) coreIndexingExecutionContext;
      try
      {
        if (!this.ValidateCIFeatureFlags(executionContext))
        {
          operationResult.Status = OperationStatus.Succeeded;
          return operationResult;
        }
        string repositoryName = ((CodeRepoTFSAttributes) this.IndexingUnit.TFSEntityAttributes).RepositoryName;
        this.UpdateProjectNameInSqlChangeEventData(executionContext, scopingCacheUpdateData);
        this.PopulateRoutingCacheUpdateData(scopingCacheUpdateData, repositoryName);
        if (coreIndexingExecutionContext.IndexingUnit.IsLargeRepository(coreIndexingExecutionContext.RequestContext))
        {
          string collectionConfigValue = coreIndexingExecutionContext.GetCollectionConfigValue("/Service/ALMSearch/Settings/LargeRepositoriesName");
          string name = this.IndexingUnit.Properties.Name;
          LinkedList<string> values = new LinkedList<string>((IEnumerable<string>) collectionConfigValue.Trim().Split(','));
          if (values.Contains(repositoryName))
          {
            values.Remove(repositoryName);
            values.AddLast(name);
            coreIndexingExecutionContext.SetCollectionConfigValue<string>("/Service/ALMSearch/Settings/LargeRepositoriesName", string.Join(",", (IEnumerable<string>) values));
          }
        }
        ((CodeRepoTFSAttributes) this.IndexingUnit.TFSEntityAttributes).RepositoryName = this.IndexingUnit.Properties.Name;
        this.IndexingUnitDataAccess.UpdateIndexingUnit(coreIndexingExecutionContext.RequestContext, this.IndexingUnit);
        stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "Successfully completed GitRepoRenameFinalizeOperation for repo id {0}", (object) this.IndexingUnit.TFSEntityId);
        operationResult.Status = OperationStatus.Succeeded;
      }
      finally
      {
        operationResult.Message = stringBuilder.ToString();
        if (coreIndexingExecutionContext.IndexingUnit.EntityType.Name == "Code" && operationResult.Status == OperationStatus.Succeeded)
        {
          IEnumerable<Type> entitiesKnownTypes = coreIndexingExecutionContext.RequestContext.To(TeamFoundationHostType.Deployment).GetService<IEntityService>().GetEntitiesKnownTypes();
          SQLNotificationSenders.SendSqlNotification(coreIndexingExecutionContext.RequestContext, (object) scopingCacheUpdateData, Microsoft.VisualStudio.Services.Search.Common.SqlNotificationEventClasses.QueryScopingCacheInvalidated, "Search.Server.ScopedQuery", entitiesKnownTypes);
        }
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(1080641, "Indexing Pipeline", "IndexingOperation", nameof (RunOperation));
      }
      return operationResult;
    }

    private void UpdateProjectNameInSqlChangeEventData(
      IndexingExecutionContext executionContext,
      QueryScopingCacheUpdateData updateData)
    {
      ProjectCodeTFSAttributes entityAttributes = (ProjectCodeTFSAttributes) this.IndexingUnitDataAccess.GetIndexingUnit(executionContext.RequestContext, this.IndexingUnit.ParentUnitId)?.TFSEntityAttributes;
      if (entityAttributes == null || updateData.ParentHierarchy != null)
        return;
      updateData.ParentHierarchy = (IDictionary<string, string>) new Dictionary<string, string>();
      updateData.ParentHierarchy.Add("ProjectFilters", entityAttributes.ProjectName);
    }

    internal override void PopulateRoutingCacheUpdateData(
      QueryScopingCacheUpdateData queryScopingCacheUpdateData,
      string oldEntityName)
    {
      queryScopingCacheUpdateData.EventType = QueryScopingCacheUpdateEvent.RepositoryRename;
      queryScopingCacheUpdateData.IndexingUnitType = "Git_Repository";
      base.PopulateRoutingCacheUpdateData(queryScopingCacheUpdateData, oldEntityName);
    }
  }
}
