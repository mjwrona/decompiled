// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.Code.CodeProjectRenameOperation
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities.EntityProperties;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Indexer;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.Code
{
  internal class CodeProjectRenameOperation : ProjectUpdateFieldOperation
  {
    public CodeProjectRenameOperation(
      ExecutionContext executionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent)
      : base(executionContext, indexingUnit, indexingUnitChangeEvent)
    {
    }

    internal override void QueueDependentOperations(
      IndexingExecutionContext executionContext,
      StringBuilder resultMessage)
    {
      DocumentContractType contractType = executionContext.ProvisioningContext.ContractType;
      if (AbstractSearchDocumentContract.CreateContract(contractType).IsSourceEnabled(executionContext.RequestContext) && (contractType.IsDedupeFileContract() || contractType.IsSourceNoDedupeFileContract() && executionContext.RequestContext.GetConfigValue<bool>("/Service/ALMSearch/Settings/SourceEnabledInNoPayloadMapping", TeamFoundationHostType.Deployment, true)))
        this.CreateDependentOperationsWhenMappingSupportsUpdates(executionContext, resultMessage);
      else
        this.CreateDependantOperationsForProjectRename(executionContext, resultMessage);
    }

    internal void CreateDependantOperationsForProjectRename(
      IndexingExecutionContext executionContext,
      StringBuilder resultMessage)
    {
      int indexingUnitId = this.IndexingUnit.IndexingUnitId;
      List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> unitsWithGivenParent = this.IndexingUnitDataAccess.GetIndexingUnitsWithGivenParent(executionContext.RequestContext, indexingUnitId, -1);
      string oldProjectName = this.IndexingUnit.GetProjectNameFromTFSAttributesIfProjectIUElseNull() ?? throw new InvalidOperationException("ProjectName is not set in IndexingUnit.GetProjectNameFromTFSAttributesIfProjectIUElseNull()");
      List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent> preReqIndexingEvents = new List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent>();
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(1080620, "Indexing Pipeline", "IndexingOperation", FormattableString.Invariant(FormattableStringFactory.Create("Erasing Indexing watermarks for {0}", (object) this.IndexingUnit)));
      this.IndexingUnit.EraseIndexingWatermarksOfTree(executionContext, this.IndexingUnitDataAccess);
      if (unitsWithGivenParent != null && unitsWithGivenParent.Count > 0)
      {
        string name = this.IndexingUnit.Properties.Name;
        if (executionContext.RequestContext.ExecutionEnvironment.IsOnPremisesDeployment)
          preReqIndexingEvents.AddRange((IEnumerable<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent>) this.CreateRepoRenameOperationsForSameNameRepositories(unitsWithGivenParent, (ExecutionContext) executionContext, oldProjectName, name));
        IList<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent> list = (IList<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent>) unitsWithGivenParent.Select<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent>((Func<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent>) (repo => this.CreateBulkIndexEvent(executionContext, repo))).ToList<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent>();
        IList<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent> indexingUnitChangeEventList = this.IndexingUnitChangeEventHandler.HandleEvents((ExecutionContext) executionContext, list);
        preReqIndexingEvents.AddRange((IEnumerable<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent>) indexingUnitChangeEventList);
        resultMessage.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "Successfully created BulkIndex/BulkTreeCrawl operation for {0} repositories because of project rename.", (object) unitsWithGivenParent.Count);
        this.HandleOrphanFilesDeletion(executionContext, indexingUnitChangeEventList, oldProjectName);
        resultMessage.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "Successfully created Orphan files delete operation for project Id {0}.", (object) this.IndexingUnit.TFSEntityId);
      }
      this.QueueProjectRenameFinalizeOperation(executionContext, (IList<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent>) preReqIndexingEvents, resultMessage);
    }

    internal Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent CreateBulkIndexEvent(
      IndexingExecutionContext executionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit repo)
    {
      return new Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent(this.IndexingUnitChangeEvent.LeaseId)
      {
        IndexingUnitId = repo.IndexingUnitId,
        ChangeData = (ChangeEventData) new CodeBulkIndexEventData((ExecutionContext) executionContext),
        ChangeType = "BeginBulkIndex",
        State = IndexingUnitChangeEventState.Pending,
        AttemptCount = 0
      };
    }

    internal void HandleOrphanFilesDeletion(
      IndexingExecutionContext executionContext,
      IList<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent> repoBulkIndexingEvents,
      string oldProjectName)
    {
      if (oldProjectName == null)
        throw new ArgumentNullException(nameof (oldProjectName));
      if (!repoBulkIndexingEvents.Any<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent>())
        return;
      IList<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent> indexingUnitChangeEventList = (IList<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent>) new List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent>();
      foreach (Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent bulkIndexingEvent in (IEnumerable<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent>) repoBulkIndexingEvents)
      {
        IndexingUnitChangeEventPrerequisites eventPrerequisites = new IndexingUnitChangeEventPrerequisites();
        eventPrerequisites.Add(new IndexingUnitChangeEventPrerequisitesFilter()
        {
          Id = bulkIndexingEvent.Id,
          Operator = IndexingUnitChangeEventFilterOperator.Contains,
          PossibleStates = new List<IndexingUnitChangeEventState>()
          {
            IndexingUnitChangeEventState.Succeeded
          }
        });
        Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent = new Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent()
        {
          IndexingUnitId = bulkIndexingEvent.IndexingUnitId,
          ChangeType = "DeleteOrphanFiles",
          ChangeData = (ChangeEventData) new CodeDeleteOrphanFilesEventData((ExecutionContext) executionContext)
          {
            OldEntityName = oldProjectName,
            UnitType = "Project"
          },
          State = IndexingUnitChangeEventState.Pending,
          AttemptCount = 0,
          Prerequisites = eventPrerequisites
        };
        indexingUnitChangeEventList.Add(indexingUnitChangeEvent);
      }
      this.IndexingUnitChangeEventHandler.HandleEvents((ExecutionContext) executionContext, indexingUnitChangeEventList);
    }

    internal void CreateDependentOperationsWhenMappingSupportsUpdates(
      IndexingExecutionContext executionContext,
      StringBuilder resultMessage)
    {
      List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> unitsWithGivenParent = this.IndexingUnitDataAccess.GetIndexingUnitsWithGivenParent(executionContext.RequestContext, this.IndexingUnit.IndexingUnitId, -1);
      IList<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent> indexingUnitChangeEventList1 = (IList<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent>) new List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent>();
      foreach (Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit in unitsWithGivenParent)
      {
        if (indexingUnit.IndexingUnitType != "TFVC_Repository" || executionContext.ProvisioningContext.ContractType.IsNoPayloadContract())
        {
          indexingUnitChangeEventList1.Add(new Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent(this.IndexingUnitChangeEvent.LeaseId)
          {
            IndexingUnitId = indexingUnit.IndexingUnitId,
            ChangeType = "UpdateField",
            ChangeData = (ChangeEventData) new EntityRenameEventData((ExecutionContext) executionContext)
            {
              FieldUpdateEventIndexingUnitType = "Project"
            },
            State = IndexingUnitChangeEventState.Pending,
            AttemptCount = (byte) 0
          });
        }
        else
        {
          indexingUnit.EraseIndexingWatermarksOfTree(executionContext, this.IndexingUnitDataAccess);
          indexingUnitChangeEventList1.Add(this.CreateBulkIndexEvent(executionContext, indexingUnit));
        }
      }
      IList<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent> indexingUnitChangeEventList2 = this.IndexingUnitChangeEventHandler.HandleEvents((ExecutionContext) executionContext, indexingUnitChangeEventList1);
      this.HandleOrphanFilesDeletion(executionContext, (IList<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent>) indexingUnitChangeEventList2.Where<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent>((Func<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent, bool>) (it => it.ChangeType == "BeginBulkIndex")).ToList<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent>(), this.IndexingUnit.GetProjectNameFromTFSAttributesIfProjectIUElseNull());
      resultMessage.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "Successfully created UpdateField operation for {0} repositories because of project rename.", (object) unitsWithGivenParent.Count);
      this.QueueProjectRenameFinalizeOperation(executionContext, indexingUnitChangeEventList2, resultMessage);
    }

    internal virtual List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent> CreateRepoRenameOperationsForSameNameRepositories(
      List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> repositories,
      ExecutionContext executionContext,
      string oldProjectName,
      string newProjectName)
    {
      List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> repositoriesToBeRenamed = repositories.FindAll((Predicate<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>) (repo => (repo.TFSEntityAttributes as CodeRepoTFSAttributes).RepositoryName.Equals(oldProjectName)));
      List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent> source = new List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent>();
      if (repositoriesToBeRenamed.Any<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>())
      {
        repositories.RemoveAll((Predicate<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>) (repo => repositoriesToBeRenamed.Contains(repo)));
        foreach (Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit in repositoriesToBeRenamed)
          source.Add(new Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent()
          {
            IndexingUnitId = indexingUnit.IndexingUnitId,
            ChangeType = "BeginEntityRename",
            ChangeData = (ChangeEventData) new EntityRenameEventData(executionContext)
            {
              NewEntityName = newProjectName
            },
            State = IndexingUnitChangeEventState.Pending,
            AttemptCount = (byte) 0
          });
        if (source.Any<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent>())
          this.IndexingUnitChangeEventHandler.HandleEvents(executionContext, (IList<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent>) source);
      }
      return source;
    }
  }
}
