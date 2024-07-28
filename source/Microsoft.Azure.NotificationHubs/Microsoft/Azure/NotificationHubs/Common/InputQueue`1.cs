// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Common.InputQueue`1
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using System;
using System.Collections.Generic;
using System.Threading;

namespace Microsoft.Azure.NotificationHubs.Common
{
  internal sealed class InputQueue<T> : IDisposable where T : class
  {
    private static Action<object> completeOutstandingReadersCallback;
    private static Action<object> completeWaitersFalseCallback;
    private static Action<object> completeWaitersTrueCallback;
    private static Action<object> onDispatchCallback;
    private static Action<object> onInvokeDequeuedCallback;
    private InputQueue<T>.QueueState queueState;
    private InputQueue<T>.ItemQueue itemQueue;
    private Queue<InputQueue<T>.IQueueReader> readerQueue;
    private List<InputQueue<T>.IQueueWaiter> waiterList;

    public InputQueue()
    {
      this.itemQueue = new InputQueue<T>.ItemQueue();
      this.readerQueue = new Queue<InputQueue<T>.IQueueReader>();
      this.waiterList = new List<InputQueue<T>.IQueueWaiter>();
      this.queueState = InputQueue<T>.QueueState.Open;
    }

    public InputQueue(
      Func<Action<AsyncCallback, IAsyncResult>> asyncCallbackGenerator)
      : this()
    {
      this.AsyncCallbackGenerator = asyncCallbackGenerator;
    }

    public int PendingCount
    {
      get
      {
        lock (this.ThisLock)
          return this.itemQueue.ItemCount;
      }
    }

    public int ReadersQueueCount
    {
      get
      {
        lock (this.ThisLock)
          return this.readerQueue.Count;
      }
    }

    public Action<T> DisposeItemCallback { get; set; }

    private Func<Action<AsyncCallback, IAsyncResult>> AsyncCallbackGenerator { get; set; }

    private object ThisLock => (object) this.itemQueue;

    public IAsyncResult BeginDequeue(TimeSpan timeout, AsyncCallback callback, object state)
    {
      InputQueue<T>.Item obj = new InputQueue<T>.Item();
      lock (this.ThisLock)
      {
        if (this.queueState == InputQueue<T>.QueueState.Open)
        {
          if (this.itemQueue.HasAvailableItem)
          {
            obj = this.itemQueue.DequeueAvailableItem();
          }
          else
          {
            InputQueue<T>.AsyncQueueReader asyncQueueReader = new InputQueue<T>.AsyncQueueReader(this, timeout, callback, state);
            this.readerQueue.Enqueue((InputQueue<T>.IQueueReader) asyncQueueReader);
            return (IAsyncResult) asyncQueueReader;
          }
        }
        else if (this.queueState == InputQueue<T>.QueueState.Shutdown)
        {
          if (this.itemQueue.HasAvailableItem)
            obj = this.itemQueue.DequeueAvailableItem();
          else if (this.itemQueue.HasAnyItem)
          {
            InputQueue<T>.AsyncQueueReader asyncQueueReader = new InputQueue<T>.AsyncQueueReader(this, timeout, callback, state);
            this.readerQueue.Enqueue((InputQueue<T>.IQueueReader) asyncQueueReader);
            return (IAsyncResult) asyncQueueReader;
          }
        }
      }
      InputQueue<T>.InvokeDequeuedCallback(obj.DequeuedCallback);
      return (IAsyncResult) new CompletedAsyncResult<T>(obj.GetValue(), callback, state);
    }

