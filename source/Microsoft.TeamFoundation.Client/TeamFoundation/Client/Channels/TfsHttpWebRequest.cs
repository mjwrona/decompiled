// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.Channels.TfsHttpWebRequest
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Client.Internal;
using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.TeamFoundation.Framework.Client;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Services.Protocols;
using System.Xml;

namespace Microsoft.TeamFoundation.Client.Channels
{
  internal class TfsHttpWebRequest : AsyncOperation
  {
    private int m_retries;
    private bool m_aborted;
    private bool m_expired;
    private long m_requestId;
    private string m_methodName;
    private string m_methodNameEx;
    private TfsMessage m_request;
    private TfsMessage m_response;
    private System.Threading.Timer m_responseTimer;
    private Stream m_requestStream;
    private Microsoft.TeamFoundation.Framework.Common.TimeoutHelper m_timeout;
    private long m_beginWorkingSet;
    private Stopwatch m_requestTimer;
    private WebException m_webException;
    private HttpWebRequest m_webRequest;
    private HttpWebResponse m_webResponse;
    private ITfsRequestListener m_listener;
    private TfsHttpRequestChannel m_channel;
    private bool m_lastResponseDemandedProxyAuth;
    private Microsoft.VisualStudio.Services.Common.IssuedToken m_currentToken;
    private Microsoft.VisualStudio.Services.Common.IssuedTokenProvider m_tokenProvider;
    private CancellationTokenSource m_cancellationTokenSource;
    private const string c_offlineEnvironmentVariable = "DDSUITES_SERVER_OFFLINE";
    private const int c_authRetriesTotal = 3;

    internal TfsHttpWebRequest(
      TfsHttpRequestChannel channel,
      TfsMessage message,
      TimeSpan timeout,
      ITfsRequestListener listener,
      AsyncCallback callback,
      object state)
      : base(callback, state)
    {
      this.m_retries = 3;
      this.m_channel = channel;
      this.m_request = message;
      this.m_listener = listener;
      this.m_timeout = new Microsoft.TeamFoundation.Framework.Common.TimeoutHelper(timeout);
      Uri result;
      if (string.IsNullOrEmpty(message.Action) || !Uri.TryCreate(message.Action, UriKind.Absolute, out result) || result.Segments.Length == 0)
        return;
      this.m_methodName = result.Segments[result.Segments.Length - 1];
    }

    public TfsMessage Request => this.m_request;

    public TfsMessage Response => this.m_response;

    public Uri RequestUri => this.m_channel.Uri;

    protected VssCredentials Credentials => this.m_channel.Credentials;

    protected TfsRequestSettings Settings => this.m_channel.Settings;

    protected Microsoft.TeamFoundation.Framework.Common.TimeoutHelper Timeout => this.m_timeout;

    internal void Abort() => this.Abort(false);

    private void Abort(bool expired)
    {
      this.m_aborted = true;
      this.m_expired = expired;
      if (this.m_webRequest != null)
        this.m_webRequest.Abort();
      if (this.m_cancellationTokenSource == null)
        return;
      this.m_cancellationTokenSource.Cancel();
    }

