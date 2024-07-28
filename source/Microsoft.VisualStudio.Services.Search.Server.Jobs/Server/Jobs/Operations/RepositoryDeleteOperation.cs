// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.RepositoryDeleteOperation
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Indexer;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions;
using Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer;
using Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions;
using Microsoft.VisualStudio.Services.Search.Server.Pipeline;
using Microsoft.VisualStudio.Services.Search.V2Jobs.Common;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations
{
  internal class RepositoryDeleteOperation : IndexingUnitDeleteOperation
  {
    private IndexOperations m_indexOperations;
    protected OperationStatus m_innerOperationStatus;

    public RepositoryDeleteOperation(
      CoreIndexingExecutionContext executionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent)
      : base((ExecutionContext) executionContext, indexingUnit, indexingUnitChangeEvent)
    {
      this.m_indexOperations = new IndexOperations();
    }

    public RepositoryDeleteOperation(
      CoreIndexingExecutionContext executionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent,
      IDataAccessFactory dataAccessFactory,
      IndexOperations indexOperations)
      : base((ExecutionContext) executionContext, indexingUnit, indexingUnitChangeEvent, dataAccessFactory)
    {
      this.m_indexOperations = indexOperations;
    }

    public override OperationResult RunOperation(
      CoreIndexingExecutionContext coreIndexingExecutionContext)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(1080692, "Indexing Pipeline", "IndexingOperation", nameof (RunOperation));
      OperationResult operationResult = new OperationResult();
      StringBuilder stringBuilder1 = new StringBuilder();
      bool flag1 = false;
      IndexingExecutionContext executionContext = (IndexingExecutionContext) coreIndexingExecutionContext;
      try
      {
        if (!this.ValidateCIFeatureFlags(executionContext))
        {
          operationResult.Status = OperationStatus.Succeeded;
          return operationResult;
        }
        operationResult.Status = OperationStatus.Succeeded;
        bool flag2 = true;
        if (coreIndexingExecutionContext.RequestContext.ExecutionEnvironment.IsOnPremisesDeployment)
        {
          IVssRequestContext vssRequestContext = coreIndexingExecutionContext.RequestContext.To(TeamFoundationHostType.Deployment);
          if (string.IsNullOrEmpty(vssRequestContext.GetService<IVssRegistryService>().GetValue<string>(vssRequestContext, (RegistryQuery) "/Service/ALMSearch/Settings/ATSearchPlatformConnectionString", false, (string) null)))
            flag2 = false;
        }
        if (flag2)
        {
          operationResult.Status = OperationStatus.Succeeded;
          IndexOperationsResponse operationsResponse = this.PerformBulkDelete(executionContext);
          if (operationsResponse != null)
          {
            if (!operationsResponse.Success)
              throw new SearchPlatformException(FormattableString.Invariant(FormattableStringFactory.Create("{0} PerformBulkDelete Failed.", (object) this.GetType().Name)));
            if (operationsResponse.IsOperationIncomplete)
            {
              this.RequeueOperation(executionContext);
              stringBuilder1.Append(FormattableString.Invariant(FormattableStringFactory.Create("Partially deleted documents. Re-queued the operation for {0}. ", (object) this.IndexingUnit)));
              operationResult.Status = OperationStatus.PartiallySucceeded;
            }
            else
            {
              stringBuilder1.Append(FormattableString.Invariant(FormattableStringFactory.Create("Successfully deleted documents for Repo {0} ", (object) this.IndexingUnit)));
              base.RunOperation(coreIndexingExecutionContext);
              flag1 = true;
            }
          }
          else
          {
            operationResult.Status = OperationStatus.Failed;
            stringBuilder1.Append(FormattableString.Invariant(FormattableStringFactory.Create("RepositoryDeleteOperation for Repo {0} failed due to null IndexOperationsResponse. ", (object) this.IndexingUnit)));
          }
        }
        else
          stringBuilder1.Append(FormattableString.Invariant(FormattableStringFactory.Create("RepositoryDeleteOperation for Repo {0} failed because Search is not configured. ", (object) this.IndexingUnit)));
      }
      catch (Exception ex)
      {
        if (coreIndexingExecutionContext.RequestContext.ExecutionEnvironment.IsOnPremisesDeployment && (int) this.IndexingUnitChangeEvent.AttemptCount >= this.m_maxIndexRetryCount || this.IsCollectionIndexResetTriggerForOnPremises(coreIndexingExecutionContext.RequestContext))
        {
          stringBuilder1.Append(FormattableString.Invariant(FormattableStringFactory.Create("RepositoryDeleteOperation for Repo {0} failed. Going ahead with Indexing Unit delete. ", (object) this.IndexingUnit)));
          base.RunOperation(coreIndexingExecutionContext);
          flag1 = true;
          operationResult.Status = OperationStatus.Failed;
        }
        ExceptionDispatchInfo.Capture(ex).Throw();
      }
      finally
      {
        if (coreIndexingExecutionContext.RequestContext.ExecutionEnvironment.IsOnPremisesDeployment & flag1)
        {
          ITeamFoundationJobService service = coreIndexingExecutionContext.RequestContext.GetService<ITeamFoundationJobService>();
          Guid? associatedJobId = this.IndexingUnit.AssociatedJobId;
          if (associatedJobId.HasValue)
          {
            ITeamFoundationJobService jobService = service;
            IVssRequestContext requestContext = coreIndexingExecutionContext.RequestContext;
            Guid[] jobsToDelete = new Guid[1];
            associatedJobId = this.IndexingUnit.AssociatedJobId;
            jobsToDelete[0] = associatedJobId.Value;
            jobService.DeleteJobDefinitions(requestContext, (IEnumerable<Guid>) jobsToDelete);
            StringBuilder stringBuilder2 = stringBuilder1;
            object[] objArray = new object[2];
            associatedJobId = this.IndexingUnit.AssociatedJobId;
            objArray[0] = (object) associatedJobId.Value;
            objArray[1] = (object) this.IndexingUnit;
            string str = FormattableString.Invariant(FormattableStringFactory.Create("Deleted JobDefinition for JobId {0} associated with IndexingUnit: {1} ", objArray));
            stringBuilder2.Append(str);
          }
        }
        this.m_innerOperationStatus = operationResult.Status;
        if (operationResult.Status == OperationStatus.PartiallySucceeded)
          operationResult.Status = OperationStatus.Succeeded;
        if (coreIndexingExecutionContext.IndexingUnit.EntityType.Name == "Code" & flag1)
          CodeQueryScopingCacheUtil.SqlNotifyForRepoDeletion(executionContext, this.DataAccessFactory);
        operationResult.Message = stringBuilder1.ToString();
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(1080692, "Indexing Pipeline", "IndexingOperation", nameof (RunOperation));
      }
      return operationResult;
    }

    protected override int GetMaxIndexingRetryCount(ExecutionContext executionContext) => this.IsCollectionIndexResetTriggerForOnPremises(executionContext.RequestContext) ? 1 : base.GetMaxIndexingRetryCount(executionContext);

    protected internal override TimeSpan GetChangeEventDelay(
      CoreIndexingExecutionContext executionContext,
      Exception e)
    {
      return this.IsCollectionIndexResetTriggerForOnPremises(executionContext.RequestContext) ? new TimeSpan(0L) : base.GetChangeEventDelay(executionContext, e);
    }

    internal virtual IndexOperationsResponse PerformBulkDelete(
      IndexingExecutionContext indexingExecutionContext)
    {
      string normalizedString = this.IndexingUnit.GetTfsEntityIdAsNormalizedString();
      if (this.IndexingUnit.IsShadow && this.IndexingUnit.GetIndexInfo() == null)
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceError(1080692, "Indexing Pipeline", "IndexingOperation", FormattableString.Invariant(FormattableStringFactory.Create("Not proceeding with PerformBulkDelete for shadow IU {0} as IndexInfo is null.", (object) this.IndexingUnit.TFSEntityId)));
        return new IndexOperationsResponse()
        {
          Success = true
        };
      }
      IExpression queryExpression = new IndexOperationsMetadata()
      {
        RepositoryId = normalizedString
      }.GetQueryExpression();
      IndexIdentity index = indexingExecutionContext.GetIndex();
      if (index == null || string.IsNullOrWhiteSpace(index.Name))
        return new IndexOperationsResponse()
        {
          IsOperationIncomplete = true
        };
      DocumentContractType contractType = indexingExecutionContext.ProvisioningContext.ContractType;
      return contractType.IsDedupeFileContract() || contractType.IsSourceNoDedupeFileContract() || DocumentContractType.WikiContract == contractType ? this.m_indexOperations.PerformBulkDelete(indexingExecutionContext, indexingExecutionContext.GetIndex(), queryExpression, leniant: true) : (IndexOperationsResponse) null;
    }

    private Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent RequeueOperation(
      IndexingExecutionContext executionContext)
    {
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent = new Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent(this.IndexingUnitChangeEvent.LeaseId)
      {
        IndexingUnitId = this.IndexingUnit.IndexingUnitId,
        ChangeType = this.IndexingUnitChangeEvent.ChangeType,
        ChangeData = this.IndexingUnitChangeEvent.ChangeData,
        State = IndexingUnitChangeEventState.Pending,
        AttemptCount = 0
      };
      return this.IndexingUnitChangeEventHandler.HandleEvent((ExecutionContext) executionContext, indexingUnitChangeEvent);
    }

    private bool IsCollectionIndexResetTriggerForOnPremises(IVssRequestContext requestContext)
    {
      if (!requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
        return false;
      return this.IndexingUnitChangeEvent.ChangeData.Trigger == 9 || this.IndexingUnitChangeEvent.ChangeData.Trigger == 1;
    }
  }
}
