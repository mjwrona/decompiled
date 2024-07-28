// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Common.VssRequestPump
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 203E0171-FB50-4FDE-9B1F-EFC6366423BC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Content.Common;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Content.Server.Common
{
  [DebuggerStepThrough]
  public static class VssRequestPump
  {
    public static async Task<T> PumpFromAsync<T>(
      this IVssRequestContext requestContext,
      Func<VssRequestPump.Processor, Task<T>> workAsync)
    {
      CancellationToken cancellationToken = requestContext.CancellationToken;
      Guid e2Eid = requestContext.E2EId;
      Guid uniqueIdentifier = requestContext.UniqueIdentifier;
      T obj1;
      using (AsyncQueue<VssRequestPump.Callback> queue = new AsyncQueue<VssRequestPump.Callback>())
      {
        VssRequestPump.Processor processor = new VssRequestPump.Processor(requestContext.ActivityId, e2Eid, uniqueIdentifier, (VssRequestPump.ProcessorFunc) (vssCallback => queue.Enqueue(vssCallback)), cancellationToken);
        Task<T> workerTask = Task.Run<T>((Func<Task<T>>) (async () =>
        {
          T obj2;
          try
          {
            obj2 = await workAsync(processor).ConfigureAwait(false);
          }
          finally
          {
            queue.CompleteAdding();
          }
          return obj2;
        }), cancellationToken);
        IConcurrentIterator<VssRequestPump.Callback> reader = queue.GetAsyncReader();
        try
        {
          while (true)
          {
            Task task;
            do
            {
              if (await reader.MoveNextAsync(cancellationToken).ConfigureAwait(true))
                task = reader.Current(requestContext);
              else
                goto label_10;
            }
            while (task == null);
            await task.ConfigureAwait(true);
          }
        }
        finally
        {
          reader?.Dispose();
        }
label_10:
        reader = (IConcurrentIterator<VssRequestPump.Callback>) null;
        obj1 = await workerTask.ConfigureAwait(true);
      }
      cancellationToken = new CancellationToken();
      return obj1;
    }

    public static T Pump<T>(
      this IVssRequestContext requestContext,
      Func<VssRequestPump.Processor, Task<T>> workAsync)
    {
      CancellationToken cancellationToken = requestContext.CancellationToken;
      Guid e2Eid = requestContext.E2EId;
      Guid uniqueIdentifier = requestContext.UniqueIdentifier;
      using (AsyncQueue<VssRequestPump.Callback> queue = new AsyncQueue<VssRequestPump.Callback>())
      {
        VssRequestPump.Processor processor = new VssRequestPump.Processor(requestContext.ActivityId, e2Eid, uniqueIdentifier, (VssRequestPump.ProcessorFunc) (vssCallback => queue.Enqueue(vssCallback)), cancellationToken);
        Task<T> task1 = Task.Run<T>((Func<Task<T>>) (async () =>
        {
          T obj;
          try
          {
            obj = await workAsync(processor).ConfigureAwait(false);
          }
          finally
          {
            queue.CompleteAdding();
          }
          return obj;
        }), cancellationToken);
        foreach (VssRequestPump.Callback callback in queue.GetReader())
        {
          Task task2 = callback(requestContext);
          if (task2 != null)
            task2.SyncResult();
        }
        return task1.SyncResult<T>();
      }
    }

    public static Task PumpFromAsync(
      this IVssRequestContext requestContext,
      Func<VssRequestPump.Processor, Task> workAsync)
    {
      return (Task) requestContext.PumpFromAsync<int>((Func<VssRequestPump.Processor, Task<int>>) (async processor =>
      {
        await workAsync(processor).ConfigureAwait(false);
        return 0;
      }));
    }

    public static void Pump(
      this IVssRequestContext requestContext,
      Func<VssRequestPump.Processor, Task> workAsync)
    {
      requestContext.Pump<int>((Func<VssRequestPump.Processor, Task<int>>) (async processor =>
      {
        await workAsync(processor).ConfigureAwait(false);
        return 0;
      }));
    }

    [DebuggerStepThrough]
    public static void PumpOrInline(
      this IVssRequestContext requestContext,
      Func<VssRequestPump.Processor, Task> workAsync,
      bool runCallbacksOnThisThread)
    {
      requestContext.PumpOrInline<int>((Func<VssRequestPump.Processor, Task<int>>) (async p =>
      {
        await workAsync(p).ConfigureAwait(false);
        return 0;
      }), runCallbacksOnThisThread);
    }

    [DebuggerStepThrough]
    public static T PumpOrInline<T>(
      this IVssRequestContext requestContext,
      Func<VssRequestPump.Processor, Task<T>> workAsync,
      bool runCallbacksOnThisThread)
    {
      return runCallbacksOnThisThread ? requestContext.Pump<T>((Func<VssRequestPump.Processor, Task<T>>) (vssProcessor => workAsync(vssProcessor))) : workAsync(VssRequestPump.Processor.CreateWithoutRequestContext(requestContext.CancellationToken, requestContext.ActivityId, requestContext.E2EId, requestContext.UniqueIdentifier)).SyncResult<T>();
    }

    [DebuggerStepThrough]
    public static Task PumpOrInlineFromAsync(
      this IVssRequestContext requestContext,
      Func<VssRequestPump.Processor, Task> workAsync,
      bool runCallbacksOnThisThread)
    {
      return (Task) requestContext.PumpOrInlineFromAsync<int>((Func<VssRequestPump.Processor, Task<int>>) (async p =>
      {
        await workAsync(p).ConfigureAwait(false);
        return 0;
      }), runCallbacksOnThisThread);
    }

    [DebuggerStepThrough]
    public static Task<T> PumpOrInlineFromAsync<T>(
      this IVssRequestContext requestContext,
      Func<VssRequestPump.Processor, Task<T>> workAsync,
      bool runCallbacksOnThisThread)
    {
      return runCallbacksOnThisThread ? requestContext.PumpFromAsync<T>((Func<VssRequestPump.Processor, Task<T>>) (vssProcessor => workAsync(vssProcessor))) : workAsync(VssRequestPump.Processor.CreateWithoutRequestContext(requestContext.CancellationToken, requestContext.ActivityId, requestContext.E2EId, requestContext.UniqueIdentifier));
    }

    public static async Task<T> PumpFromAsync<T>(
      this IVssRequestContext requestContext,
      ISecuredDomainRequest domainRequest,
      Func<VssRequestPump.SecuredDomainProcessor, Task<T>> workAsync)
    {
      CancellationToken cancellationToken = requestContext.CancellationToken;
      Guid e2Eid = requestContext.E2EId;
      Guid uniqueIdentifier = requestContext.UniqueIdentifier;
      T obj1;
      using (AsyncQueue<VssRequestPump.Callback> queue = new AsyncQueue<VssRequestPump.Callback>())
      {
        VssRequestPump.SecuredDomainProcessor processor = new VssRequestPump.SecuredDomainProcessor(requestContext.ActivityId, e2Eid, uniqueIdentifier, domainRequest, (VssRequestPump.ProcessorFunc) (vssCallback => queue.Enqueue(vssCallback)), cancellationToken);
        Task<T> workerTask = Task.Run<T>((Func<Task<T>>) (async () =>
        {
          T obj2;
          try
          {
            obj2 = await workAsync(processor).ConfigureAwait(false);
          }
          finally
          {
            queue.CompleteAdding();
          }
          return obj2;
        }), cancellationToken);
        IConcurrentIterator<VssRequestPump.Callback> reader = queue.GetAsyncReader();
        try
        {
          while (true)
          {
            Task task;
            do
            {
              if (await reader.MoveNextAsync(cancellationToken).ConfigureAwait(true))
                task = reader.Current(requestContext);
              else
                goto label_10;
            }
            while (task == null);
            await task.ConfigureAwait(true);
          }
        }
        finally
        {
          reader?.Dispose();
        }
label_10:
        reader = (IConcurrentIterator<VssRequestPump.Callback>) null;
        obj1 = await workerTask.ConfigureAwait(true);
      }
      cancellationToken = new CancellationToken();
      return obj1;
    }

    public static T Pump<T>(
      this IVssRequestContext requestContext,
      ISecuredDomainRequest domainRequest,
      Func<VssRequestPump.SecuredDomainProcessor, Task<T>> workAsync)
    {
      CancellationToken cancellationToken = requestContext.CancellationToken;
      Guid e2Eid = requestContext.E2EId;
      Guid uniqueIdentifier = requestContext.UniqueIdentifier;
      using (AsyncQueue<VssRequestPump.Callback> queue = new AsyncQueue<VssRequestPump.Callback>())
      {
        VssRequestPump.SecuredDomainProcessor processor = new VssRequestPump.SecuredDomainProcessor(requestContext.ActivityId, e2Eid, uniqueIdentifier, domainRequest, (VssRequestPump.ProcessorFunc) (vssCallback => queue.Enqueue(vssCallback)), cancellationToken);
        Task<T> task1 = Task.Run<T>((Func<Task<T>>) (async () =>
        {
          T obj;
          try
          {
            obj = await workAsync(processor).ConfigureAwait(false);
          }
          finally
          {
            queue.CompleteAdding();
          }
          return obj;
        }), cancellationToken);
        foreach (VssRequestPump.Callback callback in queue.GetReader())
        {
          Task task2 = callback(requestContext);
          if (task2 != null)
            task2.SyncResult();
        }
        return task1.SyncResult<T>();
      }
    }

    public static Task PumpFromAsync(
      this IVssRequestContext requestContext,
      ISecuredDomainRequest domainRequest,
      Func<VssRequestPump.SecuredDomainProcessor, Task> workAsync)
    {
      return (Task) requestContext.PumpFromAsync<int>(domainRequest, (Func<VssRequestPump.SecuredDomainProcessor, Task<int>>) (async processor =>
      {
        await workAsync(processor).ConfigureAwait(false);
        return 0;
      }));
    }

    public static void Pump(
      this IVssRequestContext requestContext,
      ISecuredDomainRequest domainRequest,
      Func<VssRequestPump.SecuredDomainProcessor, Task> workAsync)
    {
      requestContext.Pump<int>(domainRequest, (Func<VssRequestPump.SecuredDomainProcessor, Task<int>>) (async processor =>
      {
        await workAsync(processor).ConfigureAwait(false);
        return 0;
      }));
    }

    [DebuggerStepThrough]
    public static void PumpOrInline(
      this IVssRequestContext requestContext,
      ISecuredDomainRequest domainRequest,
      Func<VssRequestPump.SecuredDomainProcessor, Task> workAsync,
      bool runCallbacksOnThisThread)
    {
      requestContext.PumpOrInline<int>(domainRequest, (Func<VssRequestPump.SecuredDomainProcessor, Task<int>>) (async p =>
      {
        await workAsync(p).ConfigureAwait(false);
        return 0;
      }), runCallbacksOnThisThread);
    }

    [DebuggerStepThrough]
    public static T PumpOrInline<T>(
      this IVssRequestContext requestContext,
      ISecuredDomainRequest domainRequest,
      Func<VssRequestPump.SecuredDomainProcessor, Task<T>> workAsync,
      bool runCallbacksOnThisThread)
    {
      return runCallbacksOnThisThread ? requestContext.Pump<T>(domainRequest, (Func<VssRequestPump.SecuredDomainProcessor, Task<T>>) (vssProcessor => workAsync(vssProcessor))) : workAsync(VssRequestPump.SecuredDomainProcessor.CreateWithoutRequestContext(requestContext.ActivityId, requestContext.CancellationToken, requestContext.E2EId, requestContext.UniqueIdentifier, domainRequest)).SyncResult<T>();
    }

    [DebuggerStepThrough]
    public static Task PumpOrInlineFromAsync(
      this IVssRequestContext requestContext,
      ISecuredDomainRequest domainRequest,
      Func<VssRequestPump.SecuredDomainProcessor, Task> workAsync,
      bool runCallbacksOnThisThread)
    {
      return (Task) requestContext.PumpOrInlineFromAsync<int>(domainRequest, (Func<VssRequestPump.SecuredDomainProcessor, Task<int>>) (async p =>
      {
        await workAsync(p).ConfigureAwait(false);
        return 0;
      }), runCallbacksOnThisThread);
    }

    [DebuggerStepThrough]
    public static Task<T> PumpOrInlineFromAsync<T>(
      this IVssRequestContext requestContext,
      ISecuredDomainRequest domainRequest,
      Func<VssRequestPump.SecuredDomainProcessor, Task<T>> workAsync,
      bool runCallbacksOnThisThread)
    {
      return runCallbacksOnThisThread ? requestContext.PumpFromAsync<T>(domainRequest, (Func<VssRequestPump.SecuredDomainProcessor, Task<T>>) (vssProcessor => workAsync(vssProcessor))) : workAsync(VssRequestPump.SecuredDomainProcessor.CreateWithoutRequestContext(requestContext.ActivityId, requestContext.CancellationToken, requestContext.E2EId, requestContext.UniqueIdentifier, domainRequest));
    }

    public delegate Task Callback(IVssRequestContext requestContext);

    public delegate void ProcessorFunc(VssRequestPump.Callback callback);

    public class Processor
    {
      protected readonly VssRequestPump.ProcessorFunc func;
      public readonly CancellationToken CancellationToken;
      public readonly Guid ActivityId;
      public readonly Guid E2EId;
      public readonly Guid X_TFS_Session;

      public bool HasRequestContext => this.func != null;

      internal Processor(
        Guid activityId,
        Guid E2EId,
        Guid sessionId,
        VssRequestPump.ProcessorFunc processor,
        CancellationToken cancellationToken)
      {
        this.ActivityId = activityId;
        this.E2EId = E2EId;
        this.func = processor;
        this.CancellationToken = cancellationToken;
        this.X_TFS_Session = sessionId;
      }

      public static VssRequestPump.Processor CreateWithoutRequestContext(
        CancellationToken cancellationToken,
        Guid activityId,
        Guid E2EId,
        Guid sessionId)
      {
        return new VssRequestPump.Processor(activityId, E2EId, sessionId, (VssRequestPump.ProcessorFunc) null, cancellationToken);
      }

      public static VssRequestPump.Processor CreateWithNullRequestContext(
        Guid activityId,
        Guid E2EId,
        Guid sessionId,
        CancellationToken cancellationToken)
      {
        Task task;
        return new VssRequestPump.Processor(activityId, E2EId, sessionId, (VssRequestPump.ProcessorFunc) (callback => task = callback((IVssRequestContext) null)), cancellationToken);
      }

      public static VssRequestPump.Processor CreatePassthroughForOriginalThread(
        IVssRequestContext requestContext)
      {
        SynchronizationContext originalContext = SynchronizationContext.Current;
        return new VssRequestPump.Processor(requestContext.ActivityId, requestContext.E2EId, requestContext.UniqueIdentifier, (VssRequestPump.ProcessorFunc) (callback =>
        {
          if (originalContext != SynchronizationContext.Current)
            throw new InvalidOperationException("CreatePassthroughForOriginalThread is used improperly. The callback is trying to get access to request context from a different synchronization context.");
          Task task = callback(requestContext);
        }), requestContext.CancellationToken);
      }

      public static VssRequestPump.Processor CreatePassthroughForUnitTests(
        IVssRequestContext requestContext)
      {
        Task task;
        return new VssRequestPump.Processor(requestContext.ActivityId == Guid.Empty ? Guid.NewGuid() : requestContext.ActivityId, requestContext.E2EId == Guid.Empty ? Guid.NewGuid() : requestContext.E2EId, requestContext.UniqueIdentifier == Guid.Empty ? Guid.NewGuid() : requestContext.UniqueIdentifier, (VssRequestPump.ProcessorFunc) (callback => task = callback(requestContext)), requestContext.CancellationToken);
      }

      public VssRequestPump.Processor CreateWithTimeout(TimeSpan timeout) => new VssRequestPump.Processor(this.ActivityId, this.E2EId, this.X_TFS_Session, this.func, CancellationTokenSource.CreateLinkedTokenSource(this.CancellationToken, new CancellationTokenSource(timeout).Token).Token);

      public Task<T> ExecuteWorkAsync<T>(Func<IVssRequestContext, T> callback)
      {
        if (this.func == null)
          throw new InvalidOperationException("Created without RequestContext, but attempted to use it.");
        SafeTaskCompletionSource<T> tcs = new SafeTaskCompletionSource<T>(TaskCreationOptions.RunContinuationsAsynchronously);
        this.func((VssRequestPump.Callback) (requestContext =>
        {
          try
          {
            tcs.SetResult(callback(requestContext));
          }
          catch (Exception ex)
          {
            tcs.SetException(ex);
          }
          return (Task) null;
        }));
        return tcs.Task;
      }

      public Task<T> ExecuteAsyncWorkAsync<T>(Func<IVssRequestContext, Task<T>> callbackAsync)
      {
        if (this.func == null)
          throw new InvalidOperationException("Created without RequestContext, but attempted to use it.");
        SafeTaskCompletionSource<T> tcs = new SafeTaskCompletionSource<T>(TaskCreationOptions.RunContinuationsAsynchronously);
        this.func((VssRequestPump.Callback) (async requestContext =>
        {
          try
          {
            T result = await callbackAsync(requestContext);
            tcs.SetResult(result);
          }
          catch (Exception ex)
          {
            tcs.SetException(ex);
          }
        }));
        return tcs.Task;
      }

      public Task ExecuteWorkAsync(Action<IVssRequestContext> callback) => (Task) this.ExecuteWorkAsync<int>((Func<IVssRequestContext, int>) (requestContext =>
      {
        callback(requestContext);
        return 0;
      }));

      public Task ExecuteAsyncWorkAsync(Func<IVssRequestContext, Task> callbackAsync) => (Task) this.ExecuteAsyncWorkAsync<int>((Func<IVssRequestContext, Task<int>>) (async requestContext =>
      {
        await callbackAsync(requestContext);
        return 0;
      }));
    }

    public class SecuredDomainProcessor : VssRequestPump.Processor
    {
      public readonly ISecuredDomainRequest SecuredDomainRequest;

      internal SecuredDomainProcessor(
        Guid activityId,
        Guid E2EId,
        Guid sessionId,
        ISecuredDomainRequest domainRequest,
        VssRequestPump.ProcessorFunc processor,
        CancellationToken cancellationToken)
        : base(activityId, E2EId, sessionId, processor, cancellationToken)
      {
        this.SecuredDomainRequest = domainRequest;
      }

      public static VssRequestPump.SecuredDomainProcessor CreateWithoutRequestContext(
        Guid activityId,
        CancellationToken cancellationToken,
        Guid E2EId,
        Guid sessionId,
        ISecuredDomainRequest domainRequest)
      {
        return new VssRequestPump.SecuredDomainProcessor(activityId, E2EId, sessionId, domainRequest, (VssRequestPump.ProcessorFunc) null, cancellationToken);
      }

      public static VssRequestPump.SecuredDomainProcessor CreateWithNullRequestContext(
        Guid activityId,
        Guid E2EId,
        Guid sessionId,
        ISecuredDomainRequest domainRequest,
        CancellationToken cancellationToken)
      {
        Task task;
        return new VssRequestPump.SecuredDomainProcessor(activityId, E2EId, sessionId, domainRequest, (VssRequestPump.ProcessorFunc) (callback => task = callback((IVssRequestContext) null)), cancellationToken);
      }

      public static VssRequestPump.SecuredDomainProcessor CreatePassthroughForOriginalThread(
        IVssRequestContext requestContext,
        ISecuredDomainRequest domainRequest)
      {
        SynchronizationContext originalContext = SynchronizationContext.Current;
        return new VssRequestPump.SecuredDomainProcessor(requestContext.ActivityId, requestContext.E2EId, requestContext.UniqueIdentifier, domainRequest, (VssRequestPump.ProcessorFunc) (callback =>
        {
          if (originalContext != SynchronizationContext.Current)
            throw new InvalidOperationException("CreatePassthroughForOriginalThread is used improperly. The callback is trying to get access to request context from a different synchronization context.");
          Task task = callback(requestContext);
        }), requestContext.CancellationToken);
      }

      public static VssRequestPump.SecuredDomainProcessor CreatePassthroughForUnitTests(
        IVssRequestContext requestContext,
        ISecuredDomainRequest domainRequest)
      {
        Task task;
        return new VssRequestPump.SecuredDomainProcessor(requestContext.ActivityId == Guid.Empty ? Guid.NewGuid() : requestContext.ActivityId, requestContext.E2EId == Guid.Empty ? Guid.NewGuid() : requestContext.E2EId, requestContext.UniqueIdentifier == Guid.Empty ? Guid.NewGuid() : requestContext.UniqueIdentifier, domainRequest, (VssRequestPump.ProcessorFunc) (callback => task = callback(requestContext)), requestContext.CancellationToken);
      }

      public VssRequestPump.SecuredDomainProcessor CreateWithTimeout(TimeSpan timeout) => new VssRequestPump.SecuredDomainProcessor(this.ActivityId, this.E2EId, this.X_TFS_Session, this.SecuredDomainRequest, this.func, CancellationTokenSource.CreateLinkedTokenSource(this.CancellationToken, new CancellationTokenSource(timeout).Token).Token);
    }
  }
}
