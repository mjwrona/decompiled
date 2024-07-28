// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Table.Extensions.TableExtensionRetryPolicy
// Assembly: Microsoft.Azure.Cosmos.Table, Version=1.0.7.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 461D0B3A-0B96-4D42-B330-3A8E714FC39A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Table.dll

using Microsoft.Azure.Documents;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos.Table.Extensions
{
  internal static class TableExtensionRetryPolicy
  {
    internal static readonly TimeSpan MaximumRetryBackoff = TimeSpan.FromHours(1.0);

    public static Task<TResult> Execute<TResult>(
      Func<Task<TResult>> executionMethod,
      CancellationToken cancellationToken,
      OperationContext operationContext,
      TableRequestOptions requestOptions)
    {
      return requestOptions != null && requestOptions.RetryPolicy != null ? TableExtensionRetryPolicy.ExecuteUnderRetryPolicy<TResult>(executionMethod, cancellationToken, operationContext, requestOptions) : executionMethod();
    }

    private static async Task<TResult> ExecuteUnderRetryPolicy<TResult>(
      Func<Task<TResult>> executionMethod,
      CancellationToken cancellationToken,
      OperationContext operationContext,
      TableRequestOptions requestOptions)
    {
      Microsoft.Azure.Cosmos.Table.IRetryPolicy retryPolicyInstance = (Microsoft.Azure.Cosmos.Table.IRetryPolicy) null;
      int retryCount = 0;
      DateTime startTime = DateTime.UtcNow;
      StorageException previousException = (StorageException) null;
      TResult result;
      while (true)
      {
        do
        {
          operationContext = operationContext ?? new OperationContext();
          TimeSpan retryInterval;
          try
          {
            TableExtensionRetryPolicy.ThrowTimeoutIfElapsed(requestOptions.MaximumExecutionTime, startTime, operationContext, previousException);
            TableExtensionRetryPolicy.ThrowCancellationIfRequested(cancellationToken);
            result = await executionMethod();
            goto label_19;
          }
          catch (NotFoundException ex)
          {
            Logger.LogError(operationContext, "Retry policy did not allow for a retry. Failing with {0}.", (object) ex.Message);
            throw;
          }
          catch (StorageException ex)
          {
            if (retryPolicyInstance == null)
              retryPolicyInstance = requestOptions.RetryPolicy.CreateInstance();
            if (!ex.IsRetryable)
            {
              Logger.LogError(operationContext, "Retry policy did not allow for a retry. Failing with {0}.", (object) ex.Message);
              throw;
            }
            else if (!retryPolicyInstance.ShouldRetry(retryCount++, ex.RequestInformation != null ? ex.RequestInformation.HttpStatusCode : 500, (Exception) ex, out retryInterval, operationContext))
            {
              Logger.LogError(operationContext, "Retry policy did not allow for a retry. Failing with {0}.", (object) ex.Message);
              throw;
            }
            else
              previousException = ex;
          }
          if (retryInterval < TimeSpan.Zero || retryInterval > TableExtensionRetryPolicy.MaximumRetryBackoff)
            retryInterval = TableExtensionRetryPolicy.MaximumRetryBackoff;
          if (retryInterval != TimeSpan.Zero)
          {
            TableExtensionRetryPolicy.ThrowTimeoutIfElapsed(requestOptions.MaximumExecutionTime, startTime, operationContext, previousException);
            Logger.LogInformational(operationContext, "Operation will be retried after {0}ms.", (object) (int) retryInterval.TotalMilliseconds);
            await Task.Delay(retryInterval);
          }
        }
        while (operationContext == null);
        operationContext.FireRetrying(new RequestEventArgs(operationContext.LastResult));
      }
label_19:
      return result;
    }

    private static void ThrowTimeoutIfElapsed(
      TimeSpan? maxExecutionTime,
      DateTime startTime,
      OperationContext operationContext,
      StorageException previousException)
    {
      if (!maxExecutionTime.HasValue || !(DateTime.UtcNow - startTime > maxExecutionTime.Value))
        return;
      Logger.LogError(operationContext, "Operation cannot be retried because the maximum execution time has been reached. Failing with {0}.", (object) "The client could not finish the operation within specified timeout.");
      if (previousException != null)
        throw previousException;
      RequestResult res = new RequestResult();
      res.HttpStatusCode = 408;
      TimeoutException inner = new TimeoutException("The client could not finish the operation within specified timeout.");
      throw new StorageException(res, inner.Message, (Exception) inner)
      {
        IsRetryable = false
      };
    }

    private static void ThrowCancellationIfRequested(CancellationToken cancellationToken)
    {
      if (cancellationToken.IsCancellationRequested)
      {
        RequestResult res = new RequestResult();
        res.HttpStatusCode = 306;
        res.HttpStatusMessage = "Unused";
        OperationCanceledException inner = new OperationCanceledException("Operation was canceled by user.", (Exception) null);
        throw new StorageException(res, inner.Message, (Exception) inner)
        {
          IsRetryable = false
        };
      }
    }
  }
}
