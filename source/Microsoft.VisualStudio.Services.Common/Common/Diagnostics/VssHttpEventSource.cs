// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Common.Diagnostics.VssHttpEventSource
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;

namespace Microsoft.VisualStudio.Services.Common.Diagnostics
{
  [EventSource(Name = "Microsoft-VSS-Http")]
  internal sealed class VssHttpEventSource : EventSource
  {
    private static Lazy<VssHttpEventSource> m_log = new Lazy<VssHttpEventSource>((Func<VssHttpEventSource>) (() => new VssHttpEventSource()));

    internal static VssHttpEventSource Log => VssHttpEventSource.m_log.Value;

    [NonEvent]
    public void AuthenticationStart(VssTraceActivity activity)
    {
      if (!this.IsEnabled())
        return;
      this.SetActivityId(activity);
      this.AuthenticationStart();
    }

    [NonEvent]
    public void AuthenticationStop(VssTraceActivity activity)
    {
      if (!this.IsEnabled())
        return;
      this.SetActivityId(activity);
      this.AuthenticationStop();
    }

    [NonEvent]
    public void AuthenticationError(
      VssTraceActivity activity,
      IssuedTokenProvider provider,
      string message)
    {
      if (!this.IsEnabled(EventLevel.Error, (EventKeywords) 1))
        return;
      this.SetActivityId(activity);
      this.WriteMessageEvent(provider.CredentialType, provider.GetHashCode(), message, new Action<VssCredentialsType, int, string>(this.AuthenticationError));
    }

    [NonEvent]
    public void AuthenticationError(
      VssTraceActivity activity,
      IssuedTokenProvider provider,
      Exception exception)
    {
      if (!this.IsEnabled(EventLevel.Error, (EventKeywords) 1))
        return;
      if (exception is AggregateException)
        exception = ((AggregateException) exception).Flatten().InnerException;
      this.AuthenticationError(activity, provider, exception.ToString());
    }

    [NonEvent]
    public void HttpOperationStart(VssTraceActivity activity, string area, string operation)
    {
      if (!this.IsEnabled())
        return;
      this.SetActivityId(activity);
      this.HttpOperationStart(area, operation);
    }

    [NonEvent]
    public void HttpOperationStop(VssTraceActivity activity, string area, string operation)
    {
      if (!this.IsEnabled())
        return;
      this.SetActivityId(activity);
      this.HttpOperationStop(area, operation);
    }

    [NonEvent]
    public void HttpRequestStart(VssTraceActivity activity, HttpRequestMessage request)
    {
      if (!this.IsEnabled())
        return;
      this.SetActivityId(activity);
      this.HttpRequestStart(request.GetHttpMethod(), VssHttpEventSource.SanitizeUrl(request.RequestUri));
    }

    [NonEvent]
    public Exception HttpRequestFailed(
      VssTraceActivity activity,
      HttpRequestMessage request,
      Exception exception)
    {
      if (this.IsEnabled())
        this.HttpRequestFailed(activity, request, exception.ToString());
      return exception;
    }

    [NonEvent]
    public void HttpRequestFailed(
      VssTraceActivity activity,
      HttpRequestMessage request,
      string message)
    {
      if (!this.IsEnabled())
        return;
      this.SetActivityId(activity);
      this.WriteMessageEvent(request.GetHttpMethod(), VssHttpEventSource.SanitizeUrl(request.RequestUri), message, new Action<VssHttpMethod, string, string>(this.HttpRequestFailed));
    }

    [NonEvent]
    public void HttpRequestFailed(
      VssTraceActivity activity,
      HttpRequestMessage request,
      HttpStatusCode statusCode,
      string afdRefInfo)
    {
      if (!this.IsEnabled())
        return;
      this.SetActivityId(activity);
      string message = string.Format((IFormatProvider) CultureInfo.InstalledUICulture, "HTTP Status: {0}", (object) statusCode);
      if (!string.IsNullOrEmpty(afdRefInfo))
        message = message + ", AFD Ref: " + afdRefInfo;
      this.WriteMessageEvent(request.GetHttpMethod(), VssHttpEventSource.SanitizeUrl(request.RequestUri), message, new Action<VssHttpMethod, string, string>(this.HttpRequestFailed));
    }

    [NonEvent]
    public void HttpRequestUnauthorized(
      VssTraceActivity activity,
      HttpRequestMessage request,
      string message)
    {
      if (!this.IsEnabled())
        return;
      this.SetActivityId(activity);
      this.HttpRequestUnauthorized(request.GetHttpMethod(), VssHttpEventSource.SanitizeUrl(request.RequestUri), message);
    }

