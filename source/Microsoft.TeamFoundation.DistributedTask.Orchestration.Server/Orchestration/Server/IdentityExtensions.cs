// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.IdentityExtensions
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server
{
  internal static class IdentityExtensions
  {
    public static IdentityRef Clone(this IdentityRef identityRef) => new IdentityRef()
    {
      Id = identityRef.Id,
      DisplayName = identityRef.DisplayName,
      UniqueName = identityRef.UniqueName,
      Url = identityRef.Url,
      ProfileUrl = identityRef.ProfileUrl,
      ImageUrl = identityRef.ImageUrl,
      IsContainer = identityRef.IsContainer,
      IsAadIdentity = identityRef.IsAadIdentity
    };

    public static IdentityRef GetRequesterIdentity(this IVssRequestContext requestContext)
    {
      string key = requestContext.GetUserId(true).ToString();
      return new HashSet<string>() { key }.QueryIdentities(requestContext)[key];
    }

    public static IDictionary<string, IdentityRef> QueryIdentities(
      this HashSet<string> identityIds,
      IVssRequestContext requestContext)
    {
      IdentityService service = requestContext.GetService<IdentityService>();
      IEnumerable<Guid> guids = identityIds.Select<string, Guid>((Func<string, Guid>) (x => Guid.Parse(x)));
      IVssRequestContext requestContext1 = requestContext;
      IEnumerable<Guid> identityIds1 = guids;
      IList<Microsoft.VisualStudio.Services.Identity.Identity> identities = service.GetIdentities(requestContext1, identityIds1);
      Dictionary<string, Microsoft.VisualStudio.Services.Identity.Identity> dictionary1 = new Dictionary<string, Microsoft.VisualStudio.Services.Identity.Identity>(identities.Count);
      foreach (Microsoft.VisualStudio.Services.Identity.Identity identity in (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) identities)
      {
        if (identity != null)
          dictionary1[identity.Id.ToString()] = identity;
      }
      Dictionary<string, IdentityRef> dictionary2 = new Dictionary<string, IdentityRef>(identityIds.Count);
      string str = Guid.Empty.ToString();
      foreach (string identityId in identityIds)
      {
        if (identityId == str)
        {
          dictionary2[identityId] = new IdentityRef()
          {
            Id = identityId,
            DisplayName = string.Empty,
            UniqueName = string.Empty,
            IsContainer = false
          };
        }
        else
        {
          Microsoft.VisualStudio.Services.Identity.Identity identity;
          if (!dictionary1.TryGetValue(identityId, out identity))
            dictionary2[identityId] = new IdentityRef()
            {
              Id = identityId
            };
          else
            dictionary2[identityId] = new IdentityRef()
            {
              DisplayName = identity.DisplayName,
              Id = identityId,
              IsContainer = identity.IsContainer,
              UniqueName = IdentityHelper.GetUniqueName(identity)
            };
        }
      }
      return (IDictionary<string, IdentityRef>) dictionary2;
    }

    private static IList<Microsoft.VisualStudio.Services.Identity.Identity> GetIdentities(
      this IdentityService identityService,
      IVssRequestContext requestContext,
      IEnumerable<Guid> identityIds)
    {
      List<Guid> list = identityIds.Where<Guid>((Func<Guid, bool>) (x => x != Guid.Empty)).ToList<Guid>();
      return identityService.ReadIdentities(requestContext, (IList<Guid>) list, QueryMembership.None, (IEnumerable<string>) null);
    }
  }
}
