// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.FeedSqlResourceComponent
// Assembly: Microsoft.VisualStudio.Services.Feed.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55555083-3B79-4F4F-AA85-92D66019974E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Server.dll

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
  public class FeedSqlResourceComponent : FeedSqlResourceComponentBase
  {
    public const int FeedDescriptionMaxLength = 255;
    public const int FeedNameMaxLength = 64;
    public const int UpstreamNameMaxLength = 64;
    public const int UpstreamProtocolTypeMaxLength = 20;
    public const int UpstreamLocationMaxLength = 2000;
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[16]
    {
      (IComponentCreator) new ComponentCreator<FeedSqlResourceComponent>(2),
      (IComponentCreator) new ComponentCreator<FeedSqlResourceComponent3>(3),
      (IComponentCreator) new ComponentCreator<FeedSqlResourceComponent4>(4),
      (IComponentCreator) new ComponentCreator<FeedSqlResourceComponent5>(5),
      (IComponentCreator) new ComponentCreator<FeedSqlResourceComponent6>(6),
      (IComponentCreator) new ComponentCreator<FeedSqlResourceComponent7>(7),
      (IComponentCreator) new ComponentCreator<FeedSqlResourceComponent8>(8),
      (IComponentCreator) new ComponentCreator<FeedSqlResourceComponent9>(9),
      (IComponentCreator) new ComponentCreator<FeedSqlResourceComponent10>(10),
      (IComponentCreator) new ComponentCreator<FeedSqlResourceComponent11>(11),
      (IComponentCreator) new ComponentCreator<FeedSqlResourceComponent12>(12),
      (IComponentCreator) new ComponentCreator<FeedSqlResourceConponent13>(13),
      (IComponentCreator) new ComponentCreator<FeedSqlResourceComponent14>(14),
      (IComponentCreator) new ComponentCreator<FeedSqlResourceComponent15>(15),
      (IComponentCreator) new ComponentCreator<FeedSqlResourceComponent16>(16),
      (IComponentCreator) new ComponentCreator<FeedSqlResourceComponent17>(17)
    }, "Feed");
    private static Dictionary<int, SqlExceptionFactory> sqlExceptionFactories = new Dictionary<int, SqlExceptionFactory>();

    static FeedSqlResourceComponent()
    {
      FeedSqlResourceComponent.sqlExceptionFactories.Add(1620003, new SqlExceptionFactory(typeof (FeedNotReleasedException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((requestContext, errorVal, sqlException, sqlError) => (Exception) FeedNotReleasedException.Create(sqlError.ExtractString("feedName")))));
      FeedSqlResourceComponent.sqlExceptionFactories.Add(1620000, new SqlExceptionFactory(typeof (FeedNameAlreadyExistsException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((requestContext, errorVal, sqlException, sqlError) => (Exception) FeedNameAlreadyExistsException.Create(sqlError.ExtractString("feedName")))));
      FeedSqlResourceComponent.sqlExceptionFactories.Add(1620001, new SqlExceptionFactory(typeof (FeedIdNotFoundException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((requestContext, errorVal, sqlException, sqlError) => (Exception) FeedIdNotFoundException.Create(sqlError.ExtractString("feedName")))));
      FeedSqlResourceComponent.sqlExceptionFactories.Add(1620002, new SqlExceptionFactory(typeof (FeedIdNotFoundException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((requestContext, errorVal, sqlException, sqlError) => (Exception) FeedIdNotFoundException.Create(sqlError.ExtractString("feedIdString")))));
      FeedSqlResourceComponent.sqlExceptionFactories.Add(1620006, new SqlExceptionFactory(typeof (DatabaseStateIsNoLongerValidException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((requestContext, errorVal, sqlException, sqlError) => (Exception) new DatabaseStateIsNoLongerValidException())));
      FeedSqlResourceComponent.sqlExceptionFactories.Add(1620011, new SqlExceptionFactory(typeof (FeedViewNotFoundException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((requestContext, errorVal, sqlException, sqlError) => (Exception) FeedViewNotFoundException.Create(sqlError.ExtractString("feedIdString"), sqlError.ExtractString("feedViewIdString")))));
    }

    public FeedSqlResourceComponent()
    {
      this.SelectedFeatures |= SqlResourceComponentFeatures.BindPartitionIdByDefault;
      this.ContainerErrorCode = 50000;
    }

    protected override IDictionary<int, SqlExceptionFactory> TranslatedExceptions => (IDictionary<int, SqlExceptionFactory>) FeedSqlResourceComponent.sqlExceptionFactories;

    protected FeedBinder GetFeedBinder() => new FeedBinder((TeamFoundationSqlResourceComponent) this);

    public static Microsoft.VisualStudio.Services.Feed.WebApi.Feed DeepCloneFeed(Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed) => FeedBinder.DeepCloneFeed(feed);

    public virtual Microsoft.VisualStudio.Services.Feed.WebApi.Feed CreateFeed(
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
      return this.ReadFeed();
    }

    public virtual Microsoft.VisualStudio.Services.Feed.WebApi.Feed CreateFeed(
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
      this.BindInt("@deleteHoldLifeTime", deleteHoldLifetime);
      return this.ReadFeed();
    }

    public virtual IEnumerable<Microsoft.VisualStudio.Services.Feed.WebApi.Feed> GetFeeds(
      Guid? projectId,
      bool includeDeletedUpstreams = false)
    {
      this.PrepareStoredProcedure("Feed.prc_GetFeed");
      this.BindInt("@dataspaceId", this.GetDataspaceId(Guid.Empty));
      return this.ReadFeeds();
    }

    public virtual Microsoft.VisualStudio.Services.Feed.WebApi.Feed GetFeed(
      FeedIdentity feedId,
      bool includeDeletedUpstreams = false)
    {
      this.PrepareStoredProcedure("Feed.prc_GetFeed");
      this.BindFeedIdentity(feedId);
      return this.ReadFeed();
    }

    public virtual Microsoft.VisualStudio.Services.Feed.WebApi.Feed GetFeedByIdForAnyScope(
      Guid feedId,
      bool includeSoftDeletedFeeds)
    {
      throw new NotSupportedException();
    }

    public virtual Microsoft.VisualStudio.Services.Feed.WebApi.Feed GetFeed(
      string feedName,
      Guid? projectId,
      bool includeDeleted = false,
      bool includeDeletedUpstreams = false)
    {
      this.PrepareStoredProcedure("Feed.prc_GetFeed");
      this.BindInt("@dataspaceId", this.GetDataspaceId(Guid.Empty));
      this.BindString("@feedName", feedName, 64, false, SqlDbType.NVarChar);
      return this.ReadFeed();
    }

    public virtual Microsoft.VisualStudio.Services.Feed.WebApi.Feed UpdateFeed(
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
      return this.ReadFeed();
    }

    public virtual Microsoft.VisualStudio.Services.Feed.WebApi.Feed RenameFeed(
      FeedIdentity feedId,
      string feedName)
    {
      this.PrepareStoredProcedure("Feed.prc_RenameFeed");
      this.BindFeedIdentity(feedId);
      this.BindString("@feedName", feedName, 64, true, SqlDbType.NVarChar);
      return this.ReadFeed();
    }

    public virtual void DeleteFeed(FeedIdentity feedId)
    {
      this.PrepareStoredProcedure("Feed.prc_DeleteFeed");
      this.BindFeedIdentity(feedId);
      this.ExecuteNonQuery();
    }

    public virtual Microsoft.VisualStudio.Services.Feed.WebApi.Feed SetFeedCapabilities(
      FeedIdentity feedId,
      FeedCapabilities capabilities)
    {
      this.PrepareStoredProcedure("Feed.prc_GetFeed");
      this.BindFeedIdentity(feedId);
      return this.ReadFeed();
    }

    public virtual FeedInternalState GetInternalState(Guid feedId, Guid? projectId) => (FeedInternalState) null;

    public virtual void EndFeedUpgrade(Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed) => throw new NotImplementedException();

    public virtual void BeginFeedUpgrade(Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed) => throw new NotImplementedException();

    public virtual void ResetFeedUpgrade(Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed) => throw new NotImplementedException();

    public virtual IEnumerable<DownstreamFeed> GetDownstreamFeedsFromUpstreamFromAllPartitions(
      Guid upstreamCollectionId,
      Guid upstreamFeedId,
      Guid upstreamViewId,
      string protocolType)
    {
      throw new NotImplementedException();
    }

    public virtual IEnumerable<DownstreamFeed> GetFeedsAffectedByUpstreamChange(
      Guid upstreamCollectionId,
      Guid upstreamFeedId,
      Guid upstreamViewId,
      string protocolType,
      string normalizedPackageName)
    {
      return Enumerable.Empty<DownstreamFeed>();
    }

    protected Microsoft.VisualStudio.Services.Feed.WebApi.Feed ReadFeed() => this.ReadFeeds().FirstOrDefault<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>();

    protected IEnumerable<Microsoft.VisualStudio.Services.Feed.WebApi.Feed> ReadFeeds()
    {
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>((ObjectBinder<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>) this.GetFeedBinder());
        return (IEnumerable<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>) resultCollection.GetCurrent<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>().Items;
      }
    }
  }
}
