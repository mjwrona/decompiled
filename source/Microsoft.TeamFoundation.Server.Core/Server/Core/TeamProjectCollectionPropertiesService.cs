// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.TeamProjectCollectionPropertiesService
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Server.Core
{
  public sealed class TeamProjectCollectionPropertiesService : 
    TeamProjectCollectionPropertiesServiceBase
  {
    protected override IEnumerable<HostProperties> GetCollectionHostPropertiesCached(
      IVssRequestContext requestContext)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      return vssRequestContext.GetService<ITeamFoundationHostManagementService>().QueryChildrenServiceHostPropertiesCached(vssRequestContext, requestContext.ServiceHost.InstanceId);
    }

    protected override IEnumerable<TeamFoundationServiceHostProperties> GetCollectionHostProperties(
      IVssRequestContext requestContext,
      ServiceHostFilterFlags filterFlags)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      return (IEnumerable<TeamFoundationServiceHostProperties>) vssRequestContext.GetService<ITeamFoundationHostManagementService>().QueryServiceHostProperties(vssRequestContext, requestContext.ServiceHost.InstanceId, ServiceHostFilterFlags.IncludeChildren | filterFlags).Children;
    }

    protected override bool TryCheckGetCollectionPropertiesPermission(
      IVssRequestContext requestContext)
    {
      IVssSecurityNamespace securityNamespace = requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, FrameworkSecurity.FrameworkNamespaceId);
      if (securityNamespace == null)
        return false;
      int requestedPermissions = requestContext.ExecutionEnvironment.IsHostedDeployment ? 1 : 2;
      securityNamespace.CheckPermission(requestContext, FrameworkSecurity.FrameworkNamespaceToken, requestedPermissions);
      return true;
    }
  }
}
