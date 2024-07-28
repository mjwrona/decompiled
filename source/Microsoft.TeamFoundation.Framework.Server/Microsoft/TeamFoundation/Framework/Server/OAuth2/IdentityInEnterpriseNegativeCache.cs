// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.OAuth2.IdentityInEnterpriseNegativeCache
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Framework.Server.OAuth2
{
  public class IdentityInEnterpriseNegativeCache : VssMemoryCacheService<Guid, object>
  {
    public IdentityInEnterpriseNegativeCache()
      : base((IEqualityComparer<Guid>) EqualityComparer<Guid>.Default, IdentityInEnterpriseNegativeCache.GetConfiguration())
    {
    }

    public virtual void SetIdentityAbsent(IVssRequestContext requestContext, Guid vsid) => this.Set(requestContext, vsid, (object) null);

    public virtual bool IsIdentityAbsent(IVssRequestContext requestContext, Guid vsid) => this.TryGetValue(requestContext, vsid, out object _);

    private static MemoryCacheConfiguration<Guid, object> GetConfiguration() => new MemoryCacheConfiguration<Guid, object>().WithCleanupInterval(TimeSpan.FromHours(1.0)).WithExpiryInterval(TimeSpan.FromMinutes(30.0));
  }
}
