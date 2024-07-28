// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.ThreadPoolThrottle
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using System.Collections.Generic;
using System.Threading;

namespace Microsoft.TeamFoundation.Server.Core
{
  public class ThreadPoolThrottle
  {
    private int m_maxThreads;
    private int m_currentThreads;
    private ThreadPriority m_priority = ThreadPriority.Normal;
    private WaitCallback m_callback;
    private int m_waiting;
    private Queue<ThreadPoolThrottle.ThrottleCallbackState> m_queue = new Queue<ThreadPoolThrottle.ThrottleCallbackState>(32);

    public ThreadPoolThrottle(int maxThreads)
    {
      this.m_maxThreads = maxThreads;
      this.m_callback = new WaitCallback(this.ProcessWorkItem);
    }

    public ThreadPriority Priority
    {
      get => this.m_priority;
      set => this.m_priority = value;
    }

    public bool QueueUserWorkItemWithWait(WaitCallback callBack, object state)
    {
label_0:
      if (this.m_currentThreads < this.m_maxThreads)
      {
        if (Interlocked.Increment(ref this.m_currentThreads) <= this.m_maxThreads)
          return ThreadPool.UnsafeQueueUserWorkItem(this.m_callback, (object) new ThreadPoolThrottle.ThrottleCallbackState(callBack, state));
        Interlocked.Decrement(ref this.m_currentThreads);
      }
      lock (this.m_queue)
      {
        if (this.m_currentThreads >= this.m_maxThreads)
        {
          ++this.m_waiting;
          Monitor.Wait((object) this.m_queue);
          --this.m_waiting;
          return ThreadPool.UnsafeQueueUserWorkItem(this.m_callback, (object) new ThreadPoolThrottle.ThrottleCallbackState(callBack, state));
        }
        goto label_0;
      }
    }

    public bool QueueUserWorkItem(WaitCallback callBack, object state)
    {
label_0:
      if (this.m_currentThreads < this.m_maxThreads)
      {
        if (Interlocked.Increment(ref this.m_currentThreads) <= this.m_maxThreads)
          return ThreadPool.UnsafeQueueUserWorkItem(this.m_callback, (object) new ThreadPoolThrottle.ThrottleCallbackState(callBack, state));
        Interlocked.Decrement(ref this.m_currentThreads);
      }
      lock (this.m_queue)
      {
        if (this.m_currentThreads >= this.m_maxThreads)
        {
          try
          {
            this.m_queue.Enqueue(new ThreadPoolThrottle.ThrottleCallbackState(callBack, state));
            return true;
          }
          catch
          {
            return false;
          }
        }
        else
          goto label_0;
      }
    }

    public int UncompletedWorkItems => this.m_currentThreads + this.m_waiting + this.m_queue.Count;

    private void ProcessWorkItem(object state)
    {
      ThreadPoolThrottle.ThrottleCallbackState state1 = (ThreadPoolThrottle.ThrottleCallbackState) state;
label_1:
      try
      {
        if (this.m_priority != ThreadPriority.Normal)
          Thread.CurrentThread.Priority = this.m_priority;
        state1.m_callback(state1.m_state);
      }
      finally
      {
        if (Thread.CurrentThread.Priority != ThreadPriority.Normal)
          Thread.CurrentThread.Priority = ThreadPriority.Normal;
      }
      lock (this.m_queue)
      {
        if (this.m_waiting > 0)
          Monitor.Pulse((object) this.m_queue);
        else if (this.m_queue.Count > 0)
        {
          state1 = this.m_queue.Dequeue();
          if (!ThreadPool.UnsafeQueueUserWorkItem(this.m_callback, (object) state1))
            goto label_1;
        }
        else
          Interlocked.Decrement(ref this.m_currentThreads);
      }
    }

    private class ThrottleCallbackState
    {
      internal WaitCallback m_callback;
      internal object m_state;

      public ThrottleCallbackState(WaitCallback callback, object state)
      {
        this.m_callback = callback;
        this.m_state = state;
      }
    }
  }
}
