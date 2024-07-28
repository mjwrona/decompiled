// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.Core.Util.StorageCommandAsyncResult
// Assembly: Microsoft.Azure.Storage.Common, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 0978DA65-6954-4A99-9ACB-2EF3D979A5D5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.Common.dll

using System;
using System.Diagnostics;
using System.Threading;

namespace Microsoft.Azure.Storage.Core.Util
{
  internal class StorageCommandAsyncResult : 
    CancellableOperationBase,
    ICancellableAsyncResult,
    IAsyncResult,
    IDisposable
  {
    private AsyncCallback userCallback;
    private object userState;
    private bool isCompleted;
    private bool completedSynchronously = true;
    private ManualResetEvent asyncWaitEvent;

    [DebuggerNonUserCode]
    protected StorageCommandAsyncResult()
    {
    }

    [DebuggerNonUserCode]
    protected StorageCommandAsyncResult(AsyncCallback callback, object state)
    {
      this.userCallback = callback;
      this.userState = state;
    }

    [DebuggerNonUserCode]
    public object AsyncState => this.userState;

    [DebuggerNonUserCode]
    public WaitHandle AsyncWaitHandle => this.LazyCreateWaitHandle();

    [DebuggerNonUserCode]
    public bool CompletedSynchronously => this.completedSynchronously && this.isCompleted;

    [DebuggerNonUserCode]
    public bool IsCompleted => this.isCompleted;

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    protected virtual void Dispose(bool disposing)
    {
      if (!disposing || this.asyncWaitEvent == null)
        return;
      this.asyncWaitEvent.Close();
      this.asyncWaitEvent = (ManualResetEvent) null;
    }

    private WaitHandle LazyCreateWaitHandle()
    {
      if (this.asyncWaitEvent != null)
        return (WaitHandle) this.asyncWaitEvent;
      ManualResetEvent manualResetEvent = new ManualResetEvent(false);
      if (Interlocked.CompareExchange<ManualResetEvent>(ref this.asyncWaitEvent, manualResetEvent, (ManualResetEvent) null) != null)
        manualResetEvent.Close();
      if (this.isCompleted)
        this.asyncWaitEvent.Set();
      return (WaitHandle) this.asyncWaitEvent;
    }

    [DebuggerNonUserCode]
    internal void OnComplete()
    {
      if (this.isCompleted)
        return;
      this.isCompleted = true;
      Thread.MemoryBarrier();
      if (this.asyncWaitEvent != null)
        this.asyncWaitEvent.Set();
      if (this.userCallback == null)
        return;
      this.userCallback((IAsyncResult) this);
    }

    [DebuggerNonUserCode]
    internal virtual void End()
    {
      if (!this.isCompleted)
        this.AsyncWaitHandle.WaitOne();
      this.Dispose();
    }

    [DebuggerNonUserCode]
    internal void UpdateCompletedSynchronously(bool lastOperationCompletedSynchronously) => this.completedSynchronously &= lastOperationCompletedSynchronously;
  }
}
