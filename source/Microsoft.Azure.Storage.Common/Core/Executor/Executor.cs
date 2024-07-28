// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.Core.Executor.Executor
// Assembly: Microsoft.Azure.Storage.Common, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 0978DA65-6954-4A99-9ACB-2EF3D979A5D5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.Common.dll

using Microsoft.Azure.Storage.Core.Util;
using Microsoft.Azure.Storage.RetryPolicies;
using Microsoft.Azure.Storage.Shared.Protocol;
using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Storage.Core.Executor
{
  internal class Executor : ExecutorBase
  {
    public static T ExecuteSync<T>(
      RESTCommand<T> cmd,
      IRetryPolicy policy,
      OperationContext operationContext)
    {
      using (new ExecutionState<T>((StorageCommandBase<T>) cmd, policy, operationContext))
        return CommonUtility.RunWithoutSynchronizationContext<T>((Func<T>) (() => Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteAsync<T>(cmd, policy, operationContext, CancellationToken.None).GetAwaiter().GetResult()));
    }

    public static async Task<T> ExecuteAsync<T>(
      RESTCommand<T> cmd,
      IRetryPolicy policy,
      OperationContext operationContext,
      CancellationToken token)
    {
      T result;
      using (ExecutionState<T> executionState1 = new ExecutionState<T>((StorageCommandBase<T>) cmd, policy, operationContext))
      {
        using (CancellationTokenSource timeoutTokenSource = CancellationTokenSource.CreateLinkedTokenSource(token))
        {
          bool shouldRetry = false;
          TimeSpan delay = TimeSpan.Zero;
          HttpClient client = cmd.HttpClient ?? HttpClientFactory.Instance;
          do
          {
            object obj;
            try
            {
              executionState1.Init();
              ExecutorBase.StartRequestAttempt<T>(executionState1);
              Microsoft.Azure.Storage.Core.Executor.Executor.ProcessStartOfRequest<T>(executionState1, "Starting asynchronous request to {0}.", timeoutTokenSource);
              ExecutorBase.CheckTimeout<T>(executionState1, true);
            }
            catch (Exception ex1)
            {
              obj = (object) ex1;
              Exception ex2 = (Exception) obj;
              Logger.LogError(executionState1.OperationContext, "Exception thrown while initializing request: {0}.", (object) ex2.Message);
              StorageException storageException = await ExecutorBase.TranslateExceptionBasedOnParseErrorAsync(ex2, executionState1.Cmd.CurrentResult, executionState1.Resp, executionState1.Cmd.ParseErrorAsync, token).ConfigureAwait(false);
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
                executionState.Resp = await client.SendAsync(executionState1.Req, HttpCompletionOption.ResponseHeadersRead, timeoutTokenSource.Token).ConfigureAwait(false);
                executionState = (ExecutionState<T>) null;
                executionState1.CurrentOperation = ExecutorOperation.EndGetResponse;
                string header1 = HttpRequestParsers.GetHeader(executionState1.Req, "x-ms-client-request-id");
                string header2 = HttpResponseParsers.GetHeader(executionState1.Resp, "x-ms-client-request-id");
                if (header2 != null && header2 != header1)
                {
                  string header3 = HttpResponseParsers.GetHeader(executionState1.Resp, "x-ms-request-id");
                  StorageException storageException = new StorageException("Echoed client request ID: " + header2 + " does not match sent client request ID: " + header1 + ".  Service request ID: " + header3)
                  {
                    IsRetryable = false
                  };
                  executionState1.ExceptionRef = (Exception) storageException;
                  throw storageException;
                }
                if (!executionState1.Resp.IsSuccessStatusCode)
                {
                  executionState = executionState1;
                  executionState.ExceptionRef = (Exception) await Exceptions.PopulateStorageExceptionFromHttpResponseMessage(executionState1.Resp, executionState1.Cmd.CurrentResult, token, executionState1.Cmd.ParseErrorAsync).ConfigureAwait(false);
                  executionState = (ExecutionState<T>) null;
                }
                Logger.LogInformational(executionState1.OperationContext, "Response received. Status code = {0}, Request ID = {1}, Content-MD5 = {2}, Content-CRC64 = {3}, ETag = {4}.", (object) executionState1.Cmd.CurrentResult.HttpStatusCode, (object) executionState1.Cmd.CurrentResult.ServiceRequestID, (object) executionState1.Cmd.CurrentResult.ContentMd5, (object) executionState1.Cmd.CurrentResult.ContentCrc64, (object) executionState1.Cmd.CurrentResult.Etag);
                ExecutorBase.FireResponseReceived<T>(executionState1);
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
                Stream wrappedStream = await executionState1.Resp.Content.ReadAsStreamAsync().ConfigureAwait(false);
                if (((StorageCommandBase<T>) cmd).NetworkTimeout.HasValue)
                  wrappedStream = (Stream) new TimeoutStream(wrappedStream, ((StorageCommandBase<T>) cmd).NetworkTimeout.Value);
                cmd.ResponseStream = wrappedStream;
                if (executionState1.ExceptionRef != null)
                {
                  executionState1.CurrentOperation = ExecutorOperation.BeginDownloadResponse;
                  Logger.LogInformational(executionState1.OperationContext, "Downloading error response body.");
                  try
                  {
                    cmd.ErrorStream = (Stream) new MemoryStream();
                    await cmd.ResponseStream.WriteToAsync<T>(cmd.ErrorStream, (IBufferManager) null, new long?(), new long?(), ChecksumRequested.None, executionState1, new StreamDescriptor(), timeoutTokenSource.Token).ConfigureAwait(false);
                    cmd.ErrorStream.Seek(0L, SeekOrigin.Begin);
                    executionState1.ExceptionRef = (Exception) StorageException.TranslateExceptionWithPreBufferedStream(executionState1.ExceptionRef, executionState1.Cmd.CurrentResult, (Func<Stream, StorageExtendedErrorInformation>) (stream => executionState1.Cmd.ParseError(stream, executionState1.Resp, (string) null)), cmd.ErrorStream, executionState1.Resp);
                    throw executionState1.ExceptionRef;
                  }
                  finally
                  {
                    cmd.ResponseStream.Dispose();
                    cmd.ResponseStream = (Stream) null;
                    cmd.ErrorStream.Dispose();
                    cmd.ErrorStream = (Stream) null;
                  }
                }
                else
                {
                  if (!cmd.RetrieveResponseStream)
                    cmd.DestinationStream = Stream.Null;
                  if (cmd.DestinationStream != null)
                  {
                    if (((StorageCommandBase<T>) cmd).StreamCopyState == null)
                      ((StorageCommandBase<T>) cmd).StreamCopyState = new StreamDescriptor();
                    try
                    {
                      executionState1.CurrentOperation = ExecutorOperation.BeginDownloadResponse;
                      Logger.LogInformational(executionState1.OperationContext, "Downloading response body.");
                      await cmd.ResponseStream.WriteToAsync<T>(cmd.DestinationStream, (IBufferManager) null, new long?(), new long?(), cmd.ChecksumRequestedForResponseStream, executionState1, ((StorageCommandBase<T>) cmd).StreamCopyState, timeoutTokenSource.Token).ConfigureAwait(false);
                    }
                    finally
                    {
                      cmd.ResponseStream.Dispose();
                      cmd.ResponseStream = (Stream) null;
                    }
                  }
                  await Microsoft.Azure.Storage.Core.Executor.Executor.ProcessEndOfRequestAsync<T>(executionState1, token).ConfigureAwait(false);
                  ExecutorBase.FinishRequestAttempt<T>(executionState1);
                  result = executionState1.Result;
                  goto label_75;
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
                ExecutorBase.FinishRequestAttempt<T>(executionState1);
                if (exception is OperationCanceledException)
                {
                  DateTime? operationExpiryTime = executionState1.OperationExpiryTime;
                  if (operationExpiryTime.HasValue)
                  {
                    DateTime now = DateTime.Now;
                    ref DateTime local = ref now;
                    operationExpiryTime = executionState1.OperationExpiryTime;
                    DateTime dateTime = operationExpiryTime.Value;
                    if (local.CompareTo(dateTime) > 0)
                      exception = (Exception) new TimeoutException("The client could not finish the operation within specified timeout.", exception);
                  }
                }
                StorageException storageException = await ExecutorBase.TranslateExceptionBasedOnParseErrorAsync(exception, executionState1.Cmd.CurrentResult, executionState1.Resp, executionState1.Cmd.ParseErrorAsync, token).ConfigureAwait(false);
                executionState1.ExceptionRef = (Exception) storageException;
                Logger.LogInformational(executionState1.OperationContext, "Checking if the operation should be retried. Retry count = {0}, HTTP status code = {1}, Retryable exception = {2}, Exception = {3}.", (object) executionState1.RetryCount, (object) executionState1.Cmd.CurrentResult.HttpStatusCode, storageException.IsRetryable ? (object) "yes" : (object) "no", (object) storageException.Message);
                shouldRetry = false;
                if (storageException.IsRetryable)
                {
                  if (executionState1.RetryPolicy != null)
                  {
                    executionState1.CurrentLocation = ExecutorBase.GetNextLocation(executionState1.CurrentLocation, cmd.LocationMode);
                    Logger.LogInformational(executionState1.OperationContext, "The next location has been set to {0}, based on the location mode.", (object) executionState1.CurrentLocation);
                    if (executionState1.RetryPolicy is IExtendedRetryPolicy retryPolicy)
                    {
                      RetryContext retryContext = new RetryContext(executionState1.RetryCount++, ((StorageCommandBase<T>) cmd).CurrentResult, executionState1.CurrentLocation, cmd.LocationMode);
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
                      shouldRetry = executionState1.RetryPolicy.ShouldRetry(executionState1.RetryCount++, ((StorageCommandBase<T>) cmd).CurrentResult.HttpStatusCode, executionState1.ExceptionRef, out delay, executionState1.OperationContext);
                    if (!(delay < TimeSpan.Zero))
                    {
                      if (!(delay > Constants.MaximumRetryBackoff))
                        goto label_59;
                    }
                    delay = Constants.MaximumRetryBackoff;
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
label_59:
            if (shouldRetry)
            {
              DateTime? operationExpiryTime = executionState1.OperationExpiryTime;
              if (operationExpiryTime.HasValue)
              {
                DateTime dateTime1 = DateTime.Now + delay;
                ref DateTime local = ref dateTime1;
                operationExpiryTime = executionState1.OperationExpiryTime;
                DateTime dateTime2 = operationExpiryTime.Value;
                if (local.CompareTo(dateTime2) > 0)
                  goto label_62;
              }
              Action<StorageCommandBase<T>, Exception, OperationContext> recoveryAction = ((StorageCommandBase<T>) cmd).RecoveryAction;
              if (recoveryAction != null)
                recoveryAction((StorageCommandBase<T>) cmd, executionState1.Cmd.CurrentResult.Exception, executionState1.OperationContext);
              if (delay > TimeSpan.Zero)
                await Task.Delay(delay, token).ConfigureAwait(false);
              Logger.LogInformational(executionState1.OperationContext, "Retrying failed operation.");
              ExecutorBase.FireRetrying<T>(executionState1);
              continue;
            }
label_62:
            Logger.LogError(executionState1.OperationContext, shouldRetry ? "Operation cannot be retried because the maximum execution time has been reached. Failing with {0}." : "Retry policy did not allow for a retry. Failing with {0}.", (object) executionState1.ExceptionRef.Message);
            throw executionState1.ExceptionRef;
          }
          while (shouldRetry);
          throw new NotImplementedException("Unexpected internal storage client error.");
        }
      }
label_75:
      return result;
    }

    private static void ProcessStartOfRequest<T>(
      ExecutionState<T> executionState,
      string startLogMessage,
      CancellationTokenSource timeoutTokenSource = null)
    {
      RESTCommand<T> restCmd = executionState.RestCMD;
      executionState.CurrentOperation = ExecutorOperation.BeginOperation;
      HttpContent httpContent = restCmd.BuildContent != null ? restCmd.BuildContent(restCmd, executionState.OperationContext) : (HttpContent) null;
      Uri uri1 = restCmd.StorageUri.GetUri(executionState.CurrentLocation);
      Uri uri2 = restCmd.Credentials.TransformUri(uri1);
      Logger.LogInformational(executionState.OperationContext, "Starting asynchronous request to {0}.", (object) uri2);
      UriQueryBuilder uriQueryBuilder = new UriQueryBuilder(executionState.RestCMD.Builder);
      executionState.Req = (HttpRequestMessage) restCmd.BuildRequest(restCmd, uri2, uriQueryBuilder, httpContent, restCmd.ServerTimeoutInSeconds, executionState.OperationContext);
      ExecutorBase.ApplyUserHeaders<T>(executionState);
      ExecutorBase.FireSendingRequest<T>(executionState);
      if (executionState.OperationExpiryTime.HasValue)
        timeoutTokenSource?.CancelAfter(executionState.RemainingTimeout);
      else
        timeoutTokenSource?.CancelAfter(int.MaxValue);
    }

    private static async Task ProcessEndOfRequestAsync<T>(
      ExecutionState<T> executionState,
      CancellationToken cancellationToken)
    {
      if (executionState.RestCMD.PostProcessResponseAsync != null)
      {
        executionState.CurrentOperation = ExecutorOperation.PostProcess;
        Logger.LogInformational(executionState.OperationContext, "Processing response body.");
        ExecutionState<T> executionState1 = executionState;
        executionState1.Result = await executionState.RestCMD.PostProcessResponseAsync(executionState.RestCMD, executionState.Resp, executionState.OperationContext, cancellationToken).ConfigureAwait(false);
        executionState1 = (ExecutionState<T>) null;
      }
      if (executionState.RestCMD.DisposeAction != null)
      {
        Logger.LogInformational(executionState.OperationContext, "Disposing action invoked.");
        executionState.RestCMD.DisposeAction(executionState.RestCMD);
        executionState.RestCMD.DisposeAction = (Action<RESTCommand<T>>) null;
      }
      executionState.CurrentOperation = ExecutorOperation.EndOperation;
      Logger.LogInformational(executionState.OperationContext, "Operation completed successfully.");
      executionState.CancelDelegate = (Action) null;
    }
  }
}
