// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.ResolvedCloudBlobContainer
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.VisualStudio.Services.Content.Server.Azure;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype
{
  public class ResolvedCloudBlobContainer : IResolvedCloudBlobContainer
  {
    private readonly ICloudBlobContainer wrappedContainer;

    public ResolvedCloudBlobContainer(
      ICloudBlobContainer wrappedContainer,
      ContainerAddress address)
    {
      this.wrappedContainer = wrappedContainer;
      this.Address = address;
      this.Name = wrappedContainer.Name;
      this.AccountName = wrappedContainer.AccountName;
    }

    public ContainerAddress Address { get; }

    public string Name { get; }

    public string AccountName { get; }

    public async Task<bool> CreateIfNotExistsAsync(
      VssRequestPump.Processor processor,
      BlobContainerPublicAccessType accessType = BlobContainerPublicAccessType.Off,
      BlobRequestOptions options = null,
      OperationContext operationContext = null)
    {
      return await this.wrappedContainer.CreateIfNotExistsAsync(processor, accessType, options, operationContext);
    }

    public async Task<bool> DeleteIfExistsAsync(
      VssRequestPump.Processor processor,
      AccessCondition accessCondition = null,
      BlobRequestOptions options = null,
      OperationContext operationContext = null)
    {
      return await this.wrappedContainer.DeleteIfExistsAsync(processor, accessCondition, options, operationContext);
    }

    public ICloudBlockBlob GetBlockBlobReference(string blobName) => this.wrappedContainer.GetBlockBlobReference(blobName);

    public async Task<IBlobResultSegment> ListBlobsSegmentedAsync(
      VssRequestPump.Processor processor,
      string prefix,
      bool useFlatListing,
      BlobListingDetails blobListingDetails,
      int? maxResults,
      BlobContinuationToken currentToken,
      BlobRequestOptions options = null,
      OperationContext operationContext = null)
    {
      return await this.wrappedContainer.ListBlobsSegmentedAsync(processor, prefix, useFlatListing, blobListingDetails, maxResults, currentToken, options, operationContext);
    }
  }
}
