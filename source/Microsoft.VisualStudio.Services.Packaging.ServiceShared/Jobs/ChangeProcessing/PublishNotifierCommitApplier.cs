// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Jobs.ChangeProcessing.PublishNotifierCommitApplier
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Packaging.CrossProtocol.Internal.WebApi.Types;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Constants;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Ingestion;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;


#nullable enable
namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Jobs.ChangeProcessing
{
  public class PublishNotifierCommitApplier : ICommitApplier
  {
    private readonly IPackagePublishNotifier notifier;
    private readonly ITimeProvider timeProvider;
    private readonly IFeedService feedService;
    private readonly ITracerService tracerService;
    private readonly IImmutableList<IPduNotificationDetector> detectors;

    public PublishNotifierCommitApplier(
      IPackagePublishNotifier notifier,
      ITimeProvider timeProvider,
      IFeedService feedService,
      ITracerService tracerService,
      IImmutableList<IPduNotificationDetector> detectors)
    {
      this.notifier = notifier;
      this.timeProvider = timeProvider;
      this.feedService = feedService;
      this.tracerService = tracerService;
      this.detectors = detectors;
    }

    [ExcludeFromCodeCoverage]
    public static PublishNotifierCommitApplier Bootstrap(
      IVssRequestContext requestContext,
      ITimeProvider timeProvider,
      IEnumerable<IPduNotificationDetector> protocolSpecificDetectors)
    {
      return new PublishNotifierCommitApplier((IPackagePublishNotifier) new PackagePublishNotifier((IMessageBusPublisherFacade) new MessageBusPublisherFacade(requestContext.To(TeamFoundationHostType.Deployment)), requestContext.GetExecutionEnvironmentFacade(), FeatureAvailabilityConstants.PushDrivenUpstreamsEnabled.Bootstrap(requestContext)), timeProvider, (IFeedService) new FeedServiceFacade(requestContext), requestContext.GetTracerFacade(), PublishNotifierCommitApplier.GetDefaultDetectors().AddRange(protocolSpecificDetectors));
    }

    public static IImmutableList<IPduNotificationDetector> GetDefaultDetectors() => (IImmutableList<IPduNotificationDetector>) ImmutableList.Create<IPduNotificationDetector>((IPduNotificationDetector) new AddCommitPduNotificationDetector(), (IPduNotificationDetector) new PromoteCommitPduNotificationDetector(), (IPduNotificationDetector) new ListingStateCommitPduNotificationDetector<IListingStateChangeOperationData>(), (IPduNotificationDetector) new ListingStateCommitPduNotificationDetector<IDeprecateOperationData>());

    public async Task<AggregationApplyTimings> ApplyCommitAsync(
      IFeedRequest feedRequest,
      IReadOnlyList<ICommitLogEntry> commitLogEntries)
    {
      PublishNotifierCommitApplier sendInTheThisObject = this;
      Stopwatch stopwatch;
      AggregationApplyTimings timings;
      AggregationApplyTimings aggregationApplyTimings;
      using (sendInTheThisObject.tracerService.Enter((object) sendInTheThisObject, nameof (ApplyCommitAsync)))
      {
        stopwatch = Stopwatch.StartNew();
        timings = new AggregationApplyTimings();
        List<ICommitLogEntry> flattenedCommits = commitLogEntries.FlattenBatches().ToList<ICommitLogEntry>();
        List<PendingPduNotification> list = sendInTheThisObject.detectors.SelectMany<IPduNotificationDetector, PendingPduNotification>((Func<IPduNotificationDetector, IEnumerable<PendingPduNotification>>) (detector => detector.GetNotificationsForCommits((IEnumerable<ICommitLogEntry>) flattenedCommits))).ToList<PendingPduNotification>();
        IReadOnlyList<PendingPduNotification> pendingPduNotificationList = await sendInTheThisObject.FlattenViewsAsync(feedRequest, (IReadOnlyList<PendingPduNotification>) list);
        foreach (IGrouping<TriggerCommitType, PendingPduNotification> source in pendingPduNotificationList.GroupBy<PendingPduNotification, TriggerCommitType>((Func<PendingPduNotification, TriggerCommitType>) (x => x.CommitType)))
          timings.AddTime(string.Format("PublishNotifier.{0}Notifications.Count.Not", (object) source.Key), (long) source.Count<PendingPduNotification>());
        await sendInTheThisObject.SendNotifications(feedRequest, (IEnumerable<PendingPduNotification>) pendingPduNotificationList);
        timings.AddTime("PublishNotifier", stopwatch.ElapsedMilliseconds);
        aggregationApplyTimings = timings;
      }
      stopwatch = (Stopwatch) null;
      timings = (AggregationApplyTimings) null;
      return aggregationApplyTimings;
    }

    private async Task<IReadOnlyList<PendingPduNotification>> FlattenViewsAsync(
      IFeedRequest feedRequest,
      IReadOnlyList<PendingPduNotification> source)
    {
      FeedView viewOrDefaultAsync = await this.feedService.GetLocalViewOrDefaultAsync(feedRequest.Feed);
      CommitAffectsViews.Specific[] specificArray;
      if (viewOrDefaultAsync == null)
        specificArray = Array.Empty<CommitAffectsViews.Specific>();
      else
        specificArray = new CommitAffectsViews.Specific[1]
        {
          new CommitAffectsViews.Specific(viewOrDefaultAsync.Id)
        };
      IReadOnlyList<CommitAffectsViews.Specific> viewsToNotifyWhenCommitAffectsLocalOnly = (IReadOnlyList<CommitAffectsViews.Specific>) specificArray;
      IReadOnlyList<CommitAffectsViews.Specific> specificList1;
      if (source.Any<PendingPduNotification>((Func<PendingPduNotification, bool>) (x => x.CommitAffectsViews is CommitAffectsViews.All)))
        specificList1 = (IReadOnlyList<CommitAffectsViews.Specific>) (await this.feedService.GetViewsAsync(feedRequest.Feed)).Select<FeedView, CommitAffectsViews.Specific>((Func<FeedView, CommitAffectsViews.Specific>) (x => new CommitAffectsViews.Specific(x.Id))).ToList<CommitAffectsViews.Specific>();
      else
        specificList1 = viewsToNotifyWhenCommitAffectsLocalOnly;
      List<PendingPduNotification> pendingPduNotificationList1 = new List<PendingPduNotification>();
      foreach (PendingPduNotification pendingPduNotification in (IEnumerable<PendingPduNotification>) source)
      {
        PendingPduNotification notification = pendingPduNotification;
        CommitAffectsViews commitAffectsViews = notification.CommitAffectsViews;
        IReadOnlyList<CommitAffectsViews.Specific> specificList2;
        if ((object) (commitAffectsViews as CommitAffectsViews.All) == null)
        {
          if ((object) (commitAffectsViews as CommitAffectsViews.LocalOnly) == null)
            specificList2 = (IReadOnlyList<CommitAffectsViews.Specific>) new CommitAffectsViews.Specific[1]
            {
              commitAffectsViews as CommitAffectsViews.Specific ?? throw new ArgumentOutOfRangeException()
            };
          else
            specificList2 = viewsToNotifyWhenCommitAffectsLocalOnly;
        }
        else
          specificList2 = specificList1;
        IReadOnlyList<CommitAffectsViews.Specific> source1 = specificList2;
        pendingPduNotificationList1.AddRange(source1.Select<CommitAffectsViews.Specific, PendingPduNotification>((Func<CommitAffectsViews.Specific, PendingPduNotification>) (view => notification with
        {
          CommitAffectsViews = (CommitAffectsViews) view
        })));
      }
      IReadOnlyList<PendingPduNotification> pendingPduNotificationList2 = (IReadOnlyList<PendingPduNotification>) pendingPduNotificationList1;
      viewsToNotifyWhenCommitAffectsLocalOnly = (IReadOnlyList<CommitAffectsViews.Specific>) null;
      return pendingPduNotificationList2;
    }

    private async Task SendNotifications(
      IFeedRequest feedRequest,
      IEnumerable<PendingPduNotification> notifications)
    {
      DateTime now = this.timeProvider.Now;
      foreach (PendingPduNotification notification in notifications)
      {
        CommitAffectsViews.Specific commitAffectsViews = (CommitAffectsViews.Specific) notification.CommitAffectsViews;
        await this.notifier.NotifyPublishAsync(feedRequest, commitAffectsViews.ViewId, notification.PackageIdentity, notification.Commit.CreatedDate, now, notification.CommitType);
      }
    }
  }
}
