// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.Authentication.UserAuthenticationSessionToken
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Microsoft.TeamFoundation.Framework.Server.Authentication
{
  public class UserAuthenticationSessionToken
  {
    public UserAuthenticationSessionToken(
      ClaimsPrincipal user,
      JwtSecurityToken securityToken,
      AuthenticationMechanism authenticationMechanism)
    {
      this.User = user;
      this.SecurityToken = securityToken;
      this.AuthenticationMechanism = authenticationMechanism;
    }

    public ClaimsPrincipal User { get; private set; }

    public JwtSecurityToken SecurityToken { get; private set; }

    public AuthenticationMechanism AuthenticationMechanism { get; private set; }
  }
}
