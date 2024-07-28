// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.PackageRecycleBinService
// Assembly: Microsoft.VisualStudio.Services.Feed.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55555083-3B79-4F4F-AA85-92D66019974E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Feed.Common;
using Microsoft.VisualStudio.Services.Feed.Server.RecycleBin;
using Microsoft.VisualStudio.Services.Feed.Server.Types;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Feed.WebApi.AzureArtifacts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;

namespace Microsoft.VisualStudio.Services.Feed.Server
{
  internal class PackageRecycleBinService : IPackageRecycleBinService, IVssFrameworkService
  {
    public const string DefaultScheduleImmediatePermanentDeletionSizeRegistryPath = "/Configuration/Feed/RecycleBin/DefaultScheduleImmediatePermanentDeletionSize";
    private readonly ITimeProvider timeProvider;
    private const string EmptyRecycleBinJobName = "EmptyRecycleBinProcessingJob";
    private const string EmptyRecycleBinExtensionName = "Microsoft.VisualStudio.Services.Feed.Server.Plugins.EmptyRecycleBinProcessingJob";

    public PackageRecycleBinService()
      : this((ITimeProvider) new DefaultTimeProvider())
    {
    }

    public PackageRecycleBinService(ITimeProvider timeProvider) => this.timeProvider = timeProvider;

    public void ServiceStart(IVssRequestContext systemRequestContext) => systemRequestContext.CheckProjectCollectionRequestContext();

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public IEnumerable<Package> GetPackages(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed,
      string protocolType = null,
      string nameQuery = null,
      int top = 1000,
      int skip = 0,
      bool includeAllVersions = false)
    {
      feed.ThrowIfNull<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>((Func<Exception>) (() => (Exception) InvalidUserInputException.Create(nameof (feed))));
      feed.ThrowIfViewNotation();
      PagingOptions pagingOptions = new PagingOptions()
      {
        Top = top,
        Skip = skip
      };
      pagingOptions.Validate();
      FeedSecurityHelper.CheckReadFeedPermissions(requestContext, (FeedCore) feed);
      using (PackageRecycleBinSqlResourceComponent component = requestContext.CreateComponent<PackageRecycleBinSqlResourceComponent>())
        return ((IEnumerable<Package>) component.GetPackages(feed, protocolType, nameQuery, pagingOptions, this.timeProvider.Now, includeAllVersions).ToArray<Package>()).GroupBy<Package, Guid, Package>((Func<Package, Guid>) (x => x.Id), (Func<Guid, IEnumerable<Package>, Package>) ((id, package) => this.GroupPackagesWithSameId(requestContext, package, !includeAllVersions)));
    }

    public Package GetPackage(IVssRequestContext requestContext, Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed, Guid packageId)
    {
      feed.ThrowIfNull<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>((Func<Exception>) (() => (Exception) InvalidUserInputException.Create(nameof (feed))));
      feed.ThrowIfViewNotation();
      FeedSecurityHelper.CheckReadFeedPermissions(requestContext, (FeedCore) feed);
      Package package;
      using (PackageRecycleBinSqlResourceComponent component = requestContext.CreateComponent<PackageRecycleBinSqlResourceComponent>())
        package = component.GetPackage(feed, packageId, this.timeProvider.Now);
      return package != null ? package : throw PackageNotFoundException.Create(packageId.ToString(), feed.Id.ToString());
    }

    public IEnumerable<RecycleBinPackageVersion> GetPackageVersions(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed,
      Guid packageId)
    {
      feed.ThrowIfNull<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>((Func<Exception>) (() => (Exception) InvalidUserInputException.Create(nameof (feed))));
      feed.ThrowIfViewNotation();
      FeedSecurityHelper.CheckReadFeedPermissions(requestContext, (FeedCore) feed);
      using (PackageRecycleBinSqlResourceComponent component = requestContext.CreateComponent<PackageRecycleBinSqlResourceComponent>())
        return component.GetPackageVersions(feed, packageId, this.timeProvider.Now);
    }

