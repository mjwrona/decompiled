// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Types.Server.GalleryService
// Assembly: Microsoft.VisualStudio.Services.Gallery.Types.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FF687265-4AE2-4CD2-A134-409D61826008
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Types.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Microsoft.VisualStudio.Services.Gallery.Types.Server
{
  internal class GalleryService : IGalleryService, IVssFrameworkService
  {
    public void ServiceStart(IVssRequestContext requestContext)
    {
      if (!requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        throw new UnexpectedHostTypeException(requestContext.ServiceHost.HostType);
    }

    public void ServiceEnd(IVssRequestContext requestContext)
    {
    }

    public PublishedExtension GetExtension(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName,
      string version,
      ExtensionQueryFlags flags,
      string accountToken = null)
    {
      requestContext.CheckDeploymentRequestContext();
      PublishedExtension extension = (PublishedExtension) null;
      if (!requestContext.IsFeatureEnabled(GallerySdkFeatureFlags.BypassGalleryCalls))
        extension = requestContext.GetClient<GalleryHttpClient>().GetExtensionAsync(publisherName, extensionName, version, new ExtensionQueryFlags?(flags), accountTokenHeader: accountToken).SyncResult<PublishedExtension>();
      return extension;
    }

    public ExtensionQueryResult QueryExtensions(
      IVssRequestContext requestContext,
      ExtensionQuery query)
    {
      requestContext.CheckDeploymentRequestContext();
      ExtensionQueryResult extensionQueryResult = (ExtensionQueryResult) null;
      if (!requestContext.IsFeatureEnabled(GallerySdkFeatureFlags.BypassGalleryCalls))
        extensionQueryResult = requestContext.GetClient<GalleryHttpClient>().QueryExtensionsAsync(query).SyncResult<ExtensionQueryResult>();
      return extensionQueryResult;
    }

    public void UpdateExtensionInstallCount(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName)
    {
      GalleryHttpClient client = requestContext.Elevate().To(TeamFoundationHostType.Deployment).GetClient<GalleryHttpClient>();
      ExtensionStatisticUpdate extensionStatisticUpdate = new ExtensionStatisticUpdate();
      extensionStatisticUpdate.Statistic = new ExtensionStatistic()
      {
        StatisticName = "install",
        Value = 1.0
      };
      extensionStatisticUpdate.Operation = ExtensionStatisticOperation.Increment;
      extensionStatisticUpdate.PublisherName = publisherName;
      extensionStatisticUpdate.ExtensionName = extensionName;
      ExtensionStatisticUpdate extensionStatisticsUpdate = extensionStatisticUpdate;
      string publisherName1 = extensionStatisticUpdate.PublisherName;
      string extensionName1 = extensionStatisticUpdate.ExtensionName;
      CancellationToken cancellationToken = new CancellationToken();
      client.UpdateExtensionStatisticsAsync(extensionStatisticsUpdate, publisherName1, extensionName1, cancellationToken: cancellationToken).SyncResult();
    }

    public PublishedExtension CreateExtension(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName,
      byte[] packageBytes)
    {
      requestContext.CheckDeploymentRequestContext();
      GalleryHttpClient client = requestContext.GetClient<GalleryHttpClient>();
      ExtensionPackage extensionPackage = new ExtensionPackage();
      extensionPackage.ExtensionManifest = Convert.ToBase64String(packageBytes);
      CancellationToken cancellationToken = new CancellationToken();
      return client.CreateExtensionAsync(extensionPackage, cancellationToken: cancellationToken).SyncResult<PublishedExtension>();
    }

    public PublishedExtension UpdateExtension(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName,
      byte[] packageBytes)
    {
      requestContext.CheckDeploymentRequestContext();
      GalleryHttpClient client = requestContext.GetClient<GalleryHttpClient>();
      ExtensionPackage extensionPackage = new ExtensionPackage();
      extensionPackage.ExtensionManifest = Convert.ToBase64String(packageBytes);
      string publisherName1 = publisherName;
      string extensionName1 = extensionName;
      CancellationToken cancellationToken = new CancellationToken();
      return client.UpdateExtensionAsync(extensionPackage, publisherName1, extensionName1, cancellationToken: cancellationToken).SyncResult<PublishedExtension>();
    }

    public void PublishExtensionEvents(
      IVssRequestContext requestContext,
      IEnumerable<ExtensionEvents> extensionEvents)
    {
      if (!requestContext.ExecutionEnvironment.IsHostedDeployment || !requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        return;
      requestContext.GetClient<GalleryHttpClient>().PublishExtensionEventsAsync(extensionEvents).SyncResult();
    }
  }
}
