// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.Cache.ImsCacheDescendants
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Identity.Cache
{
  internal class ImsCacheDescendants : ImsCacheObject<Guid, ISet<IdentityId>>
  {
    internal ImsCacheDescendants()
    {
    }

    internal ImsCacheDescendants(ImsCacheIdKey key, ISet<IdentityId> value)
      : base((ImsCacheKey<Guid>) key, value)
    {
    }

    internal ImsCacheDescendants(ImsCacheIdKey key, ISet<IdentityId> value, DateTimeOffset time)
      : base((ImsCacheKey<Guid>) key, value, time)
    {
    }

    public override object Clone() => (object) new ImsCacheDescendants((ImsCacheIdKey) this.Key?.Clone(), this.Value == null ? (ISet<IdentityId>) null : (ISet<IdentityId>) new HashSet<IdentityId>(this.Value.Select<IdentityId, IdentityId>((Func<IdentityId, IdentityId>) (x => (IdentityId) x?.Clone()))), this.Time);

    protected override bool ValueEquals(object otherValue)
    {
      if (this.Value == otherValue)
        return true;
      return otherValue is ISet<IdentityId> other && this.Value.SetEquals((IEnumerable<IdentityId>) other);
    }
  }
}