    public IEnumerable<DeletedPackageVersion> GetAllPackageVersionsByFeed(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed)
    {
      feed.ThrowIfNull<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>((Func<Exception>) (() => (Exception) InvalidUserInputException.Create(nameof (feed))));
      feed.ThrowIfViewNotation();
      FeedSecurityHelper.CheckReadFeedPermissions(requestContext, (FeedCore) feed);
      using (PackageRecycleBinSqlResourceComponent component = requestContext.CreateComponent<PackageRecycleBinSqlResourceComponent>())
        return component.GetAllPackageVersionsByFeed(feed);
    }

    public RecycleBinPackageVersion GetPackageVersion(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed,
      Guid packageId,
      Guid packageVersionId)
    {
      feed.ThrowIfNull<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>((Func<Exception>) (() => (Exception) InvalidUserInputException.Create(nameof (feed))));
      feed.ThrowIfViewNotation();
      FeedSecurityHelper.CheckReadFeedPermissions(requestContext, (FeedCore) feed);
      RecycleBinPackageVersion packageVersion;
      using (PackageRecycleBinSqlResourceComponent component = requestContext.CreateComponent<PackageRecycleBinSqlResourceComponent>())
        packageVersion = component.GetPackageVersion(feed, packageId, packageVersionId, this.timeProvider.Now);
      return packageVersion != null ? packageVersion : throw PackageVersionNotFoundException.CreateByIdRecycleBin(packageId.ToString(), packageVersionId.ToString(), feed.Id);
    }

    public void PermanentlyDeletePackageVersion(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed,
      Guid packageId,
      Guid packageVersionId)
    {
      feed.ThrowIfViewNotation();
      FeedSecurityHelper.CheckModifyIndexPermissions(requestContext, (FeedCore) feed);
      using (PackageRecycleBinSqlResourceComponent component = requestContext.CreateComponent<PackageRecycleBinSqlResourceComponent>())
        component.PermanentlyDeletePackageVersion(feed, packageId, packageVersionId);
    }

    public void RestorePackageVersionToFeed(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed,
      Guid packageId,
      Guid packageVersionId)
    {
      feed.ThrowIfViewNotation();
      FeedSecurityHelper.CheckModifyIndexPermissions(requestContext, (FeedCore) feed);
      using (PackageRecycleBinSqlResourceComponent component = requestContext.CreateComponent<PackageRecycleBinSqlResourceComponent>())
        component.RestorePackageVersionToFeed(feed, packageId, packageVersionId);
    }

