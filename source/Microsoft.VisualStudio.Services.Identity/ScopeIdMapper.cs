// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.ScopeIdMapper
// Assembly: Microsoft.VisualStudio.Services.Identity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1372DA81-8681-4BFE-8E91-D1AB4333F834
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Identity.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Concurrent;

namespace Microsoft.VisualStudio.Services.Identity
{
  internal class ScopeIdMapper : IScopeMapper
  {
    protected readonly ConcurrentDictionary<Guid, Guid> m_localToMasterIdMap;

    internal ScopeIdMapper(IVssRequestContext requestContext) => this.m_localToMasterIdMap = new ConcurrentDictionary<Guid, Guid>();

    public Guid MapFromLocalId(
      IVssRequestContext requestContext,
      Guid securingHostId,
      Guid localId)
    {
      Guid guid1;
      if (!this.m_localToMasterIdMap.TryGetValue(localId, out guid1))
      {
        using (GroupComponent groupComponent = PlatformIdentityStore.CreateGroupComponent(requestContext))
        {
          Guid guid2 = groupComponent.ReadMasterScope(securingHostId, new Guid[1]
          {
            localId
          })[0];
          guid1 = !(guid2 != Guid.Empty) ? localId : this.m_localToMasterIdMap.GetOrAdd(localId, guid2);
        }
      }
      return guid1;
    }

    public void ClearCache() => this.m_localToMasterIdMap.Clear();
  }
}
