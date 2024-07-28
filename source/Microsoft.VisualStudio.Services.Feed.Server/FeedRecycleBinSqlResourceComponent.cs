// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.FeedRecycleBinSqlResourceComponent
// Assembly: Microsoft.VisualStudio.Services.Feed.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55555083-3B79-4F4F-AA85-92D66019974E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.Common;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Feed.Server
{
  public class FeedRecycleBinSqlResourceComponent : FeedSqlResourceComponentBase
  {
    public const int FeedNameMaxLength = 64;
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[1]
    {
      (IComponentCreator) new ComponentCreator<FeedRecycleBinSqlResourceComponent>(1)
    }, "FeedRecycleBin");
    private static Dictionary<int, SqlExceptionFactory> sqlExceptionFactories = new Dictionary<int, SqlExceptionFactory>();

    static FeedRecycleBinSqlResourceComponent() => FeedRecycleBinSqlResourceComponent.sqlExceptionFactories.Add(1620009, new SqlExceptionFactory(typeof (UnknownDatabaseErrorOcurredException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((requestContext, errorVal, sqlException, sqlError) => (Exception) UnknownDatabaseErrorOcurredException.Create(sqlError.ExtractString("optionalMessageString")))));

    public FeedRecycleBinSqlResourceComponent()
    {
      this.SelectedFeatures |= SqlResourceComponentFeatures.BindPartitionIdByDefault;
      this.ContainerErrorCode = 50000;
    }

    protected override IDictionary<int, SqlExceptionFactory> TranslatedExceptions => (IDictionary<int, SqlExceptionFactory>) FeedRecycleBinSqlResourceComponent.sqlExceptionFactories;

    public virtual IEnumerable<Microsoft.VisualStudio.Services.Feed.WebApi.Feed> GetFeedsInRecycleBin(
      Guid? projectId)
    {
      this.PrepareStoredProcedure("Feed.prc_GetFeedsInRecycleBin");
      Guid? nullable = projectId;
      this.BindInt("@dataspaceId", this.GetFeedDataspaceId(new Guid?(nullable ?? Guid.Empty)));
      int num;
      if (projectId.HasValue)
      {
        nullable = projectId;
        Guid empty = Guid.Empty;
        num = nullable.HasValue ? (nullable.HasValue ? (nullable.GetValueOrDefault() != empty ? 1 : 0) : 0) : 1;
      }
      else
        num = 0;
      this.BindBoolean("@isProjectScopedQuery", num != 0);
      return this.ReadFeeds();
    }

    public virtual Microsoft.VisualStudio.Services.Feed.WebApi.Feed GetFeedInRecycleBinById(
      FeedIdentity feed)
    {
      this.PrepareStoredProcedure("Feed.prc_GetFeedInRecycleBin");
      this.BindInt("@dataspaceId", this.GetFeedDataspaceId(new Guid?(feed.ProjectId ?? Guid.Empty)));
      this.BindGuid("@feedId", feed.Id);
      return this.ReadFeed();
    }

    public virtual Microsoft.VisualStudio.Services.Feed.WebApi.Feed GetFeedInRecycleBinByName(
      Guid? projectId,
      string feedName)
    {
      this.PrepareStoredProcedure("Feed.prc_GetFeedInRecycleBin");
      this.BindInt("@dataspaceId", this.GetFeedDataspaceId(new Guid?(projectId ?? Guid.Empty)));
      this.BindString("@feedName", feedName, 64, false, SqlDbType.NVarChar);
      return this.ReadFeed();
    }

    public virtual void PermanentDeleteFeed(FeedIdentity feed)
    {
      this.PrepareStoredProcedure("Feed.prc_PermanentDeleteFeed");
      this.BindFeedIdentity(feed, false, "@dataspaceId", "@feedId");
      this.ExecuteNonQuery();
    }

    public virtual void RestoreDeletedFeed(FeedIdentity feed)
    {
      this.PrepareStoredProcedure("Feed.prc_RestoreDeletedFeed");
      this.BindFeedIdentity(feed, false, "@dataspaceId", "@feedId");
      this.ExecuteNonQuery();
    }

    protected FeedBinder GetFeedBinder() => new FeedBinder((TeamFoundationSqlResourceComponent) this);

    protected IEnumerable<Microsoft.VisualStudio.Services.Feed.WebApi.Feed> ReadFeeds()
    {
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>((ObjectBinder<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>) this.GetFeedBinder());
        return (IEnumerable<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>) resultCollection.GetCurrent<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>().Items;
      }
    }

    protected Microsoft.VisualStudio.Services.Feed.WebApi.Feed ReadFeed() => this.ReadFeeds().FirstOrDefault<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>();

    protected int GetFeedsRowCount(string columnName)
    {
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<int>((ObjectBinder<int>) new SingleInt32ValueBinder(columnName));
        return resultCollection.GetCurrent<int>().Items[0];
      }
    }

    protected override void BindFeedIdentity(
      FeedIdentity feedId,
      bool createMissingDataspace = false,
      string dataspaceName = "@dataspaceId",
      string feedIdName = "@feedId")
    {
      this.BindInt(dataspaceName, this.GetFeedDataspaceId(feedId.ProjectId));
      this.BindGuid(feedIdName, feedId.Id);
    }
  }
}
