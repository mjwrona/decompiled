// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.Cache.ImsCacheScopeMembership
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;

namespace Microsoft.VisualStudio.Services.Identity.Cache
{
  internal class ImsCacheScopeMembership : ImsCacheObject<ScopedId, bool>
  {
    internal ImsCacheScopeMembership()
    {
    }

    internal ImsCacheScopeMembership(ImsCacheScopedIdKey key, bool value)
      : base((ImsCacheKey<ScopedId>) key, value)
    {
    }

    internal ImsCacheScopeMembership(ImsCacheScopedIdKey key, bool value, DateTimeOffset time)
      : base((ImsCacheKey<ScopedId>) key, value, time)
    {
    }

    public override object Clone() => (object) new ImsCacheScopeMembership((ImsCacheScopedIdKey) this.Key?.Clone(), this.Value, this.Time);
  }
}
