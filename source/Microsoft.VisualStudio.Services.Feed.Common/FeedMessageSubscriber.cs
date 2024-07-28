// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Common.FeedMessageSubscriber
// Assembly: Microsoft.VisualStudio.Services.Feed.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AAC6BCA4-7F6C-4DFE-8058-1CCDD886477F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Newtonsoft.Json;
using System;

namespace Microsoft.VisualStudio.Services.Feed.Common
{
  public class FeedMessageSubscriber : IMessageBusSubscriberJobExtensionReceiver
  {
    TeamFoundationHostType IMessageBusSubscriberJobExtensionReceiver.AcceptedHostTypes => TeamFoundationHostType.ProjectCollection;

    public void Receive(IVssRequestContext requestContext, IMessage message)
    {
      requestContext.TraceEnter(10019060, "Feed", "CacheService", nameof (Receive));
      try
      {
        if (message.ContentType.Equals(typeof (FeedCore).FullName))
        {
          FeedCore body = message.GetBody<FeedCore>();
          if (body != null)
            this.HandleCachedFeedNoLongerValidMessage(requestContext, body);
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(10019061, "Feed", "CacheService", ex);
      }
      requestContext.TraceLeave(10019062, "Feed", "CacheService", nameof (Receive));
    }

    private void HandleCachedFeedNoLongerValidMessage(
      IVssRequestContext requestContext,
      FeedCore feed)
    {
      ITeamFoundationSqlNotificationService service = requestContext.GetService<ITeamFoundationSqlNotificationService>();
      string str = JsonConvert.SerializeObject((object) new FeedSqlNotificationMessage()
      {
        Feed = feed
      });
      IVssRequestContext requestContext1 = requestContext;
      Guid feedNoLongerValid = FeedSqlNotificationEventClasses.CachedFeedNoLongerValid;
      string eventData = str;
      service.SendNotification(requestContext1, feedNoLongerValid, eventData);
    }
  }
}
