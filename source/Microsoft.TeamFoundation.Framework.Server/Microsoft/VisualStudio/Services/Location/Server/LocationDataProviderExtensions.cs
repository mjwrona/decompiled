// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Location.Server.LocationDataProviderExtensions
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Location.Server
{
  public static class LocationDataProviderExtensions
  {
    internal static ServiceDefinition FindServiceDefinitionWithFaultIn(
      this ILocationDataProvider provider,
      IVssRequestContext requestContext,
      string serviceType,
      Guid identifier,
      bool previewFaultIn)
    {
      IInternalLocationDataProvider locationDataProvider = provider.GetInternal(requestContext);
      return locationDataProvider != null ? locationDataProvider.FindServiceDefinitionWithFaultIn(requestContext, serviceType, identifier, previewFaultIn) : provider.FindServiceDefinition(requestContext, serviceType, identifier);
    }

    internal static AccessMapping ConfigureAccessMapping(
      this ILocationDataProvider provider,
      IVssRequestContext requestContext,
      AccessMapping accessMapping,
      bool makeDefault,
      bool allowOverlapping)
    {
      IInternalLocationDataProvider locationDataProvider = provider.GetInternal(requestContext);
      return locationDataProvider != null ? locationDataProvider.ConfigureAccessMapping(requestContext, accessMapping, makeDefault, allowOverlapping) : provider.ConfigureAccessMapping(requestContext, accessMapping, makeDefault, false);
    }

    private static IInternalLocationDataProvider GetInternal(
      this ILocationDataProvider provider,
      IVssRequestContext requestContext)
    {
      switch (provider)
      {
        case ILocationService locationService:
          pattern_0 = locationService.GetLocationData(requestContext, Guid.Empty) as IInternalLocationDataProvider;
          break;
      }
      return pattern_0;
    }

    public static Uri GetResourceUri(
      this ILocationDataProvider provider,
      IVssRequestContext requestContext,
      string serviceType,
      Guid identifier,
      object routeValues,
      AccessMapping accessMapping)
    {
      ServiceDefinition serviceDefinition = provider.FindServiceDefinition(requestContext, serviceType, identifier);
      if (serviceDefinition == null)
        throw new VssResourceNotFoundException(identifier);
      Dictionary<string, object> routeDictionary = VssHttpUriUtility.ToRouteDictionary(routeValues, serviceType, serviceDefinition.DisplayName);
      serviceDefinition.RelativePath = VssHttpUriUtility.ReplaceRouteValues(serviceDefinition.RelativePath, routeDictionary, RouteReplacementOptions.None);
      return new Uri(provider.LocationForAccessMapping(requestContext, serviceDefinition, accessMapping));
    }

    public static Uri GetRelativeResourceUri(
      this ILocationDataProvider provider,
      IVssRequestContext requestContext,
      string serviceType,
      Guid identifier,
      object routeValues)
    {
      ServiceDefinition serviceDefinition = provider.FindServiceDefinition(requestContext, serviceType, identifier);
      if (serviceDefinition == null)
        throw new VssResourceNotFoundException(identifier);
      Dictionary<string, object> routeDictionary = VssHttpUriUtility.ToRouteDictionary(routeValues, serviceType, serviceDefinition.DisplayName);
      return new Uri(VssHttpUriUtility.ReplaceRouteValues(serviceDefinition.RelativePath, routeDictionary, RouteReplacementOptions.None), UriKind.Relative);
    }
  }
}
