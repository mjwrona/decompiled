// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Sdk.Server.HooksPublisherTestNotificationsController
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3575E571-FF3A-4E7B-A8CC-64FFB01E8C91
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ServiceHooks.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ServiceHooks.Common;
using Microsoft.VisualStudio.Services.ServiceHooks.Consumers;
using Microsoft.VisualStudio.Services.ServiceHooks.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Sdk.Server
{
  [VersionedApiControllerCustomName(Area = "hooks", ResourceName = "TestNotifications")]
  [ClientGroupByResource("notifications")]
  public class HooksPublisherTestNotificationsController : ServiceHooksPublisherControllerBase
  {
    [HttpPost]
    public Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Notification CreateTestNotification(
      Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Notification testNotification,
      [FromUri] bool useRealData = false)
    {
      ArgumentUtility.CheckForNull<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Notification>(testNotification, nameof (testNotification));
      ArgumentUtility.CheckForNull<NotificationDetails>(testNotification.Details, "testNotification.Details");
      ArgumentUtility.CheckForNull<string>(testNotification.Details.EventType, "testNotification.Details.EventType");
      ArgumentUtility.CheckForNull<string>(testNotification.Details.ConsumerId, "testNotification.Details.ConsumerId");
      ArgumentUtility.CheckForNull<string>(testNotification.Details.ConsumerActionId, "testNotification.Details.ConsumerActionId");
      this.CheckScope(testNotification.Details.EventType);
      DateTime now = DateTime.Now;
      ServiceHooksPublisher publisher = this.FindPublisher(testNotification.Details.PublisherId);
      string resourceVersion = HooksPublisherTestNotificationsController.NegotiateResourceVersion(this.TfsRequestContext, testNotification.Details.EventType, testNotification.Details.ConsumerId, testNotification.Details.ConsumerActionId, testNotification.Details.Event?.ResourceVersion);
      if (useRealData)
      {
        ArgumentUtility.CheckForEmptyGuid(testNotification.SubscriptionId, "testNotification.SubscriptionId");
        testNotification.Details.Event = publisher.GetRealSampleEvent(this.TfsRequestContext, testNotification, resourceVersion);
      }
      else
        testNotification.Details.Event = publisher.GetSampleEvent(this.TfsRequestContext, testNotification.Details.PublisherInputs, testNotification.Details.EventType, resourceVersion);
      return publisher.SendTestNotification(this.TfsRequestContext, testNotification);
    }

    private static string NegotiateResourceVersion(
      IVssRequestContext requestContext,
      string eventType,
      string consumerId,
      string consumerActionId,
      string resourceVersion)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      if (!string.IsNullOrWhiteSpace(resourceVersion) && !string.Equals(resourceVersion, "latest"))
        return resourceVersion;
      ConsumerActionImplementation actionImplementation = HooksPublisherTestNotificationsController.GetConsumerService(requestContext).GetConsumer(requestContext, consumerId).Actions.FirstOrDefault<ConsumerActionImplementation>((Func<ConsumerActionImplementation, bool>) (action => action.Id == consumerActionId));
      if (actionImplementation == null)
        throw new ConsumerActionNotFoundException(consumerId, consumerActionId);
      string[] versions = Array.Empty<string>();
      return actionImplementation.SupportedResourceVersions == null || !actionImplementation.SupportedResourceVersions.TryGetValue(eventType, out versions) ? "latest" : ResourceVersionComparer.Sort((IEnumerable<string>) versions).FirstOrDefault<string>();
    }

    private static ServiceHooksConsumerService GetConsumerService(IVssRequestContext requestContext) => requestContext.To(TeamFoundationHostType.Deployment).GetService<ServiceHooksConsumerService>();
  }
}
