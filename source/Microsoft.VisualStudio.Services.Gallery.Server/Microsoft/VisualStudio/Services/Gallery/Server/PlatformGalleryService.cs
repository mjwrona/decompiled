// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.PlatformGalleryService
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.Types.Server;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  internal class PlatformGalleryService : IGalleryService, IVssFrameworkService
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
      return requestContext.GetService<PublishedExtensionService>().QueryExtension(requestContext, publisherName, extensionName, version, flags, accountToken, false);
    }

    public ExtensionQueryResult QueryExtensions(
      IVssRequestContext requestContext,
      ExtensionQuery query)
    {
      requestContext.CheckDeploymentRequestContext();
      return requestContext.GetService<IPublishedExtensionService>().QueryExtensions(requestContext, query, (string) null);
    }

    public void UpdateExtensionInstallCount(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName)
    {
      Microsoft.VisualStudio.Services.Identity.Identity userIdentity = requestContext.GetUserIdentity();
      if ((requestContext.ExecutionEnvironment.IsHostedDeployment || !IdentityHelper.IsWellKnownGroup(userIdentity.Descriptor, GroupWellKnownIdentityDescriptors.ServiceUsersGroup)) && !ServicePrincipals.IsServicePrincipal(requestContext, requestContext.UserContext))
        return;
      List<ExtensionStatisticUpdate> extensionStatisticUpdateList = new List<ExtensionStatisticUpdate>();
      IExtensionStatisticService service = requestContext.GetService<IExtensionStatisticService>();
      extensionStatisticUpdateList.Add(new ExtensionStatisticUpdate()
      {
        Statistic = new ExtensionStatistic()
        {
          StatisticName = "install",
          Value = 1.0
        },
        Operation = ExtensionStatisticOperation.Increment,
        PublisherName = publisherName,
        ExtensionName = extensionName
      });
      IVssRequestContext requestContext1 = requestContext;
      List<ExtensionStatisticUpdate> statistics = extensionStatisticUpdateList;
      service.UpdateStatistics(requestContext1, (IEnumerable<ExtensionStatisticUpdate>) statistics);
    }

    public PublishedExtension CreateExtension(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName,
      byte[] packageBytes)
    {
      requestContext.CheckDeploymentRequestContext();
      IPublishedExtensionService service = requestContext.GetService<IPublishedExtensionService>();
      IVssRequestContext requestContext1 = requestContext;
      ExtensionPackage extensionPackage = new ExtensionPackage();
      extensionPackage.ExtensionManifest = Convert.ToBase64String(packageBytes);
      string requestingPublisherName = publisherName;
      return service.CreateExtension(requestContext1, extensionPackage, requestingPublisherName);
    }

    public PublishedExtension UpdateExtension(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName,
      byte[] packageBytes)
    {
      requestContext.CheckDeploymentRequestContext();
      IPublishedExtensionService service = requestContext.GetService<IPublishedExtensionService>();
      IVssRequestContext requestContext1 = requestContext;
      ExtensionPackage extensionPackage = new ExtensionPackage();
      extensionPackage.ExtensionManifest = Convert.ToBase64String(packageBytes);
      string requestedExtensionName = extensionName;
      string requestingPublisherName = publisherName;
      return service.UpdateExtension(requestContext1, extensionPackage, requestedExtensionName, requestingPublisherName);
    }

    public void PublishExtensionEvents(
      IVssRequestContext requestContext,
      IEnumerable<ExtensionEvents> extensionEvents)
    {
    }
  }
}
