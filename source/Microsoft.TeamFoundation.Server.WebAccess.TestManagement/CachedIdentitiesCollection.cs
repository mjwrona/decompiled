// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.CachedIdentitiesCollection
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.TestManagement, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2E4165D5-898A-42D9-B816-9FABF135E4DA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.TestManagement.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.TestManagement
{
  public class CachedIdentitiesCollection
  {
    private Dictionary<Guid, TeamFoundationIdentity> m_cachedIdentities = new Dictionary<Guid, TeamFoundationIdentity>();
    private Dictionary<string, TeamFoundationIdentity> m_cachedIdentitiesByName = new Dictionary<string, TeamFoundationIdentity>((IEqualityComparer<string>) StringComparer.CurrentCultureIgnoreCase);
    private TeamFoundationIdentityService m_identityService;

    public CachedIdentitiesCollection(TestManagerRequestContext testContext) => this.TestContext = testContext;

    public TestManagerRequestContext TestContext { get; private set; }

    public TeamFoundationIdentityService IdentityService
    {
      get
      {
        if (this.m_identityService == null)
          this.m_identityService = this.TestContext.TfsRequestContext.GetService<TeamFoundationIdentityService>();
        return this.m_identityService;
      }
    }

    public string GetUserDisplayName(Guid teamFoundationUserId) => this.GetIdentity(teamFoundationUserId)?.DisplayName;

    public TeamFoundationIdentity GetIdentity(Guid teamFoundationUserId) => this.GetIdentities((IEnumerable<Guid>) new Guid[1]
    {
      teamFoundationUserId
    }).FirstOrDefault<TeamFoundationIdentity>();

    public List<TeamFoundationIdentity> GetIdentities(IEnumerable<Guid> teamFoundationUserIds)
    {
      this.EnsureCachedIdentities(teamFoundationUserIds);
      List<TeamFoundationIdentity> identities = new List<TeamFoundationIdentity>();
      foreach (Guid foundationUserId in teamFoundationUserIds)
      {
        TeamFoundationIdentity foundationIdentity;
        if (this.m_cachedIdentities.TryGetValue(foundationUserId, out foundationIdentity))
          identities.Add(foundationIdentity);
      }
      return identities;
    }

    public void EnsureCachedIdentities(IEnumerable<Guid> teamFoundationIds)
    {
      IEnumerable<Guid> source = teamFoundationIds.Where<Guid>((Func<Guid, bool>) (id => id != Guid.Empty && !this.m_cachedIdentities.ContainsKey(id)));
      if (!source.Any<Guid>())
        return;
      this.CacheIdentities(source.ToArray<Guid>());
    }

    public TeamFoundationIdentity GetUserByDisplayName(string displayName)
    {
      TeamFoundationIdentity userByDisplayName = (TeamFoundationIdentity) null;
      if (this.m_cachedIdentitiesByName.TryGetValue(displayName, out userByDisplayName))
        return userByDisplayName;
      TeamFoundationIdentity[][] foundationIdentityArray = this.IdentityService.ReadIdentities(this.TestContext.TfsRequestContext, IdentitySearchFactor.DisplayName, new string[1]
      {
        displayName
      });
      if (foundationIdentityArray[0].Length != 0)
        userByDisplayName = foundationIdentityArray[0][0];
      this.m_cachedIdentitiesByName[displayName] = userByDisplayName;
      return userByDisplayName;
    }

    private TeamFoundationIdentity[] CacheIdentities(Guid[] teamFoundationUserIds)
    {
      TeamFoundationIdentity[] foundationIdentityArray = this.IdentityService.ReadIdentities(this.TestContext.TfsRequestContext, teamFoundationUserIds);
      for (int index = 0; index < teamFoundationUserIds.Length; ++index)
      {
        this.m_cachedIdentities[teamFoundationUserIds[index]] = foundationIdentityArray[index];
        if (foundationIdentityArray[index] != null)
          this.m_cachedIdentitiesByName[foundationIdentityArray[index].DisplayName] = foundationIdentityArray[index];
      }
      return foundationIdentityArray;
    }
  }
}
