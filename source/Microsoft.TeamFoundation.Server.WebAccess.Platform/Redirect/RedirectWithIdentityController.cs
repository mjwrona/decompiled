// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Redirect.RedirectWithIdentityController
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.Platform;
using Microsoft.TeamFoundation.Server.WebAccess.Routing;
using Microsoft.VisualStudio.Services.Compliance;
using Microsoft.VisualStudio.Services.DelegatedAuthorization;
using Microsoft.VisualStudio.Services.Identity.Client;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Threading;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.Redirect
{
  [SupportedRouteArea("", NavigationContextLevels.All)]
  public class RedirectWithIdentityController : WebPlatformController
  {
    private const string s_area = "Redirect";
    private const string s_layer = "RedirectWithIdentity";

    [AcceptVerbs(HttpVerbs.Get)]
    [PublicCollectionRequestRestrictions(false, true, null)]
    public ActionResult Redirect(string target)
    {
      SecureFlowLocation secureFlowLocation = this.ValidateTargetUrl(target);
      if (secureFlowLocation == null)
        return (ActionResult) new HttpStatusCodeResult(HttpStatusCode.BadRequest);
      if (!this.TfsRequestContext.IsFeatureEnabled("VisualStudio.Services.Framework.EnableIdentityNavigation") || this.TfsRequestContext.IsAnonymous())
        return (ActionResult) new RedirectResult(secureFlowLocation.ToString(), false);
      string newValue = secureFlowLocation.GetScheme() + "://" + secureFlowLocation.Host + "/_signedin" + "?reply_to=" + Uri.EscapeDataString(secureFlowLocation.ToString());
      string signedInToken = this.GetSignedInToken(this.TfsRequestContext);
      string str = FrameworkResources.SignedInNavigation().Replace("$$Url$$", newValue).Replace("$$token$$", signedInToken);
      return (ActionResult) new ContentResult()
      {
        Content = str,
        ContentEncoding = Encoding.UTF8,
        ContentType = "text/html"
      };
    }

    protected internal virtual SecureFlowLocation ValidateTargetUrl(string targetUrl)
    {
      Uri url = new Uri(targetUrl);
      SecureFlowLocation location;
      if (SecureFlowLocation.TryCreate(this.TfsRequestContext, url, SecureFlowLocation.GetDefaultLocation(this.TfsRequestContext), out location) == SecureFlowLocationState.Valid)
        return location;
      this.TfsRequestContext.Trace(10002010, TraceLevel.Error, "Redirect", "RedirectWithIdentity", string.Format("Invalid url passed as target url - {0}", (object) url));
      return (SecureFlowLocation) null;
    }

    protected internal virtual string GetSignedInToken(IVssRequestContext requestContext)
    {
      try
      {
        AccessTokenResult accessTokenResult = requestContext.GetClient<IdentityHttpClient>().GetSignedInToken((object) null, new CancellationToken()).SyncResult<AccessTokenResult>();
        if (!accessTokenResult.HasError)
          return accessTokenResult.AccessToken.EncodedToken;
        requestContext.Trace(10002010, TraceLevel.Error, "Redirect", "RedirectWithIdentity", string.Format("Error getting signedin token - {0}", (object) accessTokenResult.AccessTokenError));
        return (string) null;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(10002011, "Redirect", "RedirectWithIdentity", ex);
        return (string) null;
      }
    }
  }
}
