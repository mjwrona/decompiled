// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Azure.ServiceBusQueueSendAction
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
  public sealed class ServiceBusQueueSendAction : ServiceBusActionBase
  {
    private const string c_id = "serviceBusQueueSend";
    private const string c_queueNameInputId = "queueName";
    private const int c_queueNameInputLengthMin = 1;
    private const int c_queueNameInputLengthMax = 50;
    private const string c_queueNameInputPattern = "^[A-Za-z0-9]$|^[A-Za-z0-9][\\w-\\.\\/]*[A-Za-z0-9]$";

    public ServiceBusQueueSendAction() => this.RegisterCommonInputsChangedCallback("queueName", new Func<string, InputValues>(this.GetInputValuesForQueueCombo), this.GetDefaultsForQueueCombo());

    public static string ConsumerActionId => "serviceBusQueueSend";

    public static string QueueNameInputId => "queueName";

    public override string Id => "serviceBusQueueSend";

    public override string Name => AzureConsumerResources.ServiceBusQueueSendActionName;

    public override string Description => AzureConsumerResources.ServiceBusQueueSendActionDescription;

    public override IList<InputDescriptor> InputDescriptors => (IList<InputDescriptor>) new List<InputDescriptor>()
    {
      new InputDescriptor()
      {
        Name = AzureConsumerResources.ServiceBusQueueSendAction_QueueNameInputName,
        Description = AzureConsumerResources.ServiceBusQueueSendAction_QueueNameInputDescription,
        InputMode = InputMode.Combo,
        Id = "queueName",
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
      consumerInputValues?.TryGetValue("queueName", out str);
      return !string.IsNullOrEmpty(str) ? string.Format(AzureConsumerResources.ActionDescription_BusQueueFormat, (object) str) : throw new ArgumentException();
    }

    public ActionTask HandleEvent(
      IVssRequestContext requestContext,
      Event raisedEvent,
      HandleEventArgs eventArgs)
    {
      string serviceBusConnectionString = AzureServiceBusConsumer.BuildConnectionStringFromNotification(eventArgs.Notification);
      bool bypassSerializer = ServiceBusActionBase.GetBypassSerializer(eventArgs);
      string consumerInput = eventArgs.Notification.GetConsumerInput("queueName", true);
      EventResourceDetails resourceDetailsToSend = this.GetDefaultResourceDetailsToSend(eventArgs.Notification);
      EventMessages defaultMessagesToSend = this.GetDefaultMessagesToSend(eventArgs.Notification);
      EventMessages detailedMessagesToSend = this.GetDefaultDetailedMessagesToSend(eventArgs.Notification);
      string messageContent1 = ServiceBusActionBase.GetMessageContent(eventArgs.Notification, raisedEvent, resourceDetailsToSend, defaultMessagesToSend, detailedMessagesToSend);
      string queueName = consumerInput;
      string messageContent2 = messageContent1;
      int num = bypassSerializer ? 1 : 0;
      return (ActionTask) new ServiceBusQueueSendActionTask(serviceBusConnectionString, queueName, messageContent2, num != 0);
    }

    private InputValues GetDefaultsForQueueCombo() => new InputValues()
    {
      InputId = "queueName",
      DefaultValue = string.Empty,
      PossibleValues = (IList<InputValue>) new List<InputValue>(),
      IsLimitedToPossibleValues = false,
      IsDisabled = false,
      IsReadOnly = false
    };

    private InputValues GetInputValuesForQueueCombo(string sbConnectionString)
    {
      InputValues defaultsForQueueCombo = this.GetDefaultsForQueueCombo();
      foreach (QueueDescription queue in this.CreateServiceBusNamespaceManagerForQueries(sbConnectionString).GetQueues())
        defaultsForQueueCombo.PossibleValues.Add(new InputValue()
        {
          Value = queue.Path,
          DisplayValue = queue.Path
        });
      return defaultsForQueueCombo;
    }
  }
}
