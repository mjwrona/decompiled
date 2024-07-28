// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.ServiceIdentityProvider
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Security.Claims;
using System.Security.Principal;

namespace Microsoft.VisualStudio.Services.Identity
{
  public sealed class ServiceIdentityProvider : NonSyncableIdentityProvider
  {
    private static readonly string[] s_supportedIdentityTypes = new string[1]
    {
      "Microsoft.TeamFoundation.ServiceIdentity"
    };

    protected override string[] SupportedIdentityTypes() => ServiceIdentityProvider.s_supportedIdentityTypes;

    protected override IdentityDescriptor CreateDescriptor(
      IVssRequestContext requestContext,
      IIdentity identity)
    {
      string identifier = (string) null;
      foreach (Claim claim in ((ClaimsIdentity) identity).Claims)
      {
        if (claim.Type.Equals("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier", StringComparison.OrdinalIgnoreCase))
          identifier = claim.Value;
      }
      return new IdentityDescriptor("Microsoft.TeamFoundation.ServiceIdentity", identifier);
    }

    protected override Microsoft.VisualStudio.Services.Identity.Identity GetIdentity(
      IVssRequestContext requestContext,
      IIdentity identity)
    {
      return (Microsoft.VisualStudio.Services.Identity.Identity) null;
    }
  }
}
