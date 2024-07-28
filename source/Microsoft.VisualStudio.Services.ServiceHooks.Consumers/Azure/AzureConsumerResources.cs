// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Azure.AzureConsumerResources
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Consumers, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5BEF54E7-7304-4071-B5F1-22428BB21801
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ServiceHooks.Consumers.dll

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Azure
{
  [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
  [DebuggerNonUserCode]
  [CompilerGenerated]
  internal class AzureConsumerResources
  {
    private static ResourceManager resourceMan;
    private static CultureInfo resourceCulture;

    internal AzureConsumerResources()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static ResourceManager ResourceManager
    {
      get
      {
        if (AzureConsumerResources.resourceMan == null)
          AzureConsumerResources.resourceMan = new ResourceManager("Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Azure.AzureConsumerResources", typeof (AzureConsumerResources).Assembly);
        return AzureConsumerResources.resourceMan;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static CultureInfo Culture
    {
      get => AzureConsumerResources.resourceCulture;
      set => AzureConsumerResources.resourceCulture = value;
    }

    internal static string ActionDescription_BusHubFormat => AzureConsumerResources.ResourceManager.GetString(nameof (ActionDescription_BusHubFormat), AzureConsumerResources.resourceCulture);

    internal static string ActionDescription_BusQueueFormat => AzureConsumerResources.ResourceManager.GetString(nameof (ActionDescription_BusQueueFormat), AzureConsumerResources.resourceCulture);

    internal static string ActionDescription_BusTopicFormat => AzureConsumerResources.ResourceManager.GetString(nameof (ActionDescription_BusTopicFormat), AzureConsumerResources.resourceCulture);

    internal static string ActionDescription_StorageQueueFormat => AzureConsumerResources.ResourceManager.GetString(nameof (ActionDescription_StorageQueueFormat), AzureConsumerResources.resourceCulture);

    internal static string AppServiceConsumerDescription => AzureConsumerResources.ResourceManager.GetString(nameof (AppServiceConsumerDescription), AzureConsumerResources.resourceCulture);

    internal static string AppServiceConsumerName => AzureConsumerResources.ResourceManager.GetString(nameof (AppServiceConsumerName), AzureConsumerResources.resourceCulture);

    internal static string AppServiceDeployWebAppActionDescription => AzureConsumerResources.ResourceManager.GetString(nameof (AppServiceDeployWebAppActionDescription), AzureConsumerResources.resourceCulture);

    internal static string AppServiceDeployWebAppActionName => AzureConsumerResources.ResourceManager.GetString(nameof (AppServiceDeployWebAppActionName), AzureConsumerResources.resourceCulture);

    internal static string AppServiceDeployWebAppTestTokenName => AzureConsumerResources.ResourceManager.GetString(nameof (AppServiceDeployWebAppTestTokenName), AzureConsumerResources.resourceCulture);

    internal static string AppServiceDeployWebAppTokenName => AzureConsumerResources.ResourceManager.GetString(nameof (AppServiceDeployWebAppTokenName), AzureConsumerResources.resourceCulture);

    internal static string Response_Error => AzureConsumerResources.ResourceManager.GetString(nameof (Response_Error), AzureConsumerResources.resourceCulture);

    internal static string Response_OK => AzureConsumerResources.ResourceManager.GetString(nameof (Response_OK), AzureConsumerResources.resourceCulture);

    internal static string ServiceBus_Response_Error_EndpointNotFound => AzureConsumerResources.ResourceManager.GetString(nameof (ServiceBus_Response_Error_EndpointNotFound), AzureConsumerResources.resourceCulture);

    internal static string ServiceBus_Response_Error_Management_Unauthorized => AzureConsumerResources.ResourceManager.GetString(nameof (ServiceBus_Response_Error_Management_Unauthorized), AzureConsumerResources.resourceCulture);

    internal static string ServiceBus_Response_Error_Timeout => AzureConsumerResources.ResourceManager.GetString(nameof (ServiceBus_Response_Error_Timeout), AzureConsumerResources.resourceCulture);

    internal static string ServiceBus_Response_Error_Unauthorized => AzureConsumerResources.ResourceManager.GetString(nameof (ServiceBus_Response_Error_Unauthorized), AzureConsumerResources.resourceCulture);

    internal static string ServiceBusAction_BypassSerializerInputDescription => AzureConsumerResources.ResourceManager.GetString(nameof (ServiceBusAction_BypassSerializerInputDescription), AzureConsumerResources.resourceCulture);

    internal static string ServiceBusAction_BypassSerializerInputName => AzureConsumerResources.ResourceManager.GetString(nameof (ServiceBusAction_BypassSerializerInputName), AzureConsumerResources.resourceCulture);

    internal static string ServiceBusAction_QueryError_ExceptionFormat => AzureConsumerResources.ResourceManager.GetString(nameof (ServiceBusAction_QueryError_ExceptionFormat), AzureConsumerResources.resourceCulture);

    internal static string ServiceBusAction_QueryError_MessagingExceptionFormat => AzureConsumerResources.ResourceManager.GetString(nameof (ServiceBusAction_QueryError_MessagingExceptionFormat), AzureConsumerResources.resourceCulture);

    internal static string ServiceBusAction_QueryError_SuppliedConnectionStringNotAuthorized => AzureConsumerResources.ResourceManager.GetString(nameof (ServiceBusAction_QueryError_SuppliedConnectionStringNotAuthorized), AzureConsumerResources.resourceCulture);

    internal static string ServiceBusAction_QueryError_SuppliedConnectionStringNotWellFormed => AzureConsumerResources.ResourceManager.GetString(nameof (ServiceBusAction_QueryError_SuppliedConnectionStringNotWellFormed), AzureConsumerResources.resourceCulture);

    internal static string ServiceBusConsumer_ConnectionStringInputDescription => AzureConsumerResources.ResourceManager.GetString(nameof (ServiceBusConsumer_ConnectionStringInputDescription), AzureConsumerResources.resourceCulture);

    internal static string ServiceBusConsumer_ConnectionStringInputName => AzureConsumerResources.ResourceManager.GetString(nameof (ServiceBusConsumer_ConnectionStringInputName), AzureConsumerResources.resourceCulture);

    internal static string ServiceBusConsumerDescription => AzureConsumerResources.ResourceManager.GetString(nameof (ServiceBusConsumerDescription), AzureConsumerResources.resourceCulture);

    internal static string ServiceBusConsumerName => AzureConsumerResources.ResourceManager.GetString(nameof (ServiceBusConsumerName), AzureConsumerResources.resourceCulture);

    internal static string ServiceBusNotificationHubSendAction_NotificationHubNameInputDescription => AzureConsumerResources.ResourceManager.GetString(nameof (ServiceBusNotificationHubSendAction_NotificationHubNameInputDescription), AzureConsumerResources.resourceCulture);

    internal static string ServiceBusNotificationHubSendAction_NotificationHubNameInputName => AzureConsumerResources.ResourceManager.GetString(nameof (ServiceBusNotificationHubSendAction_NotificationHubNameInputName), AzureConsumerResources.resourceCulture);

    internal static string ServiceBusNotificationHubSendAction_Response_Ok_Template => AzureConsumerResources.ResourceManager.GetString(nameof (ServiceBusNotificationHubSendAction_Response_Ok_Template), AzureConsumerResources.resourceCulture);

    internal static string ServiceBusNotificationHubSendAction_TagsExpressionInputDescription => AzureConsumerResources.ResourceManager.GetString(nameof (ServiceBusNotificationHubSendAction_TagsExpressionInputDescription), AzureConsumerResources.resourceCulture);

    internal static string ServiceBusNotificationHubSendAction_TagsExpressionInputName => AzureConsumerResources.ResourceManager.GetString(nameof (ServiceBusNotificationHubSendAction_TagsExpressionInputName), AzureConsumerResources.resourceCulture);

    internal static string ServiceBusNotificationHubSendAction_TagsExpressionInputPatternErrorMessage => AzureConsumerResources.ResourceManager.GetString(nameof (ServiceBusNotificationHubSendAction_TagsExpressionInputPatternErrorMessage), AzureConsumerResources.resourceCulture);

    internal static string ServiceBusNotificationHubSendActionDescription => AzureConsumerResources.ResourceManager.GetString(nameof (ServiceBusNotificationHubSendActionDescription), AzureConsumerResources.resourceCulture);

    internal static string ServiceBusNotificationHubSendActionName => AzureConsumerResources.ResourceManager.GetString(nameof (ServiceBusNotificationHubSendActionName), AzureConsumerResources.resourceCulture);

    internal static string ServiceBusQueueMessageTooLong => AzureConsumerResources.ResourceManager.GetString(nameof (ServiceBusQueueMessageTooLong), AzureConsumerResources.resourceCulture);

    internal static string ServiceBusQueueSendAction_QueueNameInputDescription => AzureConsumerResources.ResourceManager.GetString(nameof (ServiceBusQueueSendAction_QueueNameInputDescription), AzureConsumerResources.resourceCulture);

    internal static string ServiceBusQueueSendAction_QueueNameInputName => AzureConsumerResources.ResourceManager.GetString(nameof (ServiceBusQueueSendAction_QueueNameInputName), AzureConsumerResources.resourceCulture);

    internal static string ServiceBusQueueSendActionDescription => AzureConsumerResources.ResourceManager.GetString(nameof (ServiceBusQueueSendActionDescription), AzureConsumerResources.resourceCulture);

    internal static string ServiceBusQueueSendActionName => AzureConsumerResources.ResourceManager.GetString(nameof (ServiceBusQueueSendActionName), AzureConsumerResources.resourceCulture);

    internal static string ServiceBusTopicSendAction_TopicNameInputDescription => AzureConsumerResources.ResourceManager.GetString(nameof (ServiceBusTopicSendAction_TopicNameInputDescription), AzureConsumerResources.resourceCulture);

    internal static string ServiceBusTopicSendAction_TopicNameInputName => AzureConsumerResources.ResourceManager.GetString(nameof (ServiceBusTopicSendAction_TopicNameInputName), AzureConsumerResources.resourceCulture);

    internal static string ServiceBusTopicSendActionDescription => AzureConsumerResources.ResourceManager.GetString(nameof (ServiceBusTopicSendActionDescription), AzureConsumerResources.resourceCulture);

    internal static string ServiceBusTopicSendActionName => AzureConsumerResources.ResourceManager.GetString(nameof (ServiceBusTopicSendActionName), AzureConsumerResources.resourceCulture);

    internal static string StorageConsumerDescription => AzureConsumerResources.ResourceManager.GetString(nameof (StorageConsumerDescription), AzureConsumerResources.resourceCulture);

    internal static string StorageConsumerName => AzureConsumerResources.ResourceManager.GetString(nameof (StorageConsumerName), AzureConsumerResources.resourceCulture);

    internal static string StorageQueueEnqueueAction_AccountKeyInputDescription => AzureConsumerResources.ResourceManager.GetString(nameof (StorageQueueEnqueueAction_AccountKeyInputDescription), AzureConsumerResources.resourceCulture);

    internal static string StorageQueueEnqueueAction_AccountKeyInputName => AzureConsumerResources.ResourceManager.GetString(nameof (StorageQueueEnqueueAction_AccountKeyInputName), AzureConsumerResources.resourceCulture);

    internal static string StorageQueueEnqueueAction_AccountNameInputDescription => AzureConsumerResources.ResourceManager.GetString(nameof (StorageQueueEnqueueAction_AccountNameInputDescription), AzureConsumerResources.resourceCulture);

    internal static string StorageQueueEnqueueAction_AccountNameInputName => AzureConsumerResources.ResourceManager.GetString(nameof (StorageQueueEnqueueAction_AccountNameInputName), AzureConsumerResources.resourceCulture);

    internal static string StorageQueueEnqueueAction_MessageTtlInputDescription => AzureConsumerResources.ResourceManager.GetString(nameof (StorageQueueEnqueueAction_MessageTtlInputDescription), AzureConsumerResources.resourceCulture);

    internal static string StorageQueueEnqueueAction_MessageTtlInputName => AzureConsumerResources.ResourceManager.GetString(nameof (StorageQueueEnqueueAction_MessageTtlInputName), AzureConsumerResources.resourceCulture);

    internal static string StorageQueueEnqueueAction_QueueNameInputDescription => AzureConsumerResources.ResourceManager.GetString(nameof (StorageQueueEnqueueAction_QueueNameInputDescription), AzureConsumerResources.resourceCulture);

    internal static string StorageQueueEnqueueAction_QueueNameInputName => AzureConsumerResources.ResourceManager.GetString(nameof (StorageQueueEnqueueAction_QueueNameInputName), AzureConsumerResources.resourceCulture);

    internal static string StorageQueueEnqueueAction_VisibilityTimeoutInputDescription => AzureConsumerResources.ResourceManager.GetString(nameof (StorageQueueEnqueueAction_VisibilityTimeoutInputDescription), AzureConsumerResources.resourceCulture);

    internal static string StorageQueueEnqueueAction_VisibilityTimeoutInputName => AzureConsumerResources.ResourceManager.GetString(nameof (StorageQueueEnqueueAction_VisibilityTimeoutInputName), AzureConsumerResources.resourceCulture);

    internal static string StorageQueueEnqueueActionDescription => AzureConsumerResources.ResourceManager.GetString(nameof (StorageQueueEnqueueActionDescription), AzureConsumerResources.resourceCulture);

    internal static string StorageQueueEnqueueActionName => AzureConsumerResources.ResourceManager.GetString(nameof (StorageQueueEnqueueActionName), AzureConsumerResources.resourceCulture);
  }
}
