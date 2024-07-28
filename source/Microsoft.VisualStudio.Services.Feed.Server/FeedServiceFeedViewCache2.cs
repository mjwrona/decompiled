// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.FeedServiceFeedViewCache2
// Assembly: Microsoft.VisualStudio.Services.Feed.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55555083-3B79-4F4F-AA85-92D66019974E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Feed.Common;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;


#nullable enable
namespace Microsoft.VisualStudio.Services.Feed.Server
{
  internal class FeedServiceFeedViewCache2 : 
    VssVersionedCacheService<FeedViewCache2Data>,
    IFeedServiceFeedViewCache2,
    IVssFrameworkService
  {
    private const int DefaultFetchAllViewsThreshold = 3;
    public static readonly RegistryQuery FetchAllViewsThresholdRegistryPath = (RegistryQuery) "/Configuration/Feeds/ViewCache/FetchAllViewsThreshold";
    public static readonly Guid FeedViewChangedNotificationId = new Guid("88A497B2-C285-4685-AA39-B2CF5A0962CA");

    public IEnumerable<FeedView> GetFeedViews(IVssRequestContext requestContext, Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed) => this.CloneFeedViews((IEnumerable<FeedView>) (this.ReadViewsForFeedFromCacheOrDefault(requestContext, feed) ?? this.LoadViewsForFeedFromDbIntoCache(requestContext, feed)));

    public IReadOnlyDictionary<Guid, IReadOnlyList<FeedView>> GetFeedViews(
      IVssRequestContext requestContext,
      IReadOnlyCollection<Microsoft.VisualStudio.Services.Feed.WebApi.Feed> feeds)
    {
      requestContext.CheckSystemRequestContext();
      List<(Guid, IReadOnlyList<FeedView>)> source1 = new List<(Guid, IReadOnlyList<FeedView>)>(feeds.Count);
      List<Microsoft.VisualStudio.Services.Feed.WebApi.Feed> source2 = new List<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>(feeds.Count);
      Dictionary<Guid, IReadOnlyList<FeedView>> dictionary = this.Read<Dictionary<Guid, IReadOnlyList<FeedView>>>(requestContext, (Func<FeedViewCache2Data, Dictionary<Guid, IReadOnlyList<FeedView>>>) (cache => cache.ViewsByFeed));
      foreach (Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed in (IEnumerable<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>) feeds)
      {
        IReadOnlyList<FeedView> feedViewList;
        if (dictionary.TryGetValue(feed.Id, out feedViewList))
          source1.Add((feed.Id, feedViewList));
        else
          source2.Add(feed);
      }
      int allViewsThreshold = this.GetFetchAllViewsThreshold(requestContext);
      if (source2.Count < allViewsThreshold)
      {
        source1.AddRange(source2.Select<Microsoft.VisualStudio.Services.Feed.WebApi.Feed, (Guid, IReadOnlyList<FeedView>)>((Func<Microsoft.VisualStudio.Services.Feed.WebApi.Feed, (Guid, IReadOnlyList<FeedView>)>) (feed => (feed.Id, this.LoadViewsForFeedFromDbIntoCache(requestContext, feed)))));
        return (IReadOnlyDictionary<Guid, IReadOnlyList<FeedView>>) source1.ToDictionary<(Guid, IReadOnlyList<FeedView>), Guid, IReadOnlyList<FeedView>>((Func<(Guid, IReadOnlyList<FeedView>), Guid>) (x => x.FeedId), (Func<(Guid, IReadOnlyList<FeedView>), IReadOnlyList<FeedView>>) (x => (IReadOnlyList<FeedView>) this.CloneFeedViews((IEnumerable<FeedView>) x.Views).ToList<FeedView>()));
      }
      IReadOnlyDictionary<Guid, IReadOnlyList<FeedView>> allViews = this.GetAllNonDeletedViewsByFeedForCollection(requestContext, (IEnumerable<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>) feeds);
      return (IReadOnlyDictionary<Guid, IReadOnlyList<FeedView>>) feeds.ToDictionary<Microsoft.VisualStudio.Services.Feed.WebApi.Feed, Guid, IReadOnlyList<FeedView>>((Func<Microsoft.VisualStudio.Services.Feed.WebApi.Feed, Guid>) (x => x.Id), (Func<Microsoft.VisualStudio.Services.Feed.WebApi.Feed, IReadOnlyList<FeedView>>) (x => allViews[x.Id]));
    }

    public FeedView? GetFeedView(IVssRequestContext requestContext, Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed, Guid viewId) => FeedServiceFeedViewCache2.CloneFeedView(this.GetFeedViewInternal(requestContext, feed, (Func<FeedView, bool>) (view => view.Id == viewId)));

    public FeedView? GetFeedView(IVssRequestContext requestContext, Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed, string viewName) => FeedServiceFeedViewCache2.CloneFeedView(this.GetFeedViewInternal(requestContext, feed, (Func<FeedView, bool>) (view => StringComparer.InvariantCultureIgnoreCase.Equals(view.Name, viewName))));

    public void InvalidateFeedViews(IVssRequestContext requestContext, Guid feedId) => this.Invalidate<int>(requestContext, (Func<FeedViewCache2Data, int>) (feedViewCacheData =>
    {
      feedViewCacheData.ViewsByFeed.Remove(feedId);
      return 0;
    }));

    public IReadOnlyDictionary<Guid, IReadOnlyList<FeedView>> GetAllNonDeletedViewsByFeedForCollection(
      IVssRequestContext requestContext,
      IEnumerable<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>? knownExistingFeeds = null)
    {
      if (knownExistingFeeds == null)
        knownExistingFeeds = Enumerable.Empty<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>();
      requestContext.CheckSystemRequestContext();
      return (IReadOnlyDictionary<Guid, IReadOnlyList<FeedView>>) this.Synchronize<Dictionary<Guid, IReadOnlyList<FeedView>>>(requestContext, (Func<Dictionary<Guid, IReadOnlyList<FeedView>>>) (() =>
      {
        Dictionary<Guid, IReadOnlyList<FeedView>> collectionFromDb = FeedServiceFeedViewCache2.GetAllNonDeletedViewsByFeedForCollectionFromDb(requestContext);
        foreach (Microsoft.VisualStudio.Services.Feed.WebApi.Feed knownExistingFeed in knownExistingFeeds)
        {
          if (!collectionFromDb.ContainsKey(knownExistingFeed.Id))
            collectionFromDb.Add(knownExistingFeed.Id, (IReadOnlyList<FeedView>) Array.Empty<FeedView>());
        }
        return collectionFromDb;
      }), (Action<FeedViewCache2Data, Dictionary<Guid, IReadOnlyList<FeedView>>>) ((cache, newData) =>
      {
        cache.ViewsByFeed.Clear();
        cache.ViewsByFeed.AddRange<KeyValuePair<Guid, IReadOnlyList<FeedView>>, Dictionary<Guid, IReadOnlyList<FeedView>>>((IEnumerable<KeyValuePair<Guid, IReadOnlyList<FeedView>>>) newData);
      })).ToDictionary<KeyValuePair<Guid, IReadOnlyList<FeedView>>, Guid, IReadOnlyList<FeedView>>((Func<KeyValuePair<Guid, IReadOnlyList<FeedView>>, Guid>) (x => x.Key), (Func<KeyValuePair<Guid, IReadOnlyList<FeedView>>, IReadOnlyList<FeedView>>) (x => (IReadOnlyList<FeedView>) this.CloneFeedViews((IEnumerable<FeedView>) x.Value).ToList<FeedView>()));
    }

    protected override FeedViewCache2Data InitializeCache(IVssRequestContext requestContext) => !this.ShouldPrePopulateCache(requestContext) ? new FeedViewCache2Data(new Dictionary<Guid, IReadOnlyList<FeedView>>()) : new FeedViewCache2Data(FeedServiceFeedViewCache2.GetAllNonDeletedViewsByFeedForCollectionFromDb(requestContext.Elevate()));

    protected override void ServiceStart(IVssRequestContext systemRequestContext)
    {
      if (!systemRequestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
        throw new UnexpectedHostTypeException(systemRequestContext.ServiceHost.HostType);
      base.ServiceStart(systemRequestContext);
      systemRequestContext.GetService<ITeamFoundationSqlNotificationService>().RegisterNotification(systemRequestContext, "Default", FeedServiceFeedViewCache2.FeedViewChangedNotificationId, new SqlNotificationHandler(this.OnViewChanged), false);
    }

    protected override void ServiceEnd(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.GetService<ITeamFoundationSqlNotificationService>().UnregisterNotification(systemRequestContext, "Default", FeedServiceFeedViewCache2.FeedViewChangedNotificationId, new SqlNotificationHandler(this.OnViewChanged), false);
      base.ServiceEnd(systemRequestContext);
    }

    private IReadOnlyList<FeedView>? ReadViewsForFeedFromCacheOrDefault(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed)
    {
      return this.Read<IReadOnlyList<FeedView>>(requestContext, (Func<FeedViewCache2Data, IReadOnlyList<FeedView>>) (feedViewCacheData => feedViewCacheData.ViewsByFeed.GetValueOrDefault<Guid, IReadOnlyList<FeedView>>(feed.Id, (IReadOnlyList<FeedView>) null)));
    }

    private FeedView? GetFeedViewInternal(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed,
      Func<FeedView, bool> predicate)
    {
      IReadOnlyList<FeedView> source = this.ReadViewsForFeedFromCacheOrDefault(requestContext, feed);
      return (source != null ? source.FirstOrDefault<FeedView>(predicate) : (FeedView) null) ?? this.LoadViewsForFeedFromDbIntoCache(requestContext, feed).FirstOrDefault<FeedView>(predicate);
    }

    private IReadOnlyList<FeedView> LoadViewsForFeedFromDbIntoCache(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed)
    {
      IReadOnlyList<FeedView> viewsFromDatabase = FeedServiceFeedViewCache2.GetViewsForFeedFromDatabase(requestContext, feed);
      return this.Synchronize<IReadOnlyList<FeedView>>(requestContext, (Func<IReadOnlyList<FeedView>>) (() => viewsFromDatabase), (Action<FeedViewCache2Data, IReadOnlyList<FeedView>>) ((cache, viewsFromDb) => cache.ViewsByFeed[feed.Id] = viewsFromDb));
    }

    private static Dictionary<Guid, IReadOnlyList<FeedView>> GetAllNonDeletedViewsByFeedForCollectionFromDb(
      IVssRequestContext requestContext)
    {
      requestContext.CheckSystemRequestContext();
      try
      {
        using (FeedViewSqlResourceComponent component = requestContext.CreateComponent<FeedViewSqlResourceComponent>())
          return component.GetAllNonDeletedFeedViewsInCollection().GroupBy<(Guid, FeedView), Guid, FeedView>((Func<(Guid, FeedView), Guid>) (x => x.FeedId), (Func<(Guid, FeedView), FeedView>) (x => x.View)).ToDictionary<IGrouping<Guid, FeedView>, Guid, IReadOnlyList<FeedView>>((Func<IGrouping<Guid, FeedView>, Guid>) (x => x.Key), (Func<IGrouping<Guid, FeedView>, IReadOnlyList<FeedView>>) (x => (IReadOnlyList<FeedView>) x.ToList<FeedView>()));
      }
      catch (NotSupportedException ex)
      {
        return IterateFeedsToCollectViews();
      }

      Dictionary<Guid, IReadOnlyList<FeedView>> IterateFeedsToCollectViews()
      {
        IEnumerable<Microsoft.VisualStudio.Services.Feed.WebApi.Feed> feeds;
        using (FeedSqlResourceComponent component = requestContext.CreateComponent<FeedSqlResourceComponent>())
          feeds = component.GetFeeds(new Guid?());
        Dictionary<Guid, IReadOnlyList<FeedView>> collectViews = new Dictionary<Guid, IReadOnlyList<FeedView>>();
        foreach (Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed in feeds)
          collectViews[feed.Id] = FeedServiceFeedViewCache2.GetViewsForFeedFromDatabase(requestContext, feed);
        return collectViews;
      }
    }

    private void OnViewChanged(IVssRequestContext systemRequestContext, NotificationEventArgs args)
    {
      FeedViewChangedNotificationData notificationData = args.Deserialize<FeedViewChangedNotificationData>();
      this.InvalidateFeedViews(systemRequestContext, notificationData.FeedId);
    }

    private static FeedView CloneFeedView(FeedView? feedView) => FeedViewSqlResourceComponent.DeepCloneFeedView(feedView);

    private IEnumerable<FeedView> CloneFeedViews(IEnumerable<FeedView> feedViewList) => feedViewList.Select<FeedView, FeedView>((Func<FeedView, FeedView>) (feed => FeedServiceFeedViewCache2.CloneFeedView(feed)));

    private static IReadOnlyList<FeedView> GetViewsForFeedFromDatabase(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed)
    {
      using (FeedViewSqlResourceComponent component = requestContext.CreateComponent<FeedViewSqlResourceComponent>())
        return (IReadOnlyList<FeedView>) component.GetFeedViews(feed.GetIdentity()).ToList<FeedView>();
    }

    private int GetFetchAllViewsThreshold(IVssRequestContext requestContext) => requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, in FeedServiceFeedViewCache2.FetchAllViewsThresholdRegistryPath, 3);

    private bool ShouldPrePopulateCache(IVssRequestContext requestContext) => requestContext.IsFeatureEnabled("Packaging.Feed.PrePopulateFeedViewCache");
  }
}