    [NonEvent]
    public void HttpRequestSucceeded(VssTraceActivity activity, HttpResponseMessage response)
    {
      if (!this.IsEnabled())
        return;
      this.SetActivityId(activity);
      this.HttpRequestSucceeded(response.RequestMessage.GetHttpMethod(), VssHttpEventSource.SanitizeUrl(response.RequestMessage.RequestUri), (int) response.StatusCode);
    }

    [NonEvent]
    public void HttpRequestRetrying(
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
      if (!this.IsEnabled())
        return;
      string reason = "<unknown>";
      if (httpStatusCode.HasValue)
        reason = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "HTTP Status: {0}", (object) httpStatusCode.Value);
      else if (webExceptionStatus.HasValue)
        reason = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Web Exception Status: {0}", (object) webExceptionStatus.Value);
      else if (socketErrorCode.HasValue)
        reason = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Socket Error: {0}", (object) socketErrorCode.Value);
      else if (winHttpErrorCode.HasValue)
        reason = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "WinHttp Error: {0}", (object) winHttpErrorCode);
      else if (curlErrorCode.HasValue)
        reason = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Curl Error: {0}", (object) curlErrorCode);
      if (!string.IsNullOrEmpty(afdRefInfo))
        reason = reason + ", AFD Ref: " + afdRefInfo;
      this.SetActivityId(activity);
      this.HttpRequestRetrying(request.GetHttpMethod(), VssHttpEventSource.SanitizeUrl(request.RequestUri), attempt, reason, backoffDuration.TotalSeconds);
    }

    [NonEvent]
    public void HttpRequestFailedMaxAttempts(
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
      if (!this.IsEnabled())
        return;
      string reason = "<unknown>";
      if (httpStatusCode.HasValue)
        reason = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "HTTP Status: {0}", (object) httpStatusCode.Value);
      else if (webExceptionStatus.HasValue)
        reason = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Web Exception Status: {0}", (object) webExceptionStatus.Value);
      else if (socketErrorCode.HasValue)
        reason = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Socket Error: {0}", (object) socketErrorCode.Value);
      else if (winHttpErrorCode.HasValue)
        reason = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "WinHttp Error: {0}", (object) winHttpErrorCode);
      else if (curlErrorCode.HasValue)
        reason = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Curl Error: {0}", (object) curlErrorCode);
      if (!string.IsNullOrEmpty(afdRefInfo))
        reason = reason + ", AFD Ref: " + afdRefInfo;
      this.SetActivityId(activity);
      this.HttpRequestFailedMaxAttempts(request.GetHttpMethod(), VssHttpEventSource.SanitizeUrl(request.RequestUri), attempt, reason);
    }

    [NonEvent]
    public void HttpRequestSucceededWithRetry(
      VssTraceActivity activity,
      HttpResponseMessage response,
      int attempt)
    {
      if (!this.IsEnabled())
        return;
      this.SetActivityId(activity);
      this.HttpRequestSucceededWithRetry(response.RequestMessage.GetHttpMethod(), VssHttpEventSource.SanitizeUrl(response.RequestMessage.RequestUri), attempt);
    }

    [NonEvent]
    public void HttpRequestCancelled(VssTraceActivity activity, HttpRequestMessage request)
    {
      if (!this.IsEnabled())
        return;
      this.SetActivityId(activity);
      this.HttpRequestCancelled(request.GetHttpMethod(), VssHttpEventSource.SanitizeUrl(request.RequestUri));
    }

    [NonEvent]
    public void HttpRequestTimedOut(
      VssTraceActivity activity,
      HttpRequestMessage request,
      TimeSpan timeout)
    {
      if (!this.IsEnabled())
        return;
      this.SetActivityId(activity);
      this.HttpRequestTimedOut(request.GetHttpMethod(), VssHttpEventSource.SanitizeUrl(request.RequestUri), (int) timeout.TotalSeconds);
    }

    [NonEvent]
    public void HttpRequestStop(VssTraceActivity activity, HttpResponseMessage response)
    {
      if (!this.IsEnabled())
        return;
      this.SetActivityId(activity);
      this.HttpRequestStop(response.RequestMessage.GetHttpMethod(), VssHttpEventSource.SanitizeUrl(response.RequestMessage.RequestUri), (int) response.StatusCode);
    }

    [NonEvent]
    public void AuthenticationFailed(VssTraceActivity activity, HttpResponseMessage response)
    {
      if (!this.IsEnabled())
        return;
      this.SetActivityId(activity);
      this.WriteMessageEvent((int) response.StatusCode, response.Headers.ToString(), new Action<int, string>(this.AuthenticationFailed));
    }

    [NonEvent]
    public void IssuedTokenProviderCreated(VssTraceActivity activity, IssuedTokenProvider provider)
    {
      if (!this.IsEnabled())
        return;
      this.SetActivityId(activity);
      this.IssuedTokenProviderCreated(provider.CredentialType, provider.GetHashCode(), provider.GetAuthenticationParameters());
    }

    [NonEvent]
    public void IssuedTokenProviderRemoved(VssTraceActivity activity, IssuedTokenProvider provider)
    {
      if (!this.IsEnabled())
        return;
      this.SetActivityId(activity);
      this.IssuedTokenProviderRemoved(provider.CredentialType, provider.GetHashCode(), provider.GetAuthenticationParameters());
    }

    [NonEvent]
    internal void IssuedTokenProviderNotFound(VssTraceActivity activity)
    {
      if (!this.IsEnabled())
        return;
      this.SetActivityId(activity);
      this.IssuedTokenProviderNotFound();
    }

    [NonEvent]
    internal void IssuedTokenProviderPromptRequired(
      VssTraceActivity activity,
      IssuedTokenProvider provider)
    {
      if (!this.IsEnabled())
        return;
      this.SetActivityId(activity);
      this.IssuedTokenProviderPromptRequired(provider.CredentialType, provider.GetHashCode());
    }

    [NonEvent]
    public void IssuedTokenAcquiring(VssTraceActivity activity, IssuedTokenProvider provider)
    {
      if (!this.IsEnabled())
        return;
      this.SetActivityId(activity);
      this.IssuedTokenAcquiring(provider.CredentialType, provider.GetHashCode());
    }

    [NonEvent]
    public void IssuedTokenWaitStart(
      VssTraceActivity activity,
      IssuedTokenProvider provider,
      Guid waitForActivityId)
    {
      if (!this.IsEnabled(EventLevel.Verbose, (EventKeywords) 1))
        return;
      this.SetActivityId(activity);
      this.IssuedTokenWaitStart(provider.CredentialType, provider.GetHashCode(), waitForActivityId);
    }

    [NonEvent]
    public void IssuedTokenWaitStop(
      VssTraceActivity activity,
      IssuedTokenProvider provider,
      IssuedToken token)
    {
      if (!this.IsEnabled(EventLevel.Verbose, (EventKeywords) 1))
        return;
      this.SetActivityId(activity);
      this.IssuedTokenWaitStop(provider.CredentialType, provider.GetHashCode(), token != null ? token.GetHashCode() : 0);
    }

    [NonEvent]
    public void IssuedTokenAcquired(
      VssTraceActivity activity,
      IssuedTokenProvider provider,
      IssuedToken token)
    {
      if (!this.IsEnabled())
        return;
      this.SetActivityId(activity);
      this.IssuedTokenAcquired(provider.CredentialType, provider.GetHashCode(), token != null ? token.GetHashCode() : 0);
    }

    [NonEvent]
    public void IssuedTokenInvalidated(
      VssTraceActivity activity,
      IssuedTokenProvider provider,
      IssuedToken token)
    {
      if (!this.IsEnabled())
        return;
      this.SetActivityId(activity);
      this.IssuedTokenInvalidated(provider.CredentialType, provider.GetHashCode(), token.GetHashCode());
    }

    [NonEvent]
    public void IssuedTokenValidated(
      VssTraceActivity activity,
      IssuedTokenProvider provider,
      IssuedToken token)
    {
      if (!this.IsEnabled())
        return;
      this.SetActivityId(activity);
      this.IssuedTokenValidated(provider.CredentialType, provider.GetHashCode(), token.GetHashCode());
    }

    [NonEvent]
    public void IssuedTokenRetrievedFromCache(
      VssTraceActivity activity,
      IssuedTokenProvider provider,
      IssuedToken token)
    {
      if (!this.IsEnabled())
        return;
      this.SetActivityId(activity);
      this.IssuedTokenRetrievedFromCache(provider.CredentialType, provider.GetHashCode(), token.GetHashCode());
    }

    [NonEvent]
    public static string SanitizeUrl(Uri url) => new UriBuilder(url)
    {
      UserName = ((string) null),
      Password = ((string) null),
      Query = ((string) null),
      Fragment = ((string) null)
    }.Uri.AbsoluteUri;

    [Event(1, Level = EventLevel.Verbose, Task = (EventTask) 1, Opcode = EventOpcode.Start, Message = "Started {0} request to {1}")]
    private void HttpRequestStart(VssHttpMethod method, string url)
    {
      if (!this.IsEnabled())
        return;
      this.WriteEvent(1, (int) method, url);
    }

    [Event(2, Level = EventLevel.Error, Task = (EventTask) 1, Opcode = EventOpcode.Info, Message = "{0} request to {1} failed. {2}")]
    private void HttpRequestFailed(VssHttpMethod method, string url, string message)
    {
      if (!this.IsEnabled())
        return;
      this.WriteEvent(2, (int) method, url, message);
    }

    [Event(3, Level = EventLevel.Informational, Task = (EventTask) 1, Opcode = EventOpcode.Info, Message = "{0} request to {1} succeeded with status code {2}")]
    private void HttpRequestSucceeded(VssHttpMethod method, string url, int statusCode)
    {
      if (!this.IsEnabled())
        return;
      this.WriteEvent(3, (int) method, url, statusCode);
    }

    [Event(4, Level = EventLevel.Warning, Task = (EventTask) 1, Opcode = EventOpcode.Info, Message = "Attempt {2} of {0} request to {1} failed ({3}). The operation will be retried in {4} seconds.")]
    private void HttpRequestRetrying(
      VssHttpMethod method,
      string url,
      int attempt,
      string reason,
      double backoffDurationInSeconds)
    {
      if (!this.IsEnabled())
        return;
      this.WriteEvent(4, (int) method, url, attempt, reason, backoffDurationInSeconds);
    }

    [Event(5, Level = EventLevel.Error, Task = (EventTask) 1, Opcode = EventOpcode.Info, Message = "Attempt {2} of {0} request to {1} failed ({3}). The maximum number of attempts has been reached.")]
    private void HttpRequestFailedMaxAttempts(
      VssHttpMethod method,
      string url,
      int attempt,
      string reason)
    {
      if (!this.IsEnabled())
        return;
      this.WriteEvent(5, (int) method, url, attempt, reason);
    }

    [Event(6, Level = EventLevel.Verbose, Task = (EventTask) 1, Opcode = EventOpcode.Info, Message = "Attempt {2} of {0} request to {1} succeeded.")]
    private void HttpRequestSucceededWithRetry(VssHttpMethod method, string url, int attempt)
    {
      if (!this.IsEnabled())
        return;
      this.WriteEvent(6, (int) method, url, attempt);
    }

    [Event(7, Level = EventLevel.Warning, Task = (EventTask) 1, Opcode = EventOpcode.Info, Message = "{0} request to {1} has been cancelled.")]
    private void HttpRequestCancelled(VssHttpMethod method, string url)
    {
      if (!this.IsEnabled())
        return;
      this.WriteEvent(7, (int) method, url);
    }

    [Event(8, Level = EventLevel.Warning, Task = (EventTask) 1, Opcode = EventOpcode.Info, Message = "{0} request to {1} timed out after {2} seconds.")]
    private void HttpRequestTimedOut(VssHttpMethod method, string url, int timeoutInSeconds)
    {
      if (!this.IsEnabled())
        return;
      this.WriteEvent(8, (int) method, url, timeoutInSeconds);
    }

    [Event(9, Level = EventLevel.Error, Task = (EventTask) 1, Opcode = EventOpcode.Info, Message = "{0} request to {1} is not authorized. Details: {2}")]
    private void HttpRequestUnauthorized(VssHttpMethod method, string url, string message)
    {
      if (!this.IsEnabled())
        return;
      this.WriteEvent(9, (int) method, url, message);
    }

    [Event(10, Keywords = (EventKeywords) 1, Level = EventLevel.Warning, Task = (EventTask) 1, Message = "Authentication failed with status code {0}.%n{1}")]
    private void AuthenticationFailed(int statusCode, string headers)
    {
      if (!this.IsEnabled(EventLevel.Verbose, (EventKeywords) 1))
        return;
      this.WriteEvent(10, statusCode, headers);
    }

    [Event(11, Keywords = (EventKeywords) 1, Level = EventLevel.Informational, Task = (EventTask) 1, Message = "Authentication successful using {0} credentials")]
    private void AuthenticationSucceeded(VssCredentialsType credentialsType)
    {
      if (!this.IsEnabled(EventLevel.Verbose, (EventKeywords) 1))
        return;
      this.WriteEvent(11, (int) credentialsType);
    }

    [Event(12, Keywords = (EventKeywords) 1, Level = EventLevel.Verbose, Task = (EventTask) 2, Opcode = EventOpcode.Start, Message = "Started authentication")]
    private void AuthenticationStart()
    {
      if (!this.IsEnabled(EventLevel.Verbose, (EventKeywords) 1))
        return;
      this.WriteEvent(12);
    }

    [Event(13, Keywords = (EventKeywords) 1, Level = EventLevel.Verbose, Task = (EventTask) 2, Message = "Created {0} issued token provider instance {1} ({2})")]
    private void IssuedTokenProviderCreated(
      VssCredentialsType credentialsType,
      int providerId,
      string parameters)
    {
      if (!this.IsEnabled())
        return;
      this.WriteEvent(13, (int) credentialsType, providerId, parameters);
    }

    [Event(14, Keywords = (EventKeywords) 1, Level = EventLevel.Verbose, Task = (EventTask) 2, Message = "Removed {0} issued token provider instance {1} ({2})")]
    private void IssuedTokenProviderRemoved(
      VssCredentialsType credentialsType,
      int providerId,
      string parameters)
    {
      if (!this.IsEnabled(EventLevel.Verbose, (EventKeywords) 1))
        return;
      this.WriteEvent(14, (int) credentialsType, providerId, parameters);
    }

    [Event(15, Keywords = (EventKeywords) 1, Level = EventLevel.Verbose, Task = (EventTask) 2, Message = "{0} issued token provider instance {1} is acquiring a token")]
    private void IssuedTokenAcquiring(VssCredentialsType credentialsType, int providerId)
    {
      if (!this.IsEnabled(EventLevel.Verbose, (EventKeywords) 1))
        return;
      this.WriteEvent(15, (int) credentialsType, providerId);
    }

    [Event(16, Keywords = (EventKeywords) 1, Level = EventLevel.Verbose, Task = (EventTask) 2, Opcode = EventOpcode.Suspend, Message = "{0} issued token provider instance {1} is waiting for issued token from activity {2}")]
    private void IssuedTokenWaitStart(
      VssCredentialsType credentialsType,
      int providerId,
      Guid waitForActivityId)
    {
      this.WriteEvent(16, (int) credentialsType, providerId, waitForActivityId);
    }

    [Event(17, Keywords = (EventKeywords) 1, Level = EventLevel.Verbose, Task = (EventTask) 2, Opcode = EventOpcode.Resume, Message = "{0} issued token provider instance {1} received token instance {2}")]
    private void IssuedTokenWaitStop(
      VssCredentialsType credentialsType,
      int providerId,
      int issuedTokenId)
    {
      this.WriteEvent(17, (int) credentialsType, providerId, issuedTokenId);
    }

    [Event(18, Keywords = (EventKeywords) 1, Level = EventLevel.Verbose, Task = (EventTask) 2, Message = "{0} issued token provider instance {1} acquired new token instance {2}")]
    private void IssuedTokenAcquired(
      VssCredentialsType credentialsType,
      int providerId,
      int issuedTokenId)
    {
      if (!this.IsEnabled(EventLevel.Verbose, (EventKeywords) 1))
        return;
      this.WriteEvent(18, (int) credentialsType, providerId, issuedTokenId);
    }

    [Event(20, Keywords = (EventKeywords) 1, Level = EventLevel.Verbose, Task = (EventTask) 2, Message = "{0} issued token provider instance {1} invalidated token instance {2}")]
    private void IssuedTokenInvalidated(
      VssCredentialsType credentialsType,
      int providerId,
      int issuedTokenId)
    {
      if (!this.IsEnabled(EventLevel.Verbose, (EventKeywords) 1))
        return;
      this.WriteEvent(20, (int) credentialsType, providerId, issuedTokenId);
    }

    [Event(21, Keywords = (EventKeywords) 1, Level = EventLevel.Verbose, Task = (EventTask) 2, Message = "{0} issued token provider instance {1} validated token instance {2}")]
    private void IssuedTokenValidated(
      VssCredentialsType credentialsType,
      int providerId,
      int issuedTokenId)
    {
      if (!this.IsEnabled(EventLevel.Verbose, (EventKeywords) 1))
        return;
      this.WriteEvent(21, (int) credentialsType, providerId, issuedTokenId);
    }

    [Event(22, Keywords = (EventKeywords) 1, Level = EventLevel.Verbose, Task = (EventTask) 2, Message = "{0} issued token provider instance {1} retrieved token instance {2}")]
    private void IssuedTokenRetrievedFromCache(
      VssCredentialsType credentialsType,
      int providerId,
      int issuedTokenId)
    {
      if (!this.IsEnabled(EventLevel.Verbose, (EventKeywords) 1))
        return;
      this.WriteEvent(22, (int) credentialsType, providerId, issuedTokenId);
    }

    [Event(23, Keywords = (EventKeywords) 1, Level = EventLevel.Verbose, Task = (EventTask) 2, Opcode = EventOpcode.Stop, Message = "Finished authentication")]
    private void AuthenticationStop()
    {
      if (!this.IsEnabled(EventLevel.Verbose, (EventKeywords) 1))
        return;
      this.WriteEvent(23);
    }

    [Event(24, Level = EventLevel.Verbose, Task = (EventTask) 1, Opcode = EventOpcode.Stop, Message = "Finished {0} request to {1} with status code {2}")]
    private void HttpRequestStop(VssHttpMethod method, string url, int statusCode)
    {
      if (!this.IsEnabled())
        return;
      this.WriteEvent(24, (int) method, url, statusCode);
    }

    [Event(25, Keywords = (EventKeywords) 2, Level = EventLevel.Informational, Task = (EventTask) 3, Opcode = EventOpcode.Start, Message = "Starting operation {0}.{1}")]
    private void HttpOperationStart(string area, string operation)
    {
      if (!this.IsEnabled(EventLevel.Informational, (EventKeywords) 2))
        return;
      this.WriteEvent(25, area, operation);
    }

    [Event(26, Keywords = (EventKeywords) 2, Level = EventLevel.Informational, Task = (EventTask) 3, Opcode = EventOpcode.Stop, Message = "Finished operation {0}.{1}")]
    private void HttpOperationStop(string area, string operation)
    {
      if (!this.IsEnabled(EventLevel.Informational, (EventKeywords) 2))
        return;
      this.WriteEvent(26, area, operation);
    }

    [Event(27, Keywords = (EventKeywords) 1, Level = EventLevel.Error, Task = (EventTask) 2, Opcode = EventOpcode.Info, Message = "{0} issued token provider instance {1} failed to retrieve a token.%nReason: {2}")]
    private void AuthenticationError(
      VssCredentialsType credentialsType,
      int providerId,
      string message)
    {
      if (!this.IsEnabled(EventLevel.Error, (EventKeywords) 1))
        return;
      this.WriteEvent(27, (int) credentialsType, providerId, message);
    }

    [Event(28, Keywords = (EventKeywords) 1, Level = EventLevel.Warning, Task = (EventTask) 1, Opcode = EventOpcode.Info, Message = "No issued token provider found which can handle the authentication challenge")]
    private void IssuedTokenProviderNotFound()
    {
      if (!this.IsEnabled(EventLevel.Warning, (EventKeywords) 1))
        return;
      this.WriteEvent(28);
    }

    [Event(29, Keywords = (EventKeywords) 1, Level = EventLevel.Warning, Task = (EventTask) 1, Opcode = EventOpcode.Info, Message = "{0} issued token provider instance {1} requires an interactive prompt which is not allowed by the current settings")]
    private void IssuedTokenProviderPromptRequired(
      VssCredentialsType credentialsType,
      int providerId)
    {
      if (!this.IsEnabled(EventLevel.Warning, (EventKeywords) 1))
        return;
      this.WriteEvent(29, (int) credentialsType, providerId);
    }

    [Event(30, Keywords = (EventKeywords) 2, Level = EventLevel.Critical, Task = (EventTask) 3, Opcode = EventOpcode.Info, Message = "A task completion source was not properly completed during authentication")]
    public void TokenSourceNotCompleted()
    {
      if (!this.IsEnabled(EventLevel.Critical, (EventKeywords) 2))
        return;
      this.WriteEvent(30);
    }

    [Event(31, Keywords = (EventKeywords) 1, Level = EventLevel.Warning, Task = (EventTask) 2, Opcode = EventOpcode.Info, Message = "Retrieving an AAD auth token took a long time ({0} seconds)")]
    public void AuthorizationDelayed(string timespan)
    {
      if (!this.IsEnabled(EventLevel.Warning, (EventKeywords) 1))
        return;
      this.WriteEvent(31, timespan);
    }

    [Event(32, Keywords = (EventKeywords) 1, Level = EventLevel.Informational, Task = (EventTask) 2, Opcode = EventOpcode.Info, Message = "AAD Correlation ID for this token request: {0}")]
    public void AADCorrelationID(string aadCorrelationId)
    {
      if (!this.IsEnabled(EventLevel.Informational, (EventKeywords) 1))
        return;
      this.WriteEvent(32, aadCorrelationId);
    }

    [NonEvent]
    private void SetActivityId(VssTraceActivity activity)
    {
      if (activity == null)
        return;
      Guid id = activity.Id;
      System.Diagnostics.Eventing.EventProvider.SetActivityId(ref id);
    }

    [NonEvent]
    private static IList<string> SplitMessage(string message)
    {
      List<string> stringList = new List<string>();
      if (message.Length > 30000)
      {
        int startIndex = 0;
        do
        {
          int length = message.Length - startIndex > 30000 ? 30000 : message.Length - startIndex;
          stringList.Add(message.Substring(startIndex, length));
          startIndex += length;
        }
        while (message.Length > startIndex);
      }
      else
        stringList.Add(message);
      return (IList<string>) stringList;
    }

    [NonEvent]
    private void WriteMessageEvent(int param0, string message, Action<int, string> writeEvent)
    {
      writeEvent(param0, message);
      if (System.Diagnostics.Eventing.EventProvider.GetLastWriteEventError() != System.Diagnostics.Eventing.EventProvider.WriteEventErrorCode.EventTooBig)
        return;
      foreach (string str in (IEnumerable<string>) VssHttpEventSource.SplitMessage(message))
        writeEvent(param0, str);
    }

    [NonEvent]
    private void WriteMessageEvent(
      VssCredentialsType param0,
      int param1,
      string message,
      Action<VssCredentialsType, int, string> writeEvent)
    {
      writeEvent(param0, param1, message);
      if (System.Diagnostics.Eventing.EventProvider.GetLastWriteEventError() != System.Diagnostics.Eventing.EventProvider.WriteEventErrorCode.EventTooBig)
        return;
      foreach (string str in (IEnumerable<string>) VssHttpEventSource.SplitMessage(message))
        writeEvent(param0, param1, str);
    }

    [NonEvent]
    private void WriteMessageEvent(
      VssHttpMethod param0,
      string param1,
      string message,
      Action<VssHttpMethod, string, string> writeEvent)
    {
      writeEvent(param0, param1, message);
      if (System.Diagnostics.Eventing.EventProvider.GetLastWriteEventError() != System.Diagnostics.Eventing.EventProvider.WriteEventErrorCode.EventTooBig)
        return;
      foreach (string str in (IEnumerable<string>) VssHttpEventSource.SplitMessage(message))
        writeEvent(param0, param1, str);
    }

    [NonEvent]
    private new unsafe void WriteEvent(int eventId, int param0, string param1)
    {
      param1 = param1 ?? string.Empty;
      int eventDataCount = 2;
      // ISSUE: untyped stack allocation
      EventSource.EventData* data = (EventSource.EventData*) __untypedstackalloc((IntPtr) (uint) (sizeof (EventSource.EventData) * eventDataCount));
      data->Size = 4;
      data[1].Size = (param1.Length + 1) * 2;
      fixed (char* chPtr = param1)
      {
        data->DataPointer = (IntPtr) (void*) &param0;
        data[1].DataPointer = (IntPtr) (void*) chPtr;
        this.WriteEventCore(eventId, eventDataCount, data);
      }
    }

    [NonEvent]
    private unsafe void WriteEvent(int eventId, int param0, string param1, string param2)
    {
      param1 = param1 ?? string.Empty;
      param2 = param2 ?? string.Empty;
      int eventDataCount = 3;
      // ISSUE: untyped stack allocation
      EventSource.EventData* data = (EventSource.EventData*) __untypedstackalloc((IntPtr) (uint) (sizeof (EventSource.EventData) * eventDataCount));
      data->Size = 4;
      data[1].Size = (param1.Length + 1) * 2;
      data[2].Size = (param2.Length + 1) * 2;
      fixed (char* chPtr1 = param1)
        fixed (char* chPtr2 = param2)
        {
          data->DataPointer = (IntPtr) (void*) &param0;
          data[1].DataPointer = (IntPtr) (void*) chPtr1;
          data[2].DataPointer = (IntPtr) (void*) chPtr2;
          this.WriteEventCore(eventId, eventDataCount, data);
        }
    }

    [NonEvent]
    private unsafe void WriteEvent(int eventId, int param0, int param1, Guid param2)
    {
      int eventDataCount = 3;
      // ISSUE: untyped stack allocation
      EventSource.EventData* data = (EventSource.EventData*) __untypedstackalloc((IntPtr) (uint) (sizeof (EventSource.EventData) * eventDataCount));
      data->Size = 4;
      data[1].Size = 4;
      data[2].Size = sizeof (Guid);
      data->DataPointer = (IntPtr) (void*) &param0;
      data[1].DataPointer = (IntPtr) (void*) &param1;
      data[2].DataPointer = (IntPtr) (void*) &param2;
      this.WriteEventCore(eventId, eventDataCount, data);
    }

    [NonEvent]
    private unsafe void WriteEvent(int eventId, int param0, int param1, string param2)
    {
      param2 = param2 ?? string.Empty;
      int eventDataCount = 3;
      // ISSUE: untyped stack allocation
      EventSource.EventData* data = (EventSource.EventData*) __untypedstackalloc((IntPtr) (uint) (sizeof (EventSource.EventData) * eventDataCount));
      data->Size = 4;
      data[1].Size = 4;
      data[2].Size = (param2.Length + 1) * 2;
      fixed (char* chPtr = param2)
      {
        data->DataPointer = (IntPtr) (void*) &param0;
        data[1].DataPointer = (IntPtr) (void*) &param1;
        data[2].DataPointer = (IntPtr) (void*) chPtr;
        this.WriteEventCore(eventId, eventDataCount, data);
      }
    }

    [NonEvent]
    private unsafe void WriteEvent(int eventId, int param0, string param1, int param2)
    {
      param1 = param1 ?? string.Empty;
      int eventDataCount = 3;
      // ISSUE: untyped stack allocation
      EventSource.EventData* data = (EventSource.EventData*) __untypedstackalloc((IntPtr) (uint) (sizeof (EventSource.EventData) * eventDataCount));
      data->Size = 4;
      data[1].Size = (param1.Length + 1) * 2;
      data[2].Size = 4;
      fixed (char* chPtr = param1)
      {
        data->DataPointer = (IntPtr) (void*) &param0;
        data[1].DataPointer = (IntPtr) (void*) chPtr;
        data[2].DataPointer = (IntPtr) (void*) &param2;
        this.WriteEventCore(eventId, eventDataCount, data);
      }
    }

    [NonEvent]
    private unsafe void WriteEvent(int eventId, int param0, int param1, int param2, Guid param3)
    {
      int eventDataCount = 4;
      // ISSUE: untyped stack allocation
      EventSource.EventData* data = (EventSource.EventData*) __untypedstackalloc((IntPtr) (uint) (sizeof (EventSource.EventData) * eventDataCount));
      data->Size = 4;
      data[1].Size = 4;
      data[2].Size = 4;
      data[3].Size = sizeof (Guid);
      data->DataPointer = (IntPtr) (void*) &param0;
      data[1].DataPointer = (IntPtr) (void*) &param1;
      data[2].DataPointer = (IntPtr) (void*) &param2;
      data[3].DataPointer = (IntPtr) (void*) &param3;
      this.WriteEventCore(eventId, eventDataCount, data);
    }

    [NonEvent]
    private unsafe void WriteEvent(int eventId, int param0, int param1, Guid param2, Guid param3)
    {
      int eventDataCount = 4;
      // ISSUE: untyped stack allocation
      EventSource.EventData* data = (EventSource.EventData*) __untypedstackalloc((IntPtr) (uint) (sizeof (EventSource.EventData) * eventDataCount));
      data->Size = 4;
      data[1].Size = 4;
      data[2].Size = sizeof (Guid);
      data[3].Size = sizeof (Guid);
      data->DataPointer = (IntPtr) (void*) &param0;
      data[1].DataPointer = (IntPtr) (void*) &param1;
      data[2].DataPointer = (IntPtr) (void*) &param2;
      data[3].DataPointer = (IntPtr) (void*) &param3;
      this.WriteEventCore(eventId, eventDataCount, data);
    }

    [NonEvent]
    private unsafe void WriteEvent(
      int eventId,
      int param0,
      string param1,
      int param2,
      string param3)
    {
      param1 = param1 ?? string.Empty;
      param3 = param3 ?? string.Empty;
      int eventDataCount = 4;
      // ISSUE: untyped stack allocation
      EventSource.EventData* data = (EventSource.EventData*) __untypedstackalloc((IntPtr) (uint) (sizeof (EventSource.EventData) * eventDataCount));
      data->Size = 4;
      data[1].Size = (param1.Length + 1) * 2;
      data[2].Size = 4;
      data[3].Size = (param3.Length + 1) * 2;
      fixed (char* chPtr1 = param1)
        fixed (char* chPtr2 = param3)
        {
          data->DataPointer = (IntPtr) (void*) &param0;
          data[1].DataPointer = (IntPtr) (void*) chPtr1;
          data[2].DataPointer = (IntPtr) (void*) &param2;
          data[3].DataPointer = (IntPtr) (void*) chPtr2;
          this.WriteEventCore(eventId, eventDataCount, data);
        }
    }

    [NonEvent]
    private unsafe void WriteEvent(
      int eventId,
      int param0,
      string param1,
      int param2,
      string param3,
      double param4)
    {
      param1 = param1 ?? string.Empty;
      param3 = param3 ?? string.Empty;
      int eventDataCount = 5;
      // ISSUE: untyped stack allocation
      EventSource.EventData* data = (EventSource.EventData*) __untypedstackalloc((IntPtr) (uint) (sizeof (EventSource.EventData) * eventDataCount));
      data->Size = 4;
      data[1].Size = (param1.Length + 1) * 2;
      data[2].Size = 4;
      data[3].Size = (param3.Length + 1) * 2;
      data[4].Size = 8;
      fixed (char* chPtr1 = param1)
        fixed (char* chPtr2 = param3)
        {
          data->DataPointer = (IntPtr) (void*) &param0;
          data[1].DataPointer = (IntPtr) (void*) chPtr1;
          data[2].DataPointer = (IntPtr) (void*) &param2;
          data[3].DataPointer = (IntPtr) (void*) chPtr2;
          data[4].DataPointer = (IntPtr) (void*) &param4;
          this.WriteEventCore(eventId, eventDataCount, data);
        }
    }

    public static class Tasks
    {
      public const EventTask HttpRequest = (EventTask) 1;
      public const EventTask Authentication = (EventTask) 2;
      public const EventTask HttpOperation = (EventTask) 3;
    }

    public static class Keywords
    {
      public const EventKeywords Authentication = (EventKeywords) 1;
      public const EventKeywords HttpOperation = (EventKeywords) 2;
    }
  }
}
