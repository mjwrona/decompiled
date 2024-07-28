// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Orchestration.RetryInterceptor`1
// Assembly: Microsoft.VisualStudio.Services.Orchestration, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C0C603F4-BE31-455B-860A-9FD3B046611C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Orchestration.dll

using System;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Orchestration
{
  public class RetryInterceptor<T>
  {
    private OrchestrationContext context;
    private RetryOptions retryOptions;
    private Func<Task<T>> retryCall;

    public RetryInterceptor(
      OrchestrationContext context,
      RetryOptions retryOptions,
      Func<Task<T>> retryCall)
    {
      this.context = context;
      this.retryOptions = retryOptions;
      this.retryCall = retryCall;
    }

    public async Task<T> Invoke()
    {
      Exception lastException = (Exception) null;
      DateTime firstAttempt = this.context.CurrentUtcDateTime;
      for (int retryCount = 0; retryCount < this.retryOptions.MaxNumberOfAttempts; ++retryCount)
      {
        T obj;
        try
        {
          obj = await this.retryCall();
          goto label_9;
        }
        catch (Exception ex)
        {
          lastException = ex;
        }
        TimeSpan nextDelay = this.ComputeNextDelay(retryCount, firstAttempt, lastException);
        if (!(nextDelay == TimeSpan.Zero))
        {
          string timer = await this.context.CreateTimer<string>(this.context.CurrentUtcDateTime.Add(nextDelay), "Retry Attempt " + retryCount.ToString() + 1.ToString());
          continue;
        }
        break;
label_9:
        lastException = (Exception) null;
        return obj;
      }
      throw lastException;
    }

    private TimeSpan ComputeNextDelay(int attempt, DateTime firstAttempt, Exception failure)
    {
      TimeSpan nextDelay = TimeSpan.Zero;
      try
      {
        if (this.retryOptions.Handle(failure))
        {
          if (this.context.CurrentUtcDateTime < (this.retryOptions.RetryTimeout != TimeSpan.MaxValue ? firstAttempt.Add(this.retryOptions.RetryTimeout) : DateTime.MaxValue))
          {
            double num = this.retryOptions.FirstRetryInterval.TotalMilliseconds * Math.Pow(this.retryOptions.BackoffCoefficient, (double) attempt);
            nextDelay = num < this.retryOptions.MaxRetryInterval.TotalMilliseconds ? TimeSpan.FromMilliseconds(num) : this.retryOptions.MaxRetryInterval;
          }
        }
      }
      catch (Exception ex)
      {
      }
      return nextDelay;
    }
  }
}
