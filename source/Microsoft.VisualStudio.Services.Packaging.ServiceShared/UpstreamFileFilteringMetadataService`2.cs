// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamFileFilteringMetadataService`2
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Settings;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamCache.V2;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;


#nullable enable
namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared
{
  public class UpstreamFileFilteringMetadataService<TPackageIdentity, TMetadataEntry> : 
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
    where TMetadataEntry : class, IMetadataEntry<TPackageIdentity>, IPackageFiles, IMetadataEntryFilesUpdater<TMetadataEntry>
  {
    private readonly IMetadataDocumentService<TPackageIdentity, TMetadataEntry> innerMetadataService;
    private readonly IUpstreamEntriesValidChecker upstreamEntriesValidChecker;
    private readonly IFeedPerms feedPermsFacade;
    private readonly IExecutionEnvironment executionEnvironment;
    private readonly IOrgLevelPackagingSetting<bool> allowUpstreamsForPublicFeedsSettings;
    private readonly IOrgLevelPackagingSetting<bool> allowUpstreamsForPublicFeedsMSFTSettings;

    public UpstreamFileFilteringMetadataService(
      IMetadataDocumentService<TPackageIdentity, TMetadataEntry> innerMetadataService,
      IUpstreamEntriesValidChecker upstreamEntriesValidChecker,
      IFeedPerms feedPermsFacade,
      IExecutionEnvironment executionEnvironment,
      IOrgLevelPackagingSetting<bool> allowUpstreamsForPublicFeedsSettings,
      IOrgLevelPackagingSetting<bool> allowUpstreamsForPublicFeedsMSFTSettings)
    {
      this.innerMetadataService = innerMetadataService;
      this.upstreamEntriesValidChecker = upstreamEntriesValidChecker;
      this.feedPermsFacade = feedPermsFacade;
      this.executionEnvironment = executionEnvironment;
      this.allowUpstreamsForPublicFeedsSettings = allowUpstreamsForPublicFeedsSettings;
      this.allowUpstreamsForPublicFeedsMSFTSettings = allowUpstreamsForPublicFeedsMSFTSettings;
    }

    public Task ApplyCommitsAsync(
      IReadOnlyList<PackageCommit<TPackageIdentity>> packageCommits,
      IReadOnlyList<PackageNameCommit<IPackageName>> packageNameCommits)
    {
      return this.innerMetadataService.ApplyCommitsAsync(packageCommits, packageNameCommits);
    }

    public async Task<TMetadataEntry> GetPackageVersionStateAsync(
      IPackageRequest<TPackageIdentity> packageRequest)
    {
      MetadataDocument<TMetadataEntry> statesDocumentAsync = await this.GetPackageVersionStatesDocumentAsync(packageRequest.ToSingleVersionPackageNameQuery<TPackageIdentity, TMetadataEntry>());
      return statesDocumentAsync != null ? statesDocumentAsync.Entries.FirstOrDefault<TMetadataEntry>() : default (TMetadataEntry);
    }

    public async Task<List<TMetadataEntry>> GetPackageVersionStatesAsync(
      PackageNameQuery<TMetadataEntry> packageNameQueryRequest)
    {
      return (await this.GetPackageVersionStatesDocumentAsync(packageNameQueryRequest))?.Entries ?? new List<TMetadataEntry>();
    }

    public async Task<MetadataDocument<TMetadataEntry>> GetPackageVersionStatesDocumentAsync(
      PackageNameQuery<TMetadataEntry> packageNameQueryRequest)
    {
      UpstreamFileFilteringMetadataService<TPackageIdentity, TMetadataEntry>.EnsurePackageFilesProjected(packageNameQueryRequest);
      MetadataDocument<TMetadataEntry> statesDocumentAsync = await this.innerMetadataService.GetPackageVersionStatesDocumentAsync(packageNameQueryRequest);
      return statesDocumentAsync != null ? new MetadataDocument<TMetadataEntry>(this.FilterEntries((IFeedRequest) packageNameQueryRequest.RequestData, statesDocumentAsync), statesDocumentAsync.Properties) : (MetadataDocument<TMetadataEntry>) null;
    }

    private static void EnsurePackageFilesProjected(
      PackageNameQuery<TMetadataEntry> packageNameQueryRequest)
    {
      List<string> projections = packageNameQueryRequest?.Options?.GetProjections();
      if (projections == null || !projections.Any<string>())
        return;
      packageNameQueryRequest.Options = packageNameQueryRequest.Options.OnlyProjecting((Expression<Func<TMetadataEntry, object>>) (x => x.PackageFiles));
    }

    private List<TMetadataEntry> FilterEntries(
      IFeedRequest feedRequest,
      MetadataDocument<TMetadataEntry> doc)
    {
      List<TMetadataEntry> source = doc.Entries;
      if (source.Count == 0)
        return source;
      bool flag1 = this.feedPermsFacade.HasPermissions(feedRequest.Feed, FeedPermissionConstants.AddUpstreamPackage);
      int num1 = this.allowUpstreamsForPublicFeedsSettings.Get() ? 1 : (!this.allowUpstreamsForPublicFeedsMSFTSettings.Get() ? 0 : (this.executionEnvironment.IsCollectionInMicrosoftTenant() ? 1 : 0));
      bool flag2 = ((num1 == 0 ? 0 : (this.executionEnvironment.IsUnauthenticatedWebRequest() ? 1 : 0)) | (flag1 ? 1 : 0)) != 0;
      int num2 = this.upstreamEntriesValidChecker.IsUpstreamInfoValid(feedRequest, (IMetadataDocument) doc) ? 1 : 0;
      bool flag3 = feedRequest.Feed.GetSourcesForProtocol(feedRequest.Protocol).Count<UpstreamSource>() > 0;
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      List<TMetadataEntry> list = source.Where<TMetadataEntry>(UpstreamFileFilteringMetadataService<TPackageIdentity, TMetadataEntry>.\u003C\u003EO.\u003C0\u003E__EntryHasLocalContent ?? (UpstreamFileFilteringMetadataService<TPackageIdentity, TMetadataEntry>.\u003C\u003EO.\u003C0\u003E__EntryHasLocalContent = new Func<TMetadataEntry, bool>(UpstreamFileFilteringMetadataService<TPackageIdentity, TMetadataEntry>.EntryHasLocalContent))).Select<TMetadataEntry, TMetadataEntry>(new Func<TMetadataEntry, TMetadataEntry>(this.RemoveUpstreamFiles)).ToList<TMetadataEntry>();
      if (num2 == 0 || !flag2 || feedRequest.Feed.View != null)
        source = list;
      if ((num1 & (flag3 ? 1 : 0)) != 0 && !list.Any<TMetadataEntry>() && this.executionEnvironment.IsUnauthenticatedWebRequest())
        throw new UnauthorizedRequestException(Resources.Error_AuthForPossibleUpstreamRefresh((object) doc.Properties.PackageName), HttpStatusCode.Unauthorized);
      return source;
    }

    private TMetadataEntry RemoveUpstreamFiles(TMetadataEntry entry)
    {
      int count = entry.PackageFiles.Count;
      List<IPackageFile> list = entry.PackageFiles.Where<IPackageFile>((Func<IPackageFile, bool>) (f => !(f.StorageId is UpstreamStorageId))).ToList<IPackageFile>();
      return list.Count != count ? entry.CreateEntryWithUpdatedFiles(list) : entry;
    }

    private static bool EntryHasLocalContent(TMetadataEntry entry) => (object) entry != null && entry.PackageFiles.Any<IPackageFile>((Func<IPackageFile, bool>) (e => e.StorageId.IsLocal));

    public async Task<MetadataDocument<IMetadataEntry>> GetGenericUnfilteredPackageVersionStatesDocumentWithoutRefreshAsync(
      IPackageNameRequest packageNameRequest)
    {
      return await this.innerMetadataService.GetGenericUnfilteredPackageVersionStatesDocumentWithoutRefreshAsync(packageNameRequest);
    }
  }
}
