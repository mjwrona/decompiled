// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.RetryHelper`1
// Assembly: Microsoft.VisualStudio.Services.Feed.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55555083-3B79-4F4F-AA85-92D66019974E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Diagnostics;
using System.Threading;

namespace Microsoft.VisualStudio.Services.Feed.Server
{
  public class RetryHelper<T>
  {
    private readonly IVssRequestContext requestContext;
    private readonly int maxRetries;
    private readonly TimeSpan maxRetryDelay;
    private readonly Func<Exception, bool> canRetryDelegate;
    private readonly Random rand = new Random();

    public RetryHelper(
      IVssRequestContext requestContext,
      int maxRetries,
      TimeSpan maxRetryDelay,
      Func<Exception, bool> canRetryDelegate)
    {
      this.requestContext = requestContext;
      this.maxRetries = maxRetries;
      this.maxRetryDelay = maxRetryDelay;
      this.canRetryDelegate = canRetryDelegate;
    }

    public void Invoke(Action function) => this.requestContext.TraceBlock(10019227, 10019228, 10019229, "FeedIndex", "Service", (Action) (() =>
    {
      int remainingRetries = this.maxRetries - 1;
label_1:
      try
      {
        function();
      }
      catch (Exception ex)
      {
        if (!this.HandleException(ex, ref remainingRetries))
          throw;
        else
          goto label_1;
      }
    }), nameof (Invoke));

    public T InvokeWithReturn(Func<T> function) => this.requestContext.TraceBlock<T>(10019227, 10019228, 10019229, "FeedIndex", "Service", (Func<T>) (() =>
    {
      int remainingRetries = this.maxRetries - 1;
label_1:
      try
      {
        return function();
      }
      catch (Exception ex)
      {
        if (!this.HandleException(ex, ref remainingRetries))
          throw;
        else
          goto label_1;
      }
    }), nameof (InvokeWithReturn));

    private bool HandleException(Exception exception, ref int remainingRetries)
    {
      if (remainingRetries <= 0 || this.canRetryDelegate == null || !this.canRetryDelegate(exception))
        return false;
      string empty = string.Empty;
      for (; exception != null; exception = exception.InnerException)
        empty += string.Format(", exceptionType : {0}, exceptionMessage : {1}", (object) exception.GetType().Name, (object) exception.Message);
      this.requestContext.Trace(10019230, TraceLevel.Info, "FeedIndex", "Service", empty);
      Thread.Sleep(this.rand.Next((int) this.maxRetryDelay.TotalMilliseconds));
      --remainingRetries;
      return true;
    }
  }
}
