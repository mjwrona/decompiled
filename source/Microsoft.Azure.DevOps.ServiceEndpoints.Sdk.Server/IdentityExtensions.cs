// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.IdentityExtensions
// Assembly: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 002C83BC-B53E-470A-8038-76E47B5E5BF3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server
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
      return new List<string>() { key }.QueryIdentities(requestContext)[key];
    }

    public static IDictionary<string, IdentityRef> QueryIdentities(
      this IList<string> identityIds,
      IVssRequestContext requestContext)
    {
      Dictionary<string, IdentityRef> dictionary = new Dictionary<string, IdentityRef>();
      IdentityService service = requestContext.GetService<IdentityService>();
      IEnumerable<Guid> guids = identityIds.Select<string, Guid>((Func<string, Guid>) (x => Guid.Parse(x))).Distinct<Guid>();
      IVssRequestContext requestContext1 = requestContext;
      IEnumerable<Guid> identityIds1 = guids;
      IList<Microsoft.VisualStudio.Services.Identity.Identity> identities = service.GetIdentities(requestContext1, identityIds1);
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
          Microsoft.VisualStudio.Services.Identity.Identity identity1 = identities.FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (identity => identity != null && identity.Id == Guid.Parse(identityId)));
          if (identity1 == null)
            dictionary[identityId] = new IdentityRef()
            {
              Id = identityId
            };
          else
            dictionary[identityId] = identity1.ToIdentityRef(requestContext);
        }
      }
      return (IDictionary<string, IdentityRef>) dictionary;
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
