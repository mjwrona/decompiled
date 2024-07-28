// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.Cache.ImsCacheAccountNameSearchIndex
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Identity.Cache
{
  internal class ImsCacheAccountNameSearchIndex : 
    ImsCacheObject<Guid, IList<ImsCacheIdentityIdByAccountName>>
  {
    internal ImsCacheAccountNameSearchIndex()
    {
    }

    internal ImsCacheAccountNameSearchIndex(
      ImsCacheIdKey key,
      IList<ImsCacheIdentityIdByAccountName> value)
      : base((ImsCacheKey<Guid>) key, value)
    {
    }

    internal ImsCacheAccountNameSearchIndex(
      ImsCacheIdKey key,
      IList<ImsCacheIdentityIdByAccountName> value,
      DateTimeOffset time)
      : base((ImsCacheKey<Guid>) key, value, time)
    {
    }

    public override object Clone()
    {
      ImsCacheIdKey key = (ImsCacheIdKey) this.Key?.Clone();
      IList<ImsCacheIdentityIdByAccountName> source = this.Value;
      List<ImsCacheIdentityIdByAccountName> list = source != null ? source.Select<ImsCacheIdentityIdByAccountName, ImsCacheIdentityIdByAccountName>((Func<ImsCacheIdentityIdByAccountName, ImsCacheIdentityIdByAccountName>) (x => (ImsCacheIdentityIdByAccountName) x?.Clone())).ToList<ImsCacheIdentityIdByAccountName>() : (List<ImsCacheIdentityIdByAccountName>) null;
      DateTimeOffset time = this.Time;
      return (object) new ImsCacheAccountNameSearchIndex(key, (IList<ImsCacheIdentityIdByAccountName>) list, time);
    }
  }
}
