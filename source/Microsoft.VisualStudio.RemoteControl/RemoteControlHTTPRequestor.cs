// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.RemoteControl.RemoteControlHTTPRequestor
// Assembly: Microsoft.VisualStudio.RemoteControl, Version=14.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4D9D0761-3208-49DD-A9E2-BF705DBE6B5D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.RemoteControl.dll

using Microsoft.VisualStudio.Utilities.Internal;
using System;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Cache;
using System.Runtime.ExceptionServices;
using System.Security;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.RemoteControl
{
  internal sealed class RemoteControlHTTPRequestor : IRemoteControlHTTPRequestor
  {
    private const string ErrorMarkerFileExtension = ".errormarker";
    private const string AgeHeaderName = "Age";
    private const string CancelledMessage = "Download was cancelled by caller";
    private const string WebExceptionMessage = "Reading HTTP response stream throws an WebException";
    private const int MinRetryIntervalSeconds = 2;
    private const int MaxRetryIntervalSeconds = 32;
    private const int ConvertMilliToSeconds = 1000;
    private static readonly HttpRequestCachePolicy CacheOnlyPolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.CacheOnly);
    private static readonly HttpRequestCachePolicy ServerRevalidatePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.Revalidate);
    private readonly string url;
    private readonly string errorMarkerFileUrl;
    private readonly int httpRequestTimeoutMillis;
    private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

    internal RemoteControlHTTPRequestor(string url, int httpRequestTimeoutMillis)
      : this()
    {
      url = url.TrimEnd('/');
      this.url = !url.EndsWith(".errormarker", StringComparison.OrdinalIgnoreCase) ? url : throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "url '{0}' is not allowed. url argument must not end with {1}", new object[2]
      {
        (object) url,
        (object) ".errormarker"
      }));
      this.errorMarkerFileUrl = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}{1}", new object[2]
      {
        (object) url,
        (object) ".errormarker"
      });
      this.httpRequestTimeoutMillis = httpRequestTimeoutMillis;
    }

    private RemoteControlHTTPRequestor()
    {
    }

    async Task<GetFileResult> IRemoteControlHTTPRequestor.GetFileFromServerAsync()
    {
      GetFileResult result = new GetFileResult()
      {
        Code = HttpStatusCode.Unused
      };
      try
      {
        for (int i = 0; i < 2; ++i)
        {
          if (i > 0)
          {
            int num = Math.Min(32, (int) Math.Pow(2.0, (double) i));
            try
            {
              await Task.Delay(num * 1000, this.cancellationTokenSource.Token).ConfigureAwait(false);
            }
            catch (OperationCanceledException ex)
            {
              break;
            }
            catch (ObjectDisposedException ex)
            {
              break;
            }
            result.Dispose();
          }
          result = await this.GetFile(this.url, this.httpRequestTimeoutMillis, RemoteControlHTTPRequestor.ServerRevalidatePolicy).ConfigureAwait(false);
          if (result.IsSuccessStatusCode)
            break;
        }
      }
      finally
      {
        int num;
        if (num < 0 && !result.IsSuccessStatusCode && Platform.IsWindows)
          WinINetHelper.WriteErrorResponseToCache(this.errorMarkerFileUrl, result.Code);
      }
      GetFileResult fileFromServerAsync = result;
      result = (GetFileResult) null;
      return fileFromServerAsync;
    }

    async Task<GetFileResult> IRemoteControlHTTPRequestor.GetFileFromCacheAsync() => await this.GetFile(this.url, this.httpRequestTimeoutMillis, RemoteControlHTTPRequestor.CacheOnlyPolicy).ConfigureAwait(false);

    async Task<int> IRemoteControlHTTPRequestor.LastServerRequestErrorSecondsAgoAsync()
    {
      int num;
      using (GetFileResult getFileResult = await this.GetFile(this.errorMarkerFileUrl, this.httpRequestTimeoutMillis, RemoteControlHTTPRequestor.CacheOnlyPolicy).ConfigureAwait(false))
        num = !getFileResult.IsFromCache ? int.MaxValue : getFileResult.AgeSeconds.Value;
      return num;
    }

    void IRemoteControlHTTPRequestor.Cancel() => this.cancellationTokenSource.Cancel();

    private static int? ExtractAgeHeaderValue(HttpWebResponse resp)
    {
      string[] values = resp.Headers.GetValues("Age");
      if (values != null && values.Length != 0 && !string.IsNullOrEmpty(values[0]))
      {
        int result = 0;
        if (int.TryParse(values[0], out result))
          return new int?(result);
      }
      return new int?();
    }

    [HandleProcessCorruptedStateExceptions]
    [SecurityCritical]
    private static HttpWebRequest CreateHttpRequest(string requestUrl)
    {
      try
      {
        return (HttpWebRequest) WebRequest.Create(requestUrl);
      }
      catch (ConfigurationErrorsException ex)
      {
        return (HttpWebRequest) null;
      }
    }

    private async Task<GetFileResult> GetFile(
      string requestUrl,
      int requestTimeoutMillis,
      HttpRequestCachePolicy cachePolicy)
    {
      HttpWebRequest request = RemoteControlHTTPRequestor.CreateHttpRequest(requestUrl);
      if (request == null)
        return new GetFileResult()
        {
          ErrorMessage = "Create HTTP Request Error",
          Code = HttpStatusCode.Unused
        };
      request.Timeout = requestTimeoutMillis;
      request.CachePolicy = (RequestCachePolicy) (cachePolicy ?? new HttpRequestCachePolicy(HttpCacheAgeControl.MaxStale, TimeSpan.MaxValue));
      request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
      resp = (HttpWebResponse) null;
      WebException exception = (WebException) null;
      bool shouldAbort;
      try
      {
        Task<WebResponse> requestTask = request.GetResponseAsync();
        shouldAbort = false;
        try
        {
          if (await Task.WhenAny((Task) requestTask, Task.Delay(requestTimeoutMillis, this.cancellationTokenSource.Token)).ConfigureAwait(false) != requestTask)
            shouldAbort = true;
        }
        catch (Exception ex)
        {
          shouldAbort = true;
        }
        if (shouldAbort)
        {
          request.Abort();
          requestTask.SwallowException();
          return new GetFileResult()
          {
            ErrorMessage = "Request timed out",
            Code = HttpStatusCode.Unused
          };
        }
        resp = (HttpWebResponse) await requestTask.ConfigureAwait(false);
        requestTask = (Task<WebResponse>) null;
      }
      catch (WebException ex)
      {
        exception = ex;
      }
      catch (Exception ex)
      {
        return new GetFileResult()
        {
          ErrorMessage = "Unknown Exception: " + ex.Message,
          Code = HttpStatusCode.Unused
        };
      }
      string errorMessage = (string) null;
      if (exception != null)
      {
        if (exception.Status == WebExceptionStatus.ProtocolError)
        {
          if (!(exception.Response is HttpWebResponse resp))
          {
            errorMessage = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Protocol Error - null response");
            return new GetFileResult()
            {
              ErrorMessage = errorMessage,
              Code = HttpStatusCode.Unused
            };
          }
          errorMessage = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Protocol Error {0}", new object[1]
          {
            (object) System.Enum.GetName(typeof (HttpStatusCode), (object) resp.StatusCode)
          });
        }
        else
        {
          errorMessage = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Non-Protocol Error {0}", new object[1]
          {
            (object) System.Enum.GetName(typeof (WebExceptionStatus), (object) exception.Status)
          });
          return new GetFileResult()
          {
            ErrorMessage = errorMessage,
            Code = HttpStatusCode.Unused
          };
        }
      }
      int? ageInSeconds = resp != null ? RemoteControlHTTPRequestor.ExtractAgeHeaderValue(resp) : throw new InvalidOperationException("WebException is protocol, but response is null", (Exception) exception);
      Stream stream = (Stream) null;
      shouldAbort = false;
      try
      {
        if (!resp.IsFromCache)
        {
          shouldAbort = true;
          switch (resp.StatusCode)
          {
            case HttpStatusCode.OK:
              stream = await this.WrapInSeekableStreamAsync(resp.GetResponseStream()).ConfigureAwait(false);
              break;
            case HttpStatusCode.NotFound:
              if (Platform.IsWindows)
              {
                WinINetHelper.WriteErrorResponseToCache(requestUrl, HttpStatusCode.NotFound);
                break;
              }
              break;
          }
        }
        return new GetFileResult()
        {
          Code = resp.StatusCode,
          RespStream = resp.StatusCode == HttpStatusCode.OK ? stream ?? resp.GetResponseStream() : (Stream) null,
          AgeSeconds = ageInSeconds,
          IsFromCache = resp.IsFromCache,
          ErrorMessage = errorMessage
        };
      }
      catch (UnauthorizedAccessException ex)
      {
        shouldAbort = true;
        errorMessage = ex.Message;
      }
      catch (OperationCanceledException ex)
      {
        shouldAbort = true;
        errorMessage = "Download was cancelled by caller";
      }
      catch (ObjectDisposedException ex)
      {
        shouldAbort = true;
        errorMessage = "Download was cancelled by caller";
      }
      catch (WebException ex)
      {
        shouldAbort = true;
        errorMessage = "Reading HTTP response stream throws an WebException";
      }
      finally
      {
        if (shouldAbort || resp.StatusCode != HttpStatusCode.OK)
          resp.GetResponseStream()?.Dispose();
      }
      return new GetFileResult()
      {
        ErrorMessage = errorMessage,
        Code = HttpStatusCode.Unused
      };
    }

    private async Task<Stream> WrapInSeekableStreamAsync(Stream s)
    {
      int bufferSize = 10000;
      MemoryStream ms = new MemoryStream();
      Stream stream;
      try
      {
        await s.CopyToAsync((Stream) ms, bufferSize, this.cancellationTokenSource.Token);
        ms.Position = 0L;
        stream = (Stream) ms;
      }
      catch
      {
        ms.Dispose();
        throw;
      }
      ms = (MemoryStream) null;
      return stream;
    }
  }
}
