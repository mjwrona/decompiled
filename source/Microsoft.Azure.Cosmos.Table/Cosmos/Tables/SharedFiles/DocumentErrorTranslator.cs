// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Tables.SharedFiles.DocumentErrorTranslator
// Assembly: Microsoft.Azure.Cosmos.Table, Version=1.0.7.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 461D0B3A-0B96-4D42-B330-3A8E714FC39A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Table.dll

using Microsoft.Azure.Cosmos.Table;
using Microsoft.Azure.Cosmos.Table.Protocol;
using Microsoft.Azure.Documents;
using System;
using System.Globalization;
using System.Net;

namespace Microsoft.Azure.Cosmos.Tables.SharedFiles
{
  internal static class DocumentErrorTranslator
  {
    private const string OperationCountPrefix = "OperationCount:";

    public static TableErrorResult TranslateDocumentErrorForTables(
      this Exception exception,
      TableOperationWrapper tableOperation = null)
    {
      DocumentClientException documentClientException = exception as DocumentClientException;
      TableErrorResult tableErrorResult;
      if (documentClientException == null)
      {
        tableErrorResult = DocumentErrorTranslator.HandleNonDocumentClientExceptions(exception);
      }
      else
      {
        TableErrorResult reqResult = new TableErrorResult()
        {
          HttpStatusCode = (int) documentClientException.StatusCode.Value
        };
        reqResult.HttpStatusMessage = string.IsNullOrWhiteSpace(documentClientException.StatusDescription) ? DocumentErrorTranslator.GetStatusDescription(reqResult.HttpStatusCode) : documentClientException.StatusDescription;
        reqResult.ServiceRequestID = documentClientException.ActivityId;
        reqResult.RequestCharge = documentClientException.RequestCharge;
        reqResult.ExtendedErrorCode = documentClientException.Error.Code;
        reqResult.ExtendedErroMessage = documentClientException.Message;
        tableErrorResult = DocumentErrorTranslator.FillTableErrorResult(reqResult, tableOperation);
      }
      return tableErrorResult;
    }

    private static TableErrorResult HandleNonDocumentClientExceptions(Exception exception)
    {
      switch (exception)
      {
        case NotSupportedException _:
        case TimeoutException _:
          throw exception;
        default:
          TableErrorResult tableErrorResult = new TableErrorResult();
          switch (exception)
          {
            case InvalidEtagException _:
              tableErrorResult.HttpStatusCode = 400;
              tableErrorResult.HttpStatusMessage = RMResources.BadRequest;
              tableErrorResult.ExtendedErrorCode = HttpStatusCode.BadRequest.ToString();
              tableErrorResult.ExtendedErroMessage = exception.Message;
              return tableErrorResult;
            case InvalidFilterException _:
              tableErrorResult.HttpStatusCode = 400;
              tableErrorResult.HttpStatusMessage = RMResources.BadRequest;
              tableErrorResult.ExtendedErrorCode = TableErrorCodeStrings.InvalidInput;
              tableErrorResult.ExtendedErroMessage = RMResources.BadUrl;
              return tableErrorResult;
            default:
              tableErrorResult.HttpStatusCode = 500;
              tableErrorResult.HttpStatusMessage = "Server encountered an internal error.Please try again after some time.";
              tableErrorResult.ExtendedErrorCode = HttpStatusCode.InternalServerError.ToString();
              tableErrorResult.ExtendedErroMessage = exception.Message;
              return tableErrorResult;
          }
      }
    }

