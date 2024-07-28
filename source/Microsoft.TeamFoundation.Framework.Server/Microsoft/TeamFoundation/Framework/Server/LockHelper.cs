// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.LockHelper
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class LockHelper : IDisposable
  {
    private List<LockHeld> m_locksHeld;
    private long m_id;
    private const string s_Area = "HostManagement";
    private const string s_Layer = "LockHelper";

    public LockHelper()
    {
    }

    internal static LockHelper Lock(
      LockManager lockManager,
      long requestId,
      KeyValuePair<LockName<short, Guid, short>, bool>[] lockNames,
      LockManager.LockType lockType)
    {
      LockHelper lockHelper = (LockHelper) null;
      LockHelper.TryGetLock(lockManager, requestId, lockNames, lockType, TimeSpan.MaxValue, out lockHelper);
      return lockHelper;
    }

    internal static bool TryGetLock(
      LockManager lockManager,
      long requestId,
      KeyValuePair<LockName<short, Guid, short>, bool>[] lockNames,
      LockManager.LockType lockType,
      TimeSpan timeout,
      out LockHelper lockHelper)
    {
      lockHelper = (LockHelper) null;
      int millisecondsTimeout;
      if (timeout == TimeSpan.MaxValue)
      {
        millisecondsTimeout = -1;
      }
      else
      {
        ArgumentUtility.CheckForOutOfRange<TimeSpan>(timeout, nameof (timeout), TimeSpan.Zero, TimeSpan.FromDays(24.0));
        millisecondsTimeout = (int) timeout.TotalMilliseconds;
      }
      List<LockHeld> locksHeld = new List<LockHeld>(3);
      try
      {
        if (!LockHelper.TryGetLocksInternal(lockManager, requestId, lockNames, lockType, locksHeld, millisecondsTimeout))
          return false;
        lockHelper = new LockHelper(requestId, locksHeld);
        return true;
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(58658, "HostManagement", nameof (LockHelper), ex);
        LockHelper.ReleaseAllLocks(locksHeld);
        throw;
      }
    }

    private LockHelper(long requestId, List<LockHeld> locksHeld)
    {
      this.m_locksHeld = locksHeld;
      this.m_id = requestId;
    }

    public void Dispose()
    {
      try
      {
        if (this.m_id < 0L && !LockHelperContext.RequestIdIsSet && this.m_id != (long) -Environment.CurrentManagedThreadId)
          TeamFoundationTracingService.TraceRaw(58665, TraceLevel.Error, "HostManagement", nameof (LockHelper), "This object was created on Thread {0} but disposed on Thread {1}", (object) this.m_id, (object) Environment.CurrentManagedThreadId);
        LockHelper.ReleaseAllLocks(this.m_locksHeld);
        this.m_locksHeld = (List<LockHeld>) null;
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(58668, "HostManagement", nameof (LockHelper), ex);
        throw;
      }
    }

    private static void ReleaseAllLocks(List<LockHeld> locksHeld)
    {
      if (locksHeld == null)
        return;
      for (int index = locksHeld.Count - 1; index >= 0; --index)
        locksHeld[index]?.Dispose();
    }

    private static bool TryGetLocksInternal(
      LockManager lockManager,
      long requestId,
      KeyValuePair<LockName<short, Guid, short>, bool>[] lockNames,
      LockManager.LockType requestedLockType,
      List<LockHeld> locksHeld,
      int millisecondsTimeout)
    {
      bool locksInternal = false;
      try
      {
        for (short index = 0; (int) index < lockNames.Length; ++index)
        {
          if (lockNames[(int) index].Key != null)
          {
            LockManager.LockType lockType;
            switch (requestedLockType)
            {
              case LockManager.LockType.HostLockShared:
                lockType = LockManager.LockType.HostLockIntentShared;
                break;
              case LockManager.LockType.HostLockUpdate:
                lockType = !lockNames[(int) index].Value ? LockManager.LockType.HostLockIntentUpdate : LockManager.LockType.HostLockUpdate;
                break;
              case LockManager.LockType.HostLockExclusive:
                lockType = !lockNames[(int) index].Value ? LockManager.LockType.HostLockIntentExclusive : LockManager.LockType.HostLockExclusive;
                break;
              default:
                throw new ArgumentOutOfRangeException("lockType");
            }
            if (!LockHelper.TryGetLock(lockManager, requestId, (ILockName) lockNames[(int) index].Key, lockType, millisecondsTimeout, locksHeld))
            {
              locksInternal = false;
              return locksInternal;
            }
          }
        }
        locksInternal = true;
        return locksInternal;
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(58675, "HostManagement", nameof (LockHelper), ex);
        throw;
      }
      finally
      {
        if (!locksInternal)
          LockHelper.ReleaseAllLocks(locksHeld);
      }
    }

    internal static bool TryGetLock(
      LockManager lockManager,
      long requestId,
      ILockName lockName,
      LockManager.LockType lockType,
      int millisecondsTimeout,
      List<LockHeld> locksHeld)
    {
      LockHeld lockHeld = new LockHeld(lockManager, lockName, lockType, requestId);
      if (!lockManager.TryGetLock(lockName, lockType, requestId, millisecondsTimeout))
        return false;
      locksHeld.Add(lockHeld);
      return true;
    }

    internal long RequestId => this.m_id;
  }
}
