// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Common.BaseAsyncResult
// Assembly: Microsoft.TeamFoundation.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E643B42-FE11-4FC2-A9D6-79417E26CF92
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Common.dll

using System;
using System.Diagnostics;
using System.Threading;

namespace Microsoft.TeamFoundation.Common
{
  public class BaseAsyncResult : IAsyncResult
  {
    private AsyncCallback m_userCallback;
    private object m_stateObject;
    private ManualResetEvent m_waitHandle;
    private bool m_isCompleted;
    private Exception m_exception;

    public BaseAsyncResult(AsyncCallback userCallback, object stateObject)
    {
      this.m_userCallback = userCallback;
      this.m_stateObject = stateObject;
    }

    public object AsyncState => this.m_stateObject;

    public WaitHandle AsyncWaitHandle
    {
      get
      {
        if (this.m_waitHandle == null)
        {
          lock (this)
          {
            if (this.m_waitHandle == null)
              this.m_waitHandle = new ManualResetEvent(this.m_isCompleted);
          }
        }
        return (WaitHandle) this.m_waitHandle;
      }
    }

    public bool CompletedSynchronously => false;

    public bool IsCompleted => this.m_isCompleted;

    protected internal void Completed(Exception ex)
    {
      this.m_exception = ex;
      try
      {
        lock (this)
        {
          this.m_isCompleted = true;
          if (this.m_waitHandle != null)
            this.m_waitHandle.Set();
        }
        if (this.m_userCallback == null)
          return;
        this.m_userCallback((IAsyncResult) this);
      }
      catch (Exception ex1)
      {
        Trace.WriteLine("Recorded exception in BaseAsyncResult.Completed(): " + ex1.ToString());
      }
    }

    public Exception Exception => this.m_exception;
  }
}
