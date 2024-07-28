// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Integration.Server.IntegrationHost
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.TeamFoundation.Integration.Server
{
  public class IntegrationHost : IVssFrameworkService
  {
    private SecuredAuthorizationManager m_securedAuthorizationManager;

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      if (!systemRequestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
        throw new UnexpectedHostTypeException(systemRequestContext.ServiceHost.HostType);
      systemRequestContext.GetService<CachedRegistryService>();
      this.m_securedAuthorizationManager = new SecuredAuthorizationManager(systemRequestContext);
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    internal SecuredAuthorizationManager SecuredAuthorizationManager => this.m_securedAuthorizationManager;
  }
}
