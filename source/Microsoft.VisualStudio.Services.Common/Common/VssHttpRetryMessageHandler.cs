// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Common.VssHttpRetryMessageHandler
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using Microsoft.VisualStudio.Services.Common.Diagnostics;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Common
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class VssHttpRetryMessageHandler : DelegatingHandler
  {
    private VssHttpRetryOptions m_retryOptions;
    public const string HttpRetryInfoKey = "HttpRetryInfo";
    public const string HttpRetryOptionsKey = "VssHttpRetryOptions";

    public VssHttpRetryMessageHandler(int maxRetries)
      : this(new VssHttpRetryOptions()
      {
        MaxRetries = maxRetries
      })
    {
    }

    public VssHttpRetryMessageHandler(int maxRetries, string clientName)
      : this(new VssHttpRetryOptions()
      {
        MaxRetries = maxRetries
      })
    {
      this.ClientName = clientName;
    }

    public VssHttpRetryMessageHandler(VssHttpRetryOptions options) => this.m_retryOptions = options;

    public VssHttpRetryMessageHandler(VssHttpRetryOptions options, HttpMessageHandler innerHandler)
      : base(innerHandler)
    {
      this.m_retryOptions = options;
    }

    public string ClientName { get; set; } = "";

    public virtual bool IsCustomTransientException(Exception ex) => false;

    protected override async Task<HttpResponseMessage> SendAsync(
      HttpRequestMessage request,
      CancellationToken cancellationToken)
    {
      VssHttpRetryMessageHandler retryMessageHandler = this;
      int attempt = 1;
      HttpResponseMessage response = (HttpResponseMessage) null;
      Exception exception = (Exception) null;
      VssTraceActivity traceActivity = VssTraceActivity.Current;
      VssHttpRetryOptions httpRetryOptions1 = retryMessageHandler.m_retryOptions;
      object obj1;
      if (request.Properties.TryGetValue("VssHttpRetryOptions", out obj1))
      {
        if (!(obj1 is VssHttpRetryOptions httpRetryOptions2))
          httpRetryOptions2 = retryMessageHandler.m_retryOptions;
        httpRetryOptions1 = httpRetryOptions2;
      }
      TimeSpan minBackoff = httpRetryOptions1.MinBackoff;
      int maxAttempts = httpRetryOptions1.MaxRetries + 1;
      IVssHttpRetryInfo retryInfo = (IVssHttpRetryInfo) null;
      object obj2;
      if (request.Properties.TryGetValue("HttpRetryInfo", out obj2))
        retryInfo = obj2 as IVssHttpRetryInfo;
      if (VssHttpRetryMessageHandler.IsLowPriority(request))
      {
        minBackoff = TimeSpan.FromSeconds(minBackoff.TotalSeconds * 2.0);
        maxAttempts *= 10;
      }
      while (attempt <= maxAttempts)
      {
        exception = (Exception) null;
        SocketError? socketError = new SocketError?();
        HttpStatusCode? statusCode = new HttpStatusCode?();
        WebExceptionStatus? webExceptionStatus = new WebExceptionStatus?();
        WinHttpErrorCode? winHttpErrorCode = new WinHttpErrorCode?();
        CurlErrorCode? curlErrorCode = new CurlErrorCode?();
        string afdRefInfo = (string) null;
        bool flag;
        try
        {
          if (attempt == 1)
            retryInfo?.InitialAttempt(request);
          // ISSUE: reference to a compiler-generated method
          response = await retryMessageHandler.\u003C\u003En__0(request, cancellationToken).ConfigureAwait(false);
          if (attempt > 1)
            retryMessageHandler.TraceHttpRequestSucceededWithRetry(traceActivity, response, attempt);
          if (!response.IsSuccessStatusCode)
          {
            statusCode = new HttpStatusCode?(response.StatusCode);
            IEnumerable<string> values;
            afdRefInfo = response.Headers.TryGetValues("X-MSEdge-Ref", out values) ? values.First<string>() : (string) null;
            flag = retryMessageHandler.m_retryOptions.IsRetryableResponse(response);
          }
          else
            break;
        }
        catch (HttpRequestException ex)
        {
          exception = (Exception) ex;
          flag = VssNetworkHelper.IsTransientNetworkException(exception, retryMessageHandler.m_retryOptions, out statusCode, out webExceptionStatus, out socketError, out winHttpErrorCode, out curlErrorCode);
        }
        catch (TimeoutException ex)
        {
          throw;
        }
        catch (Exception ex)
        {
          if (!retryMessageHandler.IsCustomTransientException(ex))
          {
            throw;
          }
          else
          {
            exception = ex;
            flag = true;
          }
        }
        if (attempt < maxAttempts & flag)
        {
          TimeSpan exponentialBackoff = BackoffTimerHelper.GetExponentialBackoff(attempt, minBackoff, retryMessageHandler.m_retryOptions.MaxBackoff, retryMessageHandler.m_retryOptions.BackoffCoefficient);
          retryInfo?.Retry(exponentialBackoff);
          retryMessageHandler.TraceHttpRequestRetrying(traceActivity, request, attempt, exponentialBackoff, statusCode, webExceptionStatus, socketError, winHttpErrorCode, curlErrorCode, afdRefInfo);
          response?.Dispose();
          ++attempt;
          retryMessageHandler.TraceRaw(request, 100011, TraceLevel.Error, "{{ \"Client\":\"{0}\", \"Endpoint\":\"{1}\", \"Attempt\":{2}, \"MaxAttempts\":{3}, \"Backoff\":{4} }}", (object) retryMessageHandler.ClientName, (object) request.RequestUri.Host, (object) attempt, (object) maxAttempts, (object) exponentialBackoff.TotalMilliseconds);
          await Task.Delay(exponentialBackoff, cancellationToken).ConfigureAwait(false);
          socketError = new SocketError?();
          statusCode = new HttpStatusCode?();
          webExceptionStatus = new WebExceptionStatus?();
          winHttpErrorCode = new WinHttpErrorCode?();
          curlErrorCode = new CurlErrorCode?();
          afdRefInfo = (string) null;
        }
        else
        {
          if (attempt < maxAttempts)
          {
            if (exception == null)
            {
              retryMessageHandler.TraceHttpRequestFailed(traceActivity, request, statusCode.HasValue ? statusCode.Value : (HttpStatusCode) 0, afdRefInfo);
              break;
            }
            retryMessageHandler.TraceHttpRequestFailed(traceActivity, request, exception);
            break;
          }
          retryMessageHandler.TraceHttpRequestFailedMaxAttempts(traceActivity, request, attempt, statusCode, webExceptionStatus, socketError, winHttpErrorCode, curlErrorCode, afdRefInfo);
          break;
        }
      }
      if (exception != null)
        throw exception;
      HttpResponseMessage httpResponseMessage = response;
      response = (HttpResponseMessage) null;
      exception = (Exception) null;
      traceActivity = (VssTraceActivity) null;
      retryInfo = (IVssHttpRetryInfo) null;
      return httpResponseMessage;
    }

    protected virtual void TraceRaw(
      HttpRequestMessage request,
      int tracepoint,
      TraceLevel level,
      string message,
      params object[] args)
    {
    }

    protected virtual void TraceHttpRequestFailed(
      VssTraceActivity activity,
      HttpRequestMessage request,
      HttpStatusCode statusCode,
      string afdRefInfo)
    {
      VssHttpEventSource.Log.HttpRequestFailed(activity, request, statusCode, afdRefInfo);
    }

    protected virtual void TraceHttpRequestFailed(
      VssTraceActivity activity,
      HttpRequestMessage request,
      Exception exception)
    {
      VssHttpEventSource.Log.HttpRequestFailed(activity, request, exception);
    }

    protected virtual void TraceHttpRequestFailedMaxAttempts(
      VssTraceActivity activity,
      HttpRequestMessage request,
      int attempt,
      HttpStatusCode? httpStatusCode,
      WebExceptionStatus? webExceptionStatus,
      SocketError? socketErrorCode,
      WinHttpErrorCode? winHttpErrorCode,
      CurlErrorCode? curlErrorCode,
      string afdRefInfo)
    {
      VssHttpEventSource.Log.HttpRequestFailedMaxAttempts(activity, request, attempt, httpStatusCode, webExceptionStatus, socketErrorCode, winHttpErrorCode, curlErrorCode, afdRefInfo);
    }

    protected virtual void TraceHttpRequestSucceededWithRetry(
      VssTraceActivity activity,
      HttpResponseMessage response,
      int attempt)
    {
      VssHttpEventSource.Log.HttpRequestSucceededWithRetry(activity, response, attempt);
    }

    protected virtual void TraceHttpRequestRetrying(
      VssTraceActivity activity,
      HttpRequestMessage request,
      int attempt,
      TimeSpan backoffDuration,
      HttpStatusCode? httpStatusCode,
      WebExceptionStatus? webExceptionStatus,
      SocketError? socketErrorCode,
      WinHttpErrorCode? winHttpErrorCode,
      CurlErrorCode? curlErrorCode,
      string afdRefInfo)
    {
      VssHttpEventSource.Log.HttpRequestRetrying(activity, request, attempt, backoffDuration, httpStatusCode, webExceptionStatus, socketErrorCode, winHttpErrorCode, curlErrorCode, afdRefInfo);
    }

    private static bool IsLowPriority(HttpRequestMessage request)
    {
      bool flag = false;
      IEnumerable<string> values;
      if (request.Headers.TryGetValues("X-VSS-RequestPriority", out values) && values != null)
        flag = string.Equals(values.FirstOrDefault<string>(), "Low", StringComparison.OrdinalIgnoreCase);
      return flag;
    }
  }
}
