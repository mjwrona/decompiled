// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.StreamingCollection`1
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class StreamingCollection<T> : IEnumerable<T>, IEnumerable
  {
    private Queue<T> m_queue;
    private Command m_command;
    private bool m_complete;
    private bool m_handleExceptions;
    private StreamingCollection<T>.StreamingEnumerator m_enumerator;
    private int m_cachedSize;
    private int m_defaultCachedSize = 16384;
    private bool m_defaultCachedSizeUsed = true;
    private List<StackTrace> m_getEnumeratorStackTraces;

    public StreamingCollection()
    {
      this.m_queue = new Queue<T>();
      this.m_complete = true;
      this.m_handleExceptions = false;
      this.m_enumerator = new StreamingCollection<T>.StreamingEnumerator(this);
      this.m_cachedSize = this.m_defaultCachedSize;
      if (this.m_enumerator.m_constructorStackTrace == null)
        return;
      this.m_getEnumeratorStackTraces = new List<StackTrace>();
    }

    public StreamingCollection(Command command)
    {
      this.m_command = command;
      this.m_handleExceptions = true;
      this.m_queue = new Queue<T>();
      this.m_enumerator = new StreamingCollection<T>.StreamingEnumerator(this);
      this.m_cachedSize = this.m_defaultCachedSize;
      if (this.m_enumerator.m_constructorStackTrace == null)
        return;
      this.m_getEnumeratorStackTraces = new List<StackTrace>();
    }

    public StreamingCollection(Command command, int cachedSize)
    {
      this.m_command = command;
      this.m_handleExceptions = true;
      this.m_queue = new Queue<T>();
      this.m_enumerator = new StreamingCollection<T>.StreamingEnumerator(this);
      this.m_cachedSize = cachedSize;
      this.m_defaultCachedSizeUsed = false;
      if (this.m_enumerator.m_constructorStackTrace == null)
        return;
      this.m_getEnumeratorStackTraces = new List<StackTrace>();
    }

    internal StreamingCollection(IEnumerable<T> items)
    {
      this.m_queue = new Queue<T>(items);
      this.m_complete = true;
      this.m_handleExceptions = false;
      this.m_enumerator = new StreamingCollection<T>.StreamingEnumerator(this);
      this.m_cachedSize = this.m_defaultCachedSize;
      if (this.m_enumerator.m_constructorStackTrace == null)
        return;
      this.m_getEnumeratorStackTraces = new List<StackTrace>();
    }

    public void Add(object item)
    {
      try
      {
        this.m_queue.Enqueue((T) item);
      }
      catch (InvalidCastException ex)
      {
        throw new ArgumentException(FrameworkResources.CouldNotCastParameterToT((object) typeof (T).AssemblyQualifiedName), nameof (item));
      }
    }

    public void Enqueue(T item)
    {
      int cachedSize = this.m_cachedSize;
      if (item is ICacheable cacheable)
        cachedSize = cacheable.GetCachedSize();
      else if (this.m_defaultCachedSizeUsed && TeamFoundationTracingService.IsRawTracingEnabled(7205, TraceLevel.Warning, nameof (Enqueue), nameof (StreamingCollection<T>), (string[]) null))
        TeamFoundationTracingService.TraceRaw(7205, TraceLevel.Warning, nameof (Enqueue), nameof (StreamingCollection<T>), "StreamingCollection used with non-ICacheable object when no cachedSize was specified! - call stack: {0}", (object) new StackTrace(true));
      this.m_queue.Enqueue(item);
      if (this.m_command == null || cachedSize <= 0)
        return;
      this.m_command.IncrementCacheUsage(cachedSize);
    }

    public void BindCommand(Command command) => this.m_command = command;

    public IEnumerator<T> GetEnumerator()
    {
      if (this.m_enumerator.m_constructorStackTrace != null)
        this.m_getEnumeratorStackTraces.Add(new StackTrace(false));
      return !this.m_enumerator.HasMovedNext ? (IEnumerator<T>) this.m_enumerator : throw new StreamingCollectionUnsupportedOperationException(this.m_enumerator.m_constructorStackTrace, this.m_getEnumeratorStackTraces);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      if (this.m_enumerator.m_constructorStackTrace != null)
        this.m_getEnumeratorStackTraces.Add(new StackTrace(false));
      return !this.m_enumerator.HasMovedNext ? (IEnumerator) this.m_enumerator : throw new StreamingCollectionUnsupportedOperationException(this.m_enumerator.m_constructorStackTrace, this.m_getEnumeratorStackTraces);
    }

    public bool MoveNext() => this.m_enumerator.MoveNext();

    public T Current => this.m_enumerator.Current;

    public bool IsComplete
    {
      get => this.m_complete;
      set => this.m_complete = value;
    }

    public bool HandleExceptions
    {
      get => this.m_handleExceptions;
      set => this.m_handleExceptions = value;
    }

    public IEnumerator<T> GetQueuedItemsEnumerator() => (IEnumerator<T>) this.m_queue.GetEnumerator();

    protected bool HasQueuedData => this.m_queue.Count > 0;

    private T Dequeue()
    {
      T obj = this.m_queue.Dequeue();
      if (this.m_command != null)
      {
        int cachedSize = this.m_cachedSize;
        if (obj is ICacheable cacheable)
          cachedSize = cacheable.GetCachedSize();
        else if (this.m_defaultCachedSizeUsed && TeamFoundationTracingService.IsRawTracingEnabled(7206, TraceLevel.Warning, nameof (Dequeue), nameof (StreamingCollection<T>), (string[]) null))
          TeamFoundationTracingService.TraceRaw(7206, TraceLevel.Warning, nameof (Dequeue), nameof (StreamingCollection<T>), "StreamingCollection used with non-ICacheable object when no cachedSize was specified! - call stack: {0}", (object) new StackTrace(true));
        if (cachedSize > 0)
          this.m_command.DecrementCacheUsage(cachedSize);
      }
      return obj;
    }

    internal Command Command => this.m_command;

    private class StreamingEnumerator : IEnumerator<T>, IDisposable, IEnumerator
    {
      private StreamingCollection<T> m_collection;
      private T m_current;
      public StackTrace m_constructorStackTrace;

      public StreamingEnumerator(StreamingCollection<T> collection)
      {
        this.m_collection = collection;
        if (!TeamFoundationTracingService.IsRawTracingEnabled(7201, TraceLevel.Verbose, nameof (StreamingCollection<T>), nameof (StreamingCollection<T>), (string[]) null))
          return;
        this.m_constructorStackTrace = new StackTrace(false);
      }

      public void Dispose()
      {
      }

      public T Current => this.m_current;

      object IEnumerator.Current => (object) this.m_current;

      public void Reset()
      {
        List<StackTrace> enumeratorCalls = (List<StackTrace>) null;
        if (this.m_constructorStackTrace != null)
        {
          enumeratorCalls = new List<StackTrace>();
          enumeratorCalls.Add(new StackTrace(false));
        }
        throw new StreamingCollectionUnsupportedOperationException(this.m_constructorStackTrace, enumeratorCalls);
      }

      public bool MoveNext()
      {
        if (this.m_collection.Command != null)
          this.m_collection.Command.RequestContext.RequestContextInternal().CheckCanceled();
        try
        {
          if (!this.m_collection.HasQueuedData)
          {
            if (!this.m_collection.IsComplete && this.m_collection.Command != null)
              this.m_collection.Command.ContinueExecution();
            if (!this.m_collection.HasQueuedData)
              return false;
          }
          this.m_current = this.m_collection.Dequeue();
        }
        catch (Exception ex)
        {
          if (this.m_collection.HandleExceptions)
          {
            if (this.m_collection.Command != null)
            {
              TeamFoundationEventLog.Default.LogException(this.m_collection.Command.RequestContext, FrameworkResources.CommandStopped((object) this.m_collection.Command.Name), ex);
              this.m_collection.Command.Cancel();
            }
            return false;
          }
          throw;
        }
        this.HasMovedNext = true;
        return true;
      }

      internal bool HasMovedNext { get; private set; }
    }
  }
}
