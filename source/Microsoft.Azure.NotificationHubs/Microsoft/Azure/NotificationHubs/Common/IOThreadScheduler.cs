// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Common.IOThreadScheduler
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using System;
using System.Security;
using System.Threading;

namespace Microsoft.Azure.NotificationHubs.Common
{
  internal class IOThreadScheduler
  {
    private const int MaximumCapacity = 32768;
    private static IOThreadScheduler current = new IOThreadScheduler(32, 32);
    private readonly IOThreadScheduler.ScheduledOverlapped overlapped;
    [SecurityCritical]
    private readonly IOThreadScheduler.Slot[] slots;
    [SecurityCritical]
    private readonly IOThreadScheduler.Slot[] slotsLowPri;
    private int headTail = -131072;
    private int headTailLowPri = -65536;

    private IOThreadScheduler(int capacity, int capacityLowPri)
    {
      this.slots = new IOThreadScheduler.Slot[capacity];
      this.slotsLowPri = new IOThreadScheduler.Slot[capacityLowPri];
      this.overlapped = new IOThreadScheduler.ScheduledOverlapped();
    }

    [SecurityCritical]
    public static void ScheduleCallbackNoFlow(Action<object> callback, object state)
    {
      if (callback == null)
        throw Fx.Exception.ArgumentNull(nameof (callback));
      bool flag = false;
      while (!flag)
      {
        try
        {
        }
        finally
        {
          flag = IOThreadScheduler.current.ScheduleCallbackHelper(callback, state);
        }
      }
    }

    [SecurityCritical]
    public static void ScheduleCallbackLowPriNoFlow(Action<object> callback, object state)
    {
      if (callback == null)
        throw Fx.Exception.ArgumentNull(nameof (callback));
      bool flag = false;
      while (!flag)
      {
        try
        {
        }
        finally
        {
          flag = IOThreadScheduler.current.ScheduleCallbackLowPriHelper(callback, state);
        }
      }
    }

    [SecurityCritical]
    private bool ScheduleCallbackHelper(Action<object> callback, object state)
    {
      int slot = Interlocked.Add(ref this.headTail, 65536);
      bool flag = IOThreadScheduler.Bits.Count(slot) == 0;
      if (flag)
        slot = Interlocked.Add(ref this.headTail, 65536);
      if (IOThreadScheduler.Bits.Count(slot) == -1)
        throw Fx.AssertAndThrowFatal("Head/Tail overflow!");
      bool wrapped;
      int num = this.slots[slot >> 16 & this.SlotMask].TryEnqueueWorkItem(callback, state, out wrapped) ? 1 : 0;
      if (wrapped)
      {
        IOThreadScheduler ioThreadScheduler = new IOThreadScheduler(Math.Min(this.slots.Length * 2, 32768), this.slotsLowPri.Length);
        Interlocked.CompareExchange<IOThreadScheduler>(ref IOThreadScheduler.current, ioThreadScheduler, this);
      }
      if (!flag)
        return num != 0;
      this.overlapped.Post(this);
      return num != 0;
    }

    [SecurityCritical]
    private bool ScheduleCallbackLowPriHelper(Action<object> callback, object state)
    {
      int slot = Interlocked.Add(ref this.headTailLowPri, 65536);
      bool flag = false;
      if (IOThreadScheduler.Bits.CountNoIdle(slot) == 1)
      {
        int headTail = this.headTail;
        if (IOThreadScheduler.Bits.Count(headTail) == -1)
        {
          int num = Interlocked.CompareExchange(ref this.headTail, headTail + 65536, headTail);
          if (headTail == num)
            flag = true;
        }
      }
      if (IOThreadScheduler.Bits.CountNoIdle(slot) == 0)
        throw Fx.AssertAndThrowFatal("Low-priority Head/Tail overflow!");
      bool wrapped;
      int num1 = this.slotsLowPri[slot >> 16 & this.SlotMaskLowPri].TryEnqueueWorkItem(callback, state, out wrapped) ? 1 : 0;
      if (wrapped)
      {
        IOThreadScheduler ioThreadScheduler = new IOThreadScheduler(this.slots.Length, Math.Min(this.slotsLowPri.Length * 2, 32768));
        Interlocked.CompareExchange<IOThreadScheduler>(ref IOThreadScheduler.current, ioThreadScheduler, this);
      }
      if (!flag)
        return num1 != 0;
      this.overlapped.Post(this);
      return num1 != 0;
    }

