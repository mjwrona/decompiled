// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Common.Migration.RequestContextBasedServiceHostAccess
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 203E0171-FB50-4FDE-9B1F-EFC6366423BC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.VisualStudio.Services.Content.Server.Common.Migration
{
  public class RequestContextBasedServiceHostAccess : IServiceHostAccess
  {
    private IVssRequestContext m_rc;
    private IVssRegistryService m_reg;

    public RequestContextBasedServiceHostAccess(IVssRequestContext requestContext)
    {
      this.m_rc = requestContext.To(TeamFoundationHostType.Deployment);
      this.m_reg = this.m_rc.GetService<IVssRegistryService>();
    }

    public bool IsProduction => this.m_rc.ServiceHost.IsProduction;

    public void DeleteRegistries(string[] paths) => this.m_reg.DeleteEntries(this.m_rc, paths);

    public int GetRegistry(string path, int defaultValue) => this.m_reg.GetValue<int>(this.m_rc, (RegistryQuery) path, defaultValue);

    public bool GetRegistry(string path, bool defaultValue) => this.m_reg.GetValue<bool>(this.m_rc, (RegistryQuery) path, defaultValue);

    public void SetRegistry(string path, int value) => this.m_reg.SetValue<int>(this.m_rc, path, value);

    public void SetRegistry(string path, bool value) => this.m_reg.SetValue<bool>(this.m_rc, path, value);
  }
}
