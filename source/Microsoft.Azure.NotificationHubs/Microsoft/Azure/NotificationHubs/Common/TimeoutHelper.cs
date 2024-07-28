// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Common.TimeoutHelper
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using System;
using System.Diagnostics;
using System.Threading;

namespace Microsoft.Azure.NotificationHubs.Common
{
  [DebuggerStepThrough]
  internal struct TimeoutHelper
  {
    private DateTime deadline;
    private bool deadlineSet;
    private TimeSpan originalTimeout;
    public static readonly TimeSpan MaxWait = TimeSpan.FromMilliseconds((double) int.MaxValue);

    public TimeoutHelper(TimeSpan timeout)
      : this(timeout, false)
    {
    }

    public TimeoutHelper(TimeSpan timeout, bool startTimeout)
    {
      this.originalTimeout = timeout;
      this.deadline = DateTime.MaxValue;
      this.deadlineSet = timeout == TimeSpan.MaxValue;
      if (!startTimeout || this.deadlineSet)
        return;
      this.SetDeadline();
    }

    public TimeSpan OriginalTimeout => this.originalTimeout;

    public static bool IsTooLarge(TimeSpan timeout) => timeout > TimeoutHelper.MaxWait && timeout != TimeSpan.MaxValue;

    public static TimeSpan FromMilliseconds(int milliseconds) => milliseconds == -1 ? TimeSpan.MaxValue : TimeSpan.FromMilliseconds((double) milliseconds);

    public static int ToMilliseconds(TimeSpan timeout)
    {
      if (timeout == TimeSpan.MaxValue)
        return -1;
      long ticks = Ticks.FromTimeSpan(timeout);
      return ticks / 10000L > (long) int.MaxValue ? int.MaxValue : Ticks.ToMilliseconds(ticks);
    }

    public static TimeSpan Min(TimeSpan val1, TimeSpan val2) => val1 > val2 ? val2 : val1;

    public static DateTime Min(DateTime val1, DateTime val2) => val1 > val2 ? val2 : val1;

    public static TimeSpan Add(TimeSpan timeout1, TimeSpan timeout2) => Ticks.ToTimeSpan(Ticks.Add(Ticks.FromTimeSpan(timeout1), Ticks.FromTimeSpan(timeout2)));

    public static DateTime Add(DateTime time, TimeSpan timeout)
    {
      if (timeout >= TimeSpan.Zero && DateTime.MaxValue - time <= timeout)
        return DateTime.MaxValue;
      return timeout <= TimeSpan.Zero && DateTime.MinValue - time >= timeout ? DateTime.MinValue : time + timeout;
    }

    public static DateTime Subtract(DateTime time, TimeSpan timeout) => TimeoutHelper.Add(time, TimeSpan.Zero - timeout);

    public static TimeSpan Divide(TimeSpan timeout, int factor) => timeout == TimeSpan.MaxValue ? TimeSpan.MaxValue : Ticks.ToTimeSpan(Ticks.FromTimeSpan(timeout) / (long) factor + 1L);

    public TimeSpan RemainingTime()
    {
      if (!this.deadlineSet)
      {
        this.SetDeadline();
        return this.originalTimeout;
      }
      if (this.deadline == DateTime.MaxValue)
        return TimeSpan.MaxValue;
      TimeSpan timeSpan = this.deadline - DateTime.UtcNow;
      return timeSpan <= TimeSpan.Zero ? TimeSpan.Zero : timeSpan;
    }

    public TimeSpan ElapsedTime() => this.originalTimeout - this.RemainingTime();

    private void SetDeadline()
    {
      this.deadline = DateTime.UtcNow + this.originalTimeout;
      this.deadlineSet = true;
    }

    public static void ThrowIfNegativeArgument(TimeSpan timeout) => TimeoutHelper.ThrowIfNegativeArgument(timeout, nameof (timeout));

    public static void ThrowIfNegativeArgument(TimeSpan timeout, string argumentName)
    {
      if (timeout < TimeSpan.Zero)
        throw Fx.Exception.ArgumentOutOfRange(argumentName, (object) timeout, SRCore.TimeoutMustBeNonNegative((object) argumentName, (object) timeout));
    }

    public static void ThrowIfNonPositiveArgument(TimeSpan timeout) => TimeoutHelper.ThrowIfNonPositiveArgument(timeout, nameof (timeout));

    public static void ThrowIfNonPositiveArgument(TimeSpan timeout, string argumentName)
    {
      if (timeout <= TimeSpan.Zero)
        throw Fx.Exception.ArgumentOutOfRange(argumentName, (object) timeout, SRCore.TimeoutMustBePositive((object) argumentName, (object) timeout));
    }

    public static bool WaitOne(WaitHandle waitHandle, TimeSpan timeout)
    {
      TimeoutHelper.ThrowIfNegativeArgument(timeout);
      if (!(timeout == TimeSpan.MaxValue))
        return waitHandle.WaitOne(timeout, false);
      waitHandle.WaitOne();
      return true;
    }
  }
}
