// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.Package.CollectionPackageUpdateFeedsOperation
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Arriba;
using Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Entities.EntityProperties;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Common.HttpClientWrappers;
using Microsoft.VisualStudio.Services.Search.Indexer;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.Package;
using Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions;
using Microsoft.VisualStudio.Services.Search.Server.Pipeline;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.Package
{
  internal class CollectionPackageUpdateFeedsOperation : AbstractIndexingOperation
  {
    private FeedHttpClientWrapper m_feedHttpClientWrapper;
    [StaticSafe]
    private static readonly TraceMetaData s_traceMetadata = new TraceMetaData(1083065, "Indexing Pipeline", "IndexingOperation");

    public CollectionPackageUpdateFeedsOperation(
      ExecutionContext executionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent)
      : base(executionContext, indexingUnit, indexingUnitChangeEvent, Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer.DataAccessFactory.GetInstance())
    {
    }

    public override OperationResult RunOperation(
      CoreIndexingExecutionContext coreIndexingExecutionContext)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(CollectionPackageUpdateFeedsOperation.s_traceMetadata, nameof (RunOperation));
      OperationResult operationResult = new OperationResult();
      StringBuilder resultMessage = new StringBuilder();
      IndexingExecutionContext indexingExecutionContext = (IndexingExecutionContext) coreIndexingExecutionContext;
      try
      {
        this.SetUpHttpClients((ExecutionContext) indexingExecutionContext);
        List<Guid> changeEventData = this.GetChangeEventData();
        List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> feedIusToProcess = this.GetFeedIUsToProcess(indexingExecutionContext, changeEventData);
        this.RenameFeedIndexingUnits(indexingExecutionContext, feedIusToProcess, resultMessage);
        operationResult.Status = OperationStatus.Succeeded;
      }
      finally
      {
        operationResult.Message = resultMessage.ToString();
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(CollectionPackageUpdateFeedsOperation.s_traceMetadata, nameof (RunOperation));
      }
      return operationResult;
    }

    internal virtual void RenameFeedIndexingUnits(
      IndexingExecutionContext indexingExecutionContext,
      List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> feedIndexingUnitsToBeUpdated,
      StringBuilder resultMessage)
    {
      foreach (Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit in feedIndexingUnitsToBeUpdated)
      {
        this.BulkUpdateFeedPackageDocumentsInElasticSearch(indexingExecutionContext, indexingUnit, resultMessage);
        this.IndexingUnitDataAccess.UpdateIndexingUnit(indexingExecutionContext.RequestContext, indexingUnit);
      }
    }

    internal virtual void BulkUpdateFeedPackageDocumentsInElasticSearch(
      IndexingExecutionContext indexingExecutionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit feedIndexingUnit,
      StringBuilder resultMessage)
    {
      string str = feedIndexingUnit.TFSEntityId.ToString();
      IndexInfo indexInfo1 = this.IndexingUnit.GetIndexInfo();
      if (indexInfo1 == null)
      {
        resultMessage.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, FormattableString.Invariant(FormattableStringFactory.Create("IndexInfo is null.")));
      }
      else
      {
        IndexIdentity indexIdentity = IndexIdentity.CreateIndexIdentity(indexInfo1.IndexName);
        ISearchIndex index = indexingExecutionContext.ProvisioningContext.SearchPlatform.GetIndex(indexIdentity);
        IndexInfo indexInfo2 = indexInfo1;
        int contractType = (int) indexingExecutionContext.ProvisioningContext.ContractType;
        IExpression expression = (IExpression) new AndExpression(new IExpression[2]
        {
          (IExpression) new TermExpression("collectionId", Operator.Equals, indexingExecutionContext.CollectionId.ToString().ToLowerInvariant()),
          (IExpression) new TermExpression("feedId", Operator.Equals, str)
        });
        PackageVersionContract updatedPartialAbstractSearchDocument = new PackageVersionContract();
        updatedPartialAbstractSearchDocument.FeedName = (feedIndexingUnit.Properties as FeedIndexingProperties).FeedName;
        IExpression query = expression;
        TimeSpan requestTimeOut = TimeSpan.FromSeconds((double) indexingExecutionContext.ServiceSettings.JobSettings.MaxAllowedOnPremiseJobExecutionTimeInSec);
        BulkUpdateByQueryRequest request = new BulkUpdateByQueryRequest(indexInfo2, (DocumentContractType) contractType, (AbstractSearchDocumentContract) updatedPartialAbstractSearchDocument, query, requestTimeOut);
        IndexOperationsResponse operationsResponse = index.BulkUpdateByQuery((ExecutionContext) indexingExecutionContext, request);
        resultMessage.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, FormattableString.Invariant(FormattableStringFactory.Create("{0} response: [{1}]. ", (object) "BulkUpdateByQuery", (object) operationsResponse)));
      }
    }

    internal virtual List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> GetFeedIUsToProcess(
      IndexingExecutionContext indexingExecutionContext,
      List<Guid> listOfFeeds)
    {
      List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> feedIusToProcess = new List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>();
      foreach (Guid listOfFeed in listOfFeeds)
      {
        Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit = this.IndexingUnitDataAccess.GetIndexingUnit(indexingExecutionContext.RequestContext, listOfFeed, "Feed", (IEntityType) PackageEntityType.GetInstance());
        if (indexingUnit == null)
        {
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(CollectionPackageUpdateFeedsOperation.s_traceMetadata, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "No valid IndexingUnit could be fetched for feed {0}", (object) listOfFeed));
        }
        else
        {
          Guid projectId = (indexingUnit.Properties as FeedIndexingProperties).ProjectId;
          Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed = !(projectId != new Guid()) ? this.m_feedHttpClientWrapper.GetFeed(listOfFeed) : this.m_feedHttpClientWrapper.GetFeed(listOfFeed, projectId);
          (indexingUnit.Properties as FeedIndexingProperties).FeedName = feed.Name;
          (indexingUnit.Properties as FeedIndexingProperties).FullyQualifiedName = feed.FullyQualifiedName;
          feedIusToProcess.Add(indexingUnit);
        }
      }
      return feedIusToProcess;
    }

    internal virtual void SetUpHttpClients(ExecutionContext executionContext) => this.m_feedHttpClientWrapper = this.GetHttpClientFactory().GetFeedHttpClient(executionContext);

    internal virtual IHttpClientWrapperFactory GetHttpClientFactory() => HttpClientWrapperFactory.GetInstance();

    internal virtual List<Guid> GetChangeEventData()
    {
      PackageContinuousIndexEventData changeData = (PackageContinuousIndexEventData) this.IndexingUnitChangeEvent.ChangeData;
      if (changeData != null)
      {
        List<Guid> feeds = changeData.Feeds;
        // ISSUE: explicit non-virtual call
        if ((feeds != null ? (__nonvirtual (feeds.Count) > 0 ? 1 : 0) : 0) != 0)
          return changeData.Feeds;
      }
      return new List<Guid>();
    }
  }
}
