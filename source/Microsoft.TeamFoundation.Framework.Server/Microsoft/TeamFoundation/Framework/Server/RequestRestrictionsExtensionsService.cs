// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.RequestRestrictionsExtensionsService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class RequestRestrictionsExtensionsService : 
    IRequestRestrictionsExtensionsService,
    IVssFrameworkService
  {
    private IDisposableReadOnlyList<IPublicRequestRestrictionsAttribute> m_publicRestrictions;
    private const string c_area = "Authorization";
    private const string c_layer = "RequestRestrictionsExtensionsService";

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.TraceEnter(13421801, "Authorization", nameof (RequestRestrictionsExtensionsService), nameof (ServiceStart));
      systemRequestContext.CheckDeploymentRequestContext();
      try
      {
        this.m_publicRestrictions = systemRequestContext.GetExtensions<IPublicRequestRestrictionsAttribute>(ExtensionLifetime.Service);
      }
      finally
      {
        systemRequestContext.TraceLeave(13421802, "Authorization", nameof (RequestRestrictionsExtensionsService), nameof (ServiceStart));
      }
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
      if (this.m_publicRestrictions == null)
        return;
      this.m_publicRestrictions.Dispose();
      this.m_publicRestrictions = (IDisposableReadOnlyList<IPublicRequestRestrictionsAttribute>) null;
    }

    public IEnumerable<IPublicRequestRestrictionsAttribute> GetPublicRestrictions(
      IVssRequestContext requestContext)
    {
      requestContext.CheckDeploymentRequestContext();
      return (IEnumerable<IPublicRequestRestrictionsAttribute>) this.m_publicRestrictions;
    }
  }
}
