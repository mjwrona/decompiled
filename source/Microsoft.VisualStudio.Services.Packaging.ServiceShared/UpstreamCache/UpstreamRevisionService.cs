// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamCache.UpstreamRevisionService
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Feed.Common;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.ItemStore.Common;
using Microsoft.VisualStudio.Services.ItemStore.Server.Common;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamCache
{
  public abstract class UpstreamRevisionService : IUpstreamRevisionService, IVssFrameworkService
  {
    private const int MaxRetries = 5;
    private const string RevisionLocatorKey = "upstreamRevisions";

    public abstract string Protocol { get; }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public List<UpstreamRevision> GetStaleUpstreamRevisions(
      IVssRequestContext requestContext,
      FeedCore feed)
    {
      using (Microsoft.VisualStudio.Services.Content.Server.Common.Tracer.Enter(requestContext, Microsoft.VisualStudio.Services.Packaging.ServiceShared.Constants.TracePoints.UpstreamRevisionCleanupService.TraceData, 5725710, nameof (GetStaleUpstreamRevisions)))
      {
        FeedSecurityHelper.CheckReadFeedPermissions(requestContext, feed);
        FeedSecurityHelper.CheckDeletePackagePermissions(requestContext, feed);
        IItemStore itemStore = this.GetItemStore(requestContext);
        Locator feedContainer = this.ComputeContainerName(feed);
        Locator tokenLocator = this.GetUpstreamRevisionLocator();
        return AsyncPump.Run<UpstreamRevisionsItem>((Func<Task<UpstreamRevisionsItem>>) (() => itemStore.GetItemAsync<UpstreamRevisionsItem>(requestContext, feedContainer, tokenLocator)))?.StaleRevisionList ?? new List<UpstreamRevision>();
      }
    }

    public void RemoveStaleUpstreamRevision(
      IVssRequestContext requestContext,
      FeedCore feed,
      UpstreamRevision revision)
    {
      using (Microsoft.VisualStudio.Services.Content.Server.Common.Tracer.Enter(requestContext, Microsoft.VisualStudio.Services.Packaging.ServiceShared.Constants.TracePoints.UpstreamRevisionCleanupService.TraceData, 5725720, nameof (RemoveStaleUpstreamRevision)))
      {
        FeedSecurityHelper.CheckReadFeedPermissions(requestContext, feed);
        FeedSecurityHelper.CheckDeletePackagePermissions(requestContext, feed);
        IItemStore itemStore = this.GetItemStore(requestContext);
        Locator feedContainer = this.ComputeContainerName(feed);
        Locator tokenLocator = this.GetUpstreamRevisionLocator();
        AsyncPump.Run<ContainerItem>((Func<Task<ContainerItem>>) (() => itemStore.GetOrAddContainerAsync(requestContext, new ContainerItem()
        {
          Name = feedContainer
        })));
        TimeSpan defaultMaxRetryDelay = RetryUtils.GetDefaultMaxRetryDelay(requestContext);
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        RetryHelper retryHelper = new RetryHelper(requestContext, 5, defaultMaxRetryDelay, UpstreamRevisionService.\u003C\u003EO.\u003C0\u003E__IsRetryableStorageException ?? (UpstreamRevisionService.\u003C\u003EO.\u003C0\u003E__IsRetryableStorageException = new Func<Exception, bool>(RetryUtils.IsRetryableStorageException)));
        AsyncPump.Run((Func<Task>) (() => retryHelper.Invoke((Func<Task>) (async () =>
        {
          // ISSUE: variable of a compiler-generated type
          UpstreamRevisionService.\u003C\u003Ec__DisplayClass8_1 cDisplayClass81 = this;
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated method
          UpstreamRevisionsItem upstreamRevisionsItem1 = AsyncPump.Run<UpstreamRevisionsItem>(cDisplayClass81.\u003C\u003E9__3 ?? (cDisplayClass81.\u003C\u003E9__3 = new Func<Task<UpstreamRevisionsItem>>(cDisplayClass81.\u003CRemoveStaleUpstreamRevision\u003Eb__3)));
          List<UpstreamRevision> upstreamRevisionList = upstreamRevisionsItem1?.StaleRevisionList ?? new List<UpstreamRevision>();
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated method
          if (upstreamRevisionList.RemoveAll(cDisplayClass81.CS\u0024\u003C\u003E8__locals1.\u003C\u003E9__4 ?? (cDisplayClass81.CS\u0024\u003C\u003E8__locals1.\u003C\u003E9__4 = new Predicate<UpstreamRevision>(cDisplayClass81.CS\u0024\u003C\u003E8__locals1.\u003CRemoveStaleUpstreamRevision\u003Eb__4))) == 0)
            return;
          UpstreamRevisionsItem upstreamRevisionsItem2 = new UpstreamRevisionsItem()
          {
            CurrentRevisionTable = upstreamRevisionsItem1?.CurrentRevisionTable ?? new Dictionary<string, uint>(),
            StaleRevisionList = upstreamRevisionList,
            StorageETag = upstreamRevisionsItem1?.StorageETag
          };
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          if (!await cDisplayClass81.itemStore.CompareSwapItemAsync(cDisplayClass81.CS\u0024\u003C\u003E8__locals1.requestContext, cDisplayClass81.feedContainer, cDisplayClass81.tokenLocator, (StoredItem) upstreamRevisionsItem2))
            throw new TargetModifiedAfterReadException(nameof (RemoveStaleUpstreamRevision));
        }))));
      }
    }

    public uint GetUpstreamPackagesRevision(
      IVssRequestContext requestContext,
      FeedCore feed,
      string upstream)
    {
      FeedSecurityHelper.CheckReadFeedPermissions(requestContext, feed);
      IItemStore itemStore = this.GetItemStore(requestContext);
      Locator feedContainer = this.ComputeContainerName(feed);
      Locator tokenLocator = this.GetUpstreamRevisionLocator();
      Dictionary<string, uint> currentRevisionTable = AsyncPump.Run<UpstreamRevisionsItem>((Func<Task<UpstreamRevisionsItem>>) (() => itemStore.GetItemAsync<UpstreamRevisionsItem>(requestContext, feedContainer, tokenLocator)))?.CurrentRevisionTable;
      return currentRevisionTable != null && currentRevisionTable.ContainsKey(upstream) ? currentRevisionTable[upstream] : 0U;
    }

    public uint CreateNewUpstreamPackagesRevision(
      IVssRequestContext requestContext,
      FeedCore feed,
      string upstream)
    {
      FeedSecurityHelper.CheckReadFeedPermissions(requestContext, feed);
      FeedSecurityHelper.CheckDeletePackagePermissions(requestContext, feed);
      IItemStore itemStore = this.GetItemStore(requestContext);
      Locator feedContainer = this.ComputeContainerName(feed);
      Locator tokenLocator = this.GetUpstreamRevisionLocator();
      AsyncPump.Run<ContainerItem>((Func<Task<ContainerItem>>) (() => itemStore.GetOrAddContainerAsync(requestContext, new ContainerItem()
      {
        Name = feedContainer
      })));
      TimeSpan defaultMaxRetryDelay = RetryUtils.GetDefaultMaxRetryDelay(requestContext);
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      RetryHelper retryHelper = new RetryHelper(requestContext, 5, defaultMaxRetryDelay, UpstreamRevisionService.\u003C\u003EO.\u003C0\u003E__IsRetryableStorageException ?? (UpstreamRevisionService.\u003C\u003EO.\u003C0\u003E__IsRetryableStorageException = new Func<Exception, bool>(RetryUtils.IsRetryableStorageException)));
      uint oldRevision = 0;
      uint revision = 0;
      AsyncPump.Run((Func<Task>) (() => retryHelper.Invoke((Func<Task>) (async () =>
      {
        // ISSUE: variable of a compiler-generated type
        UpstreamRevisionService.\u003C\u003Ec__DisplayClass10_0 cDisplayClass100 = this;
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated method
        UpstreamRevisionsItem upstreamRevisionsItem1 = AsyncPump.Run<UpstreamRevisionsItem>(cDisplayClass100.\u003C\u003E9__3 ?? (cDisplayClass100.\u003C\u003E9__3 = new Func<Task<UpstreamRevisionsItem>>(cDisplayClass100.\u003CCreateNewUpstreamPackagesRevision\u003Eb__3)));
        // ISSUE: reference to a compiler-generated field
        cDisplayClass100.oldRevision = 0U;
        Dictionary<string, uint> dictionary = upstreamRevisionsItem1?.CurrentRevisionTable ?? new Dictionary<string, uint>();
        // ISSUE: reference to a compiler-generated field
        if (dictionary.ContainsKey(cDisplayClass100.upstream))
        {
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          cDisplayClass100.oldRevision = dictionary[cDisplayClass100.upstream];
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        cDisplayClass100.revision = cDisplayClass100.oldRevision + 1U;
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        dictionary[cDisplayClass100.upstream] = cDisplayClass100.revision;
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        UpstreamRevision upstreamRevision = new UpstreamRevision()
        {
          Revision = cDisplayClass100.oldRevision,
          Source = cDisplayClass100.upstream
        };
        List<UpstreamRevision> upstreamRevisionList = upstreamRevisionsItem1?.StaleRevisionList ?? new List<UpstreamRevision>();
        upstreamRevisionList.Add(upstreamRevision);
        UpstreamRevisionsItem upstreamRevisionsItem2 = new UpstreamRevisionsItem()
        {
          CurrentRevisionTable = dictionary,
          StaleRevisionList = upstreamRevisionList,
          StorageETag = upstreamRevisionsItem1?.StorageETag
        };
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        if (!await cDisplayClass100.itemStore.CompareSwapItemAsync(cDisplayClass100.requestContext, cDisplayClass100.feedContainer, cDisplayClass100.tokenLocator, (StoredItem) upstreamRevisionsItem2))
          throw new TargetModifiedAfterReadException(nameof (CreateNewUpstreamPackagesRevision));
      }))));
      return revision;
    }

    protected abstract IItemStore GetItemStore(IVssRequestContext requestContext);

    protected Locator ComputeContainerName(FeedCore feed) => PackagingUtils.ComputeFeedContainerName(feed);

    private Locator GetUpstreamRevisionLocator() => new Locator(new string[2]
    {
      "upstreamRevisions",
      this.Protocol
    });

    private bool CompareUpstreamRevisions(UpstreamRevision a, UpstreamRevision b) => a.Source == b.Source && (int) a.Revision == (int) b.Revision;
  }
}
