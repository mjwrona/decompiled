// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Common.AsyncWaitHandle
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using System;
using System.Collections.Generic;
using System.Security;
using System.Threading;

namespace Microsoft.Azure.NotificationHubs.Common
{
  internal class AsyncWaitHandle
  {
    private static Action<object> timerCompleteCallback;
    private List<AsyncWaitHandle.AsyncWaiter> asyncWaiters;
    private volatile bool isSignaled;
    private EventResetMode resetMode;
    private object syncObject;

    public AsyncWaitHandle()
      : this(EventResetMode.AutoReset)
    {
    }

    public AsyncWaitHandle(EventResetMode resetMode)
    {
      this.resetMode = resetMode;
      this.syncObject = new object();
    }

    public bool WaitAsync(
      Action<object, TimeoutException> callback,
      object state,
      TimeSpan timeout)
    {
      if (!this.isSignaled || this.isSignaled && this.resetMode == EventResetMode.AutoReset)
      {
        lock (this.syncObject)
        {
          if (this.isSignaled && this.resetMode == EventResetMode.AutoReset)
            this.isSignaled = false;
          else if (!this.isSignaled)
          {
            AsyncWaitHandle.AsyncWaiter timerState = new AsyncWaitHandle.AsyncWaiter(this, callback, state);
            if (this.asyncWaiters == null)
              this.asyncWaiters = new List<AsyncWaitHandle.AsyncWaiter>();
            this.asyncWaiters.Add(timerState);
            if (timeout != TimeSpan.MaxValue)
            {
              if (AsyncWaitHandle.timerCompleteCallback == null)
                AsyncWaitHandle.timerCompleteCallback = new Action<object>(AsyncWaitHandle.OnTimerComplete);
              timerState.SetTimer(AsyncWaitHandle.timerCompleteCallback, (object) timerState, timeout);
            }
            return false;
          }
        }
      }
      return true;
    }

    private static void OnTimerComplete(object state)
    {
      AsyncWaitHandle.AsyncWaiter asyncWaiter = (AsyncWaitHandle.AsyncWaiter) state;
      AsyncWaitHandle parent = asyncWaiter.Parent;
      bool flag = false;
      lock (parent.syncObject)
      {
        if (parent.asyncWaiters != null)
        {
          if (parent.asyncWaiters.Remove(asyncWaiter))
          {
            asyncWaiter.TimedOut = true;
            flag = true;
          }
        }
      }
      asyncWaiter.CancelTimer();
      if (!flag)
        return;
      asyncWaiter.Call();
    }

    public void Set()
    {
      List<AsyncWaitHandle.AsyncWaiter> asyncWaiterList = (List<AsyncWaitHandle.AsyncWaiter>) null;
      AsyncWaitHandle.AsyncWaiter asyncWaiter1 = (AsyncWaitHandle.AsyncWaiter) null;
      if (!this.isSignaled)
      {
        lock (this.syncObject)
        {
          if (!this.isSignaled)
          {
            if (this.resetMode == EventResetMode.ManualReset)
            {
              this.isSignaled = true;
              Monitor.PulseAll(this.syncObject);
              asyncWaiterList = this.asyncWaiters;
              this.asyncWaiters = (List<AsyncWaitHandle.AsyncWaiter>) null;
            }
            else if (this.asyncWaiters != null && this.asyncWaiters.Count > 0)
            {
              asyncWaiter1 = this.asyncWaiters[0];
              this.asyncWaiters.RemoveAt(0);
            }
            else
              this.isSignaled = true;
          }
        }
      }
      if (asyncWaiterList != null)
      {
        foreach (AsyncWaitHandle.AsyncWaiter asyncWaiter2 in asyncWaiterList)
        {
          asyncWaiter2.CancelTimer();
          asyncWaiter2.Call();
        }
      }
      if (asyncWaiter1 == null)
        return;
      asyncWaiter1.CancelTimer();
      asyncWaiter1.Call();
    }

    public void Reset() => this.isSignaled = false;

    private class AsyncWaiter : ActionItem
    {
      [SecurityCritical]
      private Action<object, TimeoutException> callback;
      [SecurityCritical]
      private object state;
      private IOThreadTimer timer;
      private TimeSpan originalTimeout;

      public AsyncWaiter(
        AsyncWaitHandle parent,
        Action<object, TimeoutException> callback,
        object state)
      {
        this.Parent = parent;
        this.callback = callback;
        this.state = state;
      }

      public AsyncWaitHandle Parent { get; private set; }

      public bool TimedOut { get; set; }

      public void Call() => this.Schedule();

      [SecurityCritical]
      protected override void Invoke() => this.callback(this.state, this.TimedOut ? new TimeoutException(SRCore.TimeoutOnOperation((object) this.originalTimeout)) : (TimeoutException) null);

      public void SetTimer(Action<object> timerCallback, object timerState, TimeSpan timeout)
      {
        if (this.timer != null)
          throw Fx.Exception.AsError((Exception) new InvalidOperationException(SRCore.MustCancelOldTimer));
        this.originalTimeout = timeout;
        this.timer = new IOThreadTimer(timerCallback, timerState, false);
        this.timer.Set(timeout);
      }

      public void CancelTimer()
      {
        if (this.timer == null)
          return;
        this.timer.Cancel();
        this.timer = (IOThreadTimer) null;
      }
    }
  }
}