    [SecurityCritical]
    private void CompletionCallback(out Action<object> callback, out object state)
    {
      int num1 = this.headTail;
      while (true)
      {
        bool flag;
        do
        {
          flag = IOThreadScheduler.Bits.Count(num1) == 0;
          if (flag)
          {
            int num2 = this.headTailLowPri;
            while (IOThreadScheduler.Bits.CountNoIdle(num2) != 0)
            {
              if (num2 == (num2 = Interlocked.CompareExchange(ref this.headTailLowPri, IOThreadScheduler.Bits.IncrementLo(num2), num2)))
              {
                this.overlapped.Post(this);
                this.slotsLowPri[num2 & this.SlotMaskLowPri].DequeueWorkItem(out callback, out state);
                return;
              }
            }
          }
        }
        while (num1 != (num1 = Interlocked.CompareExchange(ref this.headTail, IOThreadScheduler.Bits.IncrementLo(num1), num1)));
        if (flag)
        {
          if (IOThreadScheduler.Bits.CountNoIdle(this.headTailLowPri) != 0)
          {
            int comparand = IOThreadScheduler.Bits.IncrementLo(num1);
            if (comparand == Interlocked.CompareExchange(ref this.headTail, comparand + 65536, comparand))
              num1 = comparand + 65536;
            else
              goto label_12;
          }
          else
            goto label_12;
        }
        else
          break;
      }
      this.overlapped.Post(this);
      this.slots[num1 & this.SlotMask].DequeueWorkItem(out callback, out state);
      return;
label_12:
      callback = (Action<object>) null;
      state = (object) null;
    }

    [SecurityCritical]
    private bool TryCoalesce(out Action<object> callback, out object state)
    {
      int num1 = this.headTail;
      do
      {
        for (; IOThreadScheduler.Bits.Count(num1) <= 0; num1 = this.headTail)
        {
          int headTailLowPri = this.headTailLowPri;
          if (IOThreadScheduler.Bits.CountNoIdle(headTailLowPri) > 0)
          {
            int num2;
            if (headTailLowPri == (num2 = Interlocked.CompareExchange(ref this.headTailLowPri, IOThreadScheduler.Bits.IncrementLo(headTailLowPri), headTailLowPri)))
            {
              this.slotsLowPri[num2 & this.SlotMaskLowPri].DequeueWorkItem(out callback, out state);
              return true;
            }
          }
          else
          {
            callback = (Action<object>) null;
            state = (object) null;
            return false;
          }
        }
      }
      while (num1 != (num1 = Interlocked.CompareExchange(ref this.headTail, IOThreadScheduler.Bits.IncrementLo(num1), num1)));
      this.slots[num1 & this.SlotMask].DequeueWorkItem(out callback, out state);
      return true;
    }

    private int SlotMask
    {
      [SecurityCritical] get => this.slots.Length - 1;
    }

    private int SlotMaskLowPri
    {
      [SecurityCritical] get => this.slotsLowPri.Length - 1;
    }

    ~IOThreadScheduler()
    {
      if (Environment.HasShutdownStarted || AppDomain.CurrentDomain.IsFinalizingForUnload())
        return;
      this.Cleanup();
    }

    private void Cleanup()
    {
      if (this.overlapped == null)
        return;
      this.overlapped.Cleanup();
    }

    private static class Bits
    {
      public const int HiShift = 16;
      public const int HiOne = 65536;
      public const int LoHiBit = 32768;
      public const int HiHiBit = -2147483648;
      public const int LoCountMask = 32767;
      public const int HiCountMask = 2147418112;
      public const int LoMask = 65535;
      public const int HiMask = -65536;
      public const int HiBits = -2147450880;

      public static int Count(int slot) => ((slot >> 16) - slot + 2 & (int) ushort.MaxValue) - 1;

