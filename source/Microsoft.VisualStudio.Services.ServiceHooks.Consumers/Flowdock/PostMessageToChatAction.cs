// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Flowdock.PostMessageToChatAction
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
using System.Text;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Flowdock
{
  [Export(typeof (ConsumerActionImplementation))]
  public sealed class PostMessageToChatAction : ConsumerActionImplementation
  {
    private const string c_id = "postMessageToChat";
    private const string c_externalUserNameInputPattern = "^[\\w0-9@\\.]+$";
    private const int c_externalUserNameInputMaxLength = 16;
    private const string c_tagsInputPattern = "^(\\s*[^\\s,]+\\s*)(,(\\s*[^\\s,]+\\s*))*$";
    private const string c_urlFormatSendTeamChat = "https://api.flowdock.com/v1/messages/chat/{0}";
    private const string c_registryPathUrlFormatSendTeamChat = "/Service/ServiceHooks/FlowdockConsumer/PostMessageToChatAction/UrlFormatSendTeamChat";
    public const string ExternalUserNameInputId = "externalUserName";
    public const string TagsInputId = "tags";
    private static readonly string[] s_supportedEventTypes = new string[1]
    {
      "*"
    };
    private static readonly IDictionary<string, string[]> s_supportedResourceVersions = (IDictionary<string, string[]>) new Dictionary<string, string[]>();

    public override string ConsumerId => "flowdock";

    public override string Id => "postMessageToChat";

    public override string Name => FlowdockConsumerResources.PostMessageToChatActionName;

    public override string Description => FlowdockConsumerResources.PostMessageToChatActionDescription;

    public override string[] SupportedEventTypes => PostMessageToChatAction.s_supportedEventTypes;

    public override IDictionary<string, string[]> SupportedResourceVersions => PostMessageToChatAction.s_supportedResourceVersions;

    public override bool AllowResourceVersionOverride => false;

    public override IList<InputDescriptor> InputDescriptors => (IList<InputDescriptor>) new List<InputDescriptor>()
    {
      new InputDescriptor()
      {
        Name = FlowdockConsumerResources.PostMessageToChatAction_ExternalUserNameInputName,
        Description = FlowdockConsumerResources.PostMessageToChatAction_ExternalUserNameInputDescription,
        InputMode = InputMode.TextBox,
        Id = "externalUserName",
        IsConfidential = false,
        Validation = new InputValidation()
        {
          IsRequired = true,
          DataType = InputDataType.String,
          Pattern = "^[\\w0-9@\\.]+$",
          MaxLength = new int?(16)
        }
      },
      new InputDescriptor()
      {
        Name = FlowdockConsumerResources.PostMessageToChatAction_TagsInputName,
        Description = FlowdockConsumerResources.PostMessageToChatAction_TagsInputDescription,
        InputMode = InputMode.TextBox,
        Id = "tags",
        IsConfidential = false,
        Validation = new InputValidation()
        {
          DataType = InputDataType.String,
          Pattern = "^(\\s*[^\\s,]+\\s*)(,(\\s*[^\\s,]+\\s*))*$"
        }
      }
    };

    public ActionTask HandleEvent(
      IVssRequestContext requestContext,
      Event raisedEvent,
      HandleEventArgs eventArgs)
    {
      string consumerInput = eventArgs.Notification.GetConsumerInput("flowAPIToken", true);
      bool result;
      bool.TryParse(eventArgs.Notification.GetConsumerInput("showDetails"), out result);
      string message = result ? raisedEvent.DetailedMessage.Text : raisedEvent.Message.Text;
      string confidentialUrl;
      HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, PostMessageToChatAction.BuildSendToTeamChatUrl(requestContext, consumerInput, out confidentialUrl));
      string chatPayload = this.BuildPostMessageToChatPayload(message, eventArgs);
      httpRequestMessage.Content = (HttpContent) new StringContent(chatPayload, Encoding.UTF8, JsonMediaTypeFormatter.DefaultMediaType.MediaType);
      return (ActionTask) new HttpActionTask(httpRequestMessage, httpRequestMessage.BuildHttpRequestStringRepresentation(chatPayload, confidentialUrl));
    }

    private string BuildPostMessageToChatPayload(string message, HandleEventArgs eventArgs)
    {
      string consumerInput1 = eventArgs.Notification.GetConsumerInput("externalUserName", true);
      string consumerInput2 = eventArgs.Notification.GetConsumerInput("tags");
      JObject jObject = new JObject();
      jObject.Add("content", (JToken) message);
      jObject.Add("external_user_name", (JToken) consumerInput1);
      if (!string.IsNullOrWhiteSpace(consumerInput2))
        jObject.Add("tags", (JToken) new JArray((object) ((IEnumerable<string>) consumerInput2.Split(',')).Select<string, string>((Func<string, string>) (e => e.Trim()))));
      return jObject.GetStringRepresentation();
    }

    private static string BuildSendToTeamChatUrl(
      IVssRequestContext requestContext,
      string flowApiToken,
      out string confidentialUrl)
    {
      string format = requestContext.GetService<IVssRegistryService>().GetValue<string>(requestContext, (RegistryQuery) "/Service/ServiceHooks/FlowdockConsumer/PostMessageToChatAction/UrlFormatSendTeamChat", true, "https://api.flowdock.com/v1/messages/chat/{0}");
      confidentialUrl = string.Format(format, (object) SecurityHelper.GetMaskedValue(flowApiToken));
      return string.Format(format, (object) flowApiToken);
    }
  }
}
