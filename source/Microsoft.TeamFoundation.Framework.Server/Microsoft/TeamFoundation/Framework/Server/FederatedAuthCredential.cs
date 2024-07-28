// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.FederatedAuthCredential
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.IdentityModel.Tokens;
using System.Security.Claims;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class FederatedAuthCredential : IAuthCredential
  {
    private readonly SessionSecurityToken _securityToken;

    public FederatedAuthCredential(SessionSecurityToken securityToken) => this._securityToken = securityToken;

    public bool IsAlternateCredential => false;

    public ClaimsPrincipal AuthenticatedPrincipal => this._securityToken.ClaimsPrincipal;

    public DateTimeOffset ValidFrom => (DateTimeOffset) this._securityToken.ValidFrom;

    public DateTimeOffset ValidTo => (DateTimeOffset) this._securityToken.ValidTo;
  }
}
