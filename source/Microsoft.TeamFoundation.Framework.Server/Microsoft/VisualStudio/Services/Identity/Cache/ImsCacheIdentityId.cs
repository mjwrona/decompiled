// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.Cache.ImsCacheIdentityId
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;

namespace Microsoft.VisualStudio.Services.Identity.Cache
{
  internal class ImsCacheIdentityId : ImsCacheObject<IdentityDescriptor, IdentityId>
  {
    internal ImsCacheIdentityId()
    {
    }

    internal ImsCacheIdentityId(ImsCacheDescriptorKey key, IdentityId value)
      : base((ImsCacheKey<IdentityDescriptor>) key, value)
    {
    }

    internal ImsCacheIdentityId(ImsCacheDescriptorKey key, IdentityId value, DateTimeOffset time)
      : base((ImsCacheKey<IdentityDescriptor>) key, value, time)
    {
    }

    public override object Clone() => (object) new ImsCacheIdentityId((ImsCacheDescriptorKey) this.Key?.Clone(), (IdentityId) this.Value?.Clone(), this.Time);
  }
}
