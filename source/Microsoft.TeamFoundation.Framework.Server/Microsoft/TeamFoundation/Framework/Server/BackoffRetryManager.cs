// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.BackoffRetryManager
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Linq;
using System.Threading;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public sealed class BackoffRetryManager
  {
    private readonly TimeSpan[] m_retryDelays;
    private readonly BackoffRetryManager.OnExceptionHandler m_onException;

    public BackoffRetryManager(
      TimeSpan[] retryDelays,
      BackoffRetryManager.OnExceptionHandler onException = null)
    {
      this.m_retryDelays = retryDelays;
      this.m_onException = onException;
    }

    public static TimeSpan[] ConstantDelay(int retryCount, TimeSpan retryDelay)
    {
      ArgumentUtility.CheckForOutOfRange(retryCount, nameof (retryCount), 0);
      return Enumerable.Range(0, retryCount).Select<int, TimeSpan>((Func<int, TimeSpan>) (i => retryDelay)).ToArray<TimeSpan>();
    }

    public static TimeSpan[] ExponentialDelay(int retryCount, TimeSpan maxDelayPerRetry)
    {
      ArgumentUtility.CheckForOutOfRange(retryCount, nameof (retryCount), 0);
      return Enumerable.Range(0, retryCount).Select<int, TimeSpan>((Func<int, TimeSpan>) (i =>
      {
        int num = i == 0 ? 0 : (i <= 16 ? 1 << i - 1 : 65536);
        return (double) num > maxDelayPerRetry.TotalSeconds ? maxDelayPerRetry : TimeSpan.FromSeconds((double) num);
      })).ToArray<TimeSpan>();
    }

    public void Invoke(Action action)
    {
      ArgumentUtility.CheckForNull<Action>(action, nameof (action));
      int currentRetryCount = 0;
      int num = 0;
      while (true)
      {
        try
        {
          action();
          break;
        }
        catch (Exception ex)
        {
          if (currentRetryCount < this.m_retryDelays.Length && this.m_retryDelays[currentRetryCount] != TimeSpan.Zero)
            Thread.Sleep(this.m_retryDelays[currentRetryCount]);
          BackoffRetryManager.RetryContext context = (BackoffRetryManager.RetryContext) null;
          if (this.m_onException != null)
          {
            context = new BackoffRetryManager.RetryContext(ex, currentRetryCount, this.m_retryDelays.Length - currentRetryCount);
            if (!this.m_onException(context))
              throw;
          }
          if (currentRetryCount >= this.m_retryDelays.Length)
            throw;
          else if (context != null && context.ShouldResetRetryCount)
          {
            currentRetryCount = 0;
            ++num;
            if (num > 10)
              throw new InvalidOperationException(string.Format("Excessive number of retry count resets. Retry count has been reset {0} times", (object) num), ex);
          }
          else
            ++currentRetryCount;
        }
      }
    }

    public delegate bool OnExceptionHandler(BackoffRetryManager.RetryContext context);

    public class RetryContext
    {
      public Exception Exception { get; private set; }

      public int CurrentRetryCount { get; private set; }

      public bool ShouldResetRetryCount { get; private set; }

      public int RemainingRetries { get; private set; }

      public void ResetCurrentRetryCount() => this.ShouldResetRetryCount = true;

      public RetryContext(Exception exception, int currentRetryCount, int remainingRetries)
      {
        this.Exception = exception;
        this.CurrentRetryCount = currentRetryCount;
        this.RemainingRetries = remainingRetries;
        this.ShouldResetRetryCount = false;
      }
    }
  }
}
