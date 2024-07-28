// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.FeedSqlResourceComponent8
// Assembly: Microsoft.VisualStudio.Services.Feed.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55555083-3B79-4F4F-AA85-92D66019974E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Server.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.Common;
using Microsoft.VisualStudio.Services.Feed.Server.Types;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Feed.Server
{
  public class FeedSqlResourceComponent8 : FeedSqlResourceComponent7
  {
    public override Microsoft.VisualStudio.Services.Feed.WebApi.Feed CreateFeed(
      FeedIdentity feedId,
      string feedName,
      string feedDescription,
      bool upstreamEnabled,
      bool allowUpstreamNameConflict,
      bool hideDeletedPackageVersions,
      IList<UpstreamSource> upstreamSources,
      FeedCapabilities capabilities,
      bool badgesEnabled)
    {
      this.PrepareStoredProcedure("Feed.prc_CreateFeed");
      this.BindFeedIdentity(feedId, true);
      this.BindString("@feedName", feedName, 64, false, SqlDbType.NVarChar);
      this.BindString("@feedDescription", feedDescription, (int) byte.MaxValue, true, SqlDbType.NVarChar);
      this.BindBoolean("@upstreamEnabled", upstreamEnabled);
      this.BindBoolean("@allowUpstreamNameConflict", allowUpstreamNameConflict);
      this.BindBoolean("@hideDeletedPackageVersions", hideDeletedPackageVersions);
      this.BindUpstreamTableParameter("@upstreamSources", (IEnumerable<UpstreamSource>) upstreamSources);
      this.BindInt("@capabilities", (int) capabilities);
      return this.ReadFeedAndUpstreams();
    }

    public override Microsoft.VisualStudio.Services.Feed.WebApi.Feed CreateFeed(
      FeedIdentity feedId,
      string feedName,
      string feedDescription,
      bool upstreamEnabled,
      bool allowUpstreamNameConflict,
      int deleteHoldLifetime,
      bool hideDeletedPackageVersions,
      IList<UpstreamSource> upstreamSources,
      FeedCapabilities capabilities,
      bool badgesEnabled,
      bool isTestCode)
    {
      if (!isTestCode)
        throw new NotSupportedException();
      this.PrepareStoredProcedure("Feed.prc_CreateFeed");
      this.BindFeedIdentity(feedId, true);
      this.BindString("@feedName", feedName, 64, false, SqlDbType.NVarChar);
      this.BindString("@feedDescription", feedDescription, (int) byte.MaxValue, true, SqlDbType.NVarChar);
      this.BindBoolean("@upstreamEnabled", upstreamEnabled);
      this.BindBoolean("@allowUpstreamNameConflict", allowUpstreamNameConflict);
      this.BindInt("@deleteHoldLifeTime", deleteHoldLifetime);
      this.BindBoolean("@hideDeletedPackageVersions", hideDeletedPackageVersions);
      this.BindUpstreamTableParameter("@upstreamSources", (IEnumerable<UpstreamSource>) upstreamSources);
      this.BindInt("@capabilities", (int) capabilities);
      return this.ReadFeedAndUpstreams();
    }

    public override IEnumerable<Microsoft.VisualStudio.Services.Feed.WebApi.Feed> GetFeeds(
      Guid? projectId,
      bool includeDeletedUpstreams = false)
    {
      this.PrepareStoredProcedure("Feed.prc_GetFeed");
      this.BindInt("@dataspaceId", this.GetDataspaceId(Guid.Empty));
      this.BindBoolean("@includeDeletedUpstreams", includeDeletedUpstreams);
      return (IEnumerable<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>) this.ReadFeedsAndUpstreams();
    }

    public override Microsoft.VisualStudio.Services.Feed.WebApi.Feed GetFeed(
      FeedIdentity feedId,
      bool includeDeletedUpstreams = false)
    {
      this.PrepareStoredProcedure("Feed.prc_GetFeed");
      this.BindFeedIdentity(feedId);
      this.BindBoolean("@includeDeletedUpstreams", includeDeletedUpstreams);
      return this.ReadFeedAndUpstreams();
    }

    public override Microsoft.VisualStudio.Services.Feed.WebApi.Feed GetFeed(
      string feedName,
      Guid? projectId,
      bool includeDeleted = false,
      bool includeDeletedUpstreams = false)
    {
      this.PrepareStoredProcedure("Feed.prc_GetFeed");
      this.BindInt("@dataspaceId", this.GetDataspaceId(Guid.Empty));
      this.BindString("@feedName", feedName, 64, false, SqlDbType.NVarChar);
      this.BindBoolean("@includeDeleted", includeDeleted);
      this.BindBoolean("@includeDeletedUpstreams", includeDeletedUpstreams);
      return this.ReadFeedAndUpstreams();
    }

    public override Microsoft.VisualStudio.Services.Feed.WebApi.Feed UpdateFeed(
      FeedIdentity feedId,
      string feedDescription,
      bool? upstreamEnabled,
      bool? allowUpstreamNameConflict,
      bool? hideDeletedPackageVersions,
      Guid? defaultReaderViewId,
      IList<UpstreamSource> upstreamSources,
      bool? badgesEnabled)
    {
      this.PrepareStoredProcedure("Feed.prc_UpdateFeed");
      this.BindFeedIdentity(feedId);
      this.BindString("@feedDescription", feedDescription, (int) byte.MaxValue, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindNullableBoolean("@upstreamEnabled", upstreamEnabled);
      this.BindNullableBoolean("@allowUpstreamNameConflict", allowUpstreamNameConflict);
      this.BindNullableBoolean("@hideDeletedPackageVersions", hideDeletedPackageVersions);
      this.BindNullableGuid("@defaultReaderViewId", defaultReaderViewId);
      this.BindBoolean("@setUpstreamSources", upstreamSources != null);
      this.BindUpstreamTableParameter("@upstreamSources", (IEnumerable<UpstreamSource>) upstreamSources);
      return this.ReadFeedAndUpstreams();
    }

    public override Microsoft.VisualStudio.Services.Feed.WebApi.Feed RenameFeed(
      FeedIdentity feedId,
      string feedName)
    {
      this.PrepareStoredProcedure("Feed.prc_RenameFeed");
      this.BindFeedIdentity(feedId);
      this.BindString("@feedName", feedName, 64, true, SqlDbType.NVarChar);
      return this.ReadFeedAndUpstreams();
    }

    public override Microsoft.VisualStudio.Services.Feed.WebApi.Feed SetFeedCapabilities(
      FeedIdentity feedId,
      FeedCapabilities capabilities)
    {
      this.PrepareStoredProcedure("Feed.prc_SetFeedCapabilities");
      this.BindFeedIdentity(feedId);
      this.BindInt("@capabilities", (int) capabilities);
      return this.ReadFeedAndUpstreams();
    }

    protected Microsoft.VisualStudio.Services.Feed.WebApi.Feed ReadFeedAndUpstreams() => this.ReadFeedsAndUpstreams().FirstOrDefault<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>();

    protected IList<Microsoft.VisualStudio.Services.Feed.WebApi.Feed> ReadFeedsAndUpstreams()
    {
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        Guid instanceId = this.RequestContext.ServiceHost.CollectionServiceHost.InstanceId;
        resultCollection.AddBinder<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>((ObjectBinder<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>) this.GetFeedBinder());
        resultCollection.AddBinder<UpstreamSourceSql>((ObjectBinder<UpstreamSourceSql>) new UpstreamBinder(instanceId));
        List<Microsoft.VisualStudio.Services.Feed.WebApi.Feed> items = resultCollection.GetCurrent<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>().Items;
        Dictionary<Guid, Microsoft.VisualStudio.Services.Feed.WebApi.Feed> dictionary = items.ToDictionary<Microsoft.VisualStudio.Services.Feed.WebApi.Feed, Guid, Microsoft.VisualStudio.Services.Feed.WebApi.Feed>((System.Func<Microsoft.VisualStudio.Services.Feed.WebApi.Feed, Guid>) (x => x.Id), (System.Func<Microsoft.VisualStudio.Services.Feed.WebApi.Feed, Microsoft.VisualStudio.Services.Feed.WebApi.Feed>) (x => x));
        if (resultCollection.TryNextResult())
        {
          foreach (UpstreamSourceSql upstreamSourceSql in resultCollection.GetCurrent<UpstreamSourceSql>().Items)
            dictionary[upstreamSourceSql.FeedId].UpstreamSources.Add(upstreamSourceSql.ToUpstreamSource());
        }
        return (IList<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>) items;
      }
    }

    protected virtual SqlMetaData[] typ_Upstream => new SqlMetaData[6]
    {
      new SqlMetaData("UpstreamId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("UpstreamName", SqlDbType.NVarChar, 64L),
      new SqlMetaData("ProtocolType", SqlDbType.NVarChar, 20L),
      new SqlMetaData("Location", SqlDbType.NVarChar, 2000L),
      new SqlMetaData("UpstreamSourceType", SqlDbType.Int),
      new SqlMetaData("UpstreamIndex", SqlDbType.Int)
    };

    protected virtual SqlParameter BindUpstreamTableParameter(
      string parameterName,
      IEnumerable<UpstreamSource> rows)
    {
      rows = rows ?? Enumerable.Empty<UpstreamSource>();
      rows = rows.Where<UpstreamSource>((System.Func<UpstreamSource, bool>) (x => x != null));
      IEnumerable<UpstreamSourceSql> source = rows.Select<UpstreamSource, UpstreamSourceSql>((System.Func<UpstreamSource, UpstreamSourceSql>) (x => new UpstreamSourceSql(x)));
      Func<UpstreamSourceSql, int, IEnumerable<SqlDataRecord>> selector = (Func<UpstreamSourceSql, int, IEnumerable<SqlDataRecord>>) ((row, index) =>
      {
        SqlDataRecord record = new SqlDataRecord(this.typ_Upstream);
        record.SetGuid(0, row.Id);
        record.SetString(1, row.Name, BindStringBehavior.EmptyStringToNull);
        record.SetString(2, row.Protocol, BindStringBehavior.EmptyStringToNull);
        record.SetString(3, row.Location, BindStringBehavior.EmptyStringToNull);
        record.SetInt32(4, (int) row.UpstreamSourceType);
        record.SetInt32(5, index);
        return (IEnumerable<SqlDataRecord>) new SqlDataRecord[1]
        {
          record
        };
      });
      return this.BindTable(parameterName, "Feed.typ_Upstream", source.SelectMany<UpstreamSourceSql, SqlDataRecord>(selector));
    }
  }
}
