// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Common.InputQueue`1
// Assembly: Microsoft.TeamFoundation.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E643B42-FE11-4FC2-A9D6-79417E26CF92
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Common.dll

using Microsoft.TeamFoundation.Common.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Framework.Common
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class InputQueue<T> : IDisposable where T : class
  {
    private InputQueue<T>.State m_state;
    private object m_thisLock;
    private InputQueue<T>.FastItemQueue m_items;
    private List<InputQueue<T>.IQueueWaiter> m_waiters = new List<InputQueue<T>.IQueueWaiter>();
    private List<InputQueue<T>.IQueueReader> m_readers = new List<InputQueue<T>.IQueueReader>();

    public InputQueue()
    {
      this.m_state = InputQueue<T>.State.Open;
      this.m_thisLock = new object();
      this.m_items = new InputQueue<T>.FastItemQueue(1);
    }

    public int Count
    {
      get
      {
        lock (this.m_thisLock)
          return this.m_items.Count;
      }
    }

    public void Close()
    {
      bool flag = false;
      lock (this.m_thisLock)
      {
        if (this.m_state != InputQueue<T>.State.Closed)
        {
          this.m_state = InputQueue<T>.State.Closed;
          flag = true;
        }
      }
      if (!flag)
        return;
      while (this.m_readers.Count > 0)
      {
        InputQueue<T>.IQueueReader reader = this.m_readers[0];
        this.m_readers.RemoveAt(0);
        InputQueue<T>.Item obj = new InputQueue<T>.Item((Exception) null, (ItemDequeuedCallback) null);
        reader.Set(obj);
      }
      while (this.m_items.HasAnyItem)
      {
        InputQueue<T>.Item obj = this.m_items.DequeueAnyItem();
        obj.Dispose();
        InputQueue<T>.InvokeDequeuedCallback(obj.DequeuedCallback);
      }
    }

    void IDisposable.Dispose()
    {
      this.Close();
      GC.SuppressFinalize((object) this);
    }

    public void Enqueue(T item, bool canDispatchOnThisThread = true) => this.Enqueue(item, (ItemDequeuedCallback) null, canDispatchOnThisThread);

    public void Enqueue(
      T item,
      ItemDequeuedCallback dequeuedCallback,
      bool canDispatchOnThisThread = true)
    {
      this.EnqueueAndDispatch(new InputQueue<T>.Item(item, dequeuedCallback), canDispatchOnThisThread);
    }

    public void Enqueue(Exception exception, bool canDispatchOnThisThread = true) => this.Enqueue(exception, (ItemDequeuedCallback) null, canDispatchOnThisThread);

    public void Enqueue(
      Exception exception,
      ItemDequeuedCallback dequeuedCallback,
      bool canDispatchOnThisThread = true)
    {
      this.EnqueueAndDispatch(new InputQueue<T>.Item(exception, dequeuedCallback), canDispatchOnThisThread);
    }

    private void EnqueueAndDispatch(InputQueue<T>.Item item, bool canDispatchOnThisThread)
    {
      InputQueue<T>.IQueueReader queueReader = (InputQueue<T>.IQueueReader) null;
      bool flag1 = false;
      bool flag2 = false;
      InputQueue<T>.IQueueWaiter[] waiters = (InputQueue<T>.IQueueWaiter[]) null;
      bool itemAvailable = false;
      lock (this.m_thisLock)
      {
        waiters = this.SnapshotWaiters();
        if (this.m_state == InputQueue<T>.State.Open)
        {
          itemAvailable = true;
          if (canDispatchOnThisThread)
          {
            if (this.m_readers.Count == 0)
            {
              this.m_items.EnqueueAvailableItem(item);
            }
            else
            {
              queueReader = this.m_readers[0];
              this.m_readers.RemoveAt(0);
            }
          }
          else if (this.m_readers.Count == 0)
          {
            this.m_items.EnqueueAvailableItem(item);
          }
          else
          {
            this.m_items.EnqueuePendingItem(item);
            flag2 = true;
          }
        }
        else
          flag1 = true;
      }
      if (waiters != null)
      {
        if (canDispatchOnThisThread)
          InputQueue<T>.CompleteWaiters(itemAvailable, waiters);
        else
          InputQueue<T>.CompleteWaitersLater(itemAvailable, waiters);
      }
      if (queueReader != null)
      {
        InputQueue<T>.InvokeDequeuedCallback(item.DequeuedCallback);
        queueReader.Set(item);
      }
      if (flag2)
      {
        if (ThreadPool.QueueUserWorkItem(new WaitCallback(InputQueue<T>.DispatchLaterCallback), (object) this))
          return;
        this.Dispatch();
      }
      else
      {
        if (!flag1)
          return;
        InputQueue<T>.InvokeDequeuedCallback(item.DequeuedCallback);
        item.Dispose();
      }
    }

    public bool Dequeue(TimeSpan timeout, out T item)
    {
      InputQueue<T>.Item obj = new InputQueue<T>.Item();
      InputQueue<T>.QueueReader queueReader = (InputQueue<T>.QueueReader) null;
      lock (this.m_thisLock)
      {
        if (this.m_state == InputQueue<T>.State.Open)
        {
          if (this.m_items.HasAvailableItem)
          {
            obj = this.m_items.DequeueAvailableItem();
          }
          else
          {
            queueReader = new InputQueue<T>.QueueReader(this);
            this.m_readers.Add((InputQueue<T>.IQueueReader) queueReader);
          }
        }
        else if (this.m_state == InputQueue<T>.State.Shutdown)
        {
          if (this.m_items.HasAvailableItem)
            obj = this.m_items.DequeueAvailableItem();
          else if (this.m_items.HasAnyItem)
          {
            queueReader = new InputQueue<T>.QueueReader(this);
            this.m_readers.Add((InputQueue<T>.IQueueReader) queueReader);
          }
          else
          {
            item = default (T);
            return true;
          }
        }
        else
        {
          item = default (T);
          return true;
        }
      }
      if (queueReader != null)
        return queueReader.Dequeue(timeout, out item);
      InputQueue<T>.InvokeDequeuedCallback(obj.DequeuedCallback);
      item = obj.GetValue();
      return true;
    }

    public Task<T> DequeueAsync() => this.DequeueAsync(TimeSpan.MaxValue, CancellationToken.None);

    public Task<T> DequeueAsync(TimeSpan timeout) => this.DequeueAsync(timeout, CancellationToken.None);

    public Task<T> DequeueAsync(CancellationToken cancellationToken) => this.DequeueAsync(TimeSpan.MaxValue, cancellationToken);

    public Task<T> DequeueAsync(TimeSpan timeout, CancellationToken cancellationToken) => Task.Factory.FromAsync<T>((Func<AsyncCallback, object, IAsyncResult>) ((callback, state) => this.BeginDequeue(timeout, cancellationToken, callback, state)), (Func<IAsyncResult, T>) (result => this.EndDequeue(result)), (object) null);

    public IAsyncResult BeginDequeue(TimeSpan timeout, AsyncCallback callback, object state) => this.BeginDequeue(timeout, CancellationToken.None, callback, state);

    public IAsyncResult BeginDequeue(
      CancellationToken cancellationToken,
      AsyncCallback callback,
      object state)
    {
      return this.BeginDequeue(TimeSpan.MaxValue, cancellationToken, callback, state);
    }

    public IAsyncResult BeginDequeue(
      TimeSpan timeout,
      CancellationToken cancellationToken,
      AsyncCallback callback,
      object state)
    {
      InputQueue<T>.Item obj = new InputQueue<T>.Item();
      InputQueue<T>.AsyncQueueReader asyncQueueReader = (InputQueue<T>.AsyncQueueReader) null;
      lock (this.m_thisLock)
      {
        if (this.m_state == InputQueue<T>.State.Open)
        {
          if (this.m_items.HasAvailableItem)
          {
            obj = this.m_items.DequeueAvailableItem();
          }
          else
          {
            asyncQueueReader = new InputQueue<T>.AsyncQueueReader(this, timeout, cancellationToken, callback, state);
            this.m_readers.Add((InputQueue<T>.IQueueReader) asyncQueueReader);
          }
        }
        else if (this.m_state == InputQueue<T>.State.Shutdown)
        {
          if (this.m_items.HasAvailableItem)
            obj = this.m_items.DequeueAvailableItem();
          else if (this.m_items.HasAnyItem)
          {
            asyncQueueReader = new InputQueue<T>.AsyncQueueReader(this, timeout, cancellationToken, callback, state);
            this.m_readers.Add((InputQueue<T>.IQueueReader) asyncQueueReader);
          }
        }
      }
      if (asyncQueueReader != null)
        return (IAsyncResult) asyncQueueReader;
      InputQueue<T>.InvokeDequeuedCallback(obj.DequeuedCallback);
      return (IAsyncResult) new CompletedOperation<T>(obj.GetValue(), callback, state);
    }

    public T EndDequeue(IAsyncResult result)
    {
      T obj;
      if (!this.EndDequeue(result, out obj) && result is InputQueue<T>.AsyncQueueReader)
        ((InputQueue<T>.AsyncQueueReader) result).ThrowIfExpired();
      return obj;
    }

    public bool EndDequeue(IAsyncResult result, out T item)
    {
      if (!(result is CompletedOperation<T>))
        return InputQueue<T>.AsyncQueueReader.End(result, out item);
      item = CompletedOperation<T>.End(result);
      return true;
    }

    public bool WaitForItem(TimeSpan timeout)
    {
      bool flag = false;
      InputQueue<T>.QueueWaiter queueWaiter = (InputQueue<T>.QueueWaiter) null;
      lock (this.m_thisLock)
      {
        if (this.m_state == InputQueue<T>.State.Open)
        {
          if (this.m_items.HasAvailableItem)
          {
            flag = true;
          }
          else
          {
            queueWaiter = new InputQueue<T>.QueueWaiter();
            this.m_waiters.Add((InputQueue<T>.IQueueWaiter) queueWaiter);
          }
        }
        else
        {
          if (this.m_state != InputQueue<T>.State.Shutdown)
            return true;
          if (this.m_items.HasAvailableItem)
          {
            flag = true;
          }
          else
          {
            if (!this.m_items.HasAnyItem)
              return false;
            queueWaiter = new InputQueue<T>.QueueWaiter();
            this.m_waiters.Add((InputQueue<T>.IQueueWaiter) queueWaiter);
          }
        }
      }
      return queueWaiter != null ? queueWaiter.WaitForItem(timeout) : flag;
    }

    public IAsyncResult BeginWaitForItem(TimeSpan timeout, AsyncCallback callback, object state)
    {
      lock (this.m_thisLock)
      {
        if (this.m_state == InputQueue<T>.State.Open)
        {
          if (!this.m_items.HasAvailableItem)
          {
            InputQueue<T>.AsyncQueueWaiter asyncQueueWaiter = new InputQueue<T>.AsyncQueueWaiter(timeout, callback, state);
            this.m_waiters.Add((InputQueue<T>.IQueueWaiter) asyncQueueWaiter);
            return (IAsyncResult) asyncQueueWaiter;
          }
        }
        else if (this.m_state == InputQueue<T>.State.Shutdown)
        {
          if (!this.m_items.HasAvailableItem)
          {
            if (this.m_items.HasAnyItem)
            {
              InputQueue<T>.AsyncQueueWaiter asyncQueueWaiter = new InputQueue<T>.AsyncQueueWaiter(timeout, callback, state);
              this.m_waiters.Add((InputQueue<T>.IQueueWaiter) asyncQueueWaiter);
              return (IAsyncResult) asyncQueueWaiter;
            }
          }
        }
      }
      return (IAsyncResult) new CompletedOperation<bool>(true, callback, state);
    }

    public bool EndWaitForItem(IAsyncResult result) => result is CompletedOperation<bool> ? CompletedOperation<bool>.End(result) : InputQueue<T>.AsyncQueueWaiter.End(result);

    public void Shutdown()
    {
      InputQueue<T>.IQueueReader[] array = (InputQueue<T>.IQueueReader[]) null;
      lock (this.m_thisLock)
      {
        if (this.m_state != InputQueue<T>.State.Open)
          return;
        this.m_state = InputQueue<T>.State.Shutdown;
        if (this.m_readers.Count > 0)
        {
          if (this.m_items.Count == 0)
          {
            array = new InputQueue<T>.IQueueReader[this.m_readers.Count];
            this.m_readers.CopyTo(array, 0);
            this.m_readers.Clear();
          }
        }
      }
      if (array == null)
        return;
      for (int index = 0; index < array.Length; ++index)
        array[index].Set(new InputQueue<T>.Item((Exception) null, (ItemDequeuedCallback) null));
    }

    private bool RemoveWaiter(InputQueue<T>.IQueueWaiter waiter)
    {
      bool flag = false;
      lock (this.m_thisLock)
      {
        if (this.m_state != InputQueue<T>.State.Open)
        {
          if (this.m_state != InputQueue<T>.State.Shutdown)
            goto label_11;
        }
        for (int index = this.m_waiters.Count - 1; index >= 0; --index)
        {
          if (this.m_waiters[index] == waiter)
          {
            flag = true;
            this.m_waiters.RemoveAt(index);
            break;
          }
        }
      }
label_11:
      return flag;
    }

    private bool RemoveReader(InputQueue<T>.IQueueReader reader)
    {
      bool flag = false;
      lock (this.m_thisLock)
      {
        if (this.m_state != InputQueue<T>.State.Open)
        {
          if (this.m_state != InputQueue<T>.State.Shutdown)
            goto label_11;
        }
        for (int index = this.m_readers.Count - 1; index >= 0; --index)
        {
          if (this.m_readers[index] == reader)
          {
            flag = true;
            this.m_readers.RemoveAt(index);
            break;
          }
        }
      }
label_11:
      return flag;
    }

    private InputQueue<T>.IQueueWaiter[] SnapshotWaiters()
    {
      InputQueue<T>.IQueueWaiter[] array = (InputQueue<T>.IQueueWaiter[]) null;
      if (this.m_waiters.Count > 0)
      {
        array = new InputQueue<T>.IQueueWaiter[this.m_waiters.Count];
        this.m_waiters.CopyTo(array, 0);
        this.m_waiters.Clear();
      }
      return array;
    }

    private static void CompleteReaders(InputQueue<T>.IQueueReader[] readers)
    {
      for (int index = 0; index < readers.Length; ++index)
        readers[index].Set(new InputQueue<T>.Item());
    }

    private static void CompleteReadersLater(InputQueue<T>.IQueueReader[] readers)
    {
      if (ThreadPool.QueueUserWorkItem(new WaitCallback(InputQueue<T>.CompleteReadersCallback), (object) readers))
        return;
      InputQueue<T>.CompleteReaders(readers);
    }

    private static void CompleteReadersCallback(object state) => InputQueue<T>.CompleteReaders((InputQueue<T>.IQueueReader[]) state);

    private static void CompleteWaiters(bool itemAvailable, InputQueue<T>.IQueueWaiter[] waiters)
    {
      for (int index = 0; index < waiters.Length; ++index)
        waiters[index].Set(itemAvailable);
    }

    private static void CompleteWaitersLater(
      bool itemAvailable,
      InputQueue<T>.IQueueWaiter[] waiters)
    {
      if (itemAvailable)
      {
        if (ThreadPool.QueueUserWorkItem(new WaitCallback(InputQueue<T>.CompleteWaitersTrueCallback), (object) waiters))
          return;
        InputQueue<T>.CompleteWaiters(true, waiters);
      }
      else
      {
        if (ThreadPool.QueueUserWorkItem(new WaitCallback(InputQueue<T>.CompleteWaitersFalseCallback), (object) waiters))
          return;
        InputQueue<T>.CompleteWaiters(false, waiters);
      }
    }

    private static void CompleteWaitersTrueCallback(object state) => InputQueue<T>.CompleteWaiters(true, (InputQueue<T>.IQueueWaiter[]) state);

    private static void CompleteWaitersFalseCallback(object state) => InputQueue<T>.CompleteWaiters(false, (InputQueue<T>.IQueueWaiter[]) state);

    private void Dispatch()
    {
      InputQueue<T>.Item obj = new InputQueue<T>.Item();
      InputQueue<T>.IQueueReader queueReader = (InputQueue<T>.IQueueReader) null;
      bool itemAvailable = true;
      InputQueue<T>.IQueueWaiter[] waiters = (InputQueue<T>.IQueueWaiter[]) null;
      InputQueue<T>.IQueueReader[] queueReaderArray = (InputQueue<T>.IQueueReader[]) null;
      lock (this.m_thisLock)
      {
        waiters = this.SnapshotWaiters();
        itemAvailable = this.m_state == InputQueue<T>.State.Open;
        if (this.m_state != InputQueue<T>.State.Closed)
        {
          this.m_items.MakePendingItemAvailable();
          if (this.m_readers.Count > 0)
          {
            queueReader = this.m_readers[0];
            this.m_readers.RemoveAt(0);
            obj = this.m_items.DequeueAvailableItem();
            if (this.m_state == InputQueue<T>.State.Shutdown)
            {
              if (this.m_readers.Count > 0)
              {
                if (this.m_items.Count == 0)
                {
                  queueReaderArray = new InputQueue<T>.IQueueReader[this.m_readers.Count];
                  this.m_readers.CopyTo(queueReaderArray, 0);
                  this.m_readers.Clear();
                  itemAvailable = false;
                }
              }
            }
          }
        }
      }
      if (queueReaderArray != null)
        InputQueue<T>.CompleteReadersLater(queueReaderArray);
      if (waiters != null)
        InputQueue<T>.CompleteWaitersLater(itemAvailable, waiters);
      if (queueReader == null)
        return;
      InputQueue<T>.InvokeDequeuedCallback(obj.DequeuedCallback);
      queueReader.Set(obj);
    }

    private static void DispatchLaterCallback(object state) => ((InputQueue<T>) state).Dispatch();

    private static void InvokeDequeuedCallback(ItemDequeuedCallback dequeuedCallback)
    {
      if (dequeuedCallback == null)
        return;
      dequeuedCallback();
    }

    private enum State
    {
      Open,
      Shutdown,
      Closed,
    }

    private interface IQueueReader
    {
      void Set(InputQueue<T>.Item item);
    }

    private interface IQueueWaiter
    {
      void Set(bool itemAvailable);
    }

    private sealed class QueueReader : InputQueue<T>.IQueueReader, IDisposable
    {
      private T m_item;
      private Exception m_exception;
      private InputQueue<T> m_inputQueue;
      private ManualResetEvent m_waitEvent = new ManualResetEvent(false);

      public QueueReader(InputQueue<T> inputQueue) => this.m_inputQueue = inputQueue;

      public void Dispose()
      {
        lock (this)
        {
          if (this.m_waitEvent == null)
            return;
          this.m_waitEvent.Close();
          this.m_waitEvent = (ManualResetEvent) null;
        }
      }

      public bool Dequeue(TimeSpan timeout, out T value)
      {
        bool flag = false;
        try
        {
          if (!TimeoutHelper.WaitOne((WaitHandle) this.m_waitEvent, timeout))
          {
            if (this.m_inputQueue.RemoveReader((InputQueue<T>.IQueueReader) this))
            {
              value = default (T);
              flag = true;
              return false;
            }
            this.m_waitEvent.WaitOne();
          }
          flag = true;
        }
        finally
        {
          if (flag)
            this.m_waitEvent.Close();
        }
        if (this.m_exception != null)
          throw this.m_exception;
        value = this.m_item;
        return true;
      }

      public void Set(InputQueue<T>.Item item)
      {
        lock (this)
        {
          if (this.m_waitEvent == null)
            return;
          this.m_item = item.Value;
          this.m_exception = item.Exception;
          this.m_waitEvent.Set();
        }
      }
    }

    private sealed class QueueWaiter : InputQueue<T>.IQueueWaiter
    {
      private bool m_itemAvailable;
      private ManualResetEvent m_waitEvent;

      public QueueWaiter() => this.m_waitEvent = new ManualResetEvent(false);

      public void Set(bool itemAvailable)
      {
        this.m_itemAvailable = itemAvailable;
        this.m_waitEvent.Set();
      }

      public bool WaitForItem(TimeSpan timeout)
      {
        try
        {
          return TimeoutHelper.WaitOne((WaitHandle) this.m_waitEvent, timeout) && this.m_itemAvailable;
        }
        finally
        {
          if (this.m_waitEvent != null)
            this.m_waitEvent.Close();
        }
      }
    }

    private sealed class AsyncQueueReader : AsyncOperation, InputQueue<T>.IQueueReader
    {
      private T m_item;
      private Timer m_timer;
      private bool m_expired;
      private TimeSpan m_timeout;
      private InputQueue<T> m_inputQueue;
      private CancellationTokenRegistration m_cancellationRegistration;

      public AsyncQueueReader(
        InputQueue<T> inputQueue,
        TimeSpan timeout,
        CancellationToken cancellationToken,
        AsyncCallback callback,
        object state)
        : base(callback, state)
      {
        this.m_timeout = timeout;
        this.m_inputQueue = inputQueue;
        if (timeout != TimeSpan.MaxValue)
        {
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          this.m_timer = new Timer(InputQueue<T>.AsyncQueueReader.\u003C\u003EO.\u003C0\u003E__TimerCallback ?? (InputQueue<T>.AsyncQueueReader.\u003C\u003EO.\u003C0\u003E__TimerCallback = new TimerCallback(InputQueue<T>.AsyncQueueReader.TimerCallback)), (object) this, timeout, TimeSpan.FromMilliseconds(-1.0));
        }
        if (!(cancellationToken != CancellationToken.None))
          return;
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        this.m_cancellationRegistration = cancellationToken.Register(InputQueue<T>.AsyncQueueReader.\u003C\u003EO.\u003C1\u003E__CancellationCallback ?? (InputQueue<T>.AsyncQueueReader.\u003C\u003EO.\u003C1\u003E__CancellationCallback = new Action<object>(InputQueue<T>.AsyncQueueReader.CancellationCallback)), (object) this, false);
      }

      protected override void Dispose()
      {
        if (this.m_timer != null)
        {
          this.m_timer.Dispose();
          this.m_timer = (Timer) null;
        }
        if (this.m_cancellationRegistration != new CancellationTokenRegistration())
          this.m_cancellationRegistration.Dispose();
        base.Dispose();
      }

      public void ThrowIfExpired()
      {
        if (this.m_expired)
          throw new TimeoutException(TFCommonResources.DequeueTimeout((object) this.m_timeout));
      }

      public static bool End(IAsyncResult result, out T value)
      {
        InputQueue<T>.AsyncQueueReader asyncQueueReader = AsyncOperation.End<InputQueue<T>.AsyncQueueReader>(result);
        if (asyncQueueReader.m_expired)
        {
          value = default (T);
          return false;
        }
        value = asyncQueueReader.m_item;
        return true;
      }

      private static void CancellationCallback(object state)
      {
        InputQueue<T>.AsyncQueueReader reader = (InputQueue<T>.AsyncQueueReader) state;
        if (!reader.m_inputQueue.RemoveReader((InputQueue<T>.IQueueReader) reader))
          return;
        reader.Complete(false, (Exception) new TaskCanceledException());
      }

      private static void TimerCallback(object state)
      {
        InputQueue<T>.AsyncQueueReader reader = (InputQueue<T>.AsyncQueueReader) state;
        if (!reader.m_inputQueue.RemoveReader((InputQueue<T>.IQueueReader) reader))
          return;
        reader.m_expired = true;
        reader.Complete(false);
      }

      public void Set(InputQueue<T>.Item item)
      {
        this.m_item = item.Value;
        if (this.m_timer != null)
          this.m_timer.Change(-1, -1);
        this.Complete(false, item.Exception);
      }
    }

    private sealed class AsyncQueueWaiter : AsyncOperation, InputQueue<T>.IQueueWaiter
    {
      private Timer m_timer;
      private bool m_itemAvailable;
      private object m_thisLock = new object();

      public AsyncQueueWaiter(TimeSpan timeout, AsyncCallback callback, object state)
        : base(callback, state)
      {
        if (!(timeout != TimeSpan.MaxValue))
          return;
        this.m_timer = new Timer(new TimerCallback(InputQueue<T>.AsyncQueueWaiter.TimerCallback), (object) this, timeout, TimeSpan.FromMilliseconds(-1.0));
      }

      protected override void Dispose()
      {
        if (this.m_timer != null)
        {
          this.m_timer.Dispose();
          this.m_timer = (Timer) null;
        }
        base.Dispose();
      }

      public static bool End(IAsyncResult result) => AsyncOperation.End<InputQueue<T>.AsyncQueueWaiter>(result).m_itemAvailable;

      private static void TimerCallback(object state) => ((AsyncOperation) state).Complete(false);

      public void Set(bool itemAvailable)
      {
        bool flag;
        lock (this.m_thisLock)
        {
          flag = this.m_timer == null || this.m_timer.Change(-1, -1);
          this.m_itemAvailable = itemAvailable;
        }
        if (!flag)
          return;
        this.Complete(false);
      }
    }

    private sealed class FastItemQueue
    {
      private InputQueue<T>.Item[] m_items;
      private int m_headPtr;
      private int m_tailPtr;
      private int m_totalCount;
      private int m_pendingCount;

      public FastItemQueue(int initialSize) => this.m_items = new InputQueue<T>.Item[initialSize];

      public int Count => this.m_totalCount;

      public bool HasAnyItem => this.m_totalCount > 0;

      public bool HasAvailableItem => this.m_totalCount > this.m_pendingCount;

      public bool EnqueueAvailableItem(InputQueue<T>.Item item) => this.EnqueueItem(item);

      public void EnqueuePendingItem(InputQueue<T>.Item item)
      {
        this.EnqueueItem(item);
        ++this.m_pendingCount;
      }

      private bool EnqueueItem(InputQueue<T>.Item item)
      {
        if (this.m_totalCount == this.m_items.Length)
        {
          InputQueue<T>.Item[] objArray = new InputQueue<T>.Item[this.m_items.Length * 2];
          for (int index = 0; index < this.m_totalCount; ++index)
            objArray[index] = this.m_items[(this.m_headPtr + index) % this.m_items.Length];
          this.m_headPtr = 0;
          this.m_items = objArray;
        }
        this.m_tailPtr = (this.m_headPtr + this.m_totalCount) % this.m_items.Length;
        this.m_items[this.m_tailPtr] = item;
        ++this.m_totalCount;
        return true;
      }

      public InputQueue<T>.Item DequeueAnyItem()
      {
        if (this.m_pendingCount == this.m_totalCount)
          --this.m_pendingCount;
        return this.DequeueItem();
      }

      public InputQueue<T>.Item DequeueAvailableItem()
      {
        if (this.m_totalCount == this.m_pendingCount)
          throw new InvalidOperationException();
        return this.DequeueItem();
      }

      private InputQueue<T>.Item DequeueItem()
      {
        if (this.m_totalCount == 0)
          throw new InvalidOperationException();
        InputQueue<T>.Item obj = this.m_items[this.m_headPtr];
        this.m_items[this.m_headPtr] = new InputQueue<T>.Item();
        --this.m_totalCount;
        this.m_headPtr = (this.m_headPtr + 1) % this.m_items.Length;
        return obj;
      }

      public void MakePendingItemAvailable()
      {
        if (this.m_pendingCount == 0)
          throw new InvalidOperationException();
        --this.m_pendingCount;
      }
    }

    private struct Item
    {
      public Item(T value, ItemDequeuedCallback dequeuedCallback)
        : this(value, (Exception) null, dequeuedCallback)
      {
      }

      public Item(Exception exception, ItemDequeuedCallback dequeuedCallback)
        : this(default (T), exception, dequeuedCallback)
      {
      }

      private Item(T value, Exception exception, ItemDequeuedCallback dequeuedCallback)
        : this()
      {
        this.Value = value;
        this.Exception = exception;
        this.DequeuedCallback = dequeuedCallback;
      }

      public ItemDequeuedCallback DequeuedCallback { get; private set; }

      public T Value { get; private set; }

      public Exception Exception { get; private set; }

      public void Dispose()
      {
        if ((object) this.Value == null || !((object) this.Value is IDisposable))
          return;
        ((IDisposable) (object) this.Value).Dispose();
      }

      public T GetValue()
      {
        if (this.Exception != null)
          throw this.Exception;
        return this.Value;
      }
    }
  }
}
