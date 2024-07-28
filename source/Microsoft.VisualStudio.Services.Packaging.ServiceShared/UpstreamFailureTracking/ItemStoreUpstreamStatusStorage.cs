// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamFailureTracking.ItemStoreUpstreamStatusStorage
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.ItemStore.Common;
using Microsoft.VisualStudio.Services.Packaging.CrossProtocol.Internal.WebApi.Types;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataStores.ItemStore;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamFailureTracking
{
  public class ItemStoreUpstreamStatusStorage : IUpstreamStatusStorage
  {
    private readonly IContentItemstore itemStore;
    private readonly IRetryHelper saveRetryHelper;
    private const string DateSegmentFormat = "yyyyMMdd";
    private const string UpstreamIdSegmentFormat = "d";
    private const string FeedIdSegmentFormat = "d";
    private const string CategorySegmentFormat = "G";
    private const string LastFullRefreshToken = "$LastFullRefresh";

    public ItemStoreUpstreamStatusStorage(IContentItemstore itemStore, IRetryHelper saveRetryHelper)
    {
      this.itemStore = itemStore ?? throw new ArgumentNullException(nameof (itemStore));
      this.saveRetryHelper = saveRetryHelper ?? throw new ArgumentNullException(nameof (saveRetryHelper));
    }

    public async Task ClearOldStatusRecords(DateTime maxTimestampToDelete)
    {
      CommitLogUtils.CheckDateIsUtc(maxTimestampToDelete, nameof (maxTimestampToDelete));
      CancellationToken cancellationToken = CancellationToken.None;
      DateTime date;
      await (await this.itemStore.GetContainersConcurrentIteratorAsync(ItemStoreUpstreamStatusStorage.RootContainerLocator, PathOptions.ImmediateChildren)).Select(x => !ItemStoreUpstreamStatusStorage.TryParseContainerLocator(x.Key, out date) ? null : new
      {
        Date = date,
        Container = x.Value
      }).Where(x => x != null && x.Date.Date < maxTimestampToDelete.Date).Select(x => x.Container).ForEachAsyncCaptureContext<ContainerItem>(cancellationToken, (Func<ContainerItem, Task>) (container => (Task) this.itemStore.DeleteContainerAsync(container)));
      cancellationToken = new CancellationToken();
    }

    private static bool TryParseContainerLocator(Locator locator, out DateTime date)
    {
      if (!locator.StartsWith(ItemStoreUpstreamStatusStorage.RootContainerLocator))
      {
        date = DateTime.MinValue;
        return false;
      }
      if (locator.PathSegmentCount > ItemStoreUpstreamStatusStorage.RootContainerLocator.PathSegmentCount)
        return DateTime.TryParseExact(locator.PathSegments[ItemStoreUpstreamStatusStorage.RootContainerLocator.PathSegmentCount], "yyyyMMdd", (IFormatProvider) CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal, out date);
      date = DateTime.MinValue;
      return false;
    }

    private static bool TryParsePathLocator(
      Locator locator,
      out Guid feedId,
      out Guid upstreamId,
      out bool isLastFullRefresh)
    {
      feedId = Guid.Empty;
      upstreamId = Guid.Empty;
      isLastFullRefresh = false;
      if (!locator.StartsWith(ItemStoreUpstreamStatusStorage.RootPathLocator))
        return false;
      int pathSegmentCount = ItemStoreUpstreamStatusStorage.RootPathLocator.PathSegmentCount;
      if (locator.PathSegmentCount < pathSegmentCount + 3 || !Guid.TryParseExact(locator.PathSegments[pathSegmentCount], "d", out feedId) || !Guid.TryParseExact(locator.PathSegments[pathSegmentCount + 1], "d", out upstreamId))
        return false;
      isLastFullRefresh = locator.PathSegments[pathSegmentCount + 2].Equals("$LastFullRefresh", StringComparison.OrdinalIgnoreCase);
      return true;
    }

    private IEnumerable<DateTime> GetAllDatesBetween(DateTime fromInclusive, DateTime toInclusive)
    {
      CommitLogUtils.CheckDateIsUtc(fromInclusive, nameof (fromInclusive));
      CommitLogUtils.CheckDateIsUtc(toInclusive, nameof (toInclusive));
      DateTime endDate = !(toInclusive < fromInclusive) ? toInclusive.Date : throw new ArgumentException("The value of toInclusive must not be earlier than the value of fromInclusive.", nameof (toInclusive));
      for (DateTime date = fromInclusive.Date; date <= endDate; date = date.AddDays(1.0))
        yield return date.Date;
    }

    private static Locator RootContainerLocator { get; } = new Locator(new string[1]
    {
      "upstreamStatusByDate"
    });

    private static Locator GetContainerLocator(DateTime date) => ItemStoreUpstreamStatusStorage.RootContainerLocator.Append(date.ToString("yyyyMMdd", (IFormatProvider) CultureInfo.InvariantCulture));

    private static Locator RootPathLocator { get; } = new Locator(new string[1]
    {
      "feeds"
    });

    private static Locator GetPathLocatorForFeed(Guid downstreamFeedId) => ItemStoreUpstreamStatusStorage.RootPathLocator.Append(downstreamFeedId.ToString("d"));

    private static Locator GetPathLocatorForFeedUpstream(UpstreamKey upstream) => ItemStoreUpstreamStatusStorage.GetPathLocatorForFeed(upstream.DownstreamFeedId).Append(upstream.UpstreamId.ToString("d"));

    private static Locator GetPathLocatorForFeedUpstreamCategory(
      UpstreamKey upstream,
      UpstreamStatusCategory category)
    {
      return ItemStoreUpstreamStatusStorage.GetPathLocatorForFeedUpstream(upstream).Append(category.ToString("G"));
    }

    private static Locator GetPathLocatorForFeedUpstreamFullRefresh(UpstreamKey upstream) => ItemStoreUpstreamStatusStorage.GetPathLocatorForFeedUpstream(upstream).Append("$LastFullRefresh");

    public async Task<IReadOnlyList<UpstreamStatusRecord>> GetUpstreamStatus(
      IProtocolAgnosticFeedRequest feedRequest,
      DateTime fromInclusive,
      DateTime toInclusive)
    {
      List<KeyValuePair<Locator, UpstreamStatusItem>> items = new List<KeyValuePair<Locator, UpstreamStatusItem>>();
      foreach (DateTime date in this.GetAllDatesBetween(fromInclusive, toInclusive))
      {
        IConcurrentIterator<KeyValuePair<Locator, UpstreamStatusItem>> concurrentIteratorAsync = await this.itemStore.GetItemsConcurrentIteratorAsync<UpstreamStatusItem>(ItemStoreUpstreamStatusStorage.GetContainerLocator(date), ItemStoreUpstreamStatusStorage.GetPathLocatorForFeed(feedRequest.Feed.Id), PathOptions.AllChildren);
        List<KeyValuePair<Locator, UpstreamStatusItem>> keyValuePairList = items;
        CancellationToken none = CancellationToken.None;
        keyValuePairList.AddRange((IEnumerable<KeyValuePair<Locator, UpstreamStatusItem>>) await concurrentIteratorAsync.ToListAsync<KeyValuePair<Locator, UpstreamStatusItem>>(none));
        keyValuePairList = (List<KeyValuePair<Locator, UpstreamStatusItem>>) null;
      }
      Guid feedId;
      Guid upstreamId;
      bool isLastFullRefresh;
      IReadOnlyList<UpstreamStatusRecord> list = (IReadOnlyList<UpstreamStatusRecord>) feedRequest.Feed.UpstreamSources.GroupJoin(items.Select(x => !ItemStoreUpstreamStatusStorage.TryParsePathLocator(x.Key, out feedId, out upstreamId, out isLastFullRefresh) ? null : new
      {
        FeedId = feedId,
        UpstreamId = upstreamId,
        IsLastFullRefresh = isLastFullRefresh,
        Timestamp = x.Value.Timestamp,
        Categories = x.Value.Categories
      }).Where(x => x != null && x.Timestamp >= fromInclusive && x.Timestamp <= toInclusive).ToList(), (Func<UpstreamSource, Guid>) (x => x.Id), x => x.UpstreamId, (source, records) =>
      {
        FullRefreshStatusRecord fullRefreshStatus = (FullRefreshStatusRecord) null;
        Dictionary<UpstreamStatusCategory, DateTime> source1 = new Dictionary<UpstreamStatusCategory, DateTime>();
        foreach (var record in records)
        {
          if (record.IsLastFullRefresh)
          {
            if (fullRefreshStatus == null || fullRefreshStatus.Timestamp < record.Timestamp)
              fullRefreshStatus = new FullRefreshStatusRecord(record.Timestamp, record.Categories);
          }
          else
          {
            foreach (UpstreamStatusCategory category in record.Categories)
            {
              DateTime dateTime;
              if (!source1.TryGetValue(category, out dateTime) || dateTime < record.Timestamp)
                source1[category] = record.Timestamp;
            }
          }
        }
        return new UpstreamStatusRecord(UpstreamKey.FromFeed(feedRequest.Feed, source), fullRefreshStatus, source1.Select<KeyValuePair<UpstreamStatusCategory, DateTime>, PartialRefreshStatusRecord>((Func<KeyValuePair<UpstreamStatusCategory, DateTime>, PartialRefreshStatusRecord>) (x => new PartialRefreshStatusRecord(x.Value, x.Key))), fromInclusive, toInclusive);
      }).ToList<UpstreamStatusRecord>();
      items = (List<KeyValuePair<Locator, UpstreamStatusItem>>) null;
      return list;
    }

    public async Task AddUpstreamRefreshStatus(
      UpstreamKey upstream,
      UpstreamRefreshScope refreshScope,
      IEnumerable<UpstreamStatusCategory> categories,
      DateTime timestamp)
    {
      ItemStoreUpstreamStatusStorage upstreamStatusStorage = this;
      Locator containerLocator = ItemStoreUpstreamStatusStorage.GetContainerLocator(timestamp);
      if (refreshScope == UpstreamRefreshScope.Full)
        await upstreamStatusStorage.ItemStoreSaveCycle<UpstreamStatusItem, object>(containerLocator, (IEnumerable<(Locator, object)>) new (Locator, object)[1]
        {
          (ItemStoreUpstreamStatusStorage.GetPathLocatorForFeedUpstreamFullRefresh(upstream), (object) null)
        }, CancellationToken.None, (Func<Locator, object, UpstreamStatusItem>) ((_, __) => new UpstreamStatusItem()
        {
          Categories = categories,
          Timestamp = timestamp
        }), (Func<Locator, object, UpstreamStatusItem, bool>) ((_, __, item) => UpdateItem(item, categories)));
      else
        await upstreamStatusStorage.ItemStoreSaveCycle<UpstreamStatusItem, UpstreamStatusCategory>(containerLocator, categories.Select<UpstreamStatusCategory, (Locator, UpstreamStatusCategory)>((Func<UpstreamStatusCategory, (Locator, UpstreamStatusCategory)>) (category => (ItemStoreUpstreamStatusStorage.GetPathLocatorForFeedUpstreamCategory(upstream, category), category))), CancellationToken.None, (Func<Locator, UpstreamStatusCategory, UpstreamStatusItem>) ((_, category) => new UpstreamStatusItem()
        {
          Categories = (IEnumerable<UpstreamStatusCategory>) new UpstreamStatusCategory[1]
          {
            category
          },
          Timestamp = timestamp
        }), (Func<Locator, UpstreamStatusCategory, UpstreamStatusItem, bool>) ((_, category, item) => UpdateItem(item, (IEnumerable<UpstreamStatusCategory>) new UpstreamStatusCategory[1]
        {
          category
        })));

      bool UpdateItem(UpstreamStatusItem item, IEnumerable<UpstreamStatusCategory> newCategories)
      {
        if (item.Timestamp >= timestamp)
          return false;
        item.Categories = newCategories;
        item.Timestamp = timestamp;
        return true;
      }
    }

    private async Task ItemStoreSaveCycle<TItem, TData>(
      Locator containerLocator,
      IEnumerable<(Locator Locator, TData Data)> itemLocators,
      CancellationToken cancellationToken,
      Func<Locator, TData, TItem> getNewItem,
      Func<Locator, TData, TItem, bool> updateItem)
      where TItem : StoredItem
    {
      ImmutableList<(Locator, TData)> itemLocatorsList = itemLocators.ToImmutableList<(Locator, TData)>();
      ContainerItem addContainerAsync = await this.itemStore.GetOrAddContainerAsync(new ContainerItem()
      {
        Name = containerLocator
      });
      IDictionary<Locator, bool> dictionary = await this.saveRetryHelper.Invoke<IDictionary<Locator, bool>>(new Func<Task<IDictionary<Locator, bool>>>(TrySaveOnce), new Func<IDictionary<Locator, bool>, bool>(ShouldRetryOnResult));

      static bool ShouldRetryOnResult(IDictionary<Locator, bool> results) => results.Any<KeyValuePair<Locator, bool>>((Func<KeyValuePair<Locator, bool>, bool>) (x => !x.Value));

      async Task<IDictionary<Locator, bool>> TrySaveOnce()
      {
        List<Locator> list = itemLocatorsList.Select<(Locator, TData), Locator>((Func<(Locator, TData), Locator>) (x => x.Locator)).ToList<Locator>();
        IDictionary<Locator, TItem> dictionaryAsync = await (await this.itemStore.GetItemsConcurrentIteratorAsync<TItem>(containerLocator, (IReadOnlyCollection<Locator>) list)).ToDictionaryAsync<Locator, TItem>(cancellationToken);
        Dictionary<Locator, StoredItem> items = new Dictionary<Locator, StoredItem>();
        foreach ((Locator key, TData data) in itemLocatorsList)
        {
          TItem obj;
          if (!dictionaryAsync.TryGetValue(key, out obj) || (object) obj == null)
            items.Add(key, (StoredItem) getNewItem(key, data));
          else if (updateItem(key, data, obj))
            items.Add(key, (StoredItem) obj);
        }
        return await this.itemStore.CompareSwapItemsAsync(containerLocator, (IReadOnlyDictionary<Locator, StoredItem>) items, true);
      }
    }
  }
}
