// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.Cache.ImsCacheMruIdentityIds
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Identity.Cache
{
  internal class ImsCacheMruIdentityIds : ImsCacheObject<ScopedId, IList<Guid>>
  {
    internal ImsCacheMruIdentityIds()
    {
    }

    internal ImsCacheMruIdentityIds(ImsCacheScopedIdKey key, IList<Guid> value)
      : base((ImsCacheKey<ScopedId>) key, value)
    {
    }

    internal ImsCacheMruIdentityIds(
      ImsCacheScopedIdKey key,
      IList<Guid> value,
      DateTimeOffset time)
      : base((ImsCacheKey<ScopedId>) key, value, time)
    {
    }

    public override object Clone() => (object) new ImsCacheMruIdentityIds((ImsCacheScopedIdKey) this.Key?.Clone(), this.Value == null ? (IList<Guid>) null : (IList<Guid>) new List<Guid>((IEnumerable<Guid>) this.Value), this.Time);

    protected override bool ValueEquals(object otherValue)
    {
      if (this.Value == otherValue)
        return true;
      return otherValue is IList<Guid> second && this.Value.SequenceEqual<Guid>((IEnumerable<Guid>) second);
    }
  }
}
