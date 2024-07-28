// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.AccountFaultInJob
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Constants;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions;
using Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer;
using Microsoft.VisualStudio.Services.Search.Server.EventHandler;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs
{
  public class AccountFaultInJob : AbstractAccountFaultInJob
  {
    public AccountFaultInJob()
      : this(Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer.DataAccessFactory.GetInstance(), (IIndexingUnitChangeEventHandler) new Microsoft.VisualStudio.Services.Search.Server.EventHandler.IndexingUnitChangeEventHandler())
    {
    }

    internal AccountFaultInJob(
      IDataAccessFactory dataAccessFactory,
      IIndexingUnitChangeEventHandler indexingUnitChangeEventHandler)
      : base(dataAccessFactory, indexingUnitChangeEventHandler, (IEntityType) CodeEntityType.GetInstance(), JobConstants.AccountFaultInJobId)
    {
    }

    protected override void AddMetadataCrawlOperation(
      ExecutionContext executionContext,
      int indexingUnitId,
      IndexingUnitChangeEventPrerequisites metadataCrawlPrerequisites)
    {
      IndexingUnitChangeEvent indexingUnitChangeEvent1 = new IndexingUnitChangeEvent();
      indexingUnitChangeEvent1.IndexingUnitId = indexingUnitId;
      CodeMetadataCrawlEventData metadataCrawlEventData = new CodeMetadataCrawlEventData(executionContext);
      metadataCrawlEventData.TriggerBulkIndexing = true;
      metadataCrawlEventData.Trigger = this.Trigger;
      indexingUnitChangeEvent1.ChangeData = (ChangeEventData) metadataCrawlEventData;
      indexingUnitChangeEvent1.ChangeType = "CrawlMetadata";
      indexingUnitChangeEvent1.State = IndexingUnitChangeEventState.Pending;
      indexingUnitChangeEvent1.Prerequisites = metadataCrawlPrerequisites;
      indexingUnitChangeEvent1.AttemptCount = (byte) 0;
      IndexingUnitChangeEvent indexingUnitChangeEvent2 = indexingUnitChangeEvent1;
      this.IndexingUnitChangeEventHandler.HandleEvent(executionContext, indexingUnitChangeEvent2);
    }

    protected override void ValidateRequestContext(IVssRequestContext requestContext)
    {
      if (!requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
        throw new UnsupportedHostTypeException(requestContext.ServiceHost.HostType);
    }

    protected internal override IndexIdentity ProvisionIndex(
      ExecutionContext executionContext,
      IndexingUnit collectionIndexingUnit,
      bool assignNewIndex)
    {
      if (!executionContext.RequestContext.GetCurrentHostConfigValue<bool>("/Service/ALMSearch/Settings/Routing/SizeBasedIndexProvisioningEnabled", true))
        return base.ProvisionIndex(executionContext, collectionIndexingUnit, assignNewIndex);
      this.ResultMessage.Append("SizeBasedIndexProvisioningEnabled is enabled, account fault-in job will not provision the index.");
      return (IndexIdentity) null;
    }

    protected internal override bool PreRun(
      ExecutionContext executionContext,
      out TeamFoundationJobExecutionResult jobResult)
    {
      jobResult = TeamFoundationJobExecutionResult.Succeeded;
      return executionContext.RequestContext.IsCodeIndexingEnabled();
    }
  }
}
