// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WaitHandleLock`2
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

using System;
using System.Threading;

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  internal class WaitHandleLock<T, TResult>
  {
    private object m_lock = new object();
    private EventWaitHandle m_waitHandle;
    private TResult m_result;
    private const string c_defaultTimeOutExceptionMessage = "WaitHandleLock timed out waiting for action to complete.";

    public WaitHandleLock(
      Func<T, TResult> action,
      TimeSpan singleWaitTime,
      TimeSpan overallWaitTime)
    {
      this.Action = action;
      this.SingleWaitTime = singleWaitTime;
      this.OverallWaitTime = overallWaitTime;
    }

    public WaitHandleLock(TimeSpan singleWaitTime, TimeSpan overallWaitTime)
    {
      this.SingleWaitTime = singleWaitTime;
      this.OverallWaitTime = overallWaitTime;
    }

    protected Func<T, TResult> Action { get; set; }

    public TimeSpan SingleWaitTime { get; private set; }

    public TimeSpan OverallWaitTime { get; private set; }

    public TResult PerformAction(T argument) => this.PerformAction(argument, DateTime.Now.Add(this.OverallWaitTime));

    private TResult PerformAction(T argument, DateTime expirationTime)
    {
      if ((object) this.m_result != null)
        return this.m_result;
      EventWaitHandle eventWaitHandle = (EventWaitHandle) null;
      Func<T, TResult> func = (Func<T, TResult>) null;
      lock (this.m_lock)
      {
        if ((object) this.m_result != null)
          return this.m_result;
        if (this.Action == null)
          throw new InvalidOperationException("The Action must be set before calling this method.");
        if (this.m_waitHandle != null)
        {
          eventWaitHandle = this.m_waitHandle;
        }
        else
        {
          eventWaitHandle = (EventWaitHandle) new ManualResetEvent(false);
          func = this.Action;
          this.m_waitHandle = eventWaitHandle;
        }
      }
      if (func != null)
      {
        try
        {
          TResult result = func(argument);
          lock (this.m_lock)
          {
            this.m_result = result;
            this.Action = (Func<T, TResult>) null;
            this.m_waitHandle = (EventWaitHandle) null;
          }
        }
        catch (Exception ex)
        {
          lock (this.m_lock)
            this.m_waitHandle = (EventWaitHandle) null;
          throw;
        }
        finally
        {
          eventWaitHandle.Set();
        }
      }
      else
      {
        eventWaitHandle.WaitOne(this.SingleWaitTime);
        if ((object) this.m_result == null)
        {
          if (DateTime.Now > expirationTime)
            throw new TimeoutException(this.TimeOutExceptionMessage ?? "WaitHandleLock timed out waiting for action to complete.");
          lock (this.m_lock)
          {
            if (this.m_waitHandle == eventWaitHandle)
              this.m_waitHandle = (EventWaitHandle) null;
          }
          return this.PerformAction(argument, expirationTime);
        }
      }
      return this.m_result;
    }

    public string TimeOutExceptionMessage { get; set; }
  }
}
