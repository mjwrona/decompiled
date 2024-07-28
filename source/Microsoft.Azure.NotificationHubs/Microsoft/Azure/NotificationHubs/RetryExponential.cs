// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.RetryExponential
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using Microsoft.Azure.NotificationHubs.Common;
using Microsoft.Azure.NotificationHubs.Messaging;
using Microsoft.Azure.NotificationHubs.Properties;
using System;

namespace Microsoft.Azure.NotificationHubs
{
  public sealed class RetryExponential : RetryPolicy
  {
    public RetryExponential(
      TimeSpan minBackoff,
      TimeSpan maxBackoff,
      TimeSpan deltaBackoff,
      TimeSpan terminationTimeBuffer,
      int maxRetryCount)
    {
      TimeoutHelper.ThrowIfNegativeArgument(minBackoff, nameof (minBackoff));
      TimeoutHelper.ThrowIfNonPositiveArgument(maxBackoff, nameof (maxBackoff));
      TimeoutHelper.ThrowIfNonPositiveArgument(deltaBackoff, nameof (deltaBackoff));
      TimeoutHelper.ThrowIfNonPositiveArgument(terminationTimeBuffer, nameof (terminationTimeBuffer));
      if (maxRetryCount <= 0)
        throw new ArgumentOutOfRangeException(nameof (maxRetryCount), Resources.ValueMustBePositive);
      this.MinimalBackoff = minBackoff;
      this.MaximumBackoff = maxBackoff;
      this.DeltaBackoff = deltaBackoff;
      this.TerminationTimeBuffer = terminationTimeBuffer;
      this.MaxRetryCount = maxRetryCount;
    }

    public TimeSpan MinimalBackoff { get; private set; }

    public TimeSpan MaximumBackoff { get; private set; }

    public TimeSpan DeltaBackoff { get; private set; }

    public TimeSpan TerminationTimeBuffer { get; private set; }

    public int MaxRetryCount { get; private set; }

    protected override bool OnShouldRetry(
      TimeSpan remainingTime,
      int currentRetryCount,
      out TimeSpan retryInterval)
    {
      if (currentRetryCount > this.MaxRetryCount)
      {
        retryInterval = TimeSpan.Zero;
        return false;
      }
      TimeSpan timeSpan = this.DeltaBackoff;
      int minValue = (int) (timeSpan.TotalMilliseconds * 0.8);
      timeSpan = this.DeltaBackoff;
      int maxValue = (int) (timeSpan.TotalMilliseconds * 1.2);
      int num1 = ConcurrentRandom.Next(minValue, maxValue);
      double num2 = (Math.Pow(2.0, (double) currentRetryCount) - 1.0) * (double) num1;
      timeSpan = this.MinimalBackoff;
      double val1 = timeSpan.TotalMilliseconds + num2;
      timeSpan = this.MaximumBackoff;
      double totalMilliseconds = timeSpan.TotalMilliseconds;
      double num3 = Math.Min(val1, totalMilliseconds);
      retryInterval = TimeSpan.FromMilliseconds(num3);
      if (this.IsServerBusy)
        retryInterval += RetryPolicy.ServerBusyBaseSleepTime;
      if (!(retryInterval >= remainingTime - this.TerminationTimeBuffer))
        return true;
      retryInterval = TimeSpan.Zero;
      return false;
    }

    public override RetryPolicy Clone()
    {
      RetryExponential retryExponential = new RetryExponential(this.MinimalBackoff, this.MaximumBackoff, this.DeltaBackoff, this.TerminationTimeBuffer, this.MaxRetryCount);
      if (this.IsServerBusy)
        retryExponential.SetServerBusy(this.ServerBusyExceptionMessage);
      return (RetryPolicy) retryExponential;
    }

    private bool IsRetryableException(MessagingException lastException) => lastException != null ? lastException.IsTransient : throw FxTrace.Exception.ArgumentNull(nameof (lastException));

    protected override bool IsRetryableException(Exception lastException)
    {
      if (lastException == null)
        throw FxTrace.Exception.ArgumentNull(nameof (lastException));
      return lastException is MessagingException lastException1 && this.IsRetryableException(lastException1);
    }
  }
}
