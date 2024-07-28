// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.EntitlementChangeNotifier
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Licensing
{
  public static class EntitlementChangeNotifier
  {
    private const string EntitlementChangeTopicName = "Microsoft.VisualStudio.Services.Licensing.EntitlementChange";
    private const string s_Area = "Licensing";
    private const string s_Layer = "EntitlementChangeNotifier";

    public static void Publish(
      IVssRequestContext requestContext,
      EntitlementChangeMessage message,
      EntitlementChangePublisherType publisherType)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<EntitlementChangeMessage>(message, nameof (message));
      ArgumentUtility.CheckForEmptyGuid(message.AccountId, "accountId");
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) message.UserIds, "userIds");
      requestContext.TraceEnter(1039100, "Licensing", nameof (EntitlementChangeNotifier), nameof (Publish));
      try
      {
        requestContext.TraceConditionally(1039101, TraceLevel.Verbose, "Licensing", nameof (EntitlementChangeNotifier), (Func<string>) (() => string.Format("publishing entitlement change message to providerType: {0} for messageType: {1}, account: {2}, user ids: {3}", (object) publisherType, (object) message.EntitlementChangeType, (object) message.AccountId, (object) string.Join<Guid>(",", (IEnumerable<Guid>) message.UserIds))));
        switch (publisherType)
        {
          case EntitlementChangePublisherType.SqlNotification:
            EntitlementChangeNotifier.PublishSqlNotification(requestContext, message);
            break;
          case EntitlementChangePublisherType.ServiceBus:
            EntitlementChangeNotifier.PublishServiceBusMessage(requestContext, message);
            break;
          case EntitlementChangePublisherType.SqlNotificationAndServiceBus:
            EntitlementChangeNotifier.PublishSqlNotification(requestContext, message);
            EntitlementChangeNotifier.PublishServiceBusMessage(requestContext, message);
            break;
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1039108, "Licensing", nameof (EntitlementChangeNotifier), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(1039109, "Licensing", nameof (EntitlementChangeNotifier), nameof (Publish));
      }
    }

    private static void PublishServiceBusMessage(
      IVssRequestContext requestContext,
      EntitlementChangeMessage message)
    {
      if (requestContext.IsFeatureEnabled("VisualStudio.Services.Licensing.DisablePublishServiceBusMessages"))
        return;
      IVssRequestContext vssRequestContext = requestContext.Elevate();
      vssRequestContext.GetService<IMessageBusPublisherService>().TryPublish(vssRequestContext, "Microsoft.VisualStudio.Services.Licensing.EntitlementChange", new object[1]
      {
        (object) message
      }, allowLoopback: false);
    }

    private static void PublishSqlNotification(
      IVssRequestContext requestContext,
      EntitlementChangeMessage message)
    {
      requestContext.GetService<ITeamFoundationSqlNotificationService>().SendNotification(requestContext, SqlNotificationEventClasses.UserEntitlementChanged, TeamFoundationSerializationUtility.SerializeToString<EntitlementChangeMessage>(message));
    }
  }
}
