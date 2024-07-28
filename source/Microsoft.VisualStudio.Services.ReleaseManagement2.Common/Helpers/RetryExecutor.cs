// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Common.Helpers.RetryExecutor
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C3F75541-7C8A-4AF6-A47E-709CEEE7550D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Common.dll

using System;
using System.Threading;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Common.Helpers
{
  public class RetryExecutor
  {
    private const int DefaultMaximumRetryCount = 5;
    private const int DefaultMillisecondsToSleepBetweenRetries = 1000;

    public int MaximumRetryCount { get; set; }

    public int MillisecondsToSleepBetweenRetries { get; set; }

    public Func<Exception, bool> ShouldRetryAction { get; set; }

    protected Action<int> SleepAction { get; set; }

    public RetryExecutor()
    {
      this.MaximumRetryCount = 5;
      this.MillisecondsToSleepBetweenRetries = 1000;
      this.ShouldRetryAction = (Func<Exception, bool>) (ex => true);
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      this.SleepAction = RetryExecutor.\u003C\u003EO.\u003C0\u003E__Sleep ?? (RetryExecutor.\u003C\u003EO.\u003C0\u003E__Sleep = new Action<int>(Thread.Sleep));
    }

    public void Execute(Action action)
    {
      if (action == null)
        throw new ArgumentNullException(nameof (action));
      for (int retryCount = 0; retryCount < this.MaximumRetryCount; ++retryCount)
      {
        try
        {
          action();
          break;
        }
        catch (Exception ex)
        {
          if (this.ShouldThrow(retryCount, ex))
            throw;
          else
            this.SleepAction(this.MillisecondsToSleepBetweenRetries);
        }
      }
    }

    public TResult Execute<TResult>(Func<TResult> action)
    {
      if (action == null)
        throw new ArgumentNullException(nameof (action));
      TResult result = default (TResult);
      for (int retryCount = 0; retryCount < this.MaximumRetryCount; ++retryCount)
      {
        try
        {
          return action();
        }
        catch (Exception ex)
        {
          if (this.ShouldThrow(retryCount, ex))
            throw;
          else
            this.SleepAction(this.MillisecondsToSleepBetweenRetries);
        }
      }
      return result;
    }

    private bool ShouldThrow(int retryCount, Exception ex) => retryCount == this.MaximumRetryCount - 1 || !this.ShouldRetryAction(ex);
  }
}
