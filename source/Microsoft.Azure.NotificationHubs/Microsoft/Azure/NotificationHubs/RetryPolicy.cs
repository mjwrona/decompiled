// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.RetryPolicy
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using Microsoft.Azure.NotificationHubs.Common;
using Microsoft.Azure.NotificationHubs.Messaging;
using Microsoft.Azure.NotificationHubs.Properties;
using System;
using System.Transactions;

namespace Microsoft.Azure.NotificationHubs
{
  public abstract class RetryPolicy
  {
    internal static readonly TimeSpan ServerBusyBaseSleepTime = TimeSpan.FromSeconds(10.0);
    private object serverBusyLock = new object();
    private volatile bool serverBusy;
    private volatile string serverBusyExceptionMessage;
    private volatile IOThreadTimer serverBusyResetTimer;

    internal RetryPolicy() => this.serverBusyResetTimer = new IOThreadTimer(new Action<object>(RetryPolicy.OnTimerCallback), (object) this, true);

    public static RetryPolicy NoRetry => (RetryPolicy) new Microsoft.Azure.NotificationHubs.NoRetry();

    public static RetryPolicy Default => (RetryPolicy) new RetryExponential(Constants.DefaultRetryMinBackoff, Constants.DefaultRetryMaxBackoff, Constants.DefaultRetryDeltaBackoff, Constants.DefaultRetryTerminationBuffer, Constants.DefaultMaxDeliveryCount);

    internal bool IsServerBusy => this.serverBusy;

    internal string ServerBusyExceptionMessage => this.serverBusyExceptionMessage;

    internal bool ShouldRetry(
      TimeSpan remainingTime,
      int currentRetryCount,
      Exception lastException,
      out TimeSpan retryInterval)
    {
      if (lastException is ServerBusyException)
        this.SetServerBusy(lastException.Message);
      if (Transaction.Current == (Transaction) null)
      {
        if (lastException == null || remainingTime == TimeSpan.Zero)
        {
          retryInterval = TimeSpan.Zero;
          return false;
        }
        if (this.IsRetryableException(lastException))
          return this.OnShouldRetry(remainingTime, currentRetryCount, out retryInterval);
      }
      retryInterval = TimeSpan.Zero;
      return false;
    }

    public abstract RetryPolicy Clone();

    internal void SetServerBusy(string exceptionMessage)
    {
      if (this.serverBusy)
        return;
      lock (this.serverBusyLock)
      {
        if (this.serverBusy)
          return;
        this.serverBusy = true;
        this.serverBusyExceptionMessage = !string.IsNullOrWhiteSpace(exceptionMessage) ? exceptionMessage : SR.GetString(Resources.ServerBusy);
        this.serverBusyResetTimer.Set(RetryPolicy.ServerBusyBaseSleepTime);
      }
    }

    internal void ResetServerBusy()
    {
      if (!this.serverBusy)
        return;
      lock (this.serverBusyLock)
      {
        if (!this.serverBusy)
          return;
        this.serverBusy = false;
        this.serverBusyExceptionMessage = SR.GetString(Resources.ServerBusy);
        this.serverBusyResetTimer.Cancel();
      }
    }

    private static void OnTimerCallback(object state)
    {
      RetryPolicy retryPolicy = (RetryPolicy) state;
      if (!retryPolicy.IsServerBusy)
        return;
      retryPolicy.ResetServerBusy();
    }

    protected abstract bool OnShouldRetry(
      TimeSpan remainingTime,
      int currentRetryCount,
      out TimeSpan retryInterval);

    protected abstract bool IsRetryableException(Exception lastException);
  }
}
