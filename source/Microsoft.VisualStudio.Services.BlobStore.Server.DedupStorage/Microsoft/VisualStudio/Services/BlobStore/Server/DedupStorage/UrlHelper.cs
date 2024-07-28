// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.DedupStorage.UrlHelper
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server.DedupStorage, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9521FAE3-5DB1-49D0-98DB-6A544E3AB730
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.DedupStorage.dll

using BuildXL.Cache.ContentStore.Hashing;
using Microsoft.VisualStudio.Services.BlobStore.Common;

namespace Microsoft.VisualStudio.Services.BlobStore.Server.DedupStorage
{
  public static class UrlHelper
  {
    public static string GetContentStitcherSigningPath(
      IDomainId domainId,
      string containerSetId,
      DedupIdentifier dedupId)
    {
      return string.Format("{0}/blobstoredomain:{1}/{2}/dedups/{3}/signedUrl", (object) "contentstitcher", (object) domainId, (object) containerSetId, (object) dedupId);
    }

    public static string GetContentStitcherZippingPath(IDomainId domainId, string containerSetId) => string.Format("{0}/blobstoredomain:{1}/{2}/dedups/zip", (object) "contentstitcher", (object) domainId, (object) containerSetId);
  }
}