    public IAsyncResult BeginWaitForItem(TimeSpan timeout, AsyncCallback callback, object state)
    {
      lock (this.ThisLock)
      {
        if (this.queueState == InputQueue<T>.QueueState.Open)
        {
          if (!this.itemQueue.HasAvailableItem)
          {
            InputQueue<T>.AsyncQueueWaiter asyncQueueWaiter = new InputQueue<T>.AsyncQueueWaiter(timeout, callback, state);
            this.waiterList.Add((InputQueue<T>.IQueueWaiter) asyncQueueWaiter);
            return (IAsyncResult) asyncQueueWaiter;
          }
        }
        else if (this.queueState == InputQueue<T>.QueueState.Shutdown)
        {
          if (!this.itemQueue.HasAvailableItem)
          {
            if (this.itemQueue.HasAnyItem)
            {
              InputQueue<T>.AsyncQueueWaiter asyncQueueWaiter = new InputQueue<T>.AsyncQueueWaiter(timeout, callback, state);
              this.waiterList.Add((InputQueue<T>.IQueueWaiter) asyncQueueWaiter);
              return (IAsyncResult) asyncQueueWaiter;
            }
          }
        }
      }
      return (IAsyncResult) new CompletedAsyncResult<bool>(true, callback, state);
    }

    public void Close() => this.Dispose();

    public T Dequeue(TimeSpan timeout)
    {
      T obj;
      if (!this.Dequeue(timeout, out obj))
        throw Fx.Exception.AsInformation((Exception) new TimeoutException(SRCore.TimeoutInputQueueDequeue((object) timeout)));
      return obj;
    }

    public bool Dequeue(TimeSpan timeout, out T value)
    {
      InputQueue<T>.WaitQueueReader waitQueueReader = (InputQueue<T>.WaitQueueReader) null;
      InputQueue<T>.Item obj = new InputQueue<T>.Item();
      lock (this.ThisLock)
      {
        if (this.queueState == InputQueue<T>.QueueState.Open)
        {
          if (this.itemQueue.HasAvailableItem)
          {
            obj = this.itemQueue.DequeueAvailableItem();
          }
          else
          {
            waitQueueReader = new InputQueue<T>.WaitQueueReader(this);
            this.readerQueue.Enqueue((InputQueue<T>.IQueueReader) waitQueueReader);
          }
        }
        else if (this.queueState == InputQueue<T>.QueueState.Shutdown)
        {
          if (this.itemQueue.HasAvailableItem)
            obj = this.itemQueue.DequeueAvailableItem();
          else if (this.itemQueue.HasAnyItem)
          {
            waitQueueReader = new InputQueue<T>.WaitQueueReader(this);
            this.readerQueue.Enqueue((InputQueue<T>.IQueueReader) waitQueueReader);
          }
          else
          {
            value = default (T);
            return true;
          }
        }
        else
        {
          value = default (T);
          return true;
        }
      }
      if (waitQueueReader != null)
        return waitQueueReader.Wait(timeout, out value);
      InputQueue<T>.InvokeDequeuedCallback(obj.DequeuedCallback);
      value = obj.GetValue();
      return true;
    }

    public void Dispatch()
    {
      InputQueue<T>.IQueueReader queueReader = (InputQueue<T>.IQueueReader) null;
      InputQueue<T>.Item obj = new InputQueue<T>.Item();
      InputQueue<T>.IQueueReader[] queueReaderArray = (InputQueue<T>.IQueueReader[]) null;
      InputQueue<T>.IQueueWaiter[] waiters = (InputQueue<T>.IQueueWaiter[]) null;
      bool itemAvailable = true;
      lock (this.ThisLock)
      {
        itemAvailable = this.queueState != InputQueue<T>.QueueState.Closed && this.queueState != InputQueue<T>.QueueState.Shutdown;
        this.GetWaiters(out waiters);
        if (this.queueState != InputQueue<T>.QueueState.Closed)
        {
          this.itemQueue.MakePendingItemAvailable();
          if (this.readerQueue.Count > 0)
          {
            obj = this.itemQueue.DequeueAvailableItem();
            queueReader = this.readerQueue.Dequeue();
            if (this.queueState == InputQueue<T>.QueueState.Shutdown)
            {
              if (this.readerQueue.Count > 0)
              {
                if (this.itemQueue.ItemCount == 0)
                {
                  queueReaderArray = new InputQueue<T>.IQueueReader[this.readerQueue.Count];
                  this.readerQueue.CopyTo(queueReaderArray, 0);
                  this.readerQueue.Clear();
                  itemAvailable = false;
                }
              }
            }
          }
        }
      }
      if (queueReaderArray != null)
      {
        if (InputQueue<T>.completeOutstandingReadersCallback == null)
          InputQueue<T>.completeOutstandingReadersCallback = new Action<object>(InputQueue<T>.CompleteOutstandingReadersCallback);
        ActionItem.Schedule(InputQueue<T>.completeOutstandingReadersCallback, (object) queueReaderArray);
      }
      if (waiters != null)
        InputQueue<T>.CompleteWaitersLater(itemAvailable, waiters);
      if (queueReader == null)
        return;
      InputQueue<T>.InvokeDequeuedCallback(obj.DequeuedCallback);
      queueReader.Set(obj);
    }

