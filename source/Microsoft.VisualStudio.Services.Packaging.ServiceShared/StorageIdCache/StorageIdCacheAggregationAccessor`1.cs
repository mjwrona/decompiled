// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.StorageIdCache.StorageIdCacheAggregationAccessor`1
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using System.Collections.Generic;
using System.Threading.Tasks;


#nullable enable
namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.StorageIdCache
{
  public class StorageIdCacheAggregationAccessor<TAggregation> : 
    IStorageIdCacheAggregationAccessor<TAggregation>,
    IAggregationAccessor<TAggregation>,
    IAggregationAccessor,
    IMetadataCacheService
    where TAggregation : IAggregation
  {
    private readonly IMetadataCacheService storageIdCache;

    public StorageIdCacheAggregationAccessor(
      TAggregation aggregation,
      IMetadataCacheService storageIdCache)
    {
      this.storageIdCache = storageIdCache;
      this.Aggregation = (IAggregation) aggregation;
    }

    public IAggregation Aggregation { get; }

    public Task ApplyCommitAsync(
      IFeedRequest feedRequest,
      IReadOnlyList<ICommitLogEntry> commitLogEntries)
    {
      foreach (ICommitOperationData batchCommitOperation in (IEnumerable<ICommitOperationData>) StorageIdCacheAggregationAccessor<TAggregation>.ExpandBatchCommitOperations(commitLogEntries))
      {
        if (batchCommitOperation is IDeleteOperationData deleteOperationData)
          this.InvalidatePackageMetadata((IPackageRequest) feedRequest.WithPackage<IPackageIdentity>(deleteOperationData.Identity));
      }
      return Task.CompletedTask;
    }

    public void SetPackageMetadata(
      IPackageFileRequest request,
      ICachablePackageMetadata packageMetadata)
    {
      this.storageIdCache.SetPackageMetadata(request, packageMetadata);
    }

    public void InvalidatePackageMetadata(IPackageRequest request) => this.storageIdCache.InvalidatePackageMetadata(request);

    public bool TryGetPackageMetadata(
      IPackageFileRequest request,
      out ICachablePackageMetadata? packageMetadata)
    {
      return this.storageIdCache.TryGetPackageMetadata(request, out packageMetadata);
    }

    private static IReadOnlyList<ICommitOperationData> ExpandBatchCommitOperations(
      IReadOnlyList<ICommitLogEntry> commitLogEntries)
    {
      List<ICommitOperationData> commitOperationDataList = new List<ICommitOperationData>();
      foreach (ICommitLogEntry commitLogEntry in (IEnumerable<ICommitLogEntry>) commitLogEntries)
      {
        if (commitLogEntry.CommitOperationData is BatchCommitOperationData commitOperationData)
          commitOperationDataList.AddRange(commitOperationData.Operations);
        else
          commitOperationDataList.Add(commitLogEntry.CommitOperationData);
      }
      return (IReadOnlyList<ICommitOperationData>) commitOperationDataList;
    }
  }
}
