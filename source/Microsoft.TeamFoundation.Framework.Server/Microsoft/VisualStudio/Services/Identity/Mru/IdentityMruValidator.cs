// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.Mru.IdentityMruValidator
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Identity.Mru
{
  internal static class IdentityMruValidator
  {
    internal static void ValidateMruContainerId(Guid containerId)
    {
      if (containerId == Guid.Empty)
        throw new IdentityMruBadRequestException(FrameworkResources.InvalidIdentityMruId());
    }

    internal static void ValidateMruValue(IList<Guid> identityIds)
    {
      if (identityIds == null || identityIds.Any<Guid>((Func<Guid, bool>) (x => x == Guid.Empty)))
        throw new IdentityMruBadRequestException(FrameworkResources.InvalidIdentityMruValue());
    }

    internal static void ValidateMruValue(IList<Microsoft.VisualStudio.Services.Identity.Identity> identities)
    {
      if (identities == null)
        throw new IdentityMruBadRequestException(FrameworkResources.InvalidIdentityMruValue());
    }

    internal static void ValidateIdentityExists(IVssRequestContext context, Guid identityId)
    {
      if (identityId == Guid.Empty)
        throw new IdentityMruResourceNotFoundException(IdentityResources.IdentityNotFoundWithTfid((object) identityId));
      if (context.GetService<IdentityService>().ReadIdentities(context, (IList<Guid>) new Guid[1]
      {
        identityId
      }, QueryMembership.None, (IEnumerable<string>) null)[0] == null)
        throw new IdentityMruResourceNotFoundException(IdentityResources.IdentityNotFoundWithTfid((object) identityId));
    }

    internal static bool IsRequestForSelf(IVssRequestContext requestContext, Guid identityId)
    {
      Microsoft.VisualStudio.Services.Identity.Identity userIdentity = requestContext.GetUserIdentity();
      return userIdentity != null && (userIdentity.Id == identityId || userIdentity.MasterId == identityId);
    }
  }
}
