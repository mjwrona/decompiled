// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.SignalR.Infrastructure.DispatchingTaskCompletionSource`1
// Assembly: Microsoft.AspNet.SignalR.Core, Version=2.4.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 095D5FBC-6474-494D-BE26-4EBE2B9AD3D0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.AspNet.SignalR.Core.dll

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.AspNet.SignalR.Infrastructure
{
  internal class DispatchingTaskCompletionSource<TResult>
  {
    private readonly TaskCompletionSource<TResult> _tcs = new TaskCompletionSource<TResult>();

    public System.Threading.Tasks.Task<TResult> Task => this._tcs.Task;

    public void SetCanceled() => TaskAsyncHelper.Dispatch((Action) (() => this._tcs.SetCanceled()));

    public void SetException(Exception exception) => TaskAsyncHelper.Dispatch((Action) (() => this._tcs.SetException(exception)));

    public void SetException(IEnumerable<Exception> exceptions) => TaskAsyncHelper.Dispatch((Action) (() => this._tcs.SetException(exceptions)));

    public void SetResult(TResult result) => TaskAsyncHelper.Dispatch((Action) (() => this._tcs.SetResult(result)));

    public void TrySetCanceled() => TaskAsyncHelper.Dispatch((Action) (() => this._tcs.TrySetCanceled()));

    public void TrySetException(Exception exception) => TaskAsyncHelper.Dispatch((Action) (() => this._tcs.TrySetException(exception)));

    public void TrySetException(IEnumerable<Exception> exceptions) => TaskAsyncHelper.Dispatch((Action) (() => this._tcs.TrySetException(exceptions)));

    public void SetUnwrappedException(Exception e)
    {
      if (e is AggregateException aggregateException)
        this.SetException((IEnumerable<Exception>) aggregateException.InnerExceptions);
      else
        this.SetException(e);
    }

    public void TrySetUnwrappedException(Exception e)
    {
      if (e is AggregateException aggregateException)
        this.TrySetException((IEnumerable<Exception>) aggregateException.InnerExceptions);
      else
        this.TrySetException(e);
    }

    public void TrySetResult(TResult result) => TaskAsyncHelper.Dispatch((Action) (() => this._tcs.TrySetResult(result)));
  }
}