    public bool EndDequeue(IAsyncResult result, out T value)
    {
      if (!(result is CompletedAsyncResult<T>))
        return InputQueue<T>.AsyncQueueReader.End(result, out value);
      value = CompletedAsyncResult<T>.End(result);
      return true;
    }

    public T EndDequeue(IAsyncResult result)
    {
      T obj;
      if (!this.EndDequeue(result, out obj))
        throw Fx.Exception.AsInformation((Exception) new TimeoutException());
      return obj;
    }

    public bool EndWaitForItem(IAsyncResult result) => result is CompletedAsyncResult<bool> ? CompletedAsyncResult<bool>.End(result) : InputQueue<T>.AsyncQueueWaiter.End(result);

    public void EnqueueAndDispatch(T item) => this.EnqueueAndDispatch(item, (Action) null);

    public void EnqueueAndDispatch(T item, Action dequeuedCallback) => this.EnqueueAndDispatch(item, dequeuedCallback, true);

    public void EnqueueAndDispatch(
      Exception exception,
      Action dequeuedCallback,
      bool canDispatchOnThisThread)
    {
      this.EnqueueAndDispatch(new InputQueue<T>.Item(exception, dequeuedCallback), canDispatchOnThisThread);
    }

    public void EnqueueAndDispatch(T item, Action dequeuedCallback, bool canDispatchOnThisThread) => this.EnqueueAndDispatch(new InputQueue<T>.Item(item, dequeuedCallback), canDispatchOnThisThread);

    public bool EnqueueWithoutDispatch(T item, Action dequeuedCallback) => this.EnqueueWithoutDispatch(new InputQueue<T>.Item(item, dequeuedCallback));

    public bool EnqueueWithoutDispatch(Exception exception, Action dequeuedCallback) => this.EnqueueWithoutDispatch(new InputQueue<T>.Item(exception, dequeuedCallback));

    public void Shutdown() => this.Shutdown((Func<Exception>) null);

    public void Shutdown(Func<Exception> pendingExceptionGenerator)
    {
      InputQueue<T>.IQueueReader[] array = (InputQueue<T>.IQueueReader[]) null;
      lock (this.ThisLock)
      {
        if (this.queueState == InputQueue<T>.QueueState.Shutdown || this.queueState == InputQueue<T>.QueueState.Closed)
          return;
        this.queueState = InputQueue<T>.QueueState.Shutdown;
        if (this.readerQueue.Count > 0)
        {
          if (this.itemQueue.ItemCount == 0)
          {
            array = new InputQueue<T>.IQueueReader[this.readerQueue.Count];
            this.readerQueue.CopyTo(array, 0);
            this.readerQueue.Clear();
          }
        }
      }
      if (array == null)
        return;
      for (int index = 0; index < array.Length; ++index)
      {
        Exception exception = pendingExceptionGenerator != null ? pendingExceptionGenerator() : (Exception) null;
        array[index].Set(new InputQueue<T>.Item(exception, (Action) null));
      }
    }

