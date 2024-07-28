// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ITeamFoundationAuthenticationService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server.Authentication;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Http;
using System.Web;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [DefaultServiceImplementation(typeof (TeamFoundationAuthenticationService))]
  public interface ITeamFoundationAuthenticationService : IVssFrameworkService
  {
    void ConfigureBasic(IVssRequestContext requestContext, Uri realm);

    void ConfigureRequest(IVssRequestContext requestContext);

    List<SessionSecurityTokenData> GetSessionSecurityTokenDataFromCookies(
      IVssRequestContext requestContext);

    IAuthCredential GetAuthCredential();

    string GetSignInRedirectLocation(
      IVssRequestContext requestContext,
      bool force = false,
      IDictionary<string, string> parameters = null,
      Uri replyToOverride = null,
      SwitchHintParameter switchHintParameter = null);

    void AddFederatedAuthHeaders(IVssRequestContext requestContext, HttpResponseBase response);

    [EditorBrowsable(EditorBrowsableState.Never)]
    void AddFederatedAuthHeaders(IVssRequestContext requestContext, HttpResponseMessage response);

    void AddTenantInfoResponseHeader(
      IVssRequestContext tfsRequestContext,
      HttpResponseMessage response);

    void SignOutFromSessionModule(IVssRequestContext requestContext);

    string LocationForRealm(IVssRequestContext requestContext, string relativePath);

    string DetermineRealm(IVssRequestContext requestContext);

    void RedirectToIdentityProvider(
      IVssRequestContext requestContext,
      bool hasInvalidToken = false,
      bool force = false);

    void ProcessSignOutCookie(IVssRequestContext requestContext, Uri realmUri);

    string BuildHostedSignOutUrl(IVssRequestContext requestContext);

    string BuildAADSignOutUrl(IVssRequestContext requestContext, string callBackUrl = null);

    SignOutUris ReadSignOutCookie(IVssRequestContext requestContext);

    void ClearSignOutCookie(IVssRequestContext requestContext);
  }
}
