// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Ingestion.PackagePublishNotifier
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.Common.Notification;
using Microsoft.VisualStudio.Services.Packaging.CrossProtocol.Internal.WebApi.Types;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Settings;
using System;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Ingestion
{
  public class PackagePublishNotifier : IPackagePublishNotifier
  {
    private const string PublisherName = "Microsoft.VisualStudio.Services.Packaging.Ingestion";
    private readonly IMessageBusPublisherFacade messageBusPublisher;
    private readonly IExecutionEnvironment executionEnvironment;
    private readonly IOrgLevelPackagingSetting<bool> enabledSetting;

    public PackagePublishNotifier(
      IMessageBusPublisherFacade messageBusPublisher,
      IExecutionEnvironment executionEnvironment,
      IOrgLevelPackagingSetting<bool> enabledSetting)
    {
      this.messageBusPublisher = (messageBusPublisher.HostType & TeamFoundationHostType.Deployment) != TeamFoundationHostType.Unknown ? messageBusPublisher : throw new ArgumentException("The PackagePublishNotifier must be used with a deployment-level IMessageBusPublisherFacade.", nameof (messageBusPublisher));
      this.executionEnvironment = executionEnvironment;
      this.enabledSetting = enabledSetting;
    }

    public async Task NotifyPublishAsync(
      IFeedRequest request,
      Guid viewId,
      IPackageIdentity packageIdentity,
      DateTime ingestionTimestamp,
      DateTime notificationTimestamp,
      TriggerCommitType triggerCommitType)
    {
      if (!this.enabledSetting.Get())
        return;
      await this.messageBusPublisher.PublishAsync("Microsoft.VisualStudio.Services.Packaging.Ingestion", new object[1]
      {
        (object) new PackagePublishNotificationMessage()
        {
          CollectionId = this.executionEnvironment.HostId,
          ProjectId = request.Feed.Project?.Id,
          FeedId = request.Feed.Id,
          ViewId = viewId,
          Protocol = request.Protocol.CorrectlyCasedName,
          NormalizedPackageName = packageIdentity.Name.NormalizedName,
          PushDrivenUpstreamsTelemetry = new PushDrivenUpstreamsNotificationTelemetry()
          {
            PackageVersion = packageIdentity.Version.NormalizedVersion,
            UpstreamFeedId = new Guid?(request.Feed.Id),
            UpstreamViewId = new Guid?(viewId),
            TriggerCommitType = triggerCommitType,
            IngestionTimestamp = ingestionTimestamp,
            UpstreamToFeedNotificationSendTimestamp = notificationTimestamp,
            UpstreamToFeedNotificationSendActivityId = this.executionEnvironment.ActivityId
          }
        }
      });
    }
  }
}
