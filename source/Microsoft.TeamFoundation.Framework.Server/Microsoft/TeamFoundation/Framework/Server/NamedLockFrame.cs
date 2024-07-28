// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.NamedLockFrame
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Threading;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public struct NamedLockFrame : IDisposable
  {
    private LockManager lockManager;
    private LockManager.NamedLockObject lockObject;
    private LockManager.LockType lockType;
    private IList<LockManager.LockHeldEntry> locksHeld;
    private long requestId;
    private bool freeLocksHeldEntries;

    private NamedLockFrame(ILockName lockName)
    {
      this.lockManager = (LockManager) null;
      this.lockObject = new LockManager.NamedLockObject(lockName);
      this.lockType = (LockManager.LockType) 0;
      this.locksHeld = (IList<LockManager.LockHeldEntry>) null;
      this.requestId = 0L;
      this.freeLocksHeldEntries = false;
      Monitor.Enter((object) this.lockObject.LockName);
    }

    internal NamedLockFrame(
      LockManager lockManager,
      LockManager.NamedLockObject lockObject,
      LockManager.LockType lockType,
      IList<LockManager.LockHeldEntry> locksHeld,
      long requestId,
      bool freeLocksHeldEntries = true)
    {
      LockManager.RetailAssert(lockManager != null, "Invalid Lock Manager reference");
      lockManager.TryGetNamedLockImpl(lockObject, lockType, locksHeld, requestId, -1);
      this.lockObject = lockObject;
      this.lockType = lockType;
      this.lockManager = lockManager;
      this.locksHeld = locksHeld;
      this.requestId = requestId;
      this.freeLocksHeldEntries = freeLocksHeldEntries;
    }

    internal static NamedLockFrame CreateMockNamedLockFrame(ILockName lockName) => new NamedLockFrame(lockName);

    public void Dispose()
    {
      if (this.lockObject == null)
        return;
      if (this.lockManager != null)
      {
        if (!this.lockManager.ReleaseNamedLockImpl(this.lockObject, this.lockType, this.locksHeld, this.requestId) || !this.freeLocksHeldEntries)
          return;
        this.lockManager.FreeLocksHeldEntries(this.requestId);
      }
      else
        Monitor.Exit((object) this.lockObject.LockName);
    }
  }
}
