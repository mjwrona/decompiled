// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.QueuedActionLimiter
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Diagnostics;
using System.Threading;

namespace Microsoft.TeamFoundation.Git.Server
{
  internal class QueuedActionLimiter
  {
    private bool m_firstActionQueued;
    private bool m_secondActionQueued;
    private object m_lock;
    private Stopwatch m_stopwatch;
    private int m_suppressionCount;
    private object m_timerLock;
    private Timer m_timer;
    private readonly WaitCallback m_action;
    private readonly bool m_allowRecursiveQueueing;
    private readonly long m_rateLimitInMilliseconds;
    private int m_deliveringThreadId;
    private static int s_workQueuedCount;

    public QueuedActionLimiter(
      WaitCallback action,
      bool allowRecursiveQueueing,
      long rateLimitInMilliseconds)
    {
      ArgumentUtility.CheckForNull<WaitCallback>(action, nameof (action));
      if (rateLimitInMilliseconds < 0L)
        throw new ArgumentOutOfRangeException(nameof (rateLimitInMilliseconds));
      this.m_action = action;
      this.m_allowRecursiveQueueing = allowRecursiveQueueing;
      this.m_rateLimitInMilliseconds = rateLimitInMilliseconds;
      this.m_lock = new object();
      this.m_timerLock = new object();
    }

    public bool IsWorkQueued
    {
      get
      {
        lock (this.m_lock)
          return this.m_firstActionQueued || this.m_secondActionQueued;
      }
    }

    public static bool IsAnyWorkQueued => QueuedActionLimiter.s_workQueuedCount > 0;

    public void QueueAction()
    {
      if (Environment.CurrentManagedThreadId == this.m_deliveringThreadId && !this.m_allowRecursiveQueueing)
        return;
      bool flag = false;
      long delay = 0;
      lock (this.m_lock)
      {
        if (!this.m_firstActionQueued)
        {
          flag = true;
          this.m_firstActionQueued = true;
          delay = Math.Max(this.m_rateLimitInMilliseconds - (this.m_stopwatch != null ? this.m_stopwatch.ElapsedMilliseconds : this.m_rateLimitInMilliseconds), 0L);
          Interlocked.Increment(ref QueuedActionLimiter.s_workQueuedCount);
        }
        else if (!this.m_secondActionQueued)
          this.m_secondActionQueued = true;
      }
      if (!flag)
        return;
      this.QueueWithDelay(delay);
    }

    private void DeliverAction(object state)
    {
      bool flag = false;
      this.m_deliveringThreadId = Environment.CurrentManagedThreadId;
      lock (this.m_lock)
      {
        while (this.m_suppressionCount > 0)
          Monitor.Wait(this.m_lock);
        this.m_secondActionQueued = false;
      }
      try
      {
        this.m_action((object) null);
      }
      catch (Exception ex)
      {
        TeamFoundationTrace.TraceException(ex);
      }
      finally
      {
        this.m_deliveringThreadId = 0;
      }
      lock (this.m_lock)
      {
        this.m_firstActionQueued = false;
        this.m_stopwatch = Stopwatch.StartNew();
        if (this.m_secondActionQueued)
        {
          flag = true;
          this.m_firstActionQueued = true;
          this.m_secondActionQueued = false;
        }
        else
          Interlocked.Decrement(ref QueuedActionLimiter.s_workQueuedCount);
      }
      if (!flag)
        return;
      this.QueueWithDelay(this.m_rateLimitInMilliseconds);
    }

    private void QueueWithDelay(long delay)
    {
      if (delay > 0L)
      {
        lock (this.m_timerLock)
          this.m_timer = new Timer(new System.Threading.TimerCallback(this.TimerCallback), (object) null, delay, -1L);
      }
      else
        ThreadPool.QueueUserWorkItem(new WaitCallback(this.DeliverAction));
    }

    private void TimerCallback(object state)
    {
      try
      {
        lock (this.m_timerLock)
        {
          if (this.m_timer != null)
          {
            this.m_timer.Dispose();
            this.m_timer = (Timer) null;
          }
        }
        ThreadPool.QueueUserWorkItem(new WaitCallback(this.DeliverAction), (object) null);
      }
      catch (Exception ex)
      {
        TeamFoundationTrace.TraceException(ex);
      }
    }

    public void Suppress() => Interlocked.Increment(ref this.m_suppressionCount);

    public void Unsuppress()
    {
      if (Interlocked.Decrement(ref this.m_suppressionCount) != 0)
        return;
      lock (this.m_lock)
        Monitor.PulseAll(this.m_lock);
    }
  }
}