      public static int CountNoIdle(int slot) => (slot >> 16) - slot + 1 & (int) ushort.MaxValue;

      public static int IncrementLo(int slot) => slot + 1 & (int) ushort.MaxValue | slot & -65536;

      public static bool IsComplete(int gate) => (gate & -65536) == gate << 16;
    }

    private struct Slot
    {
      private int gate;
      private Action<object> heldCallback;
      private object heldState;

      public bool TryEnqueueWorkItem(Action<object> callback, object state, out bool wrapped)
      {
        int num1 = Interlocked.Increment(ref this.gate);
        wrapped = (num1 & (int) short.MaxValue) != 1;
        if (wrapped)
        {
          if ((num1 & 32768) != 0 && IOThreadScheduler.Bits.IsComplete(num1))
            Interlocked.CompareExchange(ref this.gate, 0, num1);
          return false;
        }
        this.heldState = state;
        this.heldCallback = callback;
        int comparand = Interlocked.Add(ref this.gate, 32768);
        if ((comparand & 2147418112) == 0)
          return true;
        this.heldState = (object) null;
        this.heldCallback = (Action<object>) null;
        if (comparand >> 16 != (comparand & (int) short.MaxValue) || Interlocked.CompareExchange(ref this.gate, 0, comparand) != comparand)
        {
          int num2 = Interlocked.Add(ref this.gate, int.MinValue);
          if (IOThreadScheduler.Bits.IsComplete(num2))
            Interlocked.CompareExchange(ref this.gate, 0, num2);
        }
        return false;
      }

      public void DequeueWorkItem(out Action<object> callback, out object state)
      {
        int num1 = Interlocked.Add(ref this.gate, 65536);
        if ((num1 & 32768) == 0)
        {
          callback = (Action<object>) null;
          state = (object) null;
        }
        else if ((num1 & 2147418112) == 65536)
        {
          callback = this.heldCallback;
          state = this.heldState;
          this.heldState = (object) null;
          this.heldCallback = (Action<object>) null;
          if ((num1 & (int) short.MaxValue) == 1 && Interlocked.CompareExchange(ref this.gate, 0, num1) == num1)
            return;
          int num2 = Interlocked.Add(ref this.gate, int.MinValue);
          if (!IOThreadScheduler.Bits.IsComplete(num2))
            return;
          Interlocked.CompareExchange(ref this.gate, 0, num2);
        }
        else
        {
          callback = (Action<object>) null;
          state = (object) null;
          if (!IOThreadScheduler.Bits.IsComplete(num1))
            return;
          Interlocked.CompareExchange(ref this.gate, 0, num1);
        }
      }
    }

    [SecurityCritical]
    private class ScheduledOverlapped
    {
      private readonly unsafe NativeOverlapped* nativeOverlapped;
      private IOThreadScheduler scheduler;

      public unsafe ScheduledOverlapped() => this.nativeOverlapped = new Overlapped().UnsafePack(Fx.ThunkCallback(new IOCompletionCallback(this.IOCallback)), (object) null);

      private unsafe void IOCallback(
        uint errorCode,
        uint numBytes,
        NativeOverlapped* nativeOverlappedCallback)
      {
        IOThreadScheduler scheduler = this.scheduler;
        this.scheduler = (IOThreadScheduler) null;
        Action<object> callback;
        object state;
        try
        {
        }
        finally
        {
          scheduler.CompletionCallback(out callback, out state);
        }
        bool flag = true;
        while (flag)
        {
          if (callback != null)
            callback(state);
          try
          {
          }
          finally
          {
            flag = scheduler.TryCoalesce(out callback, out state);
          }
        }
      }

      public unsafe void Post(IOThreadScheduler iots)
      {
        this.scheduler = iots;
        ThreadPool.UnsafeQueueNativeOverlapped(this.nativeOverlapped);
      }

      public unsafe void Cleanup()
      {
        if (this.scheduler != null)
          throw Fx.AssertAndThrowFatal("Cleanup called on an overlapped that is in-flight.");
        Overlapped.Free(this.nativeOverlapped);
      }
    }
  }
}
