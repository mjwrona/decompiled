// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.Services.HttpWebRequest
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.Utilities.Internal;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net;
using System.Net.Cache;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Telemetry.Services
{
  [ExcludeFromCodeCoverage]
  internal class HttpWebRequest : IHttpWebRequest
  {
    private System.Net.HttpWebRequest request;

    public string Url { get; }

    public string Method
    {
      get => this.request.Method;
      set
      {
        value.RequiresArgumentNotNullAndNotEmpty(nameof (value));
        this.request.Method = value;
      }
    }

    public RequestCachePolicy CachePolicy
    {
      get => this.request.CachePolicy;
      set => this.request.CachePolicy = value;
    }

    public string ContentType
    {
      get => this.request.ContentType;
      set
      {
        value.RequiresArgumentNotNullAndNotEmpty(nameof (value));
        this.request.ContentType = value;
      }
    }

    public long ContentLength
    {
      get => this.request.ContentLength;
      set => this.request.ContentLength = value;
    }

    public bool AllowAutoRedirect
    {
      get => this.request.AllowAutoRedirect;
      set => this.request.AllowAutoRedirect = value;
    }

    public HttpWebRequest(string url)
    {
      this.Url = url;
      this.request = (System.Net.HttpWebRequest) WebRequest.Create(url);
    }

    public void AddHeaders(IEnumerable<KeyValuePair<string, string>> headers)
    {
      headers.RequiresArgumentNotNull<IEnumerable<KeyValuePair<string, string>>>(nameof (headers));
      foreach (KeyValuePair<string, string> header in headers)
      {
        if (!string.IsNullOrEmpty(header.Key) && !string.IsNullOrEmpty(header.Value))
          this.request.Headers.Add(header.Key, header.Value);
      }
    }

    public async Task<IHttpWebResponse> GetResponseAsync(CancellationToken token)
    {
      HttpWebRequest httpWebRequest = this;
      try
      {
        // ISSUE: reference to a compiler-generated method
        using (token.Register(new Action(httpWebRequest.\u003CGetResponseAsync\u003Eb__21_0), false))
        {
          Task<WebResponse> requestTask = httpWebRequest.request.GetResponseAsync();
          System.Net.HttpWebResponse httpWebResponse = (System.Net.HttpWebResponse) await requestTask.ConfigureAwait(false);
          if (token.IsCancellationRequested)
          {
            AggregateException exception;
            requestTask.ContinueWith((Action<Task<WebResponse>>) (task => exception = task.Exception), CancellationToken.None, TaskContinuationOptions.None, TaskScheduler.Default);
            return (IHttpWebResponse) new HttpWebResponse()
            {
              ErrorCode = ErrorCode.RequestTimedOut
            };
          }
          if (httpWebResponse == null)
            return (IHttpWebResponse) new HttpWebResponse()
            {
              ErrorCode = ErrorCode.NullResponse
            };
          return (IHttpWebResponse) new HttpWebResponse()
          {
            ErrorCode = ErrorCode.NoError,
            Response = httpWebResponse,
            Headers = httpWebResponse.Headers
          };
        }
      }
      catch (Exception ex)
      {
        WebException webException = !(ex is AggregateException) ? ex as WebException : ex.InnerException as WebException;
        if (webException == null)
        {
          throw;
        }
        else
        {
          HttpStatusCode httpStatusCode = HttpStatusCode.Unused;
          if (webException.Status == WebExceptionStatus.ProtocolError)
            httpStatusCode = (webException.Response as System.Net.HttpWebResponse).StatusCode;
          return (IHttpWebResponse) new HttpWebResponse()
          {
            ErrorCode = ErrorCode.WebExceptionThrown,
            ExceptionCode = webException.Status,
            StatusCode = httpStatusCode
          };
        }
      }
    }

    public async Task<Stream> GetRequestStreamAsync(CancellationToken cancellationToken)
    {
      Stream requestStreamAsync;
      try
      {
        requestStreamAsync = await this.request.GetRequestStreamAsync().WithCancellation<Stream>(cancellationToken).ConfigureAwait(false);
      }
      catch (Exception ex)
      {
        this.request.Abort();
        throw;
      }
      return requestStreamAsync;
    }
  }
}
