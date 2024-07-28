// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.Authentication.TokenRevocationValidator
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.TokenRevocation;
using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Security.Principal;
using System.Web;

namespace Microsoft.TeamFoundation.Framework.Server.Authentication
{
  public class TokenRevocationValidator
  {
    protected string TraceArea => nameof (TokenRevocationValidator);

    protected string TraceLayer => "Authentication";

    public void PreValidateRequestAuthentication(
      IVssRequestContext requestContext,
      ITeamFoundationAuthenticationService authenticationService,
      Microsoft.VisualStudio.Services.Identity.Identity identity)
    {
      if (this.IsValid(requestContext, authenticationService, identity))
        return;
      if (this.SkipValidation(requestContext))
        HttpContext.Current.User = (IPrincipal) new GenericPrincipal((IIdentity) WindowsIdentity.GetAnonymous(), Array.Empty<string>());
      else
        requestContext.GetService<IInvalidRequestCompletionService>().CompleteInvalidRequest(requestContext, authenticationService, "RevokedToken", nameof (TokenRevocationValidator), traceArea: this.TraceArea);
    }

    private bool IsValid(
      IVssRequestContext requestContext,
      ITeamFoundationAuthenticationService authenticationService,
      Microsoft.VisualStudio.Services.Identity.Identity identity)
    {
      requestContext.TraceEnter(1049075, this.TraceArea, this.TraceLayer, nameof (IsValid));
      try
      {
        if (!this.CanValidate(requestContext, identity.Descriptor))
        {
          requestContext.Trace(1049081, TraceLevel.Info, this.TraceArea, this.TraceLayer, "Request cannot be validated. Skipping check");
          return true;
        }
        IAuthCredential authCredential = authenticationService.GetAuthCredential();
        Guid failingRuleId;
        if (requestContext.GetService<ITokenRevocationService>().IsValid(requestContext.Elevate(), authCredential, identity.Id, out failingRuleId))
          return true;
        requestContext.Trace(1049094, TraceLevel.Info, this.TraceArea, this.TraceLayer, string.Format("Token failed rule {0}", (object) failingRuleId));
        return false;
      }
      finally
      {
        requestContext.TraceLeave(1049076, this.TraceArea, this.TraceLayer, nameof (IsValid));
      }
    }

    internal bool CanValidate(IVssRequestContext requestContext, IdentityDescriptor descriptor)
    {
      requestContext.TraceEnter(1049077, this.TraceArea, this.TraceLayer, nameof (CanValidate));
      try
      {
        if (requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
          return false;
        if (!ServicePrincipals.IsServicePrincipal(requestContext, descriptor))
          return true;
        if (requestContext.IsTracing(1049080, TraceLevel.Verbose, this.TraceArea, this.TraceLayer))
          requestContext.Trace(1049080, TraceLevel.Verbose, this.TraceArea, this.TraceLayer, "The authenticated identity {0} is a service principal.", (object) descriptor);
        return false;
      }
      finally
      {
        requestContext.TraceLeave(1049078, this.TraceArea, this.TraceLayer, nameof (CanValidate));
      }
    }

    private bool SkipValidation(IVssRequestContext requestContext)
    {
      if (requestContext is IVssWebRequestContext)
      {
        if (requestContext.RequestRestrictions() != null && requestContext.RequestRestrictions().RequiredAuthentication >= RequiredAuthentication.Authenticated)
        {
          if (!requestContext.RequestRestrictions().HasAnyLabel("Signout"))
          {
            NameValueCollection queryString = HttpUtility.ParseQueryString(((IVssWebRequestContext) requestContext).RequestUri.Query);
            bool result;
            if (((string.IsNullOrEmpty(queryString["forceSignout"]) ? 0 : (bool.TryParse(queryString["forceSignout"], out result) ? 1 : 0)) & (result ? 1 : 0)) != 0)
            {
              requestContext.Trace(1011110, TraceLevel.Info, this.TraceArea, this.TraceLayer, "Skipping IsRequestAuthenticationValid. Current request query params contains forceSignout.");
              return true;
            }
            goto label_6;
          }
        }
        requestContext.Trace(1011110, TraceLevel.Info, this.TraceArea, this.TraceLayer, "Skipping IsRequestAuthenticationValid. Current request restriction labels {0} matched Signout request", requestContext.RequestRestrictions() == null ? (object) string.Empty : (object) requestContext.RequestRestrictions().Label);
        return true;
      }
label_6:
      return false;
    }
  }
}
