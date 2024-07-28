// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.PackageSqlResourceComponent
// Assembly: Microsoft.VisualStudio.Services.Feed.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55555083-3B79-4F4F-AA85-92D66019974E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Server.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.Common;
using Microsoft.VisualStudio.Services.Feed.Server.Types;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Feed.WebApi.Internal;
using Microsoft.VisualStudio.Services.Feed.WebApi.Internal.Contracts;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Feed.Server
{
  public class PackageSqlResourceComponent : FeedSqlResourceComponentBase
  {
    public const int PackageNameMaxLength = 255;
    public const int PackageNameQueryMaxLength = 255;
    public const int NormalizedPackageVersionMaxLength = 127;
    public const int PackageDescriptionMaxLength = -1;
    public const int PackageVersionMaxLength = 127;
    public const int PackageSummaryMaxLength = -1;
    public const int CreatedByMaxLength = 255;
    public const int ProtocolMetadataMaxLength = -1;
    public const int StorageIdMaxLength = 127;
    public const int TagsMaxLength = -1;
    public const int DependenciesMaxLength = -1;
    public const int ProvenanceMaxLength = -1;
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[18]
    {
      (IComponentCreator) new ComponentCreator<PackageSqlResourceComponent>(7),
      (IComponentCreator) new ComponentCreator<PackageSqlResourceComponent2>(8),
      (IComponentCreator) new ComponentCreator<PackageSqlResourceComponent3>(9),
      (IComponentCreator) new ComponentCreator<PackageSqlResourceComponent4>(10),
      (IComponentCreator) new ComponentCreator<PackageSqlResourceComponent5>(11),
      (IComponentCreator) new ComponentCreator<PackageSqlResourceComponent6>(12),
      (IComponentCreator) new ComponentCreator<PackageSqlResourceComponent7>(13),
      (IComponentCreator) new ComponentCreator<PackageSqlResourceComponent8>(14),
      (IComponentCreator) new ComponentCreator<PackageSqlResourceComponent9>(15),
      (IComponentCreator) new ComponentCreator<PackageSqlResourceComponent10>(16),
      (IComponentCreator) new ComponentCreator<PackageSqlResourceComponent11>(17),
      (IComponentCreator) new ComponentCreator<PackageSqlResourceComponent12>(18),
      (IComponentCreator) new ComponentCreator<PackageSqlResourceComponent13>(19),
      (IComponentCreator) new ComponentCreator<PackageSqlResourceComponent14>(20),
      (IComponentCreator) new ComponentCreator<PackageSqlResourceComponent15>(21),
      (IComponentCreator) new ComponentCreator<PackageSqlResourceComponent16>(22),
      (IComponentCreator) new ComponentCreator<PackageSqlResourceComponent17>(23),
      (IComponentCreator) new ComponentCreator<PackageSqlResourceComponent18>(24)
    }, "Packaging");
    private static Dictionary<int, SqlExceptionFactory> sqlExceptionFactories = new Dictionary<int, SqlExceptionFactory>();

    protected virtual int SortablePackageVersionMaxLength => -1;

    protected virtual SqlMetaData[] Typ_PackageVersionUpdate => new SqlMetaData[3]
    {
      new SqlMetaData("PackageId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("NormalizedPackageVersion", SqlDbType.NVarChar, (long) sbyte.MaxValue),
      new SqlMetaData("SortableVersion", SqlDbType.NVarChar, (long) this.SortablePackageVersionMaxLength)
    };

    static PackageSqlResourceComponent()
    {
      PackageSqlResourceComponent.sqlExceptionFactories.Add(1620005, new SqlExceptionFactory(typeof (PackageVersionNotFoundException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((requestContext, errorVal, sqlException, sqlError) => (Exception) PackageVersionNotFoundException.CreateByName(sqlError.ExtractString("packageIdString"), sqlError.ExtractString("normalizedPackageVersion"), Guid.Parse(sqlError.ExtractString("feedIdString"))))));
      PackageSqlResourceComponent.sqlExceptionFactories.Add(1620004, new SqlExceptionFactory(typeof (PackageVersionNotFoundException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((requestContext, errorVal, sqlException, sqlError) => (Exception) PackageVersionNotFoundException.CreateById(sqlError.ExtractString("packageIdString"), sqlError.ExtractString("packageVersionId"), Guid.Parse(sqlError.ExtractString("feedIdString"))))));
      PackageSqlResourceComponent.sqlExceptionFactories.Add(1620007, new SqlExceptionFactory(typeof (PackageVersionDeletedException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((requestContext, errorVal, sqlException, sqlError) => (Exception) PackageVersionDeletedException.CreateById(sqlError.ExtractString("packageIdString"), sqlError.ExtractString("packageVersionId"), sqlError.ExtractString("feedIdString")))));
      PackageSqlResourceComponent.sqlExceptionFactories.Add(1620008, new SqlExceptionFactory(typeof (PackageVersionDeletedException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((requestContext, errorVal, sqlException, sqlError) => (Exception) PackageVersionDeletedException.CreateByName(sqlError.ExtractString("packageIdString"), sqlError.ExtractString("normalizedPackageVersion"), sqlError.ExtractString("feedIdString")))));
      PackageSqlResourceComponent.sqlExceptionFactories.Add(1620009, new SqlExceptionFactory(typeof (UnknownDatabaseErrorOcurredException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((requestContext, errorVal, sqlException, sqlError) => (Exception) UnknownDatabaseErrorOcurredException.Create(sqlError.ExtractString("optionalMessageString")))));
      PackageSqlResourceComponent.sqlExceptionFactories.Add(1620017, new SqlExceptionFactory(typeof (UnknownDatabaseErrorOcurredException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((requestContext, errorVal, sqlException, sqlError) => (Exception) new UnknownDatabaseErrorOcurredException(sqlError.ExtractString("optionalMessageString")))));
    }

    public PackageSqlResourceComponent()
    {
      this.SelectedFeatures |= SqlResourceComponentFeatures.BindPartitionIdByDefault;
      this.ContainerErrorCode = 50000;
    }

    protected override IDictionary<int, SqlExceptionFactory> TranslatedExceptions => (IDictionary<int, SqlExceptionFactory>) PackageSqlResourceComponent.sqlExceptionFactories;

    public bool UseQuerySproc { get; set; }

    protected override void Initialize(
      IVssRequestContext requestContext,
      string dataspaceCategory,
      Guid dataspaceIdentifier,
      int serviceVersion,
      DatabaseConnectionType connectionType,
      ITFLogger logger)
    {
      this.UseQuerySproc = requestContext.IsFeatureEnabled("Packaging.Feed.UseQuerySproc");
      base.Initialize(requestContext, dataspaceCategory, dataspaceIdentifier, serviceVersion, connectionType, logger);
    }

    public virtual PackageIndexEntryResponse SetPackageVersion(
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
      return this.ReadPackageIndexEntry();
    }

    public virtual void UpdatePackageVersion(
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed,
      Guid packageId,
      Guid packageVersionId,
      bool? listed,
      string files)
    {
      this.PrepareStoredProcedure("Feed.prc_UpdatePackageVersion");
      this.BindFeedIdentity(feed.GetIdentity());
      this.BindGuid("@packageId", packageId);
      this.BindGuid("@packageVersionId", packageVersionId);
      this.BindNullableBoolean("@listed", listed);
      this.ExecuteNonQuery();
    }

    public virtual IEnumerable<Package> QueryPackages(
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
      throw new NotImplementedException();
    }

    public virtual IEnumerable<Package> GetPackages(
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
      return this.ReadPackages(resultOptions.IncludeDescriptions);
    }

    public virtual IEnumerable<Package> GetPackagesTopVersions(
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
      bool flag1 = resultOptions.IncludeAllVersions && !string.IsNullOrWhiteSpace(protocolType) && !isRelease.HasValue;
      bool flag2 = !string.IsNullOrWhiteSpace(packageNameQuery);
      bool flag3 = isListed.HasValue && isListed.Value;
      if (flag1 && !isListed.HasValue && !flag2)
        return this.GetAllTopVersionsFromAllPackagesWithinProtocol(feed, protocolType, pagingOptions, resultOptions);
      if (((!flag1 ? 0 : (!isListed.HasValue ? 1 : 0)) & (flag2 ? 1 : 0)) != 0)
        return this.GetAllTopVersionsFromQueriedPackagesWithinProtocol(feed, protocolType, packageNameQuery, pagingOptions, resultOptions);
      if (flag1 & flag3 && !flag2)
        return this.GetAllTopListedVersionsFromAllPackagesWithinProtocol(feed, protocolType, pagingOptions, resultOptions);
      return flag1 & flag3 & flag2 ? this.GetAllTopListedVersionsFromQueriedPackagesWithinProtocol(feed, protocolType, packageNameQuery, pagingOptions, resultOptions) : this.GetTopVersionsFromPackagesDynamic(feed, protocolType, packageNameQuery, pagingOptions, resultOptions, isListed, isRelease);
    }

    public virtual IEnumerable<Package> GetLatestPackages(
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
      return Enumerable.Empty<Package>();
    }

    public virtual IEnumerable<Package> GetPackage(
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
      return this.ReadPackages(resultOptions.IncludeDescriptions);
    }

    public virtual IEnumerable<Package> GetPackage(
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
      return this.ReadPackages(resultOptions.IncludeDescriptions);
    }

    public virtual IEnumerable<PackageDependencyDetails> GetPackageDependencyByName(
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed,
      string protocolType,
      IEnumerable<PackageDependency> packageDependencies)
    {
      return (IEnumerable<PackageDependencyDetails>) new List<PackageDependencyDetails>();
    }

    public virtual IEnumerable<PackageVersion> GetPackageVersions(
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed,
      Guid packageId,
      bool? isListed = null,
      string protocolType = null,
      bool? isDeleted = null)
    {
      this.PrepareStoredProcedure("Feed.prc_GetPackageVersions");
      this.BindFeedIdentity(feed.GetIdentity());
      this.BindGuid("@packageId", packageId);
      this.BindNullableBoolean("@isListed", isListed);
      return this.ReadPackageVersions();
    }

    public virtual PackageVersion GetPackageVersion(
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed,
      Guid packageId,
      string normalizedPackageVersion)
    {
      this.PrepareStoredProcedure("Feed.prc_GetPackageVersions");
      this.BindFeedIdentity(feed.GetIdentity());
      this.BindGuid("@packageId", packageId);
      this.BindString("@normalizedPackageVersion", normalizedPackageVersion, (int) byte.MaxValue, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      return this.ReadPackageVersions().SingleOrDefault<PackageVersion>();
    }

    public virtual PackageVersion GetPackageVersion(
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed,
      Guid packageId,
      Guid packageVersionId)
    {
      this.PrepareStoredProcedure("Feed.prc_GetPackageVersions");
      this.BindFeedIdentity(feed.GetIdentity());
      this.BindGuid("@packageId", packageId);
      this.BindGuid("@packageVersionId", packageVersionId);
      return this.ReadPackageVersions().SingleOrDefault<PackageVersion>();
    }

    public virtual void UpdatePackageVersions(
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed,
      IEnumerable<PackageVersionUpdate> packageVersionUpdates)
    {
      this.PrepareStoredProcedure("Feed.prc_BulkUpdatePackageVersion");
      this.BindFeedIdentity(feed.GetIdentity());
      this.BindPackageVersionBulkUpdateTable("@packageVersionUpdates", packageVersionUpdates);
      this.ExecuteNonQuery();
    }

    public virtual void ClearCachedPackages(Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed, string protocolType)
    {
      this.PrepareStoredProcedure("Feed.prc_ClearCachedPackages");
      this.BindFeedIdentity(feed.GetIdentity());
      this.BindString("@protocolType", protocolType, 20, BindStringBehavior.NullToEmptyString, SqlDbType.NVarChar);
      this.ExecuteNonQuery();
    }

    public virtual int DeletePackageVersion(
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed,
      Guid packageId,
      Guid packageVersionId,
      DateTime deletedDate,
      DateTime scheduledPermanentDeleteDate)
    {
      this.PrepareStoredProcedure("Feed.prc_DeletePackageVersion");
      this.BindFeedIdentity(feed.GetIdentity());
      this.BindGuid("@packageId", packageId);
      this.BindGuid("@packageVersionId", packageVersionId);
      this.BindDateTime2("@DeletedDate", deletedDate);
      this.ExecuteNonQuery();
      return 0;
    }

    public virtual IEnumerable<Guid> GetPackagesExceedingRetentionLimit(Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed, int countLimit) => Enumerable.Empty<Guid>();

    public virtual PackageStats GetPackageStats(Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed, Guid packageId) => throw new NotSupportedException();

    public virtual List<string> GetCachedPackages(Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed, string protocol) => throw new NotSupportedException();

    public virtual void UpgradeCachedPackages(
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed,
      IEnumerable<string> packages,
      IEnumerable<FeedView> views)
    {
      throw new NotSupportedException();
    }

    public virtual IEnumerable<ProtocolPackageVersionIdentity> GetVersionsExceedingRetentionLimit(
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed,
      int countLimit,
      IEnumerable<Guid> packageIds)
    {
      return Enumerable.Empty<ProtocolPackageVersionIdentity>();
    }

    public virtual IEnumerable<ProtocolPackageVersionIdentity> GetVersionsExceedingRetentionLimitV2(
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed,
      int countLimit,
      IEnumerable<Guid> packageIds,
      int daysToKeepRecentlyDownloadedPackages,
      DateTime packageMetricsEnabledTimestamp)
    {
      return Enumerable.Empty<ProtocolPackageVersionIdentity>();
    }

    private IEnumerable<Package> GetAllTopListedVersionsFromAllPackagesWithinProtocol(
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed,
      string protocolType,
      PagingOptions pagingOptions,
      ResultOptions resultOptions)
    {
      this.PrepareStoredProcedure("Feed.prc_GetAllTopListedVersionsFromAllPackagesWithinProtocol");
      this.BindFeedIdentity(feed.GetIdentity());
      this.BindString("@protocolType", protocolType, 20, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindPagingOptions(pagingOptions);
      return this.ReadPackages(resultOptions.IncludeDescriptions);
    }

    private IEnumerable<Package> GetAllTopListedVersionsFromQueriedPackagesWithinProtocol(
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed,
      string protocolType,
      string packageNameQuery,
      PagingOptions pagingOptions,
      ResultOptions resultOptions)
    {
      this.PrepareStoredProcedure("Feed.prc_GetAllTopListedVersionsFromQueriedPackagesWithinProtocol");
      this.BindFeedIdentity(feed.GetIdentity());
      this.BindString("@protocolType", protocolType, 20, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindString("@packageNameQuery", packageNameQuery, (int) byte.MaxValue, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindPagingOptions(pagingOptions);
      return this.ReadPackages(resultOptions.IncludeDescriptions);
    }

    private IEnumerable<Package> GetAllTopVersionsFromAllPackagesWithinProtocol(
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed,
      string protocolType,
      PagingOptions pagingOptions,
      ResultOptions resultOptions)
    {
      this.PrepareStoredProcedure("Feed.prc_GetAllTopVersionsFromAllPackagesWithinProtocol");
      this.BindFeedIdentity(feed.GetIdentity());
      this.BindString("@protocolType", protocolType, 20, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindPagingOptions(pagingOptions);
      return this.ReadPackages(resultOptions.IncludeDescriptions);
    }

    private IEnumerable<Package> GetAllTopVersionsFromQueriedPackagesWithinProtocol(
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed,
      string protocolType,
      string packageNameQuery,
      PagingOptions pagingOptions,
      ResultOptions resultOptions)
    {
      this.PrepareStoredProcedure("Feed.prc_GetAllTopVersionsFromQueriedPackagesWithinProtocol");
      this.BindFeedIdentity(feed.GetIdentity());
      this.BindString("@protocolType", protocolType, 20, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindString("@packageNameQuery", packageNameQuery, (int) byte.MaxValue, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindPagingOptions(pagingOptions);
      return this.ReadPackages(resultOptions.IncludeDescriptions);
    }

    private IEnumerable<Package> GetTopVersionsFromPackagesDynamic(
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed,
      string protocolType,
      string packageNameQuery,
      PagingOptions pagingOptions,
      ResultOptions resultOptions,
      bool? isListed,
      bool? isRelease)
    {
      pagingOptions.ApplyToVersions = new bool?(true);
      this.PrepareStoredProcedure("Feed.prc_GetPackagesTopVersions");
      this.BindFeedIdentity(feed.GetIdentity());
      this.BindString("@protocolType", protocolType, 20, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindString("@packageNameQuery", packageNameQuery, (int) byte.MaxValue, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindPagingOptions(pagingOptions);
      this.BindNullableBoolean("@isListed", isListed);
      this.BindNullableBoolean("@isRelease", isRelease);
      this.BindBoolean("@includeAllVersions", resultOptions.IncludeAllVersions);
      return this.ReadPackages(resultOptions.IncludeDescriptions);
    }

    protected virtual IEnumerable<Package> ReadPackages(bool includeDescriptions)
    {
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        PackageVersionBindOptions bindOptions = includeDescriptions ? PackageVersionBindOptions.IncludePackageDescriptionInMinimalPackageVersion : PackageVersionBindOptions.None;
        resultCollection.AddBinder<Package>((ObjectBinder<Package>) new PackageBinder((IBindOnto<MinimalPackageVersion>) new MinimalPackageVersionBinder(bindOptions)));
        Dictionary<Guid, Package> dictionary = new Dictionary<Guid, Package>();
        foreach (Package package in resultCollection.GetCurrent<Package>())
        {
          if (dictionary.ContainsKey(package.Id))
          {
            (dictionary[package.Id].Versions as List<MinimalPackageVersion>).AddRange(package.Versions);
          }
          else
          {
            dictionary[package.Id] = package;
            package.Versions = (IEnumerable<MinimalPackageVersion>) new List<MinimalPackageVersion>(package.Versions);
          }
        }
        return (IEnumerable<Package>) dictionary.Values;
      }
    }

    protected virtual IEnumerable<PackageVersion> ReadPackageVersions()
    {
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        PackageVersionBindOptions bindOptions = PackageVersionBindOptions.None;
        resultCollection.AddBinder<PackageVersion>((ObjectBinder<PackageVersion>) new BindOntoBinder<PackageVersion>((IBindOnto<PackageVersion>) new PackageVersionBinder(bindOptions, (IBindOnto<MinimalPackageVersion>) new MinimalPackageVersionBinder(bindOptions))));
        return (IEnumerable<PackageVersion>) resultCollection.GetCurrent<PackageVersion>().Items;
      }
    }

    protected virtual SqlParameter BindPackageVersionBulkUpdateTable(
      string parameterName,
      IEnumerable<PackageVersionUpdate> updates)
    {
      updates = updates ?? Enumerable.Empty<PackageVersionUpdate>();
      System.Func<PackageVersionUpdate, SqlDataRecord> selector = (System.Func<PackageVersionUpdate, SqlDataRecord>) (update =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(this.Typ_PackageVersionUpdate);
        sqlDataRecord.SetGuid(0, update.PackageId);
        sqlDataRecord.SetString(1, update.NormalizedPackageVersion);
        sqlDataRecord.SetString(2, update.SortablePackageVersion);
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "Feed.typ_PackageVersionUpdate", updates.Select<PackageVersionUpdate, SqlDataRecord>(selector));
    }

    protected PackageIndexEntryResponse ReadPackageIndexEntry()
    {
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<PackageIndexEntryResponse>((ObjectBinder<PackageIndexEntryResponse>) new PackageIndexEntryResponseBinder());
        return resultCollection.GetCurrent<PackageIndexEntryResponse>().Items[0];
      }
    }

    protected virtual void BindPagingOptions(PagingOptions pagingOptions)
    {
      pagingOptions = pagingOptions ?? new PagingOptions();
      this.BindInt("@top", pagingOptions.Top);
      this.BindInt("@skip", pagingOptions.Skip);
      if (!pagingOptions.ApplyToVersions.HasValue)
        return;
      this.BindBoolean("@countIndividualVersions", pagingOptions.ApplyToVersions.Value);
    }
  }
}
