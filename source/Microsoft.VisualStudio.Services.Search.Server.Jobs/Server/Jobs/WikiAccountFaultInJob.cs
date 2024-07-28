// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.WikiAccountFaultInJob
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
  public class WikiAccountFaultInJob : AbstractAccountFaultInJob
  {
    public WikiAccountFaultInJob()
      : this(Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer.DataAccessFactory.GetInstance(), (IIndexingUnitChangeEventHandler) new Microsoft.VisualStudio.Services.Search.Server.EventHandler.IndexingUnitChangeEventHandler())
    {
    }

    public WikiAccountFaultInJob(
      IDataAccessFactory dataAccessFactory,
      IIndexingUnitChangeEventHandler indexingUnitChangeEventHandler)
      : base(dataAccessFactory, indexingUnitChangeEventHandler, (IEntityType) WikiEntityType.GetInstance(), JobConstants.WikiAccountFaultInJobId)
    {
    }

    protected override void AddMetadataCrawlOperation(
      Microsoft.VisualStudio.Services.Search.Common.ExecutionContext executionContext,
      int indexingUnitId,
      IndexingUnitChangeEventPrerequisites metadataCrawlPrerequisites)
    {
      IndexingUnitChangeEvent indexingUnitChangeEvent1 = new IndexingUnitChangeEvent();
      indexingUnitChangeEvent1.IndexingUnitId = indexingUnitId;
      WikiMetadataCrawlEventData metadataCrawlEventData = new WikiMetadataCrawlEventData(executionContext);
      metadataCrawlEventData.Trigger = this.Trigger;
      metadataCrawlEventData.Finalize = true;
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
      Microsoft.VisualStudio.Services.Search.Common.ExecutionContext executionContext,
      IndexingUnit collectionIndexingUnit,
      bool assignNewIndex)
    {
      if (!executionContext.RequestContext.GetCurrentHostConfigValue<bool>("/service/ALMSearch/Settings/Routing/WikiDocCountBasedIndexProvisioningEnabled", true))
        return this.ProvisionIndexForAccount(executionContext, collectionIndexingUnit, assignNewIndex);
      this.ResultMessage.Append("SizeBasedIndexProvisioningEnabled is enabled, account fault-in job will not provision the index.");
      return (IndexIdentity) null;
    }

    protected internal override bool PreRun(
      Microsoft.VisualStudio.Services.Search.Common.ExecutionContext executionContext,
      out TeamFoundationJobExecutionResult jobResult)
    {
      jobResult = TeamFoundationJobExecutionResult.Succeeded;
      if (executionContext.RequestContext.IsWikiIndexingEnabled())
        return true;
      ITeamFoundationTaskService service = executionContext.RequestContext.To(TeamFoundationHostType.Deployment).GetService<ITeamFoundationTaskService>();
      bool wikiIndexingEnabled = false;
      TimeSpan timeSpan = TimeSpan.FromSeconds((double) executionContext.GetConfigValue<int>("/Service/ALMSearch/Settings/WorkItemIndexingFeatureFlagStatusPollTimeInSeconds"));
      Stopwatch stopwatch = Stopwatch.StartNew();
      do
      {
        service.AddTask(executionContext.RequestContext, (TeamFoundationTaskCallback) ((requestContext, ignored) =>
        {
          if (!requestContext.IsWikiIndexingEnabled())
            return;
          wikiIndexingEnabled = true;
        }));
        Thread.Sleep(TimeSpan.FromSeconds((double) executionContext.GetConfigValue<int>("/Service/ALMSearch/Settings/WorkItemIndexingFeatureFlagStatusSleepTimeInSeconds", 1)));
        if (wikiIndexingEnabled)
          return true;
      }
      while (stopwatch.Elapsed < timeSpan);
      return false;
    }
  }
}
