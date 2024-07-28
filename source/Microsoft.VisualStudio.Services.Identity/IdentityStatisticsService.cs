// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.IdentityStatisticsService
// Assembly: Microsoft.VisualStudio.Services.Identity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1372DA81-8681-4BFE-8E91-D1AB4333F834
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Identity.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Security;

namespace Microsoft.VisualStudio.Services.Identity
{
  internal class IdentityStatisticsService : IVssFrameworkService
  {
    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    internal int GetNumberOfUsers(IVssRequestContext requestContext)
    {
      using (IdentityManagementStatisticsComponent component = requestContext.To(TeamFoundationHostType.Deployment).CreateComponent<IdentityManagementStatisticsComponent>())
        return component.GetNumberOfUsers();
    }

    public long GetMaxSequenceId(IVssRequestContext requestContext)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      if (!vssRequestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(vssRequestContext, FrameworkSecurity.FrameworkNamespaceId).HasPermission(vssRequestContext, FrameworkSecurity.FrameworkNamespaceToken, 4, false))
        throw new AccessCheckException(FrameworkResources.InvalidAccessException());
      using (IdentityManagementStatisticsComponent component = requestContext.CreateComponent<IdentityManagementStatisticsComponent>())
        return component.GetMaxSequenceIdOfIdentity();
    }
  }
}
