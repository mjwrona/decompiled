// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Azure.ServiceBusTopicSendAction
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Consumers, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5BEF54E7-7304-4071-B5F1-22428BB21801
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ServiceHooks.Consumers.dll

using Microsoft.ServiceBus.Messaging;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.FormInput;
using Microsoft.VisualStudio.Services.ServiceHooks.Common;
using Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Common;
using Microsoft.VisualStudio.Services.ServiceHooks.WebApi;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Azure
{
  [Export(typeof (ConsumerActionImplementation))]
  public sealed class ServiceBusTopicSendAction : ServiceBusActionBase
  {
    private const string c_id = "serviceBusTopicSend";
    private const string c_topicNameInputId = "topicName";
    private const int c_topicNameInputLengthMin = 1;
    private const int c_topicNameInputLengthMax = 50;
    private const string c_topicNameInputPattern = "^[A-Za-z0-9]$|^[A-Za-z0-9][\\w-\\.\\/]*[A-Za-z0-9]$";

    public ServiceBusTopicSendAction() => this.RegisterCommonInputsChangedCallback("topicName", new Func<string, InputValues>(this.GetInputValuesForTopicCombo), this.GetDefaultsForTopicCombo());

    public static string ConsumerActionId => "serviceBusTopicSend";

    public static string TopicNameInputId => "topicName";

    public override string Id => "serviceBusTopicSend";

    public override string Name => AzureConsumerResources.ServiceBusTopicSendActionName;

    public override string Description => AzureConsumerResources.ServiceBusTopicSendActionDescription;

    public override IList<InputDescriptor> InputDescriptors => (IList<InputDescriptor>) new List<InputDescriptor>()
    {
      new InputDescriptor()
      {
        Name = AzureConsumerResources.ServiceBusTopicSendAction_TopicNameInputName,
        Description = AzureConsumerResources.ServiceBusTopicSendAction_TopicNameInputDescription,
        InputMode = InputMode.Combo,
        Id = "topicName",
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
      ServiceBusActionBase.BuildBypassSerializerInputDescriptor()
    }.Union<InputDescriptor>(EventTransformerConsumerActionImplementation.BuildAllPayloadControllersInputDescriptors()).ToList<InputDescriptor>();

    public override string GetActionDescription(
      IVssRequestContext requestContext,
      IDictionary<string, string> consumerInputValues)
    {
      string str = (string) null;
      consumerInputValues?.TryGetValue("topicName", out str);
      return !string.IsNullOrEmpty(str) ? string.Format(AzureConsumerResources.ActionDescription_BusTopicFormat, (object) str) : throw new ArgumentException();
    }

    public ActionTask HandleEvent(
      IVssRequestContext requestContext,
      Event raisedEvent,
      HandleEventArgs eventArgs)
    {
      string serviceBusConnectionString = AzureServiceBusConsumer.BuildConnectionStringFromNotification(eventArgs.Notification);
      bool bypassSerializer = ServiceBusActionBase.GetBypassSerializer(eventArgs);
      string consumerInput = eventArgs.Notification.GetConsumerInput("topicName", true);
      EventResourceDetails resourceDetailsToSend = this.GetDefaultResourceDetailsToSend(eventArgs.Notification);
      EventMessages defaultMessagesToSend = this.GetDefaultMessagesToSend(eventArgs.Notification);
      EventMessages detailedMessagesToSend = this.GetDefaultDetailedMessagesToSend(eventArgs.Notification);
      string messageContent1 = ServiceBusActionBase.GetMessageContent(eventArgs.Notification, raisedEvent, resourceDetailsToSend, defaultMessagesToSend, detailedMessagesToSend);
      string topicName = consumerInput;
      string messageContent2 = messageContent1;
      int num = bypassSerializer ? 1 : 0;
      return (ActionTask) new ServiceBusTopicSendActionTask(serviceBusConnectionString, topicName, messageContent2, num != 0);
    }

    private InputValues GetDefaultsForTopicCombo()
    {
      IList<InputValue> inputValueList = (IList<InputValue>) new List<InputValue>();
      return new InputValues()
      {
        InputId = "topicName",
        DefaultValue = string.Empty,
        PossibleValues = inputValueList,
        IsLimitedToPossibleValues = false,
        IsDisabled = false,
        IsReadOnly = false
      };
    }

    private InputValues GetInputValuesForTopicCombo(string sbConnectionString)
    {
      InputValues defaultsForTopicCombo = this.GetDefaultsForTopicCombo();
      foreach (TopicDescription topic in this.CreateServiceBusNamespaceManagerForQueries(sbConnectionString).GetTopics())
        defaultsForTopicCombo.PossibleValues.Add(new InputValue()
        {
          Value = topic.Path,
          DisplayValue = topic.Path
        });
      return defaultsForTopicCombo;
    }
  }
}
