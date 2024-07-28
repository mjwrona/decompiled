// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Aad.Cache.AadCacheAncestorIds
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Aad.Cache
{
  internal class AadCacheAncestorIds : AadCacheObject<ISet<Guid>>
  {
    internal AadCacheAncestorIds()
    {
    }

    internal AadCacheAncestorIds(AadCacheKey key, ISet<Guid> value)
      : base(key, value)
    {
    }

    internal AadCacheAncestorIds(AadCacheKey key, ISet<Guid> value, DateTimeOffset time)
      : base(key, value, time)
    {
    }

    internal override AadCacheObject WithTime(DateTimeOffset time) => (AadCacheObject) new AadCacheAncestorIds(this.Key, this.Value, time);

    protected override bool ValueEquals(object otherValue)
    {
      if (this.Value == otherValue)
        return true;
      return otherValue is ISet<Guid> other && this.Value != null && this.Value.SetEquals((IEnumerable<Guid>) other);
    }
  }
}
