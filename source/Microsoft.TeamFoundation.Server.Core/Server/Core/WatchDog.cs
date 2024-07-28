// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.WatchDog
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using System;
using System.Collections.Generic;
using System.Threading;

namespace Microsoft.TeamFoundation.Server.Core
{
  internal class WatchDog : IDisposable
  {
    private Dictionary<int, WatchDog.Item> m_requests = new Dictionary<int, WatchDog.Item>(50);
    private bool m_isShutdown;
    private AutoResetEvent m_workReady = new AutoResetEvent(false);
    private Thread m_workerThread;
    private int m_currentCookie;

    public WatchDog()
    {
      this.m_workerThread = new Thread(new ThreadStart(this.WatchDogThread));
      this.m_workerThread.IsBackground = true;
      this.m_workerThread.Start();
    }

    public IDisposable AddTimer(TimeSpan timeSpan, WatchDog.TimeExpired callback) => this.AddTimer(timeSpan, callback, (object) null);

    public IDisposable AddTimer(TimeSpan timeSpan, WatchDog.TimeExpired callback, object state)
    {
      lock (this.m_requests)
      {
        this.ThrowIfDisposed();
        WatchDog.Item obj = new WatchDog.Item(this, this.m_currentCookie++, callback, DateTime.UtcNow.Add(timeSpan), state);
        this.m_requests.Add(obj.Cookie, obj);
        this.m_workReady.Set();
        return (IDisposable) obj;
      }
    }

    public void RemoveTimer(int cookie)
    {
      lock (this.m_requests)
      {
        if (this.m_workReady == null)
          return;
        this.m_requests.Remove(cookie);
        if (this.m_requests.Count != 0)
          return;
        this.m_workReady.Reset();
      }
    }

    public void Dispose()
    {
      if (this.m_isShutdown)
        return;
      this.m_isShutdown = true;
      if (this.m_workReady != null)
        this.m_workReady.Set();
      if (this.m_workerThread != null)
      {
        if (this.m_workerThread.ThreadState != ThreadState.Unstarted)
          this.m_workerThread.Join();
        this.m_workerThread = (Thread) null;
      }
      lock (this.m_requests)
      {
        if (this.m_workReady == null)
          return;
        this.m_workReady.Close();
        this.m_workReady = (AutoResetEvent) null;
      }
    }

    public void WatchDogThread()
    {
      Thread.CurrentThread.Name = "WatchDog Thread";
      TimeSpan t2_1 = TimeSpan.FromMilliseconds((double) int.MaxValue);
      TimeSpan timeout = t2_1;
label_1:
      this.m_workReady.WaitOne(timeout, false);
      if (this.m_isShutdown)
        return;
      lock (this.m_requests)
      {
        DateTime utcNow = DateTime.UtcNow;
        DateTime t2_2 = DateTime.MaxValue;
        List<int> intList = new List<int>();
        foreach (KeyValuePair<int, WatchDog.Item> request in this.m_requests)
        {
          if (request.Value.CheckForExpiry(utcNow))
            intList.Add(request.Key);
          else
            t2_2 = this.Min(request.Value.ExpiryTime, t2_2);
        }
        timeout = this.Min(t2_2 - utcNow, t2_1);
        using (List<int>.Enumerator enumerator = intList.GetEnumerator())
        {
          while (enumerator.MoveNext())
            this.m_requests.Remove(enumerator.Current);
          goto label_1;
        }
      }
    }

    private TimeSpan Min(TimeSpan t1, TimeSpan t2)
    {
      if (t1.Ticks < 0L || t2.Ticks < 0L)
        return new TimeSpan(0L);
      return t1 > t2 ? t2 : t1;
    }

    private DateTime Min(DateTime t1, DateTime t2) => t1 > t2 ? t2 : t1;

    private void ThrowIfDisposed()
    {
      if (this.m_workerThread == null)
        throw new ObjectDisposedException("Watchdog", "This object is disposed");
    }

    public delegate void TimeExpired(object state);

    private class Item : IDisposable
    {
      public Item(
        WatchDog watchDog,
        int cookie,
        WatchDog.TimeExpired callback,
        DateTime expiryTime,
        object state)
      {
        this.WatchDog = watchDog;
        this.Cookie = cookie;
        this.Callback = callback;
        this.ExpiryTime = expiryTime;
        this.State = state;
      }

      public void Dispose()
      {
        if (this.WatchDog == null)
          return;
        this.WatchDog.RemoveTimer(this.Cookie);
        this.WatchDog = (WatchDog) null;
      }

      public bool CheckForExpiry(DateTime utcNow)
      {
        if (!(this.ExpiryTime < utcNow))
          return false;
        this.Callback(this.State);
        return true;
      }

      public WatchDog.TimeExpired Callback { get; set; }

      public DateTime ExpiryTime { get; set; }

      public object State { get; set; }

      public WatchDog WatchDog { get; set; }

      public int Cookie { get; set; }
    }
  }
}
