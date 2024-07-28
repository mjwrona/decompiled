// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.PackageSqlResourceComponent2
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

namespace Microsoft.VisualStudio.Services.Feed.Server
{
  public class PackageSqlResourceComponent2 : PackageSqlResourceComponent
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
      this.PrepareStoredProcedure("Feed.prc_GetPackagesTopVersions");
      this.BindFeedIdentity(feed.GetIdentity());
      this.BindString("@protocolType", protocolType, 20, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindString("@packageNameQuery", packageNameQuery, (int) byte.MaxValue, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindPagingOptions(pagingOptions);
      this.BindNullableBoolean("@isListed", isListed);
      this.BindNullableBoolean("@isRelease", isRelease);
      this.BindNullableBoolean("@isDeleted", isDeleted);
      this.BindBoolean("@includeAllVersions", resultOptions.IncludeAllVersions);
      return this.ReadPackages(resultOptions.IncludeDescriptions);
    }

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
      this.BindString("@protocolType", protocolType, 20, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindString("@packageNameQuery", packageNameQuery, (int) byte.MaxValue, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindPagingOptions(pagingOptions);
      this.BindBoolean("@includeAllVersions", resultOptions.IncludeAllVersions);
      this.BindNullableBoolean("@isListed", isListed);
      this.BindNullableBoolean("@isRelease", isRelease);
      this.BindNullableBoolean("@isDeleted", isDeleted);
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
      this.BindString("@protocolType", protocolType, 20, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindString("@normalizedPackageName", normalizedPackageName, (int) byte.MaxValue, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindBoolean("@includeAllVersions", resultOptions.IncludeAllVersions);
      this.BindNullableBoolean("@isListed", isListed);
      this.BindNullableBoolean("@isRelease", isRelease);
      this.BindNullableBoolean("@isDeleted", isDeleted);
      return this.ReadPackages(resultOptions.IncludeDescriptions);
    }
  }
}
