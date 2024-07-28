// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Flowdock.PostMessageToTeamInboxAction
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
  public class PostMessageToTeamInboxAction : ConsumerActionImplementation
  {
    private const string c_id = "postMessageToTeamInbox";
    private const int c_fromAddressInputLengthMax = 254;
    private const int c_replyToInputLengthMax = 254;
    private const string c_tagsInputPattern = "^(\\s*[^\\s,]+\\s*)(,(\\s*[^\\s,]+\\s*))*$";
    private const string c_projectInputPattern = "^[\\w0-9\\s]*$";
    private const string c_emailAddressInputPattern = "^.+\\@.+\\..+$";
    private const string c_urlFormatSendTeamInbox = "https://api.flowdock.com/v1/vso/{0}";
    private const string c_registryPathUrlFormatSendTeamInbox = "/Service/ServiceHooks/FlowdockConsumer/PostMessageToTeamInboxAction/UrlFormatSendTeamInbox";
    public const string SubjectInputId = "subject";
    public const string FromNameInputId = "fromName";
    public const string FromAddressInputId = "fromAddress";
    public const string ReplyToInputId = "replyTo";
    public const string ProjectInputId = "project";
    public const string TagsInputId = "tags";
    private static readonly string[] s_supportedEventTypes = new string[1]
    {
      "*"
    };
    private static readonly IDictionary<string, string[]> s_supportedResourceVersions = (IDictionary<string, string[]>) new Dictionary<string, string[]>();

    public override string ConsumerId => "flowdock";

    public override string Id => "postMessageToTeamInbox";

    public override string Name => FlowdockConsumerResources.PostMessageToTeamInboxActionName;

    public override string Description => FlowdockConsumerResources.PostMessageToTeamInboxActionDescription;

    public override string[] SupportedEventTypes => PostMessageToTeamInboxAction.s_supportedEventTypes;

    public override IDictionary<string, string[]> SupportedResourceVersions => PostMessageToTeamInboxAction.s_supportedResourceVersions;

    public override bool AllowResourceVersionOverride => false;

    public override IList<InputDescriptor> InputDescriptors => (IList<InputDescriptor>) new List<InputDescriptor>()
    {
      new InputDescriptor()
      {
        Name = FlowdockConsumerResources.PostMessageToTeamInboxAction_SubjectInputName,
        Description = FlowdockConsumerResources.PostMessageToTeamInboxAction_SubjectInputDescription,
        InputMode = InputMode.TextBox,
        Id = "subject",
        Validation = new InputValidation()
        {
          IsRequired = true,
          DataType = InputDataType.String
        }
      },
      new InputDescriptor()
      {
        Name = FlowdockConsumerResources.PostMessageToTeamInboxAction_FromNameInputName,
        Description = FlowdockConsumerResources.PostMessageToTeamInboxAction_FromNameInputDescription,
        InputMode = InputMode.TextBox,
        Id = "fromName",
        Validation = new InputValidation()
        {
          DataType = InputDataType.String
        }
      },
      new InputDescriptor()
      {
        Name = FlowdockConsumerResources.PostMessageToTeamInboxAction_FromAddressInputName,
        Description = FlowdockConsumerResources.PostMessageToTeamInboxAction_FromAddressInputDescription,
        InputMode = InputMode.TextBox,
        Id = "fromAddress",
        IsConfidential = false,
        Validation = new InputValidation()
        {
          IsRequired = true,
          DataType = InputDataType.String,
          MaxLength = new int?(254),
          Pattern = "^.+\\@.+\\..+$"
        }
      },
      new InputDescriptor()
      {
        Name = FlowdockConsumerResources.PostMessageToTeamInboxAction_ReplyToInputName,
        Description = FlowdockConsumerResources.PostMessageToTeamInboxAction_ReplyToInputDescription,
        InputMode = InputMode.TextBox,
        Id = "replyTo",
        Validation = new InputValidation()
        {
          DataType = InputDataType.String,
          MaxLength = new int?(254),
          Pattern = "^.+\\@.+\\..+$"
        }
      },
      new InputDescriptor()
      {
        Name = FlowdockConsumerResources.PostMessageToTeamInboxAction_ProjectInputName,
        Description = FlowdockConsumerResources.PostMessageToTeamInboxAction_ProjectInputDescription,
        InputMode = InputMode.TextBox,
        Id = "project",
        Validation = new InputValidation()
        {
          DataType = InputDataType.String,
          Pattern = "^[\\w0-9\\s]*$"
        }
      },
      new InputDescriptor()
      {
        Name = FlowdockConsumerResources.PostMessageToTeamInboxAction_TagsInputName,
        Description = FlowdockConsumerResources.PostMessageToTeamInboxAction_TagsInputDescription,
        InputMode = InputMode.TextBox,
        Id = "tags",
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
      string message = result ? raisedEvent.DetailedMessage.Html : raisedEvent.Message.Html;
      string confidentialUrl;
      HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, PostMessageToTeamInboxAction.BuildSendTeamInboxUrl(requestContext, consumerInput, out confidentialUrl));
      string teamInboxPayload = this.BuildPostMessageToTeamInboxPayload(message, PostMessageToTeamInboxAction.GetSource(requestContext), eventArgs, raisedEvent);
      httpRequestMessage.Content = (HttpContent) new StringContent(teamInboxPayload, Encoding.UTF8, JsonMediaTypeFormatter.DefaultMediaType.MediaType);
      return (ActionTask) new HttpActionTask(httpRequestMessage, httpRequestMessage.BuildHttpRequestStringRepresentation(teamInboxPayload, confidentialUrl));
    }

    private string BuildPostMessageToTeamInboxPayload(
      string message,
      string source,
      HandleEventArgs eventArgs,
      Event raisedEvent)
    {
      string consumerInput1 = eventArgs.Notification.GetConsumerInput("fromAddress", true);
      string consumerInput2 = eventArgs.Notification.GetConsumerInput("subject", true);
      string consumerInput3 = eventArgs.Notification.GetConsumerInput("fromName");
      string consumerInput4 = eventArgs.Notification.GetConsumerInput("replyTo");
      string consumerInput5 = eventArgs.Notification.GetConsumerInput("tags");
      string consumerInput6 = eventArgs.Notification.GetConsumerInput("project");
      JObject jobject = new JObject();
      jobject.Add(nameof (source), (JToken) source);
      jobject.Add("content", (JToken) message);
      jobject.Add("from_address", (JToken) consumerInput1);
      jobject.Add("subject", (JToken) consumerInput2);
      PostMessageToTeamInboxAction.AddPropertyIfHasValue(jobject, "from_name", consumerInput3);
      PostMessageToTeamInboxAction.AddPropertyIfHasValue(jobject, "project", consumerInput6);
      PostMessageToTeamInboxAction.AddPropertyIfHasValue(jobject, "reply_to", consumerInput4);
      if (!string.IsNullOrWhiteSpace(consumerInput5))
        jobject.Add("tags", (JToken) new JArray((object) ((IEnumerable<string>) consumerInput5.Split(',')).Select<string, string>((Func<string, string>) (e => e.Trim()))));
      return jobject.GetStringRepresentation();
    }

    private static void AddPropertyIfHasValue(
      JObject jBodyPayload,
      string propertyName,
      string value)
    {
      if (string.IsNullOrWhiteSpace(value))
        return;
      jBodyPayload.Add(propertyName, (JToken) value);
    }

    private static string BuildSendTeamInboxUrl(
      IVssRequestContext requestContext,
      string flowApiToken,
      out string confidentialUrl)
    {
      string str = requestContext.GetService<IVssRegistryService>().GetValue<string>(requestContext, (RegistryQuery) "/Service/ServiceHooks/FlowdockConsumer/PostMessageToTeamInboxAction/UrlFormatSendTeamInbox", true, "https://api.flowdock.com/v1/vso/{0}");
      confidentialUrl = string.Format(str, (object) SecurityHelper.GetMaskedValue(str));
      return string.Format(str, (object) flowApiToken);
    }

    private static string GetSource(IVssRequestContext requestContext) => requestContext.ExecutionEnvironment.IsOnPremisesDeployment ? FlowdockConsumerResources.FlowdockConsumer_OnPremiseSourceName : FlowdockConsumerResources.FlowdockConsumer_HostedSourceName;
  }
}