    public bool WaitForItem(TimeSpan timeout)
    {
      InputQueue<T>.WaitQueueWaiter waitQueueWaiter = (InputQueue<T>.WaitQueueWaiter) null;
      bool flag = false;
      lock (this.ThisLock)
      {
        if (this.queueState == InputQueue<T>.QueueState.Open)
        {
          if (this.itemQueue.HasAvailableItem)
          {
            flag = true;
          }
          else
          {
            waitQueueWaiter = new InputQueue<T>.WaitQueueWaiter();
            this.waiterList.Add((InputQueue<T>.IQueueWaiter) waitQueueWaiter);
          }
        }
        else
        {
          if (this.queueState != InputQueue<T>.QueueState.Shutdown)
            return true;
          if (this.itemQueue.HasAvailableItem)
          {
            flag = true;
          }
          else
          {
            if (!this.itemQueue.HasAnyItem)
              return true;
            waitQueueWaiter = new InputQueue<T>.WaitQueueWaiter();
            this.waiterList.Add((InputQueue<T>.IQueueWaiter) waitQueueWaiter);
          }
        }
      }
      return waitQueueWaiter != null ? waitQueueWaiter.Wait(timeout) : flag;
    }

    public void Dispose()
    {
      bool flag = false;
      lock (this.ThisLock)
      {
        if (this.queueState != InputQueue<T>.QueueState.Closed)
        {
          this.queueState = InputQueue<T>.QueueState.Closed;
          flag = true;
        }
      }
      if (!flag)
        return;
      while (this.readerQueue.Count > 0)
        this.readerQueue.Dequeue().Set(new InputQueue<T>.Item());
      while (this.itemQueue.HasAnyItem)
      {
        InputQueue<T>.Item obj = this.itemQueue.DequeueAnyItem();
        this.DisposeItem(obj);
        InputQueue<T>.InvokeDequeuedCallback(obj.DequeuedCallback);
      }
    }

    private void DisposeItem(InputQueue<T>.Item item)
    {
      T obj = item.Value;
      if ((object) obj == null)
        return;
      if (obj is IDisposable disposable)
      {
        disposable.Dispose();
      }
      else
      {
        Action<T> disposeItemCallback = this.DisposeItemCallback;
        if (disposeItemCallback == null)
          return;
        disposeItemCallback(obj);
      }
    }

    private static void CompleteOutstandingReadersCallback(object state)
    {
      foreach (InputQueue<T>.IQueueReader queueReader in (InputQueue<T>.IQueueReader[]) state)
        queueReader.Set(new InputQueue<T>.Item());
    }

    private static void CompleteWaiters(bool itemAvailable, InputQueue<T>.IQueueWaiter[] waiters)
    {
      for (int index = 0; index < waiters.Length; ++index)
        waiters[index].Set(itemAvailable);
    }

    private static void CompleteWaitersFalseCallback(object state) => InputQueue<T>.CompleteWaiters(false, (InputQueue<T>.IQueueWaiter[]) state);

    private static void CompleteWaitersLater(
      bool itemAvailable,
      InputQueue<T>.IQueueWaiter[] waiters)
    {
      if (itemAvailable)
      {
        if (InputQueue<T>.completeWaitersTrueCallback == null)
          InputQueue<T>.completeWaitersTrueCallback = new Action<object>(InputQueue<T>.CompleteWaitersTrueCallback);
        ActionItem.Schedule(InputQueue<T>.completeWaitersTrueCallback, (object) waiters);
      }
      else
      {
        if (InputQueue<T>.completeWaitersFalseCallback == null)
          InputQueue<T>.completeWaitersFalseCallback = new Action<object>(InputQueue<T>.CompleteWaitersFalseCallback);
        ActionItem.Schedule(InputQueue<T>.completeWaitersFalseCallback, (object) waiters);
      }
    }

    private static void CompleteWaitersTrueCallback(object state) => InputQueue<T>.CompleteWaiters(true, (InputQueue<T>.IQueueWaiter[]) state);

    private static void InvokeDequeuedCallback(Action dequeuedCallback)
    {
      if (dequeuedCallback == null)
        return;
      dequeuedCallback();
    }

