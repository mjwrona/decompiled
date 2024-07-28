// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamMetadataManager`3
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Packaging.CrossProtocol.Internal.WebApi.Types;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Settings;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Telemetry;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Upstream;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamCache.V2;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamFailureTracking;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamFailureTracking.Classifier;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Exceptions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;


#nullable enable
namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared
{
  public class UpstreamMetadataManager<TPackageName, TPackageIdentity, TMetadataEntry> : 
    IUpstreamMetadataManager
    where TPackageName : 
    #nullable disable
    class, IPackageName
    where TPackageIdentity : class, IPackageIdentity
    where TMetadataEntry : class, IMetadataEntry<TPackageIdentity>
  {
    private readonly IFactory<IFeedRequest, IUpstreamMetadataCacheInfoStore> metadataCacheInfoStoreFactory;
    private readonly IReadMetadataDocumentService<TPackageIdentity, TMetadataEntry> localMetadataService;
    private readonly IUpstreamRefreshStrategy<TPackageName, TPackageIdentity, TMetadataEntry> upstreamRefreshStrategy;
    private readonly IConverter<string, TPackageName> stringToPackageNameConverter;
    private readonly IConverter<IPackageIdentity, TPackageIdentity> packageIdentityConverter;
    private readonly IHandler<IFeedRequest, IEnumerable<UpstreamSource>> upstreamsFromFeedHandler;
    private readonly IUpstreamsConfigurationHasher upstreamConfigurationHasher;
    private readonly ITelemetryService telemetryService;
    private readonly IAsyncHandler<FeedRequest<RefreshPackageResult>, ICiData> telemetryProducer;
    private readonly ITracerService tracerService;
    private readonly IRegistryService registryService;
    private readonly IPackagingTraces propertyTracer;
    private readonly ICancellationFacade cancellationFacade;
    private readonly IUpstreamStatusHandler upstreamStatusResultHandler;
    private readonly IFeatureFlagService featureFlagService;
    private readonly IInPlaceSorter<TPackageName> packageNameEvaluationOrderSorter;
    private readonly IExecutionEnvironment executionEnvironment;
    private readonly ITimeProvider timeProvider;
    private readonly IUpstreamPackagesToRefreshInformationConverter upstreamRefreshDataToPackageListConverter;
    private readonly IFeedViewVisibilityValidator upstreamValidator;
    private readonly IUpstreamStatusClassifier upstreamStatusClassifier;
    private readonly IOrgLevelPackagingSetting<bool> addToRefreshListOnlyIfAnyVersionsSetting;
    public const string maxPackageFailuresRegistryFormat = "/Configuration/Packaging/{0}/UpstreamMetadataCache/MaxPackageFailures";
    public const int maxPackageFailuresDefault = 50;
    private const string SingleFeedPackageRefreshTelemetryTag = "SingleFeedPackageRefreshTelemetry";

    public UpstreamMetadataManager(
      IFactory<IFeedRequest, IUpstreamMetadataCacheInfoStore> cacheStoreFactory,
      IReadMetadataDocumentService<TPackageIdentity, TMetadataEntry> localMetadataService,
      IUpstreamRefreshStrategy<TPackageName, TPackageIdentity, TMetadataEntry> upstreamRefreshStrategy,
      IConverter<string, TPackageName> stringToPackageNameConverter,
      IConverter<IPackageIdentity, TPackageIdentity> packageIdentityConverter,
      IHandler<IFeedRequest, IEnumerable<UpstreamSource>> upstreamsFromFeedHandler,
      IUpstreamsConfigurationHasher upstreamConfigurationHasher,
      ITelemetryService telemetryService,
      IAsyncHandler<FeedRequest<RefreshPackageResult>, ICiData> telemetryProducer,
      ITracerService tracerService,
      IRegistryService registryService,
      IPackagingTraces propertyTracer,
      ICancellationFacade cancellationFacade,
      IUpstreamStatusHandler upstreamStatusResultHandler,
      IFeatureFlagService featureFlagService,
      IInPlaceSorter<TPackageName> packageNameEvaluationOrderSorter,
      IExecutionEnvironment executionEnvironment,
      ITimeProvider timeProvider,
      IUpstreamPackagesToRefreshInformationConverter upstreamRefreshDataToPackageListConverter,
      IFeedViewVisibilityValidator upstreamValidator,
      IUpstreamStatusClassifier upstreamStatusClassifier,
      IOrgLevelPackagingSetting<bool> addToRefreshListOnlyIfAnyVersionsSetting)
    {
      this.metadataCacheInfoStoreFactory = cacheStoreFactory ?? throw new ArgumentNullException(nameof (cacheStoreFactory));
      this.localMetadataService = localMetadataService ?? throw new ArgumentNullException(nameof (localMetadataService));
      this.upstreamRefreshStrategy = upstreamRefreshStrategy;
      this.stringToPackageNameConverter = stringToPackageNameConverter ?? throw new ArgumentNullException(nameof (stringToPackageNameConverter));
      this.packageIdentityConverter = packageIdentityConverter ?? throw new ArgumentNullException(nameof (packageIdentityConverter));
      this.upstreamsFromFeedHandler = upstreamsFromFeedHandler;
      this.upstreamConfigurationHasher = upstreamConfigurationHasher;
      this.telemetryService = telemetryService;
      this.telemetryProducer = telemetryProducer;
      this.tracerService = tracerService;
      this.registryService = registryService;
      this.propertyTracer = propertyTracer;
      this.cancellationFacade = cancellationFacade;
      this.upstreamStatusResultHandler = upstreamStatusResultHandler;
      this.featureFlagService = featureFlagService;
      this.packageNameEvaluationOrderSorter = packageNameEvaluationOrderSorter;
      this.executionEnvironment = executionEnvironment;
      this.timeProvider = timeProvider;
      this.upstreamRefreshDataToPackageListConverter = upstreamRefreshDataToPackageListConverter;
      this.upstreamValidator = upstreamValidator;
      this.upstreamStatusClassifier = upstreamStatusClassifier;
      this.addToRefreshListOnlyIfAnyVersionsSetting = addToRefreshListOnlyIfAnyVersionsSetting;
    }

    public async Task<RefreshPackageResult> RefreshPackageVersionAsync(
      IFeedRequest feedRequest,
      IPackageIdentity packageIdentity,
      bool forceRefreshAllUpstreamVersionLists)
    {
      UpstreamMetadataManager<TPackageName, TPackageIdentity, TMetadataEntry> sendInTheThisObject = this;
      sendInTheThisObject.tracerService.EnableStoredTraces();
      FeedCore feed = feedRequest.Feed;
      using (ITracerBlock tracer = sendInTheThisObject.tracerService.Enter((object) sendInTheThisObject, nameof (RefreshPackageVersionAsync)))
      {
        tracer.TraceMarker("Refreshing package", packageIdentity.DisplayStringForMessages);
        if (!sendInTheThisObject.IsUpstreamsEnabled(feedRequest))
        {
          tracer.TraceInfo(string.Format("Upstreams is disabled for feed '{0}'", (object) feed.Id));
          return RefreshPackageResult.UpstreamsDisabled(feed, packageIdentity.Name);
        }
        IEnumerable<UpstreamSource> upstreams = sendInTheThisObject.upstreamsFromFeedHandler.Handle(feedRequest);
        RefreshPackageResult result = await sendInTheThisObject.RefreshPackageVersionInternal(feedRequest, packageIdentity, upstreams, forceRefreshAllUpstreamVersionLists);
        await sendInTheThisObject.ThrowIfResultFailed(feedRequest, result, upstreams);
        await sendInTheThisObject.metadataCacheInfoStoreFactory.Get(feedRequest).AddPackageIfNotExistsAsync(feed, packageIdentity.Name);
        ICiData request = await sendInTheThisObject.telemetryProducer.Handle(new FeedRequest<RefreshPackageResult>(feedRequest, result));
        NullResult nullResult = await sendInTheThisObject.telemetryService.Handle(request);
        return result;
      }
    }

    public async Task<RefreshPackageResult> RefreshPackageAsync(
      IFeedRequest feedRequest,
      IPackageName packageName,
      bool forceRefreshAllUpstreamVersionLists,
      ISet<Guid> upstreamVersionListsToForceRefreshByUpstreamId,
      bool isFromExplicitUserAction,
      PushDrivenUpstreamsNotificationTelemetry notificationTelemetry,
      bool needIntermediateData)
    {
      UpstreamMetadataManager<TPackageName, TPackageIdentity, TMetadataEntry> sendInTheThisObject = this;
      sendInTheThisObject.tracerService.EnableStoredTraces();
      FeedCore feed = feedRequest.Feed;
      using (ITracerBlock tracer = sendInTheThisObject.tracerService.Enter((object) sendInTheThisObject, nameof (RefreshPackageAsync)))
      {
        try
        {
          tracer.TraceMarker("Refreshing package", packageName.DisplayName);
          if (!sendInTheThisObject.IsUpstreamsEnabled(feedRequest))
          {
            tracer.TraceInfo(string.Format("Not refreshing {0} package {1} for feed '{2}'. UpstreamsEnabled: {3}. Number of upstreams (all protocols): {4}", (object) feedRequest.Protocol, (object) packageName, (object) feed.Id, (object) feed.UpstreamEnabled, (object) feed.UpstreamSources.Count));
            return RefreshPackageResult.UpstreamsDisabled(feed, packageName);
          }
          IEnumerable<UpstreamSource> upstreams = sendInTheThisObject.upstreamsFromFeedHandler.Handle(feedRequest);
          RefreshPackageResult result = await sendInTheThisObject.RefreshPackageInternal(feedRequest, packageName, upstreams, forceRefreshAllUpstreamVersionLists, upstreamVersionListsToForceRefreshByUpstreamId, notificationTelemetry, needIntermediateData);
          await sendInTheThisObject.ThrowIfResultFailed(feedRequest, result, upstreams);
          bool flag = result.LocalVersions + result.CurUpstreamVersions > 0;
          if (isFromExplicitUserAction && (flag || !sendInTheThisObject.addToRefreshListOnlyIfAnyVersionsSetting.Get()))
            await sendInTheThisObject.metadataCacheInfoStoreFactory.Get(feedRequest).AddPackageIfNotExistsAsync(feed, packageName);
          ICiData request = await sendInTheThisObject.telemetryProducer.Handle(new FeedRequest<RefreshPackageResult>(feedRequest, result));
          NullResult nullResult = await sendInTheThisObject.telemetryService.Handle(request);
          return result;
        }
        catch (Exception ex)
        {
          tracer.TraceException(ex);
          throw;
        }
      }
    }

    public async Task<RefreshPackagesResult> RefreshPackagesAsync(
      IFeedRequest feedRequest,
      bool forceRefreshAllUpstreamVersionLists,
      ISet<Guid> upstreamVersionListsToForceRefreshByUpstreamId,
      UpstreamPackagesToRefreshInformation upstreamRefreshData)
    {
      UpstreamMetadataManager<TPackageName, TPackageIdentity, TMetadataEntry> sendInTheThisObject = this;
      sendInTheThisObject.tracerService.EnableStoredTraces();
      UpstreamStatusCategory statusCategory = UpstreamStatusCategory.FullRefreshSuccess;
      int maxPackageFailures = sendInTheThisObject.GetMaxPackageFailures(feedRequest);
      FeedCore feed = feedRequest.Feed;
      using (ITracerBlock tracer = sendInTheThisObject.tracerService.Enter((object) sendInTheThisObject, nameof (RefreshPackagesAsync)))
      {
        if (!sendInTheThisObject.IsUpstreamsEnabled(feedRequest))
        {
          tracer.TraceInfo(string.Format("Upstreams is disabled for feed '{0}'", (object) feed.Id));
          return new RefreshPackagesResult(feed.Id);
        }
        UpstreamMetadataCacheInfo metadataCacheInfoAsync = await sendInTheThisObject.metadataCacheInfoStoreFactory.Get(feedRequest).GetMetadataCacheInfoAsync(feed);
        tracer.TraceMarker("After GetMetadataCacheInfoAsync");
        if (metadataCacheInfoAsync?.PackageNames == null || !metadataCacheInfoAsync.PackageNames.Any<IPackageName>())
        {
          tracer.TraceInfo("No upstream cached packages being tracked, therefore nothing to refresh");
          return new RefreshPackagesResult(feed.Id);
        }
        List<TPackageName> packageNameList = new List<TPackageName>();
        tracer.TraceInfo("Upstream Refresh Data - FirstPackageDisplayName: " + (upstreamRefreshData.FirstPackageDisplayName ?? "(null)") + " and LastPackageDisplayName: " + (upstreamRefreshData.LastPackageDisplayName ?? "(null)") + " ");
        List<TPackageName> list = sendInTheThisObject.upstreamRefreshDataToPackageListConverter.FindPackagesToRefresh(metadataCacheInfoAsync.PackageNames, upstreamRefreshData).Cast<TPackageName>().ToList<TPackageName>();
        sendInTheThisObject.packageNameEvaluationOrderSorter.SortInPlace((IList<TPackageName>) list);
        tracer.TraceInfo(string.Format("Number of Packages to Refresh: {0}", (object) list.Count<TPackageName>()));
        IEnumerable<UpstreamSource> upstreams = sendInTheThisObject.upstreamsFromFeedHandler.Handle(feedRequest);
        List<RefreshPackageResult> successes = new List<RefreshPackageResult>();
        List<RefreshPackageFailure> failures = new List<RefreshPackageFailure>();
        List<RefreshPackageFailure> whitelistedFailures = new List<RefreshPackageFailure>();
        List<RefreshPackageResult> failedStatusResults = new List<RefreshPackageResult>();
        bool aborted = false;
        int totalNumberOfPackagesToRefresh = list.Count<TPackageName>();
        int plannedUnitsOfWork = UpstreamPackageUtils.GetUnitsOfWork(sendInTheThisObject.registryService, totalNumberOfPackagesToRefresh, feed, feedRequest.Protocol);
        foreach (TPackageName packageName1 in list)
        {
          TPackageName packageName = packageName1;
          try
          {
            sendInTheThisObject.cancellationFacade.ThrowIfCancellationRequested();
            RefreshPackageResult refreshPackageResult = await sendInTheThisObject.RefreshPackageInternal(feedRequest, (IPackageName) packageName, upstreams, forceRefreshAllUpstreamVersionLists, upstreamVersionListsToForceRefreshByUpstreamId, (PushDrivenUpstreamsNotificationTelemetry) null, false);
            if (refreshPackageResult.IsFailed)
            {
              failedStatusResults.Add(refreshPackageResult);
              ExceptionDispatchInfo.Capture((Exception) refreshPackageResult.UpstreamFailureException).Throw();
            }
            successes.Add(refreshPackageResult);
          }
          catch (UpstreamFailureException ex)
          {
            statusCategory = ex.ErrorCategory;
            if (ex.InnerException is OperationCanceledException)
              statusCategory = UpstreamStatusCategory.Aborted;
            tracer.TraceError(string.Format("Failed to refresh feed '{0}', package '{1}', error category '{2}', inner exception type: '{3}'", (object) feed.Id, (object) packageName, (object) statusCategory, (object) ex.InnerException?.GetType().Name));
            tracer.TraceException((Exception) ex);
            RefreshPackageFailure refreshPackageFailure = new RefreshPackageFailure(packageName.DisplayName, ex.ErrorCategory, ex.InnerException ?? (Exception) ex);
            if (UpstreamMetadataManager<TPackageName, TPackageIdentity, TMetadataEntry>.IsFailureUserResponsibility(statusCategory))
              whitelistedFailures.Add(refreshPackageFailure);
            else
              failures.Add(refreshPackageFailure);
            aborted |= UpstreamMetadataManager<TPackageName, TPackageIdentity, TMetadataEntry>.ShouldAbortOnFailure(statusCategory);
          }
          catch (Exception ex)
          {
            tracer.TraceError(string.Format("Failed to refresh feed '{0}', package '{1}'. An exception was thrown that was not an UpstreamFailureException. ex: '{2}'", (object) feed.Id, (object) packageName, (object) ex));
            tracer.TraceException(ex);
            UpstreamStatusCategory upstreamStatusCategory;
            switch (ex)
            {
              case OperationCanceledException _:
                upstreamStatusCategory = UpstreamStatusCategory.Aborted;
                break;
              case RequestCanceledException _:
                upstreamStatusCategory = UpstreamStatusCategory.Aborted;
                break;
              case UpstreamOrganizationNotAccessibleException _:
                upstreamStatusCategory = UpstreamStatusCategory.TargetOrganizationInaccessible;
                break;
              case UpstreamProjectDoesNotExistException _:
                upstreamStatusCategory = UpstreamStatusCategory.TargetProjectDeleted;
                break;
              default:
                upstreamStatusCategory = UpstreamStatusCategory.UnknownFailure;
                break;
            }
            statusCategory = upstreamStatusCategory;
            RefreshPackageFailure refreshPackageFailure = new RefreshPackageFailure(packageName.DisplayName, statusCategory, ex);
            if (UpstreamMetadataManager<TPackageName, TPackageIdentity, TMetadataEntry>.IsFailureUserResponsibility(statusCategory))
              whitelistedFailures.Add(refreshPackageFailure);
            else
              failures.Add(refreshPackageFailure);
            aborted |= UpstreamMetadataManager<TPackageName, TPackageIdentity, TMetadataEntry>.ShouldAbortOnFailure(statusCategory);
          }
          if (failures.Count >= maxPackageFailures)
          {
            tracer.TraceError(string.Format("Feed '{0}' encountered {1} upstream package refresh failures, which is above the allowed limit {2}. Aborting.", (object) feed.Id, (object) failures.Count, (object) maxPackageFailures));
            aborted = true;
          }
          if (!aborted)
            packageName = default (TPackageName);
          else
            break;
        }
        List<UpstreamStatistics> aggregatedStats = sendInTheThisObject.AggregateUpstreamStatistics(successes);
        try
        {
          List<RefreshPackageResult> results = failedStatusResults.Any<RefreshPackageResult>() ? failedStatusResults : successes;
          await sendInTheThisObject.upstreamStatusResultHandler.Handle(feedRequest, new UpstreamPackageRefreshResult((IEnumerable<RefreshPackageResult>) results, statusCategory), upstreams);
          tracer.TraceMarker("after upstreamStatusResultHandler");
        }
        catch (Exception ex)
        {
          tracer.TraceError(string.Format("Failed to handle upstream status with category '{0}'.", (object) statusCategory));
          tracer.TraceException(ex);
        }
        return new RefreshPackagesResult(feed.Id, (IList<RefreshPackageResult>) successes, (IList<RefreshPackageFailure>) failures, (IList<RefreshPackageFailure>) whitelistedFailures, (IList<UpstreamStatistics>) aggregatedStats, !aborted, successes.Sum<RefreshPackageResult>((Func<RefreshPackageResult, int>) (x => x.UpstreamVersionListCacheHits)), successes.Sum<RefreshPackageResult>((Func<RefreshPackageResult, int>) (x => x.UpstreamVersionListCacheMisses)), totalNumberOfPackagesToRefresh, plannedUnitsOfWork);
      }
    }

    private static bool ShouldAbortOnFailure(UpstreamStatusCategory category) => !category.Details().ShouldTryOtherPackagesAfterFailure;

    private static bool IsFailureUserResponsibility(UpstreamStatusCategory category) => category.Details().IsUserResponsibility;

    private async Task<RefreshPackageResult> RefreshPackageInternal(
      IFeedRequest feedRequest,
      IPackageName packageName,
      IEnumerable<UpstreamSource> upstreams,
      bool forceRefreshAllUpstreamVersionLists,
      ISet<Guid> upstreamVersionListsToForceRefreshByUpstreamId,
      PushDrivenUpstreamsNotificationTelemetry pushDrivenUpstreamsNotificationTelemetry,
      bool needIntermediateData)
    {
      UpstreamMetadataManager<TPackageName, TPackageIdentity, TMetadataEntry> sendInTheThisObject = this;
      FeedCore feed = feedRequest.Feed;
      using (ITracerBlock tracer = sendInTheThisObject.tracerService.Enter((object) sendInTheThisObject, nameof (RefreshPackageInternal)))
      {
        DateTime startTime = sendInTheThisObject.timeProvider.Now;
        Stopwatch stopwatch = Stopwatch.StartNew();
        try
        {
          foreach (UpstreamSource upstreamSource in upstreams)
          {
            try
            {
              sendInTheThisObject.cancellationFacade.ThrowIfCancellationRequested();
              await sendInTheThisObject.upstreamValidator.Validate(feedRequest, upstreamSource);
              tracer.TraceMarker("after upstreamValidator");
            }
            catch (Exception ex)
            {
              return RefreshPackageResult.Failed(feedRequest.Feed, packageName, upstreamSource, sendInTheThisObject.upstreamStatusClassifier.Classify(ex, upstreamSource, feed));
            }
          }
          MetadataDocument<TMetadataEntry> statesDocumentAsync = await sendInTheThisObject.localMetadataService.GetPackageVersionStatesDocumentAsync(new PackageNameQuery<TMetadataEntry>((IPackageNameRequest) new PackageNameRequest<IPackageName>(feedRequest, packageName)));
          tracer.TraceMarker("Got metadata doc", packageName.DisplayName);
          if (!(packageName is TPackageName packageName1))
            packageName1 = sendInTheThisObject.stringToPackageNameConverter.Convert(packageName.DisplayName);
          sendInTheThisObject.propertyTracer.Increment("UpstreamVersionRefreshCount");
          RefreshPackageResult result = await sendInTheThisObject.upstreamRefreshStrategy.RefreshPackageAsync(feedRequest, packageName1, upstreams, statesDocumentAsync, sendInTheThisObject.upstreamConfigurationHasher, forceRefreshAllUpstreamVersionLists, upstreamVersionListsToForceRefreshByUpstreamId, needIntermediateData);
          sendInTheThisObject.TracePackageResult(result, startTime, stopwatch.Elapsed, pushDrivenUpstreamsNotificationTelemetry);
          return result;
        }
        catch (Exception ex)
        {
          sendInTheThisObject.TracePackageResult(UpstreamMetadataManager<TPackageName, TPackageIdentity, TMetadataEntry>.CreateRefreshPackageResultForUnhandledException((IProtocolAgnosticFeedRequest) feedRequest, packageName, ex, upstreams), startTime, stopwatch.Elapsed, pushDrivenUpstreamsNotificationTelemetry);
          tracer.TraceError(string.Format("Failed to refresh upstreams for feed '{0}', package '{1}', upstreams: '{2}'", (object) feed.Id, (object) packageName, (object) string.Join<UpstreamSource>(",", upstreams)));
          tracer.TraceException(ex);
          throw;
        }
      }
    }

    private async Task<RefreshPackageResult> RefreshPackageVersionInternal(
      IFeedRequest feedRequest,
      IPackageIdentity packageIdentity,
      IEnumerable<UpstreamSource> upstreams,
      bool forceRefreshAllUpstreamVersionLists)
    {
      UpstreamMetadataManager<TPackageName, TPackageIdentity, TMetadataEntry> sendInTheThisObject = this;
      FeedCore feed = feedRequest.Feed;
      RefreshPackageResult refreshPackageResult;
      using (ITracerBlock tracer = sendInTheThisObject.tracerService.Enter((object) sendInTheThisObject, nameof (RefreshPackageVersionInternal)))
      {
        DateTime startTime = sendInTheThisObject.timeProvider.Now;
        Stopwatch stopwatch = Stopwatch.StartNew();
        try
        {
          MetadataDocument<TMetadataEntry> statesDocumentAsync = await sendInTheThisObject.localMetadataService.GetPackageVersionStatesDocumentAsync(new PackageNameQuery<TMetadataEntry>((IPackageNameRequest) new PackageNameRequest<IPackageName>(feedRequest, packageIdentity.Name)));
          tracer.TraceMarker("Got metadata doc", packageIdentity.Name.DisplayName);
          sendInTheThisObject.propertyTracer.Increment("UpstreamVersionRefreshCount");
          RefreshPackageResult result = await sendInTheThisObject.upstreamRefreshStrategy.RefreshPackageVersionAsync(feedRequest, sendInTheThisObject.packageIdentityConverter.Convert(packageIdentity), upstreams, statesDocumentAsync, sendInTheThisObject.upstreamConfigurationHasher, forceRefreshAllUpstreamVersionLists);
          tracer.TraceMarker("After RefreshPackageVersionAsync");
          sendInTheThisObject.TracePackageResult(result, startTime, stopwatch.Elapsed, (PushDrivenUpstreamsNotificationTelemetry) null);
          refreshPackageResult = result;
        }
        catch (Exception ex)
        {
          sendInTheThisObject.TracePackageResult(UpstreamMetadataManager<TPackageName, TPackageIdentity, TMetadataEntry>.CreateRefreshPackageResultForUnhandledException((IProtocolAgnosticFeedRequest) feedRequest, packageIdentity.Name, ex, upstreams), startTime, stopwatch.Elapsed, (PushDrivenUpstreamsNotificationTelemetry) null);
          tracer.TraceError(string.Format("Failed to refresh upstreams for feed '{0}', package '{1}', version '{2}', upstreams: '{3}'", (object) feed.Id, (object) packageIdentity.Name, (object) packageIdentity.Version, (object) string.Join<UpstreamSource>(",", upstreams)));
          tracer.TraceException(ex);
          throw;
        }
      }
      feed = (FeedCore) null;
      return refreshPackageResult;
    }

    private static RefreshPackageResult CreateRefreshPackageResultForUnhandledException(
      IProtocolAgnosticFeedRequest feedRequest,
      IPackageName packageName,
      Exception ex,
      IEnumerable<UpstreamSource> upstreams)
    {
      if (!(ex is UpstreamFailureException upstreamFailure))
        upstreamFailure = new UpstreamFailureException(ex.Message, ex, UpstreamStatusCategory.UnknownFailure);
      UpstreamSource upstreamSource = (UpstreamSource) null;
      if (ex is UpstreamFailureWithUpstreamSourceException upstreamSourceException)
      {
        upstreamSource = upstreamSourceException.UpstreamSource;
      }
      else
      {
        Guid? upstreamId = ex.GetRelatedUpstreamSourceIdOrDefault();
        if (upstreamId.HasValue)
          upstreamSource = upstreams.FirstOrDefault<UpstreamSource>((Func<UpstreamSource, bool>) (x => x.Id.Equals(upstreamId.Value)));
      }
      return RefreshPackageResult.Failed(feedRequest.Feed, packageName, upstreamSource, upstreamFailure);
    }

    private void TracePackageResult(
      RefreshPackageResult result,
      DateTime packageRefreshStartTime,
      TimeSpan packageRefreshElapsedTime,
      PushDrivenUpstreamsNotificationTelemetry pushDrivenUpstreamsNotificationTelemetry)
    {
      using (ITracerBlock tracerBlock = this.tracerService.Enter((object) this, nameof (TracePackageResult)))
        tracerBlock.TraceInfoAlways(new string[1]
        {
          "SingleFeedPackageRefreshTelemetry"
        }, JsonConvert.SerializeObject((object) new UpstreamMetadataManager<TPackageName, TPackageIdentity, TMetadataEntry>.SingleFeedPackageRefreshTelemetry(this.executionEnvironment.RequestStartTime, result, packageRefreshStartTime, (int) packageRefreshElapsedTime.TotalMilliseconds, pushDrivenUpstreamsNotificationTelemetry)));
    }

    private async Task ThrowIfResultFailed(
      IFeedRequest feedRequest,
      RefreshPackageResult result,
      IEnumerable<UpstreamSource> upstreams)
    {
      UpstreamMetadataManager<TPackageName, TPackageIdentity, TMetadataEntry> sendInTheThisObject = this;
      ITracerBlock tracer = sendInTheThisObject.tracerService.Enter((object) sendInTheThisObject, nameof (ThrowIfResultFailed));
      try
      {
        if (!result.IsFailed)
        {
          tracer = (ITracerBlock) null;
        }
        else
        {
          if (sendInTheThisObject.featureFlagService.IsEnabled("Packaging.UpstreamStatus.AT"))
          {
            await sendInTheThisObject.upstreamStatusResultHandler.Handle(feedRequest, new UpstreamPackageRefreshResult(result, result.UpstreamFailureException.ErrorCategory, UpstreamRefreshScope.Package), upstreams);
            tracer.TraceMarker("After upstreamStatusResultHandler");
            ExceptionDispatchInfo.Capture((Exception) result.UpstreamFailureException).Throw();
          }
          ExceptionDispatchInfo.Capture(result.UpstreamFailureException.InnerException ?? (Exception) result.UpstreamFailureException).Throw();
          tracer = (ITracerBlock) null;
        }
      }
      finally
      {
        tracer?.Dispose();
      }
    }

    private bool IsUpstreamsEnabled(IFeedRequest feedRequest)
    {
      FeedCore feed = feedRequest.Feed;
      return feed.UpstreamEnabled && feed.GetSourcesForProtocol(feedRequest.Protocol).Any<UpstreamSource>();
    }

    private int GetMaxPackageFailures(IFeedRequest feedRequest) => this.registryService.GetValue<int>(new RegistryQuery(string.Format("/Configuration/Packaging/{0}/UpstreamMetadataCache/MaxPackageFailures", (object) feedRequest.Protocol.CorrectlyCasedName)), 50);

    private List<UpstreamStatistics> AggregateUpstreamStatistics(
      List<RefreshPackageResult> successes)
    {
      Dictionary<Guid, UpstreamStatistics> source = new Dictionary<Guid, UpstreamStatistics>();
      foreach (RefreshPackageResult success in successes)
      {
        foreach (UpstreamStatistics upstreamStatistic in (IEnumerable<UpstreamStatistics>) success.UpstreamStatistics)
        {
          UpstreamStatistics upstreamStatistics;
          if (source.TryGetValue(upstreamStatistic.Id, out upstreamStatistics))
            upstreamStatistics.WaitTimeMs += upstreamStatistic.WaitTimeMs;
          else
            source[upstreamStatistic.Id] = new UpstreamStatistics(upstreamStatistic.Id, upstreamStatistic.Type, upstreamStatistic.WaitTimeMs);
        }
      }
      return source.Select<KeyValuePair<Guid, UpstreamStatistics>, UpstreamStatistics>((Func<KeyValuePair<Guid, UpstreamStatistics>, UpstreamStatistics>) (x => x.Value)).ToList<UpstreamStatistics>();
    }

    private class SingleFeedPackageRefreshTelemetry
    {
      public DateTime RequestStartTime { get; }

      public RefreshPackageResult PackageRefreshResult { get; }

      public DateTime PackageRefreshStartTime { get; }

      public int PackageRefreshElapsedTimeMs { get; }

      public PushDrivenUpstreamsNotificationTelemetry NotificationTelemetry { get; }

      public SingleFeedPackageRefreshTelemetry(
        DateTime requestStartTime,
        RefreshPackageResult packageRefreshResult,
        DateTime packageRefreshStartTime,
        int packageRefreshElapsedTimeMs,
        PushDrivenUpstreamsNotificationTelemetry notificationTelemetry)
      {
        this.RequestStartTime = requestStartTime;
        this.PackageRefreshResult = packageRefreshResult;
        this.PackageRefreshStartTime = packageRefreshStartTime;
        this.PackageRefreshElapsedTimeMs = packageRefreshElapsedTimeMs;
        this.NotificationTelemetry = notificationTelemetry;
      }
    }
  }
}
