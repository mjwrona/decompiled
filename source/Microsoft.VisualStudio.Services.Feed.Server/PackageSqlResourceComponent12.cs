// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.PackageSqlResourceComponent12
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
using System.Linq;

namespace Microsoft.VisualStudio.Services.Feed.Server
{
  public class PackageSqlResourceComponent12 : PackageSqlResourceComponent11
  {
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
      this.PrepareStoredProcedure("Feed.prc_GetPackages");
      this.BindFeedIdentity(feed.GetIdentity());
      this.BindNullableGuid("@viewId", feed.View?.Id);
      this.BindString("@protocolType", protocolType, 20, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindString("@normalizedPackageName", normalizedPackageName, (int) byte.MaxValue, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindString("@packageNameQuery", packageNameQuery, (int) byte.MaxValue, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindPagingOptions(pagingOptions);
      this.BindNullableBoolean("@isListed", isListed);
      this.BindNullableBoolean("@isRelease", isRelease);
      this.BindNullableBoolean("@isDeleted", isDeleted);
      this.BindBoolean("@includeAllVersions", resultOptions.IncludeAllVersions);
      this.BindNullableBoolean("@isCached", isCached);
      this.BindNullableGuid("@directUpstreamSourceId", directUpstreamSourceId);
      return this.ReadPackages(resultOptions.IncludeDescriptions);
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
      this.PrepareStoredProcedure("Feed.prc_GetPackages");
      this.BindFeedIdentity(feed.GetIdentity());
      this.BindNullableGuid("@viewId", feed.View?.Id);
      this.BindString("@protocolType", protocolType, 20, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindString("@packageNameQuery", packageNameQuery, (int) byte.MaxValue, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindPagingOptions(pagingOptions);
      this.BindNullableBoolean("@isListed", isListed);
      this.BindNullableBoolean("@isRelease", isRelease);
      this.BindNullableBoolean("@isDeleted", isDeleted);
      this.BindNullableBoolean("@isCached", isCached);
      this.BindNullableGuid("@directUpstreamSourceId", directUpstreamSourceId);
      this.BindNullableBoolean("@includeAllVersions", new bool?(false));
      return this.ReadPackages(includeDescriptions);
    }

    public override IEnumerable<Guid> GetPackagesExceedingRetentionLimit(Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed, int countLimit)
    {
      this.PrepareStoredProcedure("Feed.prc_GetPackagesExceedingRetentionLimit");
      this.BindFeedIdentity(feed.GetIdentity());
      this.BindInt("@countLimit", countLimit);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<Guid>((ObjectBinder<Guid>) new ColumnBinder<Guid>("PackageId", (System.Func<IDataReader, Guid>) (reader => reader.GetGuid(0))));
        return (IEnumerable<Guid>) resultCollection.GetCurrent<Guid>().Items;
      }
    }

    public override IEnumerable<ProtocolPackageVersionIdentity> GetVersionsExceedingRetentionLimit(
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed,
      int countLimit,
      IEnumerable<Guid> packageIds)
    {
      this.PrepareStoredProcedure("Feed.prc_GetVersionsExceedingRetentionLimit");
      this.BindFeedIdentity(feed.GetIdentity());
      this.BindInt("@countLimit", countLimit);
      this.BindGuidTable("@packageIds", packageIds);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<ProtocolPackageVersionIdentity>((ObjectBinder<ProtocolPackageVersionIdentity>) new ProtocolPackageVersionIdentityBinder());
        return (IEnumerable<ProtocolPackageVersionIdentity>) resultCollection.GetCurrent<ProtocolPackageVersionIdentity>().Items;
      }
    }

    public override PackageStats GetPackageStats(Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed, Guid packageId)
    {
      this.PrepareStoredProcedure("Feed.prc_GetPackageStats");
      this.BindFeedIdentity(feed.GetIdentity());
      this.BindGuid("@packageId", packageId);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<PackageStats>((ObjectBinder<PackageStats>) new PackageStatsBinder());
        return resultCollection.GetCurrent<PackageStats>().Items.Single<PackageStats>();
      }
    }
  }
}
