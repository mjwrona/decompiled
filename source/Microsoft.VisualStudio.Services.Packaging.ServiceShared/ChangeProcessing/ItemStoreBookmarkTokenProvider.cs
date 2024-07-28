// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.ChangeProcessing.ItemStoreBookmarkTokenProvider
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using Microsoft.VisualStudio.Services.ItemStore.Common;
using Microsoft.VisualStudio.Services.ItemStore.Server.Common;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.ItemStore;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using System;
using System.Threading.Tasks;


#nullable enable
namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.ChangeProcessing
{
  public class ItemStoreBookmarkTokenProvider : ILegacyBookmarkTokenProvider
  {
    private const int MaxRetries = 10;
    private static readonly TraceData TraceData = Microsoft.VisualStudio.Services.Packaging.ServiceShared.Constants.TracePoints.ItemStoreBookmarkTokenProvider.TraceData;

    public string? GetToken(
      IVssRequestContext requestContext,
      Guid containerId,
      BookmarkTokenKey bookmarkTokenKey)
    {
      using (Microsoft.VisualStudio.Services.Content.Server.Common.Tracer.Enter(requestContext, ItemStoreBookmarkTokenProvider.TraceData, 5724200, nameof (GetToken)))
      {
        IItemStore itemStore = this.GetItemStore(requestContext);
        Locator feedContainer = this.ComputeContainerName(bookmarkTokenKey, containerId);
        Locator tokenLocator = this.GetTokenLocator(bookmarkTokenKey);
        return AsyncPump.Run<TokenItem>((Func<Task<TokenItem>>) (() => itemStore.GetItemAsync<TokenItem>(requestContext, feedContainer, tokenLocator)))?.Token ?? (string) null;
      }
    }

    public void StoreToken(
      IVssRequestContext requestContext,
      Guid containerId,
      string token,
      BookmarkTokenKey bookmarkTokenKey)
    {
      using (Microsoft.VisualStudio.Services.Content.Server.Common.Tracer.Enter(requestContext, ItemStoreBookmarkTokenProvider.TraceData, 5724210, nameof (StoreToken)))
      {
        IItemStore itemStore = this.GetItemStore(requestContext);
        Locator feedContainer = this.ComputeContainerName(bookmarkTokenKey, containerId);
        Locator tokenLocator = this.GetTokenLocator(bookmarkTokenKey);
        TokenItem tokenItem = new TokenItem()
        {
          Token = token
        };
        TimeSpan defaultMaxRetryDelay = RetryUtils.GetDefaultMaxRetryDelay(requestContext);
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        RetryHelper retryHelper = new RetryHelper(requestContext, 10, defaultMaxRetryDelay, ItemStoreBookmarkTokenProvider.\u003C\u003EO.\u003C0\u003E__IsRetryableStorageException ?? (ItemStoreBookmarkTokenProvider.\u003C\u003EO.\u003C0\u003E__IsRetryableStorageException = new Func<Exception, bool>(RetryUtils.IsRetryableStorageException)));
        AsyncPump.Run<ContainerItem>((Func<Task<ContainerItem>>) (() => itemStore.GetOrAddContainerAsync(requestContext, new ContainerItem()
        {
          Name = feedContainer
        })));
        AsyncPump.Run((Func<Task>) (() => retryHelper.Invoke((Func<Task>) (async () =>
        {
          TokenItem itemAsync = await itemStore.GetItemAsync<TokenItem>(requestContext, feedContainer, tokenLocator);
          if (itemAsync != null)
            tokenItem.StorageETag = itemAsync.StorageETag;
          if (!await itemStore.CompareSwapItemAsync(requestContext, feedContainer, tokenLocator, (StoredItem) tokenItem))
            throw new TargetModifiedAfterReadException(nameof (StoreToken));
        }))));
      }
    }

    public string GetOrStoreToken(
      IVssRequestContext requestContext,
      Guid containerId,
      string token,
      BookmarkTokenKey bookmarkTokenKey)
    {
      using (Microsoft.VisualStudio.Services.Content.Server.Common.Tracer.Enter(requestContext, ItemStoreBookmarkTokenProvider.TraceData, 5724220, nameof (GetOrStoreToken)))
      {
        IItemStore itemStore = this.GetItemStore(requestContext);
        Locator feedContainer = this.ComputeContainerName(bookmarkTokenKey, containerId);
        Locator tokenLocator = this.GetTokenLocator(bookmarkTokenKey);
        TokenItem tokenItem = new TokenItem()
        {
          Token = token
        };
        TimeSpan defaultMaxRetryDelay = RetryUtils.GetDefaultMaxRetryDelay(requestContext);
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        RetryHelper retryHelper = new RetryHelper(requestContext, 10, defaultMaxRetryDelay, ItemStoreBookmarkTokenProvider.\u003C\u003EO.\u003C0\u003E__IsRetryableStorageException ?? (ItemStoreBookmarkTokenProvider.\u003C\u003EO.\u003C0\u003E__IsRetryableStorageException = new Func<Exception, bool>(RetryUtils.IsRetryableStorageException)));
        AsyncPump.Run<ContainerItem>((Func<Task<ContainerItem>>) (() => itemStore.GetOrAddContainerAsync(requestContext, new ContainerItem()
        {
          Name = feedContainer
        })));
        string result = (string) null;
        AsyncPump.Run((Func<Task>) (() => retryHelper.Invoke((Func<Task>) (async () =>
        {
          if (await itemStore.CompareSwapItemAsync(requestContext, feedContainer, tokenLocator, (StoredItem) tokenItem))
            result = tokenItem.Token;
          else
            result = (await itemStore.GetItemAsync<TokenItem>(requestContext, feedContainer, tokenLocator))?.Token;
          if (string.IsNullOrEmpty(result))
            throw new ItemNotFoundException(tokenLocator.Value, containerId.ToString());
        }))));
        return result;
      }
    }

    private IItemStore GetItemStore(IVssRequestContext requestContext) => (IItemStore) requestContext.GetService<PackagingItemStore>();

    private Locator GetTokenLocator(BookmarkTokenKey bookmarkTokenKey) => new Locator(new string[1]
    {
      string.Format("{0}/{1}_V{2}", (object) bookmarkTokenKey.ProtocolName, (object) bookmarkTokenKey.TokenName, (object) bookmarkTokenKey.TokenVersion)
    });

    private Locator ComputeContainerName(BookmarkTokenKey bookmarkTokenKey, Guid containerId) => new Locator(new string[1]
    {
      string.Format("{0}/{1}", (object) bookmarkTokenKey.ContainerTypeName, (object) containerId)
    });
  }
}
