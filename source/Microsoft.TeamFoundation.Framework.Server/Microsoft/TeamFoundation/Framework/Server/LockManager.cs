// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.LockManager
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal abstract class LockManager
  {
    private IDictionary<long, IList<LockManager.LockHeldEntry>>[] m_locksHeld;
    private const int c_estNumberOfObjects = 400;
    private const int c_numberOfObjectHashes = 32;
    private const int c_estNumberOfLocksPerThread = 6;
    private bool m_checkForLockViolations = true;
    protected bool m_isProduction = true;
    private const int c_staleLockCleanupInitialListSize = 10;
    private static TimeSpan s_staleLockCleanupInterval = TimeSpan.FromMinutes(10.0);
    private static int s_staleLockCleanupSkipCount = 1000;
    private static readonly string s_area = "Common";
    private static readonly string s_layer = nameof (LockManager);
    private LockManager.NamedLockObjectsPartition[] m_namedLockObjects;
    private const int LockLevelBitCount = 4;
    private const int LockKindBitCount = 16;

    static LockManager()
    {
      LockManager.RetailAssert(true, "You need to update LockManager.LockLevelBitCount");
      LockManager.RetailAssert(true, "You need to update LockManager.LockKindBitCount");
    }

    internal LockManager()
    {
      this.InitializeLocksHeldArray();
      this.InitializeNamedLockObjectsPartitionArray();
    }

    private void InitializeLocksHeldArray()
    {
      IDictionary<long, IList<LockManager.LockHeldEntry>>[] dictionaryArray = (IDictionary<long, IList<LockManager.LockHeldEntry>>[]) new Dictionary<long, IList<LockManager.LockHeldEntry>>[32];
      for (int index = 0; index < 32; ++index)
        dictionaryArray[index] = (IDictionary<long, IList<LockManager.LockHeldEntry>>) new Dictionary<long, IList<LockManager.LockHeldEntry>>(6);
      this.m_locksHeld = dictionaryArray;
    }

    [Conditional("DEBUG")]
    internal void AssertNoLocksHeld(LockManager.LockType lockType, long requestId)
    {
      IList<LockManager.LockHeldEntry> locksHeld = this.GetLocksHeld(requestId, false);
      if (locksHeld == null)
        return;
      lock (locksHeld)
      {
        for (int index = 0; index < locksHeld.Count; ++index)
        {
          if (locksHeld[index].LockType == lockType)
          {
            LockManager.RetailAssert(false, "lock assertion failed - lock {0} is already held", (object) lockType);
            break;
          }
        }
      }
    }

    internal void AssertNoLocksHeld(long requestId) => this.EnsureNoLocksHeld(requestId, string.Empty, true);

    internal void AssertNoLocksHeld(long requestId, string message) => this.EnsureNoLocksHeld(requestId, message, true);

    internal void EnsureNoLocksHeld(long requestId, string message, bool assert)
    {
      try
      {
        int num = 0;
        IList<LockManager.LockHeldEntry> locksHeld = this.GetLocksHeld(requestId, false);
        if (locksHeld != null)
        {
          lock (locksHeld)
          {
            num = locksHeld.Count;
            for (int index = 0; index < num; ++index)
            {
              LockManager.LockHeldEntry lockHeldEntry = locksHeld[index];
              TeamFoundationTracingService.TraceRaw(39815, TraceLevel.Error, this.Area, this.Layer, "Thread {0} with Request ID {1} has lock {2} in mode {3} [debug info {4}]", (object) Environment.CurrentManagedThreadId, (object) requestId, lockHeldEntry.LockObject, (object) lockHeldEntry.LockType, (object) message);
            }
          }
        }
        if (!assert)
          return;
        LockManager.DebugAssert((num == 0 ? 1 : 0) != 0, "Thread still holds some locks. {0}", (object) message);
        LockManager.DebugAssert((locksHeld == null ? 1 : 0) != 0, "LocksHeld structure should be cleaned up. {0}", (object) message);
      }
      finally
      {
        this.FreeAllLocks(requestId);
      }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void Assert(bool condition, bool isRetailAssert, string msg)
    {
      if (condition)
        return;
      TeamFoundationTracingService.TraceRaw(39805, TraceLevel.Error, LockManager.s_area, LockManager.s_layer, msg);
      if (isRetailAssert)
        throw new InvalidOperationException(msg);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void Assert(
      bool condition,
      bool isRetailAssert,
      string msg,
      params object[] args)
    {
      if (condition)
        return;
      string message = string.Format(msg, args);
      TeamFoundationTracingService.TraceRaw(39805, TraceLevel.Error, LockManager.s_area, LockManager.s_layer, message);
      if (isRetailAssert)
        throw new InvalidOperationException(message);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void DebugAssert(bool condition, string message) => LockManager.Assert(condition, false, message);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void DebugAssert(bool condition, string message, params object[] args) => LockManager.Assert(condition, false, message, args);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void RetailAssert(bool condition, string message) => LockManager.Assert(condition, true, message);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void RetailAssert(bool condition, string message, params object[] args) => LockManager.Assert(condition, true, message, args);

    protected static int GetPartitionNumber(long requestId) => Math.Abs(requestId.GetHashCode() % 32);

    internal object CreateLocksHeldObject() => (object) new List<LockManager.LockHeldEntry>(6);

    private IList<LockManager.LockHeldEntry> CheckLocksHeldObject(object locksHeldObject)
    {
      IList<LockManager.LockHeldEntry> lockHeldEntryList = locksHeldObject as IList<LockManager.LockHeldEntry>;
      LockManager.RetailAssert(lockHeldEntryList != null, "locksHeld object was invalid");
      return lockHeldEntryList;
    }

    protected IList<LockManager.LockHeldEntry> GetLocksHeld(
      long requestId,
      bool allocateIfNecessary)
    {
      int partitionNumber = LockManager.GetPartitionNumber(requestId);
      IList<LockManager.LockHeldEntry> locksHeld;
      lock (this.m_locksHeld[partitionNumber])
      {
        if (!this.m_locksHeld[partitionNumber].TryGetValue(requestId, out locksHeld) & allocateIfNecessary)
        {
          locksHeld = (IList<LockManager.LockHeldEntry>) new List<LockManager.LockHeldEntry>(6);
          this.m_locksHeld[partitionNumber].Add(requestId, locksHeld);
        }
      }
      return locksHeld;
    }

    protected internal void FreeLocksHeldEntries(long requestId)
    {
      int partitionNumber = LockManager.GetPartitionNumber(requestId);
      lock (this.m_locksHeld[partitionNumber])
      {
        IList<LockManager.LockHeldEntry> lockHeldEntryList;
        if (!this.m_locksHeld[partitionNumber].TryGetValue(requestId, out lockHeldEntryList))
          return;
        bool lockTaken = false;
        try
        {
          Monitor.TryEnter((object) lockHeldEntryList, 0, ref lockTaken);
          if (lockTaken)
          {
            if (lockHeldEntryList.Count != 0)
              return;
            this.m_locksHeld[partitionNumber].Remove(requestId);
          }
          else
            TeamFoundationTracingService.TraceRaw(39205, TraceLevel.Error, this.Area, this.Layer, "Potentially leaking locksHeld list for requestId {0}", (object) requestId);
        }
        finally
        {
          if (lockTaken)
            Monitor.Exit((object) lockHeldEntryList);
        }
      }
    }

    internal int GetNumberOfLocksHeld(long requestId)
    {
      IList<LockManager.LockHeldEntry> locksHeld = this.GetLocksHeld(requestId, false);
      if (locksHeld == null)
        return 0;
      lock (locksHeld)
        return locksHeld.Count;
    }

    private void FreeAllLocks(long requestId)
    {
      IList<LockManager.LockHeldEntry> locksHeld = this.GetLocksHeld(requestId, false);
      if (locksHeld == null)
        return;
      bool flag = true;
      lock (locksHeld)
      {
        for (int index1 = locksHeld.Count - 1; index1 >= 0; --index1)
        {
          LockManager.LockHeldEntry lockHeldEntry = locksHeld[index1];
          int refCount = lockHeldEntry.RefCount;
          for (int index2 = 0; index2 < refCount; ++index2)
          {
            TeamFoundationTracingService.TraceRaw(39150, TraceLevel.Error, this.Area, this.Layer, "Lock Manager is releasing locks {0} {1}", lockHeldEntry.LockObject, (object) lockHeldEntry.LockType);
            object lockObject1 = lockHeldEntry.LockObject;
            flag = !(lockObject1 is LockManager.NamedLockObject lockObject2) ? this.ReleaseObjectLockImpl(lockObject1, lockHeldEntry.LockType, locksHeld, requestId) : this.ReleaseNamedLockImpl(lockObject2, lockHeldEntry.LockType, locksHeld, requestId);
          }
        }
      }
      if (!flag)
        return;
      this.FreeLocksHeldEntries(requestId);
    }

    internal bool CheckForLockViolations
    {
      set => this.m_checkForLockViolations = value;
    }

    internal bool IsProduction
    {
      set => this.m_isProduction = value;
    }

    internal static TimeSpan StaleLockCleanupInterval
    {
      get => LockManager.s_staleLockCleanupInterval;
      set => LockManager.s_staleLockCleanupInterval = value;
    }

    internal static int StaleLockCleanupSkipCount
    {
      get => LockManager.s_staleLockCleanupSkipCount;
      set
      {
        LockManager.RetailAssert(value > 0, "StaleLockCleanupSkipCount must be positive, 1 means call cleanup on every named lock release");
        LockManager.s_staleLockCleanupSkipCount = value;
      }
    }

    protected virtual string Area => LockManager.s_area;

    protected virtual string Layer => LockManager.s_layer;

    internal static ILockName GetConnectionLockName(ConnectionLockNameType type) => LockManager.CachedLockName.GetConnectionLockName(type);

    internal static ILockName GetExemptionLockName() => LockManager.CachedLockName.GetExemptionLockName();

    private void InitializeNamedLockObjectsPartitionArray()
    {
      LockManager.NamedLockObjectsPartition[] objectsPartitionArray = new LockManager.NamedLockObjectsPartition[32];
      for (int index = 0; index < 32; ++index)
        objectsPartitionArray[index] = new LockManager.NamedLockObjectsPartition();
      this.m_namedLockObjects = objectsPartitionArray;
    }

    internal NamedLockFrame Lock(ILockName lockName, LockManager.LockType lockType, long requestId)
    {
      IList<LockManager.LockHeldEntry> locksHeld = this.GetLocksHeld(requestId, true);
      return this.Lock(lockName, lockType, locksHeld, requestId, true);
    }

    internal NamedLockFrame Lock(
      ILockName lockName,
      LockManager.LockType lockType,
      object locksHeld,
      long requestId)
    {
      return this.Lock(lockName, lockType, this.CheckLocksHeldObject(locksHeld), requestId, false);
    }

    private NamedLockFrame Lock(
      ILockName lockName,
      LockManager.LockType lockType,
      IList<LockManager.LockHeldEntry> locksHeld,
      long requestId,
      bool freeLocksHeldEntries)
    {
      return new NamedLockFrame(this, this.GetCachedLockObject(lockName, true), lockType, locksHeld, requestId, freeLocksHeldEntries);
    }

    internal bool TryGetLock(
      ILockName lockName,
      LockManager.LockType lockType,
      long requestId,
      int millisecondsTimeout)
    {
      IList<LockManager.LockHeldEntry> locksHeld = this.GetLocksHeld(requestId, true);
      return this.TryGetLock(lockName, lockType, locksHeld, requestId, millisecondsTimeout);
    }

    internal bool TryGetLock(
      ILockName lockName,
      LockManager.LockType lockType,
      object locksHeld,
      long requestId,
      int millisecondsTimeout)
    {
      return this.TryGetLock(lockName, lockType, this.CheckLocksHeldObject(locksHeld), requestId, millisecondsTimeout);
    }

    private bool TryGetLock(
      ILockName lockName,
      LockManager.LockType lockType,
      IList<LockManager.LockHeldEntry> locksHeld,
      long requestId,
      int millisecondsTimeout)
    {
      return this.TryGetNamedLockImpl(this.GetCachedLockObject(lockName, true), lockType, locksHeld, requestId, millisecondsTimeout);
    }

    internal bool TryGetNamedLockImpl(
      LockManager.NamedLockObject lockObject,
      LockManager.LockType lockType,
      IList<LockManager.LockHeldEntry> locksHeld,
      long requestId,
      int millisecondsTimeout)
    {
      LockManager.RetailAssert(lockObject.RefCount > 0, "Lock object must be addref'ed for a lock.");
      LockLevel lockLevel = LockManager.LockLevelFromLockType(lockType, true);
      LockManager.RetailAssert(lockObject.LockName.LockLevel == lockLevel, "Lock object is not appropriate for this lock level!");
      lock (locksHeld)
      {
        int count = locksHeld.Count;
        bool flag = lockType == LockManager.LockType.ResourceExemption || !this.m_checkForLockViolations;
        if (!flag)
        {
          for (int index = 0; index < count; ++index)
          {
            if (locksHeld[index].LockType == LockManager.LockType.ResourceExemption)
            {
              flag = true;
              break;
            }
          }
        }
        for (int index = 0; index < count; ++index)
        {
          LockManager.LockHeldEntry lockHeld = locksHeld[index];
          if (lockHeld.LockObject == lockObject)
          {
            if (lockHeld.LockType != lockType && LockManager.Max(lockHeld.EffectiveLockType, lockType) != lockHeld.EffectiveLockType)
            {
              TeamFoundationTracingService.TraceRaw(39000, TraceLevel.Info, this.Area, this.Layer, "Escalating lock from {0} to {1}", (object) lockHeld.LockType, (object) lockType);
              if (!lockObject.TryGetLock(lockObject.LockName, LockManager.LockKindFromLockType(lockType), requestId, millisecondsTimeout))
              {
                TeamFoundationTracingService.TraceRaw(39005, TraceLevel.Warning, this.Area, this.Layer, "Time out while Escalating lock from {0} to {1}", (object) lockHeld.LockType, (object) lockType);
                lockObject.ReleaseRef();
                return false;
              }
            }
            if (!flag)
              this.CheckForNamedLockReentryViolation(lockObject, lockType, lockHeld, requestId);
            lockHeld.ReLock(lockType);
            return true;
          }
        }
        if (count != 0)
        {
          LockManager.LockHeldEntry lockHeldEntry = locksHeld[count - 1];
          LockLevel lastLockLevel = LockManager.LockLevelFromLockType(lockHeldEntry.LockType);
          LockManager.NamedLockObject lockObject1 = lockHeldEntry.LockObject is LockManager.NamedLockObject ? (LockManager.NamedLockObject) lockHeldEntry.LockObject : (LockManager.NamedLockObject) null;
          if (this.IsNamedLockHierarchyViolation(lockObject.LockName.LockLevel, lastLockLevel, lockObject, lockObject1))
          {
            string str = lockObject1 != null ? lockObject1.LockName.ToString() : "unknown";
            LockManager.RetailAssert(false, "Lock Hierarchy violation: You are trying to get a {0} lock on {1} but you already have a {2} lock on {3}", (object) lockType, (object) lockObject.LockName, (object) lockHeldEntry.LockType, (object) str);
          }
        }
        if (!flag)
          this.CheckForNamedLockUsageViolation(lockObject, lockType, locksHeld, requestId);
        bool namedLockImpl = false;
        try
        {
          LockManager.LockHeldEntry lockHeldEntry = new LockManager.LockHeldEntry(lockType, (object) lockObject);
          if (millisecondsTimeout > -1)
          {
            namedLockImpl = lockObject.TryGetLock(lockObject.LockName, LockManager.LockKindFromLockType(lockType), requestId, millisecondsTimeout);
          }
          else
          {
            lockObject.GetLock(lockObject.LockName, LockManager.LockKindFromLockType(lockType), requestId);
            namedLockImpl = true;
          }
          if (namedLockImpl)
            locksHeld.Add(lockHeldEntry);
        }
        catch (Exception ex)
        {
          TeamFoundationTracingService.TraceRaw(39006, TraceLevel.Error, this.Area, this.Layer, "Failed to get the lock on requestId {0}, releasing ref for {1}", (object) requestId, (object) lockObject.LockName);
          throw;
        }
        finally
        {
          if (!namedLockImpl)
            lockObject.ReleaseRef();
        }
        return namedLockImpl;
      }
    }

    internal bool TestLock(ILockName lockName, LockManager.LockType lockType, long requestId)
    {
      IList<LockManager.LockHeldEntry> locksHeld = this.GetLocksHeld(requestId, false);
      return this.TestLock(lockName, lockType, locksHeld, requestId);
    }

    internal bool TestLock(
      ILockName lockName,
      LockManager.LockType lockType,
      object locksHeld,
      long requestId)
    {
      return this.TestLock(lockName, lockType, this.CheckLocksHeldObject(locksHeld), requestId);
    }

    private bool TestLock(
      ILockName lockName,
      LockManager.LockType lockType,
      IList<LockManager.LockHeldEntry> locksHeld,
      long requestId)
    {
      LockManager.NamedLockObject lockObject = (LockManager.NamedLockObject) lockName.CachedLockObject;
      if (lockObject == null)
        lockName.CachedLockObject = (INamedLockObject) (lockObject = this.GetLockObject(lockName, false));
      return this.TestLockImpl((object) lockObject, lockType, locksHeld, requestId);
    }

    private bool TestLockImpl(
      ILockName lockName,
      LockManager.LockType lockType,
      IList<LockManager.LockHeldEntry> locksHeld,
      long requestId)
    {
      LockLevel lockLevel = LockManager.LockLevelFromLockType(lockType, true);
      LockManager.RetailAssert(lockName.LockLevel == lockLevel, "Lock name is not appropriate for this lock level!");
      if (locksHeld != null)
      {
        lock (locksHeld)
        {
          for (int index = 0; index < locksHeld.Count; ++index)
          {
            LockManager.LockHeldEntry lockHeldEntry = locksHeld[index];
            if (lockName.Equals(((LockManager.NamedLockObject) lockHeldEntry.LockObject).LockName) && LockManager.Max(lockHeldEntry.EffectiveLockType, lockType) == lockHeldEntry.EffectiveLockType)
              return true;
          }
        }
      }
      return false;
    }

    internal void ReleaseLock(ILockName lockName, LockManager.LockType lockType, long requestId)
    {
      IList<LockManager.LockHeldEntry> locksHeld = this.GetLocksHeld(requestId, false);
      this.ReleaseLock(lockName, lockType, locksHeld, requestId, true);
    }

    internal void ReleaseLock(
      ILockName lockName,
      LockManager.LockType lockType,
      object locksHeld,
      long requestId)
    {
      this.ReleaseLock(lockName, lockType, this.CheckLocksHeldObject(locksHeld), requestId, false);
    }

    private void ReleaseLock(
      ILockName lockName,
      LockManager.LockType lockType,
      IList<LockManager.LockHeldEntry> locksHeld,
      long requestId,
      bool freeLocksHeldEntries)
    {
      try
      {
        if (!(this.ReleaseNamedLockImpl(this.GetCachedLockObject(lockName, false), lockType, locksHeld, requestId) & freeLocksHeldEntries))
          return;
        this.FreeLocksHeldEntries(requestId);
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(39105, this.Area, this.Layer, ex);
        LockManager.RetailAssert(false, "LockManager.ReleaseLock: Exception releasing lock: {0}", (object) ex);
        throw;
      }
    }

    internal bool ReleaseNamedLockImpl(
      LockManager.NamedLockObject lockObject,
      LockManager.LockType lockType,
      IList<LockManager.LockHeldEntry> locksHeld,
      long requestId)
    {
      LockManager.RetailAssert(lockObject.RefCount > 0, "Lock object must not be stale and must be addrefed by get lock.");
      LockLevel lockLevel = LockManager.LockLevelFromLockType(lockType, true);
      LockManager.RetailAssert(lockObject.LockName.LockLevel == lockLevel, "Lock object is not appropriate for this lock level!");
      if (locksHeld == null)
        LockManager.RetailAssert(false, "Releasing a lock {0} the user does not hold", (object) lockType);
      lock (locksHeld)
      {
        if (locksHeld.Count == 0)
          LockManager.RetailAssert(false, "Releasing a lock {0} the user does not hold", (object) lockType);
        for (int index = 0; index < locksHeld.Count; ++index)
        {
          LockManager.LockHeldEntry lockHeldEntry = locksHeld[index];
          if (lockHeldEntry.LockObject == lockObject && lockHeldEntry.LockType == lockType && lockHeldEntry.RefCount > 1)
          {
            LockManager.LockType effectiveLockType1 = lockHeldEntry.EffectiveLockType;
            if (lockHeldEntry.UnLock(lockType))
            {
              LockManager.LockType effectiveLockType2 = lockHeldEntry.EffectiveLockType;
              TeamFoundationTracingService.TraceRaw(39100, TraceLevel.Info, this.Area, this.Layer, "Downgrading lock {0} from {1} to {2}", (object) lockObject.LockName, (object) effectiveLockType1, (object) effectiveLockType2);
              lockObject.GetLock(lockObject.LockName, LockManager.LockKindFromLockType(effectiveLockType2), requestId);
            }
            lockObject.ReleaseRef();
            return false;
          }
        }
        LockManager.LockHeldEntry lockHeldEntry1 = locksHeld[locksHeld.Count - 1];
        if (lockObject != lockHeldEntry1.LockObject)
        {
          bool flag = false;
          int num = 1;
          for (int index = locksHeld.Count - 2; index >= 0; --index)
          {
            if (lockObject == locksHeld[index].LockObject)
            {
              flag = true;
              break;
            }
            ++num;
          }
          if (flag)
            LockManager.RetailAssert(false, "Lock Hierarchy violation: Attempting to release lock on {0} ahead of {1} other locked objects.", (object) lockObject, (object) num);
          else
            LockManager.RetailAssert(false, "Lock Hierarchy violation: Attempting to release lock on {0}, which we have no locks for.", (object) lockObject);
        }
        if (lockType != lockHeldEntry1.LockType)
        {
          bool flag = false;
          int num = 1;
          for (int index = lockHeldEntry1.PreviousLockTypes.Count - 1; index >= 0; --index)
          {
            if (lockHeldEntry1.PreviousLockTypes[index] == lockType)
            {
              flag = true;
              break;
            }
            ++num;
          }
          if (flag)
            LockManager.RetailAssert(false, "Lock Hierarchy violation: Attempting to release {0} on {1} ahead of {2} other locktypes.", (object) lockType, (object) lockObject, (object) num);
          else
            LockManager.RetailAssert(false, "Lock Hierarchy violation: Attempting to release {0} on {1}, when we don't have that lockType on object.", (object) lockType, (object) lockObject);
        }
        lockObject.ReleaseLock(lockObject.LockName, LockManager.LockKindFromLockType(lockType), requestId);
        try
        {
          lockObject.ReleaseRef();
        }
        finally
        {
          locksHeld.RemoveAt(locksHeld.Count - 1);
        }
        LockManager.NamedLockObjectsPartition objectsPartition = this.GetNamedLockObjectsPartition(lockObject.LockName);
        if (!objectsPartition.ShouldSkipCleanup())
          this.CleanupUnusedNamedLocks(objectsPartition);
        return locksHeld.Count == 0;
      }
    }

    [Conditional("DEBUG")]
    internal void AssertLockHeld(ILockName lockName, LockManager.LockType lockType, long requestId) => LockManager.RetailAssert((this.TestLock(lockName, lockType, requestId) ? 1 : 0) != 0, "lock assertion failed - lock {0} is not held", (object) lockType);

    [Conditional("DEBUG")]
    internal void AssertLockHeld(
      ILockName lockName,
      LockManager.LockType lockType,
      object locksHeld,
      long requestId)
    {
      LockManager.RetailAssert((this.TestLock(lockName, lockType, this.CheckLocksHeldObject(locksHeld), requestId) ? 1 : 0) != 0, "lock assertion failed - lock {0} is not held", (object) lockType);
    }

    [Conditional("DEBUG")]
    internal void AssertLockNotHeld(
      ILockName lockName,
      LockManager.LockType lockType,
      long requestId)
    {
      LockManager.RetailAssert((!this.TestLock(lockName, lockType, requestId) ? 1 : 0) != 0, "lock assertion failed - lock {0} is already held", (object) lockType);
    }

    [Conditional("DEBUG")]
    internal void AssertLockNotHeld(
      ILockName lockName,
      LockManager.LockType lockType,
      object locksHeld,
      long requestId)
    {
      LockManager.RetailAssert((!this.TestLock(lockName, lockType, this.CheckLocksHeldObject(locksHeld), requestId) ? 1 : 0) != 0, "lock assertion failed - lock {0} is already held", (object) lockType);
    }

    internal void AssertZeroActiveLockObjects()
    {
      for (int index = 0; index < 32; ++index)
      {
        LockManager.NamedLockObjectsPartition namedLockObject1 = this.m_namedLockObjects[index];
        if (namedLockObject1 != null)
        {
          lock (namedLockObject1)
          {
            foreach (LockManager.NamedLockObject namedLockObject2 in (IEnumerable<LockManager.NamedLockObject>) namedLockObject1.LockObjectsDictionary.Values)
              LockManager.Assert((namedLockObject2.RefCount <= 0 ? 1 : 0) != 0, false, "lockObject {0} has {1} refcount", (object) namedLockObject2, (object) namedLockObject2.RefCount);
          }
        }
      }
    }

    private LockManager.NamedLockObjectsPartition GetNamedLockObjectsPartition(ILockName lockName) => this.m_namedLockObjects[Math.Abs(lockName.GetHashCode() % 32)];

    private LockManager.NamedLockObject GetCachedLockObject(ILockName lockName, bool addRef)
    {
      LockManager.RetailAssert(lockName != null, "lock name must not be null");
      LockManager.NamedLockObject cachedLockObject = (LockManager.NamedLockObject) lockName.CachedLockObject;
      LockManager.RetailAssert(cachedLockObject == null || lockName.Equals(cachedLockObject.LockName), "cachedLockObject should have the same name!");
      if (cachedLockObject == null || addRef && !cachedLockObject.TryAddRef())
        lockName.CachedLockObject = (INamedLockObject) (cachedLockObject = this.GetLockObject(lockName, addRef));
      return cachedLockObject;
    }

    private LockManager.NamedLockObject GetLockObject(ILockName lockName, bool addref)
    {
      LockManager.NamedLockObjectsPartition objectsPartition = this.GetNamedLockObjectsPartition(lockName);
      LockManager.NamedLockObject lockObject;
      lock (objectsPartition)
      {
        if (!objectsPartition.LockObjectsDictionary.TryGetValue(lockName, out lockObject))
        {
          if (!addref)
            return (LockManager.NamedLockObject) null;
          lockObject = new LockManager.NamedLockObject(lockName);
          objectsPartition.LockObjectsDictionary.Add(lockName, lockObject);
        }
        if (addref)
        {
          LockManager.RetailAssert(lockObject.TryAddRef(), "TryAddRef must never fail here under partition lock");
          LockManager.RetailAssert(lockObject.RefCount > 0, "lock object must be referenced here!");
        }
      }
      return lockObject;
    }

    protected virtual void CheckForNamedLockReentryViolation(
      LockManager.NamedLockObject lockObject,
      LockManager.LockType lockType,
      LockManager.LockHeldEntry lockHeld,
      long requestId)
    {
    }

    protected virtual void CheckForNamedLockUsageViolation(
      LockManager.NamedLockObject lockObject,
      LockManager.LockType lockType,
      IList<LockManager.LockHeldEntry> locksHeld,
      long requestId)
    {
    }

    protected virtual bool IsNamedLockHierarchyViolation(
      LockLevel lockLevel,
      LockLevel lastLockLevel,
      LockManager.NamedLockObject lockObject,
      LockManager.NamedLockObject lastLockObject)
    {
      return lockLevel <= lastLockLevel;
    }

    private void CleanupUnusedNamedLocks(LockManager.NamedLockObjectsPartition partition)
    {
      if (!Monitor.TryEnter((object) partition, 0))
        return;
      try
      {
        DateTime freeLocksUsedBeforeTime;
        if (!partition.TimeToRunCleanup(out freeLocksUsedBeforeTime))
          return;
        IList<LockManager.NamedLockObject> namedLockObjectList = (IList<LockManager.NamedLockObject>) null;
        foreach (LockManager.NamedLockObject namedLockObject in (IEnumerable<LockManager.NamedLockObject>) partition.LockObjectsDictionary.Values)
        {
          if (namedLockObject.CanDispose(freeLocksUsedBeforeTime))
          {
            if (namedLockObjectList == null)
              namedLockObjectList = (IList<LockManager.NamedLockObject>) new List<LockManager.NamedLockObject>(10);
            namedLockObjectList.Add(namedLockObject);
          }
        }
        if (namedLockObjectList == null)
          return;
        foreach (LockManager.NamedLockObject namedLockObject in (IEnumerable<LockManager.NamedLockObject>) namedLockObjectList)
        {
          if (namedLockObject.TryDispose())
            partition.LockObjectsDictionary.Remove(namedLockObject.LockName);
        }
      }
      finally
      {
        Monitor.Exit((object) partition);
      }
    }

    internal LockManager.ObjectLockFrame Lock(
      object lockObject,
      LockManager.LockType lockType,
      long requestId)
    {
      IList<LockManager.LockHeldEntry> locksHeld = this.GetLocksHeld(requestId, true);
      return new LockManager.ObjectLockFrame(this, lockObject, lockType, locksHeld, requestId);
    }

    internal LockManager.ObjectLockFrame Lock(
      object lockObject,
      LockManager.LockType lockType,
      object locksHeld,
      long requestId)
    {
      return new LockManager.ObjectLockFrame(this, lockObject, lockType, this.CheckLocksHeldObject(locksHeld), requestId, false);
    }

    internal bool TryGetLock(object lockObject, LockManager.LockType lockType, long requestId)
    {
      IList<LockManager.LockHeldEntry> locksHeld = this.GetLocksHeld(requestId, true);
      return this.GetObjectLockImpl(lockObject, lockType, locksHeld, requestId, true);
    }

    internal bool TryGetLock(
      object lockObject,
      LockManager.LockType lockType,
      object locksHeld,
      long requestId)
    {
      return this.GetObjectLockImpl(lockObject, lockType, this.CheckLocksHeldObject(locksHeld), requestId, true);
    }

    private bool GetObjectLockImpl(
      object lockObject,
      LockManager.LockType lockType,
      IList<LockManager.LockHeldEntry> locksHeld,
      long requestId,
      bool tryGetLock)
    {
      LockManager.RetailAssert(!(lockObject is ILockName), "this method must not be called for named locks!");
      LockManager.RetailAssert(!(lockObject is string), "this method must not be called for strings!");
      LockManager.RetailAssert(LockManager.LockKindFromLockType(lockType) == LockManager.LockKind.Monitor, "Invalid lock kind for object lock");
      LockLevel lockLevel = LockManager.LockLevelFromLockType(lockType, true);
      lock (locksHeld)
      {
        LockManager.LockHeldEntry lockHeldEntry = locksHeld[locksHeld.Count - 1];
        if (locksHeld.Count != 0 && this.IsObjectLockHierarchyViolation(lockLevel, LockManager.LockLevelFromLockType(lockHeldEntry.LockType)))
          LockManager.RetailAssert(false, "Lock Hierarchy violation: Taking {0} violates {1}", (object) lockType, (object) lockHeldEntry.LockType);
        bool objectLockImpl;
        try
        {
          if (tryGetLock)
          {
            objectLockImpl = Monitor.TryEnter(lockObject, 0);
          }
          else
          {
            Monitor.Enter(lockObject);
            objectLockImpl = true;
          }
          if (objectLockImpl)
            locksHeld.Add(new LockManager.LockHeldEntry(lockType, lockObject));
        }
        catch (Exception ex)
        {
          TeamFoundationTracingService.TraceRaw(39006, TraceLevel.Error, this.Area, this.Layer, "Failed to get the lock on requestId {0}", (object) requestId);
          throw;
        }
        return objectLockImpl;
      }
    }

    internal bool TestLock(object lockObject, LockManager.LockType lockType, long requestId)
    {
      if (lockObject == null)
        return false;
      IList<LockManager.LockHeldEntry> locksHeld = this.GetLocksHeld(requestId, false);
      return this.TestLockImpl(lockObject, lockType, locksHeld, requestId);
    }

    internal bool TestLock(
      object lockObject,
      LockManager.LockType lockType,
      object locksHeld,
      long requestId)
    {
      return this.TestLockImpl(lockObject, lockType, this.CheckLocksHeldObject(locksHeld), requestId);
    }

    private bool TestLockImpl(
      object lockObject,
      LockManager.LockType lockType,
      IList<LockManager.LockHeldEntry> locksHeld,
      long requestId)
    {
      if (lockObject == null)
        return false;
      LockManager.RetailAssert(!(lockObject is ILockName), "this method must not be called for named locks!");
      LockManager.RetailAssert(!(lockObject is string), "this method must not be called for strings!");
      int num = (int) LockManager.LockLevelFromLockType(lockType, true);
      if (locksHeld != null)
      {
        lock (locksHeld)
        {
          for (int index = 0; index < locksHeld.Count; ++index)
          {
            LockManager.LockHeldEntry lockHeldEntry = locksHeld[index];
            if (lockHeldEntry.LockObject == lockObject && LockManager.Max(lockHeldEntry.LockType, lockType) == lockHeldEntry.LockType)
              return true;
          }
        }
      }
      return false;
    }

    internal void ReleaseLock(object lockObject, LockManager.LockType lockType, long requestId)
    {
      IList<LockManager.LockHeldEntry> locksHeld = this.GetLocksHeld(requestId, false);
      if (!this.ReleaseObjectLockImpl(lockObject, lockType, locksHeld, requestId))
        return;
      this.FreeLocksHeldEntries(requestId);
    }

    internal void ReleaseLock(
      object lockObject,
      LockManager.LockType lockType,
      object locksHeld,
      long requestId)
    {
      this.ReleaseObjectLockImpl(lockObject, lockType, this.CheckLocksHeldObject(locksHeld), requestId);
    }

    private bool ReleaseObjectLockImpl(
      object lockObject,
      LockManager.LockType lockType,
      IList<LockManager.LockHeldEntry> locksHeld,
      long requestId)
    {
      LockManager.RetailAssert(!(lockObject is ILockName), "this method must not be called for named locks!");
      LockManager.RetailAssert(!(lockObject is string), "this method must not be called for strings!");
      LockManager.RetailAssert(LockManager.LockKindFromLockType(lockType) == LockManager.LockKind.Monitor, "Invalid lock kind for object lock");
      int num = (int) LockManager.LockLevelFromLockType(lockType, true);
      LockManager.RetailAssert((locksHeld != null ? 1 : 0) != 0, "Releasing a lock {0} the user does not hold", (object) lockType);
      lock (locksHeld)
      {
        LockManager.RetailAssert((locksHeld.Count != 0 ? 1 : 0) != 0, "Releasing a lock {0} the user does not hold", (object) lockType);
        LockManager.LockHeldEntry lockHeldEntry = locksHeld[locksHeld.Count - 1];
        if (lockHeldEntry.LockType != lockType || lockHeldEntry.LockObject != lockObject)
          LockManager.RetailAssert(false, "Lock Hierarchy violation: Releasing {0} violates {1}", (object) lockType, (object) lockHeldEntry.LockType);
        Monitor.Exit(lockObject);
        locksHeld.RemoveAt(locksHeld.Count - 1);
        return locksHeld.Count == 0;
      }
    }

    [Conditional("DEBUG")]
    internal void AssertLockHeld(object lockObject, LockManager.LockType lockType, long requestId) => LockManager.RetailAssert((this.TestLock(lockObject, lockType, requestId) ? 1 : 0) != 0, "lock assertion failed - lock {0} is not held", (object) lockType);

    [Conditional("DEBUG")]
    internal void AssertLockHeld(
      object lockObject,
      LockManager.LockType lockType,
      object locksHeld,
      long requestId)
    {
      LockManager.RetailAssert((this.TestLock(lockObject, lockType, (object) this.CheckLocksHeldObject(locksHeld), requestId) ? 1 : 0) != 0, "lock assertion failed - lock {0} is not held", (object) lockType);
    }

    [Conditional("DEBUG")]
    internal void AssertLockNotHeld(
      object lockObject,
      LockManager.LockType lockType,
      long requestId)
    {
      LockManager.RetailAssert((!this.TestLock(lockObject, lockType, requestId) ? 1 : 0) != 0, "lock assertion failed - lock {0} is already held", (object) lockType);
    }

    [Conditional("DEBUG")]
    internal void AssertLockNotHeld(
      object lockObject,
      LockManager.LockType lockType,
      object locksHeld,
      long requestId)
    {
      LockManager.RetailAssert((!this.TestLock(lockObject, lockType, (object) this.CheckLocksHeldObject(locksHeld), requestId) ? 1 : 0) != 0, "lock assertion failed - lock {0} is already held", (object) lockType);
    }

    protected virtual bool IsObjectLockHierarchyViolation(
      LockLevel lockLevel,
      LockLevel lastLockLevel)
    {
      return lockLevel <= lastLockLevel;
    }

    private static LockManager.LockKind LockKindFromLockType(LockManager.LockType lockType)
    {
      LockManager.LockKind lockKind = (LockManager.LockKind) (lockType & (LockManager.LockType) 1048560);
      int num;
      switch (lockKind)
      {
        case LockManager.LockKind.Shared:
        case LockManager.LockKind.Update:
        case LockManager.LockKind.Exclusive:
        case LockManager.LockKind.IntentUpdate:
        case LockManager.LockKind.IntentShared:
        case LockManager.LockKind.IntentExclusive:
        case LockManager.LockKind.SharedIntentExclusive:
        case LockManager.LockKind.SharedIntentUpdate:
        case LockManager.LockKind.UpdateIntentExclusive:
        case LockManager.LockKind.Monitor:
        case LockManager.LockKind.Reader:
        case LockManager.LockKind.Writer:
        case LockManager.LockKind.Connection:
          num = 1;
          break;
        default:
          num = lockKind == LockManager.LockKind.Exemption ? 1 : 0;
          break;
      }
      LockManager.RetailAssert(num != 0, "Invalid lock kind");
      return lockKind;
    }

    private static LockLevel LockLevelFromLockType(
      LockManager.LockType lockType,
      bool checkLockLevel = false)
    {
      LockLevel lockLevel = (LockLevel) (lockType & (LockManager.LockType) 15);
      if (checkLockLevel)
        LockManager.RetailAssert(lockLevel > LockLevel.First && lockLevel <= LockLevel.Last, "Invalid lockType");
      return lockLevel;
    }

    private static LockManager.LockType Max(LockManager.LockType one, LockManager.LockType two)
    {
      LockLevel lockLevel = LockManager.LockLevelFromLockType(one) == LockManager.LockLevelFromLockType(two) ? LockManager.LockLevelFromLockType(one) : throw new ArgumentOutOfRangeException();
      LockManager.LockKind lockKind1 = LockManager.LockKindFromLockType(one);
      LockManager.LockKind lockKind2 = LockManager.LockKindFromLockType(two);
      if (lockLevel == LockLevel.Resource || lockLevel == LockLevel.Last)
      {
        if (lockKind1 == LockManager.LockKind.Monitor && lockKind2 == LockManager.LockKind.Monitor)
          return one;
        if (lockKind1 == LockManager.LockKind.Reader && lockKind2 == LockManager.LockKind.Writer)
          return two;
        if (lockKind1 == LockManager.LockKind.Writer && lockKind2 == LockManager.LockKind.Reader || lockKind1 == lockKind2)
          return one;
      }
      return (LockManager.LockType) ((LockLevel) ((int) IntentLock.Max((LockMode) ((uint) lockKind1 >> 4), (LockMode) ((uint) lockKind2 >> 4)) << 4) | LockManager.LockLevelFromLockType(one));
    }

    internal class LockHeldEntry
    {
      internal LockManager.LockType LockType;
      internal IList<LockManager.LockType> PreviousLockTypes;
      internal LockManager.LockType EffectiveLockType;
      internal object LockObject;
      internal SortedDictionary<int, StackTrace> StackTraces;

      internal int RefCount => this.PreviousLockTypes == null ? 1 : this.PreviousLockTypes.Count + 1;

      internal LockHeldEntry(LockManager.LockType lockType, object lockObject)
      {
        this.LockType = lockType;
        this.EffectiveLockType = lockType;
        this.LockObject = lockObject;
      }

      internal void ReLock(LockManager.LockType lockType)
      {
        if (this.PreviousLockTypes == null)
          this.PreviousLockTypes = (IList<LockManager.LockType>) new List<LockManager.LockType>();
        this.PreviousLockTypes.Add(this.LockType);
        this.LockType = lockType;
        this.EffectiveLockType = LockManager.Max(this.LockType, this.EffectiveLockType);
      }

      internal bool UnLock(LockManager.LockType lockType)
      {
        LockManager.RetailAssert(this.RefCount > 1, "RefCount should be greater than 1");
        LockManager.RetailAssert(this.LockType == lockType, "You cannot release a lock you do not have");
        this.LockType = this.PreviousLockTypes[this.PreviousLockTypes.Count - 1];
        this.PreviousLockTypes.RemoveAt(this.PreviousLockTypes.Count - 1);
        LockManager.LockType one = this.LockType;
        for (int index = 0; index < this.PreviousLockTypes.Count && one != this.EffectiveLockType; ++index)
          one = LockManager.Max(one, this.PreviousLockTypes[index]);
        if (one == this.EffectiveLockType)
          return false;
        this.EffectiveLockType = one;
        return true;
      }

      [Conditional("DEBUG")]
      private void AddStackTrace(LockManager.LockType lockType)
      {
        if (!TeamFoundationTracingService.IsRawTracingEnabled(39909, TraceLevel.Verbose, "TeamFoundation", nameof (LockManager), (string[]) null) || LockManager.LockLevelFromLockType(lockType) != LockLevel.Resource)
          return;
        if (this.StackTraces == null)
          this.StackTraces = new SortedDictionary<int, StackTrace>();
        this.StackTraces.Add(this.StackTraces.Count, new StackTrace(5, true));
      }

      [Conditional("DEBUG")]
      private void RemoveStackTrace(LockManager.LockType lockType)
      {
        if (!TeamFoundationTracingService.IsRawTracingEnabled(39909, TraceLevel.Verbose, "TeamFoundation", nameof (LockManager), (string[]) null) || LockManager.LockLevelFromLockType(lockType) != LockLevel.Resource || this.StackTraces == null)
          return;
        this.StackTraces.Remove(this.StackTraces.Count);
      }
    }

    private class CachedLockName
    {
      private static readonly ILockName s_blobStorageLockName = (ILockName) new LockName<string>("BlobStorage", LockLevel.Resource);
      private static readonly ILockName s_emailLockName = (ILockName) new LockName<string>("Email", LockLevel.Resource);
      private static readonly ILockName s_restLockName = (ILockName) new LockName<string>("REST", LockLevel.Resource);
      private static readonly ILockName s_serviceBusLockName = (ILockName) new LockName<string>("ServiceBus", LockLevel.Resource);
      private static readonly ILockName s_sqlLockName = (ILockName) new LockName<string>("SQL", LockLevel.Resource);
      private static readonly ILockName s_tableStorageLockName = (ILockName) new LockName<string>("TableStorage", LockLevel.Resource);
      private static readonly ILockName s_documentDBLockName = (ILockName) new LockName<string>("DocumentDB", LockLevel.Resource);
      private static readonly ILockName s_exemptionLockName = (ILockName) new LockName<string>("Exemption", LockLevel.Resource);

      internal static ILockName GetConnectionLockName(ConnectionLockNameType type)
      {
        switch (type)
        {
          case ConnectionLockNameType.BlobStorage:
            return LockManager.CachedLockName.s_blobStorageLockName;
          case ConnectionLockNameType.Email:
            return LockManager.CachedLockName.s_emailLockName;
          case ConnectionLockNameType.REST:
            return LockManager.CachedLockName.s_restLockName;
          case ConnectionLockNameType.ServiceBus:
            return LockManager.CachedLockName.s_serviceBusLockName;
          case ConnectionLockNameType.SQL:
            return LockManager.CachedLockName.s_sqlLockName;
          case ConnectionLockNameType.TableStorage:
            return LockManager.CachedLockName.s_tableStorageLockName;
          case ConnectionLockNameType.DocumentDB:
            return LockManager.CachedLockName.s_documentDBLockName;
          default:
            throw new InvalidOperationException("invalid lock name for connection lock kind");
        }
      }

      internal static ILockName GetExemptionLockName() => LockManager.CachedLockName.s_exemptionLockName;

      internal static bool IsValidConnectionLockName(ILockName lockName) => lockName == LockManager.CachedLockName.s_blobStorageLockName || lockName == LockManager.CachedLockName.s_emailLockName || lockName == LockManager.CachedLockName.s_restLockName || lockName == LockManager.CachedLockName.s_serviceBusLockName || lockName == LockManager.CachedLockName.s_sqlLockName || lockName == LockManager.CachedLockName.s_tableStorageLockName || lockName == LockManager.CachedLockName.s_documentDBLockName;
    }

    internal class NamedLockObject : INamedLockObject
    {
      private readonly ILockName lockName;
      private IntentLock intentLock;
      private ReaderWriterLockSlim readerWriterLock;
      private int refCount;
      private int lastOwnerThreadId;
      private DateTime lastUsed;

      internal NamedLockObject(ILockName lockName) => this.lockName = lockName;

      internal ILockName LockName => this.lockName;

      internal int RefCount => this.refCount;

      internal DateTime LastUsed => this.lastUsed;

      internal bool TryAddRef()
      {
        while (true)
        {
          int refCount;
          int num;
          do
          {
            refCount = this.refCount;
            if (refCount >= 0)
            {
              num = refCount + 1;
              if (num <= 0)
                goto label_4;
            }
            else
              goto label_5;
          }
          while (refCount != Interlocked.CompareExchange(ref this.refCount, num, refCount));
          break;
label_4:
          LockManager.RetailAssert(false, "lock object refcount overflow");
        }
        return true;
label_5:
        return false;
      }

      internal void ReleaseRef()
      {
        int num = Interlocked.Decrement(ref this.refCount);
        if (num > 0)
          return;
        LockManager.RetailAssert(num == 0, "lock object refcounting problem - refcount goes negative");
        this.lastUsed = DateTime.UtcNow;
      }

      internal bool CanDispose(DateTime cutoffTime) => this.refCount == 0 && this.lastUsed <= cutoffTime;

      internal bool TryDispose()
      {
        if (Interlocked.CompareExchange(ref this.refCount, -1, 0) != 0)
          return false;
        if (this.intentLock != null)
        {
          this.intentLock.Dispose();
          this.intentLock = (IntentLock) null;
        }
        return true;
      }

      internal void GetLock(ILockName lockName, LockManager.LockKind lockKind, long requestId)
      {
        LockManager.RetailAssert(this.refCount > 0, "the lock must be refcounted!");
        string name = lockName.ToString();
        switch (lockKind)
        {
          case LockManager.LockKind.Shared:
            this.GetIntentLock(name).EnterLock(LockMode.Shared, requestId);
            goto case LockManager.LockKind.Exemption;
          case LockManager.LockKind.Update:
            this.GetIntentLock(name).EnterLock(LockMode.Update, requestId);
            goto case LockManager.LockKind.Exemption;
          case LockManager.LockKind.Exclusive:
            this.GetIntentLock(name).EnterLock(LockMode.Exclusive, requestId);
            goto case LockManager.LockKind.Exemption;
          case LockManager.LockKind.IntentUpdate:
            this.GetIntentLock(name).EnterLock(LockMode.IntentUpdate, requestId);
            goto case LockManager.LockKind.Exemption;
          case LockManager.LockKind.IntentShared:
            this.GetIntentLock(name).EnterLock(LockMode.IntentShared, requestId);
            goto case LockManager.LockKind.Exemption;
          case LockManager.LockKind.IntentExclusive:
            this.GetIntentLock(name).EnterLock(LockMode.IntentExclusive, requestId);
            goto case LockManager.LockKind.Exemption;
          case LockManager.LockKind.SharedIntentExclusive:
            this.GetIntentLock(name).EnterLock(LockMode.SharedIntentExclusive, requestId);
            goto case LockManager.LockKind.Exemption;
          case LockManager.LockKind.Monitor:
            LockManager.RetailAssert(this.intentLock == null, "inconsistent lock kind for a lock object");
            Monitor.Enter((object) this);
            goto case LockManager.LockKind.Exemption;
          case LockManager.LockKind.Reader:
            this.GetReaderWriterLock(name).EnterReadLock();
            goto case LockManager.LockKind.Exemption;
          case LockManager.LockKind.Writer:
            this.GetReaderWriterLock(name).EnterWriteLock();
            goto case LockManager.LockKind.Exemption;
          case LockManager.LockKind.Connection:
            LockManager.RetailAssert(LockManager.CachedLockName.IsValidConnectionLockName(lockName), "invalid lock name for connection lock kind");
            goto case LockManager.LockKind.Exemption;
          case LockManager.LockKind.Exemption:
            this.lastOwnerThreadId = Environment.CurrentManagedThreadId;
            break;
          default:
            LockManager.RetailAssert(false, "invalid lock kind");
            goto case LockManager.LockKind.Exemption;
        }
      }

      internal bool TryGetLock(
        ILockName lockName,
        LockManager.LockKind lockKind,
        long requestId,
        int millisecondsTimeout)
      {
        LockManager.RetailAssert(this.refCount > 0, "the lock must be refcounted!");
        bool flag = false;
        string name = lockName.ToString();
        TimeSpan timeSpan = new TimeSpan(0, 0, 0, 0, millisecondsTimeout);
        switch (lockKind)
        {
          case LockManager.LockKind.Shared:
            flag = this.GetIntentLock(name).TryEnterLock(LockMode.Shared, requestId, timeSpan);
            goto case LockManager.LockKind.Exemption;
          case LockManager.LockKind.Update:
            flag = this.GetIntentLock(name).TryEnterLock(LockMode.Update, requestId, timeSpan);
            goto case LockManager.LockKind.Exemption;
          case LockManager.LockKind.Exclusive:
            flag = this.GetIntentLock(name).TryEnterLock(LockMode.Exclusive, requestId, timeSpan);
            goto case LockManager.LockKind.Exemption;
          case LockManager.LockKind.IntentUpdate:
            flag = this.GetIntentLock(name).TryEnterLock(LockMode.IntentUpdate, requestId, timeSpan);
            goto case LockManager.LockKind.Exemption;
          case LockManager.LockKind.IntentShared:
            flag = this.GetIntentLock(name).TryEnterLock(LockMode.IntentShared, requestId, timeSpan);
            goto case LockManager.LockKind.Exemption;
          case LockManager.LockKind.IntentExclusive:
            flag = this.GetIntentLock(name).TryEnterLock(LockMode.IntentExclusive, requestId, timeSpan);
            goto case LockManager.LockKind.Exemption;
          case LockManager.LockKind.SharedIntentExclusive:
            flag = this.GetIntentLock(name).TryEnterLock(LockMode.SharedIntentExclusive, requestId, timeSpan);
            goto case LockManager.LockKind.Exemption;
          case LockManager.LockKind.Monitor:
            LockManager.RetailAssert(this.intentLock == null, "inconsistent lock kind for a lock object");
            flag = Monitor.TryEnter((object) this, millisecondsTimeout);
            goto case LockManager.LockKind.Exemption;
          case LockManager.LockKind.Reader:
            flag = this.GetReaderWriterLock(name).TryEnterReadLock(timeSpan);
            goto case LockManager.LockKind.Exemption;
          case LockManager.LockKind.Writer:
            flag = this.GetReaderWriterLock(name).TryEnterWriteLock(timeSpan);
            goto case LockManager.LockKind.Exemption;
          case LockManager.LockKind.Connection:
            LockManager.RetailAssert(LockManager.CachedLockName.IsValidConnectionLockName(lockName), "invalid lock name for connection lock kind");
            goto case LockManager.LockKind.Exemption;
          case LockManager.LockKind.Exemption:
            if (flag)
              this.lastOwnerThreadId = Environment.CurrentManagedThreadId;
            return flag;
          default:
            LockManager.RetailAssert(false, "invalid lock kind");
            goto case LockManager.LockKind.Exemption;
        }
      }

      internal void ReleaseLock(ILockName lockName, LockManager.LockKind lockKind, long requestId)
      {
        LockManager.RetailAssert(this.refCount > 0, "the lock must be refcounted!");
        string name = lockName.ToString();
        switch (lockKind)
        {
          case LockManager.LockKind.Shared:
            this.GetIntentLock(name).ExitLock(LockMode.Shared, requestId);
            break;
          case LockManager.LockKind.Update:
            this.GetIntentLock(name).ExitLock(LockMode.Update, requestId);
            break;
          case LockManager.LockKind.Exclusive:
            this.GetIntentLock(name).ExitLock(LockMode.Exclusive, requestId);
            break;
          case LockManager.LockKind.IntentUpdate:
            this.GetIntentLock(name).ExitLock(LockMode.IntentUpdate, requestId);
            break;
          case LockManager.LockKind.IntentShared:
            this.GetIntentLock(name).ExitLock(LockMode.IntentShared, requestId);
            break;
          case LockManager.LockKind.IntentExclusive:
            this.GetIntentLock(name).ExitLock(LockMode.IntentExclusive, requestId);
            break;
          case LockManager.LockKind.SharedIntentExclusive:
            this.GetIntentLock(name).ExitLock(LockMode.SharedIntentExclusive, requestId);
            break;
          case LockManager.LockKind.Monitor:
            LockManager.RetailAssert(this.intentLock == null, "inconsistent lock kind for a lock object");
            Monitor.Exit((object) this);
            break;
          case LockManager.LockKind.Reader:
            this.GetReaderWriterLock(name).ExitReadLock();
            break;
          case LockManager.LockKind.Writer:
            this.GetReaderWriterLock(name).ExitWriteLock();
            break;
          case LockManager.LockKind.Connection:
            LockManager.RetailAssert(LockManager.CachedLockName.IsValidConnectionLockName(lockName), "trying to release connection lock with invalid lock name");
            break;
          case LockManager.LockKind.Exemption:
            break;
          default:
            LockManager.RetailAssert(false, "invalid lock kind");
            break;
        }
      }

      public override string ToString() => this.lockName.ToString();

      private IntentLock GetIntentLock(string name)
      {
        if (this.intentLock == null)
        {
          IntentLock intentLock = new IntentLock(name);
          if (Interlocked.CompareExchange<IntentLock>(ref this.intentLock, intentLock, (IntentLock) null) != null)
            intentLock.Dispose();
        }
        return this.intentLock;
      }

      private ReaderWriterLockSlim GetReaderWriterLock(string name)
      {
        if (this.readerWriterLock == null)
        {
          ReaderWriterLockSlim readerWriterLockSlim = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
          if (Interlocked.CompareExchange<ReaderWriterLockSlim>(ref this.readerWriterLock, readerWriterLockSlim, (ReaderWriterLockSlim) null) != null)
            readerWriterLockSlim.Dispose();
        }
        return this.readerWriterLock;
      }
    }

    private class NamedLockObjectsPartition
    {
      private int cleanupSkipCounter;
      private DateTime lastCleanupTime;
      private IDictionary<ILockName, LockManager.NamedLockObject> lockObjectsDictionary;

      internal NamedLockObjectsPartition()
      {
        LockManager.RetailAssert(LockManager.StaleLockCleanupSkipCount > 0, "StaleLockCleanupSkipCount must be positive");
        LockManager.RetailAssert(LockManager.StaleLockCleanupInterval >= TimeSpan.Zero, "StaleLockCleanupInterval must be zero or positive");
        this.cleanupSkipCounter = 0;
        this.lastCleanupTime = DateTime.UtcNow;
        this.lockObjectsDictionary = (IDictionary<ILockName, LockManager.NamedLockObject>) new Dictionary<ILockName, LockManager.NamedLockObject>(12);
      }

      internal IDictionary<ILockName, LockManager.NamedLockObject> LockObjectsDictionary => this.lockObjectsDictionary;

      internal bool ShouldSkipCleanup() => ++this.cleanupSkipCounter % LockManager.StaleLockCleanupSkipCount != 0;

      internal bool TimeToRunCleanup(out DateTime freeLocksUsedBeforeTime)
      {
        DateTime utcNow = DateTime.UtcNow;
        freeLocksUsedBeforeTime = utcNow.Add(-LockManager.StaleLockCleanupInterval);
        if (!(this.lastCleanupTime <= freeLocksUsedBeforeTime))
          return false;
        this.lastCleanupTime = utcNow;
        return true;
      }
    }

    internal struct ObjectLockFrame : IDisposable
    {
      private LockManager lockManager;
      private object lockObject;
      private LockManager.LockType lockType;
      private IList<LockManager.LockHeldEntry> locksHeld;
      private long requestId;
      private bool freeLocksHeldEntries;

      internal ObjectLockFrame(
        LockManager lockManager,
        object lockObject,
        LockManager.LockType lockType,
        IList<LockManager.LockHeldEntry> locksHeld,
        long requestId,
        bool freeLocksHeldEntries = true)
      {
        LockManager.RetailAssert(lockManager != null, "Invalid Lock Manager reference");
        lockManager.GetObjectLockImpl(lockObject, lockType, locksHeld, requestId, false);
        this.lockObject = lockObject;
        this.lockType = lockType;
        this.lockManager = lockManager;
        this.locksHeld = locksHeld;
        this.requestId = requestId;
        this.freeLocksHeldEntries = freeLocksHeldEntries;
      }

      public void Dispose()
      {
        if (this.lockObject == null || !this.lockManager.ReleaseObjectLockImpl(this.lockObject, this.lockType, this.locksHeld, this.requestId) || !this.freeLocksHeldEntries)
          return;
        this.lockManager.FreeLocksHeldEntries(this.requestId);
      }
    }

    internal enum LockKind
    {
      Free = 16, // 0x00000010
      Shared = 32, // 0x00000020
      Update = 48, // 0x00000030
      Exclusive = 64, // 0x00000040
      IntentUpdate = 80, // 0x00000050
      IntentShared = 96, // 0x00000060
      IntentExclusive = 112, // 0x00000070
      SharedIntentExclusive = 128, // 0x00000080
      SharedIntentUpdate = 144, // 0x00000090
      UpdateIntentExclusive = 160, // 0x000000A0
      Monitor = 4096, // 0x00001000
      Reader = 4112, // 0x00001010
      Writer = 4128, // 0x00001020
      Connection = 4144, // 0x00001030
      Exemption = 4160, // 0x00001040
      Last = 4161, // 0x00001041
    }

    internal enum LockType
    {
      HostLockShared = 33, // 0x00000021
      HostLockUpdate = 49, // 0x00000031
      HostLockExclusive = 65, // 0x00000041
      HostLockIntentUpdate = 81, // 0x00000051
      HostLockIntentShared = 97, // 0x00000061
      HostLockIntentExclusive = 113, // 0x00000071
      HostLockSharedIntentExclusive = 129, // 0x00000081
      HostLockSharedIntentUpdate = 145, // 0x00000091
      HostLockUpdateIntentExclusive = 161, // 0x000000A1
      ResourceMonitorLock = 4099, // 0x00001003
      LeafMonitorLock = 4100, // 0x00001004
      AuthorityShared = 4114, // 0x00001012
      ResourceShared = 4115, // 0x00001013
      HostTableShared = 4116, // 0x00001014
      MapLockShared = 4116, // 0x00001014
      AuthorityExclusive = 4130, // 0x00001022
      ResourceExclusive = 4131, // 0x00001023
      HostTableExclusive = 4132, // 0x00001024
      MapLockExclusive = 4132, // 0x00001024
      ResourceConnection = 4147, // 0x00001033
      ResourceExemption = 4163, // 0x00001043
    }
  }
}
