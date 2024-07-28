// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.IBuildResourceAuthorizationServiceExtensions
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Threading;
using System;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Build2.Server
{
  public static class IBuildResourceAuthorizationServiceExtensions
  {
    public static BuildProcessResources GetAuthorizedResources(
      this IBuildResourceAuthorizationService resourceService,
      IVssRequestContext requestContext,
      Guid projectId,
      ResourceType? resourceType = null,
      string resourceId = null)
    {
      return requestContext.RunSynchronously<BuildProcessResources>((Func<Task<BuildProcessResources>>) (() => resourceService.GetAuthorizedResourcesAsync(requestContext, projectId, resourceType, resourceId)));
    }

    public static BuildProcessResources GetAuthorizedResources(
      this IBuildResourceAuthorizationService resourceService,
      IVssRequestContext requestContext,
      Guid projectId,
      int definitionId,
      ResourceType? resourceType = null,
      string resourceId = null)
    {
      return requestContext.RunSynchronously<BuildProcessResources>((Func<Task<BuildProcessResources>>) (() => resourceService.GetAuthorizedResourcesAsync(requestContext, projectId, definitionId, resourceType, resourceId)));
    }

    public static BuildProcessResources GetResources(
      this IBuildResourceAuthorizationService resourceService,
      IVssRequestContext requestContext,
      Guid projectId,
      int definitionId)
    {
      return requestContext.RunSynchronously<BuildProcessResources>((Func<Task<BuildProcessResources>>) (() => resourceService.GetResourcesAsync(requestContext, projectId, definitionId)));
    }

    public static BuildProcessResources UpdateResources(
      this IBuildResourceAuthorizationService resourceService,
      IVssRequestContext requestContext,
      Guid projectId,
      int definitionId,
      BuildProcessResources resourcesToUpdate)
    {
      return requestContext.RunSynchronously<BuildProcessResources>((Func<Task<BuildProcessResources>>) (() => resourceService.UpdateResourcesAsync(requestContext, projectId, definitionId, resourcesToUpdate)));
    }

    public static BuildProcessResources UpdateResources(
      this IBuildResourceAuthorizationService resourceService,
      IVssRequestContext requestContext,
      Guid projectId,
      BuildProcessResources resourcesToUpdate)
    {
      return requestContext.RunSynchronously<BuildProcessResources>((Func<Task<BuildProcessResources>>) (() => resourceService.UpdateResourcesAsync(requestContext, projectId, resourcesToUpdate)));
    }
  }
}
