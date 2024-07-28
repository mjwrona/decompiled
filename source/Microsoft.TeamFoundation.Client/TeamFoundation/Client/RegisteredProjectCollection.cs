// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.RegisteredProjectCollection
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using System;

namespace Microsoft.TeamFoundation.Client
{
  public sealed class RegisteredProjectCollection
  {
    private bool m_offline;
    private bool m_autoReconnect;

    internal RegisteredProjectCollection(RegisteredTfsConnections.ServerInfo info)
    {
      this.Uri = info.Uri;
      this.Name = info.Name;
      this.DisplayName = info.DisplayName ?? info.Name;
      this.m_offline = info.Offline;
      this.m_autoReconnect = info.AutoReconnect;
      this.RegistryKeyName = info.RegistryKeyName;
      this.InstanceId = info.InstanceId;
    }

    public string Name { get; private set; }

    public string DisplayName { get; private set; }

    public Uri Uri { get; private set; }

    public Guid InstanceId { get; private set; }

    public bool Offline
    {
      get => this.m_offline;
      set
      {
        RegisteredTfsConnections.SetOfflineInternal(this.RegistryKeyName, value);
        this.m_offline = value;
      }
    }

    public bool AutoReconnect
    {
      get => this.m_autoReconnect;
      set
      {
        RegisteredTfsConnections.SetAutoReconnectInternal(this.RegistryKeyName, value);
        this.m_autoReconnect = value;
      }
    }

    internal string RegistryKeyName { get; private set; }
  }
}
