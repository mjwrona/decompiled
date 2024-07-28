// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.PackageSqlResourceComponent13
// Assembly: Microsoft.VisualStudio.Services.Feed.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55555083-3B79-4F4F-AA85-92D66019974E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Server.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.Common;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Feed.Server
{
  public class PackageSqlResourceComponent13 : PackageSqlResourceComponent12
  {
    public override List<string> GetCachedPackages(Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed, string protocolType)
    {
      this.PrepareStoredProcedure("Feed.prc_GetCachedPackages");
      this.BindFeedIdentity(feed.GetIdentity());
      this.BindString("@protocolType", protocolType, 20, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<string>((ObjectBinder<string>) new CachedPackageBinder());
        return resultCollection.GetCurrent<string>().Items;
      }
    }

    public override void UpgradeCachedPackages(
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed,
      IEnumerable<string> packages,
      IEnumerable<FeedView> views)
    {
      this.PrepareStoredProcedure("Feed.prc_UpgradeCachedPackages");
      this.BindFeedIdentity(feed.GetIdentity());
      this.BindStringIdList("@packages", packages);
      this.BindViewList("@views", views);
      this.BindBoolean("@isPinned", views.Any<FeedView>((System.Func<FeedView, bool>) (v => v.Type != FeedViewType.Implicit)));
      this.ExecuteNonQuery();
    }

    protected virtual SqlParameter BindStringIdList(string parameterName, IEnumerable<string> ids)
    {
      ids = ids ?? Enumerable.Empty<string>();
      System.Func<string, SqlDataRecord> selector = (System.Func<string, SqlDataRecord>) (id =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(this.Typ_StringIdList);
        sqlDataRecord.SetString(0, id);
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "Feed.Typ_StringIdList", ids.Select<string, SqlDataRecord>(selector));
    }

    protected virtual SqlParameter BindViewList(string parameterName, IEnumerable<FeedView> views)
    {
      views = views ?? Enumerable.Empty<FeedView>();
      System.Func<FeedView, SqlDataRecord> selector = (System.Func<FeedView, SqlDataRecord>) (view =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(this.Typ_ViewList);
        sqlDataRecord.SetGuid(0, view.Id);
        sqlDataRecord.SetString(1, view.Name);
        sqlDataRecord.SetInt32(2, (int) view.Type);
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "Feed.Typ_ViewList", views.Select<FeedView, SqlDataRecord>(selector));
    }

    protected virtual SqlMetaData[] Typ_StringIdList => new SqlMetaData[1]
    {
      new SqlMetaData("StringId", SqlDbType.NVarChar, (long) byte.MaxValue)
    };

    protected virtual SqlMetaData[] Typ_ViewList => new SqlMetaData[3]
    {
      new SqlMetaData("Id", SqlDbType.UniqueIdentifier),
      new SqlMetaData("Name", SqlDbType.NVarChar, 64L),
      new SqlMetaData("Type", SqlDbType.Int)
    };
  }
}
