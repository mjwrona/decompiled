// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Table.Extensions.DocumentHelpers
// Assembly: Microsoft.Azure.Cosmos.Table, Version=1.0.7.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 461D0B3A-0B96-4D42-B330-3A8E714FC39A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Table.dll

using Microsoft.Azure.Cosmos.Tables.SharedFiles;
using System;

namespace Microsoft.Azure.Cosmos.Table.Extensions
{
  internal static class DocumentHelpers
  {
    public static StorageException ToStorageException(
      this Exception exception,
      TableOperation tableOperation)
    {
      if (exception is StorageException)
        return (StorageException) exception;
      RequestResult requestResult;
      if (exception is TableException)
      {
        TableException tableException = exception as TableException;
        requestResult = TableExtensionOperationHelper.GenerateRequestResult(tableException.HttpStatusMessage, (int) tableException.HttpStatusCode, tableException.ErrorCode, tableException.ErrorMessage, (string) null, new double?());
      }
      else
      {
        TableOperationWrapper tableOperation1 = (TableOperationWrapper) null;
        if (tableOperation != null)
          tableOperation1 = new TableOperationWrapper()
          {
            IsTableEntity = tableOperation.IsTableEntity,
            OperationType = tableOperation.OperationType
          };
        TableErrorResult tableErrorResult = exception.TranslateDocumentErrorForStoredProcs(tableOperation1);
        requestResult = TableExtensionOperationHelper.GenerateRequestResult(tableErrorResult.HttpStatusMessage, tableErrorResult.HttpStatusCode, tableErrorResult.ExtendedErrorCode, tableErrorResult.ExtendedErroMessage, tableErrorResult.ServiceRequestID, new double?(tableErrorResult.RequestCharge));
      }
      return new StorageException(requestResult, requestResult.ExtendedErrorInformation.ErrorMessage, exception);
    }
  }
}
