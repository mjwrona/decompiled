// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.OAuth.VssOAuthTokenHttpClient
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Common.Diagnostics;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.OAuth
{
  public class VssOAuthTokenHttpClient
  {
    private readonly Uri m_authorizationUrl;
    private readonly MediaTypeFormatter m_formatter;

    public VssOAuthTokenHttpClient(Uri authorizationUrl)
    {
      ArgumentUtility.CheckForNull<Uri>(authorizationUrl, nameof (authorizationUrl));
      this.m_authorizationUrl = authorizationUrl;
      this.m_formatter = (MediaTypeFormatter) new VssJsonMediaTypeFormatter();
    }

    public Uri AuthorizationUrl => this.m_authorizationUrl;

    public Task<VssOAuthTokenResponse> GetTokenAsync(
      VssOAuthTokenRequest request,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ArgumentUtility.CheckForNull<VssOAuthTokenRequest>(request, nameof (request));
      return this.GetTokenAsync(request.Grant, request.ClientCredential, request.Parameters, cancellationToken);
    }

    public async Task<VssOAuthTokenResponse> GetTokenAsync(
      VssOAuthGrant grant,
      VssOAuthClientCredential credential,
      VssOAuthTokenParameters tokenParameters = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      VssTraceActivity current = VssTraceActivity.Current;
      VssOAuthTokenResponse tokenAsync;
      using (HttpClient client = new HttpClient(VssOAuthTokenHttpClient.CreateMessageHandler(this.AuthorizationUrl)))
      {
        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, this.AuthorizationUrl);
        request.Content = VssOAuthTokenHttpClient.CreateRequestContent((IVssOAuthTokenParameterProvider) grant, (IVssOAuthTokenParameterProvider) credential, (IVssOAuthTokenParameterProvider) tokenParameters);
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        foreach (ProductInfoHeaderValue productInfoHeaderValue in VssClientHttpRequestSettings.Default.UserAgent)
        {
          if (!request.Headers.UserAgent.Contains(productInfoHeaderValue))
            request.Headers.UserAgent.Add(productInfoHeaderValue);
        }
        using (HttpResponseMessage response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false))
        {
          string aadCorrelationId = "Unknown";
          IEnumerable<string> values;
          if (response.Headers.TryGetValues("x-ms-request-id", out values))
            aadCorrelationId = string.Join(",", values);
          VssHttpEventSource.Log.AADCorrelationID(aadCorrelationId);
          if (!VssOAuthTokenHttpClient.IsValidTokenResponse(response))
            throw new VssServiceResponseException(response.StatusCode, await response.Content.ReadAsStringAsync().ConfigureAwait(false), (Exception) null);
          tokenAsync = await response.Content.ReadAsAsync<VssOAuthTokenResponse>((IEnumerable<MediaTypeFormatter>) new MediaTypeFormatter[1]
          {
            this.m_formatter
          }, cancellationToken).ConfigureAwait(false);
        }
      }
      return tokenAsync;
    }

    private static bool IsValidTokenResponse(HttpResponseMessage response)
    {
      if (response.StatusCode == HttpStatusCode.OK)
        return true;
      return response.StatusCode == HttpStatusCode.BadRequest && VssOAuthTokenHttpClient.IsJsonResponse(response);
    }

    private static HttpMessageHandler CreateMessageHandler(Uri requestUri)
    {
      VssHttpRetryOptions options = new VssHttpRetryOptions();
      options.RetryableStatusCodes.Add(HttpStatusCode.InternalServerError);
      options.RetryableStatusCodes.Add((HttpStatusCode) 429);
      WebRequestHandler webRequestHandler = new WebRequestHandler();
      webRequestHandler.UseDefaultCredentials = false;
      WebRequestHandler innerHandler = webRequestHandler;
      if (VssHttpMessageHandler.DefaultWebProxy != null)
      {
        innerHandler.Proxy = VssHttpMessageHandler.DefaultWebProxy;
        innerHandler.UseProxy = true;
      }
      if (requestUri.Scheme.Equals("https", StringComparison.OrdinalIgnoreCase) && VssClientHttpRequestSettings.Default.ClientCertificateManager != null && VssClientHttpRequestSettings.Default.ClientCertificateManager.ClientCertificates != null && VssClientHttpRequestSettings.Default.ClientCertificateManager.ClientCertificates.Count > 0)
        innerHandler.ClientCertificates.AddRange((X509CertificateCollection) VssClientHttpRequestSettings.Default.ClientCertificateManager.ClientCertificates);
      if (requestUri.Scheme.Equals("https", StringComparison.OrdinalIgnoreCase) && VssClientHttpRequestSettings.Default.ServerCertificateValidationCallback != null)
        innerHandler.ServerCertificateValidationCallback = VssClientHttpRequestSettings.Default.ServerCertificateValidationCallback;
      return (HttpMessageHandler) new VssHttpRetryMessageHandler(options, (HttpMessageHandler) innerHandler);
    }

    private static HttpContent CreateRequestContent(
      params IVssOAuthTokenParameterProvider[] parameterProviders)
    {
      Dictionary<string, string> dictionary = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      foreach (IVssOAuthTokenParameterProvider parameterProvider in parameterProviders)
        parameterProvider?.SetParameters((IDictionary<string, string>) dictionary);
      return (HttpContent) new FormUrlEncodedContent((IEnumerable<KeyValuePair<string, string>>) dictionary);
    }

    private static bool HasContent(HttpResponseMessage response)
    {
      if (response != null && response.StatusCode != HttpStatusCode.NoContent && response.Content != null && response.Content.Headers != null)
      {
        long? contentLength = response.Content.Headers.ContentLength;
        if (contentLength.HasValue)
        {
          contentLength = response.Content.Headers.ContentLength;
          long num = 0;
          if (!(contentLength.GetValueOrDefault() == num & contentLength.HasValue))
            return true;
        }
      }
      return false;
    }

    private static bool IsJsonResponse(HttpResponseMessage response) => VssOAuthTokenHttpClient.HasContent(response) && response.Content.Headers != null && response.Content.Headers.ContentType != null && !string.IsNullOrEmpty(response.Content.Headers.ContentType.MediaType) && string.Equals("application/json", response.Content.Headers.ContentType.MediaType, StringComparison.OrdinalIgnoreCase);
  }
}
