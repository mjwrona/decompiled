// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Common.EventTransformer
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Consumers, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5BEF54E7-7304-4071-B5F1-22428BB21801
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ServiceHooks.Consumers.dll

using Microsoft.VisualStudio.Services.ServiceHooks.WebApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Common
{
  public static class EventTransformer
  {
    private const EventResourceDetails c_defaultResourcesToSend = EventResourceDetails.All;
    private const EventMessages c_defaultMessagesToSend = EventMessages.All;
    private const EventMessages c_defaultDetailedMessagesToSend = EventMessages.All;
    private static readonly string[] s_minimalResourceFields = new string[13]
    {
      "id",
      "url",
      "webUrl",
      "name",
      "pullRequestId",
      "workItemId",
      "pushId",
      "changesetId",
      "stageName",
      "attemptId",
      "runId",
      "runUrl",
      "approvalId"
    };
    private static readonly Dictionary<EventResourceDetails, Func<JObject, JObject>> s_resourceFieldConverters = new Dictionary<EventResourceDetails, Func<JObject, JObject>>()
    {
      {
        EventResourceDetails.All,
        (Func<JObject, JObject>) (r => r)
      },
      {
        EventResourceDetails.None,
        (Func<JObject, JObject>) (r => (JObject) null)
      },
      {
        EventResourceDetails.Minimal,
        (Func<JObject, JObject>) (r => EventTransformer.GetMinimalResourceFields(r))
      }
    };
    private static readonly Dictionary<EventMessages, Func<JObject, JObject>> s_messageConverters = new Dictionary<EventMessages, Func<JObject, JObject>>()
    {
      {
        EventMessages.All,
        (Func<JObject, JObject>) (m => m)
      },
      {
        EventMessages.None,
        (Func<JObject, JObject>) (m => (JObject) null)
      },
      {
        EventMessages.Text,
        (Func<JObject, JObject>) (m => EventTransformer.FilterMessageField(m, "text"))
      },
      {
        EventMessages.Html,
        (Func<JObject, JObject>) (m => EventTransformer.FilterMessageField(m, "html"))
      },
      {
        EventMessages.Markdown,
        (Func<JObject, JObject>) (m => EventTransformer.FilterMessageField(m, "markdown"))
      }
    };
    private const string c_resourceFieldName = "resource";
    private const string c_resoureContainersFieldName = "resoureContainers";
    private const string c_resourceVersionFieldName = "resourceVersion";
    private const string c_messageFieldName = "message";
    private const string c_detailedMessageFieldName = "detailedMessage";
    private const string c_notificationDataFieldName = "data";

    public static JObject TransformEvent(
      Event raisedEvent,
      EventResourceDetails resourceDetailsToSend = EventResourceDetails.All,
      EventMessages messagesToSend = EventMessages.All,
      EventMessages detailedMessagesToSend = EventMessages.All,
      IDictionary<string, string> notificationData = null)
    {
      JObject jobject1 = JObject.FromObject((object) raisedEvent, JsonSerializer.Create(CommonConsumerSettings.JsonSerializerSettings));
      JObject jobject2 = EventTransformer.s_resourceFieldConverters[resourceDetailsToSend](jobject1["resource"] as JObject);
      jobject1["resource"] = (JToken) jobject2;
      if (jobject2 == null)
        jobject1["resourceVersion"] = (JToken) null;
      if (notificationData != null)
      {
        JObject jobject3 = JObject.FromObject((object) notificationData, JsonSerializer.Create(CommonConsumerSettings.JsonSerializerSettings));
        jobject1["data"] = (JToken) EventTransformer.s_resourceFieldConverters[EventResourceDetails.All](jobject3);
      }
      jobject1["message"] = (JToken) EventTransformer.s_messageConverters[messagesToSend](jobject1["message"] as JObject);
      jobject1["detailedMessage"] = (JToken) EventTransformer.s_messageConverters[detailedMessagesToSend](jobject1["detailedMessage"] as JObject);
      return jobject1;
    }

    public static EventResourceDetails ToEventResourceDetailsValue(
      this string value,
      EventResourceDetails defaultValue = EventResourceDetails.All)
    {
      return string.IsNullOrEmpty(value) ? defaultValue : (EventResourceDetails) Enum.Parse(typeof (EventResourceDetails), value, true);
    }

    public static EventMessages ToEventMessagesValue(this string value, EventMessages defaultValue = EventMessages.All) => string.IsNullOrEmpty(value) ? defaultValue : (EventMessages) Enum.Parse(typeof (EventMessages), value, true);

    private static JObject GetMinimalResourceFields(JObject resource)
    {
      JObject minimalResourceFields = new JObject();
      foreach (string minimalResourceField in EventTransformer.s_minimalResourceFields)
      {
        if (resource[minimalResourceField] != null)
          minimalResourceFields.Add(minimalResourceField, resource[minimalResourceField]);
      }
      return minimalResourceFields;
    }

    private static JObject FilterMessageField(JObject messageObject, string field) => new JObject()
    {
      {
        field,
        messageObject[field]
      }
    };
  }
}
