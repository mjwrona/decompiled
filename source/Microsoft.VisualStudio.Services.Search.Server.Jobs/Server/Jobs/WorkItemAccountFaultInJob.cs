// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.WorkItemAccountFaultInJob
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
using System;
using System.Diagnostics;
using System.Threading;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs
{
  public class WorkItemAccountFaultInJob : AbstractAccountFaultInJob
  {
    public WorkItemAccountFaultInJob()
      : this(Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer.DataAccessFactory.GetInstance(), (IIndexingUnitChangeEventHandler) new Microsoft.VisualStudio.Services.Search.Server.EventHandler.IndexingUnitChangeEventHandler())
    {
    }

    internal WorkItemAccountFaultInJob(
      IDataAccessFactory dataAccessFactory,
      IIndexingUnitChangeEventHandler indexingUnitChangeEventHandler)
      : base(dataAccessFactory, indexingUnitChangeEventHandler, (IEntityType) WorkItemEntityType.GetInstance(), JobConstants.WorkItemAccountFaultInJobId)
    {
    }

    protected override void AddMetadataCrawlOperation(
      Microsoft.VisualStudio.Services.Search.Common.ExecutionContext executionContext,
      int indexingUnitId,
      IndexingUnitChangeEventPrerequisites metadataCrawlPrerequisites)
    {
      IndexingUnitChangeEvent indexingUnitChangeEvent1 = new IndexingUnitChangeEvent();
      indexingUnitChangeEvent1.IndexingUnitId = indexingUnitId;
      WorkItemMetadataCrawlEventData metadataCrawlEventData = new WorkItemMetadataCrawlEventData(executionContext);
      metadataCrawlEventData.Trigger = this.Trigger;
      indexingUnitChangeEvent1.ChangeData = (ChangeEventData) metadataCrawlEventData;
      indexingUnitChangeEvent1.ChangeType = "CrawlMetadata";
      indexingUnitChangeEvent1.State = IndexingUnitChangeEventState.Pending;
      indexingUnitChangeEvent1.AttemptCount = (byte) 0;
      indexingUnitChangeEvent1.Prerequisites = metadataCrawlPrerequisites;
      IndexingUnitChangeEvent indexingUnitChangeEvent2 = indexingUnitChangeEvent1;
      this.IndexingUnitChangeEventHandler.HandleEvent(executionContext, indexingUnitChangeEvent2);
    }

    protected internal override IndexIdentity ProvisionIndex(
      Microsoft.VisualStudio.Services.Search.Common.ExecutionContext executionContext,
      IndexingUnit collectionIndexingUnit,
      bool assignNewIndex)
    {
      if (!executionContext.RequestContext.GetCurrentHostConfigValue<bool>("/service/ALMSearch/Settings/Routing/WorkItemDocCountBasedIndexProvisioningEnabled", true))
        return this.ProvisionIndexForAccount(executionContext, collectionIndexingUnit, assignNewIndex);
      this.ResultMessage.Append("Doc count based index provisioning is enabled, account fault-in job will not provision the index.");
      return (IndexIdentity) null;
    }

    protected internal override bool PreRun(
      Microsoft.VisualStudio.Services.Search.Common.ExecutionContext executionContext,
      out TeamFoundationJobExecutionResult jobResult)
    {
      jobResult = TeamFoundationJobExecutionResult.Succeeded;
      if (executionContext.RequestContext.IsWorkItemIndexingEnabled())
        return true;
      TeamFoundationTaskService service = executionContext.RequestContext.To(TeamFoundationHostType.Deployment).GetService<TeamFoundationTaskService>();
      bool workItemIndexingEnabled = false;
      TimeSpan timeSpan = TimeSpan.FromSeconds((double) executionContext.GetConfigValue<int>("/Service/ALMSearch/Settings/WorkItemIndexingFeatureFlagStatusPollTimeInSeconds"));
      Stopwatch stopwatch = Stopwatch.StartNew();
      do
      {
        service.AddTask(executionContext.RequestContext, (TeamFoundationTaskCallback) ((requestContext, ignored) =>
        {
          if (!requestContext.IsWorkItemIndexingEnabled())
            return;
          workItemIndexingEnabled = true;
        }));
        Thread.Sleep(TimeSpan.FromSeconds(1.0));
        if (workItemIndexingEnabled)
          return true;
      }
      while (stopwatch.Elapsed < timeSpan);
      return false;
    }

    protected internal override void UpdateFeatureFlagsIfNeeded(IVssRequestContext requestContext) => requestContext.SetCollectionConfigValue<bool>("/Service/ALMSearch/Settings/EnableIndexingIdentityFields", true);

    protected override void ValidateRequestContext(IVssRequestContext requestContext)
    {
      if (!requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
        throw new UnsupportedHostTypeException(requestContext.ServiceHost.HostType);
    }
  }
}
