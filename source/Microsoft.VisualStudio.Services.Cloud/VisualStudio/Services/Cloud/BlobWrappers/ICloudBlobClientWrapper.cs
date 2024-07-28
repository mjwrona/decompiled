// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.BlobWrappers.ICloudBlobClientWrapper
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Azure.Storage.Shared.Protocol;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Cloud.BlobWrappers
{
  public interface ICloudBlobClientWrapper
  {
    ICloudBlobContainerWrapper GetContainerReference(string containerName);

    IContainerResultSegmentWrapper ListContainersSegmented(
      string prefix,
      ContainerListingDetails detailsIncluded,
      int? maxResults,
      BlobContinuationToken currentToken,
      BlobRequestOptions options = null,
      OperationContext operationContext = null);

    Task<IContainerResultSegmentWrapper> ListContainersSegmentedAsync(
      string prefix,
      ContainerListingDetails detailsIncluded,
      int? maxResults,
      BlobContinuationToken currentToken,
      BlobRequestOptions options = null,
      OperationContext operationContext = null,
      CancellationToken cancellationToken = default (CancellationToken));

    AccountProperties GetAccountProperties(BlobRequestOptions options = null);

    ServiceStats GetServiceStats(BlobRequestOptions options = null);
  }
}
