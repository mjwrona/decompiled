// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Azure.MessagingFactoryWrapper
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Consumers, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5BEF54E7-7304-4071-B5F1-22428BB21801
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ServiceHooks.Consumers.dll

using Microsoft.ServiceBus.Messaging;
using System;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Azure
{
  public class MessagingFactoryWrapper
  {
    private MessagingFactory m_messagingFactory;

    public MessagingFactoryWrapper()
    {
    }

    public MessagingFactoryWrapper(MessagingFactory messagingFactory) => this.m_messagingFactory = messagingFactory;

    public static MessagingFactoryWrapper Create(
      Uri address,
      MessagingFactorySettings factorySettings)
    {
      return new MessagingFactoryWrapper(MessagingFactory.Create(address, factorySettings));
    }

    public virtual TopicClientWrapper CreateTopicClient(string path) => new TopicClientWrapper(this.m_messagingFactory.CreateTopicClient(path));

    public virtual QueueClientWrapper CreateQueueClient(string path) => new QueueClientWrapper(this.m_messagingFactory.CreateQueueClient(path));
  }
}
