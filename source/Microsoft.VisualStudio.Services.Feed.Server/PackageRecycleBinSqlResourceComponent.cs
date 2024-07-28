// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.PackageRecycleBinSqlResourceComponent
// Assembly: Microsoft.VisualStudio.Services.Feed.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55555083-3B79-4F4F-AA85-92D66019974E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.Common;
using Microsoft.VisualStudio.Services.Feed.Server.Binders;
using Microsoft.VisualStudio.Services.Feed.Server.Types;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Feed.WebApi.AzureArtifacts;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Feed.Server
{
  public class PackageRecycleBinSqlResourceComponent : FeedSqlResourceComponentBase
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[7]
    {
      (IComponentCreator) new ComponentCreator<PackageRecycleBinSqlResourceComponent>(1),
      (IComponentCreator) new ComponentCreator<PackageRecycleBinSqlResourceComponent2>(2),
      (IComponentCreator) new ComponentCreator<PackageRecycleBinSqlResourceComponent3>(3),
      (IComponentCreator) new ComponentCreator<PackageRecycleBinSqlResourceComponent4>(4),
      (IComponentCreator) new ComponentCreator<PackageRecycleBinSqlResourceComponent5>(5),
      (IComponentCreator) new ComponentCreator<PackageRecycleBinSqlResourceComponent6>(6),
      (IComponentCreator) new ComponentCreator<PackageRecycleBinSqlResourceComponent7>(7)
    }, "PackagingRecycleBin");
    private static Dictionary<int, SqlExceptionFactory> sqlExceptionFactories = new Dictionary<int, SqlExceptionFactory>();

    static PackageRecycleBinSqlResourceComponent()
    {
      PackageRecycleBinSqlResourceComponent.sqlExceptionFactories.Add(1620004, new SqlExceptionFactory(typeof (PackageVersionNotFoundException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((requestContext, errorVal, sqlException, sqlError) => (Exception) PackageVersionNotFoundException.CreateById(sqlError.ExtractString("packageIdString"), sqlError.ExtractString("packageVersionId"), Guid.Parse(sqlError.ExtractString("feedIdString"))))));
      PackageRecycleBinSqlResourceComponent.sqlExceptionFactories.Add(1620015, new SqlExceptionFactory(typeof (PackageVersionNotFoundException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((requestContext, errorVal, sqlException, sqlError) => (Exception) PackageVersionNotFoundException.CreateByIdRecycleBin(sqlError.ExtractString("packageIdString"), sqlError.ExtractString("packageVersionId"), Guid.Parse(sqlError.ExtractString("feedIdString"))))));
      PackageRecycleBinSqlResourceComponent.sqlExceptionFactories.Add(1620009, new SqlExceptionFactory(typeof (UnknownDatabaseErrorOcurredException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((requestContext, errorVal, sqlException, sqlError) => (Exception) UnknownDatabaseErrorOcurredException.Create(sqlError.ExtractString("optionalMessageString")))));
    }

    public PackageRecycleBinSqlResourceComponent()
    {
      this.SelectedFeatures |= SqlResourceComponentFeatures.BindPartitionIdByDefault;
      this.ContainerErrorCode = 50000;
    }

    protected override IDictionary<int, SqlExceptionFactory> TranslatedExceptions => (IDictionary<int, SqlExceptionFactory>) PackageRecycleBinSqlResourceComponent.sqlExceptionFactories;

    public virtual IEnumerable<Package> GetPackages(
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed,
      string protocolType,
      string packageNameQuery,
      PagingOptions pagingOptions,
      DateTime now,
      bool includeAllVersions)
    {
      this.PrepareStoredProcedure("Feed.prc_GetRecycleBinPackages");
      this.BindFeedIdentity(feed.GetIdentity());
      this.BindString("@protocolType", protocolType, 20, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindString("@packageNameQuery", packageNameQuery, (int) byte.MaxValue, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindInt("@top", pagingOptions.Top);
      this.BindInt("@skip", pagingOptions.Skip);
      this.BindDateTime2("@now", now);
      return this.ReadPackages();
    }

    public virtual Package GetPackage(Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed, Guid packageId, DateTime now)
    {
      this.PrepareStoredProcedure("Feed.prc_GetRecycleBinPackages");
      this.BindFeedIdentity(feed.GetIdentity());
      this.BindGuid("@packageId", packageId);
      this.BindDateTime2("@now", now);
      return this.ReadPackages().SingleOrDefault<Package>();
    }

    public virtual IEnumerable<RecycleBinPackageVersion> GetPackageVersions(
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed,
      Guid packageId,
      DateTime now)
    {
      this.PrepareStoredProcedure("Feed.prc_GetRecycleBinPackageVersions");
      this.BindFeedIdentity(feed.GetIdentity());
      this.BindGuid("@packageId", packageId);
      this.BindDateTime2("@now", now);
      return this.ReadRecycleBinPackageVersions();
    }

    public virtual RecycleBinPackageVersion GetPackageVersion(
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed,
      Guid packageId,
      Guid packageVersionId,
      DateTime now)
    {
      this.PrepareStoredProcedure("Feed.prc_GetRecycleBinPackageVersions");
      this.BindFeedIdentity(feed.GetIdentity());
      this.BindGuid("@packageId", packageId);
      this.BindGuid("@packageVersionId", packageVersionId);
      this.BindDateTime2("@now", now);
      return this.ReadRecycleBinPackageVersions().SingleOrDefault<RecycleBinPackageVersion>();
    }

    public virtual int PermanentlyDeletePackageVersion(
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed,
      Guid packageId,
      Guid packageVersionId)
    {
      this.PrepareStoredProcedure("Feed.prc_PermanentlyDeletePackageVersion");
      this.BindFeedIdentity(feed.GetIdentity());
      this.BindGuid("@packageId", packageId);
      this.BindGuid("@packageVersionId", packageVersionId);
      return this.GetPackageVersionRowCount("DeletedCount");
    }

    public virtual int RestorePackageVersionToFeed(
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed,
      Guid packageId,
      Guid packageVersionId)
    {
      this.PrepareStoredProcedure("Feed.prc_RestorePackageVersionToFeed");
      this.BindFeedIdentity(feed.GetIdentity());
      this.BindGuid("@packageId", packageId);
      this.BindGuid("@packageVersionId", packageVersionId);
      return this.GetPackageVersionRowCount("RestoredCount");
    }

    public virtual IEnumerable<ProtocolPackageVersionIdentity> GetTopPackageVersions(
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed,
      DateTime deletedBefore,
      int top = 1000)
    {
      throw new NotImplementedException();
    }

    public virtual void BatchPermanentlyDeletePackageVersions(
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed,
      IEnumerable<ProtocolPackageVersionIdentity> identities)
    {
      throw new NotImplementedException();
    }

    public virtual IEnumerable<DeletedPackageVersion> GetAllPackageVersionsByFeed(Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed) => throw new NotImplementedException();

    protected int GetPackageVersionRowCount(string columnName)
    {
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<int>((ObjectBinder<int>) new SingleInt32ValueBinder(columnName));
        return resultCollection.GetCurrent<int>().Items[0];
      }
    }

    protected virtual IEnumerable<Package> ReadPackages()
    {
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        PackageVersionBindOptions bindOptions = PackageVersionBindOptions.IncludePackageDescriptionInMinimalPackageVersion | PackageVersionBindOptions.IncludeDetailsForDeletedVersions;
        resultCollection.AddBinder<Package>((ObjectBinder<Package>) new PackageBinder((IBindOnto<MinimalPackageVersion>) new MinimalPackageVersionBinder(bindOptions)));
        return (IEnumerable<Package>) resultCollection.GetCurrent<Package>().Items;
      }
    }

    protected virtual IEnumerable<RecycleBinPackageVersion> ReadRecycleBinPackageVersions()
    {
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        PackageVersionBindOptions bindOptions = PackageVersionBindOptions.IncludeDetailsForDeletedVersions;
        resultCollection.AddBinder<RecycleBinPackageVersion>((ObjectBinder<RecycleBinPackageVersion>) new BindOntoBinder<RecycleBinPackageVersion>((IBindOnto<RecycleBinPackageVersion>) new RecycleBinPackageVersionBinder((IBindOnto<PackageVersion>) new PackageVersionBinder(bindOptions, (IBindOnto<MinimalPackageVersion>) new MinimalPackageVersionBinder(bindOptions)))));
        return (IEnumerable<RecycleBinPackageVersion>) resultCollection.GetCurrent<RecycleBinPackageVersion>().Items;
      }
    }

    public virtual DateTime ScheduleImmediatePermanentDeletion(Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed, int batchSize = 1000) => DateTime.UtcNow;
  }
}