    public TfsMessage SendRequest()
    {
      Exception exception1 = (Exception) null;
      try
      {
        this.OnBeginRequest();
        this.m_webRequest = this.CreateWebRequest();
        WebException webException;
        TfsMessage responseMessage;
        for (HttpWebResponse response = this.SendRequestAndGetResponse(this.m_webRequest, out webException); this.IsAuthenticationChallenge(this.m_request, response, webException, out responseMessage); response = this.SendRequestAndGetResponse(this.m_webRequest, out webException))
        {
          this.m_lastResponseDemandedProxyAuth = response != null && response.StatusCode == HttpStatusCode.ProxyAuthenticationRequired;
          Microsoft.VisualStudio.Services.Common.IssuedToken failedToken = this.EnsureTokenProvider(response);
          if (this.m_aborted || this.m_retries <= 0 || this.m_retries < 3 && !string.IsNullOrEmpty(response.Headers["X-VSS-AuthenticateError"]))
          {
            if (responseMessage == null)
            {
              responseMessage = this.ReadResponse(response, webException);
              break;
            }
            break;
          }
          --this.m_retries;
          response.Close();
          if (responseMessage != null)
          {
            responseMessage.Close();
            responseMessage = (TfsMessage) null;
          }
          Exception exception2 = (Exception) null;
          Microsoft.VisualStudio.Services.Common.IssuedToken issuedToken = (Microsoft.VisualStudio.Services.Common.IssuedToken) null;
          try
          {
            if (this.m_cancellationTokenSource == null)
              this.m_cancellationTokenSource = new CancellationTokenSource(this.m_timeout.RemainingTime());
            issuedToken = this.m_tokenProvider.GetTokenAsync(failedToken, this.m_cancellationTokenSource.Token).SyncResult<Microsoft.VisualStudio.Services.Common.IssuedToken>();
          }
          catch (Exception ex)
          {
            exception2 = ex;
          }
          finally
          {
            if (this.m_cancellationTokenSource != null)
            {
              this.m_cancellationTokenSource.Dispose();
              this.m_cancellationTokenSource = (CancellationTokenSource) null;
            }
          }
          if (issuedToken == null)
          {
            TeamFoundationAuthenticationError error = TeamFoundationAuthenticationError.Cancelled;
            if (exception2 == null)
              this.Credentials.PromptType = CredentialPromptType.DoNotPrompt;
            else
              error = TeamFoundationAuthenticationError.GetTokenFailed;
            throw new TeamFoundationServerUnauthorizedException(this.FormatServiceUnauthorizedMessage(), exception2 ?? (Exception) webException, response, error);
          }
          this.m_currentToken = issuedToken;
          this.m_webRequest = this.CreateWebRequest();
        }
        return responseMessage;
      }
      catch (Exception ex)
      {
        exception1 = ex;
        throw;
      }
      finally
      {
        this.OnEndRequest(exception1);
      }
    }

    private bool IsAuthenticationChallenge(
      TfsMessage requestMessage,
      HttpWebResponse webResponse,
      WebException webException,
      out TfsMessage responseMessage)
    {
      responseMessage = (TfsMessage) null;
      if (this.Credentials.IsAuthenticationChallenge(TfsHttpWebRequest.Wrap(webResponse)))
        return true;
      responseMessage = this.ReadResponse(webResponse, webException);
      return this.IsLegacyAuthenticationChallenge(requestMessage.Action, responseMessage);
    }

    private bool IsLegacyAuthenticationChallenge(string action, TfsMessage message)
    {
      if (message == null || !message.IsFault || !(message.CreateException() is SoapException exception))
        return false;
      switch (action)
      {
        case "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Registration/03/GetRegistrationEntries":
        case "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/ServerStatus/03/CheckAuthentication":
          return exception.Message.StartsWith("TF50309", StringComparison.OrdinalIgnoreCase);
        case "http://microsoft.com/webservices/Connect":
          return exception.SubCode != null && exception.SubCode.Code != (XmlQualifiedName) null && exception.SubCode.Code.Name == typeof (AccessCheckException).Name;
        default:
          return false;
      }
    }

