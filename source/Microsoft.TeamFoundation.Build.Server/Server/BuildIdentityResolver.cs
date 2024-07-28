// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.BuildIdentityResolver
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Build.Server
{
  internal class BuildIdentityResolver
  {
    private HashSet<Guid> m_imsCacheMisses = new HashSet<Guid>();
    private Dictionary<Guid, TeamFoundationIdentity> m_imsIdentities = new Dictionary<Guid, TeamFoundationIdentity>();

    public string GetUniqueName(IVssRequestContext requestContext, string identityValue)
    {
      TeamFoundationIdentityService service = requestContext.GetService<TeamFoundationIdentityService>();
      return this.GetUniqueName(requestContext, service, identityValue, out string _, out TeamFoundationIdentity _);
    }

    public string GetUniqueName(
      IVssRequestContext requestContext,
      TeamFoundationIdentityService identityService,
      string identityValue,
      out string displayName,
      out TeamFoundationIdentity identity)
    {
      identity = (TeamFoundationIdentity) null;
      displayName = identityValue;
      Guid result;
      if (Guid.TryParseExact(identityValue, "B", out result) && !this.m_imsCacheMisses.Contains(result))
      {
        if (!this.m_imsIdentities.TryGetValue(result, out identity))
        {
          TeamFoundationIdentity[] foundationIdentityArray = identityService.ReadIdentities(requestContext, new Guid[1]
          {
            result
          }, MembershipQuery.None, ReadIdentityOptions.None, (IEnumerable<string>) null);
          if (foundationIdentityArray.Length == 1 && foundationIdentityArray[0] != null)
          {
            identity = foundationIdentityArray[0];
            this.m_imsIdentities[result] = identity;
          }
          else
            this.m_imsCacheMisses.Add(result);
        }
        if (identity != null)
        {
          displayName = identity.DisplayName;
          identityValue = identity.UniqueName;
        }
      }
      return identityValue;
    }
  }
}
