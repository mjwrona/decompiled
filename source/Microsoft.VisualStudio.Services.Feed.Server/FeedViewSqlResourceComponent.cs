// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.FeedViewSqlResourceComponent
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
  public class FeedViewSqlResourceComponent : FeedSqlResourceComponentBase
  {
    public const int FeedViewNameMaxLength = 64;
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[6]
    {
      (IComponentCreator) new ComponentCreator<FeedViewSqlResourceComponent>(1),
      (IComponentCreator) new ComponentCreator<FeedViewSqlResourceComponent2>(2),
      (IComponentCreator) new ComponentCreator<FeedViewSqlResourceComponent3>(3),
      (IComponentCreator) new ComponentCreator<FeedViewSqlResourceComponent4>(4),
      (IComponentCreator) new ComponentCreator<FeedViewSqlResourceComponent5>(5),
      (IComponentCreator) new ComponentCreator<FeedViewSqlResourceComponent6>(6)
    }, "FeedView");
    private static Dictionary<int, SqlExceptionFactory> sqlExceptionFactories = new Dictionary<int, SqlExceptionFactory>();

    static FeedViewSqlResourceComponent()
    {
      FeedViewSqlResourceComponent.sqlExceptionFactories.Add(1620002, new SqlExceptionFactory(typeof (FeedIdNotFoundException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((requestContext, errorVal, sqlException, sqlError) => (Exception) FeedIdNotFoundException.Create(sqlError.ExtractString("feedIdString")))));
      FeedViewSqlResourceComponent.sqlExceptionFactories.Add(1620010, new SqlExceptionFactory(typeof (FeedViewNameAlreadyExistsException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((requestContext, errorVal, sqlException, sqlError) => (Exception) FeedViewNameAlreadyExistsException.Create(sqlError.ExtractString("feedViewName"), sqlError.ExtractString("feedIdString")))));
      FeedViewSqlResourceComponent.sqlExceptionFactories.Add(1620011, new SqlExceptionFactory(typeof (FeedViewNotFoundException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((requestContext, errorVal, sqlException, sqlError) => (Exception) FeedViewNotFoundException.Create(sqlError.ExtractString("feedIdString"), sqlError.ExtractString("feedViewIdString")))));
      FeedViewSqlResourceComponent.sqlExceptionFactories.Add(1620012, new SqlExceptionFactory(typeof (FeedViewNotReleasedException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((requestContext, errorVal, sqlException, sqlError) => (Exception) FeedViewNotReleasedException.Create(sqlError.ExtractString("feedIdString"), sqlError.ExtractString("viewName")))));
      FeedViewSqlResourceComponent.sqlExceptionFactories.Add(1620004, new SqlExceptionFactory(typeof (PackageVersionNotFoundException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((requestContext, errorVal, sqlException, sqlError) => (Exception) PackageVersionNotFoundException.CreateById(sqlError.ExtractString("packageIdString"), sqlError.ExtractString("packageVersionId"), Guid.Parse(sqlError.ExtractString("feedIdString"))))));
      FeedViewSqlResourceComponent.sqlExceptionFactories.Add(1620006, new SqlExceptionFactory(typeof (DatabaseStateIsNoLongerValidException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((requestContext, errorVal, sqlException, sqlError) => (Exception) new DatabaseStateIsNoLongerValidException())));
      FeedViewSqlResourceComponent.sqlExceptionFactories.Add(1620016, new SqlExceptionFactory(typeof (GenericDatabaseFailureException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((requestContext, errorVal, sqlException, sqlError) => (Exception) new GenericDatabaseFailureException())));
    }

    public FeedViewSqlResourceComponent()
    {
      this.SelectedFeatures |= SqlResourceComponentFeatures.BindPartitionIdByDefault;
      this.ContainerErrorCode = 50000;
    }

    protected override IDictionary<int, SqlExceptionFactory> TranslatedExceptions => (IDictionary<int, SqlExceptionFactory>) FeedViewSqlResourceComponent.sqlExceptionFactories;

    public virtual FeedView CreateFeedView(
      FeedIdentity feedId,
      Guid viewId,
      string viewName,
      FeedViewType viewType,
      FeedVisibility visibility)
    {
      this.PrepareStoredProcedure("Feed.prc_CreateFeedView");
      this.BindFeedIdentity(feedId);
      this.BindGuid("@viewId", viewId);
      this.BindString("@viewName", viewName, 64, false, SqlDbType.NVarChar);
      return this.ReadFeedView();
    }

    public virtual IEnumerable<FeedView> GetFeedViews(FeedIdentity feedId, bool? isDeleted = false)
    {
      this.PrepareStoredProcedure("Feed.prc_GetFeedView");
      this.BindFeedIdentity(feedId);
      return this.ReadFeedViews();
    }

    public virtual FeedView GetFeedView(FeedIdentity feedId, Guid viewId)
    {
      this.PrepareStoredProcedure("Feed.prc_GetFeedView");
      this.BindFeedIdentity(feedId);
      this.BindGuid("@viewId", viewId);
      return this.ReadFeedView();
    }

    public virtual FeedView GetFeedView(FeedIdentity feedId, string viewName)
    {
      this.PrepareStoredProcedure("Feed.prc_GetFeedView");
      this.BindFeedIdentity(feedId);
      this.BindString("@viewName", viewName, 64, BindStringBehavior.NullToEmptyString, SqlDbType.NVarChar);
      return this.ReadFeedView();
    }

    public virtual FeedView RenameFeedView(FeedIdentity feedId, Guid viewId, string viewName)
    {
      this.PrepareStoredProcedure("Feed.prc_RenameFeedView");
      this.BindFeedIdentity(feedId);
      this.BindGuid("@viewId", viewId);
      this.BindString("@viewName", viewName, 64, BindStringBehavior.NullToEmptyString, SqlDbType.NVarChar);
      return this.ReadFeedView();
    }

    public virtual void DeleteFeedView(FeedIdentity feedId, Guid viewId)
    {
      this.PrepareStoredProcedure("Feed.prc_DeleteFeedView");
      this.BindFeedIdentity(feedId);
      this.BindGuid("@viewId", viewId);
      this.ExecuteNonQuery();
    }

    public virtual int AddPackageVersionToView(
      FeedIdentity feedId,
      Guid viewId,
      Guid packageVersionId)
    {
      this.PrepareStoredProcedure("Feed.prc_AddPackageVersionToFeedView");
      this.BindInt("@dataspaceId", this.GetDataspaceId(Guid.Empty));
      this.BindGuid("@viewId", viewId);
      this.BindGuid("@packageVersionId", packageVersionId);
      return this.ExecuteNonQuery();
    }

    public virtual void RemovePackageVersionFromView(
      FeedIdentity feedId,
      Guid viewId,
      Guid packageVersionId)
    {
      this.PrepareStoredProcedure("Feed.prc_RemovePackageVersionFromFeedView");
      this.BindInt("@dataspaceId", this.GetDataspaceId(Guid.Empty));
      this.BindGuid("@viewId", viewId);
      this.BindGuid("@packageVersionId", packageVersionId);
      this.ExecuteNonQuery();
    }

    public virtual FeedView UpdateFeedView(FeedIdentity feedId, Guid viewId, FeedView updatedView) => (FeedView) null;

    public virtual bool CanGetAllNonDeletedFeedViewsForCollection => false;

    public virtual IEnumerable<(Guid FeedId, FeedView View)> GetAllNonDeletedFeedViewsInCollection() => throw new NotSupportedException();

    protected FeedView ReadFeedView() => this.ReadFeedViews().FirstOrDefault<FeedView>();

    protected IEnumerable<FeedView> ReadFeedViews()
    {
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<FeedView>((ObjectBinder<FeedView>) new FeedViewBinder());
        return (IEnumerable<FeedView>) resultCollection.GetCurrent<FeedView>().Items;
      }
    }

    public static FeedView DeepCloneFeedView(FeedView feedView) => FeedViewBinder.DeepCloneFeedView(feedView);
  }
}
