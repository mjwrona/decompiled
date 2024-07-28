// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.Threading.IVssRequestContextThreadExtensions
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Framework.Server.Threading
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class IVssRequestContextThreadExtensions
  {
    public static void RunSynchronously(this IVssRequestContext requestContext, Func<Task> function)
    {
      VssSynchronousScheduler scheduler = new VssSynchronousScheduler();
      VssRequestSynchronizationContext syncContext = new VssRequestSynchronizationContext(IVssRequestContextThreadExtensions.GetRequestId(requestContext), requestContext.ActivityId, (IVssScheduler) scheduler);
      SynchronizationContext current = SynchronizationContext.Current;
      try
      {
        SynchronizationContext.SetSynchronizationContext((SynchronizationContext) syncContext);
        scheduler.RunSynchronously(function);
      }
      finally
      {
        SynchronizationContext.SetSynchronizationContext(current);
      }
    }

    public static T RunSynchronously<T>(
      this IVssRequestContext requestContext,
      Func<Task<T>> function)
    {
      VssSynchronousScheduler scheduler = new VssSynchronousScheduler();
      VssRequestSynchronizationContext syncContext = new VssRequestSynchronizationContext(IVssRequestContextThreadExtensions.GetRequestId(requestContext), requestContext.ActivityId, (IVssScheduler) scheduler);
      SynchronizationContext current = SynchronizationContext.Current;
      try
      {
        SynchronizationContext.SetSynchronizationContext((SynchronizationContext) syncContext);
        return scheduler.RunSynchronously<T>(function);
      }
      finally
      {
        SynchronizationContext.SetSynchronizationContext(current);
      }
    }

    public static Task Fork(
      this IVssRequestContext requestContext,
      Func<IVssRequestContext, Task> function,
      [CallerMemberName] string functionName = null)
    {
      return requestContext.Fork<IVssTaskService>(function, functionName);
    }

    public static Task Fork<TThreadPool>(
      this IVssRequestContext requestContext,
      Func<IVssRequestContext, Task> function,
      [CallerMemberName] string functionName = null)
      where TThreadPool : class, IVssTaskService
    {
      return requestContext.To(TeamFoundationHostType.Deployment).GetService<TThreadPool>().RunAsync(requestContext, functionName, function);
    }

    public static void Schedule(
      this IVssRequestContext requestContext,
      Func<IVssRequestContext, Task> function,
      TimeSpan delay,
      [CallerMemberName] string functionName = null)
    {
      requestContext.Schedule<IVssTaskService>(function, delay, functionName);
    }

    public static void Schedule<TThreadPool>(
      this IVssRequestContext requestContext,
      Func<IVssRequestContext, Task> function,
      TimeSpan delay,
      [CallerMemberName] string functionName = null)
      where TThreadPool : class, IVssTaskService
    {
      requestContext.To(TeamFoundationHostType.Deployment).GetService<TThreadPool>().Schedule(requestContext, functionName, function, delay);
    }

    public static void Join(this IVssRequestContext requestContext, Task task, TimeSpan timeout = default (TimeSpan))
    {
      int millisecondsTimeout = timeout > TimeSpan.Zero ? (int) timeout.TotalMilliseconds : -1;
      if (!task.Wait(millisecondsTimeout, requestContext.CancellationToken))
        throw new TimeoutException("Timed out waiting for completion of task");
    }

    public static void Join(
      this IVssRequestContext requestContext,
      IEnumerable<Task> tasks,
      TimeSpan timeout = default (TimeSpan))
    {
      int millisecondsTimeout = timeout > TimeSpan.Zero ? (int) timeout.TotalMilliseconds : -1;
      if (!Task.WaitAll(tasks.ToArray<Task>(), millisecondsTimeout, requestContext.CancellationToken))
        throw new TimeoutException("Timed out waiting for completion of tasks");
    }

    private static long GetRequestId(IVssRequestContext requestContext) => !(requestContext is VssRequestContext vssRequestContext) ? 0L : vssRequestContext.RequestId;
  }
}
