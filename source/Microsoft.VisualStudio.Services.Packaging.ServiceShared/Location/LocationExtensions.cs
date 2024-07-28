// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Location.LocationExtensions
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework.Facades.Interfaces;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Location
{
  public static class LocationExtensions
  {
    public static IResourceUriBinder GetUnboundResourceUri(
      this ILocationDataProvider locationProvider,
      IVssRequestContext requestContext,
      string serviceType,
      Guid identifier,
      bool appendUnusedAsQueryParams,
      bool requireExplicitRouteParams)
    {
      ServiceDefinition serviceDefinition = locationProvider.FindServiceDefinition(requestContext, serviceType, identifier);
      if (serviceDefinition == null)
        throw new VssResourceNotFoundException(identifier);
      string relativePathTemplate = serviceDefinition.RelativeToSetting != RelativeToSetting.FullyQualified ? serviceDefinition.RelativePath : throw new InvalidOperationException("Unexpected FullyQualified resource location");
      serviceDefinition.RelativePath = string.Empty;
      string rootPath = locationProvider.LocationForAccessMapping(requestContext, serviceDefinition, locationProvider.DetermineAccessMapping(requestContext));
      return (IResourceUriBinder) new ResourceUriBinder(serviceType, serviceDefinition.DisplayName, rootPath, relativePathTemplate, appendUnusedAsQueryParams, requireExplicitRouteParams);
    }

    public static IResourceUriBinder GetUnboundResourceUri(
      this ILocationDataProvider locationProvider,
      IVssRequestContext requestContext,
      string serviceType,
      Guid identifier)
    {
      return locationProvider.GetUnboundResourceUri(requestContext, serviceType, identifier, false, false);
    }

    public static Uri Bind(
      this IResourceUriBinder binder,
      IFeedRequest feedRequest,
      PackagingUriNamePreference namePreference,
      object routeValues)
    {
      return binder.Bind((object) LocationExtensions.GetRouteDictionaryWithFeedParams(feedRequest, namePreference, routeValues));
    }

    public static Uri GetResourceUri(
      this ILocationFacade locationFacade,
      string serviceType,
      Guid identifier,
      IFeedRequest feedRequest,
      PackagingUriNamePreference namePreference,
      object routeValues)
    {
      return locationFacade.GetResourceUri(serviceType, identifier, (object) LocationExtensions.GetRouteDictionaryWithFeedParams(feedRequest, namePreference, routeValues));
    }

    private static Dictionary<string, object> GetRouteDictionaryWithFeedParams(
      IFeedRequest feedRequest,
      PackagingUriNamePreference namePreference,
      object routeValues)
    {
      Dictionary<string, object> routeDictionary = VssHttpUriUtility.ToRouteDictionary(routeValues);
      string feedNameOrIdForUri = feedRequest.GetFeedNameOrIdForUri(namePreference);
      routeDictionary.TryAdd<string, object>("feed", (object) feedNameOrIdForUri);
      routeDictionary.TryAdd<string, object>("feedId", (object) feedNameOrIdForUri);
      string projectNameOrIdForUri = feedRequest.GetProjectNameOrIdForUri(namePreference);
      if (!string.IsNullOrWhiteSpace(projectNameOrIdForUri))
        routeDictionary.TryAdd<string, object>("project", (object) projectNameOrIdForUri);
      return routeDictionary;
    }

    public static Uri GetHostBaseUri(this ILocationFacade locationService) => new Uri(locationService.GetLocationServiceUrl(locationService.InstanceType, AccessMappingConstants.ClientAccessMappingMoniker));
  }
}
