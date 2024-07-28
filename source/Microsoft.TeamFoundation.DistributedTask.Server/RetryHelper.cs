// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.RetryHelper
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.VisualStudio.Services.Orchestration;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  public static class RetryHelper
  {
    public static async Task<T> ExecuteAsync<T>(
      this OrchestrationContext context,
      Func<Task<T>> action,
      int maxAttempts = 5,
      int backoffIntervalInSeconds = 10,
      Func<Exception, bool> canRetry = null,
      Action<Exception> traceException = null)
    {
      while (maxAttempts-- > 0)
      {
        T obj;
        try
        {
          obj = await action();
          goto label_11;
        }
        catch (TaskFailedException ex)
        {
          if (traceException != null)
            traceException((Exception) ex);
          if (maxAttempts != 0 && canRetry != null)
          {
            if (canRetry(ex.InnerException))
              goto label_8;
          }
          throw;
        }
label_8:
        object timer = await context.CreateTimer<object>(context.CurrentUtcDateTime.Add(TimeSpan.FromSeconds((double) backoffIntervalInSeconds)), (object) null);
        backoffIntervalInSeconds *= 2;
        continue;
label_11:
        return obj;
      }
      throw new InvalidOperationException("Should never get here");
    }

    public static void TracePhaseException(OrchestrationContext context, Exception exception)
    {
      if (exception is AggregateException)
        exception = ((AggregateException) exception).Flatten().InnerException;
      if (exception is TaskFailedException && exception.InnerException != null)
        exception = exception.InnerException;
      if (exception is SubOrchestrationFailedException && exception.InnerException != null)
        exception = exception.InnerException;
      context.Trace(0, TraceLevel.Error, exception.ToString());
    }
  }
}
