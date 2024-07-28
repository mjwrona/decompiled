// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Policy.Server.PolicyConfigurationBasicCacheService
// Assembly: Microsoft.TeamFoundation.Policy.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C7B03386-B27B-4823-BB4F-89F7D7E42DDD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Policy.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Policy.Server
{
  public class PolicyConfigurationBasicCacheService : 
    VssMemoryCacheService<int, IList<PolicyConfigurationRecord>>,
    IPolicyConfigurationBasicCacheService,
    IVssFrameworkService
  {
    private static readonly MemoryCacheConfiguration<int, IList<PolicyConfigurationRecord>> s_configuration = new MemoryCacheConfiguration<int, IList<PolicyConfigurationRecord>>().WithExpiryInterval(TimeSpan.FromHours(1.0)).WithCleanupInterval(TimeSpan.FromHours(2.0)).WithMaxElements(40);

    public PolicyConfigurationBasicCacheService()
      : base((IEqualityComparer<int>) EqualityComparer<int>.Default, PolicyConfigurationBasicCacheService.s_configuration)
    {
    }

    VssMemoryCacheList<int, IList<PolicyConfigurationRecord>> IPolicyConfigurationBasicCacheService.get_MemoryCache() => this.MemoryCache;
  }
}
