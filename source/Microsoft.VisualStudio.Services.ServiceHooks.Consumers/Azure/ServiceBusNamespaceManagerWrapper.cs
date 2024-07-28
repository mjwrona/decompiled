// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Azure.ServiceBusNamespaceManagerWrapper
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Consumers, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5BEF54E7-7304-4071-B5F1-22428BB21801
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ServiceHooks.Consumers.dll

using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Azure
{
  public class ServiceBusNamespaceManagerWrapper
  {
    private readonly NamespaceManager m_namespaceManager;

    public ServiceBusNamespaceManagerWrapper()
    {
    }

    public ServiceBusNamespaceManagerWrapper(NamespaceManager namespaceManager) => this.m_namespaceManager = namespaceManager;

    public static ServiceBusNamespaceManagerWrapper CreateFromConnectionString(
      string connectionString)
    {
      return new ServiceBusNamespaceManagerWrapper(NamespaceManager.CreateFromConnectionString(connectionString));
    }

    public virtual NamespaceManagerSettings Settings => this.m_namespaceManager.Settings;

    public virtual Uri Address => this.m_namespaceManager.Address;

    public virtual IEnumerable<QueueDescription> GetQueues() => this.m_namespaceManager.GetQueues();

    public virtual IEnumerable<TopicDescription> GetTopics() => this.m_namespaceManager.GetTopics();

    public virtual TopicDescription CreateTopic(string path) => this.m_namespaceManager.CreateTopic(path);

    public virtual QueueDescription CreateQueue(string path) => this.m_namespaceManager.CreateQueue(path);
  }
}
