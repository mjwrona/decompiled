// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.ConnectedService.Server.IbizaConnectedServiceProviderController
// Assembly: Microsoft.TeamFoundation.ConnectedService.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5BEB400C-7A81-4197-B897-D0116BC50257
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.ConnectedService.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess;
using Microsoft.TeamFoundation.Server.WebAccess.Platform;
using Microsoft.TeamFoundation.Server.WebAccess.Routing;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.ConnectedService.Server
{
  [SupportedRouteArea("", NavigationContextLevels.Deployment)]
  public class IbizaConnectedServiceProviderController : WebPlatformAreaController
  {
    public override string AreaName => "connectedService";

    [HttpGet]
    public ActionResult AuthRequest(string providerType, string redirectUrl)
    {
      ArgumentUtility.CheckForNull<string>(providerType, nameof (providerType));
      ArgumentUtility.CheckForNull<string>(redirectUrl, nameof (redirectUrl));
      return (ActionResult) this.Json((object) this.TfsRequestContext.GetService<IbizaConnectedServiceProviderService>().GetProvider(providerType).GetAuthRequest(this.TfsRequestContext, redirectUrl), JsonRequestBehavior.AllowGet);
    }

    [HttpGet]
    public ActionResult Callback(
      string providerType,
      string code,
      string state,
      string redirectUrl)
    {
      ArgumentUtility.CheckForNull<string>(providerType, nameof (providerType));
      ArgumentUtility.CheckForNull<string>(redirectUrl, nameof (redirectUrl));
      ArgumentUtility.CheckForNull<string>(state, nameof (state));
      if (code == null)
        return (ActionResult) this.Redirect(redirectUrl + (redirectUrl.IndexOf("#") > 0 ? "&" : "#") + "error=access_denied");
      try
      {
        return (ActionResult) this.Redirect(this.TfsRequestContext.GetService<IbizaConnectedServiceProviderService>().GetProvider(providerType).GetRedirectUrlWithToken(this.TfsRequestContext, code, redirectUrl) ?? throw new ApplicationException(ConnectedServiceProviderResources.Error_GeneratingRedirectUrl));
      }
      catch
      {
        throw new ApplicationException(ConnectedServiceProviderResources.Error_GeneratingRedirectUrl);
      }
    }
  }
}
