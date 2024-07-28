// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Table.Extensions.Shared.ErrorTranslator
// Assembly: Microsoft.Azure.Cosmos.Table, Version=1.0.7.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 461D0B3A-0B96-4D42-B330-3A8E714FC39A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Table.dll

using Microsoft.Azure.Documents;
using System;
using System.Net;

namespace Microsoft.Azure.Cosmos.Table.Extensions.Shared
{
  internal static class ErrorTranslator
  {
    private const string ResourceNotFound = "ResourceNotFound";
    private const string ResourceNotFoundMessage = "The specified resource does not exist.";
    private const string ConditionNotMet = "ConditionNotMet";
    private const string ConditionNotMetMessage = "The update condition specified in the request was not satisfied.";
    private const string EntityAlreadyExists = "EntityAlreadyExists";
    private const string EntityAlreadyExistsMessage = "The specified entity already exists.";
    private const string RequestBodyTooLarge = "RequestBodyTooLarge";
    private const string RequestBodyTooLargeMessage = "The request body is too large and exceeds the maximum permissible limit.";

    internal static Exception TranslateStoredProcedureException(Exception ex)
    {
      DocumentClientException innerException = ex as DocumentClientException;
      HttpStatusCode? statusCode1 = innerException.StatusCode;
      HttpStatusCode httpStatusCode1 = HttpStatusCode.BadRequest;
      if (statusCode1.GetValueOrDefault() == httpStatusCode1 & statusCode1.HasValue)
      {
        if (innerException.Message.Contains("Resource Not Found"))
          return (Exception) new DocumentClientException("The specified resource does not exist.", (Exception) innerException, new HttpStatusCode?(HttpStatusCode.NotFound), statusDescription: "ResourceNotFound");
        if (innerException.Message.Contains("One of the specified"))
          return (Exception) new DocumentClientException("The update condition specified in the request was not satisfied.", (Exception) innerException, new HttpStatusCode?(HttpStatusCode.PreconditionFailed), statusDescription: "ConditionNotMet");
        if (innerException.Message.Contains("Resource with specified id or name already exists"))
          return (Exception) new DocumentClientException("The specified entity already exists.", (Exception) innerException, new HttpStatusCode?(HttpStatusCode.Conflict), statusDescription: "EntityAlreadyExists");
      }
      else
      {
        HttpStatusCode? statusCode2 = innerException.StatusCode;
        HttpStatusCode httpStatusCode2 = HttpStatusCode.RequestEntityTooLarge;
        if (statusCode2.GetValueOrDefault() == httpStatusCode2 & statusCode2.HasValue)
        {
          DocumentClientException documentClientException = new DocumentClientException("The request body is too large and exceeds the maximum permissible limit.", (Exception) innerException, new HttpStatusCode?(HttpStatusCode.RequestEntityTooLarge), statusDescription: "RequestBodyTooLarge");
        }
      }
      return ex;
    }
  }
}
