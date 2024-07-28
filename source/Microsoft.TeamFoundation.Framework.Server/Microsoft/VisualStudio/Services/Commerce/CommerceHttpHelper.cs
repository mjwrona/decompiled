// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.CommerceHttpHelper
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Commerce
{
  public class CommerceHttpHelper : ICommerceHttpHelper, IVssFrameworkService
  {
    private string AssemblyVersion;
    private static readonly ConcurrentDictionary<string, HttpMessageHandler> s_certificateRequestHandlers = new ConcurrentDictionary<string, HttpMessageHandler>();
    private static readonly ConcurrentDictionary<string, HttpMessageHandler> s_jwtTokenRequestHandlers = new ConcurrentDictionary<string, HttpMessageHandler>();
    private static JsonMediaTypeFormatter s_formatter;
    private const string TraceArea = "Commerce";
    private const string Layer = "CommerceHttpHelper";
    private const string ClientRequestHeader = "x-ms-client-request-id";
    private const string EnableHttpHandlersCachingFeatureFlag = "VisualStudio.Services.Commerce.EnableHttpHandlersCaching";
    private const string CommerceUserAgentName = "AzureDevOps-Commerce";

    public void ServiceStart(IVssRequestContext systemRequestContext) => this.AssemblyVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public virtual HttpResponseMessage GetHttpResponseMessage(
      IVssRequestContext requestContext,
      HttpClient httpClient,
      Uri serviceUri,
      HttpMethod httpMethod,
      int slowRequestThresholdMilliseconds,
      IList<HttpStatusCode> whitelistedStatusCodes = null,
      Action<HttpResponseMessage> failureAction = null)
    {
      return this.GetHttpResponseMessage(requestContext, httpClient, serviceUri, httpMethod, (object) null, slowRequestThresholdMilliseconds, whitelistedStatusCodes, failureAction);
    }

    public virtual HttpResponseMessage GetHttpResponseMessage(
      IVssRequestContext requestContext,
      HttpClient httpClient,
      Uri serviceUri,
      HttpMethod httpMethod,
      object content,
      int slowRequestThresholdMilliseconds,
      IList<HttpStatusCode> whitelistedStatusCodes = null,
      Action<HttpResponseMessage> failureAction = null)
    {
      requestContext.TraceEnter(5106741, "Commerce", nameof (CommerceHttpHelper), nameof (GetHttpResponseMessage));
      try
      {
        Stopwatch stopwatch = Stopwatch.StartNew();
        requestContext.Trace(5106754, TraceLevel.Info, "Commerce", nameof (CommerceHttpHelper), serviceUri.ToString());
        HttpResponseMessage response;
        if (httpMethod == HttpMethod.Get)
          response = httpClient.GetAsync(serviceUri).Result;
        else if (httpMethod == HttpMethod.Delete)
          response = httpClient.DeleteAsync(serviceUri).Result;
        else if (httpMethod == HttpMethod.Post)
          response = httpClient.PostAsJsonAsync<object>(serviceUri, content).Result;
        else if (httpMethod == HttpMethod.Put)
        {
          response = httpClient.PutAsJsonAsync<object>(serviceUri, content).Result;
        }
        else
        {
          if (!httpMethod.Method.Equals("PATCH"))
            throw new ArgumentException(nameof (httpMethod));
          response = CommerceHttpHelper.PatchAsJsonAsync(httpClient, serviceUri, content).Result;
        }
        if (!response.IsSuccessStatusCode && (whitelistedStatusCodes == null || !whitelistedStatusCodes.Any<HttpStatusCode>((Func<HttpStatusCode, bool>) (c => c == response.StatusCode))))
        {
          requestContext.Trace(5106745, TraceLevel.Error, "Commerce", nameof (CommerceHttpHelper), string.Format("StatusCode: {0} ({1}) \r\nError content: {2} for Uri {3} Stack trace: {4}", (object) response.StatusCode, (object) (int) response.StatusCode, (object) response.Content.ReadAsStringAsync().Result, (object) serviceUri, (object) EnvironmentWrapper.ToReadableStackTrace()));
          if (failureAction != null)
            failureAction(response);
        }
        requestContext.TraceConditionally(5106757, TraceLevel.Info, "Commerce", nameof (CommerceHttpHelper), (Func<string>) (() => string.Format("GetHttpResponseMessage with method {0} to {1} returned status code {2} with content: {3}", (object) httpMethod, (object) serviceUri, (object) response.StatusCode, (object) response.Content.ReadAsStringAsync().Result)));
        stopwatch.Stop();
        double elapsedMilliseconds = (double) stopwatch.ElapsedMilliseconds;
        if (elapsedMilliseconds > (double) slowRequestThresholdMilliseconds)
          requestContext.Trace(5106743, TraceLevel.Error, "Commerce", nameof (CommerceHttpHelper), "SlowRequest: Url: {0} ElapsedTime: {1}ms", (object) serviceUri.OriginalString, (object) elapsedMilliseconds);
        return response;
      }
      catch (AggregateException ex)
      {
        requestContext.TraceException(5106744, "Commerce", nameof (CommerceHttpHelper), (Exception) ex);
        AggregateException aggregateException1 = ex.Flatten();
        Exception exception1;
        if (aggregateException1 == null)
        {
          exception1 = (Exception) null;
        }
        else
        {
          ReadOnlyCollection<Exception> innerExceptions = aggregateException1.InnerExceptions;
          exception1 = innerExceptions != null ? innerExceptions.FirstOrDefault<Exception>((Func<Exception, bool>) (e => e is TaskCanceledException)) : (Exception) null;
        }
        Exception inner = exception1;
        if (inner != null)
        {
          HttpRequestException requestException = new HttpRequestException(inner.Message + " Request URL: " + serviceUri.OriginalString, inner);
          requestException.Data.Add((object) "{421AC3F1-A306-4C9B-B3F6-5812F9121FC8}", (object) true);
          throw requestException;
        }
        AggregateException aggregateException2 = ex.Flatten();
        Exception exception2;
        if (aggregateException2 == null)
        {
          exception2 = (Exception) null;
        }
        else
        {
          ReadOnlyCollection<Exception> innerExceptions = aggregateException2.InnerExceptions;
          exception2 = innerExceptions != null ? innerExceptions.FirstOrDefault<Exception>() : (Exception) null;
        }
        Exception exception3 = exception2;
        if (exception3 != null)
          throw exception3;
        throw;
      }
      finally
      {
        requestContext.TraceLeave(5106742, "Commerce", nameof (CommerceHttpHelper), nameof (GetHttpResponseMessage));
      }
    }

    public virtual HttpClient GetHttpClientWithJwtTokenAuth(
      IVssRequestContext requestContext,
      JwtSecurityToken jwtToken,
      int requestTimeOut)
    {
      return this.GetHttpClientWithJwtTokenAuth(requestContext, string.Empty, jwtToken, requestTimeOut);
    }

    private HttpMessageHandler CreateHttpHandler(
      IVssRequestContext requestContext,
      string serviceName,
      HttpMessageHandler innerHandler)
    {
      List<DelegatingHandler> delegatingHandlers = ClientProviderHelper.GetMinimalDelegatingHandlers(requestContext, typeof (CommerceHttpHelper), new ClientProviderHelper.Options(2, TimeSpan.FromSeconds(10.0), (byte) 100), serviceName ?? "commerce", useAcceptLanguageHandler: false);
      return HttpClientFactory.CreatePipeline(innerHandler, (IEnumerable<DelegatingHandler>) delegatingHandlers);
    }

    public virtual HttpClient GetHttpClientWithJwtTokenAuth(
      IVssRequestContext requestContext,
      string serviceName,
      JwtSecurityToken jwtToken,
      int requestTimeOut)
    {
      if (jwtToken == null)
        return (HttpClient) null;
      HttpMessageHandler innerHandler = string.IsNullOrEmpty(serviceName) || !requestContext.IsFeatureEnabled("VisualStudio.Services.Commerce.EnableHttpHandlersCaching") ? (HttpMessageHandler) new HttpClientHandler() : CommerceHttpHelper.s_jwtTokenRequestHandlers.GetOrAdd(serviceName, (Func<string, HttpMessageHandler>) (key => (HttpMessageHandler) new HttpClientHandler()));
      HttpClient withJwtTokenAuth = new HttpClient(this.CreateHttpHandler(requestContext, serviceName, innerHandler));
      withJwtTokenAuth.Timeout = TimeSpan.FromMilliseconds((double) requestTimeOut);
      withJwtTokenAuth.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken.RawData);
      withJwtTokenAuth.DefaultRequestHeaders.Add("x-ms-client-request-id", requestContext.ActivityId.ToString());
      withJwtTokenAuth.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("AzureDevOps-Commerce", this.AssemblyVersion));
      return withJwtTokenAuth;
    }

    public virtual HttpClient GetHttpClientWithCertificate(
      IVssRequestContext requestContext,
      X509Certificate2 certificate,
      int requestTimeOut)
    {
      ArgumentUtility.CheckForNull<X509Certificate2>(certificate, nameof (certificate));
      ArgumentUtility.CheckForNull<string>(certificate.Thumbprint, "Thumbprint");
      string lowerInvariant = certificate.Thumbprint.ToLowerInvariant();
      HttpMessageHandler innerHandler;
      if (requestContext.IsFeatureEnabled("VisualStudio.Services.Commerce.EnableHttpHandlersCaching"))
        innerHandler = CommerceHttpHelper.s_certificateRequestHandlers.GetOrAdd(lowerInvariant, (Func<string, HttpMessageHandler>) (key => (HttpMessageHandler) new HttpClientHandler()
        {
          ClientCertificates = {
            (X509Certificate) certificate
          }
        }));
      else
        innerHandler = (HttpMessageHandler) new HttpClientHandler()
        {
          ClientCertificates = {
            (X509Certificate) certificate
          }
        };
      return new HttpClient(this.CreateHttpHandler(requestContext, lowerInvariant, innerHandler))
      {
        Timeout = TimeSpan.FromMilliseconds((double) requestTimeOut),
        DefaultRequestHeaders = {
          UserAgent = {
            new ProductInfoHeaderValue("AzureDevOps-Commerce", this.AssemblyVersion)
          }
        }
      };
    }

    public static JsonMediaTypeFormatter JsonMediaTypeFormatter
    {
      get
      {
        if (CommerceHttpHelper.s_formatter == null)
        {
          JsonMediaTypeFormatter mediaTypeFormatter = new JsonMediaTypeFormatter();
          mediaTypeFormatter.SerializerSettings.ContractResolver = (IContractResolver) new CamelCasePropertyNamesContractResolver();
          mediaTypeFormatter.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
          CommerceHttpHelper.s_formatter = mediaTypeFormatter;
        }
        return CommerceHttpHelper.s_formatter;
      }
    }

    private static Task<HttpResponseMessage> PatchAsJsonAsync(
      HttpClient client,
      Uri requestUri,
      object value)
    {
      ArgumentUtility.CheckForNull<HttpClient>(client, nameof (client));
      ArgumentUtility.CheckForNull<Uri>(requestUri, nameof (requestUri));
      ArgumentUtility.CheckForNull<object>(value, nameof (value));
      ObjectContent<object> objectContent = new ObjectContent<object>(value, (MediaTypeFormatter) new JsonMediaTypeFormatter());
      HttpRequestMessage request = new HttpRequestMessage(new HttpMethod("PATCH"), requestUri)
      {
        Content = (HttpContent) objectContent
      };
      return client.SendAsync(request);
    }
  }
}
