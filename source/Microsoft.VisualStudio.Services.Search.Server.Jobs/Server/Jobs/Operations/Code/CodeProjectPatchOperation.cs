// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.Code.CodeProjectPatchOperation
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer;
using Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions;
using Microsoft.VisualStudio.Services.Search.Server.Pipeline;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.Code
{
  internal class CodeProjectPatchOperation : AbstractIndexingPatchOperation
  {
    [StaticSafe]
    private static readonly TraceMetaData s_traceMetadata = new TraceMetaData(1080694, "Indexing Pipeline", "IndexingOperation");

    public CodeProjectPatchOperation(
      ExecutionContext executionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent)
      : this(executionContext, indexingUnit, indexingUnitChangeEvent, Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer.DataAccessFactory.GetInstance())
    {
    }

    internal CodeProjectPatchOperation(
      ExecutionContext executionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent,
      IDataAccessFactory dataAccessFactory)
      : base(executionContext, indexingUnit, indexingUnitChangeEvent, dataAccessFactory)
    {
    }

    public override OperationResult RunOperation(CoreIndexingExecutionContext executionContext)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(CodeProjectPatchOperation.s_traceMetadata, nameof (RunOperation));
      OperationResult operationResult = new OperationResult();
      StringBuilder resultMessageBuilder = new StringBuilder();
      try
      {
        int configValueOrDefault1 = executionContext.RequestContext.GetConfigValueOrDefault("/Service/ALMSearch/Settings/CodePatchOperationQueueBatchSize", 2);
        int configValueOrDefault2 = executionContext.RequestContext.GetConfigValueOrDefault("/Service/ALMSearch/Settings/CodePatchOperationQueueDelayInSeconds", 1);
        this.CreatePatchEventsForChildIndexingUnits((ExecutionContext) executionContext, configValueOrDefault1, configValueOrDefault2, resultMessageBuilder);
        operationResult.Status = OperationStatus.Succeeded;
      }
      finally
      {
        operationResult.Message = resultMessageBuilder.ToString();
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(CodeProjectPatchOperation.s_traceMetadata, nameof (RunOperation));
      }
      return operationResult;
    }

    internal override List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> GetChildIndexingUnits(
      ExecutionContext executionContext)
    {
      return this.IndexingUnitDataAccess.GetIndexingUnitsWithGivenParent(executionContext.RequestContext, this.IndexingUnit.IndexingUnitId, -1)?.FindAll((Predicate<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>) (x => x.IndexingUnitType == "Git_Repository"));
    }
  }
}