    private static void InvokeDequeuedCallbackLater(Action dequeuedCallback)
    {
      if (dequeuedCallback == null)
        return;
      if (InputQueue<T>.onInvokeDequeuedCallback == null)
        InputQueue<T>.onInvokeDequeuedCallback = new Action<object>(InputQueue<T>.OnInvokeDequeuedCallback);
      ActionItem.Schedule(InputQueue<T>.onInvokeDequeuedCallback, (object) dequeuedCallback);
    }

    private static void OnDispatchCallback(object state) => ((InputQueue<T>) state).Dispatch();

    private static void OnInvokeDequeuedCallback(object state) => ((Action) state)();

    private void EnqueueAndDispatch(InputQueue<T>.Item item, bool canDispatchOnThisThread)
    {
      bool flag1 = false;
      InputQueue<T>.IQueueReader queueReader = (InputQueue<T>.IQueueReader) null;
      bool flag2 = false;
      InputQueue<T>.IQueueWaiter[] waiters = (InputQueue<T>.IQueueWaiter[]) null;
      bool itemAvailable = true;
      lock (this.ThisLock)
      {
        itemAvailable = this.queueState != InputQueue<T>.QueueState.Closed && this.queueState != InputQueue<T>.QueueState.Shutdown;
        this.GetWaiters(out waiters);
        if (this.queueState == InputQueue<T>.QueueState.Open)
        {
          if (canDispatchOnThisThread)
          {
            if (this.readerQueue.Count == 0)
              this.itemQueue.EnqueueAvailableItem(item);
            else
              queueReader = this.readerQueue.Dequeue();
          }
          else if (this.readerQueue.Count == 0)
          {
            this.itemQueue.EnqueueAvailableItem(item);
          }
          else
          {
            this.itemQueue.EnqueuePendingItem(item);
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
        if (InputQueue<T>.onDispatchCallback == null)
          InputQueue<T>.onDispatchCallback = new Action<object>(InputQueue<T>.OnDispatchCallback);
        ActionItem.Schedule(InputQueue<T>.onDispatchCallback, (object) this);
      }
      else
      {
        if (!flag1)
          return;
        InputQueue<T>.InvokeDequeuedCallback(item.DequeuedCallback);
        this.DisposeItem(item);
      }
    }

    private bool EnqueueWithoutDispatch(InputQueue<T>.Item item)
    {
      lock (this.ThisLock)
      {
        if (this.queueState != InputQueue<T>.QueueState.Closed)
        {
          if (this.queueState != InputQueue<T>.QueueState.Shutdown)
          {
            if (this.readerQueue.Count == 0 && this.waiterList.Count == 0)
            {
              this.itemQueue.EnqueueAvailableItem(item);
              return false;
            }
            this.itemQueue.EnqueuePendingItem(item);
            return true;
          }
        }
      }
      this.DisposeItem(item);
      InputQueue<T>.InvokeDequeuedCallbackLater(item.DequeuedCallback);
      return false;
    }

    private void GetWaiters(out InputQueue<T>.IQueueWaiter[] waiters)
    {
      if (this.waiterList.Count > 0)
      {
        waiters = this.waiterList.ToArray();
        this.waiterList.Clear();
      }
      else
        waiters = (InputQueue<T>.IQueueWaiter[]) null;
    }

    private bool RemoveReader(InputQueue<T>.IQueueReader reader)
    {
      lock (this.ThisLock)
      {
        if (this.queueState != InputQueue<T>.QueueState.Open)
        {
          if (this.queueState != InputQueue<T>.QueueState.Shutdown)
            goto label_13;
        }
        bool flag = false;
        for (int count = this.readerQueue.Count; count > 0; --count)
        {
          InputQueue<T>.IQueueReader queueReader = this.readerQueue.Dequeue();
          if (queueReader == reader)
            flag = true;
          else
            this.readerQueue.Enqueue(queueReader);
        }
        return flag;
      }
label_13:
      return false;
    }

    private enum QueueState
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

    private struct Item
    {
      private Action dequeuedCallback;
      private Exception exception;
      private T value;

      public Item(T value, Action dequeuedCallback)
        : this(value, (Exception) null, dequeuedCallback)
      {
      }

      public Item(Exception exception, Action dequeuedCallback)
        : this(default (T), exception, dequeuedCallback)
      {
      }

      private Item(T value, Exception exception, Action dequeuedCallback)
      {
        this.value = value;
        this.exception = exception;
        this.dequeuedCallback = dequeuedCallback;
      }

      public Action DequeuedCallback => this.dequeuedCallback;

      public Exception Exception => this.exception;

      public T Value => this.value;

      public T GetValue()
      {
        if (this.exception != null)
          throw Fx.Exception.AsInformation(this.exception);
        return this.value;
      }
    }

    private class AsyncQueueReader : AsyncResult, InputQueue<T>.IQueueReader
    {
      private static Action<object> timerCallback = new Action<object>(InputQueue<T>.AsyncQueueReader.TimerCallback);
      private bool expired;
      private InputQueue<T> inputQueue;
      private T item;
      private IOThreadTimer timer;

      public AsyncQueueReader(
        InputQueue<T> inputQueue,
        TimeSpan timeout,
        AsyncCallback callback,
        object state)
        : base(callback, state)
      {
        if (inputQueue.AsyncCallbackGenerator != null)
          this.VirtualCallback = new Action<AsyncCallback, IAsyncResult>(inputQueue.AsyncCallbackGenerator().Invoke);
        this.inputQueue = inputQueue;
        if (!(timeout != TimeSpan.MaxValue))
          return;
        this.timer = new IOThreadTimer(InputQueue<T>.AsyncQueueReader.timerCallback, (object) this, false);
        this.timer.Set(timeout);
      }

      public static bool End(IAsyncResult result, out T value)
      {
        InputQueue<T>.AsyncQueueReader asyncQueueReader = AsyncResult.End<InputQueue<T>.AsyncQueueReader>(result);
        if (asyncQueueReader.expired)
        {
          value = default (T);
          return false;
        }
        value = asyncQueueReader.item;
        return true;
      }

      public void Set(InputQueue<T>.Item inputItem)
      {
        this.item = inputItem.Value;
        if (this.timer != null)
          this.timer.Cancel();
        this.Complete(false, inputItem.Exception);
      }

      private static void TimerCallback(object state)
      {
        InputQueue<T>.AsyncQueueReader reader = (InputQueue<T>.AsyncQueueReader) state;
        if (!reader.inputQueue.RemoveReader((InputQueue<T>.IQueueReader) reader))
          return;
        reader.expired = true;
        reader.Complete(false);
      }
    }

    private class AsyncQueueWaiter : AsyncResult, InputQueue<T>.IQueueWaiter
    {
      private static Action<object> timerCallback = new Action<object>(InputQueue<T>.AsyncQueueWaiter.TimerCallback);
      private bool itemAvailable;
      private IOThreadTimer timer;

      public AsyncQueueWaiter(TimeSpan timeout, AsyncCallback callback, object state)
        : base(callback, state)
      {
        if (!(timeout != TimeSpan.MaxValue))
          return;
        this.timer = new IOThreadTimer(InputQueue<T>.AsyncQueueWaiter.timerCallback, (object) this, false);
        this.timer.Set(timeout);
      }

      public static bool End(IAsyncResult result) => AsyncResult.End<InputQueue<T>.AsyncQueueWaiter>(result).itemAvailable;

      public void Set(bool currentItemAvailable)
      {
        bool flag;
        lock (this.ThisLock)
        {
          flag = this.timer == null || this.timer.Cancel();
          this.itemAvailable = currentItemAvailable;
        }
        if (!flag)
          return;
        this.Complete(false);
      }

      private static void TimerCallback(object state) => ((AsyncResult) state).Complete(false);
    }

    private class ItemQueue
    {
      private int head;
      private InputQueue<T>.Item[] items;
      private int pendingCount;
      private int totalCount;

      public ItemQueue() => this.items = new InputQueue<T>.Item[1];

      public bool HasAnyItem => this.totalCount > 0;

      public bool HasAvailableItem => this.totalCount > this.pendingCount;

      public int ItemCount => this.totalCount;

      public InputQueue<T>.Item DequeueAnyItem()
      {
        if (this.pendingCount == this.totalCount)
          --this.pendingCount;
        return this.DequeueItemCore();
      }

      public InputQueue<T>.Item DequeueAvailableItem()
      {
        Fx.AssertAndThrow(this.totalCount != this.pendingCount, "ItemQueue does not contain any available items");
        return this.DequeueItemCore();
      }

      public void EnqueueAvailableItem(InputQueue<T>.Item item) => this.EnqueueItemCore(item);

      public void EnqueuePendingItem(InputQueue<T>.Item item)
      {
        this.EnqueueItemCore(item);
        ++this.pendingCount;
      }

      public void MakePendingItemAvailable()
      {
        Fx.AssertAndThrow(this.pendingCount != 0, "ItemQueue does not contain any pending items");
        --this.pendingCount;
      }

      private InputQueue<T>.Item DequeueItemCore()
      {
        Fx.AssertAndThrow(this.totalCount != 0, "ItemQueue does not contain any items");
        InputQueue<T>.Item obj = this.items[this.head];
        this.items[this.head] = new InputQueue<T>.Item();
        --this.totalCount;
        this.head = (this.head + 1) % this.items.Length;
        return obj;
      }

      private void EnqueueItemCore(InputQueue<T>.Item item)
      {
        if (this.totalCount == this.items.Length)
        {
          InputQueue<T>.Item[] objArray = new InputQueue<T>.Item[this.items.Length * 2];
          for (int index = 0; index < this.totalCount; ++index)
            objArray[index] = this.items[(this.head + index) % this.items.Length];
          this.head = 0;
          this.items = objArray;
        }
        this.items[(this.head + this.totalCount) % this.items.Length] = item;
        ++this.totalCount;
      }
    }

    private class WaitQueueReader : InputQueue<T>.IQueueReader, IDisposable
    {
      private Exception exception;
      private InputQueue<T> inputQueue;
      private T item;
      private ManualResetEvent waitEvent;

      public WaitQueueReader(InputQueue<T> inputQueue)
      {
        this.inputQueue = inputQueue;
        this.waitEvent = new ManualResetEvent(false);
      }

      public void Set(InputQueue<T>.Item newItem)
      {
        lock (this)
        {
          this.exception = newItem.Exception;
          this.item = newItem.Value;
          this.waitEvent.Set();
        }
      }

      public bool Wait(TimeSpan timeout, out T value)
      {
        bool flag = false;
        try
        {
          if (!TimeoutHelper.WaitOne((WaitHandle) this.waitEvent, timeout))
          {
            if (this.inputQueue.RemoveReader((InputQueue<T>.IQueueReader) this))
            {
              value = default (T);
              flag = true;
              return false;
            }
            this.waitEvent.WaitOne();
          }
          flag = true;
        }
        finally
        {
          if (flag)
            this.waitEvent.Close();
        }
        if (this.exception != null)
          throw Fx.Exception.AsInformation(this.exception);
        value = this.item;
        return true;
      }

      public void Dispose()
      {
        this.waitEvent.Dispose();
        GC.SuppressFinalize((object) this);
      }
    }

    private class WaitQueueWaiter : InputQueue<T>.IQueueWaiter, IDisposable
    {
      private bool itemAvailable;
      private ManualResetEvent waitEvent;

      public WaitQueueWaiter() => this.waitEvent = new ManualResetEvent(false);

      public void Set(bool isItemAvailable)
      {
        lock (this)
        {
          this.itemAvailable = isItemAvailable;
          this.waitEvent.Set();
        }
      }

      public bool Wait(TimeSpan timeout) => TimeoutHelper.WaitOne((WaitHandle) this.waitEvent, timeout) && this.itemAvailable;

      public void Dispose()
      {
        this.waitEvent.Close();
        GC.SuppressFinalize((object) this);
      }
    }
  }
}
