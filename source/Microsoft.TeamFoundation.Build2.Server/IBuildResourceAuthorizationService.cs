// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.IBuildResourceAuthorizationService
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Build2.Server
{
  [DefaultServiceImplementation(typeof (BuildResourceAuthorizationService))]
  public interface IBuildResourceAuthorizationService : IVssFrameworkService
  {
    Task<BuildProcessResources> GetAuthorizedResourcesAsync(
      IVssRequestContext requestContext,
      Guid projectId,
      ResourceType? resourceType = null,
      string resourceId = null);

    Task<BuildProcessResources> GetAuthorizedResourcesAsync(
      IVssRequestContext requestContext,
      Guid projectId,
      int definitionId,
      ResourceType? resourceType = null,
      string resourceId = null);

    Task<BuildProcessResources> GetResourcesAsync(
      IVssRequestContext requestContext,
      Guid projectId,
      int definitionId);

    Task<BuildProcessResources> UpdateResourcesAsync(
      IVssRequestContext requestContext,
      Guid projectId,
      int definitionId,
      BuildProcessResources resourcesToUpdate);

    Task<BuildProcessResources> UpdateResourcesAsync(
      IVssRequestContext requestContext,
      Guid projectId,
      BuildProcessResources resourcesToUpdate);
  }
}
