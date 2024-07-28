// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Checks.Common.IdentityServiceExtensions
// Assembly: Microsoft.Azure.Pipelines.Checks.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8C585FB3-01FB-4B82-B4E2-03BD94D0A581
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Checks.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Azure.Pipelines.Checks.Common
{
  public static class IdentityServiceExtensions
  {
    public static IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> GetIdentities(
      this IdentityService identityService,
      IVssRequestContext requestContext,
      IEnumerable<Guid> identityIds)
    {
      List<Guid> list = identityIds.Where<Guid>((Func<Guid, bool>) (x => x != Guid.Empty)).ToList<Guid>();
      return (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) identityService.ReadIdentities(requestContext, (IList<Guid>) list, QueryMembership.None, (IEnumerable<string>) null);
    }

    public static IDictionary<string, IdentityRef> QueryIdentities(
      this IdentityService identityService,
      IVssRequestContext requestContext,
      IList<string> identityIds)
    {
      Dictionary<string, IdentityRef> dictionary = new Dictionary<string, IdentityRef>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      IEnumerable<Guid> identityIds1 = identityIds.Select<string, Guid>((Func<string, Guid>) (x => Guid.Parse(x))).Distinct<Guid>();
      List<Microsoft.VisualStudio.Services.Identity.Identity> list = identityService.GetIdentities(requestContext, identityIds1).ToList<Microsoft.VisualStudio.Services.Identity.Identity>();
      foreach (string identityId1 in (IEnumerable<string>) identityIds)
      {
        string identityId = identityId1;
        if (identityId == Guid.Empty.ToString())
        {
          dictionary[identityId] = new IdentityRef()
          {
            Id = identityId,
            DisplayName = string.Empty,
            UniqueName = string.Empty,
            IsContainer = false
          };
        }
        else
        {
          Microsoft.VisualStudio.Services.Identity.Identity identity1 = list.FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (identity => identity != null && identity.Id == Guid.Parse(identityId)));
          if (identity1 == null)
            dictionary[identityId] = new IdentityRef()
            {
              Id = identityId
            };
          else
            dictionary[identityId] = identity1.ToIdentityRef(requestContext, false);
        }
      }
      return (IDictionary<string, IdentityRef>) dictionary;
    }
  }
}
