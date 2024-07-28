// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.Blob.BlobDeleteBatchOperation
// Assembly: Microsoft.Azure.Storage.Blob, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: A04A3512-352A-442F-A95B-BC1B94EF8840
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.Blob.dll

using Microsoft.Azure.Storage.Core.Util;

namespace Microsoft.Azure.Storage.Blob
{
  public sealed class BlobDeleteBatchOperation : BatchOperation
  {
    public void AddSubOperation(
      CloudBlob blob,
      DeleteSnapshotsOption deleteSnapshotsOption = DeleteSnapshotsOption.None,
      AccessCondition accessCondition = null,
      BlobRequestOptions blobRequestOptions = null)
    {
      CommonUtility.AssertInBounds<int>("operationCount", this.Operations.Count, 0, (int) byte.MaxValue);
      CommonUtility.AssertNotNull("blockBlob", (object) blob);
      this.Operations.Add(blob.DeleteBlobImpl(blob.attributes, deleteSnapshotsOption, accessCondition, blobRequestOptions ?? BlobRequestOptions.BaseDefaultRequestOptions));
    }
  }
}
