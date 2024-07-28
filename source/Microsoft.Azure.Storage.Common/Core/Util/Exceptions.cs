// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.Core.Util.Exceptions
// Assembly: Microsoft.Azure.Storage.Common, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 0978DA65-6954-4A99-9ACB-2EF3D979A5D5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.Common.dll

using Microsoft.Azure.Storage.Shared.Protocol;
using System;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Storage.Core.Util
{
  internal class Exceptions
  {
    internal static async Task<StorageException> PopulateStorageExceptionFromHttpResponseMessage(
      HttpResponseMessage response,
      RequestResult currentResult,
      CancellationToken token,
      Func<Stream, HttpResponseMessage, string, CancellationToken, Task<StorageExtendedErrorInformation>> parseError)
    {
      if (response.IsSuccessStatusCode)
        return (StorageException) null;
      try
      {
        currentResult.HttpStatusMessage = response.ReasonPhrase;
        currentResult.HttpStatusCode = (int) response.StatusCode;
        currentResult.ServiceRequestID = response.Headers.GetHeaderSingleValueOrDefault("x-ms-request-id");
        string singleValueOrDefault = response.Headers.GetHeaderSingleValueOrDefault("x-ms-date");
        currentResult.RequestDate = string.IsNullOrEmpty(singleValueOrDefault) ? DateTime.Now.ToString("R", (IFormatProvider) CultureInfo.InvariantCulture) : singleValueOrDefault;
        if (response.Headers.ETag != null)
          currentResult.Etag = response.Headers.ETag.ToString();
        if (response.Content != null && response.Content.Headers.ContentMD5 != null)
          currentResult.ContentMd5 = Convert.ToBase64String(response.Content.Headers.ContentMD5);
        if (response.Content != null && HttpResponseParsers.GetContentCRC64(response) != null)
          currentResult.ContentCrc64 = HttpResponseParsers.GetContentCRC64(response);
        currentResult.ErrorCode = response.Headers.GetHeaderSingleValueOrDefault("x-ms-error-code");
      }
      catch (Exception ex)
      {
      }
      try
      {
        Stream inputStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
        RequestResult requestResult;
        if (parseError != null)
        {
          requestResult = currentResult;
          requestResult.ExtendedErrorInformation = await parseError(inputStream, response, response.Content.Headers.ContentType.ToString(), token).ConfigureAwait(false);
          requestResult = (RequestResult) null;
        }
        else
        {
          requestResult = currentResult;
          requestResult.ExtendedErrorInformation = await StorageExtendedErrorInformation.ReadFromStreamAsync(inputStream, token).ConfigureAwait(false);
          requestResult = (RequestResult) null;
        }
      }
      catch (Exception ex)
      {
      }
      return new StorageException(currentResult, response.ReasonPhrase, (Exception) null);
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

    internal static StorageException GenerateCancellationException(
      RequestResult res,
      Exception inner)
    {
      if (res != null)
      {
        res.HttpStatusCode = 306;
        res.HttpStatusMessage = "Unused";
      }
      OperationCanceledException inner1 = new OperationCanceledException("Operation was canceled by user.", inner);
      return new StorageException(res, inner1.Message, (Exception) inner1)
      {
        IsRetryable = false
      };
    }
  }
}
