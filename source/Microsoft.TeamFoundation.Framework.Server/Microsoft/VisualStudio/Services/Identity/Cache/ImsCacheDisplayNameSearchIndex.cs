// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.Cache.ImsCacheDisplayNameSearchIndex
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Identity.Cache
{
  internal class ImsCacheDisplayNameSearchIndex : 
    ImsCacheObject<Guid, IList<ImsCacheIdentityIdByDisplayName>>
  {
    internal ImsCacheDisplayNameSearchIndex()
    {
    }

    internal ImsCacheDisplayNameSearchIndex(
      ImsCacheIdKey key,
      IList<ImsCacheIdentityIdByDisplayName> value)
      : base((ImsCacheKey<Guid>) key, value)
    {
    }

    internal ImsCacheDisplayNameSearchIndex(
      ImsCacheIdKey key,
      IList<ImsCacheIdentityIdByDisplayName> value,
      DateTimeOffset time)
      : base((ImsCacheKey<Guid>) key, value, time)
    {
    }

    public override object Clone()
    {
      ImsCacheIdKey key = (ImsCacheIdKey) this.Key?.Clone();
      IList<ImsCacheIdentityIdByDisplayName> source = this.Value;
      List<ImsCacheIdentityIdByDisplayName> list = source != null ? source.Select<ImsCacheIdentityIdByDisplayName, ImsCacheIdentityIdByDisplayName>((Func<ImsCacheIdentityIdByDisplayName, ImsCacheIdentityIdByDisplayName>) (x => (ImsCacheIdentityIdByDisplayName) x?.Clone())).ToList<ImsCacheIdentityIdByDisplayName>() : (List<ImsCacheIdentityIdByDisplayName>) null;
      DateTimeOffset time = this.Time;
      return (object) new ImsCacheDisplayNameSearchIndex(key, (IList<ImsCacheIdentityIdByDisplayName>) list, time);
    }
  }
}
