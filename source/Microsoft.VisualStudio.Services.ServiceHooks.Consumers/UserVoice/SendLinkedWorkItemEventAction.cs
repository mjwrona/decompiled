// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Consumers.UserVoice.SendLinkedWorkItemEventAction
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Consumers, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5BEF54E7-7304-4071-B5F1-22428BB21801
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ServiceHooks.Consumers.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.FormInput;
using Microsoft.VisualStudio.Services.ServiceHooks.Common;
using Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Common;
using Microsoft.VisualStudio.Services.ServiceHooks.WebApi;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Consumers.UserVoice
{
  [Export(typeof (ConsumerActionImplementation))]
  public sealed class SendLinkedWorkItemEventAction : ConsumerActionImplementation
  {
    private const string c_id = "sendLinkedWorkItemEvent";
    private const string c_schemeAuthorizationHeader = "Bearer";
    private const string c_userVoiceLinkUrlPathPattern = "^/forums/(?<forumId>\\d+)(?:(-[^/]*)?)/suggestions/(?<suggestionId>\\d+)(?:(-[^/]*)?)$";
    private const string c_workItemCreatedLinksJsonPath = "resource.relations";
    private const string c_workItemUpdatesLinksJsonPath = "resource.revision.relations";
    public const string UrlInputId = "url";
    public const string AuthTokenInputId = "authToken";
    private static readonly string[] s_supportedEventTypes = new string[2]
    {
      "workitem.created",
      "workitem.updated"
    };
    private static readonly IDictionary<string, string[]> s_supportedResourceVersions = (IDictionary<string, string[]>) new Dictionary<string, string[]>()
    {
      {
        "workitem.created",
        new string[1]{ "1.0" }
      },
      {
        "workitem.updated",
        new string[1]{ "1.0" }
      }
    };
    private static readonly Regex s_userVoiceLinkUrlPathRegex = new Regex("^/forums/(?<forumId>\\d+)(?:(-[^/]*)?)/suggestions/(?<suggestionId>\\d+)(?:(-[^/]*)?)$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

    public override string ConsumerId => "userVoice";

    public override string Id => "sendLinkedWorkItemEvent";

    public override string Name => UserVoiceConsumerResources.SendLinkedWorkItemEventActionName;

    public override string Description => UserVoiceConsumerResources.SendLinkedWorkItemEventActionDescription;

    public override string[] SupportedEventTypes => SendLinkedWorkItemEventAction.s_supportedEventTypes;

    public override IDictionary<string, string[]> SupportedResourceVersions => SendLinkedWorkItemEventAction.s_supportedResourceVersions;

    public override bool AllowResourceVersionOverride => false;

    public override IList<InputDescriptor> InputDescriptors => (IList<InputDescriptor>) new List<InputDescriptor>()
    {
      new InputDescriptor()
      {
        Name = UserVoiceConsumerResources.SendLinkedWorkItemEventAction_UrlInputName,
        Description = UserVoiceConsumerResources.SendLinkedWorkItemEventAction_UrlInputDescription,
        InputMode = InputMode.TextBox,
        Id = "url",
        IsConfidential = false,
        Validation = new InputValidation()
        {
          DataType = InputDataType.Uri,
          IsRequired = true
        }
      },
      new InputDescriptor()
      {
        Name = UserVoiceConsumerResources.SendLinkedWorkItemEventAction_AuthTokenInputName,
        Description = UserVoiceConsumerResources.SendLinkedWorkItemEventAction_AuthTokenInputDescription,
        InputMode = InputMode.TextBox,
        Id = "authToken",
        IsConfidential = true,
        Validation = new InputValidation()
        {
          DataType = InputDataType.String,
          IsRequired = true
        }
      }
    };

    public override string GetActionDescription(
      IVssRequestContext requestContext,
      IDictionary<string, string> consumerInputValues)
    {
      string uriString = (string) null;
      consumerInputValues.TryGetValue("url", out uriString);
      if (uriString == null)
        throw new ArgumentNullException();
      try
      {
        return string.Format(UserVoiceConsumerResources.SendLinkedWorkItemEventAction_DetailedDescriptionFormat, (object) new Uri(uriString).Host);
      }
      catch (UriFormatException ex)
      {
        return uriString;
      }
    }

    public ActionTask HandleEvent(
      IVssRequestContext requestContext,
      Event raisedEvent,
      HandleEventArgs eventArgs)
    {
      JObject jobject = raisedEvent.ToJObject();
      string consumerInput1 = eventArgs.Notification.GetConsumerInput("url", true);
      string host = new Uri(consumerInput1).Host;
      if (!SendLinkedWorkItemEventAction.WorkItemHasUserVoiceLinks(raisedEvent, jobject, host))
        return (ActionTask) new NoopActionTask(UserVoiceConsumerResources.NoopAction_NoUserVoiceArtifact);
      string consumerInput2 = eventArgs.Notification.GetConsumerInput("authToken", true);
      HttpRequestMessage requestMessage = SendLinkedWorkItemEventAction.CreateRequestMessage(HttpMethod.Post, consumerInput1, consumerInput2);
      string stringRepresentation = jobject.GetStringRepresentation();
      requestMessage.Content = (HttpContent) new StringContent(stringRepresentation, Encoding.UTF8, JsonMediaTypeFormatter.DefaultMediaType.MediaType);
      return (ActionTask) new HttpActionTask(requestMessage, requestMessage.BuildHttpRequestStringRepresentation(stringRepresentation));
    }

    private static bool WorkItemHasUserVoiceLinks(
      Event raisedEvent,
      JObject raisedEventJObject,
      string customerDomain)
    {
      JArray workItemLinks = SendLinkedWorkItemEventAction.GetWorkItemLinks(raisedEvent, raisedEventJObject);
      return workItemLinks != null && workItemLinks.Children<JObject>().FirstOrDefault<JObject>((Func<JObject, bool>) (l => SendLinkedWorkItemEventAction.IsUserVoiceLink((string) l["url"], customerDomain))) != null;
    }

    private static JArray GetWorkItemLinks(Event raisedEvent, JObject raisedEventJObject)
    {
      string path = raisedEvent.EventType == "workitem.updated" ? "resource.revision.relations" : "resource.relations";
      return raisedEventJObject.SelectToken(path, false) as JArray;
    }

    private static bool IsUserVoiceLink(string link, string customerDomain)
    {
      if (string.IsNullOrWhiteSpace(link) || !Uri.IsWellFormedUriString(link, UriKind.Absolute))
        return false;
      Uri uri = new Uri(link);
      return uri.Host.Equals(customerDomain, StringComparison.InvariantCultureIgnoreCase) && SendLinkedWorkItemEventAction.s_userVoiceLinkUrlPathRegex.IsMatch(uri.AbsolutePath);
    }

    private static HttpRequestMessage CreateRequestMessage(
      HttpMethod method,
      string requestUrl,
      string authToken)
    {
      return new HttpRequestMessage(method, requestUrl)
      {
        Headers = {
          Authorization = new AuthenticationHeaderValue("Bearer", authToken)
        }
      };
    }
  }
}
