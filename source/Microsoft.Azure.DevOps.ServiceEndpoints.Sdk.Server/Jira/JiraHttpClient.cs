// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.Jira.JiraHttpClient
// Assembly: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 002C83BC-B53E-470A-8038-76E47B5E5BF3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ServiceEndpoints.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.Jira
{
  public sealed class JiraHttpClient
  {
    private readonly IExternalProviderHttpRequesterFactory m_httpRequesterFactory;
    private readonly int m_maxPaginatedResults;
    private readonly TimeSpan m_timeout;
    private static string s_userAgentValue;

    internal JiraHttpClient(
      IExternalProviderHttpRequesterFactory httpRequesterFactory,
      TimeSpan timeout,
      int maxPaginatedResults = 2000)
    {
      this.m_httpRequesterFactory = httpRequesterFactory;
      this.m_maxPaginatedResults = maxPaginatedResults;
      this.m_timeout = timeout;
    }

    public JiraResult<JiraData.V2.IssuesResponseData> SearchIssues(
      JiraAuthentication authentication,
      IEnumerable<string> issueKeys)
    {
      JiraData.V2.IssuesRequestData issuesRequestData = new JiraData.V2.IssuesRequestData();
      issuesRequestData.Jql = string.Format("issueKey in ({0})", (object) this.GetCommaSeparatedIssueKeys(issueKeys));
      issuesRequestData.ValidateQuery = JiraConstants.JiraValidateQuery.Warn;
      Uri url = new Uri(authentication.JiraBaseUrl, JiraConstants.Url.jiraSearchIssuesApiRelativeUrl);
      string jsonIn = JsonConvert.SerializeObject((object) issuesRequestData);
      return this.CreateItem<JiraData.V2.IssuesResponseData>(authentication, url, jsonIn);
    }

    public JiraResult<JiraData.V0_1.DeploymentResponseData> LinkDeploymentToIssues(
      JiraAuthentication authentication,
      JiraData.V0_1.DeploymentRequestData payload)
    {
      Uri url = new Uri(authentication.JiraBaseUrl, JiraConstants.Url.jiraDeploymentApiRelativeUrl);
      string jsonIn = JsonConvert.SerializeObject((object) payload);
      return this.CreateItem<JiraData.V0_1.DeploymentResponseData>(authentication, url, jsonIn);
    }

    public HttpStatusCode IsAuthDataValid(JiraAuthentication authentication)
    {
      HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, new Uri(authentication.JiraBaseUrl, JiraConstants.Url.jiraAuthValidationRelativeUrl));
      JiraHttpClient.AddDefaultRequestHeaders(request, authentication);
      return this.SendRequest<string>(request).StatusCode;
    }

    private JiraResult<T> CreateItem<T>(JiraAuthentication authentication, Uri url, string jsonIn)
    {
      if (!string.IsNullOrEmpty(jsonIn) && jsonIn.Length > BearerTokenArgument.MaxBodySizeSupported)
        throw new InvalidOperationException(ServiceEndpointSdkResources.BodySizeLimitExceeded((object) BearerTokenArgument.MaxBodySizeSupported));
      HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, url);
      JiraHttpClient.AddDefaultRequestHeaders(request, authentication);
      request.Content = (HttpContent) new StringContent(jsonIn, Encoding.UTF8, "application/json");
      return this.SendRequest<T>(request);
    }

    private JiraResult<T> SendRequest<T>(
      HttpRequestMessage request,
      Func<string, string> getErrorFromResponse = null)
    {
      return this.SendRequest(request, getErrorFromResponse).Convert<T>((Func<string, T>) (x => !string.IsNullOrWhiteSpace(x) ? JsonConvert.DeserializeObject<T>(x) : default (T)));
    }

    private JiraResult<string> SendRequest(
      HttpRequestMessage request,
      Func<string, string> getErrorFromResponse = null)
    {
      getErrorFromResponse = getErrorFromResponse ?? new Func<string, string>(this.GetErrorDetailsForRequest);
      try
      {
        HttpResponseMessage httpResponseMessage = this.SendHttpRequest(request);
        string result = httpResponseMessage.Content.ReadAsStringAsync().SyncResult<string>();
        return !httpResponseMessage.IsSuccessStatusCode ? JiraResult<string>.Error((string.IsNullOrEmpty(result) ? (string) null : getErrorFromResponse(result)) ?? httpResponseMessage.ReasonPhrase ?? "Unknown error", httpResponseMessage.StatusCode) : JiraResult<string>.Success(result, httpResponseMessage.StatusCode);
      }
      catch (Exception ex) when (ex is TimeoutException || ex is TaskCanceledException)
      {
        return JiraResult<string>.Error(string.Format("Request timed out after {0} ms", (object) this.m_timeout.TotalMilliseconds), HttpStatusCode.RequestTimeout);
      }
    }

    private HttpResponseMessage SendHttpRequest(HttpRequestMessage request)
    {
      using (VssHttpMessageHandler httpMessageHandler = new VssHttpMessageHandler(new VssCredentials(), new VssHttpRequestSettings()
      {
        SendTimeout = this.m_timeout
      }))
      {
        using (IExternalProviderHttpRequester requester = this.m_httpRequesterFactory.GetRequester((HttpMessageHandler) httpMessageHandler))
        {
          HttpResponseMessage response;
          requester.SendRequest(request, HttpCompletionOption.ResponseContentRead, out response, out HttpStatusCode _, out string _);
          return response;
        }
      }
    }

    private string GetErrorDetailsForRequest(string responseContent)
    {
      ArgumentUtility.CheckForNull<string>(responseContent, nameof (responseContent));
      try
      {
        return string.Join(",", JsonConvert.DeserializeObject<JiraData.V2.ErrorResponse>(responseContent).ErrorMessages);
      }
      catch (Exception ex)
      {
        return (string) null;
      }
    }

    private static void AddDefaultRequestHeaders(
      HttpRequestMessage request,
      JiraAuthentication authentication)
    {
      JiraHttpClient.ValidateAuthentication(authentication);
      request.Headers.Add("Authorization", "JWT " + JiraJsonWebTokenHelper.GenerateJsonWebToken(request, authentication));
      request.Headers.Add("User-Agent", JiraHttpClient.GetVsoUserAgentValue());
    }

    private static void ValidateAuthentication(JiraAuthentication authentication)
    {
      string format = "Authentication detail missing: {0}";
      if (authentication == null)
        throw new ApplicationException("Authentication must not be null");
      if (string.IsNullOrEmpty(authentication.JiraAppKey))
        throw new ApplicationException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, format, (object) "JiraAppKey"));
      if (string.IsNullOrEmpty(authentication.SharedSecret))
        throw new ApplicationException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, format, (object) "SharedSecret"));
      if (string.IsNullOrEmpty(authentication.ClientKey))
        throw new ApplicationException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, format, (object) "ClientKey"));
      if (authentication.JiraBaseUrl == (Uri) null)
        throw new ApplicationException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, format, (object) "JiraBaseUrl"));
    }

    private static string GetVsoUserAgentValue()
    {
      if (JiraHttpClient.s_userAgentValue != null)
        return JiraHttpClient.s_userAgentValue;
      string str;
      try
      {
        str = typeof (JiraHttpClient).Assembly.GetCustomAttributes(false).OfType<AssemblyFileVersionAttribute>().FirstOrDefault<AssemblyFileVersionAttribute>()?.Version ?? "unavailable";
      }
      catch (Exception ex)
      {
        str = "unavailable";
      }
      JiraHttpClient.s_userAgentValue = "VSOnline/" + str;
      return JiraHttpClient.s_userAgentValue;
    }

    private string GetCommaSeparatedIssueKeys(IEnumerable<string> issueKeys) => string.Join(",", issueKeys);
  }
}
