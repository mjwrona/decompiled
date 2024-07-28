// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.Blob.BlobBatchException
// Assembly: Microsoft.Azure.Storage.Blob, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: A04A3512-352A-442F-A95B-BC1B94EF8840
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.Blob.dll

using System.Collections.Generic;

namespace Microsoft.Azure.Storage.Blob
{
  public class BlobBatchException : StorageException
  {
    public IList<BlobBatchSubOperationResponse> SuccessfulResponses { get; internal set; }

    public IList<BlobBatchSubOperationError> ErrorResponses { get; internal set; }

    internal BlobBatchException()
      : base("One or more of the sub responses on this batch operation has failed. Check the list members on this exception to determine which operations were unsuccessful.")
    {
    }
  }
}
