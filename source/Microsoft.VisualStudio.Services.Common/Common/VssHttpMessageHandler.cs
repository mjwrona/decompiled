// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Common.VssHttpMessageHandler
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using Microsoft.VisualStudio.Services.Common.Diagnostics;
using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Common
{
  public class VssHttpMessageHandler : HttpMessageHandler
  {
    internal static readonly string PropertyName = "MS.VS.MessageHandler";
    private static IWebProxy s_defaultWebProxy = WebRequest.DefaultWebProxy;
    internal const string CredentialsType = "CredentialsType";
    private const int m_maxAuthRetries = 3;
    private HttpMessageInvoker m_messageInvoker;
    private VssHttpMessageHandler.CredentialWrapper m_credentialWrapper;
    private bool m_appliedClientCertificatesToTransportHandler;
    private bool m_appliedServerCertificateValidationCallbackToTransportHandler;
    private readonly HttpMessageHandler m_transportHandler;

    public VssHttpMessageHandler()
      : this(new VssCredentials(), new VssHttpRequestSettings())
    {
    }

    public VssHttpMessageHandler(VssCredentials credentials, VssHttpRequestSettings settings)
      : this(credentials, settings, (HttpMessageHandler) new WebRequestHandler())
    {
    }

    public VssHttpMessageHandler(
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      HttpMessageHandler innerHandler)
    {
      this.Credentials = credentials;
      this.Settings = settings;
      this.ExpectContinue = settings.ExpectContinue;
      this.m_credentialWrapper = new VssHttpMessageHandler.CredentialWrapper();
      this.m_messageInvoker = new HttpMessageInvoker(innerHandler);
      HttpMessageHandler httpMessageHandler = innerHandler;
      for (DelegatingHandler delegatingHandler = httpMessageHandler as DelegatingHandler; delegatingHandler != null; delegatingHandler = httpMessageHandler as DelegatingHandler)
        httpMessageHandler = delegatingHandler.InnerHandler;
      this.m_transportHandler = httpMessageHandler;
      VssHttpMessageHandler.ApplySettings(this.m_transportHandler, (ICredentials) this.m_credentialWrapper, this.Settings);
    }

    public VssCredentials Credentials { get; private set; }

    public VssHttpRequestSettings Settings { get; private set; }

    private bool ExpectContinue { get; set; }

    protected override void Dispose(bool disposing)
    {
      base.Dispose(disposing);
      if (!disposing || this.m_messageInvoker == null)
        return;
      this.m_messageInvoker.Dispose();
    }

    protected override async Task<HttpResponseMessage> SendAsync(
      HttpRequestMessage request,
      CancellationToken cancellationToken)
    {
      VssHttpMessageHandler httpMessageHandler = this;
      VssTraceActivity traceActivity = VssTraceActivity.Current;
      VssHttpMessageHandlerTraceInfo traceInfo = VssHttpMessageHandlerTraceInfo.GetTraceInfo(request);
      traceInfo?.TraceHandlerStartTime();
      if (!httpMessageHandler.m_appliedClientCertificatesToTransportHandler && request.RequestUri.Scheme == "https")
      {
        if (httpMessageHandler.m_transportHandler is WebRequestHandler transportHandler && httpMessageHandler.Settings.ClientCertificateManager != null && httpMessageHandler.Settings.ClientCertificateManager.ClientCertificates != null && httpMessageHandler.Settings.ClientCertificateManager.ClientCertificates.Count > 0)
          transportHandler.ClientCertificates.AddRange((X509CertificateCollection) httpMessageHandler.Settings.ClientCertificateManager.ClientCertificates);
        httpMessageHandler.m_appliedClientCertificatesToTransportHandler = true;
      }
      if (!httpMessageHandler.m_appliedServerCertificateValidationCallbackToTransportHandler && request.RequestUri.Scheme == "https")
      {
        if (httpMessageHandler.m_transportHandler is WebRequestHandler transportHandler && httpMessageHandler.Settings.ServerCertificateValidationCallback != null)
          transportHandler.ServerCertificateValidationCallback = httpMessageHandler.Settings.ServerCertificateValidationCallback;
        httpMessageHandler.m_appliedServerCertificateValidationCallbackToTransportHandler = true;
      }
      IssuedToken token = (IssuedToken) null;
      IssuedTokenProvider provider;
      if (httpMessageHandler.Credentials.TryGetTokenProvider(request.RequestUri, out provider))
        token = provider.CurrentToken;
      request.Properties[VssHttpMessageHandler.PropertyName] = (object) httpMessageHandler;
      bool succeeded = false;
      bool lastResponseDemandedProxyAuth = false;
      int retries = 3;
      HttpResponseMessage response = (HttpResponseMessage) null;
      CancellationTokenSource tokenSource = (CancellationTokenSource) null;
      HttpResponseMessageWrapper responseWrapper;
      HttpResponseMessage httpResponseMessage;
      try
      {
        tokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        if (httpMessageHandler.Settings.SendTimeout > TimeSpan.Zero)
          tokenSource.CancelAfter(httpMessageHandler.Settings.SendTimeout);
        do
        {
          if (response != null)
            response.Dispose();
          httpMessageHandler.ApplyHeaders(request);
          httpMessageHandler.ApplyToken(request, token, lastResponseDemandedProxyAuth);
          lastResponseDemandedProxyAuth = false;
          await VssHttpMessageHandler.BufferRequestContentAsync(request, tokenSource.Token).ConfigureAwait(false);
          traceInfo?.TraceBufferedRequestTime();
          response = await httpMessageHandler.m_messageInvoker.SendAsync(request, tokenSource.Token).ConfigureAwait(false);
          traceInfo?.TraceRequestSendTime();
          await httpMessageHandler.BufferResponseContentAsync(request, response, (Func<string>) (() => "[ContentType: " + response.Content.GetType().Name + "]"), tokenSource.Token).ConfigureAwait(false);
          traceInfo?.TraceResponseContentTime();
          responseWrapper = new HttpResponseMessageWrapper(response);
          if (!httpMessageHandler.Credentials.IsAuthenticationChallenge((IHttpResponse) responseWrapper))
          {
            provider?.ValidateToken(token, (IHttpResponse) responseWrapper);
            httpMessageHandler.ExpectContinue = false;
            succeeded = true;
            break;
          }
          lastResponseDemandedProxyAuth = responseWrapper.StatusCode == HttpStatusCode.ProxyAuthenticationRequired;
          VssHttpEventSource.Log.AuthenticationFailed(traceActivity, response);
          provider?.InvalidateToken(token);
          provider = httpMessageHandler.Credentials.CreateTokenProvider(request.RequestUri, (IHttpResponse) responseWrapper, token);
          if (provider == null)
          {
            VssHttpEventSource.Log.IssuedTokenProviderNotFound(traceActivity);
            break;
          }
          if (provider.GetTokenIsInteractive && httpMessageHandler.Credentials.PromptType == CredentialPromptType.DoNotPrompt)
          {
            VssHttpEventSource.Log.IssuedTokenProviderPromptRequired(traceActivity, provider);
            break;
          }
          IEnumerable<string> values;
          bool flag = response.Headers.TryGetValues("X-VSS-AuthenticateError", out values) && !string.IsNullOrEmpty(values.FirstOrDefault<string>());
          if (retries != 0 && !(retries < 3 & flag))
          {
            token = await provider.GetTokenAsync(token, tokenSource.Token).ConfigureAwait(false);
            traceInfo?.TraceGetTokenTime();
            request.Headers.Add("X-VSS-UserData", string.Empty);
            --retries;
          }
          else
            break;
        }
        while (retries >= 0);
        if (traceInfo != null)
          traceInfo.TokenRetries = 3 - retries;
        if (!succeeded && response != null && httpMessageHandler.Credentials.IsAuthenticationChallenge((IHttpResponse) responseWrapper))
        {
          IEnumerable<string> values;
          string message = !response.Headers.TryGetValues("X-TFS-ServiceError", out values) ? CommonResources.VssUnauthorized((object) request.RequestUri.GetLeftPart(UriPartial.Authority)) : UriUtility.UrlDecode(values.FirstOrDefault<string>());
          if (response != null)
            response.Dispose();
          VssHttpEventSource.Log.HttpRequestUnauthorized(traceActivity, request, message);
          VssUnauthorizedException unauthorizedException = new VssUnauthorizedException(message);
          if (provider != null)
            unauthorizedException.Data.Add((object) "CredentialsType", (object) provider.CredentialType);
          throw unauthorizedException;
        }
        httpResponseMessage = response;
      }
      catch (OperationCanceledException ex)
      {
        if (cancellationToken.IsCancellationRequested)
        {
          VssHttpEventSource.Log.HttpRequestCancelled(traceActivity, request);
          throw;
        }
        else
        {
          VssHttpEventSource.Log.HttpRequestTimedOut(traceActivity, request, httpMessageHandler.Settings.SendTimeout);
          throw new TimeoutException(CommonResources.HttpRequestTimeout((object) httpMessageHandler.Settings.SendTimeout), (Exception) ex);
        }
      }
      finally
      {
        tokenSource?.Dispose();
        traceInfo?.TraceTrailingTime();
      }
      traceActivity = (VssTraceActivity) null;
      traceInfo = (VssHttpMessageHandlerTraceInfo) null;
      token = (IssuedToken) null;
      provider = (IssuedTokenProvider) null;
      responseWrapper = new HttpResponseMessageWrapper();
      tokenSource = (CancellationTokenSource) null;
      return httpResponseMessage;
    }

    private static async Task BufferRequestContentAsync(
      HttpRequestMessage request,
      CancellationToken cancellationToken)
    {
      if (request.Content == null)
        return;
      bool? transferEncodingChunked = request.Headers.TransferEncodingChunked;
      bool flag = true;
      if (transferEncodingChunked.GetValueOrDefault() == flag & transferEncodingChunked.HasValue)
        return;
      if (!request.Content.Headers.ContentLength.HasValue)
        await request.Content.LoadIntoBufferAsync().EnforceCancellation(cancellationToken, file: "D:\\a\\_work\\1\\s\\Vssf\\Client\\Common\\VssHttpMessageHandler.cs", member: nameof (BufferRequestContentAsync), line: 421).ConfigureAwait(false);
      request.Headers.TransferEncodingChunked = new bool?(false);
    }

    protected virtual async Task BufferResponseContentAsync(
      HttpRequestMessage request,
      HttpResponseMessage response,
      Func<string> makeErrorMessage,
      CancellationToken cancellationToken)
    {
      if (response == null || response.StatusCode == HttpStatusCode.NoContent || response.Content == null || this.Settings.MaxContentBufferSize == 0)
        return;
      HttpCompletionOption completionOption;
      if (!request.Properties.TryGetValue<HttpCompletionOption>("MS.VS.HttpCompletionOption", out completionOption))
        completionOption = HttpCompletionOption.ResponseContentRead;
      if (completionOption != HttpCompletionOption.ResponseContentRead)
        return;
      await response.Content.LoadIntoBufferAsync((long) this.Settings.MaxContentBufferSize).EnforceCancellation(cancellationToken, makeErrorMessage, "D:\\a\\_work\\1\\s\\Vssf\\Client\\Common\\VssHttpMessageHandler.cs", nameof (BufferResponseContentAsync), 464).ConfigureAwait(false);
    }

    private void ApplyHeaders(HttpRequestMessage request)
    {
      if (!this.Settings.ApplyTo(request))
        return;
      VssTraceActivity activity = request.GetActivity();
      if (activity != null && activity != VssTraceActivity.Empty && !request.Headers.Contains("X-TFS-Session"))
        request.Headers.Add("X-TFS-Session", activity.Id.ToString("D"));
      request.Headers.ExpectContinue = new bool?(this.ExpectContinue);
    }

    private void ApplyToken(
      HttpRequestMessage request,
      IssuedToken token,
      bool applyICredentialsToWebProxy = false)
    {
      if (token == null)
        return;
      if (token is ICredentials credentials)
      {
        if (applyICredentialsToWebProxy && this.m_transportHandler is HttpClientHandler transportHandler && transportHandler.Proxy != null)
          transportHandler.Proxy.Credentials = credentials;
        this.m_credentialWrapper.InnerCredentials = credentials;
      }
      else
        token.ApplyTo((IHttpRequest) new HttpRequestMessageWrapper(request));
    }

    private static void ApplySettings(
      HttpMessageHandler handler,
      ICredentials defaultCredentials,
      VssHttpRequestSettings settings)
    {
      if (!(handler is HttpClientHandler httpClientHandler))
        return;
      httpClientHandler.AllowAutoRedirect = settings.AllowAutoRedirect;
      httpClientHandler.ClientCertificateOptions = ClientCertificateOption.Manual;
      httpClientHandler.UseDefaultCredentials = false;
      httpClientHandler.Credentials = defaultCredentials;
      httpClientHandler.PreAuthenticate = false;
      httpClientHandler.Proxy = VssHttpMessageHandler.DefaultWebProxy;
      httpClientHandler.UseCookies = false;
      httpClientHandler.UseProxy = true;
      if (!settings.CompressionEnabled)
        return;
      httpClientHandler.AutomaticDecompression = DecompressionMethods.GZip;
    }

    public static IWebProxy DefaultWebProxy
    {
      get
      {
        VssHttpMessageHandler.WebProxyWrapper defaultWebProxy = VssHttpMessageHandler.WebProxyWrapper.Wrap(VssHttpMessageHandler.s_defaultWebProxy);
        if (defaultWebProxy != null && defaultWebProxy.Credentials == null)
          defaultWebProxy.Credentials = CredentialCache.DefaultCredentials;
        return (IWebProxy) defaultWebProxy;
      }
      set => VssHttpMessageHandler.s_defaultWebProxy = value;
    }

    private sealed class CredentialWrapper : ICredentials
    {
      public ICredentials InnerCredentials { get; set; }

      NetworkCredential ICredentials.GetCredential(Uri uri, string authType) => this.InnerCredentials == null ? (NetworkCredential) null : this.InnerCredentials.GetCredential(uri, authType);
    }

    private sealed class WebProxyWrapper : IWebProxy
    {
      private readonly IWebProxy m_wrapped;
      private ICredentials m_credentials;
      private static readonly ICredentials m_nullCredentials = (ICredentials) new VssHttpMessageHandler.CredentialWrapper();

      private WebProxyWrapper(IWebProxy toWrap)
      {
        this.m_wrapped = toWrap;
        this.m_credentials = (ICredentials) null;
      }

      public static VssHttpMessageHandler.WebProxyWrapper Wrap(IWebProxy toWrap) => toWrap == null ? (VssHttpMessageHandler.WebProxyWrapper) null : new VssHttpMessageHandler.WebProxyWrapper(toWrap);

      public ICredentials Credentials
      {
        get
        {
          ICredentials credentials = this.m_credentials;
          if (credentials == null)
            credentials = this.m_wrapped.Credentials;
          else if (credentials == VssHttpMessageHandler.WebProxyWrapper.m_nullCredentials)
            credentials = (ICredentials) null;
          return credentials;
        }
        set
        {
          if (value == null)
            this.m_credentials = VssHttpMessageHandler.WebProxyWrapper.m_nullCredentials;
          else
            this.m_credentials = value;
        }
      }

      public Uri GetProxy(Uri destination) => this.m_wrapped.GetProxy(destination);

      public bool IsBypassed(Uri host) => this.m_wrapped.IsBypassed(host);
    }
  }
}
