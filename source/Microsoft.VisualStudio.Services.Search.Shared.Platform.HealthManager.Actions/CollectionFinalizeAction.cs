// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.Actions.CollectionFinalizeAction
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.Actions, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DE7D0F19-C193-43CC-9602-3C8794FE9CA0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.Actions.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Indexer;
using Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer;
using Microsoft.VisualStudio.Services.Search.Server.EventHandler;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManagerAPI;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using Microsoft.VisualStudio.Services.WebPlatform;
using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.Actions
{
  public class CollectionFinalizeAction : AbstractAction
  {
    public CollectionFinalizeAction(ActionContext actionContext)
      : base(ActionType.CollectionFinalize, actionContext)
    {
    }

    public CollectionFinalizeAction(
      IDataAccessFactory dataAccessFactory,
      ActionContext actionContext)
      : base(dataAccessFactory, ActionType.CollectionFinalize, actionContext)
    {
    }

    internal Microsoft.VisualStudio.Services.Search.Common.IndexingUnit IndexingUnit { get; set; }

    internal IIndexingUnitChangeEventHandler IndexingUnitChangeEventHandler { get; set; }

    public override bool IsLongRunning() => false;

    public override bool IsCompleted(IVssRequestContext requestContext) => true;

    public override void Invoke(IVssRequestContext requestContext, out string resultMessage)
    {
      StringBuilder infoMessage = new StringBuilder();
      infoMessage.Append(FormattableString.Invariant(FormattableStringFactory.Create("Invoking Action : {0} , Scenario: {1}.", (object) nameof (CollectionFinalizeAction), (object) this.ActionContext.Scenario)));
      try
      {
        if (!this.ValidateInput(requestContext))
          return;
        ITracerCICorrelationDetails correlationDetails = TracerCICorrelationDetailsFactory.GetCICorrelationDetails(requestContext, "CompleBulkIndexEvent", 26);
        ExecutionContext executionContext = requestContext.GetExecutionContext(correlationDetails);
        this.Initialize(executionContext);
        this.CreateCollectionIndexFinalizeOperation(executionContext, this.ActionContext, infoMessage);
      }
      finally
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishClientTraceMessage("Health Manager", "HealthManagerAction", Level.Info, infoMessage.ToString());
        resultMessage = infoMessage.ToString();
        infoMessage.Clear();
      }
    }

    internal virtual bool ValidateInput(IVssRequestContext requestContext)
    {
      bool flag = true;
      if (!requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
        flag = false;
      return flag;
    }

    internal virtual void Initialize(ExecutionContext executionContext)
    {
      this.IndexingUnit = this.RetrieveIndexingUnit(executionContext);
      this.IndexingUnitChangeEventHandler = (IIndexingUnitChangeEventHandler) new Microsoft.VisualStudio.Services.Search.Server.EventHandler.IndexingUnitChangeEventHandler();
    }

    internal virtual Microsoft.VisualStudio.Services.Search.Common.IndexingUnit RetrieveIndexingUnit(
      ExecutionContext executionContext)
    {
      return this.DataAccessFactory.GetIndexingUnitDataAccess().GetIndexingUnit(executionContext.RequestContext, executionContext.RequestContext.GetCollectionID(), "Collection", this.ActionContext.EntityType);
    }

    internal virtual Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent CreateCollectionIndexFinalizeOperation(
      ExecutionContext executionContext,
      ActionContext actionContext,
      StringBuilder infoMessage)
    {
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent = new Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent()
      {
        IndexingUnitId = this.IndexingUnit.IndexingUnitId,
        ChangeData = this.GetChangeEventData(executionContext),
        ChangeType = "CompleteBulkIndex",
        State = IndexingUnitChangeEventState.Pending,
        AttemptCount = 0
      };
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent finalizeOperation = this.IndexingUnitChangeEventHandler.HandleEvent(executionContext, indexingUnitChangeEvent);
      infoMessage.Append(FormattableString.Invariant(FormattableStringFactory.Create("Added {0} change event to complete bulk indexing of {1}", (object) finalizeOperation, (object) this.IndexingUnit)));
      return finalizeOperation;
    }

    private ChangeEventData GetChangeEventData(ExecutionContext executionContext)
    {
      if (this.ActionContext.EntityType.Name == "Code")
        return (ChangeEventData) new CodeIndexPublishData(executionContext);
      throw new NotSupportedException(FormattableString.Invariant(FormattableStringFactory.Create("Entity type [{0}] is not supported.", (object) this.ActionContext.EntityType.Name)));
    }
  }
}
