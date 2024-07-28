// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata.PackageMetadataService`3
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using Microsoft.VisualStudio.Services.Feed.Common;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.ItemStore.Common;
using Microsoft.VisualStudio.Services.ItemStore.Server.Common;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata
{
  public abstract class PackageMetadataService<TMetadataEntry, TItem, TVersion> : 
    IVssFrameworkService
    where TMetadataEntry : PackageMetadataEntry
    where TItem : PackageMetadataItem
    where TVersion : IComparable<TVersion>, IPackageVersion
  {
    protected const int MaxRetries = 10;
    private const string CommitsByIdentityFolderName = "commitsByIdentity";
    private const string VersionListItemName = "versionList";
    private static readonly TraceData TraceData = Microsoft.VisualStudio.Services.Packaging.ServiceShared.Constants.TracePoints.PackageMetadataService.TraceData;
    private readonly IMetadataOperationApplierFactory operationApplierFactory;

    public PackageMetadataService(
      IMetadataOperationApplierFactory operationApplierFactory)
    {
      this.operationApplierFactory = operationApplierFactory;
    }

    protected abstract string CommitLogItemType { get; }

    protected abstract string AlternateMetadataRevision { get; }

    protected virtual bool DropAllPublishedVersionDetails { get; }

    protected virtual bool GetDropPublishedVersionDetails(
      IVssRequestContext requestContext,
      Guid feedId)
    {
      return this.DropAllPublishedVersionDetails;
    }

    public virtual void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public virtual void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public virtual async Task<TMetadataEntry> GetLatestStateForPackageVersionAsync(
      IVssRequestContext requestContext,
      FeedCore feed,
      IPackageName packageName,
      TVersion packageVersion,
      VersionSearchFilter searchFilter)
    {
      Locator commitItemLocator = this.GetPackageVersionCommitItemLocator(requestContext, packageName, (IPackageVersion) packageVersion);
      return await this.GetLatestStateForPackageVersionAsync(requestContext, feed, packageName, packageVersion, searchFilter, commitItemLocator);
    }

    public virtual async Task AddMetadataEntryAsync(
      IVssRequestContext requestContext,
      FeedCore feed,
      IPackageName packageName,
      TVersion packageVersion,
      ICommitLogEntry commitLogEntry)
    {
      Locator commitItemLocator = this.GetPackageVersionCommitItemLocator(requestContext, packageName, (IPackageVersion) packageVersion);
      Locator versionListLocator = this.GetPackageVersionListLocator(requestContext, packageName);
      await this.AddMetadataEntryAsync(requestContext, feed, packageName, packageVersion, commitLogEntry, commitItemLocator, versionListLocator);
    }

    public virtual async Task<List<TVersion>> GetAllVersionsOfPackageAsync(
      IVssRequestContext requestContext,
      FeedCore feed,
      IPackageName packageName,
      VersionSearchFilter searchFilter)
    {
      Locator versionListLocator = this.GetPackageVersionListLocator(requestContext, packageName);
      return await this.GetAllVersionsOfPackageAsync(requestContext, feed, packageName, searchFilter, versionListLocator);
    }

    public virtual async Task<List<TMetadataEntry>> GetLatestStateForAllVersionsOfPackageAsync(
      IVssRequestContext requestContext,
      FeedCore feed,
      IPackageName packageName,
      VersionSearchFilter searchFilter)
    {
      Locator commitItemsLocator = this.GetPackageCommitItemsLocator(requestContext, packageName);
      return await this.GetLatestStateForAllVersionsOfPackageAsync(requestContext, feed, packageName, searchFilter, commitItemsLocator);
    }

    protected async Task<TMetadataEntry> GetLatestStateForPackageVersionAsync(
      IVssRequestContext requestContext,
      FeedCore feed,
      IPackageName packageName,
      TVersion packageVersion,
      VersionSearchFilter searchFilter,
      Locator itemLocator)
    {
      using (Microsoft.VisualStudio.Services.Content.Server.Common.Tracer.Enter(requestContext, PackageMetadataService<TMetadataEntry, TItem, TVersion>.TraceData, 5724300, nameof (GetLatestStateForPackageVersionAsync)))
      {
        FeedSecurityHelper.CheckReadFeedPermissions(requestContext, feed);
        Locator feedContainerName = PackagingUtils.ComputeFeedContainerName(feed);
        TItem itemAsync = await this.GetItemStore(requestContext).GetItemAsync<TItem>(requestContext, feedContainerName, itemLocator);
        if ((object) itemAsync == null)
          return default (TMetadataEntry);
        TMetadataEntry packageVersionAsync = this.ConvertItemToEntry(itemAsync);
        VersionSearchFilter versionSearchFilter = searchFilter;
        Guid? viewId;
        int num;
        if (versionSearchFilter == null)
        {
          num = 0;
        }
        else
        {
          viewId = versionSearchFilter.ViewId;
          num = viewId.HasValue ? 1 : 0;
        }
        if (num != 0)
        {
          IEnumerable<Guid> views = packageVersionAsync.Views;
          viewId = searchFilter.ViewId;
          Guid guid = viewId.Value;
          if (!views.Contains<Guid>(guid))
            packageVersionAsync = default (TMetadataEntry);
        }
        return packageVersionAsync;
      }
    }

    protected async Task AddMetadataEntryAsync(
      IVssRequestContext requestContext,
      FeedCore feed,
      IPackageName packageName,
      TVersion packageVersion,
      ICommitLogEntry commitLogEntry,
      Locator commitItemLocator,
      Locator versionListLocator)
    {
      using (Microsoft.VisualStudio.Services.Content.Server.Common.Tracer.Enter(requestContext, PackageMetadataService<TMetadataEntry, TItem, TVersion>.TraceData, 5724310, nameof (AddMetadataEntryAsync)))
      {
        Locator containerLocator = PackagingUtils.ComputeFeedContainerName(feed);
        IItemStore itemStore = this.GetItemStore(requestContext);
        TimeSpan defaultMaxRetryDelay = RetryUtils.GetDefaultMaxRetryDelay(requestContext);
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        RetryHelper retryHelper = new RetryHelper(requestContext, 10, defaultMaxRetryDelay, PackageMetadataService<TMetadataEntry, TItem, TVersion>.\u003C\u003EO.\u003C0\u003E__IsRetryableStorageException ?? (PackageMetadataService<TMetadataEntry, TItem, TVersion>.\u003C\u003EO.\u003C0\u003E__IsRetryableStorageException = new Func<Exception, bool>(RetryUtils.IsRetryableStorageException)));
        IMetadataOperationApplier metadataApplier = this.operationApplierFactory.Get(commitLogEntry.CommitOperationData);
        // ISSUE: variable of a compiler-generated type
        PackageMetadataService<TMetadataEntry, TItem, TVersion>.\u003C\u003Ec__DisplayClass21_1 cDisplayClass211;
        Func<Task> function = (Func<Task>) (async () =>
        {
          VersionListItem versionListItem = await itemStore.GetItemAsync<VersionListItem>(requestContext, containerLocator, versionListLocator) ?? new VersionListItem();
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          List<TVersion> list = versionListItem.PublishedVersions.Select<string, TVersion>(new Func<string, TVersion>(cDisplayClass211.CS\u0024\u003C\u003E8__locals1.\u003C\u003E4__this.ParseVersion)).ToList<TVersion>();
          List<string> originalPublishedVersions = versionListItem.PublishedVersions.ToList<string>();
          int expectedVersionCount = list.Count<TVersion>();
          // ISSUE: reference to a compiler-generated field
          Dictionary<TVersion, VersionDetails> versionDetails = versionListItem.PublishedVersionDetails.ToDictionary<KeyValuePair<string, VersionDetails>, TVersion, VersionDetails>(closure_2 ?? (closure_2 = (Func<KeyValuePair<string, VersionDetails>, TVersion>) (kp => this.\u003C\u003E4__this.ParseVersion(kp.Key))), (Func<KeyValuePair<string, VersionDetails>, VersionDetails>) (kp => kp.Value));
          PackageMetadataItem packageMetadataItem;
          if (list.Contains(packageVersion))
          {
            TItem itemAsync = await itemStore.GetItemAsync<TItem>(requestContext, containerLocator, commitItemLocator);
            packageMetadataItem = metadataApplier.Apply(commitLogEntry, (PackageMetadataItem) itemAsync);
            if ((object) itemAsync != null)
              packageMetadataItem.StorageETag = itemAsync.StorageETag;
          }
          else
          {
            packageMetadataItem = metadataApplier.Apply(commitLogEntry, (PackageMetadataItem) null);
            list.Add(packageVersion);
            list.Sort();
            // ISSUE: reference to a compiler-generated field
            // ISSUE: reference to a compiler-generated field
            versionListItem.PublishedVersions = list.Select<TVersion, string>(new Func<TVersion, string>(cDisplayClass211.CS\u0024\u003C\u003E8__locals1.\u003C\u003E4__this.SerializeVersion));
            ++expectedVersionCount;
          }
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          if (cDisplayClass211.CS\u0024\u003C\u003E8__locals1.\u003C\u003E4__this.GetDropPublishedVersionDetails(requestContext, feed.Id))
          {
            versionListItem.PublishedVersionDetails = (IDictionary<string, VersionDetails>) new Dictionary<string, VersionDetails>();
          }
          else
          {
            versionDetails[packageVersion] = packageMetadataItem.GetProtocolVersionDetails();
            // ISSUE: reference to a compiler-generated field
            versionListItem.PublishedVersionDetails = (IDictionary<string, VersionDetails>) versionDetails.ToDictionary<KeyValuePair<TVersion, VersionDetails>, string, VersionDetails>(closure_3 ?? (closure_3 = (Func<KeyValuePair<TVersion, VersionDetails>, string>) (kp => this.\u003C\u003E4__this.SerializeVersion(kp.Key))), (Func<KeyValuePair<TVersion, VersionDetails>, VersionDetails>) (kp => kp.Value));
          }
          if (versionListItem.PublishedVersions.Count<string>() != expectedVersionCount)
          {
            string str1 = string.Join(",,", versionListItem.PublishedVersions);
            string str2 = string.Join(",,", (IEnumerable<string>) originalPublishedVersions);
            string str3 = string.Join(",", new string[1]
            {
              string.Format("{0}:{1}", (object) commitLogEntry.CommitId, (object) commitLogEntry.CommitOperationData.GetType().Name)
            });
            throw new InvalidDataException(string.Format("BUG: could not apply edit for feed {0} package name {1}, it reduces the number of package versions. Proactively failing before data loss. Expected version count: {2}. New version count: {3} Commit entry : {4}. New Package Versions: {5}. Old Package Versions: {6}", (object) feed.Id, (object) packageName.NormalizedName, (object) expectedVersionCount, (object) versionListItem.PublishedVersions.Count<string>(), (object) str3, (object) str1, (object) str2));
          }
          Dictionary<Locator, StoredItem> items = new Dictionary<Locator, StoredItem>()
          {
            {
              versionListLocator,
              (StoredItem) versionListItem
            },
            {
              commitItemLocator,
              (StoredItem) packageMetadataItem
            }
          };
          if ((await itemStore.CompareSwapItemsAsync(requestContext, containerLocator, (IReadOnlyDictionary<Locator, StoredItem>) items, true)).Any<KeyValuePair<Locator, bool>>((Func<KeyValuePair<Locator, bool>, bool>) (x => !x.Value)))
            throw new TargetModifiedAfterReadException(nameof (AddMetadataEntryAsync));
          versionListItem = (VersionListItem) null;
          originalPublishedVersions = (List<string>) null;
          versionDetails = (Dictionary<TVersion, VersionDetails>) null;
        });
        await retryHelper.Invoke(function);
      }
    }

    protected async Task<List<TVersion>> GetAllVersionsOfPackageAsync(
      IVssRequestContext requestContext,
      Guid feedId,
      IPackageName packageName,
      VersionSearchFilter searchFilter,
      Locator itemLocator)
    {
      return await this.GetAllVersionsOfPackageAsync(requestContext, this.GetFeed(requestContext, feedId), packageName, searchFilter, itemLocator);
    }

    protected async Task<List<TVersion>> GetAllVersionsOfPackageAsync(
      IVssRequestContext requestContext,
      FeedCore feed,
      IPackageName packageName,
      VersionSearchFilter searchFilter,
      Locator itemLocator)
    {
      PackageMetadataService<TMetadataEntry, TItem, TVersion> packageMetadataService = this;
      using (Microsoft.VisualStudio.Services.Content.Server.Common.Tracer.Enter(requestContext, PackageMetadataService<TMetadataEntry, TItem, TVersion>.TraceData, 5724330, nameof (GetAllVersionsOfPackageAsync)))
      {
        FeedSecurityHelper.CheckReadFeedPermissions(requestContext, feed);
        IItemStore itemStore = packageMetadataService.GetItemStore(requestContext);
        Locator feedContainerName = PackagingUtils.ComputeFeedContainerName(feed);
        IVssRequestContext requestContext1 = requestContext;
        Locator containerName = feedContainerName;
        Locator path = itemLocator;
        VersionListItem itemAsync = await itemStore.GetItemAsync<VersionListItem>(requestContext1, containerName, path);
        if (itemAsync == null)
          return new List<TVersion>();
        IEnumerable<TVersion> source = itemAsync.PublishedVersions.Select<string, TVersion>(new Func<string, TVersion>(packageMetadataService.ParseVersion));
        IDictionary<string, VersionDetails> versionDetails = itemAsync.PublishedVersionDetails;
        VersionSearchFilter versionSearchFilter = searchFilter;
        if ((versionSearchFilter != null ? (versionSearchFilter.ViewId.HasValue ? 1 : 0) : 0) != 0)
        {
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          source = source.Where<TVersion>((Func<TVersion, bool>) (v => versionDetails.ContainsKey(this.\u003C\u003E4__this.SerializeVersion(v)) && versionDetails[this.\u003C\u003E4__this.SerializeVersion(v)].Views.Contains<Guid>(searchFilter.ViewId.Value)));
        }
        return source.ToList<TVersion>();
      }
    }

    protected async Task<List<TMetadataEntry>> GetLatestStateForAllVersionsOfPackageAsync(
      IVssRequestContext requestContext,
      FeedCore feed,
      IPackageName packageName,
      VersionSearchFilter searchFilter,
      Locator itemLocator)
    {
      PackageMetadataService<TMetadataEntry, TItem, TVersion> packageMetadataService = this;
      List<TMetadataEntry> list;
      using (Microsoft.VisualStudio.Services.Content.Server.Common.Tracer.Enter(requestContext, PackageMetadataService<TMetadataEntry, TItem, TVersion>.TraceData, 5724320, nameof (GetLatestStateForAllVersionsOfPackageAsync)))
      {
        FeedSecurityHelper.CheckReadFeedPermissions(requestContext, feed);
        IItemStore itemStore = packageMetadataService.GetItemStore(requestContext);
        Locator feedContainerName = PackagingUtils.ComputeFeedContainerName(feed);
        IVssRequestContext requestContext1 = requestContext;
        Locator containerName = feedContainerName;
        Locator pathPrefix = itemLocator;
        // ISSUE: reference to a compiler-generated field
        IEnumerable<TMetadataEntry> source = (await (await itemStore.GetItemsConcurrentIteratorAsync<TItem>(requestContext1, containerName, pathPrefix, PathOptions.ImmediateChildren)).ToDictionaryAsync<Locator, TItem>(requestContext.CancellationToken)).Values.Where<TItem>((Func<TItem, bool>) (x => x.ItemType == this.\u003C\u003E4__this.CommitLogItemType)).Select<TItem, TMetadataEntry>(new Func<TItem, TMetadataEntry>(packageMetadataService.ConvertItemToEntry));
        VersionSearchFilter versionSearchFilter = searchFilter;
        if ((versionSearchFilter != null ? (versionSearchFilter.ViewId.HasValue ? 1 : 0) : 0) != 0)
          source = source.Where<TMetadataEntry>((Func<TMetadataEntry, bool>) (x => x.Views.Contains<Guid>(searchFilter.ViewId.Value)));
        list = source.ToList<TMetadataEntry>();
      }
      return list;
    }

    protected abstract TMetadataEntry ConvertItemToEntry(TItem commitLogItem);

    protected abstract IItemStore GetItemStore(IVssRequestContext requestContext);

    protected abstract TVersion ParseVersion(string rawVersion);

    protected abstract string SerializeVersion(TVersion version);

    protected Locator GetPackageVersionCommitItemLocator(
      IVssRequestContext requestContext,
      IPackageName packageName,
      IPackageVersion packageVersion)
    {
      return new Locator(new string[3]
      {
        this.GetMetadataBaseFolder(requestContext),
        packageName.NormalizedName,
        packageVersion.NormalizedVersion
      });
    }

    protected Locator GetPackageCommitItemsLocator(
      IVssRequestContext requestContext,
      IPackageName packageName)
    {
      return new Locator(new string[2]
      {
        this.GetMetadataBaseFolder(requestContext),
        packageName.NormalizedName
      });
    }

    protected Locator GetPackageVersionListLocator(
      IVssRequestContext requestContext,
      IPackageName packageName)
    {
      return new Locator(new string[3]
      {
        this.GetMetadataBaseFolder(requestContext),
        packageName.NormalizedName,
        "versionList"
      });
    }

    protected string GetMetadataBaseFolder(IVssRequestContext requestContext)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.ProjectCollection);
      int num = vssRequestContext.GetService<IVssRegistryService>().GetValue<int>(vssRequestContext, (RegistryQuery) this.AlternateMetadataRevision, 0);
      return num > 0 ? "commitsByIdentity" + string.Format("/{0}/", (object) num) : "commitsByIdentity";
    }

    private FeedCore GetFeed(IVssRequestContext requestContext, Guid feedId)
    {
      IFeedCacheService service = requestContext.GetService<IFeedCacheService>();
      Guid empty = Guid.Empty;
      IVssRequestContext requestContext1 = requestContext;
      Guid projectId = empty;
      string feedNameOrId = feedId.ToString();
      return service.GetFeed(requestContext1, projectId, feedNameOrId);
    }
  }
}
