// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Azure.ServiceBusNotificationHubSendAction
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Consumers, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5BEF54E7-7304-4071-B5F1-22428BB21801
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ServiceHooks.Consumers.dll

using Microsoft.Azure.NotificationHubs;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.FormInput;
using Microsoft.VisualStudio.Services.ServiceHooks.Common;
using Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Common;
using Microsoft.VisualStudio.Services.ServiceHooks.WebApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Text.RegularExpressions;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Azure
{
  [Export(typeof (ConsumerActionImplementation))]
  public sealed class ServiceBusNotificationHubSendAction : ServiceBusActionBase
  {
    private const string c_id = "serviceBusNotificationHubSend";
    private const string c_notificationHubNameInputId = "notificationHubName";
    private const string c_tagsExpressionInputId = "tagsExpression";
    private const int c_notificationHubNameInputLengthMin = 1;
    private const int c_notificationHubNameInputLengthMax = 50;
    private const int c_notificationHubTagInputLengthMax = 1024;
    private const string c_tagNamePattern = "[a-zA-Z0-9_@#:.-]{1,120}";
    private const string c_tagContainerPattern = "[\\s(!]*[a-zA-Z0-9_@#:.-]{1,120}[\\s)]*";
    private const string c_tagRightOperandPattern = "\\s+(?:&&|\\|\\|)\\s+[\\s(!]*[a-zA-Z0-9_@#:.-]{1,120}[\\s)]*";
    private const string c_notificationHubTagInputPattern = "^[\\s(!]*[a-zA-Z0-9_@#:.-]{1,120}[\\s)]*(?:\\s+(?:&&|\\|\\|)\\s+[\\s(!]*[a-zA-Z0-9_@#:.-]{1,120}[\\s)]*)*$";
    private const string c_notificationHubNameInputPattern = "^[A-Za-z0-9]$|^[A-Za-z0-9][\\w-\\.\\/]*[A-Za-z0-9]$";
    private const string c_jsonPathCharsToReplaceRegexPattern = "\\.|\\[|\\]";
    private static readonly Regex s_fixJsonPathRegex = new Regex("\\.|\\[|\\]", RegexOptions.Compiled);

    public ServiceBusNotificationHubSendAction() => this.RegisterCommonInputsChangedCallback("notificationHubName", new Func<string, InputValues>(this.GetInputValuesForNotificationHubCombo), this.GetDefaultsForNotificationHubCombo());

    public static string ConsumerActionId => "serviceBusNotificationHubSend";

    public static string NotificationHubNameInputId => "notificationHubName";

    public static string TagsExpressionInputId => "tagsExpression";

    public override string Id => "serviceBusNotificationHubSend";

    public override string Name => AzureConsumerResources.ServiceBusNotificationHubSendActionName;

    public override string Description => AzureConsumerResources.ServiceBusNotificationHubSendActionDescription;

    public override IList<InputDescriptor> InputDescriptors => (IList<InputDescriptor>) new List<InputDescriptor>()
    {
      new InputDescriptor()
      {
        Name = AzureConsumerResources.ServiceBusNotificationHubSendAction_NotificationHubNameInputName,
        Description = AzureConsumerResources.ServiceBusNotificationHubSendAction_NotificationHubNameInputDescription,
        InputMode = InputMode.Combo,
        Id = "notificationHubName",
        IsConfidential = false,
        Validation = new InputValidation()
        {
          IsRequired = true,
          DataType = InputDataType.String,
          MinLength = new int?(1),
          MaxLength = new int?(50),
          Pattern = "^[A-Za-z0-9]$|^[A-Za-z0-9][\\w-\\.\\/]*[A-Za-z0-9]$"
        },
        DependencyInputIds = (IList<string>) new string[1]
        {
          AzureServiceBusConsumer.ConnectionStringInputId
        },
        HasDynamicValueInformation = true
      },
      new InputDescriptor()
      {
        Name = AzureConsumerResources.ServiceBusNotificationHubSendAction_TagsExpressionInputName,
        Description = AzureConsumerResources.ServiceBusNotificationHubSendAction_TagsExpressionInputDescription,
        InputMode = InputMode.TextBox,
        Id = "tagsExpression",
        IsConfidential = false,
        Validation = new InputValidation()
        {
          IsRequired = false,
          DataType = InputDataType.String,
          MaxLength = new int?(1024),
          Pattern = "^[\\s(!]*[a-zA-Z0-9_@#:.-]{1,120}[\\s)]*(?:\\s+(?:&&|\\|\\|)\\s+[\\s(!]*[a-zA-Z0-9_@#:.-]{1,120}[\\s)]*)*$",
          PatternMismatchErrorMessage = AzureConsumerResources.ServiceBusNotificationHubSendAction_TagsExpressionInputPatternErrorMessage
        }
      }
    };

    public override string GetActionDescription(
      IVssRequestContext requestContext,
      IDictionary<string, string> consumerInputValues)
    {
      string str = (string) null;
      consumerInputValues?.TryGetValue("notificationHubName", out str);
      return !string.IsNullOrEmpty(str) ? string.Format(AzureConsumerResources.ActionDescription_BusHubFormat, (object) str) : throw new ArgumentException();
    }

    public ActionTask HandleEvent(
      IVssRequestContext requestContext,
      Event raisedEvent,
      HandleEventArgs eventArgs)
    {
      string serviceBusConnectionString = AzureServiceBusConsumer.BuildConnectionStringFromNotification(eventArgs.Notification);
      string consumerInput1 = eventArgs.Notification.GetConsumerInput("notificationHubName", true);
      string consumerInput2 = eventArgs.Notification.GetConsumerInput("tagsExpression");
      IDictionary<string, string> notificationPropertiesEvent = (IDictionary<string, string>) this.GetNotificationPropertiesEvent(EventTransformer.TransformEvent(raisedEvent, EventResourceDetails.Minimal, detailedMessagesToSend: EventMessages.None));
      string str = JsonConvert.SerializeObject((object) notificationPropertiesEvent, CommonConsumerSettings.JsonSerializerSettings);
      string notificationHubName = consumerInput1;
      string tagsExpression = consumerInput2;
      IDictionary<string, string> templateProperties = notificationPropertiesEvent;
      string messageContent = str;
      return (ActionTask) new ServiceBusNotificationHubSendActionTask(serviceBusConnectionString, notificationHubName, tagsExpression, templateProperties, messageContent);
    }

    private Dictionary<string, string> GetNotificationPropertiesEvent(JObject jObject) => jObject.Flatten<Dictionary<string, string>>(new Dictionary<string, string>(), (Func<Dictionary<string, string>, string, JToken, Dictionary<string, string>>) ((dict, path, jToken) =>
    {
      path = ServiceBusNotificationHubSendAction.s_fixJsonPathRegex.Replace(path, "_");
      dict.Add(path, jToken.ToString());
      return dict;
    }));

    private InputValues GetDefaultsForNotificationHubCombo() => new InputValues()
    {
      InputId = "notificationHubName",
      DefaultValue = string.Empty,
      PossibleValues = (IList<InputValue>) new List<InputValue>(),
      IsLimitedToPossibleValues = false,
      IsDisabled = false,
      IsReadOnly = false
    };

    private InputValues GetInputValuesForNotificationHubCombo(string sbConnectionString)
    {
      InputValues notificationHubCombo = this.GetDefaultsForNotificationHubCombo();
      foreach (NotificationHubDescription notificationHub in this.CreateNotificationHubNamespaceManagerForQueries(sbConnectionString).GetNotificationHubs())
        notificationHubCombo.PossibleValues.Add(new InputValue()
        {
          Value = notificationHub.Path,
          DisplayValue = notificationHub.Path
        });
      return notificationHubCombo;
    }
  }
}
