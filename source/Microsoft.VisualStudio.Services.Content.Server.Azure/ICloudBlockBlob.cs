// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Azure.ICloudBlockBlob
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Azure, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7823E4AE-BEB6-4A7C-9914-276DEAE1FB1F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Azure.dll

using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Content.Server.Azure
{
  public interface ICloudBlockBlob : IListBlobItem
  {
    BlobProperties Properties { get; }

    string Name { get; }

    IDictionary<string, string> Metadata { get; }

    ICloudBlobContainer GetContainer();

    [DoesServiceRequest]
    string GetSharedAccessSignature(
      SharedAccessBlobPolicy policy,
      SharedAccessBlobHeaders headers,
      string policyName);

    [DoesServiceRequest]
    Task PutBlockByteArrayAsync(
      VssRequestPump.Processor processor,
      string blockId,
      ArraySegment<byte> blockData,
      OperationContext operationContext = null);

    [DoesServiceRequest]
    Task PutBlockStreamAsync(
      VssRequestPump.Processor processor,
      string blockId,
      Stream blockData,
      OperationContext operationContext = null);

    [DoesServiceRequest]
    Task PutBlockListAsync(
      VssRequestPump.Processor processor,
      IEnumerable<string> blockList,
      [Optional] AccessCondition accessCondition,
      [Optional] BlobRequestOptions options,
      [Optional] OperationContext operationContext);

    [DoesServiceRequest]
    Task<bool> DeleteIfExistsAsync(
      VssRequestPump.Processor processor,
      [Optional] DeleteSnapshotsOption deleteSnapshotsOption,
      [Optional] AccessCondition accessCondition,
      [Optional] BlobRequestOptions options,
      [Optional] OperationContext operationContext);

    [DoesServiceRequest]
    Task<bool> UndeleteAsync(
      VssRequestPump.Processor processor,
      [Optional] AccessCondition accessCondition,
      [Optional] BlobRequestOptions options,
      [Optional] OperationContext operationContext);

    [DoesServiceRequest]
    Task<IEnumerable<ListBlockItem>> DownloadBlockListAsync(
      VssRequestPump.Processor processor,
      [Optional] BlockListingFilter blockListingFilter,
      [Optional] AccessCondition accessCondition,
      [Optional] BlobRequestOptions options,
      [Optional] OperationContext operationContext);

    [DoesServiceRequest]
    Task<Stream> OpenReadNeedsRetryAsync(
      VssRequestPump.Processor processor,
      AccessCondition accessCondition = null,
      BlobRequestOptions options = null,
      OperationContext operationContext = null);

    [DoesServiceRequest]
    Task DownloadToStreamNeedsRetryAsync(
      VssRequestPump.Processor processor,
      Stream target,
      AccessCondition accessCondition = null,
      BlobRequestOptions options = null,
      OperationContext operationContext = null);

    [DoesServiceRequest]
    Task<bool> ExistsAsync(
      VssRequestPump.Processor processor,
      BlobRequestOptions options = null,
      OperationContext operationContext = null);

    [DoesServiceRequest]
    Task UploadFromByteArrayAsync(
      VssRequestPump.Processor processor,
      ArraySegment<byte> buffer,
      bool useHttpClient,
      AccessCondition accessCondition = null,
      BlobRequestOptions options = null,
      OperationContext operationContext = null);

    [DoesServiceRequest]
    Task FetchAttributesAsync(
      VssRequestPump.Processor processor,
      AccessCondition accessCondition = null,
      BlobRequestOptions options = null,
      OperationContext operationContext = null);

    [DoesServiceRequest]
    Task SetMetadataAsync(
      VssRequestPump.Processor processor,
      AccessCondition accessCondition = null,
      BlobRequestOptions options = null,
      OperationContext operationContext = null);
  }
}
