// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.Core.Executor.ExecutorBase
// Assembly: Microsoft.Azure.Storage.Common, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 0978DA65-6954-4A99-9ACB-2EF3D979A5D5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.Common.dll

using Microsoft.Azure.Storage.Core.Util;
using Microsoft.Azure.Storage.RetryPolicies;
using Microsoft.Azure.Storage.Shared.Protocol;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Storage.Core.Executor
{
  internal abstract class ExecutorBase
  {
    protected static void ApplyUserHeaders<T>(ExecutionState<T> executionState) => ExecutorBase.ApplyUserHeaders(executionState.OperationContext, executionState.Req);

    internal static void ApplyUserHeaders(
      OperationContext operationContext,
      HttpRequestMessage request)
    {
      if (!string.IsNullOrEmpty(operationContext.ClientRequestID))
        request.Headers.Add("x-ms-client-request-id", operationContext.ClientRequestID);
      if (!string.IsNullOrEmpty(operationContext.CustomUserAgent))
      {
        request.Headers.UserAgent.TryParseAdd(operationContext.CustomUserAgent);
        request.Headers.UserAgent.Add(new ProductInfoHeaderValue("Azure-Storage", "11.2.3"));
        request.Headers.UserAgent.Add(new ProductInfoHeaderValue(Constants.HeaderConstants.UserAgentComment));
      }
      if (operationContext.UserHeaders == null || operationContext.UserHeaders.Count <= 0)
        return;
      foreach (string key in (IEnumerable<string>) operationContext.UserHeaders.Keys)
        request.Headers.Add(key, operationContext.UserHeaders[key]);
    }

    protected static void StartRequestAttempt<T>(ExecutionState<T> executionState)
    {
      executionState.ExceptionRef = (Exception) null;
      executionState.Cmd.CurrentResult = new RequestResult()
      {
        StartTime = DateTime.Now
      };
      lock (executionState.OperationContext.RequestResults)
      {
        executionState.OperationContext.RequestResults.Add(executionState.Cmd.CurrentResult);
        executionState.Cmd.RequestResults.Add(executionState.Cmd.CurrentResult);
      }
      RESTCommand<T> restCmd = executionState.RestCMD;
      if (restCmd != null)
      {
        if (!restCmd.StorageUri.ValidateLocationMode(restCmd.LocationMode))
          throw new InvalidOperationException("The Uri for the target storage location is not specified. Please consider changing the request's location mode.");
        switch (restCmd.CommandLocationMode)
        {
          case CommandLocationMode.PrimaryOnly:
            if (restCmd.LocationMode == LocationMode.SecondaryOnly)
              throw new InvalidOperationException("This operation can only be executed against the primary storage location.");
            Logger.LogInformational(executionState.OperationContext, "This operation can only be executed against the primary storage location.");
            executionState.CurrentLocation = StorageLocation.Primary;
            restCmd.LocationMode = LocationMode.PrimaryOnly;
            break;
          case CommandLocationMode.SecondaryOnly:
            if (restCmd.LocationMode == LocationMode.PrimaryOnly)
              throw new InvalidOperationException("This operation can only be executed against the secondary storage location.");
            Logger.LogInformational(executionState.OperationContext, "This operation can only be executed against the secondary storage location.");
            executionState.CurrentLocation = StorageLocation.Secondary;
            restCmd.LocationMode = LocationMode.SecondaryOnly;
            break;
        }
      }
      executionState.Cmd.CurrentResult.TargetLocation = executionState.CurrentLocation;
    }

    protected static StorageLocation GetNextLocation(
      StorageLocation lastLocation,
      LocationMode locationMode)
    {
      switch (locationMode)
      {
        case LocationMode.PrimaryOnly:
          return StorageLocation.Primary;
        case LocationMode.PrimaryThenSecondary:
        case LocationMode.SecondaryThenPrimary:
          return lastLocation != StorageLocation.Primary ? StorageLocation.Primary : StorageLocation.Secondary;
        case LocationMode.SecondaryOnly:
          return StorageLocation.Secondary;
        default:
          CommonUtility.ArgumentOutOfRange("LocationMode", (object) locationMode);
          return StorageLocation.Primary;
      }
    }

    protected static void FinishRequestAttempt<T>(ExecutionState<T> executionState)
    {
      executionState.Cmd.CurrentResult.EndTime = DateTime.Now;
      executionState.OperationContext.EndTime = DateTime.Now;
      ExecutorBase.FireRequestCompleted<T>(executionState);
    }

    protected static void FireSendingRequest<T>(ExecutionState<T> executionState)
    {
      RequestEventArgs requestEventArgs = ExecutorBase.GenerateRequestEventArgs<T>(executionState);
      executionState.OperationContext.FireSendingRequest(requestEventArgs);
    }

    protected static void FireResponseReceived<T>(ExecutionState<T> executionState)
    {
      RequestEventArgs requestEventArgs = ExecutorBase.GenerateRequestEventArgs<T>(executionState);
      executionState.OperationContext.FireResponseReceived(requestEventArgs);
    }

    protected static void FireRequestCompleted<T>(ExecutionState<T> executionState)
    {
      RequestEventArgs requestEventArgs = ExecutorBase.GenerateRequestEventArgs<T>(executionState);
      executionState.OperationContext.FireRequestCompleted(requestEventArgs);
    }

    protected static void FireRetrying<T>(ExecutionState<T> executionState)
    {
      RequestEventArgs requestEventArgs = ExecutorBase.GenerateRequestEventArgs<T>(executionState);
      executionState.OperationContext.FireRetrying(requestEventArgs);
    }

    private static RequestEventArgs GenerateRequestEventArgs<T>(ExecutionState<T> executionState) => new RequestEventArgs(executionState.Cmd.CurrentResult)
    {
      Request = executionState.Req,
      Response = executionState.Resp
    };

    protected static bool CheckTimeout<T>(ExecutionState<T> executionState, bool throwOnTimeout)
    {
      if (!executionState.ReqTimedOut && (!executionState.OperationExpiryTime.HasValue || executionState.Cmd.CurrentResult.StartTime.CompareTo(executionState.OperationExpiryTime.Value) <= 0))
        return false;
      executionState.ReqTimedOut = true;
      StorageException timeoutException = Exceptions.GenerateTimeoutException(executionState.Cmd.CurrentResult, (Exception) null);
      executionState.ExceptionRef = (Exception) timeoutException;
      if (throwOnTimeout)
        throw executionState.ExceptionRef;
      return true;
    }

    protected static bool CheckCancellation<T>(
      ExecutionState<T> executionState,
      CancellationToken token,
      bool throwOnCancellation = false)
    {
      if (!CancellationTokenSource.CreateLinkedTokenSource(executionState.CancellationTokenSource.Token, token).IsCancellationRequested)
        return false;
      executionState.ExceptionRef = (Exception) Exceptions.GenerateCancellationException(executionState.Cmd.CurrentResult, (Exception) null);
      if (throwOnCancellation)
        throw executionState.ExceptionRef;
      return true;
    }

    internal static async Task<StorageException> TranslateExceptionBasedOnParseErrorAsync(
      Exception ex,
      RequestResult currentResult,
      HttpResponseMessage response,
      Func<Stream, HttpResponseMessage, string, CancellationToken, Task<StorageExtendedErrorInformation>> parseErrorAsync,
      CancellationToken cancellationToken)
    {
      return parseErrorAsync != null ? await StorageException.TranslateExceptionAsync(ex, currentResult, (Func<Stream, CancellationToken, Task<StorageExtendedErrorInformation>>) (async (stream, token) => await parseErrorAsync(stream, response, (string) null, token).ConfigureAwait(false)), cancellationToken, response).ConfigureAwait(false) : await StorageException.TranslateExceptionAsync(ex, currentResult, (Func<Stream, CancellationToken, Task<StorageExtendedErrorInformation>>) null, cancellationToken, response).ConfigureAwait(false);
    }
  }
}
