// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.Comments.Web.Common.IdentityRefBuilder
// Assembly: Microsoft.Azure.DevOps.Comments.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6538262-E3F2-45F5-B799-587642D68EAC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DevOps.Comments.Web.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Azure.DevOps.Comments.Web.Common
{
  public static class IdentityRefBuilder
  {
    public static IDictionary<Guid, IdentityRef> Create(
      IVssRequestContext requestContext,
      IEnumerable<Guid> vsids,
      bool includeUrls = false,
      bool includeInactive = false)
    {
      List<Guid> list = vsids.Where<Guid>((Func<Guid, bool>) (x => x != Guid.Empty)).Distinct<Guid>().ToList<Guid>();
      IList<Microsoft.VisualStudio.Services.Identity.Identity> identityList = requestContext.GetService<IdentityService>().ReadIdentities(requestContext, (IList<Guid>) list, QueryMembership.None, (IEnumerable<string>) null);
      Dictionary<Guid, IdentityRef> dictionary = new Dictionary<Guid, IdentityRef>();
      for (int index = 0; index < list.Count; ++index)
      {
        if (identityList[index] != null)
        {
          IdentityRef identityRef = identityList[index].ToIdentityRef(requestContext, includeUrls, includeInactive);
          dictionary.Add(list[index], identityRef);
        }
      }
      return (IDictionary<Guid, IdentityRef>) dictionary;
    }
  }
}
