// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.Utils.SQLNotificationSenders
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.Common.Utils
{
  public static class SQLNotificationSenders
  {
    public static void SendSqlNotification(
      IVssRequestContext requestContext,
      object notificationData,
      Guid sqlNotificationEvent,
      string featureName,
      IEnumerable<Type> knownTypes = null)
    {
      if (!requestContext.IsFeatureEnabled(featureName))
        return;
      SQLNotificationSenders.SendSqlNotification(requestContext, notificationData, sqlNotificationEvent, knownTypes);
    }

    public static void SendSqlNotification(
      IVssRequestContext requestContext,
      object notificationData,
      Guid sqlNotificationEvent,
      IEnumerable<Type> knownTypes = null)
    {
      string eventData = (string) null;
      if (notificationData != null)
        eventData = Serializers.ToXmlString(notificationData, notificationData.GetType(), knownTypes);
      requestContext.GetService<ITeamFoundationSqlNotificationService>().SendNotification(requestContext, sqlNotificationEvent, eventData);
    }
  }
}
