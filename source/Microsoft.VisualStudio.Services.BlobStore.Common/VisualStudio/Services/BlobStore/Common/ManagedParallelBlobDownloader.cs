// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Common.ManagedParallelBlobDownloader
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FAFB0281-5CF2-4D3F-992C-49FBB9BEC906
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.BlobStore.Common.dll

using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Azure.Storage.RetryPolicies;
using Microsoft.VisualStudio.Services.Content.Common.Tracing;
using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.BlobStore.Common
{
  public class ManagedParallelBlobDownloader : DefaultDownloader
  {
    private readonly OperationContext operationContext;

    public ManagedParallelBlobDownloader(
      ParallelHttpDownload.DownloadConfiguration configuration,
      IAppTraceSource tracer,
      Guid correlationId,
      HttpClient httpClient = null)
      : base(configuration, tracer, correlationId, httpClient)
    {
      this.operationContext = new OperationContext()
      {
        ClientRequestID = correlationId.ToString()
      };
      new AzureStorageOperationTraceAdapter(tracer).AttachTracing(this.operationContext);
    }

    protected override bool GetStreamViaAzureSdk(Uri downloadUri) => downloadUri.Host.EndsWith("windows.net", StringComparison.OrdinalIgnoreCase) & Environment.GetEnvironmentVariable("VSTS_BLOB_USE_HTTPCLIENT") == "0";

    protected override Task<Stream> GetStreamThroughAzureBlobs(
      Uri azureUri,
      int? overrideStreamMinimumReadSizeInBytes,
      TimeSpan? requestTimeout,
      CancellationToken cancellationToken)
    {
      CloudBlockBlob cloudBlockBlob1 = new CloudBlockBlob(azureUri);
      if (overrideStreamMinimumReadSizeInBytes.HasValue)
        cloudBlockBlob1.StreamMinimumReadSizeInBytes = overrideStreamMinimumReadSizeInBytes.Value;
      CloudBlockBlob cloudBlockBlob2 = cloudBlockBlob1;
      BlobRequestOptions options = new BlobRequestOptions();
      options.MaximumExecutionTime = requestTimeout;
      options.RetryPolicy = (IRetryPolicy) new ExponentialRetry(TimeSpan.FromSeconds(4.0), this.DefaultRetryCount);
      options.ServerTimeout = new TimeSpan?();
      OperationContext operationContext = this.operationContext;
      CancellationToken cancellationToken1 = cancellationToken;
      return cloudBlockBlob2.OpenReadAsync((AccessCondition) null, options, operationContext, cancellationToken1);
    }
  }
}
