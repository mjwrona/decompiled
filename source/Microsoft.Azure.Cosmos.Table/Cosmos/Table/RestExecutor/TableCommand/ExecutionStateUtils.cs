// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Table.RestExecutor.TableCommand.ExecutionStateUtils
// Assembly: Microsoft.Azure.Cosmos.Table, Version=1.0.7.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 461D0B3A-0B96-4D42-B330-3A8E714FC39A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Table.dll

using Microsoft.Azure.Cosmos.Table.RestExecutor.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos.Table.RestExecutor.TableCommand
{
  internal static class ExecutionStateUtils
  {
    internal static void ApplyUserHeaders<T>(ExecutionState<T> executionState)
    {
      if (!string.IsNullOrEmpty(executionState.OperationContext.ClientRequestID))
        executionState.Req.Headers.Add("x-ms-client-request-id", executionState.OperationContext.ClientRequestID);
      if (!string.IsNullOrEmpty(executionState.OperationContext.CustomUserAgent))
      {
        executionState.Req.Headers.UserAgent.TryParseAdd(executionState.OperationContext.CustomUserAgent);
        executionState.Req.Headers.UserAgent.Add(new ProductInfoHeaderValue("Azure-Cosmos-Table", "1.0.7"));
        executionState.Req.Headers.UserAgent.Add(new ProductInfoHeaderValue(TableRestConstants.HeaderConstants.UserAgentComment));
      }
      if (executionState.OperationContext.UserHeaders == null || executionState.OperationContext.UserHeaders.Count <= 0)
        return;
      foreach (string key in (IEnumerable<string>) executionState.OperationContext.UserHeaders.Keys)
        executionState.Req.Headers.Add(key, executionState.OperationContext.UserHeaders[key]);
    }

    internal static void StartRequestAttempt<T>(ExecutionState<T> executionState)
    {
      executionState.ExceptionRef = (Exception) null;
      executionState.Cmd.CurrentResult = new RequestResult()
      {
        StartTime = (DateTimeOffset) DateTime.UtcNow
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

    internal static StorageLocation GetNextLocation(
      StorageLocation lastLocation,
      LocationMode locationMode)
    {
      switch (locationMode)
      {
        case LocationMode.PrimaryOnly:
          return StorageLocation.Primary;
        case LocationMode.PrimaryThenSecondary:
        case LocationMode.SecondaryThenPrimary:
          return lastLocation == StorageLocation.Primary ? StorageLocation.Secondary : StorageLocation.Primary;
        case LocationMode.SecondaryOnly:
          return StorageLocation.Secondary;
        default:
          CommonUtility.ArgumentOutOfRange("LocationMode", (object) locationMode);
          return StorageLocation.Primary;
      }
    }

    internal static void FinishRequestAttempt<T>(ExecutionState<T> executionState)
    {
      DateTime utcNow = DateTime.UtcNow;
      executionState.Cmd.CurrentResult.EndTime = (DateTimeOffset) utcNow;
      executionState.OperationContext.EndTime = (DateTimeOffset) utcNow;
      ExecutionStateUtils.FireRequestCompleted<T>(executionState);
    }

    internal static void FireSendingRequest<T>(ExecutionState<T> executionState)
    {
      RequestEventArgs requestEventArgs = ExecutionStateUtils.GenerateRequestEventArgs<T>(executionState);
      executionState.OperationContext.FireSendingRequest(requestEventArgs);
    }

    internal static void FireResponseReceived<T>(ExecutionState<T> executionState)
    {
      RequestEventArgs requestEventArgs = ExecutionStateUtils.GenerateRequestEventArgs<T>(executionState);
      executionState.OperationContext.FireResponseReceived(requestEventArgs);
    }

    internal static void FireRequestCompleted<T>(ExecutionState<T> executionState)
    {
      RequestEventArgs requestEventArgs = ExecutionStateUtils.GenerateRequestEventArgs<T>(executionState);
      executionState.OperationContext.FireRequestCompleted(requestEventArgs);
    }

    internal static void FireRetrying<T>(ExecutionState<T> executionState)
    {
      RequestEventArgs requestEventArgs = ExecutionStateUtils.GenerateRequestEventArgs<T>(executionState);
      executionState.OperationContext.FireRetrying(requestEventArgs);
    }

    private static RequestEventArgs GenerateRequestEventArgs<T>(ExecutionState<T> executionState) => new RequestEventArgs(executionState.Cmd.CurrentResult)
    {
      Request = (HttpRequestMessage) executionState.Req,
      Response = executionState.Resp
    };

    internal static bool CheckTimeout<T>(ExecutionState<T> executionState, bool throwOnTimeout)
    {
      if (!executionState.ReqTimedOut && (!executionState.OperationExpiryTime.HasValue || executionState.Cmd.CurrentResult.StartTime.CompareTo((DateTimeOffset) executionState.OperationExpiryTime.Value) <= 0))
        return false;
      executionState.ReqTimedOut = true;
      StorageException timeoutException = StorageExceptionTranslator.GenerateTimeoutException(executionState.Cmd.CurrentResult, (Exception) null);
      executionState.ExceptionRef = (Exception) timeoutException;
      if (throwOnTimeout)
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
      return parseErrorAsync != null ? await StorageExceptionTranslator.TranslateExceptionAsync(ex, currentResult, (Func<Stream, CancellationToken, Task<StorageExtendedErrorInformation>>) (async (stream, token) => await parseErrorAsync(stream, response, (string) null, token).ConfigureAwait(false)), cancellationToken, response).ConfigureAwait(false) : await StorageExceptionTranslator.TranslateExceptionAsync(ex, currentResult, (Func<Stream, CancellationToken, Task<StorageExtendedErrorInformation>>) null, cancellationToken, response).ConfigureAwait(false);
    }
  }
}
