// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.AzureBlobGeoRedundancy.AzureBlobGeoRedundancyConsistencyService
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Cloud.BlobWrappers;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Cloud.AzureBlobGeoRedundancy
{
  public class AzureBlobGeoRedundancyConsistencyService : 
    IAzureBlobGeoRedundancyConsistencyService,
    IVssFrameworkService
  {
    internal static readonly string s_baseRegistryPath = "/Service/AzureBlobGeoRedundancy/Settings/ConsistencyChecker";
    internal static readonly string s_statusUpdateIntervalRegistryPath = AzureBlobGeoRedundancyConsistencyService.s_baseRegistryPath + "/StatusUpdateInterval";
    internal static readonly string s_containerSegmentSizeRegistryPath = AzureBlobGeoRedundancyConsistencyService.s_baseRegistryPath + "/ContainerSegmentSize";
    internal static readonly string s_blobSegmentSizeRegistryPath = AzureBlobGeoRedundancyConsistencyService.s_baseRegistryPath + "/BlobSegmentSize";
    internal static readonly string s_applyOptimizationsRegistryPath = AzureBlobGeoRedundancyConsistencyService.s_baseRegistryPath + "/ApplyOptimizations";
    internal static readonly string s_parallelismRegistryPath = AzureBlobGeoRedundancyConsistencyService.s_baseRegistryPath + "/MaxDegreeOfParallelism";
    private const string c_area = "AzureBlobGeoRedundancy";
    private const string c_layer = "AzureBlobGeoRedundancyConsistencyService";

    public void ServiceStart(IVssRequestContext systemRequestContext) => systemRequestContext.CheckDeploymentRequestContext();

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public BlobRepairStats CheckConsistency(
      IVssRequestContext requestContext,
      GeoRedundantStorageAccountSettings storageAccount,
      AzureBlobGeoRedundancyConsistencySettings settings,
      ThreadSafeTraceMethod traceMethod)
    {
      settings.Validate();
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      TimeSpan timeSpan = service.GetValue<TimeSpan>(requestContext, (RegistryQuery) AzureBlobGeoRedundancyConsistencyService.s_statusUpdateIntervalRegistryPath, TimeSpan.FromMinutes(1.0));
      int containerBatchSize = service.GetValue<int>(requestContext, (RegistryQuery) AzureBlobGeoRedundancyConsistencyService.s_containerSegmentSizeRegistryPath, 5000);
      int blobBatchSize = service.GetValue<int>(requestContext, (RegistryQuery) AzureBlobGeoRedundancyConsistencyService.s_blobSegmentSizeRegistryPath, 5000);
      int num = service.GetValue<bool>(requestContext, (RegistryQuery) AzureBlobGeoRedundancyConsistencyService.s_applyOptimizationsRegistryPath, true) ? 1 : 0;
      int maxDegreeOfParallelism = service.GetValue<int>(requestContext, (RegistryQuery) AzureBlobGeoRedundancyConsistencyService.s_parallelismRegistryPath, 32);
      BlobRepairStats stats = new BlobRepairStats();
      string connectionString1 = AzureBlobGeoRedundancyUtils.GetStorageAccountConnectionString(requestContext, storageAccount.DrawerName, storageAccount.PrimaryLookupKey);
      string connectionString2 = AzureBlobGeoRedundancyUtils.GetStorageAccountConnectionString(requestContext, storageAccount.DrawerName, storageAccount.SecondaryLookupKey);
      CloudStorageAccount primaryStorageAccount = CloudStorageAccount.Parse(connectionString1);
      CloudStorageAccount secondaryStorageAccount = CloudStorageAccount.Parse(connectionString2);
      if (num != 0)
      {
        AzureBlobGeoRedundancyUtils.OptimizeBlobServiceEndpoint(requestContext, primaryStorageAccount);
        AzureBlobGeoRedundancyUtils.OptimizeBlobServiceEndpoint(requestContext, secondaryStorageAccount);
        if (settings.QueueCopies)
        {
          AzureBlobGeoRedundancyUtils.OptimizeQueueServiceEndpoint(requestContext, primaryStorageAccount);
          AzureBlobGeoRedundancyUtils.OptimizeQueueServiceEndpoint(requestContext, secondaryStorageAccount);
        }
      }
      using (CancellationTokenSource compositeCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(requestContext.CancellationToken))
      {
        CancellationToken cancellationToken = compositeCancellationTokenSource.Token;
        using (new Timer((TimerCallback) (_ =>
        {
          ThreadSafeTraceMethod threadSafeTraceMethod = traceMethod;
          if (threadSafeTraceMethod == null)
            return;
          threadSafeTraceMethod(15307000, TraceLevel.Info, "AzureBlobGeoRedundancy", nameof (AzureBlobGeoRedundancyConsistencyService), stats.ToString(), Array.Empty<object>());
        }), (object) null, timeSpan, timeSpan))
        {
          using (BlockingCollection<IRepairOperation> operations = new BlockingCollection<IRepairOperation>(5000))
            Task.WaitAll(new Task[2]
            {
              Task.Run((Action) (() =>
              {
                using (IVssRequestContext systemContext = requestContext.ServiceHost.DeploymentServiceHost.CreateSystemContext())
                {
                  try
                  {
                    BlobAccountRepairContext copyContext = new BlobAccountRepairContext(settings, operations, stats, traceMethod);
                    copyContext.Initialize(systemContext, primaryStorageAccount, secondaryStorageAccount);
                    if (settings.ContainerNames == null)
                    {
                      this.EnumerateBlobsInAllContainers(systemContext, primaryStorageAccount, secondaryStorageAccount, copyContext, containerBatchSize, blobBatchSize, traceMethod);
                    }
                    else
                    {
                      foreach (string containerName in settings.ContainerNames)
                        this.EnumerateBlobsInContainer(systemContext, primaryStorageAccount, secondaryStorageAccount, containerName, copyContext, blobBatchSize, traceMethod);
                    }
                  }
                  catch (Exception ex)
                  {
                    compositeCancellationTokenSource.Cancel();
                    throw;
                  }
                  finally
                  {
                    ThreadSafeTraceMethod threadSafeTraceMethod = traceMethod;
                    if (threadSafeTraceMethod != null)
                      threadSafeTraceMethod(15307004, TraceLevel.Info, "AzureBlobGeoRedundancy", nameof (AzureBlobGeoRedundancyConsistencyService), "Finished enumerating blobs in the storage account.", Array.Empty<object>());
                    operations.CompleteAdding();
                  }
                }
              })),
              Task.Run((Action) (() =>
              {
                ParallelOptions parallelOptions = new ParallelOptions()
                {
                  MaxDegreeOfParallelism = maxDegreeOfParallelism,
                  CancellationToken = cancellationToken
                };
                Parallel.ForEach<IRepairOperation>(operations.GetConsumingEnumerable(cancellationToken), parallelOptions, closure_0 ?? (closure_0 = (Action<IRepairOperation>) (operation =>
                {
                  try
                  {
                    operation.Repair();
                    stats.IncrementSuccessfulRepairs();
                  }
                  catch (Exception ex)
                  {
                    stats.IncrementFailedRepairs();
                    ThreadSafeTraceMethod threadSafeTraceMethod = traceMethod;
                    if (threadSafeTraceMethod == null)
                      return;
                    threadSafeTraceMethod(15307005, TraceLevel.Error, "AzureBlobGeoRedundancy", nameof (AzureBlobGeoRedundancyConsistencyService), "Repair operation failed, will continue processing. Error: {0}", new object[1]
                    {
                      (object) ex
                    });
                  }
                })));
              }))
            }, cancellationToken);
        }
      }
      return stats;
    }

    private void EnumerateBlobsInAllContainers(
      IVssRequestContext requestContext,
      CloudStorageAccount primaryStorageAccount,
      CloudStorageAccount secondaryStorageAccount,
      BlobAccountRepairContext copyContext,
      int containerBatchSize,
      int blobBatchSize,
      ThreadSafeTraceMethod traceMethod)
    {
      using (BlobAccountEnumerator sourceEnumerator = new BlobAccountEnumerator((ICloudStorageAccountWrapper) new CloudStorageAccountWrapper(primaryStorageAccount), BlobListingDetails.None, containerBatchSize, blobBatchSize))
      {
        using (BlobAccountEnumerator targetEnumerator = new BlobAccountEnumerator((ICloudStorageAccountWrapper) new CloudStorageAccountWrapper(secondaryStorageAccount), BlobListingDetails.None, containerBatchSize, blobBatchSize))
          BlobCopyUtil.IterateBlobs(requestContext, (IBlobCopyContext) copyContext, (IVsoBlobEnumerator) sourceEnumerator, (IVsoBlobEnumerator) targetEnumerator);
      }
    }

    private void EnumerateBlobsInContainer(
      IVssRequestContext requestContext,
      CloudStorageAccount primaryStorageAccount,
      CloudStorageAccount secondaryStorageAccount,
      string containerName,
      BlobAccountRepairContext copyContext,
      int blobBatchSize,
      ThreadSafeTraceMethod traceMethod)
    {
      CloudBlobContainer containerReference1 = primaryStorageAccount.CreateCloudBlobClient().GetContainerReference(containerName);
      CloudBlobContainer containerReference2 = secondaryStorageAccount.CreateCloudBlobClient().GetContainerReference(containerName);
      if (!containerReference1.Exists())
      {
        ThreadSafeTraceMethod threadSafeTraceMethod = traceMethod;
        if (threadSafeTraceMethod == null)
          return;
        threadSafeTraceMethod(15307006, TraceLevel.Error, "AzureBlobGeoRedundancy", nameof (AzureBlobGeoRedundancyConsistencyService), "Container does not exist. StorageAccount: {0}, Container: {1}", new object[2]
        {
          (object) primaryStorageAccount.Credentials?.AccountName,
          (object) containerName
        });
      }
      else
      {
        if (!containerReference2.Exists())
          AzureBlobGeoRedundancyUtils.SetupSecondaryContainer(containerReference1, containerReference2, (TraceMethod) ((tracepoint, level, area, layer, format, args) => traceMethod(tracepoint, level, area, layer, format, args)));
        using (BlobContainerEnumerator sourceEnumerator = new BlobContainerEnumerator(requestContext, (ICloudStorageAccountWrapper) new CloudStorageAccountWrapper(primaryStorageAccount), (ICloudBlobContainerWrapper) new CloudBlobContainerWrapper(containerReference1), BlobListingDetails.None, blobBatchSize))
        {
          using (BlobContainerEnumerator targetEnumerator = new BlobContainerEnumerator(requestContext, (ICloudStorageAccountWrapper) new CloudStorageAccountWrapper(secondaryStorageAccount), (ICloudBlobContainerWrapper) new CloudBlobContainerWrapper(containerReference2), BlobListingDetails.None, blobBatchSize))
            BlobCopyUtil.IterateBlobs(requestContext, (IBlobCopyContext) copyContext, (IVsoBlobEnumerator) sourceEnumerator, (IVsoBlobEnumerator) targetEnumerator);
        }
      }
    }
  }
}
