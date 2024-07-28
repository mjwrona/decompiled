// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Common.ArtifactHttpRetryMessageHandler
// Assembly: Microsoft.VisualStudio.Services.Content.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DC45E7D4-4445-41B3-8FA2-C13CD848D0F1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Common.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Common.Diagnostics;
using Microsoft.VisualStudio.Services.Content.Common.Tracing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Content.Common
{
  public class ArtifactHttpRetryMessageHandler : VssHttpRetryMessageHandler
  {
    public const string ExpectedStatusKey = "ExpectedStatus";
    internal static readonly VssHttpRetryOptions DefaultRetryOptions = new VssHttpRetryOptions();
    private readonly IAppTraceSource tracer;
    private readonly Func<int, TimeSpan, TimeSpan, TimeSpan, TimeSpan> GetExponentialBackoff;
    private readonly VssHttpRetryOptions retryOptions;

    static ArtifactHttpRetryMessageHandler()
    {
      ArtifactHttpRetryMessageHandler.DefaultRetryOptions.RetryableStatusCodes.AddRange<HttpStatusCode, ICollection<HttpStatusCode>>((IEnumerable<HttpStatusCode>) new HashSet<HttpStatusCode>()
      {
        HttpStatusCode.RequestTimeout,
        (HttpStatusCode) 429,
        HttpStatusCode.InternalServerError
      });
      ArtifactHttpRetryMessageHandler.DefaultRetryOptions.MakeReadonly();
    }

    public ArtifactHttpRetryMessageHandler(IAppTraceSource tracer)
      : this(tracer, (VssHttpRetryOptions) null, (Func<int, TimeSpan, TimeSpan, TimeSpan, TimeSpan>) null)
    {
    }

    internal ArtifactHttpRetryMessageHandler(
      IAppTraceSource tracer,
      VssHttpRetryOptions options = null,
      Func<int, TimeSpan, TimeSpan, TimeSpan, TimeSpan> getExponentialBackoff = null)
      : base(options ?? ArtifactHttpRetryMessageHandler.DefaultRetryOptions)
    {
      ArgumentUtility.CheckForNull<IAppTraceSource>(tracer, nameof (tracer));
      this.tracer = tracer;
      this.retryOptions = options ?? ArtifactHttpRetryMessageHandler.DefaultRetryOptions;
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      this.GetExponentialBackoff = getExponentialBackoff ?? ArtifactHttpRetryMessageHandler.\u003C\u003EO.\u003C0\u003E__GetExponentialBackoff ?? (ArtifactHttpRetryMessageHandler.\u003C\u003EO.\u003C0\u003E__GetExponentialBackoff = new Func<int, TimeSpan, TimeSpan, TimeSpan, TimeSpan>(BackoffTimerHelper.GetExponentialBackoff));
    }

    internal static HttpClient CreateHttpClientWithRetryHandler(
      IAppTraceSource tracer,
      ArtifactHttpRetryMessageHandler retryHandler)
    {
      HttpClientHandler innerHandler = new HttpClientHandler();
      if (retryHandler == null)
      {
        VssHttpRetryOptions defaultRetryOptions = ArtifactHttpRetryMessageHandler.DefaultRetryOptions;
        retryHandler = new ArtifactHttpRetryMessageHandler(tracer, defaultRetryOptions);
      }
      else
        retryHandler.retryOptions.RetryableStatusCodes.AddRange<HttpStatusCode, ICollection<HttpStatusCode>>((IEnumerable<HttpStatusCode>) ArtifactHttpRetryMessageHandler.DefaultRetryOptions.RetryableStatusCodes);
      DelegatingHandler[] handlers = new DelegatingHandler[1]
      {
        (DelegatingHandler) retryHandler
      };
      bool disposeHandler = true;
      return new HttpClient(ArtifactHttpRetryMessageHandler.CreatePipeline((HttpMessageHandler) innerHandler, (IEnumerable<DelegatingHandler>) handlers), disposeHandler)
      {
        Timeout = TimeSpan.FromMilliseconds(-1.0)
      };
    }

    public static HttpClient CreateHttpClientWithRetryHandler(IAppTraceSource tracer) => ArtifactHttpRetryMessageHandler.CreateHttpClientWithRetryHandler(tracer, (ArtifactHttpRetryMessageHandler) null);

    protected override async Task<HttpResponseMessage> SendAsync(
      HttpRequestMessage request,
      CancellationToken cancellationToken)
    {
      int attempt = 1;
      HttpResponseMessage response = (HttpResponseMessage) null;
      Exception exception = (Exception) null;
      TimeSpan backoff = this.retryOptions.MinBackoff;
      int maxAttempts = (this.retryOptions.MaxRetries > 0 ? this.retryOptions.MaxRetries : 0) + 1;
      string urlWithoutParams = string.IsNullOrEmpty(request.RequestUri.Query) ? request.RequestUri.AbsoluteUri : request.RequestUri.AbsoluteUri.Replace(request.RequestUri.Query, "");
      while (true)
      {
        exception = (Exception) null;
        bool flag;
        try
        {
          response = await this.GetResponseMessage(request, cancellationToken);
          HttpStatusCode[] source;
          if (!request.Properties.TryGetValue<HttpStatusCode[]>("ExpectedStatus", out source))
            source = new HttpStatusCode[0];
          if (response.IsSuccessStatusCode || ((IEnumerable<HttpStatusCode>) source).Contains<HttpStatusCode>(response.StatusCode))
            return response;
          flag = this.retryOptions.IsRetryableResponse(response);
          if (response.StatusCode != HttpStatusCode.SeeOther)
            this.tracer.Info(string.Format("{0}.{1}: {2} attempt {3}/{4} failed with {5} {6}, {7} {8}", (object) nameof (ArtifactHttpRetryMessageHandler), (object) nameof (SendAsync), (object) urlWithoutParams, (object) attempt, (object) maxAttempts, (object) "StatusCode", (object) response.StatusCode, (object) "IsRetryableResponse", (object) flag));
        }
        catch (Exception ex) when (!cancellationToken.IsCancellationRequested)
        {
          if (ex is HttpRequestException exception1)
            exception1.SetHttpMessagesForTracing(request, response);
          exception = ex;
          HttpStatusCode? httpStatusCode;
          WebExceptionStatus? webExceptionStatus;
          SocketError? socketErrorCode;
          WinHttpErrorCode? winHttpErrorCode;
          flag = AsyncHttpRetryHelper.IsTransientException(exception, cancellationToken, this.retryOptions, out httpStatusCode, out webExceptionStatus, out socketErrorCode, out winHttpErrorCode, out CurlErrorCode? _);
          string detailsForTracing = ex.GetHttpMessageDetailsForTracing();
          if (this.tracer.SwitchLevel.HasFlag((System.Enum) SourceLevels.Verbose) & !(ex is TimeoutException))
            this.tracer.Verbose(string.Format("{0}.{1}: {2} attempt {3}/{4} failed with {5}: '{6}', {7} {8}, {9} {10}, {11} {12}, {13} {14}, {15} {16}, {17}", (object) nameof (ArtifactHttpRetryMessageHandler), (object) nameof (SendAsync), (object) urlWithoutParams, (object) attempt, (object) maxAttempts, (object) ex.GetType().Name, (object) ex.Message, (object) "HttpStatusCode", (object) httpStatusCode, (object) "WebExceptionStatus", (object) webExceptionStatus, (object) "SocketError", (object) socketErrorCode, (object) "WinHttpErrorCode", (object) winHttpErrorCode, (object) "IsTransientException", (object) flag, (object) detailsForTracing));
          else if (httpStatusCode.HasValue)
            this.tracer.Info(string.Format("{0}.{1}: {2} attempt {3}/{4} failed with {5}: '{6}', {7}.{8}", (object) nameof (ArtifactHttpRetryMessageHandler), (object) nameof (SendAsync), (object) urlWithoutParams, (object) attempt, (object) maxAttempts, (object) ex.GetType().Name, (object) ex.Message, (object) "HttpStatusCode", (object) httpStatusCode));
          else
            this.tracer.Info(string.Format("{0}.{1}: {2} attempt {3}/{4} failed with {5}: '{6}'", (object) nameof (ArtifactHttpRetryMessageHandler), (object) nameof (SendAsync), (object) urlWithoutParams, (object) attempt, (object) maxAttempts, (object) ex.GetType().Name, (object) ex.Message));
        }
        if (flag && attempt < maxAttempts)
        {
          TimeSpan timeSpan = (TimeSpan?) response?.Headers?.RetryAfter?.Delta ?? this.retryOptions.MinBackoff;
          ++attempt;
          backoff = this.GetExponentialBackoff(attempt, timeSpan, this.retryOptions.MaxBackoff, this.retryOptions.BackoffCoefficient);
          this.tracer.Verbose(string.Format("{0}.{1}: {2} attempt {3}/{4} will retry after {5} {6}", (object) nameof (ArtifactHttpRetryMessageHandler), (object) nameof (SendAsync), (object) urlWithoutParams, (object) attempt, (object) maxAttempts, (object) "backoff", (object) backoff));
          await Task.Delay(backoff, cancellationToken).ConfigureAwait(false);
        }
        else
          break;
      }
      if (exception != null)
      {
        exception.Data[(object) "AttemptCount"] = (object) attempt;
        exception.Data[(object) "LastBackoff"] = (object) (backoff.Ticks / 10000L);
        this.tracer.Verbose(string.Format("{0}.{1}: {2} attempt {3}/{4} throwing {5} with AttemptCount {6}, LastBackoff {7}ms", (object) nameof (ArtifactHttpRetryMessageHandler), (object) nameof (SendAsync), (object) urlWithoutParams, (object) attempt, (object) maxAttempts, (object) exception.GetType().Name, (object) attempt, exception.Data[(object) "LastBackoff"]));
        this.tracer.Info(string.Format("{0}.{1}: {2} attempt {3}/{4} throwing {5} with AttemptCount {6}, LastBackoff {7}ms", (object) nameof (ArtifactHttpRetryMessageHandler), (object) nameof (SendAsync), (object) urlWithoutParams, (object) attempt, (object) maxAttempts, (object) exception.GetType().Name, (object) attempt, exception.Data[(object) "LastBackoff"]));
        throw exception;
      }
      return response;
    }

    protected override void TraceHttpRequestFailed(
      VssTraceActivity activity,
      HttpRequestMessage request,
      Exception exception)
    {
      base.TraceHttpRequestFailed(activity, request, exception);
      this.tracer.Verbose(string.Format("{0}.{1}: {2}, exception {3}, activity {4}", (object) nameof (ArtifactHttpRetryMessageHandler), (object) nameof (TraceHttpRequestFailed), (object) request?.RequestUri, (object) exception?.GetType().Name, (object) activity?.Id));
    }

    protected override void TraceHttpRequestFailed(
      VssTraceActivity activity,
      HttpRequestMessage request,
      HttpStatusCode statusCode,
      string afdRefInfo)
    {
      base.TraceHttpRequestFailed(activity, request, statusCode, afdRefInfo);
      this.tracer.Verbose(string.Format("{0}.{1}: {2}, {3}.{4}, activity {5}", (object) nameof (ArtifactHttpRetryMessageHandler), (object) nameof (TraceHttpRequestFailed), (object) request?.RequestUri, (object) "HttpStatusCode", (object) statusCode, (object) activity?.Id));
    }

    protected override void TraceHttpRequestFailedMaxAttempts(
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
      base.TraceHttpRequestFailedMaxAttempts(activity, request, attempt, httpStatusCode, webExceptionStatus, socketErrorCode, winHttpErrorCode, curlErrorCode, afdRefInfo);
      this.tracer.Verbose(string.Format("{0}.{1}: {2}, attempt {3}, {4}.{5}, {6}.{7}, {8}.{9}, {10}.{11}, {12}.{13}, activity {14}", (object) nameof (ArtifactHttpRetryMessageHandler), (object) nameof (TraceHttpRequestFailedMaxAttempts), (object) request?.RequestUri, (object) attempt, (object) "HttpStatusCode", (object) httpStatusCode, (object) "WebExceptionStatus", (object) webExceptionStatus, (object) "SocketError", (object) socketErrorCode, (object) "WinHttpErrorCode", (object) winHttpErrorCode, (object) "CurlErrorCode", (object) curlErrorCode, (object) activity?.Id));
    }

    protected override void TraceHttpRequestRetrying(
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
      base.TraceHttpRequestRetrying(activity, request, attempt, backoffDuration, httpStatusCode, webExceptionStatus, socketErrorCode, winHttpErrorCode, curlErrorCode, afdRefInfo);
      this.tracer.Verbose(string.Format("{0}.{1}: {2}, attempt {3}, backoff {4}, {5}.{6}, {7}.{8}, {9}.{10}, {11}.{12}, {13}.{14}, activity {15}", (object) nameof (ArtifactHttpRetryMessageHandler), (object) nameof (TraceHttpRequestRetrying), (object) request?.RequestUri, (object) attempt, (object) backoffDuration, (object) "HttpStatusCode", (object) httpStatusCode, (object) "WebExceptionStatus", (object) webExceptionStatus, (object) "SocketError", (object) socketErrorCode, (object) "WinHttpErrorCode", (object) winHttpErrorCode, (object) "CurlErrorCode", (object) curlErrorCode, (object) activity?.Id));
    }

    protected override void TraceHttpRequestSucceededWithRetry(
      VssTraceActivity activity,
      HttpResponseMessage response,
      int attempt)
    {
      base.TraceHttpRequestSucceededWithRetry(activity, response, attempt);
      this.tracer.Verbose(string.Format("{0}.{1}: {2}.{3}, attempt {4}, activity {5}", (object) nameof (ArtifactHttpRetryMessageHandler), (object) nameof (TraceHttpRequestSucceededWithRetry), (object) "HttpStatusCode", (object) response?.StatusCode, (object) attempt, (object) activity?.Id));
    }

    protected override void TraceRaw(
      HttpRequestMessage request,
      int tracepoint,
      TraceLevel level,
      string message,
      params object[] args)
    {
      base.TraceRaw(request, tracepoint, level, message, args);
      this.tracer.Verbose(string.Format("{0}.{1}: {2}, tracepoint {3}, {4}.{5}, message {6}", (object) nameof (ArtifactHttpRetryMessageHandler), (object) nameof (TraceRaw), (object) request?.RequestUri, (object) tracepoint, (object) "TraceLevel", (object) level, (object) SafeStringFormat.FormatSafe(message, args)));
    }

    protected virtual async Task<HttpResponseMessage> GetResponseMessage(
      HttpRequestMessage request,
      CancellationToken cancellationToken)
    {
      return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
    }

    private static HttpMessageHandler CreatePipeline(
      HttpMessageHandler innerHandler,
      IEnumerable<DelegatingHandler> handlers)
    {
      ArgumentUtility.CheckForNull<HttpMessageHandler>(innerHandler, nameof (innerHandler));
      if (handlers == null)
        return innerHandler;
      HttpMessageHandler pipeline = innerHandler;
      foreach (DelegatingHandler delegatingHandler in handlers.Reverse<DelegatingHandler>())
      {
        if (delegatingHandler == null)
          throw new ArgumentException("System.Net.Http.Properties.Resources.DelegatingHandlerArrayContainsNullItem", nameof (handlers));
        delegatingHandler.InnerHandler = delegatingHandler.InnerHandler == null ? pipeline : throw new ArgumentException("System.Net.Http.Properties.Resources.DelegatingHandlerArrayHasNonNullInnerHandler", nameof (handlers));
        pipeline = (HttpMessageHandler) delegatingHandler;
      }
      return pipeline;
    }
  }
}
