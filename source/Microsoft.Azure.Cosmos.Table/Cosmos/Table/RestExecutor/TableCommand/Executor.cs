// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Table.RestExecutor.TableCommand.Executor
// Assembly: Microsoft.Azure.Cosmos.Table, Version=1.0.7.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 461D0B3A-0B96-4D42-B330-3A8E714FC39A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Table.dll

using Microsoft.Azure.Cosmos.Table.RestExecutor.Common;
using Microsoft.Azure.Cosmos.Table.RestExecutor.Utils;
using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos.Table.RestExecutor.TableCommand
{
  internal class Executor
  {
    public static T ExecuteSync<T>(
      RESTCommand<T> cmd,
      IRetryPolicy policy,
      OperationContext operationContext)
    {
      using (new ExecutionState<T>(cmd, policy, operationContext))
        return RestUtility.RunWithoutSynchronizationContext<T>((Func<T>) (() => Executor.ExecuteAsync<T>(cmd, policy, operationContext, CancellationToken.None).ConfigureAwait(false).GetAwaiter().GetResult()));
    }

    public static async Task<T> ExecuteAsync<T>(
      RESTCommand<T> cmd,
      IRetryPolicy policy,
      OperationContext operationContext,
      CancellationToken token)
    {
      T result;
      using (ExecutionState<T> executionState1 = new ExecutionState<T>(cmd, policy, operationContext))
      {
        HttpClient client = cmd.HttpClient ?? HttpClientFactory.Instance;
        using (CancellationTokenSource timeoutTokenSource = CancellationTokenSource.CreateLinkedTokenSource(token))
        {
          CancellationToken timeoutToken = timeoutTokenSource.Token;
          bool shouldRetry = false;
          TimeSpan delay = TimeSpan.Zero;
          do
          {
            object obj;
            try
            {
              executionState1.Init();
              ExecutionStateUtils.StartRequestAttempt<T>(executionState1);
              Executor.ProcessStartOfRequest<T>(executionState1, "Starting asynchronous request to {0}.", timeoutTokenSource, client.Timeout);
              ExecutionStateUtils.CheckTimeout<T>(executionState1, true);
            }
            catch (Exception ex1)
            {
              obj = (object) ex1;
              Exception ex2 = (Exception) obj;
              Logger.LogError(executionState1.OperationContext, "Exception thrown while initializing request: {0}.", (object) ex2.Message);
              StorageException storageException = await ExecutionStateUtils.TranslateExceptionBasedOnParseErrorAsync(ex2, executionState1.Cmd.CurrentResult, executionState1.Resp, executionState1.Cmd.ParseErrorAsync, timeoutToken).ConfigureAwait(false);
              storageException.IsRetryable = false;
              executionState1.ExceptionRef = (Exception) storageException;
              throw executionState1.ExceptionRef;
            }
            try
            {
              int num;
              try
              {
                executionState1.CurrentOperation = ExecutorOperation.BeginGetResponse;
                Logger.LogInformational(executionState1.OperationContext, "Waiting for response.");
                ExecutionState<T> executionState = executionState1;
                executionState.Resp = await client.SendAsync((HttpRequestMessage) executionState1.Req, HttpCompletionOption.ResponseHeadersRead, timeoutToken).ConfigureAwait(false);
                executionState = (ExecutionState<T>) null;
                executionState1.CurrentOperation = ExecutorOperation.EndGetResponse;
                if (!executionState1.Resp.IsSuccessStatusCode)
                {
                  executionState = executionState1;
                  executionState.ExceptionRef = (Exception) await StorageExceptionTranslator.PopulateStorageExceptionFromHttpResponseMessage(executionState1.Resp, executionState1.Cmd.CurrentResult, timeoutToken, executionState1.Cmd.ParseErrorAsync).ConfigureAwait(false);
                  executionState = (ExecutionState<T>) null;
                }
                Logger.LogInformational(executionState1.OperationContext, "Response received. Status code = {0}, Request ID = {1}, Content-MD5 = {2}, ETag = {3}.", (object) executionState1.Cmd.CurrentResult.HttpStatusCode, (object) executionState1.Cmd.CurrentResult.ServiceRequestID, (object) executionState1.Cmd.CurrentResult.ContentMd5, (object) executionState1.Cmd.CurrentResult.Etag);
                ExecutionStateUtils.FireResponseReceived<T>(executionState1);
                if (cmd.PreProcessResponse != null)
                {
                  executionState1.CurrentOperation = ExecutorOperation.PreProcess;
                  try
                  {
                    executionState1.Result = cmd.PreProcessResponse(cmd, executionState1.Resp, executionState1.ExceptionRef, executionState1.OperationContext);
                    executionState1.ExceptionRef = (Exception) null;
                  }
                  catch (Exception ex)
                  {
                    executionState1.ExceptionRef = ex;
                  }
                  Logger.LogInformational(executionState1.OperationContext, "Response headers were processed successfully, proceeding with the rest of the operation.");
                }
                executionState1.CurrentOperation = ExecutorOperation.GetResponseStream;
                cmd.ResponseStream = (Stream) new MemoryStream();
                using (Stream responseStream = await executionState1.Resp.Content.ReadAsStreamAsync().ConfigureAwait(false))
                {
                  await responseStream.WriteToAsync<T>(cmd.ResponseStream, executionState1, timeoutTokenSource.Token).ConfigureAwait(false);
                  cmd.ResponseStream.Position = 0L;
                }
                if (executionState1.ExceptionRef != null)
                {
                  executionState1.CurrentOperation = ExecutorOperation.BeginDownloadResponse;
                  Logger.LogInformational(executionState1.OperationContext, "Downloading error response body.");
                  try
                  {
                    executionState = executionState1;
                    executionState.ExceptionRef = (Exception) await StorageExceptionTranslator.TranslateExceptionWithPreBufferedStreamAsync(executionState1.ExceptionRef, executionState1.Cmd.CurrentResult, cmd.ResponseStream, executionState1.Resp, executionState1.Cmd.ParseErrorAsync, timeoutToken).ConfigureAwait(false);
                    executionState = (ExecutionState<T>) null;
                    throw executionState1.ExceptionRef;
                  }
                  finally
                  {
                    cmd.ResponseStream.Dispose();
                    cmd.ResponseStream = (Stream) null;
                  }
                }
                else
                {
                  await Executor.ProcessEndOfRequestAsync<T>(executionState1, timeoutToken).ConfigureAwait(false);
                  ExecutionStateUtils.FinishRequestAttempt<T>(executionState1);
                  result = executionState1.Result;
                  goto label_63;
                }
              }
              catch (Exception ex)
              {
                obj = (object) ex;
                num = 1;
              }
              if (num == 1)
              {
                Exception exception = (Exception) obj;
                Logger.LogWarning(executionState1.OperationContext, "Exception thrown during the operation: {0}.", (object) exception.Message);
                ExecutionStateUtils.FinishRequestAttempt<T>(executionState1);
                if (exception is OperationCanceledException && executionState1.OperationExpiryTime.HasValue && DateTime.UtcNow.CompareTo(executionState1.OperationExpiryTime.Value) > 0)
                  exception = (Exception) new TimeoutException("The client could not finish the operation within specified timeout.", exception);
                StorageException storageException = await ExecutionStateUtils.TranslateExceptionBasedOnParseErrorAsync(exception, executionState1.Cmd.CurrentResult, executionState1.Resp, executionState1.Cmd.ParseErrorAsync, timeoutToken).ConfigureAwait(false);
                executionState1.ExceptionRef = (Exception) storageException;
                Logger.LogInformational(executionState1.OperationContext, "Checking if the operation should be retried. Retry count = {0}, HTTP status code = {1}, Retryable exception = {2}, Exception = {3}.", (object) executionState1.RetryCount, (object) executionState1.Cmd.CurrentResult.HttpStatusCode, storageException.IsRetryable ? (object) "yes" : (object) "no", (object) storageException.Message);
                shouldRetry = false;
                if (storageException.IsRetryable)
                {
                  if (executionState1.RetryPolicy != null)
                  {
                    executionState1.CurrentLocation = ExecutionStateUtils.GetNextLocation(executionState1.CurrentLocation, cmd.LocationMode);
                    Logger.LogInformational(executionState1.OperationContext, "The next location has been set to {0}, based on the location mode.", (object) executionState1.CurrentLocation);
                    if (executionState1.RetryPolicy is IExtendedRetryPolicy retryPolicy)
                    {
                      RetryContext retryContext = new RetryContext(executionState1.RetryCount++, cmd.CurrentResult, executionState1.CurrentLocation, cmd.LocationMode);
                      RetryInfo retryInfo = retryPolicy.Evaluate(retryContext, executionState1.OperationContext);
                      if (retryInfo != null)
                      {
                        Logger.LogInformational(executionState1.OperationContext, "The extended retry policy set the next location to {0} and updated the location mode to {1}.", (object) retryInfo.TargetLocation, (object) retryInfo.UpdatedLocationMode);
                        shouldRetry = true;
                        executionState1.CurrentLocation = retryInfo.TargetLocation;
                        cmd.LocationMode = retryInfo.UpdatedLocationMode;
                        delay = retryInfo.RetryInterval;
                      }
                    }
                    else
                      shouldRetry = executionState1.RetryPolicy.ShouldRetry(executionState1.RetryCount++, cmd.CurrentResult.HttpStatusCode, executionState1.ExceptionRef, out delay, executionState1.OperationContext);
                    if (!(delay < TimeSpan.Zero))
                    {
                      if (!(delay > TableRestConstants.MaximumRetryBackoff))
                        goto label_49;
                    }
                    delay = TableRestConstants.MaximumRetryBackoff;
                  }
                }
              }
            }
            finally
            {
              if (executionState1.Resp != null)
              {
                executionState1.Resp.Dispose();
                executionState1.Resp = (HttpResponseMessage) null;
              }
            }
label_49:
            if (!shouldRetry || executionState1.OperationExpiryTime.HasValue && (DateTime.UtcNow + delay).CompareTo(executionState1.OperationExpiryTime.Value) > 0)
            {
              Logger.LogError(executionState1.OperationContext, shouldRetry ? "Operation cannot be retried because the maximum execution time has been reached. Failing with {0}." : "Retry policy did not allow for a retry. Failing with {0}.", (object) executionState1.ExceptionRef.Message);
              throw executionState1.ExceptionRef;
            }
            Action<RESTCommand<T>, Exception, OperationContext> recoveryAction = cmd.RecoveryAction;
            if (recoveryAction != null)
              recoveryAction(cmd, executionState1.Cmd.CurrentResult.Exception, executionState1.OperationContext);
            if (delay > TimeSpan.Zero)
              await Task.Delay(delay, timeoutToken).ConfigureAwait(false);
            Logger.LogInformational(executionState1.OperationContext, "Retrying failed operation.");
            ExecutionStateUtils.FireRetrying<T>(executionState1);
          }
          while (shouldRetry);
          throw new NotImplementedException("Unexpected internal storage client error.");
        }
      }
label_63:
      return result;
    }

    private static void ProcessStartOfRequest<T>(
      ExecutionState<T> executionState,
      string startLogMessage,
      CancellationTokenSource timeoutTokenSource,
      TimeSpan httpClientTimeout)
    {
      RESTCommand<T> restCmd = executionState.RestCMD;
      executionState.CurrentOperation = ExecutorOperation.BeginOperation;
      HttpContent httpContent = restCmd.BuildContent != null ? restCmd.BuildContent(restCmd, executionState.OperationContext) : (HttpContent) null;
      Uri uri1 = restCmd.StorageUri.GetUri(executionState.CurrentLocation);
      Uri uri2 = restCmd.Credentials.TransformUri(uri1);
      Logger.LogInformational(executionState.OperationContext, "Starting asynchronous request to {0}.", (object) uri2);
      UriQueryBuilder uriQueryBuilder = new UriQueryBuilder(executionState.RestCMD.Builder);
      executionState.Req = restCmd.BuildRequest(restCmd, uri2, uriQueryBuilder, httpContent, restCmd.ServerTimeoutInSeconds, executionState.OperationContext);
      ExecutionStateUtils.ApplyUserHeaders<T>(executionState);
      ExecutionStateUtils.FireSendingRequest<T>(executionState);
      TimeSpan timeSpan = executionState.OperationExpiryTime.HasValue ? executionState.RemainingTimeout : TableRequestOptions.MaxMaximumExecutionTime;
      timeoutTokenSource.CancelAfter(Math.Min((int) timeSpan.TotalMilliseconds, (int) httpClientTimeout.TotalMilliseconds));
    }

    private static void TryToForceStalledCancellation<T>(ExecutionState<T> executionState)
    {
      Logger.LogWarning(executionState.OperationContext, "Attempting to force a stalled cancellation.");
      executionState?.Resp?.Content?.Dispose();
    }

    private static async Task ProcessEndOfRequestAsync<T>(
      ExecutionState<T> executionState,
      CancellationToken cancellationToken)
    {
      if (executionState.RestCMD.PostProcessResponseAsync != null)
      {
        executionState.CurrentOperation = ExecutorOperation.PostProcess;
        Logger.LogInformational(executionState.OperationContext, "Processing response body.");
        ExecutionState<T> state = executionState;
        Task<T> task = executionState.RestCMD.PostProcessResponseAsync(executionState.RestCMD, executionState.Resp, executionState.OperationContext, cancellationToken);
        ExecutionState<T> executionState1;
        if (task.IsCompleted)
        {
          executionState1 = state;
          executionState1.Result = await task.ConfigureAwait(false);
          executionState1 = (ExecutionState<T>) null;
        }
        else
        {
          using (CancellationTokenSource fallbackCancellation = new CancellationTokenSource())
          {
            CancellationTokenRegistration tokenRegistration1 = cancellationToken.Register((Action<object>) (fcts => ((CancellationTokenSource) fcts).CancelAfter(TableRestConstants.ResponseParserCancellationFallbackDelay)), (object) fallbackCancellation);
            try
            {
              CancellationTokenRegistration tokenRegistration2 = fallbackCancellation.Token.Register((Action<object>) (s => Executor.TryToForceStalledCancellation<T>((ExecutionState<T>) s)), (object) state);
              try
              {
                executionState1 = state;
                executionState1.Result = await task.ConfigureAwait(false);
                executionState1 = (ExecutionState<T>) null;
              }
              finally
              {
                tokenRegistration2.Dispose();
              }
              tokenRegistration2 = new CancellationTokenRegistration();
            }
            catch (Exception ex) when (fallbackCancellation.IsCancellationRequested && !(ex is OperationCanceledException))
            {
              Logger.LogWarning(executionState.OperationContext, string.Format("Fallback cancellation induced {0}", (object) ex));
              cancellationToken.ThrowIfCancellationRequested();
            }
            finally
            {
              tokenRegistration1.Dispose();
            }
            tokenRegistration1 = new CancellationTokenRegistration();
          }
        }
      }
      executionState.CurrentOperation = ExecutorOperation.EndOperation;
      Logger.LogInformational(executionState.OperationContext, "Operation completed successfully.");
      executionState.CancelDelegate = (Action) null;
    }
  }
}
