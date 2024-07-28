// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Jobs.RefreshListTrimming.PackageRefreshListTrimmer
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Telemetry;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Jobs.RefreshListTrimming
{
  public class PackageRefreshListTrimmer
  {
    private readonly IFactory<IFeedRequest, IUpstreamMetadataCacheInfoStore> infoStoreFactory;
    private readonly IRecentPackageUsageService recentPackageUsageService;
    private readonly IFeedService feedService;
    private readonly IProtocolRegistrar protocolRegistrar;
    private readonly ITracerService tracerService;
    private readonly IPackagingTracesBasicInfo packagingTracesBasicInfo;
    private readonly IRegistryWriterService registryService;
    private readonly ITimeProvider timeProvider;
    public const string FeedTaggingRegistryRoot = "/PackagingUncached/Upstreams/RefreshListTrimming/CompletedFeeds/Run2/";
    public static readonly RegistryQuery AllCompletedFeedsRegistryQuery = (RegistryQuery) "/PackagingUncached/Upstreams/RefreshListTrimming/CompletedFeeds/Run2/*";

    public PackageRefreshListTrimmer(
      IFactory<IFeedRequest, IUpstreamMetadataCacheInfoStore> infoStoreFactory,
      IRecentPackageUsageService recentPackageUsageService,
      IFeedService feedService,
      IProtocolRegistrar protocolRegistrar,
      ITracerService tracerService,
      IPackagingTracesBasicInfo packagingTracesBasicInfo,
      IRegistryWriterService registryService,
      ITimeProvider timeProvider)
    {
      this.infoStoreFactory = infoStoreFactory;
      this.recentPackageUsageService = recentPackageUsageService;
      this.feedService = feedService;
      this.protocolRegistrar = protocolRegistrar;
      this.tracerService = tracerService;
      this.packagingTracesBasicInfo = packagingTracesBasicInfo;
      this.registryService = registryService;
      this.timeProvider = timeProvider;
    }

    public async Task TrimPackageRefreshListsAsync(IReadOnlyList<Guid> feedIds)
    {
      PackageRefreshListTrimmer sendInTheThisObject = this;
      using (ITracerBlock tracer = sendInTheThisObject.tracerService.Enter((object) sendInTheThisObject, nameof (TrimPackageRefreshListsAsync)))
      {
        ImmutableArray<IProtocol> protocols = sendInTheThisObject.protocolRegistrar.AllProtocols.Where<IProtocol>((Func<IProtocol, bool>) (x => x.LowercasedName != "ivy")).ToImmutableArray<IProtocol>();
        // ISSUE: method pointer
        ILookup<(Guid, IProtocol), IPackageName> recentPackagesByFeed = (await sendInTheThisObject.recentPackageUsageService.GetRecentlyUsedPackageNamesForFeedsAsync((IEnumerable<Guid>) feedIds)).Select<(Guid, string, string), (Guid, IProtocol, IPackageName)>(new Func<(Guid, string, string), (Guid, IProtocol, IPackageName)>((object) sendInTheThisObject, __methodptr(\u003CTrimPackageRefreshListsAsync\u003Eg__ConvertNames\u007C11_1))).ToLookup<(Guid, IProtocol, IPackageName), (Guid, IProtocol), IPackageName>((Func<(Guid, IProtocol, IPackageName), (Guid, IProtocol)>) (x => (x.FeedId, x.Protocol)), (Func<(Guid, IProtocol, IPackageName), IPackageName>) (x => x.PackageName));
        foreach (Guid feedId1 in (IEnumerable<Guid>) feedIds)
        {
          Guid feedId = feedId1;
          foreach (IProtocol protocol in protocols)
          {
            tracer.TraceInfo(string.Format("Processing feed {0}, protocol {1}", (object) feedId, (object) protocol));
            FeedCore forAnyScopeAsync = await sendInTheThisObject.feedService.GetFeedByIdForAnyScopeAsync(feedId);
            List<IPackageName> list = recentPackagesByFeed[(feedId, protocol)].ToList<IPackageName>();
            FeedRequest feedRequest = new FeedRequest(forAnyScopeAsync, protocol);
            sendInTheThisObject.packagingTracesBasicInfo.SetFromFeedRequest((IProtocolAgnosticFeedRequest) feedRequest);
            await sendInTheThisObject.infoStoreFactory.Get((IFeedRequest) feedRequest).RemoveAllPackagesExceptAsync(forAnyScopeAsync, (IReadOnlyCollection<IPackageName>) list);
          }
          sendInTheThisObject.registryService.SetValue<DateTime>("/PackagingUncached/Upstreams/RefreshListTrimming/CompletedFeeds/Run2/" + feedId.ToString("D"), sendInTheThisObject.timeProvider.Now);
          feedId = new Guid();
        }
        protocols = new ImmutableArray<IProtocol>();
        recentPackagesByFeed = (ILookup<(Guid, IProtocol), IPackageName>) null;
      }
    }
  }
}
