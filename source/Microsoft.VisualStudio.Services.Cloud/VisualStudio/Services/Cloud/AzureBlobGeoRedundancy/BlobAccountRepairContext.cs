// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.AzureBlobGeoRedundancy.BlobAccountRepairContext
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
using System.Linq;
using System.Threading;

namespace Microsoft.VisualStudio.Services.Cloud.AzureBlobGeoRedundancy
{
  internal class BlobAccountRepairContext : IBlobCopyContext
  {
    private string m_currentContainerName;
    private CloudBlobClient m_sourceClient;
    private CloudBlobClient m_targetClient;
    private BlobGeoRedundancyEndpoint m_sourceEndpoint;
    private BlobGeoRedundancyEndpoint m_targetEndpoint;
    private readonly AzureBlobGeoRedundancyConsistencySettings m_settings;
    private readonly BlockingCollection<IRepairOperation> m_operations;
    private readonly BlobRepairStats m_stats;
    private readonly ThreadSafeTraceMethod m_traceMethod;
    private static readonly TimeSpan s_defaultMinimumAge = TimeSpan.FromDays(30.0);
    private const string c_area = "AzureBlobGeoRedundancy";
    private const string c_layer = "BlobAccountRepairContext";

    public BlobAccountRepairContext(
      AzureBlobGeoRedundancyConsistencySettings settings,
      BlockingCollection<IRepairOperation> operations,
      BlobRepairStats stats,
      ThreadSafeTraceMethod traceMethod)
    {
      this.m_settings = settings;
      this.m_operations = operations;
      this.m_stats = stats;
      this.m_traceMethod = traceMethod;
    }

    public void Initialize(
      IVssRequestContext requestContext,
      CloudStorageAccount sourceAccount,
      CloudStorageAccount targetAccount)
    {
      this.m_sourceClient = sourceAccount.CreateCloudBlobClient();
      this.m_targetClient = targetAccount.CreateCloudBlobClient();
      if (this.m_settings.QueueCopies)
      {
        IAzureBlobGeoRedundancyService service = requestContext.GetService<IAzureBlobGeoRedundancyService>();
        this.m_sourceEndpoint = service.SetupEndpoint(requestContext, sourceAccount);
        this.m_targetEndpoint = service.SetupEndpoint(requestContext, targetAccount);
      }
      else
      {
        this.m_sourceEndpoint = (BlobGeoRedundancyEndpoint) null;
        this.m_targetEndpoint = (BlobGeoRedundancyEndpoint) null;
      }
    }

    public void StartIteration(
      IVssRequestContext requestContext,
      ICloudBlobContainerWrapper sourceContainer,
      ICloudBlobContainerWrapper targetContainer,
      ICloudBlobWrapper sourceBlob,
      ICloudBlobWrapper targetBlob)
    {
    }

    public int Compare(
      IVssRequestContext requestContext,
      ICloudBlobReadOnlyInfo sourceBlob,
      ICloudBlobReadOnlyInfo targetBlob)
    {
      if (!string.Equals(this.m_currentContainerName, sourceBlob.ContainerName, StringComparison.Ordinal))
      {
        this.m_stats.IncrementProcessedContainers();
        this.m_currentContainerName = sourceBlob.ContainerName;
      }
      int num = string.Compare(sourceBlob.ContainerName, targetBlob.ContainerName, StringComparison.Ordinal);
      return num == 0 ? string.Compare(sourceBlob.Name, targetBlob.Name, StringComparison.Ordinal) : num;
    }

