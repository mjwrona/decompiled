// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.FeedMessageBus
// Assembly: Microsoft.VisualStudio.Services.Feed.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55555083-3B79-4F4F-AA85-92D66019974E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.Common.Notification;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using System;

namespace Microsoft.VisualStudio.Services.Feed.Server
{
  public class FeedMessageBus : IFeedMessageBus
  {
    public void SendCachedFeedNoLongerValidNotification(
      IVssRequestContext requestContext,
      Guid feedId)
    {
      requestContext.TraceEnter(10019054, "Feed", "MessageBus", nameof (SendCachedFeedNoLongerValidNotification));
      FeedCore feedCore = new FeedCore() { Id = feedId };
      try
      {
        requestContext.GetService<IMessageBusPublisherService>().Publish(requestContext.Elevate(), "Microsoft.VisualStudio.Services.Feed", new object[1]
        {
          (object) feedCore
        });
      }
      catch (Exception ex)
      {
        requestContext.TraceException(10019056, "Feed", "MessageBus", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(10019055, "Feed", "MessageBus", nameof (SendCachedFeedNoLongerValidNotification));
      }
    }

    public void SendPackagePublishNotification(
      IVssRequestContext requestContext,
      PackageChangedEvent data)
    {
      requestContext.TraceEnter(10019081, "Feed", "MessageBus", nameof (SendPackagePublishNotification));
      try
      {
        if (!requestContext.ExecutionEnvironment.IsHostedDeployment)
          return;
        requestContext.GetService<IMessageBusPublisherService>().Publish(requestContext, "Microsoft.VisualStudio.Services.Feed", new object[1]
        {
          (object) data
        });
      }
      catch (Exception ex)
      {
        requestContext.TraceException(10019083, "Feed", "MessageBus", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(10019082, "Feed", "MessageBus", nameof (SendPackagePublishNotification));
      }
    }

    public void PublishFeedChanged(IVssRequestContext requestContext, FeedChangedEvent data)
    {
      requestContext.TraceEnter(10019118, "Feed", "MessageBus", nameof (PublishFeedChanged));
      try
      {
        requestContext.GetService<IMessageBusPublisherService>().Publish(requestContext, "Microsoft.VisualStudio.Services.Feed", new object[1]
        {
          (object) data
        });
      }
      catch (Exception ex)
      {
        requestContext.TraceException(10019120, "Feed", "MessageBus", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(10019119, "Feed", "MessageBus", nameof (PublishFeedChanged));
      }
    }
  }
}