    private TfsMessage ReadResponse(HttpWebResponse webResponse, WebException webException = null)
    {
      bool closeResponse = true;
      Stream stream = (Stream) null;
      try
      {
        this.ThrowIfAborted();
        if (webResponse == null)
          throw new TeamFoundationServiceUnavailableException(this.FormatServiceUnavailableMessage(webException), (Exception) webException);
        if (webResponse.ResponseUri.Scheme == Uri.UriSchemeHttps && webResponse.StatusCode == HttpStatusCode.Forbidden)
          ClientCertificateManager.Instance.Invalidate();
        stream = webResponse.GetResponseStream();
        if (this.m_channel.Settings.SoapTraceEnabled)
          stream = (Stream) new TracingStream(stream, this.m_channel.Settings.Tracing);
        if (webResponse.StatusCode == HttpStatusCode.Accepted || webResponse.StatusCode == HttpStatusCode.OK || webResponse.StatusCode == HttpStatusCode.InternalServerError)
          return this.ReadMessage(webResponse, webException, stream, out closeResponse);
        if (webResponse.StatusCode == HttpStatusCode.Found || webResponse.StatusCode == HttpStatusCode.Found || webResponse.StatusCode == HttpStatusCode.Unauthorized)
          throw new TeamFoundationServerUnauthorizedException(this.FormatServiceUnauthorizedMessage(), (Exception) webException, webResponse, TeamFoundationAuthenticationError.TokenUnauthorized);
        if (webResponse.StatusCode == HttpStatusCode.BadRequest)
        {
          TfsMessage tfsMessage = this.ReadFaultFromHeader(webResponse);
          if (tfsMessage != null)
            return tfsMessage;
          string header = webResponse.Headers["X-TFS-ServiceError"];
          throw new TeamFoundationServerInvalidRequestException(TFCommonResources.InvalidServerRequest(string.IsNullOrEmpty(header) ? (object) TeamFoundationServerInvalidResponseException.FormatHttpStatus(webResponse) : (object) UriUtility.UrlDecode(header, TfsRequestSettings.RequestEncoding)), (Exception) webException);
        }
        string str = TeamFoundationServerInvalidResponseException.FormatHttpStatus(webResponse);
        if (!string.IsNullOrEmpty(webResponse.Headers["X-TFS-ServiceError"]))
        {
          str = UriUtility.UrlDecode(webResponse.Headers["X-TFS-ServiceError"], TfsRequestSettings.RequestEncoding);
          if (webResponse.StatusCode != HttpStatusCode.ServiceUnavailable)
            str = this.FormatServiceUnavailableMessage(str);
        }
        throw new TeamFoundationServiceUnavailableException(str, (Exception) webException);
      }
      finally
      {
        if (closeResponse)
        {
          stream?.Close();
          webResponse?.Close();
        }
      }
    }

    private void ThrowIfAborted()
    {
      if (this.m_aborted)
        throw new System.OperationCanceledException(ClientResources.CommandCanceled());
    }

    protected virtual HttpWebRequest CreateWebRequest()
    {
      this.ThrowIfAborted();
      if (Environment.GetEnvironmentVariable("DDSUITES_SERVER_OFFLINE") != null)
        throw new TeamFoundationServiceUnavailableException("DDSUITES_SERVER_OFFLINE");
      return TfsHttpRequestHelpers.CreateSoapRequest(this.m_channel.Uri, this.m_channel.SessionId, this.m_request.Action, this.m_methodName, this.m_channel.Culture, this.m_channel.Settings, this.m_channel.Credentials, this.m_channel.Impersonate, this.m_lastResponseDemandedProxyAuth, ref this.m_currentToken, ref this.m_tokenProvider);
    }

    public string FormatServiceUnavailableMessage(string reason) => string.IsNullOrEmpty(this.m_channel.ServerName) ? TFCommonResources.ServicesUnavailableNoServer((object) reason) : TFCommonResources.ServicesUnavailable((object) this.m_channel.ServerName, (object) reason);

    public string FormatServiceUnavailableMessage(WebException webException) => this.FormatServiceUnavailableMessage(!(webException.InnerException is SocketException innerException) ? webException.Message : string.Format("{0}{1}  {2}", (object) webException.Message, (object) Environment.NewLine, (object) innerException.Message));

    public string FormatServiceUnauthorizedMessage() => string.IsNullOrEmpty(this.m_channel.ServerName) ? TFCommonResources.Unauthorized((object) this.m_channel.Uri) : TFCommonResources.Unauthorized((object) this.m_channel.ServerName);

