// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.StorageException
// Assembly: Microsoft.Azure.Storage.Common, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 0978DA65-6954-4A99-9ACB-2EF3D979A5D5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.Common.dll

using Microsoft.Azure.Storage.Core.Util;
using Microsoft.Azure.Storage.Shared.Protocol;
using System;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Storage
{
  [Serializable]
  public class StorageException : Exception
  {
    public RequestResult RequestInformation { get; private set; }

    internal bool IsRetryable { get; set; }

    public StorageException()
      : this((RequestResult) null, (string) null, (Exception) null)
    {
    }

    public StorageException(string message)
      : this((RequestResult) null, message, (Exception) null)
    {
    }

    public StorageException(string message, Exception innerException)
      : this((RequestResult) null, message, innerException)
    {
    }

    protected StorageException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
      if (info == null)
        return;
      this.IsRetryable = info.GetBoolean(nameof (IsRetryable));
      this.RequestInformation = (RequestResult) info.GetValue(nameof (RequestInformation), typeof (RequestResult));
    }

    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
      if (info != null)
      {
        info.AddValue("IsRetryable", this.IsRetryable);
        info.AddValue("RequestInformation", (object) this.RequestInformation, typeof (RequestResult));
      }
      base.GetObjectData(info, context);
    }

    public StorageException(RequestResult res, string message, Exception inner)
      : base(message, inner)
    {
      this.RequestInformation = res;
      this.IsRetryable = true;
    }

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
          parseErrorAsync = new Func<Stream, CancellationToken, Task<StorageExtendedErrorInformation>>(StorageExtendedErrorInformation.ReadFromStreamAsync);
        StorageException storageException;
        if ((storageException = StorageException.CoreTranslateAsync(ex, reqResult, cancellationToken)) != null)
          return storageException;
        if (response != null)
        {
          StorageException.PopulateRequestResult(reqResult, response);
          RequestResult requestResult = reqResult;
          Func<Stream, CancellationToken, Task<StorageExtendedErrorInformation>> func = parseErrorAsync;
          StorageExtendedErrorInformation errorInformation = await func(await response.Content.ReadAsStreamAsync().ConfigureAwait(false), cancellationToken).ConfigureAwait(false);
          func = (Func<Stream, CancellationToken, Task<StorageExtendedErrorInformation>>) null;
          requestResult.ExtendedErrorInformation = errorInformation;
          requestResult = (RequestResult) null;
        }
      }
      catch (Exception ex1)
      {
      }
      return new StorageException(reqResult, ex.Message, ex);
    }

    internal static StorageException TranslateExceptionWithPreBufferedStream(
      Exception ex,
      RequestResult reqResult,
      Func<Stream, StorageExtendedErrorInformation> parseError,
      Stream responseStream,
      HttpResponseMessage response)
    {
      try
      {
        StorageException storageException;
        if ((storageException = StorageException.CoreTranslate(ex, reqResult, ref parseError)) != null)
          return storageException;
        if (response != null)
        {
          StorageException.PopulateRequestResult(reqResult, response);
          reqResult.ExtendedErrorInformation = StorageExtendedErrorInformation.ReadFromStreamAsync(responseStream).GetAwaiter().GetResult();
        }
      }
      catch (Exception ex1)
      {
      }
      return new StorageException(reqResult, ex.Message, ex);
    }

    private static StorageException CoreTranslate(
      Exception ex,
      RequestResult reqResult,
      ref Func<Stream, StorageExtendedErrorInformation> parseError)
    {
      CommonUtility.AssertNotNull(nameof (reqResult), (object) reqResult);
      CommonUtility.AssertNotNull(nameof (ex), (object) ex);
      if (parseError == null)
        parseError = (Func<Stream, StorageExtendedErrorInformation>) (s => StorageExtendedErrorInformation.ReadFromStreamAsync(s).GetAwaiter().GetResult());
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
        default:
          return (StorageException) null;
      }
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
        RequestResult requestResult = reqResult;
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
        requestResult.RequestDate = str;
        reqResult.Etag = response.Headers.ETag?.ToString();
        reqResult.ErrorCode = response.Headers.GetHeaderSingleValueOrDefault("x-ms-error-code");
      }
      if (response.Content == null || response.Content.Headers == null)
        return;
      reqResult.ContentMd5 = response.Content.Headers.ContentMD5 != null ? Convert.ToBase64String(response.Content.Headers.ContentMD5) : (string) null;
      reqResult.ContentCrc64 = HttpResponseParsers.GetContentCRC64(response);
      RequestResult requestResult1 = reqResult;
      long ingressBytes = requestResult1.IngressBytes;
      long? contentLength = response.Content.Headers.ContentLength;
      long num;
      if (!contentLength.HasValue)
      {
        num = 0L;
      }
      else
      {
        contentLength = response.Content.Headers.ContentLength;
        num = contentLength.Value;
      }
      requestResult1.IngressBytes = ingressBytes + num;
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendLine(base.ToString());
      if (this.RequestInformation != null)
      {
        stringBuilder.AppendLine("Request Information");
        stringBuilder.AppendLine("RequestID:" + this.RequestInformation.ServiceRequestID);
        stringBuilder.AppendLine("RequestDate:" + this.RequestInformation.RequestDate);
        stringBuilder.AppendLine("StatusMessage:" + this.RequestInformation.HttpStatusMessage);
        stringBuilder.AppendLine("ErrorCode:" + this.RequestInformation.ErrorCode);
        if (this.RequestInformation.ExtendedErrorInformation != null)
          stringBuilder.AppendLine("ErrorMessage:" + this.RequestInformation.ExtendedErrorInformation.ErrorMessage);
      }
      return stringBuilder.ToString();
    }
  }
}
