// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Common.SafeTaskCompletionSource`1
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Common
{
  public class SafeTaskCompletionSource<T>
  {
    private readonly TaskCompletionSource<T> inner;

    public SafeTaskCompletionSource()
      : this((object) null)
    {
    }

    public SafeTaskCompletionSource(object state)
      : this(state, TaskCreationOptions.None)
    {
    }

    public SafeTaskCompletionSource(TaskCreationOptions options)
      : this((object) null, options)
    {
    }

    public SafeTaskCompletionSource(object state, TaskCreationOptions options) => this.inner = new TaskCompletionSource<T>(state, options);

    public System.Threading.Tasks.Task<T> Task => this.inner.Task;

    public void SetCanceled()
    {
      this.inner.SetCanceled();
      GC.SuppressFinalize((object) this);
    }

    public void SetException(Exception exception)
    {
      this.inner.SetException(exception);
      GC.SuppressFinalize((object) this);
    }

    public void SetException(IEnumerable<Exception> exceptions)
    {
      this.inner.SetException(exceptions);
      GC.SuppressFinalize((object) this);
    }

    public void SetResult(T result)
    {
      this.inner.SetResult(result);
      GC.SuppressFinalize((object) this);
    }

    public bool TrySetCanceled()
    {
      int num = this.inner.TrySetCanceled() ? 1 : 0;
      if (num == 0)
        return num != 0;
      GC.SuppressFinalize((object) this);
      return num != 0;
    }

    public bool TrySetException(Exception exception)
    {
      int num = this.inner.TrySetException(exception) ? 1 : 0;
      if (num == 0)
        return num != 0;
      GC.SuppressFinalize((object) this);
      return num != 0;
    }

    public bool TrySetException(IEnumerable<Exception> exceptions)
    {
      int num = this.inner.TrySetException(exceptions) ? 1 : 0;
      if (num == 0)
        return num != 0;
      GC.SuppressFinalize((object) this);
      return num != 0;
    }

    public bool TrySetResult(T result)
    {
      int num = this.inner.TrySetResult(result) ? 1 : 0;
      if (num == 0)
        return num != 0;
      GC.SuppressFinalize((object) this);
      return num != 0;
    }

    public void MarkTaskAsUnused() => GC.SuppressFinalize((object) this);

    ~SafeTaskCompletionSource() => this.TrySetException((Exception) new ObjectDisposedException("TaskCompeletionSource was GC'd without completing its task."));
  }
}
