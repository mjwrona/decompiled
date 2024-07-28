// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.Channels.TaskWrapperAsyncResult`1
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using System;
using System.Threading;

namespace Microsoft.TeamFoundation.Client.Channels
{
  internal sealed class TaskWrapperAsyncResult<T> : IAsyncResult
  {
    private bool _completedSynchronously;

    internal TaskWrapperAsyncResult(System.Threading.Tasks.Task<T> task, object asyncState)
    {
      this.Task = task;
      this.AsyncState = asyncState;
    }

    public object AsyncState { get; private set; }

    public WaitHandle AsyncWaitHandle => ((IAsyncResult) this.Task).AsyncWaitHandle;

    public bool CompletedSynchronously
    {
      get => this._completedSynchronously || ((IAsyncResult) this.Task).CompletedSynchronously;
      internal set => this._completedSynchronously = true;
    }

    public bool IsCompleted => this.Task.IsCompleted;

    internal System.Threading.Tasks.Task<T> Task { get; private set; }
  }
}
