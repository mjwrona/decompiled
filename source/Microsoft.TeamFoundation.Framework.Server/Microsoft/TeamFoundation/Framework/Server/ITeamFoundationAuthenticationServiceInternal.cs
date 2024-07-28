// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ITeamFoundationAuthenticationServiceInternal
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Web;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal interface ITeamFoundationAuthenticationServiceInternal : 
    ITeamFoundationAuthenticationService,
    IVssFrameworkService
  {
    bool IsRequestAuthenticationValid(IVssWebRequestContext requestContext, bool isSshRequest);

    FederatedAuthenticationSettings GetFederatedAuthenticationSettings(
      IVssRequestContext requestContext);

    void SetAuthenticationCredential(IAuthCredential credential);

    string GetCookieRootDomain(IVssRequestContext requestContext);

    bool IssueSessionSecurityToken(
      IVssRequestContext requestContext,
      ClaimsPrincipal principal,
      IEnumerable<Claim> additionalClaims = null);

    void ReissueFedAuthToken(IVssRequestContext requestContext);

    string GetSessionCookieDomain(IVssRequestContext requestContext);

    IdentityValidationResult CompleteUnauthorizedRequest(
      IVssRequestContext requestContext,
      HttpResponseBase response,
      IdentityValidationResult validationResult,
      Uri redirectUrl);
  }
}
