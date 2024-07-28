// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.FrameworkClaimsUpdateService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Diagnostics;
using System.Security.Claims;
using System.Security.Principal;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class FrameworkClaimsUpdateService : IClaimsUpdateService, IVssFrameworkService
  {
    private const string Area = "Identity";
    private const string Layer = "FrameworkClaimsUpdateService";

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public Microsoft.VisualStudio.Services.Identity.Identity UpdateClaims(
      IVssRequestContext requestContext,
      ClaimsPrincipal claimsPrincipal)
    {
      throw new InvalidOperationException();
    }

    public Microsoft.VisualStudio.Services.Identity.Identity UpdateClaims(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity identity,
      IIdentity bclIdentity)
    {
      requestContext.TraceEnter(15109090, "Identity", nameof (FrameworkClaimsUpdateService), nameof (UpdateClaims));
      try
      {
        Microsoft.VisualStudio.Services.Identity.Identity identity1 = (Microsoft.VisualStudio.Services.Identity.Identity) null;
        if (identity != null)
        {
          requestContext.Trace(15109091, TraceLevel.Verbose, "Identity", nameof (FrameworkClaimsUpdateService), "Source identity is not null, trying to match with existing identity in IMS using ReadActor");
          IVssRequestContext vssRequestContext = requestContext.Elevate();
          identity1 = vssRequestContext.GetService<IVssIdentityRetrievalService>().ResolveEligibleActor(vssRequestContext, identity.Descriptor);
          if (identity1 != null && !IdentityDescriptorComparer.Instance.Equals(identity1.Descriptor, identity.Descriptor))
          {
            requestContext.Trace(15109091, TraceLevel.Verbose, "Identity", nameof (FrameworkClaimsUpdateService), "Source identity and found identity have different descriptors. Returning source identity with found identity ids");
            identity.Id = identity1.Id;
            identity.ResourceVersion = identity1.ResourceVersion;
            identity1 = identity;
          }
        }
        if (identity1 == null)
          requestContext.Trace(15109092, TraceLevel.Verbose, "Identity", nameof (FrameworkClaimsUpdateService), "Identity cannot be found and we don't send claims to SPS because either FF is not enabled or is not authenticated by IdP");
        return identity1 ?? identity;
      }
      finally
      {
        requestContext.TraceLeave(15109090, "Identity", nameof (FrameworkClaimsUpdateService), nameof (UpdateClaims));
      }
    }
  }
}
