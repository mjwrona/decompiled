// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.NamespaceManagerSettings
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.ServiceBus;

namespace Microsoft.VisualStudio.Services.Cloud
{
  internal class NamespaceManagerSettings
  {
    private NamespaceManager m_namespaceManager;

    public NamespaceManagerSettings(
      string ns,
      MessageBusCredentials messageBusCredentials,
      bool prefixMachineName,
      string hostnamePrefix)
    {
      this.Namespace = ns;
      this.PrefixMachineName = prefixMachineName;
      this.HostnamePrefix = hostnamePrefix;
      this.MessageBusCredentials = messageBusCredentials;
    }

    public NamespaceManager GetNamespaceManager()
    {
      if (this.m_namespaceManager == null)
        this.m_namespaceManager = new NamespaceManager(ServiceBusEnvironment.CreateServiceUri("sb", this.Namespace, string.Empty), this.MessageBusCredentials.CreateTokenProvider());
      return this.m_namespaceManager;
    }

    public string Namespace { get; private set; }

    public bool PrefixMachineName { get; private set; }

    public string HostnamePrefix { get; private set; }

    public MessageBusCredentials MessageBusCredentials { get; private set; }
  }
}