    public void OnSourceAndTarget(
      IVssRequestContext requestContext,
      ICloudBlobWrapper sourceBlob,
      ICloudBlobWrapper targetBlob)
    {
      if (this.IsExcluded(sourceBlob))
        return;
      this.m_stats.IncrementProcessedBlobs();
      if (sourceBlob.GetLength() != targetBlob.GetLength())
      {
        this.m_stats.IncrementInconsistentBlobs();
        ThreadSafeTraceMethod traceMethod = this.m_traceMethod;
        if (traceMethod != null)
          traceMethod(15307001, TraceLevel.Error, "AzureBlobGeoRedundancy", nameof (BlobAccountRepairContext), "Blob on both source and target but lengths do not match. Container: {0}, Blob: {1}, SourceLength: {2}, TargetLength: {3}, SourceLastModified: {4}, TargetLastModified: {5}", new object[6]
          {
            (object) sourceBlob.ContainerName,
            (object) sourceBlob.Name,
            (object) sourceBlob.GetLength(),
            (object) targetBlob.GetLength(),
            (object) sourceBlob.GetLastModified(),
            (object) targetBlob.GetLastModified()
          });
      }
      if (!this.m_settings.CheckContentMD5)
        return;
      if (sourceBlob.ContentMD5 == null || targetBlob.ContentMD5 == null)
      {
        ThreadSafeTraceMethod traceMethod = this.m_traceMethod;
        if (traceMethod == null)
          return;
        traceMethod(15307002, TraceLevel.Info, "AzureBlobGeoRedundancy", nameof (BlobAccountRepairContext), "No content MD5 available, validation will be skipped. Container: {0}, Blob: {1}", new object[2]
        {
          (object) sourceBlob.ContainerName,
          (object) sourceBlob.Name
        });
      }
      else
      {
        if (string.Equals(sourceBlob.ContentMD5, targetBlob.ContentMD5, StringComparison.Ordinal))
          return;
        this.m_stats.IncrementInconsistentBlobs();
        ThreadSafeTraceMethod traceMethod = this.m_traceMethod;
        if (traceMethod == null)
          return;
        traceMethod(15307001, TraceLevel.Error, "AzureBlobGeoRedundancy", nameof (BlobAccountRepairContext), "Blob on both source and target but then content MD5 does not match. Container: {0}, Blob: {1}, SourceMD5: {2}, TargetMD5: {3}, SourceLastModified: {4}, TargetLastModified: {5}", new object[6]
        {
          (object) sourceBlob.ContainerName,
          (object) sourceBlob.Name,
          (object) sourceBlob.ContentMD5,
          (object) targetBlob.ContentMD5,
          (object) sourceBlob.GetLastModified(),
          (object) targetBlob.GetLastModified()
        });
      }
    }

    public void OnSourceOnly(IVssRequestContext requestContext, ICloudBlobWrapper sourceBlob)
    {
      if (this.IsExcluded(sourceBlob))
        return;
      this.m_stats.IncrementProcessedBlobs();
      this.m_stats.IncrementBlobsMissingOnSecondary();
      ThreadSafeTraceMethod traceMethod = this.m_traceMethod;
      if (traceMethod != null)
        traceMethod(15307001, (TraceLevel) (this.m_settings.Repair ? 3 : 1), "AzureBlobGeoRedundancy", nameof (BlobAccountRepairContext), "Blob on source only. Container: {0}, Blob: {1}, LastModified: {2}", new object[3]
        {
          (object) sourceBlob.ContainerName,
          (object) sourceBlob.Name,
          (object) sourceBlob.GetLastModified()
        });
      if (!this.m_settings.Repair)
        return;
      if (this.m_sourceEndpoint == null)
      {
        BlockingCollection<IRepairOperation> operations = this.m_operations;
        CopyBlobOperation copyBlobOperation = new CopyBlobOperation();
        copyBlobOperation.ContainerName = sourceBlob.ContainerName;
        copyBlobOperation.BlobName = sourceBlob.Name;
        copyBlobOperation.SourceClient = this.m_sourceClient;
        copyBlobOperation.TargetClient = this.m_targetClient;
        CancellationToken cancellationToken = requestContext.CancellationToken;
        operations.Add((IRepairOperation) copyBlobOperation, cancellationToken);
      }
      else
      {
        BlockingCollection<IRepairOperation> operations = this.m_operations;
        QueueCreateBlobOperation createBlobOperation = new QueueCreateBlobOperation();
        createBlobOperation.ContainerName = sourceBlob.ContainerName;
        createBlobOperation.BlobName = sourceBlob.Name;
        createBlobOperation.Endpoint = this.m_sourceEndpoint;
        CancellationToken cancellationToken = requestContext.CancellationToken;
        operations.Add((IRepairOperation) createBlobOperation, cancellationToken);
      }
    }

