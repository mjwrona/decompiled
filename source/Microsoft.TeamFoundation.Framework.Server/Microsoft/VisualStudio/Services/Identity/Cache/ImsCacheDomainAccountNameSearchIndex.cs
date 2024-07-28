// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.Cache.ImsCacheDomainAccountNameSearchIndex
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Identity.Cache
{
  internal class ImsCacheDomainAccountNameSearchIndex : 
    ImsCacheObject<Guid, IList<ImsCacheIdentityIdByDomainAccountName>>
  {
    internal ImsCacheDomainAccountNameSearchIndex()
    {
    }

    internal ImsCacheDomainAccountNameSearchIndex(
      ImsCacheIdKey key,
      IList<ImsCacheIdentityIdByDomainAccountName> value)
      : base((ImsCacheKey<Guid>) key, value)
    {
    }

    internal ImsCacheDomainAccountNameSearchIndex(
      ImsCacheIdKey key,
      IList<ImsCacheIdentityIdByDomainAccountName> value,
      DateTimeOffset time)
      : base((ImsCacheKey<Guid>) key, value, time)
    {
    }

    public override object Clone()
    {
      ImsCacheIdKey key = (ImsCacheIdKey) this.Key?.Clone();
      IList<ImsCacheIdentityIdByDomainAccountName> source = this.Value;
      List<ImsCacheIdentityIdByDomainAccountName> list = source != null ? source.Select<ImsCacheIdentityIdByDomainAccountName, ImsCacheIdentityIdByDomainAccountName>((Func<ImsCacheIdentityIdByDomainAccountName, ImsCacheIdentityIdByDomainAccountName>) (x => (ImsCacheIdentityIdByDomainAccountName) x?.Clone())).ToList<ImsCacheIdentityIdByDomainAccountName>() : (List<ImsCacheIdentityIdByDomainAccountName>) null;
      DateTimeOffset time = this.Time;
      return (object) new ImsCacheDomainAccountNameSearchIndex(key, (IList<ImsCacheIdentityIdByDomainAccountName>) list, time);
    }
  }
}
