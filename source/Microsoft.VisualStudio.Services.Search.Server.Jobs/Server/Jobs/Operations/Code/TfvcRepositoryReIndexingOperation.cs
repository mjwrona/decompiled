// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.Code.TfvcRepositoryReIndexingOperation
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Common.HttpClientWrappers;
using Microsoft.VisualStudio.Services.Search.Indexer;
using Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer;
using Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions;
using Microsoft.VisualStudio.Services.Search.Server.Pipeline;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.Code
{
  internal class TfvcRepositoryReIndexingOperation : RepositoryDeleteOperation
  {
    [StaticSafe]
    private static TraceMetaData s_traceMetaData = new TraceMetaData(1080637, "Indexing Pipeline", "IndexingOperation");
    private ProjectHttpClientWrapper m_tfvcClientWrapper;
    private CoreIndexingExecutionContext m_executionContext;

    public TfvcRepositoryReIndexingOperation(
      CoreIndexingExecutionContext executionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent)
      : base(executionContext, indexingUnit, indexingUnitChangeEvent)
    {
      this.m_tfvcClientWrapper = new ProjectHttpClientWrapper((ExecutionContext) executionContext, TfvcRepositoryReIndexingOperation.s_traceMetaData);
      this.m_executionContext = executionContext;
    }

    [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
    internal TfvcRepositoryReIndexingOperation(
      IndexingExecutionContext executionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent,
      ProjectHttpClientWrapper clientWrapper,
      IIndexingUnitDataAccess indexingUnitDataAccess)
      : base((CoreIndexingExecutionContext) executionContext, indexingUnit, indexingUnitChangeEvent)
    {
      this.m_tfvcClientWrapper = clientWrapper;
      this.m_executionContext = (CoreIndexingExecutionContext) executionContext;
      this.IndexingUnitDataAccess = indexingUnitDataAccess;
    }

    public override OperationResult RunOperation(
      CoreIndexingExecutionContext coreIndexingExecutionContext)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(1080625, "Indexing Pipeline", "IndexingOperation", nameof (RunOperation));
      OperationResult operationResult = new OperationResult();
      StringBuilder resultMessage = new StringBuilder();
      IndexingExecutionContext iexContext = (IndexingExecutionContext) coreIndexingExecutionContext;
      try
      {
        if (!this.ValidateCIFeatureFlags(iexContext))
        {
          operationResult.Status = OperationStatus.Succeeded;
          return operationResult;
        }
        operationResult = this.RunBaseOperation(coreIndexingExecutionContext, resultMessage);
        if (operationResult.Status == OperationStatus.Succeeded)
        {
          if (this.GetInternalOperationStatus() == OperationStatus.Succeeded)
            this.CreateBulkIndexingEvent(this.GetOrCreateIndexingUnit(resultMessage), resultMessage);
        }
      }
      finally
      {
        operationResult.Message = resultMessage.ToString();
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(1080625, "Indexing Pipeline", "IndexingOperation", nameof (RunOperation));
      }
      return operationResult;
    }

    internal virtual OperationStatus GetInternalOperationStatus() => this.m_innerOperationStatus;

    internal virtual OperationResult RunBaseOperation(
      CoreIndexingExecutionContext indexingExecutionContext,
      StringBuilder resultMessage)
    {
      OperationResult operationResult = base.RunOperation(indexingExecutionContext);
      resultMessage.AppendLine(operationResult.Message);
      return operationResult;
    }

    internal virtual Microsoft.VisualStudio.Services.Search.Common.IndexingUnit GetOrCreateIndexingUnit(
      StringBuilder resultMessage)
    {
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit = this.IndexingUnitDataAccess.GetIndexingUnit(this.m_executionContext.RequestContext, this.IndexingUnit.TFSEntityId, "TFVC_Repository", (IEntityType) CodeEntityType.GetInstance());
      if (indexingUnit == null)
      {
        indexingUnit = this.IndexingUnitDataAccess.AddOrUpdateIndexingUnits(this.m_executionContext.RequestContext, new List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>()
        {
          this.m_tfvcClientWrapper.GetTeamProjectWithCapabilities(this.IndexingUnit.TFSEntityId.ToString()).ToTfvcRepoCodeIndexingUnit(this.IndexingUnit.ParentUnitId)
        }, true).First<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>();
        resultMessage.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Created IndexingUnit with IndexingUnitId - {0} for Repository {1}", (object) indexingUnit.IndexingUnitId, (object) indexingUnit.TFSEntityId));
      }
      return indexingUnit;
    }

    internal virtual Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent CreateBulkIndexingEvent(
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit tfvcRepoIndexingUnit,
      StringBuilder resultMessage)
    {
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent bulkIndexingEvent = this.IndexingUnitChangeEventHandler.HandleEvent((ExecutionContext) this.m_executionContext, new Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent(this.IndexingUnitChangeEvent.LeaseId)
      {
        IndexingUnitId = tfvcRepoIndexingUnit.IndexingUnitId,
        ChangeData = (ChangeEventData) new CodeBulkIndexEventData((ExecutionContext) this.m_executionContext),
        ChangeType = "BeginBulkIndex",
        State = IndexingUnitChangeEventState.Pending,
        AttemptCount = (byte) 0
      });
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(TfvcRepositoryReIndexingOperation.s_traceMetaData, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "TfvcRepositoryReIndexingOperation created BulkIndex event [Id = {0} Repository Id = {1}]", (object) bulkIndexingEvent.Id, (object) tfvcRepoIndexingUnit.TFSEntityId));
      resultMessage.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Created BulkIndex event [Id = {0} Repository Id = {1}]", (object) bulkIndexingEvent.Id, (object) tfvcRepoIndexingUnit.TFSEntityId));
      return bulkIndexingEvent;
    }

    protected internal override void HandleOperationFailure(
      IndexingExecutionContext indexingExecutionContext,
      OperationResult result,
      Exception e)
    {
      result.Message = e.ToString();
      result.Status = OperationStatus.Failed;
    }
  }
}
