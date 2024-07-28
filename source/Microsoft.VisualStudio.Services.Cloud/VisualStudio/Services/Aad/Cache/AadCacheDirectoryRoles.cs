// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Aad.Cache.AadCacheDirectoryRoles
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Aad.Cache
{
  internal class AadCacheDirectoryRoles : AadCacheObject<ISet<AadDirectoryRole>>
  {
    internal AadCacheDirectoryRoles()
    {
    }

    internal AadCacheDirectoryRoles(AadCacheKey key, ISet<AadDirectoryRole> value)
      : base(key, value)
    {
    }

    internal AadCacheDirectoryRoles(
      AadCacheKey key,
      ISet<AadDirectoryRole> value,
      DateTimeOffset time)
      : base(key, value, time)
    {
    }

    internal override AadCacheObject WithTime(DateTimeOffset time) => (AadCacheObject) new AadCacheDirectoryRoles(this.Key, this.Value, time);
  }
}
