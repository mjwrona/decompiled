// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.IClaimsUpdateService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Security.Claims;
using System.Security.Principal;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [DefaultServiceImplementation(typeof (FrameworkClaimsUpdateService))]
  public interface IClaimsUpdateService : IVssFrameworkService
  {
    Microsoft.VisualStudio.Services.Identity.Identity UpdateClaims(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity identity,
      IIdentity bclIdentity);

    Microsoft.VisualStudio.Services.Identity.Identity UpdateClaims(
      IVssRequestContext requestContext,
      ClaimsPrincipal claimsPrincipal);
  }
}
