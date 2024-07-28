// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadataAggregationAccessor`2
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


#nullable enable
namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared
{
  public abstract class PackageMetadataAggregationAccessor<TPackageIdentity, TMetadataEntry> : 
    IAggregationAccessor,
    IMetadataDocumentService<
    #nullable disable
    TPackageIdentity, TMetadataEntry>,
    IMetadataService<
    #nullable enable
    TPackageIdentity, TMetadataEntry>,
    IReadMetadataService<TPackageIdentity, TMetadataEntry>,
    IReadSingleVersionMetadataService<TPackageIdentity, TMetadataEntry>,
    IReadMetadataDocumentService<TPackageIdentity, TMetadataEntry>,
    IReadMetadataDocumentService
    where TPackageIdentity : 
    #nullable disable
    IPackageIdentity
    where TMetadataEntry : class, IMetadataEntry<TPackageIdentity>
  {
    private readonly IConverter<IAggregation, Locator> aggVersionToLocatorConverter;
    private readonly IFactory<ContainerAddress, IMetadataDocumentService<TPackageIdentity, TMetadataEntry>> metadataServiceFactory;
    private readonly IExecutionEnvironment executionEnvironment;

    public PackageMetadataAggregationAccessor(
      IAggregation aggregation,
      IConverter<IAggregation, Locator> aggVersionToLocatorConverter,
      IFactory<ContainerAddress, IMetadataDocumentService<TPackageIdentity, TMetadataEntry>> metadataServiceFactory,
      IExecutionEnvironment executionEnvironment)
    {
      this.Aggregation = aggregation;
      this.aggVersionToLocatorConverter = aggVersionToLocatorConverter;
      this.metadataServiceFactory = metadataServiceFactory;
      this.executionEnvironment = executionEnvironment;
    }

    public IAggregation Aggregation { get; }

    public async Task ApplyCommitAsync(
      IFeedRequest feedRequest,
      IReadOnlyList<ICommitLogEntry> commitLogEntries)
    {
      IReadOnlyList<PackageCommit<TPackageIdentity>> packageCommits = this.GatherPackageCommits(feedRequest, commitLogEntries);
      IReadOnlyList<PackageNameCommit<IPackageName>> list = (IReadOnlyList<PackageNameCommit<IPackageName>>) commitLogEntries.Where<ICommitLogEntry>((Func<ICommitLogEntry, bool>) (x => x.CommitOperationData is IPackageOperationData && !(x.CommitOperationData is IPackageVersionOperationData))).Select<ICommitLogEntry, PackageNameCommit<IPackageName>>((Func<ICommitLogEntry, PackageNameCommit<IPackageName>>) (x => new PackageNameCommit<IPackageName>(new PackageNameRequest<IPackageName>(feedRequest, ((IPackageOperationData) x.CommitOperationData).PackageName), x))).ToList<PackageNameCommit<IPackageName>>();
      await this.GetMetadataService().ApplyCommitsAsync(packageCommits, list);
    }

    private IReadOnlyList<PackageCommit<TPackageIdentity>> GatherPackageCommits(
      IFeedRequest feedRequest,
      IReadOnlyList<ICommitLogEntry> commitLogEntries)
    {
      List<PackageCommit<TPackageIdentity>> list = new List<PackageCommit<TPackageIdentity>>();
      foreach (ICommitLogEntry commitLogEntry in (IEnumerable<ICommitLogEntry>) commitLogEntries)
      {
        this.AddToListIfIsPackageVersionOperation(list, feedRequest, commitLogEntry);
        if (commitLogEntry.CommitOperationData is IBatchCommitOperationData commitOperationData)
        {
          foreach (ICommitOperationData operation in commitOperationData.Operations)
          {
            ICommitLogEntry internalOperation = AggregationAccessorCommonUtils.GetCommitLogEntryForInternalOperation(commitLogEntry, operation);
            this.AddToListIfIsPackageVersionOperation(list, feedRequest, internalOperation);
          }
        }
      }
      return (IReadOnlyList<PackageCommit<TPackageIdentity>>) list;
    }

    public Task ApplyCommitsAsync(
      IReadOnlyList<PackageCommit<TPackageIdentity>> packageCommits,
      IReadOnlyList<PackageNameCommit<IPackageName>> packageNameCommits)
    {
      return this.GetMetadataService().ApplyCommitsAsync(packageCommits, packageNameCommits);
    }

    public Task<TMetadataEntry> GetPackageVersionStateAsync(
      IPackageRequest<TPackageIdentity> packageRequest)
    {
      return this.GetMetadataService().GetPackageVersionStateAsync(packageRequest);
    }

    public Task<List<TMetadataEntry>> GetPackageVersionStatesAsync(
      PackageNameQuery<TMetadataEntry> packageNameQueryRequest)
    {
      return this.GetMetadataService().GetPackageVersionStatesAsync(packageNameQueryRequest);
    }

    public Task<MetadataDocument<TMetadataEntry>> GetPackageVersionStatesDocumentAsync(
      PackageNameQuery<TMetadataEntry> packageNameRequest)
    {
      return this.GetMetadataService().GetPackageVersionStatesDocumentAsync(packageNameRequest);
    }

    private void AddToListIfIsPackageVersionOperation(
      List<PackageCommit<TPackageIdentity>> list,
      IFeedRequest feedRequest,
      ICommitLogEntry commitLogEntry)
    {
      if (!(commitLogEntry.CommitOperationData is IPackageVersionOperationData commitOperationData))
        return;
      list.Add(new PackageCommit<TPackageIdentity>((IPackageRequest<TPackageIdentity>) new PackageRequest<TPackageIdentity>(feedRequest, (TPackageIdentity) commitOperationData.Identity), commitLogEntry));
    }

    private IMetadataDocumentService<TPackageIdentity, TMetadataEntry> GetMetadataService() => this.metadataServiceFactory.Get(new ContainerAddress((CollectionId) this.executionEnvironment.HostId, this.aggVersionToLocatorConverter.Convert(this.Aggregation)));

    public Task<MetadataDocument<IMetadataEntry>> GetGenericUnfilteredPackageVersionStatesDocumentWithoutRefreshAsync(
      IPackageNameRequest packageNameRequest)
    {
      return this.GetMetadataService().GetGenericUnfilteredPackageVersionStatesDocumentWithoutRefreshAsync(packageNameRequest);
    }
  }
}
