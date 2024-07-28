// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.CommitLogServiceBase
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.BlobStore.Server.Common;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Feed.Common;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.ItemStore.Common;
using Microsoft.VisualStudio.Services.ItemStore.Server.Common;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BookmarkTokens;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseSerializers;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog
{
  public abstract class CommitLogServiceBase : 
    ICommitLogService,
    IRetrievableCommitLogService,
    ICoreCommitLogService,
    IVssFrameworkService,
    IUnsafeCommitLogService
  {
    public const int MaxEntriesAddedPerTransaction = 98;
    private const string CommitLogItemFolderName = "commitLog";
    private readonly ITimeProvider timeProvider;

    public CommitLogServiceBase(ITimeProvider timeProvider) => this.timeProvider = timeProvider;

    public abstract string CommitType { get; }

    protected abstract ICommitEntryDeserializer GetDeserializer(IVssRequestContext requestContext);

    protected abstract ICommitEntrySerializer GetSerializer(IVssRequestContext requestContext);

    public abstract void ServiceStart(IVssRequestContext systemRequestContext);

    public abstract void ServiceEnd(IVssRequestContext systemRequestContext);

    public async Task<IList<CommitLogEntry>> BatchReadAsync(
      IVssRequestContext requestContext,
      FeedCore feed,
      string itemType,
      DateTime? timeWindowStart = null)
    {
      IList<CommitLogEntry> commitLogEntryList1;
      using (Microsoft.VisualStudio.Services.Content.Server.Common.Tracer.Enter(requestContext, Microsoft.VisualStudio.Services.Packaging.ServiceShared.Constants.TracePoints.CommitLogService.TraceData, 5724150, nameof (BatchReadAsync)))
      {
        FeedSecurityHelper.CheckReadFeedPermissions(requestContext, feed);
        IItemStore itemStore = this.GetItemStore(requestContext);
        Locator feedContainerName = this.ComputeFeedContainerName(feed);
        IVssRequestContext requestContext1 = requestContext;
        Locator containerName = feedContainerName;
        Locator pathPrefix = new Locator(new string[1]
        {
          "commitLog"
        });
        List<KeyValuePair<Locator, CommitLogItem>> listAsync = await (await itemStore.GetItemsConcurrentIteratorAsync<CommitLogItem>(requestContext1, containerName, pathPrefix, PathOptions.AllChildren)).ToListAsync<KeyValuePair<Locator, CommitLogItem>>(requestContext.CancellationToken);
        ICommitEntryDeserializer deserializer = this.GetDeserializer(requestContext);
        List<CommitLogEntry> commitLogEntryList2;
        if (listAsync != null)
        {
          commitLogEntryList2 = new List<CommitLogEntry>();
          foreach (KeyValuePair<Locator, CommitLogItem> keyValuePair in listAsync)
          {
            if (keyValuePair.Value.ItemType.Contains(itemType) && (!timeWindowStart.HasValue || keyValuePair.Value.CreatedDate >= timeWindowStart.Value))
              commitLogEntryList2.Add(this.ConvertItemToEntry(deserializer, keyValuePair.Value));
          }
        }
        commitLogEntryList1 = (IList<CommitLogEntry>) commitLogEntryList2;
      }
      return commitLogEntryList1;
    }

    public async Task<List<CommitLogRepairItem>> DeleteEntryAsync(
      IVssRequestContext requestContext,
      FeedCore feed,
      PackagingCommitId badEntry)
    {
      requestContext.CheckSystemRequestContext();
      List<CommitLogRepairItem> repairedEntries = new List<CommitLogRepairItem>();
      List<CommitLogRepairItem> commitLogRepairItemList1;
      using (Microsoft.VisualStudio.Services.Content.Server.Common.Tracer.Enter(requestContext, Microsoft.VisualStudio.Services.Packaging.ServiceShared.Constants.TracePoints.CommitLogService.TraceData, 5724170, nameof (DeleteEntryAsync)))
      {
        IItemStore itemStore = this.GetItemStore(requestContext);
        Locator containerLocator = this.ComputeFeedContainerName(feed);
        Locator itemLocator = this.GetCommitLogEntryItemLocator(badEntry);
        CommitLogItem badItem = await itemStore.GetItemAsync<CommitLogItem>(requestContext, containerLocator, itemLocator);
        PackagingCommitId previousCommitId = badItem.PreviousPackagingCommitId;
        PackagingCommitId nextCommitId = badItem.NextPackagingCommitId;
        Dictionary<Locator, StoredItem> items = new Dictionary<Locator, StoredItem>();
        Locator previtemLocator;
        if (previousCommitId.ToGuid() != Guid.Empty)
        {
          previtemLocator = this.GetCommitLogEntryItemLocator(previousCommitId);
          CommitLogItem itemAsync = await itemStore.GetItemAsync<CommitLogItem>(requestContext, containerLocator, previtemLocator);
          itemAsync.NextPackagingCommitId = nextCommitId;
          repairedEntries.Add(new CommitLogRepairItem(itemAsync.PackagingCommitId, nextCommitId, RepairType.ForwardPointer));
          items.Add(previtemLocator, (StoredItem) itemAsync);
          previtemLocator = (Locator) null;
        }
        else
        {
          previtemLocator = this.GetOldestCommitBookmarkItemLocator();
          EtagValue<CommitLogBookmark> bookmarkInternalAsync = await this.GetOldestCommitBookmarkInternalAsync(requestContext, feed);
          CommitLogPointerItem commitLogPointerItem1 = new CommitLogPointerItem();
          commitLogPointerItem1.Target = nextCommitId;
          commitLogPointerItem1.StorageETag = bookmarkInternalAsync.Etag;
          CommitLogPointerItem commitLogPointerItem2 = commitLogPointerItem1;
          repairedEntries.Add(new CommitLogRepairItem(bookmarkInternalAsync.Value.CommitId, nextCommitId, RepairType.Tail));
          items.Add(previtemLocator, (StoredItem) commitLogPointerItem2);
          previtemLocator = (Locator) null;
        }
        if (nextCommitId.ToGuid() != Guid.Empty)
        {
          previtemLocator = this.GetCommitLogEntryItemLocator(nextCommitId);
          CommitLogItem itemAsync = await itemStore.GetItemAsync<CommitLogItem>(requestContext, containerLocator, previtemLocator);
          itemAsync.PreviousPackagingCommitId = previousCommitId;
          repairedEntries.Add(new CommitLogRepairItem(itemAsync.PackagingCommitId, previousCommitId, RepairType.PreviousPointer));
          items.Add(previtemLocator, (StoredItem) itemAsync);
          previtemLocator = (Locator) null;
        }
        else
        {
          previtemLocator = this.GetNewestCommitBookmarkItemLocator();
          EtagValue<CommitLogBookmark> bookmarkInternalAsync = await this.GetNewestCommitBookmarkInternalAsync(requestContext, feed);
          CommitLogPointerItem commitLogPointerItem3 = new CommitLogPointerItem();
          commitLogPointerItem3.Target = previousCommitId;
          commitLogPointerItem3.StorageETag = bookmarkInternalAsync.Etag;
          CommitLogPointerItem commitLogPointerItem4 = commitLogPointerItem3;
          repairedEntries.Add(new CommitLogRepairItem(bookmarkInternalAsync.Value.CommitId, previousCommitId, RepairType.Head));
          items.Add(previtemLocator, (StoredItem) commitLogPointerItem4);
          previtemLocator = (Locator) null;
        }
        if (await itemStore.DeleteItemAsync(requestContext, containerLocator, itemLocator, (StoredItem) badItem))
        {
          if ((await itemStore.CompareSwapItemsAsync(requestContext, containerLocator, (IReadOnlyDictionary<Locator, StoredItem>) items, true)).Any<KeyValuePair<Locator, bool>>((Func<KeyValuePair<Locator, bool>, bool>) (x => !x.Value)))
            throw new TargetModifiedAfterReadException(nameof (DeleteEntryAsync));
          EtagValue<CommitLogBookmark> tailAfterDelete = await this.GetOldestCommitBookmarkInternalAsync(requestContext, feed);
          EtagValue<CommitLogBookmark> bookmarkInternalAsync = await this.GetNewestCommitBookmarkInternalAsync(requestContext, feed);
          if (tailAfterDelete.Value == CommitLogBookmark.Empty && bookmarkInternalAsync.Value == CommitLogBookmark.Empty)
          {
            List<CommitLogRepairItem> repairedHeadTailEntries = await this.DeleteHeadTailEntryAsync(requestContext, containerLocator, RepairType.HeadDeletion);
            List<CommitLogRepairItem> commitLogRepairItemList = repairedHeadTailEntries;
            commitLogRepairItemList.AddRange((IEnumerable<CommitLogRepairItem>) await this.DeleteHeadTailEntryAsync(requestContext, containerLocator, RepairType.TailDeletion));
            commitLogRepairItemList = (List<CommitLogRepairItem>) null;
            if (repairedHeadTailEntries.Count<CommitLogRepairItem>() < 2)
              repairedEntries.Clear();
            else
              repairedEntries.AddRange((IEnumerable<CommitLogRepairItem>) repairedHeadTailEntries);
            repairedHeadTailEntries = (List<CommitLogRepairItem>) null;
          }
          tailAfterDelete = new EtagValue<CommitLogBookmark>();
        }
        else
          repairedEntries.Clear();
        commitLogRepairItemList1 = repairedEntries;
      }
      repairedEntries = (List<CommitLogRepairItem>) null;
      return commitLogRepairItemList1;
    }

    public async Task<Guid> GetCommitLogIdAsync(IVssRequestContext requestContext, FeedCore feed)
    {
      Guid guid;
      using (Microsoft.VisualStudio.Services.Content.Server.Common.Tracer.Enter(requestContext, Microsoft.VisualStudio.Services.Packaging.ServiceShared.Constants.TracePoints.CommitLogService.TraceData, 5724160, nameof (GetCommitLogIdAsync)))
      {
        FeedSecurityHelper.CheckReadFeedPermissions(requestContext, feed);
        guid = (await this.GetOldestCommitBookmarkInternalAsync(requestContext, feed)).Value.CommitId.ToGuid();
      }
      return guid;
    }

    public async Task<CommitLogBookmark> GetNewestCommitBookmarkAsync(
      IVssRequestContext requestContext,
      FeedCore feed)
    {
      CommitLogBookmark commitBookmarkAsync;
      using (Microsoft.VisualStudio.Services.Content.Server.Common.Tracer.Enter(requestContext, Microsoft.VisualStudio.Services.Packaging.ServiceShared.Constants.TracePoints.CommitLogService.TraceData, 5724100, nameof (GetNewestCommitBookmarkAsync)))
      {
        FeedSecurityHelper.CheckReadFeedPermissions(requestContext, feed);
        commitBookmarkAsync = (await this.GetNewestCommitBookmarkInternalAsync(requestContext, feed)).Value;
      }
      return commitBookmarkAsync;
    }

    public async Task<CommitLogBookmark> GetOldestCommitBookmarkAsync(
      IVssRequestContext requestContext,
      FeedCore feed)
    {
      CommitLogBookmark commitBookmarkAsync;
      using (Microsoft.VisualStudio.Services.Content.Server.Common.Tracer.Enter(requestContext, Microsoft.VisualStudio.Services.Packaging.ServiceShared.Constants.TracePoints.CommitLogService.TraceData, 5724130, nameof (GetOldestCommitBookmarkAsync)))
      {
        FeedSecurityHelper.CheckReadFeedPermissions(requestContext, feed);
        commitBookmarkAsync = (await this.GetOldestCommitBookmarkInternalAsync(requestContext, feed)).Value;
      }
      return commitBookmarkAsync;
    }

    public async Task<CommitLogEntry> GetEntryAsync(
      IVssRequestContext requestContext,
      FeedCore feed,
      PackagingCommitId packagingCommitId)
    {
      CommitLogEntry entry;
      using (Microsoft.VisualStudio.Services.Content.Server.Common.Tracer.Enter(requestContext, Microsoft.VisualStudio.Services.Packaging.ServiceShared.Constants.TracePoints.CommitLogService.TraceData, 5724110, nameof (GetEntryAsync)))
      {
        FeedSecurityHelper.CheckReadFeedPermissions(requestContext, feed);
        CommitLogItem itemAsync = await this.GetItemStore(requestContext).GetItemAsync<CommitLogItem>(requestContext, this.ComputeFeedContainerName(feed), this.GetCommitLogEntryItemLocator(packagingCommitId));
        entry = itemAsync != null ? this.ConvertItemToEntry(this.GetDeserializer(requestContext), itemAsync) : (CommitLogEntry) null;
      }
      return entry;
    }

    public async Task<CommitLogEntry> AppendEntryAsync(
      IVssRequestContext requestContext,
      FeedCore feed,
      ICommitOperationData operationData)
    {
      return (await this.AppendEntriesAsync(requestContext, feed, (IReadOnlyCollection<ICommitOperationData>) new ICommitOperationData[1]
      {
        operationData
      })).Single<CommitLogEntry>();
    }

    public async Task<IReadOnlyCollection<CommitLogEntry>> AppendEntriesAsync(
      IVssRequestContext requestContext,
      FeedCore feed,
      IReadOnlyCollection<ICommitOperationData> operations)
    {
      IReadOnlyCollection<CommitLogEntry> commitLogEntries1;
      using (Microsoft.VisualStudio.Services.Content.Server.Common.Tracer.Enter(requestContext, Microsoft.VisualStudio.Services.Packaging.ServiceShared.Constants.TracePoints.CommitLogService.TraceData, 5724120, nameof (AppendEntriesAsync)))
      {
        if (operations.Count < 1 || operations.Count > 98)
          throw new ArgumentException(string.Format("cannot process more than {0} operations or less than 1.", (object) 98));
        FeedPermsFacade permsFacade = new FeedPermsFacade(requestContext);
        operations.ForEach<ICommitOperationData>((Action<ICommitOperationData>) (o => permsFacade.Validate(feed, o.PermissionDemand)));
        IItemStore itemStore = this.GetItemStore(requestContext);
        Locator containerLocator = this.ComputeFeedContainerName(feed);
        Locator newestBookmarkLocator = this.GetNewestCommitBookmarkItemLocator();
        DateTime now = this.timeProvider.Now;
        ICommitEntrySerializer serializer = this.GetSerializer(requestContext);
        List<CommitLogItem> commitItems = operations.Select<ICommitOperationData, CommitLogItem>((Func<ICommitOperationData, CommitLogItem>) (o => new CommitLogItem((IItemData) new DictionaryItemData((IDictionary<string, object>) serializer.Serialize(o).ToDictionary<KeyValuePair<string, string>, string, object>((Func<KeyValuePair<string, string>, string>) (x => x.Key), (Func<KeyValuePair<string, string>, object>) (x => (object) x.Value))), this.CommitType)
        {
          UserId = requestContext.GetUserId(),
          CreatedDate = now,
          ModifiedDate = now,
          PackagingCommitId = PackagingCommitId.CreateNew()
        })).ToList<CommitLogItem>();
        for (int index = 0; index < commitItems.Count; ++index)
        {
          if (index > 0)
            commitItems[index].PreviousPackagingCommitId = commitItems[index - 1].PackagingCommitId;
          if (index < commitItems.Count - 1)
            commitItems[index].NextPackagingCommitId = commitItems[index + 1].PackagingCommitId;
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        await new RetryHelper(requestContext, (IReadOnlyList<TimeSpan>) RetryUtils.GetRetryProfile("CommitLog", requestContext), CommitLogServiceBase.\u003C\u003EO.\u003C0\u003E__IsRetryableStorageException ?? (CommitLogServiceBase.\u003C\u003EO.\u003C0\u003E__IsRetryableStorageException = new Func<Exception, bool>(RetryUtils.IsRetryableStorageException))).Invoke((Func<Task>) (async () =>
        {
          CommitLogItem firstItemInBatch = commitItems.First<CommitLogItem>();
          CommitLogItem lastItemInBatch = commitItems.Last<CommitLogItem>();
          EtagValue<CommitLogBookmark> currentNewestBookmark = await this.GetNewestCommitBookmarkInternalAsync(requestContext, feed);
          firstItemInBatch.PreviousPackagingCommitId = currentNewestBookmark.Value.CommitId;
          Dictionary<Locator, StoredItem> items = new Dictionary<Locator, StoredItem>();
          commitItems.ForEach((Action<CommitLogItem>) (c => items.Add(this.GetCommitLogEntryItemLocator(c.PackagingCommitId), (StoredItem) c)));
          CommitLogItem commitLogItem = (CommitLogItem) null;
          if (currentNewestBookmark.Value != CommitLogBookmark.Empty)
          {
            Locator currentNewestLocator = this.GetCommitLogEntryItemLocator(currentNewestBookmark.Value.CommitId);
            commitLogItem = await itemStore.GetItemAsync<CommitLogItem>(requestContext, containerLocator, currentNewestLocator);
            if (commitLogItem == null)
              throw new MissingCommitLogEntryException(feed.Id, currentNewestBookmark.Value.CommitId, "HEAD");
            commitLogItem.NextPackagingCommitId = firstItemInBatch.PackagingCommitId;
            commitLogItem.ModifiedDate = firstItemInBatch.CreatedDate;
            items.Add(currentNewestLocator, (StoredItem) commitLogItem);
            currentNewestLocator = (Locator) null;
          }
          for (int index = 0; index < commitItems.Count; ++index)
          {
            long num = (commitLogItem != null ? commitLogItem.SequenceNumber : 0L) + (long) index + 1L;
            commitItems[index].SequenceNumber = num;
          }
          if (currentNewestBookmark.Value == CommitLogBookmark.Empty)
          {
            Locator bookmarkItemLocator = this.GetOldestCommitBookmarkItemLocator();
            CommitLogPointerItem commitLogPointerItem = new CommitLogPointerItem()
            {
              Target = firstItemInBatch.PackagingCommitId,
              SequenceNumber = new long?(firstItemInBatch.SequenceNumber),
              StorageETag = (string) null
            };
            items.Add(bookmarkItemLocator, (StoredItem) commitLogPointerItem);
          }
          CommitLogPointerItem commitLogPointerItem1 = new CommitLogPointerItem()
          {
            Target = lastItemInBatch.PackagingCommitId,
            SequenceNumber = new long?(lastItemInBatch.SequenceNumber),
            StorageETag = currentNewestBookmark.Etag
          };
          items.Add(newestBookmarkLocator, (StoredItem) commitLogPointerItem1);
          if ((await itemStore.CompareSwapItemsAsync(requestContext, containerLocator, (IReadOnlyDictionary<Locator, StoredItem>) items, true)).Any<KeyValuePair<Locator, bool>>((Func<KeyValuePair<Locator, bool>, bool>) (x => !x.Value)))
            throw new TargetModifiedAfterReadException("AppendEntryAsync");
          firstItemInBatch = (CommitLogItem) null;
          lastItemInBatch = (CommitLogItem) null;
          currentNewestBookmark = new EtagValue<CommitLogBookmark>();
        }));
        ICommitEntryDeserializer deserializer = this.GetDeserializer(requestContext);
        List<CommitLogEntry> commitLogEntries = commitItems.Select<CommitLogItem, CommitLogEntry>((Func<CommitLogItem, CommitLogEntry>) (commitItem => this.ConvertItemToEntry(deserializer, commitItem))).ToList<CommitLogEntry>();
        RequestContextItemsAsCacheFacade requestContextAsItemsCache = new RequestContextItemsAsCacheFacade(requestContext);
        NullResult nullResult1 = await new DecorateCacheWithCommitLogBookmarkHandler((IConverter<CommitLogBookmark, string>) new CommitLogBookmarkSerializingConverter(), (ICache<string, object>) requestContextAsItemsCache).Handle((ICommitLogEntry) commitLogEntries.Last<CommitLogEntry>());
        NullResult nullResult2 = await new DecorateCacheWithCommitDetailsHandler((ICache<string, object>) requestContextAsItemsCache).Handle((IReadOnlyCollection<ICommitLogEntry>) commitLogEntries);
        commitLogEntries1 = (IReadOnlyCollection<CommitLogEntry>) commitLogEntries;
      }
      return commitLogEntries1;
    }

    public async Task MarkCommitLogEntryCorruptAsync(
      IVssRequestContext requestContext,
      FeedCore feed,
      PackagingCommitId commitId,
      string reason = null)
    {
      using (Microsoft.VisualStudio.Services.Content.Server.Common.Tracer.Enter(requestContext, Microsoft.VisualStudio.Services.Packaging.ServiceShared.Constants.TracePoints.CommitLogService.TraceData, 5724180, nameof (MarkCommitLogEntryCorruptAsync)))
      {
        ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
        ArgumentUtility.CheckForNull<FeedCore>(feed, nameof (feed));
        IItemStore itemStore = this.GetItemStore(requestContext);
        Locator containerLocator = this.ComputeFeedContainerName(feed);
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        RetryHelper retryHelper = new RetryHelper(requestContext, (IReadOnlyList<TimeSpan>) RetryUtils.GetRetryProfile("CommitLog", requestContext), CommitLogServiceBase.\u003C\u003EO.\u003C0\u003E__IsRetryableStorageException ?? (CommitLogServiceBase.\u003C\u003EO.\u003C0\u003E__IsRetryableStorageException = new Func<Exception, bool>(RetryUtils.IsRetryableStorageException)));
        Locator entryItemLocator = this.GetCommitLogEntryItemLocator(commitId);
        Func<Task> function = (Func<Task>) (async () =>
        {
          CommitLogItem itemAsync = await itemStore.GetItemAsync<CommitLogItem>(requestContext, containerLocator, entryItemLocator);
          itemAsync.CorruptEntry = true;
          itemAsync.CorruptReason = reason;
          Dictionary<Locator, StoredItem> items = new Dictionary<Locator, StoredItem>()
          {
            {
              entryItemLocator,
              (StoredItem) itemAsync
            }
          };
          if ((await itemStore.CompareSwapItemsAsync(requestContext, containerLocator, (IReadOnlyDictionary<Locator, StoredItem>) items, true)).Any<KeyValuePair<Locator, bool>>((Func<KeyValuePair<Locator, bool>, bool>) (x => !x.Value)))
            throw new TargetModifiedAfterReadException(nameof (MarkCommitLogEntryCorruptAsync));
        });
        await retryHelper.Invoke(function);
      }
    }

    protected Locator ComputeFeedContainerName(FeedCore feed) => PackagingUtils.ComputeFeedContainerName(feed);

    protected abstract IItemStore GetItemStore(IVssRequestContext requestContext);

    private Task<EtagValue<CommitLogBookmark>> GetNewestCommitBookmarkInternalAsync(
      IVssRequestContext requestContext,
      FeedCore feed)
    {
      return this.GetBookmarkAsync(requestContext, feed, this.GetNewestCommitBookmarkItemLocator());
    }

    private Task<EtagValue<CommitLogBookmark>> GetOldestCommitBookmarkInternalAsync(
      IVssRequestContext requestContext,
      FeedCore feed)
    {
      return this.GetBookmarkAsync(requestContext, feed, this.GetOldestCommitBookmarkItemLocator());
    }

    private Locator GetOldestCommitBookmarkItemLocator() => new Locator(new string[2]
    {
      "commitLog",
      "TAIL"
    });

    private Locator GetNewestCommitBookmarkItemLocator() => new Locator(new string[2]
    {
      "commitLog",
      "HEAD"
    });

    private Locator GetCommitLogEntryItemLocator(PackagingCommitId packagingCommitId) => new Locator(new string[2]
    {
      "commitLog",
      packagingCommitId.ToString()
    });

    private async Task<EtagValue<CommitLogBookmark>> GetBookmarkAsync(
      IVssRequestContext requestContext,
      FeedCore feed,
      Locator itemLocator)
    {
      CommitLogPointerItem itemAsync = await this.GetItemStore(requestContext).GetItemAsync<CommitLogPointerItem>(requestContext, this.ComputeFeedContainerName(feed), itemLocator);
      return itemAsync != null ? new EtagValue<CommitLogBookmark>(new CommitLogBookmark(itemAsync.Target, itemAsync.SequenceNumber), itemAsync.StorageETag) : new EtagValue<CommitLogBookmark>(new CommitLogBookmark(PackagingCommitId.Empty, new long?()), (string) null);
    }

    private CommitLogEntry ConvertItemToEntry(
      ICommitEntryDeserializer deserializer,
      CommitLogItem item)
    {
      return new CommitLogEntry(deserializer.Deserialize(item), item.NextPackagingCommitId, item.PreviousPackagingCommitId, item.PackagingCommitId, item.SequenceNumber, item.CreatedDate, item.ModifiedDate, item.UserId, item.CorruptEntry, item.CorruptReason);
    }

    private async Task<List<CommitLogRepairItem>> DeleteHeadTailEntryAsync(
      IVssRequestContext requestContext,
      Locator containerLocator,
      RepairType repairType)
    {
      List<CommitLogRepairItem> repairedEntries = new List<CommitLogRepairItem>();
      using (Microsoft.VisualStudio.Services.Content.Server.Common.Tracer.Enter(requestContext, Microsoft.VisualStudio.Services.Packaging.ServiceShared.Constants.TracePoints.CommitLogService.TraceData, 5724190, nameof (DeleteHeadTailEntryAsync)))
      {
        IItemStore itemStore = this.GetItemStore(requestContext);
        Locator itemLocator = (Locator) null;
        switch (repairType)
        {
          case RepairType.HeadDeletion:
            itemLocator = this.GetNewestCommitBookmarkItemLocator();
            break;
          case RepairType.TailDeletion:
            itemLocator = this.GetOldestCommitBookmarkItemLocator();
            break;
        }
        if (await itemStore.DeleteItemAsync(requestContext, containerLocator, itemLocator, (StoredItem) await itemStore.GetItemAsync<CommitLogItem>(requestContext, containerLocator, itemLocator)))
          repairedEntries.Add(new CommitLogRepairItem(PackagingCommitId.Empty, PackagingCommitId.Empty, repairType));
        itemStore = (IItemStore) null;
        itemLocator = (Locator) null;
      }
      List<CommitLogRepairItem> commitLogRepairItemList = repairedEntries;
      repairedEntries = (List<CommitLogRepairItem>) null;
      return commitLogRepairItemList;
    }
  }
}
