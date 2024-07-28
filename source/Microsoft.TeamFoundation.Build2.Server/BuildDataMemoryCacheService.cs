// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.BuildDataMemoryCacheService
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Build2.Server
{
  public class BuildDataMemoryCacheService : VssMemoryCacheService<int, BuildData>
  {
    private static readonly MemoryCacheConfiguration<int, BuildData> s_configuration = new MemoryCacheConfiguration<int, BuildData>().WithExpiryInterval(TimeSpan.FromSeconds(15.0)).WithInactivityInterval(TimeSpan.FromSeconds(15.0)).WithMaxElements(10).WithMaxSize(51200L, (ISizeProvider<int, BuildData>) new BuildDataMemoryCacheService.BuildDataCacheSizeProvider());

    public BuildDataMemoryCacheService()
      : base((IEqualityComparer<int>) EqualityComparer<int>.Default, BuildDataMemoryCacheService.s_configuration)
    {
    }

    public override bool TryGetValue(
      IVssRequestContext requestContext,
      int key,
      out BuildData value)
    {
      int num = base.TryGetValue(requestContext, key, out value) ? 1 : 0;
      if (num == 0)
        return num != 0;
      value = value.Clone();
      return num != 0;
    }

    public override void Set(IVssRequestContext requestContext, int key, BuildData value)
    {
      value = value.Clone();
      base.Set(requestContext, key, value);
    }

    private sealed class BuildDataCacheSizeProvider : ISizeProvider<int, BuildData>
    {
      public long GetSize(int key, BuildData buildData) => 5120;
    }
  }
}