    internal Package GroupPackagesWithSameId(
      IVssRequestContext requestContext,
      IEnumerable<Package> packagesWithSameId,
      bool includeDescriptions = false)
    {
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) packagesWithSameId, nameof (packagesWithSameId));
      List<Package> list = packagesWithSameId.ToList<Package>();
      Package firstPackage = list.First<Package>();
      if (list.Any<Package>((Func<Package, bool>) (x => x.Id != firstPackage.Id)))
        throw new PackagesCollectionWithDifferentIdsException();
      return new Package()
      {
        ProtocolType = firstPackage.ProtocolType,
        Id = firstPackage.Id,
        NormalizedName = firstPackage.NormalizedName,
        Name = firstPackage.Name,
        IsCached = firstPackage.IsCached,
        Versions = list.Select<Package, MinimalPackageVersion>((Func<Package, MinimalPackageVersion>) (pack => pack.Versions.Single<MinimalPackageVersion>())).Select<MinimalPackageVersion, MinimalPackageVersion>((Func<MinimalPackageVersion, MinimalPackageVersion>) (v => new MinimalPackageVersion()
        {
          Id = v.Id,
          NormalizedVersion = v.NormalizedVersion,
          Version = v.Version,
          StorageId = v.StorageId,
          IsLatest = v.IsLatest,
          IsListed = v.IsListed,
          IsDeleted = v.IsDeleted,
          PackageDescription = includeDescriptions ? v.PackageDescription : (string) null,
          IsCachedVersion = v.IsCachedVersion,
          Views = v.Views,
          PublishDate = v.PublishDate
        }))
      };
    }

    private IEnumerable<ProtocolPackageVersionIdentity> GetTopPackageVersions(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed,
      DateTime deletedBefore,
      int top = 1000)
    {
      feed.ThrowIfNull<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>((Func<Exception>) (() => (Exception) InvalidUserInputException.Create(nameof (feed))));
      feed.ThrowIfViewNotation();
      FeedSecurityHelper.CheckReadFeedPermissions(requestContext, (FeedCore) feed);
      using (PackageRecycleBinSqlResourceComponent component = requestContext.CreateComponent<PackageRecycleBinSqlResourceComponent>())
        return component.GetTopPackageVersions(feed, deletedBefore, top);
    }

    public async Task<EmptyRecycleBinResult> EmptyRecycleBinAsync(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed,
      DateTime deleteBefore)
    {
      feed.ThrowIfNull<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>((Func<Exception>) (() => (Exception) InvalidUserInputException.Create(nameof (feed))));
      feed.ThrowIfViewNotation();
      FeedSecurityHelper.CheckModifyIndexPermissions(requestContext, (FeedCore) feed);
      EmptyRecycleBinResult result = new EmptyRecycleBinResult(feed.Id);
      int packagesRetrieved = 0;
      while (!requestContext.IsCanceled)
      {
        IEnumerable<ProtocolPackageVersionIdentity> topPackageVersions = this.GetTopPackageVersions(requestContext, feed, deleteBefore);
        packagesRetrieved = topPackageVersions.Count<ProtocolPackageVersionIdentity>();
        (int, int, int, ISet<string>) valueTuple = await this.PermanentlyDeletePackageVersionsAsync(requestContext, feed, topPackageVersions);
        result.AttemptedDeletes += valueTuple.Item1;
        result.SuccessfulDeletes += valueTuple.Item2;
        result.FailedDeletes += valueTuple.Item3;
        result.FailedProtocols.UnionWith((IEnumerable<string>) valueTuple.Item4);
        if (packagesRetrieved <= 999)
        {
          EmptyRecycleBinResult recycleBinResult = result;
          result = (EmptyRecycleBinResult) null;
          return recycleBinResult;
        }
      }
      throw new TaskCanceledException();
    }

    private async Task<(int AttemptedCount, int SuccessCount, int FailedCount, ISet<string> Failed)> PermanentlyDeletePackageVersionsAsync(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed,
      IEnumerable<ProtocolPackageVersionIdentity> packages)
    {
      (int, int, int, ISet<string>) result = (0, 0, 0, (ISet<string>) new HashSet<string>());
      if (!packages.Any<ProtocolPackageVersionIdentity>())
        return result;
      int maxBatchSize = requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) "/Configuration/Packaging/MaxPackagesBatchRequestSize", true, 100);
      foreach (IGrouping<string, ProtocolPackageVersionIdentity> source in packages.GroupBy<ProtocolPackageVersionIdentity, string>((Func<ProtocolPackageVersionIdentity, string>) (p => p.Protocol), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase))
      {
        string protocolType = source.Key;
        foreach (IList<ProtocolPackageVersionIdentity> batch in source.ToList<ProtocolPackageVersionIdentity>().Batch<ProtocolPackageVersionIdentity>(maxBatchSize))
        {
          result.Item1 += batch.Count;
          try
          {
            await this.PermanentlyDeletePackageVersionsFromPackagingAsync(requestContext, feed, protocolType, (IEnumerable<ProtocolPackageVersionIdentity>) batch);
            this.PermanentlyDeletePackageVersionsFromRecycleBin(requestContext, feed, (IEnumerable<ProtocolPackageVersionIdentity>) batch);
            result.Item2 += batch.Count;
          }
          catch (Exception ex)
          {
            int tracepoint = 10019141;
            string format = string.Format("There was an error when deleting a '{0}' batch for feed '{1}', id: '{2}'", (object) protocolType, (object) feed.Name, (object) feed.Id);
            result.Item4.Add(protocolType);
            result.Item3 += batch.Count;
            requestContext.TraceAlways(tracepoint, TraceLevel.Verbose, "Feed", "Service", format);
            requestContext.TraceException(tracepoint, "Feed", "Service", ex);
          }
        }
        protocolType = (string) null;
      }
      return result;
    }

    public void PermanentlyDeletePackageVersionsFromRecycleBin(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed,
      IEnumerable<ProtocolPackageVersionIdentity> packageVersionIdentities)
    {
      feed.ThrowIfViewNotation();
      using (PackageRecycleBinSqlResourceComponent component = requestContext.CreateComponent<PackageRecycleBinSqlResourceComponent>())
        component.BatchPermanentlyDeletePackageVersions(feed, packageVersionIdentities);
      string message = "Completed batch permanent delete operation for current batch of recycle bin packages.";
      int tracepoint = 10019144;
      requestContext.Trace(tracepoint, TraceLevel.Info, "Feed", "Service", message);
    }

    private async Task PermanentlyDeletePackageVersionsFromPackagingAsync(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed,
      string protocol,
      IEnumerable<ProtocolPackageVersionIdentity> packagesForOneProtocol)
    {
      try
      {
        await requestContext.To(TeamFoundationHostType.Deployment).GetService<IPackagingProtocolService>().GetPackagingOperations(protocol).PermanentlyDeletePackageVersions(requestContext, feed, (IEnumerable<PackageVersionIdentity>) packagesForOneProtocol);
        requestContext.Trace(10019143, TraceLevel.Info, "Feed", "Service", "Completed sending permanent delete operations to Packaging service for current batch of '" + protocol + "' packages");
      }
      catch (FeedIdNotFoundException ex)
      {
      }
      catch (NotSupportedException ex)
      {
        requestContext.TraceAlways(10019141, TraceLevel.Verbose, "Feed", "Service", "Ignoring permanent delete operation for protocol '" + protocol + "' as it is not supported");
        throw;
      }
    }

    public Guid QueueEmptyRecycleBin(IVssRequestContext requestContext, Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed)
    {
      feed.ThrowIfNull<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>((Func<Exception>) (() => (Exception) InvalidUserInputException.Create(nameof (feed))));
      FeedSecurityHelper.CheckDeletePackagePermissions(requestContext, (FeedCore) feed);
      int batchSize = requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) "/Configuration/Feed/RecycleBin/DefaultScheduleImmediatePermanentDeletionSize", true, 1000);
      DateTime dateTime;
      using (PackageRecycleBinSqlResourceComponent component = requestContext.CreateComponent<PackageRecycleBinSqlResourceComponent>())
        dateTime = component.ScheduleImmediatePermanentDeletion(feed, batchSize);
      EmptyRecycleBinJobData objectToSerialize = new EmptyRecycleBinJobData()
      {
        FeedId = feed.Id,
        DeleteBefore = dateTime
      };
      ITeamFoundationJobService service = requestContext.GetService<ITeamFoundationJobService>();
      string str = "EmptyRecycleBinProcessingJob_" + feed.Id.ToString();
      IVssRequestContext requestContext1 = requestContext;
      string jobName = str;
      XmlNode xml = TeamFoundationSerializationUtility.SerializeToXml((object) objectToSerialize);
      return service.QueueOneTimeJob(requestContext1, jobName, "Microsoft.VisualStudio.Services.Feed.Server.Plugins.EmptyRecycleBinProcessingJob", xml, false);
    }
  }
}
