// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.PackageSqlResourceComponent18
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
  public class PackageSqlResourceComponent18 : PackageSqlResourceComponent17
  {
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
      if (this.UseQuerySproc && feed.Capabilities.HasFlag((Enum) FeedCapabilities.UpstreamV2))
      {
        bool? nullable = isCached;
        bool flag = true;
        return nullable.GetValueOrDefault() == flag & nullable.HasValue ? Enumerable.Empty<Package>() : this.QueryPackages(feed, protocolType, normalizedPackageName, packageNameQuery, pagingOptions, resultOptions, isListed, isRelease, isDeleted, directUpstreamSourceId);
      }
      this.PrepareStoredProcedure("Feed.prc_GetPackages");
      this.BindFeedIdentity(feed.GetIdentity(), false, "@dataspaceId", "@feedId");
      this.BindNullableGuid("@viewId", feed.View?.Id);
      this.BindString("@protocolType", protocolType, 20, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindString("@normalizedPackageName", normalizedPackageName, (int) byte.MaxValue, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindString("@packageNameQuery", packageNameQuery, (int) byte.MaxValue, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindPagingOptions(pagingOptions);
      this.BindBoolean("@includeAllVersions", resultOptions.IncludeAllVersions);
      this.BindNullableBoolean("@isListed", isListed);
      this.BindNullableBoolean("@isRelease", isRelease);
      this.BindNullableBoolean("@isDeleted", isDeleted);
      this.BindNullableBoolean("@isCached", isCached);
      this.BindNullableGuid("@directUpstreamSourceId", directUpstreamSourceId);
      return this.ReadPackages(resultOptions.IncludeDescriptions);
    }

    public override IEnumerable<Package> QueryPackages(
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed,
      string protocolType,
      string normalizedPackageName,
      string packageNameQuery,
      PagingOptions pagingOptions,
      ResultOptions resultOptions,
      bool? isListed,
      bool? isRelease,
      bool? isDeleted,
      Guid? directUpstreamSourceId)
    {
      this.PrepareStoredProcedure("Feed.prc_QueryPackages");
      this.BindFeedIdentity(feed.GetIdentity(), false, "@dataspaceId", "@feedGuid");
      this.BindNullableGuid("@viewId", feed.View?.Id);
      this.BindString("@protocolType", protocolType, 20, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindString("@normalizedPackageName", normalizedPackageName, (int) byte.MaxValue, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindString("@packageNameQuery", packageNameQuery, (int) byte.MaxValue, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindPagingOptions(pagingOptions);
      this.BindBoolean("@includeAllVersions", resultOptions.IncludeAllVersions);
      this.BindNullableBoolean("@isListed", isListed);
      this.BindNullableBoolean("@isRelease", isRelease);
      this.BindNullableBoolean("@isDeleted", isDeleted);
      this.BindNullableGuid("@directUpstreamSourceId", directUpstreamSourceId);
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
      if (!this.UseQuerySproc || !feed.Capabilities.HasFlag((Enum) FeedCapabilities.UpstreamV2))
        return base.GetPackagesTopVersions(feed, protocolType, normalizedPackageName, packageNameQuery, pagingOptions, resultOptions, isListed, isRelease, isDeleted, isCached, directUpstreamSourceId);
      bool? nullable = isCached;
      bool flag = true;
      if (nullable.GetValueOrDefault() == flag & nullable.HasValue)
        return Enumerable.Empty<Package>();
      pagingOptions.ApplyToVersions = new bool?(true);
      return this.QueryPackages(feed, protocolType, normalizedPackageName, packageNameQuery, pagingOptions, resultOptions, isListed, isRelease, isDeleted, directUpstreamSourceId);
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
      if (!this.UseQuerySproc || !feed.Capabilities.HasFlag((Enum) FeedCapabilities.UpstreamV2))
        return base.GetLatestPackages(feed, protocolType, packageNameQuery, pagingOptions, includeDescriptions, isListed, isRelease, isDeleted, isCached, directUpstreamSourceId);
      bool? nullable = isCached;
      bool flag = true;
      if (nullable.GetValueOrDefault() == flag & nullable.HasValue)
        return Enumerable.Empty<Package>();
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed1 = feed;
      string protocolType1 = protocolType;
      string packageNameQuery1 = packageNameQuery;
      PagingOptions pagingOptions1 = pagingOptions;
      ResultOptions resultOptions = new ResultOptions();
      resultOptions.IncludeAllVersions = false;
      resultOptions.IncludeDescriptions = includeDescriptions;
      bool? isListed1 = isListed;
      bool? isRelease1 = isRelease;
      bool? isDeleted1 = isDeleted;
      Guid? directUpstreamSourceId1 = directUpstreamSourceId;
      return this.QueryPackages(feed1, protocolType1, (string) null, packageNameQuery1, pagingOptions1, resultOptions, isListed1, isRelease1, isDeleted1, directUpstreamSourceId1);
    }

    public override IEnumerable<Package> GetPackage(
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed,
      Guid packageId,
      ResultOptions resultOptions,
      bool? isListed,
      bool? isRelease,
      bool? isDeleted)
    {
      if (!this.UseQuerySproc || !feed.Capabilities.HasFlag((Enum) FeedCapabilities.UpstreamV2))
        return base.GetPackage(feed, packageId, resultOptions, isListed, isRelease, isDeleted);
      this.PrepareStoredProcedure("Feed.prc_QueryPackages");
      this.BindFeedIdentity(feed.GetIdentity(), false, "@dataspaceId", "@feedGuid");
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
      if (!this.UseQuerySproc || !feed.Capabilities.HasFlag((Enum) FeedCapabilities.UpstreamV2))
        return base.GetPackage(feed, protocolType, normalizedPackageName, resultOptions, isListed, isRelease, isDeleted);
      this.PrepareStoredProcedure("Feed.prc_QueryPackages");
      this.BindFeedIdentity(feed.GetIdentity(), false, "@dataspaceId", "@feedGuid");
      this.BindNullableGuid("@viewId", feed.View?.Id);
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