    protected TfsMessage ReadMessage(
      HttpWebResponse webResponse,
      WebException webException,
      Stream responseStream,
      out bool closeResponse)
    {
      closeResponse = true;
      this.ThrowIfAborted();
      if (webResponse.StatusCode != HttpStatusCode.InternalServerError && webResponse.ContentLength == 0L)
      {
        this.ValidateToken(webResponse);
        return TfsMessage.EmptyMessage;
      }
      TfsMessage tfsMessage = (TfsMessage) null;
      string header = webResponse.Headers[HttpResponseHeader.ContentType];
      if (string.IsNullOrEmpty(header) || !this.m_channel.Encoder.IsContentTypeSupported(header))
      {
        if (webResponse.StatusCode == HttpStatusCode.InternalServerError)
          tfsMessage = this.ReadFaultFromHeader(webResponse);
        if (tfsMessage == null)
        {
          string responseString = TeamFoundationServerInvalidResponseException.GetResponseString(responseStream);
          throw new TeamFoundationServerInvalidResponseException(TeamFoundationServerInvalidResponseException.FormatInvalidServerResponseMessage(webResponse), (Exception) webException, webResponse.StatusCode, responseString);
        }
      }
      else
      {
        this.ValidateToken(webResponse);
        closeResponse = false;
        tfsMessage = this.m_channel.Encoder.ReadMessage((Stream) new TfsHttpWebRequest.HttpWebResponseStream(this.m_channel, webResponse, responseStream));
      }
      return tfsMessage;
    }

    private void ValidateToken(HttpWebResponse webResponse)
    {
      if (this.m_tokenProvider == null || this.m_currentToken == null)
        return;
      this.m_tokenProvider.ValidateToken(this.m_currentToken, TfsHttpWebRequest.Wrap(webResponse));
    }

    private TfsMessage ReadFaultFromHeader(HttpWebResponse webResponse)
    {
      TfsMessage tfsMessage = (TfsMessage) null;
      string messageAsString = UriUtility.UrlDecode(webResponse.Headers["X-TFS-SoapException"], TfsRequestSettings.RequestEncoding);
      if (!string.IsNullOrEmpty(messageAsString))
        tfsMessage = this.m_channel.Encoder.ReadMessage(messageAsString);
      return tfsMessage;
    }

    private Microsoft.VisualStudio.Services.Common.IssuedToken EnsureTokenProvider(
      HttpWebResponse webResponse)
    {
      try
      {
        TeamFoundationTrace.Enter(TraceKeywordSets.Authentication, TraceLevel.Verbose, "TfsHttpWebRequest.EnsureTokenProvider");
        Microsoft.VisualStudio.Services.Common.IssuedToken failedToken = (Microsoft.VisualStudio.Services.Common.IssuedToken) null;
        if (this.m_currentToken != null)
        {
          if (this.m_tokenProvider != null)
            this.m_tokenProvider.InvalidateToken(this.m_currentToken);
          failedToken = this.m_currentToken;
          this.m_currentToken = (Microsoft.VisualStudio.Services.Common.IssuedToken) null;
        }
        this.m_tokenProvider = this.Credentials.CreateTokenProvider(this.m_channel.Uri, TfsHttpWebRequest.Wrap(webResponse), failedToken);
        TeamFoundationTrace.Verbose(TraceKeywordSets.Authentication, "Created token provider {0}", this.m_tokenProvider == null ? (object) "null" : (object) this.m_tokenProvider.GetType().FullName);
        if (this.m_tokenProvider == null || this.Credentials.PromptType != CredentialPromptType.PromptIfNeeded && this.m_tokenProvider.GetTokenIsInteractive)
        {
          if (this.m_tokenProvider == null)
            TeamFoundationTrace.Verbose(TraceKeywordSets.Authentication, "No token provider exists");
          else
            TeamFoundationTrace.Verbose(TraceKeywordSets.Authentication, "The token provider {0} requires interactivity but it is not allowed", (object) this.m_tokenProvider.GetType().FullName);
          webResponse.Close();
          throw new TeamFoundationServerUnauthorizedException(this.FormatServiceUnauthorizedMessage(), (Exception) this.m_webException, webResponse, TeamFoundationAuthenticationError.InteractiveRequired);
        }
        return failedToken;
      }
      finally
      {
        TeamFoundationTrace.Exit(TraceKeywordSets.Authentication, TraceLevel.Verbose, "TfsHttpWebRequest.EnsureTokenProvider");
      }
    }

