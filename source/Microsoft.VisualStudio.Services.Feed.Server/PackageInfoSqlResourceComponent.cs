// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.PackageInfoSqlResourceComponent
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
  public class PackageInfoSqlResourceComponent : FeedSqlResourceComponentBase
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[1]
    {
      (IComponentCreator) new ComponentCreator<PackageInfoSqlResourceComponent>(1)
    }, "PackageInfo");
    private static Dictionary<int, SqlExceptionFactory> sqlExceptionFactories = new Dictionary<int, SqlExceptionFactory>();

    static PackageInfoSqlResourceComponent()
    {
      PackageInfoSqlResourceComponent.sqlExceptionFactories.Add(1620009, new SqlExceptionFactory(typeof (UnknownDatabaseErrorOcurredException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((requestContext, errorVal, sqlException, sqlError) => (Exception) UnknownDatabaseErrorOcurredException.Create(sqlError.ExtractString("optionalMessageString")))));
      PackageInfoSqlResourceComponent.sqlExceptionFactories.Add(1620017, new SqlExceptionFactory(typeof (UnknownDatabaseErrorOcurredException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((requestContext, errorVal, sqlException, sqlError) => (Exception) new UnknownDatabaseErrorOcurredException(sqlError.ExtractString("optionalMessageString")))));
    }

    public PackageInfoSqlResourceComponent()
    {
      this.SelectedFeatures |= SqlResourceComponentFeatures.BindPartitionIdByDefault;
      this.ContainerErrorCode = 50000;
    }

    protected override IDictionary<int, SqlExceptionFactory> TranslatedExceptions => (IDictionary<int, SqlExceptionFactory>) PackageInfoSqlResourceComponent.sqlExceptionFactories;

    protected override void BindFeedIdentity(
      FeedIdentity feedId,
      bool createMissingDataspace = false,
      string dataspaceName = "@dataspaceId",
      string feedIdName = "@feedId")
    {
      this.BindInt(dataspaceName, this.GetFeedDataspaceId(feedId.ProjectId, createMissingDataspace));
      this.BindGuid(feedIdName, feedId.Id);
    }

    protected virtual IPackageInfo ReadPackageInfo()
    {
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<IPackageInfo>((ObjectBinder<IPackageInfo>) new PackageInfoBinder());
        return resultCollection.GetCurrent<IPackageInfo>().Items.SingleOrDefault<IPackageInfo>();
      }
    }

    public IPackageInfo GetPackageByName(
      FeedIdentity feedId,
      string protocolType,
      string normalizedName)
    {
      this.PrepareStoredProcedure("Feed.prc_GetPackageInfoByName");
      this.BindFeedIdentity(feedId, false, "@dataspaceId", "@feedId");
      this.BindString("@protocolType", protocolType, 20, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindString("@normalizedPackageName", normalizedName, (int) byte.MaxValue, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      return this.ReadPackageInfo();
    }

    public IPackageInfo GetPackageById(FeedIdentity feedId, Guid packageId)
    {
      this.PrepareStoredProcedure("Feed.prc_GetPackageInfoById");
      this.BindFeedIdentity(feedId, false, "@dataspaceId", "@feedId");
      this.BindGuid("@packageId", packageId);
      return this.ReadPackageInfo();
    }
  }
}
