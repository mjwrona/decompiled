// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamCache.V2.ByVersionUpstreamRefreshStrategy`5
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using BuildXL.Cache.ContentStore.UtilitiesCore.Internal;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Packaging.CrossProtocol.Internal.WebApi.Types;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Ingestion.CentralFeedServices;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Settings;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Upstream;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamFailureTracking.Classifier;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamVersionList;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Exceptions;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Types;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;


#nullable enable
namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamCache.V2
{
  public class ByVersionUpstreamRefreshStrategy<TPackageName, TPackageVersion, TPackageIdentity, TMetadataEntry, TMetadataEntryWriteable> : 
    IUpstreamRefreshStrategy<
    #nullable disable
    TPackageName, TPackageIdentity, TMetadataEntry>
    where TPackageName : IPackageName
    where TPackageVersion : class, IPackageVersion
    where TPackageIdentity : IPackageIdentity<TPackageName, TPackageVersion>
    where TMetadataEntry : class, IMetadataEntry<TPackageIdentity>, ICreateWriteable<TMetadataEntryWriteable>
    where TMetadataEntryWriteable : IMetadataEntryWritable, TMetadataEntry
  {
    private readonly ITimeProvider timeProvider;
    private readonly IFactory<UpstreamSource, Task<IUpstreamMetadataService<TPackageName, TPackageVersion, TPackageIdentity, TMetadataEntry>>> upstreamMetadataServiceFactory;
    private readonly IAsyncHandler<MetadataDocument<TMetadataEntry>, List<TMetadataEntry>> upstreamEntriesRetainingStrategyHandler;
    private readonly IUpstreamVersionListService<TPackageName, TPackageVersion> upstreamVersionListService;
    private readonly IAggregationAccessorFactory writeAggregationAccessorsFactory;
    private readonly IAggregationCommitApplier aggregationCommitApplier;
    private readonly ITracerService tracerService;
    private readonly IUpstreamPackageNameMetadataRefreshStrategy<TPackageName, TMetadataEntry> packageNameRefreshStrategy;
    private readonly IUpstreamStatusClassifier upstreamStatusClassifier;
    private readonly IFeatureFlagService featureFlagService;
    private readonly IFrotocolLevelPackagingSetting<IReadOnlyCollection<string>> bannedCustomUpstreamHostsSetting;
    private readonly ITerrapinMetadataValidator<TPackageName, TPackageVersion> terrapinValidator;
    private readonly IFrotocolLevelPackagingSetting<TimeSpan> metadataValidityPeriodWithEntriesSetting;
    private readonly IFrotocolLevelPackagingSetting<TimeSpan> metadataValidityPeriodWithoutEntriesSetting;
    private readonly bool updateUpstreamDataForLocalVersions;
    private readonly IShouldIncludeExternalVersionsHelper shouldIncludeExternalVersionsHelper;

    public ByVersionUpstreamRefreshStrategy(
      ITimeProvider timeProvider,
      IFactory<UpstreamSource, Task<IUpstreamMetadataService<TPackageName, TPackageVersion, TPackageIdentity, TMetadataEntry>>> upstreamMetadataServiceFactory,
      IAsyncHandler<MetadataDocument<TMetadataEntry>, List<TMetadataEntry>> upstreamEntriesRetainingStrategyHandler,
      IUpstreamVersionListService<TPackageName, TPackageVersion> upstreamVersionListService,
      IAggregationAccessorFactory writeAggregationAccessorsFactory,
      IAggregationCommitApplier aggregationCommitApplier,
      ITracerService tracerService,
      IUpstreamPackageNameMetadataRefreshStrategy<TPackageName, TMetadataEntry> packageNameRefreshStrategy,
      IUpstreamStatusClassifier upstreamStatusClassifier,
      IFeatureFlagService featureFlagService,
      bool updateUpstreamDataForLocalVersions,
      IFrotocolLevelPackagingSetting<IReadOnlyCollection<string>> bannedCustomUpstreamHostsSetting,
      IShouldIncludeExternalVersionsHelper shouldIncludeExternalVersionsHelper,
      ITerrapinMetadataValidator<TPackageName, TPackageVersion> terrapinValidator,
      IFrotocolLevelPackagingSetting<TimeSpan> metadataValidityPeriodWithEntriesSetting,
      IFrotocolLevelPackagingSetting<TimeSpan> metadataValidityPeriodWithoutEntriesSetting)
    {
      this.timeProvider = timeProvider;
      this.upstreamMetadataServiceFactory = upstreamMetadataServiceFactory;
      this.upstreamEntriesRetainingStrategyHandler = upstreamEntriesRetainingStrategyHandler;
      this.upstreamVersionListService = upstreamVersionListService ?? throw new ArgumentNullException(nameof (upstreamVersionListService));
      this.writeAggregationAccessorsFactory = writeAggregationAccessorsFactory;
      this.aggregationCommitApplier = aggregationCommitApplier;
      this.shouldIncludeExternalVersionsHelper = shouldIncludeExternalVersionsHelper;
      this.packageNameRefreshStrategy = packageNameRefreshStrategy ?? (IUpstreamPackageNameMetadataRefreshStrategy<TPackageName, TMetadataEntry>) new NoPackageNameMetadataRefresh<TPackageName, TMetadataEntry>();
      this.upstreamStatusClassifier = upstreamStatusClassifier;
      this.featureFlagService = featureFlagService;
      this.updateUpstreamDataForLocalVersions = updateUpstreamDataForLocalVersions;
      this.bannedCustomUpstreamHostsSetting = bannedCustomUpstreamHostsSetting;
      this.tracerService = tracerService;
      this.terrapinValidator = terrapinValidator;
      this.metadataValidityPeriodWithEntriesSetting = metadataValidityPeriodWithEntriesSetting;
      this.metadataValidityPeriodWithoutEntriesSetting = metadataValidityPeriodWithoutEntriesSetting;
    }

    public async Task<RefreshPackageResult> RefreshPackageAsync(
      IFeedRequest feedRequest,
      TPackageName packageName,
      IEnumerable<UpstreamSource> upstreams,
      MetadataDocument<TMetadataEntry> localDoc,
      IUpstreamsConfigurationHasher upstreamConfigurationHasher,
      bool forceRefreshAllUpstreamVersionLists,
      ISet<Guid> upstreamVersionListsToForceRefreshByUpstreamId,
      bool needIntermediateData)
    {
      return await this.RefreshPackageCoreAsync(feedRequest, packageName, upstreams, localDoc, upstreamConfigurationHasher, forceRefreshAllUpstreamVersionLists, upstreamVersionListsToForceRefreshByUpstreamId, needIntermediateData);
    }

    private async Task<RefreshPackageResult> RefreshPackageCoreAsync(
      IFeedRequest feedRequest,
      TPackageName packageName,
      IEnumerable<UpstreamSource> upstreams,
      MetadataDocument<TMetadataEntry> localDoc,
      IUpstreamsConfigurationHasher upstreamConfigurationHasher,
      bool forceRefreshAllUpstreamVersionLists,
      ISet<Guid> upstreamVersionListsToForceRefreshByUpstreamId,
      bool needIntermediateData)
    {
      ByVersionUpstreamRefreshStrategy<TPackageName, TPackageVersion, TPackageIdentity, TMetadataEntry, TMetadataEntryWriteable> sendInTheThisObject = this;
      FeedCore feed = feedRequest.Feed;
      int upstreamVersionListCacheHits = 0;
      int upstreamVersionListCacheMisses = 0;
      string packageDisplayNameString = packageName.DisplayName;
      using (ITracerBlock tracer = sendInTheThisObject.tracerService.Enter((object) sendInTheThisObject, nameof (RefreshPackageCoreAsync)))
      {
        try
        {
          RefreshPackageResult refreshPackageResultIfFailed;
          if (sendInTheThisObject.HasBannedUpstreams(feedRequest, packageName, upstreams, out refreshPackageResultIfFailed))
            return refreshPackageResultIfFailed;
          DateTime now = sendInTheThisObject.timeProvider.Now;
          DateTime forceSaveIfOlderThanDateTime = now - UpstreamMetadata.ShouldForceSaveUpstreamDataIfOlderThanSpan;
          DateTime versionListsInvalidIfOlderThan = now - TimeSpanExtensions.Min(sendInTheThisObject.metadataValidityPeriodWithEntriesSetting.Get(feedRequest).AbsoluteValue(), sendInTheThisObject.metadataValidityPeriodWithoutEntriesSetting.Get(feedRequest).AbsoluteValue());
          UpstreamVersionListFile<TPackageVersion> cachedUpstreamListsDoc = await sendInTheThisObject.GetCachedUpstreamVersionListFile(feedRequest, packageName, forceRefreshAllUpstreamVersionLists, upstreamVersionListsToForceRefreshByUpstreamId, versionListsInvalidIfOlderThan);
          tracer.TraceMarker("After GetCachedUpstreamVersionListFile", packageDisplayNameString);
          Dictionary<UpstreamSource, ByVersionUpstreamRefreshStrategy<TPackageName, TPackageVersion, TPackageIdentity, TMetadataEntry, TMetadataEntryWriteable>.UpstreamHelpers> upstreamHelpers = await sendInTheThisObject.BuildUpstreamHelpers(upstreams, cachedUpstreamListsDoc);
          tracer.TraceMarker("After BuildUpstreamHelpers", packageDisplayNameString);
          bool shouldUpdate = false;
          int numShadowedVersions = 0;
          // ISSUE: reference to a compiler-generated field
          tracer.TraceConditionally((Func<string>) (() => string.Format("Beginning refresh of package {0} in feed {1} ({2})\n", (object) packageName, (object) feedRequest.Feed.FullyQualifiedName, (object) feedRequest.Feed.FullyQualifiedId) + "Upstream Sources:\n" + string.Join("\n", upstreamHelpers.Select<KeyValuePair<UpstreamSource, ByVersionUpstreamRefreshStrategy<TPackageName, TPackageVersion, TPackageIdentity, TMetadataEntry, TMetadataEntryWriteable>.UpstreamHelpers>, string>(closure_18 ?? (closure_18 = (Func<KeyValuePair<UpstreamSource, ByVersionUpstreamRefreshStrategy<TPackageName, TPackageVersion, TPackageIdentity, TMetadataEntry, TMetadataEntryWriteable>.UpstreamHelpers>, string>) (x => this.\u003C\u003E4__this.DisplayUpstreamSourceForTrace(x.Key)))))));
          List<TMetadataEntry> entries = localDoc?.Entries ?? new List<TMetadataEntry>();
          List<TMetadataEntry> localEntries = entries.Where<TMetadataEntry>((Func<TMetadataEntry, bool>) (x => x.IsLocal)).ToList<TMetadataEntry>();
          int previousUpstreamEntriesCount = entries.Count<TMetadataEntry>((Func<TMetadataEntry, bool>) (x => !x.IsLocal));
          List<TMetadataEntry> source1 = await sendInTheThisObject.upstreamEntriesRetainingStrategyHandler.Handle(localDoc);
          tracer.TraceMarker("After upstreamEntriesRetainingStrategyHandler", packageDisplayNameString);
          Dictionary<TPackageVersion, TMetadataEntry> upstreamVersionsToOldEntries = source1.ToDictionary<TMetadataEntry, TPackageVersion, TMetadataEntry>((Func<TMetadataEntry, TPackageVersion>) (e => e.PackageIdentity.Version), (Func<TMetadataEntry, TMetadataEntry>) (e => e));
          Dictionary<TPackageVersion, List<UpstreamVersionInstance<TPackageVersion>>> allVersionInstances = new Dictionary<TPackageVersion, List<UpstreamVersionInstance<TPackageVersion>>>();
          foreach (TMetadataEntry versionEntry in localEntries)
          {
            UpstreamVersionInstance<TPackageVersion> upstreamVersionInstance = ByVersionUpstreamRefreshStrategy<TPackageName, TPackageVersion, TPackageIdentity, TMetadataEntry, TMetadataEntryWriteable>.ConvertLocalVersionEntryToInstance(versionEntry, (IEnumerable<UpstreamSource>) upstreamHelpers.Keys);
            // ISSUE: reference to a compiler-generated field
            // ISSUE: reference to a compiler-generated field
            // ISSUE: reference to a compiler-generated field
            tracer.TraceConditionally((Func<string>) (() => "Discovered local entry: " + this.CS\u0024\u003C\u003E8__locals2.CS\u0024\u003C\u003E8__locals1.\u003C\u003E4__this.DisplayUpstreamVersionInstanceForTrace((IUpstreamVersionInstance) upstreamVersionInstance)));
            ByVersionUpstreamRefreshStrategy<TPackageName, TPackageVersion, TPackageIdentity, TMetadataEntry, TMetadataEntryWriteable>.AddValueToGroupList<TPackageVersion, UpstreamVersionInstance<TPackageVersion>>((IDictionary<TPackageVersion, List<UpstreamVersionInstance<TPackageVersion>>>) allVersionInstances, versionEntry.PackageIdentity.Version, upstreamVersionInstance);
          }
          List<UpstreamVersionListFileUpstream<TPackageVersion>> newUpstreamVersionListsToCache = new List<UpstreamVersionListFileUpstream<TPackageVersion>>(upstreamHelpers.Count);
          List<TMetadataEntry> upstreamEntriesToAddOrUpdate = new List<TMetadataEntry>();
          List<UpstreamVersionInstance<TPackageVersion>> terrapinVersions = new List<UpstreamVersionInstance<TPackageVersion>>();
          List<TMetadataEntry> entriesToRetain = new List<TMetadataEntry>();
          bool shouldIncludeExternals;
          List<UpstreamVersionInstance<TPackageVersion>> selectedSources;
          try
          {
            foreach (KeyValuePair<UpstreamSource, ByVersionUpstreamRefreshStrategy<TPackageName, TPackageVersion, TPackageIdentity, TMetadataEntry, TMetadataEntryWriteable>.UpstreamHelpers> source2 in upstreamHelpers)
            {
              UpstreamSource key;
              ByVersionUpstreamRefreshStrategy<TPackageName, TPackageVersion, TPackageIdentity, TMetadataEntry, TMetadataEntryWriteable>.UpstreamHelpers upstreamHelpers1;
              source2.Deconstruct<UpstreamSource, ByVersionUpstreamRefreshStrategy<TPackageName, TPackageVersion, TPackageIdentity, TMetadataEntry, TMetadataEntryWriteable>.UpstreamHelpers>(out key, out upstreamHelpers1);
              UpstreamSource upstreamSource = key;
              ByVersionUpstreamRefreshStrategy<TPackageName, TPackageVersion, TPackageIdentity, TMetadataEntry, TMetadataEntryWriteable>.UpstreamHelpers thisUpstreamHelpers = upstreamHelpers1;
              // ISSUE: variable of a compiler-generated type
              ByVersionUpstreamRefreshStrategy<TPackageName, TPackageVersion, TPackageIdentity, TMetadataEntry, TMetadataEntryWriteable>.\u003C\u003Ec__DisplayClass19_3 cDisplayClass193_1;
              UpstreamVersionInstance<TPackageVersion> thisInstance;
              // ISSUE: variable of a compiler-generated type
              ByVersionUpstreamRefreshStrategy<TPackageName, TPackageVersion, TPackageIdentity, TMetadataEntry, TMetadataEntryWriteable>.\u003C\u003Ec__DisplayClass19_3 cDisplayClass193;
              await thisUpstreamHelpers.UseAsync((Func<IUpstreamMetadataService<TPackageName, TPackageVersion, TPackageIdentity, TMetadataEntry>, Task>) (async metadataService =>
              {
                tracer.TraceMarker("In UseAsync for QueryUpstreamForVersions", upstreamSource.Location);
                IReadOnlyList<VersionWithSourceChain<TPackageVersion>> versions = (IReadOnlyList<VersionWithSourceChain<TPackageVersion>>) thisUpstreamHelpers.CachedVersions;
                if (versions == null)
                {
                  ++upstreamVersionListCacheMisses;
                  // ISSUE: reference to a compiler-generated field
                  // ISSUE: reference to a compiler-generated field
                  // ISSUE: reference to a compiler-generated field
                  versions = await cDisplayClass193_1.CS\u0024\u003C\u003E8__locals3.CS\u0024\u003C\u003E8__locals1.\u003C\u003E4__this.QueryUpstreamForVersions(feedRequest, packageName, upstreamSource, metadataService);
                  tracer.TraceMarker("After QueryUpstreamForVersions", upstreamSource.Location);
                  newUpstreamVersionListsToCache.Add(new UpstreamVersionListFileUpstream<TPackageVersion>(upstreamSource.ToUpstreamSourceInfo(), (IEnumerable<VersionWithSourceChain<TPackageVersion>>) versions, now));
                }
                else
                  ++upstreamVersionListCacheHits;
                tracer.TraceInfo(string.Format("There are {0} available versions for package '{1}' on upstream: '{2}'", (object) versions.Count, (object) packageName, (object) upstreamSource));
                foreach (VersionWithSourceChain<TPackageVersion> versionWithSourceChain in (IEnumerable<VersionWithSourceChain<TPackageVersion>>) versions)
                {
                  cDisplayClass193 = cDisplayClass193_1;
                  thisInstance = UpstreamVersionInstance.FromUpstreamVersion<TPackageVersion>(versionWithSourceChain.Version, upstreamSource, (IEnumerable<UpstreamSourceInfo>) versionWithSourceChain.SourceChain, PackageOriginClassifier.ClassifyUpstreamPackageOrigin(upstreamSource, (IEnumerable<UpstreamSourceInfo>) versionWithSourceChain.SourceChain));
                  // ISSUE: reference to a compiler-generated field
                  // ISSUE: reference to a compiler-generated field
                  // ISSUE: reference to a compiler-generated field
                  tracer.TraceConditionally((Func<string>) (() => "Discovered remote entry: " + cDisplayClass193.CS\u0024\u003C\u003E8__locals3.CS\u0024\u003C\u003E8__locals1.\u003C\u003E4__this.DisplayUpstreamVersionInstanceForTrace((IUpstreamVersionInstance) thisInstance)));
                  ByVersionUpstreamRefreshStrategy<TPackageName, TPackageVersion, TPackageIdentity, TMetadataEntry, TMetadataEntryWriteable>.AddValueToGroupList<TPackageVersion, UpstreamVersionInstance<TPackageVersion>>((IDictionary<TPackageVersion, List<UpstreamVersionInstance<TPackageVersion>>>) allVersionInstances, versionWithSourceChain.Version, thisInstance);
                }
              }));
            }
            shouldIncludeExternals = sendInTheThisObject.shouldIncludeExternalVersionsHelper.ShouldIncludeExternalUpstreamVersions(feed, (IPackageName) packageName, (IReadOnlyCollection<IUpstreamVersionInstance>) allVersionInstances.Values.SelectMany<List<UpstreamVersionInstance<TPackageVersion>>, UpstreamVersionInstance<TPackageVersion>>((Func<List<UpstreamVersionInstance<TPackageVersion>>, IEnumerable<UpstreamVersionInstance<TPackageVersion>>>) (x => (IEnumerable<UpstreamVersionInstance<TPackageVersion>>) x)).ToList<UpstreamVersionInstance<TPackageVersion>>());
            selectedSources = ByVersionUpstreamRefreshStrategy<TPackageName, TPackageVersion, TPackageIdentity, TMetadataEntry, TMetadataEntryWriteable>.SelectUpstreamSources(shouldIncludeExternals, (IEnumerable<IEnumerable<UpstreamVersionInstance<TPackageVersion>>>) allVersionInstances.Values).ToList<UpstreamVersionInstance<TPackageVersion>>();
            // ISSUE: reference to a compiler-generated field
            // ISSUE: reference to a compiler-generated field
            tracer.TraceConditionally((Func<string>) (() => "Selected sources:\n" + string.Join("\n", selectedSources.Select<UpstreamVersionInstance<TPackageVersion>, string>(new Func<UpstreamVersionInstance<TPackageVersion>, string>(this.CS\u0024\u003C\u003E8__locals1.\u003C\u003E4__this.DisplayUpstreamVersionInstanceForTrace)))));
            tracer.TraceMarker("Selected Sources", packageDisplayNameString);
            numShadowedVersions = selectedSources.Count<UpstreamVersionInstance<TPackageVersion>>((Func<UpstreamVersionInstance<TPackageVersion>, bool>) (v => v.IsLocal && allVersionInstances[v.Version].Count > 1));
            foreach (IGrouping<UpstreamSource, UpstreamVersionInstance<TPackageVersion>> source3 in selectedSources.GroupBy<UpstreamVersionInstance<TPackageVersion>, UpstreamSource, UpstreamVersionInstance<TPackageVersion>>((Func<UpstreamVersionInstance<TPackageVersion>, UpstreamSource>) (a => a.ImmediateSource), (Func<UpstreamVersionInstance<TPackageVersion>, UpstreamVersionInstance<TPackageVersion>>) (a => a)))
            {
              UpstreamSource source = source3.Key;
              if (source != null)
              {
                List<UpstreamVersionInstance<TPackageVersion>> versions = source3.ToList<UpstreamVersionInstance<TPackageVersion>>();
                if (!sendInTheThisObject.updateUpstreamDataForLocalVersions)
                  versions.RemoveAll((Predicate<UpstreamVersionInstance<TPackageVersion>>) (x => x.IsLocal));
                if (versions.Any<UpstreamVersionInstance<TPackageVersion>>())
                {
                  IImmutableList<VersionWithSourceChain<TPackageVersion>> cachedOrNewVersionListForSource = newUpstreamVersionListsToCache.FirstOrDefault<UpstreamVersionListFileUpstream<TPackageVersion>>((Func<UpstreamVersionListFileUpstream<TPackageVersion>, bool>) (item => item.UpstreamSourceInfo.Id == source.Id))?.Versions ?? upstreamHelpers[source].CachedVersions;
                  // ISSUE: variable of a compiler-generated type
                  ByVersionUpstreamRefreshStrategy<TPackageName, TPackageVersion, TPackageIdentity, TMetadataEntry, TMetadataEntryWriteable>.\u003C\u003Ec__DisplayClass19_5 cDisplayClass195_1;
                  Dictionary<TPackageVersion, TMetadataEntry> thisSourceEntriesToRetain;
                  HashSet<TPackageVersion> versionsToLookup;
                  // ISSUE: variable of a compiler-generated type
                  ByVersionUpstreamRefreshStrategy<TPackageName, TPackageVersion, TPackageIdentity, TMetadataEntry, TMetadataEntryWriteable>.\u003C\u003Ec__DisplayClass19_5 cDisplayClass195;
                  await upstreamHelpers[source].UseAsync((Func<IUpstreamMetadataService<TPackageName, TPackageVersion, TPackageIdentity, TMetadataEntry>, Task>) (async upstreamMetadataService =>
                  {
                    cDisplayClass195 = cDisplayClass195_1;
                    tracer.TraceMarker("In UseAsync for GetNewEntriesFromUpstreamAsync", source.Location);
                    versionsToLookup = new HashSet<TPackageVersion>((IEqualityComparer<TPackageVersion>) PackageVersionComparer.NormalizedVersion);
                    thisSourceEntriesToRetain = new Dictionary<TPackageVersion, TMetadataEntry>((IEqualityComparer<TPackageVersion>) PackageVersionComparer.NormalizedVersion);
                    foreach (UpstreamVersionInstance<TPackageVersion> upstreamVersionInstance in versions)
                    {
                      TMetadataEntry oldEntry;
                      // ISSUE: reference to a compiler-generated field
                      // ISSUE: reference to a compiler-generated field
                      // ISSUE: reference to a compiler-generated field
                      bool flag = !upstreamVersionsToOldEntries.TryGetValue(upstreamVersionInstance.Version, out oldEntry) || !cDisplayClass195_1.CS\u0024\u003C\u003E8__locals5.CS\u0024\u003C\u003E8__locals1.\u003C\u003E4__this.IsSameUpstreamAsOldFakeEntry(oldEntry, source);
                      // ISSUE: reference to a compiler-generated field
                      // ISSUE: reference to a compiler-generated field
                      // ISSUE: reference to a compiler-generated field
                      if (cDisplayClass195_1.CS\u0024\u003C\u003E8__locals5.CS\u0024\u003C\u003E8__locals1.\u003C\u003E4__this.updateUpstreamDataForLocalVersions | flag)
                      {
                        versionsToLookup.Add(upstreamVersionInstance.Version);
                        if (flag && upstreamVersionInstance.Origin == PackageOrigin.External)
                          terrapinVersions.Add(upstreamVersionInstance);
                      }
                      else
                        thisSourceEntriesToRetain.Add(oldEntry.PackageIdentity.Version, oldEntry);
                    }
                    // ISSUE: reference to a compiler-generated field
                    // ISSUE: reference to a compiler-generated field
                    // ISSUE: reference to a compiler-generated field
                    // ISSUE: reference to a compiler-generated field
                    tracer.TraceConditionally((Func<string>) (() => "Source: " + cDisplayClass195.CS\u0024\u003C\u003E8__locals5.CS\u0024\u003C\u003E8__locals1.\u003C\u003E4__this.DisplayUpstreamSourceForTrace(cDisplayClass195.source) + "\nVersions to retain: " + string.Join<TPackageVersion>(", ", (IEnumerable<TPackageVersion>) thisSourceEntriesToRetain.Keys) + "\nVersions to lookup: " + string.Join<TPackageVersion>(", ", (IEnumerable<TPackageVersion>) versionsToLookup)));
                    // ISSUE: reference to a compiler-generated field
                    // ISSUE: reference to a compiler-generated field
                    // ISSUE: reference to a compiler-generated field
                    Dictionary<TPackageVersion, TMetadataEntry> thisSourceNewEntries = await cDisplayClass195_1.CS\u0024\u003C\u003E8__locals5.CS\u0024\u003C\u003E8__locals1.\u003C\u003E4__this.GetNewEntriesFromUpstreamAsync(feedRequest, source, upstreamMetadataService, packageName, (IReadOnlyCollection<TPackageVersion>) versionsToLookup);
                    tracer.TraceMarker("After GetNewEntriesFromUpstreamAsync", source.Location);
                    // ISSUE: reference to a compiler-generated field
                    // ISSUE: reference to a compiler-generated field
                    // ISSUE: reference to a compiler-generated field
                    cDisplayClass195_1.CS\u0024\u003C\u003E8__locals5.CS\u0024\u003C\u003E8__locals1.\u003C\u003E4__this.RemoveMissingVersionsFromCache(source, packageName, (IEnumerable<TPackageVersion>) versionsToLookup, (IEnumerable<TPackageVersion>) thisSourceNewEntries.Keys, cachedOrNewVersionListForSource, newUpstreamVersionListsToCache, now);
                    bool shouldRefreshTransientStateForRetainedEntries = forceRefreshAllUpstreamVersionLists || upstreamVersionListsToForceRefreshByUpstreamId.Contains(source.Id);
                    // ISSUE: reference to a compiler-generated field
                    // ISSUE: reference to a compiler-generated field
                    // ISSUE: reference to a compiler-generated field
                    await cDisplayClass195_1.CS\u0024\u003C\u003E8__locals5.CS\u0024\u003C\u003E8__locals1.\u003C\u003E4__this.UpdateTransientStateFromUpstreamAsync(feedRequest, source, upstreamMetadataService, packageName, thisSourceNewEntries, thisSourceEntriesToRetain, shouldRefreshTransientStateForRetainedEntries, now);
                    tracer.TraceMarker("After UpdateTransientStateFromUpstreamAsync", source.Location);
                    if (thisSourceNewEntries.Any<KeyValuePair<TPackageVersion, TMetadataEntry>>())
                      shouldUpdate = true;
                    upstreamEntriesToAddOrUpdate.AddRange((IEnumerable<TMetadataEntry>) thisSourceNewEntries.Values);
                    entriesToRetain.AddRange((IEnumerable<TMetadataEntry>) thisSourceEntriesToRetain.Values);
                    thisSourceNewEntries = (Dictionary<TPackageVersion, TMetadataEntry>) null;
                  }));
                }
              }
            }
          }
          catch (Exception ex) when (ex.HasRelatedUpstreamSource())
          {
            Guid? upstreamId = ex.GetRelatedUpstreamSourceIdOrDefault();
            UpstreamSource upstreamSource = upstreamHelpers.Keys.FirstOrDefault<UpstreamSource>((Func<UpstreamSource, bool>) (x =>
            {
              Guid id = x.Id;
              Guid? nullable = upstreamId;
              return nullable.HasValue && id == nullable.GetValueOrDefault();
            }));
            if (upstreamSource != null)
              return RefreshPackageResult.Failed(feed, (IPackageName) packageName, upstreamSource, sendInTheThisObject.upstreamStatusClassifier.Classify(ex, upstreamSource, feed));
            throw;
          }
          tracer.TraceMarker("Fetched new/updated data", packageDisplayNameString);
          List<TPackageVersion> list1 = selectedSources.Where<UpstreamVersionInstance<TPackageVersion>>((Func<UpstreamVersionInstance<TPackageVersion>, bool>) (x => !x.IsLocal ? allVersionInstances[x.Version].Any<UpstreamVersionInstance<TPackageVersion>>((Func<UpstreamVersionInstance<TPackageVersion>, bool>) (y => !y.IsLocal)) : allVersionInstances[x.Version].Any<UpstreamVersionInstance<TPackageVersion>>((Func<UpstreamVersionInstance<TPackageVersion>, bool>) (y =>
          {
            if (y.IsLocal)
              return false;
            Guid id1 = y.ImmediateSource.Id;
            Guid? id2 = x.ImmediateSource?.Id;
            return id2.HasValue && id1 == id2.GetValueOrDefault();
          })))).Select<UpstreamVersionInstance<TPackageVersion>, TPackageVersion>((Func<UpstreamVersionInstance<TPackageVersion>, TPackageVersion>) (x => x.Version)).ToList<TPackageVersion>();
          List<TPackageVersion> list2 = entries.Where<TMetadataEntry>((Func<TMetadataEntry, bool>) (x => x.IsFromUpstream)).Select<TMetadataEntry, TPackageVersion>((Func<TMetadataEntry, TPackageVersion>) (x => x.PackageIdentity.Version)).Except<TPackageVersion>((IEnumerable<TPackageVersion>) list1).ToList<TPackageVersion>();
          if (upstreamEntriesToAddOrUpdate.Any<TMetadataEntry>() || list2.Any<TPackageVersion>() || newUpstreamVersionListsToCache.Any<UpstreamVersionListFileUpstream<TPackageVersion>>() || terrapinVersions.Any<UpstreamVersionInstance<TPackageVersion>>())
            shouldUpdate = true;
          string upstreamConfigurationHash = upstreamConfigurationHasher.GetHash(new UpstreamsConfiguration(feedRequest), (IPackageNameRequest) feedRequest.WithPackageName<TPackageName>(packageName));
          MetadataDocument<TMetadataEntry> metadataDocument = localDoc;
          // ISSUE: explicit non-virtual call
          if ((metadataDocument != null ? (!__nonvirtual (metadataDocument.Properties).UpstreamsLastRefreshedUtc.HasValue ? 1 : 0) : 1) != 0 || localDoc.Properties.UpstreamsLastRefreshedUtc.Value < forceSaveIfOlderThanDateTime || localDoc.Properties.UpstreamsConfigurationHash != upstreamConfigurationHash)
            shouldUpdate = true;
          try
          {
            object localPackageNameMetadata = localDoc?.Properties?.NameMetadata;
            object refreshedPackageNameMetadata = await sendInTheThisObject.packageNameRefreshStrategy.RefreshPackageNameMetadata(feedRequest, packageName, upstreams, localPackageNameMetadata, localEntries.Concat<TMetadataEntry>((IEnumerable<TMetadataEntry>) upstreamEntriesToAddOrUpdate).Concat<TMetadataEntry>((IEnumerable<TMetadataEntry>) entriesToRetain), shouldUpdate, sendInTheThisObject.upstreamStatusClassifier);
            tracer.TraceMarker("After RefreshPackageNameMetadata", packageDisplayNameString);
            bool nameMetadataRefreshed = localPackageNameMetadata != refreshedPackageNameMetadata;
            RefreshPackageIntermediateData refreshPackageIntermediateData = (RefreshPackageIntermediateData) null;
            if (needIntermediateData)
              refreshPackageIntermediateData = new RefreshPackageIntermediateData((IReadOnlyDictionary<IPackageVersion, IReadOnlyList<IUpstreamVersionInstance>>) allVersionInstances.ToDictionary<KeyValuePair<TPackageVersion, List<UpstreamVersionInstance<TPackageVersion>>>, IPackageVersion, IReadOnlyList<IUpstreamVersionInstance>>((Func<KeyValuePair<TPackageVersion, List<UpstreamVersionInstance<TPackageVersion>>>, IPackageVersion>) (x => (IPackageVersion) x.Key), (Func<KeyValuePair<TPackageVersion, List<UpstreamVersionInstance<TPackageVersion>>>, IReadOnlyList<IUpstreamVersionInstance>>) (x => (IReadOnlyList<IUpstreamVersionInstance>) x.Value)), (IReadOnlyList<IUpstreamVersionInstance>) selectedSources, shouldIncludeExternals, (IReadOnlyCollection<IMetadataEntry>) localEntries, (IReadOnlyCollection<IMetadataEntry>) entriesToRetain, (IReadOnlyCollection<IMetadataEntry>) upstreamEntriesToAddOrUpdate);
            if (shouldUpdate | nameMetadataRefreshed)
            {
              List<TMetadataEntry> fakeEntriesToWrite = upstreamEntriesToAddOrUpdate.Concat<TMetadataEntry>((IEnumerable<TMetadataEntry>) entriesToRetain).ToList<TMetadataEntry>();
              tracer.TraceConditionally((Func<string>) (() => "Fake metadata entries to write: " + string.Join<TPackageVersion>(",", fakeEntriesToWrite.Select<TMetadataEntry, TPackageVersion>((Func<TMetadataEntry, TPackageVersion>) (x => x.PackageIdentity.Version)))));
              DateTime newLastRefreshedTime = !sendInTheThisObject.featureFlagService.IsEnabled("Packaging.Upstreams.UseNowAsNewLastRefreshedTime") ? cachedUpstreamListsDoc.Upstreams.Select<UpstreamVersionListFileUpstream<TPackageVersion>, DateTime>((Func<UpstreamVersionListFileUpstream<TPackageVersion>, DateTime>) (x => x.LastRefreshed)).Append<DateTime>(now).Min<DateTime>() : now;
              Dictionary<TPackageVersion, TerrapinIngestionValidationReason> terrapinData = await sendInTheThisObject.terrapinValidator.GetTerrapinData(packageName, (IEnumerable<UpstreamVersionInstance<TPackageVersion>>) terrapinVersions);
              tracer.TraceMarker("After GetTerrapinData");
              CommitLogEntry commit = new CommitLogEntry((ICommitOperationData) new UpdateUpstreamMetadataOperationData<TPackageIdentity, TPackageName, TPackageVersion, TMetadataEntry>(packageName, upstreamConfigurationHash, newLastRefreshedTime, fakeEntriesToWrite, refreshedPackageNameMetadata, (IEnumerable<UpstreamVersionListFileUpstream<TPackageVersion>>) newUpstreamVersionListsToCache, versionListsInvalidIfOlderThan, terrapinData), PackagingCommitId.Empty, PackagingCommitId.Empty, PackagingCommitId.Empty, 0L, now, now, Guid.Empty);
              IReadOnlyList<IAggregationAccessor> accessorsFor = await sendInTheThisObject.writeAggregationAccessorsFactory.GetAccessorsFor(feedRequest);
              tracer.TraceMarker("after GetAccessorsFor");
              AggregationApplyTimings aggregationApplyTimings = await sendInTheThisObject.aggregationCommitApplier.ApplyCommitAsync(accessorsFor, feedRequest, (IReadOnlyList<ICommitLogEntry>) new CommitLogEntry[1]
              {
                commit
              });
              tracer.TraceMarker("after ApplyCommitAsync");
              return RefreshPackageResult.Refreshed(feed, (IPackageName) packageName, previousUpstreamEntriesCount, upstreamEntriesToAddOrUpdate.Count, terrapinVersions.Count, localEntries.Count, numShadowedVersions, (IReadOnlyList<UpstreamStatistics>) upstreamHelpers.Values.Select<ByVersionUpstreamRefreshStrategy<TPackageName, TPackageVersion, TPackageIdentity, TMetadataEntry, TMetadataEntryWriteable>.UpstreamHelpers, UpstreamStatistics>((Func<ByVersionUpstreamRefreshStrategy<TPackageName, TPackageVersion, TPackageIdentity, TMetadataEntry, TMetadataEntryWriteable>.UpstreamHelpers, UpstreamStatistics>) (x => new UpstreamStatistics(x.Timer.Resource.Id, x.Timer.Resource.UpstreamSourceType, (int) x.Timer.ElapsedMilliseconds))).ToList<UpstreamStatistics>(), nameMetadataRefreshed, entriesToRetain.Count, upstreamVersionListCacheHits, upstreamVersionListCacheMisses, refreshPackageIntermediateData);
            }
            tracer.TraceMarker("Refresh not needed");
            return RefreshPackageResult.RefreshNotNeeded(feed, (IPackageName) packageName, upstreamEntriesToAddOrUpdate.Count, terrapinVersions.Count, localEntries.Count, numShadowedVersions, (IReadOnlyList<UpstreamStatistics>) upstreamHelpers.Values.Select<ByVersionUpstreamRefreshStrategy<TPackageName, TPackageVersion, TPackageIdentity, TMetadataEntry, TMetadataEntryWriteable>.UpstreamHelpers, UpstreamStatistics>((Func<ByVersionUpstreamRefreshStrategy<TPackageName, TPackageVersion, TPackageIdentity, TMetadataEntry, TMetadataEntryWriteable>.UpstreamHelpers, UpstreamStatistics>) (x => new UpstreamStatistics(x.Timer.Resource.Id, x.Timer.Resource.UpstreamSourceType, (int) x.Timer.ElapsedMilliseconds))).ToList<UpstreamStatistics>(), entriesToRetain.Count, upstreamVersionListCacheHits, upstreamVersionListCacheMisses, refreshPackageIntermediateData);
          }
          catch (UpstreamFailureWithUpstreamSourceException ex)
          {
            return RefreshPackageResult.Failed(feed, (IPackageName) packageName, ex.UpstreamSource, new UpstreamFailureException(ex.Message, ex.InnerException, ex.ErrorCategory));
          }
          catch (Exception ex)
          {
            tracer.TraceInfo(string.Format("An unexpected exception {0} occurred while refreshing the package {1}.'", (object) ex, (object) packageName));
            throw;
          }
        }
        catch (Exception ex)
        {
          tracer.TraceException(ex);
          throw;
        }
      }
    }

    private void RemoveMissingVersionsFromCache(
      UpstreamSource source,
      TPackageName packageName,
      IEnumerable<TPackageVersion> requestedVersions,
      IEnumerable<TPackageVersion> returnedVersions,
      IImmutableList<VersionWithSourceChain<TPackageVersion>> currentVersionListForSource,
      List<UpstreamVersionListFileUpstream<TPackageVersion>> newUpstreamVersionListsToCache,
      DateTime now)
    {
      HashSet<IPackageVersion> versionsNotInMetadata = ((IEnumerable<IPackageVersion>) requestedVersions.Except<TPackageVersion>(returnedVersions)).ToHashSet<IPackageVersion>((IEqualityComparer<IPackageVersion>) PackageVersionComparer.NormalizedVersion);
      if (!versionsNotInMetadata.Any<IPackageVersion>())
        return;
      using (ITracerBlock tracerBlock = this.tracerService.Enter((object) this, nameof (RemoveMissingVersionsFromCache)))
      {
        foreach (IPackageVersion packageVersion in versionsNotInMetadata)
        {
          string message = string.Format("Requested Source: {0} for Package Name: {1}; ", (object) this.DisplayUpstreamSourceForTrace(source), (object) packageName) + "Version: " + packageVersion.NormalizedVersion + " for package version metadata but was unable to find.";
          tracerBlock.TraceInfoAlways(new string[1]
          {
            "UpstreamMetadataForPackageVersionNotFound"
          }, message);
        }
        if (currentVersionListForSource == null)
          return;
        newUpstreamVersionListsToCache.RemoveAll((Predicate<UpstreamVersionListFileUpstream<TPackageVersion>>) (x => x.UpstreamSourceInfo.Id == source.Id));
        newUpstreamVersionListsToCache.Add(new UpstreamVersionListFileUpstream<TPackageVersion>(source.ToUpstreamSourceInfo(), currentVersionListForSource.Where<VersionWithSourceChain<TPackageVersion>>((Func<VersionWithSourceChain<TPackageVersion>, bool>) (upstreamVersion => !versionsNotInMetadata.Contains((IPackageVersion) upstreamVersion.Version))), now));
      }
    }

    private async Task UpdateTransientStateFromUpstreamAsync(
      IFeedRequest downstreamFeedRequest,
      UpstreamSource source,
      IUpstreamMetadataService<TPackageName, TPackageVersion, TPackageIdentity, TMetadataEntry> upstreamMetadataService,
      TPackageName packageName,
      Dictionary<TPackageVersion, TMetadataEntry> thisSourceNewEntries,
      Dictionary<TPackageVersion, TMetadataEntry> thisSourceEntriesToRetain,
      bool shouldRefreshTransientStateForRetainedEntries,
      DateTime now)
    {
      ByVersionUpstreamRefreshStrategy<TPackageName, TPackageVersion, TPackageIdentity, TMetadataEntry, TMetadataEntryWriteable> sendInTheThisObject = this;
      ITracerBlock tracer = sendInTheThisObject.tracerService.Enter((object) sendInTheThisObject, nameof (UpdateTransientStateFromUpstreamAsync));
      try
      {
        List<TMetadataEntry> list = thisSourceNewEntries.Values.ToList<TMetadataEntry>();
        if (shouldRefreshTransientStateForRetainedEntries)
          list.AddRange((IEnumerable<TMetadataEntry>) thisSourceEntriesToRetain.Values);
        list.RemoveAll((Predicate<TMetadataEntry>) (x => x.IsLocal));
        if (!list.Any<TMetadataEntry>())
        {
          tracer = (ITracerBlock) null;
        }
        else
        {
          HashSet<IPackageVersion> validVersionsToReturn = ((IEnumerable<IPackageVersion>) list.Select<TMetadataEntry, TPackageVersion>((Func<TMetadataEntry, TPackageVersion>) (x => x.PackageIdentity.Version))).ToHashSet<IPackageVersion>((IEqualityComparer<IPackageVersion>) PackageVersionComparer.NormalizedVersion);
          List<TMetadataEntry> transientUpdatedEntriesList = (await upstreamMetadataService.UpdateEntriesWithTransientStateAsync(downstreamFeedRequest, packageName, (IEnumerable<TMetadataEntry>) list, (ICommitLogEntryHeader) CommitLogEntryHeader.GenerateFake(now))).ToList<TMetadataEntry>();
          // ISSUE: reference to a compiler-generated field
          tracer.TraceConditionally((Func<string>) (() => "Got entries updated with transient state from upstream:\r\n    Versions: " + string.Join<TPackageVersion>(", ", transientUpdatedEntriesList.Select<TMetadataEntry, TPackageVersion>((Func<TMetadataEntry, TPackageVersion>) (x => x.PackageIdentity.Version))) + "\r\n    Source: " + this.\u003C\u003E4__this.DisplayUpstreamSourceForTrace(source)));
          foreach (TMetadataEntry metadataEntry in transientUpdatedEntriesList)
          {
            if (!validVersionsToReturn.Contains((IPackageVersion) metadataEntry.PackageIdentity.Version))
              throw new InvalidOperationException("UpdateEntriesWithTransientStateAsync returned an entry for " + metadataEntry.PackageIdentity.DisplayStringForMessages + ", but that identity was not passed to it!");
            thisSourceEntriesToRetain.Remove(metadataEntry.PackageIdentity.Version);
            thisSourceNewEntries[metadataEntry.PackageIdentity.Version] = metadataEntry;
          }
          validVersionsToReturn = (HashSet<IPackageVersion>) null;
          tracer = (ITracerBlock) null;
        }
      }
      catch (Exception ex)
      {
        tracer.TraceException(ex);
        throw;
      }
      finally
      {
        tracer?.Dispose();
      }
    }

    private async Task<Dictionary<TPackageVersion, TMetadataEntry>> GetNewEntriesFromUpstreamAsync(
      IFeedRequest downstreamFeedRequest,
      UpstreamSource source,
      IUpstreamMetadataService<TPackageName, TPackageVersion, TPackageIdentity, TMetadataEntry> upstreamMetadataService,
      TPackageName packageName,
      IReadOnlyCollection<TPackageVersion> versionsToLookup)
    {
      ByVersionUpstreamRefreshStrategy<TPackageName, TPackageVersion, TPackageIdentity, TMetadataEntry, TMetadataEntryWriteable> sendInTheThisObject = this;
      using (ITracerBlock tracer = sendInTheThisObject.tracerService.Enter((object) sendInTheThisObject, nameof (GetNewEntriesFromUpstreamAsync)))
      {
        try
        {
          if (!versionsToLookup.Any<TPackageVersion>())
          {
            tracer.TraceInfo("No versions to fetch new entries for. Source: " + sendInTheThisObject.DisplayUpstreamSourceForTrace(source));
            return new Dictionary<TPackageVersion, TMetadataEntry>();
          }
          Dictionary<TPackageVersion, TMetadataEntry> thisSourceNewEntries = (await upstreamMetadataService.GetPackageVersionStatesAsync(downstreamFeedRequest, packageName, (IEnumerable<TPackageVersion>) versionsToLookup)).Where<TMetadataEntry>((Func<TMetadataEntry, bool>) (x => versionsToLookup.Contains<TPackageVersion>(x.PackageIdentity.Version))).ToDictionary<TMetadataEntry, TPackageVersion, TMetadataEntry>((Func<TMetadataEntry, TPackageVersion>) (entry => entry.PackageIdentity.Version), (Func<TMetadataEntry, TMetadataEntry>) (entry => entry), (IEqualityComparer<TPackageVersion>) PackageVersionComparer.NormalizedVersion);
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          tracer.TraceConditionally((Func<string>) (() => "Got versions from lookup: " + string.Join<TPackageVersion>(", ", (IEnumerable<TPackageVersion>) thisSourceNewEntries.Keys) + "\nSource: " + this.CS\u0024\u003C\u003E8__locals1.\u003C\u003E4__this.DisplayUpstreamSourceForTrace(source)));
          return thisSourceNewEntries;
        }
        catch (Exception ex)
        {
          tracer.TraceException(ex);
          throw;
        }
      }
    }

    private bool IsSameUpstreamAsOldFakeEntry(TMetadataEntry oldEntry, UpstreamSource source)
    {
      HashSet<Guid> source1 = new HashSet<Guid>();
      if (oldEntry.PackageStorageId is UpstreamStorageId packageStorageId)
        source1.Add(packageStorageId.UpstreamContentSource.Id);
      foreach (IPackageFile packageFile in (IEnumerable<IPackageFile>) oldEntry.PackageFiles)
      {
        if (packageFile.StorageId is UpstreamStorageId storageId)
          source1.Add(storageId.UpstreamContentSource.Id);
      }
      IEnumerable<UpstreamSourceInfo> sourceChain = oldEntry.SourceChain;
      Guid? nullable = sourceChain != null ? sourceChain.FirstOrDefault<UpstreamSourceInfo>()?.Id : new Guid?();
      if (nullable.HasValue)
        source1.Add(nullable.Value);
      return source1.Count == 1 && source1.Single<Guid>() == source.Id;
    }

    private bool HasBannedUpstreams(
      IFeedRequest feedRequest,
      TPackageName packageName,
      IEnumerable<UpstreamSource> upstreams,
      out RefreshPackageResult refreshPackageResultIfFailed)
    {
      HashSet<string> bannedUpstreamHosts = new HashSet<string>((IEnumerable<string>) this.bannedCustomUpstreamHostsSetting.Get(feedRequest), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      Uri result;
      UpstreamSource upstreamSource = upstreams.FirstOrDefault<UpstreamSource>((Func<UpstreamSource, bool>) (x => Uri.TryCreate(x.Location, UriKind.Absolute, out result) && bannedUpstreamHosts.Contains(result.Host)));
      if (upstreamSource == null)
      {
        refreshPackageResultIfFailed = (RefreshPackageResult) null;
        return false;
      }
      string message = Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Error_BlockedUpstream((object) upstreamSource.Id, (object) upstreamSource.Name, (object) upstreamSource.Location);
      UpstreamBlockedException blockedException = new UpstreamBlockedException(message);
      blockedException.SetRelatedUpstreamSource(upstreamSource);
      UpstreamFailureWithUpstreamSourceException upstreamFailure = new UpstreamFailureWithUpstreamSourceException(message, (Exception) blockedException, UpstreamStatusCategory.BlockedBySystem, upstreamSource);
      refreshPackageResultIfFailed = RefreshPackageResult.Failed(feedRequest.Feed, (IPackageName) packageName, upstreamSource, (UpstreamFailureException) upstreamFailure);
      return true;
    }

    private static UpstreamVersionInstance<TPackageVersion> ConvertLocalVersionEntryToInstance(
      TMetadataEntry versionEntry,
      IEnumerable<UpstreamSource> knownUpstreams)
    {
      TPackageVersion version = versionEntry.PackageIdentity.Version;
      IEnumerable<UpstreamSourceInfo> sourceChain1 = versionEntry.SourceChain;
      ImmutableList<UpstreamSourceInfo> immutableList = (sourceChain1 != null ? sourceChain1.ToImmutableList<UpstreamSourceInfo>() : (ImmutableList<UpstreamSourceInfo>) null) ?? ImmutableList<UpstreamSourceInfo>.Empty;
      Guid? immediateUpstreamId = immutableList.FirstOrDefault<UpstreamSourceInfo>()?.Id;
      UpstreamSource immediateSource = knownUpstreams.FirstOrDefault<UpstreamSource>((Func<UpstreamSource, bool>) (x =>
      {
        Guid id = x.Id;
        Guid? nullable = immediateUpstreamId;
        return nullable.HasValue && id == nullable.GetValueOrDefault();
      }));
      ImmutableList<UpstreamSourceInfo> sourceChain2 = immutableList;
      int origin = (int) PackageOriginClassifier.ClassifyLocalPackageOrigin((IEnumerable<UpstreamSourceInfo>) immutableList);
      int num = versionEntry.IsDeleted() ? 1 : 0;
      return UpstreamVersionInstance.FromLocalVersion<TPackageVersion>(version, immediateSource, (IEnumerable<UpstreamSourceInfo>) sourceChain2, (PackageOrigin) origin, num != 0);
    }

    private static void AddValueToGroupList<TKey, TValue>(
      IDictionary<TKey, List<TValue>> dict,
      TKey key,
      TValue value)
    {
      List<TValue> objList;
      if (!dict.TryGetValue(key, out objList))
      {
        objList = new List<TValue>();
        dict.Add(key, objList);
      }
      objList.Add(value);
    }

    private async Task<UpstreamVersionListFile<TPackageVersion>> GetCachedUpstreamVersionListFile(
      IFeedRequest feedRequest,
      TPackageName packageName,
      bool forceRefreshUpstreamVersionLists,
      ISet<Guid> forceRefreshTheseUpstreamVersionListsByUpstreamId,
      DateTime invalidIfOlderThanDateTime)
    {
      ByVersionUpstreamRefreshStrategy<TPackageName, TPackageVersion, TPackageIdentity, TMetadataEntry, TMetadataEntryWriteable> sendInTheThisObject = this;
      UpstreamVersionListFile<TPackageVersion> upstreamVersionListFile1;
      using (ITracerBlock tracer = sendInTheThisObject.tracerService.Enter((object) sendInTheThisObject, nameof (GetCachedUpstreamVersionListFile)))
      {
        try
        {
          bool flag = sendInTheThisObject.featureFlagService.IsEnabled("Packaging.UseCachedUpstreamVersionLists");
          UpstreamVersionListFile<TPackageVersion> upstreamVersionListFile2;
          if (forceRefreshUpstreamVersionLists || !flag)
            upstreamVersionListFile2 = UpstreamVersionListFile<TPackageVersion>.Empty;
          else
            upstreamVersionListFile2 = (await sendInTheThisObject.upstreamVersionListService.GetUpstreamVersionListDocument(feedRequest.WithPackageName<TPackageName>(packageName))).WithoutDataOlderThan(invalidIfOlderThanDateTime).WithoutUpstreamsByUpstreamId((IEnumerable<Guid>) forceRefreshTheseUpstreamVersionListsByUpstreamId);
          upstreamVersionListFile1 = upstreamVersionListFile2;
        }
        catch (Exception ex)
        {
          tracer.TraceException(ex);
          throw;
        }
      }
      return upstreamVersionListFile1;
    }

    private async Task<Dictionary<UpstreamSource, ByVersionUpstreamRefreshStrategy<TPackageName, TPackageVersion, TPackageIdentity, TMetadataEntry, TMetadataEntryWriteable>.UpstreamHelpers>> BuildUpstreamHelpers(
      IEnumerable<UpstreamSource> upstreams,
      UpstreamVersionListFile<TPackageVersion> cachedUpstreamListsDoc)
    {
      ByVersionUpstreamRefreshStrategy<TPackageName, TPackageVersion, TPackageIdentity, TMetadataEntry, TMetadataEntryWriteable> sendInTheThisObject = this;
      Dictionary<UpstreamSource, ByVersionUpstreamRefreshStrategy<TPackageName, TPackageVersion, TPackageIdentity, TMetadataEntry, TMetadataEntryWriteable>.UpstreamHelpers> dictionary;
      using (ITracerBlock tracer = sendInTheThisObject.tracerService.Enter((object) sendInTheThisObject, nameof (BuildUpstreamHelpers)))
      {
        try
        {
          Dictionary<Guid, UpstreamVersionListFileUpstream<TPackageVersion>> cachedUpstreamVersionLists = cachedUpstreamListsDoc.Upstreams.ToDictionary<UpstreamVersionListFileUpstream<TPackageVersion>, Guid>((Func<UpstreamVersionListFileUpstream<TPackageVersion>, Guid>) (x => x.UpstreamSourceInfo.Id));
          Dictionary<UpstreamSource, ByVersionUpstreamRefreshStrategy<TPackageName, TPackageVersion, TPackageIdentity, TMetadataEntry, TMetadataEntryWriteable>.UpstreamHelpers> upstreamHelpers = new Dictionary<UpstreamSource, ByVersionUpstreamRefreshStrategy<TPackageName, TPackageVersion, TPackageIdentity, TMetadataEntry, TMetadataEntryWriteable>.UpstreamHelpers>();
          foreach (UpstreamSource source in upstreams)
          {
            IUpstreamMetadataService<TPackageName, TPackageVersion, TPackageIdentity, TMetadataEntry> metadataService = await sendInTheThisObject.upstreamMetadataServiceFactory.Get(source);
            tracer.TraceMarker("After upstreamMetadataServiceFactory.Get", source.Location);
            upstreamHelpers.Add(source, new ByVersionUpstreamRefreshStrategy<TPackageName, TPackageVersion, TPackageIdentity, TMetadataEntry, TMetadataEntryWriteable>.UpstreamHelpers(source, new ResourceTimer<UpstreamSource>(source), metadataService, cachedUpstreamVersionLists.GetValueOrDefault<Guid, UpstreamVersionListFileUpstream<TPackageVersion>>(source.Id, (UpstreamVersionListFileUpstream<TPackageVersion>) null)?.Versions));
          }
          dictionary = upstreamHelpers;
        }
        catch (Exception ex)
        {
          tracer.TraceException(ex);
          throw;
        }
      }
      return dictionary;
    }

    private async Task<IReadOnlyList<VersionWithSourceChain<TPackageVersion>>> QueryUpstreamForVersions(
      IFeedRequest downstreamFeedRequest,
      TPackageName packageName,
      UpstreamSource upstream,
      IUpstreamMetadataService<TPackageName, TPackageVersion, TPackageIdentity, TMetadataEntry> upstreamMetadataService)
    {
      ByVersionUpstreamRefreshStrategy<TPackageName, TPackageVersion, TPackageIdentity, TMetadataEntry, TMetadataEntryWriteable> sendInTheThisObject = this;
      using (ITracerBlock tracer = sendInTheThisObject.tracerService.Enter((object) sendInTheThisObject, nameof (QueryUpstreamForVersions)))
      {
        try
        {
          return await upstreamMetadataService.GetPackageVersionsAsync(downstreamFeedRequest, packageName);
        }
        catch (Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Exceptions.PackageNotFoundException ex)
        {
          tracer.TraceInfo(string.Format("Didn't find any versions of '{0}' on the upstream '{1}'", (object) packageName, (object) upstream));
          return (IReadOnlyList<VersionWithSourceChain<TPackageVersion>>) ImmutableList<VersionWithSourceChain<TPackageVersion>>.Empty;
        }
        catch (Exception ex)
        {
          tracer.TraceException(ex);
          throw;
        }
      }
    }

    private string DisplayUpstreamVersionInstanceForTrace(IUpstreamVersionInstance instance) => string.Format("Version: {0}, Origin: {1}, ImmediateSource: {2}, IsLocal: {3}, SourceChain: {4}", (object) instance.Version, (object) instance.Origin, (object) this.DisplayUpstreamSourceForTrace(instance.ImmediateSource), (object) instance.IsLocal, (object) instance.SourceChain.Serialize<IReadOnlyList<UpstreamSourceInfo>>());

    private string DisplayUpstreamSourceForTrace(UpstreamSource source)
    {
      return source == null ? "(null)" : string.Format("{0} ({1}) <{2}>", (object) source.Name, (object) source.UpstreamSourceType, (object) GetLocation());

      string GetLocation()
      {
        if (!string.IsNullOrWhiteSpace(source.DisplayLocation))
          return source.DisplayLocation;
        return !string.IsNullOrWhiteSpace(source.Location) ? source.Location : "[No location?]";
      }
    }

    private static IEnumerable<UpstreamVersionInstance<TPackageVersion>> SelectUpstreamSources(
      bool includeExternalVersions,
      IEnumerable<IEnumerable<UpstreamVersionInstance<TPackageVersion>>> instancesByVersion)
    {
      return instancesByVersion.Select<IEnumerable<UpstreamVersionInstance<TPackageVersion>>, UpstreamVersionInstance<TPackageVersion>>((Func<IEnumerable<UpstreamVersionInstance<TPackageVersion>>, UpstreamVersionInstance<TPackageVersion>>) (valueTuple => ByVersionUpstreamRefreshStrategy<TPackageName, TPackageVersion, TPackageIdentity, TMetadataEntry, TMetadataEntryWriteable>.SelectSource(valueTuple, includeExternalVersions))).Where<UpstreamVersionInstance<TPackageVersion>>((Func<UpstreamVersionInstance<TPackageVersion>, bool>) (selectedSource => selectedSource != null));
    }

    private static UpstreamVersionInstance<TPackageVersion> SelectSource(
      IEnumerable<UpstreamVersionInstance<TPackageVersion>> upstreamVersionInstances,
      bool includeExternalVersions)
    {
      IEnumerable<UpstreamVersionInstance<TPackageVersion>> source = upstreamVersionInstances;
      return !includeExternalVersions ? source.FirstOrDefault<UpstreamVersionInstance<TPackageVersion>>((Func<UpstreamVersionInstance<TPackageVersion>, bool>) (x => x.IsLocal || x.Origin != PackageOrigin.External)) : source.FirstOrDefault<UpstreamVersionInstance<TPackageVersion>>();
    }

    public async Task<RefreshPackageResult> RefreshPackageVersionAsync(
      IFeedRequest feedRequest,
      TPackageIdentity packageIdentity,
      IEnumerable<UpstreamSource> upstreams,
      MetadataDocument<TMetadataEntry> localDoc,
      IUpstreamsConfigurationHasher upstreamConfigurationHasher,
      bool forceRefreshAllUpstreamVersionLists)
    {
      return await this.RefreshPackageCoreAsync(feedRequest, packageIdentity.Name, upstreams, localDoc, upstreamConfigurationHasher, forceRefreshAllUpstreamVersionLists, (ISet<Guid>) ImmutableHashSet<Guid>.Empty, false);
    }

    private class UpstreamHelpers
    {
      private readonly IUpstreamMetadataService<TPackageName, TPackageVersion, TPackageIdentity, TMetadataEntry> metadataService;

      public UpstreamSource Source { get; }

      public ResourceTimer<UpstreamSource> Timer { get; }

      public async Task UseAsync(
        Func<IUpstreamMetadataService<TPackageName, TPackageVersion, TPackageIdentity, TMetadataEntry>, Task> func)
      {
        using (this.Timer.TimerBlock())
        {
          try
          {
            await func(this.metadataService);
          }
          catch (Exception ex)
          {
            UpstreamSource source = this.Source;
            ex.SetRelatedUpstreamSource(source);
            throw;
          }
        }
      }

      public IImmutableList<VersionWithSourceChain<TPackageVersion>> CachedVersions { get; }

      public UpstreamHelpers(
        UpstreamSource source,
        ResourceTimer<UpstreamSource> timer,
        IUpstreamMetadataService<TPackageName, TPackageVersion, TPackageIdentity, TMetadataEntry> metadataService,
        IImmutableList<VersionWithSourceChain<TPackageVersion>> cachedVersions)
      {
        this.Source = source;
        this.Timer = timer;
        this.metadataService = metadataService;
        this.CachedVersions = cachedVersions;
      }
    }
  }
}
