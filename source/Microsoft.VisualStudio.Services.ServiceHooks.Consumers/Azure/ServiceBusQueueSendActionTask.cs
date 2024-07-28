// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Azure.ServiceBusQueueSendActionTask
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Consumers, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5BEF54E7-7304-4071-B5F1-22428BB21801
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ServiceHooks.Consumers.dll

using Microsoft.ServiceBus.Messaging;
using System;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Azure
{
  public sealed class ServiceBusQueueSendActionTask : ServiceBusActionTaskBase
  {
    private QueueClientWrapper m_queueClient;

    public ServiceBusQueueSendActionTask(
      string serviceBusConnectionString,
      string queueName,
      string messageContent,
      bool bypassSerializer = false)
      : base(serviceBusConnectionString, messageContent)
    {
      this.QueueName = queueName;
      this.BypassSerializer = bypassSerializer;
    }

    public string QueueName { get; private set; }

    public bool BypassSerializer { get; private set; }

    protected override void InitializeAction(TimeSpan timeout)
    {
      ServiceBusNamespaceManagerWrapper connectionString = this.CreateNamespaceManagerFromConnectionString(this.ServiceBusConnectionString);
      MessagingFactorySettings factorySettings = new MessagingFactorySettings()
      {
        OperationTimeout = timeout,
        TokenProvider = connectionString.Settings.TokenProvider
      };
      this.m_queueClient = this.CreateMessagingFactory(connectionString.Address, factorySettings).CreateQueueClient(this.QueueName);
    }

    protected override bool TryCreateEndpointWhenNotFound()
    {
      this.CreateNamespaceManagerFromConnectionString(this.ServiceBusConnectionString).CreateQueue(this.QueueName);
      return true;
    }

    protected override async Task<string> PerformActionAsync(TimeSpan timeout)
    {
      ServiceBusQueueSendActionTask queueSendActionTask = this;
      await queueSendActionTask.m_queueClient.SendAsync(queueSendActionTask.MessageContent.ToBrokeredMessage(queueSendActionTask.BypassSerializer));
      string str;
      return str;
    }

    protected override void CleanupAction()
    {
      if (this.m_queueClient == null || this.m_queueClient.IsClosed)
        return;
      this.m_queueClient.Close();
    }
  }
}
