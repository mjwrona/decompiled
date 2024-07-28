// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.Blob.BlobBatchSubOperationError
// Assembly: Microsoft.Azure.Storage.Blob, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: A04A3512-352A-442F-A95B-BC1B94EF8840
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.Blob.dll

using System.Net;

namespace Microsoft.Azure.Storage.Blob
{
  public sealed class BlobBatchSubOperationError
  {
    public int OperationIndex { get; internal set; }

    public HttpStatusCode StatusCode { get; internal set; }

    public string ErrorCode { get; internal set; }

    public StorageExtendedErrorInformation ExtendedErrorInformation { get; internal set; }

    internal BlobBatchSubOperationError()
    {
    }
  }
}