    private void TraceHeaders(WebHeaderCollection headers)
    {
      if (!this.m_channel.Settings.Tracing.TraceVerbose)
        return;
      TfsHttpRequestHelpers.TraceHeaders(headers);
    }

    private HttpWebResponse SendRequestAndGetResponse(
      HttpWebRequest webRequest,
      out WebException webException)
    {
      webException = (WebException) null;
      bool flag = false;
      HttpWebResponse response = (HttpWebResponse) null;
      try
      {
        if (this.m_channel.Settings.CompressRequestBody)
          webRequest.Headers["Content-Encoding"] = "gzip";
        this.TraceHeaders(webRequest.Headers);
        this.m_requestStream = webRequest.GetRequestStream();
        if (this.m_channel.Settings.CompressRequestBody)
          this.m_requestStream = (Stream) new GZipStream(this.m_requestStream, CompressionMode.Compress);
        if (this.m_channel.Settings.SoapTraceEnabled)
          this.m_requestStream = (Stream) new TracingStream(this.m_requestStream, this.m_channel.Settings.Tracing);
        this.m_channel.Encoder.WriteMessage(this.m_request, this.m_requestStream);
        this.OnSendRequest(webRequest);
        flag = true;
        this.m_requestStream.Close();
        this.ProcessDelay();
        response = (HttpWebResponse) webRequest.GetResponse();
        return response;
      }
      catch (WebException ex)
      {
        webException = ex;
        response = (HttpWebResponse) webException.Response;
        return response;
      }
      finally
      {
        if (flag)
          this.OnReceiveReply(response);
      }
    }

    public void Begin()
    {
      bool flag = true;
      Exception exception = (Exception) null;
      try
      {
        this.OnBeginRequest();
        flag = this.BeginSendRequest();
      }
      catch (WebException ex)
      {
        exception = (Exception) new TeamFoundationServiceUnavailableException(this.FormatServiceUnavailableMessage(ex.Message), (Exception) ex);
      }
      catch (Exception ex)
      {
        exception = ex;
      }
      if (!flag)
        return;
      this.Complete(true, exception);
    }

    private bool BeginSendRequest()
    {
      this.m_webRequest = this.CreateWebRequest();
      if (this.m_channel.Settings.CompressRequestBody)
        this.m_webRequest.Headers["Content-Encoding"] = "gzip";
      this.TraceHeaders(this.m_webRequest.Headers);
      IAsyncResult requestStream = this.m_webRequest.BeginGetRequestStream(new AsyncCallback(TfsHttpWebRequest.EndGetRequestStream), (object) this);
      return requestStream.CompletedSynchronously && this.CompleteGetRequestStream(requestStream);
    }

    private static void EndGetRequestStream(IAsyncResult result)
    {
      if (result.CompletedSynchronously)
        return;
      TfsHttpWebRequest asyncState = (TfsHttpWebRequest) result.AsyncState;
      bool flag = true;
      Exception exception = (Exception) null;
      try
      {
        flag = asyncState.CompleteGetRequestStream(result);
      }
      catch (WebException ex)
      {
        exception = (Exception) new TeamFoundationServiceUnavailableException(asyncState.FormatServiceUnavailableMessage(ex), (Exception) ex);
      }
      catch (Exception ex)
      {
        exception = ex;
      }
      if (!flag)
        return;
      asyncState.Complete(false, exception);
    }

