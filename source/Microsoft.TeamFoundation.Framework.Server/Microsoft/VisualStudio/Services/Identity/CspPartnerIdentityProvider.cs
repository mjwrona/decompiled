// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.CspPartnerIdentityProvider
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;

namespace Microsoft.VisualStudio.Services.Identity
{
  public sealed class CspPartnerIdentityProvider : ClaimsProvider
  {
    private static readonly string[] s_supportedIdentityTypes = new string[1]
    {
      "Microsoft.TeamFoundation.Claims.CspPartnerIdentity"
    };

    public CspPartnerIdentityProvider() => this.IdentityType = "Microsoft.TeamFoundation.Claims.CspPartnerIdentity";

    protected override string[] SupportedIdentityTypes() => CspPartnerIdentityProvider.s_supportedIdentityTypes;

    protected override Microsoft.VisualStudio.Services.Identity.Identity GetIdentity(
      IVssRequestContext requestContext,
      IIdentity identity)
    {
      Microsoft.VisualStudio.Services.Identity.Identity identity1 = base.GetIdentity(requestContext, identity);
      Claim claim = ((ClaimsIdentity) identity).Claims.SingleOrDefault<Claim>((Func<Claim, bool>) (x => StringComparer.OrdinalIgnoreCase.Equals(x.Type, "CspPartner")));
      if (claim == null)
        return identity1;
      IdentityMetaType result;
      if (!Enum.TryParse<IdentityMetaType>(claim.Value, true, out result) || !Enum.IsDefined(typeof (IdentityMetaType), (object) result))
        throw new InvalidCspIdentityException("Found CSP identity whose " + claim.Type + " claim value: " + claim.Value + " cannot be converted to IdentityMetaType.");
      identity1.MetaTypeId = (int) result;
      identity1.Properties.Remove(claim.Type);
      return identity1;
    }
  }
}
