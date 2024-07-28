// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.Aggregations.PackageNames.NpmNamesAggregationAccessor
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Npm.Server.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Npm.Server.Aggregations.PackageNames
{
  public class NpmNamesAggregationAccessor : 
    INpmNamesAggregationAccessor,
    IAggregationAccessor<NpmNamesAggregation>,
    IAggregationAccessor,
    INpmNamesService,
    IPackageNamesService<NpmPackageName, NpmPackageIdentity>
  {
    private readonly IConverter<IAggregation, Locator> aggVersionToLocatorConverter;
    private readonly IFactory<ContainerAddress, IPackageNamesService<NpmPackageName, NpmPackageIdentity>> nameServiceFactory;
    private readonly IExecutionEnvironment executionEnvironment;

    public NpmNamesAggregationAccessor(
      IConverter<IAggregation, Locator> aggVersionToLocatorConverter,
      IFactory<ContainerAddress, IPackageNamesService<NpmPackageName, NpmPackageIdentity>> nameServiceFactory,
      IExecutionEnvironment executionEnvironment,
      IAggregation aggregation)
    {
      this.aggVersionToLocatorConverter = aggVersionToLocatorConverter;
      this.nameServiceFactory = nameServiceFactory;
      this.executionEnvironment = executionEnvironment;
      this.Aggregation = aggregation;
    }

    public IAggregation Aggregation { get; }

    public async Task<IReadOnlyList<IPackageNameEntry<NpmPackageName>>> GetPackageNamesAsync(
      IFeedRequest feedRequest)
    {
      return await this.GetNameService().GetPackageNamesAsync(feedRequest);
    }

    public async Task ApplyCommitsAsync(
      IFeedRequest feedRequest,
      IReadOnlyList<PackageCommit<NpmPackageIdentity>> packageCommits)
    {
      if (!packageCommits.Any<PackageCommit<NpmPackageIdentity>>())
        return;
      await this.GetNameService().ApplyCommitsAsync(feedRequest, packageCommits);
    }

    public async Task ApplyCommitAsync(
      IFeedRequest feedRequest,
      IReadOnlyList<ICommitLogEntry> commitLogEntries)
    {
      IReadOnlyList<PackageCommit<NpmPackageIdentity>> packageCommitList = this.GatherPackageCommits(feedRequest, commitLogEntries);
      if (!packageCommitList.Any<PackageCommit<NpmPackageIdentity>>())
        return;
      await this.ApplyCommitsAsync(feedRequest, packageCommitList);
    }

    private IReadOnlyList<PackageCommit<NpmPackageIdentity>> GatherPackageCommits(
      IFeedRequest feedRequest,
      IReadOnlyList<ICommitLogEntry> commitLogEntries)
    {
      List<PackageCommit<NpmPackageIdentity>> list = new List<PackageCommit<NpmPackageIdentity>>();
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
      return (IReadOnlyList<PackageCommit<NpmPackageIdentity>>) list;
    }

    private void AddToListIfIsPackageVersionOperation(
      List<PackageCommit<NpmPackageIdentity>> list,
      IFeedRequest feedRequest,
      ICommitLogEntry commitLogEntry)
    {
      if (!(commitLogEntry.CommitOperationData is IPackageVersionOperationData commitOperationData))
        return;
      list.Add(new PackageCommit<NpmPackageIdentity>((IPackageRequest<NpmPackageIdentity>) new PackageRequest<NpmPackageIdentity>(feedRequest, (NpmPackageIdentity) commitOperationData.Identity), commitLogEntry));
    }

    private IPackageNamesService<NpmPackageName, NpmPackageIdentity> GetNameService() => this.nameServiceFactory.Get(new ContainerAddress((CollectionId) this.executionEnvironment.HostId, this.aggVersionToLocatorConverter.Convert(this.Aggregation)));
  }
}
