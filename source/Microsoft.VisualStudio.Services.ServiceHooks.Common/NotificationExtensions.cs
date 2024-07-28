// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Common.NotificationExtensions
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E36C8A02-D97F-45E0-9F96-E7385D8CA092
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ServiceHooks.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ExternalEvent;
using System;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Common
{
  public static class NotificationExtensions
  {
    private static readonly string s_layer = typeof (NotificationExtensions).Name;
    private static readonly string s_area = typeof (NotificationExtensions).Namespace;

    public static string GetConsumerInput(
      this Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Notification notification,
      string inputId,
      bool required = false)
    {
      string consumerInput = (string) null;
      if (((!notification.Details.ConsumerInputs.TryGetValue(inputId, out consumerInput) ? 1 : (string.IsNullOrEmpty(consumerInput) ? 1 : 0)) & (required ? 1 : 0)) != 0)
        throw new ArgumentException(inputId);
      return consumerInput;
    }

    public static void PublishAsNotification(
      this IMessage message,
      IVssRequestContext requestContext)
    {
      requestContext.TraceEnter(1064100, NotificationExtensions.s_area, NotificationExtensions.s_layer, nameof (PublishAsNotification));
      if (message.ContentType == typeof (ExternalGitPullRequest).FullName)
      {
        ExternalGitPullRequest body = message.GetBody<ExternalGitPullRequest>();
        if (body != null)
        {
          requestContext.Trace(1064200, TraceLevel.Info, NotificationExtensions.s_area, NotificationExtensions.s_layer, "Publishing pull request notification");
          requestContext.GetService<ITeamFoundationEventService>().PublishNotification(requestContext, (object) body);
          requestContext.Trace(1064200, TraceLevel.Info, NotificationExtensions.s_area, NotificationExtensions.s_layer, "Published pull request notification");
        }
      }
      else if (message.ContentType == typeof (ExternalBuildCompletionEvent).FullName)
      {
        ExternalBuildCompletionEvent body = message.GetBody<ExternalBuildCompletionEvent>();
        if (body != null)
        {
          requestContext.Trace(1064200, TraceLevel.Info, NotificationExtensions.s_area, NotificationExtensions.s_layer, "Publishing ExternalBuildCompletionEvent notification");
          requestContext.GetService<ITeamFoundationEventService>().PublishNotification(requestContext, (object) body);
          requestContext.Trace(1064200, TraceLevel.Info, NotificationExtensions.s_area, NotificationExtensions.s_layer, "Published ExternalBuildCompletionEvent notification");
        }
      }
      else if (message.ContentType == typeof (ExternalGitPush).FullName)
      {
        ExternalGitPush body = message.GetBody<ExternalGitPush>();
        if (body != null)
        {
          requestContext.Trace(1064200, TraceLevel.Info, NotificationExtensions.s_area, NotificationExtensions.s_layer, "Publishing git push notification");
          requestContext.GetService<ITeamFoundationEventService>().PublishNotification(requestContext, (object) body);
          requestContext.Trace(1064200, TraceLevel.Info, NotificationExtensions.s_area, NotificationExtensions.s_layer, "Published git push notification");
        }
      }
      else
        requestContext.Trace(0, TraceLevel.Info, NotificationExtensions.s_area, NotificationExtensions.s_layer, "Received unhandled event {0}", (object) message.ContentType);
      requestContext.TraceLeave(1064300, NotificationExtensions.s_area, NotificationExtensions.s_layer, nameof (PublishAsNotification));
    }
  }
}
