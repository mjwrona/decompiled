// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Table.RestExecutor.Common.StorageExceptionTranslator
// Assembly: Microsoft.Azure.Cosmos.Table, Version=1.0.7.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 461D0B3A-0B96-4D42-B330-3A8E714FC39A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Table.dll

using Microsoft.Azure.Cosmos.Table.RestExecutor.Utils;
using System;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos.Table.RestExecutor.Common
{
  internal static class StorageExceptionTranslator
  {
    public static async Task<StorageException> TranslateExceptionAsync(
      Exception ex,
      RequestResult reqResult,
      Func<Stream, CancellationToken, Task<StorageExtendedErrorInformation>> parseErrorAsync,
      CancellationToken cancellationToken,
      HttpResponseMessage response)
    {
      try
      {
        if (parseErrorAsync == null)
          parseErrorAsync = new Func<Stream, CancellationToken, Task<StorageExtendedErrorInformation>>(StorageExtendedErrorInformationRestHelper.ReadFromStreamAsync);
        StorageException storageException;
        if ((storageException = StorageExceptionTranslator.CoreTranslateAsync(ex, reqResult, cancellationToken)) != null)
          return storageException;
        if (response != null)
        {
          StorageExceptionTranslator.PopulateRequestResult(reqResult, response);
          RequestResult requestResult1 = reqResult;
          Func<Stream, CancellationToken, Task<StorageExtendedErrorInformation>> func1 = parseErrorAsync;
          RequestResult requestResult = requestResult1;
          Func<Stream, CancellationToken, Task<StorageExtendedErrorInformation>> func = func1;
          requestResult.ExtendedErrorInformation = await func(await response.Content.ReadAsStreamAsync().ConfigureAwait(false), cancellationToken).ConfigureAwait(false);
          requestResult = (RequestResult) null;
          func = (Func<Stream, CancellationToken, Task<StorageExtendedErrorInformation>>) null;
        }
      }
      catch (Exception ex1)
      {
      }
      return new StorageException(reqResult, ex.Message, ex);
    }

    internal static async Task<StorageException> TranslateExceptionWithPreBufferedStreamAsync(
      Exception ex,
      RequestResult reqResult,
      Stream responseStream,
      HttpResponseMessage response,
      Func<Stream, HttpResponseMessage, string, CancellationToken, Task<StorageExtendedErrorInformation>> parseErrorAsync,
      CancellationToken cancellationToken)
    {
      try
      {
        StorageException storageException;
        if ((storageException = StorageExceptionTranslator.CoreTranslateAsync(ex, reqResult, CancellationToken.None)) != null)
          return storageException;
        if (response != null)
        {
          StorageExceptionTranslator.PopulateRequestResult(reqResult, response);
          if (parseErrorAsync == null)
          {
            RequestResult requestResult = reqResult;
            requestResult.ExtendedErrorInformation = await StorageExtendedErrorInformationRestHelper.ReadFromStreamAsync(responseStream, cancellationToken);
            requestResult = (RequestResult) null;
          }
          else
            reqResult.ExtendedErrorInformation = parseErrorAsync(responseStream, response, response.Content.Headers.ContentType.ToString(), cancellationToken).Result;
        }
      }
      catch (Exception ex1)
      {
      }
      return new StorageException(reqResult, ex.Message, ex);
    }

    private static StorageException CoreTranslateAsync(
      Exception ex,
      RequestResult reqResult,
      CancellationToken token)
    {
      CommonUtility.AssertNotNull(nameof (reqResult), (object) reqResult);
      CommonUtility.AssertNotNull(nameof (ex), (object) ex);
      switch (ex)
      {
        case StorageException _:
          return (StorageException) ex;
        case TimeoutException _:
          reqResult.HttpStatusMessage = (string) null;
          reqResult.HttpStatusCode = 408;
          reqResult.ExtendedErrorInformation = (StorageExtendedErrorInformation) null;
          return new StorageException(reqResult, ex.Message, ex);
        case ArgumentException _:
          reqResult.HttpStatusMessage = (string) null;
          reqResult.HttpStatusCode = 306;
          reqResult.ExtendedErrorInformation = (StorageExtendedErrorInformation) null;
          return new StorageException(reqResult, ex.Message, ex)
          {
            IsRetryable = false
          };
        case OperationCanceledException _:
          reqResult.HttpStatusMessage = (string) null;
          reqResult.HttpStatusCode = 306;
          reqResult.ExtendedErrorInformation = (StorageExtendedErrorInformation) null;
          return new StorageException(reqResult, ex.Message, ex);
        default:
          return (StorageException) null;
      }
    }

    private static void PopulateRequestResult(RequestResult reqResult, HttpResponseMessage response)
    {
      reqResult.HttpStatusMessage = response.ReasonPhrase;
      reqResult.HttpStatusCode = (int) response.StatusCode;
      if (response.Headers != null)
      {
        reqResult.ServiceRequestID = response.Headers.GetHeaderSingleValueOrDefault("x-ms-request-id");
        DateTimeOffset? date = response.Headers.Date;
        string str;
        if (!date.HasValue)
        {
          str = (string) null;
        }
        else
        {
          date = response.Headers.Date;
          str = date.Value.UtcDateTime.ToString("R", (IFormatProvider) CultureInfo.InvariantCulture);
        }
        reqResult.RequestDate = str;
        reqResult.Etag = response.Headers.ETag?.ToString();
        reqResult.ErrorCode = response.Headers.GetHeaderSingleValueOrDefault("x-ms-error-code");
      }
      if (response.Content == null || response.Content.Headers == null)
        return;
      reqResult.ContentMd5 = response.Content.Headers.ContentMD5 != null ? Convert.ToBase64String(response.Content.Headers.ContentMD5) : (string) null;
    }

    internal static async Task<StorageException> PopulateStorageExceptionFromHttpResponseMessage(
      HttpResponseMessage response,
      RequestResult currentResult,
      CancellationToken token,
      Func<Stream, HttpResponseMessage, string, CancellationToken, Task<StorageExtendedErrorInformation>> parseErrorAsync)
    {
      if (response.IsSuccessStatusCode)
        return (StorageException) null;
      try
      {
        currentResult.HttpStatusCode = (int) response.StatusCode;
        currentResult.HttpStatusMessage = currentResult.HttpStatusCode.Equals(400) ? "The remote server returned an error: (400) Bad Request." : response.ReasonPhrase;
        currentResult.ServiceRequestID = response.Headers.GetHeaderSingleValueOrDefault("x-ms-request-id");
        string singleValueOrDefault = response.Headers.GetHeaderSingleValueOrDefault("x-ms-date");
        currentResult.RequestDate = string.IsNullOrEmpty(singleValueOrDefault) ? DateTime.UtcNow.ToString("R", (IFormatProvider) CultureInfo.InvariantCulture) : singleValueOrDefault;
        if (response.Headers.ETag != null)
          currentResult.Etag = response.Headers.ETag.ToString();
        if (response.Content != null && response.Content.Headers.ContentMD5 != null)
          currentResult.ContentMd5 = Convert.ToBase64String(response.Content.Headers.ContentMD5);
        currentResult.ErrorCode = response.Headers.GetHeaderSingleValueOrDefault("x-ms-error-code");
      }
      catch (Exception ex)
      {
      }
      try
      {
        Stream inputStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
        RequestResult requestResult;
        if (parseErrorAsync != null)
        {
          requestResult = currentResult;
          requestResult.ExtendedErrorInformation = await parseErrorAsync(inputStream, response, response.Content.Headers.ContentType.ToString(), token).ConfigureAwait(false);
          requestResult = (RequestResult) null;
        }
        else
        {
          requestResult = currentResult;
          requestResult.ExtendedErrorInformation = await StorageExtendedErrorInformationRestHelper.ReadFromStreamAsync(inputStream, token).ConfigureAwait(false);
          requestResult = (RequestResult) null;
        }
      }
      catch (Exception ex)
      {
      }
      return new StorageException(currentResult, currentResult.HttpStatusMessage, (Exception) null);
    }

    internal static StorageException GenerateTimeoutException(RequestResult res, Exception inner)
    {
      if (res != null)
        res.HttpStatusCode = 408;
      TimeoutException inner1 = new TimeoutException("The client could not finish the operation within specified timeout.", inner);
      return new StorageException(res, inner1.Message, (Exception) inner1)
      {
        IsRetryable = false
      };
    }
  }
}
