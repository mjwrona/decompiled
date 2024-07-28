// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamVersionList.UpstreamVersionListAggregationAccessor`2
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.BlobStore.Server.Common;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.DocumentProvider;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamVersionList
{
  public class UpstreamVersionListAggregationAccessor<TPackageName, TPackageVersion> : 
    IAggregationAccessor<UpstreamVersionListAggregation<TPackageName, TPackageVersion>>,
    IAggregationAccessor,
    IUpstreamVersionListService<TPackageName, TPackageVersion>,
    IUpstreamVersionListService
    where TPackageName : IPackageName
    where TPackageVersion : class, IPackageVersion
  {
    private readonly IAggregationDocumentProvider<UpstreamVersionListFile<TPackageVersion>, TPackageName> fileProvider;
    private readonly IFeatureFlagService featureFlagService;
    private readonly IETaggedDocumentUpdater documentUpdater;

    public UpstreamVersionListAggregationAccessor(
      UpstreamVersionListAggregation<TPackageName, TPackageVersion> aggregation,
      IAggregationDocumentProvider<UpstreamVersionListFile<TPackageVersion>, TPackageName> fileProvider,
      IFeatureFlagService featureFlagService,
      IETaggedDocumentUpdater documentUpdater)
    {
      this.fileProvider = fileProvider;
      this.featureFlagService = featureFlagService;
      this.Aggregation = (IAggregation) aggregation;
      this.documentUpdater = documentUpdater;
    }

    public IAggregation Aggregation { get; }

    public async Task<UpstreamVersionListFile<TPackageVersion>> GetUpstreamVersionListDocument(
      IPackageNameRequest<TPackageName> request)
    {
      UpstreamVersionListFile<TPackageVersion> versionListDocument;
      (await this.fileProvider.GetDocumentAsync((IFeedRequest) request, request.PackageName)).Deconstruct<UpstreamVersionListFile<TPackageVersion>>(out versionListDocument, out string _);
      return versionListDocument;
    }

    async Task<IUpstreamVersionListFile<IPackageVersion>> IUpstreamVersionListService.GetGenericUpstreamVersionListDocument(
      IPackageNameRequest<IPackageName> request)
    {
      UpstreamVersionListFile<TPackageVersion> versionListDocument;
      (await this.fileProvider.GetDocumentAsync((IFeedRequest) request, (TPackageName) request.PackageName)).Deconstruct<UpstreamVersionListFile<TPackageVersion>>(out versionListDocument, out string _);
      return (IUpstreamVersionListFile<IPackageVersion>) versionListDocument;
    }

    public async Task ApplyCommitAsync(
      IFeedRequest feedRequest,
      IReadOnlyList<ICommitLogEntry> commitLogEntries)
    {
      if (!this.featureFlagService.IsEnabled("Packaging.WriteCachedUpstreamVersionLists"))
        return;
      List<IGrouping<IPackageName, IUpdateUpstreamMetadataOperationData<TPackageName, TPackageVersion>>> list = commitLogEntries.Select<ICommitLogEntry, IUpdateUpstreamMetadataOperationData<TPackageName, TPackageVersion>>((Func<ICommitLogEntry, IUpdateUpstreamMetadataOperationData<TPackageName, TPackageVersion>>) (x => x.CommitOperationData as IUpdateUpstreamMetadataOperationData<TPackageName, TPackageVersion>)).Where<IUpdateUpstreamMetadataOperationData<TPackageName, TPackageVersion>>((Func<IUpdateUpstreamMetadataOperationData<TPackageName, TPackageVersion>, bool>) (x => x != null)).GroupBy<IUpdateUpstreamMetadataOperationData<TPackageName, TPackageVersion>, IPackageName>((Func<IUpdateUpstreamMetadataOperationData<TPackageName, TPackageVersion>, IPackageName>) (x => (IPackageName) x.PackageName), (IEqualityComparer<IPackageName>) PackageNameComparer.NormalizedName).ToList<IGrouping<IPackageName, IUpdateUpstreamMetadataOperationData<TPackageName, TPackageVersion>>>();
      if (!list.Any<IGrouping<IPackageName, IUpdateUpstreamMetadataOperationData<TPackageName, TPackageVersion>>>())
        return;
      foreach (IGrouping<IPackageName, IUpdateUpstreamMetadataOperationData<TPackageName, TPackageVersion>> source in list)
        await this.ApplyOnePackageName(feedRequest.WithPackageName<TPackageName>((TPackageName) source.Key), (IReadOnlyList<IUpdateUpstreamMetadataOperationData<TPackageName, TPackageVersion>>) source.ToList<IUpdateUpstreamMetadataOperationData<TPackageName, TPackageVersion>>());
    }

    private async Task ApplyOnePackageName(
      IPackageNameRequest<TPackageName> packageNameRequest,
      IReadOnlyList<IUpdateUpstreamMetadataOperationData<TPackageName, TPackageVersion>> opDatas)
    {
      (EtagValue<UpstreamVersionListFile<TPackageVersion>>, bool) valueTuple = await this.documentUpdater.UpdateETaggedDocumentAsync<UpstreamVersionListFile<TPackageVersion>, TPackageName>(new EtagValue<UpstreamVersionListFile<TPackageVersion>>?(), this.fileProvider, (IFeedRequest) packageNameRequest, packageNameRequest.PackageName, (Func<UpstreamVersionListFile<TPackageVersion>, (UpstreamVersionListFile<TPackageVersion>, bool)>) (doc =>
      {
        bool flag = false;
        foreach (IUpdateUpstreamMetadataOperationData<TPackageName, TPackageVersion> opData in (IEnumerable<IUpdateUpstreamMetadataOperationData<TPackageName, TPackageVersion>>) opDatas)
        {
          int count = doc.Upstreams.Count;
          doc = doc.WithoutDataOlderThan(opData.MinimumUpstreamVersionListDate);
          if (doc.Upstreams.Count != count)
            flag = true;
          foreach (UpstreamVersionListFileUpstream<TPackageVersion> upstreamInfo in opData.NewUpstreamVersionListsToCache)
          {
            doc = doc.WithAddedOrUpdatedUpstream(upstreamInfo);
            flag = true;
          }
        }
        return (doc, flag);
      }), (Func<string>) (() => string.Format("Could not apply metadata changes to feed: {0}, package name: {1}.", (object) packageNameRequest.Feed.Id, (object) packageNameRequest.PackageName)));
    }
  }
}
