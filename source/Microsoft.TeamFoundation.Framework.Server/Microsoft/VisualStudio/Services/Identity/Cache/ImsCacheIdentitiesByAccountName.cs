// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.Cache.ImsCacheIdentitiesByAccountName
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Identity.Cache
{
  internal class ImsCacheIdentitiesByAccountName : ImsCacheObject<ScopedKey, ISet<Guid>>
  {
    internal ImsCacheIdentitiesByAccountName()
    {
    }

    internal ImsCacheIdentitiesByAccountName(ImsCacheScopedNameKey key, ISet<Guid> value)
      : base((ImsCacheKey<ScopedKey>) key, value)
    {
    }

    internal ImsCacheIdentitiesByAccountName(
      ImsCacheScopedNameKey key,
      ISet<Guid> value,
      DateTimeOffset time)
      : base((ImsCacheKey<ScopedKey>) key, value, time)
    {
    }

    public override object Clone() => (object) new ImsCacheIdentitiesByAccountName((ImsCacheScopedNameKey) this.Key?.Clone(), this.Value == null ? (ISet<Guid>) null : (ISet<Guid>) new HashSet<Guid>((IEnumerable<Guid>) this.Value), this.Time);

    protected override bool ValueEquals(object otherValue)
    {
      if (this.Value == otherValue)
        return true;
      return otherValue is ISet<Guid> other && this.Value.SetEquals((IEnumerable<Guid>) other);
    }
  }
}
