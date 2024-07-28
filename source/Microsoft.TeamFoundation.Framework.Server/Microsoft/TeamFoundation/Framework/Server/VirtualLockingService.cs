// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.VirtualLockingService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [Obsolete("This service has been deprecated and will be removed from a future release.", false)]
  internal class VirtualLockingService : TeamFoundationLockingService
  {
    public override TeamFoundationLock AcquireLock(
      IVssRequestContext requestContext,
      TeamFoundationLockMode lockMode,
      string resource,
      int lockTimeout)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      return vssRequestContext.GetService<ITeamFoundationLockingService>().AcquireLock(vssRequestContext, lockMode, this.GenerateVirtualResourceString(requestContext, resource), lockTimeout);
    }

    public override TeamFoundationLock AcquireLocks(
      IVssRequestContext requestContext,
      TeamFoundationLockMode lockMode,
      int lockTimeout,
      bool throwOnTimeout,
      params string[] resources)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      return vssRequestContext.GetService<ITeamFoundationLockingService>().AcquireLocks(vssRequestContext, lockMode, lockTimeout, throwOnTimeout, ((IEnumerable<string>) resources).Select<string, string>((Func<string, string>) (r => this.GenerateVirtualResourceString(requestContext, r))).ToArray<string>());
    }

    public override TeamFoundationLockMode QueryLockMode(
      IVssRequestContext requestContext,
      string resource)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      return vssRequestContext.GetService<ITeamFoundationLockingService>().QueryLockMode(vssRequestContext, this.GenerateVirtualResourceString(requestContext, resource));
    }

    private string GenerateVirtualResourceString(IVssRequestContext requestContext, string resource) => string.Format("{0}:{1}", (object) resource, (object) requestContext.ServiceHost.InstanceId.ToString("N"));
  }
}
