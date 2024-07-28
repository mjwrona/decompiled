// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Common.AsyncHttpRetryHelper
// Assembly: Microsoft.VisualStudio.Services.Content.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DC45E7D4-4445-41B3-8FA2-C13CD848D0F1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Common.dll

using Microsoft.Azure.Storage;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Content.Common.Tracing;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Content.Common
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class AsyncHttpRetryHelper : AsyncHttpRetryHelper<int>
  {
    public static Task<TResult> InvokeAsync<TResult>(
      Func<Task<TResult>> taskGenerator,
      int maxRetries,
      IAppTraceSource tracer,
      CancellationToken cancellationToken,
      bool continueOnCapturedContext,
      string context = null)
    {
      return AsyncHttpRetryHelper.InvokeAsync<TResult>(taskGenerator, maxRetries, tracer, (Func<Exception, bool>) null, cancellationToken, continueOnCapturedContext, context);
    }

    public static Task<TResult> InvokeAsync<TResult>(
      Func<Task<TResult>> taskGenerator,
      int maxRetries,
      IAppTraceSource tracer,
      Func<Exception, bool> canRetryDelegate,
      CancellationToken cancellationToken,
      bool continueOnCapturedContext,
      string context = null)
    {
      return new AsyncHttpRetryHelper<TResult>(taskGenerator, maxRetries, tracer, continueOnCapturedContext, context, canRetryDelegate).InvokeAsync(cancellationToken);
    }

    public static Task InvokeVoidAsync(
      Func<Task> taskGenerator,
      int maxRetries,
      IAppTraceSource tracer,
      CancellationToken cancellationToken,
      bool continueOnCapturedContext,
      string context)
    {
      return AsyncHttpRetryHelper.InvokeVoidAsync(taskGenerator, maxRetries, tracer, (Func<Exception, bool>) null, cancellationToken, continueOnCapturedContext, context);
    }

    public AsyncHttpRetryHelper(
      Func<Task> taskGenerator,
      int maxRetries,
      IAppTraceSource tracer,
      bool continueOnCapturedContext,
      string context,
      Func<Exception, bool> canRetryDelegate = null)
      : base(AsyncHttpRetryHelper.CreateTaskWrapper(taskGenerator, continueOnCapturedContext), maxRetries, tracer, continueOnCapturedContext, context, canRetryDelegate)
    {
    }

    public static Task InvokeVoidAsync(
      Func<Task> taskGenerator,
      int maxRetries,
      IAppTraceSource tracer,
      Func<Exception, bool> canRetryDelegate,
      CancellationToken cancellationToken,
      bool continueOnCapturedContext,
      string context)
    {
      return (Task) AsyncHttpRetryHelper<int>.InvokeAsync(AsyncHttpRetryHelper.CreateTaskWrapper(taskGenerator, continueOnCapturedContext), maxRetries, tracer, canRetryDelegate, cancellationToken, continueOnCapturedContext, context);
    }

    public static bool IsTransientException(
      Exception exception,
      CancellationToken cancellationToken,
      VssHttpRetryOptions retryOptions = null)
    {
      return AsyncHttpRetryHelper.IsTransientException(exception, cancellationToken, retryOptions, out HttpStatusCode? _, out WebExceptionStatus? _, out SocketError? _, out WinHttpErrorCode? _, out CurlErrorCode? _);
    }

    internal static bool IsTransientException(
      Exception exception,
      CancellationToken cancellationToken,
      VssHttpRetryOptions retryOptions,
      out HttpStatusCode? httpStatusCode,
      out WebExceptionStatus? webExceptionStatus,
      out SocketError? socketErrorCode,
      out WinHttpErrorCode? winHttpErrorCode,
      out CurlErrorCode? curlErrorCode)
    {
      if (retryOptions == null)
        retryOptions = new VssHttpRetryOptions();
      return VssNetworkHelper.IsTransientNetworkException(exception, retryOptions, out httpStatusCode, out webExceptionStatus, out socketErrorCode, out winHttpErrorCode, out curlErrorCode) || AsyncHttpRetryHelper.IsArtifactTransientExceptionRecurse(exception, cancellationToken);
    }

    private static IEnumerable<Exception> EnumerateInnerExceptions(
      Exception rootException,
      int depthLeft = 10)
    {
      yield return rootException;
      if (depthLeft > 0)
      {
        IEnumerable<Exception> exceptions;
        if (rootException is AggregateException aggregateException)
          exceptions = (IEnumerable<Exception>) aggregateException.InnerExceptions;
        else if (rootException.InnerException == null)
          yield break;
        else
          exceptions = (IEnumerable<Exception>) new Exception[1]
          {
            rootException.InnerException
          };
        foreach (Exception rootException1 in exceptions)
        {
          foreach (Exception enumerateInnerException in AsyncHttpRetryHelper.EnumerateInnerExceptions(rootException1, depthLeft - 1))
            yield return enumerateInnerException;
        }
      }
    }

    private static bool IsArtifactTransientExceptionRecurse(
      Exception rootException,
      CancellationToken cancellationToken)
    {
      foreach (Exception enumerateInnerException in AsyncHttpRetryHelper.EnumerateInnerExceptions(rootException))
      {
        if (enumerateInnerException is AsyncHttpRetryHelper.RetryableException)
          return true;
        if (enumerateInnerException is StorageException storageException)
        {
          if (!"ServerBusy".Equals(storageException.RequestInformation?.ErrorCode, StringComparison.Ordinal))
          {
            RequestResult requestInformation1 = storageException.RequestInformation;
            if ((requestInformation1 != null ? (requestInformation1.HttpStatusCode == 500 ? 1 : 0) : 0) == 0)
            {
              RequestResult requestInformation2 = storageException.RequestInformation;
              if ((requestInformation2 != null ? (requestInformation2.HttpStatusCode == 503 ? 1 : 0) : 0) == 0 && !enumerateInnerException.Message.Contains("500") && !enumerateInnerException.Message.Contains("503"))
                goto label_9;
            }
          }
          return true;
        }
label_9:
        if (enumerateInnerException.GetType().FullName.Equals("Microsoft.Azure.Cosmos.Table.StorageException", StringComparison.OrdinalIgnoreCase) && (enumerateInnerException.Message.Contains("500") || enumerateInnerException.Message.Contains("503")) || enumerateInnerException.GetType().Name == "CurlException")
          return true;
        switch (enumerateInnerException)
        {
          case WebException _:
            return true;
          case TimeoutException _:
            return true;
          case OperationCanceledException canceledException when canceledException.CancellationToken == CancellationToken.None || canceledException.CancellationToken != cancellationToken:
            return true;
          case HttpRequestException _ when enumerateInnerException.Message.Contains("408") || enumerateInnerException.Message.Contains("429") || enumerateInnerException.Message.Contains("500") || enumerateInnerException.Message.Contains("502") || enumerateInnerException.Message.Contains("503") || enumerateInnerException.Message.Contains("504"):
            return true;
          default:
            if (AsyncHttpRetryHelper.IsServiceRequestExceptionTransient(enumerateInnerException))
              return true;
            switch (enumerateInnerException)
            {
              case IOException _:
                return true;
              case SocketException _:
                return true;
              case Win32Exception _ when enumerateInnerException.GetType().Name.Equals("WinHttpException", StringComparison.Ordinal):
                return true;
              case ObjectDisposedException disposedException when disposedException.ObjectName.Equals("SslStream", StringComparison.Ordinal):
                return true;
              default:
                continue;
            }
        }
      }
      return false;
    }

    private static bool IsServiceRequestExceptionTransient(Exception exception) => exception is VssServiceResponseException responseException && AsyncHttpRetryHelper<int>.RetryableHttpStatusCodes.Contains((int) responseException.HttpStatusCode);

    private static Func<Task<int>> CreateTaskWrapper(
      Func<Task> taskGenerator,
      bool continueOnCapturedContext)
    {
      return (Func<Task<int>>) (async () =>
      {
        await taskGenerator().ConfigureAwait(continueOnCapturedContext);
        return 0;
      });
    }

    [Serializable]
    public class RetryableException : Exception
    {
      public RetryableException(string message)
        : base(message)
      {
      }

      public RetryableException(string message, Exception ex)
        : base(message, ex)
      {
      }

      public RetryableException(SerializationInfo info, StreamingContext context)
        : base(info, context)
      {
      }
    }
  }
}
