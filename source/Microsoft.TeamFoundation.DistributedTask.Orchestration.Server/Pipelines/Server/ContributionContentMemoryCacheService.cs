// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Server.ContributionContentMemoryCacheService
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Server
{
  internal class ContributionContentMemoryCacheService : VssMemoryCacheService<string, string>
  {
    private static readonly MemoryCacheConfiguration<string, string> s_configuration = new MemoryCacheConfiguration<string, string>().WithExpiryInterval(TimeSpan.FromMinutes(180.0)).WithInactivityInterval(TimeSpan.FromMinutes(30.0)).WithMaxElements(25).WithMaxSize(1048576L, (ISizeProvider<string, string>) new ContributionContentMemoryCacheService.ContributionContentCacheServiceSizeProvider());

    public ContributionContentMemoryCacheService()
      : base((IEqualityComparer<string>) StringComparer.Ordinal, ContributionContentMemoryCacheService.s_configuration)
    {
    }

    public override string Name => "DistributedTaskContributionContentMemoryCacheService";

    private sealed class ContributionContentCacheServiceSizeProvider : ISizeProvider<string, string>
    {
      public long GetSize(string key, string value) => (long) (key.Length + value.Length);
    }
  }
}
