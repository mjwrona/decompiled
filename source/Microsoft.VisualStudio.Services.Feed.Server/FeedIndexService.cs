// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.FeedIndexService
// Assembly: Microsoft.VisualStudio.Services.Feed.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55555083-3B79-4F4F-AA85-92D66019974E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Server.dll

using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Feed.Common;
using Microsoft.VisualStudio.Services.Feed.Common.Notification;
using Microsoft.VisualStudio.Services.Feed.Server.Provenance;
using Microsoft.VisualStudio.Services.Feed.Server.Types;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Feed.WebApi.Internal;
using Microsoft.VisualStudio.Services.Feed.WebApi.Internal.Contracts;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Feed.Server
{
  public class FeedIndexService : IFeedIndexService, IVssFrameworkService
  {
    private const int MaxRetries = 10;
    private static readonly TimeSpan MaxRetryDelay = TimeSpan.FromSeconds(1.0);

    void IVssFrameworkService.ServiceStart(IVssRequestContext requestContext) => requestContext.TraceBlock(10019200, 10019201, 10019202, "FeedIndex", "Service", (Action) (() => ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext))), "ServiceStart");

    void IVssFrameworkService.ServiceEnd(IVssRequestContext requestContext) => requestContext.TraceBlock(10019203, 10019204, 10019205, "FeedIndex", "Service", (Action) (() => ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext))), "ServiceEnd");

    public IEnumerable<Package> GetPackages(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed,
      string protocolType = null,
      string nameQuery = null,
      string normalizedPackageName = null,
      PagingOptions pagingOptions = null,
      ResultOptions resultOptions = null,
      bool? isListed = null,
      bool getTopPackageVersions = false,
      bool? isRelease = null,
      bool? isDeleted = false,
      bool? isCached = null,
      Guid? directUpstreamSourceId = null)
    {
      return requestContext.TraceBlock<IEnumerable<Package>>(10019209, 10019210, 10019211, "FeedIndex", "Service", (Func<IEnumerable<Package>>) (() =>
      {
        feed.ThrowIfNull<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>((Func<Exception>) (() => (Exception) InvalidUserInputException.Create(nameof (feed))));
        ResultOptions resultOptions1 = resultOptions;
        if (resultOptions1 == null)
          resultOptions1 = new ResultOptions()
          {
            IncludeAllVersions = false,
            IncludeDescriptions = false
          };
        resultOptions = resultOptions1;
        FeedIndexService.ValidateIncludeDescriptionAllVersions(requestContext, feed, resultOptions);
        pagingOptions = pagingOptions ?? new PagingOptions();
        pagingOptions.Validate();
        FeedSecurityHelper.CheckReadFeedPermissions(requestContext, (FeedCore) feed);
        if (!feed.UpstreamEnabled)
          isCached = new bool?(false);
        IEnumerable<Package> packages;
        using (PackageSqlResourceComponent component = requestContext.CreateComponent<PackageSqlResourceComponent>())
        {
          if (feed.View != null && !resultOptions.IncludeAllVersions)
          {
            if (!string.IsNullOrWhiteSpace(normalizedPackageName))
              throw new IncompatibleQueryParametersException(Resources.Error_NormalizedNameNotSupported());
            packages = component.GetLatestPackages(feed, protocolType, nameQuery, pagingOptions, resultOptions.IncludeDescriptions, isListed, isRelease, isDeleted, isCached, directUpstreamSourceId);
          }
          else
          {
            if (!string.IsNullOrWhiteSpace(normalizedPackageName))
            {
              if (string.IsNullOrWhiteSpace(protocolType))
                throw new IncompatibleQueryParametersException(Resources.Error_NormalizedMatchRequiresProtocol());
              if (!string.IsNullOrWhiteSpace(nameQuery))
                throw new IncompatibleQueryParametersException(Resources.Error_CannotHaveBothNormalizedNameAndNameQuery());
              if (directUpstreamSourceId.HasValue)
                throw new IncompatibleQueryParametersException(Resources.Error_CannotHaveBothNormalizedNameAndDirectUpstreamSourceId());
            }
            packages = !getTopPackageVersions ? component.GetPackages(feed, protocolType, normalizedPackageName, nameQuery, pagingOptions, resultOptions, isListed, isRelease, isDeleted, isCached, directUpstreamSourceId) : component.GetPackagesTopVersions(feed, protocolType, normalizedPackageName, nameQuery, pagingOptions, resultOptions, isListed, isRelease, isDeleted, isCached, directUpstreamSourceId);
          }
        }
        return packages.IsNullOrEmpty<Package>() ? (IEnumerable<Package>) new List<Package>() : packages.Where<Package>((Func<Package, bool>) (p => FeatureFlagFeedSettingProvider.IsProtocolFeatureFlagEnabled(requestContext, p.ProtocolType)));
      }), nameof (GetPackages));
    }

    public virtual Package GetPackage(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed,
      string packageId,
      ResultOptions resultOptions = null,
      bool? isListed = null,
      bool? isRelease = null,
      bool? isDeleted = false)
    {
      return requestContext.TraceBlock<Package>(10019206, 10019207, 10019208, "FeedIndex", "Service", (Func<Package>) (() =>
      {
        feed.ThrowIfNull<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>((Func<Exception>) (() => (Exception) InvalidUserInputException.Create(nameof (feed))));
        packageId.ThrowIfNullOrWhitespace((Func<Exception>) (() => (Exception) InvalidUserInputException.Create(nameof (packageId))));
        ResultOptions resultOptions1 = resultOptions;
        if (resultOptions1 == null)
          resultOptions1 = new ResultOptions()
          {
            IncludeAllVersions = false,
            IncludeDescriptions = false
          };
        resultOptions = resultOptions1;
        FeedIndexService.ValidateIncludeDescriptionAllVersions(requestContext, feed, resultOptions);
        FeedSecurityHelper.CheckReadFeedPermissions(requestContext, (FeedCore) feed);
        return this.GetPackageInternal(requestContext, feed, packageId, resultOptions, isListed, isRelease, isDeleted).Single<Package>();
      }), nameof (GetPackage));
    }

    public virtual IEnumerable<PackageVersion> GetPackageVersions(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed,
      string packageId,
      bool? isListed,
      bool? isDeleted = null)
    {
      return (IEnumerable<PackageVersion>) requestContext.TraceBlock<List<PackageVersion>>(10019215, 10019216, 10019217, "FeedIndex", "Service", (Func<List<PackageVersion>>) (() =>
      {
        feed.ThrowIfNull<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>((Func<Exception>) (() => (Exception) InvalidUserInputException.Create(nameof (feed))));
        packageId.ThrowIfNullOrWhitespace((Func<Exception>) (() => (Exception) InvalidUserInputException.Create(nameof (packageId))));
        FeedSecurityHelper.CheckReadFeedPermissions(requestContext, (FeedCore) feed);
        FeedView view = feed.View;
        if (view != null)
          feed.View = (FeedView) null;
        IPackageInfo packageInfo = this.GetPackageInfo(requestContext, feed, packageId);
        if (view != null)
          feed.View = view;
        return this.GetPackageVersionsInternal(requestContext, feed, packageInfo, isListed, isDeleted).ToList<PackageVersion>();
      }), nameof (GetPackageVersions));
    }

    public PackageVersion GetPackageVersion(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed,
      string packageId,
      string packageVersionId,
      bool? isListed = null,
      bool? isDeleted = null)
    {
      return requestContext.TraceBlock<PackageVersion>(10019212, 10019213, 10019214, "FeedIndex", "Service", (Func<PackageVersion>) (() =>
      {
        feed.ThrowIfNull<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>((Func<Exception>) (() => (Exception) InvalidUserInputException.Create(nameof (feed))));
        packageId.ThrowIfNullOrWhitespace((Func<Exception>) (() => (Exception) InvalidUserInputException.Create(nameof (packageId))));
        packageVersionId.ThrowIfNullOrWhitespace((Func<Exception>) (() => (Exception) InvalidUserInputException.Create(nameof (packageVersionId))));
        FeedSecurityHelper.CheckReadFeedPermissions(requestContext, (FeedCore) feed);
        IVssRequestContext requestContext1 = requestContext;
        Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed1 = feed;
        string packageRequestId = packageId;
        ResultOptions resultOptions = new ResultOptions();
        resultOptions.IncludeAllVersions = true;
        bool? isListed1 = isListed;
        bool? nullable = isDeleted;
        bool? isRelease = new bool?();
        bool? isDeleted1 = nullable;
        Package package = this.GetPackageInternal(requestContext1, feed1, packageRequestId, resultOptions, isListed1, isRelease, isDeleted1).ToList<Package>().Single<Package>();
        PackageVersion packageVersionInternal = this.GetPackageVersionInternal(requestContext, feed, (IPackageInfo) package, packageVersionId);
        packageVersionInternal.OtherVersions = package.Versions;
        return packageVersionInternal;
      }), nameof (GetPackageVersion));
    }

    public IEnumerable<PackageDependencyDetails> GetPackageVersionDependencies(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed,
      Guid packageId,
      Guid packageVersionId,
      string protocolType)
    {
      return (IEnumerable<PackageDependencyDetails>) requestContext.TraceBlock<List<PackageDependencyDetails>>(10019249, 10019250, 10019251, "FeedIndex", "Service", (Func<List<PackageDependencyDetails>>) (() =>
      {
        FeedSecurityHelper.CheckReadFeedPermissions(requestContext, (FeedCore) feed);
        PackageVersion versionByVersionId = this.GetPackageVersionByVersionId(requestContext, feed, packageId, packageVersionId);
        if (versionByVersionId == null || versionByVersionId.Dependencies.IsNullOrEmpty<PackageDependency>())
          return new List<PackageDependencyDetails>();
        List<PackageDependencyDetails> list = this.GetPackageDependencyByName(requestContext, feed, protocolType, versionByVersionId.Dependencies).ToList<PackageDependencyDetails>();
        if (list.IsNullOrEmpty<PackageDependencyDetails>())
          return new List<PackageDependencyDetails>();
        int index = 0;
        foreach (PackageDependency dependency in versionByVersionId.Dependencies)
        {
          list[index].Group = dependency.Group;
          list[index].VersionRange = dependency.VersionRange;
          ++index;
        }
        return list;
      }), nameof (GetPackageVersionDependencies));
    }

    public PackageIndexEntryResponse SetIndexEntry(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed,
      PackageIndexEntry indexEntry)
    {
      return requestContext.TraceBlock<PackageIndexEntryResponse>(10019218, 10019219, 10019220, "FeedIndex", "Service", (Func<PackageIndexEntryResponse>) (() =>
      {
        feed.ThrowIfNull<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>((Func<Exception>) (() => (Exception) InvalidUserInputException.Create(nameof (feed))));
        indexEntry.ThrowIfNull<PackageIndexEntry>((Func<Exception>) (() => (Exception) InvalidUserInputException.Create(nameof (indexEntry))));
        FeedSecurityHelper.CheckModifyIndexPermissions(requestContext, (FeedCore) feed);
        RetryHelper<PackageIndexEntryResponse> retryHelper = new RetryHelper<PackageIndexEntryResponse>(requestContext, 10, FeedIndexService.MaxRetryDelay, new Func<Exception, bool>(this.IsUniqueIndexException));
        this.ValidateProvenance(requestContext, indexEntry.PackageVersion);
        Func<PackageIndexEntryResponse> function = (Func<PackageIndexEntryResponse>) (() =>
        {
          string packageProtocolMetadata = indexEntry.PackageProtocolMetadata == null ? string.Empty : JsonConvert.SerializeObject((object) indexEntry.PackageProtocolMetadata);
          string versionProtocolMetadata = indexEntry.PackageVersion.VersionProtocolMetadata == null ? string.Empty : JsonConvert.SerializeObject((object) indexEntry.PackageVersion.VersionProtocolMetadata);
          string files = indexEntry.PackageVersion?.Files == null ? string.Empty : JsonConvert.SerializeObject((object) indexEntry.PackageVersion.Files);
          string tags = indexEntry.PackageVersion.Tags == null ? (string) null : JsonConvert.SerializeObject((object) indexEntry.PackageVersion.Tags);
          string dependencies = JsonConvert.SerializeObject((object) indexEntry.PackageVersion.Dependencies);
          string provenance = indexEntry.PackageVersion.Provenance != null ? JsonConvert.SerializeObject((object) indexEntry.PackageVersion.Provenance) : (string) null;
          IEnumerable<UpstreamSource> sourceChain4 = indexEntry.PackageVersion.SourceChain;
          Guid? nullable3 = sourceChain4 != null ? sourceChain4.FirstOrDefault<UpstreamSource>()?.Id : new Guid?();
          Guid empty = Guid.Empty;
          if ((nullable3.HasValue ? (nullable3.HasValue ? (nullable3.GetValueOrDefault() == empty ? 1 : 0) : 1) : 0) != 0)
            this.SetMissingSourceId(requestContext, feed, indexEntry.PackageVersion.SourceChain);
          string sourceChain5 = indexEntry.PackageVersion.SourceChain == null ? JsonConvert.SerializeObject((object) new List<UpstreamSource>()) : JsonConvert.SerializeObject((object) indexEntry.PackageVersion.SourceChain);
          IEnumerable<UpstreamSource> sourceChain6 = indexEntry.PackageVersion.SourceChain;
          Guid? nullable4;
          if (sourceChain6 == null)
          {
            nullable3 = new Guid?();
            nullable4 = nullable3;
          }
          else
          {
            UpstreamSource upstreamSource = sourceChain6.FirstOrDefault<UpstreamSource>();
            if (upstreamSource == null)
            {
              nullable3 = new Guid?();
              nullable4 = nullable3;
            }
            else
              nullable4 = new Guid?(upstreamSource.Id);
          }
          Guid? directUpstreamSourceId = nullable4;
          using (PackageSqlResourceComponent component = requestContext.CreateComponent<PackageSqlResourceComponent>())
            return component.SetPackageVersion(feed, indexEntry.ProtocolType, indexEntry.NormalizedName, indexEntry.Name, packageProtocolMetadata, indexEntry.PackageVersion.NormalizedVersion, indexEntry.PackageVersion.Version, indexEntry.PackageVersion.Description, indexEntry.PackageVersion.Summary, indexEntry.PackageVersion.Author, indexEntry.PackageVersion.PublishDate, indexEntry.PackageVersion.StorageId, tags, dependencies, versionProtocolMetadata, indexEntry.PackageVersion.SortableVersion, indexEntry.PackageVersion.IsRelease, files, indexEntry.PackageVersion.IsCached, sourceChain5, directUpstreamSourceId, provenance);
        });
        PackageIndexEntryResponse response = retryHelper.InvokeWithReturn(function);
        if (response.Created)
        {
          ITeamFoundationEventService service = requestContext.GetService<ITeamFoundationEventService>();
          service.PublishNotification(requestContext, (object) new DeprecatedPackageChangedEvent(feed, indexEntry, response));
          service.PublishNotification(requestContext, (object) new PackageChangedEvent(feed, indexEntry, response));
        }
        return response;
      }), nameof (SetIndexEntry));
    }

    public virtual void UpdatePackageVersion(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed,
      string packageId,
      string packageVersionId,
      PackageVersion packageVersion)
    {
      requestContext.TraceBlock(10019224, 10019225, 10019226, "FeedIndex", "Service", (Action) (() =>
      {
        feed.ThrowIfNull<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>((Func<Exception>) (() => (Exception) InvalidUserInputException.Create(nameof (feed))));
        packageId.ThrowIfNullOrWhitespace((Func<Exception>) (() => (Exception) InvalidUserInputException.Create(nameof (packageId))));
        packageVersionId.ThrowIfNullOrWhitespace((Func<Exception>) (() => (Exception) InvalidUserInputException.Create(nameof (packageVersionId))));
        packageVersion.ThrowIfNull<PackageVersion>((Func<Exception>) (() => (Exception) InvalidUserInputException.Create(nameof (packageVersion))));
        FeedSecurityHelper.CheckModifyIndexPermissions(requestContext, (FeedCore) feed);
        FeedView view1 = feed.View;
        if (view1 != null)
          feed.View = (FeedView) null;
        IPackageInfo packageInfo = this.GetPackageInfo(requestContext, feed, packageId);
        if (view1 != null)
          feed.View = view1;
        PackageVersion packageVersionInternal = this.GetPackageVersionInternal(requestContext, feed, packageInfo, packageVersionId);
        string files = packageVersion.Files == null ? string.Empty : JsonConvert.SerializeObject((object) packageVersion.Files);
        using (PackageSqlResourceComponent component = requestContext.CreateComponent<PackageSqlResourceComponent>())
          component.UpdatePackageVersion(feed, packageInfo.Id, packageVersionInternal.Id, new bool?(packageVersion.IsListed), files);
        List<FeedView> feedViewList = new List<FeedView>();
        IFeedViewService service1 = requestContext.GetService<IFeedViewService>();
        try
        {
          foreach (string addView in packageVersion.AddViews)
          {
            FeedView view2 = service1.GetView(requestContext, feed, addView);
            using (FeedViewSqlResourceComponent component = requestContext.CreateComponent<FeedViewSqlResourceComponent>())
            {
              if (component.AddPackageVersionToView(feed.GetIdentity(), view2.Id, packageVersionInternal.Id) > 0)
                feedViewList.Add(view2);
            }
          }
          foreach (string removeView in packageVersion.RemoveViews)
          {
            FeedView view3 = service1.GetView(requestContext, feed, removeView);
            using (FeedViewSqlResourceComponent component = requestContext.CreateComponent<FeedViewSqlResourceComponent>())
              component.RemovePackageVersionFromView(feed.GetIdentity(), view3.Id, packageVersionInternal.Id);
          }
        }
        finally
        {
          ITeamFoundationEventService service2 = requestContext.GetService<ITeamFoundationEventService>();
          if (packageVersion.IsListed && !packageVersionInternal.IsListed)
          {
            DeprecatedPackageChangedEvent notificationEvent1 = new DeprecatedPackageChangedEvent(feed, packageInfo, packageVersionInternal, PackageChangeType.Relist);
            service2.PublishNotification(requestContext, (object) notificationEvent1);
            PackageChangedEvent notificationEvent2 = new PackageChangedEvent(feed, packageInfo, packageVersionInternal, PackageChangeType.Relist);
            service2.PublishNotification(requestContext, (object) notificationEvent2);
          }
          else if (!packageVersion.IsListed && packageVersionInternal.IsListed)
          {
            DeprecatedPackageChangedEvent notificationEvent3 = new DeprecatedPackageChangedEvent(feed, packageInfo, packageVersionInternal, PackageChangeType.Unlist);
            service2.PublishNotification(requestContext, (object) notificationEvent3);
            PackageChangedEvent notificationEvent4 = new PackageChangedEvent(feed, packageInfo, packageVersionInternal, PackageChangeType.Unlist);
            service2.PublishNotification(requestContext, (object) notificationEvent4);
          }
          foreach (FeedView feedView in feedViewList)
          {
            feed.View = feedView;
            DeprecatedPackageChangedEvent notificationEvent5 = new DeprecatedPackageChangedEvent(feed, packageInfo, packageVersionInternal);
            service2.PublishNotification(requestContext, (object) notificationEvent5);
            PackageChangedEvent notificationEvent6 = new PackageChangedEvent(feed, packageInfo, packageVersionInternal);
            service2.PublishNotification(requestContext, (object) notificationEvent6);
          }
        }
      }), nameof (UpdatePackageVersion));
    }

    public virtual void UpdatePackageVersions(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed,
      IEnumerable<PackageVersionUpdate> updates)
    {
      requestContext.TraceBlock(10019231, 10019232, 10019233, "FeedIndex", "Service", (Action) (() =>
      {
        feed.ThrowIfNull<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>((Func<Exception>) (() => (Exception) InvalidUserInputException.Create(nameof (feed))));
        updates.ThrowIfNull<IEnumerable<PackageVersionUpdate>>((Func<Exception>) (() => (Exception) InvalidUserInputException.Create(nameof (updates))));
        FeedSecurityHelper.CheckModifyIndexPermissions(requestContext, (FeedCore) feed);
        using (PackageSqlResourceComponent component = requestContext.CreateComponent<PackageSqlResourceComponent>())
          component.UpdatePackageVersions(feed, updates);
      }), nameof (UpdatePackageVersions));
    }

    public void DeletePackageVersion(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed,
      string packageId,
      string packageVersionId,
      DateTime deletedDate,
      DateTime scheduledPermanentDeleteDate)
    {
      requestContext.TraceBlock(10019243, 10019243, 10019245, "FeedIndex", "Service", (Action) (() =>
      {
        feed.ThrowIfNull<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>((Func<Exception>) (() => (Exception) InvalidUserInputException.Create(nameof (feed))));
        packageId.ThrowIfNullOrWhitespace((Func<Exception>) (() => (Exception) InvalidUserInputException.Create(nameof (packageId))));
        packageVersionId.ThrowIfNullOrWhitespace((Func<Exception>) (() => (Exception) InvalidUserInputException.Create(nameof (packageVersionId))));
        FeedSecurityHelper.CheckModifyIndexPermissions(requestContext, (FeedCore) feed);
        FeedView view1 = feed.View;
        if (view1 != null)
          feed.View = (FeedView) null;
        IPackageInfo packageInfo = this.GetPackageInfo(requestContext, feed, packageId);
        if (view1 != null)
          feed.View = view1;
        PackageVersion packageVersionInternal = this.GetPackageVersionInternal(requestContext, feed, packageInfo, packageVersionId);
        int num = 0;
        using (PackageSqlResourceComponent component = requestContext.CreateComponent<PackageSqlResourceComponent>())
          num = component.DeletePackageVersion(feed, packageInfo.Id, packageVersionInternal.Id, deletedDate, scheduledPermanentDeleteDate);
        if (num <= 0)
          return;
        ITeamFoundationEventService service = requestContext.GetService<ITeamFoundationEventService>();
        if (feed.View == null)
        {
          DeprecatedPackageChangedEvent notificationEvent1 = new DeprecatedPackageChangedEvent(feed, packageInfo, packageVersionInternal, PackageChangeType.Delete);
          service.PublishNotification(requestContext, (object) notificationEvent1);
          PackageChangedEvent notificationEvent2 = new PackageChangedEvent(feed, packageInfo, packageVersionInternal, PackageChangeType.Delete);
          service.PublishNotification(requestContext, (object) notificationEvent2);
          if (!requestContext.IsFeatureEnabled("Packaging.Feed.PackageNotifications") || packageVersionInternal.Views == null)
            return;
          foreach (FeedView view2 in packageVersionInternal.Views)
          {
            feed.View = view2;
            DeprecatedPackageChangedEvent notificationEvent3 = new DeprecatedPackageChangedEvent(feed, packageInfo, packageVersionInternal, PackageChangeType.Delete);
            service.PublishNotification(requestContext, (object) notificationEvent3);
            PackageChangedEvent notificationEvent4 = new PackageChangedEvent(feed, packageInfo, packageVersionInternal, PackageChangeType.Delete);
            service.PublishNotification(requestContext, (object) notificationEvent4);
          }
        }
        else
        {
          DeprecatedPackageChangedEvent notificationEvent5 = new DeprecatedPackageChangedEvent(feed, packageInfo, packageVersionInternal, PackageChangeType.Delete);
          service.PublishNotification(requestContext, (object) notificationEvent5);
          PackageChangedEvent notificationEvent6 = new PackageChangedEvent(feed, packageInfo, packageVersionInternal, PackageChangeType.Delete);
          service.PublishNotification(requestContext, (object) notificationEvent6);
        }
      }), nameof (DeletePackageVersion));
    }

    public void ClearCachedPackages(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed,
      string protocolType)
    {
      requestContext.TraceBlock(10019246, 10019247, 10019248, "FeedIndex", "Service", (Action) (() =>
      {
        feed.ThrowIfNull<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>((Func<Exception>) (() => (Exception) InvalidUserInputException.Create(nameof (feed))));
        protocolType.ThrowIfNull<string>((Func<Exception>) (() => (Exception) InvalidUserInputException.Create(nameof (protocolType))));
        FeedSecurityHelper.CheckModifyIndexPermissions(requestContext, (FeedCore) feed);
        if (feed.Capabilities.HasFlag((Enum) FeedCapabilities.UpstreamV2))
          throw new InvalidFeedCapabilitiesException(Resources.Error_FeedMustNotHaveUpstreamV2Capability());
        using (PackageSqlResourceComponent component = requestContext.CreateComponent<PackageSqlResourceComponent>())
          component.ClearCachedPackages(feed, protocolType);
      }), nameof (ClearCachedPackages));
    }

    public virtual async Task<IEnumerable<BuildPackage>> GetPackagesByBuildId(
      IVssRequestContext requestContext,
      string projectId,
      int buildId)
    {
      BuildHttpClient client = requestContext.GetClient<BuildHttpClient>();
      try
      {
        if (await client.GetBuildAsync(projectId, buildId) != null)
        {
          using (BuildPackageSqlResourceComponent component = requestContext.CreateComponent<BuildPackageSqlResourceComponent>())
            return component.GetPackagesByBuildId(buildId);
        }
      }
      catch
      {
        throw new ProvenanceNotFoundException(Resources.Error_BuildCouldNotBeResolved((object) projectId, (object) buildId));
      }
      return (IEnumerable<BuildPackage>) null;
    }

    internal bool IsUniqueIndexException(Exception exception) => exception is SqlException && ((SqlException) exception).Number == 2601;

    internal virtual IPackageInfo GetPackageInfo(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed,
      string packageRequestId)
    {
      if (requestContext.IsFeatureEnabled("Packaging.Feed.QueryPackageInfo"))
      {
        IPackageInfo packageInfo = (IPackageInfo) null;
        Guid result;
        if (Guid.TryParse(packageRequestId, out result))
        {
          using (PackageInfoSqlResourceComponent component = requestContext.CreateComponent<PackageInfoSqlResourceComponent>())
            packageInfo = component.GetPackageById(feed.GetIdentity(), result);
        }
        else
        {
          (string protocolType, string normalizedName) = FeedIndexService.ParseLegacyFormat(packageRequestId);
          using (PackageInfoSqlResourceComponent component = requestContext.CreateComponent<PackageInfoSqlResourceComponent>())
            packageInfo = component.GetPackageByName(feed.GetIdentity(), protocolType, normalizedName);
        }
        return packageInfo != null ? packageInfo : throw PackageNotFoundException.Create(packageRequestId, feed.Name);
      }
      IVssRequestContext requestContext1 = requestContext;
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed1 = feed;
      string packageId = packageRequestId;
      bool? nullable = new bool?();
      bool? isListed = new bool?();
      bool? isDeleted = nullable;
      return (IPackageInfo) this.GetSinglePackage(requestContext1, feed1, packageId, isListed, isDeleted);
    }

    private static (string protocolType, string normalizedName) ParseLegacyFormat(string packageId)
    {
      string[] strArray = packageId.Split(new char[1]{ '_' }, 2);
      bool isNuGet = string.Equals(strArray[0], "NuGet", StringComparison.OrdinalIgnoreCase);
      if (!isNuGet || strArray.Length != 2 || string.IsNullOrWhiteSpace(strArray[1]))
        throw InvalidPackageIdException.Create(packageId, isNuGet);
      return (strArray[0], strArray[1]);
    }

    internal virtual Package GetSinglePackage(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed,
      string packageId,
      bool? isListed = null,
      bool? isDeleted = false)
    {
      IVssRequestContext requestContext1 = requestContext;
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed1 = feed;
      string packageRequestId = packageId;
      bool? isListed1 = isListed;
      bool? nullable = isDeleted;
      bool? isRelease = new bool?();
      bool? isDeleted1 = nullable;
      return this.GetPackageInternal(requestContext1, feed1, packageRequestId, isListed: isListed1, isRelease: isRelease, isDeleted: isDeleted1).SingleOrDefault<Package>();
    }

    internal virtual IEnumerable<Package> GetPackageInternal(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed,
      string packageRequestId,
      ResultOptions resultOptions = null,
      bool? isListed = null,
      bool? isRelease = null,
      bool? isDeleted = false)
    {
      ResultOptions resultOptions1 = resultOptions;
      if (resultOptions1 == null)
        resultOptions1 = new ResultOptions()
        {
          IncludeAllVersions = false,
          IncludeDescriptions = false
        };
      resultOptions = resultOptions1;
      Guid result;
      IEnumerable<Package> source;
      if (Guid.TryParse(packageRequestId, out result))
      {
        source = this.GetPackageById(requestContext, feed, result, resultOptions, isListed, isRelease, isDeleted);
      }
      else
      {
        (string protocolType, string str) = FeedIndexService.ParseLegacyFormat(packageRequestId);
        source = this.GetPackageByProtocolTypeAndNormalizedPackageName(requestContext, feed, protocolType, str, resultOptions, isListed, isRelease, isDeleted);
      }
      return source != null && source.Any<Package>() ? source : throw PackageNotFoundException.Create(packageRequestId, feed.Name);
    }

    internal virtual IEnumerable<Package> GetPackageById(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed,
      Guid packageId,
      ResultOptions resultOptions = null,
      bool? isListed = null,
      bool? isRelease = null,
      bool? isDeleted = null)
    {
      ResultOptions resultOptions1 = resultOptions;
      if (resultOptions1 == null)
        resultOptions1 = new ResultOptions()
        {
          IncludeAllVersions = false,
          IncludeDescriptions = true
        };
      resultOptions = resultOptions1;
      using (PackageSqlResourceComponent component = requestContext.CreateComponent<PackageSqlResourceComponent>())
        return component.GetPackage(feed, packageId, resultOptions, isListed, isRelease, isDeleted);
    }

    internal virtual IEnumerable<PackageDependencyDetails> GetPackageDependencyByName(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed,
      string protocolType,
      IEnumerable<PackageDependency> packageDependencies)
    {
      using (PackageSqlResourceComponent component = requestContext.CreateComponent<PackageSqlResourceComponent>())
        return component.GetPackageDependencyByName(feed, protocolType, packageDependencies);
    }

    internal virtual IEnumerable<Package> GetPackageByProtocolTypeAndNormalizedPackageName(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed,
      string protocolType,
      string normalizedPackageName,
      ResultOptions resultOptions = null,
      bool? isListed = null,
      bool? isRelease = null,
      bool? isDeleted = null)
    {
      ResultOptions resultOptions1 = resultOptions;
      if (resultOptions1 == null)
        resultOptions1 = new ResultOptions()
        {
          IncludeAllVersions = false,
          IncludeDescriptions = true
        };
      resultOptions = resultOptions1;
      using (PackageSqlResourceComponent component = requestContext.CreateComponent<PackageSqlResourceComponent>())
        return component.GetPackage(feed, protocolType, normalizedPackageName, resultOptions, isListed, isRelease, isDeleted);
    }

    internal virtual PackageVersion GetPackageVersionInternal(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed,
      IPackageInfo package,
      string packageVersionId)
    {
      PackageVersion packageVersion = (PackageVersion) null;
      Guid result;
      if (Guid.TryParse(packageVersionId, out result))
        packageVersion = this.GetPackageVersionByVersionId(requestContext, feed, package.Id, result);
      if (packageVersion == null)
        packageVersion = this.GetPackageVersionByNormalizedPackageVersion(requestContext, feed, package.Id, packageVersionId);
      return packageVersion != null ? packageVersion : throw PackageVersionNotFoundException.Create(package.ProtocolType, package.NormalizedName, packageVersionId, feed.Name);
    }

    internal virtual PackageVersion GetPackageVersionByVersionId(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed,
      Guid packageId,
      Guid packageVersionId)
    {
      using (PackageSqlResourceComponent component = requestContext.CreateComponent<PackageSqlResourceComponent>())
        return component.GetPackageVersion(feed, packageId, packageVersionId);
    }

    internal virtual PackageVersion GetPackageVersionByNormalizedPackageVersion(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed,
      Guid packageId,
      string normalizedPackageVersion)
    {
      using (PackageSqlResourceComponent component = requestContext.CreateComponent<PackageSqlResourceComponent>())
        return component.GetPackageVersion(feed, packageId, normalizedPackageVersion);
    }

    internal virtual IEnumerable<PackageVersion> GetPackageVersionsInternal(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed,
      IPackageInfo package,
      bool? isListed = null,
      bool? isDeleted = null)
    {
      using (PackageSqlResourceComponent component = requestContext.CreateComponent<PackageSqlResourceComponent>())
        return component.GetPackageVersions(feed, package.Id, isListed, package.ProtocolType, isDeleted);
    }

    private static void ValidateIncludeDescriptionAllVersions(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed,
      ResultOptions ro)
    {
      if (!ro.IncludeDescriptions)
        return;
      if (!ro.IncludeAllVersions)
        return;
      try
      {
        FeedSecurityHelper.CheckModifyIndexPermissions(requestContext, (FeedCore) feed);
      }
      catch (FeedNeedsPermissionsException ex)
      {
        throw new IncompatibleQueryParametersException(Resources.Error_DescriptionAndAllVersionsIncompatible());
      }
    }

    private void SetMissingSourceId(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed,
      IEnumerable<UpstreamSource> sourceChain)
    {
      requestContext.TraceBlock(10019234, 10019235, 10019236, "FeedIndex", "Service", (Action) (() =>
      {
        IEnumerable<UpstreamSource> source = sourceChain;
        UpstreamSource directUpstream = source != null ? source.FirstOrDefault<UpstreamSource>() : (UpstreamSource) null;
        if (directUpstream == null || directUpstream.Id != Guid.Empty || string.IsNullOrWhiteSpace(directUpstream.Location))
          return;
        Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed1 = requestContext.GetService<IFeedService>().GetFeed(requestContext, feed.Id.ToString(), includeDeletedUpstreams: true);
        Uri result1;
        Uri result2;
        UpstreamSource upstreamSource1 = feed1.UpstreamSources.FirstOrDefault<UpstreamSource>((Func<UpstreamSource, bool>) (x => Uri.TryCreate(x.Location, UriKind.Absolute, out result1) && Uri.TryCreate(directUpstream.Location, UriKind.Absolute, out result2) && result1 == result2));
        Uri result3;
        Uri result4;
        UpstreamSource upstreamSource2 = feed1.UpstreamSources.FirstOrDefault<UpstreamSource>((Func<UpstreamSource, bool>) (x => x.DisplayLocation != null && Uri.TryCreate(x.DisplayLocation, UriKind.Absolute, out result3) && Uri.TryCreate(directUpstream.Location, UriKind.Absolute, out result4) && result3 == result4));
        if (upstreamSource1 != null)
          directUpstream.Id = upstreamSource1.Id;
        else if (upstreamSource2 != null)
          directUpstream.Id = upstreamSource2.Id;
        else
          requestContext.TraceException(10019236, "FeedIndex", "Service", (Exception) new InvalidOperationException(Resources.Error_InvalidUpstreamSourceLocation((object) feed.Name, (object) directUpstream.Location)));
      }), nameof (SetMissingSourceId));
    }

    private bool ValidateProvenance(
      IVssRequestContext requestContext,
      PackageVersionIndexEntry packageVersion)
    {
      return requestContext.TraceBlock<bool>(10019237, 10019238, 10019239, "FeedIndex", "Service", (Func<bool>) (() =>
      {
        Exception exception;
        if (requestContext.GetService<IProvenanceService>().TryValidateProvenanceData(packageVersion.Provenance, out exception))
          return true;
        requestContext.TraceException(10019239, "FeedIndex", "Service", exception);
        if (packageVersion.Provenance.Data != null)
          packageVersion.Provenance.Data = (IDictionary<string, string>) null;
        return false;
      }), nameof (ValidateProvenance));
    }
  }
}
