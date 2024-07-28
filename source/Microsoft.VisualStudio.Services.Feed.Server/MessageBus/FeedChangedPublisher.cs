// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.MessageBus.FeedChangedPublisher
// Assembly: Microsoft.VisualStudio.Services.Feed.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55555083-3B79-4F4F-AA85-92D66019974E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.Common.Notification;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Feed.Server.MessageBus
{
  internal static class FeedChangedPublisher
  {
    internal static void CreatedFeed(IVssRequestContext requestContext, Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed)
    {
      FeedChangedEvent data = new FeedChangedEvent()
      {
        ChangeType = FeedChangeType.CreatedFeed,
        Feed = feed
      };
      FeedChangedPublisher.Publish(requestContext, data);
    }

    internal static void UpdatedFeed(IVssRequestContext requestContext, Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed)
    {
      FeedChangedEvent data = new FeedChangedEvent()
      {
        ChangeType = FeedChangeType.UpdatedFeed,
        Feed = feed
      };
      FeedChangedPublisher.Publish(requestContext, data);
    }

    internal static void DeletedFeed(IVssRequestContext requestContext, Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed)
    {
      FeedChangedEvent data = new FeedChangedEvent()
      {
        ChangeType = FeedChangeType.DeletedFeed,
        Feed = feed
      };
      FeedChangedPublisher.Publish(requestContext, data);
    }

    internal static void FeedPermissionChanged(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed,
      List<FeedPermission> permissions)
    {
      FeedChangedEvent data = new FeedChangedEvent()
      {
        ChangeType = FeedChangeType.FeedPermissionChanged,
        Feed = feed
      };
      data.Feed.Permissions = (IEnumerable<FeedPermission>) permissions;
      FeedChangedPublisher.Publish(requestContext, data);
    }

    internal static void CreatedView(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed,
      FeedView viewToCreate)
    {
      FeedChangedEvent data = new FeedChangedEvent()
      {
        ChangeType = FeedChangeType.AddedView,
        Feed = feed,
        View = viewToCreate
      };
      FeedChangedPublisher.Publish(requestContext, data);
    }

    internal static void UpdatedView(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed,
      FeedView updatedView)
    {
      FeedChangedEvent data = new FeedChangedEvent()
      {
        ChangeType = FeedChangeType.UpdatedView,
        Feed = feed,
        View = updatedView
      };
      FeedChangedPublisher.Publish(requestContext, data);
    }

    internal static void DeletedView(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed,
      FeedView deletedView)
    {
      FeedChangedEvent data = new FeedChangedEvent()
      {
        ChangeType = FeedChangeType.DeletedView,
        Feed = feed,
        View = deletedView
      };
      FeedChangedPublisher.Publish(requestContext, data);
    }

    internal static void ViewPermissionChanged(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed,
      List<FeedPermission> permissions)
    {
      FeedChangedEvent data = new FeedChangedEvent()
      {
        ChangeType = FeedChangeType.ViewPermissionChanged,
        Feed = feed,
        View = feed.View
      };
      data.Feed.Permissions = (IEnumerable<FeedPermission>) permissions;
      FeedChangedPublisher.Publish(requestContext, data);
    }

    internal static void PermanentDeletedFeed(IVssRequestContext requestContext, Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed)
    {
      FeedChangedEvent data = new FeedChangedEvent()
      {
        ChangeType = FeedChangeType.PermanentDeletedFeed,
        Feed = feed
      };
      FeedChangedPublisher.Publish(requestContext, data);
    }

    internal static void RestoredDeletedFeed(IVssRequestContext requestContext, Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed)
    {
      FeedChangedEvent data = new FeedChangedEvent()
      {
        ChangeType = FeedChangeType.RestoredFeed,
        Feed = feed
      };
      FeedChangedPublisher.Publish(requestContext, data);
    }

    private static void Publish(IVssRequestContext requestContext, FeedChangedEvent data) => new FeedMessageBus().PublishFeedChanged(requestContext, data);
  }
}
