// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.WebApiProxy.HttpRequestProxy
// Assembly: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 002C83BC-B53E-470A-8038-76E47B5E5BF3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.DistributedTask;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ServiceEndpoints.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Net.Sockets;
using System.Reflection;
using System.Text;

namespace Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.WebApiProxy
{
  public class HttpRequestProxy
  {
    private readonly Func<IVssRequestContext, HttpWebRequest, HttpRequestMessage, WebRequestHandler, HttpResponseMessage> executor;
    private string endpointId;
    private const int Maxsize = 2097152;
    private readonly IVssRequestContext _context;
    public const string UseServiceIdentityForInternalCalls = "ServiceEndpointProxy.UseServiceIdentityForInternalCalls";
    private static readonly List<string> UrlWhiteList = new List<string>()
    {
      "kusto.windows.net",
      "microsofticm.com"
    };

    protected HttpRequestProxy(
      Func<IVssRequestContext, HttpWebRequest, HttpRequestMessage, WebRequestHandler, HttpResponseMessage> executor,
      IVssRequestContext context,
      string scopeIdentifier,
      string endpointId,
      string url,
      IDictionary<string, string> requestParameters,
      IEndpointAuthorizer authorizer,
      string resourceUrl,
      ResponseSelector selector)
    {
      this.executor = executor;
      this.endpointId = endpointId;
      if (requestParameters != null)
      {
        string str1;
        requestParameters.TryGetValue("requestVerb", out str1);
        string str2;
        requestParameters.TryGetValue("requestContent", out str2);
        this.RequestVerb = string.IsNullOrEmpty(str1) ? "GET" : str1.ToUpperInvariant();
        this.RequestContent = str2;
      }
      else
        this.RequestVerb = "GET";
      this.Authorizer = authorizer;
      this.Selector = selector;
      this.Url = url;
      this.ResourceUrl = resourceUrl;
      this.ScopeIdentifier = scopeIdentifier;
      this._context = context;
    }

    public HttpRequestProxy(
      IVssRequestContext context,
      string scopeIdentifier,
      string endpointId,
      string url,
      IDictionary<string, string> requestParameters,
      IEndpointAuthorizer authorizer,
      string resourceUrl,
      ResponseSelector selector)
      : this(HttpRequestProxy.\u003C\u003EO.\u003C0\u003E__ExecuteHttpRequest ?? (HttpRequestProxy.\u003C\u003EO.\u003C0\u003E__ExecuteHttpRequest = new Func<IVssRequestContext, HttpWebRequest, HttpRequestMessage, WebRequestHandler, HttpResponseMessage>(HttpRequestProxy.ExecuteHttpRequest)), context, scopeIdentifier, endpointId, url, requestParameters, authorizer, resourceUrl, selector)
    {
      // ISSUE: reference to a compiler-generated field (out of statement scope)
      // ISSUE: reference to a compiler-generated field (out of statement scope)
    }

    public virtual ResponseSelectorResult ExecuteRequest(
      IList<TaskSourceDefinition> sourceDefinitions,
      IList<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.AuthorizationHeader> customHeaders,
      bool acceptUntrustedCertificates,
      ref HttpStatusCode httpStatusCode,
      out string errorMessage,
      out string endpointType,
      out string endpointUrl,
      bool throwOnException,
      int timeoutInSeconds = 20)
    {
      ResponseSelectorResult responseSelectorResult = new ResponseSelectorResult();
      using (HttpResponseMessage response = this.InvokeWebRequest(sourceDefinitions, customHeaders, acceptUntrustedCertificates, ref httpStatusCode, out errorMessage, timeoutInSeconds, throwOnException))
      {
        if (this.IsSuccessStatusCode(httpStatusCode))
        {
          if (string.IsNullOrEmpty(errorMessage))
            responseSelectorResult = this.ParseWebResponse(response, ref httpStatusCode, out errorMessage, throwOnException);
        }
      }
      endpointType = this.Authorizer.GetServiceEndpointType();
      endpointUrl = this.Authorizer.GetEndpointUrl();
      return responseSelectorResult;
    }

