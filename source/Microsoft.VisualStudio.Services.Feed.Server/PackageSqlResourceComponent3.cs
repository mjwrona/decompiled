// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.PackageSqlResourceComponent3
// Assembly: Microsoft.VisualStudio.Services.Feed.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55555083-3B79-4F4F-AA85-92D66019974E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Server.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.Common;
using Microsoft.VisualStudio.Services.Feed.Server.Types;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Feed.Server
{
  public class PackageSqlResourceComponent3 : PackageSqlResourceComponent2
  {
    protected override SqlMetaData[] Typ_PackageVersionUpdate => new SqlMetaData[4]
    {
      new SqlMetaData("PackageId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("NormalizedPackageVersion", SqlDbType.NVarChar, (long) sbyte.MaxValue),
      new SqlMetaData("SortableVersion", SqlDbType.NVarChar, (long) this.SortablePackageVersionMaxLength),
      new SqlMetaData("ProtocolMetadata", SqlDbType.NVarChar, -1L)
    };

    public override IEnumerable<Package> GetPackages(
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed,
      string protocolType,
      string normalizedPackageName,
      string packageNameQuery,
      PagingOptions pagingOptions,
      ResultOptions resultOptions,
      bool? isListed,
      bool? isRelease,
      bool? isDeleted,
      bool? isCached,
      Guid? directUpstreamSourceId)
    {
      this.PrepareStoredProcedure("Feed.prc_GetPackages");
      this.BindFeedIdentity(feed.GetIdentity());
      this.BindNullableGuid("@viewId", feed.View?.Id);
      this.BindString("@protocolType", protocolType, 20, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindString("@packageNameQuery", packageNameQuery, (int) byte.MaxValue, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindPagingOptions(pagingOptions);
      this.BindBoolean("@includeAllVersions", resultOptions.IncludeAllVersions);
      this.BindNullableBoolean("@isListed", isListed);
      this.BindNullableBoolean("@isRelease", isRelease);
      this.BindNullableBoolean("@isDeleted", isDeleted);
      return this.ReadPackages(resultOptions.IncludeDescriptions);
    }

    public override IEnumerable<Package> GetPackagesTopVersions(
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed,
      string protocolType,
      string normalizedPackageName,
      string packageNameQuery,
      PagingOptions pagingOptions,
      ResultOptions resultOptions,
      bool? isListed,
      bool? isRelease,
      bool? isDeleted,
      bool? isCached,
      Guid? directUpstreamSourceId)
    {
      pagingOptions.ApplyToVersions = new bool?(true);
      this.PrepareStoredProcedure("Feed.prc_GetPackagesTopVersions");
      this.BindFeedIdentity(feed.GetIdentity());
      this.BindNullableGuid("@viewId", feed.View?.Id);
      this.BindString("@protocolType", protocolType, 20, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindString("@packageNameQuery", packageNameQuery, (int) byte.MaxValue, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindPagingOptions(pagingOptions);
      this.BindNullableBoolean("@isListed", isListed);
      this.BindNullableBoolean("@isRelease", isRelease);
      this.BindNullableBoolean("@isDeleted", isDeleted);
      this.BindBoolean("@includeAllVersions", resultOptions.IncludeAllVersions);
      return this.ReadPackages(resultOptions.IncludeDescriptions);
    }

    public override IEnumerable<Package> GetPackage(
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed,
      Guid packageId,
      ResultOptions resultOptions,
      bool? isListed,
      bool? isRelease,
      bool? isDeleted)
    {
      this.PrepareStoredProcedure("Feed.prc_GetPackages");
      this.BindFeedIdentity(feed.GetIdentity());
      this.BindNullableGuid("@viewId", feed.View?.Id);
      this.BindGuid("@packageId", packageId);
      this.BindBoolean("@includeAllVersions", resultOptions.IncludeAllVersions);
      this.BindNullableBoolean("@isListed", isListed);
      this.BindNullableBoolean("@isRelease", isRelease);
      this.BindNullableBoolean("@isDeleted", isDeleted);
      return this.ReadPackages(resultOptions.IncludeDescriptions);
    }

    public override IEnumerable<Package> GetPackage(
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed,
      string protocolType,
      string normalizedPackageName,
      ResultOptions resultOptions,
      bool? isListed,
      bool? isRelease,
      bool? isDeleted)
    {
      this.PrepareStoredProcedure("Feed.prc_GetPackages");
      this.BindFeedIdentity(feed.GetIdentity());
      this.BindNullableGuid("@viewId", feed.View?.Id);
      this.BindString("@protocolType", protocolType, 20, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindString("@normalizedPackageName", normalizedPackageName, (int) byte.MaxValue, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindBoolean("@includeAllVersions", resultOptions.IncludeAllVersions);
      this.BindNullableBoolean("@isListed", isListed);
      this.BindNullableBoolean("@isRelease", isRelease);
      this.BindNullableBoolean("@isDeleted", isDeleted);
      return this.ReadPackages(resultOptions.IncludeDescriptions);
    }

    public override IEnumerable<PackageVersion> GetPackageVersions(
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed,
      Guid packageId,
      bool? isListed = null,
      string protocolType = null,
      bool? isDeleted = null)
    {
      this.PrepareStoredProcedure("Feed.prc_GetPackageVersions");
      this.BindFeedIdentity(feed.GetIdentity());
      this.BindNullableGuid("@viewId", feed.View?.Id);
      this.BindGuid("@packageId", packageId);
      this.BindNullableBoolean("@isListed", isListed);
      return this.ReadPackageVersions();
    }

    public override PackageVersion GetPackageVersion(
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed,
      Guid packageId,
      string normalizedPackageVersion)
    {
      this.PrepareStoredProcedure("Feed.prc_GetPackageVersions");
      this.BindFeedIdentity(feed.GetIdentity());
      this.BindNullableGuid("@viewId", feed.View?.Id);
      this.BindGuid("@packageId", packageId);
      this.BindString("@normalizedPackageVersion", normalizedPackageVersion, (int) byte.MaxValue, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      return this.ReadPackageVersions().SingleOrDefault<PackageVersion>();
    }

    public override PackageVersion GetPackageVersion(
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed,
      Guid packageId,
      Guid packageVersionId)
    {
      this.PrepareStoredProcedure("Feed.prc_GetPackageVersions");
      this.BindFeedIdentity(feed.GetIdentity());
      this.BindNullableGuid("@viewId", feed.View?.Id);
      this.BindGuid("@packageId", packageId);
      this.BindGuid("@packageVersionId", packageVersionId);
      return this.ReadPackageVersions().SingleOrDefault<PackageVersion>();
    }

    public override IEnumerable<Package> GetLatestPackages(
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed,
      string protocolType,
      string packageNameQuery,
      PagingOptions pagingOptions,
      bool includeDescriptions,
      bool? isListed,
      bool? isRelease,
      bool? isDeleted,
      bool? isCached,
      Guid? directUpstreamSourceId)
    {
      this.PrepareStoredProcedure("Feed.prc_GetLatestPackages");
      this.BindFeedIdentity(feed.GetIdentity());
      this.BindNullableGuid("@viewId", feed.View?.Id);
      this.BindString("@protocolType", protocolType, 20, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindString("@packageNameQuery", packageNameQuery, (int) byte.MaxValue, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindPagingOptions(pagingOptions);
      this.BindNullableBoolean("@isListed", isListed);
      this.BindNullableBoolean("@isRelease", isRelease);
      this.BindNullableBoolean("@isDeleted", isDeleted);
      return this.ReadPackages(includeDescriptions);
    }

    protected override SqlParameter BindPackageVersionBulkUpdateTable(
      string parameterName,
      IEnumerable<PackageVersionUpdate> updates)
    {
      updates = updates ?? Enumerable.Empty<PackageVersionUpdate>();
      System.Func<PackageVersionUpdate, SqlDataRecord> selector = (System.Func<PackageVersionUpdate, SqlDataRecord>) (update =>
      {
        SqlDataRecord record = new SqlDataRecord(this.Typ_PackageVersionUpdate);
        record.SetGuid(0, update.PackageId);
        record.SetString(1, update.NormalizedPackageVersion);
        record.SetString(2, update.SortablePackageVersion, BindStringBehavior.Unchanged);
        record.SetString(3, update.Metadata == null ? (string) null : JsonConvert.SerializeObject((object) update.Metadata), BindStringBehavior.Unchanged);
        return record;
      });
      return this.BindTable(parameterName, "Feed.typ_PackageVersionUpdate2", updates.Select<PackageVersionUpdate, SqlDataRecord>(selector));
    }
  }
}
