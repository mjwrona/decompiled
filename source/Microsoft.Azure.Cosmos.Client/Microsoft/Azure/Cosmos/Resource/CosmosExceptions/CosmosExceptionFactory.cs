// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Resource.CosmosExceptions.CosmosExceptionFactory
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Tracing;
using Microsoft.Azure.Documents;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace Microsoft.Azure.Cosmos.Resource.CosmosExceptions
{
  internal static class CosmosExceptionFactory
  {
    internal static CosmosException Create(DocumentClientException dce, ITrace trace)
    {
      Microsoft.Azure.Cosmos.Headers headers = dce.Headers == null ? new Microsoft.Azure.Cosmos.Headers() : new Microsoft.Azure.Cosmos.Headers(dce.Headers);
      HttpStatusCode? statusCode1 = dce.StatusCode;
      HttpStatusCode statusCode2;
      if (statusCode1.HasValue)
      {
        statusCode1 = dce.StatusCode;
        statusCode2 = statusCode1.Value;
      }
      else
        statusCode2 = dce.InnerException == null || !(dce.InnerException is TransportException) ? HttpStatusCode.InternalServerError : HttpStatusCode.RequestTimeout;
      return CosmosExceptionFactory.Create(statusCode2, dce.Message, dce.StackTrace, headers, trace, dce.Error, dce.InnerException);
    }

    internal static CosmosException Create(
      HttpStatusCode statusCode,
      RequestMessage requestMessage,
      string errorMessage)
    {
      return CosmosExceptionFactory.Create(statusCode, errorMessage, (string) null, requestMessage?.Headers, (ITrace) NoOpTrace.Singleton, (Error) null, (Exception) null);
    }

    internal static CosmosException Create(ResponseMessage responseMessage)
    {
      if (responseMessage.Content == null && responseMessage.CosmosException != null)
        return responseMessage.CosmosException;
      string message = responseMessage.CosmosException?.Message;
      (Error error, string str1) = CosmosExceptionFactory.GetErrorFromStream(responseMessage.Content);
      if (!string.IsNullOrEmpty(str1))
      {
        string str2;
        if (!string.IsNullOrEmpty(message))
          str2 = "Error Message: " + message + "; Content " + str1 + ";";
        else
          str2 = str1;
        message = str2;
      }
      return CosmosExceptionFactory.Create(responseMessage.StatusCode, message, responseMessage?.CosmosException?.StackTrace, responseMessage.Headers, responseMessage.Trace, error, responseMessage.CosmosException?.InnerException);
    }

    internal static CosmosException Create(
      DocumentServiceResponse documentServiceResponse,
      Microsoft.Azure.Cosmos.Headers responseHeaders,
      RequestMessage requestMessage)
    {
      if (documentServiceResponse == null)
        throw new ArgumentNullException(nameof (documentServiceResponse));
      if (requestMessage == null)
        throw new ArgumentNullException(nameof (requestMessage));
      if (responseHeaders == null)
        responseHeaders = new Microsoft.Azure.Cosmos.Headers(documentServiceResponse.Headers);
      (Error error, string message) = CosmosExceptionFactory.GetErrorFromStream(documentServiceResponse.ResponseBody);
      return CosmosExceptionFactory.Create(documentServiceResponse.StatusCode, message, (string) null, responseHeaders, requestMessage.Trace, error, (Exception) null);
    }

    internal static CosmosException Create(
      StoreResponse storeResponse,
      RequestMessage requestMessage)
    {
      if (storeResponse == null)
        throw new ArgumentNullException(nameof (storeResponse));
      if (requestMessage == null)
        throw new ArgumentNullException(nameof (requestMessage));
      (Error error, string message) = CosmosExceptionFactory.GetErrorFromStream(storeResponse.ResponseBody);
      Microsoft.Azure.Cosmos.Headers headers = new Microsoft.Azure.Cosmos.Headers(storeResponse.Headers);
      return CosmosExceptionFactory.Create(storeResponse.StatusCode, message, (string) null, headers, requestMessage.Trace, error, (Exception) null);
    }

    internal static (Error, string) GetErrorFromStream(Stream content)
    {
      using (content)
      {
        if (content == null || !content.CanRead)
          return ((Error) null, (string) null);
        using (StreamReader streamReader = new StreamReader(content))
        {
          string end = streamReader.ReadToEnd();
          try
          {
            JObject jobject = JObject.Parse(end);
            Error error = jobject.ToObject<Error>();
            if (error != null)
            {
              StringBuilder stringBuilder = new StringBuilder();
              foreach (KeyValuePair<string, JToken> keyValuePair in jobject)
                stringBuilder.Append(Environment.NewLine).Append(keyValuePair.Key).Append(" : ").Append((object) keyValuePair.Value);
              stringBuilder.Append(Environment.NewLine);
              return (error, stringBuilder.ToString());
            }
          }
          catch (JsonReaderException ex)
          {
          }
          content.Position = 0L;
          return ((Error) null, end);
        }
      }
    }

    internal static CosmosException CreateRequestTimeoutException(
      string message,
      Microsoft.Azure.Cosmos.Headers headers,
      string stackTrace = null,
      ITrace trace = null,
      Error error = null,
      Exception innerException = null)
    {
      return CosmosExceptionFactory.Create(HttpStatusCode.RequestTimeout, message, stackTrace, headers, trace, error, innerException);
    }

    internal static CosmosException CreateThrottledException(
      string message,
      Microsoft.Azure.Cosmos.Headers headers,
      string stackTrace = null,
      ITrace trace = null,
      Error error = null,
      Exception innerException = null)
    {
      return CosmosExceptionFactory.Create((HttpStatusCode) 429, message, stackTrace, headers, trace, error, innerException);
    }

    internal static CosmosException CreateNotFoundException(
      string message,
      Microsoft.Azure.Cosmos.Headers headers,
      string stackTrace = null,
      ITrace trace = null,
      Error error = null,
      Exception innerException = null)
    {
      return CosmosExceptionFactory.Create(HttpStatusCode.NotFound, message, stackTrace, headers, trace, error, innerException);
    }

    internal static CosmosException CreateInternalServerErrorException(
      string message,
      Microsoft.Azure.Cosmos.Headers headers,
      string stackTrace = null,
      ITrace trace = null,
      Error error = null,
      Exception innerException = null)
    {
      return CosmosExceptionFactory.Create(HttpStatusCode.InternalServerError, message, stackTrace, headers, trace, error, innerException);
    }

    internal static CosmosException CreateBadRequestException(
      string message,
      Microsoft.Azure.Cosmos.Headers headers,
      string stackTrace = null,
      ITrace trace = null,
      Error error = null,
      Exception innerException = null)
    {
      return CosmosExceptionFactory.Create(HttpStatusCode.BadRequest, message, stackTrace, headers, trace, error, innerException);
    }

    internal static CosmosException CreateUnauthorizedException(
      string message,
      Microsoft.Azure.Cosmos.Headers headers,
      Exception innerException,
      string stackTrace = null,
      ITrace trace = null,
      Error error = null)
    {
      return CosmosExceptionFactory.Create(HttpStatusCode.Unauthorized, message, stackTrace, headers, trace, error, innerException);
    }

    internal static CosmosException Create(
      HttpStatusCode statusCode,
      string message,
      string stackTrace,
      Microsoft.Azure.Cosmos.Headers headers,
      ITrace trace,
      Error error,
      Exception innerException)
    {
      return new CosmosException(statusCode, message, stackTrace, headers, trace, error, innerException);
    }
  }
}
