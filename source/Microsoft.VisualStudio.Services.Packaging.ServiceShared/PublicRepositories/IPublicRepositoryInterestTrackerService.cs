// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.PublicRepositories.IPublicRepositoryInterestTrackerService
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.Common;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using System.Collections.Generic;
using System.Threading.Tasks;


#nullable enable
namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.PublicRepositories
{
  [DefaultServiceImplementation(typeof (PublicRepositoryInterestTrackerService))]
  public interface IPublicRepositoryInterestTrackerService : IVssFrameworkService
  {
    Task RegisterInterestAsync(
      IVssRequestContext requestContext,
      CollectionId collectionId,
      IFeedRequest downstreamFeedRequest,
      IPackageName packageName,
      WellKnownUpstreamSource source);

    IEnumerable<FeedInterestedInPackage> GetFeedsInterestedInPackage(
      IVssRequestContext requestContext,
      IPackageName packageName,
      WellKnownUpstreamSource source);

    IEnumerable<string> GetAllPackagesWithInterestedFeeds(
      IVssRequestContext requestContext,
      WellKnownUpstreamSource source);
  }
}
