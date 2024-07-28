// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.BlobWrappers.CloudBlobClientWrapper
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
  public class CloudBlobClientWrapper : ICloudBlobClientWrapper
  {
    public CloudBlobClient m_client;

    public CloudBlobClientWrapper(CloudBlobClient client) => this.m_client = client;

    public ICloudBlobContainerWrapper GetContainerReference(string containerName) => (ICloudBlobContainerWrapper) new CloudBlobContainerWrapper(this.m_client.GetContainerReference(containerName));

    public IContainerResultSegmentWrapper ListContainersSegmented(
      string prefix,
      ContainerListingDetails detailsIncluded,
      int? maxResults,
      BlobContinuationToken currentToken,
      BlobRequestOptions options = null,
      OperationContext operationContext = null)
    {
      return (IContainerResultSegmentWrapper) new ContainerResultSegmentWrapper(this.m_client.ListContainersSegmented(prefix, detailsIncluded, maxResults, currentToken, options, operationContext));
    }

    public async Task<IContainerResultSegmentWrapper> ListContainersSegmentedAsync(
      string prefix,
      ContainerListingDetails detailsIncluded,
      int? maxResults,
      BlobContinuationToken currentToken,
      BlobRequestOptions options = null,
      OperationContext operationContext = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return (IContainerResultSegmentWrapper) new ContainerResultSegmentWrapper(await this.m_client.ListContainersSegmentedAsync(prefix, detailsIncluded, maxResults, currentToken, options, operationContext, cancellationToken));
    }

    public AccountProperties GetAccountProperties(BlobRequestOptions options = null) => this.m_client.GetAccountProperties(options);

    public ServiceStats GetServiceStats(BlobRequestOptions options = null) => this.m_client.GetServiceStats(options);
  }
}
