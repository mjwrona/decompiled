// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.IntentLock
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class IntentLock : IDisposable
  {
    private static readonly LockMode[,] s_lockConversionMatrix;
    private static readonly bool[,] s_granularLocksCompatibilityMatrix = new bool[11, 11];
    private List<IntentLock.LockWaiter> m_waiters;
    private HashSet<IntentLock.LockHolder> m_holders;
    private object m_internalLock;
    private LockMode m_currentLockState;
    private readonly string m_name;
    private const string s_Area = "HostManagement";
    private const string s_Layer = "IntentLock";

    static IntentLock()
    {
      IntentLock.s_lockConversionMatrix = new LockMode[11, 11];
      IntentLock.s_granularLocksCompatibilityMatrix[1, 6] = true;
      IntentLock.s_granularLocksCompatibilityMatrix[1, 7] = true;
      IntentLock.s_granularLocksCompatibilityMatrix[1, 2] = true;
      IntentLock.s_granularLocksCompatibilityMatrix[1, 8] = true;
      IntentLock.s_granularLocksCompatibilityMatrix[1, 3] = true;
      IntentLock.s_granularLocksCompatibilityMatrix[1, 4] = true;
      IntentLock.s_granularLocksCompatibilityMatrix[1, 5] = true;
      IntentLock.s_granularLocksCompatibilityMatrix[1, 9] = true;
      IntentLock.s_granularLocksCompatibilityMatrix[1, 10] = true;
      IntentLock.s_granularLocksCompatibilityMatrix[6, 6] = true;
      IntentLock.s_granularLocksCompatibilityMatrix[6, 7] = true;
      IntentLock.s_granularLocksCompatibilityMatrix[6, 2] = true;
      IntentLock.s_granularLocksCompatibilityMatrix[6, 8] = true;
      IntentLock.s_granularLocksCompatibilityMatrix[6, 3] = true;
      IntentLock.s_granularLocksCompatibilityMatrix[6, 4] = false;
      IntentLock.s_granularLocksCompatibilityMatrix[6, 5] = true;
      IntentLock.s_granularLocksCompatibilityMatrix[6, 9] = true;
      IntentLock.s_granularLocksCompatibilityMatrix[6, 10] = true;
      IntentLock.s_granularLocksCompatibilityMatrix[7, 6] = true;
      IntentLock.s_granularLocksCompatibilityMatrix[7, 7] = true;
      IntentLock.s_granularLocksCompatibilityMatrix[7, 2] = false;
      IntentLock.s_granularLocksCompatibilityMatrix[7, 8] = false;
      IntentLock.s_granularLocksCompatibilityMatrix[7, 3] = false;
      IntentLock.s_granularLocksCompatibilityMatrix[7, 4] = false;
      IntentLock.s_granularLocksCompatibilityMatrix[7, 5] = false;
      IntentLock.s_granularLocksCompatibilityMatrix[7, 9] = false;
      IntentLock.s_granularLocksCompatibilityMatrix[7, 10] = false;
      IntentLock.s_granularLocksCompatibilityMatrix[2, 6] = true;
      IntentLock.s_granularLocksCompatibilityMatrix[2, 7] = false;
      IntentLock.s_granularLocksCompatibilityMatrix[2, 2] = true;
      IntentLock.s_granularLocksCompatibilityMatrix[2, 8] = false;
      IntentLock.s_granularLocksCompatibilityMatrix[2, 3] = true;
      IntentLock.s_granularLocksCompatibilityMatrix[2, 4] = false;
      IntentLock.s_granularLocksCompatibilityMatrix[2, 5] = true;
      IntentLock.s_granularLocksCompatibilityMatrix[2, 10] = false;
      IntentLock.s_granularLocksCompatibilityMatrix[2, 9] = true;
      IntentLock.s_granularLocksCompatibilityMatrix[8, 6] = true;
      IntentLock.s_granularLocksCompatibilityMatrix[8, 7] = false;
      IntentLock.s_granularLocksCompatibilityMatrix[8, 2] = false;
      IntentLock.s_granularLocksCompatibilityMatrix[8, 8] = false;
      IntentLock.s_granularLocksCompatibilityMatrix[8, 3] = false;
      IntentLock.s_granularLocksCompatibilityMatrix[8, 4] = false;
      IntentLock.s_granularLocksCompatibilityMatrix[8, 9] = false;
      IntentLock.s_granularLocksCompatibilityMatrix[8, 10] = false;
      IntentLock.s_granularLocksCompatibilityMatrix[8, 5] = true;
      IntentLock.s_granularLocksCompatibilityMatrix[3, 6] = true;
      IntentLock.s_granularLocksCompatibilityMatrix[3, 7] = false;
      IntentLock.s_granularLocksCompatibilityMatrix[3, 2] = true;
      IntentLock.s_granularLocksCompatibilityMatrix[3, 8] = false;
      IntentLock.s_granularLocksCompatibilityMatrix[3, 3] = false;
      IntentLock.s_granularLocksCompatibilityMatrix[3, 4] = false;
      IntentLock.s_granularLocksCompatibilityMatrix[3, 5] = false;
      IntentLock.s_granularLocksCompatibilityMatrix[3, 9] = false;
      IntentLock.s_granularLocksCompatibilityMatrix[3, 10] = false;
      IntentLock.s_granularLocksCompatibilityMatrix[4, 6] = false;
      IntentLock.s_granularLocksCompatibilityMatrix[4, 7] = false;
      IntentLock.s_granularLocksCompatibilityMatrix[4, 2] = false;
      IntentLock.s_granularLocksCompatibilityMatrix[4, 8] = false;
      IntentLock.s_granularLocksCompatibilityMatrix[4, 3] = false;
      IntentLock.s_granularLocksCompatibilityMatrix[4, 4] = false;
      IntentLock.s_granularLocksCompatibilityMatrix[4, 5] = false;
      IntentLock.s_granularLocksCompatibilityMatrix[4, 9] = false;
      IntentLock.s_granularLocksCompatibilityMatrix[4, 10] = false;
      IntentLock.s_granularLocksCompatibilityMatrix[5, 6] = true;
      IntentLock.s_granularLocksCompatibilityMatrix[5, 7] = true;
      IntentLock.s_granularLocksCompatibilityMatrix[5, 2] = true;
      IntentLock.s_granularLocksCompatibilityMatrix[5, 8] = true;
      IntentLock.s_granularLocksCompatibilityMatrix[5, 3] = false;
      IntentLock.s_granularLocksCompatibilityMatrix[5, 4] = false;
      IntentLock.s_granularLocksCompatibilityMatrix[5, 5] = true;
      IntentLock.s_granularLocksCompatibilityMatrix[5, 9] = true;
      IntentLock.s_granularLocksCompatibilityMatrix[5, 10] = false;
      IntentLock.s_granularLocksCompatibilityMatrix[9, 6] = true;
      IntentLock.s_granularLocksCompatibilityMatrix[9, 7] = false;
      IntentLock.s_granularLocksCompatibilityMatrix[9, 2] = true;
      IntentLock.s_granularLocksCompatibilityMatrix[9, 8] = false;
      IntentLock.s_granularLocksCompatibilityMatrix[9, 3] = false;
      IntentLock.s_granularLocksCompatibilityMatrix[9, 4] = false;
      IntentLock.s_granularLocksCompatibilityMatrix[9, 5] = true;
      IntentLock.s_granularLocksCompatibilityMatrix[9, 9] = true;
      IntentLock.s_granularLocksCompatibilityMatrix[9, 10] = false;
      IntentLock.s_granularLocksCompatibilityMatrix[10, 6] = true;
      IntentLock.s_granularLocksCompatibilityMatrix[10, 7] = false;
      IntentLock.s_granularLocksCompatibilityMatrix[10, 2] = false;
      IntentLock.s_granularLocksCompatibilityMatrix[10, 8] = false;
      IntentLock.s_granularLocksCompatibilityMatrix[10, 3] = false;
      IntentLock.s_granularLocksCompatibilityMatrix[10, 4] = false;
      IntentLock.s_granularLocksCompatibilityMatrix[10, 5] = false;
      IntentLock.s_granularLocksCompatibilityMatrix[10, 9] = false;
      IntentLock.s_granularLocksCompatibilityMatrix[10, 10] = false;
      IntentLock.s_lockConversionMatrix[1, 6] = LockMode.IntentShared;
      IntentLock.s_lockConversionMatrix[1, 7] = LockMode.IntentExclusive;
      IntentLock.s_lockConversionMatrix[1, 2] = LockMode.Shared;
      IntentLock.s_lockConversionMatrix[1, 8] = LockMode.SharedIntentExclusive;
      IntentLock.s_lockConversionMatrix[1, 3] = LockMode.Update;
      IntentLock.s_lockConversionMatrix[1, 4] = LockMode.Exclusive;
      IntentLock.s_lockConversionMatrix[1, 5] = LockMode.IntentUpdate;
      IntentLock.s_lockConversionMatrix[1, 9] = LockMode.SharedIntentUpdate;
      IntentLock.s_lockConversionMatrix[1, 10] = LockMode.UpdateIntentExclusive;
      IntentLock.s_lockConversionMatrix[6, 6] = LockMode.IntentShared;
      IntentLock.s_lockConversionMatrix[6, 7] = LockMode.IntentExclusive;
      IntentLock.s_lockConversionMatrix[6, 2] = LockMode.Shared;
      IntentLock.s_lockConversionMatrix[6, 8] = LockMode.SharedIntentExclusive;
      IntentLock.s_lockConversionMatrix[6, 3] = LockMode.Update;
      IntentLock.s_lockConversionMatrix[6, 4] = LockMode.Exclusive;
      IntentLock.s_lockConversionMatrix[6, 5] = LockMode.IntentUpdate;
      IntentLock.s_lockConversionMatrix[6, 9] = LockMode.SharedIntentUpdate;
      IntentLock.s_lockConversionMatrix[6, 10] = LockMode.UpdateIntentExclusive;
      IntentLock.s_lockConversionMatrix[7, 6] = LockMode.IntentExclusive;
      IntentLock.s_lockConversionMatrix[7, 7] = LockMode.IntentExclusive;
      IntentLock.s_lockConversionMatrix[7, 2] = LockMode.SharedIntentExclusive;
      IntentLock.s_lockConversionMatrix[7, 8] = LockMode.SharedIntentExclusive;
      IntentLock.s_lockConversionMatrix[7, 3] = LockMode.Exclusive;
      IntentLock.s_lockConversionMatrix[7, 4] = LockMode.Exclusive;
      IntentLock.s_lockConversionMatrix[7, 5] = LockMode.IntentExclusive;
      IntentLock.s_lockConversionMatrix[7, 9] = LockMode.SharedIntentExclusive;
      IntentLock.s_lockConversionMatrix[7, 10] = LockMode.UpdateIntentExclusive;
      IntentLock.s_lockConversionMatrix[2, 6] = LockMode.Shared;
      IntentLock.s_lockConversionMatrix[2, 7] = LockMode.SharedIntentExclusive;
      IntentLock.s_lockConversionMatrix[2, 2] = LockMode.Shared;
      IntentLock.s_lockConversionMatrix[2, 8] = LockMode.SharedIntentExclusive;
      IntentLock.s_lockConversionMatrix[2, 3] = LockMode.Update;
      IntentLock.s_lockConversionMatrix[2, 4] = LockMode.Exclusive;
      IntentLock.s_lockConversionMatrix[2, 5] = LockMode.SharedIntentUpdate;
      IntentLock.s_lockConversionMatrix[2, 9] = LockMode.SharedIntentUpdate;
      IntentLock.s_lockConversionMatrix[2, 10] = LockMode.UpdateIntentExclusive;
      IntentLock.s_lockConversionMatrix[8, 6] = LockMode.SharedIntentExclusive;
      IntentLock.s_lockConversionMatrix[8, 7] = LockMode.SharedIntentExclusive;
      IntentLock.s_lockConversionMatrix[8, 2] = LockMode.SharedIntentExclusive;
      IntentLock.s_lockConversionMatrix[8, 8] = LockMode.SharedIntentExclusive;
      IntentLock.s_lockConversionMatrix[8, 3] = LockMode.SharedIntentExclusive;
      IntentLock.s_lockConversionMatrix[8, 4] = LockMode.Exclusive;
      IntentLock.s_lockConversionMatrix[8, 5] = LockMode.SharedIntentExclusive;
      IntentLock.s_lockConversionMatrix[8, 9] = LockMode.SharedIntentExclusive;
      IntentLock.s_lockConversionMatrix[8, 10] = LockMode.UpdateIntentExclusive;
      IntentLock.s_lockConversionMatrix[3, 6] = LockMode.Update;
      IntentLock.s_lockConversionMatrix[3, 7] = LockMode.Exclusive;
      IntentLock.s_lockConversionMatrix[3, 2] = LockMode.Update;
      IntentLock.s_lockConversionMatrix[3, 8] = LockMode.SharedIntentExclusive;
      IntentLock.s_lockConversionMatrix[3, 3] = LockMode.Update;
      IntentLock.s_lockConversionMatrix[3, 4] = LockMode.Exclusive;
      IntentLock.s_lockConversionMatrix[3, 5] = LockMode.Update;
      IntentLock.s_lockConversionMatrix[3, 9] = LockMode.Update;
      IntentLock.s_lockConversionMatrix[3, 10] = LockMode.UpdateIntentExclusive;
      IntentLock.s_lockConversionMatrix[5, 6] = LockMode.IntentUpdate;
      IntentLock.s_lockConversionMatrix[5, 7] = LockMode.IntentExclusive;
      IntentLock.s_lockConversionMatrix[5, 2] = LockMode.SharedIntentUpdate;
      IntentLock.s_lockConversionMatrix[5, 8] = LockMode.SharedIntentExclusive;
      IntentLock.s_lockConversionMatrix[5, 3] = LockMode.Update;
      IntentLock.s_lockConversionMatrix[5, 4] = LockMode.Exclusive;
      IntentLock.s_lockConversionMatrix[5, 5] = LockMode.IntentUpdate;
      IntentLock.s_lockConversionMatrix[5, 9] = LockMode.SharedIntentUpdate;
      IntentLock.s_lockConversionMatrix[5, 10] = LockMode.UpdateIntentExclusive;
      IntentLock.s_lockConversionMatrix[9, 6] = LockMode.SharedIntentUpdate;
      IntentLock.s_lockConversionMatrix[9, 7] = LockMode.SharedIntentExclusive;
      IntentLock.s_lockConversionMatrix[9, 2] = LockMode.SharedIntentUpdate;
      IntentLock.s_lockConversionMatrix[9, 8] = LockMode.SharedIntentExclusive;
      IntentLock.s_lockConversionMatrix[9, 3] = LockMode.Update;
      IntentLock.s_lockConversionMatrix[9, 4] = LockMode.Exclusive;
      IntentLock.s_lockConversionMatrix[9, 5] = LockMode.SharedIntentUpdate;
      IntentLock.s_lockConversionMatrix[9, 9] = LockMode.SharedIntentUpdate;
      IntentLock.s_lockConversionMatrix[9, 10] = LockMode.UpdateIntentExclusive;
      IntentLock.s_lockConversionMatrix[10, 6] = LockMode.UpdateIntentExclusive;
      IntentLock.s_lockConversionMatrix[10, 7] = LockMode.UpdateIntentExclusive;
      IntentLock.s_lockConversionMatrix[10, 2] = LockMode.UpdateIntentExclusive;
      IntentLock.s_lockConversionMatrix[10, 8] = LockMode.UpdateIntentExclusive;
      IntentLock.s_lockConversionMatrix[10, 3] = LockMode.UpdateIntentExclusive;
      IntentLock.s_lockConversionMatrix[10, 4] = LockMode.Exclusive;
      IntentLock.s_lockConversionMatrix[10, 5] = LockMode.UpdateIntentExclusive;
      IntentLock.s_lockConversionMatrix[10, 9] = LockMode.UpdateIntentExclusive;
      IntentLock.s_lockConversionMatrix[10, 10] = LockMode.UpdateIntentExclusive;
      IntentLock.s_lockConversionMatrix[4, 6] = LockMode.Exclusive;
      IntentLock.s_lockConversionMatrix[4, 7] = LockMode.Exclusive;
      IntentLock.s_lockConversionMatrix[4, 2] = LockMode.Exclusive;
      IntentLock.s_lockConversionMatrix[4, 8] = LockMode.Exclusive;
      IntentLock.s_lockConversionMatrix[4, 3] = LockMode.Exclusive;
      IntentLock.s_lockConversionMatrix[4, 4] = LockMode.Exclusive;
      IntentLock.s_lockConversionMatrix[4, 5] = LockMode.Exclusive;
      IntentLock.s_lockConversionMatrix[4, 9] = LockMode.Exclusive;
      IntentLock.s_lockConversionMatrix[4, 10] = LockMode.Exclusive;
    }

    public static bool IsCompatible(LockMode currentMode, LockMode newMode) => IntentLock.s_granularLocksCompatibilityMatrix[(int) currentMode, (int) newMode];

    public static LockMode Max(LockMode one, LockMode two) => IntentLock.s_lockConversionMatrix[(int) one, (int) two];

    public IntentLock(string name)
    {
      this.m_internalLock = new object();
      this.m_currentLockState = LockMode.Free;
      this.m_holders = new HashSet<IntentLock.LockHolder>();
      this.m_name = name;
    }

    public void EnterLock(LockMode requestedlockMode, long request) => this.TryEnterLock(requestedlockMode, request, Timeout.InfiniteTimeSpan);

    public bool TryEnterLock(LockMode requestedlockMode, long request, TimeSpan lockTimeout)
    {
      IntentLock.LockWaiter lockWaiter = (IntentLock.LockWaiter) null;
      IntentLock.LockHolder objB = new IntentLock.LockHolder(request, requestedlockMode);
      int currentThreadId = IntentLock.GetCurrentThreadId();
label_1:
      do
      {
        bool flag1 = true;
        lock (this.m_internalLock)
        {
          bool flag2 = this.m_holders.Contains(objB);
          if (flag2 && !IntentLock.IsCompatible(this.m_currentLockState, requestedlockMode))
          {
            foreach (IntentLock.LockHolder holder in this.m_holders)
            {
              if (!object.Equals((object) holder, (object) objB) && !IntentLock.IsCompatible(holder.LockMode, requestedlockMode))
              {
                flag1 = false;
                break;
              }
            }
          }
          if (flag2 & flag1 || IntentLock.IsCompatible(this.m_currentLockState, requestedlockMode) && (this.m_waiters == null || this.m_waiters.Count == 0 || this.m_waiters.Count > 0 && this.m_waiters[0] == lockWaiter))
          {
            if (flag2)
              this.m_holders.Remove(objB);
            this.m_holders.Add(objB);
            if (requestedlockMode != this.m_currentLockState)
            {
              this.m_currentLockState = LockMode.Free;
              foreach (IntentLock.LockHolder holder in this.m_holders)
                this.m_currentLockState = IntentLock.Max(this.m_currentLockState, holder.LockMode);
            }
            if (lockWaiter != null)
            {
              this.m_waiters.Remove(lockWaiter);
              lockWaiter.Dispose();
            }
            this.PeekAndWakeUp();
            return true;
          }
          if (lockTimeout == TimeSpan.Zero)
            return false;
          if (lockWaiter == null)
          {
            lockWaiter = new IntentLock.LockWaiter(request, requestedlockMode);
            if (this.m_waiters == null)
              this.m_waiters = new List<IntentLock.LockWaiter>();
            this.m_waiters.Add(lockWaiter);
          }
        }
      }
      while (lockWaiter == null);
      bool flag = false;
      try
      {
        flag = lockWaiter.Event.WaitOne(lockTimeout);
        if (!flag)
        {
          TeamFoundationTracingService.TraceRaw(28305, TraceLevel.Warning, "HostManagement", nameof (IntentLock), "Thread {0}/{1} waiting for lock {2} has timed out", (object) currentThreadId, (object) Environment.CurrentManagedThreadId, (object) this.m_name);
          if (lockTimeout > TimeSpan.FromSeconds(1.0))
          {
            lock (this.m_internalLock)
            {
              foreach (IntentLock.LockHolder holder in this.m_holders)
                TeamFoundationTracingService.TraceRawAlwaysOn(28307, TraceLevel.Warning, "HostManagement", nameof (IntentLock), string.Format("Lock {0} is owned by RequestId:{1} in mode {2}, OwningThreadId:{3}, Tid:{4} TIMESTAMP:{5}", (object) this.m_name, (object) holder.Request, (object) holder.LockMode, (object) holder.OwningThreadId, (object) holder.OwningNativeThreadId, (object) holder.DateAcquired));
            }
          }
          return false;
        }
        goto label_1;
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(28309, TraceLevel.Error, "HostManagement", nameof (IntentLock), ex);
        throw;
      }
      finally
      {
        if (!flag)
        {
          lock (this.m_internalLock)
          {
            this.m_waiters.Remove(lockWaiter);
            lockWaiter.Dispose();
            this.PeekAndWakeUp();
          }
        }
      }
    }

    public void ExitLock(LockMode lockMode, long request)
    {
      IntentLock.LockHolder lockHolder = new IntentLock.LockHolder(request, lockMode);
      lock (this.m_internalLock)
      {
        if (!this.m_holders.Remove(lockHolder))
        {
          TeamFoundationTracingService.TraceRaw(28325, TraceLevel.Error, "HostManagement", nameof (IntentLock), "You cannot exit this lock because you did not previously enter it!");
          throw new InvalidOperationException("You cannot exit this lock because you did not previously enter it");
        }
        this.m_currentLockState = LockMode.Free;
        foreach (IntentLock.LockHolder holder in this.m_holders)
          this.m_currentLockState = IntentLock.Max(this.m_currentLockState, holder.LockMode);
        this.PeekAndWakeUp();
      }
    }

    private void PeekAndWakeUp()
    {
      if (this.m_waiters == null || this.m_waiters.Count <= 0)
        return;
      this.m_waiters[0]?.Event.Set();
    }

    [DllImport("kernel32")]
    private static extern int GetCurrentThreadId();

    public void Dispose()
    {
    }

    private class LockHolder
    {
      public LockHolder(long request, LockMode lockMode)
      {
        this.Request = request;
        this.LockMode = lockMode;
        this.OwningThreadId = Environment.CurrentManagedThreadId;
        this.OwningNativeThreadId = IntentLock.GetCurrentThreadId();
        this.DateAcquired = DateTime.UtcNow;
      }

      public override int GetHashCode() => this.Request == 0L ? this.OwningThreadId.GetHashCode() : this.Request.GetHashCode();

      public override bool Equals(object obj)
      {
        if (!(obj is IntentLock.LockHolder lockHolder))
          return false;
        return this.Request != 0L ? this.Request == lockHolder.Request : this.OwningThreadId == lockHolder.OwningThreadId;
      }

      public long Request { get; private set; }

      public LockMode LockMode { get; private set; }

      public int OwningThreadId { get; private set; }

      public int OwningNativeThreadId { get; private set; }

      public DateTime DateAcquired { get; private set; }
    }

    private class LockWaiter : IntentLock.LockHolder, IDisposable
    {
      public LockWaiter(long request, LockMode lockMode)
        : base(request, lockMode)
      {
        this.Event = new AutoResetEvent(false);
      }

      public void Dispose()
      {
        if (this.Event == null)
          return;
        this.Event.Dispose();
        this.Event = (AutoResetEvent) null;
      }

      public AutoResetEvent Event { get; private set; }
    }
  }
}
