// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Tasks.TaskWrapperAsyncResult
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Tasks
{
  internal sealed class TaskWrapperAsyncResult : IAsyncResult
  {
    internal TaskWrapperAsyncResult(Task task, object asyncState, bool completedSynchronously)
    {
      this.Task = task;
      this.AsyncState = asyncState;
      this.CompletedSynchronously = completedSynchronously;
    }

    public object AsyncState { get; }

    public WaitHandle AsyncWaitHandle => ((IAsyncResult) this.Task).AsyncWaitHandle;

    public bool CompletedSynchronously { get; }

    public bool IsCompleted => this.Task.IsCompleted;

    internal Task Task { get; }
  }
}
