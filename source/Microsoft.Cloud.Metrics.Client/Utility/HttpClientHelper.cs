// Decompiled with JetBrains decompiler
// Type: Microsoft.Cloud.Metrics.Client.Utility.HttpClientHelper
// Assembly: Microsoft.Cloud.Metrics.Client, Version=2.2023.705.2051, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 06B39E1C-7DF0-4BC1-AFBA-9AD635E73CB0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Cloud.Metrics.Client.dll

using Microsoft.Cloud.Metrics.Client.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Cloud.Metrics.Client.Utility
{
  public static class HttpClientHelper
  {
    public const int HttpStatusCodeThrottled = 429;
    public static readonly HashSet<string> AllowedServerCertificateSimpleNames = new HashSet<string>((IEnumerable<string>) new string[8]
    {
      "*.dc.ad.msft.net",
      "*.test.dc.ad.msft.net",
      "*.ff.dc.ad.msft.net",
      "*.test.ff.dc.ad.msft.net",
      "*.cn.dc.ad.msft.net",
      "*.metrics.azure.microsoft.scloud",
      "*.metrics.azure.eaglex.ic.gov",
      "*.metrics.nsatc.net"
    }, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    internal const int DefaultMaxWebRequestsPerMinute = 1000;
    private const string ThrottledIdentityKey = "Throttled-Identity";
    private const string RetryAfterKey = "Retry-After";
    private static readonly string AssemblyVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
    private static readonly object LogId = Logger.CreateCustomLogId(nameof (HttpClientHelper));
    private static readonly ConcurrentDictionary<string, ConcurrentDictionary<string, DateTime>> ServerThrottledIdentities;
    private static readonly DateTime StartupTime = DateTime.UtcNow;
    private static volatile int currentMinute = 0;
    private static int requestsSentInCurrentMinute;

    static HttpClientHelper()
    {
      HttpClientHelper.EnableHttpPipelining = false;
      HttpClientHelper.ServerThrottledIdentities = new ConcurrentDictionary<string, ConcurrentDictionary<string, DateTime>>();
    }

    public static ushort MaxWebRequestsPerMinute { get; set; }

    public static bool DoNotSetCertificateValidationCallback { get; set; }

    public static bool EnableHttpPipelining { get; set; }

    public static IWebProxy Proxy { get; set; }

    public static AuthenticationHeaderValue GetAuthenticationHeader() => new AuthenticationHeaderValue("Bearer", UserAccessTokenRefresher.Instance.UserAccessToken);

    public static HttpClient CreateHttpClientWithAuthInfo(
      ConnectionInfo connectionInfo,
      string authHeaderValue = null)
    {
      WebRequestHandler webRequestHandler = new WebRequestHandler();
      webRequestHandler.AllowAutoRedirect = false;
      webRequestHandler.AllowPipelining = HttpClientHelper.EnableHttpPipelining;
      webRequestHandler.Proxy = HttpClientHelper.Proxy;
      WebRequestHandler handler = webRequestHandler;
      if (!HttpClientHelper.DoNotSetCertificateValidationCallback)
        handler.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(HttpClientHelper.CertificateValidationCallback);
      HttpClient httpClient;
      if (connectionInfo.UseAadUserAuthentication)
      {
        httpClient = new HttpClient((HttpMessageHandler) handler, true);
        AuthenticationHeaderValue authenticationHeaderValue = authHeaderValue != null ? new AuthenticationHeaderValue("Bearer", authHeaderValue) : HttpClientHelper.GetAuthenticationHeader();
        httpClient.DefaultRequestHeaders.Authorization = authenticationHeaderValue;
      }
      else
      {
        handler.ClientCertificates.Add((X509Certificate) connectionInfo.Certificate);
        httpClient = new HttpClient((HttpMessageHandler) handler, true);
      }
      HttpClientHelper.AddDefaultRequestHeaders(httpClient);
      if (connectionInfo.AdditionalDefaultRequestHeaders != null)
      {
        foreach (KeyValuePair<string, string> defaultRequestHeader in connectionInfo.AdditionalDefaultRequestHeaders)
          httpClient.DefaultRequestHeaders.Add(defaultRequestHeader.Key, defaultRequestHeader.Value);
      }
      httpClient.Timeout = connectionInfo.Timeout;
      Logger.Log(LoggerLevel.Info, HttpClientHelper.LogId, "CreateClient", "Created new HttpClient. CertThumbprintOrSubjectName:{0}, TimeoutMs:{1}", connectionInfo.UseAadUserAuthentication ? (object) "NA since user auth is in use" : (object) (connectionInfo.CertificateThumbprintOrSubjectName ?? connectionInfo.Certificate?.Thumbprint), (object) connectionInfo.Timeout.TotalMilliseconds);
      return httpClient;
    }

    public static HttpClient CreateHttpClient(TimeSpan timeout)
    {
      WebRequestHandler webRequestHandler = new WebRequestHandler();
      webRequestHandler.AllowAutoRedirect = false;
      webRequestHandler.Proxy = HttpClientHelper.Proxy;
      WebRequestHandler handler = webRequestHandler;
      if (!HttpClientHelper.DoNotSetCertificateValidationCallback)
        handler.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(HttpClientHelper.CertificateValidationCallback);
      HttpClient httpClient = new HttpClient((HttpMessageHandler) handler, true);
      HttpClientHelper.AddDefaultRequestHeaders(httpClient);
      httpClient.Timeout = timeout;
      Logger.Log(LoggerLevel.Info, HttpClientHelper.LogId, "CreateClient", "Created new HttpClient. TimeoutMs:{0}", (object) timeout.TotalMilliseconds);
      return httpClient;
    }

    public static bool CertificateValidationCallback(
      object sender,
      X509Certificate certificate,
      X509Chain chain,
      SslPolicyErrors sslPolicyErrors)
    {
      switch (sslPolicyErrors)
      {
        case SslPolicyErrors.None:
          return true;
        case SslPolicyErrors.RemoteCertificateNameMismatch:
          string nameInfo = new X509Certificate2(certificate).GetNameInfo(X509NameType.SimpleName, false);
          if (HttpClientHelper.AllowedServerCertificateSimpleNames.Contains(nameInfo))
            return true;
          break;
      }
      HttpWebRequest httpWebRequest = sender as HttpWebRequest;
      string str = chain == null || chain.ChainStatus.Length == 0 ? string.Empty : string.Join(" | ", ((IEnumerable<X509ChainStatus>) chain.ChainStatus).Select<X509ChainStatus, string>((Func<X509ChainStatus, string>) (c => c.StatusInformation)));
      throw new MetricsClientException(string.Format("CertificateValidationCallback failed. sslPolicyErrors:{0}, requestUri:{1}, ", (object) sslPolicyErrors, (object) httpWebRequest?.RequestUri) + string.Format("chain.ChainStatus.Length:{0}, chainStatusInfo:[{1}], certificate:{2}", (object) chain?.ChainStatus.Length, (object) str, (object) certificate?.ToString(true)));
    }

    public static void AddStandardHeadersToMessage(
      HttpRequestMessage message,
      Guid traceId,
      string sourceIdentity,
      string hostname)
    {
      HttpClientHelper.AddStandardHeadersToMessage(message, traceId.ToString("B"), sourceIdentity, hostname);
    }

    public static async Task<string> GetResponseAsStringAsync(
      Uri url,
      HttpMethod method,
      HttpClient client,
      string monitoringAccount,
      string operation,
      object httpContent = null,
      string clientId = "",
      string serializedContent = null,
      Guid? traceId = null,
      byte serializationVersion = 3,
      int numAttempts = 3)
    {
      Tuple<string, HttpResponseMessage> response = (Tuple<string, HttpResponseMessage>) null;
      string responseAsStringAsync;
      try
      {
        response = await HttpClientHelper.GetResponse(url, method, client, monitoringAccount, operation, httpContent, clientId, serializedContent, traceId, serializationVersion, numAttempts).ConfigureAwait(false);
        responseAsStringAsync = response.Item1;
      }
      finally
      {
        response?.Item2?.Dispose();
      }
      response = (Tuple<string, HttpResponseMessage>) null;
      return responseAsStringAsync;
    }

    public static async Task<string> GetJsonResponse(
      Uri url,
      HttpMethod method,
      HttpClient client,
      string monitoringAccount,
      string operation,
      object httpContent = null,
      string clientId = "",
      string serializedContent = null,
      Guid? traceId = null,
      byte serializationVersion = 3,
      int numAttempts = 3)
    {
      Tuple<string, HttpResponseMessage> tuple = await HttpClientHelper.GetResponse(url, method, client, monitoringAccount, operation, httpContent, clientId, serializedContent, traceId, serializationVersion, numAttempts).ConfigureAwait(false);
      string jsonResponse;
      using (tuple.Item2)
        jsonResponse = tuple.Item1;
      return jsonResponse;
    }

    public static Task<Tuple<string, HttpResponseMessage>> GetResponseWithCustomTraceId(
      Uri url,
      HttpMethod method,
      HttpClient client,
      string monitoringAccount,
      string operation,
      string traceId,
      object httpContent = null,
      string clientId = "",
      string serializedContent = null,
      byte serializationVersion = 3,
      int numAttempts = 3)
    {
      return HttpClientHelper.GetResponseWithCustomTraceId(url, method, client, monitoringAccount, operation, true, false, traceId, httpContent, clientId, serializedContent, serializationVersion, numAttempts);
    }

    public static Task<Tuple<string, HttpResponseMessage>> GetResponse(
      Uri url,
      HttpMethod method,
      HttpClient client,
      string monitoringAccount,
      string operation,
      object httpContent = null,
      string clientId = "",
      string serializedContent = null,
      Guid? traceId = null,
      byte serializationVersion = 3,
      int numAttempts = 3)
    {
      return HttpClientHelper.GetResponse(url, method, client, monitoringAccount, operation, true, false, httpContent, clientId, serializedContent, traceId, serializationVersion, numAttempts);
    }

    public static Task<Tuple<string, HttpResponseMessage>> GetResponse(
      Uri url,
      HttpMethod method,
      HttpClient client,
      string monitoringAccount,
      string operation,
      bool parseResponse,
      bool isDebugQuery,
      object httpContent = null,
      string clientId = "",
      string serializedContent = null,
      Guid? traceId = null,
      byte serializationVersion = 3,
      int numAttempts = 3)
    {
      return HttpClientHelper.GetResponseWithCustomTraceId(url, method, client, monitoringAccount, operation, parseResponse, isDebugQuery, !traceId.HasValue ? string.Empty : traceId.ToString(), httpContent, clientId, serializedContent, serializationVersion, numAttempts);
    }

    private static void AddStandardHeadersToMessage(
      HttpRequestMessage message,
      string traceId,
      string sourceIdentity,
      string hostname)
    {
      message.Headers.Add("TraceGuid", traceId);
      message.Headers.Add("SourceIdentity", sourceIdentity);
      message.Headers.Host = hostname;
    }

    private static async Task<Tuple<string, HttpResponseMessage>> GetResponseWithCustomTraceId(
      Uri url,
      HttpMethod method,
      HttpClient client,
      string monitoringAccount,
      string operation,
      bool parseResponse,
      bool isDebugQuery,
      string traceId,
      object httpContent = null,
      string clientId = "",
      string serializedContent = null,
      byte serializationVersion = 3,
      int numAttempts = 3)
    {
      Exception lastException = (Exception) null;
      if (string.IsNullOrEmpty(traceId))
        traceId = Guid.NewGuid().ToString("B");
      if (HttpClientHelper.IsThrottledByServer(monitoringAccount, operation, method))
        throw new MetricsClientException(string.Format("The request is throttled by the server. Traceid: {0}, Url:{1}, Method:{2}, Operation:{3}.", (object) traceId, (object) url, (object) method, (object) operation), (Exception) null, HttpClientHelper.ParseGuidFromTraceId(traceId), new HttpStatusCode?((HttpStatusCode) 429));
      for (int i = 1; i <= numAttempts; ++i)
      {
        Tuple<string, HttpResponseMessage> withCustomTraceId;
        int num;
        try
        {
          withCustomTraceId = await HttpClientHelper.GetResponseWithTokenRefresh(url, method, client, httpContent, clientId, serializedContent, traceId, serializationVersion, monitoringAccount, parseResponse, isDebugQuery, i).ConfigureAwait(false);
          goto label_21;
        }
        catch (MetricsClientException ex)
        {
          num = 1;
        }
        if (num == 1)
        {
          MetricsClientException metricsClientException = ex;
          lastException = (Exception) metricsClientException;
          if (metricsClientException.ResponseStatusCode.HasValue)
          {
            HttpStatusCode? responseStatusCode = metricsClientException.ResponseStatusCode;
            HttpStatusCode httpStatusCode = HttpStatusCode.ServiceUnavailable;
            if (!(responseStatusCode.GetValueOrDefault() == httpStatusCode & responseStatusCode.HasValue))
              goto label_13;
          }
          if (i != numAttempts)
            goto label_16;
label_13:
          if (!((object) ex is Exception source))
            throw (object) ex;
          ExceptionDispatchInfo.Capture(source).Throw();
label_16:
          TimeSpan delay = TimeSpan.FromSeconds((double) (5 * i));
          Logger.Log(LoggerLevel.Info, HttpClientHelper.LogId, "GetResponse", "TraceId:{0}, Delay {1} and then retry.", (object) traceId, (object) delay);
          await Task.Delay(delay).ConfigureAwait(false);
          continue;
        }
        continue;
label_21:
        lastException = (Exception) null;
        return withCustomTraceId;
      }
      throw new MetricsClientException(string.Format("TraceId:{0}, Exhausted {1} attempts.", (object) traceId, (object) numAttempts), lastException);
    }

    private static void AddDefaultRequestHeaders(HttpClient httpClient)
    {
      httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
      httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/octet-stream"));
      httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("MultiDimensionalMetricsClient");
      httpClient.DefaultRequestHeaders.Add("MultiDimensionalMetricsClientVersion", HttpClientHelper.AssemblyVersion);
    }

    private static async Task<Tuple<string, HttpResponseMessage>> GetResponseWithTokenRefresh(
      Uri url,
      HttpMethod method,
      HttpClient client,
      object httpContent,
      string clientId,
      string serializedContent,
      string traceId,
      byte serializationVersion,
      string monitoringAccount,
      bool parseResponse,
      bool isDebugQuery,
      int attemptNum)
    {
      int num;
      try
      {
        return await HttpClientHelper.GetResponseNoRetry(url, method, client, httpContent, clientId, serializedContent, traceId, serializationVersion, monitoringAccount, parseResponse, isDebugQuery, attemptNum).ConfigureAwait(false);
      }
      catch (MetricsClientException ex)
      {
        num = 1;
      }
      Tuple<string, HttpResponseMessage> withTokenRefresh;
      if (num != 1)
        return withTokenRefresh;
      HttpStatusCode? responseStatusCode = ex.ResponseStatusCode;
      HttpStatusCode httpStatusCode = HttpStatusCode.Found;
      if (responseStatusCode.GetValueOrDefault() == httpStatusCode & responseStatusCode.HasValue)
      {
        await UserAccessTokenRefresher.Instance.RefreshAccessToken().ConfigureAwait(false);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", UserAccessTokenRefresher.Instance.UserAccessToken);
        return await HttpClientHelper.GetResponseNoRetry(url, method, client, httpContent, clientId, serializedContent, traceId, serializationVersion, monitoringAccount, parseResponse, isDebugQuery, attemptNum).ConfigureAwait(false);
      }
      if (!((object) ex is Exception source))
        throw (object) ex;
      ExceptionDispatchInfo.Capture(source).Throw();
      return withTokenRefresh;
    }

    private static async Task<Tuple<string, HttpResponseMessage>> GetResponseNoRetry(
      Uri url,
      HttpMethod method,
      HttpClient client,
      object httpContent,
      string clientId,
      string serializedContent,
      string traceId,
      byte serializationVersion,
      string monitoringAccount,
      bool parseResponse,
      bool isDebugQuery,
      int attemptNum)
    {
      HttpClientHelper.HandleGeneralClientThrottling(url, method, traceId);
      UriBuilder uriBuilder1 = new UriBuilder(url);
      UriBuilder uriBuilder2 = uriBuilder1;
      uriBuilder2.Host = await ConnectionInfo.GetCachedIpAddress(url.Host).ConfigureAwait(false);
      UriBuilder uriBuilder = uriBuilder1;
      uriBuilder2 = (UriBuilder) null;
      uriBuilder1 = (UriBuilder) null;
      string urlToLog = string.Format("{0} ({1})", (object) uriBuilder.Uri, (object) url.Host);
      HttpRequestMessage httpRequestMessage = new HttpRequestMessage(method, uriBuilder.Uri);
      HttpClientHelper.AddStandardHeadersToMessage(httpRequestMessage, traceId, Environment.MachineName, url.Host);
      if (isDebugQuery)
        httpRequestMessage.Headers.Add("__DebugMode__", "true");
      if (!string.IsNullOrWhiteSpace(clientId))
        httpRequestMessage.Headers.Add("ClientId", clientId);
      if (serializationVersion == (byte) 1)
        httpRequestMessage.Headers.Add("__ScalingFactorDisabled__", "true");
      if (httpContent != null && serializedContent == null)
        serializedContent = JsonConvert.SerializeObject(httpContent);
      if (serializedContent != null)
        httpRequestMessage.Content = (HttpContent) new StringContent(serializedContent, Encoding.UTF8, "application/json");
      object logId1 = HttpClientHelper.LogId;
      object[] objArray1 = new object[8]
      {
        (object) traceId,
        (object) urlToLog,
        (object) method,
        (object) Environment.MachineName,
        null,
        null,
        null,
        null
      };
      string str1 = serializedContent;
      objArray1[4] = (object) (str1 != null ? str1.Length : 0);
      objArray1[5] = (object) ServicePointManager.DnsRefreshTimeout;
      objArray1[6] = (object) client.Timeout.TotalMilliseconds;
      objArray1[7] = (object) HttpClientHelper.AssemblyVersion;
      Logger.Log(LoggerLevel.Info, logId1, "GetResponse", "Making HTTP request. TraceId:{0}, Url:{1}, Method:{2}, SourceId:{3}, ContentLength:{4}, DnsTimeoutMs:{5}, TimeoutMs:{6}, SdkVersion:{7}", objArray1);
      string responseString = (string) null;
      Stopwatch requestLatency = Stopwatch.StartNew();
      string stage = "SendRequest";
      string handlingServer = "Unknown";
      HttpResponseMessage response = (HttpResponseMessage) null;
      Tuple<string, HttpResponseMessage> responseNoRetry;
      try
      {
        response = await HttpClientHelper.SendRequestAndTryDiagnosticUrlsOnFailure(client, httpRequestMessage, attemptNum, traceId).ConfigureAwait(false);
        Logger.Log(LoggerLevel.Info, HttpClientHelper.LogId, "GetResponse", "Sent HTTP request, reading response. TraceId:{0}, Url:{1}, SendLatencyMs:{2}", (object) traceId, (object) urlToLog, (object) requestLatency.ElapsedMilliseconds);
        stage = "ReadResponse";
        IEnumerable<string> values;
        response.Headers.TryGetValues("__HandlingServerId__", out values);
        if (values != null)
          handlingServer = values.First<string>();
        requestLatency.Restart();
        if (parseResponse || !response.IsSuccessStatusCode)
          responseString = await HttpClientHelper.ParseResponseContent(response).ConfigureAwait(false);
        object logId2 = HttpClientHelper.LogId;
        object[] objArray2 = new object[7]
        {
          (object) traceId,
          (object) urlToLog,
          (object) handlingServer,
          (object) requestLatency.ElapsedMilliseconds,
          (object) response.StatusCode,
          null,
          null
        };
        string str2 = responseString;
        objArray2[5] = (object) (str2 != null ? str2.Length : 0);
        objArray2[6] = (object) parseResponse;
        Logger.Log(LoggerLevel.Info, logId2, "GetResponse", "Received HTTP response. TraceId:{0}, Url:{1}, HandlingServer:{2}, ReadLatencyMs:{3}, ResponseStatus:{4}, ResponseLength:{5}, ParseResponse:{6}", objArray2);
        stage = "ValidateStatus";
        response.EnsureSuccessStatusCode();
        responseNoRetry = Tuple.Create<string, HttpResponseMessage>(responseString, response);
      }
      catch (MetricsClientException ex)
      {
        throw;
      }
      catch (Exception ex)
      {
        try
        {
          string str3;
          if (!HttpClientHelper.HandleServerSideThrottling(response, url, method, monitoringAccount))
            str3 = "Server side returns an exception, please check the detailed error message as followed. TraceId:" + traceId + ", Url:" + urlToLog + ", HandlingServer:" + handlingServer + " Stage:" + stage + ", " + string.Format("LatencyMs:{0}, AttemptNum:{1}, SDK Version:{2}, ", (object) requestLatency.ElapsedMilliseconds, (object) attemptNum, (object) HttpClientHelper.AssemblyVersion) + "ResponseStatus:" + (response?.StatusCode.ToString() ?? "<none>") + ", Response:" + responseString + ", SerializedContent:" + serializedContent + ".";
          else
            str3 = string.Format("HTTP request throttled by server. Url:{0}, Method:{1}, Response:{2}", (object) urlToLog, (object) method, (object) (responseString ?? "<none>"));
          string str4 = str3;
          Logger.Log(LoggerLevel.Error, HttpClientHelper.LogId, "GetResponse", str4);
          throw new MetricsClientException(str4, ex, HttpClientHelper.ParseGuidFromTraceId(traceId), response?.StatusCode);
        }
        finally
        {
          response?.Dispose();
        }
      }
      finally
      {
        requestLatency.Stop();
      }
      urlToLog = (string) null;
      responseString = (string) null;
      requestLatency = (Stopwatch) null;
      stage = (string) null;
      handlingServer = (string) null;
      response = (HttpResponseMessage) null;
      return responseNoRetry;
    }

    private static async Task<HttpResponseMessage> SendRequestAndTryDiagnosticUrlsOnFailure(
      HttpClient client,
      HttpRequestMessage request,
      int attemptNum,
      string traceId)
    {
      Uri requestUri = request.RequestUri;
      HttpResponseMessage response;
      try
      {
        response = await client.SendAsync(request).ConfigureAwait(false);
      }
      catch (Exception ex)
      {
        string loadBalancerProbeHttpsUrl = "https://" + requestUri.Host + "/public/lb-probe";
        bool? httpConnectable = new bool?();
        ConfiguredTaskAwaitable<bool> configuredTaskAwaitable = TestConnectivity(loadBalancerProbeHttpsUrl).ConfigureAwait(false);
        bool httpsConnectable = await configuredTaskAwaitable;
        if (!httpsConnectable)
        {
          configuredTaskAwaitable = TestConnectivity(loadBalancerProbeHttpsUrl.Replace("https:", "http:")).ConfigureAwait(false);
          httpConnectable = new bool?(await configuredTaskAwaitable);
        }
        throw new MetricsClientException(string.Format("Failed to send request. TraceId: {0}, URL:{1}, Hostname:{2}, Method:{3}, SDK Version:{4} ", (object) traceId, (object) requestUri, (object) request.Headers.Host, (object) request.Method, (object) HttpClientHelper.AssemblyVersion) + string.Format("loadBalancerProbeHttpUrl used for testing connectivity:{0}, httpsConnectable:{1}, httpConnectable:{2}, ", (object) loadBalancerProbeHttpsUrl, (object) httpsConnectable, (object) httpConnectable) + string.Format("AttemptNum:{0}, [{1}]", (object) attemptNum, (object) ServicePointManager.SecurityProtocol), ex);
      }
      HttpResponseMessage httpResponseMessage = response;
      response = (HttpResponseMessage) null;
      requestUri = (Uri) null;
      return httpResponseMessage;

      async Task<bool> TestConnectivity(string url)
      {
        bool succeeded = false;
        try
        {
          HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, url);
          HttpClientHelper.AddStandardHeadersToMessage(httpRequestMessage, Guid.Empty, Environment.MachineName, request.Headers.Host);
          using (await client.SendAsync(httpRequestMessage).ConfigureAwait(false))
            succeeded = true;
        }
        catch (Exception ex)
        {
        }
        return succeeded;
      }
    }

    private static async Task<string> ParseResponseContent(HttpResponseMessage response)
    {
      string responseContent;
      if (response.Content.Headers.ContentType?.MediaType != null && response.Content.Headers.ContentType.MediaType.Equals("application/octet-stream", StringComparison.OrdinalIgnoreCase))
        responseContent = "application/octet-stream";
      else
        responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
      return responseContent;
    }

    private static bool HandleServerSideThrottling(
      HttpResponseMessage response,
      Uri url,
      HttpMethod method,
      string monitoringAccount)
    {
      if (response != null && response.StatusCode == (HttpStatusCode) 429 && response.Headers != null)
      {
        if (!response.Headers.Contains("Throttled-Identity") || !response.Headers.Contains("Retry-After"))
          return false;
        string throttledIdentity = response.Headers.GetValues("Throttled-Identity").FirstOrDefault<string>();
        if (string.IsNullOrEmpty(throttledIdentity))
          return false;
        string s = response.Headers.GetValues("Retry-After").FirstOrDefault<string>();
        if (string.IsNullOrEmpty(s))
          return false;
        int retyAfterSeconds;
        if (int.TryParse(s, out retyAfterSeconds))
        {
          DateTime now = DateTime.UtcNow;
          HttpClientHelper.ServerThrottledIdentities.AddOrUpdate(monitoringAccount, (Func<string, ConcurrentDictionary<string, DateTime>>) (key =>
          {
            ConcurrentDictionary<string, DateTime> concurrentDictionary = new ConcurrentDictionary<string, DateTime>();
            concurrentDictionary.TryAdd(throttledIdentity, now.AddSeconds((double) retyAfterSeconds));
            return concurrentDictionary;
          }), (Func<string, ConcurrentDictionary<string, DateTime>, ConcurrentDictionary<string, DateTime>>) ((key, value) =>
          {
            value.AddOrUpdate(throttledIdentity, (Func<string, DateTime>) (keyInner => now.AddSeconds((double) retyAfterSeconds)), (Func<string, DateTime, DateTime>) ((keyInner, valueInner) => now.AddSeconds((double) retyAfterSeconds)));
            return value;
          }));
          return true;
        }
        Logger.Log(LoggerLevel.Debug, HttpClientHelper.LogId, nameof (HandleServerSideThrottling), "HTTP request throttled by server, but we could not parse retry-after header Url:{0}, Method:{1}, Retry-After {2}", (object) url, (object) method);
      }
      return false;
    }

    private static bool IsThrottledByServer(
      string monitoringAccount,
      string operation,
      HttpMethod httpMethod)
    {
      ConcurrentDictionary<string, DateTime> concurrentDictionary;
      if (string.IsNullOrEmpty(monitoringAccount) || !HttpClientHelper.ServerThrottledIdentities.TryGetValue(monitoringAccount, out concurrentDictionary))
        return false;
      DateTime dateTime1;
      concurrentDictionary.TryGetValue(operation, out dateTime1);
      DateTime dateTime2;
      concurrentDictionary.TryGetValue(httpMethod.ToString(), out dateTime2);
      if (!(new DateTime(Math.Max(dateTime1.Ticks, dateTime2.Ticks)) <= DateTime.UtcNow))
        return true;
      HttpClientHelper.ServerThrottledIdentities.TryRemove(monitoringAccount, out ConcurrentDictionary<string, DateTime> _);
      return false;
    }

    private static void HandleGeneralClientThrottling(Uri url, HttpMethod method, string traceId)
    {
      int totalMinutes = (int) (DateTime.UtcNow - HttpClientHelper.StartupTime).TotalMinutes;
      if (totalMinutes == HttpClientHelper.currentMinute)
      {
        Interlocked.Increment(ref HttpClientHelper.requestsSentInCurrentMinute);
      }
      else
      {
        HttpClientHelper.currentMinute = totalMinutes;
        Interlocked.Exchange(ref HttpClientHelper.requestsSentInCurrentMinute, 0);
      }
      int num = Math.Max(1000, (int) HttpClientHelper.MaxWebRequestsPerMinute);
      if (HttpClientHelper.requestsSentInCurrentMinute > num)
      {
        Logger.Log(LoggerLevel.Debug, HttpClientHelper.LogId, nameof (HandleGeneralClientThrottling), "HTTP request throttled. Url:{0}, Method:{1}, CurrentRequestsInMinute:{2}, AllowedRequestsInMinute:{3}, TraceId:{4}", (object) url, (object) method, (object) HttpClientHelper.requestsSentInCurrentMinute, (object) num, (object) traceId);
        throw new MetricsClientException(string.Format("TraceId:{0}. Client size throttling: no more than [{1}] requests can be issued in a minute. Actual requests:[{2}].", (object) traceId, (object) num, (object) HttpClientHelper.requestsSentInCurrentMinute), (Exception) null, HttpClientHelper.ParseGuidFromTraceId(traceId), new HttpStatusCode?((HttpStatusCode) 429));
      }
    }

    private static Guid ParseGuidFromTraceId(string traceId)
    {
      if (string.IsNullOrEmpty(traceId))
        return Guid.Empty;
      Guid result;
      if (!Guid.TryParse(traceId.Split(new string[1]{ ";" }, StringSplitOptions.RemoveEmptyEntries)[0], out result))
        result = Guid.Empty;
      return result;
    }
  }
}
