// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Azure.NotificationHubNamespaceManagerWrapper
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Consumers, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5BEF54E7-7304-4071-B5F1-22428BB21801
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ServiceHooks.Consumers.dll

using Microsoft.Azure.NotificationHubs;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Azure
{
  public class NotificationHubNamespaceManagerWrapper
  {
    private readonly NamespaceManager m_namespaceManager;

    public NotificationHubNamespaceManagerWrapper()
    {
    }

    public NotificationHubNamespaceManagerWrapper(NamespaceManager namespaceManager) => this.m_namespaceManager = namespaceManager;

    public static NotificationHubNamespaceManagerWrapper CreateFromConnectionString(
      string connectionString)
    {
      return new NotificationHubNamespaceManagerWrapper(NamespaceManager.CreateFromConnectionString(connectionString));
    }

    public virtual NamespaceManagerSettings Settings => this.m_namespaceManager.Settings;

    public virtual Uri Address => this.m_namespaceManager.Address;

    public virtual IEnumerable<NotificationHubDescription> GetNotificationHubs() => this.m_namespaceManager.GetNotificationHubs();
  }
}
