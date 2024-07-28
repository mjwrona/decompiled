// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Consumers.WorkplaceMessagingApps.WorkplaceMessagingAppsSendNotificationAction
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Consumers, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5BEF54E7-7304-4071-B5F1-22428BB21801
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ServiceHooks.Consumers.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.FormInput;
using Microsoft.VisualStudio.Services.ServiceHooks.Common;
using Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Common;
using Microsoft.VisualStudio.Services.ServiceHooks.Consumers.WebHooks;
using Microsoft.VisualStudio.Services.ServiceHooks.WebApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Consumers.WorkplaceMessagingApps
{
  [Export(typeof (ConsumerActionImplementation))]
  public class WorkplaceMessagingAppsSendNotificationAction : HttpRequestAction
  {
    private const string ConsumerActionId = "httpRequest";
    private static readonly string[] s_supportedEventTypes = new string[1]
    {
      "*"
    };
    private static readonly IDictionary<string, string[]> s_supportedResourceVersions = (IDictionary<string, string[]>) new Dictionary<string, string[]>();

    protected static InputDescriptor WorkplaceMessagingAppsUrlInputDesc() => new InputDescriptor()
    {
      Name = WorkplaceMessagingAppsConsumerResources.HttpRequestAction_InputUrlName,
      Description = WorkplaceMessagingAppsConsumerResources.HttpRequestAction_InputUrlDescription,
      InputMode = InputMode.TextBox,
      Id = "url",
      IsConfidential = false,
      Validation = new InputValidation()
      {
        DataType = InputDataType.Uri,
        IsRequired = true
      }
    };

    public override string Id => "httpRequest";

    public override string ConsumerId => "workplaceMessagingApps";

    public override string Name => WorkplaceMessagingAppsConsumerResources.HttpRequestActionName;

    public override string Description => WorkplaceMessagingAppsConsumerResources.HttpRequestActionDescription;

    public override IList<InputDescriptor> InputDescriptors => (IList<InputDescriptor>) new List<InputDescriptor>()
    {
      WorkplaceMessagingAppsSendNotificationAction.WorkplaceMessagingAppsUrlInputDesc(),
      HttpRequestAction.BuildBasicAuthUsernameInputDescriptor(),
      HttpRequestAction.BuildBasicAuthPasswordInputDescriptor(),
      HttpRequestAction.BuildHeadersInputDescriptor()
    }.ToList<InputDescriptor>();

    public override string[] SupportedEventTypes => WorkplaceMessagingAppsSendNotificationAction.s_supportedEventTypes;

    public override IDictionary<string, string[]> SupportedResourceVersions => WorkplaceMessagingAppsSendNotificationAction.s_supportedResourceVersions;

    public override ActionTask HandleEvent(
      IVssRequestContext requestContext,
      Event raisedEvent,
      HandleEventArgs e)
    {
      if (!requestContext.IsFeatureEnabled("ServiceHooks.Notification.AzChatOpsActionTask"))
        return base.HandleEvent(requestContext, raisedEvent, e);
      string consumerInput = e.Notification.GetConsumerInput("url", true);
      WebHookEvent webHookEvent = new WebHookEvent();
      webHookEvent.CloneEventProperties(raisedEvent);
      webHookEvent.NotificationId = e.Notification.Id;
      webHookEvent.SubscriptionId = e.Notification.SubscriptionId;
      JObject webHookEventJObject = EventTransformerConsumerActionImplementation.TransformEvent(e.Notification, (Event) webHookEvent, this.GetDefaultResourceDetailsToSend(e.Notification), this.GetDefaultMessagesToSend(e.Notification), this.GetDefaultDetailedMessagesToSend(e.Notification));
      if (webHookEvent.SessionToken != null && !string.IsNullOrWhiteSpace(webHookEvent.SessionToken.Token))
        webHookEventJObject["sessionToken"][(object) "token"] = (JToken) SecurityHelper.GetMaskedValue(webHookEvent.SessionToken.Token);
      if (e.Notification.Details is NotificationDetailsInternal details)
      {
        IDictionary<string, string> notificationData = details.NotificationData;
        int? nullable = notificationData != null ? new int?(notificationData.Count<KeyValuePair<string, string>>()) : new int?();
        int num = 0;
        if (nullable.GetValueOrDefault() > num & nullable.HasValue)
          webHookEventJObject["resource"][(object) "notificationData"] = JToken.Parse(JsonConvert.SerializeObject((object) details.NotificationData));
      }
      return (ActionTask) new AzChatOpsActionTask(webHookEventJObject, consumerInput);
    }

    protected override EventMessages GetDefaultDetailedMessagesToSend(Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Notification notification) => EventMessages.All;

    protected override EventMessages GetDefaultMessagesToSend(Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Notification notification) => EventMessages.All;

    protected override EventResourceDetails GetDefaultResourceDetailsToSend(
      Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Notification notification)
    {
      return EventResourceDetails.All;
    }
  }
}
