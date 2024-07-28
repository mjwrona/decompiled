// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.Blob.BlobSetTierBatchOperation
// Assembly: Microsoft.Azure.Storage.Blob, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: A04A3512-352A-442F-A95B-BC1B94EF8840
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.Blob.dll

using Microsoft.Azure.Storage.Core.Util;

namespace Microsoft.Azure.Storage.Blob
{
  public sealed class BlobSetTierBatchOperation : BatchOperation
  {
    public void AddSubOperation(
      CloudBlockBlob blockBlob,
      StandardBlobTier standardBlobTier,
      AccessCondition accessCondition = null,
      BlobRequestOptions blobRequestOptions = null)
    {
      CommonUtility.AssertInBounds<int>("operationCount", this.Operations.Count, 0, (int) byte.MaxValue);
      CommonUtility.AssertNotNull(nameof (blockBlob), (object) blockBlob);
      CommonUtility.AssertNotNull(nameof (standardBlobTier), (object) standardBlobTier);
      this.Operations.Add(blockBlob.SetStandardBlobTierImpl(standardBlobTier, new RehydratePriority?(), accessCondition, blobRequestOptions ?? BlobRequestOptions.BaseDefaultRequestOptions));
    }

    public void AddSubOperation(
      CloudPageBlob pageBlob,
      PremiumPageBlobTier premiumPageBlobTier,
      AccessCondition accessCondition = null,
      BlobRequestOptions blobRequestOptions = null)
    {
      CommonUtility.AssertInBounds<int>("operationCount", this.Operations.Count, 0, (int) byte.MaxValue);
      CommonUtility.AssertNotNull(nameof (pageBlob), (object) pageBlob);
      CommonUtility.AssertNotNull(nameof (premiumPageBlobTier), (object) premiumPageBlobTier);
      this.Operations.Add(pageBlob.SetBlobTierImpl(premiumPageBlobTier, blobRequestOptions ?? BlobRequestOptions.BaseDefaultRequestOptions));
    }
  }
}
