// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.PackageSqlResourceComponent15
// Assembly: Microsoft.VisualStudio.Services.Feed.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55555083-3B79-4F4F-AA85-92D66019974E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.Common;
using Microsoft.VisualStudio.Services.Feed.WebApi.Internal;
using System;
using System.Data;

namespace Microsoft.VisualStudio.Services.Feed.Server
{
  public class PackageSqlResourceComponent15 : PackageSqlResourceComponent14
  {
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
      this.BindString("@provenance", provenance, -1, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      return this.ReadPackageIndexEntry();
    }
  }
}
