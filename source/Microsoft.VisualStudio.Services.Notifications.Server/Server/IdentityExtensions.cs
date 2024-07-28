// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.IdentityExtensions
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.TeamFoundation;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  public static class IdentityExtensions
  {
    public static IdentityRef ToIdentityRef(this Microsoft.VisualStudio.Services.Identity.Identity identity) => new IdentityRef()
    {
      Id = identity.Id.ToString(),
      DisplayName = identity.DisplayName,
      IsContainer = identity.IsContainer,
      UniqueName = IdentityHelper.GetUniqueName(identity)
    };

    public static Microsoft.VisualStudio.Services.Identity.Identity ToVsIdentity(
      this IdentityRef identity,
      IVssRequestContext requestContext)
    {
      Guid result = Guid.Empty;
      if (identity != null)
      {
        if (!Guid.TryParse(identity.Id, out result))
          throw new InvalidFieldValueException(CoreRes.InvalidIdentityException((object) identity.Id));
      }
      else
        result = requestContext.GetUserId();
      Microsoft.VisualStudio.Services.Identity.Identity vsIdentity;
      if (!result.Equals(Guid.Empty) && requestContext.GetUserId().ToString() != identity.Id)
      {
        vsIdentity = requestContext.GetService<IdentityService>().ReadIdentities(requestContext, (IList<Guid>) new List<Guid>((IEnumerable<Guid>) new Guid[1]
        {
          result
        }), QueryMembership.None, (IEnumerable<string>) null)[0];
        if (vsIdentity == null)
          throw new IdentityNotFoundException(result);
      }
      else
        vsIdentity = requestContext.GetUserIdentity();
      return vsIdentity;
    }

    public static bool IsTeam(this Microsoft.VisualStudio.Services.Identity.Identity identity, IVssRequestContext requestContext)
    {
      if (requestContext.IsFeatureEnabled("Notifications.EnableCachingIdentityIsTeamProperties"))
      {
        IIdentityExtensionsCacheService service = requestContext.GetService<IIdentityExtensionsCacheService>();
        bool isTeam;
        if (service.TryGet(identity.Id, out isTeam, requestContext))
          return isTeam;
        new IdentityPropertyHelper().ReadExtendedProperties(requestContext, (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) new List<Microsoft.VisualStudio.Services.Identity.Identity>()
        {
          identity
        }, (IEnumerable<string>) new List<string>()
        {
          TeamConstants.TeamPropertyName
        }, IdentityPropertyScope.Local);
        bool property = identity.TryGetProperty(TeamConstants.TeamPropertyName, out object _);
        service.AddOrUpdate(identity.Id, property, requestContext);
        return property;
      }
      new IdentityPropertyHelper().ReadExtendedProperties(requestContext, (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) new List<Microsoft.VisualStudio.Services.Identity.Identity>()
      {
        identity
      }, (IEnumerable<string>) new List<string>()
      {
        TeamConstants.TeamPropertyName
      }, IdentityPropertyScope.Local);
      return identity.TryGetProperty(TeamConstants.TeamPropertyName, out object _);
    }
  }
}
