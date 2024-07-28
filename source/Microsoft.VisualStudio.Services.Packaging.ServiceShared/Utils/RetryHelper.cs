// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils.RetryHelper
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Feed.Common;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils
{
  public class RetryHelper : IRetryHelper, IAsyncInvoker
  {
    private readonly Func<Exception, bool> retryOnExceptionPredicate;
    private readonly Random rand = new Random();
    private readonly ITracerService tracerService;
    private readonly IReadOnlyList<TimeSpan> requestProfile;

    public RetryHelper(
      ITracerService tracerService,
      int maxRetries,
      TimeSpan maxRetryDelay,
      Func<Exception, bool> retryOnExceptionPredicate)
      : this(tracerService, (IReadOnlyList<TimeSpan>) Enumerable.Repeat<TimeSpan>(maxRetryDelay, maxRetries).ToList<TimeSpan>(), retryOnExceptionPredicate)
    {
    }

    public RetryHelper(
      ITracerService tracerService,
      IReadOnlyList<TimeSpan> requestProfile,
      Func<Exception, bool> retryOnExceptionPredicate)
    {
      this.tracerService = tracerService;
      this.requestProfile = requestProfile;
      this.retryOnExceptionPredicate = retryOnExceptionPredicate;
    }

    public RetryHelper(
      IVssRequestContext requestContext,
      int maxRetries,
      TimeSpan maxRetryDelay,
      Func<Exception, bool> retryOnExceptionPredicate)
      : this(requestContext.GetTracerFacade(), maxRetries, maxRetryDelay, retryOnExceptionPredicate)
    {
    }

    public RetryHelper(
      IVssRequestContext requestContext,
      IReadOnlyList<TimeSpan> requestProfile,
      Func<Exception, bool> retryOnExceptionPredicate)
      : this(requestContext.GetTracerFacade(), requestProfile, retryOnExceptionPredicate)
    {
    }

    public async Task Invoke(Func<Task> function)
    {
      object obj1 = await this.Invoke<object>((Func<Task<object>>) (async () =>
      {
        await function();
        object obj2;
        return obj2;
      }));
    }

    public Task<T> Invoke<T>(Func<Task<T>> function) => this.Invoke<T>(function, (Func<T, bool>) null);

    public async Task<T> Invoke<T>(Func<Task<T>> function, Func<T, bool> retryOnResultPredicate)
    {
      RetryHelper sendInTheThisObject = this;
      T obj1;
      using (ITracerBlock tracer = sendInTheThisObject.tracerService.Enter((object) sendInTheThisObject, nameof (Invoke)))
      {
        int attempt = 0;
        while (true)
        {
          try
          {
            T obj2 = await function();
            if (retryOnResultPredicate == null || !retryOnResultPredicate(obj2))
            {
              obj1 = obj2;
              break;
            }
            if (sendInTheThisObject.AnyAttemptsRemaining(attempt))
            {
              tracer.TraceInfo(string.Format("RetryHelper retrying on result \"{0}\" (retry #: {1})", (object) obj2, (object) attempt));
            }
            else
            {
              tracer.TraceInfo(string.Format("RetryHelper not retrying on result \"{0}\" (out of retries) (retry #: {1})", (object) obj2, (object) attempt));
              throw new NoMoreRetriesException();
            }
          }
          catch (Exception ex) when (
          {
            // ISSUE: unable to correctly present filter
            ex = ex;
            if (sendInTheThisObject.ShouldRetryOnException(ex, attempt))
            {
              SuccessfulFiltering;
            }
            else
              throw;
          }
          )
          {
            string str = string.Empty;
            for (; ex != null; ex = ex.InnerException)
              str = str + ", exceptionType : " + ex.GetType().Name + ", exceptionMessage : " + ex.Message;
            tracer.TraceInfo(string.Format("RetryHelper encountered exception, will retry (retry #: {0}{1})", (object) attempt, (object) str));
          }
          sendInTheThisObject.Delay(attempt);
          ++attempt;
        }
      }
      return obj1;
    }

    private bool ShouldRetryOnException(Exception exception, int attempt)
    {
      if (!this.AnyAttemptsRemaining(attempt))
        return false;
      if (VssNetworkHelper.IsTransientNetworkException(exception))
        return true;
      return this.retryOnExceptionPredicate != null && this.retryOnExceptionPredicate(exception);
    }

    private bool AnyAttemptsRemaining(int attempt) => attempt < this.requestProfile.Count;

    private void Delay(int attempt)
    {
      Random rand = this.rand;
      TimeSpan timeSpan = this.requestProfile[attempt];
      int minValue = (int) (timeSpan.TotalMilliseconds * 0.7);
      timeSpan = this.requestProfile[attempt];
      int maxValue = (int) (timeSpan.TotalMilliseconds * 1.3);
      Thread.Sleep(rand.Next(minValue, maxValue));
    }
  }
}
