// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Azure.TopicClientWrapper
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Consumers, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5BEF54E7-7304-4071-B5F1-22428BB21801
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ServiceHooks.Consumers.dll

using Microsoft.ServiceBus.Messaging;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Azure
{
  public class TopicClientWrapper
  {
    private TopicClient m_topicClient;

    public TopicClientWrapper()
    {
    }

    public TopicClientWrapper(TopicClient topicClient) => this.m_topicClient = topicClient;

    public virtual bool IsClosed => this.m_topicClient.IsClosed;

    public virtual Task SendAsync(BrokeredMessage message) => this.m_topicClient.SendAsync(message);

    public virtual void Close() => this.m_topicClient.Close();
  }
}
