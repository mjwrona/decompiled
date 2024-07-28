// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.OrderedLock
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Threading;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public sealed class OrderedLock : IDisposable
  {
    private bool m_disposed;
    private long m_callerCount;
    private ManualResetEvent m_waitEvent = new ManualResetEvent(true);
    private ReaderWriterLock m_waiterAccessLock = new ReaderWriterLock();
    private SortedList<long, OrderedLock.Position> m_currentWaiters = new SortedList<long, OrderedLock.Position>();

    public void Dispose()
    {
      try
      {
        this.m_waiterAccessLock.AcquireWriterLock(-1);
        this.m_disposed = true;
        if (this.m_waitEvent != null)
        {
          this.m_waitEvent.Set();
          this.m_waitEvent.Dispose();
          this.m_waitEvent = (ManualResetEvent) null;
        }
        this.m_currentWaiters.Clear();
      }
      finally
      {
        if (this.m_waiterAccessLock.IsWriterLockHeld)
          this.m_waiterAccessLock.ReleaseWriterLock();
      }
    }

    public OrderedLock.Position SavePosition()
    {
      try
      {
        this.m_waiterAccessLock.AcquireWriterLock(-1);
        if (this.m_disposed)
          throw new ObjectDisposedException(nameof (OrderedLock));
        OrderedLock.Position position = new OrderedLock.Position(this, ++this.m_callerCount);
        this.m_currentWaiters.Add(position.Id, position);
        return position;
      }
      finally
      {
        if (this.m_waiterAccessLock.IsWriterLockHeld)
          this.m_waiterAccessLock.ReleaseWriterLock();
      }
    }

    public sealed class Position : IDisposable
    {
      private OrderedLock m_owner;

      internal Position(OrderedLock owner, long position)
      {
        this.Id = position;
        this.m_owner = owner;
      }

      public long Id { get; private set; }

      public void Dispose()
      {
        try
        {
          this.m_owner.m_waiterAccessLock.AcquireWriterLock(-1);
          if (this.m_owner.m_currentWaiters.Count > 0 && this.m_owner.m_currentWaiters.Values[0].Id == this.Id)
            this.m_owner.m_waitEvent.Set();
          this.m_owner.m_currentWaiters.Remove(this.Id);
        }
        finally
        {
          if (this.m_owner.m_waiterAccessLock.IsWriterLockHeld)
            this.m_owner.m_waiterAccessLock.ReleaseWriterLock();
        }
      }

      public void Wait()
      {
        while (true)
        {
          try
          {
            this.m_owner.m_waiterAccessLock.AcquireReaderLock(-1);
            if (this.m_owner.m_disposed)
              throw new ObjectDisposedException("OrderedLock.Position");
            if (this.m_owner.m_currentWaiters.Values[0].Id == this.Id)
            {
              this.m_owner.m_waitEvent.Reset();
              break;
            }
          }
          finally
          {
            if (this.m_owner.m_waiterAccessLock.IsReaderLockHeld || this.m_owner.m_waiterAccessLock.IsWriterLockHeld)
              this.m_owner.m_waiterAccessLock.ReleaseReaderLock();
          }
          this.m_owner.m_waitEvent.WaitOne(-1, false);
        }
      }
    }
  }
}
