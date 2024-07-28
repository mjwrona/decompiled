// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Common.AsyncOperation
// Assembly: Microsoft.TeamFoundation.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E643B42-FE11-4FC2-A9D6-79417E26CF92
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Common.dll

using Microsoft.TeamFoundation.Common.Internal;
using System;
using System.ComponentModel;
using System.Threading;

namespace Microsoft.TeamFoundation.Framework.Common
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public abstract class AsyncOperation : IAsyncResult
  {
    private ManualResetEvent m_waitHandle;
    private object m_lockObject = new object();

    protected AsyncOperation(AsyncCallback callback, object state)
    {
      this.Callback = callback;
      this.AsyncState = state;
    }

    public object AsyncState { get; private set; }

    public ManualResetEvent AsyncWaitHandle
    {
      get
      {
        if (this.m_waitHandle != null)
          return this.m_waitHandle;
        lock (this.m_lockObject)
        {
          if (this.m_waitHandle == null)
            this.m_waitHandle = new ManualResetEvent(this.IsCompleted);
        }
        return this.m_waitHandle;
      }
    }

    WaitHandle IAsyncResult.AsyncWaitHandle => (WaitHandle) this.AsyncWaitHandle;

    public AsyncCallback Callback { get; private set; }

    public bool CompletedSynchronously { get; private set; }

    public Exception Exception { get; private set; }

    public bool IsCompleted { get; private set; }

    protected virtual void Dispose()
    {
      if (this.m_waitHandle == null)
        return;
      this.m_waitHandle.Close();
      this.m_waitHandle = (ManualResetEvent) null;
    }

    protected void Complete(bool completedSynchronously, Exception exception = null)
    {
      this.CompletedSynchronously = completedSynchronously;
      this.Exception = exception;
      lock (this.m_lockObject)
      {
        this.IsCompleted = true;
        if (this.m_waitHandle != null)
          this.m_waitHandle.Set();
      }
      if (this.Callback == null)
        return;
      this.Callback((IAsyncResult) this);
    }

    protected static T End<T>(IAsyncResult result) where T : AsyncOperation
    {
      T operation;
      Exception error;
      if (!AsyncOperation.TryEnd<T>(result, out operation, out error))
        throw error;
      return operation;
    }

    protected static bool TryEnd<T>(IAsyncResult result, out T operation, out Exception error) where T : AsyncOperation
    {
      operation = result as T;
      if ((object) operation == null)
        throw new InvalidOperationException(TFCommonResources.InvalidAsynchronousOperationParameter((object) nameof (result)));
      if (!operation.IsCompleted)
        operation.AsyncWaitHandle.WaitOne();
      operation.IsCompleted = true;
      operation.Dispose();
      error = operation.Exception;
      return error == null;
    }
  }
}
