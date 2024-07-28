// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Facade.PMP.IndexUpdater.VSCodeEventPublisherService
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.BusinessIntelligence;
using Microsoft.TeamFoundation.Framework.Server.Threading;
using Microsoft.VisualStudio.Services.Gallery.Server.Facade.PMP.IndexUpdater.PMPEvents;
using Microsoft.VisualStudio.Services.Gallery.Server.Facade.PMP.IndexUpdater.PMPEvents.PMPEventData;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Facade.PMP.IndexUpdater
{
  internal class VSCodeEventPublisherService : IEventPublisherService, IVssFrameworkService
  {
    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public void PublishArtifactFilePublishedEvent(
      IVssRequestContext requestContext,
      string extensionName,
      string version,
      string targetPlatform = null)
    {
      this.PublishEvent<ArtifactFileEventData>(requestContext, (PMPEvent<ArtifactFileEventData>) new ArtifactFilePublishedEvent("https://marketplace.visualstudio.com", new ArtifactFileEventData(extensionName, version, targetPlatform, "vscode")));
    }

    public void PublishPackageDeletedEvent(
      IVssRequestContext requestContext,
      string extensionName,
      string version)
    {
      this.PublishEvent<PackageEventData>(requestContext, (PMPEvent<PackageEventData>) new PackageDeletedEvent("https://marketplace.visualstudio.com", new PackageEventData("vscode", extensionName, version)));
    }

    public void PublishPackageAggregateDeletedEvent(
      IVssRequestContext requestContext,
      string extensionName)
    {
      this.PublishEvent<PackageAggregateEventData>(requestContext, (PMPEvent<PackageAggregateEventData>) new PackageAggregateDeletedEvent("https://marketplace.visualstudio.com", new PackageAggregateEventData("vscode", extensionName)));
    }

    public void PublishPackageAggregateLockedEvent(
      IVssRequestContext requestContext,
      string extensionName)
    {
      this.PublishEvent<PackageAggregateEventData>(requestContext, (PMPEvent<PackageAggregateEventData>) new PackageAggregateLockedEvent("https://marketplace.visualstudio.com", new PackageAggregateEventData("vscode", extensionName)));
    }

    public void PublishPackageAggregateUnlockedEvent(
      IVssRequestContext requestContext,
      string extensionName)
    {
      this.PublishEvent<PackageAggregateEventData>(requestContext, (PMPEvent<PackageAggregateEventData>) new PackageAggregateUnlockedEvent("https://marketplace.visualstudio.com", new PackageAggregateEventData("vscode", extensionName)));
    }

    public void PublishPackageAggregateArchivedEvent(
      IVssRequestContext requestContext,
      string extensionName)
    {
      this.PublishEvent<PackageAggregateEventData>(requestContext, (PMPEvent<PackageAggregateEventData>) new PackageAggregateArchivedEvent("https://marketplace.visualstudio.com", new PackageAggregateEventData("vscode", extensionName)));
    }

    public void PublishPackageAggregateUnarchivedEvent(
      IVssRequestContext requestContext,
      string extensionName)
    {
      this.PublishEvent<PackageAggregateEventData>(requestContext, (PMPEvent<PackageAggregateEventData>) new PackageAggregateUnarchivedEvent("https://marketplace.visualstudio.com", new PackageAggregateEventData("vscode", extensionName)));
    }

    public void PublishPackageAggregateForceIndexEvent(
      IVssRequestContext requestContext,
      string extensionName)
    {
      this.PublishEvent<PackageAggregateEventData>(requestContext, (PMPEvent<PackageAggregateEventData>) new PackageAggregateForcedIndexEvent("https://marketplace.visualstudio.com", new PackageAggregateEventData("vscode", extensionName)));
    }

    public void PublishPublisherUpdatedEvent(
      IVssRequestContext requestContext,
      string publisherName)
    {
      this.PublishEvent<PublisherEventData>(requestContext, (PMPEvent<PublisherEventData>) new PublisherUpdatedEvent("https://marketplace.visualstudio.com", new PublisherEventData(publisherName)));
    }

    private void PublishEvent<T>(IVssRequestContext requestContext, PMPEvent<T> pmpEvent)
    {
      VSCodeEventPublisherService.PublishTelemetryForIndexUpdater<T>(requestContext, pmpEvent, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Started publishing message for the event {0}", (object) pmpEvent.Id));
      try
      {
        IMessagePublisher serviceBusMessagePublisher = requestContext.GetService<IMessagePublisher>();
        requestContext.RunSynchronously((Func<Task>) (() => serviceBusMessagePublisher.PublishEventAsync<T>(requestContext, pmpEvent)));
      }
      catch (Exception ex)
      {
        requestContext.Trace(12062102, TraceLevel.Info, "gallery", nameof (VSCodeEventPublisherService), string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Unable to publish message for the event {0} ", (object) pmpEvent.Id, (object) ex.ToString()));
        VSCodeEventPublisherService.PublishTelemetryForIndexUpdater<T>(requestContext, pmpEvent, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Error while publishing message for the event {0}", (object) pmpEvent.Id));
      }
      VSCodeEventPublisherService.PublishTelemetryForIndexUpdater<T>(requestContext, pmpEvent, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Completed publishing message for the event {0}", (object) pmpEvent.Id));
    }

    private static void PublishTelemetryForIndexUpdater<T>(
      IVssRequestContext requestContext,
      PMPEvent<T> pmpEvent,
      string message,
      Exception exception = null)
    {
      ClientTraceData properties = new ClientTraceData();
      if (!string.IsNullOrEmpty(message))
        properties.Add(nameof (message), (object) message);
      if (exception != null)
        properties.Add(nameof (exception), (object) exception);
      properties.Add("E2EId", (object) requestContext.E2EId);
      properties.Add("EventData", (object) pmpEvent);
      requestContext.GetService<ClientTraceService>().Publish(requestContext, "Microsoft.VisualStudio.Services.Gallery", "IndexUpdater", properties);
    }
  }
}
