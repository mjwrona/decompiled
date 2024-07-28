// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Hubot.PostMessageToQueueAction
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Consumers, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5BEF54E7-7304-4071-B5F1-22428BB21801
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ServiceHooks.Consumers.dll

using Microsoft.ServiceBus.Messaging;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.FormInput;
using Microsoft.VisualStudio.Services.ServiceHooks.Common;
using Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Azure;
using Microsoft.VisualStudio.Services.ServiceHooks.WebApi;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Configuration;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Hubot
{
  [Export(typeof (ConsumerActionImplementation))]
  public sealed class PostMessageToQueueAction : ConsumerActionImplementation
  {
    private const string c_id = "postMessageToQueue";
    private const int c_connectionStringInputLengthMin = 1;
    private const int c_connectionStringInputLengthMax = 500;
    private const int c_queueNameInputLengthMin = 1;
    private const int c_queueNameInputLengthMax = 50;
    private const string c_queueNameInputPattern = "^[A-Za-z0-9]$|^[A-Za-z0-9][\\w-\\.\\/]*[A-Za-z0-9]$";
    private static readonly string[] s_supportedEventTypes = new string[1]
    {
      "message.posted"
    };
    private static readonly IDictionary<string, string[]> s_supportedResourceVersions = (IDictionary<string, string[]>) new Dictionary<string, string[]>();
    public const string ConnectionStringInputId = "connectionString";
    public const string QueueNameInputId = "queueName";

    public override string ConsumerId => "hubot";

    public override string Id => "postMessageToQueue";

    public override string Name => HubotConsumerResources.EnqueueTeamRoomMessageToHubotActionName;

    public override string Description => HubotConsumerResources.EnqueueTeamRoomMessageToHubotActionDescription;

    public override string[] SupportedEventTypes => PostMessageToQueueAction.s_supportedEventTypes;

    public override IDictionary<string, string[]> SupportedResourceVersions => PostMessageToQueueAction.s_supportedResourceVersions;

    public override bool AllowResourceVersionOverride => false;

    public override IList<InputDescriptor> InputDescriptors => (IList<InputDescriptor>) new List<InputDescriptor>()
    {
      new InputDescriptor()
      {
        Name = HubotConsumerResources.EnqueueTeamRoomMessageToHubotAction_ConnectionStringInputName,
        Description = HubotConsumerResources.EnqueueTeamRoomMessageToHubotAction_ConnectionStringInputDescription,
        InputMode = InputMode.TextBox,
        Id = "connectionString",
        IsConfidential = true,
        Validation = new InputValidation()
        {
          IsRequired = true,
          DataType = InputDataType.String,
          MaxLength = new int?(500),
          MinLength = new int?(1)
        }
      },
      new InputDescriptor()
      {
        Name = HubotConsumerResources.EnqueueTeamRoomMessageToHubotAction_QueueNameInputName,
        Description = HubotConsumerResources.EnqueueTeamRoomMessageToHubotAction_QueueNameInputDescription,
        InputMode = InputMode.Combo,
        Id = "queueName",
        IsConfidential = false,
        UseInDefaultDescription = true,
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
          "connectionString"
        },
        HasDynamicValueInformation = true
      }
    };

    public override InputValues GetInputValues(
      IVssRequestContext requestContext,
      string inputId,
      IDictionary<string, string> currentConsumerInputValues)
    {
      string connectionString;
      if (inputId != "queueName" || !currentConsumerInputValues.TryGetValue("connectionString", out connectionString) || string.IsNullOrEmpty(connectionString))
        return base.GetInputValues(requestContext, inputId, currentConsumerInputValues);
      List<InputValue> inputValueList = new List<InputValue>();
      InputValues inputValues = new InputValues()
      {
        InputId = "queueName",
        DefaultValue = string.Empty,
        PossibleValues = (IList<InputValue>) inputValueList,
        IsLimitedToPossibleValues = false,
        IsDisabled = false,
        IsReadOnly = false
      };
      string str;
      try
      {
        foreach (QueueDescription queue in this.CreateNamespaceManagerFromConnectionString(connectionString).GetQueues())
          inputValueList.Add(new InputValue()
          {
            Value = queue.Path,
            DisplayValue = queue.Path
          });
        return inputValues;
      }
      catch (ConfigurationErrorsException ex)
      {
        str = string.Format(HubotConsumerResources.EnqueueTeamRoomMessageToHubotAction_QueryError_SuppliedConnectionStringNotWellFormed, (object) ex.BareMessage);
      }
      catch (MessagingException ex)
      {
        str = string.Format(HubotConsumerResources.EnqueueTeamRoomMessageToHubotAction_QueryError_MessagingExceptionFormat, (object) ex.Message);
      }
      catch (UnauthorizedAccessException ex)
      {
        str = HubotConsumerResources.EnqueueTeamRoomMessageToHubotAction_QueryError_SuppliedConnectionStringNotAuthorized;
      }
      catch (Exception ex)
      {
        str = string.Format(HubotConsumerResources.EnqueueTeamRoomMessageToHubotAction_QueryError_ExceptionFormat, (object) ex.Message);
      }
      inputValues.Error = new InputValuesError()
      {
        Message = str
      };
      return inputValues;
    }

    public ActionTask HandleEvent(
      IVssRequestContext requestContext,
      Event raisedEvent,
      HandleEventArgs eventArgs)
    {
      return (ActionTask) new ServiceBusQueueSendActionTask(eventArgs.Notification.GetConsumerInput("connectionString", true), eventArgs.Notification.GetConsumerInput("queueName", true), HubotConsumer.BuildMessagePayloadToSend(raisedEvent), true);
    }

    internal Func<string, ServiceBusNamespaceManagerWrapper> CreateNamespaceManagerFunc { set; private get; }

    private ServiceBusNamespaceManagerWrapper CreateNamespaceManagerFromConnectionString(
      string connectionString)
    {
      return this.CreateNamespaceManagerFunc != null ? this.CreateNamespaceManagerFunc(connectionString) : ServiceBusNamespaceManagerWrapper.CreateFromConnectionString(connectionString);
    }
  }
}
