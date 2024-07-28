// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.SshSettings
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.Location.Server;
using System;

namespace Microsoft.TeamFoundation.Git.Server
{
  internal sealed class SshSettings
  {
    private const string c_sshRegistryRoot = "/Configuration/SshServer/";
    private const string c_sshDomainRegistrySetting = "/Configuration/SshServer/SshDomain";
    private const string c_sshEnabledRegistrySetting = "/Configuration/SshServer/Enabled";
    private const string c_sshPortRegistrySetting = "/Configuration/SshServer/Port";
    private static readonly RegistryQuery s_sshRegistryQuery = new RegistryQuery("/Configuration/SshServer/...");

    private SshSettings(string host, int port)
    {
      this.Host = host;
      this.Port = port;
    }

    public static SshSettings TryLoadIfEnabled(IVssRequestContext rc)
    {
      IVssRequestContext vssRequestContext = rc.To(TeamFoundationHostType.Deployment);
      RegistryEntryCollection registryEntryCollection = vssRequestContext.GetService<IVssRegistryService>().ReadEntries(vssRequestContext, SshSettings.s_sshRegistryQuery);
      if (!registryEntryCollection["/Configuration/SshServer/Enabled"].GetValue<bool>(false))
        return (SshSettings) null;
      string host;
      if (rc.ExecutionEnvironment.IsHostedDeployment)
      {
        host = registryEntryCollection["/Configuration/SshServer/SshDomain"].Value;
        if (string.IsNullOrEmpty(host))
          throw new InvalidOperationException("/Configuration/SshServer/SshDomain is required to be set on hosted");
      }
      else
        host = new Uri(vssRequestContext.GetService<ILocationService>().GetLocationServiceUrl(vssRequestContext, Guid.Empty, AccessMappingConstants.ClientAccessMappingMoniker)).Host;
      return new SshSettings(host, registryEntryCollection["/Configuration/SshServer/Port"].GetValue<int>(22));
    }

    public string Host { get; }

    public int Port { get; }
  }
}
