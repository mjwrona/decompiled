// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Azure.ServiceBusActionBase
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
using System.Configuration;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Azure
{
  public abstract class ServiceBusActionBase : EventTransformerConsumerActionImplementation
  {
    private const int c_queryTimeoutInSeconds = 30;
    public const string BypassSerizalierInputId = "bypassSerializer";
    private static readonly string[] s_supportedEventTypes = new string[1]
    {
      "*"
    };
    private static readonly IDictionary<string, string[]> s_supportedResourceVersions = (IDictionary<string, string[]>) new Dictionary<string, string[]>();
    private static readonly TimeSpan s_defaultQueryTimeout = TimeSpan.FromSeconds(30.0);
    private readonly Dictionary<string, Func<string, InputValues>> m_CommonInputsChangedHandlers = new Dictionary<string, Func<string, InputValues>>();
    private readonly Dictionary<string, InputValues> m_CommonInputsChangedHandlerDefaultValues = new Dictionary<string, InputValues>();

    public override string ConsumerId => AzureServiceBusConsumer.ConsumerId;

    public override string[] SupportedEventTypes => ServiceBusActionBase.s_supportedEventTypes;

    public override IDictionary<string, string[]> SupportedResourceVersions => ServiceBusActionBase.s_supportedResourceVersions;

    public override bool AllowResourceVersionOverride => true;

    public override InputValues GetInputValues(
      IVssRequestContext requestContext,
      string inputId,
      IDictionary<string, string> currentConsumerInputValues)
    {
      if (!this.m_CommonInputsChangedHandlers.ContainsKey(inputId))
        return base.GetInputValues(requestContext, inputId, currentConsumerInputValues);
      InputValues handlerDefaultValue = this.m_CommonInputsChangedHandlerDefaultValues[inputId];
      string str1 = AzureServiceBusConsumer.BuildConnectionStringFromInputValues(currentConsumerInputValues);
      if (str1 == null)
        return handlerDefaultValue;
      Func<string, InputValues> inputsChangedHandler = this.m_CommonInputsChangedHandlers[inputId];
      string str2;
      try
      {
        return inputsChangedHandler(str1);
      }
      catch (ConfigurationErrorsException ex)
      {
        str2 = string.Format(AzureConsumerResources.ServiceBusAction_QueryError_SuppliedConnectionStringNotWellFormed, (object) ex.BareMessage);
      }
      catch (MessagingException ex)
      {
        str2 = string.Format(AzureConsumerResources.ServiceBusAction_QueryError_MessagingExceptionFormat, (object) ex.Message);
      }
      catch (UnauthorizedAccessException ex)
      {
        str2 = AzureConsumerResources.ServiceBusAction_QueryError_SuppliedConnectionStringNotAuthorized;
      }
      catch (Exception ex)
      {
        str2 = string.Format(AzureConsumerResources.ServiceBusAction_QueryError_ExceptionFormat, (object) ex.Message);
      }
      handlerDefaultValue.Error = new InputValuesError()
      {
        Message = str2
      };
      return handlerDefaultValue;
    }

    protected void RegisterCommonInputsChangedCallback(
      string inputId,
      Func<string, InputValues> callback,
      InputValues defaultValue)
    {
      this.m_CommonInputsChangedHandlers.Add(inputId, callback);
      this.m_CommonInputsChangedHandlerDefaultValues.Add(inputId, defaultValue);
    }

    protected static string GetMessageContent(
      Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Notification notification,
      Event raisedEvent,
      EventResourceDetails resourceDetailsToSend,
      EventMessages messagesToSend,
      EventMessages detailedMessagesToSend)
    {
      return EventTransformerConsumerActionImplementation.GetStringRepresentation(EventTransformerConsumerActionImplementation.TransformEvent(notification, raisedEvent, resourceDetailsToSend, messagesToSend, detailedMessagesToSend));
    }

    protected ServiceBusNamespaceManagerWrapper CreateServiceBusNamespaceManagerForQueries(
      string sbConnectionString)
    {
      ServiceBusNamespaceManagerWrapper namespaceManager = this.CreateServiceBusNamespaceManager(sbConnectionString);
      namespaceManager.Settings.RetryPolicy = Microsoft.ServiceBus.RetryPolicy.NoRetry;
      namespaceManager.Settings.OperationTimeout = ServiceBusActionBase.s_defaultQueryTimeout;
      return namespaceManager;
    }

    protected NotificationHubNamespaceManagerWrapper CreateNotificationHubNamespaceManagerForQueries(
      string sbConnectionString)
    {
      NotificationHubNamespaceManagerWrapper namespaceManager = this.CreateNotificationHubNamespaceManager(sbConnectionString);
      namespaceManager.Settings.RetryPolicy = Microsoft.Azure.NotificationHubs.RetryPolicy.NoRetry;
      namespaceManager.Settings.OperationTimeout = ServiceBusActionBase.s_defaultQueryTimeout;
      return namespaceManager;
    }

    protected static InputDescriptor BuildBypassSerializerInputDescriptor() => new InputDescriptor()
    {
      Name = AzureConsumerResources.ServiceBusAction_BypassSerializerInputName,
      Description = AzureConsumerResources.ServiceBusAction_BypassSerializerInputDescription,
      InputMode = InputMode.CheckBox,
      Id = "bypassSerializer",
      IsConfidential = false,
      Validation = new InputValidation()
      {
        IsRequired = false,
        DataType = InputDataType.Boolean
      }
    };

    protected static bool GetBypassSerializer(HandleEventArgs eventArgs)
    {
      string consumerInput = eventArgs.Notification.GetConsumerInput("bypassSerializer");
      bool flag = false;
      ref bool local = ref flag;
      return bool.TryParse(consumerInput, out local) && flag;
    }

    internal Func<string, ServiceBusNamespaceManagerWrapper> CreateServiceBusNamespaceManagerFunc { set; private get; }

    internal Func<string, NotificationHubNamespaceManagerWrapper> CreateNotificationHubNamespaceManagerFunc { set; private get; }

    private ServiceBusNamespaceManagerWrapper CreateServiceBusNamespaceManager(
      string connectionString)
    {
      return this.CreateServiceBusNamespaceManagerFunc != null ? this.CreateServiceBusNamespaceManagerFunc(connectionString) : ServiceBusNamespaceManagerWrapper.CreateFromConnectionString(connectionString);
    }

    private NotificationHubNamespaceManagerWrapper CreateNotificationHubNamespaceManager(
      string connectionString)
    {
      return this.CreateNotificationHubNamespaceManagerFunc != null ? this.CreateNotificationHubNamespaceManagerFunc(connectionString) : NotificationHubNamespaceManagerWrapper.CreateFromConnectionString(connectionString);
    }
  }
}
