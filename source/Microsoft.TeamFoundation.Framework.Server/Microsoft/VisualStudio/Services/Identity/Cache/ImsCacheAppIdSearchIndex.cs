// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.Cache.ImsCacheAppIdSearchIndex
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Identity.Cache
{
  internal class ImsCacheAppIdSearchIndex : ImsCacheObject<Guid, IList<ImsCacheIdentityIdByAppId>>
  {
    internal ImsCacheAppIdSearchIndex()
    {
    }

    internal ImsCacheAppIdSearchIndex(ImsCacheIdKey key, IList<ImsCacheIdentityIdByAppId> value)
      : base((ImsCacheKey<Guid>) key, value)
    {
    }

    internal ImsCacheAppIdSearchIndex(
      ImsCacheIdKey key,
      IList<ImsCacheIdentityIdByAppId> value,
      DateTimeOffset time)
      : base((ImsCacheKey<Guid>) key, value, time)
    {
    }

    public override object Clone()
    {
      ImsCacheIdKey key = (ImsCacheIdKey) this.Key?.Clone();
      IList<ImsCacheIdentityIdByAppId> source = this.Value;
      List<ImsCacheIdentityIdByAppId> list = source != null ? source.Select<ImsCacheIdentityIdByAppId, ImsCacheIdentityIdByAppId>((Func<ImsCacheIdentityIdByAppId, ImsCacheIdentityIdByAppId>) (x => (ImsCacheIdentityIdByAppId) x?.Clone())).ToList<ImsCacheIdentityIdByAppId>() : (List<ImsCacheIdentityIdByAppId>) null;
      DateTimeOffset time = this.Time;
      return (object) new ImsCacheAppIdSearchIndex(key, (IList<ImsCacheIdentityIdByAppId>) list, time);
    }
  }
}
