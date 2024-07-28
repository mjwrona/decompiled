// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.Package.CollectionPackageFeedSecurityAcesSyncOperation
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.HttpClientWrappers;
using Microsoft.VisualStudio.Services.Search.Indexer;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions;
using Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer;
using Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions;
using Microsoft.VisualStudio.Services.Search.Server.Pipeline;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.Package
{
  internal class CollectionPackageFeedSecurityAcesSyncOperation : AbstractIndexingOperation
  {
    [StaticSafe]
    private static readonly TraceMetaData s_traceMetadata = new TraceMetaData(1080054, "Indexing Pipeline", "IndexingOperation");
    private readonly FeedHttpClientWrapper m_feedHttpClientWrapper;
    private readonly IPackageContainerDataAccess m_packageContainerDataAccess;
    private readonly FeedSecurityAcesUpdater m_securityAcesUpdater;

    [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
    public CollectionPackageFeedSecurityAcesSyncOperation(
      CoreIndexingExecutionContext executionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent)
      : this(executionContext, indexingUnit, indexingUnitChangeEvent, Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer.DataAccessFactory.GetInstance(), new FeedHttpClientWrapper((ExecutionContext) executionContext, CollectionPackageFeedSecurityAcesSyncOperation.s_traceMetadata), new FeedSecurityAcesUpdater((ExecutionContext) executionContext))
    {
    }

    protected CollectionPackageFeedSecurityAcesSyncOperation(
      CoreIndexingExecutionContext executionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent,
      IDataAccessFactory dataAccessFactory,
      FeedHttpClientWrapper feedHttpClientWrapper,
      FeedSecurityAcesUpdater securityAcesUpdater)
      : base((ExecutionContext) executionContext, indexingUnit, indexingUnitChangeEvent, dataAccessFactory)
    {
      this.m_packageContainerDataAccess = dataAccessFactory.GetPackageContainerDataAccess();
      this.m_feedHttpClientWrapper = feedHttpClientWrapper;
      this.m_securityAcesUpdater = securityAcesUpdater;
    }

    public override OperationResult RunOperation(
      CoreIndexingExecutionContext coreIndexingExecutionContext)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(CollectionPackageFeedSecurityAcesSyncOperation.s_traceMetadata, nameof (RunOperation));
      OperationResult operationResult = new OperationResult();
      StringBuilder resultMessage = new StringBuilder();
      IndexingExecutionContext executionContext = (IndexingExecutionContext) coreIndexingExecutionContext;
      try
      {
        List<PackageContainer> packageContainers = this.GetPackageContainers();
        List<PackageContainer> searchSyncedPackageContainers = this.FetchSearchPackageContainers(executionContext);
        IndexOperationsResponse operationsResponse = new IndexOperationsResponse()
        {
          Success = true,
          IsOperationIncomplete = false
        };
        operationsResponse.Success = this.HandleMissingAndStalePackageContainers(executionContext, packageContainers, searchSyncedPackageContainers, resultMessage);
        operationsResponse.Success &= this.HandleDeletedPackageContainers(executionContext, packageContainers, searchSyncedPackageContainers, resultMessage);
        if (!operationsResponse.Success)
          throw new SearchException(FormattableString.Invariant(FormattableStringFactory.Create("Update or delete packageContainers in DB failed for project: {0} ", (object) this.IndexingUnit)));
      }
      catch (Exception ex)
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceException(CollectionPackageFeedSecurityAcesSyncOperation.s_traceMetadata, ex);
        resultMessage.Append(FormattableString.Invariant(FormattableStringFactory.Create("Could not update or delete packageContainers for collection [{0}]. ", (object) this.IndexingUnit.TFSEntityId)));
      }
      finally
      {
        operationResult.Message = resultMessage.ToString();
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(CollectionPackageFeedSecurityAcesSyncOperation.s_traceMetadata, nameof (RunOperation));
      }
      return operationResult;
    }

    internal virtual bool HandleMissingAndStalePackageContainers(
      IndexingExecutionContext executionContext,
      List<PackageContainer> remotePackageContainers,
      List<PackageContainer> searchSyncedPackageContainers,
      StringBuilder resultMessage)
    {
      List<PackageContainer> packageContainersToBeUpdatedInDb = this.UpdateSecurityHashIfApplicableAndPersistToDb(executionContext.RequestContext, remotePackageContainers, searchSyncedPackageContainers);
      resultMessage.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, FormattableString.Invariant(FormattableStringFactory.Create("Updated [{0}] packageContainers in DB. ", (object) packageContainersToBeUpdatedInDb.Count)));
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceVerboseConditionally(CollectionPackageFeedSecurityAcesSyncOperation.s_traceMetadata, (Func<string>) (() => FormattableString.Invariant(FormattableStringFactory.Create("PackageContainers deleted are [{0}]", (object) string.Join(";", packageContainersToBeUpdatedInDb.Select<PackageContainer, string>((Func<PackageContainer, string>) (n => n.Name)))))));
      return true;
    }

    internal virtual bool HandleDeletedPackageContainers(
      IndexingExecutionContext executionContext,
      List<PackageContainer> remotePackageContainers,
      List<PackageContainer> searchSyncedPackageContainers,
      StringBuilder resultMessage)
    {
      List<PackageContainer> packageContainersToBeDeletedInDb = searchSyncedPackageContainers.Except<PackageContainer>((IEnumerable<PackageContainer>) remotePackageContainers, (IEqualityComparer<PackageContainer>) new PackageContainerIdComparator()).ToList<PackageContainer>();
      int num = this.DeletePackageContainersInDb(executionContext.RequestContext, packageContainersToBeDeletedInDb);
      resultMessage.Append(FormattableString.Invariant(FormattableStringFactory.Create("Deleted [{0}] packageContainers from DB. ", (object) num)));
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceVerboseConditionally(CollectionPackageFeedSecurityAcesSyncOperation.s_traceMetadata, (Func<string>) (() => FormattableString.Invariant(FormattableStringFactory.Create("PackageContainers deleted are [{0}]", (object) string.Join(";", packageContainersToBeDeletedInDb.Select<PackageContainer, string>((Func<PackageContainer, string>) (n => n.Name)))))));
      return true;
    }

    private int DeletePackageContainersInDb(
      IVssRequestContext requestContext,
      List<PackageContainer> packageContainersToBeDeletedInDb)
    {
      return packageContainersToBeDeletedInDb.Count > 0 ? this.m_packageContainerDataAccess.DeletePackageContainers(requestContext, packageContainersToBeDeletedInDb.Select<PackageContainer, Guid>((Func<PackageContainer, Guid>) (n => n.ContainerId)).ToList<Guid>()) : 0;
    }

    private List<PackageContainer> GetPackageContainers()
    {
      IEnumerable<Microsoft.VisualStudio.Services.Feed.WebApi.Feed> feeds = (IEnumerable<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>) new List<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>();
      return this.GetAllFeedsAndViews(this.m_feedHttpClientWrapper.GetFeeds());
    }

    private List<PackageContainer> FetchSearchPackageContainers(
      IndexingExecutionContext executionContext)
    {
      return this.m_packageContainerDataAccess.GetPackageContainers(executionContext.RequestContext, -1).ToList<PackageContainer>();
    }

    private List<PackageContainer> GetAllFeedsAndViews(IEnumerable<Microsoft.VisualStudio.Services.Feed.WebApi.Feed> feeds)
    {
      List<PackageContainer> allFeedsAndViews = new List<PackageContainer>();
      foreach (Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed in feeds)
      {
        if (feed.Project == (ProjectReference) null)
          allFeedsAndViews.Add(this.GetPackageContainer(feed.Id, feed.Name, PackageContainerType.Feed, feed.Id));
        else
          allFeedsAndViews.Add(this.GetPackageContainer(feed.Id, feed.Name, PackageContainerType.Feed, feed.Id, feed.Project.Id));
        foreach (FeedView feedView in feed.Project == (ProjectReference) null ? this.m_feedHttpClientWrapper.GetFeedViews(feed.Id) : this.m_feedHttpClientWrapper.GetFeedViews(feed.Project.Id, feed.Id))
        {
          if (feed.Project == (ProjectReference) null)
            allFeedsAndViews.Add(this.GetPackageContainer(feedView.Id, feedView.Name, PackageContainerType.View, feed.Id));
          else
            allFeedsAndViews.Add(this.GetPackageContainer(feedView.Id, feedView.Name, PackageContainerType.View, feed.Id, feed.Project.Id));
        }
      }
      return allFeedsAndViews;
    }

    private PackageContainer GetPackageContainer(
      Guid id,
      string name,
      PackageContainerType type,
      Guid feedId,
      Guid ProjectId = default (Guid))
    {
      string empty = string.Empty;
      string token;
      if (ProjectId == new Guid())
      {
        string str;
        if (type != PackageContainerType.Feed)
          str = FormattableString.Invariant(FormattableStringFactory.Create("$/{0}/{1}", (object) feedId, (object) id));
        else
          str = FormattableString.Invariant(FormattableStringFactory.Create("$/{0}", (object) id));
        token = str;
      }
      else
      {
        string str;
        if (type != PackageContainerType.Feed)
          str = FormattableString.Invariant(FormattableStringFactory.Create("$/project:{0}/{1}/{2}", (object) ProjectId, (object) feedId, (object) id));
        else
          str = FormattableString.Invariant(FormattableStringFactory.Create("$/project:{0}/{1}", (object) ProjectId, (object) id));
        token = str;
      }
      return new PackageContainer(id, name, type, token);
    }

    internal virtual List<PackageContainer> UpdateSecurityHashIfApplicableAndPersistToDb(
      IVssRequestContext requestContext,
      List<PackageContainer> packageContainers,
      List<PackageContainer> searchSyncedPackageContainers)
    {
      if (packageContainers.Count == 0)
        return packageContainers;
      packageContainers = this.m_securityAcesUpdater.UpdateFeedsWithSecurityHashAndToken(packageContainers);
      List<PackageContainer> list = packageContainers.Except<PackageContainer>((IEnumerable<PackageContainer>) searchSyncedPackageContainers, (IEqualityComparer<PackageContainer>) new PackageContainerEqualityComparator()).ToList<PackageContainer>();
      if (list.Count > 0)
        this.m_packageContainerDataAccess.AddOrUpdatePackageContainers(requestContext, list, true);
      return list;
    }
  }
}
