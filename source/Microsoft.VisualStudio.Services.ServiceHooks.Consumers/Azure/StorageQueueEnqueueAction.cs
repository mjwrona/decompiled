// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Azure.StorageQueueEnqueueAction
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Consumers, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5BEF54E7-7304-4071-B5F1-22428BB21801
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ServiceHooks.Consumers.dll

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
  public sealed class StorageQueueEnqueueAction : EventTransformerConsumerActionImplementation
  {
    private const string c_id = "enqueue";
    private const bool c_useHttps = true;
    private const int c_queueNameInputLengthMin = 3;
    private const int c_queueNameInputLengthMax = 63;
    private const string c_queueNameInputPattern = "^[a-z0-9]+[a-z0-9-]+[a-z0-9]$";
    private const int c_messageTtlInputSecondsMin = 1;
    private const int c_messageTtlInputSecondsMax = 604800;
    private const int c_messageVisiTimeoutInputSecondsMin = 0;
    private const int c_messageVisiTimeoutInputSecondsMax = 604800;
    public const string InputIdMessageTtl = "ttl";
    public const string InputIdQueueName = "queueName";
    public const string InputIdAccountKey = "accountKey";
    public const string InputIdVisibilityTimeout = "visiTimeout";
    public const string RegistryPathOverriddenConnectionString = "/Service/ServiceHooks/AzureStorageConsumer/StorageQueueEnqueueAction/OverriddenConnectionString";
    private static readonly string[] s_supportedEventTypes = new string[1]
    {
      "*"
    };
    private static readonly IDictionary<string, string[]> s_supportedResourceVersions = (IDictionary<string, string[]>) new Dictionary<string, string[]>();

    public static string ConsumerActionId => "enqueue";

    public static string QueueNameInputId => "queueName";

    public static string MessageVisibilityTimeoutInputId => "visiTimeout";

    public static string MessageTimeToLiveInputId => "ttl";

    public override string Id => "enqueue";

    public override string ConsumerId => AzureStorageConsumer.ConsumerId;

    public override string Name => AzureConsumerResources.StorageQueueEnqueueActionName;

    public override string Description => AzureConsumerResources.StorageQueueEnqueueActionDescription;

    public override string[] SupportedEventTypes => StorageQueueEnqueueAction.s_supportedEventTypes;

    public override IDictionary<string, string[]> SupportedResourceVersions => StorageQueueEnqueueAction.s_supportedResourceVersions;

    public override bool AllowResourceVersionOverride => true;

    public override IList<InputDescriptor> InputDescriptors => (IList<InputDescriptor>) new List<InputDescriptor>()
    {
      new InputDescriptor()
      {
        Name = AzureConsumerResources.StorageQueueEnqueueAction_QueueNameInputName,
        Description = AzureConsumerResources.StorageQueueEnqueueAction_QueueNameInputDescription,
        InputMode = InputMode.TextBox,
        Id = "queueName",
        IsConfidential = false,
        Validation = new InputValidation()
        {
          IsRequired = true,
          DataType = InputDataType.String,
          MinLength = new int?(3),
          MaxLength = new int?(63),
          Pattern = "^[a-z0-9]+[a-z0-9-]+[a-z0-9]$"
        }
      },
      new InputDescriptor()
      {
        Name = AzureConsumerResources.StorageQueueEnqueueAction_VisibilityTimeoutInputName,
        Description = AzureConsumerResources.StorageQueueEnqueueAction_VisibilityTimeoutInputDescription,
        InputMode = InputMode.TextBox,
        Id = "visiTimeout",
        IsConfidential = false,
        Validation = new InputValidation()
        {
          DataType = InputDataType.Number,
          IsRequired = true,
          MinValue = new Decimal?(0M),
          MaxValue = new Decimal?((Decimal) 604800)
        },
        Values = new InputValues()
        {
          DefaultValue = 0.ToString()
        }
      },
      new InputDescriptor()
      {
        Name = AzureConsumerResources.StorageQueueEnqueueAction_MessageTtlInputName,
        Description = AzureConsumerResources.StorageQueueEnqueueAction_MessageTtlInputDescription,
        InputMode = InputMode.TextBox,
        Id = "ttl",
        IsConfidential = false,
        Validation = new InputValidation()
        {
          DataType = InputDataType.Number,
          IsRequired = true,
          MinValue = new Decimal?((Decimal) 1),
          MaxValue = new Decimal?((Decimal) 604800)
        },
        Values = new InputValues()
        {
          DefaultValue = 604800.ToString()
        }
      }
    }.Union<InputDescriptor>(EventTransformerConsumerActionImplementation.BuildAllPayloadControllersInputDescriptors()).ToList<InputDescriptor>();

    public override string GetActionDescription(
      IVssRequestContext requestContext,
      IDictionary<string, string> consumerInputValues)
    {
      string str1 = (string) null;
      string str2 = (string) null;
      if (consumerInputValues != null)
      {
        consumerInputValues.TryGetValue("accountName", out str1);
        consumerInputValues.TryGetValue("queueName", out str2);
      }
      if (string.IsNullOrEmpty(str1) || string.IsNullOrEmpty(str2))
        throw new ArgumentException();
      return string.Format(AzureConsumerResources.ActionDescription_StorageQueueFormat, (object) str1, (object) str2);
    }

    public ActionTask HandleEvent(
      IVssRequestContext requestContext,
      Event raisedEvent,
      HandleEventArgs e)
    {
      CachedRegistryService service = requestContext.GetService<CachedRegistryService>();
      string consumerInput1 = e.Notification.GetConsumerInput("queueName", true);
      string consumerInput2 = e.Notification.GetConsumerInput("ttl", true);
      string consumerInput3 = e.Notification.GetConsumerInput("accountKey", true);
      string consumerInput4 = e.Notification.GetConsumerInput("accountName", true);
      string consumerInput5 = e.Notification.GetConsumerInput("visiTimeout", true);
      EventResourceDetails resourceDetailsToSend = this.GetDefaultResourceDetailsToSend(e.Notification);
      EventMessages defaultMessagesToSend = this.GetDefaultMessagesToSend(e.Notification);
      EventMessages detailedMessagesToSend = this.GetDefaultDetailedMessagesToSend(e.Notification);
      return (ActionTask) new StorageQueueEnqueueActionTask(EventTransformerConsumerActionImplementation.GetStringRepresentation(EventTransformerConsumerActionImplementation.TransformEvent(e.Notification, raisedEvent, resourceDetailsToSend, defaultMessagesToSend, detailedMessagesToSend)), consumerInput4, consumerInput3, consumerInput1, true, Convert.ToInt32(consumerInput2), Convert.ToInt32(consumerInput5), service.GetValue<string>(requestContext, (RegistryQuery) "/Service/ServiceHooks/AzureStorageConsumer/StorageQueueEnqueueAction/OverriddenConnectionString", true, (string) null));
    }
  }
}
