// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.Error.ErrorUtilities
// Assembly: Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A7EB5677-18AD-4D09-80BD-B83CBD009DB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Authentication;
using Microsoft.VisualStudio.Services.Common.Internal;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Globalization;
using System.Web;

namespace Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.Error
{
  public static class ErrorUtilities
  {
    private const string TraceArea = "ErrorHelpers";
    private const string TraceLayer = "ErrorProcessing";

    public static string GetUserEmail(IVssRequestContext requestContext)
    {
      string userEmail = string.Empty;
      try
      {
        if (requestContext != null)
        {
          if (requestContext.UserContext != (IdentityDescriptor) null)
          {
            Microsoft.VisualStudio.Services.Identity.Identity userIdentity = requestContext.GetUserIdentity();
            if (userIdentity != null)
              userEmail = userIdentity.GetProperty<string>("Mail", string.Empty);
          }
        }
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(90001003, TraceLevel.Error, "ErrorHelpers", "ErrorProcessing", ex);
      }
      return userEmail;
    }

    public static string GetRootUrl(IVssRequestContext tfsRequestContext)
    {
      string rootUrl = VirtualPathUtility.ToAbsolute("~/");
      if (tfsRequestContext != null)
      {
        if (tfsRequestContext.ServiceInstanceType() == ServiceInstanceTypes.SPS)
          tfsRequestContext = tfsRequestContext.To(TeamFoundationHostType.Deployment);
        rootUrl = tfsRequestContext.GetService<ILocationService>().DetermineAccessMapping(tfsRequestContext).AccessPoint;
      }
      return rootUrl;
    }

    public static string ExtractSignoutURL(
      IVssRequestContext tfsRequestContext,
      Uri requestUri,
      int statusCode,
      string additionalRedirectQueryParameters = null)
    {
      VirtualPathUtility.ToAbsolute("~/");
      string signoutUrl;
      if (tfsRequestContext != null)
      {
        if (tfsRequestContext.ServiceInstanceType() == ServiceInstanceTypes.SPS)
          tfsRequestContext = tfsRequestContext.To(TeamFoundationHostType.Deployment);
        AccessMapping accessMapping = tfsRequestContext.GetService<ILocationService>().DetermineAccessMapping(tfsRequestContext);
        string accessPoint = accessMapping.AccessPoint;
        if (tfsRequestContext.UserContext != (IdentityDescriptor) null && tfsRequestContext.UserContext.IsCspPartnerIdentityType())
          return ErrorUtilities.BuildSignoutUrlForCspPartnerUser(tfsRequestContext);
        signoutUrl = !accessMapping.AccessPoint.EndsWith("/", StringComparison.OrdinalIgnoreCase) ? accessMapping.AccessPoint + "/_signout" : accessMapping.AccessPoint + "_signout";
        if ((statusCode == 401 || statusCode == 403 || statusCode == 404) && requestUri != (Uri) null)
        {
          UriBuilder uriBuilder = new UriBuilder(requestUri);
          uriBuilder.Port = new Uri(accessMapping.AccessPoint).Port;
          if (statusCode == 403)
          {
            if (tfsRequestContext.RequestRestrictions().HasAnyLabel("SignedInPage", "TenantPicker"))
            {
              string uri = HttpUtility.ParseQueryString(uriBuilder.Query)["reply_to"] ?? accessPoint;
              NameValueCollection state = AadAuthUrlUtility.ParseState(tfsRequestContext);
              if (state != null)
                uri = state["reply_to"] ?? uri;
              uriBuilder = new UriBuilder(uri);
            }
          }
          if (additionalRedirectQueryParameters != null)
            uriBuilder.Query = uriBuilder.Query == null || uriBuilder.Query.Length <= 1 ? additionalRedirectQueryParameters : uriBuilder.Query.Substring(1) + "&" + additionalRedirectQueryParameters;
          signoutUrl += string.Format((IFormatProvider) CultureInfo.InvariantCulture, "?redirectUrl={0}", (object) Uri.EscapeDataString(uriBuilder.Uri.AbsoluteUri));
        }
      }
      else
        signoutUrl = VirtualPathUtility.ToAbsolute("~/_signout");
      return signoutUrl;
    }

    private static string BuildSignoutUrlForCspPartnerUser(IVssRequestContext tfsRequestContext)
    {
      string spsAbsoluteUrl = ErrorUtilities.GetSpsAbsoluteUrl(tfsRequestContext.To(TeamFoundationHostType.Deployment));
      if (string.IsNullOrWhiteSpace(spsAbsoluteUrl))
        return VirtualPathUtility.ToAbsolute("~/_signout");
      string str = spsAbsoluteUrl.EndsWith("/", StringComparison.OrdinalIgnoreCase) ? spsAbsoluteUrl + "_signout" : spsAbsoluteUrl + "/_signout";
      Dictionary<string, string> parameters = new Dictionary<string, string>()
      {
        {
          "tenantId",
          AadIdentityHelper.GetIdentityTenantId(tfsRequestContext.UserContext).ToString()
        }
      };
      string redirectLocation = tfsRequestContext.GetService<ITeamFoundationAuthenticationService>().GetSignInRedirectLocation(tfsRequestContext, true, (IDictionary<string, string>) parameters);
      return string.IsNullOrWhiteSpace(redirectLocation) ? str : str + string.Format((IFormatProvider) CultureInfo.InvariantCulture, "?redirectUrl={0}", (object) UriUtility.UrlEncode(redirectLocation));
    }

    private static string GetSpsAbsoluteUrl(IVssRequestContext context)
    {
      ILocationService service = context.GetService<ILocationService>();
      try
      {
        return new Uri(service.GetLocationServiceUrl(context, ServiceInstanceTypes.SPS, AccessMappingConstants.ClientAccessMappingMoniker)).AbsoluteUri;
      }
      catch (Exception ex)
      {
        context.Trace(90001002, TraceLevel.Error, "ErrorHelpers", "ErrorProcessing", "Failed to get SPS AbsoluteUri. " + ex.Message);
        return string.Empty;
      }
    }
  }
}
