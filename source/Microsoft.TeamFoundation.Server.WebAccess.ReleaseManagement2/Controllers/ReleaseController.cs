// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.ReleaseManagement2.Controllers.ReleaseController
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.ReleaseManagement2, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B374520F-138F-4DCB-BCF6-50FBC8C65346
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.ReleaseManagement2.dll

using Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Authentication;
using Microsoft.TeamFoundation.Server.WebAccess.ContentSecurityPolicy;
using Microsoft.TeamFoundation.Server.WebAccess.Platform;
using Microsoft.TeamFoundation.Server.WebAccess.ReleaseManagement2.Resources;
using Microsoft.TeamFoundation.Server.WebAccess.Routing;
using Microsoft.VisualStudio.Services.Common.Internal;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Services;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.ReleaseManagement2.Controllers
{
  [IncludeCspHeader]
  [SupportedRouteArea("", NavigationContextLevels.Project)]
  public class ReleaseController : WebPlatformAreaController
  {
    private const string SuccessScriptBlock = "open(location, '_self').close();";
    private const string ErrorScriptBlock = "vstsaadoauthcompleted=\"true\";vstsaadoautherrormessage=\"{0}\"";

    public override string AreaName => "ReleaseManagement";

    [AcceptVerbs(HttpVerbs.Get)]
    public ActionResult AadOauthCallback(
      [ClientQueryParameter] string accessTokenKey,
      [ClientQueryParameter] string projectId,
      [ClientQueryParameter] string tenantId,
      [ClientQueryParameter] AuthorizationHeaderFor authorizationFor)
    {
      if (string.IsNullOrEmpty(accessTokenKey))
        return (ActionResult) this.CreateJavaScript(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "vstsaadoauthcompleted=\"true\";vstsaadoautherrormessage=\"{0}\"", (object) Resource.InvalidVstsAccessTokenKey));
      string idToken;
      string nonce;
      string completeCallbackPayload;
      string errorMessage;
      return !VstsAccessTokenHelper.TryGetVstsAccessTokenFromPropertyCache(this.TfsRequestContext, accessTokenKey, out string _, out idToken, out nonce, out completeCallbackPayload, out errorMessage) ? (ActionResult) this.CreateJavaScript(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "vstsaadoauthcompleted=\"true\";vstsaadoautherrormessage=\"{0}\"", (object) errorMessage)) : this.UpdateApprovalsStatus(projectId, tenantId, authorizationFor, idToken, nonce, completeCallbackPayload);
    }

    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Aad", Justification = "Aad is well known term")]
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Oauth", Justification = "Oauth is well known term")]
    [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "This is the parameter name used by the caller, SPS Receive")]
    [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "token", Justification = "This is the case used by the caller, SPS Receive, common between TFS and RMO")]
    [SuppressMessage("Microsoft.Security.Web.Configuration", "CA3147:MarkVerbHandlersWithValidateAntiforgeryToken", Justification = "This POST does not originate from a web page we do not have an anti-forgery token.")]
    [AcceptVerbs(HttpVerbs.Post)]
    [TfsBypassAntiForgeryValidation]
    public ActionResult AadOauthCallback(
      string code,
      string state,
      string id_token,
      [ClientQueryParameter] string projectId,
      [ClientQueryParameter] string tenantId,
      [ClientQueryParameter] AuthorizationHeaderFor authorizationFor)
    {
      string errorMessage;
      if (!ReleaseController.IsParameterValid(code, nameof (code), out errorMessage) || !ReleaseController.IsParameterValid(state, nameof (state), out errorMessage) || !ReleaseController.IsParameterValid(id_token, nameof (id_token), out errorMessage))
        return (ActionResult) this.CreateJavaScript(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "vstsaadoauthcompleted=\"true\";vstsaadoautherrormessage=\"{0}\"", (object) errorMessage));
      string nonce;
      string completeCallbackPayload;
      return !ReleaseController.TryFetchOauthParameters(state, out nonce, out completeCallbackPayload, out errorMessage) ? (ActionResult) this.CreateJavaScript(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "vstsaadoauthcompleted=\"true\";vstsaadoautherrormessage=\"{0}\"", (object) errorMessage)) : this.UpdateApprovalsStatus(projectId, tenantId, authorizationFor, id_token, nonce, completeCallbackPayload);
    }

    [AcceptVerbs(HttpVerbs.Get)]
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Auth", Justification = "Auth is well known term")]
    [SuppressMessage("Microsoft.Design", "CA1026: DefaultParametersShouldNotBeUsed", Justification = "These parameters are optional for the client")]
    [SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", Justification = "Leaving it as string")]
    public ActionResult GetReleaseApprovalOAuthUrl(
      [ClientQueryParameter] string tenantId,
      [ClientQueryParameter] string redirectUri,
      [ClientQueryParameter] AadAuthUrlUtility.PromptOption promptOption = AadAuthUrlUtility.PromptOption.FreshLoginWithMfa,
      [ClientQueryParameter] string completeCallbackPayload = null,
      [ClientQueryParameter] bool completeCallbackByAuthCode = false)
    {
      return (ActionResult) new RedirectResult(MsalAzureAccessTokenHelper.GetAadOAuthRequestUrl(this.TfsRequestContext, tenantId, redirectUri, promptOption, completeCallbackPayload, completeCallbackByAuthCode));
    }

    private static bool TryFetchOauthParameters(
      string state,
      out string nonce,
      out string completeCallbackPayload,
      out string errorMessage)
    {
      NameValueCollection queryString = UriUtility.ParseQueryString(state);
      nonce = queryString[nameof (nonce)];
      completeCallbackPayload = queryString["CompleteCallbackPayload"];
      return ReleaseController.IsParameterValid(nonce, nameof (nonce), out errorMessage) && ReleaseController.IsParameterValid(completeCallbackPayload, nameof (completeCallbackPayload), out errorMessage);
    }

    private static bool IsParameterValid(
      string parameterValue,
      string parameterName,
      out string errorMessage)
    {
      errorMessage = string.Empty;
      if (!string.IsNullOrEmpty(parameterValue))
        return true;
      errorMessage = string.Format((IFormatProvider) CultureInfo.InvariantCulture, Resource.InvalidAuthenticationParameter, (object) parameterName);
      return false;
    }

    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Exception is thrown back in the response")]
    private ActionResult UpdateApprovalsStatus(
      string projectId,
      string tenantId,
      AuthorizationHeaderFor authorizationFor,
      string identificationToken,
      string nonce,
      string completeCallbackPayload)
    {
      Guid result;
      if (!Guid.TryParse(projectId, out result))
        return (ActionResult) this.CreateJavaScript(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "vstsaadoauthcompleted=\"true\";vstsaadoautherrormessage=\"{0}\"", (object) Resource.InvalidVstsProjectId));
      if (string.IsNullOrWhiteSpace(tenantId))
        return (ActionResult) this.CreateJavaScript(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "vstsaadoauthcompleted=\"true\";vstsaadoautherrormessage=\"{0}\"", (object) Resource.InvalidTenantId));
      if (string.IsNullOrEmpty(completeCallbackPayload) || authorizationFor != AuthorizationHeaderFor.RevalidateApproverIdentity)
        return (ActionResult) this.CreateJavaScript(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "vstsaadoauthcompleted=\"true\";vstsaadoautherrormessage=\"{0}\"", (object) Resource.UnexpectedCompleteCallbackPayload));
      ReleaseApproval releaseApproval = JsonUtilities.Deserialize<ReleaseApproval>(completeCallbackPayload);
      try
      {
        List<DeploymentAuthorizationInfo> authorizationInfoList = new List<DeploymentAuthorizationInfo>()
        {
          new DeploymentAuthorizationInfo()
          {
            TenantId = tenantId,
            AuthorizationHeaderFor = authorizationFor,
            OAuthParameters = new OAuthParameters()
            {
              IdentificationToken = identificationToken,
              Nonce = nonce
            }
          }
        };
        ApprovalsService service = this.TfsRequestContext.GetService<ApprovalsService>();
        IVssRequestContext tfsRequestContext = this.TfsRequestContext;
        Guid projectId1 = result;
        List<ReleaseApproval> approvals = new List<ReleaseApproval>();
        approvals.Add(releaseApproval);
        List<DeploymentAuthorizationInfo> deploymentAuthorizationInfo = authorizationInfoList;
        service.UpdateApprovalsStatus(tfsRequestContext, projectId1, (IEnumerable<ReleaseApproval>) approvals, (IList<DeploymentAuthorizationInfo>) deploymentAuthorizationInfo);
        return (ActionResult) this.CreateJavaScript("open(location, '_self').close();");
      }
      catch (Exception ex)
      {
        return (ActionResult) this.CreateJavaScript(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "vstsaadoauthcompleted=\"true\";vstsaadoautherrormessage=\"{0}\"", (object) ex.Message));
      }
    }

    private ContentResult CreateJavaScript(string javaScript) => this.Content("<script type=\"text/javascript\"" + this.CreateHtmlHelper().GenerateNonce(true) + "> " + javaScript + "</script>");

    private HtmlHelper CreateHtmlHelper()
    {
      ViewContext viewContext = new ViewContext();
      viewContext.Controller = (ControllerBase) this;
      viewContext.RequestContext = this.Request.RequestContext;
      viewContext.HttpContext = this.HttpContext;
      return new HtmlHelper(viewContext, (IViewDataContainer) new ViewPage());
    }

    internal static class AadOAuthParameters
    {
      internal const string Nonce = "nonce";
      internal const string CompleteCallbackPayload = "CompleteCallbackPayload";
    }
  }
}
