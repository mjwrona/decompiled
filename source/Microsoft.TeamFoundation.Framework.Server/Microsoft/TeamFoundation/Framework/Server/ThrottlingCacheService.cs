// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ThrottlingCacheService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class ThrottlingCacheService : VssMemoryCacheService<string, DateTime>
  {
    private static readonly TimeSpan s_defaultCleanupInterval = TimeSpan.FromMinutes(5.0);
    private const int s_defaultMaxLength = 100000;

    public ThrottlingCacheService()
      : base((IEqualityComparer<string>) EqualityComparer<string>.Default, new MemoryCacheConfiguration<string, DateTime>().WithCleanupInterval(ThrottlingCacheService.s_defaultCleanupInterval).WithMaxElements(100000))
    {
    }

    public override void Set(IVssRequestContext requestContext, string key, DateTime value)
    {
      Capture<TimeSpan> capture = Capture.Create<TimeSpan>(value.Subtract(DateTime.UtcNow));
      this.MemoryCache.Add(key, value, true, new VssCacheExpiryProvider<string, DateTime>(capture, capture));
    }
  }
}
