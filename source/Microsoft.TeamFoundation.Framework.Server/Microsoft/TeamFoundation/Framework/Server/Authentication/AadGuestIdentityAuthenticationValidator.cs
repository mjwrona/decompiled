// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.Authentication.AadGuestIdentityAuthenticationValidator
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Organization;
using System;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.Framework.Server.Authentication
{
  public static class AadGuestIdentityAuthenticationValidator
  {
    private const string TraceArea = "AadGuestIdentityAuthenticationValidator";
    private const string TraceLayer = "Authentication";

    public static bool IsValid(IVssRequestContext requestContext)
    {
      try
      {
        requestContext.TraceEnter(10012000, nameof (AadGuestIdentityAuthenticationValidator), "Authentication", nameof (IsValid));
        if (requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        {
          AadGuestIdentityAuthenticationValidator.Trace(requestContext, 10012001, TraceLevel.Verbose, "Request is for deployment-level resources.");
          return true;
        }
        if (requestContext.RequestRestrictions().HasAnyLabel("SignedInPage"))
        {
          AadGuestIdentityAuthenticationValidator.Trace(requestContext, 10012001, TraceLevel.Verbose, "Request is for signedIn. Bypassing guest user validator.");
          return true;
        }
        if (requestContext.IsPublicResourceLicense())
        {
          AadGuestIdentityAuthenticationValidator.Trace(requestContext, 10012001, TraceLevel.Verbose, "Request is for public project. Bypassing guest user validator.");
          return true;
        }
        if (!AadGuestIdentityAuthenticationValidator.IsGuestUser(requestContext))
        {
          AadGuestIdentityAuthenticationValidator.Trace(requestContext, 10012001, TraceLevel.Info, "Request is not for guest user.");
          return true;
        }
        IVssRequestContext context = requestContext.To(TeamFoundationHostType.Application);
        if (!context.IsOrganizationAadBacked())
        {
          AadGuestIdentityAuthenticationValidator.Trace(requestContext, 10012003, TraceLevel.Info, "Account {0} is not AAD backed.", (object) context.ServiceHost.InstanceId);
          return true;
        }
        if (AadGuestUserAccessHelper.IsAccessEnabled(requestContext))
          return true;
        AadGuestIdentityAuthenticationValidator.Trace(requestContext, 10012003, TraceLevel.Info, "Guest user access is not enabled for host {0}.", (object) requestContext.ServiceHost.InstanceId);
        return false;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(10012006, nameof (AadGuestIdentityAuthenticationValidator), "Authentication", ex);
        return false;
      }
      finally
      {
        requestContext.TraceLeave(10012007, nameof (AadGuestIdentityAuthenticationValidator), "Authentication", nameof (IsValid));
      }
    }

    private static void Trace(
      IVssRequestContext requestContext,
      int tracepoint,
      TraceLevel level,
      string format,
      params object[] args)
    {
      if (!requestContext.IsTracing(tracepoint, level, nameof (AadGuestIdentityAuthenticationValidator), "Authentication"))
        return;
      VssRequestContextExtensions.Trace(requestContext, tracepoint, level, nameof (AadGuestIdentityAuthenticationValidator), "Authentication", format, args);
    }

    private static bool IsGuestUser(IVssRequestContext requestContext)
    {
      Microsoft.VisualStudio.Services.Identity.Identity userIdentity = requestContext.GetUserIdentity();
      if (userIdentity != null)
      {
        if (!IdentityHelper.IsUserIdentity(requestContext, (IReadOnlyVssIdentity) userIdentity))
        {
          AadGuestIdentityAuthenticationValidator.Trace(requestContext, 10012003, TraceLevel.Verbose, "The request identity {0} is not a user identity.", (object) userIdentity.Id);
          return false;
        }
        if (!AadIdentityHelper.IsAadUser((IReadOnlyVssIdentity) userIdentity))
        {
          AadGuestIdentityAuthenticationValidator.Trace(requestContext, 10012004, TraceLevel.Verbose, "The request identity {0} is not an AAD identity.", (object) userIdentity.Id);
          return false;
        }
        if (userIdentity.MetaType == IdentityMetaType.Guest)
        {
          AadGuestIdentityAuthenticationValidator.Trace(requestContext, 10012005, TraceLevel.Warning, "The request identity {0} is guest AAD identity.", (object) userIdentity.Id);
          return true;
        }
      }
      return false;
    }
  }
}
