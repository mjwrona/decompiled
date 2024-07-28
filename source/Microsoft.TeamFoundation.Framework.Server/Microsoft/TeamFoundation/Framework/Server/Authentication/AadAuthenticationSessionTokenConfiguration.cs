// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.Authentication.AadAuthenticationSessionTokenConfiguration
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server.ConfigFramework;
using System;
using System.IdentityModel.Tokens.Jwt;

namespace Microsoft.TeamFoundation.Framework.Server.Authentication
{
  public class AadAuthenticationSessionTokenConfiguration : 
    IAadAuthenticationSessionTokenConfiguration
  {
    private const string IssueAadAuthenticationCookiePath = "Authentication.AadCookie.IssueAadAuthenticationCookie";
    private const string AcceptAadAuthenticationCookiePath = "Authentication.AadCookie.AcceptAadAuthenticationCookie";
    private static readonly IConfigPrototype<bool> issueAadAuthenticationCookiePrototype = ConfigPrototype.Create<bool>("Authentication.AadCookie.IssueAadAuthenticationCookie", false);
    private static readonly IConfigQueryable<bool> issueAadAuthenticationCookie = ConfigProxy.Create<bool>(AadAuthenticationSessionTokenConfiguration.issueAadAuthenticationCookiePrototype);
    private static readonly IConfigPrototype<bool> acceptAadAuthenticationCookiePrototype = ConfigPrototype.Create<bool>("Authentication.AadCookie.AcceptAadAuthenticationCookie", true);
    private static readonly IConfigQueryable<bool> acceptAadAuthenticationCookie = ConfigProxy.Create<bool>(AadAuthenticationSessionTokenConfiguration.acceptAadAuthenticationCookiePrototype);

    public bool IssueAadAuthenticationCookieEnabled(
      IVssRequestContext requestContext,
      Guid? remoteHostId = null)
    {
      return remoteHostId.HasValue ? AadAuthenticationSessionTokenConfiguration.issueAadAuthenticationCookie.Query(requestContext, new Query(requestContext.GetUserId(), remoteHostId.Value, requestContext.GetTenantId())) : AadAuthenticationSessionTokenConfiguration.issueAadAuthenticationCookie.QueryByCtx<bool>(requestContext);
    }

    public bool IssueAadAuthenticationCookieEnabled(
      IVssRequestContext requestContext,
      JwtSecurityToken validatedToken)
    {
      return AadAuthenticationSessionTokenConfiguration.issueAadAuthenticationCookie.QueryByToken<bool>(requestContext, validatedToken);
    }

    public bool AcceptAadAuthenticationCookieEnabled(
      IVssRequestContext requestContext,
      JwtSecurityToken validatedToken)
    {
      return AadAuthenticationSessionTokenConfiguration.acceptAadAuthenticationCookie.QueryByToken<bool>(requestContext, validatedToken);
    }
  }
}
