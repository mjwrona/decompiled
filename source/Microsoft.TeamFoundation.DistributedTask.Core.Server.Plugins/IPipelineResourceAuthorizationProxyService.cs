// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Core.Server.Plugins.IPipelineResourceAuthorizationProxyService
// Assembly: Microsoft.TeamFoundation.DistributedTask.Core.Server.Plugins, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6486B3F7-B3D2-46E4-8024-05D53FB42B10
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Core.Server.Plugins.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Core.Server.Plugins
{
  [DefaultServiceImplementation(typeof (DefaultPipelineResourceAuthorizationProxyService))]
  public interface IPipelineResourceAuthorizationProxyService : IVssFrameworkService
  {
    Task AuthorizeResourceForAllPipelinesAsync(
      IVssRequestContext requestContext,
      Guid projectId,
      string resourceId,
      string resourceType);

    Task InheritAuthorizationPolicyFromEnvironmentAsync(
      IVssRequestContext requestContext,
      Guid projectId,
      int environmentId,
      string resourceId,
      string resourceType);

    Task DeletePipelinePermissionsForResource(
      IVssRequestContext requestContext,
      Guid projectId,
      string resourceId,
      string resourceType);
  }
}