    public virtual ResponseSelectorResult ExecuteRequest(
      IList<TaskSourceDefinition> sourceDefinitions,
      IList<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.AuthorizationHeader> customHeaders,
      bool acceptUntrustedCertificates,
      ref HttpStatusCode httpStatusCode,
      out string errorMessage,
      int timeoutInSeconds = 20)
    {
      ResponseSelectorResult responseSelectorResult = (ResponseSelectorResult) null;
      using (HttpResponseMessage response = this.InvokeWebRequest(sourceDefinitions, customHeaders, acceptUntrustedCertificates, ref httpStatusCode, out errorMessage, timeoutInSeconds, false))
      {
        if (this.IsSuccessStatusCode(httpStatusCode))
        {
          if (string.IsNullOrEmpty(errorMessage))
            responseSelectorResult = this.ParseWebResponse(response, ref httpStatusCode, out errorMessage, false);
        }
      }
      return responseSelectorResult ?? new ResponseSelectorResult();
    }

    private HttpRequestMessage GetRequestMessageFromWebRequest(HttpWebRequest webRequest)
    {
      HttpRequestMessage messageFromWebRequest = new HttpRequestMessage()
      {
        Method = new HttpMethod(webRequest.Method),
        RequestUri = webRequest.RequestUri
      };
      if (this.RequestContent != null)
        messageFromWebRequest.Content = (HttpContent) new StringContent(this.RequestContent, Encoding.UTF8, webRequest.ContentType);
      if (webRequest.Accept != null)
        messageFromWebRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(webRequest.Accept));
      foreach (string name in ((IEnumerable<string>) webRequest.Headers.AllKeys).Where<string>((Func<string, bool>) (x => !string.Equals(x, "Content-Type", StringComparison.OrdinalIgnoreCase) && !string.Equals(x, "Accept", StringComparison.OrdinalIgnoreCase))).ToList<string>())
        messageFromWebRequest.Headers.TryAddWithoutValidation(name, webRequest.Headers[name]);
      return messageFromWebRequest;
    }

