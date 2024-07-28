// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.PackageSqlResourceComponent9
// Assembly: Microsoft.VisualStudio.Services.Feed.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55555083-3B79-4F4F-AA85-92D66019974E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.Common;
using Microsoft.VisualStudio.Services.Feed.Server.Types;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Feed.WebApi.Internal;
using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.VisualStudio.Services.Feed.Server
{
  public class PackageSqlResourceComponent9 : PackageSqlResourceComponent8
  {
    public const int SourceChainLength = -1;

    public override PackageIndexEntryResponse SetPackageVersion(
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed,
      string protocolType,
      string normalizedPackageName,
      string packageName,
      string packageProtocolMetadata,
      string normalizedPackageVersion,
      string packageVersion,
      string packageDescription,
      string packageSummary,
      string createdBy,
      DateTime? createdDate,
      string storageId,
      string tags,
      string dependencies,
      string versionProtocolMetadata,
      string sortablePackageVersion,
      bool isRelease,
      string files,
      bool isCachedVersion,
      string sourceChain,
      Guid? directUpstreamSourceId,
      string provenance)
    {
      this.PrepareStoredProcedure("Feed.prc_SetIndexEntry");
      this.BindFeedIdentity(feed.GetIdentity());
      this.BindString("@protocolType", protocolType, (int) byte.MaxValue, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindString("@normalizedPackageName", normalizedPackageName, (int) byte.MaxValue, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindString("@packageName", packageName, (int) byte.MaxValue, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindString("@normalizedPackageVersion", normalizedPackageVersion, (int) sbyte.MaxValue, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindString("@packageVersion", packageVersion, (int) sbyte.MaxValue, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindString("@packageDescription", packageDescription, -1, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindString("@packageSummary", packageSummary, -1, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindString("@createdBy", createdBy, (int) byte.MaxValue, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindNullableDateTime2("@createdDate", createdDate);
      this.BindString("@storageId", storageId, (int) sbyte.MaxValue, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindString("@tags", tags, -1, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindString("@dependencies", dependencies, -1, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindString("@versionProtocolMetadata", versionProtocolMetadata, -1, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindString("@sortablePackageVersion", sortablePackageVersion, this.SortablePackageVersionMaxLength, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindBoolean("@isRelease", isRelease);
      this.BindString("@files", files, -1, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindBoolean("@isCachedVersion", isCachedVersion);
      this.BindString("@sourceChain", sourceChain, -1, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindNullableGuid("@directUpstreamSourceId", directUpstreamSourceId);
      return this.ReadPackageIndexEntry();
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
      this.PrepareStoredProcedure("Feed.prc_GetLatestPackages");
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
      return this.ReadPackages(includeDescriptions);
    }
  }
}
