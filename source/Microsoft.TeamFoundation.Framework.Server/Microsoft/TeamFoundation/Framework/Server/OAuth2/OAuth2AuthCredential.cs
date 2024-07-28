// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.OAuth2.OAuth2AuthCredential
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Microsoft.TeamFoundation.Framework.Server.OAuth2
{
  public class OAuth2AuthCredential : IAuthCredential
  {
    private readonly bool _isAlternateCredential;
    private readonly ClaimsPrincipal _claimsPrincipal;
    private readonly JwtSecurityToken _token;

    public OAuth2AuthCredential(
      ClaimsPrincipal claimsPrincipal,
      JwtSecurityToken token,
      bool isAlternateCredential = false)
    {
      this._isAlternateCredential = isAlternateCredential;
      this._claimsPrincipal = claimsPrincipal;
      this._token = token;
    }

    public bool IsAlternateCredential => this._isAlternateCredential;

    public ClaimsPrincipal AuthenticatedPrincipal => this._claimsPrincipal;

    public DateTimeOffset ValidFrom => (DateTimeOffset) this._token.ValidFrom;

    public DateTimeOffset ValidTo => (DateTimeOffset) this._token.ValidTo;
  }
}
