// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Upstream.PublicUpstreamHttpClientHelper
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.BlobStore.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Exceptions;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;


#nullable enable
namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Upstream
{
  public static class PublicUpstreamHttpClientHelper
  {
    public static async Task<TResult> GetWithErrorHandlingAsync<TResult>(
      IHttpClient httpClient,
      string upstreamIdentifier,
      PackageIdentifierForMessages packageIdentifier,
      Uri uri,
      HttpCompletionOption completionOption,
      Func<HttpResponseMessage, Task<TResult>> deserializeFunc,
      Action<Uri, HttpStatusCode>? errorCodeHandlingOverride = null)
    {
      return await PublicUpstreamHttpClientHelper.GetWithErrorHandlingCoreAsync<TResult>(httpClient, upstreamIdentifier, packageIdentifier, uri, completionOption, deserializeFunc, errorCodeHandlingOverride, false, false) ?? throw new Exception("GetWithErrorHandlingCoreAsync unexpectedly returned null with nullIf404: false");
    }

    public static async Task<Stream> GetStreamWithErrorHandlingAsync(
      IHttpClient httpClient,
      string upstreamIdentifier,
      PackageIdentifierForMessages packageIdentifier,
      Uri uri,
      HttpCompletionOption completionOption,
      Action<Uri, HttpStatusCode>? errorCodeHandlingOverride = null)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return (Stream) (await PublicUpstreamHttpClientHelper.GetWithErrorHandlingCoreAsync<DisposingStreamWrapper>(httpClient, upstreamIdentifier, packageIdentifier, uri, completionOption, PublicUpstreamHttpClientHelper.\u003C\u003EO.\u003C0\u003E__ExtractStream ?? (PublicUpstreamHttpClientHelper.\u003C\u003EO.\u003C0\u003E__ExtractStream = new Func<HttpResponseMessage, Task<DisposingStreamWrapper>>(PublicUpstreamHttpClientHelper.ExtractStream)), errorCodeHandlingOverride, true, false) ?? throw new Exception("GetWithErrorHandlingCoreAsync unexpectedly returned null with nullIf404: false"));
    }

    public static async Task<TResult?> GetWithErrorHandlingNullIf404Async<TResult>(
      IHttpClient httpClient,
      string upstreamIdentifier,
      PackageIdentifierForMessages packageIdentifier,
      Uri uri,
      HttpCompletionOption completionOption,
      Func<HttpResponseMessage, Task<TResult>> deserializeFunc,
      Action<Uri, HttpStatusCode>? errorCodeHandlingOverride = null)
    {
      return await PublicUpstreamHttpClientHelper.GetWithErrorHandlingCoreAsync<TResult>(httpClient, upstreamIdentifier, packageIdentifier, uri, completionOption, deserializeFunc, errorCodeHandlingOverride, false, true);
    }

    public static async Task<Stream?> GetStreamWithErrorHandlingNullIf404Async(
      IHttpClient httpClient,
      string upstreamIdentifier,
      PackageIdentifierForMessages packageIdentifier,
      Uri uri,
      HttpCompletionOption completionOption,
      Action<Uri, HttpStatusCode>? errorCodeHandlingOverride = null)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return (Stream) await PublicUpstreamHttpClientHelper.GetWithErrorHandlingCoreAsync<DisposingStreamWrapper>(httpClient, upstreamIdentifier, packageIdentifier, uri, completionOption, PublicUpstreamHttpClientHelper.\u003C\u003EO.\u003C0\u003E__ExtractStream ?? (PublicUpstreamHttpClientHelper.\u003C\u003EO.\u003C0\u003E__ExtractStream = new Func<HttpResponseMessage, Task<DisposingStreamWrapper>>(PublicUpstreamHttpClientHelper.ExtractStream)), errorCodeHandlingOverride, true, true);
    }

    private static PublicUpstreamHttpClientHelper.OptionallyDisposable<T> OptionallyDispose<T>(
      T value)
      where T : IDisposable
    {
      return new PublicUpstreamHttpClientHelper.OptionallyDisposable<T>(value);
    }

    private static async Task<TResult?> GetWithErrorHandlingCoreAsync<TResult>(
      IHttpClient httpClient,
      string upstreamIdentifier,
      PackageIdentifierForMessages packageIdentifier,
      Uri uri,
      HttpCompletionOption completionOption,
      Func<HttpResponseMessage, Task<TResult>> deserializeFunc,
      Action<Uri, HttpStatusCode>? errorCodeOverride,
      bool keepResponseOpenOnSuccess,
      bool nullIf404)
    {
      HttpStatusCode statusCode;
      string reasonPhrase;
      try
      {
        PublicUpstreamHttpClientHelper.OptionallyDisposable<HttpResponseMessage> responseDisposable = PublicUpstreamHttpClientHelper.OptionallyDispose<HttpResponseMessage>(await httpClient.GetAsync(uri, completionOption));
        try
        {
          HttpResponseMessage httpResponseMessage = responseDisposable.Value;
          statusCode = httpResponseMessage.StatusCode;
          reasonPhrase = httpResponseMessage.ReasonPhrase;
          if (httpResponseMessage.IsSuccessStatusCode)
          {
            TResult handlingCoreAsync = await deserializeFunc(httpResponseMessage);
            if (keepResponseOpenOnSuccess)
              responseDisposable.DontDispose();
            return handlingCoreAsync;
          }
        }
        finally
        {
          responseDisposable.Dispose();
        }
        responseDisposable = new PublicUpstreamHttpClientHelper.OptionallyDisposable<HttpResponseMessage>();
      }
      catch (Exception ex)
      {
        throw new PublicUpstreamFailureException(Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Error_UpstreamFailure((object) upstreamIdentifier, (object) ex.Message), ex, uri);
      }
      Action<Uri, HttpStatusCode> action = errorCodeOverride;
      if (action != null)
        action(uri, statusCode);
      if (statusCode == HttpStatusCode.NotFound)
      {
        if (nullIf404)
          return default (TResult);
        if (packageIdentifier.HasValue)
          throw new PackageNotFoundException(packageIdentifier.FileDisplayString != null ? Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Error_PackageFileNotFoundInUpstreamNoFeed((object) packageIdentifier.FileDisplayString, (object) packageIdentifier.PackageDisplayString, (object) upstreamIdentifier, (object) uri.AbsoluteUri) : Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Error_PackageNotFoundInUpstreamNoFeed((object) packageIdentifier.PackageDisplayString, (object) upstreamIdentifier, (object) uri.AbsoluteUri));
      }
      throw new PublicUpstreamFailureException(Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Error_UpstreamFailure((object) upstreamIdentifier, (object) string.Format("{0} ({1})", (object) statusCode, (object) reasonPhrase)), uri);
    }

    private static async Task<DisposingStreamWrapper> ExtractStream(HttpResponseMessage response) => new DisposingStreamWrapper(await response.Content.ReadAsStreamAsync(), new IDisposable[1]
    {
      (IDisposable) response
    });

    private struct OptionallyDisposable<T> : IDisposable where T : IDisposable
    {
      private bool shouldDispose;

      public OptionallyDisposable(T value)
      {
        this.Value = value;
        this.shouldDispose = true;
      }

      public T Value { get; }

      public void DontDispose() => this.shouldDispose = false;

      public void Dispose()
      {
        if (!this.shouldDispose)
          return;
        this.Value.Dispose();
      }
    }
  }
}
