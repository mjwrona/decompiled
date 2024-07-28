// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.ParallelReducerUtility`1
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public class ParallelReducerUtility<T>
  {
    private int intermediateCounter = -1;
    private readonly ConcurrentQueue<T> queue;

    public int ParallelTasks { get; }

    public ParallelReducerUtility(IEnumerable<T> items, int maxParallelism)
    {
      if (maxParallelism == 0)
        maxParallelism = 1;
      this.ParallelTasks = Math.Min(items.Count<T>() / 2, maxParallelism);
      this.queue = new ConcurrentQueue<T>(items);
    }

    public T Reduce(
      TestManagementRequestContext context,
      Func<T, T, int, T> reducer,
      CancellationToken cancellationToken)
    {
      if (this.queue.Count == 0)
        return default (T);
      CancellationTokenSource cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
      context.Logger.Info(1015607, string.Format("ParallelReducerUtility: reducing with degree {0}", (object) this.ParallelTasks));
      Task[] taskArray = new Task[this.ParallelTasks];
      for (int index = 0; index < this.ParallelTasks; ++index)
        taskArray[index] = Task.Run((Action) (() => this.ReduceTask(reducer, cancellationTokenSource)), cancellationToken);
      Task.WaitAll(taskArray);
      List<Exception> innerExceptions = new List<Exception>();
      foreach (Task task in taskArray)
      {
        if (task.Exception != null)
          innerExceptions.Add((Exception) task.Exception);
      }
      if (this.queue.Count > 1)
      {
        try
        {
          this.ReduceTask(reducer, cancellationTokenSource);
        }
        catch (Exception ex)
        {
          innerExceptions.Add(ex);
        }
      }
      if (innerExceptions.Count > 0)
        throw new AggregateException(CoverageResources.ParallelReducerException, (IEnumerable<Exception>) innerExceptions);
      T result;
      this.queue.TryDequeue(out result);
      return result;
    }

    private void ReduceTask(
      Func<T, T, int, T> reducer,
      CancellationTokenSource cancellationTokenSource)
    {
      try
      {
        if (cancellationTokenSource.IsCancellationRequested)
          throw new System.OperationCanceledException();
        T result1;
        if (!this.queue.TryDequeue(out result1))
          return;
        T result2;
        for (bool flag = this.queue.TryDequeue(out result2); flag && !cancellationTokenSource.IsCancellationRequested; flag = this.queue.TryDequeue(out result2))
        {
          int num = Interlocked.Increment(ref this.intermediateCounter);
          result1 = reducer(result1, result2, num);
        }
        this.queue.Enqueue(result1);
      }
      catch (Exception ex)
      {
        cancellationTokenSource.Cancel();
        throw;
      }
    }
  }
}
