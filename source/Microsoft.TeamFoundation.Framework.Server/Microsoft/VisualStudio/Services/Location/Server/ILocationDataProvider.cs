// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Location.Server.ILocationDataProvider
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Location.Server
{
  public interface ILocationDataProvider
  {
    Guid HostId { get; }

    Guid InstanceType { get; }

    void SaveServiceDefinitions(
      IVssRequestContext requestContext,
      IEnumerable<ServiceDefinition> serviceDefinitions);

    void RemoveServiceDefinitions(
      IVssRequestContext requestContext,
      IEnumerable<ServiceDefinition> serviceDefinitions);

    ServiceDefinition FindServiceDefinition(
      IVssRequestContext requestContext,
      string serviceType,
      Guid identifier);

    ServiceDefinition FindServiceDefinition(
      IVssRequestContext requestContext,
      string serviceType,
      string toolId);

    IEnumerable<ServiceDefinition> FindServiceDefinitions(
      IVssRequestContext requestContext,
      string serviceType);

    IEnumerable<ServiceDefinition> FindServiceDefinitionsByToolId(
      IVssRequestContext requestContext,
      string toolId);

    string LocationForAccessMapping(
      IVssRequestContext requestContext,
      string serviceType,
      Guid serviceIdentifier,
      AccessMapping accessMapping);

    string LocationForAccessMapping(
      IVssRequestContext requestContext,
      string serviceType,
      string toolId,
      AccessMapping accessMapping);

    string LocationForAccessMapping(
      IVssRequestContext requestContext,
      string relativePath,
      RelativeToSetting relativeToSetting,
      AccessMapping accessMapping);

    string LocationForAccessMapping(
      IVssRequestContext requestContext,
      ServiceDefinition serviceDefinition,
      AccessMapping accessMapping);

    AccessMapping ConfigureAccessMapping(
      IVssRequestContext requestContext,
      AccessMapping accessMapping,
      bool makeDefault);

    void SetDefaultAccessMapping(IVssRequestContext requestContext, AccessMapping accessMapping);

    AccessMapping GetPublicAccessMapping(IVssRequestContext requestContext);

    AccessMapping GetServerAccessMapping(IVssRequestContext requestContext);

    AccessMapping GetDefaultAccessMapping(IVssRequestContext requestContext);

    AccessMapping DetermineAccessMapping(IVssRequestContext requestContext);

    AccessMapping GetAccessMapping(IVssRequestContext requestContext, string moniker);

    IEnumerable<AccessMapping> GetAccessMappings(IVssRequestContext requestContext);

    void RemoveAccessMapping(IVssRequestContext requestContext, AccessMapping accessMapping);

    string GetSelfReferenceUrl(IVssRequestContext requestContext, AccessMapping accessMapping);

    ApiResourceLocationCollection GetResourceLocations(IVssRequestContext requestContext);

    Uri GetResourceUri(
      IVssRequestContext requestContext,
      string serviceType,
      Guid identifier,
      object routeValues);

    Uri GetResourceUri(
      IVssRequestContext requestContext,
      string serviceType,
      Guid identifier,
      object routeValues,
      bool requireExplicitRouteParams);

    Uri GetResourceUri(
      IVssRequestContext requestContext,
      string serviceType,
      Guid identifier,
      object routeValues,
      bool appendUnusedAsQueryParams,
      bool requireExplicitRouteParams);

    Uri GetResourceUri(
      IVssRequestContext requestContext,
      string serviceType,
      Guid identifier,
      object routeValues,
      bool appendUnusedAsQueryParams,
      bool requireExplicitRouteParams,
      bool wildcardAsQueryParams);

    string GetWebApplicationRelativeDirectory(IVssRequestContext requestContext);

    DateTime GetExpirationDate(IVssRequestContext requestContext);

    long GetLastChangeId(IVssRequestContext requestContext);
  }
}
