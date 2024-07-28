// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Common.TaskCoordinator`1
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 203E0171-FB50-4FDE-9B1F-EFC6366423BC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Common.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Content.Server.Common
{
  public class TaskCoordinator<TKey> where TKey : IEquatable<TKey>
  {
    private readonly ConcurrentDictionary<TKey, SafeTaskCompletionSource<int>> m_activeRequests;

    public TaskCoordinator() => this.m_activeRequests = new ConcurrentDictionary<TKey, SafeTaskCompletionSource<int>>();

    public bool LastTaskWasStartedThreadUnsafe { get; private set; }

    public async Task<bool> TryRun(TKey key, Func<Task> taskDelegate)
    {
      SafeTaskCompletionSource<int> tcs = new SafeTaskCompletionSource<int>(TaskCreationOptions.RunContinuationsAsynchronously);
      Task runningTask;
      bool taskStarted = this.TryRunInternal(key, taskDelegate, tcs, out runningTask);
      this.LastTaskWasStartedThreadUnsafe = taskStarted;
      if (taskStarted)
        await runningTask.ConfigureAwait(false);
      else
        tcs.MarkTaskAsUnused();
      return taskStarted;
    }

    public async Task RunOrAwait(TKey key, Func<Task> taskDelegate)
    {
      SafeTaskCompletionSource<int> tcs = new SafeTaskCompletionSource<int>(TaskCreationOptions.RunContinuationsAsynchronously);
      Task runningTask;
      ConfiguredTaskAwaitable configuredTaskAwaitable;
      while (!this.TryRunInternal(key, taskDelegate, tcs, out runningTask))
      {
        this.LastTaskWasStartedThreadUnsafe = false;
        configuredTaskAwaitable = runningTask.ConfigureAwait(false);
        await configuredTaskAwaitable;
      }
      this.LastTaskWasStartedThreadUnsafe = true;
      configuredTaskAwaitable = runningTask.ConfigureAwait(false);
      await configuredTaskAwaitable;
      tcs = (SafeTaskCompletionSource<int>) null;
    }

    private bool TryRunInternal(
      TKey key,
      Func<Task> taskDelegate,
      SafeTaskCompletionSource<int> tcs,
      out Task runningTask)
    {
      SafeTaskCompletionSource<int> orAdd = this.m_activeRequests.GetOrAdd(key, tcs);
      int num = tcs == orAdd ? 1 : 0;
      if (num != 0)
      {
        Action action;
        runningTask = Task.Run((Func<Task>) (async () =>
        {
          try
          {
            await taskDelegate().ConfigureAwait(false);
          }
          finally
          {
            this.m_activeRequests.TryRemove(key, out SafeTaskCompletionSource<int> _);
            Task.Run(action ?? (action = (Action) (() => tcs.SetResult(0))));
          }
        }));
        return num != 0;
      }
      runningTask = (Task) orAdd.Task;
      return num != 0;
    }
  }
}
