// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.BoardAccountFaultInJob
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
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs
{
  public class BoardAccountFaultInJob : AbstractAccountFaultInJob
  {
    public BoardAccountFaultInJob()
      : this(Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer.DataAccessFactory.GetInstance(), (IIndexingUnitChangeEventHandler) new Microsoft.VisualStudio.Services.Search.Server.EventHandler.IndexingUnitChangeEventHandler())
    {
    }

    internal BoardAccountFaultInJob(
      IDataAccessFactory dataAccessFactory,
      IIndexingUnitChangeEventHandler indexingUnitChangeEventHandler)
      : this(dataAccessFactory, indexingUnitChangeEventHandler, JobConstants.BoardAccountFaultInJobId)
    {
    }

    protected BoardAccountFaultInJob(
      IDataAccessFactory dataAccessFactory,
      IIndexingUnitChangeEventHandler indexingUnitChangeEventHandler,
      Guid jobId)
      : base(dataAccessFactory, indexingUnitChangeEventHandler, (IEntityType) BoardEntityType.GetInstance(), jobId)
    {
    }

    protected override void AddMetadataCrawlOperation(
      ExecutionContext executionContext,
      int indexingUnitId,
      IndexingUnitChangeEventPrerequisites metadataCrawlPrerequisites)
    {
      if (executionContext.RequestContext.IsBoardIndexingFeatureFlagEnabled())
      {
        IndexingUnitChangeEvent indexingUnitChangeEvent = new IndexingUnitChangeEvent()
        {
          IndexingUnitId = indexingUnitId,
          ChangeData = new ChangeEventData(executionContext)
          {
            Trigger = this.Trigger
          },
          ChangeType = "CrawlMetadata",
          State = IndexingUnitChangeEventState.Pending,
          AttemptCount = 0,
          Prerequisites = metadataCrawlPrerequisites
        };
        this.IndexingUnitChangeEventHandler.HandleEvent(executionContext, indexingUnitChangeEvent);
      }
      else
        this.ResultMessage.Append(FormattableString.Invariant(FormattableStringFactory.Create("Board Indexing feature is disabled for {0}", (object) executionContext.RequestContext.GetCollectionID())));
    }

    protected override void ValidateRequestContext(IVssRequestContext requestContext)
    {
      if (!requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
        throw new UnsupportedHostTypeException(requestContext.ServiceHost.HostType);
    }

    protected override IndexingUnit CreateHostIndexingUnit(
      ExecutionContext executionContext,
      IIndexingUnitDataAccess indexingUnitDataAccess,
      DocumentContractType indexContractType)
    {
      IndexingUnit indexingUnit1 = new IndexingUnit(executionContext.RequestContext.GetCollectionID(), "Collection", this.EntityType, -1);
      indexingUnit1.TFSEntityAttributes = (TFSEntityAttributes) new CollectionAttributes()
      {
        CollectionName = executionContext.RequestContext.GetCollectionName()
      };
      CollectionBoardIndexingProperties indexingProperties = new CollectionBoardIndexingProperties();
      indexingProperties.IndexContractType = indexContractType;
      indexingProperties.QueryContractType = indexContractType;
      indexingProperties.IndexESConnectionString = executionContext.ServiceSettings.JobAgentSearchPlatformConnectionString;
      indexingProperties.QueryESConnectionString = executionContext.ServiceSettings.ATSearchPlatformConnectionString;
      indexingProperties.Name = executionContext.RequestContext.GetCollectionName();
      indexingUnit1.Properties = (IndexingProperties) indexingProperties;
      IndexingUnit indexingUnit2 = indexingUnit1;
      return indexingUnitDataAccess.AddIndexingUnit(executionContext.RequestContext, indexingUnit2);
    }

    protected internal override IndexIdentity ProvisionIndex(
      ExecutionContext executionContext,
      IndexingUnit collectionIndexingUnit,
      bool assignNewIndex)
    {
      return (IndexIdentity) null;
    }

    protected internal override void UpdateReIndexingStatusIfNeeded(
      IVssRequestContext requestContext)
    {
    }
  }
}
