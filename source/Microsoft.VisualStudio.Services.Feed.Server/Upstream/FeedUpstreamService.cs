// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.Upstream.FeedUpstreamService
// Assembly: Microsoft.VisualStudio.Services.Feed.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55555083-3B79-4F4F-AA85-92D66019974E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Feed.Server.Types;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Feed.Server.Upstream
{
  public class FeedUpstreamService : 
    IFeedUpstreamSQLService,
    IFeedUpstreamService,
    IVssFrameworkService
  {
    void IVssFrameworkService.ServiceStart(IVssRequestContext requestContext) => requestContext.TraceBlock(10019000, 10019001, 10019002, "Feed", "Service", (Action) (() =>
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      if (!requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        throw new UnexpectedHostTypeException(requestContext.ServiceHost.HostType);
    }), "ServiceStart");

    void IVssFrameworkService.ServiceEnd(IVssRequestContext requestContext) => requestContext.TraceBlock(10019003, 10019004, 10019005, "Feed", "Service", (Action) (() => ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext))), "ServiceEnd");

    public IEnumerable<DownstreamFeeds> GetFeedsWithUpstream(
      IVssRequestContext requestContext,
      Guid upstreamCollectionId,
      Guid? upstreamProjectId,
      Guid upstreamFeedId,
      Guid upstreamViewId,
      string protocolType,
      string normalizedPackageName)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(upstreamCollectionId, nameof (upstreamCollectionId));
      ArgumentUtility.CheckForEmptyGuid(upstreamFeedId, nameof (upstreamFeedId));
      ArgumentUtility.CheckForEmptyGuid(upstreamViewId, nameof (upstreamViewId));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(protocolType, nameof (protocolType));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(normalizedPackageName, nameof (normalizedPackageName));
      Func<ITeamFoundationDatabaseProperties, IEnumerable<DownstreamFeed>> getDownstreamFeeds = requestContext.IsFeatureEnabled("Packaging.Feed.DownstreamNotificationRequiresLocalVersion") ? (Func<ITeamFoundationDatabaseProperties, IEnumerable<DownstreamFeed>>) (_database => FeedUpstreamService.GetFeedsAffectedByUpstreamChange(upstreamCollectionId, upstreamFeedId, upstreamViewId, protocolType, normalizedPackageName, _database)) : (Func<ITeamFoundationDatabaseProperties, IEnumerable<DownstreamFeed>>) (_database => FeedUpstreamService.GetDownstreamFeedsFromAllPartitions(upstreamCollectionId, upstreamFeedId, upstreamViewId, protocolType, _database));
      return this.GetFeedsWithUpstreamInternal(requestContext, getDownstreamFeeds);
    }

    private IEnumerable<DownstreamFeeds> GetFeedsWithUpstreamInternal(
      IVssRequestContext requestContext,
      Func<ITeamFoundationDatabaseProperties, IEnumerable<DownstreamFeed>> getDownstreamFeeds)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      int? databaseId = vssRequestContext?.ServiceHost?.DeploymentServiceHost?.DatabaseProperties?.DatabaseId;
      if (!databaseId.HasValue)
        return Enumerable.Empty<DownstreamFeeds>();
      List<ITeamFoundationDatabaseProperties> databasePropertiesList = vssRequestContext.GetService<ITeamFoundationDatabaseManagementService>().QueryDatabases(vssRequestContext);
      List<DownstreamFeeds> upstreamInternal = new List<DownstreamFeeds>();
      foreach (ITeamFoundationDatabaseProperties database in databasePropertiesList)
      {
        if (this.ShouldGetDownstreamFeedsFromDatabase(database, databaseId.Value))
        {
          IEnumerable<DownstreamFeeds> collection = getDownstreamFeeds(database).GroupBy<DownstreamFeed, Guid, Guid, DownstreamFeeds>((Func<DownstreamFeed, Guid>) (f => f.HostId), (Func<DownstreamFeed, Guid>) (f => f.FeedId), (Func<Guid, IEnumerable<Guid>, DownstreamFeeds>) ((hostId, feedIds) => new DownstreamFeeds()
          {
            HostId = hostId,
            FeedIds = (IEnumerable<Guid>) feedIds.ToList<Guid>()
          }));
          upstreamInternal.AddRange(collection);
        }
      }
      return (IEnumerable<DownstreamFeeds>) upstreamInternal;
    }

    private bool ShouldGetDownstreamFeedsFromDatabase(
      ITeamFoundationDatabaseProperties database,
      int configurationDbId)
    {
      int num1 = database.DatabaseId != configurationDbId ? 1 : 0;
      bool flag1 = !string.IsNullOrEmpty(database.ServiceLevel);
      bool flag2 = database.Status == TeamFoundationDatabaseStatus.Online || database.Status == TeamFoundationDatabaseStatus.Servicing;
      bool flag3 = database.PoolName != null && (database.PoolName.Equals(DatabaseManagementConstants.DefaultPartitionPoolName, StringComparison.OrdinalIgnoreCase) || database.PoolName.Equals(DatabaseManagementConstants.RestrictedAcquisitionPartitionPool, StringComparison.OrdinalIgnoreCase));
      int num2 = flag1 ? 1 : 0;
      return (num1 & num2 & (flag2 ? 1 : 0) & (flag3 ? 1 : 0)) != 0;
    }

    private static IEnumerable<DownstreamFeed> GetDownstreamFeedsFromAllPartitions(
      Guid upstreamCollectionId,
      Guid upstreamFeedId,
      Guid upstreamViewId,
      string protocolType,
      ITeamFoundationDatabaseProperties database)
    {
      using (FeedSqlResourceComponent componentRaw = database.SqlConnectionInfo.CreateComponentRaw<FeedSqlResourceComponent>())
        return componentRaw.GetDownstreamFeedsFromUpstreamFromAllPartitions(upstreamCollectionId, upstreamFeedId, upstreamViewId, protocolType);
    }

    private static IEnumerable<DownstreamFeed> GetFeedsAffectedByUpstreamChange(
      Guid upstreamCollectionId,
      Guid upstreamFeedId,
      Guid upstreamViewId,
      string protocolType,
      string normalizedPackageName,
      ITeamFoundationDatabaseProperties database)
    {
      using (FeedSqlResourceComponent componentRaw = database.SqlConnectionInfo.CreateComponentRaw<FeedSqlResourceComponent>())
        return componentRaw.GetFeedsAffectedByUpstreamChange(upstreamCollectionId, upstreamFeedId, upstreamViewId, protocolType, normalizedPackageName);
    }
  }
}
