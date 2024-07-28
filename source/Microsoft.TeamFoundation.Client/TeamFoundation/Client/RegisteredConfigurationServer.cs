// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.RegisteredConfigurationServer
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using System;

namespace Microsoft.TeamFoundation.Client
{
  public sealed class RegisteredConfigurationServer
  {
    internal RegisteredConfigurationServer(RegisteredTfsConnections.ServerInfo info)
    {
      this.Uri = info.Uri;
      this.Name = info.Name;
      this.RegistryKeyName = info.RegistryKeyName;
      this.InstanceId = info.InstanceId;
      this.IsHosted = info.IsHosted;
    }

    public string Name { get; private set; }

    public Uri Uri { get; private set; }

    public Guid InstanceId { get; private set; }

    public bool? IsHosted { get; private set; }

    internal string RegistryKeyName { get; private set; }
  }
}
