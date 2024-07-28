// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.ProjectDeleteOperation
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
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations
{
  internal class ProjectDeleteOperation : IndexingUnitDeleteOperation
  {
    public ProjectDeleteOperation(
      ExecutionContext executionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent)
      : base(executionContext, indexingUnit, indexingUnitChangeEvent)
    {
    }

    public override OperationResult RunOperation(
      CoreIndexingExecutionContext coreIndexingExecutionContext)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(1080690, "Indexing Pipeline", "IndexingOperation", nameof (RunOperation));
      OperationResult operationResult = new OperationResult();
      QueryScopingCacheUpdateData scopingCacheUpdateData = new QueryScopingCacheUpdateData();
      IndexingExecutionContext iexContext = (IndexingExecutionContext) coreIndexingExecutionContext;
      try
      {
        if (!this.ValidateCIFeatureFlags(iexContext))
        {
          operationResult.Status = OperationStatus.Succeeded;
          return operationResult;
        }
        operationResult.Status = OperationStatus.Succeeded;
        string projectName = ((ProjectCodeTFSAttributes) this.IndexingUnit.TFSEntityAttributes).ProjectName;
        this.PopulateRoutingCacheUpdateData(scopingCacheUpdateData, projectName);
        operationResult = base.RunOperation(coreIndexingExecutionContext);
        operationResult.Message = FormattableString.Invariant(FormattableStringFactory.Create("ProjectDeletionOperation successful for {0}", (object) this.IndexingUnit));
      }
      finally
      {
        if (coreIndexingExecutionContext.IndexingUnit.EntityType.Name == "Code" && operationResult.Status == OperationStatus.Succeeded)
        {
          IEnumerable<Type> entitiesKnownTypes = coreIndexingExecutionContext.RequestContext.To(TeamFoundationHostType.Deployment).GetService<IEntityService>().GetEntitiesKnownTypes();
          SQLNotificationSenders.SendSqlNotification(coreIndexingExecutionContext.RequestContext, (object) scopingCacheUpdateData, Microsoft.VisualStudio.Services.Search.Common.SqlNotificationEventClasses.QueryScopingCacheInvalidated, "Search.Server.ScopedQuery", entitiesKnownTypes);
        }
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(1080690, "Indexing Pipeline", "IndexingOperation", nameof (RunOperation));
      }
      return operationResult;
    }

    internal override void PopulateRoutingCacheUpdateData(
      QueryScopingCacheUpdateData queryScopingCacheUpdateData,
      string oldEntityName)
    {
      queryScopingCacheUpdateData.EventType = QueryScopingCacheUpdateEvent.ProjectDeletion;
      queryScopingCacheUpdateData.IndexingUnitType = "Project";
      base.PopulateRoutingCacheUpdateData(queryScopingCacheUpdateData, oldEntityName);
    }
  }
}
