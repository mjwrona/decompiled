// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.PublicRepositories.PublicRepositoryInterestTracker`1
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.Common;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


#nullable enable
namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.PublicRepositories
{
  public class PublicRepositoryInterestTracker<TPackageName> : 
    IPublicRepositoryInterestTracker<TPackageName>
    where TPackageName : IPackageName
  {
    public PublicRepositoryInterestTracker(
      IPublicRepositoryInterestTrackerServiceFacade publicRepositoryInterestTrackerServiceFacade,
      IConverter<string, TPackageName> packageNameResolver)
    {
      // ISSUE: reference to a compiler-generated field
      this.\u003CpublicRepositoryInterestTrackerServiceFacade\u003EP = publicRepositoryInterestTrackerServiceFacade;
      // ISSUE: reference to a compiler-generated field
      this.\u003CpackageNameResolver\u003EP = packageNameResolver;
      // ISSUE: explicit constructor call
      base.\u002Ector();
    }

    public static PublicRepositoryInterestTracker<TPackageName> Bootstrap(
      IVssRequestContext requestContext,
      IConverter<string, TPackageName> packageNameResolver)
    {
      IPublicRepositoryInterestTrackerService service = requestContext.GetService<IPublicRepositoryInterestTrackerService>();
      return new PublicRepositoryInterestTracker<TPackageName>((IPublicRepositoryInterestTrackerServiceFacade) new PublicRepositoryInterestTrackerServiceFacade(requestContext, service), packageNameResolver);
    }

    public async Task RegisterInterestAsync(
      CollectionId downstreamCollectionId,
      IFeedRequest downstreamFeedRequest,
      TPackageName packageName,
      WellKnownUpstreamSource source)
    {
      // ISSUE: reference to a compiler-generated field
      await this.\u003CpublicRepositoryInterestTrackerServiceFacade\u003EP.RegisterInterestAsync(downstreamCollectionId, downstreamFeedRequest, (IPackageName) packageName, source);
    }

    public IEnumerable<FeedInterestedInPackage> GetFeedsInterestedInPackage(
      TPackageName packageName,
      WellKnownUpstreamSource source)
    {
      // ISSUE: reference to a compiler-generated field
      return this.\u003CpublicRepositoryInterestTrackerServiceFacade\u003EP.GetFeedsInterestedInPackage((IPackageName) packageName, source);
    }

    public IEnumerable<TPackageName> GetAllPackagesWithInterestedFeeds(
      WellKnownUpstreamSource source)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return this.\u003CpublicRepositoryInterestTrackerServiceFacade\u003EP.GetAllPackagesWithInterestedFeeds(source).Select<string, TPackageName>(new Func<string, TPackageName>(this.\u003CpackageNameResolver\u003EP.Convert));
    }
  }
}
