// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.TeamFoundationForwardLinkService
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.TeamFoundation.Server.Core
{
  public class TeamFoundationForwardLinkService : IVssFrameworkService
  {
    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public string GetForwardLink(IVssRequestContext requestContext, string linkName) => requestContext.GetService<CachedRegistryService>().GetValue<string>(requestContext, (RegistryQuery) string.Format("/Configuration/ForwardLink/{0}", (object) linkName), true, string.Empty);

    public void SetForwardLink(IVssRequestContext requestContext, string linkName, string value) => requestContext.GetService<CachedRegistryService>().SetValue<string>(requestContext, string.Format("/Configuration/ForwardLink/{0}", (object) linkName), value);

    public void Redirect(IVssRequestContext requestContext, string linkName, bool endResponse)
    {
      string forwardLink = this.GetForwardLink(requestContext, linkName);
      if (string.IsNullOrEmpty(forwardLink) || !(requestContext is IWebRequestContextInternal requestContextInternal))
        return;
      requestContextInternal.HttpContext.Response.Redirect(forwardLink, endResponse);
    }
  }
}
