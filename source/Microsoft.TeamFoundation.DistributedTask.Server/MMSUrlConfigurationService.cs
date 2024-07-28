// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.MMSUrlConfigurationService
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Linq;
using System.Threading;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  public sealed class MMSUrlConfigurationService : IVssFrameworkService
  {
    private string[] m_MMSDeploymentUrls;
    private static readonly string[] MMSDeploymentFallbackUrls = new string[10]
    {
      "https://vstsmms.actions.githubusercontent.com/serviceDeployments/mmsghadocus1",
      "https://vstsmms.actions.githubusercontent.com/serviceDeployments/mmsghadocus2",
      "https://vstsmms.actions.githubusercontent.com/serviceDeployments/mmsghadoeus1",
      "https://vstsmms.actions.githubusercontent.com/serviceDeployments/mmsghadoeus21",
      "https://vstsmms.actions.githubusercontent.com/serviceDeployments/mmsghadoeus22",
      "https://vstsmms.actions.githubusercontent.com/serviceDeployments/mmsghadowus1",
      "https://vstsmms.actions.githubusercontent.com/serviceDeployments/mmsghadowus21",
      "https://vstsmms.actions.githubusercontent.com/serviceDeployments/mmsghadoneu1",
      "https://vstsmms.actions.githubusercontent.com/serviceDeployments/mmsghadoweu1",
      "https://vstsmms.actions.githubusercontent.com/serviceDeployments/mmsghadocfr1"
    };
    private static readonly string MMSDeploymentUrlsRegPath = "/Configuration/Settings/MMSServiceUrls";

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.GetService<IVssRegistryService>().RegisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.OnMMSDeploymentUrlEntriesChanged), MMSUrlConfigurationService.MMSDeploymentUrlsRegPath);
      Interlocked.CompareExchange<string[]>(ref this.m_MMSDeploymentUrls, MMSUrlConfigurationService.GetMMSDeploymentUrlEntries(systemRequestContext, MMSUrlConfigurationService.MMSDeploymentUrlsRegPath), (string[]) null);
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext) => systemRequestContext.GetService<IVssRegistryService>().UnregisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.OnMMSDeploymentUrlEntriesChanged));

    public Uri GetMMSServiceUrl() => new Uri(this.m_MMSDeploymentUrls[new Random(Guid.NewGuid().GetHashCode()).Next(0, this.m_MMSDeploymentUrls.Length)]);

    private void OnMMSDeploymentUrlEntriesChanged(
      IVssRequestContext requestContext,
      RegistryEntryCollection changedEntries)
    {
      if (changedEntries == null || !changedEntries.Any<RegistryEntry>())
        return;
      this.m_MMSDeploymentUrls = MMSUrlConfigurationService.GetMMSDeploymentUrlEntries(requestContext, MMSUrlConfigurationService.MMSDeploymentUrlsRegPath);
    }

    private static string[] GetMMSDeploymentUrlEntries(
      IVssRequestContext requestContext,
      string registryKey)
    {
      try
      {
        return requestContext.GetService<IVssRegistryService>().GetValue<string>(requestContext, (RegistryQuery) registryKey, true, string.Empty).Split(',');
      }
      catch (Exception ex)
      {
        requestContext.TraceException(280492496, "DistributedTask", nameof (MMSUrlConfigurationService), ex);
        return MMSUrlConfigurationService.MMSDeploymentFallbackUrls;
      }
    }
  }
}