    private HttpResponseMessage InvokeWebRequest(
      IList<TaskSourceDefinition> sourceDefinitions,
      IList<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.AuthorizationHeader> customHeaders,
      bool acceptUntrustedCertificates,
      ref HttpStatusCode httpStatusCode,
      out string errorMessage,
      int timeoutInSeconds,
      bool throwOnException = true)
    {
      IPAddress ipAddress = (IPAddress) null;
      Uri uri1;
      if (this._context.IsFeatureEnabled("ServiceEndpoints.EndpointProxyValidateRelativeUrls"))
      {
        uri1 = UrlHelper.ValidateAndNormalize(this._context, this.Url, this.Authorizer.GetEndpointUrl(), sourceDefinitions, this.Authorizer.SupportsAbsoluteEndpoint);
        if (!this._context.ExecutionEnvironment.IsOnPremisesDeployment && !this._context.ExecutionEnvironment.IsDevFabricDeployment)
        {
          IPAddress[] source = UrlHelper.VerifyUrlIsAllowedIPAddress(uri1.ToString());
          if (this._context.IsFeatureEnabled("ServiceEndpoints.EndpointProxy.DNSPinning.Enabled") && source != null && source.Length != 0)
          {
            if (this._context.IsFeatureEnabled("ServiceEndpoints.EndpointProxy.PreferIPv4.Enabled"))
              ipAddress = ((IEnumerable<IPAddress>) source).FirstOrDefault<IPAddress>((Func<IPAddress, bool>) (ip => ip.AddressFamily == AddressFamily.InterNetwork));
            if (ipAddress == null)
              ipAddress = source[0];
          }
        }
      }
      else
        uri1 = this.ValidateAndNormalize(this.Url, sourceDefinitions);
      if (!(WebRequest.Create(uri1) is HttpWebRequest httpWebRequest))
        throw new InvalidOperationException(ServiceEndpointSdkResources.UriParseError());
      httpWebRequest.Method = this.RequestVerb;
      if (string.Equals("POST", this.RequestVerb, StringComparison.InvariantCultureIgnoreCase))
      {
        if (!string.IsNullOrEmpty(this.RequestContent) && this.RequestContent.Length > BearerTokenArgument.MaxBodySizeSupported)
          throw new InvalidOperationException(ServiceEndpointSdkResources.BodySizeLimitExceeded((object) BearerTokenArgument.MaxBodySizeSupported));
        ServiceEndpointSecurity endpointSecurity = new ServiceEndpointSecurity();
        if (!this.IsWhiteListedPostCall(uri1) && !endpointSecurity.IsCallerServicePrincipal(this._context, this.ScopeIdentifier, alwaysAllowAdministrators: true))
          throw new InvalidOperationException(ServiceEndpointSdkResources.InsufficientPermissions());
        if (!string.IsNullOrEmpty(this.RequestContent))
        {
          using (StreamWriter streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
          {
            streamWriter.Write(this.RequestContent);
            streamWriter.Flush();
            streamWriter.Close();
          }
        }
      }
      if (acceptUntrustedCertificates)
        httpWebRequest.ServerCertificateValidationCallback = (RemoteCertificateValidationCallback) ((sender, certificate, chain, sslPolicyErrors) => true);
      httpWebRequest.UserAgent = this.GetVsoUserAgentValue();
      this.Selector.AddHeaders(httpWebRequest);
      if (this._context.IsFeatureEnabled("ServiceEndpoints.EnableCustomHeadersInExtensionDataSource"))
      {
        if (!this.AppendCustomHeaders(httpWebRequest, customHeaders, ref httpStatusCode, out errorMessage, throwOnException))
          return (HttpResponseMessage) null;
        this._context.TraceInfo("WebApiProxy", "Added custom headers specified in extension's data source");
      }
      if (!this.AddAuthorization(httpWebRequest, ref httpStatusCode, out errorMessage, throwOnException))
        return (HttpResponseMessage) null;
      httpWebRequest.Timeout = timeoutInSeconds * 1000;
      HttpResponseMessage response = (HttpResponseMessage) null;
      try
      {
        HttpRequestMessage messageFromWebRequest = this.GetRequestMessageFromWebRequest(httpWebRequest);
        if (this._context.IsFeatureEnabled("ServiceEndpoints.EndpointProxy.DNSPinning.Enabled") && ipAddress != null)
        {
          messageFromWebRequest.Headers.Host = uri1.Host;
          Uri uri2 = new UriBuilder(uri1)
          {
            Host = ipAddress.ToString()
          }.Uri;
          messageFromWebRequest.RequestUri = uri2;
        }
        if (this._context.IsFeatureEnabled("ServiceEndpointProxy.UseServiceIdentityForInternalCalls") && this.Authorizer is InternalEndpointAuthorizer)
        {
          this._context.GetClient<ServiceEndpointProxyHttpClient>(ServiceInstanceTypes.TFS).SendRequest(messageFromWebRequest, out response, out httpStatusCode, out errorMessage);
        }
        else
        {
          WebRequestHandler handler = new WebRequestHandler();
          handler.ClientCertificates.AddRange(httpWebRequest.ClientCertificates);
          handler.ServerCertificateValidationCallback = httpWebRequest.ServerCertificateValidationCallback;
          response = HttpRequestProxy.ExecuteHttpRequest(this._context, httpWebRequest, messageFromWebRequest, handler);
        }
        httpStatusCode = response.StatusCode;
        if (response.IsSuccessStatusCode)
        {
          this._context.TraceInfo("WebApiProxy", "Successfully fetched response for the request.");
        }
        else
        {
          string result = response.Content?.ReadAsStringAsync().Result;
          errorMessage = ServiceEndpointSdkResources.ServiceEndpointRequestFailedMessage((object) httpWebRequest.RequestUri, (object) response.StatusCode, (object) result);
          this._context.TraceError(34000216, "ServiceEndpoints", errorMessage);
          if (throwOnException)
            throw new Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpointQueryFailedException(ServiceEndpointSdkResources.ServiceEndpointRequestFailedMessageWithoutPrefix((object) response.StatusCode, (object) result));
        }
      }
      catch (Exception ex)
      {
        errorMessage = ServiceEndpointSdkResources.ServiceEndpointRequestFailedGenericMessage((object) httpWebRequest.RequestUri, (object) ex.Message);
        this._context.TraceError(34000216, "ServiceEndpoints", errorMessage + ex.StackTrace);
        if (throwOnException)
        {
          if (!this.IsSuccessStatusCode(httpStatusCode))
            throw new Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpointQueryFailedException(errorMessage, ex);
        }
      }
      return response;
    }

    private bool AppendCustomHeaders(
      HttpWebRequest request,
      IList<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.AuthorizationHeader> customHeaders,
      ref HttpStatusCode httpStatusCode,
      out string errorMessage,
      bool throwOnException = true)
    {
      errorMessage = string.Empty;
      string name = string.Empty;
      string str = string.Empty;
      try
      {
        if (request != null)
        {
          if (customHeaders != null)
          {
            foreach (Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.AuthorizationHeader customHeader in (IEnumerable<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.AuthorizationHeader>) customHeaders)
            {
              if (customHeader != null && customHeader.Name != null && customHeader.Value != null)
              {
                name = customHeader.Name.Trim();
                str = customHeader.Value.Trim();
                if (name.Equals("Content-Type", StringComparison.OrdinalIgnoreCase))
                  request.ContentType = str;
                else if (name.Equals("Accept", StringComparison.OrdinalIgnoreCase))
                  request.Accept = str;
                else if (name.Equals("TransferEncoding", StringComparison.OrdinalIgnoreCase))
                  request.TransferEncoding = str;
                else if (name.Equals("Content-Length", StringComparison.OrdinalIgnoreCase))
                {
                  try
                  {
                    request.ContentLength = Convert.ToInt64(str);
                  }
                  catch (Exception ex)
                  {
                    request.Headers[name] = str;
                  }
                }
                else
                  request.Headers[name] = str;
              }
            }
          }
        }
      }
      catch (Exception ex)
      {
        httpStatusCode = HttpStatusCode.BadRequest;
        errorMessage = string.Format("{0}\r\n HeaderName: '{1}' and HeaderValue: '{2}'", (object) ex.Message, (object) name, (object) str);
        this._context.TraceWarning("WebApiProxy", errorMessage);
        return false;
      }
      return true;
    }

    protected static HttpResponseMessage ExecuteHttpRequest(
      IVssRequestContext context,
      HttpWebRequest request,
      HttpRequestMessage message,
      WebRequestHandler handler)
    {
      using (new MethodScope(context, "WebApiProxy", string.Format("HttpCallStart {0} {1}", (object) message.Method.Method, (object) message.RequestUri.AbsoluteUri)))
        return TaskExtensions.SyncResult(new HttpClient((HttpMessageHandler) handler)
        {
          Timeout = TimeSpan.FromMilliseconds((double) request.Timeout)
        }.SendAsync(message));
    }

    private bool IsSuccessStatusCode(HttpStatusCode httpStatusCode)
    {
      if (!string.Equals(this.RequestVerb, "POST", StringComparison.OrdinalIgnoreCase))
        return httpStatusCode == HttpStatusCode.OK;
      return HttpStatusCode.OK <= httpStatusCode && httpStatusCode <= (HttpStatusCode) 299;
    }

    private bool IsWhiteListedPostCall(Uri uri) => this.IsWhitelistedUrl(uri) || string.Equals(this.Authorizer?.GetType().Name, "AzureEndpointOidcFederationAuthorizer", StringComparison.OrdinalIgnoreCase) || string.Equals(this.Authorizer?.GetType().Name, "AzureEndpointServicePrincipalAuthorizer", StringComparison.OrdinalIgnoreCase) || string.Equals(this.Authorizer?.GetType().Name, "KubernetesAuthorizer", StringComparison.OrdinalIgnoreCase);

    protected bool IsWhitelistedUrl(Uri uri)
    {
      string host = uri.Host;
      return !host.IsNullOrEmpty<char>() && HttpRequestProxy.UrlWhiteList.Any<string>((Func<string, bool>) (trustedDomain => UrlHelper.IsSubDomain(host, trustedDomain)));
    }

    private string GetVsoUserAgentValue()
    {
      string str = string.Empty;
      try
      {
        foreach (object customAttribute in typeof (HttpRequestProxy).Assembly.GetCustomAttributes(false))
        {
          if (customAttribute is AssemblyFileVersionAttribute)
          {
            str = ((AssemblyFileVersionAttribute) customAttribute).Version;
            break;
          }
        }
      }
      catch (Exception ex)
      {
        str = "0.0";
      }
      return "vsts-serviceendpointproxy-service/v." + str + " " + (string.IsNullOrEmpty(this.endpointId) ? string.Empty : "(EndpointId/" + this.endpointId + ")");
    }

    private bool AddAuthorization(
      HttpWebRequest request,
      ref HttpStatusCode httpStatusCode,
      out string errorMessage,
      bool throwOnException = true)
    {
      errorMessage = string.Empty;
      try
      {
        this.Authorizer.AuthorizeRequest(request, this.ResourceUrl);
        return true;
      }
      catch (Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.InvalidAuthorizationDetailsException ex)
      {
        httpStatusCode = HttpStatusCode.BadRequest;
        errorMessage = ex.Message;
        if (ex.InnerException != null)
          errorMessage = string.Format("{0}. {1}: {2}", (object) errorMessage, (object) ServiceEndpointSdkResources.ExceptionMessage(), (object) ex.InnerException.Message);
        this._context.TraceWarning("WebApiProxy", errorMessage);
        if (throwOnException)
          throw;
      }
      return false;
    }

    private ResponseSelectorResult ParseWebResponse(
      HttpResponseMessage response,
      ref HttpStatusCode httpStatusCode,
      out string errorMessage,
      bool throwOnException = true)
    {
      errorMessage = string.Empty;
      try
      {
        return this.Selector.Select(response);
      }
      catch (Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.InvalidEndpointResponseException ex)
      {
        httpStatusCode = HttpStatusCode.BadRequest;
        errorMessage = ex.Message;
        if (ex.InnerException != null)
          errorMessage = string.Format("{0}. Exception Message: {1}", (object) errorMessage, (object) ex.InnerException.Message);
        if (!throwOnException)
          return new ResponseSelectorResult();
        throw;
      }
    }

    private Uri ValidateAndNormalize(string workUrl, IList<TaskSourceDefinition> sourceDefinitions)
    {
      if (!UrlHelper.IsAbsolute(workUrl))
      {
        string endpointUrl = this.Authorizer.GetEndpointUrl();
        if (!string.IsNullOrWhiteSpace(endpointUrl))
        {
          if (!endpointUrl.EndsWith("/"))
            endpointUrl += "/";
          if (workUrl.StartsWith("/"))
            workUrl = workUrl.Substring(1);
          return new Uri(endpointUrl + workUrl);
        }
      }
      else
      {
        if (!this.Authorizer.SupportsAbsoluteEndpoint)
          throw new InvalidOperationException(ServiceEndpointSdkResources.AbsoluteUriNotAllowed());
        if ((this._context.ExecutionEnvironment.IsOnPremisesDeployment ? 1 : (this._context.ExecutionEnvironment.IsDevFabricDeployment ? 1 : 0)) == 0 && this._context.IsFeatureEnabled("ServiceEndpoints.VerifyEndpointIpAddresses"))
          UrlHelper.VerifyUrlIsAllowedIPAddress(workUrl);
        UrlHelper.ValidateEndpoint(sourceDefinitions, workUrl);
      }
      return new Uri(workUrl);
    }

    private IEndpointAuthorizer Authorizer { get; set; }

    private string ScopeIdentifier { get; set; }

    private string RequestVerb { get; set; }

    private string RequestContent { get; set; }

    private ResponseSelector Selector { get; set; }

    public string Url { get; set; }

    public string ResourceUrl { get; set; }
  }
}
