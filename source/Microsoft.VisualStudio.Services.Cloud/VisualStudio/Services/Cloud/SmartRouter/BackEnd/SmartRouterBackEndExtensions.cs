// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.SmartRouter.BackEnd.SmartRouterBackEndExtensions
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Cloud.SmartRouter.Common;
using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.Location.Server;
using System;


#nullable enable
namespace Microsoft.VisualStudio.Services.Cloud.SmartRouter.BackEnd
{
  internal static class SmartRouterBackEndExtensions
  {
    public static (IServerNodeDiscoveryService discoveryService, IServerNodePublisherService publisherService, IServerNodeHealthService healthService) GetSmartRouterBackEndServices(
      this IVssRequestContext requestContext)
    {
      IServerNodeDiscoveryService discoveryService = requestContext.GetServerNodeDiscoveryService();
      IServerNodePublisherService publisherService1 = requestContext.GetServerNodePublisherService();
      IServerNodeHealthService nodeHealthService1 = requestContext.GetServerNodeHealthService();
      IServerNodePublisherService publisherService2 = publisherService1;
      IServerNodeHealthService nodeHealthService2 = nodeHealthService1;
      return (discoveryService, publisherService2, nodeHealthService2);
    }

    public static IServerNodeDiscoveryService GetServerNodeDiscoveryService(
      this IVssRequestContext requestContext)
    {
      return requestContext.RequireDeploymentLevelService<IServerNodeDiscoveryService>();
    }

    public static IServerNodePublisherService GetServerNodePublisherService(
      this IVssRequestContext requestContext)
    {
      return requestContext.RequireDeploymentLevelService<IServerNodePublisherService>();
    }

    public static IServerNodeHealthService GetServerNodeHealthService(
      this IVssRequestContext requestContext)
    {
      return requestContext.RequireDeploymentLevelService<IServerNodeHealthService>();
    }

    public static string GetAzureInstanceMappingAccessPoint(this IVssRequestContext requestContext) => requestContext.GetService<ILocationService>().GetAccessMapping(requestContext, AccessMappingConstants.AzureInstanceMappingMoniker).AccessPoint;

    public static string GetPublicAccessMappingAccessPoint(this IVssRequestContext requestContext) => requestContext.GetService<ILocationService>().GetAccessMapping(requestContext, AccessMappingConstants.PublicAccessMappingMoniker).AccessPoint;

    internal static SmartRouterBackgroundJob AsSmartRouterBackgroundJob(
      this IVssFrameworkService service)
    {
      return service is IHasSmartRouterBackgroundJob routerBackgroundJob ? routerBackgroundJob.BackgroundJob : throw new ArgumentException("Service is not IHasSmartRouterBackgroundJob", nameof (service));
    }

    private static TService RequireDeploymentLevelService<TService>(
      this IVssRequestContext requestContext)
      where TService : class, IVssFrameworkService
    {
      return requestContext.CheckRequestContext(true).GetService<TService>();
    }
  }
}
