// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.BlobCopyStats
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.Azure.Storage.Blob;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Cloud
{
  public class BlobCopyStats
  {
    public int TotalBlobs;
    public int BlobsCompleted;
    public int NoCopyInfo;
    public int PendingCopies;
    public List<string> BlobsPendingCopy = new List<string>();
    public bool ReissuedCopies;
    public bool HasPendingCopies;
    public BlobContinuationToken ContinuationToken;

    public int Downloadable => this.TotalBlobs - this.PendingCopies;

    public static BlobCopyStats Clone(BlobCopyStats other) => new BlobCopyStats()
    {
      TotalBlobs = other.TotalBlobs,
      BlobsCompleted = other.BlobsCompleted,
      NoCopyInfo = other.NoCopyInfo,
      ContinuationToken = other.ContinuationToken
    };
  }
}
