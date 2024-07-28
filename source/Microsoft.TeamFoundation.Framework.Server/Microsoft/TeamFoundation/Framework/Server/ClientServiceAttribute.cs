// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ClientServiceAttribute
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
  public sealed class ClientServiceAttribute : Attribute
  {
    private ServerConfiguration m_serverConfiguration = ServerConfiguration.TfsTeamProjectCollection;
    private string m_collectionServiceIdentifier = Guid.Empty.ToString();
    private string m_configurationServiceIdentifer = Guid.Empty.ToString();

    public string ComponentName { get; set; }

    public string RegistrationName { get; set; }

    public string ServiceName { get; set; }

    public string CollectionServiceIdentifier
    {
      get => this.m_collectionServiceIdentifier;
      set => this.m_collectionServiceIdentifier = value;
    }

    public string ConfigurationServiceIdentifier
    {
      get => this.m_configurationServiceIdentifer;
      set => this.m_configurationServiceIdentifer = value;
    }

    public ServerConfiguration ServerConfiguration
    {
      get => this.m_serverConfiguration;
      set => this.m_serverConfiguration = value;
    }
  }
}
