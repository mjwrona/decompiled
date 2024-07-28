// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.Cache.ImsCacheIdentityIdByAccountName
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;

namespace Microsoft.VisualStudio.Services.Identity.Cache
{
  internal class ImsCacheIdentityIdByAccountName : ImsCacheObject<string, IdentityId>
  {
    internal ImsCacheIdentityIdByAccountName()
    {
    }

    internal ImsCacheIdentityIdByAccountName(ImsCacheStringKey key, IdentityId value)
      : base((ImsCacheKey<string>) key, value)
    {
    }

    internal ImsCacheIdentityIdByAccountName(
      ImsCacheStringKey key,
      IdentityId value,
      DateTimeOffset time)
      : base((ImsCacheKey<string>) key, value, time)
    {
    }

    public override object Clone() => (object) new ImsCacheIdentityIdByAccountName((ImsCacheStringKey) this.Key?.Clone(), (IdentityId) this.Value?.Clone(), this.Time);
  }
}
