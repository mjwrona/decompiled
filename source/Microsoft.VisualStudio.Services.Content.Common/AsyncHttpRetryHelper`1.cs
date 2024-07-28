// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Common.AsyncHttpRetryHelper`1
// Assembly: Microsoft.VisualStudio.Services.Content.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DC45E7D4-4445-41B3-8FA2-C13CD848D0F1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Common.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Content.Common.Tracing;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Content.Common
{
  public class AsyncHttpRetryHelper<TResult>
  {
    private static readonly TimeSpan DefaultMinBackoff = TimeSpan.FromSeconds(1.0);
    private static readonly TimeSpan DefaultMaxBackoff = TimeSpan.FromMinutes(1.0);
    private static readonly TimeSpan DefaultDeltaBackoff = TimeSpan.FromSeconds(1.0);
    private readonly System.Func<Exception, bool> canRetryDelegate;
    private readonly IAppTraceSource tracer;
    private readonly Func<Task<TResult>> taskGenerator;
    private readonly int maxRetries;
    private readonly TimeSpan minBackoff;
    private readonly TimeSpan maxBackoff;
    private readonly TimeSpan deltaBackoff;
    private readonly Func<int, TimeSpan, TimeSpan, TimeSpan, TimeSpan> GetExponentialBackoff;
    private readonly bool continueOnCapturedContext;
    private readonly string context;
    public static readonly List<int> RetryableHttpStatusCodes = new List<int>()
    {
      502,
      408,
      429,
      500,
      503,
      504
    };

    public AsyncHttpRetryHelper(
      Func<Task<TResult>> taskGenerator,
      int maxRetries,
      IAppTraceSource tracer,
      bool continueOnCapturedContext,
      string context,
      System.Func<Exception, bool> canRetryDelegate = null,
      TimeSpan? minBackoff = null,
      TimeSpan? maxBackoff = null,
      TimeSpan? deltaBackoff = null,
      Func<int, TimeSpan, TimeSpan, TimeSpan, TimeSpan> getExponentialBackoff = null)
    {
      ArgumentUtility.CheckForNull<IAppTraceSource>(tracer, nameof (tracer));
      this.tracer = tracer;
      this.continueOnCapturedContext = continueOnCapturedContext;
      this.context = "[" + context + "] ";
      this.maxRetries = maxRetries;
      this.taskGenerator = taskGenerator;
      this.canRetryDelegate = canRetryDelegate;
      this.minBackoff = minBackoff ?? AsyncHttpRetryHelper<TResult>.DefaultMinBackoff;
      TimeSpan? nullable = maxBackoff;
      this.maxBackoff = nullable ?? AsyncHttpRetryHelper<TResult>.DefaultMaxBackoff;
      nullable = deltaBackoff;
      this.deltaBackoff = nullable ?? AsyncHttpRetryHelper<TResult>.DefaultDeltaBackoff;
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      this.GetExponentialBackoff = getExponentialBackoff ?? AsyncHttpRetryHelper<TResult>.\u003C\u003EO.\u003C0\u003E__GetExponentialBackoff ?? (AsyncHttpRetryHelper<TResult>.\u003C\u003EO.\u003C0\u003E__GetExponentialBackoff = new Func<int, TimeSpan, TimeSpan, TimeSpan, TimeSpan>(BackoffTimerHelper.GetExponentialBackoff));
    }

    public int RemainingRetries { get; private set; }

    public static Task<TResult> InvokeAsync(
      Func<Task<TResult>> taskGenerator,
      int maxRetries,
      IAppTraceSource tracer,
      System.Func<Exception, bool> canRetryDelegate,
      CancellationToken cancellationToken,
      bool continueOnCapturedContext,
      string context,
      TimeSpan? minBackoff = null,
      TimeSpan? maxBackoff = null,
      TimeSpan? deltaBackoff = null)
    {
      return new AsyncHttpRetryHelper<TResult>(taskGenerator, maxRetries, tracer, continueOnCapturedContext, context, canRetryDelegate, minBackoff, maxBackoff, deltaBackoff).InvokeAsync(cancellationToken);
    }

    public async Task<TResult> InvokeAsync(CancellationToken cancellationToken)
    {
      int remainingRetries = this.maxRetries;
      this.RemainingRetries = remainingRetries;
      TimeSpan? lastBackoff = new TimeSpan?();
      TResult result;
      try
      {
        do
        {
          cancellationToken.ThrowIfCancellationRequested();
          TimeSpan timeToDelay;
          try
          {
            result = await this.taskGenerator().ConfigureAwait(this.continueOnCapturedContext);
            goto label_16;
          }
          catch (Exception ex)
          {
            int num1 = this.maxRetries - remainingRetries;
            int num2 = 1 + num1;
            bool flag = AsyncHttpRetryHelper<TResult>.IsRetryable(ex, this.canRetryDelegate, cancellationToken);
            string detailsForTracing = ex.GetHttpMessageDetailsForTracing();
            if (cancellationToken.IsCancellationRequested)
            {
              this.tracer.Info(string.Format("{0}Try {1}/{2}, Task was requested to be canceled.", (object) this.context, (object) num2, (object) this.maxRetries));
              cancellationToken.ThrowIfCancellationRequested();
              throw new InvalidOperationException("unreachable.");
            }
            if (!flag)
            {
              this.tracer.Verbose(ex, string.Format("{0}Try {1}/{2}, non-retryable exception caught. Throwing. Details:{3}{4}", (object) this.context, (object) num2, (object) this.maxRetries, (object) Environment.NewLine, (object) detailsForTracing));
              ex.Data[(object) "AttemptCount"] = (object) num2;
              ex.Data[(object) "LastBackoff"] = (object) lastBackoff;
              ex.ReThrow();
              throw new InvalidOperationException("unreachable.");
            }
            if (remainingRetries <= 0)
            {
              this.tracer.Verbose(ex, string.Format("{0}Try {1}/{2}, retryable exception caught, but retries have been exhausted. Throwing.", (object) this.context, (object) num2, (object) this.maxRetries));
              this.tracer.Verbose("Details:" + Environment.NewLine + detailsForTracing);
              ex.Data[(object) "AttemptCount"] = (object) num2;
              ex.Data[(object) "LastBackoff"] = (object) lastBackoff;
              ex.ReThrow();
              throw new InvalidOperationException("unreachable.");
            }
            timeToDelay = this.GetExponentialBackoff(num1, this.minBackoff, this.maxBackoff, this.deltaBackoff);
            this.tracer.Verbose(ex, string.Format("{0}Try {1}/{2}, retryable exception caught. Retrying in {3}.", (object) this.context, (object) num2, (object) this.maxRetries, (object) timeToDelay));
            this.tracer.Verbose("Details:" + Environment.NewLine + detailsForTracing);
          }
          --remainingRetries;
          await Task.Delay(timeToDelay, cancellationToken).ConfigureAwait(this.continueOnCapturedContext);
          lastBackoff = new TimeSpan?(timeToDelay);
          timeToDelay = new TimeSpan();
        }
        while (remainingRetries >= 0);
      }
      finally
      {
        this.RemainingRetries = remainingRetries;
      }
      throw new ConstraintException(this.context + "This exception should not be reachable.");
label_16:
      return result;
    }

    private static bool IsRetryable(
      Exception exception,
      System.Func<Exception, bool> canRetryDelegate,
      CancellationToken cancellationToken)
    {
      if (AsyncHttpRetryHelper.IsTransientException(exception, cancellationToken))
        return true;
      return canRetryDelegate != null && canRetryDelegate(exception);
    }
  }
}
