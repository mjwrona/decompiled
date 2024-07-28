// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.GitObjectCoreCacheService
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server.TfsGitObjects;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Git.Server
{
  internal sealed class GitObjectCoreCacheService : 
    VssMemoryCacheService<CacheKeys.CrossHostOdbScopedObjectId, IGitObjectCore>
  {
    private int MaxItemBytes;
    private const int c_defaultMaxItemBytes = 262144;
    private const double c_defaultMaxSizeFraction = 0.03;
    private static readonly TimeSpan s_defaultInactivityInterval = TimeSpan.FromMinutes(10.0);

    public GitObjectCoreCacheService()
      : base((IEqualityComparer<CacheKeys.CrossHostOdbScopedObjectId>) EqualityComparer<CacheKeys.CrossHostOdbScopedObjectId>.Default, new MemoryCacheConfiguration<CacheKeys.CrossHostOdbScopedObjectId, IGitObjectCore>().WithCleanupInterval(GitObjectCoreCacheService.s_defaultInactivityInterval).WithInactivityInterval(GitObjectCoreCacheService.s_defaultInactivityInterval).WithMaxSize(CacheUtil.GetMemoryFraction(0.03), (ISizeProvider<CacheKeys.CrossHostOdbScopedObjectId, IGitObjectCore>) GitObjectCoreCacheService.SizeProvider.Instance))
    {
    }

    protected override void ServiceStart(IVssRequestContext systemRC)
    {
      systemRC.CheckDeploymentRequestContext();
      base.ServiceStart(systemRC);
    }

    protected override void LoadSettings(
      IVssRequestContext requestContext,
      RegistryEntryCollection ce)
    {
      base.LoadSettings(requestContext, ce);
      this.MaxCacheSize.Value = CacheUtil.GetMemoryFractionFromRegistry(ce, this.RegistryPath, 0.03);
      this.MaxItemBytes = ce.GetValueFromPath<int>(this.RegistryPath + "/MaxItemBytes", 262144);
    }

    internal (TimeSpan inactivityInterval, long maxCacheSize, int maxItemBytes) TEST_Settings => (this.InactivityInterval.Value, this.MaxCacheSize.Value, this.MaxItemBytes);

    public bool TryCache(CacheKeys.CrossHostOdbScopedObjectId key, IGitObjectCore value)
    {
      if (value.EstimatedSize > this.MaxItemBytes)
        return false;
      this.MemoryCache[key] = value;
      return true;
    }

    private class SizeProvider : ISizeProvider<CacheKeys.CrossHostOdbScopedObjectId, IGitObjectCore>
    {
      public static readonly GitObjectCoreCacheService.SizeProvider Instance = new GitObjectCoreCacheService.SizeProvider();

      public long GetSize(CacheKeys.CrossHostOdbScopedObjectId key, IGitObjectCore value) => (long) (52 + value.EstimatedSize);
    }
  }
}
