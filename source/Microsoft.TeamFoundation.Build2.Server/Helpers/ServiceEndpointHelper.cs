// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.Helpers.ServiceEndpointHelper
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server;
using Microsoft.TeamFoundation.Build2.Routes;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;

namespace Microsoft.TeamFoundation.Build2.Server.Helpers
{
  public class ServiceEndpointHelper
  {
    public static ServiceEndpointExecutionOwner GetOwnerReference(
      IVssRequestContext requestContext,
      ISecuredObject securedObject,
      Guid projectId,
      int buildId,
      string buildNumber)
    {
      ArgumentUtility.CheckForNull<ISecuredObject>(securedObject, nameof (securedObject));
      ServiceEndpointExecutionOwner ownerReference = new ServiceEndpointExecutionOwner();
      ownerReference.Id = buildId;
      ownerReference.Name = buildNumber;
      IBuildRouteService routeService = requestContext.GetService<IBuildRouteService>();
      ownerReference.Links.TryAddLink("web", securedObject, (Func<string>) (() => routeService.GetBuildWebUrl(requestContext, projectId, buildId)));
      ownerReference.Links.TryAddLink("self", securedObject, (Func<string>) (() => routeService.GetBuildRestUrl(requestContext, projectId, buildId)));
      return ownerReference;
    }

    public static ServiceEndpointExecutionOwner GetDefinitionReference(
      IVssRequestContext requestContext,
      ISecuredObject securedObject,
      Guid projectId,
      int definitionId,
      string definitionName)
    {
      ArgumentUtility.CheckForNull<ISecuredObject>(securedObject, nameof (securedObject));
      ServiceEndpointExecutionOwner definitionReference = new ServiceEndpointExecutionOwner();
      definitionReference.Id = definitionId;
      definitionReference.Name = definitionName;
      IBuildRouteService routeService = requestContext.GetService<IBuildRouteService>();
      definitionReference.Links.TryAddLink("web", securedObject, (Func<string>) (() => routeService.GetDefinitionWebUrl(requestContext, projectId, definitionId)));
      definitionReference.Links.TryAddLink("self", securedObject, (Func<string>) (() => routeService.GetDefinitionRestUrl(requestContext, projectId, definitionId)));
      return definitionReference;
    }

    public static ServiceEndpoint GetServiceEndpointWithDetails(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid connectionId,
      bool elevateRequest = false)
    {
      if (!elevateRequest && requestContext.GetService<IServiceEndpointService2>().GetServiceEndpoint(requestContext, projectId, connectionId, ServiceEndpointActionFilter.Use) == null)
        return (ServiceEndpoint) null;
      IVssRequestContext vssRequestContext = requestContext.Elevate();
      return vssRequestContext.GetService<IServiceEndpointService2>().GetServiceEndpoint(vssRequestContext, projectId, connectionId);
    }
  }
}
