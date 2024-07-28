// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.Cache.ImsCacheEmailSearchIndex
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Identity.Cache
{
  internal class ImsCacheEmailSearchIndex : ImsCacheObject<Guid, IList<ImsCacheIdentityIdByEmail>>
  {
    internal ImsCacheEmailSearchIndex()
    {
    }

    internal ImsCacheEmailSearchIndex(ImsCacheIdKey key, IList<ImsCacheIdentityIdByEmail> value)
      : base((ImsCacheKey<Guid>) key, value)
    {
    }

    internal ImsCacheEmailSearchIndex(
      ImsCacheIdKey key,
      IList<ImsCacheIdentityIdByEmail> value,
      DateTimeOffset time)
      : base((ImsCacheKey<Guid>) key, value, time)
    {
    }

    public override object Clone()
    {
      ImsCacheIdKey key = (ImsCacheIdKey) this.Key?.Clone();
      IList<ImsCacheIdentityIdByEmail> source = this.Value;
      List<ImsCacheIdentityIdByEmail> list = source != null ? source.Select<ImsCacheIdentityIdByEmail, ImsCacheIdentityIdByEmail>((Func<ImsCacheIdentityIdByEmail, ImsCacheIdentityIdByEmail>) (x => (ImsCacheIdentityIdByEmail) x?.Clone())).ToList<ImsCacheIdentityIdByEmail>() : (List<ImsCacheIdentityIdByEmail>) null;
      DateTimeOffset time = this.Time;
      return (object) new ImsCacheEmailSearchIndex(key, (IList<ImsCacheIdentityIdByEmail>) list, time);
    }
  }
}
