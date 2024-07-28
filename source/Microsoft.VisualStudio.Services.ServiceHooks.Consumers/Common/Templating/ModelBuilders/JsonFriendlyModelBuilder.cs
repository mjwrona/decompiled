// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Common.Templating.ModelBuilders.JsonFriendlyModelBuilder
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Consumers, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5BEF54E7-7304-4071-B5F1-22428BB21801
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ServiceHooks.Consumers.dll

using Microsoft.VisualStudio.Services.ServiceHooks.WebApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Common.Templating.ModelBuilders
{
  public class JsonFriendlyModelBuilder : ITemplateModelBuilder
  {
    private const string c_defaultResourceName = "resource";
    private const string c_messageFieldName = "message";
    private const string c_detailedMessageFieldName = "detailedMessage";
    private const string c_linksFieldName = "links";
    private const string c_originalLinksFieldName = "_links";
    private const string c_hrefLinkFieldName = "href";
    private static readonly StringDictionary s_resourceFriendlyNames = new StringDictionary()
    {
      {
        "build.complete",
        "build"
      },
      {
        "git.push",
        "push"
      },
      {
        "git.pullrequest.created",
        "pullrequest"
      },
      {
        "git.pullrequest.updated",
        "pullrequest"
      },
      {
        "git.pullrequest.merged",
        "pullrequest"
      },
      {
        "ms.vss-code.codereview-created-event",
        "codereview"
      },
      {
        "ms.vss-code.codereview-updated-event",
        "codereview"
      },
      {
        "ms.vss-code.codereview-iteration-changed-event",
        "codereview"
      },
      {
        "ms.vss-code.codereview-reviewers-changed-event",
        "codereview"
      },
      {
        "tfvc.checkin",
        "changeset"
      },
      {
        "workitem.created",
        "workitem"
      },
      {
        "workitem.updated",
        "workitem"
      },
      {
        "workitem.commented",
        "workitem"
      },
      {
        "message.posted",
        "messageposted"
      }
    };

    public virtual JObject Build(Event @event)
    {
      Dictionary<string, object> dictionary = new Dictionary<string, object>();
      dictionary.Add("message", (object) @event.Message);
      dictionary.Add("detailedMessage", (object) @event.DetailedMessage);
      JObject jobject = this.TransformResource(JObject.FromObject(@event.Resource, JsonSerializer.Create(CommonConsumerSettings.JsonSerializerSettings)), @event.EventType);
      dictionary.Add(this.GetResourceName(@event.EventType), (object) jobject);
      return JObject.Parse(JsonConvert.SerializeObject((object) dictionary, CommonConsumerSettings.JsonSerializerSettings));
    }

    protected virtual JObject TransformResource(JObject resource, string eventType)
    {
      JToken jtoken = resource["_links"];
      if (jtoken == null)
        return resource;
      JObject promotedLinks = new JObject();
      jtoken.Children<JProperty>().ToList<JProperty>().ForEach((Action<JProperty>) (p =>
      {
        JToken source = p.Value;
        if (source.Type == JTokenType.Array)
          source = source.First<JToken>();
        string str = (string) source[(object) "href"];
        if (string.IsNullOrEmpty(str))
          return;
        promotedLinks.Add(p.Name, (JToken) str);
      }));
      resource.Add("links", (JToken) promotedLinks);
      return resource;
    }

    protected virtual string GetResourceName(string eventType) => JsonFriendlyModelBuilder.s_resourceFriendlyNames[eventType] ?? "resource";
  }
}
