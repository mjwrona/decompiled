// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.TeamFoundationLockingService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [Obsolete("This service has been deprecated and will be removed from a future release.", false)]
  internal class TeamFoundationLockingService : ITeamFoundationLockingService, IVssFrameworkService
  {
    private IVssServiceHost m_serviceHost;
    private static readonly string s_area = "Locking";
    private static readonly string s_layer = nameof (TeamFoundationLockingService);

    void IVssFrameworkService.ServiceStart(IVssRequestContext systemRequestContext) => this.m_serviceHost = systemRequestContext.ServiceHost;

    void IVssFrameworkService.ServiceEnd(IVssRequestContext systemRequestContext) => this.m_serviceHost = (IVssServiceHost) null;

    public TeamFoundationLock AcquireLock(
      IVssRequestContext requestContext,
      TeamFoundationLockInfo lockInfo)
    {
      return this.AcquireLock(requestContext, lockInfo.LockMode, lockInfo.LockName, lockInfo.LockTimeout);
    }

    public TeamFoundationLock AcquireLock(
      IVssRequestContext requestContext,
      TeamFoundationLockMode lockMode,
      string resource)
    {
      return this.AcquireLock(requestContext, lockMode, resource, -1);
    }

    public virtual TeamFoundationLock AcquireLock(
      IVssRequestContext requestContext,
      TeamFoundationLockMode lockMode,
      string resource,
      int lockTimeout)
    {
      bool flag = false;
      LockingComponent lockingComponent = this.GetAppropriateLockingComponent(requestContext);
      try
      {
        flag = lockingComponent.AcquireLock(resource, lockMode, lockTimeout);
        if (!flag)
          return (TeamFoundationLock) null;
        return new TeamFoundationLock(lockingComponent, lockMode, false, new string[1]
        {
          resource
        });
      }
      finally
      {
        if (!flag)
          lockingComponent.Dispose();
      }
    }

    private LockingComponent GetAppropriateLockingComponent(IVssRequestContext requestContext)
    {
      LockingComponent lockingComponent = (LockingComponent) null;
      foreach (LockingComponent disposableResource in requestContext.RequestContextInternal().GetDisposableResources<LockingComponent>())
      {
        if (VssStringComparer.DataSource.Equals(disposableResource.DataSource, requestContext.FrameworkConnectionInfo.DataSource) && VssStringComparer.DatabaseName.Equals(disposableResource.InitialCatalog, requestContext.FrameworkConnectionInfo.InitialCatalog))
        {
          lockingComponent = disposableResource;
          break;
        }
      }
      if (lockingComponent == null)
        lockingComponent = requestContext.CreateComponent<LockingComponent>();
      return lockingComponent;
    }

    public TeamFoundationLock AcquireLocks(
      IVssRequestContext requestContext,
      TeamFoundationLockMode lockMode,
      params string[] resources)
    {
      return this.AcquireLocks(requestContext, lockMode, -1, true, resources);
    }

    public TeamFoundationLock AcquireLocks(
      IVssRequestContext requestContext,
      TeamFoundationLockMode lockMode,
      int lockTimeout,
      params string[] resources)
    {
      return this.AcquireLocks(requestContext, lockMode, lockTimeout, true, resources);
    }

    public virtual TeamFoundationLock AcquireLocks(
      IVssRequestContext requestContext,
      TeamFoundationLockMode lockMode,
      int lockTimeout,
      bool throwOnTimeout,
      params string[] resources)
    {
      ArgumentUtility.CheckForNull<string[]>(resources, nameof (resources));
      if (resources.Length != 0)
      {
        bool flag = false;
        LockingComponent lockingComponent = this.GetAppropriateLockingComponent(requestContext);
        try
        {
          string timedoutLockName = (string) null;
          flag = lockingComponent.AcquireLocks(lockMode, lockTimeout, resources, out timedoutLockName);
          if (flag)
            return new TeamFoundationLock(lockingComponent, lockMode, true, resources);
          if (throwOnTimeout)
            throw new LockTimeoutException(timedoutLockName, lockMode, lockTimeout);
          return (TeamFoundationLock) null;
        }
        finally
        {
          if (!flag)
            lockingComponent.Dispose();
        }
      }
      else
      {
        TeamFoundationTracingService.TraceRaw(0, TraceLevel.Info, TeamFoundationLockingService.s_area, TeamFoundationLockingService.s_layer, "Zero locks requested. Skipping db call.");
        return new TeamFoundationLock((LockingComponent) null, lockMode, true, resources);
      }
    }

    public virtual TeamFoundationLockMode QueryLockMode(
      IVssRequestContext requestContext,
      string resource)
    {
      using (LockingComponent component = requestContext.CreateComponent<LockingComponent>())
        return component.QueryLockMode(resource);
    }

    private void ValidateRequestContext(IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      if (this.m_serviceHost.InstanceId != requestContext.ServiceHost.InstanceId)
        throw new InvalidRequestContextHostException(FrameworkResources.LockingServiceRequestContextHostMessage((object) this.m_serviceHost.InstanceId, (object) requestContext.ServiceHost.InstanceId));
    }
  }
}
