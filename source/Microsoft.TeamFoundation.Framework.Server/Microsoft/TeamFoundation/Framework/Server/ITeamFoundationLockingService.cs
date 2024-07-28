// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ITeamFoundationLockingService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [Obsolete("This service has been deprecated and will be removed from a future release.", false)]
  [DefaultServiceImplementation(typeof (TeamFoundationLockingService), typeof (VirtualLockingService))]
  public interface ITeamFoundationLockingService : IVssFrameworkService
  {
    TeamFoundationLock AcquireLock(
      IVssRequestContext requestContext,
      TeamFoundationLockInfo lockInfo);

    TeamFoundationLock AcquireLock(
      IVssRequestContext requestContext,
      TeamFoundationLockMode lockMode,
      string resource);

    TeamFoundationLock AcquireLock(
      IVssRequestContext requestContext,
      TeamFoundationLockMode lockMode,
      string resource,
      int lockTimeout);

    TeamFoundationLock AcquireLocks(
      IVssRequestContext requestContext,
      TeamFoundationLockMode lockMode,
      params string[] resources);

    TeamFoundationLock AcquireLocks(
      IVssRequestContext requestContext,
      TeamFoundationLockMode lockMode,
      int lockTimeout,
      params string[] resources);

    TeamFoundationLock AcquireLocks(
      IVssRequestContext requestContext,
      TeamFoundationLockMode lockMode,
      int lockTimeout,
      bool throwOnTimeout,
      params string[] resources);

    TeamFoundationLockMode QueryLockMode(IVssRequestContext requestContext, string resource);
  }
}
