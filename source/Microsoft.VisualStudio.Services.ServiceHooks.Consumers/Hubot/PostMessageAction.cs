// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Hubot.PostMessageAction
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Consumers, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5BEF54E7-7304-4071-B5F1-22428BB21801
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ServiceHooks.Consumers.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.FormInput;
using Microsoft.VisualStudio.Services.ServiceHooks.Common;
using Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Common;
using Microsoft.VisualStudio.Services.ServiceHooks.WebApi;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Text;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Hubot
{
  [Export(typeof (ConsumerActionImplementation))]
  public sealed class PostMessageAction : ConsumerActionImplementation
  {
    private const string c_id = "postMessage";
    private const int c_usernameInputMaxLength = 200;
    private const int c_passwordInputMaxLength = 500;
    public const string UrlInputId = "url";
    public const string UsernameInputId = "username";
    public const string PasswordInputId = "password";
    private static readonly string[] s_supportedEventTypes = new string[1]
    {
      "message.posted"
    };
    private static readonly IDictionary<string, string[]> s_supportedResourceVersions = (IDictionary<string, string[]>) new Dictionary<string, string[]>();

    public override string ConsumerId => "hubot";

    public override string Id => "postMessage";

    public override string Name => HubotConsumerResources.PostTeamRoomMessageToHubotActionName;

    public override string Description => HubotConsumerResources.PostTeamRoomMessageToHubotActionDescription;

    public override string[] SupportedEventTypes => PostMessageAction.s_supportedEventTypes;

    public override IDictionary<string, string[]> SupportedResourceVersions => PostMessageAction.s_supportedResourceVersions;

    public override bool AllowResourceVersionOverride => false;

    public override IList<InputDescriptor> InputDescriptors => (IList<InputDescriptor>) new List<InputDescriptor>()
    {
      new InputDescriptor()
      {
        Name = HubotConsumerResources.PostTeamRoomMessageToHubotAction_UrlInputName,
        Description = HubotConsumerResources.PostTeamRoomMessageToHubotAction_UrlInputDescription,
        InputMode = InputMode.TextBox,
        Id = "url",
        IsConfidential = false,
        UseInDefaultDescription = true,
        Validation = new InputValidation()
        {
          IsRequired = true,
          DataType = InputDataType.Uri
        }
      },
      new InputDescriptor()
      {
        Name = HubotConsumerResources.PostTeamRoomMessageToHubotAction_UsernameInputName,
        Description = HubotConsumerResources.PostTeamRoomMessageToHubotAction_UsernameInputDescription,
        InputMode = InputMode.TextBox,
        Id = "username",
        IsConfidential = false,
        Validation = new InputValidation()
        {
          IsRequired = true,
          DataType = InputDataType.String,
          MaxLength = new int?(200)
        }
      },
      new InputDescriptor()
      {
        Name = HubotConsumerResources.PostTeamRoomMessageToHubotAction_PasswordInputName,
        Description = HubotConsumerResources.PostTeamRoomMessageToHubotAction_PasswordInputDescription,
        InputMode = InputMode.PasswordBox,
        Id = "password",
        IsConfidential = true,
        Validation = new InputValidation()
        {
          IsRequired = true,
          DataType = InputDataType.String,
          MaxLength = new int?(500)
        }
      }
    };

    public ActionTask HandleEvent(
      IVssRequestContext requestContext,
      Event raisedEvent,
      HandleEventArgs eventArgs)
    {
      string consumerInput1 = eventArgs.Notification.GetConsumerInput("url", true);
      string consumerInput2 = eventArgs.Notification.GetConsumerInput("username", true);
      string consumerInput3 = eventArgs.Notification.GetConsumerInput("password", true);
      ServiceHooksHttpRequestMessage request = new ServiceHooksHttpRequestMessage(requestContext, eventArgs.Notification, HttpMethod.Post, consumerInput1, consumerInput2, consumerInput3);
      string send = HubotConsumer.BuildMessagePayloadToSend(raisedEvent);
      request.Content = (HttpContent) new StringContent(send, Encoding.UTF8, JsonMediaTypeFormatter.DefaultMediaType.MediaType);
      return (ActionTask) new HttpActionTask((HttpRequestMessage) request, request.BuildHttpRequestStringRepresentation(send));
    }
  }
}
