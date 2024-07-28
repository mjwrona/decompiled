// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Azure.TableBatchOperationResult
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Azure, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7823E4AE-BEB6-4A7C-9914-276DEAE1FB1F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Azure.dll

using Microsoft.Azure.Cosmos.Table;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using System;
using System.Collections.Generic;
using System.Net;

namespace Microsoft.VisualStudio.Services.Content.Server.Azure
{
  public sealed class TableBatchOperationResult : 
    Result<IList<TableOperationResult>, TableBatchOperationResult.Error>
  {
    private TableBatchOperationResult(IList<TableOperationResult> results)
      : base(results)
    {
    }

    private TableBatchOperationResult(TableBatchOperationResult.Error error)
      : base(error)
    {
    }

    public TableBatchOperationResult.Error GetError() => this.Match<TableBatchOperationResult.Error>((Func<IList<TableOperationResult>, TableBatchOperationResult.Error>) (results => (TableBatchOperationResult.Error) null), (Func<TableBatchOperationResult.Error, TableBatchOperationResult.Error>) (e => e));

    public static TableBatchOperationResult FromSuccess(IList<TableOperationResult> results) => new TableBatchOperationResult(results);

    public static TableBatchOperationResult FromError(
      int? failedOperationIndex,
      TableOperationDescriptor failedOperation,
      string errorCode,
      HttpStatusCode statusCode,
      StorageException exception)
    {
      return new TableBatchOperationResult(new TableBatchOperationResult.Error(failedOperationIndex, failedOperation, errorCode, statusCode, exception));
    }

    public sealed class Error
    {
      public readonly int? FailedOperationIndex;
      public readonly TableOperationDescriptor FailedOperation;
      public readonly string ErrorCode;
      public readonly HttpStatusCode StatusCode;
      public readonly StorageException Exception;

      public Error(
        int? failedOperationIndex,
        TableOperationDescriptor failedOperation,
        string errorCode,
        HttpStatusCode statusCode,
        StorageException exception)
      {
        this.FailedOperationIndex = failedOperationIndex;
        this.ErrorCode = errorCode;
        this.StatusCode = statusCode;
        this.Exception = exception;
        this.FailedOperation = failedOperation;
      }

      public override string ToString() => string.Format("StatusCode:{0} ErrorCode:'{1}' FailedOperationIndex:{2} Exception:'{3}'", (object) this.StatusCode, (object) this.ErrorCode, (object) this.FailedOperationIndex, (object) this.Exception?.Message);
    }
  }
}
