// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.PackageAccountFaultInJob
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Constants;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Entities.EntityProperties;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions;
using Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer;
using Microsoft.VisualStudio.Services.Search.Server.EventHandler;
using System;
using System.Diagnostics;
using System.Threading;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs
{
  public class PackageAccountFaultInJob : AbstractAccountFaultInJob
  {
    public PackageAccountFaultInJob()
      : this(Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer.DataAccessFactory.GetInstance(), (IIndexingUnitChangeEventHandler) new Microsoft.VisualStudio.Services.Search.Server.EventHandler.IndexingUnitChangeEventHandler())
    {
    }

    internal PackageAccountFaultInJob(
      IDataAccessFactory dataAccessFactory,
      IIndexingUnitChangeEventHandler indexingUnitChangeEventHandler)
      : this(dataAccessFactory, indexingUnitChangeEventHandler, JobConstants.PackageAccountFaultInJobId)
    {
    }

    protected PackageAccountFaultInJob(
      IDataAccessFactory dataAccessFactory,
      IIndexingUnitChangeEventHandler indexingUnitChangeEventHandler,
      Guid jobId)
      : base(dataAccessFactory, indexingUnitChangeEventHandler, (IEntityType) PackageEntityType.GetInstance(), jobId)
    {
    }

    protected override void AddMetadataCrawlOperation(
      Microsoft.VisualStudio.Services.Search.Common.ExecutionContext executionContext,
      int indexingUnitId,
      IndexingUnitChangeEventPrerequisites metadataCrawlPrerequisites)
    {
      IndexingUnitChangeEvent indexingUnitChangeEvent1 = new IndexingUnitChangeEvent();
      indexingUnitChangeEvent1.IndexingUnitId = indexingUnitId;
      PackageBulkIndexEventData bulkIndexEventData = new PackageBulkIndexEventData(executionContext);
      bulkIndexEventData.Finalize = true;
      bulkIndexEventData.Trigger = this.Trigger;
      indexingUnitChangeEvent1.ChangeData = (ChangeEventData) bulkIndexEventData;
      indexingUnitChangeEvent1.ChangeType = "BeginBulkIndex";
      indexingUnitChangeEvent1.State = IndexingUnitChangeEventState.Pending;
      indexingUnitChangeEvent1.AttemptCount = (byte) 0;
      indexingUnitChangeEvent1.Prerequisites = metadataCrawlPrerequisites;
      IndexingUnitChangeEvent indexingUnitChangeEvent2 = indexingUnitChangeEvent1;
      this.IndexingUnitChangeEventHandler.HandleEvent(executionContext, indexingUnitChangeEvent2);
    }

    protected override void ValidateRequestContext(IVssRequestContext requestContext)
    {
      if (!requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
        throw new UnsupportedHostTypeException(requestContext.ServiceHost.HostType);
    }

    protected override IndexingUnit CreateHostIndexingUnit(
      Microsoft.VisualStudio.Services.Search.Common.ExecutionContext executionContext,
      IIndexingUnitDataAccess indexingUnitDataAccess,
      DocumentContractType indexContractType)
    {
      IndexingUnit indexingUnit1 = new IndexingUnit(executionContext.RequestContext.GetCollectionID(), "Collection", this.EntityType, -1);
      indexingUnit1.TFSEntityAttributes = (TFSEntityAttributes) new CollectionAttributes()
      {
        CollectionName = executionContext.RequestContext.GetCollectionName()
      };
      CollectionPackageIndexingProperties indexingProperties = new CollectionPackageIndexingProperties();
      indexingProperties.IndexContractType = indexContractType;
      indexingProperties.QueryContractType = indexContractType;
      indexingProperties.IndexESConnectionString = executionContext.ServiceSettings.JobAgentSearchPlatformConnectionString;
      indexingProperties.QueryESConnectionString = executionContext.ServiceSettings.ATSearchPlatformConnectionString;
      indexingProperties.Name = executionContext.RequestContext.GetCollectionName();
      indexingProperties.FeedIndexJobYieldData = new FeedIndexJobYieldData();
      indexingUnit1.Properties = (IndexingProperties) indexingProperties;
      IndexingUnit indexingUnit2 = indexingUnit1;
      return indexingUnitDataAccess.AddIndexingUnit(executionContext.RequestContext, indexingUnit2);
    }

    protected internal override IndexIdentity ProvisionIndex(
      Microsoft.VisualStudio.Services.Search.Common.ExecutionContext executionContext,
      IndexingUnit collectionIndexingUnit,
      bool assignNewIndex)
    {
      if (!executionContext.RequestContext.GetCurrentHostConfigValue<bool>("/service/ALMSearch/Settings/Routing/PackageDocCountBasedIndexProvisioningEnabled", true))
        return base.ProvisionIndex(executionContext, collectionIndexingUnit, assignNewIndex);
      this.ResultMessage.Append("Doc count based Index provisioning is enabled, not using the default model to provision the index. Index is being provisioned in CollectionPackagedIndexOperation.");
      return (IndexIdentity) null;
    }

    protected internal override bool PreRun(
      Microsoft.VisualStudio.Services.Search.Common.ExecutionContext executionContext,
      out TeamFoundationJobExecutionResult jobResult)
    {
      jobResult = TeamFoundationJobExecutionResult.Succeeded;
      if (executionContext.RequestContext.IsPackageIndexingEnabled())
        return true;
      TeamFoundationTaskService service = executionContext.RequestContext.To(TeamFoundationHostType.Deployment).GetService<TeamFoundationTaskService>();
      bool packageIndexingEnabled = false;
      TimeSpan timeSpan = TimeSpan.FromSeconds((double) executionContext.GetConfigValue<int>("/Service/ALMSearch/Settings/WorkItemIndexingFeatureFlagStatusPollTimeInSeconds"));
      Stopwatch stopwatch = Stopwatch.StartNew();
      do
      {
        service.AddTask(executionContext.RequestContext, (TeamFoundationTaskCallback) ((requestContext, ignored) =>
        {
          if (!requestContext.IsPackageIndexingEnabled())
            return;
          packageIndexingEnabled = true;
        }));
        Thread.Sleep(TimeSpan.FromSeconds(1.0));
        if (packageIndexingEnabled)
          return true;
      }
      while (stopwatch.Elapsed < timeSpan);
      return false;
    }
  }
}
