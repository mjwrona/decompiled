// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Azure.AzureBlobContainerFactory
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Azure, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7823E4AE-BEB6-4A7C-9914-276DEAE1FB1F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Azure.dll

using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Azure.Storage.RetryPolicies;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Content.Server.Azure
{
  public class AzureBlobContainerFactory : AzureBlobContainerFactoryBasic
  {
    private readonly string azureContainerNamePrefix;
    private readonly IRetryPolicy retryPolicy;
    private readonly TimeSpan maximumExecutionTime;

    public AzureBlobContainerFactory(
      StrongBoxConnectionString accountConnectionString,
      string azureContainerNamePrefix,
      string locationMode,
      int maxConnectionsPerProc)
      : this(accountConnectionString, azureContainerNamePrefix, locationMode, maxConnectionsPerProc, (IRetryPolicy) null)
    {
    }

    public AzureBlobContainerFactory(
      StrongBoxConnectionString accountConnectionString,
      string azureContainerNamePrefix,
      string locationMode,
      int maxConnectionsPerProc,
      IRetryPolicy overrideRetryPolicy,
      TimeSpan? maximumExecutionTime = null)
      : base(accountConnectionString, locationMode, maxConnectionsPerProc)
    {
      this.azureContainerNamePrefix = azureContainerNamePrefix;
      this.retryPolicy = overrideRetryPolicy ?? (IRetryPolicy) new ExponentialRetry(TimeSpan.FromMilliseconds(100.0), 10);
      this.maximumExecutionTime = maximumExecutionTime ?? TimeSpan.FromSeconds(3600.0);
    }

    public override ICloudBlobContainer CreateContainerReference(
      string partitionName,
      bool enableTracing)
    {
      return base.CreateContainerReference(this.GetContainerName(partitionName), enableTracing);
    }

    public IConcurrentIterator<ICloudBlobContainer> GetAllContainerReferences(
      CancellationToken cancellationToken)
    {
      return this.GetContainerReferences(string.Empty, cancellationToken);
    }

    public IConcurrentIterator<ICloudBlobContainer> GetContainerReferences(
      string partitionKeyPrefix,
      CancellationToken cancellationToken)
    {
      return (IConcurrentIterator<ICloudBlobContainer>) new ConcurrentIterator<ICloudBlobContainer>(new int?(2000), cancellationToken, (Func<TryAddValueAsyncFunc<ICloudBlobContainer>, CancellationToken, Task>) ((addItemAsync, cancelToken) => this.IterateOverQueryAsync(partitionKeyPrefix, (Func<IEnumerable<CloudBlobContainer>, Task<bool>>) (async segment =>
      {
        bool containerReferences = true;
        foreach (CloudBlobContainer container in segment)
        {
          containerReferences = await addItemAsync((ICloudBlobContainer) new CloudBlobContainerWrapper(container, false)).ConfigureAwait(false);
          if (!containerReferences)
            break;
        }
        return containerReferences;
      }), cancelToken)));
    }

    private async Task IterateOverQueryAsync(
      string partitionKeyPrefix,
      Func<IEnumerable<CloudBlobContainer>, Task<bool>> segmentCallback,
      CancellationToken cancellationToken)
    {
      AzureBlobContainerFactory containerFactory = this;
      ContainerResultSegment segment = (ContainerResultSegment) null;
      do
      {
        segment = await containerFactory.azureBlobClient.Value.ListContainersSegmentedAsync(containerFactory.azureContainerNamePrefix + partitionKeyPrefix, ContainerListingDetails.Metadata, new int?(1000), segment?.ContinuationToken ?? new BlobContinuationToken(), (BlobRequestOptions) null, (OperationContext) null, cancellationToken).ConfigureAwait(false);
        if (!await segmentCallback(segment.Results).ConfigureAwait(false))
          goto label_4;
      }
      while (segment.ContinuationToken != null);
      goto label_2;
label_4:
      segment = (ContainerResultSegment) null;
      return;
label_2:
      segment = (ContainerResultSegment) null;
    }

    protected override CloudBlobClient CreateBlobClient()
    {
      CloudBlobClient blobClient = base.CreateBlobClient();
      blobClient.DefaultRequestOptions.RetryPolicy = this.retryPolicy;
      blobClient.DefaultRequestOptions.MaximumExecutionTime = new TimeSpan?(this.maximumExecutionTime);
      return blobClient;
    }

    private string GetContainerName(string partitionKey) => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}{1}", (object) this.azureContainerNamePrefix, (object) partitionKey);
  }
}