    private bool CompleteGetRequestStream(IAsyncResult result)
    {
      this.m_requestStream = this.m_webRequest.EndGetRequestStream(result);
      if (this.m_channel.Settings.CompressRequestBody)
        this.m_requestStream = (Stream) new GZipStream(this.m_requestStream, CompressionMode.Compress);
      if (this.m_channel.Settings.SoapTraceEnabled)
        this.m_requestStream = (Stream) new TracingStream(this.m_requestStream, this.m_channel.Settings.Tracing);
      this.m_channel.Encoder.WriteMessage(this.m_request, this.m_requestStream);
      this.OnSendRequest(this.m_webRequest);
      this.m_requestStream.Close();
      this.ProcessDelay();
      return this.BeginGetResponse();
    }

    private bool BeginGetResponse()
    {
      this.m_responseTimer = new System.Threading.Timer(new TimerCallback(TfsHttpWebRequest.ResponseTimerCallback), (object) this, this.m_timeout.RemainingTime(), TimeSpan.FromMilliseconds(-1.0));
      IAsyncResult response = this.m_webRequest.BeginGetResponse(new AsyncCallback(this.EndGetResponse), (object) this);
      return response.CompletedSynchronously && this.CompleteGetResponse(response);
    }

    private void EndGetResponse(IAsyncResult result)
    {
      if (result.CompletedSynchronously)
        return;
      TfsHttpWebRequest asyncState = (TfsHttpWebRequest) result.AsyncState;
      bool flag = true;
      Exception exception = (Exception) null;
      try
      {
        flag = asyncState.CompleteGetResponse(result);
      }
      catch (Exception ex)
      {
        exception = ex;
      }
      if (!flag)
        return;
      asyncState.Complete(false, exception);
    }

    private bool CompleteGetResponse(IAsyncResult result)
    {
      WebException webException = (WebException) null;
      HttpWebResponse httpWebResponse = (HttpWebResponse) null;
      try
      {
        this.CancelResponseTimer();
        httpWebResponse = (HttpWebResponse) this.m_webRequest.EndGetResponse(result);
      }
      catch (WebException ex)
      {
        webException = ex;
        httpWebResponse = (HttpWebResponse) webException.Response;
      }
      finally
      {
        this.OnReceiveReply(httpWebResponse);
      }
      TfsMessage responseMessage;
      if (this.IsAuthenticationChallenge(this.m_request, httpWebResponse, webException, out responseMessage))
      {
        this.m_lastResponseDemandedProxyAuth = httpWebResponse != null && httpWebResponse.StatusCode == HttpStatusCode.ProxyAuthenticationRequired;
        Microsoft.VisualStudio.Services.Common.IssuedToken failedToken = this.EnsureTokenProvider(httpWebResponse);
        if (this.m_aborted || this.m_retries <= 0 || this.m_retries < 3 && !string.IsNullOrEmpty(httpWebResponse.Headers["X-VSS-AuthenticateError"]))
        {
          this.m_response = responseMessage ?? this.ReadResponse(httpWebResponse, webException);
          return true;
        }
        --this.m_retries;
        httpWebResponse.Close();
        responseMessage?.Close();
        this.m_webResponse = httpWebResponse;
        this.m_webException = webException;
        try
        {
          if (this.m_cancellationTokenSource == null)
          {
            this.m_cancellationTokenSource = new CancellationTokenSource();
            this.m_cancellationTokenSource.CancelAfter(this.m_timeout.RemainingTime());
          }
          IAsyncResult result1 = TaskAsyncHelper.BeginTask<Microsoft.VisualStudio.Services.Common.IssuedToken>((Func<Task<Microsoft.VisualStudio.Services.Common.IssuedToken>>) (() => this.m_tokenProvider.GetTokenAsync(failedToken, this.m_cancellationTokenSource.Token)), new AsyncCallback(TfsHttpWebRequest.EndGetToken), (object) this);
          if (result1.CompletedSynchronously)
            return this.CompleteGetToken(result1);
        }
        catch (Exception ex)
        {
          throw new TeamFoundationServerUnauthorizedException(this.FormatServiceUnauthorizedMessage(), ex, httpWebResponse, TeamFoundationAuthenticationError.GetTokenFailed);
        }
        return false;
      }
      this.m_response = responseMessage;
      return true;
    }

