// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades.FeedItemStoreContainerProvider`1
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Feed.Common;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.ItemStore.Server.Common;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades
{
  public class FeedItemStoreContainerProvider<TItemStore> : IFeedItemStoreContainerProvider where TItemStore : class, IItemStore
  {
    private readonly IVssRequestContext requestContext;
    private readonly HashSet<FeedIdentity> knownExistingContainers = new HashSet<FeedIdentity>();

    public FeedItemStoreContainerProvider(IVssRequestContext requestContext) => this.requestContext = requestContext;

    public Locator GetLocator(FeedCore feed) => PackagingUtils.ComputeFeedContainerName(feed);

    public async Task EnsureContainerExistsAsync(FeedCore feed)
    {
      if (this.IsContainerKnownToExist(feed))
        return;
      await PackagingUtils.CreateContainerIfNotExistsAsync<TItemStore>(this.requestContext, feed);
      this.MarkContainerKnownToExist(feed);
    }

    public void MarkContainerKnownToExist(FeedCore feed) => this.knownExistingContainers.Add(feed.GetIdentity());

    private bool IsContainerKnownToExist(FeedCore feed) => this.knownExistingContainers.Contains(feed.GetIdentity());
  }
}
