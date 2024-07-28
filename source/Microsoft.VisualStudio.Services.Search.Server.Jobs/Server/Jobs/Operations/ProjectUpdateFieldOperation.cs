// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.ProjectUpdateFieldOperation
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities.EntityProperties;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Common.HttpClientWrappers;
using Microsoft.VisualStudio.Services.Search.Indexer;
using Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions;
using Microsoft.VisualStudio.Services.Search.Server.Pipeline;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations
{
  internal class ProjectUpdateFieldOperation : AbstractIndexingOperation
  {
    public ProjectUpdateFieldOperation(
      ExecutionContext executionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent)
      : base(executionContext, indexingUnit, indexingUnitChangeEvent)
    {
    }

    public override OperationResult RunOperation(
      CoreIndexingExecutionContext coreIndexingExecutionContext)
    {
      Tracer.TraceEnter(1080620, "Indexing Pipeline", "IndexingOperation", nameof (RunOperation));
      OperationResult operationResult = new OperationResult();
      StringBuilder resultMessage = new StringBuilder();
      IndexingExecutionContext executionContext = (IndexingExecutionContext) coreIndexingExecutionContext;
      try
      {
        if (!this.ValidateCIFeatureFlags(executionContext))
        {
          operationResult.Status = OperationStatus.Succeeded;
          return operationResult;
        }
        if (this.UpdatePropertiesIfModified(executionContext))
        {
          this.IndexingUnitDataAccess.UpdateIndexingUnit(coreIndexingExecutionContext.RequestContext, this.IndexingUnit);
          this.QueueDependentOperations(executionContext, resultMessage);
        }
        else
          resultMessage.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "Project properties in IndexingUnit table TfsEntityAttributes column is same as that in TFS for project {0}.", (object) this.IndexingUnit.TFSEntityId);
        operationResult.Status = OperationStatus.Succeeded;
      }
      finally
      {
        operationResult.Message = resultMessage.ToString();
        Tracer.TraceLeave(1080620, "Indexing Pipeline", "IndexingOperation", nameof (RunOperation));
      }
      return operationResult;
    }

    protected virtual bool UpdatePropertiesIfModified(IndexingExecutionContext executionContext)
    {
      string projectNameFromTfs = this.GetProjectNameFromTFS(executionContext);
      string projectName = (this.IndexingUnit.TFSEntityAttributes as ProjectCodeTFSAttributes).ProjectName;
      int num = !projectNameFromTfs.Equals(projectName, StringComparison.Ordinal) ? 1 : (!projectName.Equals(this.IndexingUnit.Properties.Name, StringComparison.Ordinal) ? 1 : 0);
      if (num == 0)
        return num != 0;
      this.IndexingUnit.Properties.Name = projectNameFromTfs;
      return num != 0;
    }

    internal virtual void QueueDependentOperations(
      IndexingExecutionContext executionContext,
      StringBuilder resultMessage)
    {
      string str = "UpdateField";
      int indexingUnitId = this.IndexingUnit.IndexingUnitId;
      List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> unitsWithGivenParent = this.IndexingUnitDataAccess.GetIndexingUnitsWithGivenParent(executionContext.RequestContext, indexingUnitId, -1);
      IList<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent> indexingUnitChangeEventList = (IList<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent>) new List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent>();
      foreach (Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit in unitsWithGivenParent)
      {
        Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent = new Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent(this.IndexingUnitChangeEvent.LeaseId)
        {
          IndexingUnitId = indexingUnit.IndexingUnitId,
          ChangeType = str,
          ChangeData = (ChangeEventData) new EntityRenameEventData((ExecutionContext) executionContext)
          {
            FieldUpdateEventIndexingUnitType = "Project"
          },
          State = IndexingUnitChangeEventState.Pending,
          AttemptCount = 0
        };
        indexingUnitChangeEventList.Add(indexingUnitChangeEvent);
      }
      IList<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent> preReqIndexingEvents = this.IndexingUnitChangeEventHandler.HandleEvents((ExecutionContext) executionContext, indexingUnitChangeEventList);
      resultMessage.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "Successfully created UpdateField operation for {0} repositories because of project rename.", (object) unitsWithGivenParent.Count);
      this.QueueProjectRenameFinalizeOperation(executionContext, preReqIndexingEvents, resultMessage);
    }

    protected internal Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent QueueProjectRenameFinalizeOperation(
      IndexingExecutionContext executionContext,
      IList<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent> preReqIndexingEvents,
      StringBuilder resultMessage)
    {
      IndexingUnitChangeEventPrerequisites eventPrerequisites = new IndexingUnitChangeEventPrerequisites();
      if (preReqIndexingEvents != null && preReqIndexingEvents.Count > 0)
      {
        List<IndexingUnitChangeEventState> possibleStates = new List<IndexingUnitChangeEventState>()
        {
          IndexingUnitChangeEventState.Succeeded,
          IndexingUnitChangeEventState.Failed
        };
        eventPrerequisites.AddRange(preReqIndexingEvents.Select<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent, IndexingUnitChangeEventPrerequisitesFilter>((Func<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent, IndexingUnitChangeEventPrerequisitesFilter>) (indexingChangeEvent => new IndexingUnitChangeEventPrerequisitesFilter()
        {
          Id = indexingChangeEvent.Id,
          Operator = IndexingUnitChangeEventFilterOperator.Contains,
          PossibleStates = possibleStates
        })));
      }
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent1 = new Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent()
      {
        IndexingUnitId = this.IndexingUnit.IndexingUnitId,
        ChangeType = "CompleteEntityRename",
        ChangeData = new ChangeEventData((ExecutionContext) executionContext),
        State = IndexingUnitChangeEventState.Pending,
        AttemptCount = 0,
        Prerequisites = eventPrerequisites
      };
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent2 = this.IndexingUnitChangeEventHandler.HandleEvent((ExecutionContext) executionContext, indexingUnitChangeEvent1);
      resultMessage.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "Successfully created CompleteEntityrename for project Id {0}.", (object) this.IndexingUnit.TFSEntityId);
      return indexingUnitChangeEvent2;
    }

    internal virtual string GetProjectNameFromTFS(IndexingExecutionContext executionContext) => this.GetTeamProjectFromTfs(executionContext).Name;

    internal virtual TeamProject GetTeamProjectFromTfs(IndexingExecutionContext executionContext) => new ProjectHttpClientWrapper((ExecutionContext) executionContext, new TraceMetaData(1080620, "Indexing Pipeline", "ProjectRenameOperation")).GetTeamProjectWithCapabilities(this.IndexingUnit.TFSEntityId.ToString());
  }
}
