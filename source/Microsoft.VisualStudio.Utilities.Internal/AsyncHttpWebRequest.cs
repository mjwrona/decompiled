// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Utilities.Internal.AsyncHttpWebRequest
// Assembly: Microsoft.VisualStudio.Utilities.Internal, Version=14.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E02F1555-E063-4795-BC05-853CA7424F51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Utilities.Internal.dll

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Cache;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Utilities.Internal
{
  public class AsyncHttpWebRequest : IAsyncHttpWebRequest
  {
    private HttpWebRequest request;

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

    public int Timeout
    {
      get => this.request.Timeout;
      set => this.request.Timeout = value;
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

    public AsyncHttpWebRequest(string url)
    {
      this.Url = url;
      this.request = WebRequest.Create(url) as HttpWebRequest;
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

    public async Task<IStreamedHttpWebResponse> GetResponseAsync()
    {
      AsyncHttpWebRequest asyncHttpWebRequest = this;
      try
      {
        // ISSUE: reference to a compiler-generated method
        Task<WebResponse> requestTask = Task.Run<WebResponse>(new Func<WebResponse>(asyncHttpWebRequest.\u003CGetResponseAsync\u003Eb__18_0));
        // ISSUE: explicit non-virtual call
        if (await Task.WhenAny((Task) requestTask, Task.Delay(__nonvirtual (asyncHttpWebRequest.Timeout))).ConfigureAwait(false) == requestTask)
        {
          HttpWebResponse result = (HttpWebResponse) requestTask.Result;
          if (result == null)
            return (IStreamedHttpWebResponse) new StreamedHttpWebResponse()
            {
              ErrorCode = ErrorCode.NullResponse
            };
          return (IStreamedHttpWebResponse) new StreamedHttpWebResponse()
          {
            ErrorCode = ErrorCode.NoError,
            Response = result
          };
        }
        asyncHttpWebRequest.request.Abort();
        requestTask.SwallowException();
        return (IStreamedHttpWebResponse) new StreamedHttpWebResponse()
        {
          ErrorCode = ErrorCode.RequestTimedOut
        };
      }
      catch (WebException ex)
      {
        HttpStatusCode httpStatusCode = HttpStatusCode.Unused;
        if (ex.Status == WebExceptionStatus.ProtocolError)
          httpStatusCode = (ex.Response as HttpWebResponse).StatusCode;
        return (IStreamedHttpWebResponse) new StreamedHttpWebResponse()
        {
          ErrorCode = ErrorCode.WebExceptionThrown,
          ExceptionCode = ex.Status,
          StatusCode = httpStatusCode
        };
      }
    }
  }
}
