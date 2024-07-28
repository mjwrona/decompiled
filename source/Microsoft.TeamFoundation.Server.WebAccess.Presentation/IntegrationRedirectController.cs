// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Presentation.IntegrationRedirectController
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Presentation, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4ED0029A-5609-48A8-995C-ADAB0E762821
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Presentation.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.Routing;
using Microsoft.VisualStudio.Services.Compliance;
using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.Location.Server;
using System;
using System.Web;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.Presentation
{
  [SupportedRouteArea(NavigationContextLevels.Collection)]
  [DemandFeature("00000000-0000-0000-0000-000000000000", false)]
  public class IntegrationRedirectController : TfsController
  {
    private readonly Guid ChatOpsServiceInstanceId = new Guid("00000067-0000-8888-8000-000000000000");
    private const string TeamsTabsAzDevOAuthFeatureFlag = "AzDevOAuth.TeamsTabs";
    private const string AzChatOpsMultiInstance = "AzChatOps.MultiInstance";
    private const int SilentRedirectEnter = 15161030;
    private const int SilentRedirectLeave = 15161039;
    private const int AuthenticationRedirectEnter = 15161040;
    private const int AuthenticationRedirectLeave = 15161049;
    private const int CompleteEnter = 15161050;
    private const int CompleteLeave = 15161059;
    private const int OAuthCompleteEnter = 15161060;
    private const int OAuthCompleteLeave = 15161069;
    private const int OAuthRedirectException = 15161070;

    [AcceptVerbs(HttpVerbs.Get)]
    [TfsTraceFilter(15161030, 15161039)]
    public ActionResult SilentRedirect(string replyTo) => this.RedirectInternal(replyTo);

    [AcceptVerbs(HttpVerbs.Get)]
    [TfsTraceFilter(15161040, 15161049)]
    public ActionResult AuthenticationRedirect(string replyTo)
    {
      SecureFlowLocation location;
      SecureFlowLocationState secureFlowLocationState = SecureFlowLocation.TryCreate(this.TfsRequestContext, replyTo, SecureFlowLocation.GetDefaultLocation(this.TfsRequestContext), out location);
      if (secureFlowLocationState != SecureFlowLocationState.Valid)
        throw new InvalidRedirectException(secureFlowLocationState);
      AuthenticationRedirectViewModel model = new AuthenticationRedirectViewModel()
      {
        RedirectUrl = location.ToString()
      };
      if (this.TfsRequestContext.IsFeatureEnabled("AzDevOAuth.TeamsTabs"))
      {
        try
        {
          IVssRequestContext vssRequestContext = this.TfsRequestContext.To(TeamFoundationHostType.Deployment);
          ILocationService service = vssRequestContext.GetService<ILocationService>();
          UriBuilder uriBuilder = new UriBuilder(!this.TfsRequestContext.IsFeatureEnabled("AzChatOps.MultiInstance") ? service.GetLocationServiceUrl(vssRequestContext, this.ChatOpsServiceInstanceId, AccessMappingConstants.ClientAccessMappingMoniker) : service.GetLocationServiceUrl(vssRequestContext, this.ChatOpsServiceInstanceId, AccessMappingConstants.AffinitizedMultiInstanceAccessMappingMoniker));
          uriBuilder.Path = uriBuilder.Path.TrimEnd('/') + "/integration/initiatetabcontentoauth";
          uriBuilder.Query = "contentPath=" + HttpUtility.UrlEncode(location.ToString());
          model.IsUsingOAuth = true;
          model.OAuthInitiationUrl = uriBuilder.Uri.AbsoluteUri;
        }
        catch (Exception ex)
        {
          this.TfsRequestContext.TraceException(15161070, nameof (AuthenticationRedirect), this.TraceArea, ex);
        }
      }
      return (ActionResult) this.View((object) model);
    }

    [AcceptVerbs(HttpVerbs.Get)]
    [TfsTraceFilter(15161050, 15161059)]
    public ActionResult Complete() => (ActionResult) this.View((object) new AuthenticationRedirectCompleteViewModel());

    public ActionResult TeamsAuthRedirect(string replyTo) => this.RedirectInternal(replyTo);

    private ActionResult RedirectInternal(string replyTo)
    {
      SecureFlowLocation location;
      int num = (int) SecureFlowLocation.TryCreate(this.TfsRequestContext, replyTo, SecureFlowLocation.GetDefaultLocation(this.TfsRequestContext), out location);
      return (ActionResult) new RedirectResult(location.ToString());
    }
  }
}
