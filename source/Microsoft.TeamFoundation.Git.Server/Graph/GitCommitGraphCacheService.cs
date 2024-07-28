// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.Graph.GitCommitGraphCacheService
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Git.Server.Graph
{
  internal sealed class GitCommitGraphCacheService : 
    VssMemoryCacheService<CacheKeys.CrossHostOdbId, GitCommitGraph>
  {
    private const double c_defaultMaxSizeFraction = 0.015;

    public GitCommitGraphCacheService()
      : base((IEqualityComparer<CacheKeys.CrossHostOdbId>) EqualityComparer<CacheKeys.CrossHostOdbId>.Default, new MemoryCacheConfiguration<CacheKeys.CrossHostOdbId, GitCommitGraph>().WithMaxSize(CacheUtil.GetMemoryFraction(0.015), (ISizeProvider<CacheKeys.CrossHostOdbId, GitCommitGraph>) GitCommitGraphCacheService.GitCommitGraphSizeProvider.Instance))
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
      this.MaxCacheSize.Value = CacheUtil.GetMemoryFractionFromRegistry(ce, this.RegistryPath, 0.015);
    }

    internal long TEST_MaxCacheSize => this.MaxCacheSize.Value;

    private class GitCommitGraphSizeProvider : 
      ISizeProvider<CacheKeys.CrossHostOdbId, GitCommitGraph>
    {
      internal static readonly GitCommitGraphCacheService.GitCommitGraphSizeProvider Instance = new GitCommitGraphCacheService.GitCommitGraphSizeProvider();

      private GitCommitGraphSizeProvider()
      {
      }

      public long GetSize(CacheKeys.CrossHostOdbId key, GitCommitGraph graph) => 32L + graph.GetSize();
    }
  }
}
