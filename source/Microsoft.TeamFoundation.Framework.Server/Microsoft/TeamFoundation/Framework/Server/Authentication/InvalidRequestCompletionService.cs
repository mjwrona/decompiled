// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.Authentication.InvalidRequestCompletionService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Web;

namespace Microsoft.TeamFoundation.Framework.Server.Authentication
{
  public class InvalidRequestCompletionService : 
    IInvalidRequestCompletionService,
    IVssFrameworkService
  {
    public const string TraceArea = "Authentication";
    public const string TraceLayer = "InvalidRequestCompletionService";

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public bool CanRedirectRequest(IVssRequestContext requestContext)
    {
      IVssRequestContext requestContext1 = requestContext.To(TeamFoundationHostType.Deployment);
      if (!string.Equals(requestContext1.AuthenticationType(), "Basic", StringComparison.OrdinalIgnoreCase) && (!requestContext1.Items.ContainsKey(RequestContextItemsKeys.AlternateAuthCredentialsContextKey) || !(bool) requestContext1.Items[RequestContextItemsKeys.AlternateAuthCredentialsContextKey]) && (!requestContext1.Items.ContainsKey("IsAadAuthFlow") || !(bool) requestContext1.Items["IsAadAuthFlow"]))
        return true;
      requestContext.Trace(1011607, TraceLevel.Info, "Authentication", nameof (InvalidRequestCompletionService), "Current request authentication type is Basic or alternate credentials. Cannot redirect the request to sign in");
      return false;
    }

    public bool SuppressRedirect(IVssRequestContext requestContext, HttpContextBase context)
    {
      if (context.Request.Headers["X-TFS-FedAuthRedirect"] == "Suppress" || context.Request.Path.EndsWith(".asmx", StringComparison.OrdinalIgnoreCase) || context.Request.Path.EndsWith(".ashx", StringComparison.OrdinalIgnoreCase))
        return true;
      return (requestContext.RequestRestrictions().MechanismsToAdvertise & AuthenticationMechanisms.FederatedRedirect) == AuthenticationMechanisms.None && (requestContext.RequestRestrictions().MechanismsToAdvertise & AuthenticationMechanisms.Federated) != 0;
    }

    public void CompleteInvalidRequest(
      IVssRequestContext requestContext,
      ITeamFoundationAuthenticationService authenticationService,
      string traceArea = "Authentication")
    {
      this.CompleteInvalidRequest(requestContext, authenticationService, "InvalidRequest", nameof (InvalidRequestCompletionService), false, false, (Exception) null, traceArea, (string) null);
    }

    public void CompleteInvalidRequest(
      IVssRequestContext requestContext,
      ITeamFoundationAuthenticationService authenticationService,
      string authenticateErrorReason,
      string moduleName,
      bool forceRedirect = false,
      bool forceForbiddenResponse = false,
      Exception forbiddenResponseException = null,
      string traceArea = "Authentication",
      string signinRedirectLocation = null)
    {
      requestContext.TraceEnter(1011608, traceArea, nameof (InvalidRequestCompletionService), nameof (CompleteInvalidRequest));
      AuthenticationHelpers.EnterMethodIfNull(requestContext, "Authentication", moduleName + ".CompleteInvalidRequest");
      if (string.IsNullOrWhiteSpace(signinRedirectLocation))
        signinRedirectLocation = authenticationService.GetSignInRedirectLocation(requestContext, true);
      HttpContextBase current = HttpContextFactory.Current;
      current.Response.AddHeader("X-VSS-AuthenticateError", authenticateErrorReason);
      authenticationService.AddFederatedAuthHeaders(requestContext, current.Response);
      if (forceRedirect)
        this.RedirectRequestToSignin(requestContext, current, signinRedirectLocation);
      else if (forceForbiddenResponse)
        this.GenerateForbiddenResponse(requestContext, current, forbiddenResponseException);
      else if (this.SuppressRedirect(requestContext, current))
      {
        if (string.IsNullOrWhiteSpace(HttpContext.Current.Response.Headers.Get("X-TFS-FedAuthRedirect")))
        {
          current.Response.AddHeader("WWW-Authenticate", "TFS-Federated");
          current.Response.AddHeader("X-TFS-FedAuthRedirect", signinRedirectLocation);
        }
        TeamFoundationApplicationCore.CompleteRequest(current.GetApplicationInstance(), HttpStatusCode.Unauthorized, FrameworkResources.InvalidAccessException(), (IEnumerable<KeyValuePair<string, string>>) null, (Exception) new InvalidAccessException(authenticateErrorReason), FrameworkResources.InvalidAccessException(), (string) null);
      }
      else if (!this.CanRedirectRequest(requestContext))
        this.GenerateForbiddenResponse(requestContext, current, forbiddenResponseException);
      else
        this.RedirectRequestToSignin(requestContext, current, signinRedirectLocation);
      requestContext.TraceLeave(1011609, traceArea, nameof (InvalidRequestCompletionService), nameof (CompleteInvalidRequest));
    }

    private void GenerateForbiddenResponse(
      IVssRequestContext requestContext,
      HttpContextBase context,
      Exception forbiddenResponseException)
    {
      requestContext.Trace(1011610, TraceLevel.Info, "Authentication", nameof (InvalidRequestCompletionService), "Returning forbidden response");
      string message = forbiddenResponseException?.Message;
      string errorMessage = !string.IsNullOrEmpty(message) ? message : FrameworkResources.InvalidAccessException();
      TeamFoundationApplicationCore.CompleteRequest(context.GetApplicationInstance(), HttpStatusCode.Forbidden, FrameworkResources.InvalidAccessException(), (IEnumerable<KeyValuePair<string, string>>) null, forbiddenResponseException, errorMessage, (string) null);
    }

    private void RedirectRequestToSignin(
      IVssRequestContext requestContext,
      HttpContextBase context,
      string signinRedirectLocation)
    {
      requestContext.Trace(1011610, TraceLevel.Info, "Authentication", nameof (InvalidRequestCompletionService), "Redirecting current request to signin location {0}", (object) signinRedirectLocation);
      context.Response.Redirect(signinRedirectLocation, false);
      Microsoft.TeamFoundation.Framework.Server.HttpApplicationFactory.Current.CompleteRequest();
    }

    private static class TracePoints
    {
      public const int CompleteInvalidRequestEnter = 1011608;
      public const int CompleteInvalidRequestExit = 1011609;
      public const int CompleteInvalidRequestInfo = 1011610;
    }
  }
}
