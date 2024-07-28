// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.ProjectUpdateFinalizeOperation
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

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations
{
  internal class ProjectUpdateFinalizeOperation : AbstractIndexingOperation
  {
    public ProjectUpdateFinalizeOperation(
      ExecutionContext executionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent)
      : base(executionContext, indexingUnit, indexingUnitChangeEvent)
    {
    }

    public override OperationResult RunOperation(
      CoreIndexingExecutionContext coreIndexingExecutionContext)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(1080640, "Indexing Pipeline", "IndexingOperation", nameof (RunOperation));
      OperationResult operationResult = new OperationResult();
      StringBuilder stringBuilder = new StringBuilder();
      QueryScopingCacheUpdateData scopingCacheUpdateData = new QueryScopingCacheUpdateData();
      IndexingExecutionContext iexContext = (IndexingExecutionContext) coreIndexingExecutionContext;
      try
      {
        if (!this.ValidateCIFeatureFlags(iexContext))
        {
          operationResult.Status = OperationStatus.Succeeded;
          return operationResult;
        }
        string projectName = ((ProjectCodeTFSAttributes) this.IndexingUnit.TFSEntityAttributes).ProjectName;
        this.PopulateRoutingCacheUpdateData(scopingCacheUpdateData, projectName);
        this.FinalizeTfsAttributes();
        this.IndexingUnitDataAccess.UpdateIndexingUnit(coreIndexingExecutionContext.RequestContext, this.IndexingUnit);
        stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "Successfully completed ProjectRenameFinalizeOperation for project id {0}", (object) this.IndexingUnit.TFSEntityId);
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
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(1080640, "Indexing Pipeline", "IndexingOperation", nameof (RunOperation));
      }
      return operationResult;
    }

    internal virtual void FinalizeTfsAttributes() => ((ProjectCodeTFSAttributes) this.IndexingUnit.TFSEntityAttributes).ProjectName = this.IndexingUnit.Properties.Name;

    internal override void PopulateRoutingCacheUpdateData(
      QueryScopingCacheUpdateData queryScopingCacheUpdateData,
      string oldEntityName)
    {
      queryScopingCacheUpdateData.EventType = QueryScopingCacheUpdateEvent.ProjectRename;
      queryScopingCacheUpdateData.IndexingUnitType = "Project";
      base.PopulateRoutingCacheUpdateData(queryScopingCacheUpdateData, oldEntityName);
    }
  }
}
