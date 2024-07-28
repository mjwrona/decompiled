// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Table.RestExecutor.Utils.Exceptions
// Assembly: Microsoft.Azure.Cosmos.Table, Version=1.0.7.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 461D0B3A-0B96-4D42-B330-3A8E714FC39A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Table.dll

using System;

namespace Microsoft.Azure.Cosmos.Table.RestExecutor.Utils
{
  internal static class Exceptions
  {
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