    private static void EndGetToken(IAsyncResult result)
    {
      if (result.CompletedSynchronously)
        return;
      TfsHttpWebRequest asyncState = (TfsHttpWebRequest) result.AsyncState;
      bool flag = true;
      Exception exception = (Exception) null;
      try
      {
        flag = asyncState.CompleteGetToken(result);
      }
      catch (Exception ex)
      {
        exception = ex;
      }
      if (!flag)
        return;
      asyncState.Complete(false, exception);
    }

    private bool CompleteGetToken(IAsyncResult result)
    {
      Exception innerException = (Exception) null;
      Microsoft.VisualStudio.Services.Common.IssuedToken issuedToken = (Microsoft.VisualStudio.Services.Common.IssuedToken) null;
      try
      {
        issuedToken = TaskAsyncHelper.EndTask<Microsoft.VisualStudio.Services.Common.IssuedToken>(result);
      }
      catch (Exception ex)
      {
        innerException = ex;
      }
      if (this.m_cancellationTokenSource != null)
      {
        this.m_cancellationTokenSource.Dispose();
        this.m_cancellationTokenSource = (CancellationTokenSource) null;
      }
      if (issuedToken == null)
      {
        TeamFoundationAuthenticationError error = TeamFoundationAuthenticationError.Cancelled;
        if (innerException == null)
        {
          TeamFoundationTrace.Verbose(TraceKeywordSets.Authentication, "Received no token and no exception. Assuming operation was cancelled.");
          innerException = (Exception) this.m_webException;
          this.Credentials.PromptType = CredentialPromptType.DoNotPrompt;
        }
        else
        {
          TeamFoundationTrace.Verbose(TraceKeywordSets.Authentication, "Caught exception while obtaining a new token: {0}", (object) innerException.ToString());
          error = TeamFoundationAuthenticationError.GetTokenFailed;
        }
        throw new TeamFoundationServerUnauthorizedException(this.FormatServiceUnauthorizedMessage(), innerException, this.m_webResponse, error);
      }
      this.m_webResponse = (HttpWebResponse) null;
      this.m_webException = (WebException) null;
      this.m_currentToken = issuedToken;
      return this.BeginSendRequest();
    }

    private void OnSendRequest(HttpWebRequest request)
    {
      if (this.m_listener != null)
        this.m_listener.BeforeSendRequest(this.m_requestId, this.m_methodName, request);
      if (!this.Settings.Tracing.TraceInfo)
        return;
      if (this.m_channel.Settings.Tracing.TraceInfo && string.IsNullOrEmpty(this.m_methodNameEx))
      {
        string str = "<unknown>";
        if (this.m_channel.Uri.AbsoluteUri.IndexOf("/_vt", StringComparison.OrdinalIgnoreCase) >= 0)
          str = "SharePoint";
        else if (this.m_channel.Uri.Segments.Length > 3)
        {
          string segment = this.m_channel.Uri.Segments[this.m_channel.Uri.Segments.Length - 3];
          str = segment.Substring(0, segment.Length - 1);
        }
        this.m_methodNameEx = this.m_methodName + "[" + str + "]";
      }
      this.m_requestTimer = new Stopwatch();
      this.m_requestTimer.Start();
      if (!this.m_channel.Settings.Tracing.TraceVerbose)
        return;
      this.m_beginWorkingSet = Environment.WorkingSet;
    }

    private void OnReceiveReply(HttpWebResponse response)
    {
      if (response != null)
        this.TraceHeaders(response.Headers);
      if (this.Settings.Tracing.TraceInfo)
        this.m_requestTimer.Stop();
      if (this.m_listener == null)
        return;
      this.m_listener.AfterReceiveReply(this.m_requestId, this.m_methodName, response);
    }

    private void OnBeginRequest()
    {
      if (this.m_listener == null)
        return;
      this.m_requestId = this.m_listener.BeginRequest();
    }

