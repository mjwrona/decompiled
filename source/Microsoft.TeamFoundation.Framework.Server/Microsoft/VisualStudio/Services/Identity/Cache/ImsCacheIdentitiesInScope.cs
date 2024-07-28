// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.Cache.ImsCacheIdentitiesInScope
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Identity.Cache
{
  internal class ImsCacheIdentitiesInScope : ImsCacheObject<Guid, IList<Microsoft.VisualStudio.Services.Identity.Identity>>
  {
    internal ImsCacheIdentitiesInScope()
    {
    }

    internal ImsCacheIdentitiesInScope(ImsCacheIdKey key, IList<Microsoft.VisualStudio.Services.Identity.Identity> value)
      : base((ImsCacheKey<Guid>) key, value)
    {
    }

    internal ImsCacheIdentitiesInScope(
      ImsCacheIdKey key,
      IList<Microsoft.VisualStudio.Services.Identity.Identity> value,
      DateTimeOffset time)
      : base((ImsCacheKey<Guid>) key, value, time)
    {
    }

    public override object Clone()
    {
      ImsCacheIdKey key = (ImsCacheIdKey) this.Key?.Clone();
      IList<Microsoft.VisualStudio.Services.Identity.Identity> source = this.Value;
      List<Microsoft.VisualStudio.Services.Identity.Identity> list = source != null ? source.Select<Microsoft.VisualStudio.Services.Identity.Identity, Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, Microsoft.VisualStudio.Services.Identity.Identity>) (x => x?.Clone())).ToList<Microsoft.VisualStudio.Services.Identity.Identity>() : (List<Microsoft.VisualStudio.Services.Identity.Identity>) null;
      DateTimeOffset time = this.Time;
      return (object) new ImsCacheIdentitiesInScope(key, (IList<Microsoft.VisualStudio.Services.Identity.Identity>) list, time);
    }
  }
}
