// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.Authentication.AadTokenConfigFrameworkExtensions
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server.ConfigFramework;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;

namespace Microsoft.TeamFoundation.Framework.Server.Authentication
{
  internal static class AadTokenConfigFrameworkExtensions
  {
    public static T QueryByToken<T>(
      this IConfigQueryable<T> config,
      IVssRequestContext requestContext,
      JwtSecurityToken token)
    {
      Guid hostId = Guid.Empty;
      if (!requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        hostId = requestContext.ServiceHost.InstanceId;
      return config.QueryByToken<T>(requestContext, hostId, token);
    }

    public static T QueryByToken<T>(
      this IConfigQueryable<T> config,
      IVssRequestContext requestContext,
      Guid hostId,
      JwtSecurityToken token)
    {
      return config.Query(requestContext, AadTokenConfigFrameworkExtensions.CreateTokenQuery(requestContext, hostId, token));
    }

    private static Query CreateTokenQuery(
      IVssRequestContext requestContext,
      Guid hostId,
      JwtSecurityToken token)
    {
      Guid result1 = Guid.Empty;
      Claim claim1 = token != null ? token.Claims.FirstOrDefault<Claim>((Func<Claim, bool>) (c => c.Type == "tid")) : (Claim) null;
      if (claim1 != null)
        Guid.TryParse(claim1.Value, out result1);
      if (result1 == Guid.Empty)
        result1 = requestContext.GetTenantId();
      Guid result2 = Guid.Empty;
      Claim claim2 = token != null ? token.Claims.FirstOrDefault<Claim>((Func<Claim, bool>) (c => c.Type == "oid")) : (Claim) null;
      if (claim2 != null)
        Guid.TryParse(claim2.Value, out result2);
      return new Query(result2, hostId, result1);
    }
  }
}