    public void OnTargetOnly(IVssRequestContext requestContext, ICloudBlobWrapper targetBlob)
    {
      if (this.IsExcluded(targetBlob))
        return;
      this.m_stats.IncrementProcessedBlobs();
      this.m_stats.IncrementBlobsMissingOnPrimary();
      if (this.m_settings.SyncSource)
      {
        ThreadSafeTraceMethod traceMethod = this.m_traceMethod;
        if (traceMethod != null)
          traceMethod(15307001, (TraceLevel) (this.m_settings.Repair ? 3 : 1), "AzureBlobGeoRedundancy", nameof (BlobAccountRepairContext), "Blob on target only. Container: {0}, Blob: {1}, LastModified: {2}", new object[3]
          {
            (object) targetBlob.ContainerName,
            (object) targetBlob.Name,
            (object) targetBlob.GetLastModified()
          });
      }
      if (!this.m_settings.Repair)
        return;
      if (this.m_settings.SyncSource)
      {
        if (this.m_targetEndpoint == null)
        {
          BlockingCollection<IRepairOperation> operations = this.m_operations;
          CopyBlobOperation copyBlobOperation = new CopyBlobOperation();
          copyBlobOperation.ContainerName = targetBlob.ContainerName;
          copyBlobOperation.BlobName = targetBlob.Name;
          copyBlobOperation.SourceClient = this.m_targetClient;
          copyBlobOperation.TargetClient = this.m_sourceClient;
          CancellationToken cancellationToken = requestContext.CancellationToken;
          operations.Add((IRepairOperation) copyBlobOperation, cancellationToken);
        }
        else
        {
          BlockingCollection<IRepairOperation> operations = this.m_operations;
          QueueCreateBlobOperation createBlobOperation = new QueueCreateBlobOperation();
          createBlobOperation.ContainerName = targetBlob.ContainerName;
          createBlobOperation.BlobName = targetBlob.Name;
          createBlobOperation.Endpoint = this.m_targetEndpoint;
          CancellationToken cancellationToken = requestContext.CancellationToken;
          operations.Add((IRepairOperation) createBlobOperation, cancellationToken);
        }
      }
      else
      {
        if (!this.m_settings.CleanupTarget)
          return;
        TimeSpan timeSpan = this.m_settings.CleanupMinimumAge ?? BlobAccountRepairContext.s_defaultMinimumAge;
        DateTimeOffset? lastModified = targetBlob.GetLastModified();
        if (!lastModified.HasValue || !(lastModified.Value < (DateTimeOffset) (DateTime.UtcNow - timeSpan)))
          return;
        ThreadSafeTraceMethod traceMethod = this.m_traceMethod;
        if (traceMethod != null)
          traceMethod(15307007, TraceLevel.Info, "AzureBlobGeoRedundancy", nameof (BlobAccountRepairContext), "Deleting blob on target. Container: {0}, Blob: {1}, LastModified: {2}", new object[3]
          {
            (object) targetBlob.ContainerName,
            (object) targetBlob.Name,
            (object) lastModified
          });
        BlockingCollection<IRepairOperation> operations = this.m_operations;
        DeleteBlobOperation deleteBlobOperation = new DeleteBlobOperation();
        deleteBlobOperation.ContainerName = targetBlob.ContainerName;
        deleteBlobOperation.BlobName = targetBlob.Name;
        deleteBlobOperation.Client = this.m_targetClient;
        CancellationToken cancellationToken = requestContext.CancellationToken;
        operations.Add((IRepairOperation) deleteBlobOperation, cancellationToken);
      }
    }

    private bool IsExcluded(ICloudBlobWrapper blob)
    {
      if (AzureBlobGeoRedundancyConstants.ExcludedContainers.Contains<string>(blob.ContainerName))
        return true;
      DateTimeOffset? nullable = this.m_settings.ExcludeBlobsBefore;
      if (nullable.HasValue)
      {
        nullable = blob.GetLastModified();
        if (nullable.HasValue)
        {
          nullable = blob.GetLastModified();
          DateTimeOffset dateTimeOffset1 = nullable.Value;
          nullable = this.m_settings.ExcludeBlobsBefore;
          DateTimeOffset dateTimeOffset2 = nullable.Value;
          if (dateTimeOffset1 <= dateTimeOffset2)
            return true;
        }
      }
      return false;
    }
  }
}