    private void OnEndRequest(Exception exception)
    {
      if (this.m_listener == null)
        return;
      this.m_listener.EndRequest(this.m_requestId, exception);
    }

    private void CancelResponseTimer()
    {
      if (this.m_responseTimer == null)
        return;
      this.m_responseTimer.Change(-1, -1);
      this.m_responseTimer.Dispose();
      this.m_responseTimer = (System.Threading.Timer) null;
    }

    private static IHttpResponse Wrap(HttpWebResponse response) => response == null ? (IHttpResponse) null : (IHttpResponse) new HttpWebResponseWrapper(response);

    private static void ResponseTimerCallback(object state) => ((TfsHttpWebRequest) state).Abort(true);

    private void ProcessDelay()
    {
      if (!(TfsRequestSettings.TestDelay != TimeSpan.Zero))
        return;
      Thread.Sleep(Math.Abs((int) TfsRequestSettings.TestDelay.TotalMilliseconds));
      if (TfsRequestSettings.TestDelay < TimeSpan.Zero)
        throw new Exception("User injected failure.");
    }

    public static bool TryEnd(
      IAsyncResult result,
      out TfsHttpWebRequest webRequest,
      out Exception exception)
    {
      if (AsyncOperation.TryEnd<TfsHttpWebRequest>(result, out webRequest, out exception))
        return true;
      if (webRequest.m_expired)
        exception = (Exception) new TimeoutException(ClientResources.HttpRequestTimeout((object) webRequest.m_timeout.Value));
      return false;
    }

    protected sealed class HttpWebResponseStream : Stream
    {
      private Stream m_innerStream;
      private HttpWebResponse m_webResponse;
      private TfsHttpRequestChannel m_channel;

      public HttpWebResponseStream(
        TfsHttpRequestChannel channel,
        HttpWebResponse response,
        Stream responseStream)
      {
        this.m_channel = channel;
        this.m_webResponse = response;
        this.m_innerStream = responseStream;
      }

      public override bool CanRead => this.m_innerStream.CanRead;

      public override bool CanSeek => this.m_innerStream.CanSeek;

      public override bool CanWrite => this.m_innerStream.CanWrite;

      public override long Length => this.m_innerStream.Length;

      public override long Position
      {
        get => this.m_innerStream.Position;
        set => this.m_innerStream.Position = value;
      }

      public override void Close()
      {
        this.m_innerStream.Close();
        this.m_webResponse.Close();
        base.Close();
      }

      public override void Flush() => this.m_innerStream.Flush();

      public override IAsyncResult BeginRead(
        byte[] buffer,
        int offset,
        int count,
        AsyncCallback callback,
        object state)
      {
        this.ThrowIfCanceled();
        return this.m_innerStream.BeginRead(buffer, offset, count, callback, state);
      }

      public override IAsyncResult BeginWrite(
        byte[] buffer,
        int offset,
        int count,
        AsyncCallback callback,
        object state)
      {
        return this.m_innerStream.BeginWrite(buffer, offset, count, callback, state);
      }

      public override int EndRead(IAsyncResult asyncResult) => this.m_innerStream.EndRead(asyncResult);

      public override void EndWrite(IAsyncResult asyncResult) => this.m_innerStream.EndWrite(asyncResult);

      public override int Read(byte[] buffer, int offset, int count)
      {
        this.ThrowIfCanceled();
        return this.m_innerStream.Read(buffer, offset, count);
      }

      public override long Seek(long offset, SeekOrigin origin) => this.m_innerStream.Seek(offset, origin);

      public override void SetLength(long value) => this.m_innerStream.SetLength(value);

      public override void Write(byte[] buffer, int offset, int count) => this.m_innerStream.Write(buffer, offset, count);

      private void ThrowIfCanceled()
      {
        if (this.m_channel != null && this.m_channel.State == TfsHttpClientState.Aborted)
          throw new System.OperationCanceledException(ClientResources.CommandCanceled());
      }
    }
  }
}
