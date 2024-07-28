// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.VirtualTeamProjectCollectionPropertiesService
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Server.Core
{
  public sealed class VirtualTeamProjectCollectionPropertiesService : 
    TeamProjectCollectionPropertiesServiceBase
  {
    protected override IEnumerable<HostProperties> GetCollectionHostPropertiesCached(
      IVssRequestContext requestContext)
    {
      requestContext = this.RootContext(requestContext);
      requestContext.CheckProjectCollectionRequestContext();
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      return (IEnumerable<HostProperties>) new HostProperties[1]
      {
        vssRequestContext.GetService<ITeamFoundationHostManagementService>().QueryServiceHostPropertiesCached(vssRequestContext, requestContext.ServiceHost.InstanceId)
      };
    }

    protected override IEnumerable<TeamFoundationServiceHostProperties> GetCollectionHostProperties(
      IVssRequestContext requestContext,
      ServiceHostFilterFlags filterFlags)
    {
      requestContext = this.RootContext(requestContext);
      requestContext.CheckProjectCollectionRequestContext();
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      return (IEnumerable<TeamFoundationServiceHostProperties>) new TeamFoundationServiceHostProperties[1]
      {
        vssRequestContext.GetService<ITeamFoundationHostManagementService>().QueryServiceHostProperties(vssRequestContext, requestContext.ServiceHost.InstanceId, filterFlags)
      };
    }

    protected override bool TryCheckGetCollectionPropertiesPermission(
      IVssRequestContext requestContext)
    {
      requestContext = this.RootContext(requestContext);
      requestContext.CheckProjectCollectionRequestContext();
      IVssSecurityNamespace securityNamespace = requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, FrameworkSecurity.FrameworkNamespaceId);
      if (securityNamespace == null)
        return false;
      securityNamespace.CheckPermission(requestContext, FrameworkSecurity.FrameworkNamespaceToken, 1);
      return true;
    }

    private IVssRequestContext RootContext(IVssRequestContext requestContext) => !requestContext.IsSystemContext ? requestContext.RootContext : requestContext.RootContext.Elevate();
  }
}
