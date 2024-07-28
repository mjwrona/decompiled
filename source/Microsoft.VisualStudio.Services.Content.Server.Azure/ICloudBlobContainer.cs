// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Azure.ICloudBlobContainer
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Azure, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7823E4AE-BEB6-4A7C-9914-276DEAE1FB1F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Azure.dll

using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Content.Server.Azure
{
  public interface ICloudBlobContainer
  {
    bool RequiresVssRequestContext { get; }

    string Name { get; }

    string AccountName { get; }

    StorageUri StorageUri { get; }

    Task<IBlobResultSegment> ListBlobsSegmentedAsync(
      VssRequestPump.Processor processor,
      string prefix,
      [Optional] bool useFlatListing,
      [Optional] BlobListingDetails blobListingDetails,
      [Optional] int? maxResults,
      [Optional] BlobContinuationToken currentToken,
      [Optional] BlobRequestOptions options,
      [Optional] OperationContext operationContext);

    ICloudBlockBlob GetBlockBlobReference(string blobName);

    Task<bool> CreateIfNotExistsAsync(
      VssRequestPump.Processor processor,
      BlobContainerPublicAccessType accessType = BlobContainerPublicAccessType.Off,
      BlobRequestOptions options = null,
      OperationContext operationContext = null);

    Task<bool> DeleteIfExistsAsync(
      VssRequestPump.Processor processor,
      AccessCondition accessCondition = null,
      BlobRequestOptions options = null,
      OperationContext operationContext = null);

    string GetSharedAccessSignature(
      VssRequestPump.Processor processor,
      SharedAccessBlobPolicy policy,
      string policyName);

    Task<BlobContainerPermissions> GetPermissionsAsync(
      VssRequestPump.Processor processor,
      [Optional] OperationContext operationContext);

    Task SetPermissionsAsync(
      VssRequestPump.Processor processor,
      BlobContainerPermissions permissions,
      [Optional] OperationContext operationContext);
  }
}
