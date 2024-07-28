// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Ssh.Server.Core.SshOptionsService
// Assembly: Microsoft.TeamFoundation.Ssh.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3DF8FBEE-AA1B-4659-8650-E7C7E1E085EB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Ssh.Server.Core.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Threading;

namespace Microsoft.TeamFoundation.Ssh.Server.Core
{
  internal sealed class SshOptionsService : ISshOptionsService, IVssFrameworkService
  {
    private SshOptions m_options;

    void IVssFrameworkService.ServiceStart(IVssRequestContext systemRC)
    {
      systemRC.CheckDeploymentRequestContext();
      systemRC.GetService<IVssRegistryService>().RegisterNotification(systemRC, new RegistrySettingsChangedCallback(this.OnRegistrySettingsChanged), in SshOptions.RegistryQuery);
      Interlocked.CompareExchange<SshOptions>(ref this.m_options, SshOptions.FromRegistry(systemRC), (SshOptions) null);
    }

    void IVssFrameworkService.ServiceEnd(IVssRequestContext systemRequestContext) => systemRequestContext.GetService<IVssRegistryService>().UnregisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.OnRegistrySettingsChanged));

    private void OnRegistrySettingsChanged(
      IVssRequestContext rc,
      RegistryEntryCollection changedEntries)
    {
      Volatile.Write<SshOptions>(ref this.m_options, SshOptions.FromRegistry(rc));
    }

    public SshOptions Options => this.m_options;
  }
}
