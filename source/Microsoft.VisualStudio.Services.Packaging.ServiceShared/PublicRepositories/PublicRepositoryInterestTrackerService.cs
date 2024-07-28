// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.PublicRepositories.PublicRepositoryInterestTrackerService
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Threading;
using Microsoft.VisualStudio.Services.Feed.Common;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Components;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Upstream;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;


#nullable enable
namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.PublicRepositories
{
  public class PublicRepositoryInterestTrackerService : 
    IPublicRepositoryInterestTrackerService,
    IVssFrameworkService
  {
    private readonly Channel<PackageInterestData> interestsChannel = Channel.CreateUnbounded<PackageInterestData>();
    private const int BatchSize = 50;

    public IEnumerable<string> GetAllPackagesWithInterestedFeeds(
      IVssRequestContext requestContext,
      WellKnownUpstreamSource source)
    {
      using (PkgsInterestComponent component = requestContext.CreateComponent<PkgsInterestComponent>())
        return component.GetAllPackagesWithInterestedFeeds(source);
    }

    public IEnumerable<FeedInterestedInPackage> GetFeedsInterestedInPackage(
      IVssRequestContext requestContext,
      IPackageName packageName,
      WellKnownUpstreamSource source)
    {
      using (PkgsInterestComponent component = requestContext.CreateComponent<PkgsInterestComponent>())
        return component.GetInterestedFeeds(source, packageName);
    }

    public async Task RegisterInterestAsync(
      IVssRequestContext requestContext,
      CollectionId collectionId,
      IFeedRequest downstreamFeedRequest,
      IPackageName packageName,
      WellKnownUpstreamSource source)
    {
      UpstreamSource upstreamSource = downstreamFeedRequest.Feed.UpstreamSources.FirstOrDefault<UpstreamSource>((Func<UpstreamSource, bool>) (u => u.IsWellKnownSource(source)));
      if (upstreamSource == null)
        return;
      FeedInterestedInPackage FeedInterestedInPackage = new FeedInterestedInPackage(collectionId, (FeedId) downstreamFeedRequest.Feed.Id, upstreamSource.Id);
      await this.interestsChannel.Writer.WriteAsync(new PackageInterestData(packageName, source, FeedInterestedInPackage));
    }

    private void WritePackageInterests(IVssRequestContext requestContext, object taskArgs) => requestContext.RunSynchronously((Func<Task>) (() => this.WritePackageInterestsAsync(requestContext)));

    private async Task WritePackageInterestsAsync(IVssRequestContext requestContext)
    {
      List<PackageInterestData> listAsync = await PublicRepositoryInterestTrackerService.ReadUntilCancelledAsync<PackageInterestData>(this.interestsChannel.Reader, new CancellationTokenSource(TimeSpan.FromSeconds(1.0)).Token).Take<PackageInterestData>(50).ToListAsync<PackageInterestData>(CancellationToken.None);
      if (listAsync.Count <= 0)
        return;
      using (PkgsInterestComponent component = requestContext.CreateComponent<PkgsInterestComponent>())
        component.RegisterPackageInterest((IEnumerable<PackageInterestData>) listAsync);
    }

    private static IAsyncEnumerable<T> ReadUntilCancelledAsync<T>(
      ChannelReader<T> channelReader,
      [EnumeratorCancellation] CancellationToken cancellationToken)
    {
      // ISSUE: object of a compiler-generated type is created
      return (IAsyncEnumerable<T>) new PublicRepositoryInterestTrackerService.\u003CReadUntilCancelledAsync\u003Ed__8<T>(-2)
      {
        \u003C\u003E3__channelReader = channelReader,
        \u003C\u003E3__cancellationToken = cancellationToken
      };
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      IVssRequestContext vssRequestContext = systemRequestContext.To(TeamFoundationHostType.Deployment);
      vssRequestContext.GetService<ITeamFoundationTaskService>().AddTask(vssRequestContext, new TeamFoundationTask(new TeamFoundationTaskCallback(this.WritePackageInterests), (object) null, 1000));
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }
  }
}