    public static TableErrorResult TranslateDocumentErrorForStoredProcs(
      this Exception exception,
      TableOperationWrapper tableOperation = null,
      int batchOperationCount = 0)
    {
      if (!(exception is DocumentClientException documentClientException))
        return DocumentErrorTranslator.HandleNonDocumentClientExceptions(exception);
      TableErrorResult reqResult = new TableErrorResult();
      reqResult.RequestCharge = documentClientException.RequestCharge;
      reqResult.ServiceRequestID = documentClientException.ActivityId;
      HttpStatusCode? statusCode1 = documentClientException.StatusCode;
      HttpStatusCode httpStatusCode1 = HttpStatusCode.BadRequest;
      if (statusCode1.GetValueOrDefault() == httpStatusCode1 & statusCode1.HasValue)
      {
        bool flag = true;
        if (documentClientException.Message.Contains("Resource Not Found"))
          reqResult.HttpStatusCode = 404;
        else if (documentClientException.Message.Contains("One of the specified"))
          reqResult.HttpStatusCode = 412;
        else if (documentClientException.Message.Contains("Resource with specified id or name already exists"))
          reqResult.HttpStatusCode = 409;
        else if (documentClientException.Message.Contains("Failed to enqueue operation"))
        {
          reqResult.HttpStatusCode = 429;
          reqResult.HttpStatusMessage = "Too Many Requests";
          reqResult.ExtendedErrorCode = "TooManyRequests";
          reqResult.ExtendedErroMessage = RMResources.TooManyRequests;
        }
        else
          flag = false;
        reqResult = !flag ? exception.TranslateDocumentErrorForTables(tableOperation) : DocumentErrorTranslator.FillTableErrorResult(reqResult, tableOperation);
      }
      else
      {
        HttpStatusCode? statusCode2 = documentClientException.StatusCode;
        HttpStatusCode httpStatusCode2 = HttpStatusCode.RequestEntityTooLarge;
        if (statusCode2.GetValueOrDefault() == httpStatusCode2 & statusCode2.HasValue)
        {
          reqResult.HttpStatusCode = 413;
          reqResult.ExtendedErrorCode = StorageErrorCodeStrings.RequestBodyTooLarge;
          reqResult.ExtendedErroMessage = "The request body is too large and exceeds the maximum permissible limit.";
        }
        else
          reqResult = exception.TranslateDocumentErrorForTables(tableOperation);
      }
      reqResult.HttpStatusMessage = DocumentErrorTranslator.GetStatusDescription(reqResult.HttpStatusCode);
      int num1 = -1;
      if (batchOperationCount > 0)
        num1 = documentClientException.Message.IndexOf("OperationCount:", StringComparison.Ordinal);
      if (num1 >= 0)
      {
        int startIndex = num1 + "OperationCount:".Length;
        int num2 = documentClientException.Message.IndexOf('.', startIndex);
        int result = -1;
        int.TryParse(documentClientException.Message.Substring(startIndex, num2 - startIndex), out result);
        if (result >= 0)
          reqResult.ExtendedErroMessage = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}:{1}\n", (object) result, (object) reqResult.ExtendedErroMessage);
      }
      return reqResult;
    }

    public static TableErrorResult FillTableErrorResult(
      TableErrorResult reqResult,
      TableOperationWrapper tableOperation = null)
    {
      switch (reqResult.HttpStatusCode)
      {
        case 404:
          if (tableOperation != null && tableOperation.IsTableEntity)
          {
            if (tableOperation.OperationType != TableOperationType.Delete)
            {
              reqResult.ExtendedErrorCode = TableErrorCodeStrings.TableNotFound;
              reqResult.ExtendedErroMessage = "The specified table was not found.";
              break;
            }
            reqResult.ExtendedErrorCode = "ResourceNotFound";
            reqResult.ExtendedErroMessage = "The specified resource does not exist.";
            break;
          }
          reqResult.ExtendedErrorCode = "ResourceNotFound";
          reqResult.ExtendedErroMessage = "The specified resource does not exist.";
          break;
        case 409:
          if (tableOperation != null && tableOperation.IsTableEntity)
          {
            reqResult.ExtendedErrorCode = TableErrorCodeStrings.TableAlreadyExists;
            reqResult.ExtendedErroMessage = "The specified table already exists.";
            break;
          }
          reqResult.ExtendedErrorCode = TableErrorCodeStrings.EntityAlreadyExists;
          reqResult.ExtendedErroMessage = "The specified entity already exists.";
          break;
        case 412:
          reqResult.ExtendedErrorCode = TableErrorCodeStrings.UpdateConditionNotSatisfied;
          reqResult.ExtendedErroMessage = "The update condition specified in the request was not satisfied.";
          break;
        case 413:
          reqResult.ExtendedErrorCode = TableErrorCodeStrings.EntityTooLarge;
          reqResult.ExtendedErroMessage = "The entity is larger than the maximum size permitted.";
          reqResult.HttpStatusCode = 400;
          break;
      }
      return reqResult;
    }

    private static string GetStatusDescription(int code)
    {
      string statusDescription = ((HttpStatusCode) code).ToString();
      if (!string.IsNullOrEmpty(statusDescription))
        return statusDescription;
      return code == 429 ? "Too Many Requests" : "Internal Server Error";
    }
  }
}
