// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.Blob.BlobResultSegment
// Assembly: Microsoft.Azure.Storage.Blob, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: A04A3512-352A-442F-A95B-BC1B94EF8840
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.Blob.dll

using System.Collections.Generic;

namespace Microsoft.Azure.Storage.Blob
{
  public class BlobResultSegment
  {
    public BlobResultSegment(
      IEnumerable<IListBlobItem> blobs,
      BlobContinuationToken continuationToken)
    {
      this.Results = blobs;
      this.ContinuationToken = continuationToken;
    }

    public IEnumerable<IListBlobItem> Results { get; private set; }

    public BlobContinuationToken ContinuationToken { get; private set; }
  }
}
