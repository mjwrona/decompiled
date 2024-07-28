// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.ActiveDirectory.GraphClient.ConnectionWrapper
// Assembly: Microsoft.Azure.ActiveDirectory.GraphClient.FirstParty, Version=1.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 7AAC33C3-FEBB-470D-AB81-20ABE7F3618F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.ActiveDirectory.GraphClient.FirstParty.dll

using Microsoft.Azure.ActiveDirectory.GraphClient.ErrorHandling;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Threading;

namespace Microsoft.Azure.ActiveDirectory.GraphClient
{
  [ExcludeFromCodeCoverage]
  public class ConnectionWrapper
  {
    private readonly GraphSettings graphSettings;

    public ConnectionWrapper()
      : this(new GraphSettings())
    {
    }

    public ConnectionWrapper(GraphSettings graphSettings) => this.graphSettings = graphSettings;

    internal Guid ClientRequestId { get; set; }

    internal string AccessToken { get; set; }

    internal ICredentials GraphCredentials { get; set; }

    internal string GraphApiDomainName => this.graphSettings.GraphDomainName;

    internal string GraphApiVersion => this.graphSettings.ApiVersion;

    internal bool IsRetryEnabled => this.graphSettings.IsRetryEnabled;

    internal HashSet<string> RetryOnExceptions => this.graphSettings.RetryOnExceptions;

    internal TimeSpan WaitBeforeRetry => this.graphSettings.WaitBeforeRetry;

    internal int TotalAttempts => this.graphSettings.TotalAttempts;

    public virtual void AttachRequiredHeaders(
      HttpWebRequest httpWebRequest,
      bool includeContentType,
      WebHeaderCollection additionalHeaders)
    {
      bool addAuthorizationHeader = this.GraphCredentials == null;
      if (!addAuthorizationHeader)
        httpWebRequest.Credentials = this.GraphCredentials;
      this.AttachRequiredHeaders(httpWebRequest.Headers, includeContentType, additionalHeaders, addAuthorizationHeader);
    }

    public virtual void AttachRequiredHeaders(
      WebClient webClient,
      bool includeContentType,
      WebHeaderCollection additionalHeaders)
    {
      bool addAuthorizationHeader = this.GraphCredentials == null;
      if (!addAuthorizationHeader)
        webClient.Credentials = this.GraphCredentials;
      this.AttachRequiredHeaders(webClient.Headers, includeContentType, additionalHeaders, addAuthorizationHeader);
    }

    protected virtual void AttachRequiredHeaders(
      WebHeaderCollection headers,
      bool includeContentType,
      WebHeaderCollection additionalHeaders,
      bool addAuthorizationHeader)
    {
      if (addAuthorizationHeader)
        headers[HttpRequestHeader.Authorization] = this.AccessToken;
      headers["client-request-id"] = this.ClientRequestId.ToString();
      headers["Prefer"] = "return-content";
      if (includeContentType)
        headers[HttpRequestHeader.ContentType] = "application/json;odata=minimalmetadata";
      try
      {
        headers[HttpRequestHeader.UserAgent] = "Microsoft Azure Graph Client Library 1.0";
        if (additionalHeaders == null)
          return;
        foreach (string allKey in additionalHeaders.AllKeys)
          headers[allKey] = additionalHeaders[allKey];
      }
      catch (ArgumentException ex)
      {
      }
    }

    public virtual T InvokeNetworkOperation<T>(Func<T> action) where T : class
    {
      int num = 1;
      while (true)
      {
        try
        {
          return action();
        }
        catch (WebException ex)
        {
          GraphException webException = ErrorResolver.ParseWebException(ex);
          if (!this.graphSettings.IsRetryEnabled || this.graphSettings.TotalAttempts <= 0 || num == this.graphSettings.TotalAttempts || num == 10 || !this.graphSettings.RetryOnExceptions.Contains(webException.GetType().FullName))
            throw webException;
          ++num;
          Thread.Sleep(this.graphSettings.WaitBeforeRetry);
        }
      }
    }

    public virtual byte[] DownloadData(string address, WebHeaderCollection additionalHeaders) => this.InvokeNetworkOperation<byte[]>((Func<byte[]>) (() =>
    {
      using (WebClient webClient = new WebClient())
      {
        this.AttachRequiredHeaders(webClient, true, additionalHeaders);
        byte[] numArray = webClient.DownloadData(address);
        Utils.LogResponseHeaders(webClient.ResponseHeaders);
        return numArray;
      }
    }));

    public virtual byte[] DownloadData(Uri address, WebHeaderCollection additionalHeaders) => this.DownloadData(address.ToString(), additionalHeaders);

    public virtual string UploadString(
      string requestUri,
      string method,
      string requestBody,
      WebHeaderCollection additionalHeaders,
      WebHeaderCollection responseHeaders)
    {
      return this.InvokeNetworkOperation<string>((Func<string>) (() =>
      {
        using (WebClient webClient = new WebClient())
        {
          this.AttachRequiredHeaders(webClient, true, additionalHeaders);
          string str = webClient.UploadString(requestUri, method, requestBody);
          responseHeaders?.Add((NameValueCollection) webClient.ResponseHeaders);
          Utils.LogResponseHeaders(webClient.ResponseHeaders);
          return str;
        }
      }));
    }

    public virtual string UploadString(
      Uri requestUri,
      string method,
      string requestBody,
      WebHeaderCollection additionalHeaders,
      WebHeaderCollection responseHeaders)
    {
      return this.UploadString(requestUri.ToString(), method, requestBody, additionalHeaders, responseHeaders);
    }

    public virtual byte[] UploadData(
      Uri requestUri,
      string method,
      byte[] data,
      WebHeaderCollection additionalHeaders)
    {
      return this.InvokeNetworkOperation<byte[]>((Func<byte[]>) (() =>
      {
        using (WebClient webClient = new WebClient())
        {
          this.AttachRequiredHeaders(webClient, true, additionalHeaders);
          byte[] numArray = webClient.UploadData(requestUri, method, data);
          Utils.LogResponseHeaders(webClient.ResponseHeaders);
          return numArray;
        }
      }));
    }

    public virtual void DeleteRequest(Uri requestUri)
    {
      HttpWebRequest webRequest = this.CreateWebRequest(requestUri);
      webRequest.Method = "DELETE";
      this.InvokeNetworkOperation<WebResponse>(new Func<WebResponse>(((WebRequest) webRequest).GetResponse))?.Close();
    }

    public virtual HttpWebRequest CreateWebRequest(Uri requestUri)
    {
      HttpWebRequest httpWebRequest = WebRequest.Create(requestUri) as HttpWebRequest;
      httpWebRequest.UserAgent = "Microsoft Azure Graph Client Library 1.0";
      this.AttachRequiredHeaders(httpWebRequest, false, (WebHeaderCollection) null);
      return httpWebRequest;
    }

    public virtual string GetContentTypeOfLastResponse(WebHeaderCollection responseHeaders) => responseHeaders != null ? responseHeaders[HttpResponseHeader.ContentType] : string.Empty;
  }
}
