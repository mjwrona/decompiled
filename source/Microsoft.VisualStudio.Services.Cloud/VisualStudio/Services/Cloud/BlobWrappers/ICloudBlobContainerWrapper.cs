// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.BlobWrappers.ICloudBlobContainerWrapper
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using System;

namespace Microsoft.VisualStudio.Services.Cloud.BlobWrappers
{
  public interface ICloudBlobContainerWrapper
  {
    IBlobResultSegmentWrapper ListBlobsSegmented(
      string prefix,
      bool useFlatBlobListing,
      BlobListingDetails blobListingDetails,
      int? maxResults,
      BlobContinuationToken currentToken,
      BlobRequestOptions options,
      OperationContext operationContext);

    ICloudBlobWrapper GetBlockBlobReference(string blobName);

    ICloudBlobWrapper GetPageBlobReference(string blobName, long length);

    ICloudBlobWrapper GetBlobReferenceFromServer(
      string blobName,
      AccessCondition accessCondition = null,
      BlobRequestOptions options = null,
      OperationContext operationContext = null);

    Uri GetUriWithCredentials(ICloudBlobWrapper blob);

    void CreateIfNotExists(BlobContainerPermissions permissions);

    BlobContainerPermissions GetPermissions();

    bool Exists();

    bool UpdateCredentials();

    string Name { get; }

    Uri Uri { get; }

    string StorageAccountName { get; }
  }
}
