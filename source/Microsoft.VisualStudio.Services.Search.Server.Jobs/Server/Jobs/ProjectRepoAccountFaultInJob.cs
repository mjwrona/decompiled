// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.ProjectRepoAccountFaultInJob
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Constants;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer;
using Microsoft.VisualStudio.Services.Search.Server.EventHandler;
using System;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs
{
  public class ProjectRepoAccountFaultInJob : AbstractAccountFaultInJob
  {
    public ProjectRepoAccountFaultInJob()
      : this(Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer.DataAccessFactory.GetInstance(), (IIndexingUnitChangeEventHandler) new Microsoft.VisualStudio.Services.Search.Server.EventHandler.IndexingUnitChangeEventHandler())
    {
    }

    internal ProjectRepoAccountFaultInJob(
      IDataAccessFactory dataAccessFactory,
      IIndexingUnitChangeEventHandler indexingUnitChangeEventHandler)
      : base(dataAccessFactory, indexingUnitChangeEventHandler, (IEntityType) ProjectRepoEntityType.GetInstance(), JobConstants.ProjectCollectionFaultInJobId)
    {
    }

    protected override void AddMetadataCrawlOperation(
      ExecutionContext executionContext,
      int indexingUnitId,
      IndexingUnitChangeEventPrerequisites metadataCrawlPrerequisites)
    {
      IndexingUnitChangeEvent indexingUnitChangeEvent1 = new IndexingUnitChangeEvent();
      indexingUnitChangeEvent1.IndexingUnitId = indexingUnitId;
      ProjectRepoMetadataCrawlEventData metadataCrawlEventData = new ProjectRepoMetadataCrawlEventData(executionContext);
      metadataCrawlEventData.Trigger = this.Trigger;
      metadataCrawlEventData.EventTime = DateTime.Now;
      indexingUnitChangeEvent1.ChangeData = (ChangeEventData) metadataCrawlEventData;
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

    protected internal override void UpdateReIndexingStatusIfNeeded(
      IVssRequestContext requestContext)
    {
    }
  }
}
