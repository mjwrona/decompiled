// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Azure.ServiceBusTopicSendActionTask
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Consumers, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5BEF54E7-7304-4071-B5F1-22428BB21801
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ServiceHooks.Consumers.dll

using Microsoft.ServiceBus.Messaging;
using System;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Azure
{
  public sealed class ServiceBusTopicSendActionTask : ServiceBusActionTaskBase
  {
    private TopicClientWrapper m_topicClient;

    public string TopicName { get; private set; }

    public bool BypassSerializer { get; private set; }

    public ServiceBusTopicSendActionTask(
      string serviceBusConnectionString,
      string topicName,
      string messageContent,
      bool bypassSerializer = false)
      : base(serviceBusConnectionString, messageContent)
    {
      this.TopicName = topicName;
      this.BypassSerializer = bypassSerializer;
    }

    protected override bool TryCreateEndpointWhenNotFound()
    {
      this.CreateNamespaceManagerFromConnectionString(this.ServiceBusConnectionString).CreateTopic(this.TopicName);
      return true;
    }

    protected override void InitializeAction(TimeSpan timeout)
    {
      ServiceBusNamespaceManagerWrapper connectionString = this.CreateNamespaceManagerFromConnectionString(this.ServiceBusConnectionString);
      MessagingFactorySettings factorySettings = new MessagingFactorySettings()
      {
        OperationTimeout = timeout,
        TokenProvider = connectionString.Settings.TokenProvider
      };
      this.m_topicClient = this.CreateMessagingFactory(connectionString.Address, factorySettings).CreateTopicClient(this.TopicName);
    }

    protected override async Task<string> PerformActionAsync(TimeSpan timeout)
    {
      ServiceBusTopicSendActionTask topicSendActionTask = this;
      await topicSendActionTask.m_topicClient.SendAsync(topicSendActionTask.MessageContent.ToBrokeredMessage(topicSendActionTask.BypassSerializer));
      string str;
      return str;
    }

    protected override void CleanupAction()
    {
      if (this.m_topicClient == null || this.m_topicClient.IsClosed)
        return;
      this.m_topicClient.Close();
    }
  }
}
