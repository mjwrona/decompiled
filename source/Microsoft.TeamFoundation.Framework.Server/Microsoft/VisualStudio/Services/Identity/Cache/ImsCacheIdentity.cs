// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.Cache.ImsCacheIdentity
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;

namespace Microsoft.VisualStudio.Services.Identity.Cache
{
  internal class ImsCacheIdentity : ImsCacheObject<Guid, Microsoft.VisualStudio.Services.Identity.Identity>
  {
    internal ImsCacheIdentity()
    {
    }

    internal ImsCacheIdentity(ImsCacheIdKey key, Microsoft.VisualStudio.Services.Identity.Identity value)
      : base((ImsCacheKey<Guid>) key, value)
    {
    }

    internal ImsCacheIdentity(ImsCacheIdKey key, Microsoft.VisualStudio.Services.Identity.Identity value, DateTimeOffset time)
      : base((ImsCacheKey<Guid>) key, value, time)
    {
    }

    public override object Clone() => (object) new ImsCacheIdentity((ImsCacheIdKey) this.Key?.Clone(), this.Value?.Clone(), this.Time);
  }
}
