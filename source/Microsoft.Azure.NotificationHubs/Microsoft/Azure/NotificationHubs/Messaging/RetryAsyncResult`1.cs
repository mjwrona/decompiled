// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Messaging.RetryAsyncResult`1
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using System;
using System.Transactions;

namespace Microsoft.Azure.NotificationHubs.Messaging
{
  internal abstract class RetryAsyncResult<T> : IteratorAsyncResult<T> where T : RetryAsyncResult<T>
  {
    protected RetryAsyncResult(TimeSpan timeout, AsyncCallback callback, object state)
      : base(timeout, callback, state)
    {
      this.AmbientTransaction = Transaction.Current;
      this.TransactionExists = Transaction.Current != (Transaction) null;
    }

    protected Transaction AmbientTransaction { get; private set; }

    protected bool TransactionExists { get; private set; }
  }
}
